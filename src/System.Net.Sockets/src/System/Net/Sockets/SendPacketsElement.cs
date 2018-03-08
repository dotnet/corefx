// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;

namespace System.Net.Sockets
{
    // Class that wraps the semantics of a Winsock TRANSMIT_PACKETS_ELEMENTS struct.
    public class SendPacketsElement
    {
        internal SendPacketsElementFlags _flags;

        // Constructors for file elements.
        public SendPacketsElement(string filepath) :
            this(filepath, 0L, 0L, false)
        { }

        public SendPacketsElement(string filepath, int offset, int count) :
            this(filepath, (long)offset, count, false)
        { }

        public SendPacketsElement(string filepath, int offset, int count, bool endOfPacket) :
            this(filepath, (long)offset, count, endOfPacket)
        { }

        public SendPacketsElement(string filepath, long offset, long count) :
            this(filepath, offset, count, false)
        { }

        public SendPacketsElement(string filepath, long offset, long count, bool endOfPacket)
        {
            // We will validate if the file exists on send.
            if (filepath == null)
            {
                throw new ArgumentNullException(nameof(filepath));
            }
            // The native API will validate the file length on send.
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            Initialize(filepath, null, null, offset, count, SendPacketsElementFlags.File, endOfPacket);
        }

        // Constructors for fileStream elements.
        public SendPacketsElement(FileStream fileStream) :
            this(fileStream, 0L, 0L, false)
        { }

        public SendPacketsElement(FileStream fileStream, long offset, long count) :
            this(fileStream, offset, count, false)
        { }

        public SendPacketsElement(FileStream fileStream, long offset, long count, bool endOfPacket)
        {
            // We will validate if the fileStream exists on send.
            if (fileStream == null)
            {
                throw new ArgumentNullException(nameof(fileStream));
            }
            // The native API will validate the file length on send.
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            Initialize(null, fileStream, null, offset, count, SendPacketsElementFlags.File, endOfPacket);
        }

        // Constructors for buffer elements.
        public SendPacketsElement(byte[] buffer) :
            this(buffer, 0, (buffer != null ? buffer.Length : 0), false)
        { }

        public SendPacketsElement(byte[] buffer, int offset, int count) :
            this(buffer, offset, count, false)
        { }

        public SendPacketsElement(byte[] buffer, int offset, int count, bool endOfPacket)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }
            if ((uint)offset > (uint)buffer.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }
            if ((uint)count > (uint)(buffer.Length - offset))
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            Initialize(null, null, buffer, offset, count, SendPacketsElementFlags.Memory, endOfPacket);
        }

        private void Initialize(string filePath, FileStream fileStream, byte[] buffer, long offset, long count,
            SendPacketsElementFlags flags, bool endOfPacket)
        {
            FilePath = filePath;
            FileStream = fileStream;
            Buffer = buffer;
            LongOffset = offset;
            LongCount = count;
            _flags = flags;
            if (endOfPacket)
            {
                _flags |= SendPacketsElementFlags.EndOfPacket;
            }
        }

        public string FilePath { get; private set; }

        public FileStream FileStream { get; private set; }

        public byte[] Buffer { get; private set; }

        public int Count => checked((int)LongCount);

        public int Offset => checked((int)LongOffset);

        public long LongCount { get; private set; }

        public long LongOffset { get; private set; }

        public bool EndOfPacket => (_flags & SendPacketsElementFlags.EndOfPacket) != 0;
    }
}
