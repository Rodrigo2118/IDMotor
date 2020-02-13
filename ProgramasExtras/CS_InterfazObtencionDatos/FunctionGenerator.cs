using System;
using NationalInstruments.DAQmx;
using System.Diagnostics;

namespace NationalInstruments.Examples
{
    public enum WaveformType
    {
        SineWave = 0,
        ChirpWave = 1,
        PseudoRandomBinomial = 2
    }

	public class FunctionGenerator
	{
        public FunctionGenerator(
            Timing timingSubobject,
            string desiredFrequency,
            string samplesPerBuffer,
            string cyclesPerBuffer,
            string type,
            string amplitude)
        {
            WaveformType t = new WaveformType();
            t = WaveformType.SineWave;
            if (type == "Sine Wave")
                t = WaveformType.SineWave;
            else if (type == "Chirp Wave")
                t = WaveformType.ChirpWave;
            else if (type == "Pseudo Random Binomial")
                t = WaveformType.PseudoRandomBinomial;
            else
                Debug.Assert(false, "Invalid Waveform Type");

            Init(
                timingSubobject,
                Double.Parse(desiredFrequency),
                Double.Parse(samplesPerBuffer),
                Double.Parse(cyclesPerBuffer),
                t,
                Double.Parse(amplitude));
        }

        public FunctionGenerator(
            Timing timingSubobject,
            double desiredFrequency,
            double samplesPerBuffer,
            double cyclesPerBuffer,
            WaveformType type,
            double amplitude)
        {
            Init(
                timingSubobject,
                desiredFrequency,
                samplesPerBuffer,
                cyclesPerBuffer,
                type,
                amplitude);
        }

        private void Init(
		    Timing timingSubobject,
            double desiredFrequency,
            double samplesPerBuffer,
            double cyclesPerBuffer,
            WaveformType type,
            double amplitude)
		{
            if(desiredFrequency <= 0)
                throw new ArgumentOutOfRangeException("desiredFrequency",desiredFrequency,"This parameter must be a positive number");
            if(samplesPerBuffer <= 0)
                throw new ArgumentOutOfRangeException("samplesPerBuffer",samplesPerBuffer,"This parameter must be a positive number");
            if(cyclesPerBuffer <= 0)
                throw new ArgumentOutOfRangeException("cyclesPerBuffer",cyclesPerBuffer,"This parameter must be a positive number");

            // First configure the Task timing parameters
            if(timingSubobject.SampleTimingType == SampleTimingType.OnDemand)
                timingSubobject.SampleTimingType = SampleTimingType.SampleClock;

            _desiredSampleClockRate = (desiredFrequency * samplesPerBuffer) / cyclesPerBuffer;
            _samplesPerCycle = samplesPerBuffer / cyclesPerBuffer;

            // Determine the actual sample clock rate
            timingSubobject.SampleClockRate = _desiredSampleClockRate;
            _resultingSampleClockRate = timingSubobject.SampleClockRate;

            _resultingFrequency = _resultingSampleClockRate / (samplesPerBuffer / cyclesPerBuffer);

            switch(type)
            {
                case WaveformType.SineWave:
                    _data = GenerateSineWave(_resultingFrequency,amplitude,_resultingSampleClockRate,samplesPerBuffer);
                    break;
                case WaveformType.ChirpWave:
                    _data = GenerateChirpWave(0.01,1,10,0, amplitude, _resultingSampleClockRate, samplesPerBuffer);
                    break;
                case WaveformType.PseudoRandomBinomial:
                    _data = GeneratePseudoRandomBinomial(0.5, 50, amplitude, _resultingSampleClockRate, samplesPerBuffer);
                    break;
                default:
                    // Invalid type value
                    Debug.Assert(false);
                    break;
            }
        }

        public double[] Data
        {
            get
            {
                return _data;
            }
        }

        public double ResultingSampleClockRate
        {
            get
            {
                return _resultingSampleClockRate;
            }
        }

        public static double[] GenerateSineWave(
            double frequency, 
            double amplitude,
            double sampleClockRate,
            double samplesPerBuffer)
        {
            double deltaT = 1/sampleClockRate; // sec./samp
            int intSamplesPerBuffer = (int)samplesPerBuffer;

            double[] rVal = new double[intSamplesPerBuffer];

            for(int i=0;i<intSamplesPerBuffer;i++)
                rVal[i] = amplitude * Math.Sin( (2.0 * Math.PI) * frequency * (i*deltaT) );

            return rVal;
        }

        public static double[] GenerateChirpWave(
            double frequency_0,
            double frequency_1,
            double time2reach,
            double phase_0,
            double amplitude,
            double sampleClockRate,
            double samplesPerBuffer)
        {
            double deltaT = 1 / sampleClockRate; // sec./samp
            double c = (frequency_1 - frequency_0) / time2reach;
            int intSamplesPerBuffer = (int)samplesPerBuffer;

            double[] rVal = new double[intSamplesPerBuffer];

            for (int i = 0; i < intSamplesPerBuffer; i++)
            {
                double t = i * deltaT;
                rVal[i] = amplitude * Math.Sin(phase_0 + (2.0 * Math.PI) * (c / 2 * t * t + frequency_0 * t));
            }
            return rVal;
        }

        public static double[] GeneratePseudoRandomBinomial(
            double p0,
            int di,
            double amplitude,
            double sampleClockRate,
            double samplesPerBuffer)
        {
            Random random = new Random();
            double deltaT = 1 / sampleClockRate; // sec./samp
            int intSamplesPerBuffer = (int)samplesPerBuffer;

            double[] rVal = new double[intSamplesPerBuffer];

            for (int i = 0; i < intSamplesPerBuffer; i++)
                if(i%di==0)
                    rVal[i] = amplitude * Math.Sign(random.NextDouble()-p0);
                else
                    rVal[i] = rVal[i-1];

            return rVal;
        }

        public static void InitComboBox(System.Windows.Forms.ComboBox box)
        {
            box.Items.Clear();
            box.Items.AddRange(new object[] {
                "Sine Wave", "Chirp Wave", "Pseudo Random Binomial"});
            box.Sorted = false;
            box.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            box.Text = "Chirp Wave";
        }

        private double[] _data;
        private double _resultingSampleClockRate;
        private double _resultingFrequency;
        private double _desiredSampleClockRate;
        private double _samplesPerCycle;
    }
}
