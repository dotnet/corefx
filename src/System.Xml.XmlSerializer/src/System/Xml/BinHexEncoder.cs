// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.Xml
{
    internal abstract class BinHexEncoder
    {
        private const string s_hexDigits = "0123456789ABCDEF";
        private const int CharsChunkSize = 128;

        internal static string Encode(byte[] inArray, int offsetIn, int count)
        {
            Debug.Assert(null != inArray);
            Debug.Assert(0 <= offsetIn);
            Debug.Assert(0 <= count);
            Debug.Assert(count <= inArray.Length - offsetIn);
            
            char[] outArray = new char[2 * count];
            int lenOut = Encode(inArray, offsetIn, count, outArray);
            return new String(outArray, 0, lenOut);
        }

        private static int Encode(byte[] inArray, int offsetIn, int count, char[] outArray)
        {
            int curOffsetOut = 0, offsetOut = 0;
            byte b;
            int lengthOut = outArray.Length;

            for (int j = 0; j < count; j++)
            {
                b = inArray[offsetIn++];
                outArray[curOffsetOut++] = s_hexDigits[b >> 4];
                if (curOffsetOut == lengthOut)
                {
                    break;
                }
                outArray[curOffsetOut++] = s_hexDigits[b & 0xF];
                if (curOffsetOut == lengthOut)
                {
                    break;
                }
            }
            return curOffsetOut - offsetOut;
        } // function
    } // class
} // namespace
