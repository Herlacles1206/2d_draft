<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Density
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
        Me.components = New System.ComponentModel.Container()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.BtnExcel = New System.Windows.Forms.Button()
        Me.BtnReport = New System.Windows.Forms.Button()
        Me.BtnExit = New System.Windows.Forms.Button()
        Me.RadRect = New System.Windows.Forms.RadioButton()
        Me.RadHand = New System.Windows.Forms.RadioButton()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.TxtWhite = New System.Windows.Forms.TextBox()
        Me.TxtBlack = New System.Windows.Forms.TextBox()
        Me.LabHelp = New System.Windows.Forms.Label()
        Me.LabHelp2 = New System.Windows.Forms.Label()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.LabHelp2)
        Me.GroupBox1.Controls.Add(Me.LabHelp)
        Me.GroupBox1.Controls.Add(Me.RadHand)
        Me.GroupBox1.Controls.Add(Me.RadRect)
        Me.GroupBox1.Location = New System.Drawing.Point(27, 12)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(267, 83)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Help"
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.TxtBlack)
        Me.GroupBox2.Controls.Add(Me.TxtWhite)
        Me.GroupBox2.Controls.Add(Me.Label2)
        Me.GroupBox2.Controls.Add(Me.Label1)
        Me.GroupBox2.Location = New System.Drawing.Point(27, 101)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(267, 75)
        Me.GroupBox2.TabIndex = 1
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Density"
        '
        'BtnExcel
        '
        Me.BtnExcel.Location = New System.Drawing.Point(219, 194)
        Me.BtnExcel.Name = "BtnExcel"
        Me.BtnExcel.Size = New System.Drawing.Size(75, 23)
        Me.BtnExcel.TabIndex = 2
        Me.BtnExcel.Text = "To Excel"
        Me.BtnExcel.UseVisualStyleBackColor = True
        '
        'BtnReport
        '
        Me.BtnReport.Location = New System.Drawing.Point(124, 194)
        Me.BtnReport.Name = "BtnReport"
        Me.BtnReport.Size = New System.Drawing.Size(75, 23)
        Me.BtnReport.TabIndex = 3
        Me.BtnReport.Text = "Report"
        Me.BtnReport.UseVisualStyleBackColor = True
        '
        'BtnExit
        '
        Me.BtnExit.Location = New System.Drawing.Point(27, 194)
        Me.BtnExit.Name = "BtnExit"
        Me.BtnExit.Size = New System.Drawing.Size(75, 23)
        Me.BtnExit.TabIndex = 4
        Me.BtnExit.Text = "Exit"
        Me.BtnExit.UseVisualStyleBackColor = True
        '
        'RadRect
        '
        Me.RadRect.AutoSize = True
        Me.RadRect.Location = New System.Drawing.Point(39, 56)
        Me.RadRect.Name = "RadRect"
        Me.RadRect.Size = New System.Drawing.Size(74, 17)
        Me.RadRect.TabIndex = 0
        Me.RadRect.TabStop = True
        Me.RadRect.Text = "Rectangle"
        Me.RadRect.UseVisualStyleBackColor = True
        '
        'RadHand
        '
        Me.RadHand.AutoSize = True
        Me.RadHand.Location = New System.Drawing.Point(171, 56)
        Me.RadHand.Name = "RadHand"
        Me.RadHand.Size = New System.Drawing.Size(70, 17)
        Me.RadHand.TabIndex = 1
        Me.RadHand.TabStop = True
        Me.RadHand.Text = "Freehand"
        Me.RadHand.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(21, 19)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(105, 13)
        Me.Label1.TabIndex = 5
        Me.Label1.Text = "Percentage of White"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(22, 49)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(104, 13)
        Me.Label2.TabIndex = 6
        Me.Label2.Text = "Percentage of Black"
        '
        'TxtWhite
        '
        Me.TxtWhite.Location = New System.Drawing.Point(146, 16)
        Me.TxtWhite.Name = "TxtWhite"
        Me.TxtWhite.ReadOnly = True
        Me.TxtWhite.Size = New System.Drawing.Size(100, 20)
        Me.TxtWhite.TabIndex = 5
        '
        'TxtBlack
        '
        Me.TxtBlack.Location = New System.Drawing.Point(146, 46)
        Me.TxtBlack.Name = "TxtBlack"
        Me.TxtBlack.ReadOnly = True
        Me.TxtBlack.Size = New System.Drawing.Size(100, 20)
        Me.TxtBlack.TabIndex = 7
        '
        'LabHelp
        '
        Me.LabHelp.AutoSize = True
        Me.LabHelp.Location = New System.Drawing.Point(21, 16)
        Me.LabHelp.Name = "LabHelp"
        Me.LabHelp.Size = New System.Drawing.Size(200, 13)
        Me.LabHelp.TabIndex = 5
        Me.LabHelp.Text = "Please draw rectangle over the image for"
        '
        'LabHelp2
        '
        Me.LabHelp2.AutoSize = True
        Me.LabHelp2.Location = New System.Drawing.Point(21, 36)
        Me.LabHelp2.Name = "LabHelp2"
        Me.LabHelp2.Size = New System.Drawing.Size(91, 13)
        Me.LabHelp2.TabIndex = 5
        Me.LabHelp2.Text = "measuring density"
        '
        'Timer1
        '
        '
        'Density
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(320, 236)
        Me.Controls.Add(Me.BtnExit)
        Me.Controls.Add(Me.BtnReport)
        Me.Controls.Add(Me.BtnExcel)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GroupBox1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Name = "Density"
        Me.Text = "Density"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents LabHelp2 As Label
    Friend WithEvents LabHelp As Label
    Friend WithEvents RadHand As RadioButton
    Friend WithEvents RadRect As RadioButton
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents TxtBlack As TextBox
    Friend WithEvents TxtWhite As TextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents BtnExcel As Button
    Friend WithEvents BtnReport As Button
    Friend WithEvents BtnExit As Button
    Friend WithEvents Timer1 As Timer
End Class
