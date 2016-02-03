// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using Xunit;

namespace System.Security.Cryptography.Hashing.Algorithms.Tests
{
    public class InvalidUsageTests
    {
        [Fact]
        public void InvalidHashCoreArgumentsFromDerivedType()
        {
            using (var hmac = new DerivedHMACSHA1())
            {
                Assert.Throws<ArgumentNullException>(() => hmac.ExposedHashCore(null, 0, 0));
                Assert.Throws<ArgumentOutOfRangeException>(() => hmac.ExposedHashCore(new byte[1], -1, 1));
                Assert.Throws<ArgumentOutOfRangeException>(() => hmac.ExposedHashCore(new byte[1], 0, -1));
                Assert.Throws<ArgumentException>(() => hmac.ExposedHashCore(new byte[1], 0, 2));
                Assert.Throws<ArgumentException>(() => hmac.ExposedHashCore(new byte[2], 1, 2));
                Assert.Throws<ArgumentException>(() => hmac.ExposedHashCore(new byte[1], Int32.MaxValue, Int32.MaxValue));
            }
        }

        [Fact]
        public void InvalidHashCoreArgumentsFromStream()
        {
            using (SHA1 sha1 = SHA1.Create())
            {
                Assert.Throws<ArgumentException>(
                    () => sha1.ComputeHash(new BadReadStream(BadReadStream.ErrorCondition.TooLargeValueFromRead)));
                sha1.ComputeHash(new BadReadStream(BadReadStream.ErrorCondition.NegativeValueFromRead));
            }
        }

        private sealed class DerivedHMACSHA1 : HMACSHA1
        {
            public void ExposedHashCore(byte[] rgb, int ib, int cb)
            {
                HashCore(rgb, ib, cb);
            }
        }

        private sealed class BadReadStream : Stream
        {
            internal enum ErrorCondition
            {
                NegativeValueFromRead,
                TooLargeValueFromRead
            }

            private readonly ErrorCondition _condition;

            public BadReadStream(ErrorCondition condition)
            {
                _condition = condition;
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                switch (_condition)
                {
                    case ErrorCondition.NegativeValueFromRead: return -1;
                    case ErrorCondition.TooLargeValueFromRead: return buffer.Length + 1;
                    default: return 0;
                }
            }

            public override bool CanRead { get { return true; } }
            public override bool CanSeek { get { return false; } }
            public override bool CanWrite { get { return false; } }
            public override long Length { get { throw new NotSupportedException(); } }
            public override long Position { get { throw new NotSupportedException(); } set { throw new NotSupportedException(); } }
            public override void Flush() { }
            public override long Seek(long offset, SeekOrigin origin) { throw new NotSupportedException(); }
            public override void SetLength(long value) { throw new NotSupportedException(); }
            public override void Write(byte[] buffer, int offset, int count) { throw new NotSupportedException(); }
        }
    }
}
