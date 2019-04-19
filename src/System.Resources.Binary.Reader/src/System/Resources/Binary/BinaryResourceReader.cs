using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace System.Resources.Binary
{
    public class BinaryResourceReader : ResourceReader
    {
        public BinaryResourceReader(Stream stream) : base(stream)
        {
        }

        public BinaryResourceReader(string fileName) : base(fileName)
        {
        }

        protected override object DeserializeObject(BinaryReader reader, Type type)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            // read type
            SerializationFormat format = (SerializationFormat)reader.ReadByte();


            object value = null;

            // read data
            switch (format)
            {
                case SerializationFormat.BinaryFormatter:
                    {
                        // read length
                        int length = reader.Read7BitEncodedInt();
                        if (length < 0)
                        {
                            throw new BadImageFormatException(SR.Format(SR.BadImageFormat_ResourceDataLengthInvalid, length));
                        }

                        BinaryFormatter bf = new BinaryFormatter();

                        long originalPosition = reader.BaseStream.Position;

                        value = bf.Deserialize(reader.BaseStream);

                        long bytesRead = reader.BaseStream.Position - originalPosition;

                        // Ensure BF read what we expected.
                        if (bytesRead != length)
                        {
                            throw new BadImageFormatException(SR.Format(SR.BadImageFormat_ResourceDataLengthInvalid, length));
                        }
                        break;
                    }
                case SerializationFormat.TypeConverterByteArray:
                    {
                        // read length
                        int length = reader.Read7BitEncodedInt();
                        if (length < 0)
                        {
                            throw new BadImageFormatException(SR.Format(SR.BadImageFormat_ResourceDataLengthInvalid, length));
                        }

                        byte[] data = reader.ReadBytes(length);

                        TypeConverter converter = TypeDescriptor.GetConverter(type);

                        if (converter == null)
                        {
                            throw new TypeLoadException(SR.Format(SR.TypeLoadException_CannotLoadConverter, type));
                        }

                        value = converter.ConvertFrom(data);
                        break;
                    }
                case SerializationFormat.TypeConverterString:
                    {
                        string stringData = reader.ReadString();

                        TypeConverter converter = TypeDescriptor.GetConverter(type);

                        if (converter == null)
                        {
                            throw new TypeLoadException(SR.Format(SR.TypeLoadException_CannotLoadConverter, type));
                        }

                        value = converter.ConvertFromInvariantString(stringData);
                        break;
                    }
                case SerializationFormat.Stream:
                    {
                        // read length
                        int length = reader.Read7BitEncodedInt();
                        if (length < 0)
                        {
                            throw new BadImageFormatException(SR.Format(SR.BadImageFormat_ResourceDataLengthInvalid, length));
                        }
                        Stream stream;

                        if (reader.BaseStream is UnmanagedMemoryStream ums)
                        {
                            // For the case that we've memory mapped in the .resources
                            // file, just return a Stream pointing to that block of memory.
                            unsafe
                            {
                                stream = new UnmanagedMemoryStream(ums.PositionPointer, length, length, FileAccess.Read);
                            }
                        }
                        else
                        {

                            byte[] bytes = reader.ReadBytes(length);
                            // Lifetime of memory == lifetime of this stream.
                            stream = new MemoryStream(bytes, false);
                        }

                        value = Activator.CreateInstance(type, new object[] { stream });
                        break;
                    }
                default:
                    throw new BadImageFormatException(SR.BadImageFormat_TypeMismatch);
            }

            return value;
        }

    }
}
