<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class PhaseSegmentASTM
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
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.ComboCali = New System.Windows.Forms.ComboBox()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.phase = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.area = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.area_pro = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.BtnAdd = New System.Windows.Forms.Button()
        Me.BtnReport = New System.Windows.Forms.Button()
        Me.BtnExcel = New System.Windows.Forms.Button()
        Me.BtnExit = New System.Windows.Forms.Button()
        Me.PictureBox2 = New System.Windows.Forms.PictureBox()
        Me.HistogramBox1 = New Emgu.CV.UI.HistogramBox()
        Me.PicBoxProgress = New System.Windows.Forms.PictureBox()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.GroupBox1.SuspendLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PictureBox2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.HistogramBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PicBoxProgress, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(41, 19)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(201, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Select the Calibration given in ComboBox"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(41, 50)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(56, 13)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "Calibration"
        '
        'ComboCali
        '
        Me.ComboCali.FormattingEnabled = True
        Me.ComboCali.Location = New System.Drawing.Point(128, 47)
        Me.ComboCali.Name = "ComboCali"
        Me.ComboCali.Size = New System.Drawing.Size(121, 21)
        Me.ComboCali.TabIndex = 2
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.DataGridView1)
        Me.GroupBox1.Location = New System.Drawing.Point(12, 237)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(310, 140)
        Me.GroupBox1.TabIndex = 5
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "RESULT"
        '
        'DataGridView1
        '
        Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView1.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.phase, Me.area, Me.area_pro})
        Me.DataGridView1.Location = New System.Drawing.Point(14, 19)
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.Size = New System.Drawing.Size(290, 115)
        Me.DataGridView1.TabIndex = 0
        '
        'phase
        '
        Me.phase.HeaderText = "Phases"
        Me.phase.Name = "phase"
        '
        'area
        '
        Me.area.HeaderText = "Area"
        Me.area.Name = "area"
        '
        'area_pro
        '
        Me.area_pro.HeaderText = "Area%"
        Me.area_pro.Name = "area_pro"
        '
        'BtnAdd
        '
        Me.BtnAdd.Location = New System.Drawing.Point(12, 383)
        Me.BtnAdd.Name = "BtnAdd"
        Me.BtnAdd.Size = New System.Drawing.Size(91, 23)
        Me.BtnAdd.TabIndex = 6
        Me.BtnAdd.Text = "Add Calibration"
        Me.BtnAdd.UseVisualStyleBackColor = True
        '
        'BtnReport
        '
        Me.BtnReport.Location = New System.Drawing.Point(117, 383)
        Me.BtnReport.Name = "BtnReport"
        Me.BtnReport.Size = New System.Drawing.Size(59, 23)
        Me.BtnReport.TabIndex = 7
        Me.BtnReport.Text = "Report"
        Me.BtnReport.UseVisualStyleBackColor = True
        '
        'BtnExcel
        '
        Me.BtnExcel.Location = New System.Drawing.Point(190, 383)
        Me.BtnExcel.Name = "BtnExcel"
        Me.BtnExcel.Size = New System.Drawing.Size(59, 23)
        Me.BtnExcel.TabIndex = 8
        Me.BtnExcel.Text = "To Excel"
        Me.BtnExcel.UseVisualStyleBackColor = True
        '
        'BtnExit
        '
        Me.BtnExit.Location = New System.Drawing.Point(263, 383)
        Me.BtnExit.Name = "BtnExit"
        Me.BtnExit.Size = New System.Drawing.Size(59, 23)
        Me.BtnExit.TabIndex = 9
        Me.BtnExit.Text = "Exit"
        Me.BtnExit.UseVisualStyleBackColor = True
        '
        'PictureBox2
        '
        Me.PictureBox2.Image = Global._2D_Drafting.My.Resources.Resources.GrayGradient
        Me.PictureBox2.Location = New System.Drawing.Point(26, 211)
        Me.PictureBox2.Name = "PictureBox2"
        Me.PictureBox2.Size = New System.Drawing.Size(283, 20)
        Me.PictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox2.TabIndex = 4
        Me.PictureBox2.TabStop = False
        '
        'HistogramBox1
        '
        Me.HistogramBox1.Enabled = False
        Me.HistogramBox1.Location = New System.Drawing.Point(26, 101)
        Me.HistogramBox1.Name = "HistogramBox1"
        Me.HistogramBox1.Size = New System.Drawing.Size(283, 103)
        Me.HistogramBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.HistogramBox1.TabIndex = 2
        Me.HistogramBox1.TabStop = False
        '
        'PicBoxProgress
        '
        Me.PicBoxProgress.BackColor = System.Drawing.Color.White
        Me.PicBoxProgress.Location = New System.Drawing.Point(26, 74)
        Me.PicBoxProgress.Name = "PicBoxProgress"
        Me.PicBoxProgress.Size = New System.Drawing.Size(283, 20)
        Me.PicBoxProgress.TabIndex = 3
        Me.PicBoxProgress.TabStop = False
        '
        'Timer1
        '
        '
        'PhaseSegmentASTM
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(334, 418)
        Me.Controls.Add(Me.BtnExit)
        Me.Controls.Add(Me.BtnExcel)
        Me.Controls.Add(Me.BtnReport)
        Me.Controls.Add(Me.BtnAdd)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.PictureBox2)
        Me.Controls.Add(Me.HistogramBox1)
        Me.Controls.Add(Me.PicBoxProgress)
        Me.Controls.Add(Me.ComboCali)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Name = "PhaseSegmentASTM"
        Me.Text = "Phase/Segmentation ASTM"
        Me.GroupBox1.ResumeLayout(False)
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PictureBox2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.HistogramBox1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PicBoxProgress, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents PicBoxProgress As PictureBox
    Friend WithEvents HistogramBox1 As Emgu.CV.UI.HistogramBox
    Friend WithEvents PictureBox2 As PictureBox
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents BtnAdd As Button
    Friend WithEvents BtnReport As Button
    Friend WithEvents BtnExcel As Button
    Friend WithEvents BtnExit As Button
    Friend WithEvents phase As DataGridViewTextBoxColumn
    Friend WithEvents area As DataGridViewTextBoxColumn
    Friend WithEvents area_pro As DataGridViewTextBoxColumn
    Public WithEvents ComboCali As ComboBox
    Friend WithEvents Timer1 As Timer
End Class
