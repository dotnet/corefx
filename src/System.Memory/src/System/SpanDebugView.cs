﻿using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace System
{
    internal sealed class SpanDebugView<T>
    {
        private readonly T[] _pinnable;
        private readonly IntPtr _byteOffset;
        private readonly long _length;

        public SpanDebugView(Span<T> collection)
        {
            _pinnable = (T[])(object)collection.Pinnable;
            _byteOffset = collection.ByteOffset;
            _length = collection.Length;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public unsafe T[] Items
        {
            get
            {
                int elementSize = typeof(T).GetTypeInfo().IsValueType ? Unsafe.SizeOf<T>() : IntPtr.Size;//Workaround to VIL bug where Unsafe.SizeOf<T> where T is a class, returns the size of its fields instead of IntPtr.Size
                T[] result = new T[_length];

                if (_pinnable == null)
                {
                    byte* source = (byte*)_byteOffset.ToPointer();

                    for (long i = 0; i < result.LongLength; i++)
                    {
                        result[i] = Unsafe.Read<T>(source);
                        source = source + elementSize;
                    }
                }
                else
                {
                    long _byteOffsetInt = _byteOffset.ToInt64();
                    long sourceIndex = (_byteOffsetInt - IntPtr.Size) / elementSize;

                    for (long i = 0; i < result.LongLength; i++)
                    {
                        result[i] = _pinnable[i + sourceIndex];
                    }
                }

                return result;
            }
        }
    }
}
