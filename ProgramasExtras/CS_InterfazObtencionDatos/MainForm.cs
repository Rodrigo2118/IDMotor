/******************************************************************************
*
* Example program:
*   MultiFunctionSyncAI_ReadDigChan
*
* Category:
*   Synchronization
*
* Description:
*   This example demonstrates how to continuously acquire analog and digital
*   data at the same time, synchronized with one another on the same device.
*
* Instructions for running:
*   1.  Select the physical channel to correspond to where your analog signal is
*       input on the DAQ device.
*   2.  Select the channel to correspond to where your digital signal is input
*       on the DAQ device.
*   3.  Enter the minimum and maximum voltage ranges.Note:  For better accuracy
*       try to match the input range to the expected voltage level of the
*       measured signal.
*   4.  Set the sample rate of the acquisition.Note:  The rate should be at
*       least twice as fast as the maximum frequency component of the signal
*       being acquired.  Note:  This example requires two DMA channels to run. 
*       If your hardware does not support two DMA channels, you need to set the
*       DataTransferMechanism property for the digital input task to use
*       interrupts.  The DataTransferMechanism property is accessible via the
*       DIChannel class. Refer to your device documentation to determine how
*       many DMA channels are supported for your hardware.
*
* Steps:
*   1.  Create an analog input voltage channel and a digital input channel.
*   2.  Set the rate for the sample clocks. Additionally, define the sample
*       modes to be continuous.
*   3.  Set the source of the digital task's sample clock to the sample clock of
*       the analog task.
*   4.  Call Task.Start() on each task to start the acquisition and
*       generation.Note: The digital input task must start before the analog
*       input task to ensure that both tasks start at the same time.
*   5.  Create an AnalogMultiChannelReader and associate it with the analog
*       input task by using the task's stream. Call
*       AnalogMultiChannelReader.BeginReadWaveform to install a callback and
*       begin the asynchronous read operation.
*   6.  Create an DigitalMultiChannelReader and associate it with the digital
*       input task by using the task's stream. Call
*       DigitalMultiChannelReader.BeginReadWaveform to install a callback and
*       begin the asynchronous read operation.
*   7.  Inside the callbacks, read the data and display it.
*   8.  Dispose the Task object to clean-up any resources associated with the
*       task.
*   9.  Handle any DaqExceptions, if they occur.
*
*   Note: This example sets SynchronizeCallback to true. If SynchronizeCallback
*   is set to false, then you must give special consideration to safely dispose
*   the task and to update the UI from the callback. If SynchronizeCallback is
*   set to false, the callback executes on the worker thread and not on the main
*   UI thread. You can only update a UI component on the thread on which it was
*   created. Refer to the How to: Safely Dispose Task When Using Asynchronous
*   Callbacks topic in the NI-DAQmx .NET help for more information.
*
* I/O Connections Overview:
*   Make sure your signal input terminals match the Physical Channel I/O
*   controls.
*
* Microsoft Windows Vista User Account Control
*   Running certain applications on Microsoft Windows Vista requires
*   administrator privileges, 
*   because the application name contains keywords such as setup, update, or
*   install. To avoid this problem, 
*   you must add an additional manifest to the application that specifies the
*   privileges required to run 
*   the application. Some Measurement Studio NI-DAQmx examples for Visual Studio
*   include these keywords. 
*   Therefore, all examples for Visual Studio are shipped with an additional
*   manifest file that you must 
*   embed in the example executable. The manifest file is named
*   [ExampleName].exe.manifest, where [ExampleName] 
*   is the NI-provided example name. For information on how to embed the manifest
*   file, refer to http://msdn2.microsoft.com/en-us/library/bb756929.aspx.Note: 
*   The manifest file is not provided with examples for Visual Studio .NET 2003.
*
******************************************************************************/

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using NationalInstruments;
using NationalInstruments.DAQmx;
using System.IO;

namespace NationalInstruments.Examples.SyncAI_ReadDigChan
{
    /// <summary>
    /// Summary description for MainForm.
    /// </summary>
    public class MainForm : System.Windows.Forms.Form
    {
        private DataTable inputDataTable = null;
        private DataColumn[] inputDataColumns = null;
        private Task analogTask;
        private Task digitalTask;
        private Task waveTask;
        private AnalogMultiChannelReader analogReader;
        //private DigitalMultiChannelReader digitalReader;
        private CounterMultiChannelReader digitalReader;
        //private CounterSingleChannelReader counterInReader;
        private CIEncoderDecodingType encoderType;
        private CIEncoderZIndexPhase encoderPhase;
        private bool zIndexEnable;
        private AsyncCallback analogCallback;
        private AsyncCallback digitalCallback;
        private Task runningAnalogTask;
        private Task runningDigitalTask;
        private int samples;
        private int aCount;
        private int dCount;

        private double[,] experimentData;
        private StreamWriter myWriter;
        private string eD_File = "experimento";//.csv";
        private int nBatch = 0;
        private int nABatch = 0;
        private int nDBatch = 0;
        //private bool Saved = false;

        private System.Windows.Forms.GroupBox timingGroupBox;
        private System.Windows.Forms.NumericUpDown rateNumeric;
        private System.Windows.Forms.Label rateLabel;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.ComboBox analogInputComboBox;
        private System.Windows.Forms.NumericUpDown inputMinValNumeric;
        private System.Windows.Forms.Label analogChannelLabel;
        private System.Windows.Forms.Label inputMaxValLabel;
        private System.Windows.Forms.Label inputMinValLabel;
        private System.Windows.Forms.NumericUpDown inputMaxValNumeric;
        private System.Windows.Forms.Label samplesLabel;
        private System.Windows.Forms.NumericUpDown samplesNumeric;
        private System.Windows.Forms.ComboBox digitalInputComboBox;
        private System.Windows.Forms.Label digitalChannelLabel;
        private System.Windows.Forms.GroupBox inputDataGroupBox;
        private System.Windows.Forms.DataGrid inputDataGrid;
        private System.Windows.Forms.GroupBox analogInputGroupBox;
        private System.Windows.Forms.GroupBox digitalInputGroupBox;
        private ComboBox counterComboBox;
        private Label zIndexPhaseLabel;
        private ComboBox zIndexPhaseComboBox;
        private Label decodingTypeLabel;
        private CheckBox zIndexEnabledCheckBox;
        private Label pulsesPerRevLabel;
        private ComboBox decodingTypeComboBox;
        private Label zIndexValueLabel;
        private TextBox zIndexValueTextBox;
        private Label label5;
        private TextBox pulsePerRevTextBox;
        private Label lblNoBatches;
        internal NumericUpDown numericUpDownNoBatches;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem saveDataAsToolStripMenuItem;
        private SaveFileDialog saveFileDialog1;
        private GroupBox groupBox1;
        private GroupBox channelParametersGroupBox;
        private ComboBox physicalChannelComboBox;
        private Label physicalChannelLabel;
        private TextBox maximumTextBox;
        private TextBox minimumTextBox;
        private Label maximumLabel;
        private Label minimumLabel;
        private GroupBox functionGeneratorGroupBox;
        internal Label amplitudeLabel;
        internal NumericUpDown amplitudeNumeric;
        private NumericUpDown samplesPerBufferNumeric;
        private Label cyclesPerBufferLabel;
        private NumericUpDown cyclesPerBufferNumeric;
        private Label signalTypeLabel;
        private ComboBox signalTypeComboBox;
        private Label samplesperBufferLabel;
        private GroupBox timingParametersGroupBox;
        private NumericUpDown frequencyNumeric;
        private Label frequencyLabel;
        private Button btnStartWave;
        private Button btnStopWave;
        private IContainer components;

        public MainForm()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            FunctionGenerator.InitComboBox(signalTypeComboBox);
            // Initialize UI
            analogInputComboBox.Items.AddRange(DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.AI, PhysicalChannelAccess.External));
            counterComboBox.Items.AddRange(DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.CI, PhysicalChannelAccess.External));
            physicalChannelComboBox.Items.AddRange(DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.AO, PhysicalChannelAccess.External));

            decodingTypeComboBox.SelectedItem = "X4";
            zIndexPhaseComboBox.SelectedItem = "A High B High";

            if (analogInputComboBox.Items.Count > 0)
                analogInputComboBox.SelectedIndex = 0;
            if (counterComboBox.Items.Count > 0)
                counterComboBox.SelectedIndex = 0;
            if (physicalChannelComboBox.Items.Count > 0)
                physicalChannelComboBox.SelectedIndex = 0;

            if (analogInputComboBox.Items.Count > 0 && counterComboBox.Items.Count > 0)
                startButton.Enabled = true;
            
            // Set up the data table
            inputDataTable = new DataTable();

            inputDataGrid.DataSource = inputDataTable;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if (components != null) 
                {
                    components.Dispose();
                }
                if (analogTask != null)
                {
                    runningAnalogTask = null;
                    analogTask.Dispose();
                }
                if (digitalTask != null)
                {
                    runningDigitalTask = null;
                    digitalTask.Dispose();
                }
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.analogInputGroupBox = new System.Windows.Forms.GroupBox();
            this.analogInputComboBox = new System.Windows.Forms.ComboBox();
            this.inputMinValNumeric = new System.Windows.Forms.NumericUpDown();
            this.analogChannelLabel = new System.Windows.Forms.Label();
            this.inputMaxValLabel = new System.Windows.Forms.Label();
            this.inputMinValLabel = new System.Windows.Forms.Label();
            this.inputMaxValNumeric = new System.Windows.Forms.NumericUpDown();
            this.timingGroupBox = new System.Windows.Forms.GroupBox();
            this.lblNoBatches = new System.Windows.Forms.Label();
            this.numericUpDownNoBatches = new System.Windows.Forms.NumericUpDown();
            this.rateNumeric = new System.Windows.Forms.NumericUpDown();
            this.samplesLabel = new System.Windows.Forms.Label();
            this.rateLabel = new System.Windows.Forms.Label();
            this.samplesNumeric = new System.Windows.Forms.NumericUpDown();
            this.startButton = new System.Windows.Forms.Button();
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
            this.stopButton = new System.Windows.Forms.Button();
            this.inputDataGroupBox = new System.Windows.Forms.GroupBox();
            this.inputDataGrid = new System.Windows.Forms.DataGrid();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveDataAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnStartWave = new System.Windows.Forms.Button();
            this.channelParametersGroupBox = new System.Windows.Forms.GroupBox();
            this.physicalChannelComboBox = new System.Windows.Forms.ComboBox();
            this.physicalChannelLabel = new System.Windows.Forms.Label();
            this.maximumTextBox = new System.Windows.Forms.TextBox();
            this.minimumTextBox = new System.Windows.Forms.TextBox();
            this.maximumLabel = new System.Windows.Forms.Label();
            this.minimumLabel = new System.Windows.Forms.Label();
            this.btnStopWave = new System.Windows.Forms.Button();
            this.functionGeneratorGroupBox = new System.Windows.Forms.GroupBox();
            this.amplitudeLabel = new System.Windows.Forms.Label();
            this.amplitudeNumeric = new System.Windows.Forms.NumericUpDown();
            this.samplesPerBufferNumeric = new System.Windows.Forms.NumericUpDown();
            this.cyclesPerBufferLabel = new System.Windows.Forms.Label();
            this.cyclesPerBufferNumeric = new System.Windows.Forms.NumericUpDown();
            this.signalTypeLabel = new System.Windows.Forms.Label();
            this.signalTypeComboBox = new System.Windows.Forms.ComboBox();
            this.samplesperBufferLabel = new System.Windows.Forms.Label();
            this.timingParametersGroupBox = new System.Windows.Forms.GroupBox();
            this.frequencyNumeric = new System.Windows.Forms.NumericUpDown();
            this.frequencyLabel = new System.Windows.Forms.Label();
            this.analogInputGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.inputMinValNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.inputMaxValNumeric)).BeginInit();
            this.timingGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownNoBatches)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rateNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.samplesNumeric)).BeginInit();
            this.digitalInputGroupBox.SuspendLayout();
            this.inputDataGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.inputDataGrid)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.channelParametersGroupBox.SuspendLayout();
            this.functionGeneratorGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.amplitudeNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.samplesPerBufferNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cyclesPerBufferNumeric)).BeginInit();
            this.timingParametersGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.frequencyNumeric)).BeginInit();
            this.SuspendLayout();
            // 
            // analogInputGroupBox
            // 
            this.analogInputGroupBox.Controls.Add(this.analogInputComboBox);
            this.analogInputGroupBox.Controls.Add(this.inputMinValNumeric);
            this.analogInputGroupBox.Controls.Add(this.analogChannelLabel);
            this.analogInputGroupBox.Controls.Add(this.inputMaxValLabel);
            this.analogInputGroupBox.Controls.Add(this.inputMinValLabel);
            this.analogInputGroupBox.Controls.Add(this.inputMaxValNumeric);
            this.analogInputGroupBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.analogInputGroupBox.Location = new System.Drawing.Point(13, 46);
            this.analogInputGroupBox.Name = "analogInputGroupBox";
            this.analogInputGroupBox.Size = new System.Drawing.Size(408, 163);
            this.analogInputGroupBox.TabIndex = 0;
            this.analogInputGroupBox.TabStop = false;
            this.analogInputGroupBox.Text = "Channel Parameters - Analog Input";
            // 
            // analogInputComboBox
            // 
            this.analogInputComboBox.Location = new System.Drawing.Point(243, 35);
            this.analogInputComboBox.Name = "analogInputComboBox";
            this.analogInputComboBox.Size = new System.Drawing.Size(154, 28);
            this.analogInputComboBox.TabIndex = 1;
            this.analogInputComboBox.Text = "Dev1/ai0";
            // 
            // inputMinValNumeric
            // 
            this.inputMinValNumeric.DecimalPlaces = 2;
            this.inputMinValNumeric.Location = new System.Drawing.Point(243, 117);
            this.inputMinValNumeric.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.inputMinValNumeric.Name = "inputMinValNumeric";
            this.inputMinValNumeric.Size = new System.Drawing.Size(145, 26);
            this.inputMinValNumeric.TabIndex = 5;
            this.inputMinValNumeric.Value = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            // 
            // analogChannelLabel
            // 
            this.analogChannelLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.analogChannelLabel.Location = new System.Drawing.Point(26, 38);
            this.analogChannelLabel.Name = "analogChannelLabel";
            this.analogChannelLabel.Size = new System.Drawing.Size(192, 23);
            this.analogChannelLabel.TabIndex = 0;
            this.analogChannelLabel.Text = "Analog Input Channels:";
            // 
            // inputMaxValLabel
            // 
            this.inputMaxValLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.inputMaxValLabel.Location = new System.Drawing.Point(26, 79);
            this.inputMaxValLabel.Name = "inputMaxValLabel";
            this.inputMaxValLabel.Size = new System.Drawing.Size(153, 23);
            this.inputMaxValLabel.TabIndex = 2;
            this.inputMaxValLabel.Text = "Maximum Value:";
            // 
            // inputMinValLabel
            // 
            this.inputMinValLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.inputMinValLabel.Location = new System.Drawing.Point(26, 120);
            this.inputMinValLabel.Name = "inputMinValLabel";
            this.inputMinValLabel.Size = new System.Drawing.Size(153, 23);
            this.inputMinValLabel.TabIndex = 4;
            this.inputMinValLabel.Text = "Minimum Value:";
            // 
            // inputMaxValNumeric
            // 
            this.inputMaxValNumeric.DecimalPlaces = 2;
            this.inputMaxValNumeric.Location = new System.Drawing.Point(243, 76);
            this.inputMaxValNumeric.Name = "inputMaxValNumeric";
            this.inputMaxValNumeric.Size = new System.Drawing.Size(154, 26);
            this.inputMaxValNumeric.TabIndex = 3;
            this.inputMaxValNumeric.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // timingGroupBox
            // 
            this.timingGroupBox.Controls.Add(this.lblNoBatches);
            this.timingGroupBox.Controls.Add(this.numericUpDownNoBatches);
            this.timingGroupBox.Controls.Add(this.rateNumeric);
            this.timingGroupBox.Controls.Add(this.samplesLabel);
            this.timingGroupBox.Controls.Add(this.rateLabel);
            this.timingGroupBox.Controls.Add(this.samplesNumeric);
            this.timingGroupBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.timingGroupBox.Location = new System.Drawing.Point(427, 46);
            this.timingGroupBox.Name = "timingGroupBox";
            this.timingGroupBox.Size = new System.Drawing.Size(333, 163);
            this.timingGroupBox.TabIndex = 2;
            this.timingGroupBox.TabStop = false;
            this.timingGroupBox.Text = "Timing Parameters";
            // 
            // lblNoBatches
            // 
            this.lblNoBatches.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblNoBatches.Location = new System.Drawing.Point(26, 120);
            this.lblNoBatches.Name = "lblNoBatches";
            this.lblNoBatches.Size = new System.Drawing.Size(135, 23);
            this.lblNoBatches.TabIndex = 4;
            this.lblNoBatches.Text = "Total Batches";
            // 
            // numericUpDownNoBatches
            // 
            this.numericUpDownNoBatches.Location = new System.Drawing.Point(167, 123);
            this.numericUpDownNoBatches.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownNoBatches.Name = "numericUpDownNoBatches";
            this.numericUpDownNoBatches.Size = new System.Drawing.Size(149, 26);
            this.numericUpDownNoBatches.TabIndex = 5;
            this.numericUpDownNoBatches.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // rateNumeric
            // 
            this.rateNumeric.DecimalPlaces = 2;
            this.rateNumeric.Location = new System.Drawing.Point(167, 38);
            this.rateNumeric.Maximum = new decimal(new int[] {
            102400,
            0,
            0,
            0});
            this.rateNumeric.Name = "rateNumeric";
            this.rateNumeric.Size = new System.Drawing.Size(149, 26);
            this.rateNumeric.TabIndex = 1;
            this.rateNumeric.Value = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            // 
            // samplesLabel
            // 
            this.samplesLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.samplesLabel.Location = new System.Drawing.Point(26, 76);
            this.samplesLabel.Name = "samplesLabel";
            this.samplesLabel.Size = new System.Drawing.Size(135, 23);
            this.samplesLabel.TabIndex = 2;
            this.samplesLabel.Text = "Samples to Read:";
            // 
            // rateLabel
            // 
            this.rateLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.rateLabel.Location = new System.Drawing.Point(26, 38);
            this.rateLabel.Name = "rateLabel";
            this.rateLabel.Size = new System.Drawing.Size(153, 23);
            this.rateLabel.TabIndex = 0;
            this.rateLabel.Text = "Sample Rate (Hz):";
            // 
            // samplesNumeric
            // 
            this.samplesNumeric.Location = new System.Drawing.Point(167, 79);
            this.samplesNumeric.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.samplesNumeric.Name = "samplesNumeric";
            this.samplesNumeric.Size = new System.Drawing.Size(149, 26);
            this.samplesNumeric.TabIndex = 3;
            this.samplesNumeric.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // startButton
            // 
            this.startButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.startButton.Enabled = false;
            this.startButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.startButton.Location = new System.Drawing.Point(297, 576);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(120, 34);
            this.startButton.TabIndex = 4;
            this.startButton.Text = "Start";
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
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
            this.digitalInputGroupBox.Location = new System.Drawing.Point(13, 215);
            this.digitalInputGroupBox.Name = "digitalInputGroupBox";
            this.digitalInputGroupBox.Size = new System.Drawing.Size(408, 344);
            this.digitalInputGroupBox.TabIndex = 1;
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
            // stopButton
            // 
            this.stopButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.stopButton.Enabled = false;
            this.stopButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.stopButton.Location = new System.Drawing.Point(427, 576);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(120, 34);
            this.stopButton.TabIndex = 5;
            this.stopButton.Text = "Stop";
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // inputDataGroupBox
            // 
            this.inputDataGroupBox.Controls.Add(this.inputDataGrid);
            this.inputDataGroupBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.inputDataGroupBox.Location = new System.Drawing.Point(427, 215);
            this.inputDataGroupBox.Name = "inputDataGroupBox";
            this.inputDataGroupBox.Size = new System.Drawing.Size(333, 344);
            this.inputDataGroupBox.TabIndex = 3;
            this.inputDataGroupBox.TabStop = false;
            this.inputDataGroupBox.Text = "Input Data";
            // 
            // inputDataGrid
            // 
            this.inputDataGrid.AllowSorting = false;
            this.inputDataGrid.DataMember = "";
            this.inputDataGrid.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.inputDataGrid.Location = new System.Drawing.Point(9, 28);
            this.inputDataGrid.Name = "inputDataGrid";
            this.inputDataGrid.PreferredColumnWidth = 100;
            this.inputDataGrid.ReadOnly = true;
            this.inputDataGrid.Size = new System.Drawing.Size(307, 296);
            this.inputDataGrid.TabIndex = 0;
            this.inputDataGrid.TabStop = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1263, 33);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveDataAsToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(50, 29);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // saveDataAsToolStripMenuItem
            // 
            this.saveDataAsToolStripMenuItem.Name = "saveDataAsToolStripMenuItem";
            this.saveDataAsToolStripMenuItem.Size = new System.Drawing.Size(207, 30);
            this.saveDataAsToolStripMenuItem.Text = "Save data as...";
            this.saveDataAsToolStripMenuItem.Click += new System.EventHandler(this.saveDataAsToolStripMenuItem_Click);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.saveFileDialog1_FileOk);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnStartWave);
            this.groupBox1.Controls.Add(this.channelParametersGroupBox);
            this.groupBox1.Controls.Add(this.btnStopWave);
            this.groupBox1.Controls.Add(this.functionGeneratorGroupBox);
            this.groupBox1.Controls.Add(this.timingParametersGroupBox);
            this.groupBox1.Location = new System.Drawing.Point(766, 46);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(379, 564);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Continuous Voltage Generator";
            // 
            // btnStartWave
            // 
            this.btnStartWave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnStartWave.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnStartWave.Location = new System.Drawing.Point(48, 519);
            this.btnStartWave.Name = "btnStartWave";
            this.btnStartWave.Size = new System.Drawing.Size(120, 34);
            this.btnStartWave.TabIndex = 8;
            this.btnStartWave.Text = "Start";
            this.btnStartWave.Click += new System.EventHandler(this.button1_Click);
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
            this.channelParametersGroupBox.Location = new System.Drawing.Point(18, 25);
            this.channelParametersGroupBox.Name = "channelParametersGroupBox";
            this.channelParametersGroupBox.Size = new System.Drawing.Size(355, 164);
            this.channelParametersGroupBox.TabIndex = 2;
            this.channelParametersGroupBox.TabStop = false;
            this.channelParametersGroupBox.Text = "Channel Parameters";
            // 
            // physicalChannelComboBox
            // 
            this.physicalChannelComboBox.Location = new System.Drawing.Point(192, 35);
            this.physicalChannelComboBox.Name = "physicalChannelComboBox";
            this.physicalChannelComboBox.Size = new System.Drawing.Size(109, 28);
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
            this.maximumTextBox.Location = new System.Drawing.Point(192, 122);
            this.maximumTextBox.Name = "maximumTextBox";
            this.maximumTextBox.Size = new System.Drawing.Size(109, 26);
            this.maximumTextBox.TabIndex = 5;
            this.maximumTextBox.Text = "10";
            // 
            // minimumTextBox
            // 
            this.minimumTextBox.Location = new System.Drawing.Point(192, 78);
            this.minimumTextBox.Name = "minimumTextBox";
            this.minimumTextBox.Size = new System.Drawing.Size(109, 26);
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
            // btnStopWave
            // 
            this.btnStopWave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnStopWave.Enabled = false;
            this.btnStopWave.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnStopWave.Location = new System.Drawing.Point(189, 519);
            this.btnStopWave.Name = "btnStopWave";
            this.btnStopWave.Size = new System.Drawing.Size(120, 34);
            this.btnStopWave.TabIndex = 9;
            this.btnStopWave.Text = "Stop";
            this.btnStopWave.Click += new System.EventHandler(this.button2_Click);
            // 
            // functionGeneratorGroupBox
            // 
            this.functionGeneratorGroupBox.Controls.Add(this.amplitudeLabel);
            this.functionGeneratorGroupBox.Controls.Add(this.amplitudeNumeric);
            this.functionGeneratorGroupBox.Controls.Add(this.samplesPerBufferNumeric);
            this.functionGeneratorGroupBox.Controls.Add(this.cyclesPerBufferLabel);
            this.functionGeneratorGroupBox.Controls.Add(this.cyclesPerBufferNumeric);
            this.functionGeneratorGroupBox.Controls.Add(this.signalTypeLabel);
            this.functionGeneratorGroupBox.Controls.Add(this.signalTypeComboBox);
            this.functionGeneratorGroupBox.Controls.Add(this.samplesperBufferLabel);
            this.functionGeneratorGroupBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.functionGeneratorGroupBox.Location = new System.Drawing.Point(18, 274);
            this.functionGeneratorGroupBox.Name = "functionGeneratorGroupBox";
            this.functionGeneratorGroupBox.Size = new System.Drawing.Size(355, 210);
            this.functionGeneratorGroupBox.TabIndex = 4;
            this.functionGeneratorGroupBox.TabStop = false;
            this.functionGeneratorGroupBox.Text = "Function Generator Parameters";
            // 
            // amplitudeLabel
            // 
            this.amplitudeLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.amplitudeLabel.Location = new System.Drawing.Point(26, 164);
            this.amplitudeLabel.Name = "amplitudeLabel";
            this.amplitudeLabel.Size = new System.Drawing.Size(89, 23);
            this.amplitudeLabel.TabIndex = 6;
            this.amplitudeLabel.Text = "Amplitude:";
            // 
            // amplitudeNumeric
            // 
            this.amplitudeNumeric.DecimalPlaces = 1;
            this.amplitudeNumeric.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.amplitudeNumeric.Location = new System.Drawing.Point(192, 162);
            this.amplitudeNumeric.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.amplitudeNumeric.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.amplitudeNumeric.Name = "amplitudeNumeric";
            this.amplitudeNumeric.Size = new System.Drawing.Size(109, 26);
            this.amplitudeNumeric.TabIndex = 7;
            this.amplitudeNumeric.UseWaitCursor = true;
            this.amplitudeNumeric.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // samplesPerBufferNumeric
            // 
            this.samplesPerBufferNumeric.Location = new System.Drawing.Point(192, 120);
            this.samplesPerBufferNumeric.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.samplesPerBufferNumeric.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.samplesPerBufferNumeric.Name = "samplesPerBufferNumeric";
            this.samplesPerBufferNumeric.Size = new System.Drawing.Size(109, 26);
            this.samplesPerBufferNumeric.TabIndex = 5;
            this.samplesPerBufferNumeric.Value = new decimal(new int[] {
            2500,
            0,
            0,
            0});
            // 
            // cyclesPerBufferLabel
            // 
            this.cyclesPerBufferLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cyclesPerBufferLabel.Location = new System.Drawing.Point(26, 80);
            this.cyclesPerBufferLabel.Name = "cyclesPerBufferLabel";
            this.cyclesPerBufferLabel.Size = new System.Drawing.Size(164, 23);
            this.cyclesPerBufferLabel.TabIndex = 2;
            this.cyclesPerBufferLabel.Text = "Cycles Per Buffer:";
            // 
            // cyclesPerBufferNumeric
            // 
            this.cyclesPerBufferNumeric.Location = new System.Drawing.Point(192, 78);
            this.cyclesPerBufferNumeric.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.cyclesPerBufferNumeric.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.cyclesPerBufferNumeric.Name = "cyclesPerBufferNumeric";
            this.cyclesPerBufferNumeric.Size = new System.Drawing.Size(109, 26);
            this.cyclesPerBufferNumeric.TabIndex = 3;
            this.cyclesPerBufferNumeric.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // signalTypeLabel
            // 
            this.signalTypeLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.signalTypeLabel.Location = new System.Drawing.Point(26, 38);
            this.signalTypeLabel.Name = "signalTypeLabel";
            this.signalTypeLabel.Size = new System.Drawing.Size(139, 23);
            this.signalTypeLabel.TabIndex = 0;
            this.signalTypeLabel.Text = "Waveform Type:";
            // 
            // signalTypeComboBox
            // 
            this.signalTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.signalTypeComboBox.ItemHeight = 20;
            this.signalTypeComboBox.Items.AddRange(new object[] {
            ""});
            this.signalTypeComboBox.Location = new System.Drawing.Point(194, 35);
            this.signalTypeComboBox.Name = "signalTypeComboBox";
            this.signalTypeComboBox.Size = new System.Drawing.Size(141, 28);
            this.signalTypeComboBox.TabIndex = 1;
            // 
            // samplesperBufferLabel
            // 
            this.samplesperBufferLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.samplesperBufferLabel.Location = new System.Drawing.Point(26, 122);
            this.samplesperBufferLabel.Name = "samplesperBufferLabel";
            this.samplesperBufferLabel.Size = new System.Drawing.Size(179, 23);
            this.samplesperBufferLabel.TabIndex = 4;
            this.samplesperBufferLabel.Text = "Samples Per Buffer:";
            // 
            // timingParametersGroupBox
            // 
            this.timingParametersGroupBox.Controls.Add(this.frequencyNumeric);
            this.timingParametersGroupBox.Controls.Add(this.frequencyLabel);
            this.timingParametersGroupBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.timingParametersGroupBox.Location = new System.Drawing.Point(18, 195);
            this.timingParametersGroupBox.Name = "timingParametersGroupBox";
            this.timingParametersGroupBox.Size = new System.Drawing.Size(355, 73);
            this.timingParametersGroupBox.TabIndex = 3;
            this.timingParametersGroupBox.TabStop = false;
            this.timingParametersGroupBox.Text = "Timing Parameters";
            // 
            // frequencyNumeric
            // 
            this.frequencyNumeric.Location = new System.Drawing.Point(192, 35);
            this.frequencyNumeric.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.frequencyNumeric.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.frequencyNumeric.Name = "frequencyNumeric";
            this.frequencyNumeric.Size = new System.Drawing.Size(109, 26);
            this.frequencyNumeric.TabIndex = 1;
            this.frequencyNumeric.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // frequencyLabel
            // 
            this.frequencyLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.frequencyLabel.Location = new System.Drawing.Point(26, 38);
            this.frequencyLabel.Name = "frequencyLabel";
            this.frequencyLabel.Size = new System.Drawing.Size(140, 23);
            this.frequencyLabel.TabIndex = 0;
            this.frequencyLabel.Text = "Frequency (Hz):";
            // 
            // MainForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(8, 19);
            this.ClientSize = new System.Drawing.Size(1263, 622);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.inputDataGroupBox);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.analogInputGroupBox);
            this.Controls.Add(this.timingGroupBox);
            this.Controls.Add(this.digitalInputGroupBox);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(1206, 516);
            this.Name = "MainForm";
            this.Text = "Multi-Function Synchronization - Analog and Digital Input";
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.analogInputGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.inputMinValNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.inputMaxValNumeric)).EndInit();
            this.timingGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownNoBatches)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rateNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.samplesNumeric)).EndInit();
            this.digitalInputGroupBox.ResumeLayout(false);
            this.digitalInputGroupBox.PerformLayout();
            this.inputDataGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.inputDataGrid)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.channelParametersGroupBox.ResumeLayout(false);
            this.channelParametersGroupBox.PerformLayout();
            this.functionGeneratorGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.amplitudeNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.samplesPerBufferNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cyclesPerBufferNumeric)).EndInit();
            this.timingParametersGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.frequencyNumeric)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() 
        {
            Application.EnableVisualStyles();
            Application.DoEvents();
            Application.Run(new MainForm());
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            inputDataGroupBox.Width = this.Width - 402;
            inputDataGroupBox.Height = this.Height - 273;
            inputDataGrid.Width = this.Width - 413;
            inputDataGrid.Height = this.Height - 264;
        }

        private void ConfigNumeric(NumericUpDown numeric, decimal minVal)
        {
            numeric.Minimum = minVal;
            numeric.Maximum = Decimal.MaxValue;
        }

        private void ConfigNumeric(NumericUpDown numeric)
        {
            ConfigNumeric(numeric, Decimal.MinValue);
        }

        private void InitializeDataTables(int rows)
        {
            // Clear out the data
            inputDataTable.Rows.Clear();
            inputDataTable.Columns.Clear();

            // Get the number of columns
            aCount = analogTask.AIChannels.Count;
            //dCount = digitalTask.DIChannels.Count;
            dCount = digitalTask.CIChannels.Count;

            // Add one column of type double
            inputDataColumns = new DataColumn[aCount + dCount];

            // Add analog columns
            int i = 0;
            for (; i < aCount; i++)
            {
                inputDataColumns[i] = new DataColumn();
                inputDataColumns[i].DataType = typeof(double);
                inputDataColumns[i].ColumnName = analogTask.AIChannels[i].PhysicalName;
            }

            for (; i < aCount + dCount; i++)
            {
                inputDataColumns[i] = new DataColumn();
                inputDataColumns[i].DataType = typeof(double);
                inputDataColumns[i].ColumnName = digitalTask.CIChannels[i - aCount].PhysicalName;
            }

            inputDataTable.Columns.AddRange(inputDataColumns);

            // Now add a certain number of rows
            for(i = 0; i < rows; i++)             
            {
                object[] rowArr = new object[aCount + dCount];
                inputDataTable.Rows.Add(rowArr);
            }
        }

        private void startButton_Click(object sender, System.EventArgs e)
        {
            // Change the mouse to an hourglass for the duration of this function.
            Cursor.Current = Cursors.WaitCursor;

            // Read UI selections
            samples = Convert.ToInt32(samplesNumeric.Value);

            try
            {
                // Create the master and slave tasks
                analogTask = new Task("analogTask");
                digitalTask = new Task("digitalTask");
                


                // Configure both tasks with the values selected on the UI.
                analogTask.AIChannels.CreateVoltageChannel(analogInputComboBox.Text,
                    "",
                    AITerminalConfiguration.Differential,
                    Convert.ToDouble(inputMinValNumeric.Value),
                    Convert.ToDouble(inputMaxValNumeric.Value),
                    AIVoltageUnits.Volts);

                /*digitalTask.DIChannels.CreateChannel(digitalInputComboBox.Text,
                    "",
                    ChannelLineGrouping.OneChannelForEachLine);
                    */
                #region Counter
                double zIndexValue = Convert.ToDouble(zIndexValueTextBox.Text);
                int pulsePerRev = Convert.ToInt32(pulsePerRevTextBox.Text);
                zIndexEnable = zIndexEnabledCheckBox.Checked;

                switch (decodingTypeComboBox.SelectedIndex)
                {
                    case 0: //X1
                        encoderType = CIEncoderDecodingType.X1;
                        break;
                    case 1: //X2
                        encoderType = CIEncoderDecodingType.X2;
                        break;
                    case 2: //X4
                        encoderType = CIEncoderDecodingType.X4;
                        break;
                }

                switch (zIndexPhaseComboBox.SelectedIndex)
                {
                    case 0: //A High B High
                        encoderPhase = CIEncoderZIndexPhase.AHighBHigh;
                        break;
                    case 1: //A High B Low
                        encoderPhase = CIEncoderZIndexPhase.AHighBLow;
                        break;
                    case 2: //A Low B High
                        encoderPhase = CIEncoderZIndexPhase.ALowBHigh;
                        break;
                    case 3: //A Low B Low
                        encoderPhase = CIEncoderZIndexPhase.ALowBLow;
                        break;
                }

                digitalTask.CIChannels.CreateAngularEncoderChannel(counterComboBox.Text,
                    "", encoderType, zIndexEnable, zIndexValue, encoderPhase, pulsePerRev,
                    0.0, CIAngularEncoderUnits.Radians);
                #endregion

                // Set up the timing for the first task
                analogTask.Timing.ConfigureSampleClock("",
                    Convert.ToDouble(rateNumeric.Value),
                    SampleClockActiveEdge.Rising,
                    SampleQuantityMode.ContinuousSamples,
                    samples);

                // Use the same timebase for the second task
                string deviceName = analogInputComboBox.Text.Split('/')[0];
                string terminalNameBase = "/" + GetDeviceName(deviceName) + "/";

                digitalTask.Timing.ConfigureSampleClock(terminalNameBase + "ai/SampleClock",
                    Convert.ToDouble(rateNumeric.Value),
                    SampleClockActiveEdge.Rising,
                    SampleQuantityMode.ContinuousSamples,
                    samples);

                // Verify the tasks
                analogTask.Control(TaskAction.Verify);
                digitalTask.Control(TaskAction.Verify);

                // Set up the data table
                InitializeDataTables(Math.Min((inputDataGrid.Height - 50) / 17, samples)); 

                // Officially start the task
                StartTask();

                digitalTask.Start();
                analogTask.Start();
                

                // Start reading as well
                analogCallback = new AsyncCallback(AnalogRead);
                analogReader = new AnalogMultiChannelReader(analogTask.Stream);

                digitalCallback = new AsyncCallback(DigitalRead);
                digitalReader = new CounterMultiChannelReader(digitalTask.Stream);

                // Use SynchronizeCallbacks to specify that the object 
                // marshals callbacks across threads appropriately.
                analogReader.SynchronizeCallbacks = true;
                digitalReader.SynchronizeCallbacks = true;
                
                analogReader.BeginReadMultiSample(samples, analogCallback, analogTask);
                digitalReader.BeginReadMultiSampleDouble(samples, digitalCallback, digitalTask);

                // Initialize data array for saving data
                experimentData = new double[samples, 3];
                    
            }
            catch (Exception ex)
            {
                StopTask();
                MessageBox.Show(ex.Message);
            }
        }

        private void AnalogRead(IAsyncResult ar)
        {
            try
            {
                if (nBatch >= numericUpDownNoBatches.Value) StopTask();
                if (runningAnalogTask != null && runningAnalogTask == ar.AsyncState)
                {
                    // Read the data
                    double[,] data = analogReader.EndReadMultiSample(ar);

                    // Display the data
                    for (int i = 0; i < inputDataTable.Rows.Count && i < data.GetLength(1); i++)
                    {
                        for (int j = 0; j < data.GetLength(0); j++)
                        {
                            inputDataTable.Rows[i][j] = data[j,i];
                        }
                    }

                    // Save the data
                    for (int i = 0; i < data.GetLength(1); i++)
                    {
                        for (int j = 0; j < data.GetLength(0); j++)
                        {
                            experimentData[i,j+1] = data[j, i];
                        }
                    }

                    nABatch++;
                    if (nABatch == nDBatch)
                    {
                        nBatch++;
                        WriteDataToFile();
                    }

                    // Set up next callback
                    analogReader.BeginReadMultiSample(samples, analogCallback, analogTask);

                }
            }
            catch (Exception ex)
            {
                StopTask();
                MessageBox.Show(ex.Message);
            }
        }

        private void DigitalRead(IAsyncResult ar)
        {
            try
            {
                if (nBatch >= numericUpDownNoBatches.Value) StopTask();
                if (runningDigitalTask != null && runningDigitalTask == ar.AsyncState)
                {
                    // Read the data
                    double[,] data = digitalReader.EndReadMultiSampleDouble(ar);

                    // Display the data
                    for (int i = 0; i < inputDataTable.Rows.Count && i < data.GetLength(1); i++)
                    {
                        for (int j = 0; j < data.GetLength(0); j++)
                        {
                            inputDataTable.Rows[i][aCount + j] = data[j, i];
                        }
                    }

                    // Save the data
                    for (int i = 0; i < data.GetLength(1); i++)
                    {
                        for (int j = 0; j < data.GetLength(0); j++)
                        {
                            experimentData[i,aCount + j+1] = data[j, i];
                            experimentData[i, 0] = i * 1/Convert.ToDouble(rateNumeric.Value);
                        }
                    }

                    nDBatch++;
                    if (nABatch == nDBatch)
                    {
                        nBatch++;
                        WriteDataToFile();
                        
                    }

                    // Set up next callback
                    digitalReader.BeginReadMultiSampleDouble(samples, digitalCallback, digitalTask);

                }
            }
            catch (Exception ex)
            {
                StopTask();
                MessageBox.Show(ex.Message);
            }
        }

        private void stopButton_Click(object sender, System.EventArgs e)
        {
            StopTask();
            
        }

        private void StartTask()
        {
            if (runningAnalogTask == null)
            {
                // Change state
                runningAnalogTask = analogTask;
                runningDigitalTask = digitalTask;
                //runningWaveTask = waveTask;

                // Fix UI
                analogInputComboBox.Enabled = false;
                inputMinValNumeric.Enabled = false;
                inputMaxValNumeric.Enabled = false;
            
                digitalInputComboBox.Enabled = false;
                digitalInputComboBox.Enabled = false;
                digitalInputComboBox.Enabled = false;

                rateNumeric.Enabled = false;
                samplesNumeric.Enabled = false;
            
                startButton.Enabled = false;
                stopButton.Enabled = true;
            }
        }

        private void StopTask()
        {
            // Change state
            runningAnalogTask = null;
            runningDigitalTask = null;

            // Fix UI
            analogInputComboBox.Enabled = true;
            inputMinValNumeric.Enabled = true;
            inputMaxValNumeric.Enabled = true;
            
            digitalInputComboBox.Enabled = true;
            digitalInputComboBox.Enabled = true;
            digitalInputComboBox.Enabled = true;

            rateNumeric.Enabled = true;
            samplesNumeric.Enabled = true;
            
            startButton.Enabled = true;
            stopButton.Enabled = false;

            nBatch = 0;
        
            // Stop tasks
            analogTask.Stop();
            digitalTask.Stop();
            //waveTask.Stop();

            analogTask.Dispose();
            digitalTask.Dispose();
            //waveTask.Dispose();
        }

        public static string GetDeviceName(string deviceName)
        {
            Device device = DaqSystem.Local.LoadDevice(deviceName);
            if (device.BusType != DeviceBusType.CompactDaq)
                return deviceName;
            else
                return device.CompactDaqChassisDeviceName;
        }

        private void saveDataAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            eD_File = saveFileDialog1.FileName;
        }

        private void WriteDataToFile()
        {
            myWriter = new StreamWriter(eD_File + nBatch + ".csv");
            for (int i = 0; i < samples; i++)
            {
                if (i != 0)
                    myWriter.Write("\n");
                for (int j = 0; j < 3; j++)
                {
                    if (j != 0)
                        myWriter.Write(",");
                    myWriter.Write(experimentData[i, j]);
                }
            }
            

            myWriter.Flush();
            myWriter.Close();
            experimentData = new double[samples,3];
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Change the mouse to an hourglass for the duration of this function.
            Cursor.Current = Cursors.WaitCursor;

            // Read UI selections
            samples = Convert.ToInt32(samplesNumeric.Value);

            try
            {
                waveTask = new Task("OutputWaveTask");
                #region Wave configuration
                waveTask.AOChannels.CreateVoltageChannel(physicalChannelComboBox.Text,
                    "",
                    Convert.ToDouble(minimumTextBox.Text),
                    Convert.ToDouble(maximumTextBox.Text),
                    AOVoltageUnits.Volts);

                // verify the task before doing the waveform calculations
                waveTask.Control(TaskAction.Verify);

                // calculate some waveform parameters and generate data
                FunctionGenerator fGen = new FunctionGenerator(
                    waveTask.Timing,
                    frequencyNumeric.Value.ToString(),
                    samplesPerBufferNumeric.Value.ToString(),
                    cyclesPerBufferNumeric.Value.ToString(),
                    signalTypeComboBox.Text,
                    amplitudeNumeric.Value.ToString());

                // configure the sample clock with the calculated rate
                waveTask.Timing.ConfigureSampleClock("",
                    fGen.ResultingSampleClockRate,
                    SampleClockActiveEdge.Rising,
                    SampleQuantityMode.ContinuousSamples, 1000);


                AnalogSingleChannelWriter writer =
                    new AnalogSingleChannelWriter(waveTask.Stream);

                //write data to buffer
                writer.WriteMultiSample(false, fGen.Data);

                #endregion
                waveTask.Start();
                btnStartWave.Enabled = false;
                btnStopWave.Enabled = true;
            }
            catch (Exception ex)
            {
                button2_Click(sender,e);
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            waveTask.Stop();
            waveTask.Dispose();
            btnStartWave.Enabled = true;
            btnStopWave.Enabled = false;
        }
    }
}
