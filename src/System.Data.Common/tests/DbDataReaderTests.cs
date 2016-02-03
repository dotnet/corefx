// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using Xunit;

namespace System.Data.Common.Tests
{
    public class DbDataReaderTests
    {
        private static volatile bool _wasFinalized;

        private class FinalizingReader : DbDataReader
        {
            public static void CreateAndRelease()
            {
                new FinalizingReader();
            }

            protected override void Dispose(bool disposing)
            {
                if (!disposing)
                    _wasFinalized = true;
                base.Dispose(disposing);
            }

            public override object this[string name]
            {
                get { throw new NotImplementedException(); }
            }

            public override object this[int ordinal]
            {
                get { throw new NotImplementedException(); }
            }

            public override int Depth
            {
                get { throw new NotImplementedException(); }
            }

            public override int FieldCount
            {
                get { throw new NotImplementedException(); }
            }

            public override bool HasRows
            {
                get { throw new NotImplementedException(); }
            }

            public override bool IsClosed
            {
                get { throw new NotImplementedException(); }
            }

            public override int RecordsAffected
            {
                get { throw new NotImplementedException(); }
            }

            public override bool GetBoolean(int ordinal)
            {
                throw new NotImplementedException();
            }

            public override byte GetByte(int ordinal)
            {
                throw new NotImplementedException();
            }

            public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
            {
                throw new NotImplementedException();
            }

            public override char GetChar(int ordinal)
            {
                throw new NotImplementedException();
            }

            public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
            {
                throw new NotImplementedException();
            }

            public override string GetDataTypeName(int ordinal)
            {
                throw new NotImplementedException();
            }

            public override DateTime GetDateTime(int ordinal)
            {
                throw new NotImplementedException();
            }

            public override decimal GetDecimal(int ordinal)
            {
                throw new NotImplementedException();
            }

            public override double GetDouble(int ordinal)
            {
                throw new NotImplementedException();
            }

            public override IEnumerator GetEnumerator()
            {
                throw new NotImplementedException();
            }

            public override Type GetFieldType(int ordinal)
            {
                throw new NotImplementedException();
            }

            public override float GetFloat(int ordinal)
            {
                throw new NotImplementedException();
            }

            public override Guid GetGuid(int ordinal)
            {
                throw new NotImplementedException();
            }

            public override short GetInt16(int ordinal)
            {
                throw new NotImplementedException();
            }

            public override int GetInt32(int ordinal)
            {
                throw new NotImplementedException();
            }

            public override long GetInt64(int ordinal)
            {
                throw new NotImplementedException();
            }

            public override string GetName(int ordinal)
            {
                throw new NotImplementedException();
            }

            public override int GetOrdinal(string name)
            {
                throw new NotImplementedException();
            }

            public override string GetString(int ordinal)
            {
                throw new NotImplementedException();
            }

            public override object GetValue(int ordinal)
            {
                throw new NotImplementedException();
            }

            public override int GetValues(object[] values)
            {
                throw new NotImplementedException();
            }

            public override bool IsDBNull(int ordinal)
            {
                throw new NotImplementedException();
            }

            public override bool NextResult()
            {
                throw new NotImplementedException();
            }

            public override bool Read()
            {
                throw new NotImplementedException();
            }
        }

        [Fact]
        public void CanBeFinalized()
        {
            FinalizingReader.CreateAndRelease();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Assert.True(_wasFinalized);
        }
    }
}
