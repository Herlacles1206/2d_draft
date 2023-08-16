Imports Emgu.CV
Imports Emgu.CV.Structure

Public Class Density
    Public OriImage As Emgu.CV.Image(Of Bgr, Byte)
    Public GrayImage As Emgu.CV.Image(Of Gray, Byte)
    Public GrayCrop As Emgu.CV.Image(Of Gray, Byte)
    Private Black As Single
    Private White As Single

    Private RegionDrawed As Boolean

    Private FirstPtF As PointF
    Private SecondPtF As PointF

    Private DGV As New DataGridView
    Private Sub Density_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            Timer1.Interval = 30
            Timer1.Start()
            Dim scr = Main_Form.originImageList(Main_Form.tab_Index).ToBitmap()
            Dim bmpImage As Bitmap = New Bitmap(scr)
            OriImage = bmpImage.ToImage(Of Bgr, Byte)()
            bmpImage.Dispose()
            GrayImage = getGrayScale(OriImage)

            Main_Form.EdgeRegionDrawed = False
            Main_Form.EdgeRegionDrawReady = False
            Main_Form.FirstPtOfEdge.X = 0
            Main_Form.FirstPtOfEdge.Y = 0
            Main_Form.SecondPtOfEdge.X = 0
            Main_Form.SecondPtOfEdge.Y = 0

            RadRect.Checked = True
            Main_Form.CurveObjFlag = True

            DGV.Columns.Add(White, "White")
            DGV.Columns.Add(Black, "Black")
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub ShowResult()
        TxtBlack.Text = Black.ToString()
        TxtWhite.Text = White.ToString()

        DGV.Rows.Clear()
        Dim str_item = New String(2) {}
        str_item(0) = White.ToString()
        str_item(1) = Black.ToString()
        DGV.Rows.Add(str_item)
    End Sub
    Private Sub CalcPercentRect()
        CalcBlackAndWhite(GrayCrop, White, Black)
        ShowResult()
    End Sub

    Private Sub CalcPercentHand()
        CalcBlackAndWhite(GrayImage, Main_Form.C_CurveObjCopy, White, Black)
        ShowResult()
    End Sub
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        If RadRect.Checked Then
            If Main_Form.EdgeRegionDrawed And Main_Form.EdgeRegionDrawReady And Not RegionDrawed Then
                If Math.Abs(Main_Form.SecondPtOfEdge.X - Main_Form.FirstPtOfEdge.X) > 0.01 And Math.Abs(Main_Form.SecondPtOfEdge.Y - Main_Form.FirstPtOfEdge.Y) > 0.01 Then
                    RegionDrawed = True
                    FirstPtF = Main_Form.FirstPtOfEdge
                    SecondPtF = Main_Form.SecondPtOfEdge

                    Dim FirstPt = New Point(FirstPtF.X * OriImage.Width, FirstPtF.Y * OriImage.Height)
                    Dim SecondPt = New Point(SecondPtF.X * OriImage.Width, SecondPtF.Y * OriImage.Height)
                    Dim Rect = New Rectangle(FirstPt.X, FirstPt.Y, SecondPt.X - FirstPt.X, SecondPt.Y - FirstPt.Y)
                    GrayCrop = GrayImage.Copy()
                    GrayCrop.ROI = Rect
                    CalcPercentRect()
                End If
            End If

            If Not Main_Form.EdgeRegionDrawed And Main_Form.EdgeRegionDrawReady Then
                RegionDrawed = False
            End If
        Else
            If Main_Form.CurvePreviousPoint Is Nothing And Main_Form.C_CurveObjCopy.CPointIndx > 0 And Not RegionDrawed Then
                CalcPercentHand()
                RegionDrawed = True
            End If

            If Main_Form.CurvePreviousPoint IsNot Nothing Then
                RegionDrawed = False
            End If
        End If

    End Sub

    Private Sub RadRect_CheckedChanged(sender As Object, e As EventArgs) Handles RadRect.CheckedChanged
        If RadRect.Checked Then
            Main_Form.EdgeRegionDrawReady = True
            Main_Form.obj_selected.Refresh()
            Main_Form.curMeasureType = MeasureType.initState
            LabHelp.Text = "Please draw rectangle over the image for"
            LabHelp2.Text = "measuring density"
        End If
    End Sub

    Private Sub RadHand_CheckedChanged(sender As Object, e As EventArgs) Handles RadHand.CheckedChanged
        If RadHand.Checked Then
            Main_Form.EdgeRegionDrawReady = False
            Main_Form.obj_selected.Refresh()
            Main_Form.curMeasureType = MeasureType.objCurve
            Main_Form.obj_selected.measuringType = MeasureType.objCurve

            LabHelp.Text = "Please Draw The Mouse To Select a"
            LabHelp2.Text = "Region"
        End If
    End Sub

    Private Sub Density_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        Main_Form.CurveObjFlag = False
        Main_Form.obj_selected.Refresh()
        Main_Form.curMeasureType = MeasureType.initState
    End Sub

    Private Sub BtnExit_Click(sender As Object, e As EventArgs) Handles BtnExit.Click
        Me.Close()
    End Sub

    Private Sub BtnReport_Click(sender As Object, e As EventArgs) Handles BtnReport.Click
        Dim filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*"
        Dim title = "Save"
        Dim FirstPt = New Point(FirstPtF.X * Main_Form.ID_PICTURE_BOX(Main_Form.tab_Index).Width, FirstPtF.Y * Main_Form.ID_PICTURE_BOX(Main_Form.tab_Index).Height)
        Dim SecondPt = New Point(SecondPtF.X * Main_Form.ID_PICTURE_BOX(Main_Form.tab_Index).Width, SecondPtF.Y * Main_Form.ID_PICTURE_BOX(Main_Form.tab_Index).Height)

        If RadRect.Checked Then
            SaveListToReport(Main_Form.ID_PICTURE_BOX(Main_Form.tab_Index), DGV, filter, title, FirstPt, SecondPt, Main_Form.C_CurveObjCopy, True)
        Else
            SaveListToReport(Main_Form.ID_PICTURE_BOX(Main_Form.tab_Index), DGV, filter, title, FirstPt, SecondPt, Main_Form.C_CurveObjCopy, False)
        End If

    End Sub

    Private Sub BtnExcel_Click(sender As Object, e As EventArgs) Handles BtnExcel.Click
        Dim filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*"
        Dim title = "Save"
        SaveListToExcel(DGV, filter, title)
    End Sub
End Class