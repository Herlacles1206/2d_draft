Public Class Calibration
    Public CFList As List(Of String) = New List(Of String)
    Public CFNum As List(Of Double) = New List(Of Double)
    Public CFUnit As List(Of String) = New List(Of String)
    Public CFSelected As Integer

    Private FirstPtF As PointF
    Private SecondPtF As PointF

    Public Sub New()
        InitializeComponent()
    End Sub

    Public Sub New(_CFList As List(Of String), _CFSelected As Integer, _CFNum As List(Of Double), _CFUnit As List(Of String))
        InitializeComponent()
        CFList = _CFList.ToList()
        CFNum = _CFNum.ToList()
        CFUnit = _CFUnit.ToList()
        CFSelected = _CFSelected
    End Sub

    Private Sub ShowCaliInfor()
        ComboUnit.Text = CFUnit(CFSelected)
        TxtXPixelCnt.Text = "1"
        TxtScaleX.Text = CFNum(CFSelected).ToString
        LabUnit.Text = CFUnit(CFSelected)
    End Sub
    Private Sub Calibration_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Timer1.Interval = 30
        Timer1.Start()
        For i = 0 To CFList.Count - 1
            ComboCF.Items.Add(CFList(i))
        Next
        ComboCF.SelectedIndex = CFSelected
        ShowCaliInfor()
        Main_Form.EdgeRegionDrawReady = True
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        If Main_Form.EdgeRegionDrawReady And Not Main_Form.EdgeRegionDrawed Then
            Main_Form.SecondPtOfEdge.Y = Main_Form.FirstPtOfEdge.Y
            If Math.Abs(Main_Form.SecondPtOfEdge.X - Main_Form.FirstPtOfEdge.X) > 0.01 Then
                FirstPtF = Main_Form.FirstPtOfEdge
                SecondPtF = Main_Form.SecondPtOfEdge
                Dim dist As Integer = (SecondPtF.X - FirstPtF.X) * Main_Form.ID_PICTURE_BOX(Main_Form.tab_Index).Width
                TxtXPixelCnt.Text = dist.ToString()
            End If
        End If
    End Sub
    Private Sub ComboCF_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboCF.SelectedIndexChanged
        CFSelected = ComboCF.SelectedIndex
        ShowCaliInfor()
    End Sub

    Private Sub ComboUnit_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboUnit.SelectedIndexChanged
        LabUnit.Text = ComboUnit.Text
    End Sub


    Private Sub BtnX_Click(sender As Object, e As EventArgs) Handles BtnX.Click
        Main_Form.CalibrationAxis = 1
    End Sub

    Private Sub BtnAdd_Click(sender As Object, e As EventArgs) Handles BtnAdd.Click
        If BtnAdd.Text = "Add" Then
            BtnAdd.Text = "Modify"
        Else
            BtnAdd.Text = "Add"
        End If

        Dim PixelCount = CInt(TxtXPixelCnt.Text)
        Dim ScaleLen = CDbl(TxtScaleX.Text)

        Dim caliName = ComboCF.Text
        Dim flag = False
        Dim index = 0
        For i = 0 To CFList.Count - 1
            If caliName = CFList(i) Then
                flag = True
                index = i
                Exit For
            End If
        Next

        If flag Then
            CFNum(index) = ScaleLen / PixelCount
            CFUnit(index) = ComboUnit.Text
        Else
            CFList.Add(ComboCF.Text)
            CFNum.Add(ScaleLen / PixelCount)
            CFUnit.Add(ComboUnit.Text)
            ComboCF.Items.Add(ComboCF.Text)
        End If
    End Sub

    Private Sub BtnDel_Click(sender As Object, e As EventArgs) Handles BtnDel.Click
        Dim index = ComboCF.SelectedIndex

        If ComboCF.Items.Count > 0 Then
            CFList.RemoveAt(index)
            CFNum.RemoveAt(index)
            CFUnit.RemoveAt(index)
            ComboCF.Items.RemoveAt(index)
            If ComboCF.Items.Count > 0 Then
                ComboCF.SelectedIndex = 0
            Else
                ComboCF.Text = ""
            End If

        End If
    End Sub

    Private Sub BtnOK_Click(sender As Object, e As EventArgs) Handles BtnOK.Click
        Main_Form.CFList.Clear()
        Main_Form.CFNum.Clear()
        Main_Form.CFUnit.Clear()
        Main_Form.CFSelected = ComboCF.SelectedIndex
        Main_Form.CFList = CFList.ToList()
        Main_Form.CFNum = CFNum.ToList()
        Main_Form.CFUnit = CFUnit.ToList()
        Main_Form.CalibrationChanged = True
        Me.Close()
    End Sub

    Private Sub BtnCancel_Click(sender As Object, e As EventArgs) Handles BtnCancel.Click
        Me.Close()
    End Sub

    Private Sub Calibration_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        Main_Form.EdgeRegionDrawReady = False
        Main_Form.CalibrationAxis = 0
    End Sub
End Class