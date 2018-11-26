// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// System.Drawing.MarginsConverter.cs
//
// Author:
//   Dennis Hayes (dennish@Raytek.com)
//   Herve Poussineau (hpoussineau@fr.st)
//
// (C) 2002 Ximian, Inc
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

using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;

namespace System.Drawing.Printing
{
    public class MarginsConverter : ExpandableObjectConverter
    {
        public MarginsConverter() { }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string) || destinationType == typeof(InstanceDescriptor))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string valueString)
            {
                if (value == null)
                {
                    return new Margins();
                }

                // format [left];[right];[top];[bottom]
                const string Separator = @"( |\t)*;( |\t)*";
                const string Pattern = @"(?<left>\d+)" + Separator + @"(?<right>\d+)" + Separator + @"(?<top>\d+)" + Separator + @"(?<bottom>\d+)";

                Match match = new Regex(Pattern).Match(valueString);
                if (!match.Success)
                {
                    throw new ArgumentException(nameof(value));
                }

                int left, right, top, bottom;
                try
                {
                    left = int.Parse(match.Groups["left"].Value);
                    right = int.Parse(match.Groups["right"].Value);
                    top = int.Parse(match.Groups["top"].Value);
                    bottom = int.Parse(match.Groups["bottom"].Value);
                }
                catch (Exception e)
                {
                    throw new ArgumentException(nameof(value), e);
                }
                return new Margins(left, right, top, bottom);
            }
            else
            {
                return base.ConvertFrom(context, culture, value);
            }
        }


        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is Margins source)
            {
                const string ret = "{0}; {1}; {2}; {3}";
                return String.Format(ret, source.Left, source.Right, source.Top, source.Bottom);
            }
            if (destinationType == typeof(InstanceDescriptor) && value is Margins c)
            {
                ConstructorInfo ctor = typeof(Margins).GetTypeInfo ().GetConstructor (new Type[] {typeof(int), typeof(int), typeof(int), typeof(int)} );
                return new InstanceDescriptor(ctor, new object[] { c.Left, c.Right, c.Top, c.Bottom });
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool GetCreateInstanceSupported(ITypeDescriptorContext context) => true;

        public override object CreateInstance(ITypeDescriptorContext context, System.Collections.IDictionary propertyValues)
        {
            try
            {
                Margins margins = new Margins();
                margins.Left = int.Parse(propertyValues["Left"].ToString());
                margins.Right = int.Parse(propertyValues["Right"].ToString());
                margins.Top = int.Parse(propertyValues["Top"].ToString());
                margins.Bottom = int.Parse(propertyValues["Bottom"].ToString());
                return margins;
            }
            catch (Exception)
            {
                // in case of error, return null
                return null;
            }
        }
    }
}
