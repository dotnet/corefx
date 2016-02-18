// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;

using Xunit;

namespace System.Runtime.Tests
{
    public static class TupleTests
    {
        private class TupleTestDriver<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
        {
            private int _nItems;

            private readonly object Tuple;
            private readonly Tuple<T1> Tuple1;
            private readonly Tuple<T1, T2> Tuple2;
            private readonly Tuple<T1, T2, T3> Tuple3;
            private readonly Tuple<T1, T2, T3, T4> Tuple4;
            private readonly Tuple<T1, T2, T3, T4, T5> Tuple5;
            private readonly Tuple<T1, T2, T3, T4, T5, T6> Tuple6;
            private readonly Tuple<T1, T2, T3, T4, T5, T6, T7> Tuple7;
            private readonly Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>> Tuple8;
            private readonly Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9>> Tuple9;
            private readonly Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10>> Tuple10;

            public TupleTestDriver(params object[] values)
            {
                if (values.Length == 0)
                    throw new ArgumentOutOfRangeException("values", "You must provide at least one value");
                if (values.Length > 10)
                    throw new ArgumentOutOfRangeException("values", "You must provide at most 10 values");

                _nItems = values.Length;
                switch (_nItems)
                {
                    case 1:
                        Tuple1 = new Tuple<T1>((T1)values[0]);
                        Tuple = Tuple1;
                        break;
                    case 2:
                        Tuple2 = new Tuple<T1, T2>((T1)values[0], (T2)values[1]);
                        Tuple = Tuple2;
                        break;
                    case 3:
                        Tuple3 = new Tuple<T1, T2, T3>((T1)values[0], (T2)values[1], (T3)values[2]);
                        Tuple = Tuple3;
                        break;
                    case 4:
                        Tuple4 = new Tuple<T1, T2, T3, T4>((T1)values[0], (T2)values[1], (T3)values[2], (T4)values[3]);
                        Tuple = Tuple4;
                        break;
                    case 5:
                        Tuple5 = new Tuple<T1, T2, T3, T4, T5>((T1)values[0], (T2)values[1], (T3)values[2], (T4)values[3], (T5)values[4]);
                        Tuple = Tuple5;
                        break;
                    case 6:
                        Tuple6 = new Tuple<T1, T2, T3, T4, T5, T6>((T1)values[0], (T2)values[1], (T3)values[2], (T4)values[3],
                            (T5)values[4], (T6)values[5]);
                        Tuple = Tuple6;
                        break;
                    case 7:
                        Tuple7 = new Tuple<T1, T2, T3, T4, T5, T6, T7>((T1)values[0], (T2)values[1], (T3)values[2], (T4)values[3],
                            (T5)values[4], (T6)values[5], (T7)values[6]);
                        Tuple = Tuple7;
                        break;
                    case 8:
                        Tuple8 = new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>>((T1)values[0], (T2)values[1], (T3)values[2], (T4)values[3],
                            (T5)values[4], (T6)values[5], (T7)values[6], new Tuple<T8>((T8)values[7]));
                        Tuple = Tuple8;
                        break;
                    case 9:
                        Tuple9 = new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9>>((T1)values[0], (T2)values[1], (T3)values[2], (T4)values[3],
                            (T5)values[4], (T6)values[5], (T7)values[6], new Tuple<T8, T9>((T8)values[7], (T9)values[8]));
                        Tuple = Tuple9;
                        break;
                    case 10:
                        Tuple10 = new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10>>((T1)values[0], (T2)values[1], (T3)values[2], (T4)values[3],
                            (T5)values[4], (T6)values[5], (T7)values[6], new Tuple<T8, T9, T10>((T8)values[7], (T9)values[8], (T10)values[9]));
                        Tuple = Tuple10;
                        break;
                }
            }

            private void VerifyItem(object obj1, object obj2)
            {
                Assert.Equal(obj1, obj2);
            }

            public void TestConstructor(params object[] expectedValue)
            {
                if (expectedValue.Length != _nItems)
                    throw new ArgumentOutOfRangeException("expectedValues", "You must provide " + _nItems + " expectedvalues");

                switch (_nItems)
                {
                    case 1:
                        VerifyItem(Tuple1.Item1, expectedValue[0]);
                        break;
                    case 2:
                        VerifyItem(Tuple2.Item1, expectedValue[0]);
                        VerifyItem(Tuple2.Item2, expectedValue[1]);
                        break;
                    case 3:
                        VerifyItem(Tuple3.Item1, expectedValue[0]);
                        VerifyItem(Tuple3.Item2, expectedValue[1]);
                        VerifyItem(Tuple3.Item3, expectedValue[2]);
                        break;
                    case 4:
                        VerifyItem(Tuple4.Item1, expectedValue[0]);
                        VerifyItem(Tuple4.Item2, expectedValue[1]);
                        VerifyItem(Tuple4.Item3, expectedValue[2]);
                        VerifyItem(Tuple4.Item4, expectedValue[3]);
                        break;
                    case 5:
                        VerifyItem(Tuple5.Item1, expectedValue[0]);
                        VerifyItem(Tuple5.Item2, expectedValue[1]);
                        VerifyItem(Tuple5.Item3, expectedValue[2]);
                        VerifyItem(Tuple5.Item4, expectedValue[3]);
                        VerifyItem(Tuple5.Item5, expectedValue[4]);
                        break;
                    case 6:
                        VerifyItem(Tuple6.Item1, expectedValue[0]);
                        VerifyItem(Tuple6.Item2, expectedValue[1]);
                        VerifyItem(Tuple6.Item3, expectedValue[2]);
                        VerifyItem(Tuple6.Item4, expectedValue[3]);
                        VerifyItem(Tuple6.Item5, expectedValue[4]);
                        VerifyItem(Tuple6.Item6, expectedValue[5]);
                        break;
                    case 7:
                        VerifyItem(Tuple7.Item1, expectedValue[0]);
                        VerifyItem(Tuple7.Item2, expectedValue[1]);
                        VerifyItem(Tuple7.Item3, expectedValue[2]);
                        VerifyItem(Tuple7.Item4, expectedValue[3]);
                        VerifyItem(Tuple7.Item5, expectedValue[4]);
                        VerifyItem(Tuple7.Item6, expectedValue[5]);
                        VerifyItem(Tuple7.Item7, expectedValue[6]);
                        break;
                    case 8: // Extended Tuple
                        VerifyItem(Tuple8.Item1, expectedValue[0]);
                        VerifyItem(Tuple8.Item2, expectedValue[1]);
                        VerifyItem(Tuple8.Item3, expectedValue[2]);
                        VerifyItem(Tuple8.Item4, expectedValue[3]);
                        VerifyItem(Tuple8.Item5, expectedValue[4]);
                        VerifyItem(Tuple8.Item6, expectedValue[5]);
                        VerifyItem(Tuple8.Item7, expectedValue[6]);
                        VerifyItem(Tuple8.Rest.Item1, expectedValue[7]);
                        break;
                    case 9: // Extended Tuple
                        VerifyItem(Tuple9.Item1, expectedValue[0]);
                        VerifyItem(Tuple9.Item2, expectedValue[1]);
                        VerifyItem(Tuple9.Item3, expectedValue[2]);
                        VerifyItem(Tuple9.Item4, expectedValue[3]);
                        VerifyItem(Tuple9.Item5, expectedValue[4]);
                        VerifyItem(Tuple9.Item6, expectedValue[5]);
                        VerifyItem(Tuple9.Item7, expectedValue[6]);
                        VerifyItem(Tuple9.Rest.Item1, expectedValue[7]);
                        VerifyItem(Tuple9.Rest.Item2, expectedValue[8]);
                        break;
                    case 10: // Extended Tuple
                        VerifyItem(Tuple10.Item1, expectedValue[0]);
                        VerifyItem(Tuple10.Item2, expectedValue[1]);
                        VerifyItem(Tuple10.Item3, expectedValue[2]);
                        VerifyItem(Tuple10.Item4, expectedValue[3]);
                        VerifyItem(Tuple10.Item5, expectedValue[4]);
                        VerifyItem(Tuple10.Item6, expectedValue[5]);
                        VerifyItem(Tuple10.Item7, expectedValue[6]);
                        VerifyItem(Tuple10.Rest.Item1, expectedValue[7]);
                        VerifyItem(Tuple10.Rest.Item2, expectedValue[8]);
                        VerifyItem(Tuple10.Rest.Item3, expectedValue[9]);
                        break;
                    default:
                        throw new ArgumentException("Must specify between 1 and 10 expected values (inclusive).");
                }
            }

            public void TestToString(string expected)
            {
                Assert.Equal(expected, Tuple.ToString());
            }

            public void TestEquals_GetHashCode(TupleTestDriver<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> other, bool expectEqual, bool expectStructuallyEqual)
            {
                if (expectEqual)
                {
                    Assert.True(Tuple.Equals(other.Tuple));
                    Assert.Equal(Tuple.GetHashCode(), other.Tuple.GetHashCode());
                }

                if (expectStructuallyEqual)
                {
                    var equatable = ((IStructuralEquatable)Tuple);
                    var otherEquatable = ((IStructuralEquatable)other.Tuple);
                    Assert.True(equatable.Equals(other.Tuple, TestEqualityComparer.Instance));
                    Assert.Equal(equatable.GetHashCode(TestEqualityComparer.Instance), otherEquatable.GetHashCode(TestEqualityComparer.Instance));
                }

                Assert.False(Tuple.Equals(null));
            }

            public void TestCompareTo(TupleTestDriver<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> other, int expectedResult, int expectedStructuralResult)
            {
                Assert.Equal(expectedResult, ((IComparable)Tuple).CompareTo(other.Tuple));
                Assert.Equal(expectedStructuralResult, ((IStructuralComparable)Tuple).CompareTo(other.Tuple, TestComparer.Instance));
                Assert.Equal(1, ((IComparable)Tuple).CompareTo(null));
            }

            public void TestNotEqual()
            {
                var tupleB = new Tuple<int>(10000);
                Assert.NotEqual(Tuple, tupleB);

                Assert.False(Tuple.Equals(123)); // Other is not a tuple
            }

            public void TestCompareToThrows()
            {
                var tupleB = new Tuple<int>(10000);
                Assert.Throws<ArgumentException>("other", () => ((IComparable)Tuple).CompareTo(tupleB));
            }
        }

        [Fact]
        public static void TestCtor()
        {
            // Tuple-1
            var tupleDriver = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>(short.MaxValue);
            tupleDriver.TestConstructor(short.MaxValue);

            // Tuple-2
            tupleDriver = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>(short.MinValue, int.MaxValue);
            tupleDriver.TestConstructor(short.MinValue, int.MaxValue);

            // Tuple-3
            tupleDriver = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>((short)0, 0, long.MaxValue);
            tupleDriver.TestConstructor((short)0, 0, long.MaxValue);

            // Tuple-4
            tupleDriver = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>((short)1, 1, long.MinValue, "This");
            tupleDriver.TestConstructor((short)1, 1, long.MinValue, "This");

            // Tuple-5
            tupleDriver = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>((short)(-1), -1, (long)0, "is", 'A');
            tupleDriver.TestConstructor((short)(-1), -1, (long)0, "is", 'A');

            // Tuple-6
            tupleDriver = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>((short)10, 100, (long)1, "testing", 'Z', float.MaxValue);
            tupleDriver.TestConstructor((short)10, 100, (long)1, "testing", 'Z', float.MaxValue);

            // Tuple-7
            tupleDriver = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>((short)(-100), -1000, (long)(-1), "Tuples", ' ', float.MinValue, double.MaxValue);
            tupleDriver.TestConstructor((short)(-100), -1000, (long)(-1), "Tuples", ' ', float.MinValue, double.MaxValue);

            // Tuple-10
            var obj = new object();
            DateTime now = DateTime.Now;
            tupleDriver = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>((short)10000, 1000000, (long)10000000, "2008年7月2日", '0', (float)0.0001, 0.0000001, now, Tuple.Create(false, obj), TimeSpan.Zero);
            tupleDriver.TestConstructor((short)10000, 1000000, (long)10000000, "2008年7月2日", '0', (float)0.0001, 0.0000001, now, Tuple.Create(false, obj), TimeSpan.Zero);
        }

        [Fact]
        public static void TestToString()
        {
            // Tuple-1
            var tupleDriver = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>(short.MaxValue);
            tupleDriver.TestToString("(" + short.MaxValue + ")");

            // Tuple-2
            tupleDriver = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>(short.MinValue, int.MaxValue);
            tupleDriver.TestToString("(" + short.MinValue + ", " + int.MaxValue + ")");

            // Tuple-3
            tupleDriver = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>((short)0, 0, long.MaxValue);
            tupleDriver.TestToString("(" + ((short)0) + ", " + 0 + ", " + long.MaxValue + ")");

            // Tuple-4
            tupleDriver = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>((short)1, 1, long.MinValue, "This");
            tupleDriver.TestConstructor((short)1, 1, long.MinValue, "This");
            tupleDriver.TestToString("(" + ((short)1) + ", " + 1 + ", " + long.MinValue + ", This)");

            // Tuple-5
            tupleDriver = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>((short)(-1), -1, (long)0, "is", 'A');
            tupleDriver.TestToString("(" + ((short)(-1)) + ", " + -1 + ", " + ((long)0) + ", is, A)");

            // Tuple-6
            tupleDriver = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>((short)10, 100, (long)1, "testing", 'Z', float.MaxValue);
            tupleDriver.TestToString("(" + ((short)10) + ", " + 100 + ", " + ((long)1) + ", testing, Z, " + float.MaxValue + ")");

            // Tuple-7
            tupleDriver = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>((short)(-100), -1000, (long)(-1), "Tuples", ' ', float.MinValue, double.MaxValue);
            tupleDriver.TestToString("(" + ((short)(-100)) + ", " + -1000 + ", " + ((long)(-1)) + ", Tuples,  , " + float.MinValue + ", " + double.MaxValue + ")");

            // Tuple-10
            var obj = new object();
            DateTime now = DateTime.Now;

            tupleDriver = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>((short)10000, 1000000, (long)10000000, "2008年7月2日", '0', (float)0.0001, 0.0000001, now, Tuple.Create(false, obj), TimeSpan.Zero);
            // .NET Native bug 438149 - object.ToString in incorrect
            tupleDriver.TestToString("(" + ((short)10000) + ", " + 1000000 + ", " + ((long)10000000) + ", 2008年7月2日, 0, " + ((float)0.0001) + ", " + 0.0000001 + ", " + now + ", (False, System.Object), " + TimeSpan.Zero + ")");
        }

        [Fact]
        public static void TestEquals_GetHashCode()
        {
            // Tuple-1
            var tupleDriver1 = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>(short.MaxValue);
            var tupleDriver2 = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>(short.MaxValue);
            var tupleDriver3 = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>(short.MinValue);
            tupleDriver1.TestEquals_GetHashCode(tupleDriver2, true, true);
            tupleDriver1.TestEquals_GetHashCode(tupleDriver3, false, false);

            // Tuple-2
            tupleDriver1 = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>(short.MinValue, int.MaxValue);
            tupleDriver2 = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>(short.MinValue, int.MaxValue);
            tupleDriver3 = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>(short.MinValue, int.MinValue);
            tupleDriver1.TestEquals_GetHashCode(tupleDriver2, true, true);
            tupleDriver1.TestEquals_GetHashCode(tupleDriver3, false, false);

            // Tuple-3
            tupleDriver1 = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>((short)0, 0, long.MaxValue);
            tupleDriver2 = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>((short)0, 0, long.MaxValue);
            tupleDriver3 = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>((short)(-1), -1, long.MinValue);
            tupleDriver1.TestEquals_GetHashCode(tupleDriver2, true, true);
            tupleDriver1.TestEquals_GetHashCode(tupleDriver3, false, false);

            // Tuple-4
            tupleDriver1 = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>((short)1, 1, long.MinValue, "This");
            tupleDriver2 = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>((short)1, 1, long.MinValue, "This");
            tupleDriver3 = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>((short)1, 1, long.MinValue, "this");
            tupleDriver1.TestEquals_GetHashCode(tupleDriver2, true, true);
            tupleDriver1.TestEquals_GetHashCode(tupleDriver3, false, false);

            // Tuple-5
            tupleDriver1 = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>((short)(-1), -1, (long)0, "is", 'A');
            tupleDriver2 = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>((short)(-1), -1, (long)0, "is", 'A');
            tupleDriver3 = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>((short)0, 0, (long)1, "IS", 'a');
            tupleDriver1.TestEquals_GetHashCode(tupleDriver2, true, true);
            tupleDriver1.TestEquals_GetHashCode(tupleDriver3, false, false);

            // Tuple-6
            tupleDriver1 = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>((short)10, 100, (long)1, "testing", 'Z', float.MaxValue);
            tupleDriver2 = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>((short)10, 100, (long)1, "testing", 'Z', float.MaxValue);
            tupleDriver3 = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>((short)10, 100, (long)1, "testing", 'Z', float.MinValue);
            tupleDriver1.TestEquals_GetHashCode(tupleDriver2, true, true);
            tupleDriver1.TestEquals_GetHashCode(tupleDriver3, false, false);

            // Tuple-7
            tupleDriver1 = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>((short)(-100), -1000, (long)(-1), "Tuples", ' ', float.MinValue, double.MaxValue);
            tupleDriver2 = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>((short)(-100), -1000, (long)(-1), "Tuples", ' ', float.MinValue, double.MaxValue);
            tupleDriver3 = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>((short)(-101), -1001, (long)(-2), "tuples", ' ', float.MinValue, 0.0);
            tupleDriver1.TestEquals_GetHashCode(tupleDriver2, true, true);
            tupleDriver1.TestEquals_GetHashCode(tupleDriver3, false, false);

            // Tuple-10
            var obj = new object();
            DateTime now = DateTime.Now;

            tupleDriver1 = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>((short)10000, 1000000, (long)10000000, "2008年7月2日", '0', (float)0.0001, 0.0000001, now, Tuple.Create(false, obj), TimeSpan.Zero);
            tupleDriver2 = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>((short)10000, 1000000, (long)10000000, "2008年7月2日", '0', (float)0.0001, 0.0000001, now, Tuple.Create(false, obj), TimeSpan.Zero);
            tupleDriver3 = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>((short)10001, 1000001, (long)10000001, "2008年7月3日", '1', (float)0.0002, 0.0000002, now.AddMilliseconds(1), Tuple.Create(true, obj), TimeSpan.MaxValue);
            tupleDriver1.TestEquals_GetHashCode(tupleDriver2, true, true);
            tupleDriver1.TestEquals_GetHashCode(tupleDriver3, false, false);
        }

        [Fact]
        public static void TestCompareTo()
        {
            // Tuple-1
            var tupleDriver1 = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>(short.MaxValue);
            var tupleDriver2 = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>(short.MaxValue);
            var tupleDriver3     = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>(short.MinValue);
            tupleDriver1.TestCompareTo(tupleDriver2, 0, 5);
            tupleDriver1.TestCompareTo(tupleDriver3, 65535, 5);

            // Tuple-2
            tupleDriver1 = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>(short.MinValue, int.MaxValue);
            tupleDriver2 = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>(short.MinValue, int.MaxValue);
            tupleDriver3 = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>(short.MinValue, int.MinValue);
            tupleDriver1.TestCompareTo(tupleDriver2, 0, 5);
            tupleDriver1.TestCompareTo(tupleDriver3, 1, 5);

            // Tuple-3
            tupleDriver1 = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>((short)0, 0, long.MaxValue);
            tupleDriver2 = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>((short)0, 0, long.MaxValue);
            tupleDriver3 = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>((short)(-1), -1, long.MinValue);
            tupleDriver1.TestCompareTo(tupleDriver2, 0, 5);
            tupleDriver1.TestCompareTo(tupleDriver3, 1, 5);

            // Tuple-4
            tupleDriver1 = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>((short)1, 1, long.MinValue, "This");
            tupleDriver2 = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>((short)1, 1, long.MinValue, "This");
            tupleDriver3 = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>((short)1, 1, long.MinValue, "this");
            tupleDriver1.TestCompareTo(tupleDriver2, 0, 5);
            tupleDriver1.TestCompareTo(tupleDriver3, 1, 5);

            // Tuple-5
            tupleDriver1 = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>((short)(-1), -1, (long)0, "is", 'A');
            tupleDriver2 = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>((short)(-1), -1, (long)0, "is", 'A');
            tupleDriver3 = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>((short)0, 0, (long)1, "IS", 'a');
            tupleDriver1.TestCompareTo(tupleDriver2, 0, 5);
            tupleDriver1.TestCompareTo(tupleDriver3, -1, 5);

            // Tuple-6
            tupleDriver1 = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>((short)10, 100, (long)1, "testing", 'Z', float.MaxValue);
            tupleDriver2 = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>((short)10, 100, (long)1, "testing", 'Z', float.MaxValue);
            tupleDriver3 = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>((short)10, 100, (long)1, "testing", 'Z', float.MinValue);
            tupleDriver1.TestCompareTo(tupleDriver2, 0, 5);
            tupleDriver1.TestCompareTo(tupleDriver3, 1, 5);

            // Tuple-7
            tupleDriver1 = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>((short)(-100), -1000, (long)(-1), "Tuples", ' ', float.MinValue, double.MaxValue);
            tupleDriver2 = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>((short)(-100), -1000, (long)(-1), "Tuples", ' ', float.MinValue, double.MaxValue);
            tupleDriver3 = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>((short)(-101), -1001, (long)(-2), "tuples", ' ', float.MinValue, 0.0);
            tupleDriver1.TestCompareTo(tupleDriver2, 0, 5);
            tupleDriver1.TestCompareTo(tupleDriver3, 1, 5);

            // Tuple-10
            var obj = new object();
            DateTime now = DateTime.Now;

            tupleDriver1 = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>((short)10000, 1000000, (long)10000000, "2008年7月2日", '0', (float)0.0001, 0.0000001, now, Tuple.Create(false, obj), TimeSpan.Zero);
            tupleDriver2 = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>((short)10000, 1000000, (long)10000000, "2008年7月2日", '0', (float)0.0001, 0.0000001, now, Tuple.Create(false, obj), TimeSpan.Zero);
            tupleDriver3 = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, Tuple<bool, object>, TimeSpan>((short)10001, 1000001, (long)10000001, "2008年7月3日", '1', (float)0.0002, 0.0000002, now.AddMilliseconds(1), Tuple.Create(true, obj), TimeSpan.MaxValue);
            tupleDriver1.TestCompareTo(tupleDriver2, 0, 5);
            tupleDriver1.TestCompareTo(tupleDriver3, -1, 5);
        }

        [Fact]
        public static void TestCompareTo_Invalid()
        {
            // Tuple-1
            var tupleDriver = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, bool, TimeSpan>((short)10000);
            tupleDriver.TestCompareToThrows();

            // Tuple-2
            tupleDriver = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, bool, TimeSpan>((short)10000, 1000000);
            tupleDriver.TestCompareToThrows();

            // Tuple-3
            tupleDriver = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, bool, TimeSpan>((short)10000, 1000000, (long)10000000);
            tupleDriver.TestCompareToThrows();

            // Tuple-4
            tupleDriver = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, bool, TimeSpan>((short)10000, 1000000, (long)10000000, "2008年7月2日");
            tupleDriver.TestCompareToThrows();

            // Tuple-5
            tupleDriver = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, bool, TimeSpan>((short)10000, 1000000, (long)10000000, "2008年7月2日", '0');
            tupleDriver.TestCompareToThrows();

            // Tuple-6
            tupleDriver = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, bool, TimeSpan>((short)10000, 1000000, (long)10000000, "2008年7月2日", '0', float.NaN);
            tupleDriver.TestCompareToThrows();

            // Tuple-7
            tupleDriver = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, bool, TimeSpan>((short)10000, 1000000, (long)10000000, "2008年7月2日", '0', float.NaN, double.NegativeInfinity);

            tupleDriver.TestCompareToThrows();
            // Tuple-8, extended tuple
            tupleDriver = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, bool, TimeSpan>((short)10000, 1000000, (long)10000000, "2008年7月2日", '0', float.NaN, double.NegativeInfinity, DateTime.Now);
            tupleDriver.TestCompareToThrows();
            // Tuple-9 and Tuple-10 are not necesary because they use the same code path as Tuple-8
        }

        [Fact]
        public static void TestNotEqual()
        {
            // Tuple-1
            var tupleDriver = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, bool, TimeSpan>((short)10000);
            tupleDriver.TestNotEqual();

            // Tuple-2
            tupleDriver = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, bool, TimeSpan>((short)10000, 1000000);
            tupleDriver.TestNotEqual();

            // Tuple-3
            tupleDriver = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, bool, TimeSpan>((short)10000, 1000000, (long)10000000);
            tupleDriver.TestNotEqual();

            // Tuple-4
            tupleDriver = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, bool, TimeSpan>((short)10000, 1000000, (long)10000000, "2008年7月2日");
            tupleDriver.TestNotEqual();

            // Tuple-5
            tupleDriver = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, bool, TimeSpan>((short)10000, 1000000, (long)10000000, "2008年7月2日", '0');
            tupleDriver.TestNotEqual();

            // Tuple-6
            tupleDriver = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, bool, TimeSpan>((short)10000, 1000000, (long)10000000, "2008年7月2日", '0', float.NaN);
            tupleDriver.TestNotEqual();

            // Tuple-7
            tupleDriver = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, bool, TimeSpan>((short)10000, 1000000, (long)10000000, "2008年7月2日", '0', float.NaN, double.NegativeInfinity);
            tupleDriver.TestNotEqual();

            // Tuple-8, extended tuple
            tupleDriver = new TupleTestDriver<short, int, long, string, char, float, double, DateTime, bool, TimeSpan>((short)10000, 1000000, (long)10000000, "2008年7月2日", '0', float.NaN, double.NegativeInfinity, DateTime.Now);
            tupleDriver.TestNotEqual();
            // Tuple-9 and Tuple-10 are not necesary because they use the same code path as Tuple-8
        }
        
        [Fact]
        public static void TestCreate_CustomType1()
        {
            // Special case of Tuple<T1> where T1 is a custom type
            var testClass = new TestClass1();
            Tuple<TestClass1> a = Tuple.Create(testClass);
            Tuple<TestClass1> b = Tuple.Create(testClass);

            Assert.True(a.Equals(b));
            Assert.Equal(0, ((IComparable)a).CompareTo(b));
            Assert.Equal(a.GetHashCode(), b.GetHashCode());
            Assert.True(((IStructuralEquatable)a).Equals(b, TestEqualityComparer.Instance));
            Assert.Equal(5, ((IStructuralComparable)a).CompareTo(b, TestComparer.Instance));
            Assert.Equal(((IStructuralEquatable)a).GetHashCode(TestEqualityComparer.Instance), ((IStructuralEquatable)b).GetHashCode(TestEqualityComparer.Instance));
        }

        [Fact]
        public static void TestCreate_CustomType2()
        {
            // Special case of Tuple<T1, T2> where T2 is a custom type
            var testClass = new TestClass1(1);
            Tuple<int, TestClass1> a = Tuple.Create(1, testClass);
            Tuple<int, TestClass1> b = Tuple.Create(1, testClass);

            Assert.True(a.Equals(b));
            Assert.Equal(0, ((IComparable)a).CompareTo(b));
            Assert.Equal(a.GetHashCode(), b.GetHashCode());
            Assert.True(((IStructuralEquatable)a).Equals(b, TestEqualityComparer.Instance));
            Assert.Equal(5, ((IStructuralComparable)a).CompareTo(b, TestComparer.Instance));
            Assert.Equal(((IStructuralEquatable)a).GetHashCode(TestEqualityComparer.Instance), ((IStructuralEquatable)b).GetHashCode(TestEqualityComparer.Instance));
        }

        [Fact]
        public static void TestCreate_CustomType3()
        {
            // Special case of Tuple<T1, T2> where T1 and T2 are custom types
            var testClass1 = new TestClass1(100);
            var testClass2 = new TestClass1(101);
            Tuple<TestClass1, TestClass1> a = Tuple.Create(testClass1, testClass2);
            Tuple<TestClass1, TestClass1> b = Tuple.Create(testClass2, testClass1);

            Assert.False(a.Equals(b));
            Assert.Equal(-1, ((IComparable)a).CompareTo(b));
            Assert.False(((IStructuralEquatable)a).Equals(b, TestEqualityComparer.Instance));
            Assert.Equal(5, ((IStructuralComparable)a).CompareTo(b, TestComparer.Instance));
            // Equals(IEqualityComparer) is false, ignore hash code
        }

        [Fact]
        public static void TestCreate_CustomType4()
        {
            // Special case of Tuple<T1, T2> where T1 and T2 are custom types
            var testClass1 = new TestClass1(100);
            var testClass2 = new TestClass1(101);
            Tuple<TestClass1, TestClass1> a = Tuple.Create(testClass1, testClass2);
            Tuple<TestClass1, TestClass1> b = Tuple.Create(testClass1, testClass1);

            Assert.False(a.Equals(b));
            Assert.Equal(1, ((IComparable)a).CompareTo(b));
            Assert.False(((IStructuralEquatable)a).Equals(b, TestEqualityComparer.Instance));
            Assert.Equal(5, ((IStructuralComparable)a).CompareTo(b, TestComparer.Instance));
            // Equals(IEqualityComparer) is false, ignore hash code
        }

        [Fact]
        public static void TestCreate_DoubleFloatNaNParameters()
        {
            Tuple<double, double, float, float> a = Tuple.Create(double.MinValue, double.NaN, float.MinValue, float.NaN);
            Tuple<double, double, float, float> b = Tuple.Create(double.MinValue, double.NaN, float.MinValue, float.NaN);

            Assert.True(a.Equals(b));
            Assert.Equal(0, ((IComparable)a).CompareTo(b));
            Assert.Equal(a.GetHashCode(), b.GetHashCode());
            Assert.True(((IStructuralEquatable)a).Equals(b, TestEqualityComparer.Instance));
            Assert.Equal(5, ((IStructuralComparable)a).CompareTo(b, TestComparer.Instance));
            Assert.Equal(((IStructuralEquatable)a).GetHashCode(TestEqualityComparer.Instance), ((IStructuralEquatable)b).GetHashCode(TestEqualityComparer.Instance));
        }

        [Fact]
        public static void TestCreate_NestedTuples1()
        {
            var a = Tuple.Create(1, 2, Tuple.Create(31, 32), 4, 5, 6, 7, Tuple.Create(8, 9));
            var b = Tuple.Create(1, 2, Tuple.Create(31, 32), 4, 5, 6, 7, Tuple.Create(8, 9));

            Assert.True(a.Equals(b));
            Assert.Equal(0, ((IComparable)a).CompareTo(b));
            Assert.Equal(a.GetHashCode(), b.GetHashCode());
            Assert.True(((IStructuralEquatable)a).Equals(b, TestEqualityComparer.Instance));
            Assert.Equal(5, ((IStructuralComparable)a).CompareTo(b, TestComparer.Instance));
            Assert.Equal(((IStructuralEquatable)a).GetHashCode(TestEqualityComparer.Instance), ((IStructuralEquatable)b).GetHashCode(TestEqualityComparer.Instance));
            Assert.Equal("(1, 2, (31, 32), 4, 5, 6, 7, (8, 9))", a.ToString());
            Assert.Equal("(31, 32)", a.Item3.ToString());
            Assert.Equal("((8, 9))", a.Rest.ToString());
        }

        [Fact]
        public static void TestCreate_NestedTuples2()
        {
            var a = Tuple.Create(0, 1, 2, 3, 4, 5, 6, Tuple.Create(7, 8, 9, 10, 11, 12, 13, Tuple.Create(14, 15)));
            var b = Tuple.Create(1, 2, 3, 4, 5, 6, 7, Tuple.Create(8, 9, 10, 11, 12, 13, 14, Tuple.Create(15, 16)));

            Assert.False(a.Equals(b));
            Assert.Equal(-1, ((IComparable)a).CompareTo(b));
            Assert.False(((IStructuralEquatable)a).Equals(b, TestEqualityComparer.Instance));
            Assert.Equal(5, ((IStructuralComparable)a).CompareTo(b, TestComparer.Instance));
            Assert.Equal("(0, 1, 2, 3, 4, 5, 6, (7, 8, 9, 10, 11, 12, 13, (14, 15)))", a.ToString());
            Assert.Equal("(1, 2, 3, 4, 5, 6, 7, (8, 9, 10, 11, 12, 13, 14, (15, 16)))", b.ToString());
            Assert.Equal("((7, 8, 9, 10, 11, 12, 13, (14, 15)))", a.Rest.ToString());
        }

        [Fact]
        public static void TestCreate_NonIComparable()
        {
            // Special case when T does not implement IComparable
            var testClass1 = new TestClass2(100);
            var testClass2 = new TestClass2(100);
            var a = Tuple.Create(testClass1);
            var b = Tuple.Create(testClass2);

            Assert.True(a.Equals(b));
            Assert.Throws<ArgumentException>(null, () => ((IComparable)a).CompareTo(b)); // B does not implement IComparable
            Assert.Equal(a.GetHashCode(), b.GetHashCode());
            Assert.True(((IStructuralEquatable)a).Equals(b, TestEqualityComparer.Instance));
            Assert.Equal(5, ((IStructuralComparable)a).CompareTo(b, TestComparer.Instance));
            Assert.Equal(((IStructuralEquatable)a).GetHashCode(TestEqualityComparer.Instance), ((IStructuralEquatable)b).GetHashCode(TestEqualityComparer.Instance));
            Assert.Equal("([100])", a.ToString());
        }

        private class TestClass1 : IComparable
        {
            public TestClass1()
            {
            }

            public TestClass1(int value)
            {
                _value = value;
            }

            public override string ToString()
            {
                return "{" + _value.ToString() + "}";
            }

            private readonly int _value;

            public int CompareTo(object x)
            {
                TestClass1 tmp = x as TestClass1;
                if (tmp != null)
                    return _value.CompareTo(tmp._value);
                else
                    return 1;
            }
        }

        private class TestClass2
        {
            public TestClass2()
            {
            }

            public TestClass2(int value)
            {
                _value = value;
            }

            private readonly int _value;

            public override string ToString()
            {
                return "[" + _value.ToString() + "]";
            }

            public override bool Equals(object x)
            {
                TestClass2 tmp = x as TestClass2;
                if (tmp != null)
                    return _value.Equals(tmp._value);
                else
                    return false;
            }

            public override int GetHashCode()
            {
                return _value.GetHashCode();
            }
        }

        private class TestComparer : IComparer
        {
            public static readonly TestComparer Instance = new TestComparer();

            public int Compare(object x, object y)
            {
                return 5;
            }
        }

        private class TestEqualityComparer : IEqualityComparer
        {
            public static readonly TestEqualityComparer Instance = new TestEqualityComparer();

            public new bool Equals(object x, object y)
            {
                return x.Equals(y);
            }

            public int GetHashCode(object x)
            {
                return x.GetHashCode();
            }
        }
    }
}
