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
        private BlobReader reader;
        private SequencePoint current;

        public unsafe SequencePointBlobReader(byte* buffer, int length)
            : this(MemoryBlock.CreateChecked(buffer, length))
        {
        }

        internal SequencePointBlobReader(MemoryBlock block)
        {
            this.reader = new BlobReader(block);
            this.current = default(SequencePoint);
        }

        public bool MoveNext()
        {
            if (reader.RemainingBytes == 0)
            {
                return false;
            }

            DocumentHandle document;
            int offset, deltaLines, deltaColumns, startLine;
            ushort startColumn;

            if (current.Document.IsNil)
            {
                // first record:
                document = ReadDocumentHandle();
                offset = reader.ReadCompressedInteger();
                ReadDeltaLinesAndColumns(out deltaLines, out deltaColumns);
                startLine = reader.ReadCompressedInteger();
                startColumn = ReadColumn();

                // hidden SP:
                if (deltaLines == 0 && deltaColumns == 0)
                {
                    current = new SequencePoint(document, offset);
                    return true;
                }
            }
            else
            {
                int deltaOffset;
                document = current.Document;

                while ((deltaOffset = reader.ReadCompressedInteger()) == 0)
                {
                    // subsequent document record
                    document = ReadDocumentHandle();
                }

                // subsequent point record
                offset = AddOffsets(current.Offset, deltaOffset);
                ReadDeltaLinesAndColumns(out deltaLines, out deltaColumns);

                if (deltaLines == 0 && deltaColumns == 0)
                {
                    // hidden
                    current = new SequencePoint(current.Document, offset);
                    return true;
                }

                startLine = AddLines(current.StartLine, reader.ReadCompressedSignedInteger());
                startColumn = AddColumns(current.StartColumn, reader.ReadCompressedSignedInteger());
            }

            current = new SequencePoint(
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
            deltaLines = reader.ReadCompressedInteger();
            deltaColumns = (deltaLines == 0) ? reader.ReadCompressedInteger() : reader.ReadCompressedSignedInteger();
        }

        private ushort ReadColumn()
        {
            int column = reader.ReadCompressedInteger();
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
            uint rowId = (uint)reader.ReadCompressedInteger();
            if (rowId == 0 || !TokenTypeIds.IsValidRowId(rowId))
            {
                NamespaceCache.ThrowInvalidHandle(); 
            }

            return DocumentHandle.FromRowId(rowId);
        }

        public SequencePoint Current
        {
            get { return current; }
        }

        object IEnumerator.Current
        {
            get { return current; }
        }

        public void Reset()
        {
            reader.SeekOffset(0);
            current = default(SequencePoint);
        }

        public void Dispose()
        {
        }

        private string GetDebuggerDisplay()
        {
            return reader.GetDebuggerDisplay();
        }
    }
}
