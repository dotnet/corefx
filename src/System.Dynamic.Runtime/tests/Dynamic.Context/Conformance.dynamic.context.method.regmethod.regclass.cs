// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Xunit;
using ManagedTests.DynamicCSharp.Conformance.dynamic.context.method.regmethod.regclass.regclassregmeth.regclassregmeth;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using System.Linq;

namespace ManagedTests.DynamicCSharp.Conformance.dynamic.context.method.regmethod.regclass.regclassregmeth.regclassregmeth
{
    public class MyClass
    {
        public int Field = 0;
    }

    public struct MyStruct
    {
        public int Number;
    }

    public enum MyEnum
    {
        First = 1,
        Second = 2,
        Third = 3
    }

    public class MemberClass
    {
        public bool Method_ReturnBool() => false;

        public bool Method_ReturnBool(object o) => o == null ? true : false;

        public bool Method_ReturnBool(out byte? p1, ref ulong? p2, out int p3)
        {
            p1 = 3;
            p2 = null;
            p3 = 1;
            return false;
        }

        public bool Method_ReturnBool(params object[] o) => false;

        public bool? Method_ReturnBoolNullable() => false;

        public bool? Method_ReturnBoolNullable(MyEnum[] p1, out decimal?[] p2, params int?[] p3)
        {
            p2 = new decimal?[2];
            return true;
        }

        public bool? Method_ReturnBoolNullable(params short?[] s) => false;

        public bool? Method_ReturnBoolNullable(short? s) => s == null ? null : (bool?)true;

        public byte?[] Method_ReturnByteArrNullable()
        {
            byte?[] arr = new byte?[10];
            for (byte i = 0; i < 10; i++)
                arr[i] = i;
            return arr;
        }

        public byte?[] Method_ReturnByteArrNullable(MyStruct? m)
        {
            byte?[] arr = new byte?[10];
            for (byte i = 0; i < 10; i++)
                arr[i] = (byte)m.Value.Number;
            return arr;
        }

        public byte?[] Method_ReturnByteArrNullable(out MyStruct? m)
        {
            m = new MyStruct() { Number = 2 };

            byte?[] arr = new byte?[10];
            for (byte i = 0; i < 10; i++)
                arr[i] = (byte)m.Value.Number;
            return arr;
        }

        public byte?[] Method_ReturnByteArrNullable(out string p1, ref ulong? p2, ref byte?[] p3)
        {
            p1 = "one";
            p2 = null;
            byte?[] arr = new byte?[10];
            for (byte i = 0; i < 10; i++)
                arr[i] = 2;
            return arr;
        }

        public byte?[] Method_ReturnByteArrNullable(params MyStruct?[] m)
        {
            byte?[] arr = new byte?[10];
            for (byte i = 0; i < 10; i++)
                arr[i] = i;
            return arr;
        }

        public byte[] Method_ReturnByteArr()
        {
            byte[] arr = new byte[10];
            for (byte i = 0; i < 10; i++)
                arr[i] = i;
            return arr;
        }

        public byte[] Method_ReturnByteArr(MyClass[] d)
        {
            byte[] arr = new byte[d.Length];
            for (byte i = 0; i < d.Length; i++)
                arr[i] = 5;
            return arr;
        }

        public byte[] Method_ReturnByteArr(out short?[] p1, ref bool?[] p2, ref MyEnum p3)
        {
            p1 = new short?[2];
            p1[0] = 1;
            p1[1] = null;
            p2[0] = null;
            p3 = MyEnum.Third;
            byte[] arr = new byte[2];
            for (byte i = 0; i < 2; i++)
                arr[i] = (byte)(p1[i] ?? 0);
            return arr;
        }

        public byte[] Method_ReturnByteArr(params int[] arr)
        {
            byte[] arr2 = new byte[arr.Length];
            for (byte i = 0; i < arr.Length; i++)
                arr2[i] = (byte)arr[i];
            return arr2;
        }

        public char Method_ReturnChar(MyStruct m) =>'s';

        public char Method_ReturnChar(params MyStruct[] m) => 'b';

        public char Method_ReturnChar(ref MyStruct m)
        {
            m.Number = 2;
            return '2';
        }

        public char Method_ReturnChar(ref string p1, object p2, ulong[] p3)
        {
            p1 = p1.ToLower();
            return 'c';
        }

        public char? Method_ReturnCharNullable() => 'a';

        public char? Method_ReturnCharNullable(decimal[] p1, ref object p2, ref char p3)
        {
            p2 = new MyClass() { Field = 3 };
            p3 = 'b';
            return p3;
        }

        public char? Method_ReturnCharNullable(MyClass m) => m.ToString()[0];

        public char? Method_ReturnCharNullable(out MyClass m)
        {
            m = new MyClass() { Field = 5 };
            return m.ToString()[0];
        }

        public char? Method_ReturnCharNullable(params MyClass[] m) => 'z';

        public decimal Method_ReturnDecimal() => 1M;

        public decimal Method_ReturnDecimal(int? i) => (decimal)i;

        public decimal Method_ReturnDecimal(out ulong p1, ref MyEnum p2, params char?[] p3)
        {
            p1 = (ulong)p3.Length;
            p2 = MyEnum.Third;
            return 1;
        }

        public decimal Method_ReturnDecimal(params int?[] i) => 1M;

        public decimal? Method_ReturnDecimalNullable() => 1M;

        public decimal? Method_ReturnDecimalNullable(int i) => i;

        public decimal? Method_ReturnDecimalNullable(params int[] i) => 1M;

        public decimal? Method_ReturnDecimalNullable(ref int? p1, ref short? p2, out MyStruct[] p3)
        {
            p1 = null;
            p2 = null;
            p3 = new MyStruct[2];
            return 3m;
        }

        public dynamic Method_ReturnDynamic()
        {
            int x = 3;
            return x;
        }

        public dynamic Method_ReturnDynamic(ref int x) => x;

        public dynamic Method_ReturnDynamic(out float x)
        {
            x = 3.5f;
            return x;
        }

        public dynamic Method_ReturnDynamic(params dynamic[] d) => d.Length;

        public dynamic Method_ReturnDynamic(dynamic d, int x, ref dynamic e) => x;

        public float Method_ReturnFloat() => 3.4534f;

        public float Method_ReturnFloat(int x) => x;

        public float Method_ReturnFloat(params float[] p1) => p1[0];

        public float Method_ReturnFloat(ref float p1, out float p2, params float[] p3)
        {
            p1 = 0.000003f;
            p2 = 2342424;
            return p3.Length;
        }

        public float? Method_ReturnFloatNullable() => 3.4534f;

        public float? Method_ReturnFloatNullable(int x) => x;

        public float? Method_ReturnFloatNullable(params float[] p1) => p1[0];

        public float? Method_ReturnFloatNullable(ref float? p1, out float p2, params float?[] p3)
        {
            p1 = 0.000003f;
            p2 = 2342424;
            return p3.Length;
        }

        public float?[] Method_ReturnFloatArrNullable() => new float?[] { 3.4534f };

        public float?[] Method_ReturnFloatArrNullable(int x) => new float?[] { x };

        public float?[] Method_ReturnFloatArrNullable(params float[] p1) => new float?[] { p1[0] };

        public float?[] Method_ReturnFloatArrNullable(ref float p1, out float p2, params float[] p3)
        {
            p1 = 0.000003f;
            p2 = 2342424;
            return new float?[] { p3.Length };
        }

        public float[] Method_ReturnFloatArr() => new float[] { 3.4534f };

        public float[] Method_ReturnFloatArr(int x) => new float[] { x };

        public float[] Method_ReturnFloatArr(params float[] p1) => new float[] { p1[0] };

        public float[] Method_ReturnFloatArr(ref float p1, out float p2, params float[] p3)
        {
            p1 = 0.000003f;
            p2 = 2342424;
            return new float[] { p3.Length };
        }

        public int Method_ReturnInt(decimal[] p1, char[] p2, byte p3) => 5;

        public int Method_ReturnInt(params string[] s) => 1;

        public int Method_ReturnInt(ref string s)
        {
            s = s.ToUpper();
            return s.Length;
        }

        public int Method_ReturnInt(string s) => s.Length;

        public int? Method_ReturnIntNullable() => 1;

        public int? Method_ReturnIntNullable(bool b) => null;

        public int? Method_ReturnIntNullable(out MyClass p1, ref decimal p2, char p3)
        {
            p1 = new MyClass() { Field = 3 };
            p2 = 3m;
            return p3;
        }

        public int? Method_ReturnIntNullable(params bool[] b) => 1;

        public int? Method_ReturnIntNullable(ref bool b)
        {
            b = true;
            return b == true ? (int?)1 : null;
        }

        public MyClass Method_ReturnMyClass() => new MyClass() { Field = 1 };

        public MyClass Method_ReturnMyClass(out int? p1, short? p2, out byte p3)
        {
            p1 = 2;
            p3 = 3;
            MyClass m = new MyClass() { Field = (int)p2 };
            return m;
        }

        public MyClass Method_ReturnMyClass(out ulong l)
        {
            l = 3;
            MyClass m = new MyClass() { Field = (int)l };
            return m;
        }

        public MyClass Method_ReturnMyClass(params MyStruct[] arr) => new MyClass() { Field = 1 };

        public MyClass Method_ReturnMyClass(ulong l) => new MyClass() { Field = (int)l };

        public MyClass Method_ReturnMyClass_Throw()
        {
            throw new ArithmeticException("Test exception");
        }

        public MyClass Method_ReturnMyClass_Throw(bool? b)
        {
            throw new ArgumentException("Test exception", nameof(b));
        }

        public MyClass Method_ReturnMyClass_Throw(out MyClass p1, out decimal?[] p2, MyEnum p3)
        {
            throw new ArgumentException("Test exception", "b");
        }

        public MyClass Method_ReturnMyClass_Throw(params byte?[] arr)
        {
            throw new ArithmeticException("Test exception");
        }

        public MyEnum Method_ReturnMyEnum() => MyEnum.First;

        public MyEnum Method_ReturnMyEnum(out bool[] p1, int[] p2, out bool? p3)
        {
            p1 = new bool[] { true, false };
            p3 = null;
            MyEnum m = new MyEnum();
            m = MyEnum.First;
            return MyEnum.First;
        }

        public MyEnum Method_ReturnMyEnum(params short[] s) => MyEnum.Second;

        public MyEnum Method_ReturnMyEnum(short s) => (MyEnum)s;

        public MyEnum? Method_ReturnMyEnumNullable() => MyEnum.First;

        public MyEnum? Method_ReturnMyEnumNullable(int[] arr) => MyEnum.Second;

        public MyEnum? Method_ReturnMyEnumNullable(params MyClass[] d) => MyEnum.Third;

        public MyEnum? Method_ReturnMyEnumNullable(ref bool[] p1, bool p2, out short[] p3)
        {
            p1 = new bool[2];
            p3 = new short[3];
            return MyEnum.First;
        }

        public MyStruct Method_ReturnMyStruct() => new MyStruct() { Number = 3 };

        public MyStruct Method_ReturnMyStruct(params string[] s) => new MyStruct() { Number = 3 };

        public MyStruct Method_ReturnMyStruct(short p1, out bool p2, params MyClass[] p3)
        {
            p2 = false;
            return new MyStruct() { Number = p3.Length };
        }

        public MyStruct Method_ReturnMyStruct(string s) => new MyStruct { Number = s.Length };

        public MyStruct? Method_ReturnMyStructNullable() => new MyStruct() { Number = 3 };

        public MyStruct? Method_ReturnMyStructNullable(object o) => new MyStruct() { Number = 3 };

        public MyStruct? Method_ReturnMyStructNullable(out byte? p1, out string[] p2, params ulong[] p3)
        {
            p1 = 3;
            p2 = new string[3];
            return new MyStruct() { Number = p3.Length };
        }

        public MyStruct? Method_ReturnMyStructNullable(params object[] o) => new MyStruct() { Number = o.Length };

        public object Method_ReturnObject() => null;

        public object Method_ReturnObject(byte?[] arr) => null;

        public object Method_ReturnObject(MyEnum[] p1, int p2, ref char?[] p3)
        {
            p3 = new char?[] { 'a', 'b' };
            return null;
        }

        public object Method_ReturnObject(params bool?[] b) => null;

        public short Method_ReturnShort_Throw()
        {
            throw new ArithmeticException("Test message");
        }

        public short Method_ReturnShort_Throw(MyEnum m)
        {
            throw new ArgumentException("Test message", nameof(m));
        }

        public short Method_ReturnShort_Throw(out char? p1, ref string[] p2, short[] p3)
        {
            throw new ArgumentException("Test message", "m");
        }

        public short Method_ReturnShort_Throw(out MyEnum m)
        {
            m = MyEnum.Second;
            throw new ArgumentException("Test message", nameof(m));
        }

        public short Method_ReturnShort_Throw(params MyEnum[] m)
        {
            throw new ArithmeticException("Test message");
        }

        public short? Method_ReturnShort_ThrowNullable()
        {
            throw new ArithmeticException("Test message");
        }

        public short? Method_ReturnShort_ThrowNullable(decimal? d)
        {
            throw new ArgumentException("Test message", nameof(d));
        }

        public short? Method_ReturnShort_ThrowNullable(out char? p1, int[] p2, params bool?[] p3)
        {
            throw new ArgumentException("Test message", "d");
        }

        public short? Method_ReturnShort_ThrowNullable(params decimal?[] d)
        {
            throw new ArithmeticException("Test message");
        }

        public short? Method_ReturnShort_ThrowNullable(ref decimal? d)
        {
            d = 3m;
            throw new ArgumentException("Test message", nameof(d));
        }

        public sbyte Method_ReturnSbyte(int x, dynamic d, short s, decimal dd, MyClass c, float f, short? ss) => 1;

        public string Method_ReturnString() => "string";

        public string Method_ReturnString(char c) => c.ToString();

        public string Method_ReturnString(out ulong?[] p1, out byte[] p2, out int?[] p3)
        {
            p1 = new ulong?[2];
            p2 = new byte[2];
            p3 = new int?[3];
            return "c";
        }

        public string Method_ReturnString(params char[] c) => "string";

        public string[] Method_ReturnStringArr()
        {
            string[] arr = new string[10];
            for (int i = 0; i < 10; i++)
                arr[i] = i.ToString();
            return arr;
        }

        public string[] Method_ReturnStringArr(MyEnum? m)
        {
            string[] arr = new string[10];
            for (int i = 0; i < 10; i++)
                arr[i] = i.ToString();
            return arr;
        }

        public string[] Method_ReturnStringArr(params MyEnum?[] m)
        {
            string[] arr = new string[10];
            for (int i = 0; i < 10; i++)
                arr[i] = i.ToString();
            return arr;
        }

        public string[] Method_ReturnStringArr(ref MyEnum? m)
        {
            string[] arr = new string[10];
            for (int i = 0; i < 10; i++)
                arr[i] = m.ToString();
            m = null;
            return arr;
        }

        public string[] Method_ReturnStringArr(short?[] p1, ref MyStruct p2, out MyStruct[] p3)
        {
            p2 = new MyStruct() { Number = 3 };
            p3 = new MyStruct[2];
            p3[0].Number = 1;
            p3[1].Number = 2;
            string[] arr = new string[10];
            for (int i = 0; i < 10; i++)
                arr[i] = p2.ToString();
            return arr;
        }

        public ulong Method_ReturnUlong() => 1L;

        public ulong Method_ReturnUlong(MyClass m) => (ulong)m.ToString().Length;

        public ulong Method_ReturnUlong(out MyClass m)
        {
            m = new MyClass();
            return (ulong)m.ToString().Length;
        }

        public ulong Method_ReturnUlong(params MyClass[] m) => 1L;

        public ulong Method_ReturnUlong(ref ulong?[] p1, out MyStruct p2, ref byte?[] p3)
        {
            p1[0] = 3;
            p2 = new MyStruct() { Number = 3 };
            p3[0] = 1;
            return 1;
        }

        public ulong? Method_ReturnUlongNullable() => 1L;

        public ulong? Method_ReturnUlongNullable(long c)
        {
            return c == 0 ? (ulong?)1L : null;
        }

        public ulong? Method_ReturnUlongNullable(out object[] p1, out byte[] p2, out decimal? p3)
        {
            p1 = new object[2];
            p2 = new byte[2];
            p3 = 4m;
            return 1L;
        }

        public ulong? Method_ReturnUlongNullable(params char?[] c) => 1L;

        public void Method_ReturnVoid() { }

        public void Method_ReturnVoid(MyStruct[] arr) { }

        public void Method_ReturnVoid(params ulong[] l) { }

        public void Method_ReturnVoid(ref object[] p1, out bool?[] p2, ref MyClass[] p3)
        {
            p1 = null;
            p3 = null;
            p2 = new bool?[] { false };
            return;
        }

        public static bool StaticMethod_ReturnBool() => false;
    }
}

namespace Dynamic.RegularClass.RegularMethod.Tests
{
    public class RegularClassRegularMethodTests
    {
        public static bool TestMethod(object o)
        {
            dynamic d = new MemberClass();
            return (bool)d.Method_ReturnBool(o, null);
        }

        [Fact]
        public static void StaticMethodBody1()
        {
            Assert.False(TestMethod(new object()));
        }

        [Fact]
        public static void ExtensionMethodBody()
        {
            RegularClassRegularMethodTests t = new RegularClassRegularMethodTests();
            Assert.False(t.TestMethod());
        }

        [Fact]
        public static void CollectionIntializer()
        {
            dynamic d = new MemberClass();
            object p2 = null;
            char p3 = 'a';
            decimal[] dec = new decimal[] { 1 };
            var list = new List<char?> { (char?)d.Method_ReturnCharNullable(), (char?)d.Method_ReturnCharNullable(dec, ref p2, ref p3) };

            Assert.Equal(new List<char?> { 'a', 'b' }, list);
            Assert.Equal('b', p3);
            Assert.IsType<MyClass>(p2);
            Assert.Equal(3, ((MyClass)p2).Field);
        }

        [Fact]
        public static void CheckedExpression()
        {
            dynamic d = new MemberClass();
            string s = "a";
            int m1 = checked((int)d.Method_ReturnInt(null, null) + 1);
            int m2 = unchecked((int)d.Method_ReturnInt(s) + int.MaxValue);
            Assert.Equal(2, m1);
            Assert.Equal(-2147483648, m2);
        }

        [Fact]
        public static void TryCatchFinally()
        {
            int? result1 = null;
            decimal result2 = 0;
            dynamic d = new MemberClass();
            try
            {
                d.Method_ReturnMyClass_Throw();
            }
            catch (ArithmeticException e)
            {
                result1 = (int?)d.Method_ReturnIntNullable();
            }
            finally
            {
                result2 = (decimal)d.Method_ReturnDecimal();
            }

            Assert.Equal(result2, result1.Value);
        }

        [Fact]
        public static void ThisArgumentOfExtensionMethod()
        {
            dynamic d = new MemberClass();
            Assert.Equal(6, ((int)d.Method_ReturnInt(null, new char[] { 'a' }, 0)).ExPlus());
        }

        [Fact]
        public static void ThrowExpression()
        {
            dynamic d = new MemberClass();
            bool testSucceeded = false;
            try
            {
                throw new ArithmeticException((string)d.Method_ReturnString());
            }
            catch (ArithmeticException e)
            {
                testSucceeded = e.Message == "string";
            }
            Assert.True(testSucceeded);
        }

        [Fact]
        public static void UsingExpression()
        {
            dynamic d = new MemberClass();
            string result = null;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (StreamWriter streamWriter = new StreamWriter(memoryStream))
                {
                    streamWriter.WriteLine(d.Method_ReturnString('a'));
                    streamWriter.Flush();
                    memoryStream.Position = 0;
                    using (StreamReader sr = new StreamReader(memoryStream, (bool)d.Method_ReturnBool()))
                    {
                        result = sr.ReadToEnd();
                    }
                }
            }

            Assert.Equal("a", result.Trim());
        }

        [Fact]
        public static void LockExpression()
        {
            dynamic d = new MemberClass();
            dynamic result = true;
            object locker;
            lock (locker = d.Method_ReturnString(new char[] { 'a', 'b' }))
            {
                result = MemberClass.StaticMethod_ReturnBool();
            }
            Assert.False((bool)result);
        }

        [Fact]
        public static void ForLoop1()
        {
            dynamic d = new MemberClass();
            sbyte result = 0;
            for (ulong i = (ulong)d.Method_ReturnUlong(); i < (ulong)d.Method_ReturnUlong(new MyClass()); i++)
            {
                result += (sbyte)d.Method_ReturnSbyte((int)i, d, (short)i, (decimal)i, new MyClass(), (float)i, (short?)i);
            }
            Assert.Equal(119, result); // length of full name of MyClass.
        }

        [Fact]
        public static void ForLoop2()
        {
            dynamic d1 = new MemberClass();
            sbyte result = 0;
            dynamic d2 = new MyClass();
            for (ulong i = d1.Method_ReturnUlong(); i < d1.Method_ReturnUlong(d2); i++)
            {
                result += d1.Method_ReturnSbyte((int)i, d1, (short)i, (decimal)i, d2, (float)i, (short?)i);
            }
            Assert.Equal(119, result); // length of full name of MyClass.
        }

        [Fact]
        public static void DefaultSwitchStatement()
        {
            dynamic d = new MemberClass();
            char c = '0';
            switch (c)
            {
                case '1':
                    break;
                default:
                    c = (char)d.Method_ReturnChar(new MyStruct[] { new MyStruct() { Number = 0 }, new MyStruct() { Number = 1 } });
                    break;
            }
            Assert.Equal('b', c);
        }

        [Fact]
        public static void TryCatchFinally_TwoDynamicParameters()
        {
            dynamic d = new MemberClass();
            int result = -1;
            try
            {
                Func<short, short, short, short> func = delegate (short x, short y, short z)
                {
                    return (short)d.Method_ReturnShort_Throw((MyEnum)d.Method_ReturnMyEnum(), d.Method_ReturnMyEnum(x, (short)y), d.Method_ReturnMyEnum((short)z));
                };
                result = func(1, 2, 3);
            }
            catch (ArithmeticException e)
            {
                Assert.Equal("Test message", e.Message);
            }
            finally
            {
                result = (short)d.Method_ReturnUlong(new MyClass()) - 1;
            }

            Assert.Equal(119, result);
        }

        [Fact]
        public static void AnonymousType_InsideQueryExpression_IntroducesDynamicVariables()
        {
            List<int> list = new List<int>() { 0, 4, 1, 6, 4, 4, 5 };
            string s = "test";
            dynamic d = new MemberClass();
            var result = list.Where(p => p == (int)d.Method_ReturnInt(ref s)).Select(p => new
            {
                A = d.Method_ReturnDecimalNullable(p)
            }).ToList();
            Assert.Equal("TEST", s);
            Assert.Equal(3, result.Count);

            foreach (var m in result)
            {
                Assert.Equal(4, (decimal)m.A);
            }
        }

        [Fact]
        public static void Foreach_InsideUsingStatement_UsingDynamicIntroducedByTheUsingStatement()
        {
            int[] array = new int[] { 1, 2 };
            MemoryStream ms = new MemoryStream(new byte[] { 84, 101, 115, 116 });

            dynamic d = new MemberClass();
            string result = string.Empty;
            byte? p1 = null;
            ulong? p2 = 0;
            int p3 = 10;
            using (dynamic sr = new StreamReader(ms, (bool)d.Method_ReturnBool(out p1, ref p2, out p3)))
            {
                foreach (int s in array)
                {
                    ms.Position = 0;
                    string m = ((StreamReader)sr).ReadToEnd();
                    result += m + s.ToString();
                }
            }
            Assert.Equal((byte)3, p1);
            Assert.Null(p2);
            Assert.Equal(1, p3);
            Assert.Equal("Test1Test2", result);
        }

        [Fact]
        public static void StaticMethodBody2()
        {
            dynamic d = new MemberClass();
            decimal?[] p2 = null;
            var result = (bool?)d.Method_ReturnBoolNullable(null, out p2, null, 0);
            Assert.Equal(2, p2.Length);
            Assert.True(result.Value);
        }

        [Fact]
        public static void StaticMethodBody3()
        {
            dynamic d = new MemberClass();
            short? p = null;
            var result = (bool?)d.Method_ReturnBoolNullable(p);
            Assert.Null(result);
            p = 0;
            result = (bool?)d.Method_ReturnBoolNullable(p);
            Assert.True(result);
        }

        private static void IsEqual(byte?[] input)
        {
            Assert.Equal(new byte?[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, input);
        }

        [Fact]
        public static void MethodArgument_Regular()
        {
            dynamic d = new MemberClass();
            IsEqual((byte?[])d.Method_ReturnByteArrNullable());
        }

        private static void IsEqual<T>(byte?[] input)
        {
            Assert.Equal(new byte?[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, input);
        }

        [Fact]
        public static void MethodArgument_Generic()
        {
            dynamic d = new MemberClass();
            IsEqual<RegularClassRegularMethodTests>((byte?[])d.Method_ReturnByteArrNullable(new MyStruct?[] { new MyStruct() { Number = 10 } }));
        }

        [Fact]
        public static void StaticMethodBody4()
        {
            dynamic d = new MemberClass();
            MyStruct? m = new MyStruct { Number = 10 };

            byte?[] result = (byte?[])d.Method_ReturnByteArrNullable(out m);
            Assert.Equal(2, m.Value.Number);
            Assert.Equal(new byte?[] { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 }, result);
        }

        public static void StaticGenericMethod<T>()
        {
            dynamic d = new MemberClass();
            string p1 = null;
            ulong? p2 = 20;
            byte?[] p3 = null;
            byte?[] result = (byte?[])d.Method_ReturnByteArrNullable(out p1, ref p2, ref p3);
            Assert.Equal("one", p1);
            Assert.Null(p2);
            Assert.Equal(new byte?[] { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 }, result);
        }

        [Fact]
        public static void StaticMethodBody_Generic()
        {
            StaticGenericMethod<MyEnum>();
        }

        [Fact]
        public static void ImplicitlyTypedArrayInitializer1()
        {
            string p1 = "Test";
            dynamic d = new MemberClass();
            var array = new char[] { (char)d.Method_ReturnChar(ref p1, null, null), (char)d.Method_ReturnChar(ref p1, 1, null) };
            Assert.Equal("test", p1);
            Assert.Equal(new char[] { 'c', 'c' }, array);
        }

        [Fact]
        public static void EqualsOperator1()
        {
            dynamic d = new MemberClass();
            MyClass myClass = new MyClass() { Field = 10 };
            bool b = ((char)d.Method_ReturnCharNullable(myClass) == (char)d.Method_ReturnCharNullable(out myClass));
            Assert.True(b);
            Assert.Equal(5, myClass.Field);
        }

        [Fact]
        public static void StaticMethodBody5()
        {
            MyClass myclass0 = new MyClass() { Field = 0 };
            dynamic myclass1 = new MyClass() { Field = 1 };
            MyClass myclass2 = new MyClass() { Field = 2 };
            dynamic d = new MemberClass();

            string s = ((object)d.Method_ReturnCharNullable(new MyClass[] { myclass0, (MyClass)myclass1, myclass2 })).ToString();
            Assert.Equal("z", s);
        }

        [Fact]
        public static void StaticMethodBody6()
        {
            int? p1 = 10;
            short? p2 = 20;
            MyStruct[] p3 = new MyStruct[1];
            dynamic d = new MemberClass();

            decimal s = (decimal)d.Method_ReturnDecimalNullable(ref p1, ref p2, out p3);
            Assert.Null(p1);
            Assert.Null(p2);
            Assert.Equal(2, p3.Length);
            Assert.Equal(3M, s);
        }

        [Fact]
        public static void LockBlock()
        {
            dynamic d = new MemberClass();
            int input = int.MinValue;
            float? result;
            lock (d.Method_ReturnFloatNullable())
            {
                result = (float?)d.Method_ReturnFloatNullable(input);
            }

            Assert.Equal(int.MinValue, result);
        }

        [Fact]
        public static void ForLoopBody()
        {
            float[] array1 = new float[] { 1f, 24.333f, 3.44f };
            float?[] array2 = new float?[3];
            for (int i = 0; i < array1.Length; i++)
            {
                dynamic dy = new MemberClass();
                array2[i] = dy.Method_ReturnFloatNullable(array1[i], array1[0], array1[1], array1[2]);
            }

            Assert.Equal(new float?[] { array1[0], array1[1], array1[2] }, array2);
        }

        [Fact]
        public static void VariableNamedDynamic()
        {
            float? p1 = null;
            float p2 = 1.234f;
            float? myFloat = null;
            dynamic dynamic = new MemberClass();

            float? result = (float?)dynamic.Method_ReturnFloatNullable(ref p1, out p2, p1, p2, myFloat);
            Assert.Equal(3f, result);
            Assert.Equal(0.000003f, p1);
            Assert.Equal(2342424, p2);
        }

        [Fact]
        public static void ForeachExpression1()
        {
            dynamic d = new MemberClass();
            List<float?> list = new List<float?>();
            foreach (var f in (float?[])d.Method_ReturnFloatArrNullable())
            {
                list.Add(f);
            }
            Assert.Equal(new List<float?> { 3.4534f }, list);
        }

        [Fact]
        public static void GreaterThanOperator()
        {
            dynamic d = new MemberClass();
            int intValue = 10;
            float floatValue = 9.9f;
            Assert.True(((float?[])d.Method_ReturnFloatArrNullable(intValue))[0] >= ((float?[])d.Method_ReturnFloatArrNullable(floatValue, intValue))[0]);
        }

        [Fact]
        public static void StaticMethodBody7()
        {
            float p1 = 1f;
            float p2 = 2f;
            float myFloat = 0.111f;
            dynamic dynamic = new MemberClass();
            float?[] result = (float?[])dynamic.Method_ReturnFloatArrNullable(ref p1, out p2, p2, myFloat);
            Assert.Equal(new float?[] { 2f }, result);
            Assert.Equal(0.000003f, p1);
            Assert.Equal(2342424, p2);
        }

        private static void StaticGenericMethodWithwoParametersT<U, V>()
        {
            dynamic dy = new MemberClass();
            float[] result = (float[])dy.Method_ReturnFloatArr();
            Assert.Equal(new float[] { 3.4534f }, result);
        }

        [Fact]
        public static void StaticMethodBody_GenericMethod_TwoParameters()
        {
            StaticGenericMethodWithwoParametersT<int, RegularClassRegularMethodTests>();
        }

        [Fact]
        public static void DoWhleExpression()
        {
            dynamic d = new MemberClass();
            int value = 0;
            do
            {
                ;
            }
            while (((float[])d.Method_ReturnFloatArr(value++))[0] < 10);
            Assert.Equal(11, value);
        }

        private void RegularGenericMethod<T>(float value1, float value2, float value3)
        {
            dynamic d = new MemberClass();
            float[] result = (float[])d.Method_ReturnFloatArr(value1, value2, value3);
            Assert.Equal(new float[] { value1 }, result);
        }

        [Fact]
        public static void RegularMethodBody_GenericMethod()
        {
            RegularClassRegularMethodTests t = new RegularClassRegularMethodTests();
            t.RegularGenericMethod<RegularClassRegularMethodTests>(float.MaxValue, float.NegativeInfinity, float.PositiveInfinity);
        }

        private void RegularMethod(ref float p1, out float p2, params float[] p3)
        {
            dynamic dy = new MemberClass();
            float[] result = (float[])dy.Method_ReturnFloatArr(ref p1, out p2, p3);
            Assert.Equal(new float[] { 2 }, result);
        }

        [Fact]
        public static void RegularMethodBody()
        {
            float p1 = 1f;
            float p2 = 2f;
            new RegularClassRegularMethodTests().RegularMethod(ref p1, out p2, p1, p2);
            Assert.Equal(0.000003f, p1);
            Assert.Equal(2342424, p2);
        }

        [Fact]
        public static void ArgumentOfExtensionMethod()
        {
            dynamic d = new MemberClass();
            Assert.True(((int?)d.Method_ReturnIntNullable(true)).IsNull());
        }

        public void RegularMethod_NullableInt(int? value)
        {
            Assert.Equal('a', value);
        }

        [Fact]
        public static void RegularMethodArgument()
        {
            dynamic d = new MemberClass();
            MyClass myclass = new MyClass();
            decimal p2 = -1M;
            char p3 = 'a';
            new RegularClassRegularMethodTests().RegularMethod_NullableInt((int?)d.Method_ReturnIntNullable(out myclass, ref p2, p3));
        }

        public static void StaticMethod_NullableInt(int? value)
        {
            Assert.Equal(1, value);
        }

        [Fact]
        public static void StaticMethodArgument()
        {
            dynamic d = new MemberClass();
            bool value = true;
            StaticMethod_NullableInt((int?)d.Method_ReturnIntNullable(true, false, value));
        }
        
        public static void StaticGenericMethod_NullableInt<T>(int? value)
        {
            Assert.Equal(1, value);
        }

        [Fact]
        public static void StaticMethodArgument_Generic()
        {
            dynamic d = new MemberClass();
            bool value = false;
            StaticGenericMethod_NullableInt<bool>((int?)d.Method_ReturnIntNullable(ref value));
            Assert.True(value);
        }

        [Fact]
        public static void StaticMethodBody8()
        {
            dynamic d = new MemberClass();
            dynamic myS = new MyStruct() { Number = 10 };
            MyClass result = (MyClass)d.Method_ReturnMyClass(new MyStruct(), (MyStruct)myS);
            Assert.Equal(1, result.Field);
        }

        [Fact]
        public static void TryExpression1()
        {
            bool testSucceeded = false;
            try
            {
                dynamic dy = new MemberClass();
                bool? value1 = true;
                dynamic dvalue1 = value1;
                MyClass result = (MyClass)dy.Method_ReturnMyClass_Throw((bool?)dvalue1);
            }
            catch (ArgumentException ae)
            {
                testSucceeded = ae.Message.Contains("Test exception");
            }
            Assert.True(testSucceeded);
        }

        [Fact]
        public static void TryExpression2()
        {
            MyClass p1 = new MyClass() { Field = 1 };
            decimal?[] p2 = new decimal?[] { null };

            bool testSucceeded = false;
            MyEnum p3 = MyEnum.Third;
            try
            {
                dynamic dy = new MemberClass();
                MyClass result = (MyClass)dy.Method_ReturnMyClass_Throw(out p1, out p2, p3);
            }
            catch (ArgumentException ae)
            {
                testSucceeded = ae.Message.Contains("Test exception");
                Assert.Equal(1, p1.Field);
                Assert.Equal(new decimal?[] { null }, p2);
            }
            Assert.True(testSucceeded);
        }

        [Fact]
        public static void WhileBody()
        {
            bool value = true;
            bool testSucceeded = false;
            try
            {
                dynamic d = new MemberClass();
                while (value)
                {
                    MyClass mc = d.Method_ReturnMyClass_Throw(new byte?[0]);
                }
            }
            catch (ArithmeticException ae)
            {
                testSucceeded = ae.Message == ("Test exception");
            }
            Assert.True(testSucceeded);
        }

        [Fact]
        public static void StaticMethodBody9()
        {
            bool[] p1 = null;
            int[] p2 = new int[2];
            bool? p3 = true;
            dynamic d = new MemberClass();
            MyEnum result = d.Method_ReturnMyEnum(out p1, p2, out p3);
            Assert.Equal(new bool[] { true, false }, p1);
            Assert.Null(p3);
            Assert.Equal(MyEnum.First, result);
        }

        [Fact]
        public static void ForLoopInitializer()
        {
            dynamic d = new MemberClass();
            int index = 0;
            int expected = 1;
            for (int i = (int)d.Method_ReturnMyEnumNullable(); i <= (int)d.Method_ReturnMyEnumNullable(new int[0]); i++)
            {
                Assert.Equal(expected, i);
                expected++;
                index = i;
            }
            Assert.Equal(2, index);
        }

        [Fact]
        public static void DoWhileBody1()
        {
            dynamic d = new MemberClass();
            int result = 0;
            int index = 0;
            do
            {
                result += (int)d.Method_ReturnMyEnumNullable(new MyClass[0]);
            }
            while (index++ < 2);
            Assert.Equal(9, result);
        }

        [Fact]
        public static void DoWhileBody2()
        {
            dynamic d = new MemberClass();
            dynamic result = 0;
            dynamic index = 0;
            do
            {
                result += ((MyEnum?)d.Method_ReturnMyEnumNullable(new MyClass[0])).Value.GetHashCode();
            }
            while (index++ < 2);
            Assert.Equal(9, result);
        }

        [Fact]
        public static void StaticMethodBody10()
        {
            bool[] p1 = null;
            bool p2 = false;
            short[] p3 = null;
            dynamic dy = new MemberClass();
            MyEnum result = (MyEnum)dy.Method_ReturnMyEnumNullable(ref p1, p2, out p3);
            Assert.Equal(2, p1.Length);
            Assert.Equal(3, p3.Length);
            Assert.Equal(MyEnum.First, result);
        }

        [Fact]
        public static void ForeachBody()
        {
            string[] forLoop = new string[] { null, string.Empty, "Test" };
            List<MyStruct> list = new List<MyStruct>();
            foreach (string s in forLoop)
            {
                dynamic d = new MemberClass();
                MyStruct result = (MyStruct)d.Method_ReturnMyStruct(s, forLoop[0]);
                list.Add(result);
            }
            Assert.Equal(3, list.Count);
            Assert.Equal(3, list[0].Number);
            Assert.Equal(3, list[1].Number);
            Assert.Equal(3, list[2].Number);
        }

        [Fact]
        public static void StaticMethodBody11()
        {
            short p1 = 0;
            bool p2 = true;
            MyClass[] p3 = new MyClass[] { null, null };
            dynamic d = new MemberClass();
            MyStruct result = (MyStruct)d.Method_ReturnMyStruct(p1, out p2, p3);
            Assert.False(p2);
            Assert.Equal(2, result.Number);
        }

        [Fact]
        public static void DynamicMethodCall()
        {
            dynamic d = new MemberClass();
            MyStruct? result = (MyStruct?)d.Method_ReturnMyStructNullable((object)d.Method_ReturnMyStructNullable());
            Assert.Equal(3, result.Value.Number);
        }

        [Fact]
        public static void StaticMethodBody12()
        {
            byte? p1 = null;
            string[] p2 = new string[2];
            dynamic d = new MemberClass();
            MyStruct? result = (MyStruct?)d.Method_ReturnMyStructNullable(out p1, out p2, ulong.MaxValue, ulong.MinValue);
            Assert.Equal((byte)3, p1);
            Assert.Equal(3, p2.Length);
            Assert.Equal(2, result.Value.Number);
        }

        [Fact]
        public static void StaticMethodBody13()
        {
            dynamic d = new MemberClass();
            dynamic intdy = 1;
            dynamic enumdy = MyEnum.First;
            MyStruct? result = (MyStruct?)d.Method_ReturnMyStructNullable((object)d, (object)intdy, (object)enumdy);
            Assert.Equal(3, result.Value.Number);
        }

        [Fact]
        public static void ArrayInitializerList()
        {
            dynamic d = new MemberClass();
            MyEnum[] p1 = new MyEnum[] { MyEnum.First, default(MyEnum) };
            char?[] p3 = new char?[3];
            object[] result = new object[]
            {
                d.Method_ReturnObject(),
                d.Method_ReturnObject(new byte ? [0]),
                d.Method_ReturnObject(p1, -1, ref p3),
                d.Method_ReturnObject(new bool ? [4])
            };

            Assert.Equal(new object[] { null, null, null, null }, result);
            Assert.Equal(new char?[] { 'a', 'b' }, p3);
        }

        [Fact]
        public static void ForThrowsException()
        {
            var mc = new MemberClass();
            int index = 0;
            char? p1 = 'a';
            string[] p2 = new string[2];
            short[] p3 = new short[3];
            bool testSucceeded = false;
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    mc.Method_ReturnShort_Throw(out p1, ref p2, p3); //exception
                    index++;
                }
            }
            catch (ArgumentException ae)
            {
                testSucceeded = ae.Message.Contains("Test message");
                Assert.Equal(0, index);
                Assert.Equal('a', p1);
                Assert.Equal(2, p2.Length);
            }
            Assert.True(testSucceeded);
        }

        [Fact]
        public static void UsingBlockThrowsException()
        {
            bool testSucceeed = false;
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    dynamic d = new MemberClass();
                    short? result = (short?)d.Method_ReturnShort_ThrowNullable(); //exception
                }
            }
            catch (ArithmeticException ae)
            {
                testSucceeed = ae.Message == "Test message";
            }
            Assert.True(testSucceeed);
        }

        [Fact]
        public static void LockBlockThrowsException()
        {
            object locker = new object();
            bool testSucceeded = false;
            try
            {
                lock (locker)
                {
                    decimal? dec = 123M;
                    dynamic d = new MemberClass();
                    short? result = (short?)d.Method_ReturnShort_ThrowNullable(dec); //exception
                }
            }
            catch (ArgumentException ae)
            {
                testSucceeded = ae.Message.Contains("Test message");
            }
            Assert.True(testSucceeded);
        }

        [Fact]
        public static void SwitchSectionThrowsException()
        {
            bool testSucceeded = false;
            try
            {
                int a = 0;
                dynamic d = new MemberClass();
                short? result = null;
                switch (a)
                {
                    case 0:
                        result = (short?)d.Method_ReturnShort_ThrowNullable(null, 2M, null); //exception
                        break;
                    default:
                        break;
                }
            }
            catch (ArithmeticException ae)
            {
                testSucceeded = ae.Message.Contains("Test message");
            }
            Assert.True(testSucceeded);
        }

        [Fact]
        public static void SwitchSectionThrowsException2()
        {
            decimal? d = 0M;
            bool testSucceeded = false;
            try
            {
                string a = "a";
                dynamic dy = new MemberClass();
                short? result = null;
                switch (a)
                {
                    case "":
                        break;
                    default:
                        result = (short?)dy.Method_ReturnShort_ThrowNullable(ref d); //exception
                        break;
                }
            }
            catch (ArgumentException ae)
            {
                testSucceeded = ae.Message.Contains("Test message");
                Assert.Equal(3M, d);
            }
            Assert.True(testSucceeded);
        }

        [Fact]
        public static void StaticMethodBody14()
        {
            ulong?[] p1 = new ulong?[0];
            byte[] p2 = new byte[0];
            int?[] p3 = null;
            dynamic d = new MemberClass();

            string result = (string)d.Method_ReturnString(out p1, out p2, out p3);
            Assert.Equal(2, p1.Length);
            Assert.Equal(2, p2.Length);
            Assert.Equal(3, p3.Length);
            Assert.Equal("c", result);
        }

        [Fact]
        public static void ForeachExpression2()
        {
            dynamic d = new MemberClass();
            int index = 0;
            foreach (var s in (string[])d.Method_ReturnStringArr())
            {
                index++;
            }
            Assert.Equal(10, index);
        }

        [Fact]
        public static void StaticMethodBody15()
        {
            short?[] p1 = null;
            MyStruct p2 = new MyStruct() { Number = 10 };
            MyStruct[] p3 = new MyStruct[] { p2 };

            dynamic d = new MemberClass();
            string[] result = (string[])d.Method_ReturnStringArr(p1, ref p2, out p3);
            Assert.Equal(3, p2.Number);
            Assert.Equal(2, p3.Length);
            Assert.Equal(1, p3[0].Number);
            Assert.Equal(2, p3[1].Number);
            Assert.Equal(10, result.Length);
        }

        [Fact]
        public static void ImplicitlyTypedArrayInitializer2()
        {
            dynamic mc = new MemberClass();
            object[] p1 = null;
            byte[] p2 = null;
            decimal? p3 = null;
            var result = new long?[]
            {
                (long ? )mc.Method_ReturnUlongNullable(0),
                (long ? )mc.Method_ReturnUlongNullable(out p1, out p2, out p3),
                (long ? )mc.Method_ReturnUlongNullable('a', 'b', null)
            };

            Assert.Equal(new long?[] { 1L, 1L, 1L }, result);
            Assert.Equal(2, p1.Length);
            Assert.Equal(2, p2.Length);
            Assert.Equal(4M, p3);
        }

        [Fact]
        public static void EqualsOperator2()
        {
            dynamic d = new MemberClass();
            MyClass[] m = new MyClass[] { null, null, new MyClass() };
            ulong?[] p1 = new ulong?[4];
            MyStruct p2 = new MyStruct();
            byte?[] p3 = new byte?[1];

            var result = (ulong)d.Method_ReturnUlong(m) == (ulong)d.Method_ReturnUlong(ref p1, out p2, ref p3);
            Assert.True(result);
            Assert.Equal((ulong)3, p1[0]);
            Assert.Equal(3, p2.Number);
            Assert.Equal((byte)1, p3[0]);
        }

        [Fact]
        public static void StaticMethodBody16()
        {
            dynamic d = new MemberClass();
            d.Method_ReturnVoid();
        }

        [Fact]
        public static void StaticMethodBody17()
        {
            dynamic mc = new MemberClass();
            mc.Method_ReturnVoid(new MyStruct[] { new MyStruct(), new MyStruct() { Number = -1 } });
        }

        [Fact]
        public static void StaticMethodBody18()
        {
            dynamic mc = new MemberClass();
            mc.Method_ReturnVoid(1UL, 2UL);
        }

        [Fact]
        public static void StaticMethodBody19()
        {
            object[] p1 = new object[7];
            bool?[] p2 = new bool?[0];
            MyClass[] p3 = new MyClass[3];
            dynamic mc = new MemberClass();
            mc.Method_ReturnVoid(ref p1, out p2, ref p3);
            Assert.Null(p1);
            Assert.Equal(new bool?[] { false }, p2);
            Assert.Null(p3);
        }
    }

    public static class Extension
    {
        public static bool TestMethod(this RegularClassRegularMethodTests t)
        {
            dynamic d = new MemberClass();
            return d.Method_ReturnBoolNullable();
        }

        public static int ExPlus(this object i)
        {
            if (i.GetType() == typeof(int))
            {
                int result = (int)i;
                return ++result;
            }
            else
                throw new ArgumentException();
        }

        public static bool IsNull(this int? value) => value == null;
    }
    
    public class ClassWithNestedClass
    {
        public class NestedClass
        {
            public MyStruct ms;
            public bool? BoolProperty { get; set; }

            public byte[] ByteArray;
        }

        [Fact]
        public static void MemberInitializerOfObjectIntializer()
        {
            MemberClass mc = new MemberClass();
            dynamic d = mc;
            NestedClass tc = new NestedClass
            {
                ms = d.Method_ReturnMyStruct(),
                BoolProperty = d.Method_ReturnBoolNullable((short?)3),
                ByteArray = d.Method_ReturnByteArr(0, 1, 2, 3, 4)
            };

            Assert.Equal(3, tc.ms.Number);
            Assert.True(tc.BoolProperty);
            Assert.Equal(new byte[] { 0, 1, 2, 3, 4 }, tc.ByteArray);
        }
    }

    public class ClassWithGetProperty
    {
        public MyClass MyClass
        {
            get
            {
                dynamic mc = new MemberClass();
                return (MyClass)mc.Method_ReturnMyClass();
            }
        }

        [Fact]
        public static void PropertyGetBody()
        {
            ClassWithGetProperty t = new ClassWithGetProperty();
            Assert.Equal(1, t.MyClass.Field);
        }
    }

    public class ClassWithIteratorCallingLambdaExpression
    {
        private static dynamic s_mc = new MemberClass();
        public IEnumerable Increment(int number, decimal max)
        {
            while (number < max)
            {
                Func<int?, decimal> func = (int? x) => (decimal)s_mc.Method_ReturnDecimal(x);
                yield return func(number++);
            }
        }

        [Fact]
        public static void LamdaExpression()
        {
            decimal index = 0;
            dynamic d = new ClassWithIteratorCallingLambdaExpression();
            foreach (decimal i in (IEnumerable)d.Increment(0, 10))
            {
                Assert.Equal(index++, i);
            }
        }
    }

    public class ClassWithObjectInitializerInsideCollectionIntializer1
    {
        private MyClass _myclass = null;

        [Fact]
        public static void ObjectInitializerInsideCollectionInitializer()
        {
            dynamic mc = new MemberClass();
            List<ClassWithObjectInitializerInsideCollectionIntializer1> list = new List<ClassWithObjectInitializerInsideCollectionIntializer1>()
            {
                new ClassWithObjectInitializerInsideCollectionIntializer1() { _myclass = mc.Method_ReturnMyClass(11) }
            };

            Assert.Equal(1, list.Count);
            Assert.Equal(11, list[0]._myclass.Field);
        }
    }

    public class ClassWithDelegate
    {
        private delegate void MyDec(byte?[] input);
        private static void IsEqual(byte?[] input)
        {
            Assert.Equal(new byte?[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, input);
        }

        [Fact]
        public static void DelegateInvokeArgument()
        {
            dynamic mc = new MemberClass();
            MyDec dec = IsEqual;
            dec((byte?[])mc.Method_ReturnByteArrNullable(null, null, null));
        }
    } 

    public class ClassWithConstructor
    {
        private byte[] _field;
        public ClassWithConstructor()
        {
            dynamic mc = new MemberClass();
            _field = (byte[])mc.Method_ReturnByteArr();
        }

        [Fact]
        public static void Constructor()
        {
            ClassWithConstructor t = new ClassWithConstructor();
            Assert.Equal(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, t._field);
        }
    }

    public class ClassWithFieldInitializerOutOfConstructor
    {
        private dynamic _d = new MemberClass();
        private byte[] _field;
        public ClassWithFieldInitializerOutOfConstructor()
        {
            _field = (byte[])_d.Method_ReturnByteArr(new MyClass[] { null, null, new MyClass() });
        }

        [Fact]
        public static void FieldInitializerOutOfConstructor()
        {
            ClassWithFieldInitializerOutOfConstructor t = new ClassWithFieldInitializerOutOfConstructor();
            Assert.Equal(new byte[] { 5, 5, 5 }, t._field);
        }
    }

    public class ClassWithStaticFieldInitializerOutOfStaticConstructor
    {
        private static dynamic s_d = new MemberClass();
        private static short?[] s_p1 = null;
        private static bool?[] s_p2 = new bool?[1];
        private static MyEnum s_p3 = default(MyEnum);
        private static byte[] s_field = (byte[])s_d.Method_ReturnByteArr(out s_p1, ref s_p2, ref s_p3);
        static ClassWithStaticFieldInitializerOutOfStaticConstructor() { }

        [Fact]
        public static void FieldInitializerOutOfStaticConstructor()
        {
            Assert.Equal(new short?[] { 1, null }, s_p1);
            Assert.Equal(new bool?[] { null }, s_p2);
            Assert.Equal(MyEnum.Third, s_p3);
            Assert.Equal(new byte[] { 1, 0 }, s_field);
        }
    }

    public class ClassWithAddOperator
    {
        private decimal _field;
        public static ClassWithAddOperator operator +(ClassWithAddOperator t1, ClassWithAddOperator t2)
        {
            return new ClassWithAddOperator() { _field = t1._field + t2._field };
        }

        [Fact]
        public static void PlusOperator()
        {
            ulong p1 = 0;
            MyEnum p2 = MyEnum.Second;
            dynamic mc = new MemberClass();
            ClassWithAddOperator t1 = new ClassWithAddOperator() { _field = (decimal)mc.Method_ReturnDecimal(out p1, ref p2, null, 'a') };
            ClassWithAddOperator t2 = new ClassWithAddOperator() { _field = (decimal)mc.Method_ReturnDecimal(null, null, 0) };

            ClassWithAddOperator t3 = t1 + t2;
            Assert.Equal((ulong)2, p1);
            Assert.Equal(MyEnum.Third, p2);

            Assert.Equal(1M, t1._field);
            Assert.Equal(1M, t2._field);
            Assert.Equal(2M, t3._field);
        }
    }

    public class ClassWithModuloOperator
    {
        private decimal? _field;
        public static ClassWithModuloOperator operator %(ClassWithModuloOperator t1, ClassWithModuloOperator t2)
        {
            return new ClassWithModuloOperator() { _field = t1._field % t2._field };
        }

        [Fact]
        public static void ModuloOperator()
        {
            dynamic d = new MemberClass();
            ClassWithModuloOperator t1 = new ClassWithModuloOperator() { _field = (decimal?)d.Method_ReturnDecimalNullable() };
            ClassWithModuloOperator t2 = new ClassWithModuloOperator() { _field = (decimal?)d.Method_ReturnDecimal(1, -1, 0) };

            ClassWithModuloOperator t3 = t1 % t2;
            Assert.Equal(1M, t1._field);
            Assert.Equal(1M, t2._field);
            Assert.Equal(0M, t3._field);
        }
    }

    public class ClassWithObjectInitializerInsideCollectionIntializer2
    {
        private float _field;

        [Fact]
        public static void ObjectInitializerInsideCollectionIntializer()
        {
            int myInt = 345;
            dynamic d = new MemberClass();
            List<ClassWithObjectInitializerInsideCollectionIntializer2> list = new List<ClassWithObjectInitializerInsideCollectionIntializer2>()
            {
                new ClassWithObjectInitializerInsideCollectionIntializer2() { _field = (float)d.Method_ReturnFloat() },
                new ClassWithObjectInitializerInsideCollectionIntializer2(){ _field = (float)d.Method_ReturnFloat(myInt)}
            };
            Assert.Equal(2, list.Count);
            Assert.Equal(3.4534f, list[0]._field);
            Assert.Equal(345f, list[1]._field);
        }
    }

    public class ClassWithStaticSetProperty
    {
        private static float s_field;
        public static float MyProp
        {
            get { return s_field; }
            set
            {
                float myFloat = 3f;
                dynamic mc = new MemberClass();
                s_field = (float)mc.Method_ReturnFloat(value, myFloat, 34.5f);
            }
        }

        [Fact]
        public static void StaticPropertySetBody()
        {
            MyProp = 10f;
            Assert.Equal(10f, MyProp);
        }
    }

    public class ClassWithStaticGetProperty
    {
        private static float s_p1;
        private static float s_p2;
        public static float MyProp
        {
            get
            {
                float myFloat = 3f;
                dynamic mc = new MemberClass();
                return (float)mc.Method_ReturnFloat(ref s_p1, out s_p2, s_p1, s_p2, myFloat);
            }
            set { }
        }

        [Fact]
        public static void StaticPropertyGetBody()
        {
            float result = ClassWithStaticGetProperty.MyProp;
            Assert.Equal(3, result);
            Assert.Equal(0.000003f, s_p1);
            Assert.Equal(2342424, s_p2);
        }
    }

    public class ClassWithUnsafeStaticMethod
    {
        private static int? s_p1 = null;
        private static short? s_p2 = 2;
        private static byte s_p3 = 0;

        private static void TestMethod(MyClass mc)
        {
            Assert.Equal(2, mc.Field);
        }

        [Fact]
        public static void StaticMethodBody_Unsafe()
        {
            dynamic d = new MemberClass();
            TestMethod((MyClass)d.Method_ReturnMyClass(out s_p1, s_p2, out s_p3));
            Assert.Equal(2, s_p1);
            Assert.Equal(3, s_p3);
        }
    }

    public class ClassWithUnsafeRegularMethod
    {
        private void TestMethod(MyClass mc, int value)
        {
            Assert.Equal(value, mc.Field);
        }

        [Fact]
        public static void RegularMethodBody_Unsafe()
        {
            dynamic dy = new MemberClass();
            ulong value = int.MaxValue;
            new ClassWithUnsafeRegularMethod().TestMethod((MyClass)dy.Method_ReturnMyClass(value), (int)value);
        }
    }

    public class ClassWithSetIndexer
    {
        private Dictionary<int, MyStruct> _dic = new Dictionary<int, MyStruct>();

        [Fact]
        public static void IndexerSetBody()
        {
            ClassWithSetIndexer t = new ClassWithSetIndexer();
            t[string.Empty] = new MyStruct() { Number = 10 };
            t["a"] = new MyStruct() { Number = -1 };

            Assert.Equal(2, t._dic.Count);
            Assert.Equal(0, t._dic[0].Number);
            Assert.Equal(1, t._dic[1].Number);
        }

        public MyStruct this[string i]
        {
            set
            {
                dynamic d = new MemberClass();
                _dic.Add(i.Length, (MyStruct)d.Method_ReturnMyStruct(i));
            }
        }
    }

    public class ClassWithIteratorThrowingException
    {
        private dynamic _d = new MemberClass();

        public IEnumerable Increment(int number)
        {
            while (number < 5)
            {
                number++;
                yield return _d.Method_ReturnShort_Throw();
            }
        }

        [Fact]
        public static void IteratorThrowingException()
        {
            ClassWithIteratorThrowingException t = new ClassWithIteratorThrowingException();
            bool testSucceeded = false;
            try
            {
                foreach (short i in t.Increment(0)) { }
            }
            catch (ArithmeticException ae)
            {
                testSucceeded = ae.Message == "Test message";
            }
            Assert.True(testSucceeded);
        }
    }

    public class ClassWithConstructorThrowsException
    {
        public ClassWithConstructorThrowsException()
        {
            dynamic d = new MemberClass();
            ClassInitializingClassThatThrowsException.result = (short)d.Method_ReturnShort_Throw(out ClassInitializingClassThatThrowsException.m);
        }
    }

    public class ClassInitializingClassThatThrowsException
    {
        public static MyEnum m = MyEnum.First;
        public static short result = -1;
        public ClassInitializingClassThatThrowsException() { }

        [Fact]
        public static void ConstructorThrowsException()
        {
            bool testSucceeded = false;
            try
            {
                ClassWithConstructorThrowsException t = new ClassWithConstructorThrowsException(); //exception
            }
            catch (ArgumentException ae)
            {
                testSucceeded = ae.Message.Contains("Test message");
                Assert.Equal(MyEnum.Second, m);
                Assert.Equal(-1, result);
            }
            Assert.True(testSucceeded);
        }
    }

    public class ClassWithDoWhileThrowingException
    {
        private char? _p1 = null;
        private int[] _p2 = new int[2];
        private bool?[] _p3 = new bool?[6];

        [Fact]
        public static void MainMethod()
        {
            bool testSucceeded = false;
            try
            {
                do
                {
                    ClassWithDoWhileThrowingException t = new ClassWithDoWhileThrowingException();
                    dynamic dy = new MemberClass();
                    short? result = (short?)dy.Method_ReturnShort_ThrowNullable(out t._p1, t._p2, t._p3); //exception
                }
                while (true);
            }
            catch (ArgumentException ae)
            {
                testSucceeded = ae.Message.Contains("Test message");
            }
            Assert.True(testSucceeded);
        }
    }

    public class ClassWithStaticFieldInitializer
    {
        private static dynamic s_dy = new MemberClass();
        private static string[] s_result = (string[])s_dy.Method_ReturnStringArr(MyEnum.First);

        [Fact]
        public static void StaticFieldInitializer()
        {
            Assert.Equal(10, s_result.Length);
        }
    }

    public class ClassWithFieldInitializer
    {
        private static dynamic s_dy = new MemberClass();
        private string[] _result = (string[])s_dy.Method_ReturnStringArr(MyEnum.First, MyEnum.Second, MyEnum.Third);

        [Fact]
        public static void FieldInitializer()
        {
            ClassWithFieldInitializer t = new ClassWithFieldInitializer();
            Assert.Equal(10, t._result.Length);
        }
    }

    public class ClassWithStaticConstructor
    {
        private static string[] s_result;
        private static MyEnum? s_m = MyEnum.Third;
        static ClassWithStaticConstructor()
        {
            dynamic d = new MemberClass();
            s_result = (string[])d.Method_ReturnStringArr(ref s_m);
        }

        [Fact]
        public static void StaticConstructor()
        {
            Assert.Equal(10, s_result.Length);
            Assert.Null(s_m);
        }
    }

    public class ClassWithObjectInitializer
    {
        private long _field1;
        private long? _field2;

        [Fact]
        public static void ObjectInitializer()
        {
            MyClass p1 = new MyClass();
            dynamic mc = new MemberClass();
            ClassWithObjectInitializer t = new ClassWithObjectInitializer()
            {
                _field1 = (long)mc.Method_ReturnUlong(p1),
                _field2 = (long?)mc.Method_ReturnUlongNullable()
            };
            Assert.Equal(0, p1.Field);
            Assert.Equal(p1.ToString().Length, t._field1);
            Assert.Equal(1L, t._field2);
        }
    }
}
