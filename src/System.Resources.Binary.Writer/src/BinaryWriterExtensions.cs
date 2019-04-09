using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace System.Resources.Binary
{
    internal static class BinaryWriterExtensions
    {
        public static void Write7BitEncodedInt(this BinaryWriter store, int value)
        {
            Debug.Assert(store != null);
            // Write out an int 7 bits at a time.  The high bit of the byte,
            // when on, tells reader to continue reading more bytes.
            uint v = (uint)value;   // support negative numbers
            while (v >= 0x80)
            {
                store.Write((byte)(v | 0x80));
                v >>= 7;
            }
            store.Write((byte)v);
        }
    }
}
