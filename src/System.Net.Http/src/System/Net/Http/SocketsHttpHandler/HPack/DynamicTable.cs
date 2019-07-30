// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0.
// See THIRD-PARTY-NOTICES.TXT in the project root for license information.

namespace System.Net.Http.HPack
{
    internal class DynamicTable
    {
        private HeaderField[] _buffer;
        private int _maxSize;
        private int _size;
        private int _count;
        private int _insertIndex;
        private int _removeIndex;

        public DynamicTable(int maxSize)
        {
            _buffer = new HeaderField[maxSize / HeaderField.RfcOverhead];
            _maxSize = maxSize;
        }

        public int Count => _count;

        public int Size => _size;

        public int MaxSize => _maxSize;

        public HeaderField this[int index]
        {
            get
            {
                if (index >= _count)
                {
                    throw new IndexOutOfRangeException();
                }

                index = _insertIndex - index - 1;

                if (index < 0)
                {
                    index += _buffer.Length;
                }

                return _buffer[index];
            }
        }

        public void Insert(ReadOnlySpan<byte> name, ReadOnlySpan<byte> value)
        {
            int entryLength = HeaderField.GetLength(name.Length, value.Length);
            EnsureAvailable(entryLength);

            if (entryLength > _maxSize)
            {
                // http://httpwg.org/specs/rfc7541.html#rfc.section.4.4
                // It is not an error to attempt to add an entry that is larger than the maximum size;
                // an attempt to add an entry larger than the maximum size causes the table to be emptied
                // of all existing entries and results in an empty table.
                return;
            }

            var entry = new HeaderField(name, value);
            _buffer[_insertIndex] = entry;
            _insertIndex = (_insertIndex + 1) % _buffer.Length;
            _size += entry.Length;
            _count++;
        }

        public void Resize(int maxSize)
        {
            // TODO: What would cause us to need to grow the table size? The connection-level limit should prevent this, right?
            // Understand this better. If we do need to resize, we may want a better resize strategy.
            if (maxSize > _maxSize)
            {
                var newBuffer = new HeaderField[maxSize / HeaderField.RfcOverhead];

                for (var i = 0; i < Count; i++)
                {
                    newBuffer[i] = _buffer[i];
                }

                _buffer = newBuffer;
                _maxSize = maxSize;
            }
            else
            {
                _maxSize = maxSize;
                EnsureAvailable(0);
            }
        }

        private void EnsureAvailable(int available)
        {
            while (_count > 0 && _maxSize - _size < available)
            {
                _size -= _buffer[_removeIndex].Length;
                _count--;
                _removeIndex = (_removeIndex + 1) % _buffer.Length;
            }
        }
    }
}
