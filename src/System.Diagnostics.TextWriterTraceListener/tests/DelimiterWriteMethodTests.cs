// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using Xunit;

namespace System.Diagnostics.TextWriterTraceListenerTests
{
    public class DelimiterWriteMethodTests : IDisposable
    {
        private readonly Stream _stream;
        private readonly string _fileName;

        public DelimiterWriteMethodTests()
        {
            _fileName = string.Format("{0}.xml", GetType().Name);
            CommonUtilities.DeleteFile(_fileName);
            _stream = new FileStream(_fileName, FileMode.OpenOrCreate, FileAccess.Write);
        }

        public static IEnumerable<object[]> TraceEventInvariants
        {
            get
            {
                TraceFilter nullFilter = null;
                TraceFilter trueFilter = new TestTraceFilter(true);
                TraceFilter falseFilter = new TestTraceFilter(false);
                TraceEventCache nullCache = null;
                TraceEventCache testCache = new TraceEventCache();
                const string format = "Dummy format \"msg: {0} and {1}";
                const string message = "Dummy message";
                object[] args = new object[] { "Hello", 6 };

                return new[]
                {
                    new object[] { nullFilter,  nullCache, "Co1971",     TraceEventType.Critical,    42, format, args },
                    new object[] { nullFilter,  testCache, "Co\"1984\"", TraceEventType.Verbose,     12, message, null},
                    new object[] { trueFilter,  nullCache, "Co1971",     TraceEventType.Error,       24, format, args },
                    new object[] { falseFilter, testCache, "Co\"1984\"", TraceEventType.Information, 33, message, null }
                };
            }
        }

        [Theory]
        [MemberData(nameof(TraceEventInvariants))]
        public void TraceEvent_FormatString_Test(TraceFilter filter, TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, object[] args)
        {
            using (var target = new DelimitedListTraceListener(_stream))
            {
                target.Filter = filter;
                target.TraceOutputOptions = TraceOptions.ProcessId | TraceOptions.ThreadId | TraceOptions.DateTime | TraceOptions.Timestamp;
                target.TraceEvent(eventCache, source, eventType, id, format, args);
            }

            string expected = CommonUtilities.ExpectedTraceEventOutput(filter, eventCache, source, eventType, id, format, args);
            Assert.Equal(expected, File.ReadAllText(_fileName));
        }

        [Theory]
        [MemberData(nameof(TraceEventInvariants))]
        public void TraceEvent_String_Test(TraceFilter filter, TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, object[] args)
        {
            string message = args != null ? string.Format(format, args) : format;
            using (var target = new DelimitedListTraceListener(_stream))
            {
                target.Filter = filter;
                target.TraceOutputOptions = TraceOptions.ProcessId | TraceOptions.ThreadId | TraceOptions.DateTime | TraceOptions.Timestamp;
                target.TraceEvent(eventCache, source, eventType, id, message);
            }

            string expected = CommonUtilities.ExpectedTraceEventOutput(filter, eventCache, source, eventType, id, format, args);
            Assert.Equal(expected, File.ReadAllText(_fileName));
        }

        public static IEnumerable<object[]> TraceDataObjectInvariants
        {
            get
            {
                TraceFilter nullFilter = null;
                TraceFilter trueFilter = new TestTraceFilter(true);
                TraceFilter falseFilter = new TestTraceFilter(false);
                TraceEventCache nullCache = null;
                TraceEventCache testCache = new TraceEventCache();
                const string message = "Dummy message";

                return new[]
                {
                    new object[] { nullFilter,  nullCache, "Co1971",     TraceEventType.Critical,    42, decimal.MaxValue },
                    new object[] { nullFilter,  testCache, "Co\"1984\"", TraceEventType.Verbose,     12, message },
                    new object[] { trueFilter,  nullCache, "Co1971",     TraceEventType.Error,       24, decimal.MinusOne },
                    new object[] { falseFilter, testCache, "Co\"1984\"", TraceEventType.Information, 33, message }
                };
            }
        }

        [Theory]
        [MemberData(nameof(TraceDataObjectInvariants))]
        public void TraceData_Object_Test(TraceFilter filter, TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
        {
            using (var target = new DelimitedListTraceListener(_stream))
            {
                target.Filter = filter;
                target.TraceOutputOptions = TraceOptions.ProcessId | TraceOptions.ThreadId | TraceOptions.DateTime | TraceOptions.Timestamp;
                target.TraceData(eventCache, source, eventType, id, data);
            }

            string expected = CommonUtilities.ExpectedTraceDataOutput(filter, eventCache, source, eventType, id, data);
            Assert.Equal(expected, File.ReadAllText(_fileName));
        }

        public static IEnumerable<object[]> TraceDataObjectArrayInvariants
        {
            get
            {
                TraceFilter nullFilter = null;
                TraceFilter trueFilter = new TestTraceFilter(true);
                TraceFilter falseFilter = new TestTraceFilter(false);
                TraceEventCache nullCache = null;
                TraceEventCache testCache = new TraceEventCache();

                return new[]
                {
                    new object[] { ",", nullFilter,  nullCache, "Co1971",     TraceEventType.Critical,    42, new object[0] },
                    new object[] { ";", nullFilter,  testCache, "Co\"1984\"", TraceEventType.Verbose,     12, null },
                    new object[] { ".", trueFilter,  nullCache, "Co1971",     TraceEventType.Error,       24, new object[] { "Hello", 6 } },
                    new object[] { ";", falseFilter, testCache, "Co\"1984\"", TraceEventType.Information, 33, null }
                };
            }
        }

        [Theory]
        [MemberData(nameof(TraceDataObjectArrayInvariants))]
        public void TraceData_ObjectArray_Test(string delimiter,TraceFilter filter, TraceEventCache eventCache, string source, TraceEventType eventType, int id, object[] data)
        {
            using (var target = new DelimitedListTraceListener(_stream))
            {
                target.Delimiter = delimiter;
                target.Filter = filter;
                target.TraceOutputOptions = TraceOptions.ProcessId | TraceOptions.ThreadId | TraceOptions.DateTime | TraceOptions.Timestamp;
                target.TraceData(eventCache, source, eventType, id, data);
            }

            string expected = CommonUtilities.ExpectedTraceDataOutput(delimiter, filter, eventCache, source, eventType, id, data);
            Assert.Equal(expected, File.ReadAllText(_fileName));
        }

        public void Dispose()
        {
            _stream.Dispose();
            CommonUtilities.DeleteFile(_fileName);
        }
    }
}
