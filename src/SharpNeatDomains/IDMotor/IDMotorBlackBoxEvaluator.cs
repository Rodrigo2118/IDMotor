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
using System.Diagnostics;
using SharpNeat.Core;
using SharpNeat.Phenomes;
using System;



namespace SharpNeat.Domains.IDMotor
{
    /// <summary>
    /// A black box evaluator for the XOR logic gate problem domain. 
    /// 
    /// XOR (also known as Exclusive OR) is a type of logical disjunction on two operands that results in
    /// a value of true if and only if exactly one of the operands has a value of 'true'. A simple way 
    /// to state this is 'one or the other but not both.'.
    /// 
    /// This evaluator therefore requires that the black box to be evaluated has two inputs and one 
    /// output all using the range 0..1
    /// 
    /// In turn each of the four possible test cases are applied to the two inputs, the network is activated
    /// and the output is evaluated. If a 'false' response is required we expect an output of zero, for true
    /// we expect a 1.0. Fitness for each test case is the difference between the output and the wrong output, 
    /// thus a maximum of 1 can be scored on each test case giving a maximum of 4. In addition each outputs is
    /// compared against a threshold of 0.5, if all four outputs are on the correct side of the threshold then
    /// 10.0 is added to the total fitness. Therefore a black box that answers correctly but very close to the
    /// threshold will score just above 10, and a black box that answers correctly with perfect 0.0 and 1.0 
    /// answers will score a maximum of 14.0.
    /// 
    /// The first type of evaluation punishes for difference from the required outputs and therefore represents
    /// a smooth fitness space (we can evolve gradually towards better scores). The +10 score for 4 correct
    /// responses is 'all or nothing', in other words it is a fitness space with a large step and no indication
    /// of where the step is, which on it's own would be a poor fitness space as it required evolution to stumble
    /// on the correct network by random rather than ascending a gradient in the fitness space. If however we do 
    /// stumble on a black box that answers correctly but close to the threshold, then we would like that box to 
    /// obtain a higher score than a network with, say, 3 strong correct responses and but wrong overall. We can
    /// improve the correct box's output difference from threshold value gradually, while the box with 3 correct
    /// responses may actually be in the wrong area of the fitness space altogether - in the wrong 'ballpark'.
    /// </summary>
    public class IDMotorBlackBoxEvaluator : IPhenomeEvaluator<IBlackBox>
    {
        static double[][,] _Data;
        static int[] _rows;
        static double StopFitness;
        static double _MaxFitness;
        double initialCondition;
        ulong _evalCount;
        bool _stopConditionSatisfied=false;
        static double tol = 0.001;
        static double kp = 1, kv = 0.2;
        static Random rt;

        #region IPhenomeEvaluator<IBlackBox> Members

        /// <summary>
        /// Gets the total number of evaluations that have been performed.
        /// </summary>
        public ulong EvaluationCount
        {
            get { return _evalCount; }
        }

        /// <summary>
        /// Gets a value indicating whether some goal fitness has been achieved and that
        /// the evolutionary algorithm/search should stop. This property's value can remain false
        /// to allow the algorithm to run indefinitely.
        /// </summary>
        public bool StopConditionSatisfied
        {
            get { return _stopConditionSatisfied; }
        }


        /// <summary>
        /// Evaluate the provided IBlackBox against the IDMotor problem domain and return its fitness score.
        /// </summary>
        public FitnessInfo Evaluate(IBlackBox box)
        {
            if (_evalCount == 0)
                Reset();
            double ePAgg = 0, eVAgg = 0;
            double fitness = 0;
            double afitness = 0;
            double[] error;
            double output;
            double[] pos;
            double[] posD;
            double[] h;
            ISignalArray inputArr = box.InputSignalArray;
            ISignalArray outputArr = box.OutputSignalArray;
            //


            _evalCount++;
            /*
            #region Null test
            box.ResetState();
            rt = new Random();
            output = 0;
            for (int i =0; i <100;i++)
            {
                inputArr[0] = rt.NextDouble();
                inputArr[1] = 0;
                if (i==0)
                    inputArr[2] = 0;
                else
                    inputArr[2] = output;
                box.Activate();
                output = outputArr[0];
                ePAgg += Math.Abs(output);
            }

            #endregion
            */
            for (int k = 0; k < _Data.GetLength(0); k++)
            {
                error = new double[_rows[k]];
                pos = new double[_rows[k]];
                posD = new double[_rows[k]];
                h = new double[_rows[k]];
                initialCondition = _Data[k][0, 2];
                //----- First Test 
                box.ResetState();

                // Set the input values
                inputArr[0] = _Data[k][0, 0];
                h[0] = inputArr[0];
                inputArr[1] = _Data[k][0, 1];
                inputArr[2] = initialCondition;
                posD[0] = _Data[k][0, 2];

                //box.OutputSignalArray[0] = initialCondition;

                // Activate the black box.
                box.Activate();
                if (!box.IsStateValid)
                {   // Any black box that gets itself into an invalid state is unlikely to be
                    // any good, so lets just bail out here.
                    return FitnessInfo.Zero;
                }

                // Read output signal.
                output = outputArr[0];
                pos[0] = output;

                // Calculate this test case's contribution to the overall fitness score.

                error[0] = _Data[k][0, 2] - output;
                if (System.Math.Abs(error[0]) > tol)
                    ePAgg += System.Math.Abs(error[0]);

                //----- Rest of the Tests

                // Set the input values
                for (int i = 1; i < _rows[k]; i++)
                {
                    inputArr[0] = _Data[k][i, 0] - _Data[k][i - 1, 0];
                    h[i] = inputArr[0];
                    inputArr[1] = _Data[k][i, 1];
                    inputArr[2] = output;
                    posD[i] = _Data[k][i, 2];

                    // Activate the black box.
                    box.Activate();
                    if (!box.IsStateValid)
                    {   // Any black box that gets itself into an invalid state is unlikely to be
                        // any good, so lets just bail out here.
                        return FitnessInfo.Zero;
                    }

                    // Read output signal.
                    output = outputArr[0];
                    pos[i] = output;

                    // Calculate this test case's contribution to the overall fitness score.
                    error[i] = _Data[k][i, 2] - output;
                    if (System.Math.Abs(error[i]) > tol)
                        ePAgg += System.Math.Abs(error[i]);
                        /*
                    if (error[i] * error[i] > tol)
                        ePAgg += error[i] * error[i];*/
                }

                double[] vel = IDMotorUtils.Derivate(pos, h);
                double[] velD = IDMotorUtils.Derivate(posD, h);

                for (int i = 0; i < _rows[k]; i++)
                    eVAgg += System.Math.Abs(velD[i] - vel[i]);
            }
            afitness = _MaxFitness - eVAgg;
            fitness = _MaxFitness - kp*ePAgg - kv*eVAgg;

            /*if (fitness >= StopFitness) {
                _stopConditionSatisfied = true;
            }
            */
            _stopConditionSatisfied = fitness >= StopFitness;
            if (fitness >= 0 && afitness>=0)
                return new FitnessInfo(fitness, afitness);

            return FitnessInfo.Zero;
        }
        /// <summary>
        /// Reset the internal state of the evaluation scheme if any exists.
        /// Note. The XOR problem domain has no internal state. This method does nothing.
        /// </summary>
        public void Reset()
        {
            IDMotorUtils.SetCSVData(false);

            _Data = IDMotorUtils.GetData;
            _rows = new int[_Data.GetLength(0)];
            for (int i = 0; i < _Data.GetLength(0); i++)
                _rows[i] = _Data[i].GetLength(0);
            _MaxFitness = 2 * IDMotorUtils.GetMaxFitness;
            StopFitness = _MaxFitness * (1 - tol);
        }

        #endregion

       
    }

}
