// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if USE_MDT_EVENTSOURCE
using Microsoft.Diagnostics.Tracing;
#else
using System.Diagnostics.Tracing;
#endif
using Xunit;
#if USE_ETW
using Microsoft.Diagnostics.Tracing.Session;
#endif
using System.Diagnostics;

namespace BasicEventSourceTests
{
    internal enum Color { Red, Blue, Green };
    internal enum ColorUInt32 : uint { Red, Blue, Green };
    internal enum ColorByte : byte { Red, Blue, Green };
    internal enum ColorSByte : sbyte { Red, Blue, Green };
    internal enum ColorInt16 : short { Red, Blue, Green };
    internal enum ColorUInt16 : ushort { Red, Blue, Green };
    internal enum ColorInt64 : long { Red, Blue, Green };
    internal enum ColorUInt64 : ulong { Red, Blue, Green };


    public class TestsWrite
    {
#if USE_ETW
        // Specifies whether the process is elevated or not.
        private static readonly Lazy<bool> s_isElevated = new Lazy<bool>(() => AdminHelpers.IsProcessElevated());
        private static bool IsProcessElevated => s_isElevated.Value;
#endif // USE_ETW

        [EventData]
        private struct PartB_UserInfo
        {
            public string UserName { get; set; }
        }

        /// <summary>
        /// Tests the EventSource.Write[T] method (can only use the self-describing mechanism).  
        /// Tests the EventListener code path
        /// </summary>
        [Fact]
        [ActiveIssue("dotnet/corefx #19455", TargetFrameworkMonikers.NetFramework)]
        public void Test_Write_T_EventListener()
        {
            using (var listener = new EventListenerListener())
            {
                Test_Write_T(listener);
            }
        }

        /// <summary>
        /// Tests the EventSource.Write[T] method (can only use the self-describing mechanism).  
        /// Tests the EventListener code path using events instead of virtual callbacks.
        /// </summary>
        [Fact]
        [ActiveIssue("dotnet/corefx #19455", TargetFrameworkMonikers.NetFramework)]
        public void Test_Write_T_EventListener_UseEvents()
        {
            Test_Write_T(new EventListenerListener(true));
        }

#if USE_ETW
        /// <summary>
        /// Tests the EventSource.Write[T] method (can only use the self-describing mechanism).  
        /// Tests the ETW code path
        /// </summary>
        [ConditionalFact(nameof(IsProcessElevated))]
        public void Test_Write_T_ETW()
        {
            using (var listener = new EtwListener())
            {
                Test_Write_T(listener);
            }
        }
#endif //USE_ETW
        /// <summary>
        /// Te
        /// </summary>
        /// <param name="listener"></param>
        private void Test_Write_T(Listener listener)
        {
            TestUtilities.CheckNoEventSourcesRunning("Start");

            using (var logger = new EventSource("EventSourceName"))
            {
                var tests = new List<SubTest>();
                /*************************************************************************/
                tests.Add(new SubTest("Write/Basic/String",
                    delegate ()
                    {
                        logger.Write("Greeting", new { msg = "Hello, world!" });
                    },
                    delegate (Event evt)
                    {
                        Assert.Equal(logger.Name, evt.ProviderName);
                        Assert.Equal("Greeting", evt.EventName);

                        Assert.Equal(evt.PayloadValue(0, "msg"), "Hello, world!");
                    }));
                /*************************************************************************/
                decimal myMoney = 300;
                tests.Add(new SubTest("Write/Basic/decimal",
                    delegate ()
                    {
                        logger.Write("Decimal", new { money = myMoney });
                    },
                    delegate (Event evt)
                    {
                        Assert.Equal(logger.Name, evt.ProviderName);
                        Assert.Equal("Decimal", evt.EventName);

                        var eventMoney = evt.PayloadValue(0, "money");
                        // TOD FIX ME - Fix TraceEvent to return decimal instead of double.
                        //Assert.Equal((decimal)eventMoney, (decimal)300);
                    }));
                /*************************************************************************/
                DateTime now = DateTime.Now;
                tests.Add(new SubTest("Write/Basic/DateTime",
                    delegate ()
                    {
                        logger.Write("DateTime", new { nowTime = now });
                    },
                    delegate (Event evt)
                    {
                        Assert.Equal(logger.Name, evt.ProviderName);
                        Assert.Equal("DateTime", evt.EventName);
                        var eventNow = evt.PayloadValue(0, "nowTime");
                        
                        Assert.Equal(eventNow, now);
                    }));
                /*************************************************************************/
                byte[] byteArray = { 0, 1, 2, 3 };
                tests.Add(new SubTest("Write/Basic/byte[]",
                    delegate ()
                    {
                        logger.Write("Bytes", new { bytes = byteArray });
                    },
                    delegate (Event evt)
                    {
                        Assert.Equal(logger.Name, evt.ProviderName);
                        Assert.Equal("Bytes", evt.EventName);

                        var eventArray = evt.PayloadValue(0, "bytes");
                        Array.Equals(eventArray, byteArray);
                    }));
                /*************************************************************************/
                tests.Add(new SubTest("Write/Basic/PartBOnly",
                    delegate ()
                    {
                        // log just a PartB
                        logger.Write("UserInfo", new EventSourceOptions { Keywords = EventKeywords.None },
                                              new { _1 = new PartB_UserInfo { UserName = "Someone Else" } });
                    },
                    delegate (Event evt)
                    {
                        Assert.Equal(logger.Name, evt.ProviderName);
                        Assert.Equal("UserInfo", evt.EventName);

                        var structValue = evt.PayloadValue(0, "PartB_UserInfo");
                        var structValueAsDictionary = structValue as IDictionary<string, object>;
                        Assert.NotNull(structValueAsDictionary);
                        Assert.Equal(structValueAsDictionary["UserName"], "Someone Else");
                    }));

                /*************************************************************************/
                tests.Add(new SubTest("Write/Basic/PartBAndC",
                    delegate ()
                    {
                        // log a PartB and a PartC
                        logger.Write("Duration", new EventSourceOptions { Keywords = EventKeywords.None },
                                              new { _1 = new PartB_UserInfo { UserName = "Myself" }, msec = 10 });
                    },
                    delegate (Event evt)
                    {
                        Assert.Equal(logger.Name, evt.ProviderName);
                        Assert.Equal("Duration", evt.EventName);

                        var structValue = evt.PayloadValue(0, "PartB_UserInfo");
                        var structValueAsDictionary = structValue as IDictionary<string, object>;
                        Assert.NotNull(structValueAsDictionary);
                        Assert.Equal(structValueAsDictionary["UserName"], "Myself");

                        Assert.Equal(evt.PayloadValue(1, "msec"), 10);
                    }));

                /*************************************************************************/
                /*************************** ENUM TESTING *******************************/
                /*************************************************************************/

                /*************************************************************************/
                GenerateEnumTest<Color>(ref tests, logger, Color.Green);
                GenerateEnumTest<ColorUInt32>(ref tests, logger, ColorUInt32.Green);
                GenerateEnumTest<ColorByte>(ref tests, logger, ColorByte.Green);
                GenerateEnumTest<ColorSByte>(ref tests, logger, ColorSByte.Green);
                GenerateEnumTest<ColorInt16>(ref tests, logger, ColorInt16.Green);
                GenerateEnumTest<ColorUInt16>(ref tests, logger, ColorUInt16.Green);
                GenerateEnumTest<ColorInt64>(ref tests, logger, ColorInt64.Green);
                GenerateEnumTest<ColorUInt64>(ref tests, logger, ColorUInt64.Green);
                /*************************************************************************/
                /*************************** ARRAY TESTING *******************************/
                /*************************************************************************/

                /*************************************************************************/
                
                GenerateArrayTest<Boolean>(ref tests, logger, new Boolean[] { false, true, false });
                GenerateArrayTest<byte>(ref tests, logger, new byte[] { 1, 10, 100 });
                GenerateArrayTest<sbyte>(ref tests, logger, new sbyte[] { 1, 10, 100 });
                GenerateArrayTest<Int16>(ref tests, logger, new Int16[] { 1, 10, 100 });
                GenerateArrayTest<UInt16>(ref tests, logger, new UInt16[] { 1, 10, 100 });
                GenerateArrayTest<Int32>(ref tests, logger, new Int32[] { 1, 10, 100 });
                GenerateArrayTest<UInt32>(ref tests, logger, new UInt32[] { 1, 10, 100 });
                GenerateArrayTest<Int64>(ref tests, logger, new Int64[] { 1, 10, 100 });
                GenerateArrayTest<UInt64>(ref tests, logger, new UInt64[] { 1, 10, 100 });
                GenerateArrayTest<Char>(ref tests, logger, new Char[] { 'a', 'c', 'b' });
                GenerateArrayTest<Double>(ref tests, logger, new Double[] { 1, 10, 100 });
                GenerateArrayTest<Single>(ref tests, logger, new Single[] { 1, 10, 100 });
                GenerateArrayTest<IntPtr>(ref tests, logger, new IntPtr[] { (IntPtr)1, (IntPtr)10, (IntPtr)100 });
                GenerateArrayTest<UIntPtr>(ref tests, logger, new UIntPtr[] { (UIntPtr)1, (UIntPtr)10, (UIntPtr)100 });
                GenerateArrayTest<Guid>(ref tests, logger, new Guid[] { Guid.Empty, new Guid("121a11ee-3bcb-49cc-b425-f4906fb14f72") });

                /*************************************************************************/
                /*********************** DICTIONARY TESTING ******************************/
                /*************************************************************************/

                var dict = new Dictionary<string, string>() { { "elem1", "10" }, { "elem2", "20" } };
                var dictInt = new Dictionary<string, int>() { { "elem1", 10 }, { "elem2", 20 } };

                /*************************************************************************/
                tests.Add(new SubTest("Write/Dict/EventWithStringDict_C",
                    delegate()
                    {
                        // log a dictionary
                        logger.Write("EventWithStringDict_C", new { 
                            myDict = dict, 
                            s = "end" });
                    },
                    delegate(Event evt)
                    {
                        Assert.Equal(logger.Name, evt.ProviderName);
                        Assert.Equal("EventWithStringDict_C", evt.EventName);

                        var keyValues = evt.PayloadValue(0, "myDict");
                        IDictionary<string, object> vDict = GetDictionaryFromKeyValueArray(keyValues);
                        Assert.Equal(vDict["elem1"], "10");
                        Assert.Equal(vDict["elem2"], "20");
                        Assert.Equal(evt.PayloadValue(1, "s"), "end");
                    }));
                /*************************************************************************/
                tests.Add(new SubTest("Write/Dict/EventWithStringDict_BC",
                    delegate()
                    {
                        // log a PartB and a dictionary as a PartC
                        logger.Write("EventWithStringDict_BC", new { 
                            PartB_UserInfo = new { UserName = "Me", LogTime = "Now" }, 
                            PartC_Dict = dict, 
                            s = "end" });
                    },
                    delegate(Event evt)
                    {
                        Assert.Equal(logger.Name, evt.ProviderName);
                        Assert.Equal("EventWithStringDict_BC", evt.EventName);

                        var structValue = evt.PayloadValue(0, "PartB_UserInfo");
                        var structValueAsDictionary = structValue as IDictionary<string, object>;
                        Assert.NotNull(structValueAsDictionary);
                        Assert.Equal(structValueAsDictionary["UserName"], "Me");
                        Assert.Equal(structValueAsDictionary["LogTime"], "Now");

                        var keyValues = evt.PayloadValue(1, "PartC_Dict");
                        var vDict = GetDictionaryFromKeyValueArray(keyValues);
                        Assert.NotNull(dict);
                        Assert.Equal(vDict["elem1"], "10");    // string values.
                        Assert.Equal(vDict["elem2"], "20");

                        Assert.Equal(evt.PayloadValue(2, "s"), "end");
                    }));
                /*************************************************************************/
                tests.Add(new SubTest("Write/Dict/EventWithIntDict_BC",
                    delegate()
                    {
                        // log a Dict<string, int> as a PartC
                        logger.Write("EventWithIntDict_BC", new { 
                            PartB_UserInfo = new { UserName = "Me", LogTime = "Now" }, 
                            PartC_Dict = dictInt,
                            s = "end" });

                    },
                    delegate(Event evt)
                    {
                        Assert.Equal(logger.Name, evt.ProviderName);
                        Assert.Equal("EventWithIntDict_BC", evt.EventName);

                        var structValue = evt.PayloadValue(0, "PartB_UserInfo");
                        var structValueAsDictionary = structValue as IDictionary<string, object>;
                        Assert.NotNull(structValueAsDictionary);
                        Assert.Equal(structValueAsDictionary["UserName"], "Me");
                        Assert.Equal(structValueAsDictionary["LogTime"], "Now");

                        var keyValues = evt.PayloadValue(1, "PartC_Dict");
                        var vDict = GetDictionaryFromKeyValueArray(keyValues);
                        Assert.NotNull(vDict);
                        Assert.Equal(vDict["elem1"], 10);  // Notice they are integers, not strings.
                        Assert.Equal(vDict["elem2"], 20);

                        Assert.Equal(evt.PayloadValue(2, "s"), "end");
                    }));
                /*************************************************************************/
                /**************************** Empty Event TESTING ************************/
                /*************************************************************************/
                tests.Add(new SubTest("Write/Basic/Message",
                delegate ()
                {
                    logger.Write("EmptyEvent");
                },
                delegate (Event evt)
                {
                    Assert.Equal(logger.Name, evt.ProviderName);
                    Assert.Equal("EmptyEvent", evt.EventName);
                }));

                /*************************************************************************/
                /**************************** EventSourceOptions TESTING *****************/
                /*************************************************************************/
                EventSourceOptions options = new EventSourceOptions();
                options.Level = EventLevel.LogAlways;
                options.Keywords = EventKeywords.All;
                options.Opcode = EventOpcode.Info;
                options.Tags = EventTags.None;
                tests.Add(new SubTest("Write/Basic/MessageOptions",
                delegate ()
                {
                    logger.Write("EmptyEvent", options);
                },
                delegate (Event evt)
                {
                    Assert.Equal(logger.Name, evt.ProviderName);
                    Assert.Equal("EmptyEvent", evt.EventName);
                }));

                tests.Add(new SubTest("Write/Basic/WriteOfTWithOptios",
                delegate ()
                {
                    logger.Write("OptionsEvent", options, new { OptionsEvent = "test options!" });
                },
                delegate (Event evt)
                {
                    Assert.Equal(logger.Name, evt.ProviderName);
                    Assert.Equal("OptionsEvent", evt.EventName);
                    Assert.Equal(evt.PayloadValue(0, "OptionsEvent"), "test options!");
                }));

                tests.Add(new SubTest("Write/Basic/WriteOfTWithRefOptios",
                delegate ()
                {
                    var v = new { OptionsEvent = "test ref options!" };
                    logger.Write("RefOptionsEvent", ref options, ref v);
                },
                delegate (Event evt)
                {
                    Assert.Equal(logger.Name, evt.ProviderName);
                    Assert.Equal("RefOptionsEvent", evt.EventName);
                    Assert.Equal(evt.PayloadValue(0, "OptionsEvent"), "test ref options!");
                }));

                tests.Add(new SubTest("Write/Basic/WriteOfTWithNullString",
                delegate ()
                {
                    string nullString = null;
                    logger.Write("NullStringEvent", new { a = (string)null, b = nullString });
                },
                delegate (Event evt)
                {
                    Assert.Equal(logger.Name, evt.ProviderName);
                    Assert.Equal("NullStringEvent", evt.EventName);
                    Assert.Equal(evt.PayloadValue(0, "a"), "");
                    Assert.Equal(evt.PayloadValue(1, "b"), "");
                }));

#if USE_ETW
                // This test only applies to ETW and will fail on EventListeners due to different behavior
                // for strings with embedded NULL characters.
                if (listener is EtwListener)
                {
                    tests.Add(new SubTest("Write/Basic/WriteOfTWithEmbeddedNullString",
                    delegate ()
                    {
                        string nullString = null;
                        logger.Write("EmbeddedNullStringEvent", new { a = "Hello" + '\0' + "World!", b = nullString });
                    },
                    delegate (Event evt)
                    {
                        Assert.Equal(logger.Name, evt.ProviderName);
                        Assert.Equal("EmbeddedNullStringEvent", evt.EventName);
                        Assert.Equal(evt.PayloadValue(0, "a"), "Hello");
                        Assert.Equal(evt.PayloadValue(1, "b"), "");
                    }));
                }
#endif // USE_ETW

                Guid activityId = new Guid("00000000-0000-0000-0000-000000000001");
                Guid relActivityId = new Guid("00000000-0000-0000-0000-000000000002");
                tests.Add(new SubTest("Write/Basic/WriteOfTWithOptios",
                delegate ()
                {
                    var v = new { ActivityMsg = "test activity!" };
                    logger.Write("ActivityEvent", ref options, ref activityId, ref relActivityId, ref v);
                },
                delegate (Event evt)
                {
                    Assert.Equal(logger.Name, evt.ProviderName);
                    Assert.Equal("ActivityEvent", evt.EventName);
                    Assert.Equal(evt.PayloadValue(0, "ActivityMsg"), "test activity!");
                }));


                // If you only wish to run one or several of the tests you can filter them here by 
                // Uncommenting the following line.  
                // tests = tests.FindAll(test => Regex.IsMatch(test.Name, "Write/Basic/EventII"));

                // Here is where we actually run tests.   First test the ETW path 
                EventTestHarness.RunTests(tests, listener, logger);
            }
            TestUtilities.CheckNoEventSourcesRunning("Stop");
        }

        /// <summary>
        /// This is not a user error but it is something unusual.   
        /// You can use the Write API in a EventSource that was did not
        /// Declare SelfDescribingSerialization.  In that case THOSE
        /// events MUST use SelfDescribing serialization.  
        /// </summary>
        [Fact]
        [ActiveIssue("dotnet/corefx #18806", TargetFrameworkMonikers.NetFramework)]
        [ActiveIssue("https://github.com/dotnet/corefx/issues/27106")]
        public void Test_Write_T_In_Manifest_Serialization()
        {
            using (var eventListener = new EventListenerListener())
            {
#if USE_ETW
                EtwListener etwListener = null;
#endif
                try
                {
                    var listenerGenerators = new List<Func<Listener>>();
                    listenerGenerators.Add(() => eventListener);
#if USE_ETW
                    if(IsProcessElevated)
                    {
                        etwListener = new EtwListener();
                        listenerGenerators.Add(() => etwListener);
                    }
#endif // USE_ETW

                    foreach (Func<Listener> listenerGenerator in listenerGenerators)
                    {
                        var events = new List<Event>();
                        using (var listener = listenerGenerator())
                        {
                            Debug.WriteLine("Testing Listener " + listener);
                            // Create an eventSource with manifest based serialization
                            using (var logger = new SdtEventSources.EventSourceTest())
                            {
                                listener.OnEvent = delegate (Event data) { events.Add(data); };
                                listener.EventSourceSynchronousEnable(logger);

                                // Use the Write<T> API.   This is OK
                                logger.Write("MyTestEvent", new { arg1 = 3, arg2 = "hi" });
                            }
                        }
                        Assert.Equal(events.Count, 1);
                        Event _event = events[0];
                        Assert.Equal("MyTestEvent", _event.EventName);
                        Assert.Equal(3, (int)_event.PayloadValue(0, "arg1"));
                        Assert.Equal("hi", (string)_event.PayloadValue(1, "arg2"));
                    }
                }
                finally
                {
#if USE_ETW
                    if(etwListener != null)
                    {
                        etwListener.Dispose();
                    }
#endif // USE_ETW
                }
            }
        }

        private void GenerateEnumTest<T>(ref List<SubTest> tests, EventSource logger, T enumValue)
        {
            string subTestName = enumValue.GetType().ToString();
            tests.Add(new SubTest("Write/Enum/EnumEvent" + subTestName,
            delegate ()
            {
                T c = enumValue;
                // log an array
                logger.Write("EnumEvent" + subTestName, new { b = "start", v = c, s = "end" });
            },
            delegate (Event evt)
            {
                Assert.Equal(logger.Name, evt.ProviderName);
                Assert.Equal("EnumEvent" + subTestName, evt.EventName);
                Assert.Equal(evt.PayloadValue(0, "b"), "start");
                if (evt.IsEtw)
                {
                    var value = evt.PayloadValue(1, "v");
                    Assert.Equal(2, int.Parse(value.ToString()));          // Green has the int value of 2. 
                }
                else
                {
                    Assert.Equal(evt.PayloadValue(1, "v"), enumValue);
                }
                Assert.Equal(evt.PayloadValue(2, "s"), "end");
            }));
        }

        private void GenerateArrayTest<T>(ref List<SubTest> tests, EventSource logger, T[] array)
        {
            string typeName = array.GetType().GetElementType().ToString();
            tests.Add(new SubTest("Write/Array/" + typeName,
                 delegate ()
                 {
                     // log an array
                     logger.Write("SomeEvent" + typeName, new { a = array, s = "end" });
                 },
                 delegate (Event evt)
                 {
                     Assert.Equal(logger.Name, evt.ProviderName);
                     Assert.Equal("SomeEvent" + typeName, evt.EventName);

                     var eventArray = evt.PayloadValue(0, "a");
                     Array.Equals(array, eventArray);

                     Assert.Equal("end", evt.PayloadValue(1, "s"));
                 }));
        }

        /// <summary>
        /// Convert an array of key value pairs (as ETW structs) into a dictionary with those values.  
        /// </summary>
        /// <param name="structValue"></param>
        /// <returns></returns>
        private IDictionary<string, object> GetDictionaryFromKeyValueArray(object structValue)
        {
            var ret = new Dictionary<string, object>();
            var asArray = structValue as object[];
            Assert.NotNull(asArray);

            foreach (var item in asArray)
            {
                var keyValue = item as IDictionary<string, object>;
                Assert.Equal(keyValue.Count, 2);
                ret.Add((string)keyValue["Key"], keyValue["Value"]);
            }
            return ret;
        }
    }
}
