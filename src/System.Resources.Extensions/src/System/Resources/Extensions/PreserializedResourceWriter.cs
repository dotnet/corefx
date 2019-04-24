// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;

namespace System.Resources.Extensions
{
    partial class PreserializedResourceWriter
    {
        bool _requiresDeserializingResourceReader = false;

        private string ResourceReaderTypeName => _requiresDeserializingResourceReader ? 
            typeof(DeserializingResourceReader).AssemblyQualifiedName :
            ResourceReaderFullyQualifiedName;

        private string ResourceSetTypeName => _requiresDeserializingResourceReader ?
            typeof(RuntimeResourceSet).AssemblyQualifiedName :
            ResSetTypeName;

        public void AddTypeConverterResource(string name, string typeName, string value)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (typeName == null)
                throw new ArgumentNullException(nameof(typeName));
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            AddResourceData(name, typeName, new ResourceDataRecord(SerializationFormat.TypeConverterString, value));

            _requiresDeserializingResourceReader = true;
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

            _requiresDeserializingResourceReader = true;
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

        public void AddActivatorResource(string name, string typeName, Stream value, bool closeAfterWrite)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (typeName == null)
                throw new ArgumentNullException(nameof(typeName));
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (!value.CanSeek)
                throw new ArgumentException(SR.NotSupported_UnseekableStream);

            AddResourceData(name, typeName, new ResourceDataRecord(SerializationFormat.ActivatorStream, value, closeAfterWrite));

            _requiresDeserializingResourceReader = true;
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

        private void WriteData(BinaryWriter writer, object dataContext)
        {
            ResourceDataRecord record = dataContext as ResourceDataRecord;

            if (record == null)
            {
                throw new InvalidOperationException(SR.Format(SR.InvalidOperation_CannotWriteType, GetType(), dataContext.GetType(), nameof(WriteData)));
            }

            if (_requiresDeserializingResourceReader)
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

                            if (_requiresDeserializingResourceReader)
                            {
                                Write7BitEncodedInt(writer, data.Length);
                            }

                            writer.Write(data);
                            break;
                        }
                    case SerializationFormat.ActivatorStream:
                        {
                            if (record.Data is byte[] data)
                            {
                                Write7BitEncodedInt(writer, data.Length);
                                writer.Write(data);
                            }
                            else
                            {
                                Stream stream = (Stream)record.Data;

                                if (stream.Length > int.MaxValue)
                                    throw new ArgumentException(SR.ArgumentOutOfRange_StreamLength);

                                stream.Position = 0;
                                Write7BitEncodedInt(writer, (int)stream.Length);
                                stream.CopyTo(writer.BaseStream);
                            }
                            break;
                        }
                    case SerializationFormat.TypeConverterByteArray:
                        {
                            byte[] data = (byte[])record.Data;
                            Write7BitEncodedInt(writer, data.Length);
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
