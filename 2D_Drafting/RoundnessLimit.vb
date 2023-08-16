Imports Emgu.CV
Imports Emgu.CV.Dnn
Imports Emgu.CV.Structure

Public Class RoundnessLimit
    Public OriImage As Emgu.CV.Image(Of Bgr, Byte)
    Public BinaryImage As Emgu.CV.Image(Of Gray, Byte)
    Public RoundUpper As Single
    Public RoundLower As Single
    Public AreaLimit As Single
    Public PerVsAreaRatioLower As Single
    Public PerVsAreaRatioUpper As Single
    Public ObjList As List(Of BlobObj) = New List(Of BlobObj)
    Public DistingshType As Boolean

    Public Sub New()
        InitializeComponent()
    End Sub

    Public Sub New(BinImg As Emgu.CV.Image(Of Gray, Byte), MinArea As Single, DistType As Boolean)
        InitializeComponent()
        BinaryImage = BinImg.Clone()
        AreaLimit = MinArea
        DistingshType = DistType
        If DistingshType = False Then
            Me.Text = "Roundness Limit"
        Else
            Me.Text = "Perimeter/Area Limit"
        End If
    End Sub

    Private Sub RoundnessLimit_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            ObjList.Clear()
            Dim scr = Main_Form.originImageList(Main_Form.tab_Index).ToBitmap()
            Dim bmpImage As Bitmap = New Bitmap(scr)
            OriImage = bmpImage.ToImage(Of Bgr, Byte)()
            bmpImage.Dispose()
        Catch ex As Exception

        End Try

    End Sub

    Public Sub DrawResult()
        ObjList.Clear()
        Dim output As Emgu.CV.Image(Of Bgr, Byte)
        If DistingshType = False Then
            output = BlobDetection(OriImage, BinaryImage, ObjList, AreaLimit, RoundLower, RoundUpper)
        Else
            output = BlobDetection(OriImage, BinaryImage, ObjList, AreaLimit, 0, 1, PerVsAreaRatioLower, PerVsAreaRatioUpper)
        End If

        Dim sz = New Size(Main_Form.resizedImageList(Main_Form.tab_Index).Width, Main_Form.resizedImageList(Main_Form.tab_Index).Height)
        Dim Resized As Mat = New Mat()
        CvInvoke.Resize(output, Resized, sz)
        Main_Form.ID_PICTURE_BOX(Main_Form.tab_Index).Image = Resized.ToBitmap()
        Main_Form.currentImageList(Main_Form.tab_Index) = output.Mat.Clone()
        output.Dispose()
        Resized.Dispose()
    End Sub

    Private Sub TrackUpper_Scroll(sender As Object, e As EventArgs) Handles TrackUpper.Scroll
        RoundUpper = TrackUpper.Value / CSng(100)
        PerVsAreaRatioUpper = RoundUpper
        LabUpper.Text = TrackUpper.Value.ToString()
        DrawResult()
    End Sub

    Private Sub TrackLower_Scroll(sender As Object, e As EventArgs) Handles TrackLower.Scroll
        RoundLower = TrackLower.Value / CSng(100)
        PerVsAreaRatioLower = RoundLower
        LabLower.Text = TrackLower.Value.ToString()
        DrawResult()
    End Sub

    Private Sub RoundnessLimit_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        OriImage.Dispose()
        BinaryImage.Dispose()
    End Sub
End Class