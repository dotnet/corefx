// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Reflection;

namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    ///     Used for specifying a range constraint
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
        AllowMultiple = false)]
    public class RangeAttribute : ValidationAttribute
    {
        /// <summary>
        ///     Constructor that takes integer minimum and maximum values
        /// </summary>
        /// <param name="minimum">The minimum value, inclusive</param>
        /// <param name="maximum">The maximum value, inclusive</param>
        public RangeAttribute(int minimum, int maximum)
            : base(() => SR.RangeAttribute_ValidationError)
        {
            Minimum = minimum;
            Maximum = maximum;
            OperandType = typeof(int);
        }

        /// <summary>
        ///     Constructor that takes double minimum and maximum values
        /// </summary>
        /// <param name="minimum">The minimum value, inclusive</param>
        /// <param name="maximum">The maximum value, inclusive</param>
        public RangeAttribute(double minimum, double maximum)
            : base(() => SR.RangeAttribute_ValidationError)
        {
            Minimum = minimum;
            Maximum = maximum;
            OperandType = typeof(double);
        }

        /// <summary>
        ///     Allows for specifying range for arbitrary types. The minimum and maximum strings
        ///     will be converted to the target type.
        /// </summary>
        /// <param name="type">The type of the range parameters. Must implement IComparable.</param>
        /// <param name="minimum">The minimum allowable value.</param>
        /// <param name="maximum">The maximum allowable value.</param>
        public RangeAttribute(Type type, string minimum, string maximum)
            : base(() => SR.RangeAttribute_ValidationError)
        {
            OperandType = type;
            Minimum = minimum;
            Maximum = maximum;
        }

        /// <summary>
        ///     Gets the minimum value for the range
        /// </summary>
        public object Minimum { get; private set; }

        /// <summary>
        ///     Gets the maximum value for the range
        /// </summary>
        public object Maximum { get; private set; }

        /// <summary>
        ///     Gets the type of the <see cref="Minimum" /> and <see cref="Maximum" /> values (e.g. Int32, Double, or some custom
        ///     type)
        /// </summary>
        public Type OperandType { get; private set; }

        private Func<object, object> Conversion { get; set; }

        private void Initialize(IComparable minimum, IComparable maximum, Func<object, object> conversion)
        {
            if (minimum.CompareTo(maximum) > 0)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                    SR.RangeAttribute_MinGreaterThanMax, maximum, minimum));
            }

            Minimum = minimum;
            Maximum = maximum;
            Conversion = conversion;
        }


        /// <summary>
        ///     Returns true if the value falls between min and max, inclusive.
        /// </summary>
        /// <param name="value">The value to test for validity.</param>
        /// <returns><c>true</c> means the <paramref name="value" /> is valid</returns>
        /// <exception cref="InvalidOperationException"> is thrown if the current attribute is ill-formed.</exception>
        public override bool IsValid(object value)
        {
            // Validate our properties and create the conversion function
            SetupConversion();

            // Automatically pass if value is null or empty. RequiredAttribute should be used to assert a value is not empty.
            if (value == null)
            {
                return true;
            }
            var s = value as string;
            if (s != null && string.IsNullOrEmpty(s))
            {
                return true;
            }

            object convertedValue = null;

            try
            {
                convertedValue = Conversion(value);
            }
            catch (FormatException)
            {
                return false;
            }
            catch (InvalidCastException)
            {
                return false;
            }
            catch (NotSupportedException)
            {
                return false;
            }

            var min = (IComparable)Minimum;
            var max = (IComparable)Maximum;
            return min.CompareTo(convertedValue) <= 0 && max.CompareTo(convertedValue) >= 0;
        }

        /// <summary>
        ///     Override of <see cref="ValidationAttribute.FormatErrorMessage" />
        /// </summary>
        /// <remarks>This override exists to provide a formatted message describing the minimum and maximum values</remarks>
        /// <param name="name">The user-visible name to include in the formatted message.</param>
        /// <returns>A localized string describing the minimum and maximum values</returns>
        /// <exception cref="InvalidOperationException"> is thrown if the current attribute is ill-formed.</exception>
        public override string FormatErrorMessage(string name)
        {
            SetupConversion();

            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, Minimum, Maximum);
        }

        /// <summary>
        ///     Validates the properties of this attribute and sets up the conversion function.
        ///     This method throws exceptions if the attribute is not configured properly.
        ///     If it has once determined it is properly configured, it is a NOP.
        /// </summary>
        private void SetupConversion()
        {
            if (Conversion == null)
            {
                object minimum = Minimum;
                object maximum = Maximum;

                if (minimum == null || maximum == null)
                {
                    throw new InvalidOperationException(SR.RangeAttribute_Must_Set_Min_And_Max);
                }

                // Careful here -- OperandType could be int or double if they used the long form of the ctor.
                // But the min and max would still be strings.  Do use the type of the min/max operands to condition
                // the following code.
                Type operandType = minimum.GetType();

                if (operandType == typeof(int))
                {
                    Initialize((int)minimum, (int)maximum, v => Convert.ToInt32(v, CultureInfo.InvariantCulture));
                }
                else if (operandType == typeof(double))
                {
                    Initialize((double)minimum, (double)maximum,
                        v => Convert.ToDouble(v, CultureInfo.InvariantCulture));
                }
                else
                {
                    Type type = OperandType;
                    if (type == null)
                    {
                        throw new InvalidOperationException(
                            SR.RangeAttribute_Must_Set_Operand_Type);
                    }
                    Type comparableType = typeof(IComparable);
                    if (!comparableType.GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
                    {
                        throw new InvalidOperationException(
                            string.Format(
                                CultureInfo.CurrentCulture,
                                SR.RangeAttribute_ArbitraryTypeNotIComparable,
                                type.FullName,
                                comparableType.FullName));
                    }

                    Func<object, object> conversion =
                        value =>
                            (value != null && value.GetType() == type)
                                ? value
                                : Convert.ChangeType(value, type, CultureInfo.CurrentCulture);
                    var min = (IComparable)conversion(minimum);
                    var max = (IComparable)conversion(maximum);
                    Initialize(min, max, conversion);
                }
            }
        }
    }
}
