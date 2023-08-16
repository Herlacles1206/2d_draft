Public Class SetROI
    Public RoiSize As Integer
    Public RoiUse As Boolean
    Public RoiUnit As String
    Public Sub New(_RoiSize As Integer, _RoiUnit As String)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        RoiSize = _RoiSize
        RoiUnit = _RoiUnit
    End Sub

    Private Sub SetROI_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        NumSizeROI.Value = RoiSize
        If RoiUnit IsNot Nothing Then
            ComboUnit.SelectedText = RoiUnit
        Else
            ComboUnit.SelectedIndex = 3
        End If

        CheckROI.Checked = RoiUse
    End Sub

    Private Sub DrawPreview()
        CalcRoiRect(Main_Form.scaleUnit, Main_Form.CFNum(Main_Form.CFSelected), RoiUnit, RoiSize, Main_Form.originImageList(Main_Form.tab_Index).Width, Main_Form.originImageList(Main_Form.tab_Index).Height, Main_Form.RoiRect)
        Dim FirstPt = New Point(Main_Form.RoiRect.X * Main_Form.ID_PICTURE_BOX(Main_Form.tab_Index).Width, Main_Form.RoiRect.Y * Main_Form.ID_PICTURE_BOX(Main_Form.tab_Index).Height)
        Dim SecondPt = New Point((Main_Form.RoiRect.X + Main_Form.RoiRect.Width) * Main_Form.ID_PICTURE_BOX(Main_Form.tab_Index).Width, (Main_Form.RoiRect.Y + Main_Form.RoiRect.Height) * Main_Form.ID_PICTURE_BOX(Main_Form.tab_Index).Height)
        DrawRectangle(Main_Form.ID_PICTURE_BOX(Main_Form.tab_Index), FirstPt, SecondPt)
    End Sub
    Private Sub ComboUnit_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboUnit.SelectedIndexChanged
        If ComboUnit.SelectedIndex = 0 Then
            RoiUnit = "m×m"
        ElseIf ComboUnit.SelectedIndex = 1 Then
            RoiUnit = "cm×cm"
        ElseIf ComboUnit.SelectedIndex = 2 Then
            RoiUnit = "mm×mm"
        Else
            RoiUnit = "μm×μm"
        End If
        DrawPreview()
    End Sub

    Private Sub NumSizeROI_ValueChanged(sender As Object, e As EventArgs) Handles NumSizeROI.ValueChanged
        RoiSize = NumSizeROI.Value
        DrawPreview()
    End Sub

    Private Sub CheckROI_CheckedChanged(sender As Object, e As EventArgs) Handles CheckROI.CheckedChanged
        RoiUse = CheckROI.Checked
        If RoiUse = False Then
            Main_Form.ID_PICTURE_BOX(Main_Form.tab_Index).Refresh()
        Else
            DrawPreview()
        End If
    End Sub

    Private Sub BtnDefault_Click(sender As Object, e As EventArgs) Handles BtnDefault.Click
        ComboUnit.SelectedIndex = 3
    End Sub
End Class