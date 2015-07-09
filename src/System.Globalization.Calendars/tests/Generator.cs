// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace TestLibrary
{
    public static class Generator
    {
        internal static Random m_rand = new Random();
        internal static int? seed = null;

        public static int? Seed
        {
            get
            {
                if (seed.HasValue)
                {
                    return seed.Value;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (!(seed.HasValue))
                {
                    seed = value;
                    if (seed.HasValue)
                    {
                        m_rand = new Random(seed.Value);
                    }
                }
            }
        }

        // returns a byte array of random data
        public static void GetBytes(int new_seed, byte[] buffer)
        {
            Seed = new_seed;
            GetBytes(buffer);
        }
        public static void GetBytes(byte[] buffer)
        {
            m_rand.NextBytes(buffer);
        }

        // returns a non-negative Int64 between 0 and Int64.MaxValue
        public static Int64 GetInt64(Int32 new_seed)
        {
            Seed = new_seed;
            return GetInt64();
        }
        public static Int64 GetInt64()
        {
            byte[] buffer = new byte[8];
            Int64 iVal;

            GetBytes(buffer);

            // convert to Int64
            iVal = 0;
            for (int i = 0; i < buffer.Length; i++)
            {
                iVal |= ((Int64)buffer[i] << (i * 8));
            }

            if (0 > iVal) iVal *= -1;

            return iVal;
        }

        // returns a non-negative Int32 between 0 and Int32.MaxValue
        public static Int32 GetInt32(Int32 new_seed)
        {
            Seed = new_seed;
            return GetInt32();
        }
        public static Int32 GetInt32()
        {
            Int32 i = m_rand.Next();
            return i;
        }

        // returns a non-negative Int16 between 0 and Int16.MaxValue
        public static Int16 GetInt16(Int32 new_seed)
        {
            Seed = new_seed;
            return GetInt16();
        }
        public static Int16 GetInt16()
        {
            Int16 i = Convert.ToInt16(m_rand.Next() % (1 + Int16.MaxValue));
            return i;
        }

        // returns a non-negative Byte between 0 and Byte.MaxValue
        public static Byte GetByte(Int32 new_seed)
        {
            Seed = new_seed;
            return GetByte();
        }
        public static Byte GetByte()
        {
            Byte i = Convert.ToByte(m_rand.Next() % (1 + Byte.MaxValue));
            return i;
        }

        // returns a non-negative Double between 0.0 and 1.0
        public static Double GetDouble(Int32 new_seed)
        {
            Seed = new_seed;
            return GetDouble();
        }
        public static Double GetDouble()
        {
            Double i = m_rand.NextDouble();
            return i;
        }

        // returns a non-negative Single between 0.0 and 1.0
        public static Single GetSingle(Int32 new_seed)
        {
            Seed = new_seed;
            return GetSingle();
        }
        public static Single GetSingle()
        {
            Single i = Convert.ToSingle(m_rand.NextDouble());
            return i;
        }

        // returns a valid char that is a letter
        public static Char GetCharLetter(Int32 new_seed)
        {
            Seed = new_seed;
            return GetCharLetter();
        }
        public static Char GetCharLetter()
        {
            return GetCharLetter(true);
        }
        // returns a valid char that is a letter
        // if allowsurrogate is true then surrogates are valid return values
        public static Char GetCharLetter(Int32 new_seed, bool allowsurrogate)
        {
            Seed = new_seed;
            return GetCharLetter(allowsurrogate);
        }
        public static Char GetCharLetter(bool allowsurrogate)
        {
            return GetCharLetter(allowsurrogate, true);
        }

        // returns a valid char that is a letter
        // if allowsurrogate is true then surrogates are valid return values
        // if allownoweight is true, then no-weight characters are valid return values
        public static Char GetCharLetter(Int32 new_seed, bool allowsurrogate, bool allownoweight)
        {
            Seed = new_seed;
            return GetCharLetter(allowsurrogate, allownoweight);
        }
        public static Char GetCharLetter(bool allowsurrogate, bool allownoweight)
        {
            Int16 iVal;
            Char c = 'a';
            Int32 counter;
            bool loopCondition = true;

            // attempt to randomly find a letter
            counter = 100;
            do
            {
                counter--;
                iVal = GetInt16();

                if (false == allownoweight)
                {
                    throw new NotSupportedException("allownoweight = false is not supported in TestLibrary with FEATURE_NOPINVOKES");
                }

                c = Convert.ToChar(iVal);
                loopCondition = allowsurrogate ? (!Char.IsLetter(c)) : (!Char.IsLetter(c) || Char.IsSurrogate(c));
            }
            while (loopCondition && 0 < counter);


            if (!Char.IsLetter(c))
            {
                // we tried and failed to get a letter
                //  Grab an ASCII letter
                c = Convert.ToChar(GetInt16() % 26 + 'A');
            }

            return c;
        }

        // returns a valid char that is a number
        public static char GetCharNumber(Int32 new_seed)
        {
            Seed = new_seed;
            return GetCharNumber();
        }
        public static char GetCharNumber()
        {
            return GetCharNumber(true);
        }

        // returns a valid char that is a number
        // if allownoweight is true, then no-weight characters are valid return values
        public static char GetCharNumber(Int32 new_seed, bool allownoweight)
        {
            Seed = new_seed;
            return GetCharNumber(allownoweight);
        }
        public static char GetCharNumber(bool allownoweight)
        {
            Char c = '0';
            Int32 counter;
            Int16 iVal;
            bool loopCondition = true;

            // attempt to randomly find a number
            counter = 100;
            do
            {
                counter--;
                iVal = GetInt16();

                if (false == allownoweight)
                {
                    throw new InvalidOperationException("allownoweight = false is not supported in TestLibrary with FEATURE_NOPINVOKES");
                }

                c = Convert.ToChar(iVal);
                loopCondition = !Char.IsNumber(c);
            }
            while (loopCondition && 0 < counter);

            if (!Char.IsNumber(c))
            {
                // we tried and failed to get a letter
                //  Grab an ASCII number
                c = Convert.ToChar(GetInt16() % 10 + '0');
            }

            return c;
        }

        // returns a valid char
        public static Char GetChar(Int32 new_seed)
        {
            Seed = new_seed;
            return GetChar();
        }
        public static Char GetChar()
        {
            return GetChar(true);
        }

        // returns a valid char
        // if allowsurrogate is true then surrogates are valid return values
        public static Char GetChar(Int32 new_seed, bool allowsurrogate)
        {
            Seed = new_seed;
            return GetChar(allowsurrogate);
        }
        public static Char GetChar(bool allowsurrogate)
        {
            return GetChar(allowsurrogate, true);
        }

        // returns a valid char
        // if allowsurrogate is true then surrogates are valid return values
        // if allownoweight characters then noweight characters are valid return values
        public static Char GetChar(Int32 new_seed, bool allowsurrogate, bool allownoweight)
        {
            Seed = new_seed;
            return GetChar(allowsurrogate, allownoweight);
        }
        public static Char GetChar(bool allowsurrogate, bool allownoweight)
        {
            Int16 iVal = GetInt16();

            Char c = (char)(iVal);

            if (!Char.IsLetter(c))
            {
                // we tried and failed to get a letter
                // Just grab an ASCII letter
                c = (char)(GetInt16() % 26 + 'A');
            }

            return c;
        }

        // returns a  string.  If "validPath" is set, only valid path characters
        //  will be included
        public static string GetString(Int32 new_seed, Boolean validPath, Int32 minLength, Int32 maxLength)
        {
            Seed = new_seed;
            return GetString(validPath, minLength, maxLength);
        }
        public static string GetString(Boolean validPath, Int32 minLength, Int32 maxLength)
        {
            return GetString(validPath, true, true, minLength, maxLength);
        }

        public static string GetString(Int32 new_seed, Boolean validPath, Boolean allowNulls, Int32 minLength, Int32 maxLength)
        {
            Seed = new_seed;
            return GetString(validPath, allowNulls, minLength, maxLength);
        }
        public static string GetString(Boolean validPath, Boolean allowNulls, Int32 minLength, Int32 maxLength)
        {
            return GetString(validPath, allowNulls, true, minLength, maxLength);
        }

        public static string GetString(Int32 new_seed, Boolean validPath, Boolean allowNulls, Boolean allowNoWeight, Int32 minLength, Int32 maxLength)
        {
            Seed = new_seed;
            return GetString(validPath, allowNulls, allowNoWeight, minLength, maxLength);
        }
        public static string GetString(Boolean validPath, Boolean allowNulls, Boolean allowNoWeight, Int32 minLength, Int32 maxLength)
        {
            StringBuilder sVal = new StringBuilder();
            Char c;
            Int32 length;

            if (0 == minLength && 0 == maxLength) return String.Empty;
            if (minLength > maxLength) return null;

            length = minLength;

            if (minLength != maxLength)
            {
                length = (GetInt32() % (maxLength - minLength)) + minLength;
            }

            for (int i = 0; length > i; i++)
            {
                if (validPath)
                {
                    if (0 == (GetByte() % 2))
                    {
                        c = GetCharLetter(true, allowNoWeight);
                    }
                    else
                    {
                        c = GetCharNumber(allowNoWeight);
                    }
                }
                else if (!allowNulls)
                {
                    do
                    {
                        c = GetChar(true, allowNoWeight);
                    } while (c == '\u0000');
                }
                else
                {
                    c = GetChar(true, allowNoWeight);
                }

                sVal.Append(c);
            }
            string s = sVal.ToString();

            return s;
        }

        public static string[] GetStrings(Int32 new_seed, Boolean validPath, Int32 minLength, Int32 maxLength)
        {
            Seed = new_seed;
            return GetStrings(validPath, minLength, maxLength);
        }
        public static string[] GetStrings(Boolean validPath, Int32 minLength, Int32 maxLength)
        {
            string validString;
            const char c_LATIN_A = '\u0100';
            const char c_LOWER_A = 'a';
            const char c_UPPER_A = 'A';
            const char c_ZERO_WEIGHT = '\uFEFF';
            const char c_DOUBLE_WIDE_A = '\uFF21';
            const string c_SURROGATE_UPPER = "\uD801\uDC00";
            const string c_SURROGATE_LOWER = "\uD801\uDC28";
            const char c_LOWER_SIGMA1 = (char)0x03C2;
            const char c_LOWER_SIGMA2 = (char)0x03C3;
            const char c_UPPER_SIGMA = (char)0x03A3;
            const char c_SPACE = ' ';
            int numConsts = 12;
            string[] retStrings;

            if (2 >= minLength && 2 >= maxLength || minLength > maxLength) return null;

            retStrings = new string[numConsts];

            validString = TestLibrary.Generator.GetString(validPath, minLength - 1, maxLength - 1);
            retStrings[0] = TestLibrary.Generator.GetString(validPath, minLength, maxLength);
            retStrings[1] = validString + c_LATIN_A;
            retStrings[2] = validString + c_LOWER_A;
            retStrings[3] = validString + c_UPPER_A;
            retStrings[4] = validString + c_ZERO_WEIGHT;
            retStrings[5] = validString + c_DOUBLE_WIDE_A;
            retStrings[6] = TestLibrary.Generator.GetString(validPath, minLength - 2, maxLength - 2) + c_SURROGATE_UPPER;
            retStrings[7] = TestLibrary.Generator.GetString(validPath, minLength - 2, maxLength - 2) + c_SURROGATE_LOWER;
            retStrings[8] = validString + c_LOWER_SIGMA1;
            retStrings[9] = validString + c_LOWER_SIGMA2;
            retStrings[10] = validString + c_UPPER_SIGMA;
            retStrings[11] = validString + c_SPACE;

            return retStrings;
        }

        public static object GetType(Type t)
        {
            return Activator.CreateInstance(t);
        }
    }
}
