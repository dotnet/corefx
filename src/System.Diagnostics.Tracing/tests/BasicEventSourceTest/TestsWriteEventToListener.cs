// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using System.Threading;
using System.Diagnostics.Tracing;
using SdtEventSources;

namespace BasicEventSourceTests
{
    public class TestsWriteEventToListener
    {
        [Fact]
        public void Test_WriteEvent_ArgsBasicTypes()
        {
            TestUtilities.CheckNoEventSourcesRunning("Start");
            using (var log = new EventSourceTest())
            {
                using (var el = new LoudListener())
                {
                    var sources = EventSource.GetSources();
                    Assert.True(sources.Contains(log));

                    Assert.NotNull(EventSource.GenerateManifest(typeof(SimpleEventSource), string.Empty, EventManifestOptions.OnlyIfNeededForRegistration));
                    Assert.Null(EventSource.GenerateManifest(typeof(EventSourceTest), string.Empty, EventManifestOptions.OnlyIfNeededForRegistration));

                    log.Event0();
                    Assert.Equal(1, LoudListener.LastEvent.EventId);
                    Assert.Equal(0, LoudListener.LastEvent.Payload.Count);

                    #region Validate "int" arguments
                    log.EventI(10);
                    Assert.Equal(2, LoudListener.LastEvent.EventId);
                    Assert.Equal(1, LoudListener.LastEvent.Payload.Count);
                    Assert.Equal(10, (int)LoudListener.LastEvent.Payload[0]);

                    log.EventII(10, 11);
                    Assert.Equal(3, LoudListener.LastEvent.EventId);
                    Assert.Equal(2, LoudListener.LastEvent.Payload.Count);
                    Assert.Equal(10, (int)LoudListener.LastEvent.Payload[0]);
                    Assert.Equal(11, (int)LoudListener.LastEvent.Payload[1]);

                    log.EventIII(10, 11, 12);
                    Assert.Equal(4, LoudListener.LastEvent.EventId);
                    Assert.Equal(3, LoudListener.LastEvent.Payload.Count);
                    Assert.Equal(10, (int)LoudListener.LastEvent.Payload[0]);
                    Assert.Equal(11, (int)LoudListener.LastEvent.Payload[1]);
                    Assert.Equal(12, (int)LoudListener.LastEvent.Payload[2]);
                    #endregion

                    #region Validate "long" arguments
                    log.EventL(10);
                    Assert.Equal(5, LoudListener.LastEvent.EventId);
                    Assert.Equal(1, LoudListener.LastEvent.Payload.Count);
                    Assert.Equal(10, (long)LoudListener.LastEvent.Payload[0]);

                    log.EventLL(10, 11);
                    Assert.Equal(6, LoudListener.LastEvent.EventId);
                    Assert.Equal(2, LoudListener.LastEvent.Payload.Count);
                    Assert.Equal(10, (long)LoudListener.LastEvent.Payload[0]);
                    Assert.Equal(11, (long)LoudListener.LastEvent.Payload[1]);

                    log.EventLLL(10, 11, 12);
                    Assert.Equal(7, LoudListener.LastEvent.EventId);
                    Assert.Equal(3, LoudListener.LastEvent.Payload.Count);
                    Assert.Equal(10, (long)LoudListener.LastEvent.Payload[0]);
                    Assert.Equal(11, (long)LoudListener.LastEvent.Payload[1]);
                    Assert.Equal(12, (long)LoudListener.LastEvent.Payload[2]);

                    #endregion

                    #region Validate "string" arguments
                    log.EventS("10");
                    Assert.Equal(8, LoudListener.LastEvent.EventId);
                    Assert.Equal(1, LoudListener.LastEvent.Payload.Count);
                    Assert.Equal("10", (string)LoudListener.LastEvent.Payload[0]);

                    log.EventSS("10", "11");
                    Assert.Equal(9, LoudListener.LastEvent.EventId);
                    Assert.Equal(2, LoudListener.LastEvent.Payload.Count);
                    Assert.Equal("10", (string)LoudListener.LastEvent.Payload[0]);
                    Assert.Equal("11", (string)LoudListener.LastEvent.Payload[1]);

                    log.EventSSS("10", "11", "12");
                    Assert.Equal(10, LoudListener.LastEvent.EventId);
                    Assert.Equal(3, LoudListener.LastEvent.Payload.Count);
                    Assert.Equal("10", (string)LoudListener.LastEvent.Payload[0]);
                    Assert.Equal("11", (string)LoudListener.LastEvent.Payload[1]);
                    Assert.Equal("12", (string)LoudListener.LastEvent.Payload[2]);
                    #endregion

                    #region Validate byte array arguments
                    byte[] arr = new byte[20];
                    log.EventWithByteArray(arr);
                    Assert.Equal(52, LoudListener.LastEvent.EventId);
                    Assert.Equal(1, LoudListener.LastEvent.Payload.Count);
                    Assert.Equal(arr.Length, ((byte[])LoudListener.LastEvent.Payload[0]).Length);
                    #endregion

                    #region Validate mixed type arguments
                    log.EventSI("10", 11);
                    Assert.Equal(11, LoudListener.LastEvent.EventId);
                    Assert.Equal(2, LoudListener.LastEvent.Payload.Count);
                    Assert.Equal("10", (string)LoudListener.LastEvent.Payload[0]);
                    Assert.Equal(11, (int)LoudListener.LastEvent.Payload[1]);

                    log.EventSL("10", 11);
                    Assert.Equal(12, LoudListener.LastEvent.EventId);
                    Assert.Equal(2, LoudListener.LastEvent.Payload.Count);
                    Assert.Equal("10", (string)LoudListener.LastEvent.Payload[0]);
                    Assert.Equal(11, (long)LoudListener.LastEvent.Payload[1]);

                    log.EventSII("10", 11, 12);
                    Assert.Equal(13, LoudListener.LastEvent.EventId);
                    Assert.Equal(3, LoudListener.LastEvent.Payload.Count);
                    Assert.Equal("10", (string)LoudListener.LastEvent.Payload[0]);
                    Assert.Equal(11, (int)LoudListener.LastEvent.Payload[1]);
                    Assert.Equal(12, (int)LoudListener.LastEvent.Payload[2]);
                    #endregion

                    #region Validate enums/flags
                    log.EventEnum(MyColor.Blue);
                    Assert.Equal(19, LoudListener.LastEvent.EventId);
                    Assert.Equal(1, LoudListener.LastEvent.Payload.Count);
                    Assert.Equal(MyColor.Blue, (MyColor)LoudListener.LastEvent.Payload[0]);

                    log.EventEnum1(MyColor.Green);
                    Assert.Equal(20, LoudListener.LastEvent.EventId);
                    Assert.Equal(1, LoudListener.LastEvent.Payload.Count);
                    Assert.Equal(MyColor.Green, (MyColor)LoudListener.LastEvent.Payload[0]);

                    log.EventFlags(MyFlags.Flag1);
                    Assert.Equal(21, LoudListener.LastEvent.EventId);
                    Assert.Equal(1, LoudListener.LastEvent.Payload.Count);
                    Assert.Equal(MyFlags.Flag1, (MyFlags)LoudListener.LastEvent.Payload[0]);

                    log.EventFlags1(MyFlags.Flag1);
                    Assert.Equal(22, LoudListener.LastEvent.EventId);
                    Assert.Equal(1, LoudListener.LastEvent.Payload.Count);
                    Assert.Equal(MyFlags.Flag1, (MyFlags)LoudListener.LastEvent.Payload[0]);
                    #endregion

#if USE_ETW // TODO: Enable when TraceEvent is available on CoreCLR. GitHub issue #4864.
                    #region Validate DateTime
                    DateTime now = DateTime.Now;
                    log.EventDateTime(now);
                    Assert.Equal(24, LoudListener.LastEvent.EventId);
                    Assert.Equal(1, LoudListener.LastEvent.Payload.Count);
                    Assert.Equal((DateTime)LoudListener.LastEvent.Payload[0], now);
                    #endregion
#endif // USE_ETW
                }
            }
            TestUtilities.CheckNoEventSourcesRunning("Stop");
        }

        [Fact]
        public void Test_WriteEvent_ArgsCornerCases()
        {
            TestUtilities.CheckNoEventSourcesRunning("Start");
            using (var log = new EventSourceTest())
            {
                using (var el = new LoudListener())
                {
                    // coverage for EventSource.SendCommand()
                    var options = new Dictionary<string, string>() { { "arg", "val" } };
                    EventSource.SendCommand(log, EventCommand.SendManifest, options);

                    Guid guid = Guid.NewGuid();
#if USE_ETW // TODO: Enable when TraceEvent is available on CoreCLR. GitHub issue #4864.
                    log.EventWithManyTypeArgs("Hello", 0, 0, 0, 'a', 0, 0, 0, 0, (float)10.0, (double)11.0, guid);
                    Assert.Equal(25, LoudListener.LastEvent.EventId);
                    Assert.Equal(12, LoudListener.LastEvent.Payload.Count);
                    Assert.Equal("Hello", (string)LoudListener.LastEvent.Payload[0]);
                    Assert.Equal(0, (long)LoudListener.LastEvent.Payload[1]);
                    Assert.Equal((uint)0, (uint)LoudListener.LastEvent.Payload[2]);
                    Assert.Equal((ulong)0, (ulong)LoudListener.LastEvent.Payload[3]);
                    Assert.Equal('a', (char)LoudListener.LastEvent.Payload[4]);
                    Assert.Equal((byte)0, (byte)LoudListener.LastEvent.Payload[5]);
                    Assert.Equal((sbyte)0, (sbyte)LoudListener.LastEvent.Payload[6]);
                    Assert.Equal((short)0, (short)LoudListener.LastEvent.Payload[7]);
                    Assert.Equal((ushort)0, (ushort)LoudListener.LastEvent.Payload[8]);
                    Assert.Equal((float)10.0, (float)LoudListener.LastEvent.Payload[9]);
                    Assert.Equal((double)11.0, (double)LoudListener.LastEvent.Payload[10]);
                    Assert.Equal(guid, (Guid)LoudListener.LastEvent.Payload[11]);
#endif // USE_ETW
                    log.EventWith7Strings("s0", "s1", "s2", "s3", "s4", "s5", "s6");
                    Assert.Equal(26, LoudListener.LastEvent.EventId);
                    Assert.Equal(7, LoudListener.LastEvent.Payload.Count);
                    Assert.Equal("s0", (string)LoudListener.LastEvent.Payload[0]);
                    Assert.Equal("s6", (string)LoudListener.LastEvent.Payload[6]);

                    log.EventWith9Strings("s0", "s1", "s2", "s3", "s4", "s5", "s6", "s7", "s8");
                    Assert.Equal(27, LoudListener.LastEvent.EventId);
                    Assert.Equal(9, LoudListener.LastEvent.Payload.Count);
                    Assert.Equal("s0", (string)LoudListener.LastEvent.Payload[0]);
                    Assert.Equal("s8", (string)LoudListener.LastEvent.Payload[8]);
#if USE_ETW // TODO: Enable when TraceEvent is available on CoreCLR. GitHub issue #4864.
                    log.EventWithWeirdArgs(IntPtr.Zero, true, MyLongEnum.LongVal1 /*, 9999999999999999999999999999m*/);
                    Assert.Equal(30, LoudListener.LastEvent.EventId);
                    Assert.Equal(3 /*4*/, LoudListener.LastEvent.Payload.Count);
                    Assert.Equal(IntPtr.Zero, (IntPtr)LoudListener.LastEvent.Payload[0]);
                    Assert.Equal(true, (bool)LoudListener.LastEvent.Payload[1]);
                    Assert.Equal(MyLongEnum.LongVal1, (MyLongEnum)LoudListener.LastEvent.Payload[2]);
                    // Assert.Equal(9999999999999999999999999999m, (decimal)LoudListener.LastEvent.Payload[3]);
#endif // USE_ETW
                }
            }
            TestUtilities.CheckNoEventSourcesRunning("Stop");
        }

        [Fact]
        public void Test_WriteEvent_InvalidCalls()
        {
            TestUtilities.CheckNoEventSourcesRunning("Start");
            using (var log = new InvalidCallsToWriteEventEventSource())
            {
                using (var el = new LoudListener())
                {
                    log.WriteTooManyArgs("Hello");
                    Assert.Equal(2, LoudListener.LastEvent.EventId);
                    Assert.Equal(1, LoudListener.LastEvent.Payload.Count);           // Faked count (compat)
                    Assert.Equal("Hello", LoudListener.LastEvent.Payload[0]);

                    log.WriteTooFewArgs(10, 100);
                    Assert.Equal(1, LoudListener.LastEvent.EventId);
                    Assert.Equal(1, LoudListener.LastEvent.Payload.Count);           // Real # of args passed to WriteEvent
                    Assert.Equal(10, LoudListener.LastEvent.Payload[0]);
                }
            }
            TestUtilities.CheckNoEventSourcesRunning("Stop");
        }

        [Fact]
        public void Test_WriteEvent_ToChannel_Coverage()
        {
            TestUtilities.CheckNoEventSourcesRunning("Start");

            using (var el = new LoudListener())
            using (var log = new SimpleEventSource())
            {
                el.EnableEvents(log, EventLevel.Verbose);
                log.WriteIntToAdmin(10);
            }
            TestUtilities.CheckNoEventSourcesRunning("Stop");
        }

#if USE_ETW // TODO: Enable when TraceEvent is available on CoreCLR. GitHub issue #4864.
        [Fact]
        public void Test_WriteEvent_TransferEvents()
        {
            TestUtilities.CheckNoEventSourcesRunning("Start");
            using (var log = new EventSourceTest())
            {
                using (var el = new LoudListener())
                {
                    Guid actid = Guid.NewGuid();
                    log.LogTaskScheduled(actid, "Hello from a test");
                    Assert.Equal(17, LoudListener.LastEvent.EventId);
                    Assert.Equal(actid, LoudListener.LastEvent.RelatedActivityId);
                    Assert.Equal(1, LoudListener.LastEvent.Payload.Count);
                    Assert.Equal("Hello from a test", (string)LoudListener.LastEvent.Payload[0]);

                    actid = Guid.NewGuid();
                    log.LogTaskScheduledBad(actid, "Hello again");
                    Assert.Equal(23, LoudListener.LastEvent.EventId);
                    Assert.Equal(actid, LoudListener.LastEvent.RelatedActivityId);
                    Assert.Equal(1, LoudListener.LastEvent.Payload.Count);
                    Assert.Equal("Hello again", (string)LoudListener.LastEvent.Payload[0]);

                    actid = Guid.NewGuid();
                    Guid guid = Guid.NewGuid();
                    log.EventWithXferManyTypeArgs(actid, 0, 0, 0, 'a', 0, 0, 0, 0, (float)10.0, (double)11.0, guid);
                    Assert.Equal(29, LoudListener.LastEvent.EventId);
                    Assert.Equal(actid, LoudListener.LastEvent.RelatedActivityId);
                    Assert.Equal(11, LoudListener.LastEvent.Payload.Count);
                    Assert.Equal(0, (long)LoudListener.LastEvent.Payload[0]);
                    Assert.Equal((uint)0, (uint)LoudListener.LastEvent.Payload[1]);
                    Assert.Equal((ulong)0, (ulong)LoudListener.LastEvent.Payload[2]);
                    Assert.Equal('a', (char)LoudListener.LastEvent.Payload[3]);
                    Assert.Equal((byte)0, (byte)LoudListener.LastEvent.Payload[4]);
                    Assert.Equal((sbyte)0, (sbyte)LoudListener.LastEvent.Payload[5]);
                    Assert.Equal((short)0, (short)LoudListener.LastEvent.Payload[6]);
                    Assert.Equal((ushort)0, (ushort)LoudListener.LastEvent.Payload[7]);
                    Assert.Equal((float)10.0, (float)LoudListener.LastEvent.Payload[8]);
                    Assert.Equal((double)11.0, (double)LoudListener.LastEvent.Payload[9]);
                    Assert.Equal(guid, (Guid)LoudListener.LastEvent.Payload[10]);

                    actid = Guid.NewGuid();
                    log.EventWithXferWeirdArgs(actid, IntPtr.Zero, true, MyLongEnum.LongVal1 /*, 9999999999999999999999999999m*/);
                    Assert.Equal(31, LoudListener.LastEvent.EventId);
                    Assert.Equal(actid, LoudListener.LastEvent.RelatedActivityId);
                    Assert.Equal(3 /*4*/, LoudListener.LastEvent.Payload.Count);
                    Assert.Equal(IntPtr.Zero, (IntPtr)LoudListener.LastEvent.Payload[0]);
                    Assert.Equal(true, (bool)LoudListener.LastEvent.Payload[1]);
                    Assert.Equal(MyLongEnum.LongVal1, (MyLongEnum)LoudListener.LastEvent.Payload[2]);
                    // Assert.Equal(9999999999999999999999999999m, (decimal)LoudListener.LastEvent.Payload[3]);

                    actid = Guid.NewGuid();
                    Assert.Throws<EventSourceException>(() => log.LogTransferNoOpcode(actid));
                }
            }
            TestUtilities.CheckNoEventSourcesRunning("Stop");
        }

#endif // USE_ETW

        [Fact]
        public void Test_WriteEvent_ZeroKwds()
        {
            TestUtilities.CheckNoEventSourcesRunning("Start");

            using (var log = new EventSourceTest())
            {
                using (var el = new LoudListener())
                {
                    // match any kwds == 0
                    el.EnableEvents(log, 0, 0);

                    // Fire an event without a kwd: EventWithEscapingMessage

                    // 1. Validate that the event fires when ETW event method called unconditionally
                    log.EventWithEscapingMessage("Hello world!", 10);
                    Assert.NotNull(LoudListener.LastEvent);
                    Assert.Equal(2, LoudListener.LastEvent.Payload.Count);
                    Assert.Equal("Hello world!", (string)LoudListener.LastEvent.Payload[0]);
                    Assert.Equal(10, (int)LoudListener.LastEvent.Payload[1]);

                    // reset LastEvent
                    LoudListener.LastEvent = null;

                    // 2. Validate that the event fires when ETW event method call is guarded by IsEnabled
                    if (log.IsEnabled(EventLevel.Informational, 0))
                        log.EventWithEscapingMessage("Goodbye skies!", 100);
                    Assert.NotNull(LoudListener.LastEvent);
                    Assert.Equal(2, LoudListener.LastEvent.Payload.Count);
                    Assert.Equal("Goodbye skies!", (string)LoudListener.LastEvent.Payload[0]);
                    Assert.Equal(100, (int)LoudListener.LastEvent.Payload[1]);
                }
            }
            TestUtilities.CheckNoEventSourcesRunning("Stop");
        }

#if false // TODO: EventListener events are not enabled yet. GitHub issues #4865.
        [Fact]
        public void Test_EventSourceCreatedEvents_BeforeListener()
        {
            TestUtilities.CheckNoEventSourcesRunning("Start");

            EventSource log = null;
            EventSource log2 = null;
            EventListenerListener el = null;

            try
            {
                string esName = "EventSourceName_HopefullyUnique";
                string esName2 = "EventSourceName_HopefullyUnique2";
                bool esNameHit = false;
                bool esName2Hit = false;

                log = new EventSource(esName);
                log2 = new EventSource(esName2);

                el = new EventListenerListener();

                List<EventSource> eventSourceNotificationsReceived = new List<EventSource>();
                el.EventSourceCreated += (s, a) =>
                {
                    if (a.EventSource.Name.Equals(esName))
                    {
                        esNameHit = true;
                    }

                    if (a.EventSource.Name.Equals(esName2))
                    {
                        esName2Hit = true;
                    }
                };

                Thread.Sleep(1000);

                Assert.Equal(true, esNameHit);
                Assert.Equal(true, esName2Hit);
            }
            finally
            {
                if (log != null)
                {
                    log.Dispose();
                }

                if (log2 != null)
                {
                    log2.Dispose();
                }

                if (el != null)
                {
                    el.Dispose();
                }
            }

            TestUtilities.CheckNoEventSourcesRunning("Stop");
        }

        [Fact]
        public void Test_EventSourceCreatedEvents_AfterListener()
        {
            TestUtilities.CheckNoEventSourcesRunning("Start");

            EventSource log = null;
            EventSource log2 = null;
            EventListenerListener el = null;

            try
            {
                el = new EventListenerListener();

                string esName = "EventSourceName_HopefullyUnique";
                string esName2 = "EventSourceName_HopefullyUnique2";
                bool esNameHit = false;
                bool esName2Hit = false;

                List<EventSource> eventSourceNotificationsReceived = new List<EventSource>();
                el.EventSourceCreated += (s, a) =>
                {
                    if(a.EventSource.Name.Equals(esName))
                    {
                        esNameHit = true;
                    }

                    if (a.EventSource.Name.Equals(esName2))
                    {
                        esName2Hit = true;
                    }
                };

                log = new EventSource(esName);
                log2 = new EventSource(esName2);

                Thread.Sleep(1000);

                Assert.Equal(true, esNameHit);
                Assert.Equal(true, esName2Hit);
            }
            finally
            {
                if (log != null)
                {
                    log.Dispose();
                }

                if (log2 != null)
                {
                    log2.Dispose();
                }

                if (el != null)
                {
                    el.Dispose();
                }
            }

            TestUtilities.CheckNoEventSourcesRunning("Stop");
        }
#endif // false
    }
}
