// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System.Buffers.Text
{
    /// <summary>
    /// Methods to format common data types as Utf8 strings.
    /// </summary>
    public static partial class Utf8Formatter
    {
        //
        // Common worker for all signed integer TryFormat overloads
        //
        private static bool TryFormatInt64(long value, ulong mask, Span<byte> buffer, out int bytesWritten, StandardFormat format)
        {
            if (format.IsDefault)
            {
                // Officially, the default is "G" but "G without a precision is equivalent to "D" so that's why we're calling the "D" helper.
                return TryFormatInt64D(value, format.Precision, buffer, out bytesWritten);
            }

            switch (format.Symbol)
            {
                case 'G':
                case 'g':
                    if (format.HasPrecision)
                        throw new NotSupportedException(SR.Argument_GWithPrecisionNotSupported); // With a precision, 'G' can produce exponential format, even for integers.
                    return TryFormatInt64D(value, format.Precision, buffer, out bytesWritten);

                case 'd':
                case 'D':
                    return TryFormatInt64D(value, format.Precision, buffer, out bytesWritten);

                case 'n':
                case 'N':
                    return TryFormatInt64N(value, format.Precision, buffer, out bytesWritten);

                case 'x':
                    return TryFormatUInt64X((ulong)value & mask, format.Precision, true, buffer, out bytesWritten);

                case 'X':
                    return TryFormatUInt64X((ulong)value & mask, format.Precision, false, buffer, out bytesWritten);

                default:
                    throw new FormatException(SR.Argument_BadFormatSpecifier);
            }
        }
    }
}
