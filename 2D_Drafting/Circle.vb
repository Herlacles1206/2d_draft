﻿Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports Emgu.CV
Imports Emgu.CV.Structure

Public Class Circle
    Private roundness As Integer
    Private thr_cir As Integer
    Private thr_seg As Integer
    Private img_subtracted As Image
    Private subtract As Integer
    Private percent_circle As Double
    Private percent_black As Double
    Private percent_white As Double

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub InitLabel()
        percent_circle = 0
        percent_black = 0
        percent_white = 0
        LabCircle.Text = ""
        LabBlack.Text = ""
        LabWhite.Text = ""
    End Sub
    Private Sub Circle_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        InitLabel()
    End Sub

    Private Sub LoadDataToGridView()
        DataGridView1.Rows.Clear()

        Dim str_item = New String(3) {}

        For i = 0 To Main_Form.Obj_Seg.circleObj.Cnt - 1
            str_item(0) = (i + 1).ToString
            str_item(1) = CInt(Main_Form.Obj_Seg.circleObj.pos(i).X * Width).ToString
            str_item(2) = CInt(Main_Form.Obj_Seg.circleObj.pos(i).Y * Height).ToString
            str_item(3) = CInt(Main_Form.Obj_Seg.circleObj.size(i) * Width).ToString
            DataGridView1.Rows.Add(str_item)
        Next
    End Sub

    Private Sub ID_SCROLL_ROUNDNESS_Scroll(sender As Object, e As EventArgs) Handles ID_SCROLL_ROUNDNESS.Scroll
        roundness = ID_SCROLL_ROUNDNESS.Value
        ID_LABEL_ROUND.Text = roundness.ToString()
        Main_Form.Obj_Seg.Refresh()

        Dim image = Main_Form.originImageList(Main_Form.tab_Index).ToBitmap()
        Dim output = IdentifyCicles(image, roundness, thr_cir, Main_Form.Obj_Seg)

        ShowResult(GetMatFromSDImage(output))

        subtract = 0
        InitLabel()
        LoadDataToGridView()

        image.Dispose()
        output.Dispose()
    End Sub

    Private Sub ID_SCROLL_THR_SEG_Scroll(sender As Object, e As EventArgs) Handles ID_SCROLL_THR_SEG.Scroll
        If subtract = 0 Then
            subtract = 1

            'subtract circles from image
            Dim image = Main_Form.originImageList(Main_Form.tab_Index).ToBitmap()
            Dim output = SubtractCircles(image, Main_Form.Obj_Seg, percent_circle)
            ShowResult(GetMatFromSDImage(output))

            img_subtracted = output.Clone()
            image.Dispose()
            output.Dispose()
        End If

        thr_seg = ID_SCROLL_THR_SEG.Value
        ID_LABEL_THR_SEG.Text = thr_seg.ToString()

        'when button "ID_BUTTON_SUBTRACT" is clicked
        If subtract = 1 Then
            'Segment Remaing Image into Black and White
            Dim output = SegmentIntoBlackAndWhite(img_subtracted, thr_seg, Main_Form.Obj_Seg, percent_black, percent_white)
            ShowResult(GetMatFromSDImage(output))

            LabCircle.Text = percent_circle.ToString()
            LabWhite.Text = percent_white.ToString()
            LabBlack.Text = percent_black.ToString()

            output.Dispose()
        End If
    End Sub

    Private Sub ID_NUM_THR_CIR_ValueChanged(sender As Object, e As EventArgs) Handles ID_NUM_THR_CIR.ValueChanged
        thr_cir = CInt(ID_NUM_THR_CIR.Value)
        Main_Form.Obj_Seg.Refresh()
        'identify circles 
        Dim image = Main_Form.originImageList(Main_Form.tab_Index).ToBitmap()
        Dim output = IdentifyCicles(image, roundness, thr_cir, Main_Form.Obj_Seg)
        ShowResult(GetMatFromSDImage(output))

        subtract = 0
        InitLabel()
        LoadDataToGridView()
        image.Dispose()
        output.Dispose()
    End Sub

    Private Sub BtnExcel_Click(sender As Object, e As EventArgs) Handles BtnExcel.Click
        Dim filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*"
        Dim title = "Save"
        SaveListToExcel(DataGridView1, filter, title)
    End Sub

    Private Sub BtnReport_Click(sender As Object, e As EventArgs) Handles BtnReport.Click
        Dim filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*"
        Dim title = "Save"
        SaveListToReport(Main_Form.ID_PICTURE_BOX(Main_Form.tab_Index).Image, DataGridView1, filter, title)
    End Sub

    Private Sub BtnExit_Click(sender As Object, e As EventArgs) Handles BtnExit.Click
        Me.Close()
    End Sub

    Private Sub Circle_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        'Main_Form.Controls.Remove(Me)

    End Sub
End Class