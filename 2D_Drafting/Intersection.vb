Imports Emgu.CV
Imports Emgu.CV.Structure

Public Class Intersection
    Public thr_seg As Integer
    Public OriImage As Emgu.CV.Image(Of Bgr, Byte)
    Public GrayImage As Emgu.CV.Image(Of Gray, Byte)
    Public BinaryImage As Emgu.CV.Image(Of Gray, Byte)

    Public Edge As Image
    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub Intersection_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Main_Form.Obj_Seg.Refresh()
        Dim scr = Main_Form.originImageList(Main_Form.tab_Index).ToBitmap()
        Dim bmpImage As Bitmap = New Bitmap(scr)
        OriImage = bmpImage.ToImage(Of Bgr, Byte)()
        bmpImage.Dispose()
        GrayImage = getGrayScale(OriImage)
        BinaryImage = GrayImage.CopyBlank()
    End Sub

    Private Sub LoadDataToGridView()
        DataGridView1.Rows.Clear()

        Dim str_item = New String(2) {}

        Dim maxLine = Math.Max(Main_Form.Obj_Seg.sectObj.horLine, Main_Form.Obj_Seg.sectObj.verLine)
        For j = 0 To maxLine - 1
            str_item(0) = (j + 1).ToString
            str_item(1) = "0"
            str_item(2) = "0"
            If j < Main_Form.Obj_Seg.sectObj.horLine Then
                str_item(1) = Main_Form.Obj_Seg.sectObj.horSectCnt(j)
            End If
            If j < Main_Form.Obj_Seg.sectObj.verLine Then
                str_item(2) = Main_Form.Obj_Seg.sectObj.verSectCnt(j)
            End If
            DataGridView1.Rows.Add(str_item)
        Next
    End Sub

    Private Sub ShowResult(output As Mat)
        Dim sz = New Size(Main_Form.resizedImageList(Main_Form.tab_Index).Width, Main_Form.resizedImageList(Main_Form.tab_Index).Height)
        Dim Resized As Mat = New Mat()
        CvInvoke.Resize(output, Resized, sz)
        Main_Form.ID_PICTURE_BOX(Main_Form.tab_Index).Image = Resized.ToBitmap()
        Main_Form.currentImageList(Main_Form.tab_Index) = output.Clone()
        output.Dispose()
        Resized.Dispose()
    End Sub
    Private Sub ID_SCROLL_THR_SEG_Scroll(sender As Object, e As EventArgs) Handles ID_SCROLL_THR_SEG.Scroll
        thr_seg = ID_SCROLL_THR_SEG.Value
        ID_LABEL_THR_SEG.Text = thr_seg.ToString()
        Main_Form.Obj_Seg.sectObj.thr_seg = thr_seg

        Dim percent_black = 0
        Dim percent_white = 0

        Dim image = OriImage.ToBitmap()
        Dim output = SegmentIntoBlackAndWhite(image, thr_seg, Main_Form.Obj_Seg, percent_black, percent_white)
        ShowResult(GetMatFromSDImage(output))
    End Sub

    Private Sub ID_BTN_EDGE_Click(sender As Object, e As EventArgs) Handles ID_BTN_EDGE.Click
        thr_seg = ID_SCROLL_THR_SEG.Value

        BinaryImage = GetBinary(OriImage, thr_seg)
        Dim output = OverLapSegToOri(OriImage, BinaryImage)

        ShowResult(output.Mat)
    End Sub

    Private Sub BtnCount_Click(sender As Object, e As EventArgs) Handles BtnCount.Click
        Main_Form.Obj_Seg.sectObj.horLine = ID_NUM_HORIZON.Value
        Main_Form.Obj_Seg.sectObj.verLine = ID_NUM_VERTICAL.Value
        IdentifyInterSections(Main_Form.ID_PICTURE_BOX(Main_Form.tab_Index), BinaryImage, Main_Form.Obj_Seg)
        LoadDataToGridView()
    End Sub

    Private Sub BtnExcel_Click(sender As Object, e As EventArgs) Handles BtnExcel.Click
        Dim filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*"
        Dim title = "Save"
        SaveListToExcel(DataGridView1, filter, title)
    End Sub

    Private Sub BtnReport_Click(sender As Object, e As EventArgs) Handles BtnReport.Click
        Dim filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*"
        Dim title = "Save"
        SaveListToReport(Main_Form.ID_PICTURE_BOX(Main_Form.tab_Index), DataGridView1, filter, title, Main_Form.Obj_Seg, Font)
    End Sub

    Private Sub BtnExit_Click(sender As Object, e As EventArgs) Handles BtnExit.Click
        Me.Close()
    End Sub

    Private Sub Intersection_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        Main_Form.Controls.Remove(Me)
        OriImage.Dispose()
        GrayImage.Dispose()
        BinaryImage.Dispose()

    End Sub
End Class