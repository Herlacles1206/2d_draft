Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports Emgu.CV
Imports Emgu.CV.Structure

Public Class PhaseSegmentASTM
    Public OriImage As Emgu.CV.Image(Of Bgr, Byte)
    Public GrayImage As Emgu.CV.Image(Of Gray, Byte)

    Public Shared CaliName As List(Of String) = New List(Of String)
    Public Shared PhaseNameList As List(Of List(Of String)) = New List(Of List(Of String))
    Public PhaseName As List(Of String) = New List(Of String)
    Public Shared PhaseValList As List(Of List(Of Integer)) = New List(Of List(Of Integer))
    Public PhaseVal As List(Of Integer) = New List(Of Integer)
    Public PhaseCol As List(Of Integer()) = New List(Of Integer())
    Private PhaseSel As List(Of Integer) = New List(Of Integer)
    Public PhaseArea As List(Of Integer) = New List(Of Integer)
    Private PhaseNum As Integer

    Private Sub PhaseSegmentASTM_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            PhaseNum = CaliName.Count
            Timer1.Interval = 30
            Timer1.Start()
            CaliName = Main_Form.CaliName.ToList()
            PhaseNameList = Main_Form.PhaseNameList.ToList()
            PhaseValList = Main_Form.PhaseValList.ToList()
            Dim scr = Main_Form.originImageList(Main_Form.tab_Index).ToBitmap()
            Dim bmpImage As Bitmap = New Bitmap(scr)
            OriImage = bmpImage.ToImage(Of Bgr, Byte)()
            bmpImage.Dispose()
            GrayImage = getGrayScale(OriImage)
            HistogramBox1.GenerateHistograms(GrayImage, 256)
            HistogramBox1.Refresh()

            For i = 0 To CaliName.Count - 1
                ComboCali.Items.Add(CaliName(i))
            Next
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub PrviewSegmentation()
        Dim flag As Boolean = False
        Dim FirstPt = New Point(0, 0)
        Dim SecondPt = New Point(0, 0)
        PhaseSel.Clear()
        For i = 0 To PhaseCol.Count - 1
            PhaseSel.Add(1)
        Next
        Dim output = MultiSegment(OriImage, PhaseVal, PhaseCol, PhaseArea, PhaseSel, FirstPt, SecondPt, flag)
        Dim sz = New Size(Main_Form.resizedImageList(Main_Form.tab_Index).Width, Main_Form.resizedImageList(Main_Form.tab_Index).Height)
        Dim Resized As Mat = New Mat()
        CvInvoke.Resize(output, Resized, sz)

        Main_Form.ID_PICTURE_BOX(Main_Form.tab_Index).Image = Resized.ToBitmap()
        Main_Form.currentImageList(Main_Form.tab_Index) = output.Mat.Clone()
        output.Dispose()
    End Sub

    Private Sub LoadDataToGridView()
        DataGridView1.Rows.Clear()
        Dim str_item = New String(2) {}
        If Main_Form.originImageList(Main_Form.tab_Index) Is Nothing Then
            Return
        End If
        Dim squrare = Main_Form.originImageList(Main_Form.tab_Index).Width * Main_Form.originImageList(Main_Form.tab_Index).Height

        For i = 0 To PhaseCol.Count - 1
            str_item(0) = PhaseName(i)
            str_item(1) = PhaseArea(i).ToString
            str_item(2) = Math.Round(CDbl(PhaseArea(i) / squrare) * 100, 2).ToString()

            DataGridView1.Rows.Add(str_item)
        Next
    End Sub
    Private Sub DrawResult()
        DrawProcess(PicBoxProgress, PhaseVal, PhaseCol)
        PrviewSegmentation()
        LoadDataToGridView()
    End Sub
    Private Sub ComboCali_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboCali.SelectedIndexChanged
        Dim index = ComboCali.SelectedIndex
        If index < 0 Then Return
        PhaseName.Clear()
        PhaseVal.Clear()
        PhaseCol.Clear()
        PhaseName = PhaseNameList(index).ToList()
        PhaseVal = PhaseValList(index).ToList()
        For i = 0 To PhaseVal.Count - 2
            PhaseCol.Add(Main_Form.colList(i + 1))
        Next
        DrawResult()
    End Sub

    Private Sub BtnAdd_Click(sender As Object, e As EventArgs) Handles BtnAdd.Click
        PhaseNum = CaliName.Count
        Dim form = New AddCalibration
        form.Show()

    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        If PhaseNum <> CaliName.Count Then
            ComboCali.Items.Clear()
            For i = 0 To CaliName.Count - 1
                ComboCali.Items.Add(CaliName(i))
            Next
            PhaseNum = CaliName.Count
        End If
    End Sub

    Private Sub BtnReport_Click(sender As Object, e As EventArgs) Handles BtnReport.Click
        Dim filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*"
        Dim title = "Save"
        SaveListToReport(Main_Form.ID_PICTURE_BOX(Main_Form.tab_Index).Image, DataGridView1, filter, title)
    End Sub

    Private Sub BtnExcel_Click(sender As Object, e As EventArgs) Handles BtnExcel.Click
        Dim filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*"
        Dim title = "Save"
        SaveListToExcel(DataGridView1, filter, title)
    End Sub

    Private Sub BtnExit_Click(sender As Object, e As EventArgs) Handles BtnExit.Click
        Me.Close()
    End Sub
End Class