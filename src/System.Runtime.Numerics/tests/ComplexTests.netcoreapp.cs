// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;

using Xunit;

namespace System.Numerics.Tests
{
    public partial class ComplexTests
    {
        [Fact]
        public static void NaN()
        {
            var complexNaN = new Complex(double.NaN, double.NaN);

            Assert.True(Complex.IsNaN(complexNaN));
            Assert.Equal(complexNaN, Complex.NaN);
            Assert.Equal(double.NaN, Complex.NaN.Real);
            Assert.Equal(double.NaN, Complex.NaN.Imaginary);
            VerifyRealImaginaryProperties(Complex.NaN, double.NaN, double.NaN);
            VerifyMagnitudePhaseProperties(Complex.NaN, double.NaN, double.NaN);
        }

        [Fact]
        public static void Infinity()
        {
            var complexInfinity = new Complex(double.PositiveInfinity, double.PositiveInfinity);

            Assert.True(Complex.IsInfinity(complexInfinity));
            Assert.False(Complex.IsFinite(complexInfinity));
            Assert.Equal(complexInfinity, Complex.Infinity);
            Assert.Equal(double.PositiveInfinity, Complex.Infinity.Real);
            Assert.Equal(double.PositiveInfinity, Complex.Infinity.Imaginary);
            VerifyRealImaginaryProperties(Complex.Infinity, double.PositiveInfinity, double.PositiveInfinity);
            VerifyMagnitudePhaseProperties(Complex.Infinity, double.PositiveInfinity, Math.PI / 4);

            Assert.True(Complex.IsFinite(Complex.ImaginaryOne));
            Assert.False(Complex.IsInfinity(new Complex(double.PositiveInfinity, 12)));
            Assert.False(Complex.IsInfinity(new Complex(12, double.PositiveInfinity)));
        }
    }
}
