using System.IO;
using System.Windows.Forms;
using System.Linq.Expressions;

namespace SharpNeat.Domains.IDMotor
{
    public static class IDMotorUtils
    {
        public static string[] _datafile = new string[] { "expData.csv" };
        public static double[][,] _Data;
        public static double _MaxFitness = 0;
        public static string _performance;

        #region Misc
        public static double[] [,] GetData
        {
            get{ return _Data; }
        }

        public static double GetMaxFitness
        {
            get { return _MaxFitness; }
        }

        public static void SetCSVData( bool norm)
        {
            
            double[][,] Data = new double[_datafile.Length][,];//[rows, cols];
            //_Data = new double[_datafile.Length][,];
            for (int k=0;k<_datafile.Length;k++)
            {
                StreamReader csvReader = new StreamReader(_datafile[k]);
                string[] strData = csvReader.ReadToEnd().Split('\n');
                int rows = strData.Length, cols = strData[0].Split(',').Length;
                Data[k] = new double[rows, cols];
                for (int i = 0; i < rows; i++)
                {
                    string[] strRow = strData[i].Split(',');
                    if (strRow.Length > 1)
                        for (int j = 0; j < cols; j++)
                            if (strRow[j] != "")
                                Data[k][i, j] = double.Parse(strRow[j]);
                }
                csvReader.Close();
            }
            if (norm)
                _Data[0] = NormalizeData(Data[0]); //MapTo1(Data);
            else
                _Data = Data;

            SetMaxFitness();
        }

        private static double[,] NormalizeData(double[,] Data)
        {
            int rows=Data.GetLength(0), cols=Data.GetLength(1);
            double[] Norms= new double[cols];

            for (int i=0; i<rows; i++)
                for(int j=1;j<cols; j++)
                    Norms[j] += Data[i, j];
            for (int i = 0; i < rows; i++)
                for (int j = 1; j < cols; j++)
                    Data[i,j] = Data[i, j] / Norms[j];

            return Data;
        }

        private static double[,] MapTo1(double[,] Data)
        {
            int rows = Data.GetLength(0), cols = Data.GetLength(1);
            double[,] MapData = new double[rows, cols];
            double[] MaxVal = new double[] { 1, 1, 1 };
            double[] MinVal = new double[] { 1, 1, 1 };

            for (int i = 0; i < rows; i++)
                for (int j = 1; j < cols; j++)
                {
                    if (Data[i, j] > MaxVal[j])
                        MaxVal[j] = Data[i, j];
                    if (Data[i, j] < MinVal[j])
                        MinVal[j] = Data[i, j];
                }

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    if (j == 0)
                        MapData[i, j] = Data[i, j];
                    else
                        MapData[i, j] = (Data[i, j] - MinVal[j]) / (MaxVal[j] - MinVal[j]);

            return MapData;
        }

        public static double[] Derivate(double[] pos,double[] h)
        {
            int rows = pos.Length;
            double[] vel = new double[rows];
            vel[0] = (pos[1] - pos[0]) / h[1];
            for (int i = 1; i < pos.Length - 1; i++)
                vel[i] = (pos[i + 1] - pos[i - 1]) / (h[i]+h[i+1]);
            vel[rows - 1] = (pos[rows - 1] - pos[rows - 2]) / h[rows-1];
            return vel;
        }

        public static string SetData(string[] file)
        {
            _datafile = new string[file.Length];
            for (int i =0; i<file.Length;i++)
                _datafile[i] = file[i];

            return _datafile[0];
        }

        public static string SetData()
        {
            return _datafile[0];
        }

        public static void SetMaxFitness()
        {
            _MaxFitness = 0;
            for (int k = 0; k < _Data.GetLength(0);k++) {
                int rows = _Data[k].GetLength(0), cols = _Data[k].GetLength(1);
                for (int i = 0; i < rows; i++)
                    _MaxFitness += System.Math.Abs(_Data[k][i, 2]);
            }
        }

       
        #endregion
    }
}