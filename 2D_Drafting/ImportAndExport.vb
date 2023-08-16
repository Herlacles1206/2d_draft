

Imports ClosedXML.Excel
Imports Emgu.CV
Imports Emgu.CV.CvEnum
Imports Emgu.CV.Structure
Imports System.IO
Imports System.Runtime.CompilerServices

''' <summary>
''' This class contains all the functions for import and export.
''' </summary>
Public Module ImportAndExport

    ''' <summary>
    ''' Load Image to picture box from file that user choises.
    ''' </summary>
    ''' <paramname="pictureBox">The picture box to load image on.</param>
    ''' <paramname="filter">The filter to be used to get type of images. example ("PNG Files|*.png|BMP Files|*.bmp")</param>
    ''' <paramname="fileDialogTitle">The title for the dialog appears for the user.</param>
    ''' <paramname="originImageList">The list of original input image.</param>
    ''' <paramname="resizedImageList">The list of image which is resized to fit the picturebox control.</param>
    ''' <paramname="initial_ratio">The ratio of the size of original image and the size of picturebox control.</param>
    ''' <paramname="tab_Index">The index of current tab page.</param>
    ''' <paramname="imgImportFlag">The flag specify you can import image to target tag.</param>
    <Extension()>
    Public Function LoadImageFromFiles(ByVal pictureBox As PictureBox, ByVal filter As String, ByVal fileDialogTitle As String,
                                          ByRef originImageList As List(Of Mat), ByRef resizedImageList As List(Of Mat),
                                          ByRef currentImageList As List(Of Mat), ByVal tab_Index As Integer, ByVal imgImportFlag As Boolean(),
                                       ByRef filenames As List(Of String)) As Integer
        Dim openFileDialog As OpenFileDialog = New OpenFileDialog()
        openFileDialog.Filter = filter
        openFileDialog.Title = fileDialogTitle

        openFileDialog.Multiselect = True

        openFileDialog.FileName = ""
        Dim image_filepaths = New String(24) {}
        If openFileDialog.ShowDialog() = DialogResult.OK Then
            image_filepaths = openFileDialog.FileNames
        Else
            Return 0
        End If

        Dim picturebox_h = pictureBox.Height
        Dim picturebox_w = pictureBox.Width
        Dim i = tab_Index
        Dim img_cnt = 0
        While img_cnt < image_filepaths.Length()
            If i > 24 Then Exit While

            If imgImportFlag(i) = False Then
                i += 1
                Continue While
            End If

            Dim originImg = CvInvoke.Imread(image_filepaths(img_cnt), ImreadModes.AnyColor)
            DisposeElemOfList(originImageList, originImg, i)
            DisposeElemOfList(resizedImageList, originImg.Clone(), i)
            DisposeElemOfList(currentImageList, originImg.Clone(), i)
            filenames(i) = System.IO.Path.GetFileName(image_filepaths(img_cnt))
            i += 1
            img_cnt += 1
        End While
        Return img_cnt
    End Function

    ''' <summary>
    ''' Load Image to picture box from file that user choises.
    ''' </summary>
    ''' <paramname="pictureBox">The picture box to load image on.</param>
    ''' <paramname="filename">The picture box to load image on.</param>
    ''' <paramname="originImageList">The list of original input image.</param>
    ''' <paramname="resizedImageList">The list of image which is resized to fit the picturebox control.</param>
    ''' <paramname="initial_ratio">The ratio of the size of original image and the size of picturebox control.</param>
    ''' <paramname="tab_Index">The index of current tab page.</param>
    <Extension()>
    Public Sub LoadImageFromFile(ByVal pictureBox As PictureBox, ByVal filename As String,
                                          ByRef originImageList As List(Of Mat), ByRef resizedImageList As List(Of Mat),
                                          ByRef currentImageList As List(Of Mat), ByVal tab_Index As Integer, ByRef filenames As List(Of String))


        Dim i = tab_Index

        Dim originImg = CvInvoke.Imread(filename, ImreadModes.AnyColor)
        DisposeElemOfList(originImageList, originImg.Clone(), tab_Index)
        'originImageList(tab_Index) = originImg
        DisposeElemOfList(resizedImageList, originImg.Clone(), tab_Index)
        DisposeElemOfList(currentImageList, originImg.Clone(), tab_Index)
        originImg.Dispose()
        filenames(i) = System.IO.Path.GetFileName(filename)

    End Sub


    ''' <summary>
    ''' Save Image of picture box.
    ''' </summary>
    ''' <paramname="pictureBox">The picture box to save its image</param>
    ''' <paramname="filter">The filter to be used to get type of images. example ("PNG Files|*.png|BMP Files|*.bmp")</param>
    ''' <paramname="saveDialogTitle">The title for the save dialog appears for the user.</param>

    <Extension()>
    Public Sub SaveImageInFile(ByVal pictureBox As PictureBox, ByVal filter As String, ByVal saveDialogTitle As String)

        Dim saveFileDialog As SaveFileDialog = New SaveFileDialog()

        saveFileDialog.Title = saveDialogTitle
        saveFileDialog.Filter = filter
        saveFileDialog.FileName = ""

        Dim img = pictureBox.Image
        If saveFileDialog.ShowDialog() = DialogResult.OK Then

            Dim result As Bitmap = New Bitmap(img.Width, img.Height)

            Using graph = Graphics.FromImage(result)
                Dim PointX = 0
                Dim PointY = 0
                Dim Width = img.Width
                Dim Height = img.Height
                graph.DrawImage(img, PointX, PointY, Width, Height)
                graph.Flush()

            End Using

            result.Save(saveFileDialog.FileName)
        End If

    End Sub

    ''' <summary>
    ''' Save the information of objects and image to excel file.
    ''' </summary>
    ''' <paramname="picturebox">The picturbox contains image</param>
    ''' <paramname="filter">The filter to be used to get type of images. example ("PNG Files|*.png|BMP Files|*.bmp")</param>
    ''' <paramname="saveDialogTitle">The title for the save dialog appears for the user.</param>
    ''' <paramname="obj_list_list">The list of list of objects.</param>
    ''' <paramname="digit">The digit of decimal numbers.</param>
    ''' <paramname="CF">The factor of measuring scale.</param>
    ''' <paramname="unit">The unit in length.</param>
    <Extension()>
    Public Sub SaveReportToExcel(ByVal picturebox As PictureBox, DGV As DataGridView, ByVal filter As String, ByVal saveDialogTitle As String, ByVal obj_list As List(Of MeasureObject), ByVal digit As Integer, ByVal CF As Double, ByVal unit As String)
        Dim SaveFileDialog As SaveFileDialog = New SaveFileDialog()
        SaveFileDialog.Filter = filter
        SaveFileDialog.Title = saveDialogTitle

        Dim xlsx_savepath = ""
        If SaveFileDialog.ShowDialog() = DialogResult.OK Then
            xlsx_savepath = SaveFileDialog.FileName

            Dim NameSet = "ABCDHIJKLMNOPQRSTUVWXYZ"

            Using workbook As XLWorkbook = XLWorkbook.OpenFromTemplate("C:\Users\Public\Documents\template.xlsx")
                Dim img = picturebox.Image
                If img IsNot Nothing Then
                    Dim result As Bitmap = New Bitmap(img.Width, img.Height)

                    Using graph = Graphics.FromImage(result)
                        Dim PointX = 0
                        Dim PointY = 0
                        Dim Width = img.Width
                        Dim Height = img.Height
                        graph.DrawImage(img, PointX, PointY, Width, Height)
                        DrawObjList2(graph, picturebox, obj_list, digit, CF)
                        graph.Flush()
                    End Using

                    Dim ms As MemoryStream = New MemoryStream()
                    result.Save(ms, Imaging.ImageFormat.Png)

                    Dim worksheet As IXLWorksheet
                    workbook.TryGetWorksheet("Sheet1", worksheet)

                    Dim row_count_listbox = DGV.Rows.Count
                    Dim workRow As IXLRow = worksheet.Row(33)
                    workRow.InsertRowsBelow(row_count_listbox - 7)

                    For i = 0 To row_count_listbox - 1
                        For j = 0 To DGV.Columns.Count - 1
                            If j = 3 Or j = 4 Then
                                If DGV.Rows(i).Cells(j).Value = "" Then
                                    Continue For
                                End If
                            End If
                            worksheet.Cell(NameSet(j) & (i + 33).ToString()).Value = DGV.Rows(i).Cells(j).Value
                        Next
                    Next
                    Dim image = worksheet.AddPicture(ms).MoveTo(worksheet.Cell("A14"), worksheet.Cell("K31")) 'the cast is only to be sure

                    workbook.SaveAs(xlsx_savepath)
                End If
            End Using
        End If

    End Sub

    ''' <summary>
    ''' Save the information of objects to excel file.
    ''' </summary>
    ''' <paramname="listview">The datagridview contains data</param>
    ''' <paramname="filter">The filter to be used to get type of images. example ("PNG Files|*.png|BMP Files|*.bmp")</param>
    ''' <paramname="saveDialogTitle">The title for the save dialog appears for the user.</param>

    Public Sub SaveListToExcel(ByVal listview As DataGridView, ByVal filter As String, ByVal saveDialogTitle As String)
        Dim SaveFileDialog As SaveFileDialog = New SaveFileDialog()
        SaveFileDialog.Filter = filter
        SaveFileDialog.Title = saveDialogTitle

        Dim xlsx_savepath = ""
        If SaveFileDialog.ShowDialog() = DialogResult.OK Then
            xlsx_savepath = SaveFileDialog.FileName

            Dim NameSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"

            Using workbook = New XLWorkbook()
                For cnt = 0 To 24
                    Dim worksheet = workbook.Worksheets.Add("Result Sheet" & cnt.ToString())
                    For i = 0 To listview.Columns.Count - 1
                        worksheet.Cell(NameSet(i) & 1).Value = listview.Columns(i).HeaderText
                    Next

                    Dim row_count_listbox = listview.Rows.Count
                    For i = 0 To row_count_listbox - 1
                        For j = 0 To listview.Columns.Count - 1
                            worksheet.Cell(NameSet(j) & (i + 2).ToString()).Value = listview.Rows(i).Cells(j).Value
                        Next
                    Next
                Next

                workbook.SaveAs(xlsx_savepath)
            End Using
        End If
    End Sub


    ''' <summary>
    ''' Save the information of objects to excel file.
    ''' </summary>
    ''' <paramname="image">The result image</param>
    ''' <paramname="listview">The datagridview contains data</param>
    ''' <paramname="filter">The filter to be used to get type of images. example ("PNG Files|*.png|BMP Files|*.bmp")</param>
    ''' <paramname="saveDialogTitle">The title for the save dialog appears for the user.</param>

    Public Sub SaveListToReport(img As Image, ByVal listview As DataGridView, ByVal filter As String, ByVal saveDialogTitle As String)
        Dim SaveFileDialog As SaveFileDialog = New SaveFileDialog()
        SaveFileDialog.Filter = filter
        SaveFileDialog.Title = saveDialogTitle

        Dim xlsx_savepath = ""
        If SaveFileDialog.ShowDialog() = DialogResult.OK Then
            xlsx_savepath = SaveFileDialog.FileName

            Dim NameSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
            Using workbook As XLWorkbook = XLWorkbook.OpenFromTemplate("C:\Users\Public\Documents\temp.xlsx")
                Dim result As Bitmap = New Bitmap(img.Width, img.Height)

                Using graph = Graphics.FromImage(result)
                    Dim PointX = 0
                    Dim PointY = 0
                    Dim Width = img.Width
                    Dim Height = img.Height
                    graph.DrawImage(img, PointX, PointY, Width, Height)
                    graph.Flush()
                End Using

                Dim ms As MemoryStream = New MemoryStream()
                result.Save(ms, Imaging.ImageFormat.Png)
                Dim worksheet As IXLWorksheet
                workbook.TryGetWorksheet("Sheet1", worksheet)

                Dim row_count_listbox = listview.Rows.Count

                Dim workRow As IXLRow = worksheet.Row(32)
                workRow.InsertRowsBelow(row_count_listbox)

                For i = 0 To listview.Columns.Count - 1
                    worksheet.Cell(NameSet(i) & 32).Value = listview.Columns(i).HeaderText
                Next

                For i = 0 To row_count_listbox - 1
                    For j = 0 To listview.Columns.Count - 1
                        worksheet.Cell(NameSet(j) & (i + 33).ToString()).Value = listview.Rows(i).Cells(j).Value
                    Next
                Next
                Dim image = worksheet.AddPicture(ms).MoveTo(worksheet.Cell("A14"), worksheet.Cell("K31")) 'the cast is only to be sure

                workbook.SaveAs(xlsx_savepath)
            End Using
        End If

    End Sub

    ''' <summary>
    ''' Save the information of objects to excel file.
    ''' </summary>
    ''' <paramname="image">The result image</param>
    ''' <paramname="listview">The datagridview contains data</param>
    ''' <paramname="filter">The filter to be used to get type of images. example ("PNG Files|*.png|BMP Files|*.bmp")</param>
    ''' <paramname="saveDialogTitle">The title for the save dialog appears for the user.</param>

    Public Sub SaveListToReport(pic As PictureBox, ByVal listview As DataGridView, ByVal filter As String, ByVal saveDialogTitle As String, Obj As SegObject, font As Font)
        Dim SaveFileDialog As SaveFileDialog = New SaveFileDialog()
        SaveFileDialog.Filter = filter
        SaveFileDialog.Title = saveDialogTitle

        Dim xlsx_savepath = ""
        If SaveFileDialog.ShowDialog() = DialogResult.OK Then
            xlsx_savepath = SaveFileDialog.FileName

            Dim NameSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
            Using workbook As XLWorkbook = XLWorkbook.OpenFromTemplate("C:\Users\Public\Documents\temp.xlsx")
                Dim result As Bitmap = New Bitmap(pic.Width, pic.Height)

                Using graph = Graphics.FromImage(result)
                    Dim PointX = 0
                    Dim PointY = 0
                    Dim Width = pic.Width
                    Dim Height = pic.Height
                    graph.DrawImage(pic.Image, PointX, PointY, Width, Height)
                    If Obj.measureType = SegType.BlobSegment Then
                        DrawLabelForCount(graph, pic, Obj.BlobSegObj.BlobList, font)
                    ElseIf Obj.measureType = SegType.intersection Then
                        Dim img = Main_Form.originImageList(Main_Form.tab_Index).ToBitmap()
                        Dim colorImg = img.ToImage(Of Bgr, Byte)()
                        img.Dispose()
                        Dim BinaryImg = GetBinary(colorImg, Obj.sectObj.thr_seg)
                        IdentifyInterSections(graph, BinaryImg, Obj, Width, Height)
                        colorImg.Dispose()
                        BinaryImg.Dispose()
                    ElseIf Obj.measureType = SegType.intercept Then
                        DrawInterCepts(graph, Main_Form.ID_PICTURE_BOX(Main_Form.tab_Index), Main_Form.Obj_Seg.ceptObj)
                        DrawLines(graph, Main_Form.ID_PICTURE_BOX(Main_Form.tab_Index), Main_Form.Obj_Seg.ceptObj, False)
                    ElseIf Obj.measureType = SegType.circleCept Then
                        DrawInterCepts(graph, Main_Form.ID_PICTURE_BOX(Main_Form.tab_Index), Main_Form.Obj_Seg.ceptObj)
                        DrawCircles(graph, Main_Form.ID_PICTURE_BOX(Main_Form.tab_Index))
                    End If

                    graph.Flush()
                End Using

                Dim ms As MemoryStream = New MemoryStream()
                result.Save(ms, Imaging.ImageFormat.Png)

                Dim worksheet As IXLWorksheet
                workbook.TryGetWorksheet("Sheet1", worksheet)

                Dim row_count_listbox = listview.Rows.Count
                Dim workRow As IXLRow = worksheet.Row(32)
                workRow.InsertRowsBelow(row_count_listbox)

                For i = 0 To listview.Columns.Count - 1
                    worksheet.Cell(NameSet(i) & 32).Value = listview.Columns(i).HeaderText
                Next

                For i = 0 To row_count_listbox - 1
                    For j = 0 To listview.Columns.Count - 1
                        worksheet.Cell(NameSet(j) & (i + 33).ToString()).Value = listview.Rows(i).Cells(j).Value
                    Next
                Next
                Dim image = worksheet.AddPicture(ms).MoveTo(worksheet.Cell("A14"), worksheet.Cell("K31")) 'the cast is only to be sure

                workbook.SaveAs(xlsx_savepath)
            End Using
        End If

    End Sub

    ''' <summary>
    ''' Save the information of objects to excel file.
    ''' </summary>
    ''' <paramname="image">The result image</param>
    ''' <paramname="listview">The datagridview contains data</param>
    ''' <paramname="filter">The filter to be used to get type of images. example ("PNG Files|*.png|BMP Files|*.bmp")</param>
    ''' <paramname="saveDialogTitle">The title for the save dialog appears for the user.</param>

    Public Sub SaveListToReport(pic As PictureBox, ByVal listview As DataGridView, ByVal filter As String, ByVal saveDialogTitle As String, FirstPt As Point, SecondPt As Point, CurveObj As CurveObj, flag As Boolean)
        Dim SaveFileDialog As SaveFileDialog = New SaveFileDialog()
        SaveFileDialog.Filter = filter
        SaveFileDialog.Title = saveDialogTitle

        Dim xlsx_savepath = ""
        If SaveFileDialog.ShowDialog() = DialogResult.OK Then
            xlsx_savepath = SaveFileDialog.FileName

            Dim NameSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
            Using workbook As XLWorkbook = XLWorkbook.OpenFromTemplate("C:\Users\Public\Documents\temp.xlsx")
                Dim result As Bitmap = New Bitmap(pic.Width, pic.Height)

                Using graph = Graphics.FromImage(result)
                    Dim PointX = 0
                    Dim PointY = 0
                    Dim Width = pic.Width
                    Dim Height = pic.Height
                    graph.DrawImage(pic.Image, PointX, PointY, Width, Height)
                    If flag Then
                        DrawRectangle(graph, pic, FirstPt, SecondPt)
                    Else
                        DrawCurveObj(graph, pic, Main_Form.line_infor, CurveObj)
                    End If

                    graph.Flush()
                End Using

                Dim ms As MemoryStream = New MemoryStream()
                result.Save(ms, Imaging.ImageFormat.Png)

                Dim worksheet As IXLWorksheet
                workbook.TryGetWorksheet("Sheet1", worksheet)

                Dim row_count_listbox = listview.Rows.Count
                Dim workRow As IXLRow = worksheet.Row(32)
                workRow.InsertRowsBelow(row_count_listbox)

                For i = 0 To listview.Columns.Count - 1
                    worksheet.Cell(NameSet(i) & 32).Value = listview.Columns(i).HeaderText
                Next

                For i = 0 To row_count_listbox - 1
                    For j = 0 To listview.Columns.Count - 1
                        worksheet.Cell(NameSet(j) & (i + 33).ToString()).Value = listview.Rows(i).Cells(j).Value
                    Next
                Next
                Dim image = worksheet.AddPicture(ms).MoveTo(worksheet.Cell("A14"), worksheet.Cell("K31")) 'the cast is only to be sure

                workbook.SaveAs(xlsx_savepath)
            End Using
        End If

    End Sub
End Module
