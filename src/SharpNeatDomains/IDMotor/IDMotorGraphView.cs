/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2016 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using System;
using System.Drawing;
using SharpNeat.Core;
using SharpNeat.Genomes.Neat;
using SharpNeat.Phenomes;
using ZedGraph;
using System.IO;

namespace SharpNeat.Domains.IDMotor
{
    /// <summary>
    /// Domain View for function regression with one input and one output.
    /// Plots function on 2D graph.
    /// </summary>
    public partial class IDMotorGraphView : AbstractDomainView
    {
        //static double[,] Data = IDMotorUtils.GetCSVData("Entrenamiento.csv",false);
        static double[,] _Data;
        readonly double[] _yArrTarget;
        static double tol = 0.001;
        IGenomeDecoder<NeatGenome,IBlackBox> _genomeDecoder;
        PointPairList _plotPointListTarget;
        PointPairList _plotPointListResponse;
        PointPairList _plotPointListError;
        PointPairList _plotPointErrorSum;

        static PointPairList _splotPointListTarget;
        static PointPairList _splotPointListResponse;
        static PointPairList _splotPointListError;
        static PointPairList _splotPointErrorSum;

        #region Constructor

        /// <summary>
        /// Constructs with the details of the function regression problem to be visualized. 
        /// </summary>
        /// <param name="fn">The function being regressed.</param>
        /// <param name="generativeMode">Indicates that blacbox has no inputs; it will generate a waveform as a function of time.</param>
        /// <param name="paramSamplingInfo">Parameter sampling info.</param>
        /// <param name="genomeDecoder">Genome decoder.</param>
        public IDMotorGraphView(IGenomeDecoder<NeatGenome,IBlackBox> genomeDecoder)
        {
            Random r = new Random();
            InitializeComponent();
            InitGraph(string.Empty, string.Empty, string.Empty);
            //IDMotorUtils.SetCSVData(false);
            double[] [,] Datas= IDMotorUtils.GetData;
            _Data = Datas[r.Next(Datas.GetLength(0))];

            _genomeDecoder = genomeDecoder;
            
            _yArrTarget = new double[_Data.GetLength(0)];
            double[] xArr = new double[_Data.GetLength(0)];
            for(int i=0;i<xArr.Length; i++)
            {
                xArr[i] = _Data[i,0];
                _yArrTarget[i] = _Data[i, 2];
            }

            // Pre-build plot point objects.
            _plotPointListTarget = new PointPairList();
            _plotPointListResponse = new PointPairList();
            _plotPointListError = new PointPairList();
            _plotPointErrorSum = new PointPairList();



            for (int i=0; i<xArr.Length; i++)
            {
                double x = xArr[i];
                _plotPointListTarget.Add(x, _yArrTarget[i]);
                _plotPointListResponse.Add(x, 0.0);
                _plotPointListError.Add(x, 0.0);
                _plotPointErrorSum.Add(x, 0.0);
            }

            // Bind plot points to graph.
            zed.GraphPane.AddCurve("Target", _plotPointListTarget, Color.Black, SymbolType.None);
            zed.GraphPane.AddCurve("Network Response", _plotPointListResponse, Color.Blue, SymbolType.None);
            zed.GraphPane.AddCurve("Error", _plotPointListError, Color.Red, SymbolType.None);
            zed.GraphPane.AddCurve("Error Sum", _plotPointErrorSum, Color.OrangeRed, SymbolType.None);
        }

        #endregion

        #region Private Methods

        private void InitGraph(string title, string xAxisTitle, string yAxisTitle)
        {
            GraphPane graphPane = zed.GraphPane;
            graphPane.Title.Text = title;

			graphPane.XAxis.Title.Text = xAxisTitle;
			graphPane.XAxis.MajorGrid.IsVisible = true;

			graphPane.YAxis.Title.Text = yAxisTitle;
			graphPane.YAxis.MajorGrid.IsVisible = true;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Refresh/update the view with the provided genome.
        /// </summary>
        public override void RefreshView(object genome)
        {
            NeatGenome neatGenome = genome as NeatGenome;
            if(null == neatGenome) {
                return;
            }

            // Decode genome.
            IBlackBox box = _genomeDecoder.Decode(neatGenome);
            box.ResetState();

            // Probe the black box.
            //_blackBoxProbe.Probe(box, _yArrTarget);

            // Update plot points.
            double[] xArr = new double[_Data.GetLength(0)];
            for (int i = 0; i < xArr.Length; i++)
            {

                box.InputSignalArray[1] = _Data[i, 1];
                if (i == 0)
                {
                    box.InputSignalArray[0] = _Data[i, 0];
                    box.InputSignalArray[2] = _Data[i, 2];
                }
                else
                {
                    box.InputSignalArray[0] = _Data[i, 0] - _Data[i - 1, 0];
                    box.InputSignalArray[2] = box.OutputSignalArray[0];
                }
                box.Activate();
                //_plotPointListResponse[i].Y = _yArrTarget[i];
                _plotPointListResponse[i].Y = box.OutputSignalArray[0];
                _plotPointListError[i].Y = _plotPointListResponse[i].Y - _yArrTarget[i];
                if (System.Math.Abs(_plotPointListError[i].Y) < tol)
                    _plotPointListError[i].Y = 0;
                if (i != 0)
                    _plotPointErrorSum[i].Y = Math.Abs(_plotPointListError[i].Y) + _plotPointErrorSum[i - 1].Y;
            }
            _splotPointErrorSum = _plotPointErrorSum;
            _splotPointListResponse = _plotPointListResponse;
            _splotPointListError = _plotPointListError;
            // Trigger graph to redraw.
            zed.AxisChange();
            Refresh();
        }

        #endregion

        public static void SavePerformance(string file)
        {
            string performance = "";
            performance+="Tiempo,Valor esperado,Valor de la red,Error,Error Acumulado\n";
            for (int i = 0; i < _Data.GetLength(0); i++)
                performance+=_Data[i, 0] + "," +_Data[i,2] + "," + _splotPointListResponse[i].Y + "," + _splotPointListError[i].Y + "," + _splotPointErrorSum[i].Y+"\n";
            StreamWriter writer = new StreamWriter(file);
            writer.Write(performance);
            writer.Close();
        }

    }
}
