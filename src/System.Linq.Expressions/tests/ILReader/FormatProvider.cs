// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Code adapted from https://blogs.msdn.microsoft.com/haibo_luo/2010/04/19/ilvisualizer-2010-solution

using System.Collections.Generic;
using System.Text;

namespace System.Linq.Expressions.Tests
{
    public interface IFormatProvider
    {
        string Int32ToHex(int int32);
        string Int16ToHex(int int16);
        string Int8ToHex(int int8);
        string Argument(int ordinal);
        string EscapedString(string str);
        string Label(int offset, Dictionary<int, int> targetLabels);
        string MultipleLabels(int[] offsets, Dictionary<int, int> targetLabels);
        string SigByteArrayToString(byte[] sig);
    }

    public sealed class DefaultFormatProvider : IFormatProvider
    {
        private DefaultFormatProvider() { }

        public static readonly DefaultFormatProvider Instance = new DefaultFormatProvider();

        public string Int32ToHex(int int32) => int32.ToString("X8");
        public string Int16ToHex(int int16) => int16.ToString("X4");
        public string Int8ToHex(int int8) => int8.ToString("X2");
        public string Argument(int ordinal) => string.Format("V_{0}", ordinal);
        public string Label(int offset, Dictionary<int, int> targetLabels) => $"Label_{targetLabels[offset]:x2}";

        public string MultipleLabels(int[] offsets, Dictionary<int, int> targetLabels)
        {
            var sb = new StringBuilder();
            int length = offsets.Length;
            for (int i = 0; i < length; i++)
            {
                sb.AppendFormat(i == 0 ? "(" : ", ");
                sb.Append(Label(offsets[i], targetLabels));
            }
            sb.AppendFormat(")");
            return sb.ToString();
        }

        public string EscapedString(string str)
        {
            int length = str.Length;
            var sb = new StringBuilder(length * 2);
            for (int i = 0; i < length; i++)
            {
                char ch = str[i];
                if (ch == '\t') sb.Append("\\t");
                else if (ch == '\n') sb.Append("\\n");
                else if (ch == '\r') sb.Append("\\r");
                else if (ch == '\"') sb.Append("\\\"");
                else if (ch == '\\') sb.Append("\\");
                else if (ch < 0x20 || ch >= 0x7f) sb.AppendFormat("\\u{0:x4}", (int)ch);
                else sb.Append(ch);
            }
            return "\"" + sb.ToString() + "\"";
        }

        public string SigByteArrayToString(byte[] sig)
        {
            var sb = new StringBuilder();
            int length = sig.Length;
            for (int i = 0; i < length; i++)
            {
                sb.AppendFormat(i == 0 ? "SIG [" : " ");
                sb.Append(Int8ToHex(sig[i]));
            }
            sb.AppendFormat("]");
            return sb.ToString();
        }
    }
}
