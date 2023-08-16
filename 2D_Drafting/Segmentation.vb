Imports System.Runtime.Serialization.Formatters
Imports Emgu.CV
Imports Emgu.CV.Dnn
Imports Emgu.CV.Structure
Imports Microsoft.VisualBasic.Devices

Public Class Segmentation
    Public OriImage As Emgu.CV.Image(Of Bgr, Byte)
    Public ResImage As Emgu.CV.Image(Of Bgr, Byte)
    Public BinaryImage As Emgu.CV.Image(Of Gray, Byte)
    Public ChannelImages As Emgu.CV.Image(Of Gray, Byte)()

    Private ColorRed As Integer = 1
    Private ColorGreen As Integer = 2
    Private ColorBlue As Integer = 3
    Private SelectedColor As Integer

    Private MinThres As Integer
    Private MaxThres As Integer

    Private DGV As New DataGridView

    Private Sub Segmentation_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            Dim scr = Main_Form.originImageList(Main_Form.tab_Index).ToBitmap()
            Dim bmpImage As Bitmap = New Bitmap(scr)
            OriImage = bmpImage.ToImage(Of Bgr, Byte)()
            bmpImage.Dispose()
            ResImage = OriImage.Clone()

            ChannelImages = OriImage.Split()
            Pic1.Image = OriImage.ToBitmap()
            Pic2.Image = ResImage.ToBitmap()

            DGV.Columns.Add("Min", "Min")
            DGV.Columns.Add("Max", "Max")
            DGV.Columns.Add("Area", "Area")

            TxtArea.Text = "0"
            TxtMax.Text = "0"
            TxtMin.Text = "0"
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub BtnRed_Click(sender As Object, e As EventArgs) Handles BtnRed.Click
        SelectedColor = ColorRed
        BtnColor.BackColor = Color.Red
        LabColor.Text = "Red"
    End Sub

    Private Sub BtnGreen_Click(sender As Object, e As EventArgs) Handles BtnGreen.Click
        SelectedColor = ColorGreen
        BtnColor.BackColor = Color.Green
        LabColor.Text = "Green"
    End Sub

    Private Sub BtnBlue_Click(sender As Object, e As EventArgs) Handles BtnBlue.Click
        SelectedColor = ColorBlue
        BtnColor.BackColor = Color.Blue
        LabColor.Text = "Blue"
    End Sub

    Private Sub DisplayResultImage()
        If SelectedColor = ColorBlue Then
            BinaryImage = GetBinaryWith2Thr(ChannelImages(0), MinThres, MaxThres)
            ResImage = OverLapSegToOri(OriImage, BinaryImage, 0, 0, 0, 0, "Blue")
        ElseIf SelectedColor = ColorGreen Then
            BinaryImage = GetBinaryWith2Thr(ChannelImages(1), MinThres, MaxThres)
            ResImage = OverLapSegToOri(OriImage, BinaryImage, 0, 0, 0, 0, "Green")
        ElseIf SelectedColor = ColorRed Then
            BinaryImage = GetBinaryWith2Thr(ChannelImages(2), MinThres, MaxThres)
            ResImage = OverLapSegToOri(OriImage, BinaryImage, 0, 0, 0, 0, "Red")
        End If

        Dim total = BinaryImage.Width * BinaryImage.Height
        Dim scalarBlack = CvInvoke.Sum(BinaryImage)
        'CvInvoke.Imshow("bin", binary)
        Dim WhitePixel As Integer = scalarBlack.V0 / 255
        Dim white = WhitePixel / CSng(total) * 100
        TxtArea.Text = white.ToString()

        Pic2.Image = ResImage.ToBitmap()
    End Sub
    Private Sub BarMin_Scroll(sender As Object, e As EventArgs) Handles BarMin.Scroll
        MinThres = BarMin.Value
        TxtMin.Text = MinThres.ToString()
        DisplayResultImage()
    End Sub

    Private Sub BarMax_Scroll(sender As Object, e As EventArgs) Handles BarMax.Scroll
        MaxThres = BarMax.Value
        TxtMax.Text = MaxThres.ToString()
        DisplayResultImage()
    End Sub

    Private Sub LoadDataToDGV()
        DGV.Rows.Clear()
        Dim str_item = New String(3) {}
        str_item(0) = TxtMin.Text
        str_item(1) = TxtMax.Text
        str_item(2) = TxtArea.Text
        DGV.Rows.Add(str_item)

    End Sub
    Private Sub BtnReport_Click(sender As Object, e As EventArgs) Handles BtnReport.Click
        LoadDataToDGV()
        Dim filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*"
        Dim title = "Save"
        SaveListToReport(Pic2.Image, DGV, filter, title)
    End Sub
End Class