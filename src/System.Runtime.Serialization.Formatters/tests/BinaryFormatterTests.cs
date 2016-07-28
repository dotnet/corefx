// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Xunit;

namespace System.Runtime.Serialization.Formatters.Tests
{
    public class BinaryFormatterTests : RemoteExecutorTestBase
    {
        public static IEnumerable<object> SerializableObjects()
        {
            // Primitive types
            yield return byte.MinValue;
            yield return byte.MaxValue;
            yield return sbyte.MinValue;
            yield return sbyte.MaxValue;
            yield return short.MinValue;
            yield return short.MaxValue;
            yield return int.MinValue;
            yield return int.MaxValue;
            yield return uint.MinValue;
            yield return uint.MaxValue;
            yield return long.MinValue;
            yield return long.MaxValue;
            yield return ulong.MinValue;
            yield return ulong.MaxValue;
            yield return char.MinValue;
            yield return char.MaxValue;
            yield return float.MinValue;
            yield return float.MaxValue;
            yield return double.MinValue;
            yield return double.MaxValue;
            yield return true;
            yield return false;
            yield return "";
            yield return "c";
            yield return "\u4F60\u597D";
            yield return "some\0data\0with\0null\0chars";
            yield return "<>&\"\'";
            yield return " < ";
            yield return "minchar" + char.MinValue + "minchar";

            // Enum values
            yield return DayOfWeek.Monday;
            yield return DateTimeKind.Local;

            // Nullables
            yield return (int?)1;
            yield return (StructWithIntField?)new StructWithIntField() { X = 42 };

            // Other core serializable types
            yield return TimeSpan.FromDays(7);
            yield return new Version(1, 2, 3, 4);
            yield return Tuple.Create(1, "2", Tuple.Create(3.4));
            yield return new Guid("0CACAA4D-C6BD-420A-B660-2F557337CA89");
            yield return new AttributeUsageAttribute(AttributeTargets.Class);
            yield return new List<int>();
            yield return new List<int>() { 1, 2, 3, 4, 5 };

            // Arrays of primitive types
            yield return Enumerable.Range(0, 256).Select(i => (byte)i).ToArray();
            yield return new int[] { };
            yield return new int[] { 1 };
            yield return new int[] { 1, 2, 3, 4, 5 };
            yield return new char[] { 'a', 'b', 'c', 'd', 'e' };
            yield return new string[] { };
            yield return new string[] { "hello", "world" };
            yield return new short[] { short.MaxValue };
            yield return new long[] { long.MaxValue };
            yield return new ushort[] { ushort.MaxValue };
            yield return new uint[] { uint.MaxValue };
            yield return new ulong[] { ulong.MaxValue };
            yield return new bool[] { true, false };
            yield return new double[] { 1.2 };
            yield return new float[] { 1.2f, 3.4f };

            // Arrays of other types
            yield return new object[] { };
            yield return new Guid[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            yield return new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday };
            yield return new Point[] { new Point(1, 2), new Point(3, 4) };
            yield return new ObjectWithArrays
            {
                IntArray = new int[0],
                StringArray = new string[] { "hello", "world" },
                ByteArray = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 },
                JaggedArray = new int[][] { new int[] { 1, 2, 3 }, new int[] { 4, 5, 6, 7 } },
                MultiDimensionalArray = new int[,] { { 1, 2 }, { 3, 4 }, { 5, 6 } },
                TreeArray = new Tree<int>[] { new Tree<int>(1, new Tree<int>(2, null, null), new Tree<int>(3, null, null)) }
            };
            yield return new object[] { new int[,] { { 1, 2, 3, 4, 5 }, { 6, 7, 8, 9, 10 }, { 11, 12, 13, 14, 15 } } };
            yield return new object[] { new int[,,] { { { 1, 2, 3, 4, 5 }, { 6, 7, 8, 9, 10 }, { 11, 12, 13, 14, 15 } } } };
            yield return new object[] { new int[,,,,] { { { { { 1 } } } } } };
            yield return new ArraySegment<int>(new int[] { 1, 2, 3, 4, 5 }, 1, 2);
            yield return Enumerable.Range(0, 10000).Select(i => (object)i).ToArray();
            yield return new object[200]; // fewer than 256 nulls
            yield return new object[300]; // more than 256 nulls

            // Non-vector arrays
            yield return Array.CreateInstance(typeof(uint), new[] { 5 }, new[] { 1 });
            yield return Array.CreateInstance(typeof(int), new[] { 0, 0, 0 }, new[] { 0, 0, 0 });
            var arr = Array.CreateInstance(typeof(string), new[] { 1, 2 }, new[] { 3, 4 });
            arr.SetValue("hello", new[] { 3, 5 });
            yield return arr;

            // Custom object
            var sealedObjectWithIntStringFields = new SealedObjectWithIntStringFields();
            yield return sealedObjectWithIntStringFields;
            yield return new SealedObjectWithIntStringFields() { Member1 = 42, Member2 = null, Member3 = "84" };

            // Custom object with fields pointing to the same object
            yield return new ObjectWithIntStringUShortUIntULongAndCustomObjectFields
            {
                Member1 = 10,
                Member2 = "hello",
                _member3 = "hello",
                Member4 = sealedObjectWithIntStringFields,
                Member4shared = sealedObjectWithIntStringFields,
                Member5 = new SealedObjectWithIntStringFields(),
                Member6 = "Hello World",
                str1 = "hello < world",
                str2 = "<",
                str3 = "< world",
                str4 = "hello < world",
                u16 = ushort.MaxValue,
                u32 = uint.MaxValue,
                u64 = ulong.MaxValue,
            };

            // Simple type without a default ctor
            var point = new Point(1, 2);
            yield return point;

            // Graph without cycles
            yield return new Tree<int>(42, null, null);
            yield return new Tree<int>(1, new Tree<int>(2, new Tree<int>(3, null, null), null), null);
            yield return new Tree<Colors>(Colors.Red, null, new Tree<Colors>(Colors.Blue, null, new Tree<Colors>(Colors.Green, null, null)));
            yield return new Tree<int>(1, new Tree<int>(2, new Tree<int>(3, null, null), new Tree<int>(4, null, null)), new Tree<int>(5, null, null));

            // Graph with cycles
            Graph<int> a = new Graph<int> { Value = 1 };
            yield return a;
            Graph<int> b = new Graph<int> { Value = 2, Links = new[] { a } };
            yield return b;
            Graph<int> c = new Graph<int> { Value = 3, Links = new[] { a, b } };
            yield return c;
            Graph<int> d = new Graph<int> { Value = 3, Links = new[] { a, b, c } };
            yield return d;
            a.Links = new[] { b, c, d }; // complete the cycle
            yield return a;

            // Structs
            yield return new EmptyStruct();
            yield return new StructWithIntField { X = 42 };
            yield return new StructWithStringFields { String1 = "hello", String2 = "world" };
            yield return new StructContainingOtherStructs { Nested1 = new StructWithStringFields { String1 = "a", String2 = "b" }, Nested2 = new StructWithStringFields { String1 = "3", String2 = "4" } };
            yield return new StructContainingArraysOfOtherStructs();
            yield return new StructContainingArraysOfOtherStructs { Nested = new StructContainingOtherStructs[0] };
            var s = new StructContainingArraysOfOtherStructs
            {
                Nested = new[]
                {
                    new StructContainingOtherStructs { Nested1 = new StructWithStringFields { String1 = "a", String2 = "b" }, Nested2 = new StructWithStringFields { String1 = "3", String2 = "4" } },
                    new StructContainingOtherStructs { Nested1 = new StructWithStringFields { String1 = "e", String2 = "f" }, Nested2 = new StructWithStringFields { String1 = "7", String2 = "8" } },
                }
            };
            yield return s;
            yield return new object[] { s, new StructContainingArraysOfOtherStructs?(s) };

            // ISerializable
            yield return new BasicISerializableObject(1, "2");

            // Various other special cases
            yield return new TypeWithoutNamespace();
        }

        public static IEnumerable<object> NonSerializableObjects()
        {
            yield return new NonSerializableStruct();
            yield return new NonSerializableClass();
            yield return new SerializableClassWithBadField();
            yield return new object[] { 1, 2, 3, new NonSerializableClass() };

            // TODO: Move these to the serializable category when enabled in the runtime
            var e = new OperationCanceledException();
            yield return e;
            try { throw e; } catch { } yield return e;
            yield return new Dictionary<int, string>() { { 1, "test" }, { 2, "another test" } };
            yield return IntPtr.Zero;
            yield return UIntPtr.Zero;
        }

        public static IEnumerable<object[]> ValidateBasicObjectsRoundtrip_MemberData()
        {
            foreach (object obj in SerializableObjects())
            {
                foreach (FormatterAssemblyStyle assemblyFormat in new[] { FormatterAssemblyStyle.Full, FormatterAssemblyStyle.Simple })
                {
                    foreach (TypeFilterLevel filterLevel in new[] { TypeFilterLevel.Full, TypeFilterLevel.Low })
                    {
                        foreach (FormatterTypeStyle typeFormat in new[] { FormatterTypeStyle.TypesAlways, FormatterTypeStyle.TypesWhenNeeded, FormatterTypeStyle.XsdString })
                        {
                            foreach (bool unsafeDeserialize in new[] { true, false })
                            {
                                yield return new object[] { obj, assemblyFormat, filterLevel, typeFormat, unsafeDeserialize };
                            }
                        }
                    }
                }
            }
        }

        [Theory]
        [MemberData(nameof(ValidateBasicObjectsRoundtrip_MemberData))]
        public void ValidateBasicObjectsRoundtrip(object obj, FormatterAssemblyStyle assemblyFormat, TypeFilterLevel filterLevel, FormatterTypeStyle typeFormat, bool unsafeDeserialize)
        {
            object result = FormatterClone(obj, null, assemblyFormat, filterLevel, typeFormat, unsafeDeserialize);
            if (!ReferenceEquals(obj, string.Empty)) // "" is interned and will roundtrip as the same object
            {
                Assert.NotSame(obj, result);
            }
            Assert.Equal(obj, result);
        }

        public static IEnumerable<object[]> RoundtripWithHeaders_MemberData()
        {
            foreach (object obj in SerializableObjects())
            {
                // Fails with strings as the root of the graph, both in core and on desktop:
                // "The object with ID 1 was referenced in a fixup but does not exist"
                if (obj is string) continue;

                yield return new[] { obj };
            }
        }

        [Theory]
        [MemberData(nameof(RoundtripWithHeaders_MemberData))]
        public void RoundtripWithHeaders(object obj)
        {
            Assert.Equal(obj, FormatterClone(obj, headers: new Header[0]));
            Assert.Equal(obj, FormatterClone(obj, headers: new[] { new Header("SomeHeader", "some value") }));
            Assert.Equal(obj, FormatterClone(obj, headers: new[] { new Header("SomeHeader", "some value", true, "some namespace") }));
            Assert.Equal(obj, FormatterClone(obj, headers: new[] { new Header("SomeHeader", 42), new Header("SomeOtherHeader", obj) })); ;
        }

        [Fact]
        public void RoundtripManyObjectsInOneStream()
        {
            object[] objects = SerializableObjects().ToArray();
            var s = new MemoryStream();
            var f = new BinaryFormatter();

            foreach (object obj in objects)
            {
                f.Serialize(s, obj);
            }
            s.Position = 0;
            foreach (object obj in objects)
            {
                object result = f.Deserialize(s);
                Assert.Equal(obj, result);
            }
        }

        [Fact]
        public void SameObjectRepeatedInArray()
        {
            object o = new object();
            object[] arr = new[] { o, o, o, o, o };
            object[] result = FormatterClone(arr);

            Assert.Equal(arr.Length, result.Length);
            Assert.NotSame(arr, result);
            Assert.NotSame(arr[0], result[0]);
            for (int i = 1; i < result.Length; i++)
            {
                Assert.Same(result[0], result[i]);
            }
        }

        public static IEnumerable<object[]> ValidateNonSerializableTypes_MemberData()
        {
            foreach (object obj in NonSerializableObjects())
            {
                foreach (FormatterAssemblyStyle assemblyFormat in new[] { FormatterAssemblyStyle.Full, FormatterAssemblyStyle.Simple })
                {
                    foreach (TypeFilterLevel filterLevel in new[] { TypeFilterLevel.Full, TypeFilterLevel.Low })
                    {
                        foreach (FormatterTypeStyle typeFormat in new[] { FormatterTypeStyle.TypesAlways, FormatterTypeStyle.TypesWhenNeeded, FormatterTypeStyle.XsdString })
                        {
                            yield return new object[] { obj, assemblyFormat, filterLevel, typeFormat };
                        }
                    }
                }
            }
        }

        [Theory]
        [MemberData(nameof(ValidateNonSerializableTypes_MemberData))]
        public void ValidateNonSerializableTypes(object obj, FormatterAssemblyStyle assemblyFormat, TypeFilterLevel filterLevel, FormatterTypeStyle typeFormat)
        {
            var f = new BinaryFormatter()
            {
                AssemblyFormat = assemblyFormat,
                FilterLevel = filterLevel,
                TypeFormat = typeFormat
            };
            using (var s = new MemoryStream())
            {
                Assert.Throws<SerializationException>(() => f.Serialize(s, obj));
            }
        }

        [Fact]
        public void SerializeNonSerializableTypeWithSurrogate()
        {
            var p = new NonSerializablePair<int, string>() { Value1 = 1, Value2 = "2" };
            Assert.False(p.GetType().GetTypeInfo().IsSerializable);
            Assert.Throws<SerializationException>(() => FormatterClone(p));

            NonSerializablePair<int, string> result = FormatterClone(p, new NonSerializablePairSurrogate());
            Assert.NotSame(p, result);
            Assert.Equal(p.Value1, result.Value1);
            Assert.Equal(p.Value2, result.Value2);
        }

        [Fact]
        public void SerializationEvents_FireAsExpected()
        {
            var f = new BinaryFormatter();

            var obj = new IncrementCountsDuringRoundtrip(null);

            Assert.Equal(0, obj.IncrementedDuringOnSerializingMethod);
            Assert.Equal(0, obj.IncrementedDuringOnSerializedMethod);
            Assert.Equal(0, obj.IncrementedDuringOnDeserializingMethod);
            Assert.Equal(0, obj.IncrementedDuringOnDeserializedMethod);

            using (var s = new MemoryStream())
            {
                f.Serialize(s, obj);
                s.Position = 0;

                Assert.Equal(1, obj.IncrementedDuringOnSerializingMethod);
                Assert.Equal(1, obj.IncrementedDuringOnSerializedMethod);
                Assert.Equal(0, obj.IncrementedDuringOnDeserializingMethod);
                Assert.Equal(0, obj.IncrementedDuringOnDeserializedMethod);

                var result = (IncrementCountsDuringRoundtrip)f.Deserialize(s);

                Assert.Equal(1, obj.IncrementedDuringOnSerializingMethod);
                Assert.Equal(1, obj.IncrementedDuringOnSerializedMethod);
                Assert.Equal(0, obj.IncrementedDuringOnDeserializingMethod);
                Assert.Equal(0, obj.IncrementedDuringOnDeserializedMethod);

                Assert.Equal(1, result.IncrementedDuringOnSerializingMethod);
                Assert.Equal(0, result.IncrementedDuringOnSerializedMethod);
                Assert.Equal(1, result.IncrementedDuringOnDeserializingMethod);
                Assert.Equal(1, result.IncrementedDuringOnDeserializedMethod);
            }
        }

        [Fact]
        public void SerializationEvents_DerivedTypeWithEvents_FireAsExpected()
        {
            var f = new BinaryFormatter();

            var obj = new DerivedIncrementCountsDuringRoundtrip(null);

            Assert.Equal(0, obj.IncrementedDuringOnSerializingMethod);
            Assert.Equal(0, obj.IncrementedDuringOnSerializedMethod);
            Assert.Equal(0, obj.IncrementedDuringOnDeserializingMethod);
            Assert.Equal(0, obj.IncrementedDuringOnDeserializedMethod);
            Assert.Equal(0, obj.DerivedIncrementedDuringOnSerializingMethod);
            Assert.Equal(0, obj.DerivedIncrementedDuringOnSerializedMethod);
            Assert.Equal(0, obj.DerivedIncrementedDuringOnDeserializingMethod);
            Assert.Equal(0, obj.DerivedIncrementedDuringOnDeserializedMethod);

            using (var s = new MemoryStream())
            {
                f.Serialize(s, obj);
                s.Position = 0;

                Assert.Equal(1, obj.IncrementedDuringOnSerializingMethod);
                Assert.Equal(1, obj.IncrementedDuringOnSerializedMethod);
                Assert.Equal(0, obj.IncrementedDuringOnDeserializingMethod);
                Assert.Equal(0, obj.IncrementedDuringOnDeserializedMethod);
                Assert.Equal(1, obj.DerivedIncrementedDuringOnSerializingMethod);
                Assert.Equal(1, obj.DerivedIncrementedDuringOnSerializedMethod);
                Assert.Equal(0, obj.DerivedIncrementedDuringOnDeserializingMethod);
                Assert.Equal(0, obj.DerivedIncrementedDuringOnDeserializedMethod);

                var result = (DerivedIncrementCountsDuringRoundtrip)f.Deserialize(s);

                Assert.Equal(1, obj.IncrementedDuringOnSerializingMethod);
                Assert.Equal(1, obj.IncrementedDuringOnSerializedMethod);
                Assert.Equal(0, obj.IncrementedDuringOnDeserializingMethod);
                Assert.Equal(0, obj.IncrementedDuringOnDeserializedMethod);
                Assert.Equal(1, obj.DerivedIncrementedDuringOnSerializingMethod);
                Assert.Equal(1, obj.DerivedIncrementedDuringOnSerializedMethod);
                Assert.Equal(0, obj.DerivedIncrementedDuringOnDeserializingMethod);
                Assert.Equal(0, obj.DerivedIncrementedDuringOnDeserializedMethod);

                Assert.Equal(1, result.IncrementedDuringOnSerializingMethod);
                Assert.Equal(0, result.IncrementedDuringOnSerializedMethod);
                Assert.Equal(1, result.IncrementedDuringOnDeserializingMethod);
                Assert.Equal(1, result.IncrementedDuringOnDeserializedMethod);
                Assert.Equal(1, result.DerivedIncrementedDuringOnSerializingMethod);
                Assert.Equal(0, result.DerivedIncrementedDuringOnSerializedMethod);
                Assert.Equal(1, result.DerivedIncrementedDuringOnDeserializingMethod);
                Assert.Equal(1, result.DerivedIncrementedDuringOnDeserializedMethod);
            }
        }

        [Fact]
        public void Properties_Roundtrip()
        {
            var f = new BinaryFormatter();

            Assert.Null(f.Binder);
            var binder = new DelegateBinder();
            f.Binder = binder;
            Assert.Same(binder, f.Binder);

            Assert.NotNull(f.Context);
            Assert.Null(f.Context.Context);
            Assert.Equal(StreamingContextStates.All, f.Context.State);
            var context = new StreamingContext(StreamingContextStates.Clone);
            f.Context = context;
            Assert.Equal(StreamingContextStates.Clone, f.Context.State);

            Assert.Null(f.SurrogateSelector);
            var selector = new SurrogateSelector();
            f.SurrogateSelector = selector;
            Assert.Same(selector, f.SurrogateSelector);

            Assert.Equal(FormatterAssemblyStyle.Simple, f.AssemblyFormat);
            f.AssemblyFormat = FormatterAssemblyStyle.Full;
            Assert.Equal(FormatterAssemblyStyle.Full, f.AssemblyFormat);

            Assert.Equal(TypeFilterLevel.Full, f.FilterLevel);
            f.FilterLevel = TypeFilterLevel.Low;
            Assert.Equal(TypeFilterLevel.Low, f.FilterLevel);

            Assert.Equal(FormatterTypeStyle.TypesAlways, f.TypeFormat);
            f.TypeFormat = FormatterTypeStyle.XsdString;
            Assert.Equal(FormatterTypeStyle.XsdString, f.TypeFormat);
        }

        [Fact]
        public void SerializeDeserialize_InvalidArguments_ThrowsException()
        {
            var f = new BinaryFormatter();
            Assert.Throws<ArgumentNullException>("serializationStream", () => f.Serialize(null, new object()));
            Assert.Throws<ArgumentNullException>("serializationStream", () => f.Deserialize(null));
            Assert.Throws<SerializationException>(() => f.Deserialize(new MemoryStream())); // seekable, 0-length
        }

        [Theory]
        [InlineData(FormatterAssemblyStyle.Simple, false)]
        [InlineData(FormatterAssemblyStyle.Full, true)]
        public void MissingField_FailsWithAppropriateStyle(FormatterAssemblyStyle style, bool exceptionExpected)
        {
            var f = new BinaryFormatter();
            var s = new MemoryStream();
            f.Serialize(s, new Version1ClassWithoutField());
            s.Position = 0;

            f = new BinaryFormatter() { AssemblyFormat = style };
            f.Binder = new DelegateBinder { BindToTypeDelegate = (_, __) => typeof(Version2ClassWithoutOptionalField) };
            if (exceptionExpected)
            {
                Assert.Throws<SerializationException>(() => f.Deserialize(s));
            }
            else
            {
                var result = (Version2ClassWithoutOptionalField)f.Deserialize(s);
                Assert.NotNull(result);
                Assert.Equal(null, result.Value);
            }
        }

        [Theory]
        [InlineData(FormatterAssemblyStyle.Simple)]
        [InlineData(FormatterAssemblyStyle.Full)]
        public void OptionalField_Missing_Success(FormatterAssemblyStyle style)
        {
            var f = new BinaryFormatter();
            var s = new MemoryStream();
            f.Serialize(s, new Version1ClassWithoutField());
            s.Position = 0;

            f = new BinaryFormatter() { AssemblyFormat = style };
            f.Binder = new DelegateBinder { BindToTypeDelegate = (_, __) => typeof(Version2ClassWithOptionalField) };
            var result = (Version2ClassWithOptionalField)f.Deserialize(s);
            Assert.NotNull(result);
            Assert.Equal(null, result.Value);
        }

        [Fact]
        public void ObjectReference_RealObjectSerialized()
        {
            var obj = new ObjRefReturnsObj { Real = 42 };
            object real = FormatterClone<object>(obj);
            Assert.Equal(42, real);
        }

        public static IEnumerable<object[]> Deserialize_FuzzInput_MemberData()
        {
            var rand = new Random(42);
            foreach (object obj in SerializableObjects())
            {
                const int FuzzingsPerObject = 20;
                for (int i = 0; i < FuzzingsPerObject; i++)
                {
                    yield return new object[] { obj, rand };
                }
            }
        }

        [OuterLoop]
        [Theory]
        [MemberData(nameof(Deserialize_FuzzInput_MemberData))]
        public void Deserialize_FuzzInput(object obj, Random rand)
        {
            // Get the serialized data for the object
            var f = new BinaryFormatter();
            var s = new MemoryStream();
            f.Serialize(s, obj);

            // Make some "random" changes to it
            byte[] data = s.ToArray();
            for (int i = 1; i < rand.Next(1, 100); i++)
            {
                data[rand.Next(data.Length)] = (byte)rand.Next(256);
            }

            // Try to deserialize that.
            try
            {
                f.Deserialize(new MemoryStream(data));
                // Since there's no checksum, it's possible we changed data that didn't corrupt the instance
            }
            catch (ArgumentOutOfRangeException) { }
            catch (ArrayTypeMismatchException) { }
            catch (DecoderFallbackException) { }
            catch (FormatException) { }
            catch (IndexOutOfRangeException) { }
            catch (InvalidCastException) { }
            catch (OutOfMemoryException) { }
            catch (OverflowException) { }
            catch (NullReferenceException) { }
            catch (SerializationException) { }
        }

        [Fact]
        public void Deserialize_EndOfStream_ThrowsException()
        {
            var f = new BinaryFormatter();
            var s = new MemoryStream();
            f.Serialize(s, 1024);

            for (long i = s.Length - 1; i >= 0; i--)
            {
                s.Position = 0;
                var data = new byte[i];
                Assert.Equal(data.Length, s.Read(data, 0, data.Length));
                Assert.Throws<SerializationException>(() => f.Deserialize(new MemoryStream(data)));
            }
        }

        public static IEnumerable<object[]> Roundtrip_CrossProcess_MemberData()
        {
            // Just a few objects to verify we can roundtrip out of process memory
            yield return new object[] { "test" };
            yield return new object[] { new List<int> { 1, 2, 3, 4, 5 } };
            yield return new object[] { new Tree<int>(1, new Tree<int>(2, new Tree<int>(3, null, null), new Tree<int>(4, null, null)), new Tree<int>(5, null, null)) };
        }

        [Theory]
        [MemberData(nameof(Roundtrip_CrossProcess_MemberData))]
        public void Roundtrip_CrossProcess(object obj)
        {
            string outputPath = GetTestFilePath();
            string inputPath = GetTestFilePath();

            // Serialize out to a file
            using (FileStream fs = File.OpenWrite(outputPath))
            {
                new BinaryFormatter().Serialize(fs, obj);
            }

            // In another process, deserialize from that file and serialize to another
            RemoteInvoke((remoteInput, remoteOutput) =>
            {
                Assert.False(File.Exists(remoteOutput));
                using (FileStream input = File.OpenRead(remoteInput))
                using (FileStream output = File.OpenWrite(remoteOutput))
                {
                    var b = new BinaryFormatter();
                    b.Serialize(output, b.Deserialize(input));
                    return SuccessExitCode;
                }
            }, outputPath, inputPath).Dispose();

            // Deserialize what the other process serialized and compare it to the original
            using (FileStream fs = File.OpenRead(inputPath))
            {
                object deserialized = new BinaryFormatter().Deserialize(fs);
                Assert.Equal(obj, deserialized);
            }
        }

        [ActiveIssue("Fails on desktop and core: 'The object with ID 1 was referenced in a fixup but does not exist'")]
        [Fact]
        public void RoundtripWithHeaders_StringAsGraphRootAndInHeader()
        {
            RoundtripWithHeaders("any string");
        }

        [ActiveIssue("Fails on desktop and core: 'Unable to cast object of type 'System.UInt32[][*]' to type 'System.Object[]'")]
        [Fact]
        public void Roundtrip_ArrayContainingArrayAtNonZeroLowerBound()
        {
            FormatterClone(Array.CreateInstance(typeof(uint[]), new[] { 5 }, new[] { 1 }));
        }

        private class DelegateBinder : SerializationBinder
        {
            public Func<string, string, Type> BindToTypeDelegate = null;
            public override Type BindToType(string assemblyName, string typeName) => BindToTypeDelegate?.Invoke(assemblyName, typeName);
        }

        private static T FormatterClone<T>(
            T obj, 
            ISerializationSurrogate surrogate = null,
            FormatterAssemblyStyle assemblyFormat = FormatterAssemblyStyle.Full,
            TypeFilterLevel filterLevel = TypeFilterLevel.Full,
            FormatterTypeStyle typeFormat = FormatterTypeStyle.TypesAlways,
            bool unsafeDeserialize = false,
            Header[] headers = null)
        {
            BinaryFormatter f;
            if (surrogate == null)
            {
                f = new BinaryFormatter();
            }
            else
            {
                var c = new StreamingContext();
                var s = new SurrogateSelector();
                s.AddSurrogate(obj.GetType(), c, surrogate);
                f = new BinaryFormatter(s, c);
            }
            f.AssemblyFormat = assemblyFormat;
            f.FilterLevel = filterLevel;
            f.TypeFormat = typeFormat;

            using (var s = new MemoryStream())
            {
                f.Serialize(s, obj, headers);
                Assert.NotEqual(0, s.Position);
                s.Position = 0;
                return (T)(unsafeDeserialize ? f.UnsafeDeserialize(s, handler: null) : f.Deserialize(s));
            }
        }
    }
}
