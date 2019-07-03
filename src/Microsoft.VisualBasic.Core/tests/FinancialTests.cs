// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Xunit;

namespace Microsoft.VisualBasic.Tests
{
    public class FinancialTests
    {
        private const int Precision = 15;

        [Theory]
        [InlineData(0, 1.0, 1.0, 1.0, 1.0, 0)]
        [InlineData(2000.0, 500.0, 2.0, 1.0, 2.0, 1500.0)]
        public void DDB(double Cost, double Salvage, double Life, double Period, double Factor, double expected)
        {
            Assert.Equal(expected, Financial.DDB(Cost, Salvage, Life, Period, Factor), Precision);
        }

        [Theory]
        [InlineData(0, 0, 0, 0, DueDate.EndOfPeriod, 0)]
        [InlineData(0.02 / 12, 12.0, -100.0, -100.0, DueDate.BegOfPeriod, 1315.0982120264073)]
        public void FV(double Rate, double NPer, double Pmt, double PV, DueDate Due, double expected)
        {
            Assert.Equal(expected, Financial.FV(Rate, NPer, Pmt, PV, Due), Precision);
        }

        [Theory]
        [InlineData(0, 1.0, 1.0, 0, 0, DueDate.EndOfPeriod, 0)]
        [InlineData(0.1 / 12, 12.0, 48.0, -20000.0, 0, DueDate.BegOfPeriod, 133.00409235108953)]
        public void IPmt(double Rate, double Per, double NPer, double PV, double FV, DueDate Due, double expected)
        {
            Assert.Equal(expected, Financial.IPmt(Rate, Per, NPer, PV, FV, Due), Precision);
        }

        [Theory]
        [InlineData(new double[] { -1, 1 }, 0, 0)]
        [InlineData(new double[] { -70000.0, 22000.0, 25000.0, 28000.0, 31000.0 }, 0.1, 0.177435884421108)]
        public void IRR(double[] ValueArray, double Guess, double expected)
        {
            Assert.Equal(expected, Financial.IRR(ref ValueArray, Guess), Precision);
        }

        [Theory]
        [InlineData(new double[] { -1, 1 }, 0, 0, 0)]
        [InlineData(new double[] { -70000.0, 22000.0, 25000.0, 28000.0, 31000.0 }, 0.1, 0.12, 0.15512706281927668)]
        public void MIRR(double[] ValueArray, double FinanceRate, double ReinvestRate, double expected)
        {
            Assert.Equal(expected, Financial.MIRR(ref ValueArray, FinanceRate, ReinvestRate), Precision);
        }

        [Theory]
        [InlineData(0, 1.0, 0, 0, DueDate.EndOfPeriod, 0)]
        [InlineData(0.02 / 12, -800.0, 10000, 0, DueDate.BegOfPeriod, 12.621310788105905)]
        public void NPer(double Rate, double Pmt, double PV, double FV, DueDate Due, double expected)
        {
            Assert.Equal(expected, Financial.NPer(Rate, Pmt, PV, FV, Due), Precision);
        }

        [Theory]
        [InlineData(0, new double[] { 0 }, 0)]
        [InlineData(0.1, new double[] { -70000.0, 22000.0, 25000.0, 28000.0, 31000.0 }, 11701.262333049774)]
        public void NPV(double Rate, double[] ValueArray, double expected)
        {
            Assert.Equal(expected, Financial.NPV(Rate, ref ValueArray), Precision);
        }

        [Theory]
        [InlineData(0, 1.0, 0, 0, DueDate.EndOfPeriod, 0)]
        [InlineData(0.02 / 12, 24, -10000, 0, DueDate.BegOfPeriod, 424.6948090031214)]
        public void Pmt(double Rate, double NPer, double PV, double FV, DueDate Due, double expected)
        {
            Assert.Equal(expected, Financial.Pmt(Rate, NPer, PV, FV, Due), Precision);
        }

        [Theory]
        [InlineData(0, 1.0, 1.0, 0, 0, DueDate.EndOfPeriod, 0)]
        [InlineData(0.02 / 12, 1.0, 24, -10000, 0, DueDate.BegOfPeriod, 424.6948090031214)]
        public void PPmt(double Rate, double Per, double NPer, double PV, double FV, DueDate Due, double expected)
        {
            Assert.Equal(expected, Financial.PPmt(Rate, Per, NPer, PV, FV, Due), Precision);
        }

        [Theory]
        [InlineData(0, 0, 0, 0, DueDate.EndOfPeriod, 0)]
        [InlineData(0.02 / 12, 12.0, -100.0, -100.0, DueDate.BegOfPeriod, 1287.1004825212165)]
        public void PV(double Rate, double NPer, double Pmt, double FV, DueDate Due, double expected)
        {
            Assert.Equal(expected, Financial.PV(Rate, NPer, Pmt, FV, Due), Precision);
        }

        [Theory]
        [InlineData(1.0, 1.0, 1.0, 0, DueDate.EndOfPeriod, 0, -2)]
        [InlineData(24.0, -800.0, 10000.0, 0.0, DueDate.BegOfPeriod, 0.1, 0.06767027865651142)]
        public void Rate(double NPer, double Pmt, double PV, double FV, DueDate Due, double Guess, double expected)
        {
            Assert.Equal(expected, Financial.Rate(NPer, Pmt, PV, FV, Due, Guess), Precision);
        }

        [Theory]
        [InlineData(1.0, 1.0, 1.0, 0)]
        [InlineData(1000.0, 800.0, 50.0, 4.0)]
        public void SLN(double Cost, double Salvage, double Life, double expected)
        {
            Assert.Equal(expected, Financial.SLN(Cost, Salvage, Life), Precision);
        }

        [Theory]
        [InlineData(1.0, 1.0, 1.0, 1.0, 0)]
        [InlineData(1000.0, 800.0, 50, 2.0, 7.686274509803922)]
        public void SYD(double Cost, double Salvage, double Life, double Period, double expected)
        {
            Assert.Equal(expected, Financial.SYD(Cost, Salvage, Life, Period), Precision);
        }
    }
}
