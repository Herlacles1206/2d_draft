Imports Emgu.CV
Imports Emgu.CV.Structure
Imports Microsoft.VisualBasic.Devices

Public Class Intensity
    Public OriImage As Emgu.CV.Image(Of Bgr, Byte)
    Public GrayImage As Emgu.CV.Image(Of Gray, Byte)
    Public BinaryImage As Emgu.CV.Image(Of Gray, Byte)
    Public Upper As Integer
    Public Lower As Integer
    Public RoiUse As Boolean


    Public Sub New(Optional _UseRoi As Boolean = False)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        RoiUse = _UseRoi
    End Sub
    Private Sub Intensity_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim scr = Main_Form.originImageList(Main_Form.tab_Index).Clone().ToBitmap()
        Dim bmpImage As Bitmap = New Bitmap(scr)
        OriImage = bmpImage.ToImage(Of Bgr, Byte)()
        bmpImage.Dispose()
        scr.Dispose()
        GrayImage = getGrayScale(OriImage)
        BinaryImage = GrayImage.CopyBlank()
    End Sub

    Public Sub DrawResult()
        BinaryImage = GetBinaryWith2Thr(GrayImage, Lower, Upper)

        Dim output As Emgu.CV.Image(Of Bgr, Byte)
        If RoiUse Then
            Dim left = Main_Form.RoiRect.X
            Dim right = (Main_Form.RoiRect.X + Main_Form.RoiRect.Width)
            Dim top = Main_Form.RoiRect.Y
            Dim bottom = (Main_Form.RoiRect.Y + Main_Form.RoiRect.Height)

            output = OverLapSegToOri(OriImage, BinaryImage, left, top, right, bottom)
        Else
            output = OverLapSegToOri(OriImage, BinaryImage)
        End If

        Dim sz = New Size(Main_Form.resizedImageList(Main_Form.tab_Index).Width, Main_Form.resizedImageList(Main_Form.tab_Index).Height)
        Dim Resized As Mat = New Mat()
        CvInvoke.Resize(outPut, Resized, sz)

        Main_Form.ID_PICTURE_BOX(Main_Form.tab_Index).Image = Resized.ToBitmap()
        output.Dispose()
        Resized.Dispose()
    End Sub
    Private Sub TrackBar1_Scroll(sender As Object, e As EventArgs) Handles TrackBar1.Scroll
        Upper = TrackBar1.Value
        LabUpper.Text = Upper.ToString()
        DrawResult()
    End Sub

    Private Sub TrackBar2_Scroll(sender As Object, e As EventArgs) Handles TrackBar2.Scroll
        Lower = TrackBar2.Value
        LabLower.Text = Lower.ToString()
        DrawResult()
    End Sub

    Private Sub Intensity_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        OriImage.Dispose()
        GrayImage.Dispose()
        BinaryImage.Dispose()
    End Sub
End Class