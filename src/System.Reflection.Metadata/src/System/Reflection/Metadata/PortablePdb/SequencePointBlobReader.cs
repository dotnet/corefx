// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Internal;
using System.Reflection.Metadata.Ecma335;

namespace System.Reflection.Metadata
{
    [DebuggerDisplay("{GetDebuggerDisplay(),nq}")]
    public struct SequencePointBlobReader : IEnumerator<SequencePoint>
    {
        private BlobReader _reader;
        private SequencePoint _current;

        public unsafe SequencePointBlobReader(byte* buffer, int length)
            : this(MemoryBlock.CreateChecked(buffer, length))
        {
        }

        internal SequencePointBlobReader(MemoryBlock block)
        {
            _reader = new BlobReader(block);
            _current = default(SequencePoint);
        }

        public bool MoveNext()
        {
            if (_reader.RemainingBytes == 0)
            {
                return false;
            }

            DocumentHandle document;
            int offset, deltaLines, deltaColumns, startLine;
            ushort startColumn;

            if (_current.Document.IsNil)
            {
                // first record:
                document = ReadDocumentHandle();
                offset = _reader.ReadCompressedInteger();
                ReadDeltaLinesAndColumns(out deltaLines, out deltaColumns);
                startLine = _reader.ReadCompressedInteger();
                startColumn = ReadColumn();

                // hidden SP:
                if (deltaLines == 0 && deltaColumns == 0)
                {
                    _current = new SequencePoint(document, offset);
                    return true;
                }
            }
            else
            {
                int deltaOffset;
                document = _current.Document;

                while ((deltaOffset = _reader.ReadCompressedInteger()) == 0)
                {
                    // subsequent document record
                    document = ReadDocumentHandle();
                }

                // subsequent point record
                offset = AddOffsets(_current.Offset, deltaOffset);
                ReadDeltaLinesAndColumns(out deltaLines, out deltaColumns);

                if (deltaLines == 0 && deltaColumns == 0)
                {
                    // hidden
                    _current = new SequencePoint(_current.Document, offset);
                    return true;
                }

                startLine = AddLines(_current.StartLine, _reader.ReadCompressedSignedInteger());
                startColumn = AddColumns(_current.StartColumn, _reader.ReadCompressedSignedInteger());
            }

            _current = new SequencePoint(
                document,
                offset,
                startLine,
                startColumn,
                AddLines(startLine, deltaLines),
                AddColumns(startColumn, deltaColumns));

            return true;
        }

        private void ReadDeltaLinesAndColumns(out int deltaLines, out int deltaColumns)
        {
            deltaLines = _reader.ReadCompressedInteger();
            deltaColumns = (deltaLines == 0) ? _reader.ReadCompressedInteger() : _reader.ReadCompressedSignedInteger();
        }

        private ushort ReadColumn()
        {
            int column = _reader.ReadCompressedInteger();
            if (column > ushort.MaxValue)
            {
                // TODO:
                throw new BadImageFormatException();
            }

            return (ushort)column;
        }

        private int AddOffsets(int value, int delta)
        {
            // TODO:
            int result = unchecked(value + delta);
            if (result < 0 || result > int.MaxValue)
            {
                throw new BadImageFormatException();
            }

            return result;
        }

        private int AddLines(int value, int delta)
        {
            // TODO:
            int result = unchecked(value + delta);
            if (result < 0 || result >= SequencePoint.HiddenLine)
            {
                throw new BadImageFormatException();
            }

            return result;
        }

        private ushort AddColumns(ushort value, int delta)
        {
            // TODO:
            int result = unchecked(value + delta);
            if (result < 0 || result >= ushort.MaxValue)
            {
                throw new BadImageFormatException();
            }

            return (ushort)result;
        }

        private DocumentHandle ReadDocumentHandle()
        {
            int rowId = _reader.ReadCompressedInteger();
            if (rowId == 0 || !TokenTypeIds.IsValidRowId(rowId))
            {
                NamespaceCache.ThrowInvalidHandle();
            }

            return DocumentHandle.FromRowId(rowId);
        }

        public SequencePoint Current
        {
            get { return _current; }
        }

        object IEnumerator.Current
        {
            get { return _current; }
        }

        public void Reset()
        {
            _reader.SeekOffset(0);
            _current = default(SequencePoint);
        }

        public void Dispose()
        {
        }

        private string GetDebuggerDisplay()
        {
            return _reader.GetDebuggerDisplay();
        }
    }
}
