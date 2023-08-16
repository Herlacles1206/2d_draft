<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class SetROI
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
        Me.NumSizeROI = New System.Windows.Forms.NumericUpDown()
        Me.ComboUnit = New System.Windows.Forms.ComboBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.CheckROI = New System.Windows.Forms.CheckBox()
        Me.BtnDefault = New System.Windows.Forms.Button()
        CType(Me.NumSizeROI, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'NumSizeROI
        '
        Me.NumSizeROI.Location = New System.Drawing.Point(148, 32)
        Me.NumSizeROI.Maximum = New Decimal(New Integer() {1000000, 0, 0, 0})
        Me.NumSizeROI.Name = "NumSizeROI"
        Me.NumSizeROI.Size = New System.Drawing.Size(79, 20)
        Me.NumSizeROI.TabIndex = 0
        '
        'ComboUnit
        '
        Me.ComboUnit.FormattingEnabled = True
        Me.ComboUnit.Items.AddRange(New Object() {"m×m", "cm×cm", "mm×mm", "μm×μm"})
        Me.ComboUnit.Location = New System.Drawing.Point(148, 68)
        Me.ComboUnit.Name = "ComboUnit"
        Me.ComboUnit.Size = New System.Drawing.Size(79, 21)
        Me.ComboUnit.TabIndex = 1
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(50, 34)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(64, 13)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Size of ROI:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(50, 71)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(29, 13)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "Unit:"
        '
        'Button1
        '
        Me.Button1.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Button1.Location = New System.Drawing.Point(90, 160)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(75, 23)
        Me.Button1.TabIndex = 4
        Me.Button1.Text = "OK"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Button2
        '
        Me.Button2.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Button2.Location = New System.Drawing.Point(178, 160)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(75, 23)
        Me.Button2.TabIndex = 5
        Me.Button2.Text = "Cancel"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'CheckROI
        '
        Me.CheckROI.AutoSize = True
        Me.CheckROI.Location = New System.Drawing.Point(53, 109)
        Me.CheckROI.Name = "CheckROI"
        Me.CheckROI.Size = New System.Drawing.Size(86, 17)
        Me.CheckROI.TabIndex = 6
        Me.CheckROI.Text = "Preview ROI"
        Me.CheckROI.UseVisualStyleBackColor = True
        '
        'BtnDefault
        '
        Me.BtnDefault.Location = New System.Drawing.Point(152, 105)
        Me.BtnDefault.Name = "BtnDefault"
        Me.BtnDefault.Size = New System.Drawing.Size(75, 23)
        Me.BtnDefault.TabIndex = 7
        Me.BtnDefault.Text = "Set Default"
        Me.BtnDefault.UseVisualStyleBackColor = True
        '
        'SetROI
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(265, 195)
        Me.Controls.Add(Me.BtnDefault)
        Me.Controls.Add(Me.CheckROI)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.ComboUnit)
        Me.Controls.Add(Me.NumSizeROI)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Name = "SetROI"
        Me.Text = "SetROI"
        CType(Me.NumSizeROI, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents NumSizeROI As NumericUpDown
    Friend WithEvents ComboUnit As ComboBox
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Button1 As Button
    Friend WithEvents Button2 As Button
    Friend WithEvents CheckROI As CheckBox
    Friend WithEvents BtnDefault As Button
End Class
