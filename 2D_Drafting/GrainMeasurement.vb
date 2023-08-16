Imports System.Windows.Forms.MonthCalendar
Imports Emgu.CV
Imports Emgu.CV.CvEnum
Imports Emgu.CV.Structure
Imports Microsoft.VisualBasic.Devices

Public Class GrainMeasurement
    'commom member
    Public OriImage As Emgu.CV.Image(Of Bgr, Byte)
    Public GrayImage As Emgu.CV.Image(Of Gray, Byte)
    Public BinaryImage As Emgu.CV.Image(Of Gray, Byte)
    Public SkelImage As Emgu.CV.Image(Of Gray, Byte)

    Private AutoBinary As Integer = 0
    Private ManualBinary As Integer = 1
    Private AutoBinaryInv As Integer = 2
    Private RadioBinState As Integer

    Private Bri_Upper As Integer
    Private Bri_Lower As Integer

    Private RoiLeft As Single
    Private RoiTop As Single
    Private RoiRight As Single
    Private RoiBottom As Single

    'individual member
    'Manual Line ASTM
    Private MLA_num_Line As Integer
    Private MLA_num_Degree As Integer
    Private MLA_CeptData As LineCept
    Private MLA_CeptList As List(Of LineCept) = New List(Of LineCept)
    Private MLA_CeptObj As InterCeptObj = New InterCeptObj()
    Private MLA_UseRoi As Boolean
    Private MLA_ShowEdge As Boolean

    'Automatic Line ASTM
    Private ALA_num_Line As Integer
    Private ALA_num_Degree As Integer
    Private ALA_CeptData As LineCept
    Private ALA_CeptList As List(Of LineCept) = New List(Of LineCept)
    Private ALA_UseRoi As Boolean
    Private ALA_ShowEdge As Boolean

    'Automatic Circle ASTM
    Private ACA_Sr_No As Integer
    Private ACA_GrainData As CircleCept
    Private ACA_GrainList As List(Of CircleCept) = New List(Of CircleCept)
    Private ACA_UseRoi As Boolean
    Private ACA_ShowEdge As Boolean

    'Automatic Circle ASTM
    Private ACB_Sr_No As Integer
    Private ACB_GrainData As CircleCept
    Private ACB_GrainList As List(Of CircleCept) = New List(Of CircleCept)
    Private ACB_UseRoi As Boolean
    Private ACB_ShowEdge As Boolean

    'ALA Grain
    Private ALAG_GrainSize As Single
    Private ALAG_totalCnt As Integer
    Private ALAG_maxArea As Single
    Private ALAG_minArea As Single
    Private ALAG_minGrainNo As Integer
    Private ALAG_GrainNoList As Single() = {0, 0.5, 1.0, 1.5, 2.0, 2.5, 3.0, 3.5, 4.0, 4.5, 5.0, 5.5, 6.0, 6.5, 7.0, 7.5, 8.0, 8.5, 9.0, 9.5, 10.0, 10.5, 11.0, 11.5, 12.0, 12.5, 13.0, 13.5, 14.0}


    Private Sub GrainMeasurement_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim scr = Main_Form.originImageList(Main_Form.tab_Index).ToBitmap()
        Dim bmpImage As Bitmap = New Bitmap(scr)
        OriImage = bmpImage.ToImage(Of Bgr, Byte)()
        bmpImage.Dispose()
        GrayImage = getGrayScale(OriImage)
        BinaryImage = GrayImage.CopyBlank()
        SkelImage = GrayImage.CopyBlank()
        CalcPosRoi()
        Timer1.Interval = 30
        Timer1.Start()

        MLA_BtnMacro.Visible = False
    End Sub

    Private Sub TabControl1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles TabControl1.SelectedIndexChanged
        If TabControl1.SelectedTab Is Manual_Line_ASTM Then
            MLA_RadioPseudo.Checked = True
            Main_Form.Obj_Seg.Refresh()
            Main_Form.Obj_Seg.measureType = SegType.intercept
        ElseIf TabControl1.SelectedTab Is Automatic_Line_ASTM Then
            ALA_RadioPseudo.Checked = True
            Main_Form.Obj_Seg.Refresh()
            Main_Form.Obj_Seg.measureType = SegType.intercept
        ElseIf TabControl1.SelectedTab Is Automatic_Circle_ASTM Then
            Main_Form.Obj_Seg.Refresh()
            Main_Form.Obj_Seg.measureType = SegType.circleCept
        ElseIf TabControl1.SelectedTab Is Automatic_Circle_BSI Then
            Main_Form.Obj_Seg.Refresh()
            Main_Form.Obj_Seg.measureType = SegType.circleCept
        ElseIf TabControl1.SelectedTab Is ALA_Grain Then
            ALAG_RadioPesudo.Checked = True
            Main_Form.Obj_Seg.Refresh()
            Main_Form.Obj_Seg.measureType = SegType.intercept
        End If

    End Sub

    Private Sub CalcPosRoi()
        RoiLeft = Main_Form.RoiRect.X
        RoiRight = (Main_Form.RoiRect.X + Main_Form.RoiRect.Width)
        RoiTop = Main_Form.RoiRect.Y
        RoiBottom = (Main_Form.RoiRect.Y + Main_Form.RoiRect.Height)
    End Sub

    Private Sub DisplayOverlapped(RoiUse, ShowEdge)
        Dim output As Emgu.CV.Image(Of Bgr, Byte)
        Dim overlapped As Emgu.CV.Image(Of Gray, Byte)

        If ShowEdge Then
            overlapped = SkelImage
        Else
            overlapped = BinaryImage
        End If

        If RoiUse Then
            CalcPosRoi()
            output = OverLapSegToOri(OriImage, overlapped, RoiLeft, RoiTop, RoiRight, RoiBottom)
        Else
            output = OverLapSegToOri(OriImage, overlapped)
        End If

        Dim sz = New Size(Main_Form.resizedImageList(Main_Form.tab_Index).Width, Main_Form.resizedImageList(Main_Form.tab_Index).Height)
        Dim Resized As Mat = New Mat()
        CvInvoke.Resize(output, Resized, sz)

        Main_Form.ID_PICTURE_BOX(Main_Form.tab_Index).Image = Resized.ToBitmap()
        Main_Form.currentImageList(Main_Form.tab_Index) = output.Mat.Clone()
        output.Dispose()
        Resized.Dispose()
    End Sub
    Private Sub GetBinaryImage(RoiUse, ShowEdge)
        If RadioBinState = ManualBinary Then
            Dim form = New Intensity(RoiUse)
            If form.ShowDialog() = DialogResult.OK Then
                Bri_Upper = form.Upper
                Bri_Lower = form.Lower
            End If
            BinaryImage = GetBinaryWith2Thr(GrayImage, Bri_Lower, Bri_Upper)
        ElseIf RadioBinState = AutoBinary Then
            CvInvoke.Threshold(GrayImage, BinaryImage, 0, 255, ThresholdType.Otsu)
        ElseIf RadioBinState = AutoBinaryInv Then
            CvInvoke.Threshold(GrayImage, BinaryImage, 0, 255, ThresholdType.Otsu)
            BinaryImage = InvertBinary(BinaryImage)
            'CvInvoke.Imshow("1", BinaryImage)
        End If

        SkelImage = GetEdgeFromBinary(BinaryImage)
        DisplayOverlapped(RoiUse, ShowEdge)
    End Sub

    Private Function CalcRoiArea(useRoi) As String
        Dim area As Double
        Dim strArea As String
        If useRoi Then
            area = Main_Form.RoiSize
            strArea = area & Main_Form.RoiUnit
        Else
            area = OriImage.Width * OriImage.Height * Main_Form.CFNum(Main_Form.CFSelected) * Main_Form.CFNum(Main_Form.CFSelected)
            strArea = area & Main_Form.scaleUnit & "×" & Main_Form.scaleUnit
        End If

        Return strArea
    End Function

    Private Sub MLA_RemoveCeptObjs(mPtX, mPtY)
        For i = 0 To MLA_CeptObj.CeptCnt(0) - 1
            Dim PosX = MLA_CeptObj.CeptPos(0, i).X
            Dim PosY = MLA_CeptObj.CeptPos(0, i).Y

            If PosX > mPtX - 0.01 And PosX < mPtX + 0.01 And PosY > mPtY - 0.01 And PosY < mPtY + 0.01 Then
                For j = i To MLA_CeptObj.CeptCnt(0) - 2
                    MLA_CeptObj.CeptPos(0, j) = MLA_CeptObj.CeptPos(0, j + 1)
                Next
                MLA_CeptObj.CeptPos(0, MLA_CeptObj.CeptCnt(0) - 1).X = 0
                MLA_CeptObj.CeptPos(0, MLA_CeptObj.CeptCnt(0) - 1).Y = 0
                MLA_CeptObj.CeptCnt(0) -= 1
            End If
        Next
    End Sub
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        If TabControl1.SelectedTab Is Manual_Line_ASTM Then
            If Main_Form.MouseDownFlag Then
                Dim mPtX = Main_Form.mCurDrag.X
                Dim mPtY = Main_Form.mCurDrag.Y

                MLA_CeptObj.CeptPos(0, MLA_CeptObj.CeptCnt(0)).X = mPtX
                MLA_CeptObj.CeptPos(0, MLA_CeptObj.CeptCnt(0)).Y = mPtY
                MLA_CeptObj.CeptCnt(0) += 1

                MLA_ShowResult()
                Main_Form.MouseDownFlag = False
            End If
            If Main_Form.MouseRDownFlag Then
                Dim mPtX = Main_Form.mCurDrag.X
                Dim mPtY = Main_Form.mCurDrag.Y

                MLA_RemoveCeptObjs(mPtX, mPtY)
                MLA_ShowResult()
                Main_Form.MouseRDownFlag = False
            End If
        ElseIf TabControl1.SelectedTab Is Automatic_Line_ASTM Then
            If Main_Form.MouseDownFlag Then
                Main_Form.MouseDownFlag = False
                Dim mPtX = Main_Form.mCurDrag.X
                Dim mPtY = Main_Form.mCurDrag.Y
                Dim mX As Integer = mPtX * OriImage.Width
                Dim mY As Integer = mPtY * OriImage.Height

                AddIntercepts(Main_Form.Obj_Seg.ceptObj, OriImage.Width, OriImage.Height, mX, mY)
                ALA_DrawInterCepts()
            End If
            If Main_Form.MouseRDownFlag Then
                Dim mPtX = Main_Form.mCurDrag.X
                Dim mPtY = Main_Form.mCurDrag.Y
                Dim mX As Integer = mPtX * OriImage.Width
                Dim mY As Integer = mPtY * OriImage.Height

                RemoveIntercepts(Main_Form.Obj_Seg.ceptObj, OriImage.Width, OriImage.Height, mX, mY)
                ALA_DrawInterCepts()
                Main_Form.MouseRDownFlag = False
            End If
        ElseIf TabControl1.SelectedTab Is Automatic_Circle_ASTM Then
            If Main_Form.MouseDownFlag Then
                Main_Form.MouseDownFlag = False
                Dim mPtX = Main_Form.mCurDrag.X
                Dim mPtY = Main_Form.mCurDrag.Y
                Dim mX As Integer = mPtX * OriImage.Width
                Dim mY As Integer = mPtY * OriImage.Height

                AddCircleIntercepts(Main_Form.Obj_Seg.ceptObj, OriImage.Width, OriImage.Height, mX, mY)
                ACA_DrawIntercepts()
            End If
            If Main_Form.MouseRDownFlag Then
                Dim mPtX = Main_Form.mCurDrag.X
                Dim mPtY = Main_Form.mCurDrag.Y
                Dim mX As Integer = mPtX * OriImage.Width
                Dim mY As Integer = mPtY * OriImage.Height

                RemoveIntercepts(Main_Form.Obj_Seg.ceptObj, OriImage.Width, OriImage.Height, mX, mY)
                ACA_DrawIntercepts()
                Main_Form.MouseRDownFlag = False
            End If
        ElseIf TabControl1.SelectedTab Is Automatic_Circle_BSI Then
            If Main_Form.MouseDownFlag Then
                Main_Form.MouseDownFlag = False
                Dim mPtX = Main_Form.mCurDrag.X
                Dim mPtY = Main_Form.mCurDrag.Y
                Dim mX As Integer = mPtX * OriImage.Width
                Dim mY As Integer = mPtY * OriImage.Height

                AddCircleIntercepts(Main_Form.Obj_Seg.ceptObj, OriImage.Width, OriImage.Height, mX, mY)
                ACB_DrawIntercepts()
            End If
            If Main_Form.MouseRDownFlag Then
                Dim mPtX = Main_Form.mCurDrag.X
                Dim mPtY = Main_Form.mCurDrag.Y
                Dim mX As Integer = mPtX * OriImage.Width
                Dim mY As Integer = mPtY * OriImage.Height

                RemoveIntercepts(Main_Form.Obj_Seg.ceptObj, OriImage.Width, OriImage.Height, mX, mY)
                ACB_DrawIntercepts()
                Main_Form.MouseRDownFlag = False
            End If
        End If
    End Sub

#Region "Manual Line ASTM"
    Private Sub MLA_RadioMacro_CheckedChanged(sender As Object, e As EventArgs) Handles MLA_RadioMacro.CheckedChanged
        If MLA_RadioMacro.Checked Then
            MLA_BtnMacro.Visible = True
            MLA_BtnAuto.Visible = False
            MLA_BtnThres.Visible = False
            MLA_BtnAuto.Visible = False
            MLA_Lab.Visible = False
            MLA_NumDegree.Visible = False
        End If
    End Sub

    Private Sub MLA_RadioPseudo_CheckedChanged(sender As Object, e As EventArgs) Handles MLA_RadioPseudo.CheckedChanged
        If MLA_RadioPseudo.Checked Then
            MLA_BtnMacro.Visible = False
            MLA_BtnAuto.Visible = True
            MLA_BtnThres.Visible = True
            MLA_BtnAuto.Visible = True
            MLA_Lab.Visible = True
            MLA_NumDegree.Visible = True
        End If
    End Sub

    Private Sub MLA_NumLine_ValueChanged(sender As Object, e As EventArgs) Handles MLA_NumLine.ValueChanged
        MLA_num_Line = MLA_NumLine.Value
        Main_Form.Obj_Seg.ceptObj.numLine = MLA_num_Line
    End Sub

    Private Sub MLA_BtnAuto_Click(sender As Object, e As EventArgs) Handles MLA_BtnAuto.Click
        RadioBinState = AutoBinaryInv
        GetBinaryImage(MLA_UseRoi, MLA_ShowEdge)
    End Sub

    Private Sub MLA_BtnThres_Click(sender As Object, e As EventArgs) Handles MLA_BtnThres.Click
        RadioBinState = ManualBinary
        GetBinaryImage(MLA_UseRoi, MLA_ShowEdge)
    End Sub

    Private Sub MLA_NumDegree_ValueChanged(sender As Object, e As EventArgs) Handles MLA_NumDegree.ValueChanged
        MLA_num_Degree = MLA_NumDegree.Value
        Main_Form.Obj_Seg.ceptObj.Degree = MLA_num_Degree / 180 * Math.PI
    End Sub

    Private Sub MLA_ShowResult()

        DisplayOverlapped(MLA_UseRoi, MLA_ShowEdge)
        If MLA_num_Line = 0 Then
            Return
        End If
        Main_Form.Obj_Seg.Refresh()
        Main_Form.Obj_Seg.ceptObj.numLine = MLA_num_Line
        Main_Form.Obj_Seg.ceptObj.Degree = MLA_num_Degree / 180 * Math.PI
        DrawInterCepts(Main_Form.ID_PICTURE_BOX(Main_Form.tab_Index), MLA_CeptObj)
        If MLA_UseRoi Then
            DrawLines(Main_Form.ID_PICTURE_BOX(Main_Form.tab_Index), Main_Form.Obj_Seg.ceptObj, True, MLA_CeptObj, RoiLeft, RoiTop, RoiRight, RoiBottom)
        Else
            DrawLines(Main_Form.ID_PICTURE_BOX(Main_Form.tab_Index), Main_Form.Obj_Seg.ceptObj, True, MLA_CeptObj)
        End If

        GetInterCeptInfor()
        LoadMLACeptDetailDataToDGV()
    End Sub
    Private Sub MLA_CheckRoi_CheckedChanged(sender As Object, e As EventArgs) Handles MLA_CheckRoi.CheckedChanged
        If MLA_CheckRoi.Checked Then
            MLA_UseRoi = True
        Else
            MLA_UseRoi = False
        End If
        DisplayOverlapped(MLA_UseRoi, MLA_ShowEdge)
    End Sub
    Private Sub MLA_BtnMacro_Click(sender As Object, e As EventArgs) Handles MLA_BtnMacro.Click

    End Sub

    Public Sub GetInterCeptInfor()

        Dim Obj = Main_Form.Obj_Seg.ceptObj
        Dim InterCnt = 0
        For i = 0 To 100
            InterCnt += Obj.CeptCnt(i)
        Next
        Dim InterNo = InterCnt / Main_Form.Obj_Seg.ceptObj.numLine
        Dim Dia = "0.0000"
        Dim GrainArea = "0.0000"
        Dim GrainNo = "0.0000"

        If TabControl1.SelectedTab Is Manual_Line_ASTM Then
            Dim cnt = MLA_CeptList.Count
            Dim field = "Field" & cnt
            MLA_CeptData.field = field
            MLA_CeptData.InterNo = InterNo
            MLA_CeptData.Dia = Dia
            MLA_CeptData.GrainArea = GrainArea
            MLA_CeptData.GrainNo = GrainNo
            MLA_CeptData.totalArea = CalcRoiArea(MLA_UseRoi)

            MLA_TextField.Text = field
            MLA_TextInterNo.Text = InterNo
            MLA_TextDia.Text = Dia
            MLA_TextGrainArea.Text = GrainArea
            MLA_TextGrainNo.Text = GrainNo

        ElseIf TabControl1.SelectedTab Is Automatic_Line_ASTM Then
            Dim cnt = ALA_CeptList.Count
            Dim field = "Field" & cnt
            ALA_CeptData.field = field
            ALA_CeptData.InterNo = InterNo
            ALA_CeptData.Dia = Dia
            ALA_CeptData.GrainArea = GrainArea
            ALA_CeptData.GrainNo = GrainNo
            ALA_CeptData.totalArea = CalcRoiArea(ALA_UseRoi)

            ALA_TextField.Text = field
            ALA_TextInterNo.Text = InterNo
            ALA_TextDia.Text = Dia
            ALA_TextGrainArea.Text = GrainArea
            ALA_TextGrainNo.Text = GrainNo

        End If

    End Sub

    Private Sub LoadMLACeptDetailDataToDGV()
        MLA_GRID_DETAIL.Rows.Clear()
        Dim str_item = New String(3) {}

        For i = 0 To Main_Form.Obj_Seg.ceptObj.numLine - 1
            str_item(0) = "L" & (i + 1).ToString()
            str_item(1) = Main_Form.Obj_Seg.ceptObj.CeptCnt(i).ToString() & " intercepts"
            str_item(2) = Math.Floor(Main_Form.Obj_Seg.ceptObj.lineLength(i) * Main_Form.CFNum(Main_Form.CFSelected) / Main_Form.zoomFactor(Main_Form.tab_Index)) & Main_Form.scaleUnit

            MLA_GRID_DETAIL.Rows.Add(str_item)
        Next
    End Sub
    Private Sub MLA_BtnMeasure_Click(sender As Object, e As EventArgs) Handles MLA_BtnMeasure.Click
        If MLA_BtnMeasure.Text Is "Measure" Then
            MLA_BtnMeasure.Text = "Draw Intercepts"
        Else
            MLA_BtnMeasure.Text = "Measure"
        End If
        MLA_ShowResult()

    End Sub

    Private Sub LoadMLACeptDataToGridView()
        MLA_GRID.Rows.Clear()
        Dim str_item = New String(6) {}

        For i = 0 To MLA_CeptList.Count - 1
            Dim Obj = MLA_CeptList(i)
            str_item(0) = Obj.field
            str_item(1) = Obj.InterNo
            str_item(2) = Obj.Dia
            str_item(3) = Obj.GrainArea
            str_item(4) = Obj.GrainNo
            str_item(5) = Obj.totalArea

            MLA_GRID.Rows.Add(str_item)
        Next
    End Sub
    Private Sub MLA_BtnAdd_Click(sender As Object, e As EventArgs) Handles MLA_BtnAdd.Click
        MLA_CeptList.Add(MLA_CeptData)
        LoadMLACeptDataToGridView()
    End Sub

    Private Sub MLA_BtnDel_Click(sender As Object, e As EventArgs) Handles MLA_BtnDel.Click
        If MLA_CeptList.Count > 0 Then
            MLA_CeptList.RemoveAt(MLA_CeptList.Count - 1)
            LoadMLACeptDataToGridView()
        End If
    End Sub

    Private Sub MLA_BtnReport_Click(sender As Object, e As EventArgs) Handles MLA_BtnReport.Click
        Dim filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*"
        Dim title = "Save"
        SaveListToReport(Main_Form.ID_PICTURE_BOX(Main_Form.tab_Index), MLA_GRID, filter, title, Main_Form.Obj_Seg, Font)
    End Sub

    Private Sub MLA_BtnExcel_Click(sender As Object, e As EventArgs) Handles MLA_BtnExcel.Click
        Dim filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*"
        Dim title = "Save"
        SaveListToExcel(MLA_GRID, filter, title)
    End Sub

    Private Sub MLA_BtnUndo_Click(sender As Object, e As EventArgs) Handles MLA_BtnUndo.Click
        Main_Form.Obj_Seg.Refresh()
        Main_Form.ID_PICTURE_BOX(Main_Form.tab_Index).Refresh()
    End Sub

    Private Sub MLA_CheckEdge_CheckedChanged(sender As Object, e As EventArgs) Handles MLA_CheckEdge.CheckedChanged
        If MLA_CheckEdge.Checked Then
            MLA_ShowEdge = True
        Else
            MLA_ShowEdge = False
        End If
        DisplayOverlapped(MLA_UseRoi, MLA_ShowEdge)
    End Sub
#End Region

#Region "Automatic Line ASTM"
    Private Sub ALA_RadioMacro_CheckedChanged(sender As Object, e As EventArgs) Handles ALA_RadioMacro.CheckedChanged
        If ALA_RadioMacro.Checked Then
            ALA_BtnMacro.Visible = True
            ALA_BtnAuto.Visible = False
            ALA_BtnThres.Visible = False
            ALA_BtnAuto.Visible = False
            ALA_Lab.Visible = False
            ALA_NumDegree.Visible = False
        End If
    End Sub

    Private Sub ALA_RadioPseudo_CheckedChanged(sender As Object, e As EventArgs) Handles ALA_RadioPseudo.CheckedChanged
        If ALA_RadioPseudo.Checked Then
            ALA_BtnMacro.Visible = False
            ALA_BtnAuto.Visible = True
            ALA_BtnThres.Visible = True
            ALA_BtnAuto.Visible = True
            ALA_Lab.Visible = True
            ALA_NumDegree.Visible = True
        End If
    End Sub

    Private Sub ALA_NumLine_ValueChanged(sender As Object, e As EventArgs) Handles ALA_NumLine.ValueChanged
        ALA_num_Line = ALA_NumLine.Value
        Main_Form.Obj_Seg.ceptObj.numLine = ALA_num_Line
    End Sub

    Private Sub ALA_BtnAuto_Click(sender As Object, e As EventArgs) Handles ALA_BtnAuto.Click
        RadioBinState = AutoBinaryInv
        GetBinaryImage(ALA_UseRoi, ALA_ShowEdge)
    End Sub

    Private Sub ALA_BtnThres_Click(sender As Object, e As EventArgs) Handles ALA_BtnThres.Click
        RadioBinState = ManualBinary
        GetBinaryImage(ALA_UseRoi, ALA_ShowEdge)
    End Sub

    Private Sub ALA_BtnMacro_Click(sender As Object, e As EventArgs) Handles ALA_BtnMacro.Click

    End Sub

    Private Sub ALA_NumDegree_ValueChanged(sender As Object, e As EventArgs) Handles ALA_NumDegree.ValueChanged
        ALA_num_Degree = ALA_NumDegree.Value
        Main_Form.Obj_Seg.ceptObj.Degree = ALA_num_Degree / 180 * Math.PI
    End Sub

    Private Sub LoadALACeptDetailDataToDGV()
        ALA_GRID_DETAIL.Rows.Clear()
        Dim str_item = New String(3) {}

        For i = 0 To Main_Form.Obj_Seg.ceptObj.numLine - 1
            str_item(0) = "L" & (i + 1).ToString()
            str_item(1) = Main_Form.Obj_Seg.ceptObj.CeptCnt(i).ToString() & " intercepts"
            str_item(2) = Math.Floor(Main_Form.Obj_Seg.ceptObj.lineLength(i) * Main_Form.CFNum(Main_Form.CFSelected) / Main_Form.zoomFactor(Main_Form.tab_Index)) & Main_Form.scaleUnit

            ALA_GRID_DETAIL.Rows.Add(str_item)
        Next
    End Sub

    Private Sub ALA_DrawInterCepts()
        DrawInterCepts(Main_Form.ID_PICTURE_BOX(Main_Form.tab_Index), Main_Form.Obj_Seg.ceptObj)
        If ALA_UseRoi Then
            DrawLines(Main_Form.ID_PICTURE_BOX(Main_Form.tab_Index), Main_Form.Obj_Seg.ceptObj, False, MLA_CeptObj, RoiLeft, RoiTop, RoiRight, RoiBottom)
        Else
            DrawLines(Main_Form.ID_PICTURE_BOX(Main_Form.tab_Index), Main_Form.Obj_Seg.ceptObj, False, MLA_CeptObj)
        End If
        GetInterCeptInfor()
        LoadALACeptDetailDataToDGV()
    End Sub
    Private Sub ALA_ShowResult()
        DisplayOverlapped(ALA_UseRoi, ALA_ShowEdge)
        If ALA_num_Line = 0 Then
            Return
        End If

        If ALA_UseRoi Then
            CalcPosRoi()
            IdentifyInterCepts(BinaryImage, Main_Form.Obj_Seg, RoiLeft, RoiTop, RoiRight, RoiBottom)
        Else
            IdentifyInterCepts(BinaryImage, Main_Form.Obj_Seg)
        End If

        ALA_DrawInterCepts()
    End Sub
    Private Sub ALA_CheckRoi_CheckedChanged(sender As Object, e As EventArgs) Handles ALA_CheckRoi.CheckedChanged
        If ALA_CheckRoi.Checked Then
            ALA_UseRoi = True
        Else
            ALA_UseRoi = False
        End If
        DisplayOverlapped(ALA_UseRoi, ALA_ShowEdge)
    End Sub
    Private Sub ALA_BtnMeasure_Click(sender As Object, e As EventArgs) Handles ALA_BtnMeasure.Click
        ALA_ShowResult()
    End Sub

    Private Sub LoadALACeptDataToGridView()
        ALA_Grid.Rows.Clear()
        Dim str_item = New String(6) {}

        For i = 0 To ALA_CeptList.Count - 1
            Dim Obj = ALA_CeptList(i)
            str_item(0) = Obj.field
            str_item(1) = Obj.InterNo
            str_item(2) = Obj.Dia
            str_item(3) = Obj.GrainArea
            str_item(4) = Obj.GrainNo
            str_item(5) = Obj.totalArea

            ALA_Grid.Rows.Add(str_item)
        Next
    End Sub
    Private Sub ALA_BtnAdd_Click(sender As Object, e As EventArgs) Handles ALA_BtnAdd.Click
        ALA_CeptList.Add(ALA_CeptData)
        LoadALACeptDataToGridView()
    End Sub

    Private Sub ALA_BtnDel_Click(sender As Object, e As EventArgs) Handles ALA_BtnDel.Click
        If ALA_CeptList.Count > 0 Then
            ALA_CeptList.RemoveAt(ALA_CeptList.Count - 1)
            LoadALACeptDataToGridView()
        End If
    End Sub

    Private Sub ALA_BtnReport_Click(sender As Object, e As EventArgs) Handles ALA_BtnReport.Click
        Dim filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*"
        Dim title = "Save"
        SaveListToReport(Main_Form.ID_PICTURE_BOX(Main_Form.tab_Index), ALA_Grid, filter, title, Main_Form.Obj_Seg, Font)
    End Sub

    Private Sub ALA_BtnExcel_Click(sender As Object, e As EventArgs) Handles ALA_BtnExcel.Click
        Dim filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*"
        Dim title = "Save"
        SaveListToExcel(ALA_Grid, filter, title)
    End Sub

    Private Sub ALA_BtnUndo_Click(sender As Object, e As EventArgs) Handles ALA_BtnUndo.Click
        Main_Form.Obj_Seg.Refresh()
        Main_Form.ID_PICTURE_BOX(Main_Form.tab_Index).Refresh()
    End Sub

    Private Sub ALA_CheckEdge_CheckedChanged(sender As Object, e As EventArgs) Handles ALA_CheckEdge.CheckedChanged
        If ALA_CheckEdge.Checked Then
            ALA_ShowEdge = True
        Else
            ALA_ShowEdge = False
        End If
        DisplayOverlapped(ALA_UseRoi, ALA_ShowEdge)
    End Sub

#End Region

#Region "Automatic Circle ASTM"
    Private Sub ACA_BtnAuto_Click(sender As Object, e As EventArgs) Handles ACA_BtnAuto.Click
        RadioBinState = AutoBinaryInv
        GetBinaryImage(ACA_UseRoi, ACA_ShowEdge)
    End Sub

    Private Sub ACA_BtnThres_Click(sender As Object, e As EventArgs) Handles ACA_BtnThres.Click
        RadioBinState = ManualBinary
        GetBinaryImage(ACA_UseRoi, ACA_ShowEdge)
    End Sub

    Private Sub DisplayACAResult()
        ACA_TextSrNo.Text = ACA_Sr_No.ToString()
        Dim InterCeptCnt = 0
        For i = 0 To 3
            InterCeptCnt += Main_Form.Obj_Seg.ceptObj.CeptCnt(i)
        Next
        ACA_TextNoInter.Text = InterCeptCnt.ToString()
        'Temp code
        Dim GrainSize = 0

        ACA_GrainData.field = ACA_Sr_No.ToString()
        ACA_GrainData.InterNo = InterCeptCnt.ToString()
        ACA_GrainData.GrainSize = GrainSize.ToString()
    End Sub

    Private Sub LoadACACeptDetailDataToDGV()
        ACA_GRID_DETAIL.Rows.Clear()
        Dim str_item = New String(2) {}

        For i = 1 To Main_Form.Obj_Seg.ceptObj.numLine - 1
            str_item(0) = "L" & (i).ToString()
            str_item(1) = Main_Form.Obj_Seg.ceptObj.CeptCnt(i).ToString() & " intercepts"

            ACA_GRID_DETAIL.Rows.Add(str_item)
        Next
    End Sub

    Private Sub ACA_DrawIntercepts()
        DrawInterCepts(Main_Form.ID_PICTURE_BOX(Main_Form.tab_Index), Main_Form.Obj_Seg.ceptObj)
        DrawCircles(Main_Form.ID_PICTURE_BOX(Main_Form.tab_Index))
        DisplayACAResult()
        LoadACACeptDetailDataToDGV()
    End Sub
    Private Sub ACA_ShowResult()
        DisplayOverlapped(ACA_UseRoi, ACA_ShowEdge)
        If ACA_UseRoi Then
            CalcPosRoi()
            IdentifyCircularInterCepts(BinaryImage, Main_Form.Obj_Seg, RoiLeft, RoiTop, RoiRight, RoiBottom)
        Else
            IdentifyCircularInterCepts(BinaryImage, Main_Form.Obj_Seg)
        End If

        ACA_DrawIntercepts()
    End Sub

    Private Sub ACA_CheckRoi_CheckedChanged(sender As Object, e As EventArgs) Handles ACA_CheckRoi.CheckedChanged
        If ACA_CheckRoi.Checked Then
            ACA_UseRoi = True
        Else
            ACA_UseRoi = False
        End If
        DisplayOverlapped(ACA_UseRoi, ACA_ShowEdge)
    End Sub
    Private Sub ACA_BtnDraw_Click(sender As Object, e As EventArgs) Handles ACA_BtnDraw.Click
        ACA_Sr_No += 1
        ACA_ShowResult()
    End Sub

    Private Sub LoadACADataToGridView()
        ACA_Grid.Rows.Clear()
        Dim str_item = New String(3) {}

        For i = 0 To ACA_GrainList.Count - 1
            Dim Obj = ACA_GrainList(i)
            str_item(0) = Obj.field
            str_item(1) = Obj.InterNo
            str_item(2) = Obj.GrainSize

            ACA_Grid.Rows.Add(str_item)
        Next
    End Sub
    Private Sub ACA_BtnAdd_Click(sender As Object, e As EventArgs) Handles ACA_BtnAdd.Click
        ACA_GrainList.Add(ACA_GrainData)
        LoadACADataToGridView()
    End Sub

    Private Sub ACA_BtnDel_Click(sender As Object, e As EventArgs) Handles ACA_BtnDel.Click
        If ACA_GrainList.Count > 0 Then
            ACA_GrainList.RemoveAt(ACA_GrainList.Count - 1)
            LoadACADataToGridView()
        End If
    End Sub

    Private Sub ACA_BtnReport_Click(sender As Object, e As EventArgs) Handles ACA_BtnReport.Click
        Dim filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*"
        Dim title = "Save"
        SaveListToReport(Main_Form.ID_PICTURE_BOX(Main_Form.tab_Index), ACA_Grid, filter, title, Main_Form.Obj_Seg, Font)
    End Sub

    Private Sub ACA_BtnExcel_Click(sender As Object, e As EventArgs) Handles ACA_BtnExcel.Click
        Dim filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*"
        Dim title = "Save"
        SaveListToExcel(ACA_Grid, filter, title)
    End Sub

    Private Sub ACA_BtnDelAll_Click(sender As Object, e As EventArgs) Handles ACA_BtnDelAll.Click
        ACA_GrainList.Clear()
        LoadACADataToGridView()
    End Sub

    Private Sub ACA_CheckEdge_CheckedChanged(sender As Object, e As EventArgs) Handles ACA_CheckEdge.CheckedChanged
        If ACA_CheckEdge.Checked Then
            ACA_ShowEdge = True
        Else
            ACA_ShowEdge = False
        End If
        DisplayOverlapped(ACA_UseRoi, ACA_ShowEdge)
    End Sub
#End Region

#Region "Automatic Circle BSI"
    Private Sub ACB_BtnAuto_Click(sender As Object, e As EventArgs) Handles ACB_BtnAuto.Click
        RadioBinState = AutoBinaryInv
        GetBinaryImage(ACB_UseRoi, ACB_ShowEdge)
    End Sub

    Private Sub ACB_BtnThre_Click(sender As Object, e As EventArgs) Handles ACB_BtnThre.Click
        RadioBinState = ManualBinary
        GetBinaryImage(ACB_UseRoi, ACB_ShowEdge)
    End Sub

    Private Sub DisplayACBResult()

        ACB_TextSrNo.Text = ACB_Sr_No.ToString()
        Dim InterCeptCnt = 0
        For i = 0 To 3
            InterCeptCnt += Main_Form.Obj_Seg.ceptObj.CeptCnt(i)
        Next
        ACB_TextNoInter.Text = InterCeptCnt.ToString()
        'Temp code
        Dim GrainSize = 0

        ACB_GrainData.field = ACB_Sr_No.ToString()
        ACB_GrainData.InterNo = InterCeptCnt.ToString()
        ACB_GrainData.GrainSize = GrainSize.ToString()
    End Sub
    Private Sub LoadACBCeptDetailDataToDGV()
        ACB_GRID_DETAIL.Rows.Clear()
        Dim str_item = New String(2) {}

        For i = 1 To Main_Form.Obj_Seg.ceptObj.numLine - 1
            str_item(0) = "L" & (i).ToString()
            str_item(1) = Main_Form.Obj_Seg.ceptObj.CeptCnt(i).ToString() & " intercepts"

            ACB_GRID_DETAIL.Rows.Add(str_item)
        Next
    End Sub

    Private Sub ACB_DrawIntercepts()
        DrawInterCepts(Main_Form.ID_PICTURE_BOX(Main_Form.tab_Index), Main_Form.Obj_Seg.ceptObj)
        DrawCircles(Main_Form.ID_PICTURE_BOX(Main_Form.tab_Index))
        DisplayACBResult()
        LoadACBCeptDetailDataToDGV()
    End Sub
    Private Sub ACB_ShowResult()
        DisplayOverlapped(ACB_UseRoi, ACB_ShowEdge)
        If ACB_UseRoi Then
            CalcPosRoi()
            IdentifyCircularInterCepts(BinaryImage, Main_Form.Obj_Seg, RoiLeft, RoiTop, RoiRight, RoiBottom)
        Else
            IdentifyCircularInterCepts(BinaryImage, Main_Form.Obj_Seg)
        End If
        ACB_DrawIntercepts()
    End Sub
    Private Sub ACB_CheckRoi_CheckedChanged(sender As Object, e As EventArgs) Handles ACB_CheckRoi.CheckedChanged
        If ACB_CheckRoi.Checked Then
            ACB_UseRoi = True
        Else
            ACB_UseRoi = False
        End If
        DisplayOverlapped(ACB_UseRoi, ACB_ShowEdge)
    End Sub
    Private Sub ACB_BtnDraw_Click(sender As Object, e As EventArgs) Handles ACB_BtnDraw.Click
        ACB_Sr_No += 1
        ACB_ShowResult()
    End Sub

    Private Sub LoadACBDataToGridView()
        ACB_Grid.Rows.Clear()
        Dim str_item = New String(3) {}

        For i = 0 To ACB_GrainList.Count - 1
            Dim Obj = ACB_GrainList(i)
            str_item(0) = Obj.field
            str_item(1) = Obj.InterNo
            str_item(2) = Obj.GrainSize

            ACB_Grid.Rows.Add(str_item)
        Next
    End Sub
    Private Sub ACB_BtnAdd_Click(sender As Object, e As EventArgs) Handles ACB_BtnAdd.Click
        ACB_GrainList.Add(ACB_GrainData)
        LoadACBDataToGridView()
    End Sub

    Private Sub ACB_BtnDel_Click(sender As Object, e As EventArgs) Handles ACB_BtnDel.Click
        If ACB_GrainList.Count > 0 Then
            ACB_GrainList.RemoveAt(ACB_GrainList.Count - 1)
            LoadACBDataToGridView()
        End If
    End Sub

    Private Sub ACB_BtnReport_Click(sender As Object, e As EventArgs) Handles ACB_BtnReport.Click
        Dim filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*"
        Dim title = "Save"
        SaveListToReport(Main_Form.ID_PICTURE_BOX(Main_Form.tab_Index), ACB_Grid, filter, title, Main_Form.Obj_Seg, Font)
    End Sub

    Private Sub ACB_BtnExcel_Click(sender As Object, e As EventArgs) Handles ACB_BtnExcel.Click
        Dim filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*"
        Dim title = "Save"
        SaveListToExcel(ACB_Grid, filter, title)
    End Sub

    Private Sub ACB_BtnDelAll_Click(sender As Object, e As EventArgs) Handles ACB_BtnDelAll.Click
        ACB_GrainList.Clear()
        LoadACBDataToGridView()
    End Sub

    Private Sub ACB_CheckEdge_CheckedChanged(sender As Object, e As EventArgs) Handles ACB_CheckEdge.CheckedChanged
        If ACB_CheckEdge.Checked Then
            ACB_ShowEdge = True
        Else
            ACB_ShowEdge = False
        End If
        DisplayOverlapped(ACB_UseRoi, ACB_ShowEdge)
    End Sub

#End Region

#Region "ALA Grain"


    Private Sub ALAG_RadioMacro_CheckedChanged(sender As Object, e As EventArgs) Handles ALAG_RadioMacro.CheckedChanged
        If ALAG_RadioMacro.Checked Then
            ALAG_BtnAuto.Visible = False
            ALAG_BtnThre.Visible = False
            ALAG_BtnMacro.Visible = True
        End If
    End Sub

    Private Sub ALAG_RadioPesudo_CheckedChanged(sender As Object, e As EventArgs) Handles ALAG_RadioPesudo.CheckedChanged
        If ALAG_RadioPesudo.Checked Then
            ALAG_BtnMacro.Visible = False
            ALAG_BtnAuto.Visible = True
            ALAG_BtnThre.Visible = True
        End If
    End Sub

    Private Sub ALAG_ComboSize_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ALAG_ComboSize.SelectedIndexChanged
        Dim index = ALAG_ComboSize.SelectedIndex

        Dim output = GrainDetection(OriImage, BinaryImage, Main_Form.Obj_Seg.BlobSegObj.BlobList, Main_Form.colList, index)
        ShowResult(output.Mat)
    End Sub

    Private Sub ALAG_BtnAuto_Click(sender As Object, e As EventArgs) Handles ALAG_BtnAuto.Click
        RadioBinState = AutoBinaryInv
        GetBinaryImage(False, False)
    End Sub

    Private Sub ALAG_BtnThre_Click(sender As Object, e As EventArgs) Handles ALAG_BtnThre.Click
        RadioBinState = ManualBinary
        GetBinaryImage(False, False)
    End Sub

    Private Sub ALAG_BtnMacro_Click(sender As Object, e As EventArgs) Handles ALAG_BtnMacro.Click

    End Sub

    Private Sub GetMinMax()
        ALAG_totalCnt = Main_Form.Obj_Seg.BlobSegObj.BlobList.Count

        ALAG_maxArea = 0
        ALAG_minGrainNo = 0
        ALAG_minArea = 9999999

        For i = 0 To ALAG_totalCnt - 1
            Dim Obj = Main_Form.Obj_Seg.BlobSegObj.BlobList(i)
            If ALAG_maxArea < Obj.Area Then ALAG_maxArea = Obj.Area
            If ALAG_minArea > Obj.Area Then
                ALAG_minArea = Obj.Area
                ALAG_minGrainNo = i
            End If
        Next

        ALAG_LabArea.Text = ALAG_maxArea.ToString()
        ALAG_LabSmallNo.Text = ALAG_minGrainNo.ToString()
        ALAG_LabCnt.Text = ALAG_totalCnt.ToString()
    End Sub
    Private Sub LoadALAGDataToGridView()
        ALAG_DGV.Rows.Clear()
        Dim str_item = New String(3) {}

        For i = 0 To Main_Form.Obj_Seg.BlobSegObj.BlobList.Count - 1
            Dim Obj = Main_Form.Obj_Seg.BlobSegObj.BlobList(i)
            str_item(0) = (i + 1).ToString()
            str_item(1) = Obj.Area.ToString()
            Dim GrainNo = ALAG_GrainNoList(GetGrainNo(Obj.Area))
            str_item(2) = GrainNo.ToString()

            ALAG_DGV.Rows.Add(str_item)
        Next
    End Sub
    Private Sub ALAG_BtnMeasure_Click(sender As Object, e As EventArgs) Handles ALAG_BtnMeasure.Click
        Dim output = GrainDetection(OriImage, BinaryImage, Main_Form.Obj_Seg.BlobSegObj.BlobList, Main_Form.colList)
        ShowResult(output.Mat)
        GetMinMax()
        LoadALAGDataToGridView()
    End Sub

    Private Sub ALAG_ColorDistrib_Click(sender As Object, e As EventArgs) Handles ALAG_ColorDistrib.Click
        Dim form As New ColorDistribution()
        form.Show()
    End Sub

    Private Sub ALAG_BtnReport_Click(sender As Object, e As EventArgs) Handles ALAG_BtnReport.Click
        Dim filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*"
        Dim title = "Save"
        SaveListToReport(Main_Form.ID_PICTURE_BOX(Main_Form.tab_Index), ALAG_DGV, filter, title, Main_Form.Obj_Seg, Font)
    End Sub

    Private Sub ALAG_BtnExcel_Click(sender As Object, e As EventArgs) Handles ALAG_BtnExcel.Click
        Dim filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*"
        Dim title = "Save"
        SaveListToExcel(ALAG_DGV, filter, title)
    End Sub

    Private Sub ALAG_BtnUndo_Click(sender As Object, e As EventArgs) Handles ALAG_BtnUndo.Click
        Main_Form.Obj_Seg.Refresh()
        ALAG_DGV.Rows.Clear()
        Main_Form.ID_PICTURE_BOX(Main_Form.tab_Index).Image = Main_Form.resizedImageList(Main_Form.tab_Index).ToBitmap()
    End Sub

#End Region




    Private Sub GrainMeasurement_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        'Main_Form.Controls.Remove(Me)
        OriImage.Dispose()
        GrayImage.Dispose()
        BinaryImage.Dispose()
        SkelImage.Dispose()
    End Sub
End Class