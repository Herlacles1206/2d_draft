Imports System.Drawing.Imaging
Imports System.IO
Imports System.Runtime.CompilerServices
Imports Emgu.CV
Imports Emgu.CV.Face
Imports Emgu.CV.Features2D
Imports Emgu.CV.Structure
Imports Emgu.CV.Util
Imports System.Drawing
Imports Image = System.Drawing.Image
Imports System.Runtime.Remoting
Imports Emgu.CV.CvEnum
Imports System.Windows.Forms.AxHost
Imports AForge
Imports Point = System.Drawing.Point
Imports DocumentFormat.OpenXml.Vml.Office

''' <summary>
''' This class contains all the functions for image processing.
''' </summary>
Public Module ImageProcess
    ''' <summary>
    ''' Clip the range of input
    ''' </summary>
    ''' <paramname="input">The input of function.</param>
    Public Function ColorRange(ByVal input As Double) As Byte
        input = Math.Min(Math.Max(input, 0), 255)
        Return input
    End Function

    ''' <summary>
    ''' return the value of interpolation function
    ''' </summary>
    ''' <paramname="x">The input of function.</param>
    Public Function Interpolation(ByVal x As Double) As Double
        x = (x - 100) / 100.0
        Dim y = Math.Pow(0.04, x)
        Return y
    End Function
    ''' <summary>
    ''' Adjust the brightness and contrast of image
    ''' </summary>
    ''' <paramname="src">The source image.</param>
    ''' <paramname="brightness">The brightness of dst image.</param>
    ''' <paramname="contrast">The contrast of dst image.</param>
    ''' <paramname="gamma">The gamma for gamma correction.</param>
    Public Function AdjustBrightnessAndContrast(ByVal scr As Image, ByVal brightness As Integer, ByVal contrast As Integer, ByVal gamma As Integer) As Image
        If brightness = 0 AndAlso contrast = 0 AndAlso gamma = 100 Then Return scr
        Dim bmpImage As Bitmap = New Bitmap(scr)
        Dim colorImage As Emgu.CV.Image(Of Bgr, Byte) = bmpImage.ToImage(Of Bgr, Byte)()
        bmpImage.Dispose()

        Dim B = brightness / 255.0
        Dim lC = contrast / 255.0
        Dim K = Math.Tan((45 + 44 * lC) / 180 * Math.PI)
        Dim G = Interpolation(gamma)

        Dim BCtrans = New Byte(255) {}

        For i = 0 To 255
            Dim val1 = (i - 127.5 * (1 - B)) * K + 127.5 * (1 + B)
            Dim val2 = Math.Pow(ColorRange(val1) / 255.0, G) * 255.0
            BCtrans(i) = ColorRange(val2)
        Next

        Dim color_data = colorImage.Data

        For y = 0 To colorImage.Height - 1
            For x = 0 To colorImage.Width - 1
                For c = 0 To 2
                    'brightness and contrast
                    'double pixelValue = (color_data[y, x, c] - 127.5 * (1 - B)) * K + 127.5 * (1 + B);
                    'gamma correction
                    'pixelValue = Math.Pow((ColorRange(pixelValue) / 255.0), G) * 255.0;
                    color_data(y, x, c) = BCtrans(color_data(y, x, c))
                Next

            Next
        Next


        Dim array As Byte() = colorImage.ToJpegData()

        colorImage.Dispose()
        Dim stream As MemoryStream = New MemoryStream(array)
        Dim img = Image.FromStream(stream)

        Return img
    End Function


    ''' <summary>
    ''' detect edge of selected region
    ''' </summary>
    ''' <paramname="src">The source image.</param>
    ''' <paramname="FirstPtOfEdge">The left top cornor of selected region.</param>
    ''' <paramname="SecondPtOfEdge">The right bottom cornor of selected region.</param>
    Public Function Canny(ByVal scr As Image, ByVal FirstPtOfEdge As Point, ByVal SecondPtOfEdge As Point) As CurveObj
        Dim bmpImage As Bitmap = New Bitmap(scr)
        Dim grayImage As Emgu.CV.Image(Of Gray, Byte) = bmpImage.ToImage(Of Gray, Byte)()
        Dim colorImage As Emgu.CV.Image(Of Bgr, Byte) = bmpImage.ToImage(Of Bgr, Byte)()
        bmpImage.Dispose()

        Dim C_Curve As CurveObj() = New CurveObj(1000) {}
        For i = 0 To 1000
            C_Curve(i) = New CurveObj()
        Next
        Dim Width = scr.Width
        Dim Height = scr.Height
        Dim CropWidth = SecondPtOfEdge.X - FirstPtOfEdge.X
        Dim CropHeight = SecondPtOfEdge.Y - FirstPtOfEdge.Y
        Dim Region As Rectangle = New Rectangle(FirstPtOfEdge.X, FirstPtOfEdge.Y, CropWidth, CropHeight)
        grayImage.ROI = Region

        Dim cropped = grayImage.Copy()
        Dim tempimg As Emgu.CV.Image(Of Emgu.CV.Structure.Gray, Byte) = cropped
        CvInvoke.MedianBlur(cropped, cropped, 3)
        'Emgu.CV.CvInvoke.Imshow("original image", cropped)
        CvInvoke.Canny(cropped, tempimg, 100, 200)

        Dim Edge = tempimg.Data

        Dim totalCnt As Integer = 0
        Dim nX2, nY2, nX3, nY3, nStackNum, tempX, tempY As Integer
        Dim iContinue As Boolean
        'get connected elements
        For x = 0 To CropWidth - 1
            For y = 0 To CropHeight - 1
                Dim OriX = FirstPtOfEdge.X + x
                Dim OriY = FirstPtOfEdge.Y + y

                If Edge(y, x, 0) <> 0 Then
                    Dim Pos = New PointF(OriX / CDbl(Width), OriY / CDbl(Height))
                    C_Curve(totalCnt).CurvePoint(C_Curve(totalCnt).CPointIndx) = Pos
                    C_Curve(totalCnt).CPointIndx += 1
                    Edge(y, x, 0) = 0
                    nX3 = x
                    nY3 = y
                    While (1)
                        nX2 = nX3 - 1
                        nY2 = nY3 - 1
                        iContinue = True
                        nStackNum = 0
                        If nX2 < 0 Or nX2 > CropWidth - 1 Or nY2 < 0 Or nY2 > CropHeight - 1 Then
                            iContinue = False
                        Else
                            If Edge(nY2, nX2, 0) = 0 Then
                                iContinue = False
                            End If
                        End If
                        If iContinue Then
                            Pos = New PointF((FirstPtOfEdge.X + nX2) / CDbl(Width), (FirstPtOfEdge.Y + nY2) / CDbl(Height))
                            C_Curve(totalCnt).CurvePoint(C_Curve(totalCnt).CPointIndx) = Pos
                            C_Curve(totalCnt).CPointIndx += 1
                            Edge(nY2, nX2, 0) = 0
                            nStackNum += 1
                            tempX = nX2 : tempY = nY2
                        End If

                        nX2 = nX3
                        nY2 = nY3 - 1
                        iContinue = True
                        If nX2 < 0 Or nX2 > CropWidth - 1 Or nY2 < 0 Or nY2 > CropHeight - 1 Then
                            iContinue = False
                        Else
                            If Edge(nY2, nX2, 0) = 0 Then
                                iContinue = False
                            End If
                        End If
                        If iContinue Then
                            Pos = New PointF((FirstPtOfEdge.X + nX2) / CDbl(Width), (FirstPtOfEdge.Y + nY2) / CDbl(Height))
                            C_Curve(totalCnt).CurvePoint(C_Curve(totalCnt).CPointIndx) = Pos
                            C_Curve(totalCnt).CPointIndx += 1
                            Edge(nY2, nX2, 0) = 0
                            nStackNum += 1
                            tempX = nX2 : tempY = nY2
                        End If

                        nX2 = nX3 + 1
                        nY2 = nY3 - 1
                        iContinue = True
                        If nX2 < 0 Or nX2 > CropWidth - 1 Or nY2 < 0 Or nY2 > CropHeight - 1 Then
                            iContinue = False
                        Else
                            If Edge(nY2, nX2, 0) = 0 Then
                                iContinue = False
                            End If
                        End If
                        If iContinue Then
                            Pos = New PointF((FirstPtOfEdge.X + nX2) / CDbl(Width), (FirstPtOfEdge.Y + nY2) / CDbl(Height))
                            C_Curve(totalCnt).CurvePoint(C_Curve(totalCnt).CPointIndx) = Pos
                            C_Curve(totalCnt).CPointIndx += 1
                            Edge(nY2, nX2, 0) = 0
                            nStackNum += 1
                            tempX = nX2 : tempY = nY2
                        End If

                        nX2 = nX3 - 1
                        nY2 = nY3
                        iContinue = True
                        If nX2 < 0 Or nX2 > CropWidth - 1 Or nY2 < 0 Or nY2 > CropHeight - 1 Then
                            iContinue = False
                        Else
                            If Edge(nY2, nX2, 0) = 0 Then
                                iContinue = False
                            End If
                        End If
                        If iContinue Then
                            Pos = New PointF((FirstPtOfEdge.X + nX2) / CDbl(Width), (FirstPtOfEdge.Y + nY2) / CDbl(Height))
                            C_Curve(totalCnt).CurvePoint(C_Curve(totalCnt).CPointIndx) = Pos
                            C_Curve(totalCnt).CPointIndx += 1
                            Edge(nY2, nX2, 0) = 0
                            nStackNum += 1
                            tempX = nX2 : tempY = nY2
                        End If

                        nX2 = nX3 + 1
                        nY2 = nY3
                        iContinue = True
                        If nX2 < 0 Or nX2 > CropWidth - 1 Or nY2 < 0 Or nY2 > CropHeight - 1 Then
                            iContinue = False
                        Else
                            If Edge(nY2, nX2, 0) = 0 Then
                                iContinue = False
                            End If
                        End If
                        If iContinue Then
                            Pos = New PointF((FirstPtOfEdge.X + nX2) / CDbl(Width), (FirstPtOfEdge.Y + nY2) / CDbl(Height))
                            C_Curve(totalCnt).CurvePoint(C_Curve(totalCnt).CPointIndx) = Pos
                            C_Curve(totalCnt).CPointIndx += 1
                            Edge(nY2, nX2, 0) = 0
                            nStackNum += 1
                            tempX = nX2 : tempY = nY2
                        End If

                        nX2 = nX3 - 1
                        nY2 = nY3 + 1
                        iContinue = True
                        If nX2 < 0 Or nX2 > CropWidth - 1 Or nY2 < 0 Or nY2 > CropHeight - 1 Then
                            iContinue = False
                        Else
                            If Edge(nY2, nX2, 0) = 0 Then
                                iContinue = False
                            End If
                        End If
                        If iContinue Then
                            Pos = New PointF((FirstPtOfEdge.X + nX2) / CDbl(Width), (FirstPtOfEdge.Y + nY2) / CDbl(Height))
                            C_Curve(totalCnt).CurvePoint(C_Curve(totalCnt).CPointIndx) = Pos
                            C_Curve(totalCnt).CPointIndx += 1
                            Edge(nY2, nX2, 0) = 0
                            nStackNum += 1
                            tempX = nX2 : tempY = nY2
                        End If

                        nX2 = nX3
                        nY2 = nY3 + 1
                        iContinue = True
                        If nX2 < 0 Or nX2 > CropWidth - 1 Or nY2 < 0 Or nY2 > CropHeight - 1 Then
                            iContinue = False
                        Else
                            If Edge(nY2, nX2, 0) = 0 Then
                                iContinue = False
                            End If
                        End If
                        If iContinue Then
                            Pos = New PointF((FirstPtOfEdge.X + nX2) / CDbl(Width), (FirstPtOfEdge.Y + nY2) / CDbl(Height))
                            C_Curve(totalCnt).CurvePoint(C_Curve(totalCnt).CPointIndx) = Pos
                            C_Curve(totalCnt).CPointIndx += 1
                            Edge(nY2, nX2, 0) = 0
                            nStackNum += 1
                            tempX = nX2 : tempY = nY2
                        End If

                        nX2 = nX3 + 1
                        nY2 = nY3 + 1
                        iContinue = True
                        If nX2 < 0 Or nX2 > CropWidth - 1 Or nY2 < 0 Or nY2 > CropHeight - 1 Then
                            iContinue = False
                        Else
                            If Edge(nY2, nX2, 0) = 0 Then
                                iContinue = False
                            End If
                        End If
                        If iContinue Then
                            Pos = New PointF((FirstPtOfEdge.X + nX2) / CDbl(Width), (FirstPtOfEdge.Y + nY2) / CDbl(Height))
                            C_Curve(totalCnt).CurvePoint(C_Curve(totalCnt).CPointIndx) = Pos
                            C_Curve(totalCnt).CPointIndx += 1
                            Edge(nY2, nX2, 0) = 0
                            nStackNum += 1
                            tempX = nX2 : tempY = nY2
                        End If

                        If nStackNum < 1 Then
                            Exit While
                        End If
                        nX3 = tempX : nY3 = tempY
                    End While
                    totalCnt += 1
                    If totalCnt > 1000 Then
                        Exit For
                    End If
                End If
            Next
        Next

        'get the tallest curve
        Dim maxCnt, maxCount As Integer
        maxCnt = 0 : maxCount = 0
        For i = 0 To totalCnt - 1
            If C_Curve(i).CPointIndx > maxCount Then
                maxCnt = i
                maxCount = C_Curve(i).CPointIndx
            End If
        Next
        C_Curve(maxCnt).CPointIndx -= 1
        Return C_Curve(maxCnt)

    End Function


    ''' <summary>
    ''' Convert system.drawing.Image to Mat
    ''' </summary>
    ''' <paramname="image">The source image.</param>
    Public Function GetMatFromSDImage(ByVal image As Image) As Mat
        If image Is Nothing Then
            Return Nothing
        End If
        Dim stride = 0
        Dim bmp As Bitmap = New Bitmap(image)

        Dim rect As Rectangle = New Rectangle(0, 0, bmp.Width, bmp.Height)
        Dim bmpData = bmp.LockBits(rect, Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat)

        Dim pf = bmp.PixelFormat
        If pf = Imaging.PixelFormat.Format32bppArgb Then
            stride = bmp.Width * 4
        Else
            stride = bmp.Width * 3
        End If

        Dim cvImage As Emgu.CV.Image(Of Bgra, Byte) = New Emgu.CV.Image(Of Bgra, Byte)(bmp.Width, bmp.Height, stride, bmpData.Scan0)

        bmp.UnlockBits(bmpData)

        Return cvImage.Mat
    End Function

    ''' <summary>
    ''' Load Image to picture box from a byte array.
    ''' </summary>
    ''' <paramname="pictureBox">The picture box to load image on.</param>
    ''' <paramname="byteArray">The byte array to get image form.</param>
    <Extension()>
    Public Sub LoadImageFromByteArray(ByVal pictureBox As PictureBox, ByVal byteArray As Byte())

        Dim stream As MemoryStream = New MemoryStream(byteArray)
        Dim img = Image.FromStream(stream)
        stream.Close()
        pictureBox.Image = img

    End Sub

    ''' <summary>
    ''' Gets a byte array copy of the image inside picture box.
    ''' </summary>
    ''' <paramname="pictureBox">The picture box to get byte image form.</param>
    ''' <returns></returns>
    <Extension()>
    Public Function GetByteImage(ByVal pictureBox As PictureBox) As Byte()

        Dim ImgByteArray As Byte()
        Dim stream As MemoryStream = New MemoryStream()
        pictureBox.Image.Save(stream, ImageFormat.Jpeg) 'TODO- Fix gdi+ error
        ImgByteArray = stream.ToArray()
        stream.Close()

        Return ImgByteArray
    End Function

    ''' <summary>
    ''' Gets a byte array copy of the image inside picture box.
    ''' </summary>
    ''' <paramname="pictureBox">The picture box to get byte image form.</param>
    ''' <returns></returns>
    Public Function GetByteImage(ByVal image As Image) As Byte()

        Dim ImgByteArray As Byte()
        Dim stream As MemoryStream = New MemoryStream()
        image.Save(stream, ImageFormat.Jpeg) 'TODO- Fix gdi+ error
        ImgByteArray = stream.ToArray()
        stream.Close()

        Return ImgByteArray
    End Function

    Public Function GetEdgeFromBinary(Scr As Emgu.CV.Image(Of Gray, Byte)) As Emgu.CV.Image(Of Gray, Byte)
        Dim Dst = Scr.Copy()

        CvInvoke.MedianBlur(Dst, Dst, 3)
        CvInvoke.Canny(Dst, Dst, 100, 200)
        Return Dst
    End Function

    ''' <summary>
    ''' get skeleton image.
    ''' </summary>
    ''' <paramname="image">The input image.</param>
    ''' <returns></returns>
    Public Function Skelatanize(ByVal image As Bitmap) As Emgu.CV.Image(Of Gray, Byte)
        Dim imgOld As Emgu.CV.Image(Of Gray, Byte) = image.ToImage(Of Gray, Byte)()
        Dim img2 As Emgu.CV.Image(Of Gray, Byte) = (New Emgu.CV.Image(Of Gray, Byte)(imgOld.Width, imgOld.Height, New Gray(255))).Sub(imgOld)
        Dim eroded As Emgu.CV.Image(Of Gray, Byte) = New Emgu.CV.Image(Of Gray, Byte)(img2.Size)
        Dim temp As Emgu.CV.Image(Of Gray, Byte) = New Emgu.CV.Image(Of Gray, Byte)(img2.Size)
        Dim skel As Emgu.CV.Image(Of Gray, Byte) = New Emgu.CV.Image(Of Gray, Byte)(img2.Size)
        skel.SetValue(0)
        CvInvoke.Threshold(img2, img2, 127, 256, 0)
        Dim element = CvInvoke.GetStructuringElement(ElementShape.Cross, New Size(3, 3), New Point(-1, -1))
        Dim done = False

        While Not done
            Call CvInvoke.Erode(img2, eroded, element, New Point(-1, -1), 1, BorderType.Reflect, Nothing)
            Call CvInvoke.Dilate(eroded, temp, element, New Point(-1, -1), 1, BorderType.Reflect, Nothing)
            CvInvoke.Subtract(img2, temp, temp)
            CvInvoke.BitwiseOr(skel, temp, skel)
            eroded.CopyTo(img2)
            If CvInvoke.CountNonZero(img2) = 0 Then done = True
        End While
        Return skel
    End Function

    ''' <summary>
    ''' Dilate image.
    ''' </summary>
    ''' <paramname="image">The input image.</param>
    ''' <returns></returns>
    Public Function Dilation(Src As Bitmap) As Bitmap
        Dim imgOld As Emgu.CV.Image(Of Bgr, Byte) = Src.ToImage(Of Bgr, Byte)()
        Dim Dst As Emgu.CV.Image(Of Bgr, Byte) = New Emgu.CV.Image(Of Bgr, Byte)(imgOld.Size)
        Dim element = CvInvoke.GetStructuringElement(ElementShape.Cross, New Size(3, 3), New Point(-1, -1))
        Call CvInvoke.Dilate(imgOld, Dst, element, New Point(-1, -1), 1, BorderType.Reflect, Nothing)
        imgOld.Dispose()
        Return Dst.AsBitmap()
    End Function

    ''' <summary>
    ''' Dilate image.
    ''' </summary>
    ''' <paramname="image">The input image.</param>
    ''' <returns></returns>
    Public Function Erosion(Src As Bitmap) As Bitmap
        Dim imgOld As Emgu.CV.Image(Of Bgr, Byte) = Src.ToImage(Of Bgr, Byte)()
        Dim Dst As Emgu.CV.Image(Of Bgr, Byte) = New Emgu.CV.Image(Of Bgr, Byte)(imgOld.Size)
        Dim element = CvInvoke.GetStructuringElement(ElementShape.Cross, New Size(3, 3), New Point(-1, -1))
        Call CvInvoke.Erode(imgOld, Dst, element, New Point(-1, -1), 1, BorderType.Reflect, Nothing)
        imgOld.Dispose()
        Return Dst.AsBitmap()
    End Function

#Region "Identify Circle Methods"
    ''' <summary>
    ''' Identify circles according to roundness and thr_cir.
    ''' </summary>
    ''' <paramname="scr">The source image.</param>
    ''' <paramname="roundness">The threshold for detecing circles with circularity.</param>
    ''' <paramname="thr_cir">The threshold for detecing circles with area.</param>
    ''' <paramname="Obj">The object to contain circles.</param>
    ''' 
    Public Function IdentifyCicles(ByVal scr As Image, ByVal roundness As Integer, ByVal thr_cir As Integer, ByVal Obj As SegObject) As Image
        'inputImage type is System.Drawing.Image

        Dim bitmapImage As Bitmap = New Bitmap(scr)

        Dim rectangle As Rectangle = New Rectangle(0, 0, bitmapImage.Width, bitmapImage.Height) 'System.Drawing
        Dim bmpData = bitmapImage.LockBits(rectangle, ImageLockMode.ReadWrite, bitmapImage.PixelFormat) 'System.Drawing.Imaging

        'outputImage type is Emgu.CV.Image
        Dim outputImage As Emgu.CV.Image(Of Bgra, Byte) = New Emgu.CV.Image(Of Bgra, Byte)(bitmapImage.Width, bitmapImage.Height, bmpData.Stride, bmpData.Scan0) '(IntPtr)
        bitmapImage.UnlockBits(bmpData)


        Dim simpleBlobDetector As SimpleBlobDetector = New SimpleBlobDetector(New SimpleBlobDetectorParams() With {
    .FilterByCircularity = True,
    .FilterByArea = True,
    .MinCircularity = CSng(roundness) / 100,
    .MaxCircularity = 1.0F,
    .MinArea = thr_cir,
    .MaxArea = 10000
})

        Dim modelKeyPoints As VectorOfKeyPoint = New VectorOfKeyPoint()

        Dim keypoints = simpleBlobDetector.Detect(outputImage)

        Dim circles As List(Of Integer()) = New List(Of Integer())
        For i = 0 To keypoints.Length - 1
            Dim circle = New Integer(3) {}
            circle(0) = SegType.circle
            circle(1) = CInt(keypoints(i).Point.X)
            circle(2) = CInt(keypoints(i).Point.Y)
            circle(3) = CInt(keypoints(i).Size)

            circles.Add(circle)
        Next

        For i = 0 To circles.Count - 1 - 1
            For j = i + 1 To circles.Count - 1
                Dim circle_i = circles(i)
                Dim circle_j = circles(j)
                If circle_i(3) < circle_j(3) Then
                    circles(i) = circle_j
                    circles(j) = circle_i
                End If
            Next
        Next

        For i = 0 To keypoints.Length - 1
            If i > 1000 Then Continue For

            Dim circle = circles(i)

            Dim pos = New PointF(circle(1) / CDbl(scr.Width), circle(2) / CDbl(scr.Height))
            Dim size = circle(3) / CDbl(scr.Width)

            If Obj.circleObj.Cnt < 1001 Then
                Obj.circleObj.Cnt = i + 1
                Obj.circleObj.pos(i) = pos
                Obj.circleObj.size(i) = size
            End If

        Next

        modelKeyPoints.Push(keypoints)

        Call Features2DToolbox.DrawKeypoints(outputImage, modelKeyPoints, outputImage, New Bgr(Color.Red), Features2DToolbox.KeypointDrawType.DrawRichKeypoints)

        'ImageView.imshow(outputImage);

        Dim array As Byte() = outputImage.ToJpegData()
        outputImage.Dispose()
        circles.Clear()

        Dim stream As MemoryStream = New MemoryStream(array)
        Dim img = Image.FromStream(stream)

        Return img
    End Function

    ''' <summary>
    ''' Identify circles according to roundness and thr_cir.
    ''' </summary>
    ''' <paramname="pictureBox">The source image.</param>
    ''' <paramname="roundness">The threshold for detecing circles with circularity.</param>
    ''' <paramname="thr_seg">The threshold for segmetation.</param>
    ''' <paramname="Obj">The object containing items.</param>

    Public Sub IdentifyInterSections(g As Graphics, grayImage As Emgu.CV.Image(Of Gray, Byte), Obj As SegObject, Width As Integer, Height As Integer) As Image

        Dim Pen As Pen = New Pen(Color.Blue, 1)
        Dim Pen2 As Pen = New Pen(Color.Green, 1)

        Dim tempimg = grayImage.Copy()
        CvInvoke.MedianBlur(grayImage, grayImage, 3)
        CvInvoke.Canny(grayImage, tempimg, 100, 200)

        Dim gray_data = tempimg.Data
        Dim CntInLine = 0
        For i = 1 To Obj.sectObj.horLine
            Dim y As Integer = i * grayImage.Height / (Obj.sectObj.horLine + 1)
            CntInLine = 0
            Dim yDraw As Integer = i * Height / (Obj.sectObj.horLine + 1)
            g.DrawLine(Pen2, New Point(0, yDraw), New Point(Width - 1, yDraw))
            For x = 0 To grayImage.Width - 1
                If gray_data(y, x, 0) > 0 Then

                    Dim pos = New PointF(x / CDbl(grayImage.Width), y / CDbl(grayImage.Height))
                    Obj.sectObj.horSectPos(i - 1, CntInLine) = pos
                    CntInLine += 1
                    x += 5

                    g.DrawEllipse(Pen, New Rectangle(pos.X * Width - 2, pos.Y * Height - 2, 4, 4))
                End If
            Next

            Obj.sectObj.horSectCnt(i - 1) = CntInLine
        Next

        For i = 1 To Obj.sectObj.verLine
            Dim x As Integer = i * grayImage.Width / (Obj.sectObj.verLine + 1)
            CntInLine = 0
            Dim xDraw As Integer = i * Width / (Obj.sectObj.verLine + 1)
            g.DrawLine(Pen2, New Point(xDraw, 0), New Point(xDraw, Height - 1))
            For y = 0 To grayImage.Height - 1
                If gray_data(y, x, 0) > 0 Then

                    Dim pos = New PointF(x / CDbl(grayImage.Width), y / CDbl(grayImage.Height))
                    Obj.sectObj.verSectPos(i - 1, CntInLine) = pos
                    CntInLine += 1
                    y += 5

                    g.DrawEllipse(Pen, New Rectangle(pos.X * Width - 2, pos.Y * Height - 2, 4, 4))
                End If
            Next
            Obj.sectObj.verSectCnt(i - 1) = CntInLine
        Next

        Pen.Dispose()
        Pen2.Dispose()
        tempimg.Dispose()
    End Sub

    ''' <summary>
    ''' Identify circles according to roundness and thr_cir.
    ''' </summary>
    ''' <paramname="pictureBox">The source image.</param>
    ''' <paramname="roundness">The threshold for detecing circles with circularity.</param>
    ''' <paramname="thr_seg">The threshold for segmetation.</param>
    ''' <paramname="Obj">The object containing items.</param>

    Public Sub IdentifyInterSections(ByVal pictureBox As PictureBox, scr As Emgu.CV.Image(Of Gray, Byte), Obj As SegObject) As Image
        Dim g As Graphics = pictureBox.CreateGraphics()
        IdentifyInterSections(g, scr, Obj, pictureBox.Width, pictureBox.Height)
        g.Dispose()

    End Sub



#End Region

#Region "Segment Method"
    ''' <summary>
    ''' Segment Remaing Image into Black and White.
    ''' </summary>
    ''' <paramname="src">The source image.</param>
    ''' <paramname="thr_seg">The threshold for segmentation into black and white.</param>
    ''' <paramname="Obj">The object containing circles.</param>
    ''' <paramname="percent_black">The percentage of black area.</param>
    ''' <paramname="percent_white">The percentage of white area.</param>

    Public Function SegmentIntoBlackAndWhite(ByVal scr As Image, ByVal thr_seg As Integer, ByVal Obj As SegObject, ByRef percent_black As Double, ByRef percent_white As Double) As Image
        'inputImage type is System.Drawing.Image

        Dim bmpImage As Bitmap = New Bitmap(scr)
        Dim colorImage As Emgu.CV.Image(Of Bgr, Byte) = bmpImage.ToImage(Of Bgr, Byte)()
        bmpImage.Dispose()

        Dim grayImage As Emgu.CV.Image(Of Gray, Byte) = colorImage.Convert(Of Gray, Byte)()

        'ImageView.imshow(grayImage);
        CvInvoke.Threshold(grayImage, grayImage, thr_seg, 255, CvEnum.ThresholdType.Binary)


        Dim area_cir = 0
        Dim area_black = 0
        Dim area_white = 0
        Dim color = New Bgr(Drawing.Color.Gray)

        For i = 0 To Obj.circleObj.Cnt - 1
            Dim center = New Point(Obj.circleObj.pos(i).X * scr.Width, Obj.circleObj.pos(i).Y * scr.Height)
            Dim radius As Integer = Obj.circleObj.size(i) * scr.Width / 2
            Dim thickness = 1

            Dim left = Math.Max(0, center.X - radius)
            Dim top = Math.Max(0, center.Y - radius)
            Dim right = Math.Min(grayImage.Width, center.X + radius)
            Dim bottom = Math.Min(grayImage.Height, center.Y + radius)

            For y As Integer = top To bottom - 1
                For x As Integer = left To right - 1
                    'offset to center
                    Dim virtX = x - center.X
                    Dim virtY = y - center.Y

                    If Math.Sqrt(virtX * virtX + virtY * virtY) <= radius Then
                        grayImage.Data(y, x, 0) = 128
                    End If

                Next
            Next

            'CvInvoke.Circle(grayImage, center, radius, color.MCvScalar, thickness, Emgu.CV.CvEnum.LineType.Filled, 0);

        Next

        'counts the pixels in white region and convert gray to bgr

        Dim color_circle = New Bgr(Drawing.Color.Red)
        Dim color_black = New Bgr(Drawing.Color.Black)

        Dim color_data = colorImage.Data
        Dim gray_data = grayImage.Data

        ' Assign color to each pixel
        For y = 0 To colorImage.Height - 1
            For x = 0 To colorImage.Width - 1
                If gray_data(y, x, 0) = 255 Then
                    area_white += 1
                ElseIf gray_data(y, x, 0) = 128 Then
                    color_data(y, x, 0) = 0
                    color_data(y, x, 1) = 0
                    color_data(y, x, 2) = 255
                    area_cir += 1
                Else
                    color_data(y, x, 0) = 0
                    color_data(y, x, 1) = 0
                    color_data(y, x, 2) = 0
                    area_black += 1
                End If
            Next
        Next


        percent_white = Math.Round(area_white * 100 / (grayImage.Width * grayImage.Height), 2)
        percent_black = Math.Round((grayImage.Height * grayImage.Width - area_white - area_cir) * 100 / (grayImage.Height * grayImage.Width), 2)

        Dim array As Byte() = colorImage.ToJpegData()

        grayImage.Dispose()
        colorImage.Dispose()

        Dim stream As MemoryStream = New MemoryStream(array)
        Dim img = Image.FromStream(stream)

        Return img
    End Function

    ''' <summary>
    ''' Segment Remaing Image into Black and White.
    ''' </summary>
    ''' <paramname="src">The source image.</param>
    ''' <paramname="thr_seg">The threshold for segmentation into black and white.</param>

    Public Function GetEdgeFromBinary(ByVal scr As Image, ByVal thr_seg As Integer) As Image
        'inputImage type is System.Drawing.Image

        Dim bmpImage As Bitmap = New Bitmap(scr)
        Dim colorImage As Emgu.CV.Image(Of Bgr, Byte) = bmpImage.ToImage(Of Bgr, Byte)()
        bmpImage.Dispose()

        Dim grayImage As Emgu.CV.Image(Of Gray, Byte) = colorImage.Convert(Of Gray, Byte)()

        'ImageView.imshow(grayImage);
        CvInvoke.Threshold(grayImage, grayImage, thr_seg, 255, CvEnum.ThresholdType.Binary)

        Dim tempimg As Emgu.CV.Image(Of Emgu.CV.Structure.Gray, Byte) = grayImage
        CvInvoke.MedianBlur(grayImage, grayImage, 3)
        'Emgu.CV.CvInvoke.Imshow("original image", cropped)
        CvInvoke.Canny(grayImage, tempimg, 100, 200)

        Dim array As Byte() = tempimg.ToJpegData()

        colorImage.Dispose()
        grayImage.Dispose()
        tempimg.Dispose()

        Dim stream As MemoryStream = New MemoryStream(array)
        Dim img = Image.FromStream(stream)

        Return img
    End Function

    ''' <summary>
    ''' Colour third thresholded region as green.
    ''' </summary>
    ''' <paramname="src">The source image.</param>
    ''' <paramname="thr_seg">The threshold for segmentation into black and white.</param>
    ''' <paramname="circles">The List for center points and size of circles.</param>
    ''' <paramname="percent_black">The percentage of black area.</param>
    ''' <paramname="percent_white">The percentage of white area.</param>

    Public Function ColourThirdRegion(ByVal scr As Image, ByVal thr_seg As Integer, ByVal circles As List(Of Integer()), ByRef percent_black As Integer, ByRef percent_white As Integer) As Image
        'inputImage type is System.Drawing.Image

        Dim bmpImage As Bitmap = New Bitmap(scr)
        Dim colorImage As Emgu.CV.Image(Of Bgr, Byte) = bmpImage.ToImage(Of Bgr, Byte)()
        bmpImage.Dispose()

        Dim grayImage As Emgu.CV.Image(Of Gray, Byte) = colorImage.Convert(Of Gray, Byte)()

        'ImageView.imshow(grayImage);
        CvInvoke.Threshold(grayImage, grayImage, thr_seg, 255, CvEnum.ThresholdType.Binary)


        Dim area_cir = 0
        Dim area_black = 0
        Dim area_white = 0
        Dim color = New Bgr(Drawing.Color.Gray)

        For i = 0 To circles.Count - 1
            Dim circle = circles(i)
            Dim center = New Point(circle(1), circle(2))
            Dim radius = circle(3) / 2
            Dim thickness = 1
            Dim area = radius * radius * Math.PI
            area_cir += CInt(area)

            Dim left = Math.Max(0, center.X - radius)
            Dim top = Math.Max(0, center.Y - radius)
            Dim right = Math.Min(grayImage.Width, center.X + radius)
            Dim bottom = Math.Min(grayImage.Height, center.Y + radius)

            For y As Integer = top To bottom - 1
                For x As Integer = left To right - 1
                    'offset to center
                    Dim virtX = x - center.X
                    Dim virtY = y - center.Y

                    If Math.Sqrt(virtX * virtX + virtY * virtY) <= radius Then
                        grayImage.Data(y, x, 0) = 128
                    End If

                Next
            Next

            'CvInvoke.Circle(grayImage, center, radius, color.MCvScalar, thickness, Emgu.CV.CvEnum.LineType.Filled, 0);

        Next

        'counts the pixels in white region and convert gray to bgr

        Dim color_circle = New Bgr(Drawing.Color.Red)
        Dim color_black = New Bgr(Drawing.Color.Black)

        Dim color_data = colorImage.Data
        Dim gray_data = grayImage.Data

        ' Assign color to each pixel
        For y = 0 To colorImage.Height - 1
            For x = 0 To colorImage.Width - 1
                If gray_data(y, x, 0) = 255 Then
                    area_white += 1
                    color_data(y, x, 0) = 0
                    color_data(y, x, 1) = 255
                    color_data(y, x, 2) = 0
                ElseIf gray_data(y, x, 0) = 128 Then
                    color_data(y, x, 0) = 0
                    color_data(y, x, 1) = 0
                    color_data(y, x, 2) = 255
                Else
                    color_data(y, x, 0) = 0
                    color_data(y, x, 1) = 0
                    color_data(y, x, 2) = 0
                End If
            Next
        Next


        percent_white = area_white * 100 / (grayImage.Width * grayImage.Height)
        percent_black = (grayImage.Height * grayImage.Width - area_white - area_cir) * 100 / (grayImage.Height * grayImage.Width)

        Dim array As Byte() = colorImage.ToJpegData()

        grayImage.Dispose()
        colorImage.Dispose()

        Dim stream As MemoryStream = New MemoryStream(array)
        Dim img = Image.FromStream(stream)

        Return img
    End Function

#End Region

#Region "Subtract Method"
    ''' <summary>
    ''' Subtract Circles from Image.
    ''' </summary>
    ''' <paramname="scr">The source image.</param>
    ''' <paramname="Obj">The object containing circles.</param>
    ''' <paramname="percentage">The percentage of circles.</param>

    Public Function SubtractCircles(ByVal src As Image, ByVal Obj As SegObject, ByRef percentage As Double) As Image
        'inputImage type is System.Drawing.Image

        Dim bitmapImage As Bitmap = New Bitmap(src)

        Dim rectangle As Rectangle = New Rectangle(0, 0, bitmapImage.Width, bitmapImage.Height) 'System.Drawing
        Dim bmpData = bitmapImage.LockBits(rectangle, ImageLockMode.ReadWrite, bitmapImage.PixelFormat) 'System.Drawing.Imaging

        'outputImage type is Emgu.CV.Image
        Dim outputImage As Emgu.CV.Image(Of Bgra, Byte) = New Emgu.CV.Image(Of Bgra, Byte)(bitmapImage.Width, bitmapImage.Height, bmpData.Stride, bmpData.Scan0) '(IntPtr)
        bitmapImage.UnlockBits(bmpData)

        'ImageView.imShow(outputImage);
        'subtract pixels in image by set its value to 128
        Dim area_cir = 0

        For i = 0 To Obj.circleObj.Cnt - 1
            Dim center = New Point(Obj.circleObj.pos(i).X * src.Width, Obj.circleObj.pos(i).Y * src.Height)
            Dim radius As Integer = Obj.circleObj.size(i) * src.Width / 2
            Dim color = New Bgra(0, 0, 255, 128)
            Dim thickness = 1
            Dim area = Math.PI * radius * radius
            area_cir += CInt(area)

            Dim left = Math.Max(0, center.X - radius)
            Dim top = Math.Max(0, center.Y - radius)
            Dim right = Math.Min(bitmapImage.Width, center.X + radius)
            Dim bottom = Math.Min(bitmapImage.Height, center.Y + radius)

            For y As Integer = top To bottom - 1
                For x As Integer = left To right - 1
                    'offset to center
                    Dim virtX = x - center.X
                    Dim virtY = y - center.Y

                    If Math.Sqrt(virtX * virtX + virtY * virtY) <= radius Then
                        'data[y, x, 0] = 128;
                        'data[y, x, 1] = 128;
                        'data[y, x, 2] = 128;
                        outputImage(y, x) = color
                    End If

                Next
            Next

        Next

        Dim array As Byte() = outputImage.ToJpegData()
        outputImage.Dispose()
        percentage = Math.Round(area_cir * 100 / (bitmapImage.Width * bitmapImage.Height), 2)

        Dim stream As MemoryStream = New MemoryStream(array)
        Dim img = Image.FromStream(stream)

        Return img

    End Function

#End Region

#Region "Highlight Method"
    ''' <summary>
    ''' Hightlight the Circle which is clicked.
    ''' </summary>
    ''' <paramname="scr">The source image.</param>
    ''' <paramname="point">The point that mouse cursor indicates.</param>
    ''' <paramname="circles">The List includes the center points and size of circles.</param>
    ''' <paramname="index">The index of circles which is selected.</param>

    Public Function HighlightCircle(ByVal scr As Image, ByVal point As Point, ByVal circles As List(Of Integer()), ByRef index As Integer) As Image
        'inputImage type is System.Drawing.Image

        Dim bitmapImage As Bitmap = New Bitmap(scr)

        Dim rectangle As Rectangle = New Rectangle(0, 0, bitmapImage.Width, bitmapImage.Height) 'System.Drawing
        Dim bmpData = bitmapImage.LockBits(rectangle, ImageLockMode.ReadWrite, bitmapImage.PixelFormat) 'System.Drawing.Imaging

        'outputImage type is Emgu.CV.Image
        Dim outputImage As Emgu.CV.Image(Of Bgra, Byte) = New Emgu.CV.Image(Of Bgra, Byte)(bitmapImage.Width, bitmapImage.Height, bmpData.Stride, bmpData.Scan0)
        bitmapImage.UnlockBits(bmpData)

        'find the index of circles
        For i = 0 To circles.Count - 1
            Dim circle = circles(i)
            Dim radius = circle(3) / 2
            Dim left As Integer = Math.Max(circle(1) - radius, 0)
            Dim top As Integer = Math.Max(circle(2) - radius, 0)
            Dim right As Integer = Math.Min(circle(1) + radius, bitmapImage.Width - 1)
            Dim bottom As Integer = Math.Min(circle(2) + radius, bitmapImage.Height - 1)

            If point.X > left AndAlso point.X < right AndAlso point.Y > top AndAlso point.Y < bottom Then
                index = i
                Exit For
            End If
        Next

        If index >= 0 Then
            Dim circle = circles(index)
            Dim center = New Point(circle(1), circle(2))
            Dim color = New Bgr(Drawing.Color.Blue)
            Dim size = circle(3)
            Dim radius = circle(3) / 2
            Dim thickness = 2

            CvInvoke.Circle(outputImage, center, radius, color.MCvScalar, thickness, CvEnum.LineType.FourConnected, 0)
        End If

        'ImageView.imshow(outputImage);
        Dim array As Byte() = outputImage.ToJpegData()
        outputImage.Dispose()

        Dim stream As MemoryStream = New MemoryStream(array)
        Dim img = Image.FromStream(stream)

        Return img
    End Function

#End Region

#Region "Combination Method"
    ''' <summary>
    ''' Combine the original image and segmented image.
    ''' </summary>
    ''' <paramname="ori">The original image.</param>
    ''' <paramname="segmented">The segmented image.</param>
    ''' <paramname="trans">The transparent of segmented image.</param>

    Public Function CombineOriAndSeg(ByVal ori As Image, ByVal segmented As Image, ByVal trans As Integer) As Image

        Dim bmpImage As Bitmap = New Bitmap(ori)
        Dim ori_Image As Emgu.CV.Image(Of Bgr, Byte) = bmpImage.ToImage(Of Bgr, Byte)()
        bmpImage.Dispose()

        Dim bmpImage2 As Bitmap = New Bitmap(segmented)
        Dim seg_Image As Emgu.CV.Image(Of Bgr, Byte) = bmpImage2.ToImage(Of Bgr, Byte)()
        bmpImage2.Dispose()

        'ImageView.imshow(ori_Image);
        'ImageView.imShow(seg_Image);

        ' Data, it's faster not to iterate over a property
        Dim data = ori_Image.Data
        Dim pixels = seg_Image.Data

        ' Assign color to each pixel
        For y = 0 To ori_Image.Height - 1
            For x = 0 To ori_Image.Width - 1
                Dim xImg = x
                Dim yImg = y
                Dim alphaLay = CSng(trans) / 100
                Dim alphaImg = 1 - alphaLay

                data(yImg, xImg, 0) = CByte(pixels(y, x, 0) * alphaLay + data(yImg, xImg, 0) * alphaImg)
                data(yImg, xImg, 1) = CByte(pixels(y, x, 1) * alphaLay + data(yImg, xImg, 1) * alphaImg)
                data(yImg, xImg, 2) = CByte(pixels(y, x, 2) * alphaLay + data(yImg, xImg, 2) * alphaImg)
                'data[yImg, xImg, 3] = (byte)((alphaLay == data[yImg, xImg, 3] && alphaLay == 0) ? 0 : 255);
            Next
        Next

        'ImageView.imshow(ori_Image);
        Dim array As Byte() = ori_Image.ToJpegData()
        ori_Image.Dispose()
        seg_Image.Dispose()

        Dim stream As MemoryStream = New MemoryStream(array)
        Dim img = Image.FromStream(stream)

        Return img
    End Function

    ''' <summary>
    ''' Overlap segmented image up to Original Image.
    ''' </summary>
    ''' <paramname="ori">The original image.</param>
    ''' <paramname="segmented">The segmented image.</param>

    Public Function OverLapSegToOri(ByVal ori As Emgu.CV.Image(Of Emgu.CV.Structure.Bgr, Byte), ByVal segmented As Emgu.CV.Image(Of Emgu.CV.Structure.Gray, Byte), Optional left As Single = 0, Optional top As Single = 0, Optional right As Single = 0, Optional bottom As Single = 0, Optional FillingColor As String = "Red") As Emgu.CV.Image(Of Emgu.CV.Structure.Bgr, Byte)
        Dim Result = ori.Clone()
        Dim Col = System.Drawing.Color.FromName(FillingColor)


        ' Data, it's faster not to iterate over a property
        Dim data = Result.Data
        Dim pixels = segmented.Data

        Dim startX = 0
        Dim startY = 0
        Dim endX = ori.Width - 1
        Dim endY = ori.Height - 1

        If left <> 0 Or right <> 0 Or top <> 0 Or bottom <> 0 Then
            startX = left * ori.Width
            startY = top * ori.Height
            endX = right * ori.Width
            endY = bottom * ori.Height
        End If
        ' Assign color to each pixel
        For y = startY To endY
            For x = startX To endX
                Dim xImg = x
                Dim yImg = y
                If pixels(yImg, xImg, 0) > 128 Then
                    data(yImg, xImg, 0) = Col.B
                    data(yImg, xImg, 1) = Col.G
                    data(yImg, xImg, 2) = Col.R
                End If
            Next
        Next

        Return Result
    End Function
#End Region

#Region "Phase Segmentation"
    ''' <summary>
    ''' Convert RGB to Gray.
    ''' </summary>
    ''' <paramname="buf">The RGB image.</param>
    Public Function getGrayScale(ByVal buf As Emgu.CV.Image(Of Emgu.CV.Structure.Bgr, Byte)) As Emgu.CV.Image(Of Emgu.CV.Structure.Gray, Byte)

        Dim gray As Emgu.CV.Image(Of Emgu.CV.Structure.Gray, Byte) = buf.Convert(Of Gray, Byte)()

        Return gray
    End Function

    ''' <summary>
    ''' draw progress to picturebox.
    ''' </summary>
    ''' <paramname="pictureBox">The picturebox for drawing.</param>
    ''' <paramname="PhaseVal">The list of phase values.</param>
    ''' <paramname="PhaseCol">The list of colors for each phase.</param>
    Public Sub DrawProcess(pictureBox As PictureBox, PhaseVal As List(Of Integer), PhaseCol As List(Of Integer()))
        pictureBox.Refresh()
        Dim g As Graphics = pictureBox.CreateGraphics()
        Dim drawSt As Integer
        Dim drawEnd As Integer
        For i = 0 To PhaseCol.Count - 1
            drawEnd = PhaseVal(i + 1) * pictureBox.Width / 255
            drawSt = PhaseVal(i) * pictureBox.Width / 255
            Dim Col_Array = PhaseCol(i)
            Dim brushCol As Color = Color.FromArgb(Col_Array(2), Col_Array(1), Col_Array(0))
            Dim brush As Brush = New SolidBrush(brushCol)
            g.FillRectangle(brush, New Rectangle(drawSt, 0, drawEnd - drawSt, pictureBox.Height))
            brush.Dispose()
        Next

        g.Dispose()
    End Sub

    ''' <summary>
    ''' show multi segmentation.
    ''' </summary>
    ''' <paramname="ori">The original image.</param>
    ''' <paramname="PhaseVal">The list of phase values.</param>
    ''' <paramname="PhaseCol">The list of colors for each phase.</param>
    ''' <paramname="PhaseArea">The list of areas for each phase.</param>
    Public Function MultiSegment(src As Emgu.CV.Image(Of Bgr, Byte), PhaseVal As List(Of Integer), PhaseCol As List(Of Integer()), PhaseArea As List(Of Integer), PhaseSel As List(Of Integer), FirstPt As Point, SecondPt As Point, flag As Boolean) As Emgu.CV.Image(Of Bgr, Byte)
        PhaseArea.Clear()
        For i = 0 To PhaseCol.Count - 1
            PhaseArea.Add(0)
        Next

        Dim Result = src.Clone()
        Dim grayImage = getGrayScale(src)
        Dim color = Result.Data
        Dim gray = grayImage.Data

        Dim startX = 0
        Dim endX = src.Width - 1
        Dim startY = 0
        Dim endY = src.Height - 1
        If flag Then
            startX = FirstPt.X
            endX = SecondPt.X
            startY = FirstPt.Y
            endY = SecondPt.Y
        End If
        For y = startY To endY
            For x = startX To endX
                For i = 0 To PhaseCol.Count - 1
                    'Dim brushCol As Color = System.Drawing.Color.FromName(PhaseCol(i))
                    Dim brushCol = PhaseCol(i)
                    If gray(y, x, 0) < PhaseVal(i + 1) And gray(y, x, 0) >= PhaseVal(i) Then
                        If PhaseSel(i) = 1 Then
                            color(y, x, 0) = brushCol(0)    'B
                            color(y, x, 1) = brushCol(1)    'G
                            color(y, x, 2) = brushCol(2)    'R
                        End If
                        PhaseArea(i) += 1
                    End If
                Next
            Next
        Next
        grayImage.Dispose()

        Return Result
    End Function

    Public Function GetImageFromEmgu(ori As Emgu.CV.Image(Of Bgr, Byte)) As Image
        Dim array As Byte() = ori.ToJpegData()
        ori.Dispose()
        Dim stream As MemoryStream = New MemoryStream(array)
        Dim img = Image.FromStream(stream)

        Return img
    End Function

    Public Function GetImageFromEmgu(ori As Emgu.CV.Image(Of Gray, Byte)) As Image
        Dim array As Byte() = ori.ToJpegData()
        ori.Dispose()
        Dim stream As MemoryStream = New MemoryStream(array)
        Dim img = Image.FromStream(stream)

        Return img
    End Function

    Public Function GetBinary(Src As Emgu.CV.Image(Of Bgr, Byte), threshold As Integer) As Emgu.CV.Image(Of Gray, Byte)
        Dim grayImage As Emgu.CV.Image(Of Gray, Byte) = Src.Convert(Of Gray, Byte)()
        Dim Dst = grayImage.Clone()
        CvInvoke.Threshold(grayImage, Dst, threshold, 255, CvEnum.ThresholdType.Binary)
        grayImage.Dispose()
        Return Dst
    End Function
    Public Function GetBinaryWith2Thr(ori As Emgu.CV.Image(Of Gray, Byte), min As Integer, max As Integer) As Emgu.CV.Image(Of Gray, Byte)
        Dim ResImg = ori.CopyBlank()
        Dim ScrData = ori.Data
        Dim ResData = ResImg.Data

        For y = 0 To ResImg.Height - 1
            For x = 0 To ResImg.Width - 1
                If ScrData(y, x, 0) >= min And ScrData(y, x, 0) <= max Then
                    ResData(y, x, 0) = 255
                End If
            Next
        Next

        Return ResImg
    End Function

    ''' <summary>
    ''' Get current phase segmentation from mouse click.
    ''' </summary>
    ''' <paramname="ori">The original image.</param>
    ''' <paramname="mPt">The position of mouse clicked.</param>
    ''' <paramname="PhaseVal">The list of phase values.</param>
    Public Function GetCurrentSelPhase(ori As Emgu.CV.Image(Of Gray, Byte), mPt As PointF, PhaseVal As List(Of Integer)) As Integer
        Dim pos = New Point(mPt.X * ori.Width, mPt.Y * ori.Height)
        Dim intensity = ori.Data(pos.Y, pos.X, 0)
        For i = 0 To PhaseVal.Count - 2
            If intensity > PhaseVal(i) And intensity < PhaseVal(i + 1) Then
                Return i
            End If
        Next
        Return 0
    End Function


#End Region

#Region "Count and Classification"

    Public Function blur(ByVal buf As Emgu.CV.Image(Of Emgu.CV.Structure.Gray, Byte)) As Emgu.CV.Image(Of Emgu.CV.Structure.Gray, Byte)
        Dim tempimg As Emgu.CV.Image(Of Emgu.CV.Structure.Gray, Byte) = buf
        Emgu.CV.CvInvoke.GaussianBlur(buf, tempimg, New Size(5, 5), 0)
        Return tempimg
    End Function


    ''' <summary>
    ''' Identify blobs.
    ''' </summary>
    ''' <paramname="scr">The source image.</param>
    ''' <paramname="BinaryImg">The binary image.</param>
    ''' <paramname="ObjList">The object to contain information for blobs.</param>
    Public Function BlobDetection(colorImage As Emgu.CV.Image(Of Bgr, Byte), BinaryImg As Emgu.CV.Image(Of Gray, Byte), ObjList As List(Of BlobObj), minArea As Single, Optional minRound As Single = 0, Optional maxRound As Single = 1, Optional minPerAreaRatio As Single = 0, Optional maxPerAreaRatio As Single = 100) As Emgu.CV.Image(Of Bgr, Byte)

        Dim contours As New VectorOfVectorOfPoint()

        CvInvoke.FindContours(BinaryImg, contours, Nothing, RetrType.Tree, ChainApproxMethod.ChainApproxSimple)
        Dim totalArea = colorImage.Width * colorImage.Height
        Dim maxArea = totalArea / 4

        Dim ResultImg = colorImage.Copy()
        Dim perimeter, area, circularity, width, height, roundness As Double
        Dim minX, maxX, minY, maxY As Integer
        Dim Obj As BlobObj = New BlobObj
        For i = 0 To contours.Size - 1
            Dim contour = contours(i)
            perimeter = CvInvoke.ArcLength(contour, True)
            area = CvInvoke.ContourArea(contour, False)
            If area > maxArea Or area < minArea Then
                Continue For
            End If
            circularity = Math.Pow(perimeter, 2) / (4 * Math.PI * area)
            minX = 9999999
            minY = 9999999
            maxX = 0
            maxY = 0
            Dim pts As Point() = New Point(contour.Size - 1) {}
            For j = 0 To contour.Size - 1
                Dim point = contour(j)
                If point.X < minX Then minX = point.X
                If point.X > maxX Then maxX = point.X
                If point.Y < minY Then minY = point.Y
                If point.Y > maxY Then maxY = point.Y
                pts(j) = point
            Next
            width = maxX - minX
            height = maxY - minY
            roundness = Math.Round(4 * Math.PI * area / (perimeter * perimeter), 2)

            Dim perVsAreaRatio = perimeter / area

            If roundness > minRound And roundness < maxRound And perVsAreaRatio > minPerAreaRatio And perVsAreaRatio < maxPerAreaRatio Then
                Obj.Area = Math.Round(area, 2)
                Obj.Perimeter = Math.Round(perimeter, 2)
                Obj.height = height
                Obj.Width = width
                Obj.topLeft = New PointF(minX / CDbl(colorImage.Width), minY / CDbl(colorImage.Height))
                Obj.rightBottom = New PointF(maxX / CDbl(colorImage.Width), maxY / CDbl(colorImage.Height))
                Obj.roundness = roundness

                ObjList.Add(Obj)

                ResultImg.FillConvexPoly(pts, New Bgr(0, 0, 255), LineType.EightConnected)
            Else
                ResultImg.FillConvexPoly(pts, New Bgr(255, 0, 0), LineType.EightConnected)
            End If

            'colorImage.DrawPolyline(pts, True, New Bgr(0, 0, 255))

        Next

        Return ResultImg
    End Function

    Public Sub DrawLabelForCount(g As Graphics, pic As PictureBox, ObjList As List(Of BlobObj), font As Font)
        Dim graphBrush As SolidBrush = New SolidBrush(Color.Black)
        Dim width = pic.Width
        Dim Height = pic.Height
        For i = 0 To ObjList.Count - 1
            Dim Obj = ObjList(i)
            Dim pos = New Point((Obj.topLeft.X + Obj.rightBottom.X) * width / 2, (Obj.topLeft.Y + Obj.rightBottom.Y) * Height / 2)
            g.DrawString((i + 1).ToString(), font, graphBrush, pos)
        Next

        graphBrush.Dispose()
    End Sub
    Public Sub DrawLabelForCount(pic As PictureBox, ObjList As List(Of BlobObj), font As Font)
        pic.Refresh()
        Dim g = pic.CreateGraphics()
        DrawLabelForCount(g, pic, ObjList, font)
        g.Dispose()

        'Using g = Graphics.FromHwnd(CInt(pic.Handle))
        '    DrawLabelForCount(g, pic, ObjList, font)
        'End Using
    End Sub

    Public Function InvertBinary(BinaryImg As Emgu.CV.Image(Of Gray, Byte)) As Emgu.CV.Image(Of Gray, Byte)
        Dim Scrdata = BinaryImg.Data

        Dim output = BinaryImg.CopyBlank()
        Dim outData = output.Data

        For y = 0 To BinaryImg.Height - 1
            For x = 0 To BinaryImg.Width - 1
                If Scrdata(y, x, 0) > 128 Then
                    outData(y, x, 0) = 0
                Else
                    outData(y, x, 0) = 255
                End If
            Next
        Next

        Return output
    End Function

#End Region

#Region "Grain Measurements"
    Public Sub IdentifyInterCepts(scr As Emgu.CV.Image(Of Gray, Byte), Obj As SegObject, Optional left As Single = 0, Optional top As Single = 0, Optional right As Single = 0, Optional bottom As Single = 0)
        Dim tempimg = scr.Copy()
        CvInvoke.MedianBlur(scr, scr, 3)
        CvInvoke.Canny(scr, tempimg, 100, 200)

        Dim gray_data = tempimg.Data
        Dim ceptObj = Obj.ceptObj

        For i = 0 To 100
            ceptObj.CeptCnt(i) = 0
            For j = 0 To 1000
                ceptObj.CeptPos(i, j).X = 0
                ceptObj.CeptPos(i, j).Y = 0
            Next
        Next

        Dim startX, startY, endX, endY As Integer
        If left = 0 And right = 0 And top = 0 And bottom = 0 Then
            startX = 0 : startY = 0 : endX = scr.Width - 1 : endY = scr.Height - 1
        Else
            startX = left * scr.Width
            startY = top * scr.Height
            endX = right * scr.Width
            endY = bottom * scr.Height
        End If

        Dim Width = scr.Width
        Dim Height = scr.Height
        If 0 <= ceptObj.Degree And ceptObj.Degree <= Math.PI / 2 Then
            Dim extra_Y_axis = Width * Math.Tan(ceptObj.Degree)
            Dim total_Y_axis = Height + extra_Y_axis
            Dim step_y As Integer = total_Y_axis / (ceptObj.numLine + 1)


            For i = 0 To ceptObj.numLine - 1

                Dim x1 = 0
                Dim y1 = step_y * (i + 1)
                Dim x2 = Width - 1
                Dim y2 = y1 - extra_Y_axis
                For x = startX To endX
                    Dim y As Integer = y1 - (x - x1) * Math.Tan(ceptObj.Degree)
                    If y < startY Or y >= endY Then
                        Continue For
                    End If
                    If gray_data(y, x, 0) > 0 Then
                        Dim pos = New PointF(x / CDbl(Width), y / CDbl(Height))
                        Obj.ceptObj.CeptPos(i, Obj.ceptObj.CeptCnt(i)) = pos
                        Obj.ceptObj.CeptCnt(i) += 1
                        x += 15
                    End If
                Next
            Next
        Else
            Dim extra_X_axis = Height * Math.Tan(Math.PI / 2 - ceptObj.Degree)
            Dim total_X_axis = Width + extra_X_axis
            Dim step_x As Integer = total_X_axis / (ceptObj.numLine + 1)


            For i = 0 To ceptObj.numLine - 1

                Dim x1 = step_x * (i + 1)
                Dim y1 = 0
                Dim x2 = x1 - extra_X_axis
                Dim y2 = Height - 1
                For y = startY To endY
                    Dim x As Integer = x1 - (y - y1) * Math.Tan(Math.PI / 2 - ceptObj.Degree)
                    If x < startX Or x >= endX Then
                        Continue For
                    End If
                    If gray_data(y, x, 0) > 0 Then
                        Dim pos = New PointF(x / CDbl(Width), y / CDbl(Height))
                        Obj.ceptObj.CeptPos(i, Obj.ceptObj.CeptCnt(i)) = pos
                        Obj.ceptObj.CeptCnt(i) += 1
                        y += 15
                    End If
                Next

            Next
        End If

    End Sub

    Public Sub IdentifyCircularInterCepts(scr As Emgu.CV.Image(Of Gray, Byte), Obj As SegObject, Optional left As Single = 0, Optional top As Single = 0, Optional right As Single = 0, Optional bottom As Single = 0)
        Dim tempimg = scr.Copy()

        CvInvoke.MedianBlur(scr, scr, 3)
        CvInvoke.Canny(scr, tempimg, 100, 200)
        Dim gray_data = tempimg.Data
        Dim ceptObj = Obj.ceptObj

        For i = 0 To 100
            ceptObj.CeptCnt(i) = 0
            For j = 0 To 1000
                ceptObj.CeptPos(i, j).X = 0
                ceptObj.CeptPos(i, j).Y = 0
            Next
        Next

        ceptObj.numLine = 4

        Dim Width = scr.Width
        Dim Height = scr.Height

        Dim startX, startY, endX, endY As Integer
        If left = 0 And right = 0 And top = 0 And bottom = 0 Then
            startX = 0 : startY = 0 : endX = scr.Width - 1 : endY = scr.Height - 1
        Else
            startX = left * Width
            startY = top * Height
            endX = right * Width
            endY = bottom * Height
        End If

        Dim MinLen = Math.Min(Width, Height)
        Dim dis As Integer = MinLen / 8

        'Draw 3 circles
        For i = 1 To 3
            Dim diameter = dis * i
            Dim x1 As Integer = 0
            Dim x2 As Integer = 179

            For deg = x1 To x2
                Dim rad = deg / 360 * Math.PI * 2
                Dim x = Width / 2 - diameter * Math.Cos(rad)
                Dim y_delta = Math.Sqrt(diameter * diameter - CInt(x - (Width / 2)) * CInt(x - (Width / 2)))
                Dim y1 = Height / 2 + y_delta

                If x < startX Or x > endX Or y1 < startY Or y1 > endY Then
                    Continue For
                End If
                If gray_data(y1, x, 0) > 0 Then
                    Dim pos = New PointF(x / CDbl(Width), y1 / CDbl(Height))
                    Obj.ceptObj.CeptPos(i, Obj.ceptObj.CeptCnt(i)) = pos
                    Obj.ceptObj.CeptCnt(i) += 1
                    deg += 5
                End If

            Next

            For deg = x1 To x2
                Dim rad = deg / 360 * Math.PI * 2
                Dim x = Width / 2 - diameter * Math.Cos(rad)
                Dim y_delta = Math.Sqrt(diameter * diameter - CInt(x - (Width / 2)) * CInt(x - (Width / 2)))
                Dim y2 = Height / 2 - y_delta

                If x < startX Or x > endX Or y2 < startY Or y2 > endY Then
                    Continue For
                End If
                If gray_data(y2, x, 0) > 0 Then
                    Dim pos = New PointF(x / CDbl(Width), y2 / CDbl(Height))
                    Obj.ceptObj.CeptPos(i, Obj.ceptObj.CeptCnt(i)) = pos
                    Obj.ceptObj.CeptCnt(i) += 1
                    deg += 5
                End If
            Next
        Next
    End Sub

    Public Sub DrawCircles(ByVal g As Graphics, ByVal pictureBox As PictureBox)
        Dim Pen As Pen = New Pen(Color.Blue, 1)

        Dim width = pictureBox.Width
        Dim height = pictureBox.Height

        Dim MinLen = Math.Min(width, height)
        Dim dis As Integer = MinLen / 8

        For i = 1 To 3
            Dim dia = dis * i
            g.DrawEllipse(Pen, New Rectangle(width / 2 - dia, height / 2 - dia, 2 * dia, 2 * dia))
        Next

        Pen.Dispose()
    End Sub

    Public Sub DrawCircles(ByVal pictureBox As PictureBox)
        Dim g As Graphics = pictureBox.CreateGraphics()
        DrawCircles(g, pictureBox)
        g.Dispose()
    End Sub

    Public Sub DrawLines(ByVal g As Graphics, ByVal pictureBox As PictureBox, ByRef Dst As InterCeptObj, flag As Boolean, Optional Src As InterCeptObj = Nothing, Optional left As Single = 0, Optional top As Single = 0, Optional right As Single = 0, Optional bottom As Single = 0)
        Dim Pen As Pen = New Pen(Color.Blue, 1)

        Dim width = pictureBox.Width
        Dim height = pictureBox.Height

        For i = 0 To 100
            Dst.lineLength(i) = 0
        Next
        If Dst.numLine = 0 Then
            Return
        End If

        Dim startX, startY, endX, endY As Integer
        If left = 0 And right = 0 And top = 0 And bottom = 0 Then
            startX = 0 : startY = 0 : endX = width - 1 : endY = height - 1
        Else
            startX = left * width
            startY = top * height
            endX = right * width
            endY = bottom * height
        End If

        If 0 <= Dst.Degree And Dst.Degree <= Math.PI / 4 Then
            Dim extra_Y_axis = width * Math.Tan(Dst.Degree)
            Dim total_Y_axis = height + extra_Y_axis
            Dim step_y As Integer = total_Y_axis / (Dst.numLine + 1)

            For i = 0 To Dst.numLine - 1

                Dim x1 = startX
                Dim y1 = step_y * (i + 1)
                Dim x2 = endX
                Dim y2 = y1 - extra_Y_axis
                Dim xP1 = startX
                Dim yP1 = GetYCoordinate(Dst.Degree, 0, -y1, xP1)
                If yP1 = 99999 Then yP1 = height / (Dst.numLine + 1) * i
                If yP1 > startY Then
                    If startY = 0 Or yP1 < endY Then
                        Dim xP2 = endX
                        Dim yP2 = GetYCoordinate(Dst.Degree, 0, -y1, xP2)
                        If yP2 = 99999 Then yP2 = height / (Dst.numLine + 1) * i
                        If yP2 < startY Then
                            yP2 = startY
                            xP2 = GetXCoordinate(Dst.Degree, 0, -y1, yP2)
                            If xP2 = 99999 Then xP2 = endX
                        End If
                        Dst.lineLength(i) = CalcDistBetweenPoints(xP1, yP1, xP2, yP2)

                        g.DrawLine(Pen, xP1, yP1, xP2, yP2)
                    End If
                End If

                If i > Dst.numLine / 2 Then
                    Dst.lineLength(i) = Dst.lineLength(Dst.numLine - i - 1)
                End If

                If flag Then
                    For x = x1 To x2
                        Dim y As Integer = y1 - (x - x1) * Math.Tan(Dst.Degree)
                        Dim xf = x / CSng(width)
                        Dim yf = y / CSng(height)
                        If y < 0 Or y >= height Then
                            Continue For
                        End If
                        For num = 0 To Src.CeptCnt(0) - 1
                            If Src.CeptPos(0, num).X > xf - 0.01 And Src.CeptPos(0, num).X < xf + 0.01 And Src.CeptPos(0, num).Y > yf - 0.01 And Src.CeptPos(0, num).Y < yf + 0.01 Then
                                Dst.CeptPos(i, Dst.CeptCnt(i)) = Src.CeptPos(0, num)
                                Dst.CeptCnt(i) += 1
                                Dim bias As Integer = 0.02 * width
                                x += bias
                            End If
                        Next
                    Next
                End If

            Next
        Else
            Dim extra_X_axis = height * Math.Tan(Math.PI / 2 - Dst.Degree)
            Dim total_X_axis = width + extra_X_axis
            Dim step_x As Integer = total_X_axis / (Dst.numLine + 1)

            For i = 0 To Dst.numLine - 1

                Dim x1 = step_x * (i + 1)
                Dim y1 = startY
                Dim x2 = x1 - extra_X_axis
                Dim y2 = endY
                Dim yP1 = startY
                Dim xP1 = GetXCoordinate(Dst.Degree, x1, 0, yP1)
                If xP1 = 99999 Then xP1 = width / (Dst.numLine + 1) * i
                If xP1 > startX Then
                    If startX = 0 Or xP1 < endX Then
                        Dim yP2 = endY
                        Dim xP2 = GetXCoordinate(Dst.Degree, x1, 0, yP2)
                        If xP2 = 99999 Then xP2 = width / (Dst.numLine + 1) * i
                        If xP2 < startX Then
                            xP2 = startX
                            yP2 = GetYCoordinate(Dst.Degree, x1, 0, xP2)
                            If yP2 = 99999 Then yP2 = endY
                        End If
                        Dst.lineLength(i) = CalcDistBetweenPoints(xP1, yP1, xP2, yP2)
                        g.DrawLine(Pen, xP1, yP1, xP2, yP2)
                    End If
                End If

                If i > Dst.numLine / 2 Then
                    Dst.lineLength(i) = Dst.lineLength(Dst.numLine - i - 1)
                End If

                If flag Then
                    For y = y1 To y2
                        Dim x As Integer = x1 - (y - y1) * Math.Tan(Math.PI / 2 - Dst.Degree)
                        Dim xf = x / CSng(width)
                        Dim yf = y / CSng(height)
                        If x < 0 Or x >= width Then
                            Continue For
                        End If
                        For num = 0 To Src.CeptCnt(0) - 1
                            If Src.CeptPos(0, num).X > xf - 0.01 And Src.CeptPos(0, num).X < xf + 0.01 And Src.CeptPos(0, num).Y > yf - 0.01 And Src.CeptPos(0, num).Y < yf + 0.01 Then
                                Dst.CeptPos(i, Dst.CeptCnt(i)) = Src.CeptPos(0, num)
                                Dst.CeptCnt(i) += 1
                                Dim bias As Integer = 0.02 * height
                                y += bias
                            End If
                        Next
                    Next
                End If

            Next
        End If

        Pen.Dispose()
    End Sub
    Public Sub DrawLines(ByVal pictureBox As PictureBox, ByRef Dst As InterCeptObj, flag As Boolean, Optional Src As InterCeptObj = Nothing, Optional left As Single = 0, Optional top As Single = 0, Optional right As Single = 0, Optional bottom As Single = 0)
        Dim g As Graphics = pictureBox.CreateGraphics()
        DrawLines(g, pictureBox, Dst, flag, Src, left, top, right, bottom)
        g.Dispose()
    End Sub

    Public Sub DrawInterCepts(ByVal g As Graphics, ByVal pictureBox As PictureBox, ceptObj As InterCeptObj)
        Dim Pen As Pen = New Pen(Color.Black, 1)

        Dim width = pictureBox.Width
        Dim height = pictureBox.Height

        For i = 0 To 100
            For j = 0 To ceptObj.CeptCnt(i)
                Dim x = ceptObj.CeptPos(i, j).X * width
                Dim y = ceptObj.CeptPos(i, j).Y * height
                g.DrawEllipse(Pen, New Rectangle(x - 4, y - 4, 8, 8))
            Next
        Next

        Pen.Dispose()
    End Sub
    Public Sub DrawInterCepts(ByVal pictureBox As PictureBox, Obj As InterCeptObj)
        pictureBox.Refresh()
        Dim g As Graphics = pictureBox.CreateGraphics()
        DrawInterCepts(g, pictureBox, Obj)
        g.Dispose()
    End Sub

    ''' <summary>
    ''' Identify Grains.
    ''' </summary>
    ''' <paramname="scr">The source image.</param>
    ''' <paramname="BinaryImg">The binary image.</param>
    ''' <paramname="ObjList">The object to contain information for blobs.</param>
    Public Function GrainDetection(colorImage As Emgu.CV.Image(Of Bgr, Byte), BinaryImg As Emgu.CV.Image(Of Gray, Byte), ObjList As List(Of BlobObj), colList As List(Of Integer()), Optional GrainNo As Integer = 99) As Emgu.CV.Image(Of Bgr, Byte)

        Dim contours As New VectorOfVectorOfPoint()

        CvInvoke.FindContours(BinaryImg, contours, Nothing, RetrType.Ccomp, ChainApproxMethod.ChainApproxSimple)
        Dim totalArea = colorImage.Width * colorImage.Height
        Dim maxArea = 3000

        Dim ResultImg = colorImage.Copy()
        Dim perimeter, area, circularity, width, height, roundness As Double
        Dim minX, maxX, minY, maxY As Integer
        Dim Obj As BlobObj = New BlobObj
        For i = 0 To contours.Size - 1
            Dim contour = contours(i)
            perimeter = CvInvoke.ArcLength(contour, True)
            area = CvInvoke.ContourArea(contour)
            Dim rect As Rectangle = CvInvoke.BoundingRectangle(contour)
            Dim GrayCrop = BinaryImg.Copy()
            GrayCrop.ROI = rect
            Dim maskCrop = GrayCrop.CopyBlank()
            Dim pts As Point() = New Point(contour.Size - 1) {}

            For j = 0 To contour.Size - 1
                Dim point = contour(j)
                point.X = point.X - rect.X
                point.Y = point.Y - rect.Y
                pts(j) = point
            Next

            maskCrop.FillConvexPoly(pts, New Gray(255), LineType.EightConnected)
            'CvInvoke.Imshow("mask", maskCrop)
            'CvInvoke.WaitKey(0)

            Dim gray_data = GrayCrop.Data
            Dim mask_data = maskCrop.Data
            Dim intensitySum As Integer = 0
            ' Assign color to each
            For y = 0 To GrayCrop.Height - 1
                For x = 0 To GrayCrop.Width - 1
                    If mask_data(y, x, 0) = 255 Then
                        intensitySum += gray_data(y, x, 0)
                    End If
                Next
            Next

            GrayCrop.Dispose()
            maskCrop.Dispose()

            If area > maxArea Or intensitySum = 0 Then
                Continue For
            End If
            circularity = Math.Pow(perimeter, 2) / (4 * Math.PI * area)
            minX = 9999999
            minY = 9999999
            maxX = 0
            maxY = 0

            For j = 0 To contour.Size - 1
                Dim point = contour(j)
                If point.X < minX Then minX = point.X
                If point.X > maxX Then maxX = point.X
                If point.Y < minY Then minY = point.Y
                If point.Y > maxY Then maxY = point.Y
                pts(j) = point
            Next
            width = maxX - minX
            height = maxY - minY
            roundness = Math.Round(4 * Math.PI * area / (perimeter * perimeter), 2)

            Dim perVsAreaRatio = perimeter / area

            Obj.Area = Math.Round(area, 2)
            Obj.Perimeter = Math.Round(perimeter, 2)
            Obj.height = height
            Obj.Width = width
            Obj.topLeft = New PointF(minX / CDbl(colorImage.Width), minY / CDbl(colorImage.Height))
            Obj.rightBottom = New PointF(maxX / CDbl(colorImage.Width), maxY / CDbl(colorImage.Height))
            Obj.roundness = roundness

            ObjList.Add(Obj)

            Dim No = GetGrainNo(area)
            If GrainNo = 99 Or No = GrainNo Then
                Dim CurCol = colList(No + 1)
                ResultImg.FillConvexPoly(pts, New Bgr(CurCol(0), CurCol(1), CurCol(2)), LineType.EightConnected)
            End If

        Next

        Return ResultImg
    End Function

#End Region

#Region "Density"
    ''' <summary>
    ''' Segment using Otsu and calculate black and white region.
    ''' </summary>
    ''' <paramname="gray">The original image.</param>
    ''' <paramname="white">The percentage of white region.</param>
    ''' <paramname="black">The percentage of black region.</param>

    Public Sub CalcBlackAndWhite(ByVal gray As Emgu.CV.Image(Of Emgu.CV.Structure.Gray, Byte), ByRef white As Single, ByRef black As Single)
        Dim binary = gray.CopyBlank()
        CvInvoke.Threshold(gray, binary, 0, 255, ThresholdType.Otsu)
        Dim total = gray.Width * gray.Height
        Dim scalarBlack = CvInvoke.Sum(binary)
        'CvInvoke.Imshow("bin", binary)
        Dim WhitePixel As Integer = scalarBlack.V0 / 255
        white = WhitePixel / CSng(total) * 100
        black = 100 - white
    End Sub

    ''' <summary>
    ''' Segment using Otsu and calculate black and white region.
    ''' </summary>
    ''' <paramname="gray">The original image.</param>
    ''' <paramname="white">The percentage of white region.</param>
    ''' <paramname="black">The percentage of black region.</param>

    Public Sub CalcBlackAndWhite(ByVal gray As Emgu.CV.Image(Of Emgu.CV.Structure.Gray, Byte), CurveObj As CurveObj, ByRef white As Single, ByRef black As Single)
        Dim binary = gray.CopyBlank()
        Dim mask = binary.CopyBlank()
        CvInvoke.Threshold(gray, binary, 0, 255, ThresholdType.Otsu)

        Dim PtVector As New VectorOfPoint()
        Dim PtArray As Point() = New Point(CurveObj.CPointIndx) {}
        For i = 0 To CurveObj.CPointIndx
            Dim Pt = New Point()
            Pt.X = CurveObj.CurvePoint(i).X * gray.Width
            Pt.Y = CurveObj.CurvePoint(i).Y * gray.Height
            PtArray(i) = Pt
        Next
        PtVector.Push(PtArray)
        Dim total = CvInvoke.ContourArea(PtVector)

        mask.FillConvexPoly(PtArray, New Gray(255), LineType.EightConnected)
        'CvInvoke.Imshow("mask", mask)
        'CvInvoke.WaitKey(0)

        Dim gray_data = binary.Data
        Dim mask_data = mask.Data
        Dim intensitySum As Integer = 0
        ' Assign color to each
        For y = 0 To binary.Height - 1
            For x = 0 To binary.Width - 1
                If mask_data(y, x, 0) = 255 Then
                    intensitySum += gray_data(y, x, 0)
                End If
            Next
        Next

        Dim whitePixel As Integer = intensitySum / 255
        white = whitePixel / CSng(total) * 100
        black = 100 - white

        PtVector.Dispose()
        binary.Dispose()
        mask.Dispose()
    End Sub
#End Region

End Module

