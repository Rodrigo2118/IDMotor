using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NationalInstruments;
using NationalInstruments.DAQmx;
using SharpNeat.Genomes.Neat;
using SharpNeat.Phenomes;
using SharpNeat.Core;
using SharpNeat.Decoders.Neat;
using SharpNeat.Decoders;
using SharpNeat.Domains.IDMotor;
using SharpNeat.Domains;
using System.Xml;
using System.IO;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private Task digitalTask;
        private Task waveTask;
        private CounterMultiChannelReader digitalReader;
        private AnalogSingleChannelWriter writer;
        private CIEncoderDecodingType encoderType;
        private CIEncoderZIndexPhase encoderPhase;
        private bool zIndexEnable;
        //private AsyncCallback analogCallback;
        //private AsyncCallback digitalCallback;
        //private Task runningAnalogTask;
        //private Task runningDigitalTask;
        private Pen myPen;
        private NeatGenome neatGenome;
        private IBlackBox box;
        private IGenomeDecoder<NeatGenome, IBlackBox> _genomeDecoder;
        private NetworkActivationScheme _activationScheme;
        private XmlDocument xmlConfig;
        private string genomeFile= "C:\\Users\\Rodrigo\\Desktop\\Tesis\\DAQ\\Datos_Para_Entrenamiento\\GenomasGanadores\\champ_Chirp_1_26_11_2019.gnm.xml.xml";
        private double desiredPos=0;
        private double actualPos=0;
        private double motorPos = 0;
        private double error=0;
        private double motorerror = 0;
        private double P=1;
        private double ctrlV=0;
        private double ctrlVmotor = 0;
        private double n = 0;
        //private double tn = 0;
        private bool Sine = false;
        private bool Standar = false;
        private double frec = 0.001;
        private string dataFile = "Position_NN_Real_Control0.csv";
        private StreamWriter csvWriter;
        private StreamReader csvReader;
        private double SatV=9;
        private DateTime tn;
        private DateTime ta;
        private DateTime ti;
        private int sign = 1;
        private string csFile;
        private double[,] ControlSignal;
        private long i = 0;


        public Form1()
        {
            InitializeComponent();
            
            counterComboBox.Items.AddRange(DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.CI, PhysicalChannelAccess.External));
            physicalChannelComboBox.Items.AddRange(DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.AO, PhysicalChannelAccess.External));

            decodingTypeComboBox.SelectedItem = "X4";
            zIndexPhaseComboBox.SelectedItem = "A High B High";


            if (counterComboBox.Items.Count > 0)
                counterComboBox.SelectedIndex = 0;
            if (physicalChannelComboBox.Items.Count > 0)
                physicalChannelComboBox.SelectedIndex = 0;

            if (physicalChannelComboBox.Items.Count > 0 && counterComboBox.Items.Count > 0)
                btnStart.Enabled = true;

            myPen = new Pen(Color.DarkCyan, 1);
            pBNetGraphicOut.Dock = DockStyle.Fill;
            pBNetGraphicOut.BackColor = Color.White;
            pBNetGraphicOut.Refresh();
        }

        private void pBNetGraphicOut_Paint(object sender, PaintEventArgs e)
        {
            double radius = pBNetGraphicOut.Width * 0.4;
            double centerX=pBNetGraphicOut.Width/2, centerY=centerX;
            double theta = actualPos;

            e.Graphics.DrawEllipse(myPen, (float)(centerX - radius), (float)(centerY - radius ), (float)radius*2, (float)radius*2);
            e.Graphics.DrawLine(myPen, (float)(centerX), (float)(centerY), (float)(centerX + radius * Math.Cos(-theta)), (float)(centerY + radius * Math.Sin(-theta)));
        }

        private void trckBrPosition_Scroll(object sender, EventArgs e)
        {
            lblPosition.Text = "Desired position: " + trckBrPosition.Value + "°";
            txtBxDesPos.Text = trckBrPosition.Value + "°";
        }

        private void tmrRefresh_Tick(object sender, EventArgs e)
        {
            
            tn = (DateTime.Now);
            //n++;
            //lbldebug.Text = n.ToString()+", "+tn ;
            n = (tn-ta).TotalSeconds;
            ta = tn;

            if (Sine)
            {
                desiredPos = Math.PI * Math.Sin(frec * (tn - ti).TotalSeconds);
                trckBrPosition.Value = (int)(desiredPos * 180 / Math.PI);
                txtBxDesPos.Text = trckBrPosition.Value + "°";
            }
            else if (Standar && ControlSignal != null)
            {
                if (ControlSignal.GetLength(0) <= i)
                {
                    btnStopDemo_Click(sender, e);
                    return;
                }
                if(i!=0)
                    n = ControlSignal[i, 0]-ControlSignal[i-1,0];
                else
                    n = 0;
                desiredPos = ControlSignal[i, 1];
                
                //desiredPos = sign *3* Math.PI* Math.Sin(15 * (tn-ti).TotalSeconds);
                trckBrPosition.Value = (int)(desiredPos * 180 / Math.PI);
                txtBxDesPos.Text = trckBrPosition.Value + "°";
                i++;
            }
            else
                desiredPos = trckBrPosition.Value * Math.PI / 180;

            error = (desiredPos - actualPos);
            motorerror = desiredPos - motorPos;

            ctrlV = P * error;
            ctrlVmotor = P * motorerror;
            if (Math.Abs(ctrlVmotor) > SatV)
                ctrlVmotor = SatV* Math.Sign(ctrlVmotor);
            if (Math.Abs(ctrlV) > SatV)
                ctrlV = SatV * Math.Sign(ctrlV);
            if (chkMotor.Checked)
                writer.WriteSingleSample(false, ctrlVmotor );

            box.InputSignalArray[0] = n;// tmrRefresh.Interval/1000.0 * n;
            box.InputSignalArray[1] = ctrlV;
            box.InputSignalArray[2] = actualPos;

            box.Activate();

            actualPos = box.OutputSignalArray[0];
            lblOutPosition.Text = "Output position: " + (actualPos * 180 / Math.PI).ToString("0.000") + "°";

            if (chkMotor.Checked)
                motorPos = digitalReader.ReadSingleSampleDouble()[0];
            lblMotorPos.Text = "Motor position: " + (motorPos * 180 / Math.PI).ToString("0.000") + "°";

            
            
            pBNetGraphicOut.Refresh();

            if (!chckBoxWarmUp.Checked)
            {
                csvWriter.Flush();
                csvWriter.WriteLine((tn-ti).TotalSeconds + "," + actualPos + "," + motorPos+","+desiredPos);
            }
            else if(chkOnlyNN.Checked)
            {
                csvWriter.Flush();
                csvWriter.WriteLine((tn - ti).TotalSeconds + "," + actualPos);
            }
            
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                #region NNInit
                IDMotorExperiment _experiment = new IDMotorExperiment();
                xmlConfig = new XmlDocument();
                xmlConfig.Load("IDMotor.config.xml");
                _experiment.Initialize("IDMotor", xmlConfig.DocumentElement);
                _activationScheme = ExperimentUtils.CreateActivationScheme(xmlConfig.DocumentElement, "Activation");
                XmlReader xr = XmlReader.Create(genomeFile);
                List<NeatGenome> _genomeList = _experiment.LoadPopulation(xr);
                _genomeDecoder = new NeatGenomeDecoder(_activationScheme);
                neatGenome = _genomeList[0];
                box = _genomeDecoder.Decode(neatGenome);
                #endregion

                if (chkMotor.Checked)
                {
                    #region Encoder Init
                    digitalTask = new Task("digitalTask");

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

                    digitalTask.Control(TaskAction.Verify);

                    digitalReader = new CounterMultiChannelReader(digitalTask.Stream);
                    digitalTask.Start();

                    #endregion

                    #region Vout Init
                    waveTask = new Task("ControlVoltageTask");
                    waveTask.AOChannels.CreateVoltageChannel(physicalChannelComboBox.Text,
                    "",
                    Convert.ToDouble(minimumTextBox.Text),
                    Convert.ToDouble(maximumTextBox.Text),
                    AOVoltageUnits.Volts);

                    // verify the task before doing the waveform calculations
                    waveTask.Control(TaskAction.Verify);

                    writer = new AnalogSingleChannelWriter(waveTask.Stream);


                    waveTask.Start();

                    #endregion
                }
                box.ResetState();
                actualPos = 0;
                n = 0;
                tmrRefresh.Enabled = true;
                btnStart.Enabled = false;
                btnStopDemo.Enabled = true;
                motorPos = 0;
                ti = DateTime.Now;
                ta = ti;

                if (csFile != null && ControlSignal ==null)
                    ControlSignal = unpackControlSignal(csFile);
                i = 0;

                if(!chckBoxWarmUp.Checked)
                    csvWriter = new StreamWriter(dataFile);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnStopDemo_Click(object sender, EventArgs e)
        {
            if (chkMotor.Checked)
            {
                digitalTask.Stop();
                digitalTask.Dispose();

                writer.WriteSingleSample(false, 0);
                waveTask.Stop();
                waveTask.Dispose();
            }
            if (!chckBoxWarmUp.Checked) csvWriter.Dispose();

            tmrRefresh.Enabled = false;
            btnStart.Enabled = true;
            btnStopDemo.Enabled = false;
        }

        private void openGenomeToTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //openFileDialog1.ShowDialog();
            try
            {
                openFileDialog1.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
                openFileDialog1.FilterIndex = 2;
                openFileDialog1.ShowDialog();
                
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            genomeFile = openFileDialog1.FileName;
            saveFileDialog1.FileName = "C:\\Users\\Rodrigo\\Desktop\\Tesis\\DAQ\\Datos_Para_Entrenamiento\\Genomas\\Validacion\\" + openFileDialog1.SafeFileName.Replace(".gnm.xml", "_val.csv");
            dataFile = saveFileDialog1.FileName;

        }

        private void txtBxDesPos_TextChanged(object sender, EventArgs e)
        {
            int i = 0;
            if (int.TryParse(txtBxDesPos.Text.Trim('°'), out i))
                trckBrPosition.Value =i ;
            //txtBxDesPos.Text+='°';
        }

        private void btnSine_Click(object sender, EventArgs e)
        {
            Sine = true;
            Standar = false;
            btnSine.Enabled = false;
            btnManual.Enabled = true;
            btnStandar.Enabled = true;
        }

        private void btnManual_Click(object sender, EventArgs e)
        {
            Sine = false;
            btnSine.Enabled = true;
            btnManual.Enabled = false;
        }

        private void numUDSineFrec_ValueChanged(object sender, EventArgs e)
        {
            frec = (double)numUDSineFrec.Value;
        }

        private void savePositionDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            dataFile=saveFileDialog1.FileName;
        }

        private void btnStandar_Click(object sender, EventArgs e)
        {
            Sine = false;
            Standar = true;
            btnSine.Enabled = true;
            btnManual.Enabled = true;
            btnStandar.Enabled = false;

        }

        #region Control Signal
        private void openControlSignalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog2.ShowDialog();
        }

        private void openFileDialog2_FileOk(object sender, CancelEventArgs e)
        {
            csFile = openFileDialog2.FileName;
        }

        private double[,] unpackControlSignal(string file)
        {
            csvReader = new StreamReader(file);
            string[] strData = csvReader.ReadToEnd().Split('\n');
            int rows = strData.Length, cols = strData[0].Split(',').Length;
            double[,] Data = new double[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                string[] strRow = strData[i].Split(',');
                if (strRow.Length > 1)
                    for (int j = 0; j < cols; j++)
                        if (strRow[j] != "")
                            Data[i, j] = double.Parse(strRow[j]);
            }
            csvReader.Close();
            return Data;
        }

        #endregion 

    }
}
