// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;

namespace System.IO
{
    public partial class StringWriter
    {
        // Writes a string segment to the underlying string buffer. If the given string is
        // null, nothing is written.
        //
        public override void Write(StringSegment value)
        {
            if (!_isOpen)
            {
                throw new ObjectDisposedException(null, SR.ObjectDisposed_WriterClosed);
            }

            if (!value.IsEmpty)
            {
                var buffer = value.GetBuffer(out int offset, out int length);
                _sb.Append(buffer, offset, length);
            }
        }

        public override Task WriteLineAsync(StringSegment value)
        {
            WriteLine(value);
            return Task.CompletedTask;
        }
    }
}
