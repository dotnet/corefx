// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// System.Drawing.Printing.PrinterUnitConvert.cs
//
// Authors:
//   Martin Willemoes Hansen (mwh@sysrq.dk)
//   Herve Poussineau (hpoussineau@fr.st)
//
// (C) 2003 Martin Willemoes Hansen
//

//
// Copyright (C) 2004 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

namespace System.Drawing.Printing
{
    public sealed class PrinterUnitConvert
    {
        private PrinterUnitConvert()
        {
        }

        public static double Convert(double value,
                          PrinterUnit fromUnit,
                          PrinterUnit toUnit)
        {
            switch (fromUnit)
            {
                case PrinterUnit.Display:
                    switch (toUnit)
                    {
                        case PrinterUnit.Display:
                            return value;
                        case PrinterUnit.ThousandthsOfAnInch:
                            return value * 10;
                        case PrinterUnit.HundredthsOfAMillimeter:
                            return value * 25.4;
                        case PrinterUnit.TenthsOfAMillimeter:
                            return value * 2.54;
                    }
                    break;
                case PrinterUnit.ThousandthsOfAnInch:
                    switch (toUnit)
                    {
                        case PrinterUnit.Display:
                            return value / 10;
                        case PrinterUnit.ThousandthsOfAnInch:
                            return value;
                        case PrinterUnit.HundredthsOfAMillimeter:
                            return value * 2.54;
                        case PrinterUnit.TenthsOfAMillimeter:
                            return value * 0.254;
                    }
                    break;
                case PrinterUnit.HundredthsOfAMillimeter:
                    switch (toUnit)
                    {
                        case PrinterUnit.Display:
                            return value / 25.4;
                        case PrinterUnit.ThousandthsOfAnInch:
                            return value / 2.54;
                        case PrinterUnit.HundredthsOfAMillimeter:
                            return value;
                        case PrinterUnit.TenthsOfAMillimeter:
                            return value / 10;
                    }
                    break;
                case PrinterUnit.TenthsOfAMillimeter:
                    switch (toUnit)
                    {
                        case PrinterUnit.Display:
                            return value / 2.54;
                        case PrinterUnit.ThousandthsOfAnInch:
                            return value / 0.254;
                        case PrinterUnit.HundredthsOfAMillimeter:
                            return value * 10;
                        case PrinterUnit.TenthsOfAMillimeter:
                            return value;
                    }
                    break;
            }
            // should never happen
            throw new NotImplementedException();
        }

        public static int Convert(int value,
                       PrinterUnit fromUnit,
                       PrinterUnit toUnit)
        {
            double rslt;
            rslt = Convert((double)value, fromUnit, toUnit);
            return (int)Math.Round(rslt);

        }

        public static Margins Convert(Margins value,
                           PrinterUnit fromUnit,
                           PrinterUnit toUnit)
        {
            return new Margins(
                Convert(value.Left, fromUnit, toUnit),
                Convert(value.Right, fromUnit, toUnit),
                Convert(value.Top, fromUnit, toUnit),
                Convert(value.Bottom, fromUnit, toUnit));
        }

        public static Point Convert(Point value,
                         PrinterUnit fromUnit,
                         PrinterUnit toUnit)
        {
            return new Point(
                Convert(value.X, fromUnit, toUnit),
                Convert(value.Y, fromUnit, toUnit));
        }

        public static Rectangle Convert(Rectangle value,
                         PrinterUnit fromUnit,
                         PrinterUnit toUnit)
        {
            return new Rectangle(
                Convert(value.X, fromUnit, toUnit),
                Convert(value.Y, fromUnit, toUnit),
                Convert(value.Width, fromUnit, toUnit),
                Convert(value.Height, fromUnit, toUnit));
        }

        public static Size Convert(Size value,
                        PrinterUnit fromUnit,
                        PrinterUnit toUnit)
        {
            return new Size(
                Convert(value.Width, fromUnit, toUnit),
                Convert(value.Height, fromUnit, toUnit));
        }
    }
}
