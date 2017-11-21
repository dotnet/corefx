// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.UnitTesting;
using System.Collections.Generic;
using System.ComponentModel.Composition.Diagnostics;
using Xunit;

namespace System.ComponentModel.Composition.Diagnostics
{
    public class ComposableTraceSourceTests
    {
#if FEATURE_TRACING
        [Fact]
        public void CanWriteInformation_ShouldReturnFalseByDefault()
        {
            Assert.False(CompositionTraceSource.CanWriteInformation);
        }

        [Fact]
        public void CanWriteWarning_ShouldReturnTrueByDefault()
        {
            Assert.True(CompositionTraceSource.CanWriteWarning);
        }

        [Fact]
        public void CanWriteError_ShouldReturnTrueByDefault()
        {
            Assert.True(CompositionTraceSource.CanWriteError);
        }

        [Fact]
        public void CanWriteInformation_WhenSwitchLevelLessThanInformation_ShouldReturnFalse()
        {
            var levels = GetSourceLevelsLessThan(SourceLevels.Information);
            foreach (var level in levels)
            {
                using (new TraceContext(level))
                {
                    Assert.False(CompositionTraceSource.CanWriteInformation);
                }
            }
        }

        [Fact]
        public void CanWriteInformation_WhenSwitchLevelGreaterThanOrEqualToInformation_ShouldReturnTrue()
        {
            var levels = GetSourceLevelsGreaterThanOrEqualTo(SourceLevels.Information);
            foreach (var level in levels)
            {
                using (new TraceContext(level))
                {
                    Assert.True(CompositionTraceSource.CanWriteInformation);
                }
            }
        }

        [Fact]
        public void CanWriteWarning_WhenSwitchLevelLessThanWarning_ShouldReturnFalse()
        {
            var levels = GetSourceLevelsLessThan(SourceLevels.Warning);
            foreach (var level in levels)
            {
                using (new TraceContext(level))
                {
                    Assert.False(CompositionTraceSource.CanWriteWarning);
                }
            }
        }

        [Fact]
        public void CanWriteWarning_WhenSwitchLevelGreaterThanOrEqualToWarning_ShouldReturnTrue()
        {
            var levels = GetSourceLevelsGreaterThanOrEqualTo(SourceLevels.Warning);
            foreach (var level in levels)
            {
                using (new TraceContext(level))
                {
                    Assert.True(CompositionTraceSource.CanWriteWarning);
                }
            }
        }

        [Fact]
        public void CanWriteError_WhenSwitchLevelLessThanError_ShouldReturnFalse()
        {
            var levels = GetSourceLevelsLessThan(SourceLevels.Error);
            foreach (var level in levels)
            {
                using (new TraceContext(level))
                {
                    Assert.False(CompositionTraceSource.CanWriteError);
                }
            }
        }

        [Fact]
        public void CanWriteError_WhenSwitchLevelGreaterThanOrEqualToError_ShouldReturnTrue()
        {
            var levels = GetSourceLevelsGreaterThanOrEqualTo(SourceLevels.Error);
            foreach (var level in levels)
            {
                using (new TraceContext(level))
                {
                    Assert.True(CompositionTraceSource.CanWriteError);
                }
            }
        }

        [Fact]
        public void WriteInformation_WhenSwitchLevelLessThanInformation_ShouldThrowInternalError()
        {
            var levels = GetSourceLevelsLessThan(SourceLevels.Information);
            foreach (var level in levels)
            {
                using (TraceContext context = new TraceContext(level))
                {
                    ThrowsInternalError(() =>
                    {
                        CompositionTraceSource.WriteInformation(0, "format", "arguments");
                    });
                }
            }
        }

        [Fact]
        public void WriteInformation_WhenSwitchLevelGreaterThanOrEqualToInformation_ShouldWriteToTraceListener()
        {
            var levels = GetSourceLevelsGreaterThanOrEqualTo(SourceLevels.Information);
            foreach (var level in levels)
            {
                using (TraceContext context = new TraceContext(level))
                {
                    CompositionTraceSource.WriteInformation(0, "format", "arguments");

                    Assert.NotNull(context.LastTraceEvent);
                }
            }
        }

        [Fact]
        public void WriteWarning_WhenSwitchLevelLessThanWarning_ShouldThrowInternalError()
        {
            var levels = GetSourceLevelsLessThan(SourceLevels.Warning);
            foreach (var level in levels)
            {
                using (TraceContext context = new TraceContext(level))
                {
                    ThrowsInternalError(() =>
                    {
                        CompositionTraceSource.WriteWarning(0, "format", "arguments");
                    });
                }
            }
        }

        [Fact]
        public void WriteWarning_WhenSwitchLevelGreaterThanOrEqualToWarning_ShouldWriteToTraceListener()
        {
            var levels = GetSourceLevelsGreaterThanOrEqualTo(SourceLevels.Information);
            foreach (var level in levels)
            {
                using (TraceContext context = new TraceContext(level))
                {
                    CompositionTraceSource.WriteWarning(0, "format", "arguments");

                    Assert.NotNull(context.LastTraceEvent);
                }
            }
        }

        [Fact]
        public void WriteError_WhenSwitchLevelLessThanError_ShouldThrowInternalError()
        {
            var levels = GetSourceLevelsLessThan(SourceLevels.Error);
            foreach (var level in levels)
            {
                using (TraceContext context = new TraceContext(level))
                {
                    ThrowsInternalError(() =>
                    {
                        CompositionTraceSource.WriteError(0, "format", "arguments");
                    });
                }
            }
        }

        [Fact]
        public void WriteError_WhenSwitchLevelGreaterThanOrEqualToError_ShouldWriteToTraceListener()
        {
            var levels = GetSourceLevelsGreaterThanOrEqualTo(SourceLevels.Error);
            foreach (var level in levels)
            {
                using (TraceContext context = new TraceContext(level))
                {
                    CompositionTraceSource.WriteError(0, "format", "arguments");

                    Assert.NotNull(context.LastTraceEvent);
                }
            }
        }

        [Fact]
        public void WriteInformation_ShouldWriteTraceEventTypeInformationToTraceListener()
        {
            using (var context = new TraceContext(SourceLevels.Information))
            {
                CompositionTraceSource.WriteInformation(0, "format", "arguments");

                Assert.Equal(TraceEventType.Information, context.LastTraceEvent.EventType);
            }
        }

        [Fact]
        public void WriteWarning_ShouldWriteTraceEventTypeWarningToTraceListener()
        {
            using (var context = new TraceContext(SourceLevels.Warning))
            {
                CompositionTraceSource.WriteWarning(0, "format", "arguments");

                Assert.Equal(TraceEventType.Warning, context.LastTraceEvent.EventType);
            }
        }

        [Fact]
        public void WriteError_ShouldWriteTraceEventTypeWarningToTraceListener()
        {
            using (var context = new TraceContext(SourceLevels.Error))
            {
                CompositionTraceSource.WriteError(0, "format", "arguments");

                Assert.Equal(TraceEventType.Error, context.LastTraceEvent.EventType);
            }
        }

        [Fact]
        public void WriteInformation_ShouldWriteCorrectSourceToTraceListener()
        {
            using (var context = new TraceContext(SourceLevels.Information))
            {
                CompositionTraceSource.WriteInformation(0, "format", "arguments");

                Assert.Equal("System.ComponentModel.Composition", context.LastTraceEvent.Source);
            }
        }

        [Fact]
        public void WriteWarning_ShouldWriteCorrectSourceToTraceListener()
        {
            using (var context = new TraceContext(SourceLevels.Warning))
            {
                CompositionTraceSource.WriteWarning(0, "format", "arguments");

                Assert.Equal("System.ComponentModel.Composition", context.LastTraceEvent.Source);
            }
        }

        [Fact]
        public void WriteError_ShouldWriteCorrectSourceToTraceListener()
        {
            using (var context = new TraceContext(SourceLevels.Error))
            {
                CompositionTraceSource.WriteError(0, "format", "arguments");

                Assert.Equal("System.ComponentModel.Composition", context.LastTraceEvent.Source);
            }
        }

        [Fact]
        public void WriteInformation_ValueAsTraceId_ShouldWriteIdToTraceListener()
        {
            using (var context = new TraceContext(SourceLevels.Information))
            {
                var expectations = Expectations.GetEnumValues<CompositionTraceId>();
                foreach (var e in expectations)
                {
                    CompositionTraceSource.WriteInformation(e, "format", "arguments");

                    Assert.Equal(e, (CompositionTraceId)context.LastTraceEvent.Id);
                }
            }
        }

        [Fact]
        public void WriteWarning_ValueAsTraceId_ShouldWriteIdToTraceListener()
        {
            using (var context = new TraceContext(SourceLevels.Warning))
            {
                var expectations = Expectations.GetEnumValues<CompositionTraceId>();
                foreach (var e in expectations)
                {
                    CompositionTraceSource.WriteWarning(e, "format", "arguments");

                    Assert.Equal(e, (CompositionTraceId)context.LastTraceEvent.Id);
                }
            }
        }

        [Fact]
        public void WriteError_ValueAsTraceId_ShouldWriteIdToTraceListener()
        {
            using (var context = new TraceContext(SourceLevels.Error))
            {
                var expectations = Expectations.GetEnumValues<CompositionTraceId>();
                foreach (var e in expectations)
                {
                    CompositionTraceSource.WriteError(e, "format", "arguments");

                    Assert.Equal(e, (CompositionTraceId)context.LastTraceEvent.Id);
                }
            }
        }

        [Fact]
        public void WriteInformation_ValueAsFormat_ShouldWriteFormatToTraceListener()
        {
            using (var context = new TraceContext(SourceLevels.Information))
            {
                var expectations = Expectations.GetExceptionMessages();
                foreach (var e in expectations)
                {
                    CompositionTraceSource.WriteInformation(0, e, "arguments");

                    Assert.Equal(e, context.LastTraceEvent.Format);
                }
            }
        }

        [Fact]
        public void WriteWarning_ValueAsFormat_ShouldWriteFormatToTraceListener()
        {
            using (var context = new TraceContext(SourceLevels.Warning))
            {
                var expectations = Expectations.GetExceptionMessages();
                foreach (var e in expectations)
                {
                    CompositionTraceSource.WriteWarning(0, e, "arguments");

                    Assert.Equal(e, context.LastTraceEvent.Format);
                }
            }
        }

        [Fact]
        public void WriteError_ValueAsFormat_ShouldWriteFormatToTraceListener()
        {
            using (var context = new TraceContext(SourceLevels.Error))
            {
                var expectations = Expectations.GetExceptionMessages();
                foreach (var e in expectations)
                {
                    CompositionTraceSource.WriteError(0, e, "arguments");

                    Assert.Equal(e, context.LastTraceEvent.Format);
                }
            }
        }

        [Fact]
        public void WriteInformation_ValueAsArgs_ShouldWriteArgsToTraceListener()
        {
            using (var context = new TraceContext(SourceLevels.Information))
            {
                var expectations = Expectations.GetObjectArraysWithNull();
                foreach (var e in expectations)
                {
                    CompositionTraceSource.WriteInformation(0, "format", e);

                    Assert.Same(e, context.LastTraceEvent.Args);
                }
            }
        }

        [Fact]
        public void WriteWarning_ValueAsArgs_ShouldWriteArgsToTraceListener()
        {
            using (var context = new TraceContext(SourceLevels.Warning))
            {
                var expectations = Expectations.GetObjectArraysWithNull();
                foreach (var e in expectations)
                {
                    CompositionTraceSource.WriteWarning(0, "format", e);

                    Assert.Same(e, context.LastTraceEvent.Args);
                }
            }
        }

        [Fact]
        public void WriteError_ValueAsArgs_ShouldWriteArgsToTraceListener()
        {
            using (var context = new TraceContext(SourceLevels.Error))
            {
                var expectations = Expectations.GetObjectArraysWithNull();
                foreach (var e in expectations)
                {
                    CompositionTraceSource.WriteError(0, "format", e);

                    Assert.Same(e, context.LastTraceEvent.Args);
                }
            }
        }

        private static IEnumerable<SourceLevels> GetSourceLevelsLessThan(SourceLevels level)
        {
            return GetOnSourceLevels(level, false);
        }

        private static IEnumerable<SourceLevels> GetSourceLevelsGreaterThanOrEqualTo(SourceLevels level)
        {
            return GetOnSourceLevels(level, true);
        }

        private static IEnumerable<SourceLevels> GetOnSourceLevels(SourceLevels sourceLevel, bool on)
        {
            // SourceSwitch determines if a particular level gets traced based on whether its bit is
            // set in the current level. For example, if the current level was Warning (0000 0111),
            // then Warning (0000 0111), Error (0000 0011), and Critical (0000 0001) all get traced.

            var levels = TestServices.GetEnumValues<SourceLevels>();

            foreach (var level in levels)
            {
                if (level == 0)
                    continue;
                
                if (((level & sourceLevel) == sourceLevel) == on)
                {
                    yield return level;
                }
            }

            if (!on)
            {   
                yield return SourceLevels.Off;
            }
        }

        private static void ThrowsInternalError(Action action)
        {
            try
            {
                action();
                Assert.False(true);
            }
            catch (Exception ex)
            {
                Type exceptionType = ex.GetType();

                // The exception should not be a 
                // publicily catchable exception
                Assert.False(exceptionType.IsVisible);
            }
        }
#else
        [Fact]
        public void CanWriteInformation_ShouldReturnFalse()
        {
            Assert.False(CompositionTraceSource.CanWriteInformation);
        }

        [Fact]
        public void CanWriteWarning_ShouldReturnDebuggerLogging()
        {
            Assert.Equal(CompositionTraceSource.CanWriteWarning, Debugger.IsLogging());
        }

        [Fact]
        public void CanWriteError_ShouldReturnDebuggerLogging()
        {
            Assert.Equal(CompositionTraceSource.CanWriteError, Debugger.IsLogging());
        }

        [Fact]
        public void CreateLogMessage_ContainsTraceEventType()
        {
            IEnumerable<DebuggerTraceWriter.TraceEventType> eventTypes = Expectations.GetEnumValues<DebuggerTraceWriter.TraceEventType>();
            
            foreach(var eventType in eventTypes)
            {
                string message = DebuggerTraceWriter.CreateLogMessage(eventType, CompositionTraceId.Discovery_AssemblyLoadFailed, "Format");
                Assert.True(message.Contains(eventType.ToString()), "Should contain enum string of EventType");
            }            
        }

        [Fact]
        public void CreateLogMessage_ContainsTraceIdAsInt()
        {
            IEnumerable<CompositionTraceId> traceIds = Expectations.GetEnumValues<CompositionTraceId>();
            
            foreach(var traceId in traceIds)
            {
                string message = DebuggerTraceWriter.CreateLogMessage(DebuggerTraceWriter.TraceEventType.Information, traceId, "Format");
                Assert.True(message.Contains(((int)traceId).ToString()), "Should contain int version of TraceId");
            }            
        }

        [Fact]
        public void CreateLogMessage_FormatNull_ThrowsArugmentNull()
        {
            ExceptionAssert.ThrowsArgumentNull("format", () =>
                DebuggerTraceWriter.CreateLogMessage(DebuggerTraceWriter.TraceEventType.Information, CompositionTraceId.Discovery_AssemblyLoadFailed, null));
        }

        [Fact]
        public void CreateLogMessage_ArgumentsNull_ShouldCreateValidString()
        {
            string message = DebuggerTraceWriter.CreateLogMessage(DebuggerTraceWriter.TraceEventType.Information, CompositionTraceId.Discovery_AssemblyLoadFailed, "Format", null);

            Assert.False(string.IsNullOrEmpty(message));
        }

        [Fact]
        public void CreateLogMessage_ArgumentsPassed_ShouldCreateValidString()
        {
            string message = DebuggerTraceWriter.CreateLogMessage(DebuggerTraceWriter.TraceEventType.Information, CompositionTraceId.Discovery_AssemblyLoadFailed, "{0}", 9999);

            Assert.True(message.Contains("9999"));
        }
#endif

    }
}
