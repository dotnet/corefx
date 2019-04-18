using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace System.Resources.Binary
{
    public class BinaryResourceWriter : ResourceWriter
    {

        bool _requiresBinaryResourceReader = false;

        public BinaryResourceWriter(string fileName) : base(fileName)
        { }

        public BinaryResourceWriter(Stream stream) : base(stream)
        { }

        protected override string ResourceReaderTypeName => _requiresBinaryResourceReader ? 
            typeof(BinaryResourceReader).AssemblyQualifiedName : 
            base.ResourceReaderTypeName;

        protected override string ResourceSetTypeName => _requiresBinaryResourceReader ?
            typeof(RuntimeResourceSet).AssemblyQualifiedName :
            base.ResourceSetTypeName;

        public void AddTypeConverterResource(string name, string typeName, string value)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (typeName == null)
                throw new ArgumentNullException(nameof(typeName));
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            AddResourceData(name, typeName, new ResourceDataRecord(SerializationFormat.TypeConverterString, value));

            _requiresBinaryResourceReader = true;
        }

        public void AddTypeConverterResource(string name, string typeName, byte[] value)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (typeName == null)
                throw new ArgumentNullException(nameof(typeName));
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            AddResourceData(name, typeName, new ResourceDataRecord(SerializationFormat.TypeConverterByteArray, value));

            _requiresBinaryResourceReader = true;
        }

        public void AddBinaryFormattedResource(string name, string typeName, byte[] value)
        {

            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (typeName == null)
                throw new ArgumentNullException(nameof(typeName));
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            AddResourceData(name, typeName, new ResourceDataRecord(SerializationFormat.BinaryFormatter, value));
        }

        public void AddStreamResource(string name, string typeName, byte[] value)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (typeName == null)
                throw new ArgumentNullException(nameof(typeName));
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            AddResourceData(name, typeName, new ResourceDataRecord(SerializationFormat.Stream, value));

            _requiresBinaryResourceReader = true;
        }

        public void AddStreamResource(string name, string typeName, Stream value, bool closeAfterWrite)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (typeName == null)
                throw new ArgumentNullException(nameof(typeName));
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (!value.CanSeek)
                throw new ArgumentException(SR.NotSupported_UnseekableStream);

            AddResourceData(name, typeName, new ResourceDataRecord(SerializationFormat.Stream, value, closeAfterWrite));

            _requiresBinaryResourceReader = true;
        }

        private class ResourceDataRecord
        {
            internal readonly SerializationFormat Format;
            internal readonly object Data;
            internal readonly bool CloseAfterWrite;

            internal ResourceDataRecord(SerializationFormat format, object data, bool closeAfterWrite = false)
            {
                Format = format;
                Data = data;
                CloseAfterWrite = closeAfterWrite;
            }
        }

        protected override void WriteData(BinaryWriter writer, object dataContext)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));
            if (dataContext == null)
                throw new ArgumentNullException(nameof(dataContext));

            ResourceDataRecord record = dataContext as ResourceDataRecord;

            if (record == null)
            {
                throw new InvalidOperationException(SR.Format(SR.InvalidOperation_CannotWriteType, GetType(), dataContext.GetType(), nameof(WriteData)));
            }

            if (_requiresBinaryResourceReader)
            {
                writer.Write((byte)record.Format);
            }

            try
            {
                switch (record.Format)
                {
                    case SerializationFormat.BinaryFormatter:
                        {
                            byte[] data = (byte[])record.Data;

                            if (_requiresBinaryResourceReader)
                            {
                                writer.Write7BitEncodedInt(data.Length);
                            }

                            writer.Write(data);
                            break;
                        }
                    case SerializationFormat.Stream:
                        {
                            if (record.Data is byte[] data)
                            {
                                writer.Write7BitEncodedInt(data.Length);
                                writer.Write(data);
                            }
                            else
                            {
                                Stream stream = (Stream)record.Data;

                                if (stream.Length > int.MaxValue)
                                    throw new ArgumentException(SR.ArgumentOutOfRange_StreamLength);

                                stream.Position = 0;
                                writer.Write7BitEncodedInt((int)stream.Length);
                                stream.CopyTo(writer.BaseStream);
                            }
                            break;
                        }
                    case SerializationFormat.TypeConverterByteArray:
                        {
                            byte[] data = (byte[])record.Data;
                            writer.Write7BitEncodedInt(data.Length);
                            writer.Write(data);
                            break;
                        }
                    case SerializationFormat.TypeConverterString:
                        {
                            string data = (string)record.Data;
                            writer.Write(data);
                            break;
                        }
                    default:
                        // unreachable: indicates inconsistency in this class
                        throw new ArgumentException(nameof(ResourceDataRecord.Format));
                }
            }
            finally
            {
                if (record.Data is IDisposable disposable && record.CloseAfterWrite)
                {
                    disposable.Dispose();
                }
            }

        }
    }
}
