
namespace System.Resources.Binary
{
    internal enum SerializationFormat : byte
    {
        BinaryFormatter = 0x1,
        TypeConverterByteArray = 0x2,
        TypeConverterString = 0x3,
        Stream = 0x4
    }
}
