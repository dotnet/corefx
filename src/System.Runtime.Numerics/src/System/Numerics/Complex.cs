// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace System.Numerics
{
    /// <summary>
    /// A complex number z is a number of the form z = x + yi, where x and y 
    /// are real numbers, and i is the imaginary unit, with the property i2= -1.
    /// </summary>
    [Serializable]
    public struct Complex : IEquatable<Complex>, IFormattable
    {
        public static readonly Complex Zero = new Complex(0.0, 0.0);
        public static readonly Complex One = new Complex(1.0, 0.0);
        public static readonly Complex ImaginaryOne = new Complex(0.0, 1.0);

        private const double InverseOfLog10 = 0.43429448190325; // 1 / Log(10)

        // This is the largest x for which (Hypot(x,x) + x) will not overflow. It is used for branching inside Sqrt.
        private static readonly double s_sqrtRescaleThreshold = double.MaxValue / (Math.Sqrt(2.0) + 1.0);

        // This is the largest x for which 2 x^2 will not overflow. It is used for branching inside Asin and Acos.
        private static readonly double s_asinOverflowThreshold = Math.Sqrt(double.MaxValue) / 2.0;

        // This value is used inside Asin and Acos.
        private static readonly double s_log2 = Math.Log(2.0);

        private double _real;
        private double _imaginary;
        
        public Complex(double real, double imaginary)
        {
            _real = real;
            _imaginary = imaginary;
        }

        public double Real { get { return _real; } }
        public double Imaginary { get { return _imaginary; } }

        public double Magnitude { get { return Abs(this); } }
        public double Phase { get { return Math.Atan2(_imaginary, _real); } }

        public static Complex FromPolarCoordinates(double magnitude, double phase)
        {
            return new Complex(magnitude * Math.Cos(phase), magnitude * Math.Sin(phase));
        }

        public static Complex Negate(Complex value)
        {
            return -value;
        }

        public static Complex Add(Complex left, Complex right)
        {
            return left + right;
        }

        public static Complex Subtract(Complex left, Complex right)
        {
            return left - right;
        }

        public static Complex Multiply(Complex left, Complex right)
        {
            return left * right;
        }

        public static Complex Divide(Complex dividend, Complex divisor)
        {
            return dividend / divisor;
        }
        
        public static Complex operator -(Complex value)  /* Unary negation of a complex number */
        {
            return new Complex(-value._real, -value._imaginary);
        }
        
        public static Complex operator +(Complex left, Complex right)
        {
            return new Complex(left._real + right._real, left._imaginary + right._imaginary);
        }

        public static Complex operator -(Complex left, Complex right)
        {
            return new Complex(left._real - right._real, left._imaginary - right._imaginary);
        }

        public static Complex operator *(Complex left, Complex right)
        {
            // Multiplication:  (a + bi)(c + di) = (ac -bd) + (bc + ad)i
            double result_Realpart = (left._real * right._real) - (left._imaginary * right._imaginary);
            double result_Imaginarypart = (left._imaginary * right._real) + (left._real * right._imaginary);
            return new Complex(result_Realpart, result_Imaginarypart);
        }

        public static Complex operator /(Complex left, Complex right)
        {
            // Division : Smith's formula.
            double a = left._real;
            double b = left._imaginary;
            double c = right._real;
            double d = right._imaginary;

            if (Math.Abs(d) < Math.Abs(c))
            {
                double doc = d / c;
                return new Complex((a + b * doc) / (c + d * doc), (b - a * doc) / (c + d * doc));
            }
            else
            {
                double cod = c / d;
                return new Complex((b + a * cod) / (d + c * cod), (-a + b * cod) / (d + c * cod));
            }
        }

        public static double Abs(Complex value)
        {
            return Hypot(value._real, value._imaginary);
        }

        private static double Hypot(double a, double b)
        {
            // Using
            //   sqrt(a^2 + b^2) = |a| * sqrt(1 + (b/a)^2)
            // we can factor out the larger component to dodge overflow even when a * a would overflow.

            a = Math.Abs(a);
            b = Math.Abs(b);

            double small, large;
            if (a < b)
            {
                small = a;
                large = b;
            }
            else
            {
                small = b;
                large = a;
            }

            if (small == 0.0)
            {
                return (large);
            }
            else if (double.IsPositiveInfinity(large) && !double.IsNaN(small))
            {
                // The NaN test is necessary so we don't return +inf when small=NaN and large=+inf.
                // NaN in any other place returns NaN without any special handling.
                return (double.PositiveInfinity);
            }
            else
            {
                double ratio = small / large;
                return (large * Math.Sqrt(1.0 + ratio * ratio));
            }

        }


        private static double Log1P(double x)
        {
            // Compute log(1 + x) without loss of accuracy when x is small.

            // Our only use case so far is for positive values, so this isn't coded to handle negative values.
            Debug.Assert((x >= 0.0) || double.IsNaN(x));

            double xp1 = 1.0 + x;
            if (xp1 == 1.0)
            {
                return x;
            }
            else if (x < 0.75)
            {
                // This is accurate to within 5 ulp with any floating-point system that uses a guard digit,
                // as proven in Theorem 4 of "What Every Computer Scientist Should Know About Floating-Point
                // Arithmetic" (https://docs.oracle.com/cd/E19957-01/806-3568/ncg_goldberg.html)
                return x * Math.Log(xp1) / (xp1 - 1.0);
            }
            else
            {
                return Math.Log(xp1);
            }

        }

        public static Complex Conjugate(Complex value)
        {
            // Conjugate of a Complex number: the conjugate of x+i*y is x-i*y
            return new Complex(value._real, -value._imaginary);
        }

        public static Complex Reciprocal(Complex value)
        {
            // Reciprocal of a Complex number : the reciprocal of x+i*y is 1/(x+i*y)
            if (value._real == 0 && value._imaginary == 0)
            {
                return Zero;
            }
            return One / value;
        }
        
        public static bool operator ==(Complex left, Complex right)
        {
            return left._real == right._real && left._imaginary == right._imaginary;
        }

        public static bool operator !=(Complex left, Complex right)
        {
            return left._real != right._real || left._imaginary != right._imaginary;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Complex)) return false;
            return Equals((Complex)obj);
        }

        public bool Equals(Complex value)
        {
            return _real.Equals(value._real) && _imaginary.Equals(value._imaginary);
        }

        public override int GetHashCode()
        {
            int n1 = 99999997;
            int realHash = _real.GetHashCode() % n1;
            int imaginaryHash = _imaginary.GetHashCode();
            int finalHash = realHash ^ imaginaryHash;
            return finalHash;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "({0}, {1})", _real, _imaginary);
        }

        public string ToString(string format)
        {
            return string.Format(CultureInfo.CurrentCulture, "({0}, {1})", _real.ToString(format, CultureInfo.CurrentCulture), _imaginary.ToString(format, CultureInfo.CurrentCulture));
        }

        public string ToString(IFormatProvider provider)
        {
            return string.Format(provider, "({0}, {1})", _real, _imaginary);
        }

        public string ToString(string format, IFormatProvider provider)
        {
            return string.Format(provider, "({0}, {1})", _real.ToString(format, provider), _imaginary.ToString(format, provider));
        }

        public static Complex Sin(Complex value)
        {
            // We need both sinh and cosh of imaginary part. To avoid multiple calls to Math.Exp with the same value,
            // we compute them both here from a single call to Math.Exp.
            double p = Math.Exp(value._imaginary);
            double q = 1.0 / p;
            double sinh = (p - q) * 0.5;
            double cosh = (p + q) * 0.5;
            return new Complex(Math.Sin(value._real) * cosh, Math.Cos(value._real) * sinh);
            // There is a known limitation with this algorithm: inputs that cause sinh and cosh to overflow, but for
            // which sin or cos are small enough that sin * cosh or cos * sinh are still representable, nonetheless
            // produce overflow. For example, Sin((0.01, 711.0)) should produce (~3.0E306, PositiveInfinity), but
            // instead produces (PositiveInfinity, PositiveInfinity). 
        }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sinh", Justification = "Sinh is the name of a mathematical function.")]
        public static Complex Sinh(Complex value)
        {
            // Use sinh(z) = -i sin(iz) to compute via sin(z).
            Complex sin = Sin(new Complex(-value._imaginary, value._real));
            return new Complex(sin._imaginary, -sin._real);
        }

        public static Complex Asin(Complex value)
        {
            double b, bPrime, v;
            Asin_Internal(Math.Abs(value.Real), Math.Abs(value.Imaginary), out b, out bPrime, out v);

            double u;
            if (bPrime < 0.0)
            {
                u = Math.Asin(b);
            }
            else
            {
                u = Math.Atan(bPrime);
            }

            if (value.Real < 0.0) u = -u;
            if (value.Imaginary < 0.0) v = -v;

            return new Complex(u, v);
        }

        public static Complex Cos(Complex value) {
            double p = Math.Exp(value._imaginary);
            double q = 1.0 / p;
            double sinh = (p - q) * 0.5;
            double cosh = (p + q) * 0.5;
            return new Complex(Math.Cos(value._real) * cosh, -Math.Sin(value._real) * sinh);
        }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Cosh", Justification = "Cosh is the name of a mathematical function.")]
        public static Complex Cosh(Complex value)
        {
            // Use cosh(z) = cos(iz) to compute via cos(z).
            return Cos(new Complex(-value._imaginary, value._real));
        }

        public static Complex Acos(Complex value)
        {
            double b, bPrime, v;
            Asin_Internal(Math.Abs(value.Real), Math.Abs(value.Imaginary), out b, out bPrime, out v);

            double u;
            if (bPrime < 0.0)
            {
                u = Math.Acos(b);
            }
            else
            {
                u = Math.Atan(1.0 / bPrime);
            }

            if (value.Real < 0.0) u = Math.PI - u;
            if (value.Imaginary > 0.0) v = -v;

            return new Complex(u, v);
        }

        public static Complex Tan(Complex value)
        {
            // tan z = sin z / cos z, but to avoid unnecessary repeated trig computations, use
            //   tan z = (sin(2x) + i sinh(2y)) / (cos(2x) + cosh(2y))
            // (see Abramowitz & Stegun 4.3.57 or derive by hand), and compute trig functions here.

            // This approach does not work for |y| > ~355, because sinh(2y) and cosh(2y) overflow,
            // even though their ratio does not. In that case, divide through by cosh to get:
            //   tan z = (sin(2x) / cosh(2y) + i \tanh(2y)) / (1 + cos(2x) / cosh(2y))
            // which correctly computes the (tiny) real part and the (normal-sized) imaginary part.
            
            double x2 = 2.0 * value._real;
            double y2 = 2.0 * value._imaginary;
            double p = Math.Exp(y2);
            double q = 1.0 / p;
            double cosh = (p + q) * 0.5;
            if (Math.Abs(value._imaginary) <= 4.0)
            {
                double sinh = (p - q) * 0.5;
                double D = Math.Cos(x2) + cosh;
                return new Complex(Math.Sin(x2) / D, sinh / D);
            }
            else
            {
                double D = 1.0 + Math.Cos(x2) / cosh;
                return new Complex(Math.Sin(x2) / cosh / D, Math.Tanh(y2) / D);
            }
        }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Tanh", Justification = "Tanh is the name of a mathematical function.")]
        public static Complex Tanh(Complex value)
        {
            // Use tanh(z) = -i tan(iz) to compute via tan(z).
            Complex tan = Tan(new Complex(-value._imaginary, value._real));
            return new Complex(tan._imaginary, -tan._real);
        }

        public static Complex Atan(Complex value)
        {
            Complex two = new Complex(2.0, 0.0);
            return (ImaginaryOne / two) * (Log(One - ImaginaryOne * value) - Log(One + ImaginaryOne * value));
        }

        private static void Asin_Internal (double x, double y, out double b, out double bPrime, out double v) {

            // This method for the inverse complex sine (and cosine) is described in Hull, Fairgrieve,
            // and Tang, "Implementing the Complex Arcsine and Arccosine Functions Using Exception Handling",
            // ACM Transactions on Mathematical Software (1997)
            // (https://www.researchgate.net/profile/Ping_Tang3/publication/220493330_Implementing_the_Complex_Arcsine_and_Arccosine_Functions_Using_Exception_Handling/links/55b244b208ae9289a085245d.pdf)

            // First, the basics: start with sin(w) = (e^{iw} - e^{-iw}) / (2i) = z. Here z is the input
            // and w is the output. To solve for w, define t = e^{i w} and multiply through by t to
            // get the quadratic equation t^2 - 2 i z t - 1 = 0. The solution is t = i z + sqrt(1 - z^2), so
            //   w = arcsin(z) = - i log( i z + sqrt(1 - z^2) )
            // Decompose z = x + i y, multiply out i z + sqrt(1 - z^2), use log(s) = |s| + i arg(s), and do a
            // bunch of algebra to get the components of w = arcsin(z) = u + i v
            //   u = arcsin(beta)  v = sign(y) log(alpha + sqrt(alpha^2 - 1))
            // where
            //   alpha = (rho + sigma) / 2      beta = (rho - sigma) / 2
            //   rho = sqrt((x + 1)^2 + y^2)    sigma = sqrt((x - 1)^2 + y^2)
            // These formulas appear in DLMF section 4.23. (http://dlmf.nist.gov/4.23), along with the analogous
            //   arccos(w) = arccos(beta) - i sign(y) log(alpha + sqrt(alpha^2 - 1))
            // So alpha and beta together give us arcsin(w) and arccos(w).

            // As written, alpha is not susceptible to cancelation errors, but beta is. To avoid cancelation, note
            //   beta = (rho^2 - sigma^2) / (rho + sigma) / 2 = (2 x) / (rho + sigma) = x / alpha
            // which is not subject to cancelation. Note alpha >= 1 and |beta| <= 1.

            // For alpha ~ 1, the argument of the log is near unity, so we compute (alpha - 1) instead,
            // write the argument as 1 + (alpha - 1) + sqrt((alpha - 1)(alpha + 1)), and use the log1p function
            // to compute the log without loss of accuracy.
            // For beta ~ 1, arccos does not accurately resolve small angles, so we compute the tangent of the angle
            // instead.
            // Hull, Fairgrieve, and Tang derive formulas for (alpha - 1) and beta' = tan(u) that do not suffer
            // from cancelation in these cases.

            // For simplicity, we assume all positive inputs and return all positive outputs. The caller should
            // assign signs appropriate to the desired cut conventions. We return v directly since its magnitude
            // is the same for both arcsin and arccos. Instead of u, we usually return beta and sometimes beta'.
            // If beta' is not computed, it is set to -1; if it is computed, it should be used instead of beta
            // to determine u. Compute u = arcsin(beta) or u = arctan(beta') for arcsin, u = arccos(beta)
            // or arctan(1/beta') for arccos.

            Debug.Assert((x >= 0.0) || double.IsNaN(x));
            Debug.Assert((y >= 0.0) || double.IsNaN(y));

            // For x or y large enough to overflow alpha^2, we can simplify our formulas and avoid overflow.
            if ((x > s_asinOverflowThreshold) || (y > s_asinOverflowThreshold))
            {
                b = -1.0;
                bPrime = x / y;

                double small, big;
                if (x < y)
                {
                    small = x;
                    big = y;
                }
                else
                {
                    small = y;
                    big = x;
                }
                double ratio = small / big;
                v = s_log2 + Math.Log(big) + 0.5 * Log1P(ratio * ratio);
            }
            else
            {
                double r = Hypot((x + 1.0), y);
                double s = Hypot((x - 1.0), y);

                double a = (r + s) * 0.5;
                b = x / a;

                if (b > 0.75)
                {
                    if (x <= 1.0)
                    {
                        double amx = (y * y / (r + (x + 1.0)) + (s + (1.0 - x))) * 0.5;
                        bPrime = x / Math.Sqrt((a + x) * amx);
                    }
                    else
                    {
                        // In this case, amx ~ y^2. Since we take the square root of amx, we should
                        // pull y out from under the square root so we don't lose its contribution
                        // when y^2 underflows.
                        double t = (1.0 / (r + (x + 1.0)) + 1.0 / (s + (x - 1.0))) * 0.5;
                        bPrime = x / y / Math.Sqrt((a + x) * t);
                    }
                }
                else
                {
                    bPrime = -1.0;
                }

                if (a < 1.5)
                {
                    if (x < 1.0)
                    {
                        // This is another case where our expression is proportional to y^2 and
                        // we take its square root, so again we pull out a factor of y from
                        // under the square root.
                        double t = (1.0 / (r + (x + 1.0)) + 1.0 / (s + (1.0 - x))) * 0.5;
                        double am1 = y * y * t;
                        v = Log1P(am1 + y * Math.Sqrt(t * (a + 1.0)));
                    }
                    else
                    {
                        double am1 = (y * y / (r + (x + 1.0)) + (s + (x - 1.0))) * 0.5;
                        v = Log1P(am1 + Math.Sqrt(am1 * (a + 1.0)));
                    }
                }
                else
                {
                    // Because of the test above, we can be sure that a * a will not overflow.
                    v = Math.Log(a + Math.Sqrt((a - 1.0) * (a + 1.0)));
                }
            }
        }


        public static Complex Log(Complex value)
        {
            return new Complex(Math.Log(Abs(value)), Math.Atan2(value._imaginary, value._real));
        }

        public static Complex Log(Complex value, double baseValue)
        {
            return Log(value) / Log(baseValue);
        }

        public static Complex Log10(Complex value)
        {
            Complex tempLog = Log(value);
            return Scale(tempLog, InverseOfLog10);
        }

        public static Complex Exp(Complex value)
        {
            double expReal = Math.Exp(value._real);
            double cosImaginary = expReal * Math.Cos(value._imaginary);
            double sinImaginary = expReal * Math.Sin(value._imaginary);
            return new Complex(cosImaginary, sinImaginary);
        }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sqrt", Justification = "Sqrt is the name of a mathematical function.")]
        public static Complex Sqrt(Complex value)
        {

            if (value._imaginary == 0.0)
            {
                // Handle the trivial case quickly.
                if (value._real < 0.0)
                {
                    return new Complex(0.0, Math.Sqrt(-value._real));
                }
                else
                {
                    return new Complex(Math.Sqrt(value._real), 0.0);
                }
            }
            else
            {

                // One way to compute Sqrt(z) is just to call Pow(z, 0.5), which coverts to polar coordinates
                // (sqrt + atan), halves the phase, and reconverts to cartesian coordinates (cos + sin).
                // Not only is this more expensive than necessary, it also fails to preserve certain expected
                // symmetries, such as that the square root of a pure negative is a pure imaginary, and that the
                // square root of a pure imaginary has exactly equal real and imaginary parts. This all goes
                // back to the fact that Math.PI is not stored with infinite precision, so taking half of Math.PI
                // does not land us on an argument with cosine exactly equal to zero.

                // To find a fast and symmetry-respecting formula for complex square root,
                // note x + i y = \sqrt{a + i b} implies x^2 + 2 i x y - y^2 = a + i b,
                // so x^2 - y^2 = a and 2 x y = b. Cross-substitute and use the quadratic formula to obtain
                //   x = \sqrt{\frac{\sqrt{a^2 + b^2} + a}{2}}  y = \pm \sqrt{\frac{\sqrt{a^2 + b^2} - a}{2}}
                // There is just one complication: depending on the sign on a, either x or y suffers from
                // cancelation when |b| << |a|. We can get aroud this by noting that our formulas imply
                // x^2 y^2 = b^2 / 4, so |x| |y| = |b| / 2. So after computing the one that doesn't suffer
                // from cancelation, we can compute the other with just a division. This is basically just
                // the right way to evaluate the quadratic formula without cancelation.

                // All this reduces our total cost to two sqrts and a few flops, and it respects the desired
                // symmetries. Much better than atan + cos + sin!

                // The signs are a matter of choice of branch cut, which is traditionally taken so x > 0 and sign(y) = sign(b).
      
                // If the components are too large, Hypot will overflow, even though the subsequent sqrt would
                // make the result representable. To avoid this, we re-scale (by exact powers of 2 for accuracy)
                // when we encounter very large components to avoid intermediate infinities.
                bool rescale = false;
                if ((Math.Abs(value._real) >= s_sqrtRescaleThreshold) || (Math.Abs(value._imaginary) >= s_sqrtRescaleThreshold))
                {
                    if (double.IsInfinity(value._imaginary) && !double.IsNaN(value._real))
                    {
                        // We need to handle infinite imaginary parts specially because otherwise
                        // our formulas below produce inf/inf = NaN. The NaN test is necessary
                        // so that we return NaN rather than (+inf,inf) for (NaN,inf).
                        return (new Complex(double.PositiveInfinity, value._imaginary));
                    }
                    else
                    {
                        value._real *= 0.25;
                        value._imaginary *= 0.25;
                        rescale = true;
                    }
                }
 
                // This is the core of the algorithm. Everything else is special case handling.
                double x, y;
                if (value._real >= 0.0)
                {
                    x = Math.Sqrt((Hypot(value._real, value._imaginary) + value._real) * 0.5);
                    y = value._imaginary / (2.0 * x);
                }
                else
                {
                    y = Math.Sqrt((Hypot(value._real, value._imaginary) - value._real) * 0.5);
                    if (value._imaginary < 0.0) y = -y;
                    x = value._imaginary / (2.0 * y);
                }

                if (rescale)
                {
                    x *= 2.0;
                    y *= 2.0;
                }

                return new Complex(x, y);

            }
            
        }

        public static Complex Pow(Complex value, Complex power)
        {
            if (power == Zero)
            {
                return One;
            }

            if (value == Zero)
            {
                return Zero;
            }

            double valueReal = value._real;
            double valueImaginary = value._imaginary;
            double powerReal = power._real;
            double powerImaginary = power._imaginary;

            double rho = Abs(value);
            double theta = Math.Atan2(valueImaginary, valueReal);
            double newRho = powerReal * theta + powerImaginary * Math.Log(rho);

            double t = Math.Pow(rho, powerReal) * Math.Pow(Math.E, -powerImaginary * theta);

            return new Complex(t * Math.Cos(newRho), t * Math.Sin(newRho));
        }

        public static Complex Pow(Complex value, double power)
        {
            return Pow(value, new Complex(power, 0));
        }
        
        private static Complex Scale(Complex value, double factor)
        {
            double realResult = factor * value._real;
            double imaginaryResuilt = factor * value._imaginary;
            return new Complex(realResult, imaginaryResuilt);
        }

        public static implicit operator Complex(short value)
        {
            return new Complex(value, 0.0);
        }

        public static implicit operator Complex(int value)
        {
            return new Complex(value, 0.0);
        }

        public static implicit operator Complex(long value)
        {
            return new Complex(value, 0.0);
        }

        [CLSCompliant(false)]
        public static implicit operator Complex(ushort value)
        {
            return new Complex(value, 0.0);
        }

        [CLSCompliant(false)]
        public static implicit operator Complex(uint value)
        {
            return new Complex(value, 0.0);
        }

        [CLSCompliant(false)]
        public static implicit operator Complex(ulong value)
        {
            return new Complex(value, 0.0);
        }

        [CLSCompliant(false)]
        public static implicit operator Complex(sbyte value)
        {
            return new Complex(value, 0.0);
        }

        public static implicit operator Complex(byte value)
        {
            return new Complex(value, 0.0);
        }

        public static implicit operator Complex(float value)
        {
            return new Complex(value, 0.0);
        }

        public static implicit operator Complex(double value)
        {
            return new Complex(value, 0.0);
        }

        public static explicit operator Complex(BigInteger value)
        {
            return new Complex((double)value, 0.0);
        }

        public static explicit operator Complex(decimal value)
        {
            return new Complex((double)value, 0.0);
        }
    }
}
