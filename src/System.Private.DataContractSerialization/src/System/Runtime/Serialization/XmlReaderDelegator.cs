// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Xml;
using System.Globalization;
using System.Collections.Generic;
using System.Xml.Serialization;


namespace System.Runtime.Serialization
{
#if USE_REFEMIT || uapaot
    public class XmlReaderDelegator
#else
    internal class XmlReaderDelegator
#endif
    {
        protected XmlReader reader;
        protected XmlDictionaryReader dictionaryReader;
        protected bool isEndOfEmptyElement = false;

        public XmlReaderDelegator(XmlReader reader)
        {
            XmlObjectSerializer.CheckNull(reader, nameof(reader));
            this.reader = reader;
            this.dictionaryReader = reader as XmlDictionaryReader;
        }

        internal XmlReader UnderlyingReader
        {
            get { return reader; }
        }

        internal ExtensionDataReader UnderlyingExtensionDataReader
        {
            get { return reader as ExtensionDataReader; }
        }

        internal int AttributeCount
        {
            get { return isEndOfEmptyElement ? 0 : reader.AttributeCount; }
        }

        internal string GetAttribute(string name)
        {
            return isEndOfEmptyElement ? null : reader.GetAttribute(name);
        }

        internal string GetAttribute(string name, string namespaceUri)
        {
            return isEndOfEmptyElement ? null : reader.GetAttribute(name, namespaceUri);
        }

        internal string GetAttribute(int i)
        {
            if (isEndOfEmptyElement)
                throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(i), SR.Format(SR.XmlElementAttributes)));
            return reader.GetAttribute(i);
        }

        internal bool IsEmptyElement
        {
            get { return false; }
        }

        internal bool IsNamespaceURI(string ns)
        {
            if (dictionaryReader == null)
                return ns == reader.NamespaceURI;
            else
                return dictionaryReader.IsNamespaceUri(ns);
        }

        internal bool IsLocalName(string localName)
        {
            if (dictionaryReader == null)
                return localName == reader.LocalName;
            else
                return dictionaryReader.IsLocalName(localName);
        }

        internal bool IsNamespaceUri(XmlDictionaryString ns)
        {
            if (dictionaryReader == null)
                return ns.Value == reader.NamespaceURI;
            else
                return dictionaryReader.IsNamespaceUri(ns);
        }

        internal bool IsLocalName(XmlDictionaryString localName)
        {
            if (dictionaryReader == null)
                return localName.Value == reader.LocalName;
            else
                return dictionaryReader.IsLocalName(localName);
        }

        internal int IndexOfLocalName(XmlDictionaryString[] localNames, XmlDictionaryString ns)
        {
            if (dictionaryReader != null)
                return dictionaryReader.IndexOfLocalName(localNames, ns);

            if (reader.NamespaceURI == ns.Value)
            {
                string localName = this.LocalName;
                for (int i = 0; i < localNames.Length; i++)
                {
                    if (localName == localNames[i].Value)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

#if USE_REFEMIT        
        public bool IsStartElement()
#else
        internal bool IsStartElement()
#endif
        {
            return !isEndOfEmptyElement && reader.IsStartElement();
        }

        internal bool IsStartElement(string localname, string ns)
        {
            return !isEndOfEmptyElement && reader.IsStartElement(localname, ns);
        }

#if USE_REFEMIT        
        public bool IsStartElement(XmlDictionaryString localname, XmlDictionaryString ns)
#else
        internal bool IsStartElement(XmlDictionaryString localname, XmlDictionaryString ns)
#endif
        {
            if (dictionaryReader == null)
                return !isEndOfEmptyElement && reader.IsStartElement(localname.Value, ns.Value);
            else
                return !isEndOfEmptyElement && dictionaryReader.IsStartElement(localname, ns);
        }

        internal bool MoveToAttribute(string name)
        {
            return isEndOfEmptyElement ? false : reader.MoveToAttribute(name);
        }

        internal bool MoveToAttribute(string name, string ns)
        {
            return isEndOfEmptyElement ? false : reader.MoveToAttribute(name, ns);
        }

        internal void MoveToAttribute(int i)
        {
            if (isEndOfEmptyElement)
                throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(i), SR.Format(SR.XmlElementAttributes)));
            reader.MoveToAttribute(i);
        }

        internal bool MoveToElement()
        {
            return isEndOfEmptyElement ? false : reader.MoveToElement();
        }

        internal bool MoveToFirstAttribute()
        {
            return isEndOfEmptyElement ? false : reader.MoveToFirstAttribute();
        }

        internal bool MoveToNextAttribute()
        {
            return isEndOfEmptyElement ? false : reader.MoveToNextAttribute();
        }

#if USE_REFEMIT        
        public XmlNodeType NodeType
#else
        internal XmlNodeType NodeType
#endif
        {
            get { return isEndOfEmptyElement ? XmlNodeType.EndElement : reader.NodeType; }
        }

        internal bool Read()
        {
            //reader.MoveToFirstAttribute();
            //if (NodeType == XmlNodeType.Attribute)
            reader.MoveToElement();
            if (!reader.IsEmptyElement)
                return reader.Read();
            if (isEndOfEmptyElement)
            {
                isEndOfEmptyElement = false;
                return reader.Read();
            }
            isEndOfEmptyElement = true;
            return true;
        }

        internal XmlNodeType MoveToContent()
        {
            if (isEndOfEmptyElement)
                return XmlNodeType.EndElement;

            return reader.MoveToContent();
        }

        internal bool ReadAttributeValue()
        {
            return isEndOfEmptyElement ? false : reader.ReadAttributeValue();
        }

#if USE_REFEMIT        
        public void ReadEndElement()
#else
        internal void ReadEndElement()
#endif
        {
            if (isEndOfEmptyElement)
                Read();
            else
                reader.ReadEndElement();
        }

        private Exception CreateInvalidPrimitiveTypeException(Type type)
        {
            return new InvalidDataContractException(SR.Format(
                type.IsInterface ? SR.InterfaceTypeCannotBeCreated : SR.InvalidPrimitiveType_Serialization,
                DataContract.GetClrTypeFullName(type)));
        }

        public object ReadElementContentAsAnyType(Type valueType)
        {
            Read();
            object o = ReadContentAsAnyType(valueType);
            ReadEndElement();
            return o;
        }

        internal object ReadContentAsAnyType(Type valueType)
        {
            switch (Type.GetTypeCode(valueType))
            {
                case TypeCode.Boolean:
                    return ReadContentAsBoolean();
                case TypeCode.Char:
                    return ReadContentAsChar();
                case TypeCode.Byte:
                    return ReadContentAsUnsignedByte();
                case TypeCode.Int16:
                    return ReadContentAsShort();
                case TypeCode.Int32:
                    return ReadContentAsInt();
                case TypeCode.Int64:
                    return ReadContentAsLong();
                case TypeCode.Single:
                    return ReadContentAsSingle();
                case TypeCode.Double:
                    return ReadContentAsDouble();
                case TypeCode.Decimal:
                    return ReadContentAsDecimal();
                case TypeCode.DateTime:
                    return ReadContentAsDateTime();
                case TypeCode.String:
                    return ReadContentAsString();

                case TypeCode.SByte:
                    return ReadContentAsSignedByte();
                case TypeCode.UInt16:
                    return ReadContentAsUnsignedShort();
                case TypeCode.UInt32:
                    return ReadContentAsUnsignedInt();
                case TypeCode.UInt64:
                    return ReadContentAsUnsignedLong();
                case TypeCode.Empty:
                case TypeCode.DBNull:
                case TypeCode.Object:
                default:
                    if (valueType == Globals.TypeOfByteArray)
                        return ReadContentAsBase64();
                    else if (valueType == Globals.TypeOfObject)
                        return new object();
                    else if (valueType == Globals.TypeOfTimeSpan)
                        return ReadContentAsTimeSpan();
                    else if (valueType == Globals.TypeOfGuid)
                        return ReadContentAsGuid();
                    else if (valueType == Globals.TypeOfUri)
                        return ReadContentAsUri();
                    else if (valueType == Globals.TypeOfXmlQualifiedName)
                        return ReadContentAsQName();
                    break;
            }
            throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateInvalidPrimitiveTypeException(valueType));
        }

        internal IDataNode ReadExtensionData(Type valueType)
        {
            switch (Type.GetTypeCode(valueType))
            {
                case TypeCode.Boolean:
                    return new DataNode<bool>(ReadContentAsBoolean());
                case TypeCode.Char:
                    return new DataNode<char>(ReadContentAsChar());
                case TypeCode.Byte:
                    return new DataNode<byte>(ReadContentAsUnsignedByte());
                case TypeCode.Int16:
                    return new DataNode<short>(ReadContentAsShort());
                case TypeCode.Int32:
                    return new DataNode<int>(ReadContentAsInt());
                case TypeCode.Int64:
                    return new DataNode<long>(ReadContentAsLong());
                case TypeCode.Single:
                    return new DataNode<float>(ReadContentAsSingle());
                case TypeCode.Double:
                    return new DataNode<double>(ReadContentAsDouble());
                case TypeCode.Decimal:
                    return new DataNode<decimal>(ReadContentAsDecimal());
                case TypeCode.DateTime:
                    return new DataNode<DateTime>(ReadContentAsDateTime());
                case TypeCode.String:
                    return new DataNode<string>(ReadContentAsString());
                case TypeCode.SByte:
                    return new DataNode<sbyte>(ReadContentAsSignedByte());
                case TypeCode.UInt16:
                    return new DataNode<ushort>(ReadContentAsUnsignedShort());
                case TypeCode.UInt32:
                    return new DataNode<uint>(ReadContentAsUnsignedInt());
                case TypeCode.UInt64:
                    return new DataNode<ulong>(ReadContentAsUnsignedLong());
                case TypeCode.Empty:
                case TypeCode.DBNull:
                case TypeCode.Object:
                default:
                    if (valueType == Globals.TypeOfByteArray)
                        return new DataNode<byte[]>(ReadContentAsBase64());
                    else if (valueType == Globals.TypeOfObject)
                        return new DataNode<object>(new object());
                    else if (valueType == Globals.TypeOfTimeSpan)
                        return new DataNode<TimeSpan>(ReadContentAsTimeSpan());
                    else if (valueType == Globals.TypeOfGuid)
                        return new DataNode<Guid>(ReadContentAsGuid());
                    else if (valueType == Globals.TypeOfUri)
                        return new DataNode<Uri>(ReadContentAsUri());
                    else if (valueType == Globals.TypeOfXmlQualifiedName)
                        return new DataNode<XmlQualifiedName>(ReadContentAsQName());
                    break;
            }
            throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateInvalidPrimitiveTypeException(valueType));
        }

        private void ThrowConversionException(string value, string type)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(XmlObjectSerializer.TryAddLineInfo(this, SR.Format(SR.XmlInvalidConversion, value, type))));
        }

        private void ThrowNotAtElement()
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(SR.Format(SR.XmlStartElementExpected, "EndElement")));
        }

#if USE_REFEMIT
        public virtual char ReadElementContentAsChar()
#else
        internal virtual char ReadElementContentAsChar()
#endif
        {
            return ToChar(ReadElementContentAsInt());
        }

        internal virtual char ReadContentAsChar()
        {
            return ToChar(ReadContentAsInt());
        }

        private char ToChar(int value)
        {
            if (value < char.MinValue || value > char.MaxValue)
            {
                ThrowConversionException(value.ToString(NumberFormatInfo.CurrentInfo), "Char");
            }
            return (char)value;
        }

#if USE_REFEMIT
        public string ReadElementContentAsString()
#else
        internal string ReadElementContentAsString()
#endif
        {
            if (isEndOfEmptyElement)
                ThrowNotAtElement();

            return reader.ReadElementContentAsString();
        }

        internal string ReadContentAsString()
        {
            return isEndOfEmptyElement ? string.Empty : reader.ReadContentAsString();
        }

#if USE_REFEMIT
        public bool ReadElementContentAsBoolean()
#else
        internal bool ReadElementContentAsBoolean()
#endif
        {
            if (isEndOfEmptyElement)
                ThrowNotAtElement();

            return reader.ReadElementContentAsBoolean();
        }

        internal bool ReadContentAsBoolean()
        {
            if (isEndOfEmptyElement)
                ThrowConversionException(string.Empty, "Boolean");

            return reader.ReadContentAsBoolean();
        }

#if USE_REFEMIT
        public float ReadElementContentAsFloat()
#else
        internal float ReadElementContentAsFloat()
#endif
        {
            if (isEndOfEmptyElement)
                ThrowNotAtElement();

            return reader.ReadElementContentAsFloat();
        }

        internal float ReadContentAsSingle()
        {
            if (isEndOfEmptyElement)
                ThrowConversionException(string.Empty, "Float");

            return reader.ReadContentAsFloat();
        }

#if USE_REFEMIT
        public double ReadElementContentAsDouble()
#else
        internal double ReadElementContentAsDouble()
#endif
        {
            if (isEndOfEmptyElement)
                ThrowNotAtElement();

            return reader.ReadElementContentAsDouble();
        }

        internal double ReadContentAsDouble()
        {
            if (isEndOfEmptyElement)
                ThrowConversionException(string.Empty, "Double");

            return reader.ReadContentAsDouble();
        }

#if USE_REFEMIT
        public decimal ReadElementContentAsDecimal()
#else
        internal decimal ReadElementContentAsDecimal()
#endif
        {
            if (isEndOfEmptyElement)
                ThrowNotAtElement();

            return reader.ReadElementContentAsDecimal();
        }

        internal decimal ReadContentAsDecimal()
        {
            if (isEndOfEmptyElement)
                ThrowConversionException(string.Empty, "Decimal");

            return reader.ReadContentAsDecimal();
        }

#if USE_REFEMIT
        public virtual byte[] ReadElementContentAsBase64()
#else
        internal virtual byte[] ReadElementContentAsBase64()
#endif
        {
            if (isEndOfEmptyElement)
                ThrowNotAtElement();

            if (dictionaryReader == null)
            {
                return ReadContentAsBase64(reader.ReadElementContentAsString());
            }
            else
            {
                return dictionaryReader.ReadElementContentAsBase64();
            }
        }

        public virtual byte[] ReadContentAsBase64()
        {
            if (isEndOfEmptyElement)
                return Array.Empty<byte>();

            if (dictionaryReader == null)
            {
                return ReadContentAsBase64(reader.ReadContentAsString());
            }
            else
            {
                return dictionaryReader.ReadContentAsBase64();
            }
        }

        internal byte[] ReadContentAsBase64(string str)
        {
            if (str == null)
                return null;
            str = str.Trim();
            if (str.Length == 0)
                return Array.Empty<byte>();

            try
            {
                return Convert.FromBase64String(str);
            }
            catch (ArgumentException exception)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(str, "byte[]", exception));
            }
            catch (FormatException exception)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(str, "byte[]", exception));
            }
        }

#if USE_REFEMIT
        public virtual DateTime ReadElementContentAsDateTime()
#else
        internal virtual DateTime ReadElementContentAsDateTime()
#endif
        {
            if (isEndOfEmptyElement)
                ThrowNotAtElement();

            return XmlConvert.ToDateTime(reader.ReadElementContentAsString(), XmlDateTimeSerializationMode.RoundtripKind);
        }

        internal virtual DateTime ReadContentAsDateTime()
        {
            if (isEndOfEmptyElement)
                ThrowConversionException(string.Empty, "DateTime");

            return reader.ReadContentAsDateTime();
        }

#if USE_REFEMIT
        public int ReadElementContentAsInt()
#else
        internal int ReadElementContentAsInt()
#endif
        {
            if (isEndOfEmptyElement)
                ThrowNotAtElement();

            return reader.ReadElementContentAsInt();
        }

        internal int ReadContentAsInt()
        {
            if (isEndOfEmptyElement)
                ThrowConversionException(string.Empty, "Int32");

            return reader.ReadContentAsInt();
        }

#if USE_REFEMIT
        public long ReadElementContentAsLong()
#else
        internal long ReadElementContentAsLong()
#endif
        {
            if (isEndOfEmptyElement)
                ThrowNotAtElement();

            return reader.ReadElementContentAsLong();
        }

        internal long ReadContentAsLong()
        {
            if (isEndOfEmptyElement)
                ThrowConversionException(string.Empty, "Int64");

            return reader.ReadContentAsLong();
        }

#if USE_REFEMIT
        public short ReadElementContentAsShort()
#else
        internal short ReadElementContentAsShort()
#endif
        {
            return ToShort(ReadElementContentAsInt());
        }

        internal short ReadContentAsShort()
        {
            return ToShort(ReadContentAsInt());
        }

        private short ToShort(int value)
        {
            if (value < short.MinValue || value > short.MaxValue)
            {
                ThrowConversionException(value.ToString(NumberFormatInfo.CurrentInfo), "Int16");
            }
            return (short)value;
        }

#if USE_REFEMIT
        public byte ReadElementContentAsUnsignedByte()
#else
        internal byte ReadElementContentAsUnsignedByte()
#endif
        {
            return ToByte(ReadElementContentAsInt());
        }

        internal byte ReadContentAsUnsignedByte()
        {
            return ToByte(ReadContentAsInt());
        }

        private byte ToByte(int value)
        {
            if (value < byte.MinValue || value > byte.MaxValue)
            {
                ThrowConversionException(value.ToString(NumberFormatInfo.CurrentInfo), "Byte");
            }
            return (byte)value;
        }

#if USE_REFEMIT
        [CLSCompliant(false)]
        public SByte ReadElementContentAsSignedByte()
#else
        internal sbyte ReadElementContentAsSignedByte()
#endif
        {
            return ToSByte(ReadElementContentAsInt());
        }

        internal sbyte ReadContentAsSignedByte()
        {
            return ToSByte(ReadContentAsInt());
        }

        private sbyte ToSByte(int value)
        {
            if (value < sbyte.MinValue || value > sbyte.MaxValue)
            {
                ThrowConversionException(value.ToString(NumberFormatInfo.CurrentInfo), "SByte");
            }
            return (sbyte)value;
        }

#if USE_REFEMIT
        [CLSCompliant(false)]
        public UInt32 ReadElementContentAsUnsignedInt()
#else
        internal uint ReadElementContentAsUnsignedInt()
#endif
        {
            return ToUInt32(ReadElementContentAsLong());
        }

        internal uint ReadContentAsUnsignedInt()
        {
            return ToUInt32(ReadContentAsLong());
        }

        private uint ToUInt32(long value)
        {
            if (value < uint.MinValue || value > uint.MaxValue)
            {
                ThrowConversionException(value.ToString(NumberFormatInfo.CurrentInfo), "UInt32");
            }
            return (uint)value;
        }

#if USE_REFEMIT
        [CLSCompliant(false)]
        public virtual UInt64 ReadElementContentAsUnsignedLong()
#else
        internal virtual ulong ReadElementContentAsUnsignedLong()
#endif
        {
            if (isEndOfEmptyElement)
                ThrowNotAtElement();

            string str = reader.ReadElementContentAsString();

            if (str == null || str.Length == 0)
                ThrowConversionException(string.Empty, "UInt64");

            return XmlConverter.ToUInt64(str);
        }

        internal virtual ulong ReadContentAsUnsignedLong()
        {
            string str = reader.ReadContentAsString();

            if (str == null || str.Length == 0)
                ThrowConversionException(string.Empty, "UInt64");

            return XmlConverter.ToUInt64(str);
        }

#if USE_REFEMIT
        [CLSCompliant(false)]
        public UInt16 ReadElementContentAsUnsignedShort()
#else
        internal ushort ReadElementContentAsUnsignedShort()
#endif
        {
            return ToUInt16(ReadElementContentAsInt());
        }

        internal ushort ReadContentAsUnsignedShort()
        {
            return ToUInt16(ReadContentAsInt());
        }

        private ushort ToUInt16(int value)
        {
            if (value < ushort.MinValue || value > ushort.MaxValue)
            {
                ThrowConversionException(value.ToString(NumberFormatInfo.CurrentInfo), "UInt16");
            }
            return (ushort)value;
        }

#if USE_REFEMIT
        public TimeSpan ReadElementContentAsTimeSpan()
#else
        internal TimeSpan ReadElementContentAsTimeSpan()
#endif
        {
            if (isEndOfEmptyElement)
                ThrowNotAtElement();

            string str = reader.ReadElementContentAsString();
            return XmlConverter.ToTimeSpan(str);
        }

        internal TimeSpan ReadContentAsTimeSpan()
        {
            string str = reader.ReadContentAsString();
            return XmlConverter.ToTimeSpan(str);
        }

#if USE_REFEMIT
        public Guid ReadElementContentAsGuid()
#else
        internal Guid ReadElementContentAsGuid()
#endif
        {
            if (isEndOfEmptyElement)
                ThrowNotAtElement();

            string str = reader.ReadElementContentAsString();
            try
            {
                return new Guid(str);
            }
            catch (ArgumentException exception)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(str, "Guid", exception));
            }
            catch (FormatException exception)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(str, "Guid", exception));
            }
            catch (OverflowException exception)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(str, "Guid", exception));
            }
        }

        internal Guid ReadContentAsGuid()
        {
            string str = reader.ReadContentAsString();
            try
            {
                return Guid.Parse(str);
            }
            catch (ArgumentException exception)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(str, "Guid", exception));
            }
            catch (FormatException exception)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(str, "Guid", exception));
            }
            catch (OverflowException exception)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(str, "Guid", exception));
            }
        }

#if USE_REFEMIT
        public Uri ReadElementContentAsUri()
#else
        internal Uri ReadElementContentAsUri()
#endif
        {
            if (isEndOfEmptyElement)
                ThrowNotAtElement();

            string str = ReadElementContentAsString();
            try
            {
                return new Uri(str, UriKind.RelativeOrAbsolute);
            }
            catch (ArgumentException exception)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(str, "Uri", exception));
            }
            catch (FormatException exception)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(str, "Uri", exception));
            }
        }

        internal Uri ReadContentAsUri()
        {
            string str = ReadContentAsString();
            try
            {
                return new Uri(str, UriKind.RelativeOrAbsolute);
            }
            catch (ArgumentException exception)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(str, "Uri", exception));
            }
            catch (FormatException exception)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(str, "Uri", exception));
            }
        }

#if USE_REFEMIT
        public XmlQualifiedName ReadElementContentAsQName()
#else
        internal XmlQualifiedName ReadElementContentAsQName()
#endif
        {
            Read();
            XmlQualifiedName obj = ReadContentAsQName();
            ReadEndElement();
            return obj;
        }

        internal virtual XmlQualifiedName ReadContentAsQName()
        {
            return ParseQualifiedName(ReadContentAsString());
        }

        private XmlQualifiedName ParseQualifiedName(string str)
        {
            string name, ns, prefix;
            if (str == null || str.Length == 0)
                name = ns = string.Empty;
            else
                XmlObjectSerializerReadContext.ParseQualifiedName(str, this, out name, out ns, out prefix);
            return new XmlQualifiedName(name, ns);
        }

        private void CheckExpectedArrayLength(XmlObjectSerializerReadContext context, int arrayLength)
        {
            context.IncrementItemCount(arrayLength);
        }

        protected int GetArrayLengthQuota(XmlObjectSerializerReadContext context)
        {
            return Math.Min(context.RemainingItemCount, int.MaxValue);
        }

        private void CheckActualArrayLength(int expectedLength, int actualLength, XmlDictionaryString itemName, XmlDictionaryString itemNamespace)
        {
            if (expectedLength != actualLength)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(SR.Format(SR.ArrayExceededSizeAttribute, expectedLength, itemName.Value, itemNamespace.Value)));
        }

#if USE_REFEMIT
        public bool TryReadBooleanArray(XmlObjectSerializerReadContext context,
#else
        internal bool TryReadBooleanArray(XmlObjectSerializerReadContext context,
#endif
            XmlDictionaryString itemName, XmlDictionaryString itemNamespace,
            int arrayLength, out bool[] array)
        {
            if (dictionaryReader == null)
            {
                array = null;
                return false;
            }

            if (arrayLength != -1)
            {
                CheckExpectedArrayLength(context, arrayLength);
                array = new bool[arrayLength];
                int read = 0, offset = 0;
                while ((read = dictionaryReader.ReadArray(itemName, itemNamespace, array, offset, arrayLength - offset)) > 0)
                {
                    offset += read;
                }
                CheckActualArrayLength(arrayLength, offset, itemName, itemNamespace);
            }
            else
            {
                array = BooleanArrayHelperWithDictionaryString.Instance.ReadArray(
                    dictionaryReader, itemName, itemNamespace, GetArrayLengthQuota(context));
                context.IncrementItemCount(array.Length);
            }
            return true;
        }

#if USE_REFEMIT
        public virtual bool TryReadDateTimeArray(XmlObjectSerializerReadContext context,
#else
        internal virtual bool TryReadDateTimeArray(XmlObjectSerializerReadContext context,
#endif
            XmlDictionaryString itemName, XmlDictionaryString itemNamespace,
            int arrayLength, out DateTime[] array)
        {
            if (dictionaryReader == null)
            {
                array = null;
                return false;
            }

            if (arrayLength != -1)
            {
                CheckExpectedArrayLength(context, arrayLength);
                array = new DateTime[arrayLength];
                int read = 0, offset = 0;
                while ((read = dictionaryReader.ReadArray(itemName, itemNamespace, array, offset, arrayLength - offset)) > 0)
                {
                    offset += read;
                }
                CheckActualArrayLength(arrayLength, offset, itemName, itemNamespace);
            }
            else
            {
                array = DateTimeArrayHelperWithDictionaryString.Instance.ReadArray(
                    dictionaryReader, itemName, itemNamespace, GetArrayLengthQuota(context));
                context.IncrementItemCount(array.Length);
            }
            return true;
        }

#if USE_REFEMIT
        public bool TryReadDecimalArray(XmlObjectSerializerReadContext context,
#else
        internal bool TryReadDecimalArray(XmlObjectSerializerReadContext context,
#endif
            XmlDictionaryString itemName, XmlDictionaryString itemNamespace,
            int arrayLength, out decimal[] array)
        {
            if (dictionaryReader == null)
            {
                array = null;
                return false;
            }

            if (arrayLength != -1)
            {
                CheckExpectedArrayLength(context, arrayLength);
                array = new decimal[arrayLength];
                int read = 0, offset = 0;
                while ((read = dictionaryReader.ReadArray(itemName, itemNamespace, array, offset, arrayLength - offset)) > 0)
                {
                    offset += read;
                }
                CheckActualArrayLength(arrayLength, offset, itemName, itemNamespace);
            }
            else
            {
                array = DecimalArrayHelperWithDictionaryString.Instance.ReadArray(
                    dictionaryReader, itemName, itemNamespace, GetArrayLengthQuota(context));
                context.IncrementItemCount(array.Length);
            }
            return true;
        }

#if USE_REFEMIT
        public bool TryReadInt32Array(XmlObjectSerializerReadContext context,
#else
        internal bool TryReadInt32Array(XmlObjectSerializerReadContext context,
#endif
            XmlDictionaryString itemName, XmlDictionaryString itemNamespace,
            int arrayLength, out int[] array)
        {
            if (dictionaryReader == null)
            {
                array = null;
                return false;
            }

            if (arrayLength != -1)
            {
                CheckExpectedArrayLength(context, arrayLength);
                array = new int[arrayLength];
                int read = 0, offset = 0;
                while ((read = dictionaryReader.ReadArray(itemName, itemNamespace, array, offset, arrayLength - offset)) > 0)
                {
                    offset += read;
                }
                CheckActualArrayLength(arrayLength, offset, itemName, itemNamespace);
            }
            else
            {
                array = Int32ArrayHelperWithDictionaryString.Instance.ReadArray(
                    dictionaryReader, itemName, itemNamespace, GetArrayLengthQuota(context));
                context.IncrementItemCount(array.Length);
            }
            return true;
        }

#if USE_REFEMIT
        public bool TryReadInt64Array(XmlObjectSerializerReadContext context,
#else
        internal bool TryReadInt64Array(XmlObjectSerializerReadContext context,
#endif
            XmlDictionaryString itemName, XmlDictionaryString itemNamespace,
            int arrayLength, out long[] array)
        {
            if (dictionaryReader == null)
            {
                array = null;
                return false;
            }

            if (arrayLength != -1)
            {
                CheckExpectedArrayLength(context, arrayLength);
                array = new long[arrayLength];
                int read = 0, offset = 0;
                while ((read = dictionaryReader.ReadArray(itemName, itemNamespace, array, offset, arrayLength - offset)) > 0)
                {
                    offset += read;
                }
                CheckActualArrayLength(arrayLength, offset, itemName, itemNamespace);
            }
            else
            {
                array = Int64ArrayHelperWithDictionaryString.Instance.ReadArray(
                    dictionaryReader, itemName, itemNamespace, GetArrayLengthQuota(context));
                context.IncrementItemCount(array.Length);
            }
            return true;
        }

#if USE_REFEMIT
        public bool TryReadSingleArray(XmlObjectSerializerReadContext context,
#else
        internal bool TryReadSingleArray(XmlObjectSerializerReadContext context,
#endif
        XmlDictionaryString itemName, XmlDictionaryString itemNamespace,
            int arrayLength, out float[] array)
        {
            if (dictionaryReader == null)
            {
                array = null;
                return false;
            }

            if (arrayLength != -1)
            {
                CheckExpectedArrayLength(context, arrayLength);
                array = new float[arrayLength];
                int read = 0, offset = 0;
                while ((read = dictionaryReader.ReadArray(itemName, itemNamespace, array, offset, arrayLength - offset)) > 0)
                {
                    offset += read;
                }
                CheckActualArrayLength(arrayLength, offset, itemName, itemNamespace);
            }
            else
            {
                array = SingleArrayHelperWithDictionaryString.Instance.ReadArray(
                    dictionaryReader, itemName, itemNamespace, GetArrayLengthQuota(context));
                context.IncrementItemCount(array.Length);
            }
            return true;
        }

#if USE_REFEMIT
        public bool TryReadDoubleArray(XmlObjectSerializerReadContext context,
#else
        internal bool TryReadDoubleArray(XmlObjectSerializerReadContext context,
#endif   
            XmlDictionaryString itemName, XmlDictionaryString itemNamespace,
            int arrayLength, out double[] array)
        {
            if (dictionaryReader == null)
            {
                array = null;
                return false;
            }

            if (arrayLength != -1)
            {
                CheckExpectedArrayLength(context, arrayLength);
                array = new double[arrayLength];
                int read = 0, offset = 0;
                while ((read = dictionaryReader.ReadArray(itemName, itemNamespace, array, offset, arrayLength - offset)) > 0)
                {
                    offset += read;
                }
                CheckActualArrayLength(arrayLength, offset, itemName, itemNamespace);
            }
            else
            {
                array = DoubleArrayHelperWithDictionaryString.Instance.ReadArray(
                    dictionaryReader, itemName, itemNamespace, GetArrayLengthQuota(context));
                context.IncrementItemCount(array.Length);
            }
            return true;
        }

        internal IDictionary<string, string> GetNamespacesInScope(XmlNamespaceScope scope)
        {
            return (reader is IXmlNamespaceResolver) ? ((IXmlNamespaceResolver)reader).GetNamespacesInScope(scope) : null;
        }

        // IXmlLineInfo members
        internal bool HasLineInfo()
        {
            IXmlLineInfo iXmlLineInfo = reader as IXmlLineInfo;
            return (iXmlLineInfo == null) ? false : iXmlLineInfo.HasLineInfo();
        }

        internal int LineNumber
        {
            get
            {
                IXmlLineInfo iXmlLineInfo = reader as IXmlLineInfo;
                return (iXmlLineInfo == null) ? 0 : iXmlLineInfo.LineNumber;
            }
        }

        internal int LinePosition
        {
            get
            {
                IXmlLineInfo iXmlLineInfo = reader as IXmlLineInfo;
                return (iXmlLineInfo == null) ? 0 : iXmlLineInfo.LinePosition;
            }
        }

        // IXmlTextParser members
        internal bool Normalized
        {
            get
            {
                XmlTextReader xmlTextReader = reader as XmlTextReader;
                if (xmlTextReader == null)
                {
                    IXmlTextParser xmlTextParser = reader as IXmlTextParser;
                    return (xmlTextParser == null) ? false : xmlTextParser.Normalized;
                }
                else
                    return xmlTextReader.Normalization;
            }
            set
            {
                XmlTextReader xmlTextReader = reader as XmlTextReader;
                if (xmlTextReader == null)
                {
                    IXmlTextParser xmlTextParser = reader as IXmlTextParser;
                    if (xmlTextParser != null)
                        xmlTextParser.Normalized = value;
                }
                else
                    xmlTextReader.Normalization = value;
            }
        }

        internal WhitespaceHandling WhitespaceHandling
        {
            get
            {
                XmlTextReader xmlTextReader = reader as XmlTextReader;
                if (xmlTextReader == null)
                {
                    IXmlTextParser xmlTextParser = reader as IXmlTextParser;
                    return (xmlTextParser == null) ? WhitespaceHandling.None : xmlTextParser.WhitespaceHandling;
                }
                else
                    return xmlTextReader.WhitespaceHandling;
            }
            set
            {
                XmlTextReader xmlTextReader = reader as XmlTextReader;
                if (xmlTextReader == null)
                {
                    IXmlTextParser xmlTextParser = reader as IXmlTextParser;
                    if (xmlTextParser != null)
                        xmlTextParser.WhitespaceHandling = value;
                }
                else
                    xmlTextReader.WhitespaceHandling = value;
            }
        }

        // delegating properties and methods
        internal string Name { get { return reader.Name; } }
        internal string LocalName { get { return reader.LocalName; } }
        internal string NamespaceURI { get { return reader.NamespaceURI; } }
        internal string Value { get { return reader.Value; } }
        internal Type ValueType { get { return reader.ValueType; } }
        internal int Depth { get { return reader.Depth; } }
        internal string LookupNamespace(string prefix) { return reader.LookupNamespace(prefix); }
        internal bool EOF { get { return reader.EOF; } }

        internal void Skip()
        {
            reader.Skip();
            isEndOfEmptyElement = false;
        }
    }
}

