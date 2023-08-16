Imports Emgu.CV
''' <summary>
''' This class contains all the functions for GrainMeasurments.
''' </summary>
Public Module GrainMethod
    ''' <summary>
    ''' add intercepts
    ''' </summary>
    ''' <paramname="Dst">the InterCeptObj.</param>
    ''' <paramname="mX">The x position of new cept.</param>
    ''' <paramname="mY">The y position of new cept.</param>
    Public Sub AddIntercepts(ByRef Dst As InterCeptObj, Width As Integer, Height As Integer, mX As Integer, mY As Integer)
        If Dst.numLine = 0 Then
            Return
        End If
        If 0 <= Dst.Degree And Dst.Degree <= Math.PI / 2 Then
            Dim extra_Y_axis = Width * Math.Tan(Dst.Degree)
            Dim total_Y_axis = Height + extra_Y_axis
            Dim step_y As Integer = total_Y_axis / (Dst.numLine + 1)

            For i = 0 To Dst.numLine - 1

                Dim x1 = 0
                Dim y1 = step_y * (i + 1)
                Dim x2 = Width - 1
                Dim y2 = y1 - extra_Y_axis

                Dim dist = pFind_BPointLineDistance(x1, y1, x2, y2, mX, mY)
                If dist < 10 Then
                    Dst.CeptPos(i, Dst.CeptCnt(i)) = New PointF(mX / Width, mY / Height)
                    Dst.CeptCnt(i) += 1
                End If

            Next
        Else
            Dim extra_X_axis = Height * Math.Tan(Math.PI / 2 - Dst.Degree)
            Dim total_X_axis = Width + extra_X_axis
            Dim step_x As Integer = total_X_axis / (Dst.numLine + 1)

            For i = 0 To Dst.numLine - 1

                Dim x1 = step_x * (i + 1)
                Dim y1 = 0
                Dim x2 = x1 - extra_X_axis
                Dim y2 = Height - 1

                Dim dist = pFind_BPointLineDistance(x1, y1, x2, y2, mX, mY)
                If dist < 10 Then
                    Dst.CeptPos(i, Dst.CeptCnt(i)) = New PointF(mX / Width, mY / Height)
                    Dst.CeptCnt(i) += 1
                End If
            Next
        End If
    End Sub

    ''' <summary>
    ''' remove intercepts
    ''' </summary>
    ''' <paramname="Dst">the InterCeptObj.</param>
    ''' <paramname="mX">The x position of new cept.</param>
    ''' <paramname="mY">The y position of new cept.</param>
    Public Sub RemoveIntercepts(ByRef Dst As InterCeptObj, Width As Integer, Height As Integer, mPtX As Integer, mPtY As Integer)
        If Dst.numLine = 0 Then
            Return
        End If

        For i = 0 To Dst.numLine - 1
            For j = 0 To Dst.CeptCnt(i) - 1
                Dim PosX = Dst.CeptPos(i, j).X * Width
                Dim PosY = Dst.CeptPos(i, j).Y * Height

                If PosX > mPtX - 5 And PosX < mPtX + 5 And PosY > mPtY - 5 And PosY < mPtY + 5 Then
                    For k = j To Dst.CeptCnt(i) - 2
                        Dst.CeptPos(i, k) = Dst.CeptPos(i, k + 1)
                    Next
                    Dst.CeptPos(i, Dst.CeptCnt(i) - 1).X = 0
                    Dst.CeptPos(i, Dst.CeptCnt(i) - 1).Y = 0
                    Dst.CeptCnt(i) -= 1
                End If
            Next
        Next
    End Sub

    ''' <summary>
    ''' add intercepts
    ''' </summary>
    ''' <paramname="Dst">the InterCeptObj.</param>
    ''' <paramname="mX">The x position of new cept.</param>
    ''' <paramname="mY">The y position of new cept.</param>
    Public Sub AddCircleIntercepts(ByRef Dst As InterCeptObj, Width As Integer, Height As Integer, mX As Integer, mY As Integer)
        Dim MinLen = Math.Min(Width, Height)
        Dim minRadius As Single = MinLen / 8

        Dim centerX As Integer = Width / 2
        Dim centerY As Integer = Height / 2

        Dim offset = CalcDistBetweenPoints(centerX, centerY, mX, mY)

        For i = 1 To 3
            Dim radius = minRadius * i
            Dim dist = Math.Abs(radius - offset)

            If dist < 5 Then
                Dim pos = New PointF(mX / CDbl(Width), mY / CDbl(Height))
                Dst.CeptPos(i, Dst.CeptCnt(i)) = pos
                Dst.CeptCnt(i) += 1
            End If
        Next
    End Sub

    Public Function GetGrainNo(area As Single) As Integer
        Dim index As Integer = Math.Sqrt(area)
        index = Math.Min(Math.Max(index, 0), 28)
        Return index
    End Function

    Public Sub ShowResult(output As Mat)
        Dim sz = New Size(Main_Form.resizedImageList(Main_Form.tab_Index).Width, Main_Form.resizedImageList(Main_Form.tab_Index).Height)
        Dim Resized As Mat = New Mat()
        CvInvoke.Resize(output, Resized, sz)
        Main_Form.ID_PICTURE_BOX(Main_Form.tab_Index).Image = Resized.ToBitmap()
        Main_Form.currentImageList(Main_Form.tab_Index) = output.Clone()
        output.Dispose()
    End Sub
End Module
