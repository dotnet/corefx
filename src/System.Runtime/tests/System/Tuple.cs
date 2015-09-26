// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using Xunit;

public class TupleTests
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

        internal TupleTestDriver(params object[] values)
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

        private void VerifyItem(int itemPos, object Item1, object Item2)
        {
            Assert.True(object.Equals(Item1, Item2));
        }

        public void TestConstructor(params object[] expectedValue)
        {
            if (expectedValue.Length != _nItems)
                throw new ArgumentOutOfRangeException("expectedValues", "You must provide " + _nItems + " expectedvalues");

            switch (_nItems)
            {
                case 1:
                    VerifyItem(1, Tuple1.Item1, expectedValue[0]);
                    break;
                case 2:
                    VerifyItem(1, Tuple2.Item1, expectedValue[0]);
                    VerifyItem(2, Tuple2.Item2, expectedValue[1]);
                    break;
                case 3:
                    VerifyItem(1, Tuple3.Item1, expectedValue[0]);
                    VerifyItem(2, Tuple3.Item2, expectedValue[1]);
                    VerifyItem(3, Tuple3.Item3, expectedValue[2]);
                    break;
                case 4:
                    VerifyItem(1, Tuple4.Item1, expectedValue[0]);
                    VerifyItem(2, Tuple4.Item2, expectedValue[1]);
                    VerifyItem(3, Tuple4.Item3, expectedValue[2]);
                    VerifyItem(4, Tuple4.Item4, expectedValue[3]);
                    break;
                case 5:
                    VerifyItem(1, Tuple5.Item1, expectedValue[0]);
                    VerifyItem(2, Tuple5.Item2, expectedValue[1]);
                    VerifyItem(3, Tuple5.Item3, expectedValue[2]);
                    VerifyItem(4, Tuple5.Item4, expectedValue[3]);
                    VerifyItem(5, Tuple5.Item5, expectedValue[4]);
                    break;
                case 6:
                    VerifyItem(1, Tuple6.Item1, expectedValue[0]);
                    VerifyItem(2, Tuple6.Item2, expectedValue[1]);
                    VerifyItem(3, Tuple6.Item3, expectedValue[2]);
                    VerifyItem(4, Tuple6.Item4, expectedValue[3]);
                    VerifyItem(5, Tuple6.Item5, expectedValue[4]);
                    VerifyItem(6, Tuple6.Item6, expectedValue[5]);
                    break;
                case 7:
                    VerifyItem(1, Tuple7.Item1, expectedValue[0]);
                    VerifyItem(2, Tuple7.Item2, expectedValue[1]);
                    VerifyItem(3, Tuple7.Item3, expectedValue[2]);
                    VerifyItem(4, Tuple7.Item4, expectedValue[3]);
                    VerifyItem(5, Tuple7.Item5, expectedValue[4]);
                    VerifyItem(6, Tuple7.Item6, expectedValue[5]);
                    VerifyItem(7, Tuple7.Item7, expectedValue[6]);
                    break;
                case 8: // Extended Tuple
                    VerifyItem(1, Tuple8.Item1, expectedValue[0]);
                    VerifyItem(2, Tuple8.Item2, expectedValue[1]);
                    VerifyItem(3, Tuple8.Item3, expectedValue[2]);
                    VerifyItem(4, Tuple8.Item4, expectedValue[3]);
                    VerifyItem(5, Tuple8.Item5, expectedValue[4]);
                    VerifyItem(6, Tuple8.Item6, expectedValue[5]);
                    VerifyItem(7, Tuple8.Item7, expectedValue[6]);
                    VerifyItem(8, Tuple8.Rest.Item1, expectedValue[7]);
                    break;
                case 9: // Extended Tuple
                    VerifyItem(1, Tuple9.Item1, expectedValue[0]);
                    VerifyItem(2, Tuple9.Item2, expectedValue[1]);
                    VerifyItem(3, Tuple9.Item3, expectedValue[2]);
                    VerifyItem(4, Tuple9.Item4, expectedValue[3]);
                    VerifyItem(5, Tuple9.Item5, expectedValue[4]);
                    VerifyItem(6, Tuple9.Item6, expectedValue[5]);
                    VerifyItem(7, Tuple9.Item7, expectedValue[6]);
                    VerifyItem(8, Tuple9.Rest.Item1, expectedValue[7]);
                    VerifyItem(9, Tuple9.Rest.Item2, expectedValue[8]);
                    break;
                case 10: // Extended Tuple
                    VerifyItem(1, Tuple10.Item1, expectedValue[0]);
                    VerifyItem(2, Tuple10.Item2, expectedValue[1]);
                    VerifyItem(3, Tuple10.Item3, expectedValue[2]);
                    VerifyItem(4, Tuple10.Item4, expectedValue[3]);
                    VerifyItem(5, Tuple10.Item5, expectedValue[4]);
                    VerifyItem(6, Tuple10.Item6, expectedValue[5]);
                    VerifyItem(7, Tuple10.Item7, expectedValue[6]);
                    VerifyItem(8, Tuple10.Rest.Item1, expectedValue[7]);
                    VerifyItem(9, Tuple10.Rest.Item2, expectedValue[8]);
                    VerifyItem(10, Tuple10.Rest.Item3, expectedValue[9]);
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
            Tuple<Int32> tupleB = new Tuple<Int32>((Int32)10000);
            Assert.NotEqual(Tuple, tupleB);
        }

        internal void TestCompareToThrows()
        {
            Tuple<Int32> tupleB = new Tuple<Int32>((Int32)10000);
            Assert.Throws<ArgumentException>(() => ((IComparable)Tuple).CompareTo(tupleB));
        }
    }

    [Fact]
    public static void TestConstructor()
    {
        TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan> tupleDriverA;
        //Tuple-1
        tupleDriverA = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>(Int16.MaxValue);
        tupleDriverA.TestConstructor(Int16.MaxValue);
        //Tuple-2
        tupleDriverA = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>(Int16.MinValue, Int32.MaxValue);
        tupleDriverA.TestConstructor(Int16.MinValue, Int32.MaxValue);
        //Tuple-3
        tupleDriverA = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>((Int16)0, (Int32)0, Int64.MaxValue);
        tupleDriverA.TestConstructor((Int16)0, (Int32)0, Int64.MaxValue);
        //Tuple-4
        tupleDriverA = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>((Int16)1, (Int32)1, Int64.MinValue, "This");
        tupleDriverA.TestConstructor((Int16)1, (Int32)1, Int64.MinValue, "This");
        //Tuple-5
        tupleDriverA = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>((Int16)(-1), (Int32)(-1), (Int64)0, "is", 'A');
        tupleDriverA.TestConstructor((Int16)(-1), (Int32)(-1), (Int64)0, "is", 'A');
        //Tuple-6
        tupleDriverA = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>((Int16)10, (Int32)100, (Int64)1, "testing", 'Z', Single.MaxValue);
        tupleDriverA.TestConstructor((Int16)10, (Int32)100, (Int64)1, "testing", 'Z', Single.MaxValue);
        //Tuple-7
        tupleDriverA = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>((Int16)(-100), (Int32)(-1000), (Int64)(-1), "Tuples", ' ', Single.MinValue, Double.MaxValue);
        tupleDriverA.TestConstructor((Int16)(-100), (Int32)(-1000), (Int64)(-1), "Tuples", ' ', Single.MinValue, Double.MaxValue);

        Object myObj = new Object();
        //Tuple-10
        DateTime now = DateTime.Now;
        tupleDriverA = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>((Int16)10000, (Int32)1000000, (Int64)10000000, "2008年7月2日", '0', (Single)0.0001, (Double)0.0000001, now, Tuple.Create(false, myObj), TimeSpan.Zero);
        tupleDriverA.TestConstructor((Int16)10000, (Int32)1000000, (Int64)10000000, "2008年7月2日", '0', (Single)0.0001, (Double)0.0000001, now, Tuple.Create(false, myObj), TimeSpan.Zero);
    }

    [Fact]
    public static void TestToString()
    {
        TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan> tupleDriverA;
        //Tuple-1
        tupleDriverA = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>(Int16.MaxValue);
        tupleDriverA.TestToString("(" + Int16.MaxValue + ")");
        //Tuple-2
        tupleDriverA = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>(Int16.MinValue, Int32.MaxValue);
        tupleDriverA.TestToString("(" + Int16.MinValue + ", " + Int32.MaxValue + ")");
        //Tuple-3
        tupleDriverA = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>((Int16)0, (Int32)0, Int64.MaxValue);
        tupleDriverA.TestToString("(" + ((Int16)0) + ", " + ((Int32)0) + ", " + Int64.MaxValue + ")");
        //Tuple-4
        tupleDriverA = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>((Int16)1, (Int32)1, Int64.MinValue, "This");
        tupleDriverA.TestConstructor((Int16)1, (Int32)1, Int64.MinValue, "This");
        tupleDriverA.TestToString("(" + ((Int16)1) + ", " + ((Int32)1) + ", " + Int64.MinValue + ", This)");
        //Tuple-5
        tupleDriverA = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>((Int16)(-1), (Int32)(-1), (Int64)0, "is", 'A');
        tupleDriverA.TestToString("(" + ((Int16)(-1)) + ", " + ((Int32)(-1)) + ", " + ((Int64)0) + ", is, A)");
        //Tuple-6
        tupleDriverA = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>((Int16)10, (Int32)100, (Int64)1, "testing", 'Z', Single.MaxValue);
        tupleDriverA.TestToString("(" + ((Int16)10) + ", " + ((Int32)100) + ", " + ((Int64)1) + ", testing, Z, " + Single.MaxValue + ")");
        //Tuple-7
        tupleDriverA = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>((Int16)(-100), (Int32)(-1000), (Int64)(-1), "Tuples", ' ', Single.MinValue, Double.MaxValue);
        tupleDriverA.TestToString("(" + ((Int16)(-100)) + ", " + ((Int32)(-1000)) + ", " + ((Int64)(-1)) + ", Tuples,  , " + Single.MinValue + ", " + Double.MaxValue + ")");

        Object myObj = new Object();
        //Tuple-10
        DateTime now = DateTime.Now;

        tupleDriverA = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>((Int16)10000, (Int32)1000000, (Int64)10000000, "2008年7月2日", '0', (Single)0.0001, (Double)0.0000001, now, Tuple.Create(false, myObj), TimeSpan.Zero);
        // .NET Native bug 438149 - object.ToString in incorrect
        tupleDriverA.TestToString("(" + ((Int16)10000) + ", " + ((Int32)1000000) + ", " + ((Int64)10000000) + ", 2008年7月2日, 0, " + ((Single)0.0001) + ", " + ((Double)0.0000001) + ", " + now + ", (False, System.Object), " + TimeSpan.Zero + ")");
    }

    [Fact]
    public static void TestEquals_GetHashCode()
    {
        TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan> tupleDriverA, tupleDriverB, tupleDriverC;
        //Tuple-1
        tupleDriverA = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>(Int16.MaxValue);
        tupleDriverB = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>(Int16.MaxValue);
        tupleDriverC = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>(Int16.MinValue);
        tupleDriverA.TestEquals_GetHashCode(tupleDriverB, true, true);
        tupleDriverA.TestEquals_GetHashCode(tupleDriverC, false, false);
        //Tuple-2
        tupleDriverA = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>(Int16.MinValue, Int32.MaxValue);
        tupleDriverB = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>(Int16.MinValue, Int32.MaxValue);
        tupleDriverC = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>(Int16.MinValue, Int32.MinValue);
        tupleDriverA.TestEquals_GetHashCode(tupleDriverB, true, true);
        tupleDriverA.TestEquals_GetHashCode(tupleDriverC, false, false);
        //Tuple-3
        tupleDriverA = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>((Int16)0, (Int32)0, Int64.MaxValue);
        tupleDriverB = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>((Int16)0, (Int32)0, Int64.MaxValue);
        tupleDriverC = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>((Int16)(-1), (Int32)(-1), Int64.MinValue);
        tupleDriverA.TestEquals_GetHashCode(tupleDriverB, true, true);
        tupleDriverA.TestEquals_GetHashCode(tupleDriverC, false, false);
        //Tuple-4
        tupleDriverA = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>((Int16)1, (Int32)1, Int64.MinValue, "This");
        tupleDriverB = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>((Int16)1, (Int32)1, Int64.MinValue, "This");
        tupleDriverC = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>((Int16)1, (Int32)1, Int64.MinValue, "this");
        tupleDriverA.TestEquals_GetHashCode(tupleDriverB, true, true);
        tupleDriverA.TestEquals_GetHashCode(tupleDriverC, false, false);
        //Tuple-5
        tupleDriverA = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>((Int16)(-1), (Int32)(-1), (Int64)0, "is", 'A');
        tupleDriverB = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>((Int16)(-1), (Int32)(-1), (Int64)0, "is", 'A');
        tupleDriverC = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>((Int16)0, (Int32)0, (Int64)1, "IS", 'a');
        tupleDriverA.TestEquals_GetHashCode(tupleDriverB, true, true);
        tupleDriverA.TestEquals_GetHashCode(tupleDriverC, false, false);
        //Tuple-6
        tupleDriverA = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>((Int16)10, (Int32)100, (Int64)1, "testing", 'Z', Single.MaxValue);
        tupleDriverB = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>((Int16)10, (Int32)100, (Int64)1, "testing", 'Z', Single.MaxValue);
        tupleDriverC = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>((Int16)10, (Int32)100, (Int64)1, "testing", 'Z', Single.MinValue);
        tupleDriverA.TestEquals_GetHashCode(tupleDriverB, true, true);
        tupleDriverA.TestEquals_GetHashCode(tupleDriverC, false, false);
        //Tuple-7
        tupleDriverA = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>((Int16)(-100), (Int32)(-1000), (Int64)(-1), "Tuples", ' ', Single.MinValue, Double.MaxValue);
        tupleDriverB = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>((Int16)(-100), (Int32)(-1000), (Int64)(-1), "Tuples", ' ', Single.MinValue, Double.MaxValue);
        tupleDriverC = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>((Int16)(-101), (Int32)(-1001), (Int64)(-2), "tuples", ' ', Single.MinValue, (Double)0.0);
        tupleDriverA.TestEquals_GetHashCode(tupleDriverB, true, true);
        tupleDriverA.TestEquals_GetHashCode(tupleDriverC, false, false);

        Object myObj = new Object();
        //Tuple-10
        DateTime now = DateTime.Now;

        tupleDriverA = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>((Int16)10000, (Int32)1000000, (Int64)10000000, "2008年7月2日", '0', (Single)0.0001, (Double)0.0000001, now, Tuple.Create(false, myObj), TimeSpan.Zero);
        tupleDriverB = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>((Int16)10000, (Int32)1000000, (Int64)10000000, "2008年7月2日", '0', (Single)0.0001, (Double)0.0000001, now, Tuple.Create(false, myObj), TimeSpan.Zero);
        tupleDriverC = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>((Int16)10001, (Int32)1000001, (Int64)10000001, "2008年7月3日", '1', (Single)0.0002, (Double)0.0000002, now.AddMilliseconds(1), Tuple.Create(true, myObj), TimeSpan.MaxValue);
        tupleDriverA.TestEquals_GetHashCode(tupleDriverB, true, true);
        tupleDriverA.TestEquals_GetHashCode(tupleDriverC, false, false);
    }

    [Fact]
    [ActiveIssue(846, PlatformID.AnyUnix)]
    public static void TestCompareTo()
    {
        TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan> tupleDriverA, tupleDriverB, tupleDriverC;
        //Tuple-1
        tupleDriverA = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>(Int16.MaxValue);
        tupleDriverB = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>(Int16.MaxValue);
        tupleDriverC = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>(Int16.MinValue);
        tupleDriverA.TestCompareTo(tupleDriverB, 0, 5);
        tupleDriverA.TestCompareTo(tupleDriverC, 65535, 5);
        //Tuple-2
        tupleDriverA = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>(Int16.MinValue, Int32.MaxValue);
        tupleDriverB = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>(Int16.MinValue, Int32.MaxValue);
        tupleDriverC = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>(Int16.MinValue, Int32.MinValue);
        tupleDriverA.TestCompareTo(tupleDriverB, 0, 5);
        tupleDriverA.TestCompareTo(tupleDriverC, 1, 5);
        //Tuple-3
        tupleDriverA = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>((Int16)0, (Int32)0, Int64.MaxValue);
        tupleDriverB = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>((Int16)0, (Int32)0, Int64.MaxValue);
        tupleDriverC = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>((Int16)(-1), (Int32)(-1), Int64.MinValue);
        tupleDriverA.TestCompareTo(tupleDriverB, 0, 5);
        tupleDriverA.TestCompareTo(tupleDriverC, 1, 5);
        //Tuple-4
        tupleDriverA = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>((Int16)1, (Int32)1, Int64.MinValue, "This");
        tupleDriverB = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>((Int16)1, (Int32)1, Int64.MinValue, "This");
        tupleDriverC = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>((Int16)1, (Int32)1, Int64.MinValue, "this");
        tupleDriverA.TestCompareTo(tupleDriverB, 0, 5);
        tupleDriverA.TestCompareTo(tupleDriverC, 1, 5);
        //Tuple-5
        tupleDriverA = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>((Int16)(-1), (Int32)(-1), (Int64)0, "is", 'A');
        tupleDriverB = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>((Int16)(-1), (Int32)(-1), (Int64)0, "is", 'A');
        tupleDriverC = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>((Int16)0, (Int32)0, (Int64)1, "IS", 'a');
        tupleDriverA.TestCompareTo(tupleDriverB, 0, 5);
        tupleDriverA.TestCompareTo(tupleDriverC, -1, 5);
        //Tuple-6
        tupleDriverA = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>((Int16)10, (Int32)100, (Int64)1, "testing", 'Z', Single.MaxValue);
        tupleDriverB = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>((Int16)10, (Int32)100, (Int64)1, "testing", 'Z', Single.MaxValue);
        tupleDriverC = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>((Int16)10, (Int32)100, (Int64)1, "testing", 'Z', Single.MinValue);
        tupleDriverA.TestCompareTo(tupleDriverB, 0, 5);
        tupleDriverA.TestCompareTo(tupleDriverC, 1, 5);
        //Tuple-7
        tupleDriverA = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>((Int16)(-100), (Int32)(-1000), (Int64)(-1), "Tuples", ' ', Single.MinValue, Double.MaxValue);
        tupleDriverB = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>((Int16)(-100), (Int32)(-1000), (Int64)(-1), "Tuples", ' ', Single.MinValue, Double.MaxValue);
        tupleDriverC = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>((Int16)(-101), (Int32)(-1001), (Int64)(-2), "tuples", ' ', Single.MinValue, (Double)0.0);
        tupleDriverA.TestCompareTo(tupleDriverB, 0, 5);
        tupleDriverA.TestCompareTo(tupleDriverC, 1, 5);

        Object myObj = new Object();
        //Tuple-10
        DateTime now = DateTime.Now;

        tupleDriverA = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>((Int16)10000, (Int32)1000000, (Int64)10000000, "2008年7月2日", '0', (Single)0.0001, (Double)0.0000001, now, Tuple.Create(false, myObj), TimeSpan.Zero);
        tupleDriverB = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>((Int16)10000, (Int32)1000000, (Int64)10000000, "2008年7月2日", '0', (Single)0.0001, (Double)0.0000001, now, Tuple.Create(false, myObj), TimeSpan.Zero);
        tupleDriverC = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Tuple<Boolean, Object>, TimeSpan>((Int16)10001, (Int32)1000001, (Int64)10000001, "2008年7月3日", '1', (Single)0.0002, (Double)0.0000002, now.AddMilliseconds(1), Tuple.Create(true, myObj), TimeSpan.MaxValue);
        tupleDriverA.TestCompareTo(tupleDriverB, 0, 5);
        tupleDriverA.TestCompareTo(tupleDriverC, -1, 5);
    }

    [Fact]
    public static void TestNotEqual()
    {
        TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Boolean, TimeSpan> tupleDriverA;
        //Tuple-1
        tupleDriverA = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Boolean, TimeSpan>((Int16)10000);
        tupleDriverA.TestNotEqual();
        // This is for code coverage purposes
        //Tuple-2
        tupleDriverA = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Boolean, TimeSpan>((Int16)10000, (Int32)1000000);
        tupleDriverA.TestNotEqual();
        //Tuple-3
        tupleDriverA = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Boolean, TimeSpan>((Int16)10000, (Int32)1000000, (Int64)10000000);
        tupleDriverA.TestNotEqual();
        //Tuple-4
        tupleDriverA = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Boolean, TimeSpan>((Int16)10000, (Int32)1000000, (Int64)10000000, "2008年7月2日");
        tupleDriverA.TestNotEqual();
        //Tuple-5
        tupleDriverA = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Boolean, TimeSpan>((Int16)10000, (Int32)1000000, (Int64)10000000, "2008年7月2日", '0');
        tupleDriverA.TestNotEqual();
        //Tuple-6
        tupleDriverA = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Boolean, TimeSpan>((Int16)10000, (Int32)1000000, (Int64)10000000, "2008年7月2日", '0', Single.NaN);
        tupleDriverA.TestNotEqual();
        //Tuple-7
        tupleDriverA = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Boolean, TimeSpan>((Int16)10000, (Int32)1000000, (Int64)10000000, "2008年7月2日", '0', Single.NaN, Double.NegativeInfinity);
        tupleDriverA.TestNotEqual();
        //Tuple-8, extended tuple
        tupleDriverA = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Boolean, TimeSpan>((Int16)10000, (Int32)1000000, (Int64)10000000, "2008年7月2日", '0', Single.NaN, Double.NegativeInfinity, DateTime.Now);
        tupleDriverA.TestNotEqual();
        //Tuple-9 and Tuple-10 are not necesary because they use the same code path as Tuple-8
    }

    [Fact]
    public static void IncomparableTypes()
    {
        TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Boolean, TimeSpan> tupleDriverA;
        //Tuple-1
        tupleDriverA = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Boolean, TimeSpan>((Int16)10000);
        tupleDriverA.TestCompareToThrows();
        // This is for code coverage purposes
        //Tuple-2
        tupleDriverA = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Boolean, TimeSpan>((Int16)10000, (Int32)1000000);
        tupleDriverA.TestCompareToThrows();
        //Tuple-3
        tupleDriverA = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Boolean, TimeSpan>((Int16)10000, (Int32)1000000, (Int64)10000000);
        tupleDriverA.TestCompareToThrows();
        //Tuple-4
        tupleDriverA = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Boolean, TimeSpan>((Int16)10000, (Int32)1000000, (Int64)10000000, "2008年7月2日");
        tupleDriverA.TestCompareToThrows();
        //Tuple-5
        tupleDriverA = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Boolean, TimeSpan>((Int16)10000, (Int32)1000000, (Int64)10000000, "2008年7月2日", '0');
        tupleDriverA.TestCompareToThrows();
        //Tuple-6
        tupleDriverA = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Boolean, TimeSpan>((Int16)10000, (Int32)1000000, (Int64)10000000, "2008年7月2日", '0', Single.NaN);
        tupleDriverA.TestCompareToThrows();
        //Tuple-7
        tupleDriverA = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Boolean, TimeSpan>((Int16)10000, (Int32)1000000, (Int64)10000000, "2008年7月2日", '0', Single.NaN, Double.NegativeInfinity);
        tupleDriverA.TestCompareToThrows();
        //Tuple-8, extended tuple
        tupleDriverA = new TupleTestDriver<Int16, Int32, Int64, String, Char, Single, Double, DateTime, Boolean, TimeSpan>((Int16)10000, (Int32)1000000, (Int64)10000000, "2008年7月2日", '0', Single.NaN, Double.NegativeInfinity, DateTime.Now);
        tupleDriverA.TestCompareToThrows();
        //Tuple-9 and Tuple-10 are not necesary because they use the same code path as Tuple-8
    }

    [Fact]
    public static void FloatingPointNaNCases()
    {
        var a = Tuple.Create(Double.MinValue, Double.NaN, Single.MinValue, Single.NaN);
        var b = Tuple.Create(Double.MinValue, Double.NaN, Single.MinValue, Single.NaN);

        Assert.True(a.Equals(b));
        Assert.Equal(0, ((IComparable)a).CompareTo(b));
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
        Assert.True(((IStructuralEquatable)a).Equals(b, TestEqualityComparer.Instance));
        Assert.Equal(5, ((IStructuralComparable)a).CompareTo(b, TestComparer.Instance));
        Assert.Equal(
            ((IStructuralEquatable)a).GetHashCode(TestEqualityComparer.Instance),
            ((IStructuralEquatable)b).GetHashCode(TestEqualityComparer.Instance));
    }

    [Fact]
    public static void TestCustomTypeParameter1()
    {
        // Special case of Tuple<T1> where T1 is a custom type
        var testClass = new TestClass();
        var a = Tuple.Create(testClass);
        var b = Tuple.Create(testClass);

        Assert.True(a.Equals(b));
        Assert.Equal(0, ((IComparable)a).CompareTo(b));
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
        Assert.True(((IStructuralEquatable)a).Equals(b, TestEqualityComparer.Instance));
        Assert.Equal(5, ((IStructuralComparable)a).CompareTo(b, TestComparer.Instance));
        Assert.Equal(
            ((IStructuralEquatable)a).GetHashCode(TestEqualityComparer.Instance),
            ((IStructuralEquatable)b).GetHashCode(TestEqualityComparer.Instance));
    }

    [Fact]
    public static void TestCustomTypeParameter2()
    {
        // Special case of Tuple<T1, T2> where T2 is a custom type
        var testClass = new TestClass(1);
        var a = Tuple.Create(1, testClass);
        var b = Tuple.Create(1, testClass);

        Assert.True(a.Equals(b));
        Assert.Equal(0, ((IComparable)a).CompareTo(b));
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
        Assert.True(((IStructuralEquatable)a).Equals(b, TestEqualityComparer.Instance));
        Assert.Equal(5, ((IStructuralComparable)a).CompareTo(b, TestComparer.Instance));
        Assert.Equal(
            ((IStructuralEquatable)a).GetHashCode(TestEqualityComparer.Instance),
            ((IStructuralEquatable)b).GetHashCode(TestEqualityComparer.Instance));
    }

    [Fact]
    public static void TestCustomTypeParameter3()
    {
        // Special case of Tuple<T1, T2> where T1 and T2 are custom types
        var testClassA = new TestClass(100);
        var testClassB = new TestClass(101);
        var a = Tuple.Create(testClassA, testClassB);
        var b = Tuple.Create(testClassB, testClassA);

        Assert.False(a.Equals(b));
        Assert.Equal(-1, ((IComparable)a).CompareTo(b));
        Assert.False(((IStructuralEquatable)a).Equals(b, TestEqualityComparer.Instance));
        Assert.Equal(5, ((IStructuralComparable)a).CompareTo(b, TestComparer.Instance));
        // Equals(IEqualityComparer) is false, ignore hash code
    }

    [Fact]
    public static void TestCustomTypeParameter4()
    {
        // Special case of Tuple<T1, T2> where T1 and T2 are custom types
        var testClassA = new TestClass(100);
        var testClassB = new TestClass(101);
        var a = Tuple.Create(testClassA, testClassB);
        var b = Tuple.Create(testClassA, testClassA);

        Assert.False(a.Equals(b));
        Assert.Equal(1, ((IComparable)a).CompareTo(b));
        Assert.False(((IStructuralEquatable)a).Equals(b, TestEqualityComparer.Instance));
        Assert.Equal(5, ((IStructuralComparable)a).CompareTo(b, TestComparer.Instance));
        // Equals(IEqualityComparer) is false, ignore hash code
    }

    [Fact]
    public static void NestedTuples1()
    {
        var a = Tuple.Create(1, 2, Tuple.Create(31, 32), 4, 5, 6, 7, Tuple.Create(8, 9));
        var b = Tuple.Create(1, 2, Tuple.Create(31, 32), 4, 5, 6, 7, Tuple.Create(8, 9));

        Assert.True(a.Equals(b));
        Assert.Equal(0, ((IComparable)a).CompareTo(b));
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
        Assert.True(((IStructuralEquatable)a).Equals(b, TestEqualityComparer.Instance));
        Assert.Equal(5, ((IStructuralComparable)a).CompareTo(b, TestComparer.Instance));
        Assert.Equal(
            ((IStructuralEquatable)a).GetHashCode(TestEqualityComparer.Instance),
            ((IStructuralEquatable)b).GetHashCode(TestEqualityComparer.Instance));
        Assert.Equal("(1, 2, (31, 32), 4, 5, 6, 7, (8, 9))", a.ToString());
        Assert.Equal("(31, 32)", a.Item3.ToString());
        Assert.Equal("((8, 9))", a.Rest.ToString());
    }

    [Fact]
    public static void NestedTuples2()
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
    public static void IncomparableTypesSpecialCase()
    {
        // Special case when T does not implement IComparable
        var testClassA = new TestClass2(100);
        var testClassB = new TestClass2(100);
        var a = Tuple.Create(testClassA);
        var b = Tuple.Create(testClassB);

        Assert.True(a.Equals(b));
        Assert.Throws<ArgumentException>(() => ((IComparable)a).CompareTo(b));
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
        Assert.True(((IStructuralEquatable)a).Equals(b, TestEqualityComparer.Instance));
        Assert.Equal(5, ((IStructuralComparable)a).CompareTo(b, TestComparer.Instance));
        Assert.Equal(
            ((IStructuralEquatable)a).GetHashCode(TestEqualityComparer.Instance),
            ((IStructuralEquatable)b).GetHashCode(TestEqualityComparer.Instance));
        Assert.Equal("([100])", a.ToString());
    }

    private class TestClass : IComparable
    {
        private readonly int _value;

        internal TestClass()
            : this(0)
        { }

        internal TestClass(int value)
        {
            this._value = value;
        }

        public override string ToString()
        {
            return "{" + _value.ToString() + "}";
        }

        public int CompareTo(object x)
        {
            TestClass tmp = x as TestClass;
            if (tmp != null)
                return this._value.CompareTo(tmp._value);
            else
                return 1;
        }
    }

    private class TestClass2
    {
        private readonly int _value;

        internal TestClass2()
            : this(0)
        { }

        internal TestClass2(int value)
        {
            this._value = value;
        }

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
