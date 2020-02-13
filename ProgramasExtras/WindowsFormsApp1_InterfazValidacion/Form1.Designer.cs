namespace WindowsFormsApp1
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.digitalInputGroupBox = new System.Windows.Forms.GroupBox();
            this.counterComboBox = new System.Windows.Forms.ComboBox();
            this.digitalInputComboBox = new System.Windows.Forms.ComboBox();
            this.zIndexPhaseLabel = new System.Windows.Forms.Label();
            this.digitalChannelLabel = new System.Windows.Forms.Label();
            this.zIndexPhaseComboBox = new System.Windows.Forms.ComboBox();
            this.decodingTypeLabel = new System.Windows.Forms.Label();
            this.zIndexEnabledCheckBox = new System.Windows.Forms.CheckBox();
            this.pulsesPerRevLabel = new System.Windows.Forms.Label();
            this.decodingTypeComboBox = new System.Windows.Forms.ComboBox();
            this.zIndexValueLabel = new System.Windows.Forms.Label();
            this.zIndexValueTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.pulsePerRevTextBox = new System.Windows.Forms.TextBox();
            this.channelParametersGroupBox = new System.Windows.Forms.GroupBox();
            this.physicalChannelComboBox = new System.Windows.Forms.ComboBox();
            this.physicalChannelLabel = new System.Windows.Forms.Label();
            this.maximumTextBox = new System.Windows.Forms.TextBox();
            this.minimumTextBox = new System.Windows.Forms.TextBox();
            this.maximumLabel = new System.Windows.Forms.Label();
            this.minimumLabel = new System.Windows.Forms.Label();
            this.gBNetGraphicOut = new System.Windows.Forms.GroupBox();
            this.pBNetGraphicOut = new System.Windows.Forms.PictureBox();
            this.tmrRefresh = new System.Windows.Forms.Timer(this.components);
            this.trckBrPosition = new System.Windows.Forms.TrackBar();
            this.btnStart = new System.Windows.Forms.Button();
            this.lblPosition = new System.Windows.Forms.Label();
            this.lblOutPosition = new System.Windows.Forms.Label();
            this.btnStopDemo = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.archivoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openGenomeToTestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.savePositionDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openControlSignalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.lblMotorPos = new System.Windows.Forms.Label();
            this.txtBxDesPos = new System.Windows.Forms.TextBox();
            this.btnSine = new System.Windows.Forms.Button();
            this.btnManual = new System.Windows.Forms.Button();
            this.lblSineFrec = new System.Windows.Forms.Label();
            this.numUDSineFrec = new System.Windows.Forms.NumericUpDown();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.chckBoxWarmUp = new System.Windows.Forms.CheckBox();
            this.chkMotor = new System.Windows.Forms.CheckBox();
            this.btnStandar = new System.Windows.Forms.Button();
            this.chkOnlyNN = new System.Windows.Forms.CheckBox();
            this.openFileDialog2 = new System.Windows.Forms.OpenFileDialog();
            this.digitalInputGroupBox.SuspendLayout();
            this.channelParametersGroupBox.SuspendLayout();
            this.gBNetGraphicOut.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pBNetGraphicOut)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trckBrPosition)).BeginInit();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numUDSineFrec)).BeginInit();
            this.SuspendLayout();
            // 
            // digitalInputGroupBox
            // 
            this.digitalInputGroupBox.Controls.Add(this.counterComboBox);
            this.digitalInputGroupBox.Controls.Add(this.digitalInputComboBox);
            this.digitalInputGroupBox.Controls.Add(this.zIndexPhaseLabel);
            this.digitalInputGroupBox.Controls.Add(this.digitalChannelLabel);
            this.digitalInputGroupBox.Controls.Add(this.zIndexPhaseComboBox);
            this.digitalInputGroupBox.Controls.Add(this.decodingTypeLabel);
            this.digitalInputGroupBox.Controls.Add(this.zIndexEnabledCheckBox);
            this.digitalInputGroupBox.Controls.Add(this.pulsesPerRevLabel);
            this.digitalInputGroupBox.Controls.Add(this.decodingTypeComboBox);
            this.digitalInputGroupBox.Controls.Add(this.zIndexValueLabel);
            this.digitalInputGroupBox.Controls.Add(this.zIndexValueTextBox);
            this.digitalInputGroupBox.Controls.Add(this.label5);
            this.digitalInputGroupBox.Controls.Add(this.pulsePerRevTextBox);
            this.digitalInputGroupBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.digitalInputGroupBox.Location = new System.Drawing.Point(12, 52);
            this.digitalInputGroupBox.Name = "digitalInputGroupBox";
            this.digitalInputGroupBox.Size = new System.Drawing.Size(408, 344);
            this.digitalInputGroupBox.TabIndex = 4;
            this.digitalInputGroupBox.TabStop = false;
            this.digitalInputGroupBox.Text = "Channel Parameters - Digital Input";
            // 
            // counterComboBox
            // 
            this.counterComboBox.Location = new System.Drawing.Point(243, 79);
            this.counterComboBox.Name = "counterComboBox";
            this.counterComboBox.Size = new System.Drawing.Size(154, 28);
            this.counterComboBox.TabIndex = 1;
            this.counterComboBox.Text = "Dev1/ctr0";
            // 
            // digitalInputComboBox
            // 
            this.digitalInputComboBox.Location = new System.Drawing.Point(243, 35);
            this.digitalInputComboBox.Name = "digitalInputComboBox";
            this.digitalInputComboBox.Size = new System.Drawing.Size(154, 28);
            this.digitalInputComboBox.TabIndex = 1;
            this.digitalInputComboBox.Text = "Dev1/port0";
            // 
            // zIndexPhaseLabel
            // 
            this.zIndexPhaseLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.zIndexPhaseLabel.Location = new System.Drawing.Point(29, 246);
            this.zIndexPhaseLabel.Name = "zIndexPhaseLabel";
            this.zIndexPhaseLabel.Size = new System.Drawing.Size(147, 23);
            this.zIndexPhaseLabel.TabIndex = 7;
            this.zIndexPhaseLabel.Text = "Z Index Phase:";
            // 
            // digitalChannelLabel
            // 
            this.digitalChannelLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.digitalChannelLabel.Location = new System.Drawing.Point(26, 38);
            this.digitalChannelLabel.Name = "digitalChannelLabel";
            this.digitalChannelLabel.Size = new System.Drawing.Size(192, 23);
            this.digitalChannelLabel.TabIndex = 0;
            this.digitalChannelLabel.Text = "Digital Input Channels:";
            // 
            // zIndexPhaseComboBox
            // 
            this.zIndexPhaseComboBox.DisplayMember = "1";
            this.zIndexPhaseComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.zIndexPhaseComboBox.Items.AddRange(new object[] {
            "A High B High",
            "A High B Low",
            "A Low B High",
            "A Low B Low"});
            this.zIndexPhaseComboBox.Location = new System.Drawing.Point(243, 246);
            this.zIndexPhaseComboBox.Name = "zIndexPhaseComboBox";
            this.zIndexPhaseComboBox.Size = new System.Drawing.Size(154, 28);
            this.zIndexPhaseComboBox.TabIndex = 8;
            // 
            // decodingTypeLabel
            // 
            this.decodingTypeLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.decodingTypeLabel.Location = new System.Drawing.Point(29, 152);
            this.decodingTypeLabel.Name = "decodingTypeLabel";
            this.decodingTypeLabel.Size = new System.Drawing.Size(179, 24);
            this.decodingTypeLabel.TabIndex = 3;
            this.decodingTypeLabel.Text = "Decoding Type:";
            // 
            // zIndexEnabledCheckBox
            // 
            this.zIndexEnabledCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.zIndexEnabledCheckBox.Location = new System.Drawing.Point(26, 105);
            this.zIndexEnabledCheckBox.Name = "zIndexEnabledCheckBox";
            this.zIndexEnabledCheckBox.Size = new System.Drawing.Size(192, 35);
            this.zIndexEnabledCheckBox.TabIndex = 2;
            this.zIndexEnabledCheckBox.Text = "Z Index Enabled";
            // 
            // pulsesPerRevLabel
            // 
            this.pulsesPerRevLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.pulsesPerRevLabel.Location = new System.Drawing.Point(29, 292);
            this.pulsesPerRevLabel.Name = "pulsesPerRevLabel";
            this.pulsesPerRevLabel.Size = new System.Drawing.Size(179, 23);
            this.pulsesPerRevLabel.TabIndex = 9;
            this.pulsesPerRevLabel.Text = "Pulses per Revolution:";
            // 
            // decodingTypeComboBox
            // 
            this.decodingTypeComboBox.DisplayMember = "1";
            this.decodingTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.decodingTypeComboBox.Items.AddRange(new object[] {
            "X1",
            "X2",
            "X4"});
            this.decodingTypeComboBox.Location = new System.Drawing.Point(243, 148);
            this.decodingTypeComboBox.Name = "decodingTypeComboBox";
            this.decodingTypeComboBox.Size = new System.Drawing.Size(154, 28);
            this.decodingTypeComboBox.TabIndex = 4;
            // 
            // zIndexValueLabel
            // 
            this.zIndexValueLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.zIndexValueLabel.Location = new System.Drawing.Point(29, 199);
            this.zIndexValueLabel.Name = "zIndexValueLabel";
            this.zIndexValueLabel.Size = new System.Drawing.Size(179, 26);
            this.zIndexValueLabel.TabIndex = 5;
            this.zIndexValueLabel.Text = "Z Index Value:";
            // 
            // zIndexValueTextBox
            // 
            this.zIndexValueTextBox.Location = new System.Drawing.Point(243, 196);
            this.zIndexValueTextBox.Name = "zIndexValueTextBox";
            this.zIndexValueTextBox.Size = new System.Drawing.Size(154, 26);
            this.zIndexValueTextBox.TabIndex = 6;
            this.zIndexValueTextBox.Text = "0";
            // 
            // label5
            // 
            this.label5.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label5.Location = new System.Drawing.Point(29, 79);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(154, 23);
            this.label5.TabIndex = 0;
            this.label5.Text = "Counter(s):";
            // 
            // pulsePerRevTextBox
            // 
            this.pulsePerRevTextBox.Location = new System.Drawing.Point(243, 289);
            this.pulsePerRevTextBox.Name = "pulsePerRevTextBox";
            this.pulsePerRevTextBox.Size = new System.Drawing.Size(154, 26);
            this.pulsePerRevTextBox.TabIndex = 10;
            this.pulsePerRevTextBox.Text = "1024";
            // 
            // channelParametersGroupBox
            // 
            this.channelParametersGroupBox.Controls.Add(this.physicalChannelComboBox);
            this.channelParametersGroupBox.Controls.Add(this.physicalChannelLabel);
            this.channelParametersGroupBox.Controls.Add(this.maximumTextBox);
            this.channelParametersGroupBox.Controls.Add(this.minimumTextBox);
            this.channelParametersGroupBox.Controls.Add(this.maximumLabel);
            this.channelParametersGroupBox.Controls.Add(this.minimumLabel);
            this.channelParametersGroupBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.channelParametersGroupBox.Location = new System.Drawing.Point(12, 417);
            this.channelParametersGroupBox.Name = "channelParametersGroupBox";
            this.channelParametersGroupBox.Size = new System.Drawing.Size(408, 164);
            this.channelParametersGroupBox.TabIndex = 5;
            this.channelParametersGroupBox.TabStop = false;
            this.channelParametersGroupBox.Text = "Channel Parameters";
            // 
            // physicalChannelComboBox
            // 
            this.physicalChannelComboBox.Location = new System.Drawing.Point(243, 33);
            this.physicalChannelComboBox.Name = "physicalChannelComboBox";
            this.physicalChannelComboBox.Size = new System.Drawing.Size(154, 28);
            this.physicalChannelComboBox.TabIndex = 1;
            this.physicalChannelComboBox.Text = "Dev1/ao0";
            // 
            // physicalChannelLabel
            // 
            this.physicalChannelLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.physicalChannelLabel.Location = new System.Drawing.Point(26, 38);
            this.physicalChannelLabel.Name = "physicalChannelLabel";
            this.physicalChannelLabel.Size = new System.Drawing.Size(153, 23);
            this.physicalChannelLabel.TabIndex = 0;
            this.physicalChannelLabel.Text = "Physical Channel:";
            // 
            // maximumTextBox
            // 
            this.maximumTextBox.Location = new System.Drawing.Point(243, 122);
            this.maximumTextBox.Name = "maximumTextBox";
            this.maximumTextBox.Size = new System.Drawing.Size(154, 26);
            this.maximumTextBox.TabIndex = 5;
            this.maximumTextBox.Text = "10";
            // 
            // minimumTextBox
            // 
            this.minimumTextBox.Location = new System.Drawing.Point(243, 78);
            this.minimumTextBox.Name = "minimumTextBox";
            this.minimumTextBox.Size = new System.Drawing.Size(154, 26);
            this.minimumTextBox.TabIndex = 3;
            this.minimumTextBox.Text = "-10";
            // 
            // maximumLabel
            // 
            this.maximumLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.maximumLabel.Location = new System.Drawing.Point(26, 125);
            this.maximumLabel.Name = "maximumLabel";
            this.maximumLabel.Size = new System.Drawing.Size(179, 24);
            this.maximumLabel.TabIndex = 4;
            this.maximumLabel.Text = "Maximum Value (V):";
            // 
            // minimumLabel
            // 
            this.minimumLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.minimumLabel.Location = new System.Drawing.Point(26, 81);
            this.minimumLabel.Name = "minimumLabel";
            this.minimumLabel.Size = new System.Drawing.Size(166, 23);
            this.minimumLabel.TabIndex = 2;
            this.minimumLabel.Text = "Minimum Value (V):";
            // 
            // gBNetGraphicOut
            // 
            this.gBNetGraphicOut.Controls.Add(this.pBNetGraphicOut);
            this.gBNetGraphicOut.Location = new System.Drawing.Point(426, 52);
            this.gBNetGraphicOut.Name = "gBNetGraphicOut";
            this.gBNetGraphicOut.Size = new System.Drawing.Size(423, 437);
            this.gBNetGraphicOut.TabIndex = 6;
            this.gBNetGraphicOut.TabStop = false;
            this.gBNetGraphicOut.Text = "Graphic Output";
            // 
            // pBNetGraphicOut
            // 
            this.pBNetGraphicOut.Location = new System.Drawing.Point(6, 25);
            this.pBNetGraphicOut.Name = "pBNetGraphicOut";
            this.pBNetGraphicOut.Size = new System.Drawing.Size(400, 400);
            this.pBNetGraphicOut.TabIndex = 0;
            this.pBNetGraphicOut.TabStop = false;
            this.pBNetGraphicOut.Paint += new System.Windows.Forms.PaintEventHandler(this.pBNetGraphicOut_Paint);
            // 
            // tmrRefresh
            // 
            this.tmrRefresh.Interval = 10;
            this.tmrRefresh.Tick += new System.EventHandler(this.tmrRefresh_Tick);
            // 
            // trckBrPosition
            // 
            this.trckBrPosition.Location = new System.Drawing.Point(426, 533);
            this.trckBrPosition.Maximum = 720;
            this.trckBrPosition.Minimum = -720;
            this.trckBrPosition.Name = "trckBrPosition";
            this.trckBrPosition.Size = new System.Drawing.Size(423, 69);
            this.trckBrPosition.TabIndex = 7;
            this.trckBrPosition.Scroll += new System.EventHandler(this.trckBrPosition_Scroll);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(12, 587);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(176, 34);
            this.btnStart.TabIndex = 8;
            this.btnStart.Text = "Start Demo";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // lblPosition
            // 
            this.lblPosition.AutoSize = true;
            this.lblPosition.Location = new System.Drawing.Point(428, 573);
            this.lblPosition.Name = "lblPosition";
            this.lblPosition.Size = new System.Drawing.Size(145, 20);
            this.lblPosition.TabIndex = 9;
            this.lblPosition.Text = "Desired position: 0°";
            // 
            // lblOutPosition
            // 
            this.lblOutPosition.AutoSize = true;
            this.lblOutPosition.Location = new System.Drawing.Point(428, 501);
            this.lblOutPosition.Name = "lblOutPosition";
            this.lblOutPosition.Size = new System.Drawing.Size(139, 20);
            this.lblOutPosition.TabIndex = 9;
            this.lblOutPosition.Text = "Output position: 0°";
            // 
            // btnStopDemo
            // 
            this.btnStopDemo.Enabled = false;
            this.btnStopDemo.Location = new System.Drawing.Point(233, 587);
            this.btnStopDemo.Name = "btnStopDemo";
            this.btnStopDemo.Size = new System.Drawing.Size(176, 34);
            this.btnStopDemo.TabIndex = 8;
            this.btnStopDemo.Text = "Stop Demo";
            this.btnStopDemo.UseVisualStyleBackColor = true;
            this.btnStopDemo.Click += new System.EventHandler(this.btnStopDemo_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.archivoToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1134, 33);
            this.menuStrip1.TabIndex = 10;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // archivoToolStripMenuItem
            // 
            this.archivoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openGenomeToTestToolStripMenuItem,
            this.savePositionDataToolStripMenuItem,
            this.openControlSignalToolStripMenuItem});
            this.archivoToolStripMenuItem.Name = "archivoToolStripMenuItem";
            this.archivoToolStripMenuItem.Size = new System.Drawing.Size(50, 29);
            this.archivoToolStripMenuItem.Text = "File";
            // 
            // openGenomeToTestToolStripMenuItem
            // 
            this.openGenomeToTestToolStripMenuItem.Name = "openGenomeToTestToolStripMenuItem";
            this.openGenomeToTestToolStripMenuItem.Size = new System.Drawing.Size(268, 30);
            this.openGenomeToTestToolStripMenuItem.Text = "Open Genome to test";
            this.openGenomeToTestToolStripMenuItem.Click += new System.EventHandler(this.openGenomeToTestToolStripMenuItem_Click);
            // 
            // savePositionDataToolStripMenuItem
            // 
            this.savePositionDataToolStripMenuItem.Name = "savePositionDataToolStripMenuItem";
            this.savePositionDataToolStripMenuItem.Size = new System.Drawing.Size(268, 30);
            this.savePositionDataToolStripMenuItem.Text = "Save Position Data";
            this.savePositionDataToolStripMenuItem.Click += new System.EventHandler(this.savePositionDataToolStripMenuItem_Click);
            // 
            // openControlSignalToolStripMenuItem
            // 
            this.openControlSignalToolStripMenuItem.Name = "openControlSignalToolStripMenuItem";
            this.openControlSignalToolStripMenuItem.Size = new System.Drawing.Size(268, 30);
            this.openControlSignalToolStripMenuItem.Text = "Open Control Signal";
            this.openControlSignalToolStripMenuItem.Click += new System.EventHandler(this.openControlSignalToolStripMenuItem_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog1_FileOk);
            // 
            // lblMotorPos
            // 
            this.lblMotorPos.AutoSize = true;
            this.lblMotorPos.Location = new System.Drawing.Point(668, 573);
            this.lblMotorPos.Name = "lblMotorPos";
            this.lblMotorPos.Size = new System.Drawing.Size(131, 20);
            this.lblMotorPos.TabIndex = 9;
            this.lblMotorPos.Text = "Motor position: 0°";
            // 
            // txtBxDesPos
            // 
            this.txtBxDesPos.Location = new System.Drawing.Point(554, 570);
            this.txtBxDesPos.Name = "txtBxDesPos";
            this.txtBxDesPos.Size = new System.Drawing.Size(64, 26);
            this.txtBxDesPos.TabIndex = 5;
            this.txtBxDesPos.Text = "0°";
            this.txtBxDesPos.TextChanged += new System.EventHandler(this.txtBxDesPos_TextChanged);
            // 
            // btnSine
            // 
            this.btnSine.Location = new System.Drawing.Point(893, 115);
            this.btnSine.Name = "btnSine";
            this.btnSine.Size = new System.Drawing.Size(176, 34);
            this.btnSine.TabIndex = 8;
            this.btnSine.Text = "Sine Control";
            this.btnSine.UseVisualStyleBackColor = true;
            this.btnSine.Click += new System.EventHandler(this.btnSine_Click);
            // 
            // btnManual
            // 
            this.btnManual.Enabled = false;
            this.btnManual.Location = new System.Drawing.Point(893, 56);
            this.btnManual.Name = "btnManual";
            this.btnManual.Size = new System.Drawing.Size(176, 34);
            this.btnManual.TabIndex = 8;
            this.btnManual.Text = "Manual";
            this.btnManual.UseVisualStyleBackColor = true;
            this.btnManual.Click += new System.EventHandler(this.btnManual_Click);
            // 
            // lblSineFrec
            // 
            this.lblSineFrec.AutoSize = true;
            this.lblSineFrec.Location = new System.Drawing.Point(871, 249);
            this.lblSineFrec.Name = "lblSineFrec";
            this.lblSineFrec.Size = new System.Drawing.Size(122, 20);
            this.lblSineFrec.TabIndex = 12;
            this.lblSineFrec.Text = "Frequency (Hz):";
            // 
            // numUDSineFrec
            // 
            this.numUDSineFrec.DecimalPlaces = 4;
            this.numUDSineFrec.Location = new System.Drawing.Point(1000, 249);
            this.numUDSineFrec.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numUDSineFrec.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            262144});
            this.numUDSineFrec.Name = "numUDSineFrec";
            this.numUDSineFrec.Size = new System.Drawing.Size(120, 26);
            this.numUDSineFrec.TabIndex = 13;
            this.numUDSineFrec.Value = new decimal(new int[] {
            1,
            0,
            0,
            262144});
            this.numUDSineFrec.ValueChanged += new System.EventHandler(this.numUDSineFrec_ValueChanged);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.saveFileDialog1_FileOk);
            // 
            // chckBoxWarmUp
            // 
            this.chckBoxWarmUp.AutoSize = true;
            this.chckBoxWarmUp.Location = new System.Drawing.Point(893, 297);
            this.chckBoxWarmUp.Name = "chckBoxWarmUp";
            this.chckBoxWarmUp.Size = new System.Drawing.Size(102, 24);
            this.chckBoxWarmUp.TabIndex = 14;
            this.chckBoxWarmUp.Text = "Warm Up";
            this.chckBoxWarmUp.UseVisualStyleBackColor = true;
            // 
            // chkMotor
            // 
            this.chkMotor.AutoSize = true;
            this.chkMotor.Location = new System.Drawing.Point(891, 342);
            this.chkMotor.Name = "chkMotor";
            this.chkMotor.Size = new System.Drawing.Size(108, 24);
            this.chkMotor.TabIndex = 14;
            this.chkMotor.Text = "with Motor";
            this.chkMotor.UseVisualStyleBackColor = true;
            // 
            // btnStandar
            // 
            this.btnStandar.Location = new System.Drawing.Point(891, 171);
            this.btnStandar.Name = "btnStandar";
            this.btnStandar.Size = new System.Drawing.Size(176, 34);
            this.btnStandar.TabIndex = 8;
            this.btnStandar.Text = "Standar Control";
            this.btnStandar.UseVisualStyleBackColor = true;
            this.btnStandar.Click += new System.EventHandler(this.btnStandar_Click);
            // 
            // chkOnlyNN
            // 
            this.chkOnlyNN.AutoSize = true;
            this.chkOnlyNN.Location = new System.Drawing.Point(891, 389);
            this.chkOnlyNN.Name = "chkOnlyNN";
            this.chkOnlyNN.Size = new System.Drawing.Size(175, 24);
            this.chkOnlyNN.TabIndex = 14;
            this.chkOnlyNN.Text = "only Neural Network";
            this.chkOnlyNN.UseVisualStyleBackColor = true;
            // 
            // openFileDialog2
            // 
            this.openFileDialog2.FileName = "openFileDialog1";
            this.openFileDialog2.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog2_FileOk);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1134, 633);
            this.Controls.Add(this.chkOnlyNN);
            this.Controls.Add(this.chkMotor);
            this.Controls.Add(this.chckBoxWarmUp);
            this.Controls.Add(this.numUDSineFrec);
            this.Controls.Add(this.lblSineFrec);
            this.Controls.Add(this.lblOutPosition);
            this.Controls.Add(this.lblMotorPos);
            this.Controls.Add(this.txtBxDesPos);
            this.Controls.Add(this.lblPosition);
            this.Controls.Add(this.btnManual);
            this.Controls.Add(this.btnStandar);
            this.Controls.Add(this.btnSine);
            this.Controls.Add(this.btnStopDemo);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.trckBrPosition);
            this.Controls.Add(this.gBNetGraphicOut);
            this.Controls.Add(this.channelParametersGroupBox);
            this.Controls.Add(this.digitalInputGroupBox);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Validation";
            this.digitalInputGroupBox.ResumeLayout(false);
            this.digitalInputGroupBox.PerformLayout();
            this.channelParametersGroupBox.ResumeLayout(false);
            this.channelParametersGroupBox.PerformLayout();
            this.gBNetGraphicOut.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pBNetGraphicOut)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trckBrPosition)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numUDSineFrec)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.GroupBox digitalInputGroupBox;
        private System.Windows.Forms.ComboBox counterComboBox;
        private System.Windows.Forms.ComboBox digitalInputComboBox;
        private System.Windows.Forms.Label zIndexPhaseLabel;
        private System.Windows.Forms.Label digitalChannelLabel;
        private System.Windows.Forms.ComboBox zIndexPhaseComboBox;
        private System.Windows.Forms.Label decodingTypeLabel;
        private System.Windows.Forms.CheckBox zIndexEnabledCheckBox;
        private System.Windows.Forms.Label pulsesPerRevLabel;
        private System.Windows.Forms.ComboBox decodingTypeComboBox;
        private System.Windows.Forms.Label zIndexValueLabel;
        private System.Windows.Forms.TextBox zIndexValueTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox pulsePerRevTextBox;
        private System.Windows.Forms.GroupBox channelParametersGroupBox;
        private System.Windows.Forms.ComboBox physicalChannelComboBox;
        private System.Windows.Forms.Label physicalChannelLabel;
        private System.Windows.Forms.TextBox maximumTextBox;
        private System.Windows.Forms.TextBox minimumTextBox;
        private System.Windows.Forms.Label maximumLabel;
        private System.Windows.Forms.Label minimumLabel;
        private System.Windows.Forms.GroupBox gBNetGraphicOut;
        private System.Windows.Forms.PictureBox pBNetGraphicOut;
        private System.Windows.Forms.Timer tmrRefresh;
        private System.Windows.Forms.TrackBar trckBrPosition;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Label lblPosition;
        private System.Windows.Forms.Label lblOutPosition;
        private System.Windows.Forms.Button btnStopDemo;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem archivoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openGenomeToTestToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label lblMotorPos;
        private System.Windows.Forms.TextBox txtBxDesPos;
        private System.Windows.Forms.Button btnSine;
        private System.Windows.Forms.Button btnManual;
        private System.Windows.Forms.Label lblSineFrec;
        private System.Windows.Forms.NumericUpDown numUDSineFrec;
        private System.Windows.Forms.ToolStripMenuItem savePositionDataToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.CheckBox chckBoxWarmUp;
        private System.Windows.Forms.CheckBox chkMotor;
        private System.Windows.Forms.Button btnStandar;
        private System.Windows.Forms.ToolStripMenuItem openControlSignalToolStripMenuItem;
        private System.Windows.Forms.CheckBox chkOnlyNN;
        private System.Windows.Forms.OpenFileDialog openFileDialog2;
    }
}

