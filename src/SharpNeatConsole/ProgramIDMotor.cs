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
using System.Collections.Generic;
using System.IO;
using System.Xml;
using log4net.Config;
using SharpNeat.Core;
using SharpNeat.Domains.IDMotor;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;
using SharpNeat.Phenomes;

namespace SharpNeatConsole
{
    /// <summary>
    /// Minimal console application that hardwires the setting up on a evolution algorithm and start it running.
    /// </summary>
    class Program
    {
        static IGenomeFactory<NeatGenome> _genomeFactory;
        static List<NeatGenome> _genomeList;
        static NeatEvolutionAlgorithm<NeatGenome> _ea;
        static XmlDocument xmlConfig = new XmlDocument();
        static string DataFolder = "C:\\Users\\Rodrigo\\Desktop\\Tesis\\DAQ\\Datos_Para_Entrenamiento\\";
        static string LogFolder = "C:\\Users\\Rodrigo\\Desktop\\Tesis\\DAQ\\Datos_Para_Entrenamiento\\Logs\\";
        static string GenomeFolder = "C:\\Users\\Rodrigo\\Desktop\\Tesis\\DAQ\\Datos_Para_Entrenamiento\\Genomas\\";
        static string PerformanceFolder = "C:\\Users\\Rodrigo\\Desktop\\Tesis\\DAQ\\Datos_Para_Entrenamiento\\Desempeño\\";
        static string ValidationFolder = "C:\\Users\\Rodrigo\\Desktop\\Tesis\\DAQ\\Datos_Para_Entrenamiento\\Genomas\\Validacion\\";
        static string[][] safeNames;
        static string[][] dataFiles;
        static DateTime T0;
        static DateTime now;
        static bool next = false;
        static int MaxTMin = 40;
        static int i;
        static string NameFile;
        static double[,] ControlSignal;
        static string csFile = "C:\\Users\\Rodrigo\\Desktop\\Tesis\\DAQ\\Datos_Para_Entrenamiento\\Genomas\\Validacion\\ControlSignal.csv";
        static string safeNamesFile = "C:\\Users\\Rodrigo\\Desktop\\Tesis\\DAQ\\Datos_Para_Entrenamiento\\Entrenamientos.fls";
        static string genomeNameFile = "C:\\Users\\Rodrigo\\Desktop\\Tesis\\DAQ\\Datos_Para_Entrenamiento\\Genomas\\gnm_filename.txt";



        static void Main(string[] args)
        {
            for (; ; )
            {
                string[] cmdArgs = Console.ReadLine().Split(' ');

                ControlSignal = unpackControlSignal(csFile);

                switch (cmdArgs[0])
                {
                    case "Evolve":
                        {
                            if (cmdArgs.Length == 2)
                                safeNamesFile = cmdArgs[1];

                            SetSafeNames();

                            SetDataFiles();
                            for (i = 0; i < dataFiles.GetLength(0); i++)
                            {

                                SetCurrentNameFile();

                                // Initialise log4net (log to console).
                                XmlConfigurator.Configure(new FileInfo("log4net.properties"));

                                // Experiment classes encapsulate much of the nuts and bolts of setting up a NEAT search.
                                IDMotorExperiment experiment = new IDMotorExperiment();

                                //Set training datasets 
                                IDMotorUtils.SetData(dataFiles[i]);

                                // Load config XML.
                                xmlConfig.Load("IDMotor.config.xml");
                                experiment.Initialize("IDMotor", xmlConfig.DocumentElement);

                                // Create a genome factory with our neat genome parameters object and the appropriate number of input and output neuron genes.
                                _genomeFactory = experiment.CreateGenomeFactory();

                                // Create an initial population of randomly generated genomes.
                                _genomeList = _genomeFactory.CreateGenomeList(1000, 0);

                                // Create evolution algorithm and attach update event.
                                _ea = experiment.CreateEvolutionAlgorithm(_genomeFactory, _genomeList);
                                _ea.UpdateEvent += new EventHandler(ea_UpdateEvent);

                                // Start algorithm (it will run on a background thread).
                                T0 = DateTime.Now;
                                now = T0;
                                next = false;
                                _ea.StartContinue();

                                while (!(((now - T0).Minutes >= MaxTMin)) || next)
                                    now = DateTime.Now;



                                StopNReturn(experiment);
                            }
                            break;
                        }

                    case "Validate":
                        {
                            if (cmdArgs.Length == 2)
                                genomeNameFile = cmdArgs[1];

                            StreamReader gnmReader = new StreamReader(genomeNameFile);
                            string[] gnmfiles = gnmReader.ReadToEnd().Split('\n');
                            gnmReader.Close();

                            IDMotorExperiment _experiment = new IDMotorExperiment();
                            xmlConfig = new XmlDocument();
                            xmlConfig.Load("IDMotor.config.xml");
                            _experiment.Initialize("IDMotor", xmlConfig.DocumentElement);

                            for (i = 0; i < gnmfiles.GetLength(0); i++)
                            {
                                NameFile = gnmfiles[i].Replace(".gnm.xml", "").Replace("\r", "");

                                Console.WriteLine("Validating {0}...", NameFile);

                                XmlReader xr = XmlReader.Create(GenomeFolder + NameFile + ".gnm.xml");
                                _genomeList = _experiment.LoadPopulation(xr);
                                IGenomeDecoder<NeatGenome, IBlackBox> _genomeDecoder = _experiment.CreateGenomeDecoder();
                                IBlackBox nn = _genomeDecoder.Decode(_genomeList[0]);


                                SaveValidation(nn);

                                Console.WriteLine("Done.");
                            }
                            break;

                        }
                    default:
                        Console.WriteLine("Error, Evolve or Validate only.");
                        break;
                }
                // Hit return to quit.
                Console.ReadLine();
            }
        }

        static void ea_UpdateEvent(object sender, EventArgs e)
        {
            StreamWriter logWriter = new StreamWriter(LogFolder + NameFile + "_" + MaxTMin + "m" + ".log", true);
            Console.WriteLine(string.Format("gen={0:N0} bestFitness={1:N6}", _ea.CurrentGeneration, _ea.Statistics._maxFitness));
            NeatAlgorithmStats stats = _ea.Statistics;
            now = DateTime.Now;
            logWriter.Flush();
            logWriter.WriteLine("{0:yyyy-MM-dd HH:mm:ss.fff},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                                                            now,
                                                            stats._generation,
                                                            stats._maxFitness,
                                                            stats._meanFitness,
                                                            stats._meanSpecieChampFitness,
                                                            _ea.CurrentChampGenome.Complexity,
                                                            stats._meanComplexity,
                                                            stats._maxComplexity,
                                                            stats._totalEvaluationCount,
                                                            stats._evaluationsPerSec,
                                                            _ea.ComplexityRegulationMode);
            if (_ea.StopConditionSatisfied == true)
            {
                Console.WriteLine("Stop condition staisfied.");
                logWriter.WriteLine("Stop condition satisfied.");
                next = true;
            }
            logWriter.Close();
            logWriter.Dispose();
            /*if ((now - T0).Seconds%60 == 1)
                Console.WriteLine(now);
            
            if ((now - T0).Minutes >= 20)
                next = true;
            */
        }

        static void StopNReturn(IDMotorExperiment experiment)
        {

            _ea.RequestTerminateAndWait();

            // Save performance data
            IGenomeDecoder<NeatGenome, IBlackBox> deco = experiment.CreateGenomeDecoder();
            IBlackBox nn = deco.Decode(_ea.CurrentChampGenome);
            SavePerformance(nn);

            SaveValidation(nn);

            // Save genome to xml file.
            XmlWriterSettings xwSettings = new XmlWriterSettings();
            xwSettings.Indent = true;
            using (XmlWriter xw = XmlWriter.Create(GenomeFolder + NameFile + "_" + MaxTMin + "m" + ".gnm.xml", xwSettings))
            {
                experiment.SavePopulation(xw, new NeatGenome[] { _ea.CurrentChampGenome });
            }

            Console.WriteLine(NameFile + "finished. Continuing to the next data set => \n");

           

        }

        static void SavePerformance(IBlackBox box)
        {
            #region Performance
            double tol = 0.001;
            Random r = new Random();
            double[][,] Datas = IDMotorUtils.GetData;
            double[,] _Data = Datas[r.Next(Datas.GetLength(0))];

            box.ResetState();
            double[] xArr = new double[_Data.GetLength(0)];
            StreamWriter prfWriter = new StreamWriter(PerformanceFolder + NameFile + "_" + MaxTMin + "m" + "_prf.csv");
            for (int j = 0; j < xArr.Length; j++)
            {
                box.InputSignalArray[1] = _Data[j, 1];
                if (j == 0)
                {
                    box.InputSignalArray[0] = _Data[j, 0];
                    box.InputSignalArray[2] = _Data[j, 2];
                }
                else
                {
                    box.InputSignalArray[0] = _Data[j, 0] - _Data[j - 1, 0];
                    box.InputSignalArray[2] = box.OutputSignalArray[0];
                }
                box.Activate();
                //_plotPointListResponse[i].Y = _yArrTarget[i];
                double output = box.OutputSignalArray[0];
                double error = box.OutputSignalArray[0] - _Data[j, 2];
                if (System.Math.Abs(error) < tol)
                    error = 0;

                prfWriter.WriteLine("{0},{1},{2},{3}", _Data[j, 0], _Data[j, 2], output, error);

            }
            prfWriter.Close();
            prfWriter.Dispose();
            #endregion

            
        }

        static void SaveValidation(IBlackBox box)
        {
            #region Validation


            int n = ControlSignal.GetLength(0);
            double Dt = 0;
            double desiredPos;
            double SatV = 9;
            box.ResetState();

            double actualPos = 0;
            StreamWriter ValWriter = new StreamWriter(ValidationFolder + NameFile + "_" + MaxTMin + "m" + "_val.csv");
            for (int j = 0; j < n; j++)
            {
                if (j != 0)
                    Dt = ControlSignal[j, 0] - ControlSignal[j - 1, 0];

                desiredPos = ControlSignal[j, 1];

                double ctrlV = desiredPos - actualPos;

                if (Math.Abs(ctrlV) > SatV)
                    ctrlV = SatV * Math.Sign(ctrlV);

                box.InputSignalArray[0] = Dt;
                box.InputSignalArray[1] = ctrlV;
                box.InputSignalArray[2] = actualPos;

                box.Activate();

                actualPos = box.OutputSignalArray[0];
                ValWriter.Flush();
                ValWriter.WriteLine("{0},{1}", ControlSignal[j, 0], actualPos);
            }
            ValWriter.Close();

            #endregion
        }

        static void SetDataFiles()
        {
            int n = safeNames.GetLength(0);
            dataFiles = new string[n][];
            for (i = 0; i < n; i++)
            {
                int m = safeNames[i].GetLength(0);
                dataFiles[i] = new string[m];
                for (int j = 0; j < m; j++)
                    dataFiles[i][j] = DataFolder + safeNames[i][j] + ".csv";
            }
        }

        static void SetCurrentNameFile()
        {
            int m = safeNames[i].GetLength(0);
            NameFile = "_";
            for (int j = 0; j < m; j++)
                NameFile += safeNames[i][j] + "_";
        }

        static double[,] unpackControlSignal(string file)
        {
            StreamReader csvReader = new StreamReader(file);
            string[] strData = csvReader.ReadToEnd().Split('\n');
            int rows = strData.Length-1, cols = strData[0].Split(',').Length;
            double[,] Data = new double[rows, cols];

            for (int k = 0; k < rows; k++)
            {
                string[] strRow = strData[k].Split(',');
                if (strRow.Length > 1)
                    for (int j = 0; j < cols; j++)
                        if (strRow[j] != "")
                            Data[k, j] = double.Parse(strRow[j]);
            }
            csvReader.Close();
            return Data;
        }

        static void SetSafeNames()
        {
            StreamReader snReader = new StreamReader(safeNamesFile);
            string[] strData = snReader.ReadToEnd().Split('\n');
            int sets = strData.GetLength(0);
            safeNames = new string[sets][];
            for (int j = 0; j < sets; j++)
                safeNames[j] = strData[j].Replace("\r","").Replace(".csv","").Split(',');
            snReader.Close();
        }
    }
}
