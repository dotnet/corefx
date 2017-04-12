using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace DesktopTestData
{
    public static class PrimitiveCreator
    {
        static Dictionary<Type, MethodInfo> Creators;
        static PrimitiveCreator()
        {
            Type primitiveCreatorType = typeof(PrimitiveCreator);
            Creators = new Dictionary<Type, MethodInfo>();
            Creators.Add(typeof(Boolean), primitiveCreatorType.GetMethod("CreateInstanceOfBoolean", BindingFlags.Public | BindingFlags.Static));
            Creators.Add(typeof(Byte), primitiveCreatorType.GetMethod("CreateInstanceOfByte", BindingFlags.Public | BindingFlags.Static));
            Creators.Add(typeof(Char), primitiveCreatorType.GetMethod("CreateInstanceOfChar", BindingFlags.Public | BindingFlags.Static));
            Creators.Add(typeof(DateTime), primitiveCreatorType.GetMethod("CreateInstanceOfDateTime", BindingFlags.Public | BindingFlags.Static));
            Creators.Add(typeof(DateTimeOffset), primitiveCreatorType.GetMethod("CreateInstanceOfDateTimeOffset", BindingFlags.Public | BindingFlags.Static));
            Creators.Add(typeof(DBNull), primitiveCreatorType.GetMethod("CreateInstanceOfDBNull", BindingFlags.Public | BindingFlags.Static));
            Creators.Add(typeof(Decimal), primitiveCreatorType.GetMethod("CreateInstanceOfDecimal", BindingFlags.Public | BindingFlags.Static));
            Creators.Add(typeof(Double), primitiveCreatorType.GetMethod("CreateInstanceOfDouble", BindingFlags.Public | BindingFlags.Static));
            Creators.Add(typeof(Guid), primitiveCreatorType.GetMethod("CreateInstanceOfGuid", BindingFlags.Public | BindingFlags.Static));
            Creators.Add(typeof(Int16), primitiveCreatorType.GetMethod("CreateInstanceOfInt16", BindingFlags.Public | BindingFlags.Static));
            Creators.Add(typeof(Int32), primitiveCreatorType.GetMethod("CreateInstanceOfInt32", BindingFlags.Public | BindingFlags.Static));
            Creators.Add(typeof(Int64), primitiveCreatorType.GetMethod("CreateInstanceOfInt64", BindingFlags.Public | BindingFlags.Static));
            Creators.Add(typeof(Object), primitiveCreatorType.GetMethod("CreateInstanceOfObject", BindingFlags.Public | BindingFlags.Static));
            Creators.Add(typeof(SByte), primitiveCreatorType.GetMethod("CreateInstanceOfSByte", BindingFlags.Public | BindingFlags.Static));
            Creators.Add(typeof(Single), primitiveCreatorType.GetMethod("CreateInstanceOfSingle", BindingFlags.Public | BindingFlags.Static));
            Creators.Add(typeof(String), primitiveCreatorType.GetMethod("CreateInstanceOfString", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(Random) }, null));
            Creators.Add(typeof(TimeSpan), primitiveCreatorType.GetMethod("CreateInstanceOfTimeSpan", BindingFlags.Public | BindingFlags.Static));
            Creators.Add(typeof(UInt16), primitiveCreatorType.GetMethod("CreateInstanceOfUInt16", BindingFlags.Public | BindingFlags.Static));
            Creators.Add(typeof(UInt32), primitiveCreatorType.GetMethod("CreateInstanceOfUInt32", BindingFlags.Public | BindingFlags.Static));
            Creators.Add(typeof(UInt64), primitiveCreatorType.GetMethod("CreateInstanceOfUInt64", BindingFlags.Public | BindingFlags.Static));
            Creators.Add(typeof(Uri), primitiveCreatorType.GetMethod("CreateInstanceOfUri", BindingFlags.Public | BindingFlags.Static));
            Creators.Add(typeof(XmlQualifiedName), primitiveCreatorType.GetMethod("CreateInstanceOfXmlQualifiedName", BindingFlags.Public | BindingFlags.Static));
            Creators.Add(typeof(Stream), primitiveCreatorType.GetMethod("CreateInstanceOfStream", BindingFlags.Public | BindingFlags.Static));



#if !SILVERLIGHT
            Creators.Add(typeof(XmlElement), primitiveCreatorType.GetMethod("CreateInstanceOfXmlElement", BindingFlags.Public | BindingFlags.Static));
            Creators.Add(typeof(XmlNode[]), primitiveCreatorType.GetMethod("CreateInstanceOfXmlNodeArray", BindingFlags.Public | BindingFlags.Static));
            Creators.Add(typeof(DataTable), primitiveCreatorType.GetMethod("CreateInstanceOfDataTable", BindingFlags.Public | BindingFlags.Static));
            Creators.Add(typeof(DataSet), primitiveCreatorType.GetMethod("CreateInstanceOfDataSet", BindingFlags.Public | BindingFlags.Static));

#endif
        }
        public static System.Boolean CreateInstanceOfBoolean(Random rndGen)
        {
            return (rndGen.Next(2) == 0);
        }

        public static System.Byte CreateInstanceOfByte(Random rndGen)
        {
            byte[] rndValue = new byte[1];
            rndGen.NextBytes(rndValue);
            return rndValue[0];
        }

        public static System.Char CreateInstanceOfChar(Random rndGen)
        {
            if (CreatorSettings.CreateOnlyAsciiChars)
            {
                return (Char)rndGen.Next(0x20, 0x7F);
            }
            else if (CreatorSettings.DontCreateSurrogateChars)
            {
                char c;
                do
                {
                    c = (Char)rndGen.Next((int)Char.MinValue, (int)Char.MaxValue);
                } while (Char.IsSurrogate(c));
                return c;
            }
            else
            {
                return (Char)rndGen.Next((int)Char.MinValue, (int)Char.MaxValue + 1);
            }
        }

        public static System.DateTime CreateInstanceOfDateTime(Random rndGen)
        {
            long temp = CreateInstanceOfInt64(rndGen);
            temp = Math.Abs(temp);
            DateTime result;
            try
            {
                result = new DateTime(temp % (DateTime.MaxValue.Ticks + 1));
            }
            catch (ArgumentOutOfRangeException) // jasonv - approved; specific, commented
            {
                // From http://msdn.microsoft.com/en-us/library/z2xf7zzk.aspx
                // ticks is less than MinValue or greater than MaxValue. 
                result = DateTime.Now;
            }

            int kind = rndGen.Next(3);
            switch (kind)
            {
                case 0:
                    result = DateTime.SpecifyKind(result, DateTimeKind.Local);
                    break;
                case 1:
                    result = DateTime.SpecifyKind(result, DateTimeKind.Unspecified);
                    break;
                default:
                    result = DateTime.SpecifyKind(result, DateTimeKind.Utc);
                    break;
            }

            if (!CreatorSettings.CreateDateTimeWithSubMilliseconds)
            {
                result = new DateTime(result.Year, result.Month, result.Day,
                    result.Hour, result.Minute, result.Second, result.Millisecond, result.Kind);
            }

            return result;
        }

        public static System.DateTimeOffset CreateInstanceOfDateTimeOffset(Random rndGen)
        {
            DateTime temp = CreateInstanceOfDateTime(rndGen);
            temp = DateTime.SpecifyKind(temp, DateTimeKind.Unspecified);
            int offsetMinutes = rndGen.Next(-14 * 60, 14 * 60);
            DateTimeOffset result = new DateTimeOffset(temp, TimeSpan.FromMinutes(offsetMinutes));
            return result;
        }

        public static System.DBNull CreateInstanceOfDBNull(Random rndGen)
        {
            return (rndGen.Next(2) == 0) ? null : DBNull.Value;
        }

        public static System.Decimal CreateInstanceOfDecimal(Random rndGen)
        {
            int low = CreateInstanceOfInt32(rndGen);
            int mid = CreateInstanceOfInt32(rndGen);
            int high = CreateInstanceOfInt32(rndGen);
            bool isNegative = (rndGen.Next(2) == 0);
            const int maxDecimalScale = 28;
            byte scale = (byte)rndGen.Next(0, maxDecimalScale + 1);
            return new Decimal(low, mid, high, isNegative, scale);
        }

        public static System.Double CreateInstanceOfDouble(Random rndGen)
        {
            bool negative = (rndGen.Next(2) == 0);
            int temp = rndGen.Next(40);
            Double result;
            switch (temp)
            {
                case 0: return Double.NaN;
                case 1: return Double.PositiveInfinity;
                case 2: return Double.NegativeInfinity;
                case 3: return Double.MinValue;
                case 4: return Double.MaxValue;
                case 5: return Double.Epsilon;
                default:
                    result = (Double)(rndGen.NextDouble() * 100000);
                    if (negative) result = -result;
                    return result;
            }
        }

        public static System.Guid CreateInstanceOfGuid(Random rndGen)
        {
            byte[] temp = new byte[16];
            rndGen.NextBytes(temp);
            return new Guid(temp);
        }

        public static System.Int16 CreateInstanceOfInt16(Random rndGen)
        {
            byte[] rndValue = new byte[2];
            rndGen.NextBytes(rndValue);
            Int16 result = 0;
            for (int i = 0; i < rndValue.Length; i++)
            {
                result = (Int16)(result << 8);
                result = (Int16)(result | (Int16)rndValue[i]);
            }
            return result;
        }

        public static System.Int32 CreateInstanceOfInt32(Random rndGen)
        {
            byte[] rndValue = new byte[4];
            rndGen.NextBytes(rndValue);
            Int32 result = 0;
            for (int i = 0; i < rndValue.Length; i++)
            {
                result = (Int32)(result << 8);
                result = (Int32)(result | (Int32)rndValue[i]);
            }
            return result;
        }

        public static System.Int64 CreateInstanceOfInt64(Random rndGen)
        {
            byte[] rndValue = new byte[8];
            rndGen.NextBytes(rndValue);
            Int64 result = 0;
            for (int i = 0; i < rndValue.Length; i++)
            {
                result = (Int64)(result << 8);
                result = (Int64)(result | (Int64)rndValue[i]);
            }
            return result;
        }

        public static System.Object CreateInstanceOfObject(Random rndGen)
        {
            return (rndGen.Next(5) == 0) ? null : new Object();
        }

        public static System.SByte CreateInstanceOfSByte(Random rndGen)
        {
            byte[] rndValue = new byte[1];
            rndGen.NextBytes(rndValue);
            SByte result = (SByte)rndValue[0];
            return result;
        }

        public static System.Single CreateInstanceOfSingle(Random rndGen)
        {
            bool negative = (rndGen.Next(2) == 0);
            int temp = rndGen.Next(40);
            Single result;
            switch (temp)
            {
                case 0: return Single.NaN;
                case 1: return Single.PositiveInfinity;
                case 2: return Single.NegativeInfinity;
                case 3: return Single.MinValue;
                case 4: return Single.MaxValue;
                case 5: return Single.Epsilon;
                default:
                    result = (Single)(rndGen.NextDouble() * 100000);
                    if (negative) result = -result;
                    return result;
            }
        }
        internal static string CreateRandomString(Random rndGen, int size, string charsToUse)
        {
            int maxSize = CreatorSettings.MaxStringLength;
            // invalid per the XML spec (http://www.w3.org/TR/REC-xml/#charsets), cannot be sent as XML
            const string InvalidXmlChars = "\u0000\u0001\u0002\u0003\u0004\u0005\u0006\u0007\u0008\u000B\u000C\u000E\u000F\u0010\u0011\u0012\u0013\u0014\u0015\u0016\u0017\u0018\u0019\u001A\u001B\u001C\u001D\u001E\u001F\uFFFE\uFFFF";
            const int LowSurrogateMin = 0xDC00;
            const int LowSurrogateMax = 0xDFFF;
            const int HighSurrogateMin = 0xD800;
            const int HighSurrogateMax = 0xDBFF;

            if (size < 0)
            {
                double rndNumber = rndGen.NextDouble();
                if (rndNumber < CreatorSettings.NullValueProbability) return null; // 1% chance of null value
                size = (int)Math.Pow(maxSize, rndNumber); // this will create more small strings than large ones
                size--;
            }
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < size; i++)
            {
                char c;
                if (charsToUse != null)
                {
                    c = charsToUse[rndGen.Next(charsToUse.Length)];
                    sb.Append(c);
                }
                else
                {
                    if (CreatorSettings.CreateOnlyAsciiChars || rndGen.Next(2) == 0)
                    {
                        c = (char)rndGen.Next(0x20, 0x7F); // low-ascii chars
                        sb.Append(c);
                    }
                    else
                    {
                        do
                        {
                            c = (char)rndGen.Next((int)char.MinValue, (int)char.MaxValue + 1);
                        } while ((LowSurrogateMin <= c && c <= LowSurrogateMax) || (InvalidXmlChars.IndexOf(c) >= 0));
                        sb.Append(c);
                        if (HighSurrogateMin <= c && c <= HighSurrogateMax) // need to add a low surrogate
                        {
                            c = (char)rndGen.Next(LowSurrogateMin, LowSurrogateMax + 1);
                            sb.Append(c);
                        }
                    }
                }
            }
            return sb.ToString();
        }
        public static System.String CreateInstanceOfString(Random rndGen)
        {
            double rndNumber = rndGen.NextDouble();
            if (rndNumber < CreatorSettings.NullValueProbability)
            {
                return null;
            }
            return CreateRandomString(rndGen, -1, null);
        }

        public static System.String CreateInstanceOfString(Random rndGen, int size, string charsToUse)
        {
            return CreateRandomString(rndGen, size, charsToUse);
        }

        public static System.TimeSpan CreateInstanceOfTimeSpan(Random rndGen)
        {
            long temp = CreateInstanceOfInt64(rndGen);
            TimeSpan result = TimeSpan.FromTicks(temp);
            return result;
        }

        public static System.UInt16 CreateInstanceOfUInt16(Random rndGen)
        {
            byte[] rndValue = new byte[2];
            rndGen.NextBytes(rndValue);
            UInt16 result = 0;
            for (int i = 0; i < rndValue.Length; i++)
            {
                result = (UInt16)(result << 8);
                result = (UInt16)(result | (UInt16)rndValue[i]);
            }
            return result;
        }

        public static System.UInt32 CreateInstanceOfUInt32(Random rndGen)
        {
            byte[] rndValue = new byte[4];
            rndGen.NextBytes(rndValue);
            UInt32 result = 0;
            for (int i = 0; i < rndValue.Length; i++)
            {
                result = (UInt32)(result << 8);
                result = (UInt32)(result | (UInt32)rndValue[i]);
            }
            return result;
        }

        public static System.UInt64 CreateInstanceOfUInt64(Random rndGen)
        {
            byte[] rndValue = new byte[8];
            rndGen.NextBytes(rndValue);
            UInt64 result = 0;
            for (int i = 0; i < rndValue.Length; i++)
            {
                result = (UInt64)(result << 8);
                result = (UInt64)(result | (UInt64)rndValue[i]);
            }
            return result;
        }

        /// <summary>
        /// Creates URI instances based on RFC 2396
        /// </summary>
        internal static class UriCreator
        {
            static readonly string digit;
            static readonly string upalpha;
            static readonly string lowalpha;
            static readonly string alpha;
            static readonly string alphanum;
            static readonly string hex;
            static readonly string mark;
            static readonly string unreserved;
            static readonly string reserved;

            static UriCreator()
            {
                digit = "0123456789";
                upalpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                lowalpha = upalpha.ToLower();
                alpha = upalpha + lowalpha;
                alphanum = alpha + digit;
                hex = digit + "ABCDEFabcdef";
                mark = "-_.!~*'()";
                unreserved = alphanum + mark;
                reserved = ";/?:@&=+$,";
            }
            private static void CreateScheme(StringBuilder sb, Random rndGen)
            {
                int size = rndGen.Next(1, 10);
                AddChars(sb, rndGen, alpha, 1);
                string schemeChars = alpha + digit + "+-.";
                AddChars(sb, rndGen, schemeChars, size);
                sb.Append(':');
            }
            private static void CreateIPv4Address(StringBuilder sb, Random rndGen)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (i > 0) sb.Append('.');
                    sb.Append(rndGen.Next(1000));
                }
            }
            private static void AddIPv6AddressPart(StringBuilder sb, Random rndGen)
            {
                int size = rndGen.Next(1, 10);
                if (size > 4) size = 4;
                AddChars(sb, rndGen, hex, size);
            }
            private static void CreateIPv6Address(StringBuilder sb, Random rndGen)
            {
                sb.Append('[');
                int temp = rndGen.Next(6);
                int i;
                switch (temp)
                {
                    case 0:
                        sb.Append("::");
                        break;
                    case 1:
                        sb.Append("::1");
                        break;
                    case 2:
                        sb.Append("FF01::101");
                        break;
                    case 3:
                        sb.Append("::1");
                        break;
                    case 4:
                        for (i = 0; i < 3; i++)
                        {
                            AddIPv6AddressPart(sb, rndGen);
                            sb.Append(':');
                        }
                        for (i = 0; i < 3; i++)
                        {
                            sb.Append(':');
                            AddIPv6AddressPart(sb, rndGen);
                        }
                        break;
                    default:
                        for (i = 0; i < 8; i++)
                        {
                            if (i > 0) sb.Append(':');
                            AddIPv6AddressPart(sb, rndGen);
                        }
                        break;
                }
                sb.Append(']');
            }
            private static void AddChars(StringBuilder sb, Random rndGen, string validChars, int size)
            {
                for (int i = 0; i < size; i++)
                {
                    sb.Append(validChars[rndGen.Next(validChars.Length)]);
                }
            }
            private static void CreateHostName(StringBuilder sb, Random rndGen)
            {
                int domainLabelCount = rndGen.Next(4);
                int size;
                for (int i = 0; i < domainLabelCount; i++)
                {
                    AddChars(sb, rndGen, alphanum, 1);
                    size = rndGen.Next(10) - 1;
                    if (size > 0)
                    {
                        AddChars(sb, rndGen, alphanum + "-", size);
                        AddChars(sb, rndGen, alphanum, 1);
                    }
                    sb.Append('.');
                }
                AddChars(sb, rndGen, alpha, 1);
                size = rndGen.Next(10) - 1;
                if (size > 0)
                {
                    AddChars(sb, rndGen, alphanum + "-", size);
                    AddChars(sb, rndGen, alphanum, 1);
                }
            }
            private static void CreateHost(StringBuilder sb, Random rndGen)
            {
                int temp = rndGen.Next(3);
                switch (temp)
                {
                    case 0:
                        CreateIPv4Address(sb, rndGen);
                        break;
                    case 1:
                        CreateIPv6Address(sb, rndGen);
                        break;
                    case 2:
                        CreateHostName(sb, rndGen);
                        break;
                }
            }
            private static void CreateUserInfo(StringBuilder sb, Random rndGen)
            {
                AddChars(sb, rndGen, alpha, rndGen.Next(1, 10));
                if (rndGen.Next(3) > 0)
                {
                    sb.Append(':');
                    AddChars(sb, rndGen, alpha, rndGen.Next(1, 10));
                }
                sb.Append('@');
            }
            private static void AddEscapedChar(StringBuilder sb, Random rndGen)
            {
                sb.Append('%');
                AddChars(sb, rndGen, hex, 2);
            }
            private static void AddPathSegment(StringBuilder sb, Random rndGen)
            {
                string pchar = unreserved + ":@&=+$,";
                int size = rndGen.Next(1, 10);
                for (int i = 0; i < size; i++)
                {
                    if (rndGen.Next(pchar.Length + 1) > 0)
                    {
                        AddChars(sb, rndGen, pchar, 1);
                    }
                    else
                    {
                        AddEscapedChar(sb, rndGen);
                    }
                }
            }
            private static void AddUriC(StringBuilder sb, Random rndGen)
            {
                int size = rndGen.Next(20);
                string reservedPlusUnreserved = reserved + unreserved;
                for (int i = 0; i < size; i++)
                {
                    if (rndGen.Next(5) > 0)
                    {
                        AddChars(sb, rndGen, reservedPlusUnreserved, 1);
                    }
                    else
                    {
                        AddEscapedChar(sb, rndGen);
                    }
                }
            }
            internal static string CreateUri(Random rndGen, out UriKind kind)
            {
                StringBuilder sb = new StringBuilder();
                //kind = UriKind.Relative;
                //Devdiv bug 187103
                kind = UriKind.Absolute;
                if (rndGen.Next(3) > 0)
                {
                    // Add URI scheme
                    CreateScheme(sb, rndGen);
                    kind = UriKind.Absolute;
                }
                if (rndGen.Next(3) > 0)
                {
                    // Add URI host
                    sb.Append("//");
                    if (rndGen.Next(10) == 0)
                    {
                        CreateUserInfo(sb, rndGen);
                    }
                    CreateHost(sb, rndGen);
                    if (rndGen.Next(2) > 0)
                    {
                        sb.Append(':');
                        sb.Append(rndGen.Next(65536));
                    }
                }
                if (rndGen.Next(4) > 0)
                {
                    // Add URI path
                    for (int i = 0; i < rndGen.Next(1, 4); i++)
                    {
                        sb.Append('/');
                        AddPathSegment(sb, rndGen);
                    }
                }
                if (rndGen.Next(3) == 0)
                {
                    // Add URI query string
                    sb.Append('?');
                    AddUriC(sb, rndGen);
                }
                return sb.ToString();
            }
        }

        public static System.Uri CreateInstanceOfUri(Random rndGen)
        {
            Uri result;
            UriKind kind;
            try
            {
                string uriString = UriCreator.CreateUri(rndGen, out kind);
                result = new Uri(uriString, kind);
            }
            catch (ArgumentException) // jasonv - approved; specific, commented
            {
                // From http://msdn.microsoft.com/en-us/library/ms131565.aspx
                // uriKind is invalid.
                result = new Uri("my.schema://userName:password@my.domain/path1/path2?query1=123&query2=%22hello%22");
            }
            return result;
        }

        public static string CreateInstanceOfUriString(Random rndGen)
        {
            UriKind kind;
            return UriCreator.CreateUri(rndGen, out kind);
        }

        public static XmlQualifiedName CreateInstanceOfXmlQualifiedName(Random rndGen)
        {
            if (rndGen.Next(20) == 0) return new XmlQualifiedName();
            StringBuilder sb = new StringBuilder();
            int localNameLength = rndGen.Next(1, 30);
            //TODO: Expand to include int'l chars
            const string LocalNameStartChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_";
            string LocalNameChars = LocalNameStartChars + "0123456789.-";
            string NamespaceChars = LocalNameChars + ":";
            for (int i = 0; i < localNameLength; i++)
            {
                if (i == 0)
                {
                    sb.Append(LocalNameStartChars[rndGen.Next(LocalNameStartChars.Length)]);
                }
                else
                {
                    sb.Append(LocalNameChars[rndGen.Next(LocalNameChars.Length)]);
                }
            }
            string localName = sb.ToString();
            if (rndGen.Next(3) == 0) return new XmlQualifiedName(localName);
            sb.Length = 0;
            int namespaceUriLength = rndGen.Next(1, 40);
            for (int i = 0; i < namespaceUriLength; i++)
            {
                sb.Append(NamespaceChars[rndGen.Next(NamespaceChars.Length)]);
            }
            string namespaceUri = sb.ToString();
            return new XmlQualifiedName(localName, namespaceUri);
        }

        public static Stream CreateInstanceOfStream(Random rndGen)
        {
            string data = (string)InstanceCreator.CreateInstanceOf(typeof(String), rndGen);
            Stream inputStream = new MemoryStream();
            byte[] bytes = Encoding.UTF8.GetBytes(data.ToCharArray());
            inputStream.Write(bytes, 0, bytes.Length);
            inputStream.Position = 0;
            return inputStream;
        }

#if !SILVERLIGHT
        const string TemplateXmlDocument =
            "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n" +
            "<Bookstore name=\"Contoso\">\r\n" +
            "\t<cts:Inventory xmlns:cts=\"http://www.contoso.org/Bookstore\" xmlns=\"http://www.contoso.org/Bookstore/BookInfo\">\r\n" +
            "\t\t<cts:Book isbn=\"0735621519\">\r\n" +
            "\t\t\t<Author>David Pallman</Author>\r\n" +
            "\t\t\t<Title>Programming \"Indigo\"</Title>\r\n" +
            "\t\t\t<Subtitle><![CDATA[The code name for the Unified Framework for building service-oriented applications on the Microsoft Windows Platform]]></Subtitle>\r\n" +
            "\t\t</cts:Book>\r\n" +
            "\t\t<cts:Book isbn=\"0672328771\">\r\n" +
            "\t\t\t<Author>Craig McMurtry</Author>\r\n" +
            "\t\t\t<Author>Marc Mercuri</Author>\r\n" +
            "\t\t\t<Author>Nigel Watling</Author>\r\n" +
            "\t\t\t<Title>Microsoft Windows Communication Foundation: Hands-on</Title>\r\n" +
            "\t\t\t<Subtitle><![CDATA[Beta Edition]]></Subtitle>\r\n" +
            "\t\t</cts:Book>\r\n" +
            "\t\t<cts:Book isbn=\"0735623066\">\r\n" +
            "\t\t\t<Author>Justin Smith</Author>\r\n" +
            "\t\t\t<Title>Inside Microsoft Windows Communication Foundation</Title>\r\n" +
            "\t\t</cts:Book>\r\n" +
            "\t\t<cts:Book isbn=\"1590597028\">\r\n" +
            "\t\t\t<Author>Chris Peiris</Author>\r\n" +
            "\t\t\t<Author>Dennis Mulder</Author>\r\n" +
            "\t\t\t<Author>Amit Bahree</Author>\r\n" +
            "\t\t\t<Author>Aftab Chopra</Author>\r\n" +
            "\t\t\t<Author>Shawn Cicoria</Author>\r\n" +
            "\t\t\t<Author>Nishith Pathak</Author>\r\n" +
            "\t\t\t<Title>Pro WCF: Practical Microsoft SOA Implementation</Title>\r\n" +
            "\t\t\t<Publisher>Apress</Publisher>\r\n" +
            "\t\t</cts:Book>\r\n" +
            "\t\t<cts:Book isbn=\"0321399838\">\r\n" +
            "\t\t\t<Author>Dharma Shukla</Author>\r\n" +
            "\t\t\t<Author>Bob Schmidt</Author>\r\n" +
            "\t\t\t<Title>Essential Windows Workflow Foundation</Title>\r\n" +
            "\t\t\t<Subtitle><![CDATA[(Microsoft .NET Development Series)]]></Subtitle>\r\n" +
            "\t\t\t<Reviews>\r\n" +
            "\t\t\t\t<Review date=\"November 9, 2006\">\r\n" +
            "\t\t\t\t\t<Reviewer>BERNARDO H. N. SILVA \"Bernardo Heynemann\"</Reviewer>\r\n" +
            "\t\t\t\t\t<Caption>Absolutely Must Read</Caption>\r\n" +
            "\t\t\t\t\t<Detail>\r\n" +
            "\t\t\t\t\t\t<![CDATA[By the end of the first chapter I found myself with a \"W00000000T?!?\" face! This book is insanely GOOD! \r\n" +
            "I'm learning a lot about the Foundation behind the Workflow. \r\n" +
            "The authors take WF and break it down by chapters. Each chapter dissects a piece of the Foundation. \r\n" +
            "An absolutely must-read for anyone working now with Windows Workflow Foundation (WF).]]>\r\n" +
            "\t\t\t\t\t</Detail>\r\n" +
            "\t\t\t\t</Review>\r\n" +
            "\t\t\t\t<Review date=\"November 2, 2006\">\r\n" +
            "\t\t\t\t\t<Reviewer>W Boudville (US)</Reviewer>\r\n" +
            "\t\t\t\t\t<Caption>takes scheduling of programs to another level</Caption>\r\n" +
            "\t\t\t\t\t<Detail>\r\n" +
            "\t\t\t\t\t\t<![CDATA[WWF encapsulates some intriguing abilities that were hitherto not available in C#/.NET, or in the competing Java environment. Or at least not easily available. In both areas, there has already been the concept of serialisation. Where you can write code from memory to disk in a manner such that the code's classes can be read back as functioning binaries, at some later time. Both also have transactions and threads. \r\n" +
            "WWF takes those ideas and merges them. The authors show how this results in the concept of a resumable program. The core idea in WWF. So a runtime program can be passivated (the equivalent of the earlier serialisation idea), and given a globally unique id. Then, a special Runtime program can de-passivate the program and run it, at some future time. In essence, it gets around the conundrum that when a conventional program, in any language, ends, then it ends. You needed to write custom code in another program, that could invoke the first, in some fashion. Very clumsy and error prone. WWF provides a declarative and robust way to transcend the ending of a program. Takes scheduling to the next level. \r\n" +
            "Plus, the book shows that the de-passivating of a resumable program can be done on another machine, that has access to the medium in which the program was passivated. (This was the point of using a globally unique id for the passivated program.) Obvious implications for load balancing and robustness design.]]></Detail>\r\n" +
            "\t\t\t\t</Review>\r\n" +
            "\t\t\t</Reviews>\r\n" +
            "\t\t</cts:Book>\r\n" +
            "\t</cts:Inventory>\r\n" +
            "</Bookstore>\r\n";

        public static XmlElement CreateInstanceOfXmlElement(Random rndGen)
        {
            XmlDocument doc = new XmlDocument();
            string docString = TemplateXmlDocument;
            if (CreatorSettings.NormalizeEndOfLineOnXmlNodes)
            {
                docString = docString.Replace("\r\n", "\n").Replace("\r", "\n");
            }
            doc.LoadXml(docString);
            if (rndGen.Next(3) == 0)
            {
                return doc.DocumentElement;
            }
            XmlNamespaceManager nsManager = new XmlNamespaceManager(doc.NameTable);
            nsManager.AddNamespace("book", "http://www.contoso.org/Bookstore");
            XmlNodeList nodes = doc.SelectNodes("//book:Book", nsManager);
            return nodes[rndGen.Next(nodes.Count)] as XmlElement;
        }

        public static XmlNode[] CreateInstanceOfXmlNodeArray(Random rndGen)
        {
            XmlDocument doc = new XmlDocument();
            string docString = TemplateXmlDocument;
            if (CreatorSettings.NormalizeEndOfLineOnXmlNodes)
            {
                docString = docString.Replace("\r\n", "\n").Replace("\r", "\n");
            }
            doc.LoadXml(docString);
            List<XmlNode> result = new List<XmlNode>();
            XmlNamespaceManager nsManager = new XmlNamespaceManager(doc.NameTable);
            nsManager.AddNamespace("book", "http://www.contoso.org/Bookstore");
            XmlNodeList nodes = doc.SelectNodes("//book:Book", nsManager);
            for (int i = 0; i < nodes.Count; i++)
            {
                if (rndGen.Next(2) == 0) result.Add(nodes[i]);
            }
            if (rndGen.Next(5) == 0) // we'll repeat some nodes
            {
                for (int i = 0; i < nodes.Count; i++)
                {
                    if (rndGen.Next(2) == 0) result.Add(nodes[i]);
                }
            }
            return result.ToArray();
        }

        public static DataTable CreateInstanceOfDataTable(Random rndGen)
        {

            string tableName = CreateInstanceOfString(rndGen);

            while (String.IsNullOrEmpty(tableName))
            {
                tableName = CreateInstanceOfString(rndGen);
            }
            string tableNamespace = null;
            if (rndGen.Next(4) == 0)
            {
                do
                {
                    tableNamespace = CreateInstanceOfString(rndGen);
                    if (null != tableNamespace)
                    {
                        tableNamespace = tableNamespace.Trim();
                    }
                    //Blank namespace resultsin: System.Xml.Schema.XmlSchemaException:"The Namespace ' ' is an invalid URI."}
                }
                while (String.IsNullOrEmpty(tableNamespace));
            }
            DataTable result = new DataTable(tableName, tableNamespace);
            Type[] types = new Type[] {
                typeof(string),
                typeof(DateTime),
                typeof(int),
                typeof(decimal),
                typeof(double),
                typeof(Uri),
            };
            int columnCount = rndGen.Next(6);
            Type[] columnTypes = new Type[columnCount];
            string columnNameChars = "abcdefghijklmnopqrstuvwxwz1234567890";
            for (int i = 0; i < columnCount; i++)
            {
                int columnNameLen = rndGen.Next(5, 10);
                string columnName = CreateInstanceOfString(rndGen, columnNameLen, columnNameChars);
                columnTypes[i] = types[rndGen.Next(types.Length)];
                result.Columns.Add(columnName, columnTypes[i]);
            }
            if (columnCount > 0)
            {
                int rowCount = rndGen.Next(CreatorSettings.MaxListLength);
                for (int i = 0; i < rowCount; i++)
                {
                    object[] rowElements = new object[columnCount];
                    for (int j = 0; j < columnCount; j++)
                    {
                        rowElements[j] = CreatePrimitiveInstance(columnTypes[j], rndGen);
                    }
                    result.Rows.Add(rowElements);
                }
            }
            return result;
        }

        public static DataSet CreateInstanceOfDataSet(Random rndGen)
        {
            string datasetName = CreateInstanceOfString(rndGen);
            while (String.IsNullOrEmpty(datasetName))
            {
                datasetName = CreateInstanceOfString(rndGen);
            }
            DataSet set = new DataSet(datasetName);
            int tableCount = rndGen.Next(6);
            if (tableCount == 0) tableCount = 1;
            for (int i = 0; i < tableCount; i++)
            {
                set.Tables.Add(PrimitiveCreator.CreateInstanceOfDataTable(rndGen));
            }
            return set;
        }

#endif

        public static bool CanCreateInstanceOf(Type type)
        {
            return Creators.ContainsKey(type);
        }
        public static object CreatePrimitiveInstance(Type type, Random rndGen)
        {
            if (Creators.ContainsKey(type))
            {
                return Creators[type].Invoke(null, new object[] { rndGen });
            }
            else
            {
                throw new ArgumentException("Type " + type.FullName + " not supported");
            }
        }
    }

}
