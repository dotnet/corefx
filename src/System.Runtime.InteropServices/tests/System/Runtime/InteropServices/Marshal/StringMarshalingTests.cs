// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security;
using Xunit;

namespace System.Runtime.InteropServices.Tests
{
    public class StringMarshalingTests
    {
        private readonly String[] TestStrings = new String[] {
                                    "", //Empty String
                                    "Test String",
                                    "A", //Single character string
                                    "This is a very long string as it repeats itself. " +
                                    "This is a very long string as it repeats itself. " +
                                    "This is a very long string as it repeats itself. " +
                                    "This is a very long string as it repeats itself. " +
                                    "This is a very long string as it repeats itself. " +
                                    "This is a very long string as it repeats itself. " +
                                    "This is a very long string as it repeats itself. " +
                                    "This is a very long string as it repeats itself. " +
                                    "This is a very long string as it repeats itself. " +
                                    "This is a very long string as it repeats itself. " +
                                    "This is a very long string as it repeats itself. " +
                                    "This is a very long string as it repeats itself. " +
                                    "This is a very long string as it repeats itself.",
                                    "This \n is \n a \n multiline \n string",
                                    "This \0 is \0 a \0 string \0 with \0 nulls",
                                    "\0string",
                                    "string\0",
                                    "\0\0\0\0\0\0\0\0"
                                    };

        [Fact]
        public void StringToBStrToString()
        {
            foreach (String ts in TestStrings)
            {
                IntPtr BStr = Marshal.StringToBSTR(ts);
                String str = Marshal.PtrToStringBSTR(BStr);
                Assert.Equal(ts, str);
                Marshal.FreeBSTR(BStr);
            }
        }

        [Fact]
        public unsafe void SecureStringToBSTRToString()
        {
            foreach (String ts in TestStrings)
            {
                SecureString secureString = new SecureString();
                foreach (char character in ts)
                {
                    secureString.AppendChar(character);
                }

                IntPtr BStr = IntPtr.Zero;
                String str;

                try
                {
                    BStr = Marshal.SecureStringToBSTR(secureString);
                    str = Marshal.PtrToStringBSTR(BStr);
                }
                finally
                {
                    if (BStr != IntPtr.Zero)
                    {
                        Marshal.ZeroFreeBSTR(BStr);
                    }
                }
                Assert.Equal(ts, str);
            }
        }

        [Fact]
        public void StringToCoTaskMemAnsiToString()
        {
            foreach (String ts in TestStrings)
            {
                if (ts.Contains("\0"))
                    continue; //Skip the string with nulls case


                IntPtr AnsiStr = Marshal.StringToCoTaskMemAnsi(ts);
                String str = Marshal.PtrToStringAnsi(AnsiStr);

                Assert.Equal(ts, str);
                if (ts.Length > 0)
                {
                    String str2 = Marshal.PtrToStringAnsi(AnsiStr, ts.Length - 1);
                    Assert.Equal(ts.Substring(0, ts.Length - 1), str2);
                }
                Marshal.FreeCoTaskMem(AnsiStr);
            }
        }

        [Fact]
        public void StringToCoTaskMemUniToString()
        {
            foreach (String ts in TestStrings)
            {
                if (ts.Contains("\0"))
                    continue; //Skip the string with nulls case


                IntPtr UniStr = Marshal.StringToCoTaskMemUni(ts);
                String str = Marshal.PtrToStringUni(UniStr);

                Assert.Equal(ts, str);
                if (ts.Length > 0)
                {
                    String str2 = Marshal.PtrToStringUni(UniStr, ts.Length - 1);
                    Assert.Equal(ts.Substring(0, ts.Length - 1), str2);
                }
                Marshal.FreeCoTaskMem(UniStr);


            }
        }

        [Fact]
        public void StringToHGlobalAnsiToString()
        {
            foreach (String ts in TestStrings)
            {
                if (ts.Contains("\0"))
                    continue; //Skip the string with nulls case

                IntPtr AnsiStr = Marshal.StringToHGlobalAnsi(ts);
                String str = Marshal.PtrToStringAnsi(AnsiStr);

                Assert.Equal(ts, str);
                if (ts.Length > 0)
                {
                    String str2 = Marshal.PtrToStringAnsi(AnsiStr, ts.Length - 1);
                    Assert.Equal(ts.Substring(0, ts.Length - 1), str2);
                }
                Marshal.FreeHGlobal(AnsiStr);
            }
        }

        [Fact]
        public void StringToHGlobalUniToString()
        {
            foreach (String ts in TestStrings)
            {
                if (ts.Contains("\0"))
                    continue; //Skip the string with nulls case


                IntPtr UniStr = Marshal.StringToHGlobalUni(ts);
                String str = Marshal.PtrToStringUni(UniStr);

                Assert.Equal(ts, str);
                if (ts.Length > 0)
                {
                    String str2 = Marshal.PtrToStringUni(UniStr, ts.Length - 1);
                    Assert.Equal(ts.Substring(0, ts.Length - 1), str2);
                }
                Marshal.FreeHGlobal(UniStr);
            }

        }

        [Fact]
        public void TestUTF8String()
        {
            foreach (String srcString in TestStrings)
            {
                // we assume string null terminated
                if (srcString.Contains("\0"))
                    continue;

                IntPtr ptrString = Marshal.StringToCoTaskMemUTF8(srcString);
                string retString = Marshal.PtrToStringUTF8(ptrString);

                Assert.True(srcString.Equals(retString), "Round triped strings do not match");
                if (srcString.Length > 0)
                {
                    string retString2 = Marshal.PtrToStringUTF8(ptrString, srcString.Length - 1);
                    Assert.True(retString2.Equals(srcString.Substring(0, srcString.Length - 1)), "Round triped strings do not match");
                }
                Marshal.FreeHGlobal(ptrString);
            }
        }

        [Fact]
        public void TestNullString()
        {
            Assert.True(Marshal.PtrToStringUTF8(IntPtr.Zero) == null, "IntPtr.Zero not marshaled to null for UTF8 strings");
            Assert.True(Marshal.PtrToStringUni(IntPtr.Zero) == null, "IntPtr.Zero not marshaled to null for Unicode strings");
            Assert.True(Marshal.PtrToStringAnsi(IntPtr.Zero) == null, "IntPtr.Zero not marshaled to null for ANSI strings");
        }
    }
}
