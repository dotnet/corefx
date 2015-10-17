// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace System.IO
{
    /* SyncTextReader intentionally locks on itself rather than a private lock object.
     * This is done to synchronize different console readers(Issue#2855).
     */
    internal sealed partial class SyncTextReader : TextReader
    {
        internal readonly TextReader _in;

        public static SyncTextReader GetSynchronizedTextReader(TextReader reader)
        {
            Debug.Assert(reader != null);
            return reader as SyncTextReader ??
                new SyncTextReader(reader);
        }

        internal SyncTextReader(TextReader t)
        {
            _in = t;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (this)
                {
                    _in.Dispose();
                }
            }
        }

        public override int Peek()
        {
            lock (this)
            {
                return _in.Peek();
            }
        }

        public override int Read()
        {
            lock (this)
            {
                return _in.Read();
            }
        }

        public override int Read([In, Out] char[] buffer, int index, int count)
        {
            lock (this)
            {
                return _in.Read(buffer, index, count);
            }
        }

        public override int ReadBlock([In, Out] char[] buffer, int index, int count)
        {
            lock (this)
            {
                return _in.ReadBlock(buffer, index, count);
            }
        }

        public override String ReadLine()
        {
            lock (this)
            {
                return _in.ReadLine();
            }
        }

        public override String ReadToEnd()
        {
            lock (this)
            {
                return _in.ReadToEnd();
            }
        }

        //
        // On SyncTextReader all APIs should run synchronously, even the async ones.
        // No explicit locking is needed, as they all just delegate
        //

        public override Task<String> ReadLineAsync()
        {
            return Task.FromResult(ReadLine());
        }

        public override Task<String> ReadToEndAsync()
        {
            return Task.FromResult(ReadToEnd());
        }

        public override Task<int> ReadBlockAsync(char[] buffer, int index, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer", SR.ArgumentNull_Buffer);
            if (index < 0 || count < 0)
                throw new ArgumentOutOfRangeException((index < 0 ? "index" : "count"), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (buffer.Length - index < count)
                throw new ArgumentException(SR.Argument_InvalidOffLen);
            Contract.EndContractBlock();

            return Task.FromResult(ReadBlock(buffer, index, count));
        }

        public override Task<int> ReadAsync(char[] buffer, int index, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer", SR.ArgumentNull_Buffer);
            if (index < 0 || count < 0)
                throw new ArgumentOutOfRangeException((index < 0 ? "index" : "count"), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (buffer.Length - index < count)
                throw new ArgumentException(SR.Argument_InvalidOffLen);
            Contract.EndContractBlock();

            return Task.FromResult(Read(buffer, index, count));
        }
    }
}
