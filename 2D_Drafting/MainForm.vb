Imports System.Data.Common
Imports System.IO
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Windows.Interop
Imports AForge.Video
Imports AForge.Video.DirectShow
Imports DocumentFormat.OpenXml.Vml.Office
Imports DocumentFormat.OpenXml.Wordprocessing
Imports Emgu.CV
Imports Emgu.CV.Ocl
Imports Color = System.Drawing.Color
Imports ComboBox = System.Windows.Forms.ComboBox
Imports Font = System.Drawing.Font
Imports TextBox = System.Windows.Forms.TextBox


Public Class Main_Form
    Public Shared originImageList As List(Of Mat) = New List(Of Mat)()           'original image
    Public Shared resizedImageList As List(Of Mat) = New List(Of Mat)()          'the image which is resized to fit the picturebox control
    Public Shared currentImageList As List(Of Mat) = New List(Of Mat)()          'the image which is currently used
    Public file_names As List(Of String) = New List(Of String)
    Public zoomFactor As Double() = New Double(24) {}                 'the zooming factor
    Public curMeasureType As Integer                                 'current measurement type
    Public MeasureTypePrev As Integer                            'backup of current measurement type
    Public cur_obj_num As Integer() = New Integer(24) {}               'the number of current object
    Public obj_selected As MeasureObject = New MeasureObject()         'current measurement object
    Public obj_selected2 As MeasureObject = New MeasureObject()         'current measurement object
    Public object_list As List(Of List(Of MeasureObject)) = New List(Of List(Of MeasureObject))()        'the list of measurement objects
    Public ID_MY_TEXTBOX As TextBox() = New TextBox(24) {}             'textbox for editing annotation
    Public left_top As Point = New Point()                             'the position left top cornor of picture control in panel
    Public scroll_pos As Point = New Point()                           'the position of scroll bar
    Public anno_num As Integer                                         'the number of annotation object in the list
    Public graphFont As Font                                           'the font for text
    Public undo_num As Integer = 0                                     'count number of undo clicked and reset
    Public graphPen As Pen = New Pen(Color.Black, 1)                   'pen for drawing objects
    Public graphPen_line As Pen = New Pen(Color.Black, 1)              'pen for drawing lines
    Public dashValues As Single() = {5, 2}                             'format dash style of line
    Public line_infor As LineStyle = New LineStyle(1)                  'include the information of style, width, color ...
    Public side_drag As Boolean = False                                'flag of side drawing
    Public showLegend As Boolean = False                              'flag of show legend
    Public scale_style As String = "horizontal"                        'the style of measuring scale horizontal or vertical
    Public scale_value As Integer = 0                                  'the value of measuring scale
    Public scaleUnit As String = "μm"                                 'unit of measuring scale may be cm, mm, ...
    Public ID_TAG_PAGE As TabPage() = New TabPage(24) {}               'tab includes panel
    Public ID_PANEL As Panel() = New Panel(24) {}                      'panel includes picturebox
    Public Shared ID_PICTURE_BOX As PictureBox() = New PictureBox(24) {}      'picturebox for drawing objects
    Public Shared tab_Index As Integer = 0                                    'selected index of tab control
    Public CF As Double = 1.0                                          'the ratio of per pixel by per unit
    Public digit As Integer                                            'The digit of decimal numbers
    Public font_infor As FontInfor = New FontInfor(10)                 'include the information font and color
    Public sel_index As Integer = -1                                   'selected index for object
    Public Shared mCurDrag As PointF = New PointF()                         'the position of mouse cursor
    Public redraw_flag As Boolean                                      'flag for redrawing objects
    Public sel_pt_index As Integer = -1                                'selected index of a point of object
    Public tag_page_flag As Boolean() = New Boolean(24) {}             'specify that target tag page is opened
    Public img_import_flag As Boolean() = New Boolean(24) {}           'specify that you can import image in target tag
    Public name_list As List(Of String) = New List(Of String)          'specify the list of item names
    Public CFList As List(Of String) = New List(Of String)            'specify the names of CF
    Public CFNum As List(Of Double) = New List(Of Double)             'specify the values of CF
    Public CFUnit As List(Of String) = New List(Of String)              'specify the unit of CF
    Public CFSelected As Integer
    Public menuClick As Boolean = False                               'specify whether the menu item is clicked

    'member variable for webcam
    Public videoDevices As FilterInfoCollection                        'usable video devices
    Public videoDevice As VideoCaptureDevice                           'video device currently used 
    Public snapshotCapabilities As VideoCapabilities()
    Public ReadOnly listCamera As ArrayList = New ArrayList()
    Public Shared needSnapshot As Boolean = False
    Public newImage As Bitmap = Nothing                                'used for capturing frame of webcam
    Public ReadOnly _devicename As String = "MultitekHDCam"            'device name
    'Public ReadOnly _devicename As String = "USB Camera"
    'Public ReadOnly _devicename As String = "Lenovo FHD Webcam"
    Public ReadOnly photoList As New System.Windows.Forms.ImageList    'list of captured images
    Public file_counter As Integer = 0                                 'the count of captured images
    Public camera_state As Boolean = False                             'the state of camera is opened or not
    Public imagepath As String = ""                                     'path of folder storing captured images
    Public flag As Boolean = False                                     'flag for live image


    'member variable for setting.ini
    Public cali_path As String = "C:\Users\Public\Documents\Calibration.ini"    'the path of setting.ini
    Public config_path As String = "C:\Users\Public\Documents\Config.ini"    'the path of setting.ini
    Public legend_path As String = "C:\Users\Public\Documents\Legend.ini"    'the path of setting.ini
    Public cali_ini As IniFile
    Public config_ini As IniFile
    Public legend_ini As IniFile

    Public PolyDrawEndFlag As Boolean          'flag specifies that end point of polygen is drawed
    Public CuPolyDrawEndFlag As Boolean        'flag specifies that end point of Curve&polygen is drawed
    Public dumyPoint As Point                  'temp point 

    Public CReadySelectFalg As Boolean                                 'flag specifies whether curve&poly object is ready to select or not. when mouse cursor is in range of object, this becomes true, otherwise this becomes false
    Public CReadySelectArrayIndx As Integer                            'the candidate index of curve object for selection
    Public CRealSelectArrayIndx As Integer                             'the real index of curve object which is selected
    Public CReadySelectArrayIndx_L As Integer                          'the candidate index of label of curve object for selection
    Public CRealSelectArrayIndx_L As Integer                           'the real index of label of curve object which is selected

    Public CuPolyReadySelectArrayIndx As Integer                       'the candidate index of curve&poly object for selection
    Public CuPolyRealSelectArrayIndx As Integer                        'the real index of curve&poly object which is selected
    Public CuPolyReadySelectArrayIndx_L As Integer                     'the candidate index of label of curve&poly object for selection
    Public CuPolyRealSelectArrayIndx_L As Integer                      'the real index of label of curve&poly object which is selected

    Public PolyReadySelectArrayIndx As Integer                         'the candidate index of polygen object for selection
    Public PolyRealSelectArrayIndx As Integer                          'the real index of polygen object which is selected
    Public PolyReadySelectArrayIndx_L As Integer                       'the candidate index of label of polygen object for selection
    Public PolyRealSelectArrayIndx_L As Integer                        'the real index of label of polygen object which is selected

    Public LReadySelectArrayIndx As Integer                            'the candidate index of line object for selection
    Public LRealSelectArrayIndx As Integer                             'the real index of line object which is selected
    Public LReadySelectArrayIndx_L As Integer                          'the candidate index of label of line object for selection
    Public LRealSelectArrayIndx_L As Integer                           'the real index of label of line object which is selected

    Public PReadySelectArrayIndx As Integer                            'the candidate index of point object for selection
    Public PRealSelectArrayIndx As Integer                             'the real index of point object which is selected
    Public PReadySelectArrayIndx_L As Integer                          'the candidate index of label of point object for selection
    Public PRealSelectArrayIndx_L As Integer                           'the real index of label of point object which is selected

    Public CurvePreviousPoint As System.Nullable(Of Point) = Nothing           'previous point of curve object
    Public LinePreviousPoint As System.Nullable(Of Point) = Nothing            'previous point of Line object
    Public PointPreviousPoint As System.Nullable(Of Point) = Nothing           'previous point of Point object
    Public PolyPreviousPoint As System.Nullable(Of Point) = Nothing            'previous point of polygen object
    Public CuPolyPreviousPoint As System.Nullable(Of Point) = Nothing          'previous point of curve&poly object
    Public MousePosPoint As System.Nullable(Of Point) = Nothing                'the position of mouse cursor

    Public XsLinePoint As Integer                                      'X-coordinate of foot of perpendicular
    Public YsLinePoint As Integer                                      'Y-coordinate of foot of perpendicular
    Public PXs, PYs As Integer                                         'points used for drawing max, min lines
    Public FinalPXs, FinalPYs As Integer

    Public DotX, DotY, CDotX, CDotY As Integer                         'points used for dotted lines

    Public OutPointFlag As Boolean                                     'flag specifies whether the foot of perpendicular is in range of object or not
    Public COutPointFlag As Boolean                                    'flag specifies whether the foot of perpendicular is in range of curve&poly object or not
    Public PDotX As Integer                                            'X-coordinate of point which is used for drawing dotted line in case of polygen object
    Public PDotY As Integer                                            'Y-coordinate of point which is used for drawing dotted line in case of polygen object
    Public POutFlag As Boolean                                         'flag specifies whether the foot of perpendicular is in range of polygen object or not

    Public C_PolyObj As PolyObj = New PolyObj()                        'PolyObj
    Public C_PointObj As PointObj = New PointObj()                     'PointObj
    Public C_LineObj As LineObj = New LineObj()                        'LineObj
    Public C_CuPolyObj As CuPolyObj = New CuPolyObj()                  'CuPolyObj
    Public C_CurveObj As CurveObj = New CurveObj()                     'CurveObj
    Public C_CurveObjCopy As CurveObj = New CurveObj()
    Public CurveObjFlag As Boolean
    Public curve_sel_index As Integer
    Public move_line As Boolean
    Public StartPtOfMove As PointF = New PointF()
    Public EndPtOfMove As PointF = New PointF()

    'member variables for edge detection
    Public Shared CalibrationAxis As Integer                    'specify the axis for calibration: 0 init, 1 X-axis, 2 Y-axis
    Public Shared CalibrationChanged As Boolean
    Public Shared EdgeRegionDrawReady As Boolean
    Public Shared EdgeRegionDrawed As Boolean
    Public Shared FirstPtOfEdge As PointF = New PointF()
    Public Shared SecondPtOfEdge As PointF = New PointF()
    Public Shared MouseDownFlag As Boolean
    Public Shared MouseRDownFlag As Boolean
    Public strColList As List(Of String) = New List(Of String)        'the list of color names
    Public colList As List(Of Integer()) = New List(Of Integer())
    Public Shared Obj_Seg As SegObject = New SegObject()

    Public RoiSize As Integer
    Public RoiRect As RectangleF
    Public RoiUnit As String = "μm×μm"
    Public controlIntialized As Boolean                            'state indentifying whether controls are intialized or not

    Public Shared CaliName As List(Of String) = New List(Of String)
    Public PhaseName As List(Of String) = New List(Of String)
    Public PhaseVal As List(Of Integer) = New List(Of Integer)
    Public Shared PhaseNameList As List(Of List(Of String)) = New List(Of List(Of String))
    Public Shared PhaseValList As List(Of List(Of Integer)) = New List(Of List(Of Integer))
    Public Sub New()
        InitializeComponent()
        InitializeCustomeComeponent()
    End Sub

    Private Const EM_GETLINECOUNT As Integer = &HBA
    <DllImport("user32", EntryPoint:="SendMessageA", CharSet:=CharSet.Ansi, SetLastError:=True, ExactSpelling:=True)>
    Private Shared Function SendMessage(ByVal hwnd As Integer, ByVal wMsg As Integer, ByVal wParam As Integer, ByVal lParam As Integer) As Integer
    End Function


#Region "Main Form Methods"
    'Initialize custome controls
    Private Sub InitializeCustomeComeponent()
        graphFont = New Font("Arial", 10, FontStyle.Regular)
        For i = 0 To 24

            ID_TAG_PAGE(i) = New TabPage()
            ID_PANEL(i) = New Panel()
            ID_PICTURE_BOX(i) = New PictureBox()
            ID_MY_TEXTBOX(i) = New TextBox()

            ID_TAG_CTRL.Controls.Add(ID_TAG_PAGE(i))

            ID_TAG_PAGE(i).Location = New Point(4, 24)
            ID_TAG_PAGE(i).Name = "ID_TAG_PAGE" & i.ToString()
            ID_TAG_PAGE(i).Padding = New Padding(3)
            ID_TAG_PAGE(i).Size = New Size(800, 700)
            ID_TAG_PAGE(i).Text = ""
            ID_TAG_PAGE(i).UseVisualStyleBackColor = True
            ID_TAG_PAGE(i).Controls.Add(ID_PANEL(i))

            ID_PANEL(i).Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
            ID_PANEL(i).AutoScroll = True
            ID_PANEL(i).AutoSizeMode = AutoSizeMode.GrowAndShrink
            ID_PANEL(i).BackColor = Color.Gray
            ID_PANEL(i).Location = New Point(0, 1)
            ID_PANEL(i).Name = "ID_PANEL" & i.ToString()
            ID_PANEL(i).Size = New Size(800, 700)
            AddHandler ID_PANEL(i).Scroll, New ScrollEventHandler(AddressOf ID_PANEL_Scroll)
            AddHandler ID_PANEL(i).SizeChanged, New EventHandler(AddressOf ID_PANEL_SizeChanged)
            AddHandler ID_PANEL(i).MouseWheel, New MouseEventHandler(AddressOf ID_PANEL_MouseWheel)
            ID_PANEL(i).Controls.Add(ID_PICTURE_BOX(i))

            ID_PICTURE_BOX(i).BackColor = Color.Gray
            ID_PICTURE_BOX(i).Location = New Point(0, -1)
            ID_PICTURE_BOX(i).Name = "ID_PICTURE_BOX" & i.ToString()
            ID_PICTURE_BOX(i).Size = New Size(800, 700)
            ID_PICTURE_BOX(i).SizeMode = PictureBoxSizeMode.AutoSize
            ID_PICTURE_BOX(i).TabIndex = 0
            ID_PICTURE_BOX(i).TabStop = False
            ID_PICTURE_BOX(i).Image = Nothing
            AddHandler ID_PICTURE_BOX(i).MouseDown, New MouseEventHandler(AddressOf ID_PICTURE_BOX_MouseDown)
            AddHandler ID_PICTURE_BOX(i).MouseMove, New MouseEventHandler(AddressOf ID_PICTURE_BOX_MouseMove)
            AddHandler ID_PICTURE_BOX(i).MouseDoubleClick, New MouseEventHandler(AddressOf ID_PICTURE_BOX_MouseDoubleClick)
            AddHandler ID_PICTURE_BOX(i).MouseUp, New MouseEventHandler(AddressOf ID_PICTURE_BOX_MouseUp)

            AddHandler ID_PICTURE_BOX(i).Paint, New PaintEventHandler(AddressOf ID_PICTURE_BOX_Paint)

            ID_PICTURE_BOX(i).Controls.Add(ID_MY_TEXTBOX(i))

            ID_MY_TEXTBOX(i).Name = "ID_MY_TEXTBOX"
            ID_MY_TEXTBOX(i).Multiline = True
            ID_MY_TEXTBOX(i).AutoSize = False
            ID_MY_TEXTBOX(i).Visible = False
            ID_MY_TEXTBOX(i).Font = graphFont
            AddHandler ID_MY_TEXTBOX(i).TextChanged, New EventHandler(AddressOf ID_MY_TEXTBOX_TextChanged)
        Next

        'remove unnessary tab pages
        For i = 1 To 24
            ID_TAG_CTRL.TabPages.Remove(ID_TAG_PAGE(i))
            tag_page_flag(i) = False
            img_import_flag(i) = True
        Next

        tag_page_flag(0) = True
        img_import_flag(0) = True

        Dim mat As Mat = Nothing

        For i = 0 To 24
            Dim list As List(Of MeasureObject) = New List(Of MeasureObject)()

            object_list.Add(list)
            originImageList.Add(mat)
            resizedImageList.Add(mat)
            currentImageList.Add(mat)
            file_names.Add("")
            zoomFactor(i) = 1.0
        Next

        Obj_Seg.circleObj = New CircleObj()
        Obj_Seg.sectObj = New InterSectionObj()
        Obj_Seg.phaseSegObj = New PhaseSeg()
        Obj_Seg.BlobSegObj = New BlobList()
        Obj_Seg.ceptObj = New InterCeptObj()
    End Sub

    ''' <summary>
    ''' Initialize variables.
    ''' </summary>
    Public Sub initVar()

        CurvePreviousPoint = Nothing
        CReadySelectFalg = False
        LReadySelectArrayIndx = -1
        LRealSelectArrayIndx = -1
        PolyDrawEndFlag = False
        PReadySelectArrayIndx = -1
        PRealSelectArrayIndx = -1
        PolyReadySelectArrayIndx = -1
        PolyRealSelectArrayIndx = -1
        CReadySelectArrayIndx = -1
        CRealSelectArrayIndx = -1
        CuPolyReadySelectArrayIndx = -1
        CuPolyRealSelectArrayIndx = -1
        dumyPoint.X = -1
        dumyPoint.Y = -1

        PRealSelectArrayIndx_L = -1
        CRealSelectArrayIndx_L = -1
        PolyRealSelectArrayIndx_L = -1
        PolyRealSelectArrayIndx_L = -1
        PReadySelectArrayIndx_L = -1
        CReadySelectArrayIndx_L = -1
        PolyReadySelectArrayIndx_L = -1
        PolyReadySelectArrayIndx_L = -1
        CuPolyRealSelectArrayIndx_L = -1
        CuPolyReadySelectArrayIndx_L = -1

        COutPointFlag = False

        anno_num = -1
        MeasureTypePrev = -1
        ID_BTN_CUR_COL.BackColor = Color.Black
        ID_BTN_TEXT_COL.BackColor = Color.Black
        ID_COMBO_LINE_SHAPE.SelectedIndex = 0

        obj_selected.Refresh()
        curMeasureType = -1
        sel_index = -1
        curve_sel_index = -1

    End Sub

    'Initialize the color of measuring buttons
    Private Sub Initialize_Button_Colors()
        ID_BTN_ANGLE.BackColor = Color.LightBlue
        ID_BTN_ANNOTATION.BackColor = Color.LightBlue
        ID_BTN_ARC.BackColor = Color.LightBlue
        ID_BTN_LINE_ALIGN.BackColor = Color.LightBlue
        ID_BTN_LINE_HOR.BackColor = Color.LightBlue
        ID_BTN_LINE_PARA.BackColor = Color.LightBlue
        ID_BTN_LINE_VER.BackColor = Color.LightBlue
        ID_BTN_PENCIL.BackColor = Color.LightBlue
        ID_BTN_P_LINE.BackColor = Color.LightBlue
        ID_BTN_RADIUS.BackColor = Color.LightBlue
        ID_BTN_SCALE.BackColor = Color.LightBlue
        ID_BTN_C_LINE.BackColor = Color.LightBlue
        ID_BTN_C_POLY.BackColor = Color.LightBlue
        ID_BTN_C_POINT.BackColor = Color.LightBlue
        ID_BTN_C_CURVE.BackColor = Color.LightBlue
        ID_BTN_C_CUPOLY.BackColor = Color.LightBlue
        ID_BTN_C_SEL.BackColor = Color.LightBlue
    End Sub

    'get setting information from ini file
    Private Sub GetCalibrationInfo()
        If IO.File.Exists(cali_path) Then
            cali_ini = New IniFile(cali_path)
            Dim Keys As ArrayList = cali_ini.GetKeys("CF")

            Dim myEnumerator As System.Collections.IEnumerator = Keys.GetEnumerator()
            While myEnumerator.MoveNext()
                If myEnumerator.Current.Name = "index" Then
                    CFSelected = CInt(myEnumerator.Current.value)
                Else
                    Dim CF_key = myEnumerator.Current.Name
                    Dim line As String = myEnumerator.Current.value
                    Dim parse_num = line.IndexOf(":")
                    Dim CF_val = CDbl(line.Substring(0, parse_num))
                    Dim CF_Unit = line.Substring(parse_num + 1)

                    CFList.Add(CF_key)
                    CFNum.Add(CF_val)
                    CFUnit.Add(CF_Unit)
                End If
            End While

            CaliName.Clear()
            PhaseNameList.Clear()
            PhaseValList.Clear()
            Keys.Clear()
            Keys = cali_ini.GetKeys("Phase")
            myEnumerator = Keys.GetEnumerator()
            While myEnumerator.MoveNext()
                Dim line As String = myEnumerator.Current.value
                CaliName.Add(line)
            End While
            For i = 0 To CaliName.Count - 1
                PhaseName.Clear()
                PhaseVal.Clear()
                Keys.Clear()
                Keys = cali_ini.GetKeys(CaliName(i))
                myEnumerator = Keys.GetEnumerator()
                While myEnumerator.MoveNext()
                    Dim Name = myEnumerator.Current.Name
                    Dim line As String = myEnumerator.Current.value
                    Dim parse_num = line.IndexOf(":")
                    Dim Val1 = CInt(line.Substring(0, parse_num))
                    Dim Val2 = CInt(line.Substring(parse_num + 1))

                    PhaseName.Add(Name)
                    If PhaseVal.Count = 0 Then
                        PhaseVal.Add(Val1)
                    End If
                    PhaseVal.Add(Val2)
                End While
                PhaseNameList.Add(PhaseName.ToList())
                PhaseValList.Add(PhaseVal.ToList())
            Next
        Else
            CFList.Add("1.0X")
            CFNum.Add(1.0)
            CFList.Add("1.25X")
            CFNum.Add(1.25)
            CFList.Add("1.5X")
            CFNum.Add(1.5)
            CFList.Add("2.0X")
            CFNum.Add(2.0)
            CFList.Add("2.5X")
            CFNum.Add(2.5)
            CFList.Add("3.5X")
            CFNum.Add(3.5)
            CFList.Add("5.0X")
            CFNum.Add(5.0)
            CFList.Add("7.5X")
            CFNum.Add(7.5)
            CFList.Add("10.0X")
            CFNum.Add(10.0)
            For i = 0 To 8
                CFUnit.Add("μm")
            Next

            CFSelected = 0
        End If

        CF = CFNum(CFSelected)
        scaleUnit = CFUnit(CFSelected)
    End Sub

    Private Sub GetConfigInfo()
        If IO.File.Exists(config_path) Then
            config_ini = New IniFile(config_path)
            Dim Keys As ArrayList = config_ini.GetKeys("Config")
            Dim myEnumerator As System.Collections.IEnumerator = Keys.GetEnumerator()
            While myEnumerator.MoveNext()
                If myEnumerator.Current.Name = "areaUnit" Then
                    RoiUnit = myEnumerator.Current.value
                ElseIf myEnumerator.Current.Name = "roiSize" Then
                    RoiSize = CInt(myEnumerator.Current.value)
                Else
                    digit = CInt(myEnumerator.Current.value)
                End If
            End While
            ID_NUM_DIGIT.Value = digit
        Else
            RoiUnit = "μm×μm"
            RoiSize = 0
            digit = 0
            ID_NUM_DIGIT.Value = digit
        End If

    End Sub
    Private Sub GetLegendInfo()
        If IO.File.Exists(legend_path) Then
            legend_ini = New IniFile(legend_path)
            Dim Keys As ArrayList = legend_ini.GetKeys("name")
            Dim myEnumerator As System.Collections.IEnumerator = Keys.GetEnumerator()
            Dim cnt As Integer = 0
            myEnumerator = Keys.GetEnumerator()
            While myEnumerator.MoveNext()
                Dim line As String = myEnumerator.Current.value
                name_list.Add(line)
            End While
        Else
            name_list.Add("Line")
            name_list.Add("Angle")
            name_list.Add("Arc")
            name_list.Add("Scale")
        End If

    End Sub
    Private Sub UpdateCalibrationInfo()
        If IO.File.Exists(cali_path) Then
            If cali_ini IsNot Nothing Then
                cali_ini.DeleteSection("CF")

                Dim Keys As ArrayList = cali_ini.GetKeys("Phase")
                Dim myEnumerator As System.Collections.IEnumerator = Keys.GetEnumerator()
                Dim cali_Name As List(Of String) = New List(Of String)
                While myEnumerator.MoveNext()
                    Dim line As String = myEnumerator.Current.value
                    cali_Name.Add(line)
                End While
                cali_ini.DeleteSection("Phase")
                For i = 0 To cali_Name.Count - 1
                    cali_ini.DeleteSection(cali_Name(i))
                Next
            End If
        Else
            cali_ini = New IniFile(cali_path)
        End If

        cali_ini.AddSection("CF")
        cali_ini.AddKey("index", CFSelected, "CF")
        For i = 0 To CFList.Count - 1
            Dim key = CFList(i)
            Dim value = CFNum(i) & ":" & CFUnit(i)
            cali_ini.AddKey(key, value, "CF")
        Next
        'cali_ini.Sort()

        cali_ini.AddSection("Phase")
        For i = 0 To CaliName.Count - 1
            Dim key = "No" & (i + 1)
            Dim value = CaliName(i)
            cali_ini.AddKey(key, value, "Phase")
        Next

        For i = 0 To CaliName.Count - 1
            cali_ini.AddSection(CaliName(i))
            PhaseName = PhaseNameList(i)
            PhaseVal = PhaseValList(i)
            For j = 0 To PhaseName.Count - 1
                Dim key = PhaseName(j)
                Dim value = PhaseVal(j) & ":" & PhaseVal(j + 1)
                cali_ini.AddKey(key, value, CaliName(i))
            Next

        Next
        'cali_ini.Sort()
        cali_ini.Save(cali_path)
    End Sub

    Private Sub UpdateConfigInfo()
        If IO.File.Exists(config_path) Then
            If config_ini IsNot Nothing Then
                config_ini.ChangeValue("digit", "Config", digit.ToString())
                config_ini.ChangeValue("areaUnit", "Config", RoiUnit)
                config_ini.ChangeValue("roiSize", "Config", RoiSize)
                config_ini.Save(config_path)
            End If
        Else
            'set default value when ini file does Not exist in document folder
            config_ini = New IniFile(config_path)

            config_ini.AddSection("Config")
            config_ini.AddKey("digit", digit.ToString(), "Config")
            config_ini.AddKey("areaUnit", RoiUnit, "Config")
            config_ini.AddKey("roiSize", RoiSize, "Config")

            config_ini.Sort()
            config_ini.Save(config_path)
        End If
    End Sub

    Private Sub GetInformationAndGetReady()
        Try
            initVar()
            Initialize_Button_Colors()
            GetCalibrationInfo()
            GetConfigInfo()
            GetLegendInfo()

            If My.Settings.imagefilepath.Equals("") Then
                imagepath = "MyImages"
                My.Settings.imagefilepath = imagepath
                My.Settings.Save()
                txtbx_imagepath.Text = imagepath
            Else
                imagepath = My.Settings.imagefilepath
                txtbx_imagepath.Text = My.Settings.imagefilepath
            End If

            Dim colType As Type = GetType(System.Drawing.Color)

            For Each prop As PropertyInfo In colType.GetProperties()
                If prop.PropertyType Is GetType(System.Drawing.Color) Then
                    strColList.Add(prop.Name)
                    Dim Col = Color.FromName(prop.Name)
                    Dim Col_Array = New Integer() {Col.B, Col.G, Col.R}
                    colList.Add(Col_Array)
                End If
            Next
            DeleteImages(imagepath)
            Createdirectory(imagepath)
        Catch ex As Exception
            MessageBox.Show(ex.Message.ToString())
        End Try

    End Sub

    'check license information when main dialog is loading
    Private Sub Main_Form_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            Timer1.Interval = 30
            Timer1.Start()
        Catch ex As Exception
            MessageBox.Show(ex.Message.ToString())

        End Try
    End Sub

    'change the color of button when it is clicked
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        If MeasureTypePrev <> curMeasureType Then
            Initialize_Button_Colors()
            MeasureTypePrev = curMeasureType
            Select Case curMeasureType
                Case MeasureType.lineAlign
                    If menuClick = False Then ID_BTN_LINE_ALIGN.BackColor = Color.DodgerBlue
                    ID_STATUS_LABEL.Text = "Calculates a line through two input points."
                Case MeasureType.lineHorizontal
                    If menuClick = False Then ID_BTN_LINE_HOR.BackColor = Color.DodgerBlue
                    ID_STATUS_LABEL.Text = "Calculates a horizontal line through two input points."
                Case MeasureType.lineVertical
                    If menuClick = False Then ID_BTN_LINE_VER.BackColor = Color.DodgerBlue
                    ID_STATUS_LABEL.Text = "Calculates a vertical line through two input points."
                Case MeasureType.angle
                    If menuClick = False Then ID_BTN_ARC.BackColor = Color.DodgerBlue
                    ID_STATUS_LABEL.Text = "Calculates angle through three points in degree."
                Case MeasureType.arc
                    If menuClick = False Then ID_BTN_RADIUS.BackColor = Color.DodgerBlue
                    ID_STATUS_LABEL.Text = "Calculates a arc through three input points."
                Case MeasureType.annotation
                    If menuClick = False Then ID_BTN_ANNOTATION.BackColor = Color.DodgerBlue
                    ID_STATUS_LABEL.Text = "Add a annotation."
                Case MeasureType.angle2Line
                    If menuClick = False Then ID_BTN_ANGLE.BackColor = Color.DodgerBlue
                    ID_STATUS_LABEL.Text = "Calculates angle through two lines in degree."
                Case MeasureType.lineParallel
                    If menuClick = False Then ID_BTN_LINE_PARA.BackColor = Color.DodgerBlue
                    ID_STATUS_LABEL.Text = "Calculates a line through two parallel lines."
                Case MeasureType.pencil
                    If menuClick = False Then ID_BTN_PENCIL.BackColor = Color.DodgerBlue
                    ID_STATUS_LABEL.Text = "Draw a line through two input points."
                Case MeasureType.ptToLine
                    If menuClick = False Then ID_BTN_P_LINE.BackColor = Color.DodgerBlue
                    ID_STATUS_LABEL.Text = "Calculates a line between a point and a line."
                Case MeasureType.measureScale
                    If menuClick = False Then ID_BTN_SCALE.BackColor = Color.DodgerBlue
                    ID_STATUS_LABEL.Text = "Insert a measuring scale."
                Case MeasureType.objLine
                    If menuClick = False Then ID_BTN_C_LINE.BackColor = Color.DodgerBlue
                    ID_STATUS_LABEL.Text = "Draw a line."
                Case MeasureType.objPoly
                    If menuClick = False Then ID_BTN_C_POLY.BackColor = Color.DodgerBlue
                    ID_STATUS_LABEL.Text = "Draw a polygen."
                Case MeasureType.objPoint
                    If menuClick = False Then ID_BTN_C_POINT.BackColor = Color.DodgerBlue
                    ID_STATUS_LABEL.Text = "Draw a point."
                Case MeasureType.objCurve
                    If menuClick = False Then ID_BTN_C_CURVE.BackColor = Color.DodgerBlue
                    ID_STATUS_LABEL.Text = "Draw a curve."
                Case MeasureType.objCuPoly
                    If menuClick = False Then ID_BTN_C_CUPOLY.BackColor = Color.DodgerBlue
                    ID_STATUS_LABEL.Text = "Draw a curve&polygen."
                Case MeasureType.objSel
                    If menuClick = False Then ID_BTN_C_SEL.BackColor = Color.DodgerBlue
                    ID_STATUS_LABEL.Text = "select objects."
            End Select

        End If

        If controlIntialized = False Then
            controlIntialized = True
            GetInformationAndGetReady()
            ID_STATUS_LABEL.Text = "loading..."
        End If

        If CalibrationChanged Then
            UpdateCalibrationInfo()
            CF = CFNum(CFSelected)
            scaleUnit = CFUnit(CFSelected)
            ID_PICTURE_BOX(tab_Index).DrawObjList(object_list.ElementAt(tab_Index), digit, CF, False)
            ID_LISTVIEW.LoadObjectList(object_list.ElementAt(tab_Index), CF, digit, scaleUnit, name_list)
            CalibrationChanged = False
        End If
    End Sub

    'open camera
    Private Sub ID_MENU_OPEN_CAM_Click(sender As Object, e As EventArgs) Handles ID_MENU_OPEN_CAM.Click
        Try
            OpenCamera()
            SelectResolution(videoDevice, CameraResolutionsCB)
            If Not My.Settings.camresindex.Equals("") Then
                CameraResolutionsCB.SelectedIndex = My.Settings.camresindex + 1
            End If
            ID_TAG_PAGE(0).Text = "WebCam"
        Catch excpt As Exception
            MessageBox.Show(excpt.Message)
        End Try

    End Sub

    'close camera
    Private Sub ID_MENU_CLOSE_CAM_Click(sender As Object, e As EventArgs) Handles ID_MENU_CLOSE_CAM.Click
        Try
            CloseCamera()
            ID_PICTURE_BOX(0).Image = Nothing
            ID_PICTURE_BOX_CAM.Image = Nothing
            ID_TAG_PAGE(0).Text = ""
        Catch excpt As Exception
            MessageBox.Show(excpt.Message)
        End Try

    End Sub

    'import image and draw it to picturebox
    'format variables
    Private Sub ID_MENU_OPEN_Click(sender As Object, e As EventArgs) Handles ID_MENU_OPEN.Click

        Dim filter = "All Files|*.*|JPEG Files|*.jpg|PNG Files|*.png|BMP Files|*.bmp"
        Dim title = "Open"

        Dim start As Integer = tab_Index
        img_import_flag(tab_Index) = True

        Dim img_cnt = ID_PICTURE_BOX(0).LoadImageFromFiles(filter, title, originImageList, resizedImageList, currentImageList, start, img_import_flag, file_names)

        If img_cnt >= 1 Then
            initVar()
            ID_PICTURE_BOX(start).Image = Nothing
        End If
        Dim added_tag = 0
        While added_tag < img_cnt
            If start > 24 Then Exit While

            If tag_page_flag(start) = True AndAlso ID_PICTURE_BOX(start).Image IsNot Nothing Then
                start = start + 1
                Continue While
            End If

            If tag_page_flag(start) = False Then
                ID_TAG_CTRL.TabPages.Add(ID_TAG_PAGE(start))
                tag_page_flag(start) = True
            End If

            cur_obj_num(start) = 0
            zoomFactor(start) = 1.0

            Dim img = originImageList.ElementAt(start)
            ID_PICTURE_BOX(start).Invoke(New Action(Sub() ID_PICTURE_BOX(start).Image = img.ToBitmap()))

            left_top = ID_PICTURE_BOX(start).CenteringImage(ID_PANEL(start))
            Enumerable.ElementAt(Of List(Of MeasureObject))(object_list, start).Clear()
            img_import_flag(start) = False
            ID_TAG_PAGE(start).Text = file_names(start)

            start = start + 1
            added_tag = added_tag + 1
        End While

        start = Math.Max(start - 1, 0)
        ID_LISTVIEW.LoadObjectList(object_list.ElementAt(start), CF, digit, scaleUnit, name_list)
        ID_TAG_CTRL.SelectedTab = ID_TAG_PAGE(start)
        If img_cnt >= 1 Then
            CalcRoiRect(scaleUnit, CFNum(CFSelected), RoiUnit, RoiSize, originImageList(tab_Index).Width, originImageList(tab_Index).Height, RoiRect)
        End If

    End Sub

    'export image to jpg
    Private Sub ID_MENU_SAVE_Click(sender As Object, e As EventArgs) Handles ID_MENU_SAVE.Click
        Dim filter = "JPEG Files|*.jpg"
        Dim title = "Save"
        ID_PICTURE_BOX(tab_Index).SaveImageInFile(filter, title)
    End Sub

    'save object information as excel file
    Private Sub ID_MENU_SAVE_XLSX_Click(sender As Object, e As EventArgs) Handles ID_MENU_SAVE_XLSX.Click
        Dim filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*"
        Dim title = "Save"
        SaveListToExcel(ID_LISTVIEW, filter, title)
    End Sub

    'save object list and image as excel file
    Private Sub ID_MENU_EXPORT_REPORT_Click(sender As Object, e As EventArgs) Handles ID_MENU_EXPORT_REPORT.Click
        Dim filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*"
        Dim title = "Save"
        ID_PICTURE_BOX(tab_Index).SaveReportToExcel(ID_LISTVIEW, filter, title, object_list(tab_Index), digit, CF, scaleUnit)
    End Sub

    'exit the program
    Private Sub ID_MENU_EXIT_Click(sender As Object, e As EventArgs) Handles ID_MENU_EXIT.Click
        Call Application.Exit()
    End Sub

    'set current measurement type as line_align
    'reset the current object
    Private Sub ID_BTN_LINE_ALIGN_Click(sender As Object, e As EventArgs) Handles ID_BTN_LINE_ALIGN.Click
        menuClick = False
        obj_selected.Refresh()
        curMeasureType = MeasureType.lineAlign
        obj_selected.measuringType = curMeasureType
    End Sub

    Private Sub LINEToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LINEToolStripMenuItem.Click
        menuClick = True
        obj_selected.Refresh()
        curMeasureType = MeasureType.lineAlign
        obj_selected.measuringType = curMeasureType
    End Sub

    'set current measurement type as line_horizontal
    'reset the current object
    Private Sub ID_BTN_LINE_HOR_Click(sender As Object, e As EventArgs) Handles ID_BTN_LINE_HOR.Click
        menuClick = False
        obj_selected.Refresh()
        curMeasureType = MeasureType.lineHorizontal
        obj_selected.measuringType = curMeasureType
    End Sub

    Private Sub HORIZONTALLINEToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles HORIZONTALLINEToolStripMenuItem.Click
        menuClick = True
        obj_selected.Refresh()
        curMeasureType = MeasureType.lineHorizontal
        obj_selected.measuringType = curMeasureType
    End Sub

    'set current measurement type as line_vertical
    'reset the current object
    Private Sub ID_BTN_LINE_VER_Click(sender As Object, e As EventArgs) Handles ID_BTN_LINE_VER.Click
        menuClick = False
        obj_selected.Refresh()
        curMeasureType = MeasureType.lineVertical
        obj_selected.measuringType = curMeasureType
    End Sub

    Private Sub VERTICALLINEToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles VERTICALLINEToolStripMenuItem.Click
        menuClick = True
        obj_selected.Refresh()
        curMeasureType = MeasureType.lineVertical
        obj_selected.measuringType = curMeasureType
    End Sub

    'set current measurement type as line parallel
    'reset the current object
    Private Sub ID_BTN_LINE_PARA_Click(sender As Object, e As EventArgs) Handles ID_BTN_LINE_PARA.Click
        menuClick = False
        obj_selected.Refresh()
        curMeasureType = MeasureType.lineParallel
        obj_selected.measuringType = curMeasureType
    End Sub

    Private Sub PARALLELLINEToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PARALLELLINEToolStripMenuItem.Click
        menuClick = True
        obj_selected.Refresh()
        curMeasureType = MeasureType.lineParallel
        obj_selected.measuringType = curMeasureType
    End Sub

    'set current measurement type as angle
    'reset the current object
    Private Sub ID_BTN_ARC_Click(sender As Object, e As EventArgs) Handles ID_BTN_ARC.Click
        menuClick = False
        obj_selected.Refresh()
        curMeasureType = MeasureType.angle
        obj_selected.measuringType = curMeasureType
    End Sub

    Private Sub ANGLETHROUGHTHREEPOINTSToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ANGLETHROUGHTHREEPOINTSToolStripMenuItem.Click
        menuClick = True
        obj_selected.Refresh()
        curMeasureType = MeasureType.angle
        obj_selected.measuringType = curMeasureType
    End Sub

    'set current measurement type as angle far
    'reset the current object
    Private Sub ID_BTN_ANGLE_Click(sender As Object, e As EventArgs) Handles ID_BTN_ANGLE.Click
        menuClick = False
        obj_selected.Refresh()
        curMeasureType = MeasureType.angle2Line
        obj_selected.measuringType = curMeasureType
    End Sub

    Private Sub ANGLETHROUGHTWOLINESToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ANGLETHROUGHTWOLINESToolStripMenuItem.Click
        menuClick = True
        obj_selected.Refresh()
        curMeasureType = MeasureType.angle2Line
        obj_selected.measuringType = curMeasureType
    End Sub

    'set current measurement type as radius
    'reset the current object
    Private Sub ID_BTN_RADIUS_Click(sender As Object, e As EventArgs) Handles ID_BTN_RADIUS.Click
        menuClick = False
        obj_selected.Refresh()
        curMeasureType = MeasureType.arc
        obj_selected.measuringType = curMeasureType
    End Sub

    Private Sub ARCToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ARCToolStripMenuItem.Click
        menuClick = True
        obj_selected.Refresh()
        curMeasureType = MeasureType.arc
        obj_selected.measuringType = curMeasureType
    End Sub

    'set current measurement type as annotation
    'reset the current object
    Private Sub ID_BTN_ANNOTATION_Click(sender As Object, e As EventArgs) Handles ID_BTN_ANNOTATION.Click
        menuClick = False
        obj_selected.Refresh()
        curMeasureType = MeasureType.annotation
        obj_selected.measuringType = curMeasureType
    End Sub

    Private Sub ANNOTATIONToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ANNOTATIONToolStripMenuItem.Click
        menuClick = True
        obj_selected.Refresh()
        curMeasureType = MeasureType.annotation
        obj_selected.measuringType = curMeasureType
    End Sub

    'set current measurement type as draw line
    'reset the current object
    Private Sub ID_BTN_PENCIL_Click(sender As Object, e As EventArgs) Handles ID_BTN_PENCIL.Click
        menuClick = False
        obj_selected.Refresh()
        curMeasureType = MeasureType.pencil
        obj_selected.measuringType = curMeasureType
    End Sub

    'set current measurement type as point to line
    'reset the current object
    Private Sub ID_BTN_P_LINE_Click(sender As Object, e As EventArgs) Handles ID_BTN_P_LINE.Click
        menuClick = False
        obj_selected.Refresh()
        curMeasureType = MeasureType.ptToLine
        obj_selected.measuringType = curMeasureType
    End Sub

    Private Sub DISTANCEFROMPOINTTOLINEToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DISTANCEFROMPOINTTOLINEToolStripMenuItem.Click
        menuClick = True
        obj_selected.Refresh()
        curMeasureType = MeasureType.ptToLine
        obj_selected.measuringType = curMeasureType
    End Sub

    'set measureing scale 
    'set current measurement type as measuring scale
    'reset the current object
    Private Sub ID_BTN_SCALE_Click(sender As Object, e As EventArgs) Handles ID_BTN_SCALE.Click
        menuClick = False
        Dim form As ID_FORM_SCALE = New ID_FORM_SCALE(scaleUnit)
        If form.ShowDialog() = DialogResult.OK Then
            scale_style = form.scale_style
            scale_value = form.scale_value
            scaleUnit = form.scaleUnit

            obj_selected.Refresh()
            curMeasureType = MeasureType.measureScale
            obj_selected.measuringType = curMeasureType

            obj_selected.scaleObject.style = scale_style
            obj_selected.scaleObject.length = scale_value
        End If

    End Sub

    'set current measurement type as circle_fixed
    Private Sub ANGLEOFFIXEDDIAMETERToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ANGLEOFFIXEDDIAMETERToolStripMenuItem.Click
        menuClick = True
        obj_selected.Refresh()
        curMeasureType = MeasureType.arcFixed
        obj_selected.measuringType = curMeasureType

        ID_STATUS_LABEL.Text = "Drawing a circle which has fixed radius"
        Dim form = New LenDiameter()
        If form.ShowDialog() = DialogResult.OK Then
            obj_selected.scaleObject.length = CSng(form.ID_TEXT_FIXED.Text)
            obj_selected.arc = obj_selected.scaleObject.length / ID_PICTURE_BOX(tab_Index).Width
        End If
    End Sub

    'set current measurement type as line_fixed
    Private Sub LINEOFFIXEDLENGTHToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LINEOFFIXEDLENGTHToolStripMenuItem.Click
        menuClick = True
        obj_selected.Refresh()
        curMeasureType = MeasureType.lineFixed
        obj_selected.measuringType = curMeasureType

        ID_STATUS_LABEL.Text = "Drawing a line which has fixed length"
        Dim form = New LenDiameter()
        If form.ShowDialog() = DialogResult.OK Then
            obj_selected.scaleObject.length = CSng(form.ID_TEXT_FIXED.Text)
            obj_selected.length = obj_selected.scaleObject.length / ID_PICTURE_BOX(tab_Index).Width
        End If
    End Sub

    'set current measurement type as angle_fixed
    Private Sub FIXEDANGLEToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FIXEDANGLEToolStripMenuItem.Click
        menuClick = True
        obj_selected.Refresh()
        curMeasureType = MeasureType.angleFixed
        obj_selected.measuringType = curMeasureType

        ID_STATUS_LABEL.Text = "Drawing a angle which has fixed angle"
        Dim form = New LenDiameter()
        If form.ShowDialog() = DialogResult.OK Then
            obj_selected.angle = CSng(form.ID_TEXT_FIXED.Text)
        End If
    End Sub

    'set move_line to ture so that you can move curves line object
    Private Sub MOVELINEOBJECTToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MOVELINEOBJECTToolStripMenuItem.Click
        move_line = True
        ID_STATUS_LABEL.Text = "Duplicating Curve Line Object"
    End Sub

    Private Sub DrawAndCentering()
        left_top = ID_PICTURE_BOX(tab_Index).CenteringImage(ID_PANEL(tab_Index))
        scroll_pos.X = ID_PANEL(tab_Index).HorizontalScroll.Value
        scroll_pos.Y = ID_PANEL(tab_Index).VerticalScroll.Value
        ID_PICTURE_BOX(tab_Index).DrawObjList(object_list.ElementAt(tab_Index), digit, CF, False)
        Dim flag = False
        If sel_index >= 0 Then flag = True
        ID_PICTURE_BOX(tab_Index).DrawObjSelected(obj_selected, flag)
        If ID_MY_TEXTBOX(tab_Index).Visible = True Then
            Dim obj_anno = object_list.ElementAt(tab_Index).ElementAt(anno_num)
            Dim st_pt As Point = New Point(obj_anno.drawPoint.X * ID_PICTURE_BOX(tab_Index).Width, obj_anno.drawPoint.Y * ID_PICTURE_BOX(tab_Index).Height)
            ID_MY_TEXTBOX(tab_Index).UpdateLocation(st_pt, left_top, scroll_pos)
        End If
    End Sub

    'zoom image
    Private Sub Zoom_Image()
        Try
            Dim ratio = zoomFactor(tab_Index)
            Dim zoomed = ZoomImage(ratio, currentImageList(tab_Index))
            Dim zoomed_ori = ZoomImage(ratio, originImageList(tab_Index))
            DisposeElemOfList(resizedImageList, zoomed_ori, tab_Index)

            ID_PICTURE_BOX(tab_Index).Image = zoomed.ToBitmap()
            zoomed.Dispose()
            DrawAndCentering()

        Catch ex As Exception
            MessageBox.Show(ex.Message.ToString())
        End Try
    End Sub

    'zoom in image and draw it to picturebox
    Private Sub ID_BTN_ZOON_IN_Click(sender As Object, e As EventArgs) Handles ID_BTN_ZOON_IN.Click
        menuClick = False
        zoomFactor(tab_Index) *= 1.1
        Zoom_Image()
        ID_STATUS_LABEL.Text = "Zoom In"
    End Sub

    'zoom out image and draw it to picturebox
    Private Sub ID_BTN_ZOOM_OUT_Click(sender As Object, e As EventArgs) Handles ID_BTN_ZOOM_OUT.Click
        menuClick = False
        zoomFactor(tab_Index) /= 1.1
        Zoom_Image()
        ID_STATUS_LABEL.Text = "Zoom Out"
    End Sub

    'zoom in
    Private Sub ZOOMINToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ZOOMINToolStripMenuItem.Click
        menuClick = True
        zoomFactor(tab_Index) *= 1.1
        Zoom_Image()
        ID_STATUS_LABEL.Text = "Zoom In"
    End Sub

    'zoom out
    Private Sub ZOOMOUTToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ZOOMOUTToolStripMenuItem.Click
        menuClick = True
        zoomFactor(tab_Index) /= 1.1
        Zoom_Image()
        ID_STATUS_LABEL.Text = "Zoom Out"
    End Sub

    Private Sub ZOOMORIGINALToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ZOOMORIGINALToolStripMenuItem.Click
        menuClick = True
        zoomFactor(tab_Index) = 1.0
        Zoom_Image()
        ID_STATUS_LABEL.Text = "Zoom Original"
    End Sub

    Private Sub ZOOMFITToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ZOOMFITToolStripMenuItem.Click
        menuClick = True
        zoomFactor(tab_Index) = CalcIntialRatio(ID_PANEL(tab_Index), originImageList(tab_Index))
        Zoom_Image()
        ID_STATUS_LABEL.Text = "Zoom Fit"
    End Sub

    'undo last object and last row of listview
    Private Sub Undo()
        If undo_num > 0 Then
            obj_selected.Refresh()
            sel_index = -1
            sel_pt_index = -1
            curve_sel_index = -1
            Dim flag = RemoveObjFromList(object_list.ElementAt(tab_Index))
            If flag = True Then
                ID_PICTURE_BOX(tab_Index).DrawObjList(object_list.ElementAt(tab_Index), digit, CF, False)
                ID_LISTVIEW.LoadObjectList(object_list.ElementAt(tab_Index), CF, digit, scaleUnit, name_list)
                undo_num -= 1
                cur_obj_num(tab_Index) -= 1
            End If
        End If
    End Sub
    Private Sub ID_BTN_UNDO_Click(sender As Object, e As EventArgs) Handles ID_BTN_UNDO.Click
        menuClick = False
        Undo()
        ID_STATUS_LABEL.Text = "Undo"
    End Sub


    Private Sub UNDOToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles UNDOToolStripMenuItem.Click
        menuClick = True
        Undo()
        ID_STATUS_LABEL.Text = "Undo"
    End Sub

    'reset current object
    Private Sub ID_BTN_RESEL_Click(sender As Object, e As EventArgs) Handles ID_BTN_RESEL.Click
        menuClick = False
        obj_selected.Refresh()
        ID_PICTURE_BOX(tab_Index).DrawObjList(object_list.ElementAt(tab_Index), digit, CF, False)
        ID_STATUS_LABEL.Text = "Reselect"
    End Sub

    Private Sub RESELECTToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RESELECTToolStripMenuItem.Click
        menuClick = True
        obj_selected.Refresh()
        ID_PICTURE_BOX(tab_Index).DrawObjList(object_list.ElementAt(tab_Index), digit, CF, False)
        ID_STATUS_LABEL.Text = "Reselect"
    End Sub

    'reset digit
    'reload image and obj_list
    Private Sub ID_NUM_DIGIT_ValueChanged(sender As Object, e As EventArgs) Handles ID_NUM_DIGIT.ValueChanged
        menuClick = False
        digit = CInt(ID_NUM_DIGIT.Value)
        ID_PICTURE_BOX(tab_Index).DrawObjList(object_list.ElementAt(tab_Index), digit, CF, False)
        ID_LISTVIEW.LoadObjectList(object_list.ElementAt(tab_Index), CF, digit, scaleUnit, name_list)
        ID_STATUS_LABEL.Text = "Changing the digit of decimal numbers."
    End Sub

    'set the width of line
    Private Sub ID_NUM_LINE_WIDTH_ValueChanged(sender As Object, e As EventArgs) Handles ID_NUM_LINE_WIDTH.ValueChanged
        menuClick = False
        line_infor.line_width = CInt(ID_NUM_LINE_WIDTH.Value)
        ID_STATUS_LABEL.Text = "Changing the width of drawing line."
    End Sub

    'change the color of LineStyle object
    Private Sub ID_BTN_COL_PICKER_Click(sender As Object, e As EventArgs) Handles ID_BTN_COL_PICKER.Click
        menuClick = False
        Dim clrDialog As ColorDialog = New ColorDialog()

        'show the colour dialog and check that user clicked ok
        If clrDialog.ShowDialog() = DialogResult.OK Then
            'save the colour that the user chose
            line_infor.line_color = clrDialog.Color
            ID_BTN_CUR_COL.BackColor = clrDialog.Color
        End If
        ID_STATUS_LABEL.Text = "Changing the color of drawing line."
    End Sub

    'set the line style
    Private Sub ID_COMBO_LINE_SHAPE_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ID_COMBO_LINE_SHAPE.SelectedIndexChanged
        menuClick = False
        Dim comboIndex = ID_COMBO_LINE_SHAPE.SelectedIndex
        If comboIndex = 0 Then
            graphPen_line.DashStyle = Drawing2D.DashStyle.Dot
            line_infor.line_style = "dotted"
        ElseIf comboIndex = 1 Then
            graphPen_line.DashPattern = dashValues
            line_infor.line_style = "dashed"
        End If
        ID_STATUS_LABEL.Text = "Changing the shape of drawing line."
    End Sub

    'set text fore color
    Private Sub ID_BTN_TEXT_COL_PICKER_Click(sender As Object, e As EventArgs) Handles ID_BTN_TEXT_COL_PICKER.Click
        Dim clrDialog As ColorDialog = New ColorDialog()

        'show the colour dialog and check that user clicked ok
        If clrDialog.ShowDialog() = DialogResult.OK Then
            'save the colour that the user chose
            font_infor.font_color = clrDialog.Color
            ID_BTN_TEXT_COL.BackColor = clrDialog.Color
        End If
        ID_STATUS_LABEL.Text = "Changing the color of text."
    End Sub

    'set text font
    Private Sub ID_BTN_TEXT_FONT_Click(sender As Object, e As EventArgs) Handles ID_BTN_TEXT_FONT.Click
        Dim fontDialog As FontDialog = New FontDialog()

        If fontDialog.ShowDialog() = DialogResult.OK Then
            font_infor.text_font = fontDialog.Font
        End If
        ID_STATUS_LABEL.Text = "Changing the font of text."
    End Sub

    'redraw objects
    Private Sub ID_PANEL_Scroll(sender As Object, e As ScrollEventArgs)
        ID_PICTURE_BOX(tab_Index).DrawObjList(object_list.ElementAt(tab_Index), digit, CF, False)
        Dim flag = False
        If sel_index >= 0 Then flag = True
        ID_PICTURE_BOX(tab_Index).DrawObjSelected(obj_selected, flag)
    End Sub

    'keep the image in the center when the panel size in changed
    'redraw objects
    Private Sub ID_PANEL_SizeChanged(sender As Object, e As EventArgs)
        DrawAndCentering()
    End Sub

    'redraw objects
    Private Sub ID_PANEL_MouseWheel(sender As Object, e As MouseEventArgs)
        Dim flag = False
        If sel_index >= 0 Then flag = True
        ID_PICTURE_BOX(tab_Index).DrawObjList(object_list.ElementAt(tab_Index), digit, CF, False)
        ID_PICTURE_BOX(tab_Index).DrawObjSelected(obj_selected, flag)
    End Sub

    'detect edge of selected region
    Private Sub EDGEDETECTToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EDGEDETECTToolStripMenuItem.Click
        EdgeRegionDrawReady = True
        obj_selected.Refresh()
        obj_selected.measuringType = MeasureType.objCurve
        ID_STATUS_LABEL.Text = "Detect edge."
    End Sub

    'update object selected
    'when mouse is clicked on annotation insert textbox there to you can edit it
    'draw objects and load list of objects to listview
    Private Sub ID_PICTURE_BOX_MouseDown(sender As Object, e As MouseEventArgs)
        If ID_PICTURE_BOX(tab_Index).Image Is Nothing OrElse currentImageList(tab_Index) Is Nothing Then
            Return
        End If
        If e.Button = MouseButtons.Left Then
            MouseDownFlag = True
            Dim m_pt As PointF = New PointF()
            m_pt.X = CSng(e.X) / ID_PICTURE_BOX(tab_Index).Width
            m_pt.Y = CSng(e.Y) / ID_PICTURE_BOX(tab_Index).Height
            m_pt.X = Math.Min(Math.Max(m_pt.X, 0), 1)
            m_pt.Y = Math.Min(Math.Max(m_pt.Y, 0), 1)
            mCurDrag = m_pt

            Dim m_pt2 As Point = New Point(e.X, e.Y)

            If curMeasureType >= 0 Then
                If curMeasureType < MeasureType.objLine Then
                    Dim completed = ModifyObjSelected(obj_selected, object_list(tab_Index), curMeasureType, m_pt, Enumerable.ElementAt(originImageList, tab_Index).Width, Enumerable.ElementAt(originImageList, tab_Index).Height, line_infor, font_infor, CF, scaleUnit)

                    If completed Then
                        obj_selected.objNum = cur_obj_num(tab_Index)
                        object_list(tab_Index).Add(obj_selected)
                        ID_PICTURE_BOX(tab_Index).DrawObjList(object_list.ElementAt(tab_Index), digit, CF, False)
                        ID_LISTVIEW.LoadObjectList(object_list.ElementAt(tab_Index), CF, digit, scaleUnit, name_list)
                        obj_selected.Refresh()
                        curMeasureType = -1
                        cur_obj_num(tab_Index) += 1
                        If undo_num < 2 Then undo_num += 1
                    Else
                        ID_PICTURE_BOX(tab_Index).DrawObjList(object_list.ElementAt(tab_Index), digit, CF, False)
                        ID_PICTURE_BOX(tab_Index).DrawObjSelected(obj_selected, False)
                    End If
                Else    'Curve objects
                    If curMeasureType = MeasureType.objPoly Then
                        If PolyPreviousPoint IsNot Nothing Then
                            C_PolyObj.PolyPoint(C_PolyObj.PolyPointIndx) = m_pt
                            C_PolyObj.PolyPointIndx += 1
                            PolyPreviousPoint = Nothing
                        End If
                    ElseIf curMeasureType = MeasureType.objCuPoly Then
                        CuPolyDrawEndFlag = False
                        C_CuPolyObj.CuPolyPointIndx_j += 1
                        C_CuPolyObj.CuPolyPoint(C_CuPolyObj.CuPolyPointIndx_j, 0) = m_pt
                    ElseIf curMeasureType = MeasureType.objPoint Then
                        C_PointObj.PointPoint = m_pt
                    ElseIf curMeasureType = MeasureType.objLine Then
                        If LinePreviousPoint Is Nothing Then
                            LinePreviousPoint = e.Location
                            C_LineObj.FirstPointOfLine = m_pt
                        End If
                    ElseIf curMeasureType = MeasureType.objSel Then
                        If curve_sel_index >= 0 Then
                            Dim obj = object_list.ElementAt(tab_Index).ElementAt(curve_sel_index)
                            If obj.measuringType = MeasureType.objCuPoly Then
                                CuPolyRealSelectArrayIndx = curve_sel_index
                            ElseIf obj.measuringType = MeasureType.objCurve Then
                                CRealSelectArrayIndx = curve_sel_index
                            ElseIf obj.measuringType = MeasureType.objLine Then
                                LRealSelectArrayIndx = curve_sel_index
                            ElseIf obj.measuringType = MeasureType.objPoint Then
                                PRealSelectArrayIndx = curve_sel_index
                            ElseIf obj.measuringType = MeasureType.objPoly Then
                                PolyRealSelectArrayIndx = curve_sel_index
                            End If
                            ID_PICTURE_BOX(tab_Index).DrawObjList(object_list.ElementAt(tab_Index), digit, CF, False)
                            DrawCurveObjSelected(ID_PICTURE_BOX(tab_Index), obj, digit, CF)
                        End If
                    End If

                End If

            Else
                'select point of selected object
                If sel_index >= 0 Then
                    sel_pt_index = ID_PICTURE_BOX(tab_Index).CheckPointInPos(object_list.ElementAt(tab_Index).ElementAt(sel_index), m_pt2)
                    If sel_pt_index >= 0 Then
                        ID_PICTURE_BOX(tab_Index).DrawObjList(object_list.ElementAt(tab_Index), digit, CF, False)
                        ID_PICTURE_BOX(tab_Index).HightLightItem(object_list.ElementAt(tab_Index).ElementAt(sel_index), ID_PICTURE_BOX(tab_Index).Width, ID_PICTURE_BOX(tab_Index).Height, CF)
                        ID_PICTURE_BOX(tab_Index).DrawObjSelected(object_list.ElementAt(tab_Index).ElementAt(sel_index), True)
                        ID_PICTURE_BOX(tab_Index).HighlightTargetPt(object_list.ElementAt(tab_Index).ElementAt(sel_index), sel_pt_index)
                        Return
                    End If
                End If

                sel_index = CheckItemInPos(m_pt, object_list.ElementAt(tab_Index), ID_PICTURE_BOX(tab_Index).Width, ID_PICTURE_BOX(tab_Index).Height, CF)
                If sel_index >= 0 Then
                    ID_PICTURE_BOX(tab_Index).DrawObjList(object_list.ElementAt(tab_Index), digit, CF, False)
                    ID_PICTURE_BOX(tab_Index).HightLightItem(object_list.ElementAt(tab_Index).ElementAt(sel_index), ID_PICTURE_BOX(tab_Index).Width, ID_PICTURE_BOX(tab_Index).Height, CF)
                    ID_PICTURE_BOX(tab_Index).DrawObjSelected(object_list.ElementAt(tab_Index).ElementAt(sel_index), True)
                Else
                    If anno_num >= 0 Then
                        ID_MY_TEXTBOX(tab_Index).DisableTextBox(object_list.ElementAt(tab_Index), anno_num, ID_PICTURE_BOX(tab_Index).Width, ID_PICTURE_BOX(tab_Index).Height)
                        ID_LISTVIEW.LoadObjectList(object_list.ElementAt(tab_Index), CF, digit, scaleUnit, name_list)
                        anno_num = -1
                    End If
                    ID_PICTURE_BOX(tab_Index).DrawObjList(object_list.ElementAt(tab_Index), digit, CF, False)
                End If
            End If

            If EdgeRegionDrawReady = True Then
                FirstPtOfEdge = m_pt
                EdgeRegionDrawed = False
            End If

            If move_line = True And curve_sel_index >= 0 Then
                Dim obj = object_list.ElementAt(tab_Index).ElementAt(curve_sel_index)
                If obj.measuringType = MeasureType.objLine Then
                    StartPtOfMove = m_pt
                    C_LineObj.Refresh()
                    C_LineObj = CloneLineObj(obj.curveObject.LineItem(0))
                    obj_selected2.Refresh()
                    InitializeLineObj(obj_selected2, C_LineObj.LDrawPos, line_infor, font_infor)
                End If
            End If
        Else    'right click
            MouseRDownFlag = True
            If curMeasureType = MeasureType.objPoly Then
                PolyPreviousPoint = Nothing
                C_PolyObj.PolyDrawPos = PolyGetPos(C_PolyObj)
                Dim tempObj = ClonePolyObj(C_PolyObj)
                obj_selected.curveObject = New CurveObject()
                obj_selected.curveObject.PolyItem.Add(tempObj)
                obj_selected.name = "PL" & cur_obj_num(tab_Index)
                AddCurveToList()
                C_PolyObj.Refresh()
                PolyDrawEndFlag = True
            ElseIf curMeasureType = MeasureType.objCuPoly Then
                CuPolyPreviousPoint = Nothing
                C_CuPolyObj.CuPolyDrawPos = CuPolyGetPos(C_CuPolyObj)
                Dim tempObj = CloneCuPolyObj(C_CuPolyObj)
                obj_selected.curveObject = New CurveObject()
                obj_selected.curveObject.CuPolyItem.Add(tempObj)
                obj_selected.name = "CP" & cur_obj_num(tab_Index)
                AddCurveToList()
                C_CuPolyObj.Refresh()
                CuPolyDrawEndFlag = True
            End If
        End If

    End Sub

    'release capture
    Private Sub ID_PICTURE_BOX_MouseUp(ByVal sender As Object, ByVal e As MouseEventArgs)
        MouseDownFlag = False
        MouseRDownFlag = False

        If curMeasureType = MeasureType.objPoint Then
            C_PointObj.PDrawPos = PGetPos(C_PointObj.PointPoint)
            Dim tempObj = ClonePointObj(C_PointObj)
            obj_selected.curveObject = New CurveObject()
            obj_selected.curveObject.PointItem.Add(tempObj)
            obj_selected.name = "P" & cur_obj_num(tab_Index)
            AddCurveToList()
            C_PointObj.Refresh()
        ElseIf curMeasureType = MeasureType.objLine Then
            If C_LineObj.SecndPointOfLine.X <> 0 And C_LineObj.SecndPointOfLine.Y <> 0 Then
                LinePreviousPoint = Nothing
                C_LineObj.LDrawPos = LGetPos(C_LineObj)
                Dim tempObj = CloneLineObj(C_LineObj)
                obj_selected.curveObject = New CurveObject()
                obj_selected.curveObject.LineItem.Add(tempObj)
                obj_selected.name = "L" & cur_obj_num(tab_Index)
                AddCurveToList()
                C_LineObj.Refresh()
            End If
        ElseIf curMeasureType = MeasureType.objCurve Then
            C_CurveObj.CDrawPos = CGetPos(C_CurveObj)
            If Not CurveObjFlag Then
                Dim tempObj = CloneCurveObj(C_CurveObj)
                obj_selected.curveObject = New CurveObject()
                obj_selected.curveObject.CurveItem.Add(tempObj)
                obj_selected.name = "C" & cur_obj_num(tab_Index)
                AddCurveToList()
            Else
                C_CurveObj.CPointIndx += 1
                C_CurveObj.CurvePoint(C_CurveObj.CPointIndx) = C_CurveObj.CurvePoint(0)
                DrawCurveObj(ID_PICTURE_BOX(tab_Index), line_infor, C_CurveObj)
                C_CurveObjCopy = CloneCurveObj(C_CurveObj)
            End If
            C_CurveObj.Refresh()
            CurvePreviousPoint = Nothing
        ElseIf curMeasureType = MeasureType.objCuPoly Then
            CuPolyPreviousPoint = Nothing
        End If

        If EdgeRegionDrawReady = True And SecondPtOfEdge.X <> 0 And SecondPtOfEdge.Y <> 0 Then
            'run code for detect edge
            If obj_selected.measuringType = MeasureType.objCurve Then
                Dim input As Image = resizedImageList(tab_Index).ToBitmap()
                Dim FirstPt = New Point(FirstPtOfEdge.X * ID_PICTURE_BOX(tab_Index).Width, FirstPtOfEdge.Y * ID_PICTURE_BOX(tab_Index).Height)
                Dim SecondPt = New Point(SecondPtOfEdge.X * ID_PICTURE_BOX(tab_Index).Width, SecondPtOfEdge.Y * ID_PICTURE_BOX(tab_Index).Height)
                C_CurveObj = Canny(input, FirstPt, SecondPt)

                CurvePreviousPoint = Nothing
                C_CurveObj.CDrawPos = CGetPos(C_CurveObj)
                Dim tempObj = CloneCurveObj(C_CurveObj)
                obj_selected.curveObject = New CurveObject()
                obj_selected.curveObject.CurveItem.Add(tempObj)
                obj_selected.name = "C" & cur_obj_num(tab_Index)
                AddCurveToList()
                C_CurveObj.Refresh()
                EdgeRegionDrawReady = False
                FirstPtOfEdge.X = 0
                FirstPtOfEdge.Y = 0
                SecondPtOfEdge.X = 0
                SecondPtOfEdge.Y = 0
                Dim form = New ToCurve()
                Dim result = form.ShowDialog()
                If result = DialogResult.Cancel Then
                    Undo()
                    undo_num += 1

                ElseIf result = DialogResult.Retry Then
                    Undo()
                    undo_num += 1
                    EdgeRegionDrawReady = True
                    obj_selected.measuringType = MeasureType.objCurve
                End If
            Else
                EdgeRegionDrawed = True
            End If
        End If

        If move_line = True And EndPtOfMove.X <> 0 And EndPtOfMove.Y <> 0 Then
            obj_selected2.objNum = cur_obj_num(tab_Index)
            object_list(tab_Index).Add(obj_selected2)
            obj_selected2.Refresh()
            curMeasureType = -1
            cur_obj_num(tab_Index) += 1
            If undo_num < 2 Then undo_num += 1

            C_LineObj.LDrawPos = LGetPos(C_LineObj)
            Dim tempObj = CloneLineObj(C_LineObj)
            obj_selected.curveObject = New CurveObject()
            obj_selected.curveObject.LineItem.Add(tempObj)
            obj_selected.name = "L" & cur_obj_num(tab_Index)
            AddCurveToList()
            C_LineObj.Refresh()
            StartPtOfMove.X = 0
            StartPtOfMove.Y = 0
            EndPtOfMove.X = 0
            EndPtOfMove.Y = 0
            move_line = False
        End If
    End Sub


    'draw temporal objects according to mouse cursor
    Private Sub ID_PICTURE_BOX_MouseMove(sender As Object, e As MouseEventArgs)
        Dim m_pt As PointF = New PointF()
        m_pt.X = CSng(e.X) / ID_PICTURE_BOX(tab_Index).Width
        m_pt.Y = CSng(e.Y) / ID_PICTURE_BOX(tab_Index).Height
        m_pt.X = Math.Min(Math.Max(m_pt.X, 0), 1)
        m_pt.Y = Math.Min(Math.Max(m_pt.Y, 0), 1)
        Dim dx = m_pt.X - mCurDrag.X
        Dim dy = m_pt.Y - mCurDrag.Y

        Dim m_pt2 = New Point(e.X, e.Y)

        If MouseDownFlag = True Then
            If curMeasureType < 0 Then
                If sel_index >= 0 Then
                    mCurDrag = m_pt
                    If sel_pt_index >= 0 Then
                        ID_PICTURE_BOX(tab_Index).Refresh()
                        MovePoint(object_list.ElementAt(tab_Index), sel_index, sel_pt_index, dx, dy)
                        ModifyObjSelected(object_list.ElementAt(tab_Index), sel_index, Enumerable.ElementAt(originImageList, tab_Index).Width, Enumerable.ElementAt(originImageList, tab_Index).Height)
                        Dim obj = object_list.ElementAt(tab_Index).ElementAt(sel_index)
                        Dim target_pt As Point = New Point()
                        If obj.measuringType = MeasureType.angle Then

                            Dim start_point As Point = New Point()
                            Dim end_point As Point = New Point()
                            Dim middle_point As Point = New Point()

                            start_point.X = CInt(obj.startPoint.X * ID_PICTURE_BOX(tab_Index).Width)
                            start_point.Y = CInt(obj.startPoint.Y * ID_PICTURE_BOX(tab_Index).Height)
                            middle_point.X = CInt(obj.middlePoint.X * ID_PICTURE_BOX(tab_Index).Width)
                            middle_point.Y = CInt(obj.middlePoint.Y * ID_PICTURE_BOX(tab_Index).Height)
                            end_point.X = CInt(obj.endPoint.X * ID_PICTURE_BOX(tab_Index).Width)
                            end_point.Y = CInt(obj.endPoint.Y * ID_PICTURE_BOX(tab_Index).Height)

                            target_pt.X = (start_point.X + end_point.X) / 2
                            target_pt.Y = (start_point.Y + end_point.Y) / 2
                            Dim angles = CalcStartAndSweepAngle(obj, start_point, middle_point, end_point, target_pt)
                            Dim start_angle, sweep_angle As Double
                            start_angle = angles(0)
                            sweep_angle = angles(1)
                            Dim angle As Integer = CInt(2 * start_angle + sweep_angle) / 2
                            Dim radius = CInt(obj.angleObject.radius * ID_PICTURE_BOX(tab_Index).Width) + 10
                            target_pt = CalcPositionInCircle(middle_point, radius, angle)
                        Else
                            target_pt = New Point(obj.drawPoint.X * ID_PICTURE_BOX(tab_Index).Width, obj.drawPoint.Y * ID_PICTURE_BOX(tab_Index).Height)
                        End If
                        ID_PICTURE_BOX(tab_Index).DrawTempFinal(obj, target_pt, side_drag, digit, CF, False)
                        object_list(tab_Index)(sel_index) = obj
                        ID_PICTURE_BOX(tab_Index).DrawObjList(object_list.ElementAt(tab_Index), digit, CF, False)
                        ID_LISTVIEW.LoadObjectList(object_list.ElementAt(tab_Index), CF, digit, scaleUnit, name_list)

                    Else
                        MoveObject(object_list.ElementAt(tab_Index), sel_index, dx, dy)
                        ID_PICTURE_BOX(tab_Index).DrawObjList(object_list.ElementAt(tab_Index), digit, CF, False)
                    End If
                    ID_PICTURE_BOX(tab_Index).HightLightItem(object_list.ElementAt(tab_Index).ElementAt(sel_index), ID_PICTURE_BOX(tab_Index).Width, ID_PICTURE_BOX(tab_Index).Height, CF)
                    ID_PICTURE_BOX(tab_Index).DrawObjSelected(object_list.ElementAt(tab_Index).ElementAt(sel_index), True)
                End If
            End If

            If curMeasureType = MeasureType.objCurve Then
                If CurvePreviousPoint Is Nothing Then
                    CurvePreviousPoint = e.Location
                    C_CurveObj.CurvePoint(0) = m_pt
                Else
                    ID_PICTURE_BOX(tab_Index).DrawObjList(object_list.ElementAt(tab_Index), digit, CF, False)
                    DrawCurveObj(ID_PICTURE_BOX(tab_Index), line_infor, C_CurveObj)
                    DrawLineBetweenTwoPoints(ID_PICTURE_BOX(tab_Index), line_infor, CurvePreviousPoint.Value, e.Location)
                    C_CurveObj.CPointIndx += 1
                    CurvePreviousPoint = e.Location
                    C_CurveObj.CurvePoint(C_CurveObj.CPointIndx) = m_pt
                End If
            ElseIf curMeasureType = MeasureType.objLine Then
                If LinePreviousPoint IsNot Nothing Then
                    C_LineObj.SecndPointOfLine = m_pt
                    ID_PICTURE_BOX(tab_Index).DrawObjList(object_list.ElementAt(tab_Index), digit, CF, False)
                    DrawLineBetweenTwoPoints(ID_PICTURE_BOX(tab_Index), line_infor, LinePreviousPoint.Value, e.Location)
                End If
            ElseIf curMeasureType = MeasureType.objCuPoly Then
                If CuPolyDrawEndFlag = False Then
                    If CuPolyPreviousPoint Is Nothing Then
                        CuPolyPreviousPoint = e.Location
                        C_CuPolyObj.CuPolyPoint(C_CuPolyObj.CuPolyPointIndx_j, 0) = m_pt
                    Else
                        ID_PICTURE_BOX(tab_Index).DrawObjList(object_list.ElementAt(tab_Index), digit, CF, False)
                        DrawCuPolyObj(ID_PICTURE_BOX(tab_Index), line_infor, C_CuPolyObj)
                        DrawLineBetweenTwoPoints(ID_PICTURE_BOX(tab_Index), line_infor, CuPolyPreviousPoint.Value, e.Location)
                        C_CuPolyObj.CuPolyPointIndx_k(C_CuPolyObj.CuPolyPointIndx_j) += 1
                        CuPolyPreviousPoint = e.Location
                        C_CuPolyObj.CuPolyPoint(C_CuPolyObj.CuPolyPointIndx_j, C_CuPolyObj.CuPolyPointIndx_k(C_CuPolyObj.CuPolyPointIndx_j)) = m_pt
                    End If
                End If
            End If

            If EdgeRegionDrawReady = True Then
                SecondPtOfEdge = m_pt
                Dim FirstPt = New Point(FirstPtOfEdge.X * ID_PICTURE_BOX(tab_Index).Width, FirstPtOfEdge.Y * ID_PICTURE_BOX(tab_Index).Height)
                Dim SecondPt = New Point(SecondPtOfEdge.X * ID_PICTURE_BOX(tab_Index).Width, SecondPtOfEdge.Y * ID_PICTURE_BOX(tab_Index).Height)
                If CalibrationAxis = 1 Then
                    SecondPt.Y = FirstPt.Y + 1
                ElseIf CalibrationAxis = 2 Then
                    SecondPt.X = FirstPt.X + 1
                End If

                ID_PICTURE_BOX(tab_Index).DrawObjList(object_list.ElementAt(tab_Index), digit, CF, False)
                ID_PICTURE_BOX(tab_Index).DrawRectangle(FirstPt, SecondPt)
            End If

            If move_line = True And curve_sel_index >= 0 And StartPtOfMove.X <> 0 And StartPtOfMove.Y <> 0 Then
                EndPtOfMove = m_pt
                Dim Obj = object_list.ElementAt(tab_Index).ElementAt(curve_sel_index).curveObject.LineItem(0)
                C_LineObj.FirstPointOfLine.X = (EndPtOfMove.X - StartPtOfMove.X) + Obj.FirstPointOfLine.X
                C_LineObj.FirstPointOfLine.Y = (EndPtOfMove.Y - StartPtOfMove.Y) + Obj.FirstPointOfLine.Y
                C_LineObj.SecndPointOfLine.X = (EndPtOfMove.X - StartPtOfMove.X) + Obj.SecndPointOfLine.X
                C_LineObj.SecndPointOfLine.Y = (EndPtOfMove.Y - StartPtOfMove.Y) + Obj.SecndPointOfLine.Y
                ID_PICTURE_BOX(tab_Index).DrawObjList(object_list.ElementAt(tab_Index), digit, CF, False)
                DrawLineObject(ID_PICTURE_BOX(tab_Index), C_LineObj)
                Dim Delta = GetNormalFromPointToLine(New Point(Obj.FirstPointOfLine.X * ID_PICTURE_BOX(tab_Index).Width, Obj.FirstPointOfLine.Y * ID_PICTURE_BOX(tab_Index).Height),
                                                     New Point(Obj.SecndPointOfLine.X * ID_PICTURE_BOX(tab_Index).Width, Obj.SecndPointOfLine.Y * ID_PICTURE_BOX(tab_Index).Height), m_pt2)
                DrawLengthBetweenLines(ID_PICTURE_BOX(tab_Index), obj_selected2, CDbl(Delta.Width / ID_PICTURE_BOX(tab_Index).Width), CDbl(Delta.Height / ID_PICTURE_BOX(tab_Index).Height), originImageList(tab_Index).Width, originImageList(tab_Index).Height, digit, CF)
            End If
        Else    'mouse is not clicked

            If sel_index >= 0 Then
                ID_PICTURE_BOX(tab_Index).HightLightItem(object_list.ElementAt(tab_Index).ElementAt(sel_index), ID_PICTURE_BOX(tab_Index).Width, ID_PICTURE_BOX(tab_Index).Height, CF)
                ID_PICTURE_BOX(tab_Index).DrawObjSelected(object_list.ElementAt(tab_Index).ElementAt(sel_index), True)
            End If

            If curMeasureType >= 0 Then
                If curMeasureType < MeasureType.objLine Then
                    Dim temp As Point = New Point(e.X, e.Y)
                    ID_PICTURE_BOX(tab_Index).DrawObjList(object_list.ElementAt(tab_Index), digit, CF, False)
                    ID_PICTURE_BOX(tab_Index).DrawObjSelected(obj_selected, False)
                    ID_PICTURE_BOX(tab_Index).DrawTempFinal(obj_selected, temp, side_drag, digit, CF, True)
                ElseIf curMeasureType = MeasureType.objPoly Then
                    'If PolyDrawEndFlag = False Then
                    If PolyPreviousPoint Is Nothing Then
                        PolyPreviousPoint = e.Location
                        Dim ptF = New PointF(e.X / CSng(ID_PICTURE_BOX(tab_Index).Width), e.Y / CSng(ID_PICTURE_BOX(tab_Index).Height))
                        C_PolyObj.PolyPoint(C_PolyObj.PolyPointIndx) = ptF
                    Else
                        If C_PolyObj.PolyPointIndx >= 1 Then
                            ID_PICTURE_BOX(tab_Index).DrawObjList(object_list.ElementAt(tab_Index), digit, CF, False)
                            DrawPolyObj(ID_PICTURE_BOX(tab_Index), line_infor, C_PolyObj)
                            DrawLineBetweenTwoPoints(ID_PICTURE_BOX(tab_Index), line_infor, PolyPreviousPoint.Value, e.Location)
                        End If
                    End If
                    'End If
                ElseIf curMeasureType = MeasureType.objCuPoly Then
                    If CuPolyDrawEndFlag = False Then
                        Dim temp As Point
                        If C_CuPolyObj.CuPolyPointIndx_j > 0 Then
                            Dim tempF = C_CuPolyObj.CuPolyPoint(C_CuPolyObj.CuPolyPointIndx_j, C_CuPolyObj.CuPolyPointIndx_k(C_CuPolyObj.CuPolyPointIndx_j))
                            temp = New Point(tempF.X * ID_PICTURE_BOX(tab_Index).Width, tempF.Y * ID_PICTURE_BOX(tab_Index).Height)
                        Else
                            temp = dumyPoint
                        End If
                        If temp <> dumyPoint Then
                            ID_PICTURE_BOX(tab_Index).DrawObjList(object_list.ElementAt(tab_Index), digit, CF, False)
                            DrawCuPolyObj(ID_PICTURE_BOX(tab_Index), line_infor, C_CuPolyObj)
                            DrawLineBetweenTwoPoints(ID_PICTURE_BOX(tab_Index), line_infor, temp, e.Location)
                        End If
                    End If
                ElseIf curMeasureType = MeasureType.objSel Then
                    curve_sel_index = CheckCurveItemInPos(ID_PICTURE_BOX(tab_Index), m_pt, object_list.ElementAt(tab_Index))
                    If curve_sel_index >= 0 Then
                        Dim obj = object_list.ElementAt(tab_Index).ElementAt(curve_sel_index)
                        ID_PICTURE_BOX(tab_Index).DrawObjList(object_list.ElementAt(tab_Index), digit, CF, False)
                        DrawCurveObjSelected(ID_PICTURE_BOX(tab_Index), obj, digit, CF)
                    End If
                End If
            End If

            If move_line Then
                curve_sel_index = CheckCurveItemInPos(ID_PICTURE_BOX(tab_Index), m_pt, object_list.ElementAt(tab_Index))
                If curve_sel_index >= 0 Then
                    Dim obj = object_list.ElementAt(tab_Index).ElementAt(curve_sel_index)
                    ID_PICTURE_BOX(tab_Index).DrawObjList(object_list.ElementAt(tab_Index), digit, CF, False)
                    DrawCurveObjSelected(ID_PICTURE_BOX(tab_Index), obj, digit, CF)
                End If
            End If
        End If
    End Sub

    'select annotation
    Private Sub ID_PICTURE_BOX_MouseDoubleClick(ByVal sender As Object, ByVal e As MouseEventArgs)
        Dim m_pt As PointF = New Point()
        m_pt.X = CSng(e.X) / ID_PICTURE_BOX(tab_Index).Width
        m_pt.Y = CSng(e.Y) / ID_PICTURE_BOX(tab_Index).Height
        m_pt.X = Math.Min(Math.Max(m_pt.X, 0), 1)
        m_pt.Y = Math.Min(Math.Max(m_pt.Y, 0), 1)

        Dim an_num = CheckAnnotation(m_pt, object_list.ElementAt(tab_Index), ID_PICTURE_BOX(tab_Index).Width, ID_PICTURE_BOX(tab_Index).Height)
        If an_num >= 0 AndAlso Enumerable.ElementAt(Of MeasureObject)(Enumerable.ElementAt(Of List(Of MeasureObject))(object_list, tab_Index), an_num).measuringType = MeasureType.annotation Then
            ID_MY_TEXTBOX(tab_Index).Font = font_infor.text_font
            ID_MY_TEXTBOX(tab_Index).EnableTextBox(object_list.ElementAt(tab_Index).ElementAt(an_num), ID_PICTURE_BOX(tab_Index).Width, ID_PICTURE_BOX(tab_Index).Height, left_top, scroll_pos)
            anno_num = an_num
        End If
    End Sub

    'draw objects to picturebox when ID_FORM_BRIGHTNESS is actived
    Private Sub ID_PICTURE_BOX_Paint(ByVal sender As Object, ByVal e As PaintEventArgs)
        If redraw_flag Then ID_PICTURE_BOX(tab_Index).DrawObjList(object_list.ElementAt(tab_Index), digit, CF, True)
    End Sub

    'change the size of textbox when the text is changed
    Private Sub ID_MY_TEXTBOX_TextChanged(sender As Object, e As EventArgs)
        Dim textBox = CType(sender, TextBox)
        Dim numberOfLines = SendMessage(textBox.Handle.ToInt32(), EM_GETLINECOUNT, 0, 0)
        textBox.Height = (textBox.Font.Height + 2) * numberOfLines
    End Sub

    'set tab_Index
    'reload image and object list
    Private Sub ID_TAG_CTRL_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ID_TAG_CTRL.SelectedIndexChanged
        Dim tab_name As String = ID_TAG_CTRL.SelectedTab.Name
        tab_name = tab_name.Substring(11)
        tab_Index = CInt(tab_name)
        obj_selected.Refresh()
        curMeasureType = -1
        sel_index = -1
        curve_sel_index = -1
        DrawAndCentering()
        ID_LISTVIEW.LoadObjectList(object_list.ElementAt(tab_Index), CF, digit, scaleUnit, name_list)
    End Sub

    Private Sub IntializePictureDependencies()
        ID_PICTURE_BOX(tab_Index).Image = Nothing
        currentImageList(tab_Index) = Nothing
        resizedImageList(tab_Index) = Nothing
        originImageList(tab_Index) = Nothing
        cur_obj_num(tab_Index) = 0
        Enumerable.ElementAt(Of List(Of MeasureObject))(object_list, tab_Index).Clear()
    End Sub

    'add new tab
    Private Sub Add_Tab()
        If tab_Index >= 24 Then
            Return
        End If

        tab_Index += 1
        If tag_page_flag(tab_Index) = False Then
            tag_page_flag(tab_Index) = True

            IntializePictureDependencies()
            img_import_flag(tab_Index) = True

            ID_TAG_CTRL.TabPages.Add(ID_TAG_PAGE(tab_Index))
            ID_TAG_CTRL.SelectedTab = ID_TAG_PAGE(tab_Index)
        End If
    End Sub

    'add tag page at the end
    Private Sub ID_BTN_TAB_ADD_Click(sender As Object, e As EventArgs) Handles ID_BTN_TAB_ADD.Click
        Add_Tab()
        ID_STATUS_LABEL.Text = "Add tab."
    End Sub

    'remove tab
    Private Sub Remove_Tab()
        If tab_Index < 0 Then
            Return
        End If

        If tag_page_flag(tab_Index) = True Then
            DisposeElemOfList(currentImageList, Nothing, tab_Index)
            DisposeElemOfList(resizedImageList, Nothing, tab_Index)
            DisposeElemOfList(originImageList, Nothing, tab_Index)

            cur_obj_num(tab_Index) = 0
            Enumerable.ElementAt(Of List(Of MeasureObject))(object_list, tab_Index).Clear()

            img_import_flag(tab_Index) = True
            ID_PICTURE_BOX(tab_Index).Image = Nothing
            ID_TAG_PAGE(tab_Index).Text = ""

            If tab_Index = 0 Then
                ID_TAG_CTRL.SelectedIndex = 0
            Else
                tag_page_flag(tab_Index) = False
                Dim cur_index = ID_TAG_CTRL.SelectedIndex
                ID_TAG_CTRL.TabPages.Remove(ID_TAG_PAGE(tab_Index))
                ID_TAG_CTRL.SelectedIndex = cur_index - 1
            End If

        End If
    End Sub
    'remove last tag page
    Private Sub ID_BTN_TAB_REMOVE_Click(sender As Object, e As EventArgs) Handles ID_BTN_TAB_REMOVE.Click
        Remove_Tab()
        ID_STATUS_LABEL.Text = "Remove tab."
    End Sub

    Private Sub ADDTAGToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ADDTAGToolStripMenuItem.Click
        Add_Tab()
        ID_STATUS_LABEL.Text = "Add tab."
    End Sub

    Private Sub REMOVETAGToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles REMOVETAGToolStripMenuItem.Click
        Remove_Tab()
        ID_STATUS_LABEL.Text = "Remove tab."
    End Sub

    'save setting information to setting.ini
    'close camera
    Private Sub Main_Form_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        On Error Resume Next

        UpdateCalibrationInfo()
        UpdateConfigInfo()

        If videoDevice Is Nothing Then
        ElseIf videoDevice.IsRunning Then
            videoDevice.SignalToStop()
            RemoveHandler videoDevice.NewFrame, New NewFrameEventHandler(AddressOf Device_NewFrame)
            videoDevice = Nothing
        End If
        camera_state = False
    End Sub

    'show About dialog
    Private Sub ID_MENU_ABOUT_Click(sender As Object, e As EventArgs) Handles ID_MENU_ABOUT.Click
        Dim ad As New About
        If ad.ShowDialog() = DialogResult.OK Then

        End If
    End Sub

    'side dragging for small thickness when ID_CHECK_SIDE is checked
    Private Sub ID_CHECK_SIDE_CheckedChanged(sender As Object, e As EventArgs) Handles ID_CHECK_SIDE.CheckedChanged
        If ID_CHECK_SIDE.Checked = True Then
            side_drag = True
        Else
            side_drag = False
        End If
    End Sub

    'show legend when ID_CHECK_SHOW_LEGEND is checked
    Private Sub ID_CHECK_SHOW_LEGEND_CheckedChanged(sender As Object, e As EventArgs) Handles ID_CHECK_SHOW_LEGEND.CheckedChanged
        If ID_CHECK_SHOW_LEGEND.Checked = True Then
            showLegend = True
        Else
            showLegend = False
        End If
        ID_PICTURE_BOX(tab_Index).DrawObjList(object_list.ElementAt(tab_Index), digit, CF, False)
    End Sub
    Private Sub CALIBRATIONINFOToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CALIBRATIONINFOToolStripMenuItem.Click
        Try
            Dim alive As System.Diagnostics.Process
            If System.IO.File.Exists(cali_path) = False Then
                cali_ini = New IniFile(cali_path)

                cali_ini.AddSection("CF")
                cali_ini.AddKey("index", CFSelected, "CF")

                For i = 0 To CFList.Count - 1
                    Dim key = CFList(i)
                    Dim value = CFNum(i) & ":" & CFUnit(i)
                    cali_ini.AddKey(key, value, "CF")
                Next
                cali_ini.AddSection("Phase")
                'cali_ini.Sort()
                cali_ini.Save(cali_path)
            End If
            alive = Process.Start(cali_path)
            alive.WaitForExit()
            CFList.Clear()
            CFNum.Clear()
            CFUnit.Clear()
            GetCalibrationInfo()
        Catch ex As Exception
            MessageBox.Show(ex.Message.ToString())
        End Try
    End Sub

    Private Sub CONFIGINFOToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CONFIGINFOToolStripMenuItem.Click
        Try
            Dim alive As System.Diagnostics.Process
            If System.IO.File.Exists(config_path) = False Then
                config_ini = New IniFile(config_path)

                config_ini.AddSection("Config")
                config_ini.AddKey("digit", digit.ToString(), "Config")
                config_ini.AddKey("areaUnit", RoiUnit, "Config")
                config_ini.AddKey("roiSize", RoiSize, "Config")

                config_ini.Sort()
                config_ini.Save(config_path)
            End If
            alive = Process.Start(config_path)
            alive.WaitForExit()
            GetConfigInfo()
        Catch ex As Exception
            MessageBox.Show(ex.Message.ToString())
        End Try
    End Sub

    Private Sub LEGENDINFOToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LEGENDINFOToolStripMenuItem.Click
        Try
            Dim alive As System.Diagnostics.Process
            If System.IO.File.Exists(legend_path) = False Then
                legend_ini = New IniFile(legend_path)
                legend_ini.AddSection("name")
                legend_ini.AddKey("No1", "Line", "name")
                legend_ini.AddKey("No2", "Angle", "name")
                legend_ini.AddKey("No3", "Arc", "name")
                legend_ini.AddKey("No4", "Scale", "name")

                legend_ini.Sort()
                legend_ini.Save(legend_path)
            End If
            alive = Process.Start(legend_path)
            alive.WaitForExit()
            name_list.Clear()
            GetLegendInfo()
        Catch ex As Exception
            MessageBox.Show(ex.Message.ToString())
        End Try
    End Sub

#End Region

#Region "DataGridView"


    'update first and fifth item of datagridview and update the object 
    Private Sub ID_LISTVIEW_CellValidating(sender As Object, e As DataGridViewCellValidatingEventArgs) Handles ID_LISTVIEW.CellValidating
        If e.ColumnIndex = 2 Or e.ColumnIndex = 6 Then

            Dim cell = TryCast(ID_LISTVIEW.Rows(e.RowIndex).Cells(e.ColumnIndex), DataGridViewComboBoxCell)

            If cell IsNot Nothing AndAlso Not Equals(e.FormattedValue.ToString(), String.Empty) Then
                cell.Items(0) = e.FormattedValue

                If ID_LISTVIEW.IsCurrentCellDirty Then
                    ID_LISTVIEW.CommitEdit(DataGridViewDataErrorContexts.Commit)
                End If

                cell.Value = e.FormattedValue
                Dim obj_list = object_list(tab_Index)
                Dim obj = obj_list.ElementAt(e.RowIndex)
                If e.ColumnIndex = 2 Then
                    obj.description = cell.Value
                Else
                    obj.judgement = cell.Value
                End If

                obj_list(e.RowIndex) = obj
                object_list(tab_Index) = obj_list

            End If
        ElseIf e.ColumnIndex = 3 Or e.ColumnIndex = 4 Then
            Dim cell = TryCast(ID_LISTVIEW.Rows(e.RowIndex).Cells(e.ColumnIndex), DataGridViewTextBoxCell)

            If cell IsNot Nothing AndAlso Not Equals(e.FormattedValue.ToString(), String.Empty) Then

                If ID_LISTVIEW.IsCurrentCellDirty Then
                    ID_LISTVIEW.CommitEdit(DataGridViewDataErrorContexts.Commit)
                End If

                cell.Value = e.FormattedValue
                Dim obj_list = object_list(tab_Index)
                Dim obj = obj_list.ElementAt(e.RowIndex)
                If e.ColumnIndex = 3 Then
                    obj.parameter = cell.Value
                Else
                    obj.spec = cell.Value
                End If

                obj_list(e.RowIndex) = obj
                object_list(tab_Index) = obj_list

            End If
        End If
    End Sub

    'handles exception for datagridview
    Private Sub ID_LISTVIEW_DataError(sender As Object, e As DataGridViewDataErrorEventArgs) Handles ID_LISTVIEW.DataError
        If e.ColumnIndex = 0 AndAlso e.RowIndex = 0 Then
            e.Cancel = True
        End If
    End Sub

#End Region

#Region "Webcam Methods"

    'pop one frame from webcam and display it to pictureboxs
    Public Sub Device_NewFrame(sender As Object, eventArgs As AForge.Video.NewFrameEventArgs)
        On Error Resume Next

        Me.Invoke(Sub()
                      newImage = DirectCast(eventArgs.Frame.Clone(), Bitmap)

                      If flag = False Then
                          ID_PICTURE_BOX(0).Image = newImage.Clone()
                      End If
                      ID_PICTURE_BOX_CAM.Image = newImage.Clone()
                      newImage?.Dispose()
                  End Sub)

    End Sub

    'open camera
    Private Sub OpenCamera()
        Dim cameraInt As Int32 = CheckPerticularCamera(videoDevices, _devicename)
        If (cameraInt < 0) Then
            MessageBox.Show("Compatible Camera not found..")
            Exit Sub
        End If

        videoDevices = New FilterInfoCollection(FilterCategory.VideoInputDevice)
        videoDevice = New VideoCaptureDevice(videoDevices(Convert.ToInt32(cameraInt)).MonikerString)
        If Not My.Settings.camresindex.Equals("") Then
            videoDevice.VideoResolution = videoDevice.VideoCapabilities(Convert.ToInt32(My.Settings.camresindex))
        End If
        AddHandler videoDevice.NewFrame, New NewFrameEventHandler(AddressOf Device_NewFrame)
        videoDevice.Start()
        camera_state = True
    End Sub

    'close camera
    Private Sub CloseCamera()

        If videoDevice Is Nothing Then
        ElseIf videoDevice.IsRunning Then
            videoDevice.SignalToStop()
            RemoveHandler videoDevice.NewFrame, New NewFrameEventHandler(AddressOf Device_NewFrame)
            videoDevice.Source = Nothing
        End If
        camera_state = False
    End Sub

    'capture image and add it to ID_LISTVIEW_IMAGE
    Private Sub ID_BTN_CAPTURE_Click(sender As Object, e As EventArgs) Handles ID_BTN_CAPTURE.Click

        Try

            If ID_PICTURE_BOX_CAM.Image Is Nothing Then
                Return
            End If
            Dim img1 As Image = ID_PICTURE_BOX_CAM.Image.Clone()

            Createdirectory(imagepath)
            If photoList.Images.Count <= 0 Then
                file_counter = photoList.Images.Count + 1
            Else
                file_counter = Convert.ToInt32(IO.Path.GetFileNameWithoutExtension(photoList.Images.Keys.Item(photoList.Images.Count - 1).ToString()).Split("_")(1)) + 1
            End If

            img1.Save(imagepath & "\\test_" & (file_counter) & ".jpeg", Imaging.ImageFormat.Jpeg)
            photoList.ImageSize = New Size(160, 120)
            photoList.Images.Add("\\test_" & (file_counter) & ".jpeg", img1)
            ID_LISTVIEW_IMAGE.LargeImageList = photoList
            'img1.Dispose()
            ID_LISTVIEW_IMAGE.Items.Clear()
            For index = 0 To photoList.Images.Count - 1
                Dim item As New ListViewItem With {
                    .ImageIndex = index,
                        .Tag = imagepath & photoList.Images.Keys.Item(index).ToString(),
                        .Text = IO.Path.GetFileNameWithoutExtension(photoList.Images.Keys.Item(index).ToString())
                }
                ID_LISTVIEW_IMAGE.Items.Add(item)
            Next

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try

    End Sub

    'clear all image list and items in ID_LISTVIEW_CAM
    Private Sub ID_BTN_CLEAR_ALL_Click(sender As Object, e As EventArgs) Handles ID_BTN_CLEAR_ALL.Click
        file_counter = 0
        ID_LISTVIEW_IMAGE.Clear()
        ID_LISTVIEW_IMAGE.Items.Clear()
        photoList.Images.Clear()
        ID_PICTURE_BOX_CAM.Image = Nothing
        ID_PICTURE_BOX(tab_Index).Image = Nothing
        DeleteImages(imagepath)
    End Sub

    'stop camera and display the selected image to ID_PICTURE_BOX
    Private Sub ID_LISTVIEW_IMAGE_DoubleClick(sender As Object, e As EventArgs) Handles ID_LISTVIEW_IMAGE.DoubleClick
        Try
            flag = True

            Dim itemSelected As Integer = GetListViewSelectedItemIndex(ID_LISTVIEW_IMAGE)
            SetListViewSelectedItem(ID_LISTVIEW_IMAGE, itemSelected)
            Dim Image As Image
            Using str As Stream = File.OpenRead(ID_LISTVIEW_IMAGE.SelectedItems(0).Tag)
                Image = Image.FromStream(str)
            End Using
            ID_PICTURE_BOX_CAM.Image = Image

            Dim page_num = tab_Index

            If tab_Index = 0 Or img_import_flag(tab_Index) = False Then
                For i = 1 To 24
                    If img_import_flag(i) = True Then
                        page_num = i
                        tag_page_flag(i) = True

                        ID_PICTURE_BOX(i).Image = Nothing
                        currentImageList(i) = Nothing
                        resizedImageList(i) = Nothing
                        originImageList(i) = Nothing
                        cur_obj_num(i) = 0
                        Enumerable.ElementAt(Of List(Of MeasureObject))(object_list, i).Clear()

                        ID_TAG_CTRL.TabPages.Add(ID_TAG_PAGE(i))
                        Exit For
                    End If
                Next
            Else
                page_num = tab_Index
            End If

            ID_PICTURE_BOX(page_num).LoadImageFromFile(ID_LISTVIEW_IMAGE.SelectedItems(0).Tag, originImageList, resizedImageList, currentImageList,
                                                         page_num, file_names)

            Dim img = originImageList(page_num).ToBitmap()
            ID_PICTURE_BOX(page_num).Image = img

            left_top = ID_PICTURE_BOX(page_num).CenteringImage(ID_PANEL(page_num))

            cur_obj_num(page_num) = 0
            zoomFactor(page_num) = 1.0
            Enumerable.ElementAt(Of List(Of MeasureObject))(object_list, page_num).Clear()

            img_import_flag(page_num) = False
            ID_TAG_PAGE(page_num).Text = file_names(page_num)
            ID_LISTVIEW.LoadObjectList(object_list.ElementAt(page_num), CF, digit, scaleUnit, name_list)

            ID_TAG_CTRL.SelectedTab = ID_TAG_PAGE(page_num)
            CalcRoiRect(scaleUnit, CFNum(CFSelected), RoiUnit, RoiSize, originImageList(page_num).Width, originImageList(page_num).Height, RoiRect)
        Catch ex As Exception
            MessageBox.Show(ex.ToString())
        End Try
    End Sub

    'display property window for the video capture
    Private Sub Btn_CameraProperties_Click(sender As Object, e As EventArgs) Handles Btn_CameraProperties.Click

        If videoDevice Is Nothing Then
            MsgBox("Please start Camera First")

        ElseIf videoDevice.IsRunning Then
            videoDevice.DisplayPropertyPage(Me.Handle)
        End If
    End Sub

    'set flag for live image so that live images can be displayed to tab
    Private Sub btn_live_Click(sender As Object, e As EventArgs) Handles btn_live.Click
        flag = False

    End Sub

    'change the resolution of webcam
    Private Sub CameraResolutionsCB_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CameraResolutionsCB.SelectedIndexChanged

        If CameraResolutionsCB.SelectedIndex > 0 Then
            My.Settings.camresindex = CameraResolutionsCB.SelectedIndex - 1
            My.Settings.Save()
            CloseCamera()
            Threading.Thread.Sleep(500)
            OpenCamera()
        End If

    End Sub

    'set the path of directory for captured images
    Private Sub btn_setpath_Click(sender As Object, e As EventArgs) Handles btn_setpath.Click
        Dim dialog = New FolderBrowserDialog With {
            .SelectedPath = Application.StartupPath
        }
        If DialogResult.OK = dialog.ShowDialog() Then
            txtbx_imagepath.Text = dialog.SelectedPath & "\MyImages"
            imagepath = txtbx_imagepath.Text
            My.Settings.imagefilepath = imagepath
            My.Settings.Save()
            Createdirectory(imagepath)
        End If
    End Sub

    'delete all captured images
    Private Sub btn_delete_Click(sender As Object, e As EventArgs) Handles btn_delete.Click

        For Each v As ListViewItem In ID_LISTVIEW_IMAGE.SelectedItems
            photoList.Images.RemoveAt(v.ImageIndex)
            ID_LISTVIEW_IMAGE.Items.Remove(v)
            Dim FileDelete As String = v.Tag
            If File.Exists(FileDelete) = True Then
                'File.Delete(FileDelete)
            End If
            'Remove_Tab()
        Next

    End Sub

    'open browser and load captured images
    Private Sub btn_browse_Click(sender As Object, e As EventArgs) Handles btn_browse.Click
        Dim ofd As New OpenFileDialog With {
            .Filter = "Image File (*.ico;*.jpg;*.jpeg;*.bmp;*.gif;*.png)|*.jpg;*.jpeg;*.bmp;*.gif;*.png;*.ico",
            .Multiselect = True,
            .FilterIndex = 1
        }

        If ofd.ShowDialog() = DialogResult.OK Then
            Try
                Dim files As String() = ofd.FileNames
                For Each file In files
                    Dim img1 As New Bitmap(file)
                    Createdirectory(imagepath)
                    If photoList.Images.Count <= 0 Then
                        file_counter = photoList.Images.Count + 1
                    Else
                        file_counter = Convert.ToInt32(IO.Path.GetFileNameWithoutExtension(photoList.Images.Keys.Item(photoList.Images.Count - 1).ToString()).Split("_")(1)) + 1
                    End If

                    img1.Save(imagepath & "\\test_" & (file_counter) & ".jpeg", Imaging.ImageFormat.Jpeg)
                    photoList.ImageSize = New Size(200, 150)
                    photoList.Images.Add("\\test_" & (file_counter) & ".jpeg", img1)
                    ID_LISTVIEW_IMAGE.LargeImageList = photoList
                    img1.Dispose()
                    ID_LISTVIEW_IMAGE.Items.Clear()
                    For index = 0 To photoList.Images.Count - 1
                        Dim item As New ListViewItem With {
                        .ImageIndex = index,
                            .Tag = imagepath & photoList.Images.Keys.Item(index).ToString(),
                            .Text = IO.Path.GetFileNameWithoutExtension(photoList.Images.Keys.Item(index).ToString())
                    }
                        ID_LISTVIEW_IMAGE.Items.Add(item)
                    Next

                Next

            Catch ex As Exception
                MessageBox.Show(ex.Message)
            End Try
        End If


    End Sub
#End Region


#Region "Curves Methods"



    ''' <summary>
    ''' set current measurement type as C_Line
    ''' </summary>
    Private Sub ID_BTN_C_LINE_Click(sender As Object, e As EventArgs) Handles ID_BTN_C_LINE.Click
        menuClick = False
        obj_selected.Refresh()
        curMeasureType = MeasureType.objLine
        obj_selected.measuringType = curMeasureType

    End Sub
    Private Sub LINEToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles LINEToolStripMenuItem1.Click
        menuClick = True
        obj_selected.Refresh()
        curMeasureType = MeasureType.objLine
        obj_selected.measuringType = curMeasureType

    End Sub

    ''' <summary>
    ''' set current measurement type as C_Poly
    ''' </summary>
    Private Sub ID_BTN_C_POLY_Click(sender As Object, e As EventArgs) Handles ID_BTN_C_POLY.Click
        menuClick = False
        obj_selected.Refresh()
        curMeasureType = MeasureType.objPoly
        obj_selected.measuringType = curMeasureType

    End Sub

    Private Sub POLYGENToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles POLYGENToolStripMenuItem.Click
        menuClick = True
        obj_selected.Refresh()
        curMeasureType = MeasureType.objPoly
        obj_selected.measuringType = curMeasureType

    End Sub

    ''' <summary>
    ''' set current measurement type as C_Point
    ''' </summary>
    Private Sub ID_BTN_C_POINT_Click(sender As Object, e As EventArgs) Handles ID_BTN_C_POINT.Click
        menuClick = False
        obj_selected.Refresh()
        curMeasureType = MeasureType.objPoint
        obj_selected.measuringType = curMeasureType

    End Sub

    Private Sub POINTToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles POINTToolStripMenuItem.Click
        menuClick = True
        obj_selected.Refresh()
        curMeasureType = MeasureType.objPoint
        obj_selected.measuringType = curMeasureType

    End Sub

    ''' <summary>
    ''' set current measurement type as C_Curve
    ''' </summary>
    Private Sub ID_BTN_C_CURVE_Click(sender As Object, e As EventArgs) Handles ID_BTN_C_CURVE.Click
        menuClick = False
        obj_selected.Refresh()
        curMeasureType = MeasureType.objCurve
        obj_selected.measuringType = curMeasureType
    End Sub

    Private Sub CURVEToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CURVEToolStripMenuItem.Click
        menuClick = True
        obj_selected.Refresh()
        curMeasureType = MeasureType.objCurve
        obj_selected.measuringType = curMeasureType

    End Sub

    ''' <summary>
    ''' set current measurement type as C_Cupoly
    ''' </summary>
    Private Sub ID_BTN_C_CUPOLY_Click(sender As Object, e As EventArgs) Handles ID_BTN_C_CUPOLY.Click
        menuClick = False
        obj_selected.Refresh()
        curMeasureType = MeasureType.objCuPoly
        obj_selected.measuringType = curMeasureType

    End Sub

    Private Sub CURVEPOLYGENToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CURVEPOLYGENToolStripMenuItem.Click
        menuClick = True
        obj_selected.Refresh()
        curMeasureType = MeasureType.objCuPoly
        obj_selected.measuringType = curMeasureType

    End Sub

    ''' <summary>
    ''' set current measurement type as C_Sel
    ''' </summary>
    Private Sub ID_BTN_C_SEL_Click(sender As Object, e As EventArgs) Handles ID_BTN_C_SEL.Click
        menuClick = False
        obj_selected.Refresh()
        curMeasureType = MeasureType.objSel
        obj_selected.measuringType = curMeasureType

    End Sub

    ''' <summary>
    ''' Add Curve object to obj list
    ''' </summary>
    Private Sub AddCurveToList()
        AddMaxMinToList()

        ID_PICTURE_BOX(tab_Index).DrawObjList(object_list.ElementAt(tab_Index), digit, CF, False)
        ID_LISTVIEW.LoadObjectList(object_list.ElementAt(tab_Index), CF, digit, scaleUnit, name_list)
    End Sub

    Private Sub AddMaxMinToList()
        obj_selected.objNum = cur_obj_num(tab_Index)
        GetObjName(object_list(tab_Index), obj_selected, scaleUnit)
        SetLineAndFont(obj_selected, line_infor, font_infor)
        object_list(tab_Index).Add(obj_selected)
        obj_selected.Refresh()
        curMeasureType = -1
        cur_obj_num(tab_Index) += 1
        If undo_num < 2 Then undo_num += 1
    End Sub

    ''' <summary>
    ''' calculate minimum distance between two selected objects
    ''' </summary>
    Private Sub MinCalcBtn_Click(sender As Object, e As EventArgs) Handles MinCalcBtn.Click
        ID_STATUS_LABEL.Text = "Calculate minimum distance between selected objects."

        If CuPolyRealSelectArrayIndx >= 0 And LRealSelectArrayIndx >= 0 Then
            Dim obj1 = object_list.ElementAt(tab_Index).ElementAt(CuPolyRealSelectArrayIndx)
            Dim obj2 = object_list.ElementAt(tab_Index).ElementAt(LRealSelectArrayIndx)
            obj_selected = CalcMinBetweenCuPolyAndLine(obj1, obj2, ID_PICTURE_BOX(tab_Index).Width, ID_PICTURE_BOX(tab_Index).Height)
            AddMaxMinToList()
        End If
        If CuPolyRealSelectArrayIndx >= 0 And PRealSelectArrayIndx >= 0 Then
            Dim obj1 = object_list.ElementAt(tab_Index).ElementAt(CuPolyRealSelectArrayIndx)
            Dim obj2 = object_list.ElementAt(tab_Index).ElementAt(PRealSelectArrayIndx)
            obj_selected = CalcMinBetweenCuPolyAndPoint(obj1, obj2, ID_PICTURE_BOX(tab_Index).Width, ID_PICTURE_BOX(tab_Index).Height)
            AddMaxMinToList()
        End If
        If CRealSelectArrayIndx >= 0 And LRealSelectArrayIndx >= 0 Then
            Dim obj1 = object_list.ElementAt(tab_Index).ElementAt(CRealSelectArrayIndx)
            Dim obj2 = object_list.ElementAt(tab_Index).ElementAt(LRealSelectArrayIndx)
            obj_selected = CalcMinBetweenCurveAndLine(obj1, obj2, ID_PICTURE_BOX(tab_Index).Width, ID_PICTURE_BOX(tab_Index).Height)
            AddMaxMinToList()
        End If
        If CRealSelectArrayIndx >= 0 And PRealSelectArrayIndx >= 0 Then
            Dim obj1 = object_list.ElementAt(tab_Index).ElementAt(CRealSelectArrayIndx)
            Dim obj2 = object_list.ElementAt(tab_Index).ElementAt(PRealSelectArrayIndx)
            obj_selected = CalcMinBetweenCurveAndPoint(obj1, obj2, ID_PICTURE_BOX(tab_Index).Width, ID_PICTURE_BOX(tab_Index).Height)
            AddMaxMinToList()
        End If
        If PRealSelectArrayIndx >= 0 And LRealSelectArrayIndx >= 0 Then
            Dim obj1 = object_list.ElementAt(tab_Index).ElementAt(PRealSelectArrayIndx)
            Dim obj2 = object_list.ElementAt(tab_Index).ElementAt(LRealSelectArrayIndx)
            obj_selected = CalcMinBetweenPointAndLine(obj1, obj2, ID_PICTURE_BOX(tab_Index).Width, ID_PICTURE_BOX(tab_Index).Height)
            AddMaxMinToList()
        End If
        If PRealSelectArrayIndx >= 0 And PolyRealSelectArrayIndx >= 0 Then
            Dim obj1 = object_list.ElementAt(tab_Index).ElementAt(PRealSelectArrayIndx)
            Dim obj2 = object_list.ElementAt(tab_Index).ElementAt(PolyRealSelectArrayIndx)
            obj_selected = CalcMinBetweenPointAndPoly(obj1, obj2, ID_PICTURE_BOX(tab_Index).Width, ID_PICTURE_BOX(tab_Index).Height)
            AddMaxMinToList()
        End If
        If LRealSelectArrayIndx >= 0 And PolyRealSelectArrayIndx >= 0 Then
            Dim obj1 = object_list.ElementAt(tab_Index).ElementAt(LRealSelectArrayIndx)
            Dim obj2 = object_list.ElementAt(tab_Index).ElementAt(PolyRealSelectArrayIndx)
            obj_selected = CalcMinBetweenLineAndPoly(obj1, obj2, ID_PICTURE_BOX(tab_Index).Width, ID_PICTURE_BOX(tab_Index).Height)
            AddMaxMinToList()
        End If
        If CRealSelectArrayIndx >= 0 And PolyRealSelectArrayIndx >= 0 Then
            Dim obj1 = object_list.ElementAt(tab_Index).ElementAt(CRealSelectArrayIndx)
            Dim obj2 = object_list.ElementAt(tab_Index).ElementAt(PolyRealSelectArrayIndx)
            obj_selected = CalcMinBetweenCurveAndPoly(obj1, obj2, ID_PICTURE_BOX(tab_Index).Width, ID_PICTURE_BOX(tab_Index).Height)
            AddMaxMinToList()
        End If

        ID_PICTURE_BOX(tab_Index).DrawObjList(object_list.ElementAt(tab_Index), digit, CF, False)
        ID_LISTVIEW.LoadObjectList(object_list.ElementAt(tab_Index), CF, digit, scaleUnit, name_list)
    End Sub

    ''' <summary>
    ''' calculate maximum distance between two selected objects
    ''' </summary>
    Private Sub MaxCalcBtn_Click(sender As Object, e As EventArgs) Handles MaxCalcBtn.Click
        ID_STATUS_LABEL.Text = "Calculate maximum distance between selected objects."

        If CuPolyRealSelectArrayIndx >= 0 And LRealSelectArrayIndx >= 0 Then
            Dim obj1 = object_list.ElementAt(tab_Index).ElementAt(CuPolyRealSelectArrayIndx)
            Dim obj2 = object_list.ElementAt(tab_Index).ElementAt(LRealSelectArrayIndx)
            obj_selected = CalcMaxBetweenCuPolyAndLine(obj1, obj2, ID_PICTURE_BOX(tab_Index).Width, ID_PICTURE_BOX(tab_Index).Height)
            AddMaxMinToList()
        End If
        If CuPolyRealSelectArrayIndx >= 0 And PRealSelectArrayIndx >= 0 Then
            Dim obj1 = object_list.ElementAt(tab_Index).ElementAt(CuPolyRealSelectArrayIndx)
            Dim obj2 = object_list.ElementAt(tab_Index).ElementAt(PRealSelectArrayIndx)
            obj_selected = CalcMaxBetweenCuPolyAndPoint(obj1, obj2, ID_PICTURE_BOX(tab_Index).Width, ID_PICTURE_BOX(tab_Index).Height)
            AddMaxMinToList()
        End If
        If CRealSelectArrayIndx >= 0 And LRealSelectArrayIndx >= 0 Then
            Dim obj1 = object_list.ElementAt(tab_Index).ElementAt(CRealSelectArrayIndx)
            Dim obj2 = object_list.ElementAt(tab_Index).ElementAt(LRealSelectArrayIndx)
            obj_selected = CalcMaxBetweenCurveAndLine(obj1, obj2, ID_PICTURE_BOX(tab_Index).Width, ID_PICTURE_BOX(tab_Index).Height)
            AddMaxMinToList()
        End If
        If CRealSelectArrayIndx >= 0 And PRealSelectArrayIndx >= 0 Then
            Dim obj1 = object_list.ElementAt(tab_Index).ElementAt(CRealSelectArrayIndx)
            Dim obj2 = object_list.ElementAt(tab_Index).ElementAt(PRealSelectArrayIndx)
            obj_selected = CalcMaxBetweenCurveAndPoint(obj1, obj2, ID_PICTURE_BOX(tab_Index).Width, ID_PICTURE_BOX(tab_Index).Height)
            AddMaxMinToList()
        End If
        If CRealSelectArrayIndx >= 0 And PolyRealSelectArrayIndx >= 0 Then
            Dim obj1 = object_list.ElementAt(tab_Index).ElementAt(CRealSelectArrayIndx)
            Dim obj2 = object_list.ElementAt(tab_Index).ElementAt(PolyRealSelectArrayIndx)
            obj_selected = CalcMaxBetweenCurveAndPoly(obj1, obj2, ID_PICTURE_BOX(tab_Index).Width, ID_PICTURE_BOX(tab_Index).Height)
            AddMaxMinToList()
        End If
        If LRealSelectArrayIndx >= 0 And PRealSelectArrayIndx >= 0 Then
            Dim obj1 = object_list.ElementAt(tab_Index).ElementAt(LRealSelectArrayIndx)
            Dim obj2 = object_list.ElementAt(tab_Index).ElementAt(PRealSelectArrayIndx)
            obj_selected = CalcMaxBetweenLineAndPoint(obj1, obj2, ID_PICTURE_BOX(tab_Index).Width, ID_PICTURE_BOX(tab_Index).Height)
            AddMaxMinToList()
        End If
        If LRealSelectArrayIndx >= 0 And PolyRealSelectArrayIndx >= 0 Then
            Dim obj1 = object_list.ElementAt(tab_Index).ElementAt(LRealSelectArrayIndx)
            Dim obj2 = object_list.ElementAt(tab_Index).ElementAt(PolyRealSelectArrayIndx)
            obj_selected = CalcMaxBetweenLineAndPoly(obj1, obj2, ID_PICTURE_BOX(tab_Index).Width, ID_PICTURE_BOX(tab_Index).Height)
            AddMaxMinToList()
        End If
        If PolyRealSelectArrayIndx >= 0 And PRealSelectArrayIndx >= 0 Then
            Dim obj1 = object_list.ElementAt(tab_Index).ElementAt(PolyRealSelectArrayIndx)
            Dim obj2 = object_list.ElementAt(tab_Index).ElementAt(PRealSelectArrayIndx)
            obj_selected = CalcMaxBetweenPolyAndPoint(obj1, obj2, ID_PICTURE_BOX(tab_Index).Width, ID_PICTURE_BOX(tab_Index).Height)
            AddMaxMinToList()
        End If
        ID_PICTURE_BOX(tab_Index).DrawObjList(object_list.ElementAt(tab_Index), digit, CF, False)
        ID_LISTVIEW.LoadObjectList(object_list.ElementAt(tab_Index), CF, digit, scaleUnit, name_list)
    End Sub

    ''' <summary>
    ''' calculate minimum perpendicular distance between two selected objects
    ''' </summary>
    Private Sub PerMin_Click(sender As Object, e As EventArgs) Handles PerMin.Click
        ID_STATUS_LABEL.Text = "Calculate perpendicular minimum distance between selected objects."

        If CuPolyRealSelectArrayIndx >= 0 And LRealSelectArrayIndx >= 0 Then
            Dim obj1 = object_list.ElementAt(tab_Index).ElementAt(CuPolyRealSelectArrayIndx)
            Dim obj2 = object_list.ElementAt(tab_Index).ElementAt(LRealSelectArrayIndx)
            obj_selected = CalcPMinBetweenCuPolyAndLine(obj1, obj2, ID_PICTURE_BOX(tab_Index).Width, ID_PICTURE_BOX(tab_Index).Height)
            AddMaxMinToList()
        End If
        If CuPolyRealSelectArrayIndx >= 0 And PRealSelectArrayIndx >= 0 Then
            Dim obj1 = object_list.ElementAt(tab_Index).ElementAt(CuPolyRealSelectArrayIndx)
            Dim obj2 = object_list.ElementAt(tab_Index).ElementAt(PRealSelectArrayIndx)
            obj_selected = CalcPMinBetweenCuPolyAndPoint(obj1, obj2, ID_PICTURE_BOX(tab_Index).Width, ID_PICTURE_BOX(tab_Index).Height)
            AddMaxMinToList()
        End If
        If CRealSelectArrayIndx >= 0 And LRealSelectArrayIndx >= 0 Then
            Dim obj1 = object_list.ElementAt(tab_Index).ElementAt(CRealSelectArrayIndx)
            Dim obj2 = object_list.ElementAt(tab_Index).ElementAt(LRealSelectArrayIndx)
            obj_selected = CalcPMinBetweenCurveAndLine(obj1, obj2, ID_PICTURE_BOX(tab_Index).Width, ID_PICTURE_BOX(tab_Index).Height)
            AddMaxMinToList()
        End If
        If CRealSelectArrayIndx >= 0 And PRealSelectArrayIndx >= 0 Then
            Dim obj1 = object_list.ElementAt(tab_Index).ElementAt(CRealSelectArrayIndx)
            Dim obj2 = object_list.ElementAt(tab_Index).ElementAt(PRealSelectArrayIndx)
            obj_selected = CalcPMinBetweenCurveAndPoint(obj1, obj2, ID_PICTURE_BOX(tab_Index).Width, ID_PICTURE_BOX(tab_Index).Height)
            AddMaxMinToList()
        End If
        If PRealSelectArrayIndx >= 0 And LRealSelectArrayIndx >= 0 Then
            Dim obj1 = object_list.ElementAt(tab_Index).ElementAt(PRealSelectArrayIndx)
            Dim obj2 = object_list.ElementAt(tab_Index).ElementAt(LRealSelectArrayIndx)
            obj_selected = CalcPMinBetweenPointAndLine(obj1, obj2, ID_PICTURE_BOX(tab_Index).Width, ID_PICTURE_BOX(tab_Index).Height)
            AddMaxMinToList()
        End If
        If PRealSelectArrayIndx >= 0 And PolyRealSelectArrayIndx >= 0 Then
            Dim obj1 = object_list.ElementAt(tab_Index).ElementAt(PRealSelectArrayIndx)
            Dim obj2 = object_list.ElementAt(tab_Index).ElementAt(PolyRealSelectArrayIndx)
            obj_selected = CalcPMinBetweenPointAndPoly(obj1, obj2, ID_PICTURE_BOX(tab_Index).Width, ID_PICTURE_BOX(tab_Index).Height)
            AddMaxMinToList()
        End If
        If LRealSelectArrayIndx >= 0 And PolyRealSelectArrayIndx >= 0 Then
            Dim obj1 = object_list.ElementAt(tab_Index).ElementAt(LRealSelectArrayIndx)
            Dim obj2 = object_list.ElementAt(tab_Index).ElementAt(PolyRealSelectArrayIndx)
            obj_selected = CalcPMinBetweenLineAndPoly(obj1, obj2, ID_PICTURE_BOX(tab_Index).Width, ID_PICTURE_BOX(tab_Index).Height)
            AddMaxMinToList()
        End If
        If CRealSelectArrayIndx >= 0 And PolyRealSelectArrayIndx >= 0 Then
            Dim obj1 = object_list.ElementAt(tab_Index).ElementAt(CRealSelectArrayIndx)
            Dim obj2 = object_list.ElementAt(tab_Index).ElementAt(PolyRealSelectArrayIndx)
            obj_selected = CalcPMinBetweenCurveAndPoly(obj1, obj2, ID_PICTURE_BOX(tab_Index).Width, ID_PICTURE_BOX(tab_Index).Height)
            AddMaxMinToList()
        End If

        ID_PICTURE_BOX(tab_Index).DrawObjList(object_list.ElementAt(tab_Index), digit, CF, False)
        ID_LISTVIEW.LoadObjectList(object_list.ElementAt(tab_Index), CF, digit, scaleUnit, name_list)
    End Sub

    ''' <summary>
    ''' calculate maximum perpendicular distance between two selected objects
    ''' </summary>
    Private Sub PerMax_Click(sender As Object, e As EventArgs) Handles PerMax.Click
        ID_STATUS_LABEL.Text = "Calculate perpendicular maximum distance between selected objects."

        If CuPolyRealSelectArrayIndx >= 0 And LRealSelectArrayIndx >= 0 Then
            Dim obj1 = object_list.ElementAt(tab_Index).ElementAt(CuPolyRealSelectArrayIndx)
            Dim obj2 = object_list.ElementAt(tab_Index).ElementAt(LRealSelectArrayIndx)
            obj_selected = CalcPMaxBetweenCuPolyAndLine(obj1, obj2, ID_PICTURE_BOX(tab_Index).Width, ID_PICTURE_BOX(tab_Index).Height)
            AddMaxMinToList()
        End If
        If CuPolyRealSelectArrayIndx >= 0 And PRealSelectArrayIndx >= 0 Then
            Dim obj1 = object_list.ElementAt(tab_Index).ElementAt(CuPolyRealSelectArrayIndx)
            Dim obj2 = object_list.ElementAt(tab_Index).ElementAt(PRealSelectArrayIndx)
            obj_selected = CalcPMaxBetweenCuPolyAndPoint(obj1, obj2, ID_PICTURE_BOX(tab_Index).Width, ID_PICTURE_BOX(tab_Index).Height)
            AddMaxMinToList()
        End If
        If CRealSelectArrayIndx >= 0 And LRealSelectArrayIndx >= 0 Then
            Dim obj1 = object_list.ElementAt(tab_Index).ElementAt(CRealSelectArrayIndx)
            Dim obj2 = object_list.ElementAt(tab_Index).ElementAt(LRealSelectArrayIndx)
            obj_selected = CalcPMaxBetweenCurveAndLine(obj1, obj2, ID_PICTURE_BOX(tab_Index).Width, ID_PICTURE_BOX(tab_Index).Height)
            AddMaxMinToList()
        End If
        If CRealSelectArrayIndx >= 0 And PRealSelectArrayIndx >= 0 Then
            Dim obj1 = object_list.ElementAt(tab_Index).ElementAt(CRealSelectArrayIndx)
            Dim obj2 = object_list.ElementAt(tab_Index).ElementAt(PRealSelectArrayIndx)
            obj_selected = CalcPMaxBetweenCurveAndPoint(obj1, obj2, ID_PICTURE_BOX(tab_Index).Width, ID_PICTURE_BOX(tab_Index).Height)
            AddMaxMinToList()
        End If
        If CRealSelectArrayIndx >= 0 And PolyRealSelectArrayIndx >= 0 Then
            Dim obj1 = object_list.ElementAt(tab_Index).ElementAt(CRealSelectArrayIndx)
            Dim obj2 = object_list.ElementAt(tab_Index).ElementAt(PolyRealSelectArrayIndx)
            obj_selected = CalcPMaxBetweenCurveAndPoly(obj1, obj2, ID_PICTURE_BOX(tab_Index).Width, ID_PICTURE_BOX(tab_Index).Height)
            AddMaxMinToList()
        End If
        If LRealSelectArrayIndx >= 0 And PRealSelectArrayIndx >= 0 Then
            Dim obj1 = object_list.ElementAt(tab_Index).ElementAt(LRealSelectArrayIndx)
            Dim obj2 = object_list.ElementAt(tab_Index).ElementAt(PRealSelectArrayIndx)
            obj_selected = CalcPMaxBetweenLineAndPoint(obj1, obj2, ID_PICTURE_BOX(tab_Index).Width, ID_PICTURE_BOX(tab_Index).Height)
            AddMaxMinToList()
        End If
        If LRealSelectArrayIndx >= 0 And PolyRealSelectArrayIndx >= 0 Then
            Dim obj1 = object_list.ElementAt(tab_Index).ElementAt(LRealSelectArrayIndx)
            Dim obj2 = object_list.ElementAt(tab_Index).ElementAt(PolyRealSelectArrayIndx)
            obj_selected = CalcPMaxBetweenLineAndPoly(obj1, obj2, ID_PICTURE_BOX(tab_Index).Width, ID_PICTURE_BOX(tab_Index).Height)
            AddMaxMinToList()
        End If
        If PolyRealSelectArrayIndx >= 0 And PRealSelectArrayIndx >= 0 Then
            Dim obj1 = object_list.ElementAt(tab_Index).ElementAt(PolyRealSelectArrayIndx)
            Dim obj2 = object_list.ElementAt(tab_Index).ElementAt(PRealSelectArrayIndx)
            obj_selected = CalcPMaxBetweenPolyAndPoint(obj1, obj2, ID_PICTURE_BOX(tab_Index).Width, ID_PICTURE_BOX(tab_Index).Height)
            AddMaxMinToList()
        End If
        ID_PICTURE_BOX(tab_Index).DrawObjList(object_list.ElementAt(tab_Index), digit, CF, False)
        ID_LISTVIEW.LoadObjectList(object_list.ElementAt(tab_Index), CF, digit, scaleUnit, name_list)
    End Sub
#End Region

#Region "Segmentation Tool"
    Private Sub CIRCLEDETECTIONToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CIRCLEDETECTIONToolStripMenuItem.Click
        Obj_Seg.Refresh()
        Obj_Seg.measureType = SegType.circle
        Dim form = New Circle()
        form.Show()
    End Sub

    Private Sub CALIBRATIONToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CALIBRATIONToolStripMenuItem.Click
        Dim form = New Calibration(CFList, CFSelected, CFNum, CFUnit)
        form.Show()

    End Sub

    Private Sub SETROIToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SETROIToolStripMenuItem.Click
        Dim form = New SetROI(RoiSize, RoiUnit)
        If form.ShowDialog = DialogResult.OK Then
            RoiSize = form.NumSizeROI.Value
            RoiUnit = form.RoiUnit
            UpdateConfigInfo()
        End If
    End Sub

    Private Sub GRAINMEASUREMENTToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GRAINMEASUREMENTToolStripMenuItem.Click
        Obj_Seg.Refresh()
        Obj_Seg.measureType = SegType.intercept

        Dim form As New GrainMeasurement
        form.Show()
    End Sub

    Private Sub DoMorphological(output As Bitmap)
        Dim newMat = GetMatFromSDImage(output)
        'DisposeElemOfList(currentImageList, newMat, tab_Index)
        currentImageList(tab_Index) = newMat.Clone()
        Zoom_Image()
        output.Dispose()
        newMat.Dispose()
    End Sub
    Private Sub DILATIONToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DILATIONToolStripMenuItem.Click
        Dim output = Dilation(currentImageList(tab_Index).ToBitmap)
        DoMorphological(output)
    End Sub

    Private Sub EROSIONToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EROSIONToolStripMenuItem.Click
        Dim output = Erosion(currentImageList(tab_Index).ToBitmap)
        DoMorphological(output)
    End Sub

    Private Sub DENSITYToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DENSITYToolStripMenuItem.Click
        Dim form = New Density()
        form.Show()
    End Sub

    Private Sub COLORMANUALLYToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles COLORMANUALLYToolStripMenuItem.Click
        Obj_Seg.Refresh()
        Obj_Seg.measureType = SegType.phaseSegmentation
        Dim form = New Segmentation()
        If form.ShowDialog() = DialogResult.OK Then
            ShowResult(GetMatFromSDImage(form.ResImage.ToBitmap()))
        End If
    End Sub

    Private Sub PHASESEGMENTATIONAUTOMATICToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PHASESEGMENTATIONAUTOMATICToolStripMenuItem.Click
        Obj_Seg.Refresh()
        Obj_Seg.measureType = SegType.phaseSegmentation
        Dim form = New PhaseSegmentASTM()
        form.Show()
    End Sub

    Private Sub PHASESEGMENTATIONMANUALToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PHASESEGMENTATIONMANUALToolStripMenuItem.Click
        Obj_Seg.Refresh()
        Obj_Seg.measureType = SegType.phaseSegmentation
        Dim form = New Phase_Segmentation()
        form.Show()
    End Sub

    Private Sub INTERSECTIONToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles INTERSECTIONToolStripMenuItem.Click
        Obj_Seg.Refresh()
        Obj_Seg.measureType = SegType.intersection
        Dim form = New Intersection()
        form.Show()
    End Sub

    Private Sub COUNTCLASSIFICATIONToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles COUNTCLASSIFICATIONToolStripMenuItem.Click
        Obj_Seg.Refresh()
        Obj_Seg.measureType = SegType.BlobSegment
        Dim form = New Count_Classification()
        form.Show()
    End Sub

    Private Sub PARTICIPLESIZEToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PARTICIPLESIZEToolStripMenuItem.Click
        Obj_Seg.Refresh()
        Obj_Seg.measureType = SegType.BlobSegment
        Dim form = New ParticipleSize()
        form.Show()
    End Sub

    Private Sub NODULARITYToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles NODULARITYToolStripMenuItem.Click
        Obj_Seg.Refresh()
        Obj_Seg.measureType = SegType.BlobSegment
        Dim form = New Nodularity()
        form.Show()
    End Sub
#End Region
End Class
