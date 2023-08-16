Imports Emgu.CV
Imports System.Threading
Imports Emgu.CV.Structure
Imports System.Windows.Forms.VisualStyles.VisualStyleElement

Public Class AddCalibration
    Public OriImage As Emgu.CV.Image(Of Bgr, Byte)
    Public GrayImage As Emgu.CV.Image(Of Gray, Byte)
    Public PhaseNum As Integer
    Public PhaseName As List(Of String) = New List(Of String)
    Public PhaseVal As List(Of Integer) = New List(Of Integer)
    Public PhaseCol As List(Of Integer()) = New List(Of Integer())
    Public PhaseArea As List(Of Integer) = New List(Of Integer)
    Private PhaseSel As List(Of Integer) = New List(Of Integer)

    Private MouseDownflag As Boolean
    Private CurSelPhaseLine As Integer
    Private CurSelPhase As Integer
    Private MouseClicked As Integer
    Private IntialPhaseVal As Integer

    Public Saved As Boolean
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

        For i = 0 To PhaseCol.Count - 1
            str_item(0) = PhaseName(i)
            str_item(1) = PhaseVal(i)
            str_item(2) = PhaseVal(i + 1)

            DataGridView1.Rows.Add(str_item)
        Next
    End Sub
    Private Sub DrawResult()
        DrawProcess(PicBoxProgress, PhaseVal, PhaseCol)
        PrviewSegmentation()
        LoadDataToGridView()
    End Sub
    Private Sub AddCalibration_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        PicBoxProgress.Cursor = Cursors.SizeWE
        Dim scr = Main_Form.originImageList(Main_Form.tab_Index).ToBitmap()
        Dim bmpImage As Bitmap = New Bitmap(scr)
        OriImage = bmpImage.ToImage(Of Bgr, Byte)()
        bmpImage.Dispose()
        GrayImage = getGrayScale(OriImage)
        HistogramBox1.GenerateHistograms(GrayImage, 256)
        HistogramBox1.Refresh()

        DrawResult()
    End Sub

    Private Sub BtnNew_Click(sender As Object, e As EventArgs) Handles BtnNew.Click
        Dim cur_col As Integer()
        Dim item As String = TxtPhaseName.Text
        If item = "" Then
            MessageBox.Show("Please enter the phase name")
            Return
        End If

        CurSelPhase = PhaseNum
        Dim SetCol_Copy As Integer() = New Integer(2) {}
        If PhaseNum = 0 Then
            PhaseName.Add(item)
            PhaseVal.Add(0)
            PhaseVal.Add(128)
            cur_col = Main_Form.colList(PhaseNum + 1)
            PhaseCol.Add(cur_col)

            NumMin.Value = 0
            NumMax.Value = 128
        Else
            If PhaseVal(PhaseVal.Count - 1) = 256 Then
                Return
            End If
            PhaseName.Add(item)
            PhaseVal.Add(256)
            cur_col = Main_Form.colList(PhaseNum + 1)
            PhaseCol.Add(cur_col)
            NumMin.Value = CInt(PhaseVal(PhaseVal.Count - 2))
            NumMax.Value = 256

        End If

        PhaseNum += 1
        DrawResult()
        BtnCol.BackColor = Color.FromArgb(PhaseCol(PhaseCol.Count - 1)(2), PhaseCol(PhaseCol.Count - 1)(1), PhaseCol(PhaseCol.Count - 1)(0))
    End Sub

    Private Sub BtnDel_Click(sender As Object, e As EventArgs) Handles BtnDel.Click
        PhaseName.Clear()
        PhaseVal.Clear()
        PhaseCol.Clear()
        PhaseNum = 0
        DrawResult()
        BtnCol.BackColor = Color.White
    End Sub

    Private Sub UpdateNumValue()
        If CurSelPhase = CurSelPhaseLine Then
            NumMin.Value = PhaseVal(CurSelPhaseLine)
            NumMax.Value = PhaseVal(CurSelPhaseLine + 1)
        Else
            NumMin.Value = PhaseVal(CurSelPhaseLine - 1)
            NumMax.Value = PhaseVal(CurSelPhaseLine)
        End If
    End Sub
    Private Sub PicBoxProgress_MouseDown(sender As Object, e As MouseEventArgs) Handles PicBoxProgress.MouseDown
        MouseDownflag = True
        If PhaseName.Count > 0 Then
            MouseClicked = e.X * 256 / PicBoxProgress.Width
            Dim minDis = 256

            For i = 0 To PhaseVal.Count - 1
                If Math.Abs(MouseClicked - PhaseVal(i)) < minDis Then
                    Dim dis = MouseClicked - PhaseVal(i)
                    minDis = Math.Abs(dis)
                    CurSelPhaseLine = i
                    IntialPhaseVal = PhaseVal(i)
                    If dis > 0 And CurSelPhaseLine <> PhaseVal.Count - 1 Then
                        CurSelPhase = i
                    Else
                        CurSelPhase = i - 1
                    End If
                End If
            Next
            If CurSelPhase < 0 Then
                CurSelPhase = 0
            End If

            UpdateNumValue()
        End If
    End Sub

    Private Sub PicBoxProgress_MouseUp(sender As Object, e As MouseEventArgs) Handles PicBoxProgress.MouseUp
        MouseDownflag = False
        MouseClicked = 0
    End Sub

    Private Sub PicBoxProgress_MouseMove(sender As Object, e As MouseEventArgs) Handles PicBoxProgress.MouseMove
        If MouseDownflag And PhaseName.Count > 0 Then
            Dim curPos = e.X * 256 / PicBoxProgress.Width
            Dim delta = curPos - MouseClicked

            PhaseVal(CurSelPhaseLine) = IntialPhaseVal + delta
            If CurSelPhaseLine + 1 < PhaseVal.Count Then
                PhaseVal(CurSelPhaseLine) = Math.Min(PhaseVal(CurSelPhaseLine), PhaseVal(CurSelPhaseLine + 1) - 1)
            Else
                PhaseVal(CurSelPhaseLine) = Math.Min(PhaseVal(CurSelPhaseLine), 256)
            End If
            If CurSelPhaseLine - 1 >= 0 Then
                PhaseVal(CurSelPhaseLine) = Math.Max(PhaseVal(CurSelPhaseLine), PhaseVal(CurSelPhaseLine - 1) + 1)
            Else
                PhaseVal(CurSelPhaseLine) = Math.Max(PhaseVal(CurSelPhaseLine), 0)
            End If

            DrawResult()

            UpdateNumValue()
        End If
    End Sub

    Private Sub NumMin_ValueChanged(sender As Object, e As EventArgs) Handles NumMin.ValueChanged
        PhaseVal(CurSelPhase) = NumMin.Value
        DrawResult()
    End Sub

    Private Sub NumMax_ValueChanged(sender As Object, e As EventArgs) Handles NumMax.ValueChanged
        If PhaseVal.Count > 0 Then
            PhaseVal(CurSelPhase + 1) = NumMax.Value
        End If
        DrawResult()
    End Sub

    Private Sub BtnSave_Click(sender As Object, e As EventArgs) Handles BtnSave.Click
        If TxtCaliName.Text = "" Then
            MessageBox.Show("Please enter calibration name")
            Return
        End If
        PhaseSegmentASTM.CaliName.Add(TxtCaliName.Text)
        PhaseSegmentASTM.PhaseNameList.Add(PhaseName)
        PhaseSegmentASTM.PhaseValList.Add(PhaseVal)

        Main_Form.CaliName.Clear()
        Main_Form.PhaseNameList.Clear()
        Main_Form.PhaseValList.Clear()
        Main_Form.CaliName = PhaseSegmentASTM.CaliName.ToList()
        Main_Form.PhaseNameList = PhaseSegmentASTM.PhaseNameList.ToList()
        Main_Form.PhaseValList = PhaseSegmentASTM.PhaseValList.ToList()
        Me.Close()
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Me.Close()
    End Sub
End Class