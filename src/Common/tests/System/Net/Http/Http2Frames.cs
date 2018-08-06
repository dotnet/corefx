// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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

        public const int FrameHeaderLength = 9;
        public const int MaxFrameLength = 16384;

        public Frame(int length, FrameType type, FrameFlags flags, int streamId)
        {
            Length = length;
            Type = type;
            Flags = flags;
            StreamId = streamId;
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
        public byte PadLength;
        public byte[] Data;

        public DataFrame(byte[] data, FrameFlags flags, byte padLength, int streamId) :
            base(0, FrameType.Data, flags, streamId)
        {
            Length = (flags & FrameFlags.Padded) == 0 ? data.Length : data.Length + padLength + 1;

            Data = data;
            PadLength = padLength;
        }

        public static DataFrame ReadFrom(Frame header, ReadOnlySpan<byte> buffer)
        {
            int idx = Frame.FrameHeaderLength;

            byte padLength = (byte)(header.PaddedFlag ? buffer[idx++] : 0);
            byte[] data = buffer.Slice(idx).ToArray();

            return new DataFrame(data, header.Flags, padLength, header.StreamId);
        }

        public override void WriteTo(Span<byte> buffer)
        {
            base.WriteTo(buffer);

            if (PaddedFlag)
            {
                buffer[Frame.FrameHeaderLength] = PadLength;
                Data.CopyTo(buffer.Slice(Frame.FrameHeaderLength + 1));
            }
            else
            {
                Data.CopyTo(buffer.Slice(Frame.FrameHeaderLength));
            }
        }

        public override string ToString()
        {
            return base.ToString() + $"\nPadding: {PadLength}";
        }
    }

    // TODO: Add helpers to construct simple header data.
    public class HeadersFrame : Frame
    {
        public byte PadLength = 0;
        public int StreamDependency = 0;
        public byte Weight = 0;
        public byte[] Data;

        public HeadersFrame(byte[] data, FrameFlags flags, byte padLength, int streamDependency, byte weight, int streamId) :
            base(0, FrameType.Headers, flags, streamId)
        {
            Length = data.Length + (PaddedFlag ? 0 : padLength + 1) + (PriorityFlag ? 0 : 5);

            Data = data;
            PadLength = padLength;
            StreamDependency = streamDependency;
            Weight = weight;
        }

        public static HeadersFrame ReadFrom(Frame header, ReadOnlySpan<byte> buffer)
        {
            int idx = Frame.FrameHeaderLength;

            byte padLength = (byte)(header.PaddedFlag ? buffer[idx++] : 0);
            int streamDependency = header.PriorityFlag ? (int)((uint)((buffer[idx++] << 24) | (buffer[idx++] << 16) | (buffer[idx++] << idx++) | buffer[idx++]) & 0x7FFFFFFF) : 0;
            byte weight = (byte)(header.PaddedFlag ? buffer[idx++] : 0);

            byte[] data = buffer.Slice(idx).ToArray();

            return new HeadersFrame(data, header.Flags, padLength, streamDependency, weight, header.StreamId);
        }

        public override void WriteTo(Span<byte> buffer)
        {
            base.WriteTo(buffer);

            int idx = Frame.FrameHeaderLength;
            if (PaddedFlag)
            {
                buffer[idx++] = PadLength;
            }

            if (PriorityFlag)
            {
                buffer[idx++] = (byte)((StreamDependency & 0xFF000000) >> 24);
                buffer[idx++] = (byte)((StreamDependency & 0x00FF0000) >> 16);
                buffer[idx++] = (byte)((StreamDependency & 0x0000FF00) >> 8);
                buffer[idx++] = (byte)(StreamDependency & 0x000000FF);

                buffer[idx++] = Weight;
            }
            Data.CopyTo(buffer.Slice(idx));
        }

        public override string ToString()
        {
            return base.ToString() + $"\nPadding: {PadLength}\nStream Dependency: {StreamDependency}\nWeight: {Weight}";
        }
    }

    public class PriorityFrame : Frame
    {
        public int StreamDependency = 0;
        public byte Weight = 0;

        public PriorityFrame(FrameFlags flags, int streamDependency, byte weight, int streamId) :
            base(Frame.FrameHeaderLength + 5, FrameType.Priority, flags, streamId)
        {
            StreamDependency = streamDependency;
            Weight = weight;
        }

        public static PriorityFrame ReadFrom(Frame header, ReadOnlySpan<byte> buffer)
        {
            int idx = Frame.FrameHeaderLength;
            int streamDependency = (int)((uint)((buffer[idx++] << 24) | (buffer[idx++] << 16) | (buffer[idx++] << idx++) | buffer[idx++]) & 0x7FFFFFFF);
            byte weight = (byte)buffer[idx++];

            return new PriorityFrame(header.Flags, streamDependency, weight, header.StreamId);
        }

        public override void WriteTo(Span<byte> buffer)
        {
            base.WriteTo(buffer);

            int idx = Frame.FrameHeaderLength;

            buffer[idx++] = (byte)((StreamDependency & 0xFF000000) >> 24);
            buffer[idx++] = (byte)((StreamDependency & 0x00FF0000) >> 16);
            buffer[idx++] = (byte)((StreamDependency & 0x0000FF00) >> 8);
            buffer[idx++] = (byte)(StreamDependency & 0x000000FF);

            buffer[idx++] = Weight;
        }

        public override string ToString()
        {
            return base.ToString() + $"\nStream Dependency: {StreamDependency}\nWeight: {Weight}";
        }
    }

    public class RstStreamFrame : Frame
    {
        public int ErrorCode = 0;

        public RstStreamFrame(FrameFlags flags, int errorCode, int streamId) :
            base(Frame.FrameHeaderLength + 4, FrameType.RstStream, flags, streamId)
        {
            ErrorCode = errorCode;
        }

        public static RstStreamFrame ReadFrom(Frame header, ReadOnlySpan<byte> buffer)
        {
            int idx = Frame.FrameHeaderLength;
            int errorCode = (int)((uint)((buffer[idx++] << 24) | (buffer[idx++] << 16) | (buffer[idx++] << idx++) | buffer[idx++]) & 0x7FFFFFFF);

            return new RstStreamFrame(header.Flags, errorCode, header.StreamId);
        }

        public override void WriteTo(Span<byte> buffer)
        {
            base.WriteTo(buffer);

            int idx = Frame.FrameHeaderLength;

            buffer[idx++] = (byte)((ErrorCode & 0xFF000000) >> 24);
            buffer[idx++] = (byte)((ErrorCode & 0x00FF0000) >> 16);
            buffer[idx++] = (byte)((ErrorCode & 0x0000FF00) >> 8);
            buffer[idx++] = (byte)(ErrorCode & 0x000000FF);
        }

        public override string ToString()
        {
            return base.ToString() + $"\nError Code: {ErrorCode}";
        }
    }

    public class PingFrame : Frame
    {
        public byte[] Data;

        public PingFrame(byte[] data, FrameFlags flags, int streamId) :
            base(Frame.FrameHeaderLength + 8, FrameType.Ping, flags, streamId)
        {
            Data = data;
        }

        public static PingFrame ReadFrom(Frame header, ReadOnlySpan<byte> buffer)
        {
            byte[] data = buffer.Slice(Frame.FrameHeaderLength).ToArray();

            return new PingFrame(data, header.Flags, header.StreamId);
        }

        public override void WriteTo(Span<byte> buffer)
        {
            base.WriteTo(buffer);

            Data.CopyTo(buffer.Slice(Frame.FrameHeaderLength));
        }

        public override string ToString()
        {
            return base.ToString() + $"\nOpaque Data: {string.Join(", ", Data)}";
        }
    }
}
