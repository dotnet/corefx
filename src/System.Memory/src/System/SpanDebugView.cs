using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System
{
    internal sealed class SpanDebugView<T>
    {
        private readonly T[] _pinnable;
        private readonly IntPtr _byteOffset;
        private readonly int _length;

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
                int elementSize = typeof(T).IsValueType ? Unsafe.SizeOf<T>() : IntPtr.Size;//Workaround to VIL bug where Unsafe.SizeOf<T> where T is a class, returns the size of its fields instead of IntPtr.Size

                if (_pinnable == null)
                {
                    var result = new T[_length];
                    byte* source = (byte*)_byteOffset.ToPointer();
                    byte* fixedResult = (byte*)Unsafe.AsPointer(ref result[0]);

                    int totalSize = _length * elementSize;

                    for (int i = 0; i < totalSize; i++)
                    {
                        *fixedResult = *source;
                        fixedResult++;
                        source++;
                    }

                    return result;
                }
                else
                {
                    var _byteOffsetInt32 = _byteOffset.ToInt32();
                    var sourceIndex = (_byteOffsetInt32 - IntPtr.Size) / elementSize;

                    var result = new T[_length];

                    Array.Copy(_pinnable, sourceIndex, result, 0, _length);

                    return result;
                }
            }
        }
    }
}
