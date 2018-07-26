// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace System.Net.Test.Common
{
    
    public enum FrameType : byte
    {
        Data = 0,
        Headers = 1,
        Priority = 2,
        RstStream = 3,
        Settings = 4,
        PushPromise = 5,
        Ping = 6,
        GoAway = 7,
        WindowUpdate = 8,
        Continuation = 9,

        Last = 9
    }

    [Flags]
    public enum FrameFlags : byte
    {
        None = 0,
        
        // Some frame types define bits differently.  Define them all here for simplicity.

        EndStream =     0b00000001,
        Ack =           0b00000001,
        EndHeaders =    0b00000100,
        Padded =        0b00001000,
        Priority =      0b00100000,

        ValidBits =     0b00101101
    }

    public class Frame
    {
        public int Length;
        public FrameType Type;
        public FrameFlags Flags;
        public int StreamId;

        public const int MinFrameSize = 9;
        public const int MaxFrameSize = 16384;

        public Frame(int length, FrameType type, FrameFlags flags, int streamId)
        {
            Length = length;
            Type = type;
            Flags = flags;
            StreamId = streamId;
        }

        // Used only by derived classes that need to initialize 
        internal Frame()
        {
            Length = 0;
            Type = 0;
            Flags = 0;
            StreamId = 0;
        }

        public bool PaddedFlag => (Flags & FrameFlags.Padded) != 0;
        public bool AckFlag => (Flags & FrameFlags.Ack) != 0;
        public bool EndHeadersFlag => (Flags & FrameFlags.EndHeaders) != 0;
        public bool EndStreamFlag => (Flags & FrameFlags.EndStream) != 0;
        public bool PriorityFlag => (Flags & FrameFlags.Priority) != 0;

        public static Frame ReadFrom(ReadOnlySpan<byte> buffer)
        {
            return new Frame(
                (buffer[0] << 16) | (buffer[1] << 8) | buffer[2],
                (FrameType)buffer[3],
                (FrameFlags)buffer[4],
                (int)((uint)((buffer[5] << 24) | (buffer[6] << 16) | (buffer[7] << 8) | buffer[8]) & 0x7FFFFFFF));
        }

        public virtual void WriteTo(Span<byte> buffer)
        {
            buffer[0] = (byte)((Length & 0x00FF0000) >> 16);
            buffer[1] = (byte)((Length & 0x0000FF00) >> 8);
            buffer[2] = (byte)(Length & 0x000000FF);

            buffer[3] = (byte)Type;
            buffer[4] = (byte)Flags;

            buffer[5] = (byte)((StreamId & 0xFF000000) >> 24);
            buffer[6] = (byte)((StreamId & 0x00FF0000) >> 16);
            buffer[7] = (byte)((StreamId & 0x0000FF00) >> 8);
            buffer[8] = (byte)(StreamId & 0x000000FF);
        }

        public override string ToString()
        {
            return $"Length: {Length}\nType: {Type}\nFlags: {Flags}\nStreamId: {StreamId}";
        }
    }

    public class DataFrame : Frame
    {
        private byte _padLength = 0;
        private byte[] _data;

        public DataFrame(byte[] data, FrameFlags flags, byte padLength, int streamId)
        {
            Length = (flags & FrameFlags.Padded) == 0 ? data.Length : data.Length + padLength + 1;
            Type = FrameType.Data;
            Flags = flags;
            StreamId = streamId;

            _data = data;
            _padLength = padLength;
        }

        public byte[] Data
        {
            get
            {
                return _data;
            }
        }

        public static DataFrame ReadFrom(Frame header, ReadOnlySpan<byte> buffer)
        {
            byte padLength = (byte)(header.PaddedFlag ? buffer[Frame.MinFrameSize] : 0);
            byte[] data = buffer.Slice(header.PaddedFlag ? Frame.MinFrameSize + 1 : Frame.MinFrameSize, buffer.Length).ToArray();

            return new DataFrame(data, header.Flags, padLength, header.StreamId);
        }

        public override void WriteTo(Span<byte> buffer)
        {
            base.WriteTo(buffer);

            if (PaddedFlag)
            {
                buffer[9] = _padLength;
                _data.CopyTo(buffer.Slice(Frame.MinFrameSize + 1));
            }
            else
            {
                _data.CopyTo(buffer.Slice(Frame.MinFrameSize));
            }
        }
    }

    public class HeadersFrame : Frame
    {
        private byte _padLength = 0;
        private int _streamDependency = 0;
        private int _weight = 0;
        private byte[] _data;

        public HeadersFrame(byte[] data, FrameFlags flags, byte padLength, int streamDependency, byte weight, int streamId)
        {
            Length = data.Length +
                     ((flags & FrameFlags.Padded) == 0 ? 0 : padLength + 1) +
                     ((flags & FrameFlags.Priority) == 0 ? 0 : 40);

            Type = FrameType.Headers;
            Flags = flags;
            StreamId = streamId;

            _data = data;
            _padLength = padLength;
            _streamDependency = streamDependency;
            _weight = weight;
        }

        public byte[] Data
        {
            get
            {
                return _data;
            }
        }

        public static HeadersFrame ReadFrom(Frame header, ReadOnlySpan<byte> buffer)
        {
            int idx = Frame.MinFrameSize; // Since we have multiple optional fields in the header, it is easiest to parse using an index.

            byte padLength = (byte)(header.PaddedFlag ? buffer[idx++] : 0);

            int streamDependency = header.PriorityFlag ? (int)((uint)((buffer[idx++] << 24) |
                                                                      (buffer[idx++] << 16) |
                                                                      (buffer[idx++] << 8) |
                                                                      (buffer[idx++])) & 0x7FFFFFFF) : 0;

            byte weight = (byte)(header.PaddedFlag ? buffer[idx++] : 0);

            byte[] data = buffer.Slice(idx, buffer.Length).ToArray();

            return new HeadersFrame(data, header.Flags, padLength, streamDependency, weight, header.StreamId);
        }

        public override void WriteTo(Span<byte> buffer)
        {
            base.WriteTo(buffer);

            if (PaddedFlag)
            {
                buffer[9] = _padLength;
                _data.CopyTo(buffer.Slice(Frame.MinFrameSize + 1));
            }
            else
            {
                _data.CopyTo(buffer.Slice(Frame.MinFrameSize));
            }
        }
    }
}