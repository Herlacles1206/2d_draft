<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AddCalibration
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
        Me.TxtCaliName = New System.Windows.Forms.TextBox()
        Me.BtnNew = New System.Windows.Forms.Button()
        Me.BtnDel = New System.Windows.Forms.Button()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.TxtPhaseName = New System.Windows.Forms.TextBox()
        Me.BtnCol = New System.Windows.Forms.Button()
        Me.PictureBox2 = New System.Windows.Forms.PictureBox()
        Me.HistogramBox1 = New Emgu.CV.UI.HistogramBox()
        Me.PicBoxProgress = New System.Windows.Forms.PictureBox()
        Me.NumMax = New System.Windows.Forms.NumericUpDown()
        Me.NumMin = New System.Windows.Forms.NumericUpDown()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.Phase = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.from = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.PhaseTo = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.BtnSave = New System.Windows.Forms.Button()
        Me.Button5 = New System.Windows.Forms.Button()
        CType(Me.PictureBox2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.HistogramBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PicBoxProgress, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.NumMax, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.NumMin, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(33, 18)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(87, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Calibration Name"
        '
        'TxtCaliName
        '
        Me.TxtCaliName.Location = New System.Drawing.Point(149, 15)
        Me.TxtCaliName.Name = "TxtCaliName"
        Me.TxtCaliName.Size = New System.Drawing.Size(163, 20)
        Me.TxtCaliName.TabIndex = 1
        '
        'BtnNew
        '
        Me.BtnNew.Location = New System.Drawing.Point(59, 50)
        Me.BtnNew.Name = "BtnNew"
        Me.BtnNew.Size = New System.Drawing.Size(75, 23)
        Me.BtnNew.TabIndex = 2
        Me.BtnNew.Text = "New Phase"
        Me.BtnNew.UseVisualStyleBackColor = True
        '
        'BtnDel
        '
        Me.BtnDel.Location = New System.Drawing.Point(191, 50)
        Me.BtnDel.Name = "BtnDel"
        Me.BtnDel.Size = New System.Drawing.Size(75, 23)
        Me.BtnDel.TabIndex = 3
        Me.BtnDel.Text = "Delete All"
        Me.BtnDel.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(33, 91)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(68, 13)
        Me.Label2.TabIndex = 4
        Me.Label2.Text = "Phase Name"
        '
        'TxtPhaseName
        '
        Me.TxtPhaseName.Location = New System.Drawing.Point(149, 88)
        Me.TxtPhaseName.Name = "TxtPhaseName"
        Me.TxtPhaseName.Size = New System.Drawing.Size(132, 20)
        Me.TxtPhaseName.TabIndex = 5
        '
        'BtnCol
        '
        Me.BtnCol.BackColor = System.Drawing.Color.White
        Me.BtnCol.Location = New System.Drawing.Point(287, 85)
        Me.BtnCol.Name = "BtnCol"
        Me.BtnCol.Size = New System.Drawing.Size(25, 25)
        Me.BtnCol.TabIndex = 6
        Me.BtnCol.UseVisualStyleBackColor = False
        '
        'PictureBox2
        '
        Me.PictureBox2.Image = Global._2D_Drafting.My.Resources.Resources.GrayGradient
        Me.PictureBox2.Location = New System.Drawing.Point(29, 256)
        Me.PictureBox2.Name = "PictureBox2"
        Me.PictureBox2.Size = New System.Drawing.Size(283, 20)
        Me.PictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox2.TabIndex = 9
        Me.PictureBox2.TabStop = False
        '
        'HistogramBox1
        '
        Me.HistogramBox1.Enabled = False
        Me.HistogramBox1.Location = New System.Drawing.Point(29, 146)
        Me.HistogramBox1.Name = "HistogramBox1"
        Me.HistogramBox1.Size = New System.Drawing.Size(283, 103)
        Me.HistogramBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.HistogramBox1.TabIndex = 7
        Me.HistogramBox1.TabStop = False
        '
        'PicBoxProgress
        '
        Me.PicBoxProgress.BackColor = System.Drawing.Color.White
        Me.PicBoxProgress.Location = New System.Drawing.Point(29, 119)
        Me.PicBoxProgress.Name = "PicBoxProgress"
        Me.PicBoxProgress.Size = New System.Drawing.Size(283, 20)
        Me.PicBoxProgress.TabIndex = 8
        Me.PicBoxProgress.TabStop = False
        '
        'NumMax
        '
        Me.NumMax.Location = New System.Drawing.Point(256, 286)
        Me.NumMax.Maximum = New Decimal(New Integer() {256, 0, 0, 0})
        Me.NumMax.Name = "NumMax"
        Me.NumMax.Size = New System.Drawing.Size(56, 20)
        Me.NumMax.TabIndex = 14
        '
        'NumMin
        '
        Me.NumMin.Location = New System.Drawing.Point(91, 286)
        Me.NumMin.Maximum = New Decimal(New Integer() {256, 0, 0, 0})
        Me.NumMin.Name = "NumMin"
        Me.NumMin.Size = New System.Drawing.Size(56, 20)
        Me.NumMin.TabIndex = 13
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(184, 290)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(62, 13)
        Me.Label3.TabIndex = 12
        Me.Label3.Text = "Max.Range"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(26, 290)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(59, 13)
        Me.Label4.TabIndex = 11
        Me.Label4.Text = "Min.Range"
        '
        'DataGridView1
        '
        Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView1.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.Phase, Me.from, Me.PhaseTo})
        Me.DataGridView1.Location = New System.Drawing.Point(29, 313)
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.Size = New System.Drawing.Size(283, 93)
        Me.DataGridView1.TabIndex = 15
        '
        'Phase
        '
        Me.Phase.HeaderText = "Phases"
        Me.Phase.Name = "Phase"
        '
        'from
        '
        Me.from.HeaderText = "From"
        Me.from.Name = "from"
        '
        'PhaseTo
        '
        Me.PhaseTo.HeaderText = "To"
        Me.PhaseTo.Name = "PhaseTo"
        '
        'BtnSave
        '
        Me.BtnSave.Location = New System.Drawing.Point(59, 420)
        Me.BtnSave.Name = "BtnSave"
        Me.BtnSave.Size = New System.Drawing.Size(75, 23)
        Me.BtnSave.TabIndex = 16
        Me.BtnSave.Text = "Save"
        Me.BtnSave.UseVisualStyleBackColor = True
        '
        'Button5
        '
        Me.Button5.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Button5.Location = New System.Drawing.Point(191, 420)
        Me.Button5.Name = "Button5"
        Me.Button5.Size = New System.Drawing.Size(75, 23)
        Me.Button5.TabIndex = 17
        Me.Button5.Text = "Cancel"
        Me.Button5.UseVisualStyleBackColor = True
        '
        'AddCalibration
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(340, 455)
        Me.Controls.Add(Me.Button5)
        Me.Controls.Add(Me.BtnSave)
        Me.Controls.Add(Me.DataGridView1)
        Me.Controls.Add(Me.NumMax)
        Me.Controls.Add(Me.NumMin)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.PictureBox2)
        Me.Controls.Add(Me.HistogramBox1)
        Me.Controls.Add(Me.PicBoxProgress)
        Me.Controls.Add(Me.BtnCol)
        Me.Controls.Add(Me.TxtPhaseName)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.BtnDel)
        Me.Controls.Add(Me.BtnNew)
        Me.Controls.Add(Me.TxtCaliName)
        Me.Controls.Add(Me.Label1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Name = "AddCalibration"
        Me.Text = "AddCalibration"
        CType(Me.PictureBox2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.HistogramBox1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PicBoxProgress, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.NumMax, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.NumMin, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Label1 As Label
    Friend WithEvents TxtCaliName As TextBox
    Friend WithEvents BtnNew As Button
    Friend WithEvents BtnDel As Button
    Friend WithEvents Label2 As Label
    Friend WithEvents TxtPhaseName As TextBox
    Friend WithEvents BtnCol As Button
    Friend WithEvents PictureBox2 As PictureBox
    Friend WithEvents HistogramBox1 As Emgu.CV.UI.HistogramBox
    Friend WithEvents PicBoxProgress As PictureBox
    Friend WithEvents NumMax As NumericUpDown
    Friend WithEvents NumMin As NumericUpDown
    Friend WithEvents Label3 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents BtnSave As Button
    Friend WithEvents Button5 As Button
    Friend WithEvents Phase As DataGridViewTextBoxColumn
    Friend WithEvents from As DataGridViewTextBoxColumn
    Friend WithEvents PhaseTo As DataGridViewTextBoxColumn
End Class
