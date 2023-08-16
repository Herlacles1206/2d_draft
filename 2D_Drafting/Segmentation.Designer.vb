<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Segmentation
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.Pic1 = New System.Windows.Forms.PictureBox()
        Me.Pic2 = New System.Windows.Forms.PictureBox()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.TxtArea = New System.Windows.Forms.TextBox()
        Me.BtnColor = New System.Windows.Forms.Button()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.LabColor = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.BtnBlue = New System.Windows.Forms.Button()
        Me.BtnGreen = New System.Windows.Forms.Button()
        Me.BtnRed = New System.Windows.Forms.Button()
        Me.TxtMin = New System.Windows.Forms.TextBox()
        Me.TxtMax = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.BarMin = New System.Windows.Forms.TrackBar()
        Me.BarMax = New System.Windows.Forms.TrackBar()
        Me.Button5 = New System.Windows.Forms.Button()
        Me.Button6 = New System.Windows.Forms.Button()
        Me.BtnReport = New System.Windows.Forms.Button()
        CType(Me.Pic1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Pic2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox1.SuspendLayout()
        CType(Me.BarMin, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BarMax, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Pic1
        '
        Me.Pic1.Location = New System.Drawing.Point(12, 12)
        Me.Pic1.Name = "Pic1"
        Me.Pic1.Size = New System.Drawing.Size(153, 137)
        Me.Pic1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.Pic1.TabIndex = 0
        Me.Pic1.TabStop = False
        '
        'Pic2
        '
        Me.Pic2.Location = New System.Drawing.Point(194, 12)
        Me.Pic2.Name = "Pic2"
        Me.Pic2.Size = New System.Drawing.Size(153, 137)
        Me.Pic2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.Pic2.TabIndex = 1
        Me.Pic2.TabStop = False
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.TxtArea)
        Me.GroupBox1.Controls.Add(Me.BtnColor)
        Me.GroupBox1.Controls.Add(Me.Label3)
        Me.GroupBox1.Controls.Add(Me.LabColor)
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Controls.Add(Me.BtnBlue)
        Me.GroupBox1.Controls.Add(Me.BtnGreen)
        Me.GroupBox1.Controls.Add(Me.BtnRed)
        Me.GroupBox1.Location = New System.Drawing.Point(12, 155)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(335, 93)
        Me.GroupBox1.TabIndex = 2
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Select Color"
        '
        'TxtArea
        '
        Me.TxtArea.Location = New System.Drawing.Point(268, 57)
        Me.TxtArea.Name = "TxtArea"
        Me.TxtArea.Size = New System.Drawing.Size(43, 20)
        Me.TxtArea.TabIndex = 6
        '
        'BtnColor
        '
        Me.BtnColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnColor.Location = New System.Drawing.Point(105, 54)
        Me.BtnColor.Name = "BtnColor"
        Me.BtnColor.Size = New System.Drawing.Size(61, 23)
        Me.BtnColor.TabIndex = 3
        Me.BtnColor.UseVisualStyleBackColor = True
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(219, 60)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(43, 13)
        Me.Label3.TabIndex = 5
        Me.Label3.Text = "Area(%)"
        '
        'LabColor
        '
        Me.LabColor.AutoSize = True
        Me.LabColor.Location = New System.Drawing.Point(166, 60)
        Me.LabColor.Name = "LabColor"
        Me.LabColor.Size = New System.Drawing.Size(27, 13)
        Me.LabColor.TabIndex = 4
        Me.LabColor.Text = "Red"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(23, 60)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(76, 13)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "Select Color-->"
        '
        'BtnBlue
        '
        Me.BtnBlue.Location = New System.Drawing.Point(235, 19)
        Me.BtnBlue.Name = "BtnBlue"
        Me.BtnBlue.Size = New System.Drawing.Size(75, 23)
        Me.BtnBlue.TabIndex = 2
        Me.BtnBlue.Text = "Blue"
        Me.BtnBlue.UseVisualStyleBackColor = True
        '
        'BtnGreen
        '
        Me.BtnGreen.Location = New System.Drawing.Point(130, 19)
        Me.BtnGreen.Name = "BtnGreen"
        Me.BtnGreen.Size = New System.Drawing.Size(75, 23)
        Me.BtnGreen.TabIndex = 1
        Me.BtnGreen.Text = "Green"
        Me.BtnGreen.UseVisualStyleBackColor = True
        '
        'BtnRed
        '
        Me.BtnRed.Location = New System.Drawing.Point(26, 19)
        Me.BtnRed.Name = "BtnRed"
        Me.BtnRed.Size = New System.Drawing.Size(75, 23)
        Me.BtnRed.TabIndex = 0
        Me.BtnRed.Text = "Red"
        Me.BtnRed.UseVisualStyleBackColor = True
        '
        'TxtMin
        '
        Me.TxtMin.Location = New System.Drawing.Point(279, 267)
        Me.TxtMin.Name = "TxtMin"
        Me.TxtMin.Size = New System.Drawing.Size(43, 20)
        Me.TxtMin.TabIndex = 7
        '
        'TxtMax
        '
        Me.TxtMax.Location = New System.Drawing.Point(279, 303)
        Me.TxtMax.Name = "TxtMax"
        Me.TxtMax.Size = New System.Drawing.Size(43, 20)
        Me.TxtMax.TabIndex = 8
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(35, 270)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(24, 13)
        Me.Label4.TabIndex = 7
        Me.Label4.Text = "Min"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(35, 306)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(27, 13)
        Me.Label5.TabIndex = 9
        Me.Label5.Text = "Max"
        '
        'BarMin
        '
        Me.BarMin.Location = New System.Drawing.Point(83, 267)
        Me.BarMin.Maximum = 255
        Me.BarMin.Name = "BarMin"
        Me.BarMin.Size = New System.Drawing.Size(175, 45)
        Me.BarMin.TabIndex = 10
        '
        'BarMax
        '
        Me.BarMax.Location = New System.Drawing.Point(83, 303)
        Me.BarMax.Maximum = 255
        Me.BarMax.Name = "BarMax"
        Me.BarMax.Size = New System.Drawing.Size(175, 45)
        Me.BarMax.TabIndex = 11
        '
        'Button5
        '
        Me.Button5.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Button5.Location = New System.Drawing.Point(38, 350)
        Me.Button5.Name = "Button5"
        Me.Button5.Size = New System.Drawing.Size(75, 23)
        Me.Button5.TabIndex = 7
        Me.Button5.Text = "Cancel"
        Me.Button5.UseVisualStyleBackColor = True
        '
        'Button6
        '
        Me.Button6.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Button6.Location = New System.Drawing.Point(142, 350)
        Me.Button6.Name = "Button6"
        Me.Button6.Size = New System.Drawing.Size(75, 23)
        Me.Button6.TabIndex = 12
        Me.Button6.Text = "OK"
        Me.Button6.UseVisualStyleBackColor = True
        '
        'BtnReport
        '
        Me.BtnReport.Location = New System.Drawing.Point(247, 350)
        Me.BtnReport.Name = "BtnReport"
        Me.BtnReport.Size = New System.Drawing.Size(75, 23)
        Me.BtnReport.TabIndex = 13
        Me.BtnReport.Text = "Report"
        Me.BtnReport.UseVisualStyleBackColor = True
        '
        'Segmentation
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(359, 385)
        Me.Controls.Add(Me.BtnReport)
        Me.Controls.Add(Me.Button6)
        Me.Controls.Add(Me.Button5)
        Me.Controls.Add(Me.BarMax)
        Me.Controls.Add(Me.BarMin)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.TxtMax)
        Me.Controls.Add(Me.TxtMin)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.Pic2)
        Me.Controls.Add(Me.Pic1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Name = "Segmentation"
        Me.Text = "SEGMENTATION"
        CType(Me.Pic1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Pic2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        CType(Me.BarMin, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BarMax, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Pic1 As PictureBox
    Friend WithEvents Pic2 As PictureBox
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents TxtArea As TextBox
    Friend WithEvents BtnColor As Button
    Friend WithEvents Label3 As Label
    Friend WithEvents LabColor As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents BtnBlue As Button
    Friend WithEvents BtnGreen As Button
    Friend WithEvents BtnRed As Button
    Friend WithEvents TxtMin As TextBox
    Friend WithEvents TxtMax As TextBox
    Friend WithEvents Label4 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents BarMin As TrackBar
    Friend WithEvents BarMax As TrackBar
    Friend WithEvents Button5 As Button
    Friend WithEvents Button6 As Button
    Friend WithEvents BtnReport As Button
End Class
