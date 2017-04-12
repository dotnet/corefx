using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml;

namespace DesktopTestData
{
    [DataContract]
    public class Person
    {
        public Person(string variation)
        {
            age = 6;
            name = "smith";
        }

        public Person()
        {
        }

        [DataMember]
        public int age;

        [DataMember]
        public string name;
    }

    [DataContract]
    public class CharClass
    {
        public CharClass()
        {
            c = default(Char);
            c1 = char.MaxValue;
            c2 = char.MinValue;
            c3 = 'c';
        }

        [DataMember]
        public char c;

        [DataMember]
        public char c1;

        [DataMember]
        public char c2;

        [DataMember]
        public char c3;
    }

    [DataContract]
    [KnownType(typeof(MyEnum1))]
    [KnownType(typeof(PublicDCStruct))]
    public class AllTypes
    {
        public AllTypes()
        {
            a = false;
            b = Byte.MaxValue;
            c = Byte.MinValue;

            //char
            //datetime
            e = Decimal.MaxValue;
            f = Decimal.MinusOne;
            g = Decimal.MinValue;
            h = Decimal.One;
            i = Decimal.Zero;
            j = default(Decimal);
            k = default(Double);
            l = Double.Epsilon;
            m = Double.MaxValue;
            n = Double.MinValue;
            o = Double.NaN;
            p = Double.NegativeInfinity;
            q = Double.PositiveInfinity;
            r = default(Single);
            s = Single.Epsilon;
            t = Single.MinValue;
            u = Single.MaxValue;
            v = Single.NaN;
            w = Single.NegativeInfinity;
            x = Single.PositiveInfinity;
            y = default(Int32);
            z = Int32.MaxValue;
            z1 = Int32.MinValue;
            z2 = default(Int64);
            z3 = Int64.MaxValue;
            z4 = Int64.MinValue;
            z5 = new Object();
            z6 = default(SByte);
            z7 = SByte.MaxValue;
            z8 = SByte.MinValue;
            z9 = default(Int16);
            z91 = Int16.MaxValue;
            z92 = Int16.MinValue;
            z93 = "abc";
            z94 = default(UInt16);
            z95 = UInt16.MaxValue;
            z96 = UInt16.MinValue;
            z97 = default(UInt32);
            z98 = UInt32.MaxValue;
            z99 = UInt32.MinValue;
            z990 = default(UInt64);
            z991 = UInt64.MaxValue;
            z992 = UInt64.MinValue;
            z993 = new Byte[] { 1, 2, 3, 4 };
        }

        [DataMember]
        public Boolean a;

        [DataMember]
        public Byte b;

        [DataMember]
        public Byte c;

        [DataMember]
        public Char d = Char.MaxValue;

        [DataMember]
        public DateTime f5 = DateTime.Now;

        [DataMember]
        public Guid guidData = Guid.NewGuid();

        [DataMember]
        public String strData;

        [DataMember]
        public Decimal e;

        [DataMember]
        public Decimal f;

        [DataMember]
        public Decimal g;

        [DataMember]
        public Decimal h;

        [DataMember]
        public Decimal i;

        [DataMember]
        public Decimal j;

        [DataMember]
        public Double k;

        [DataMember]
        public Double l;

        [DataMember]
        public Double m;

        [DataMember]
        public Double n;

        [DataMember]
        public Double o;

        [DataMember]
        public Double p;

        [DataMember]
        public Double q;

        [DataMember]
        public Single r;

        [DataMember]
        public Single s;

        [DataMember]
        public Single t;

        [DataMember]
        public Single u;

        [DataMember]
        public Single v;

        [DataMember]
        public Single w;

        [DataMember]
        public Single x;

        [DataMember]
        public Int32 y;

        [DataMember]
        public Int32 z;

        [DataMember]
        public Int32 z1;

        [DataMember]
        public Int64 z2;

        [DataMember]
        public Int64 z3;

        [DataMember]
        public Int64 z4;

        [DataMember]
        public Object z5;

        [DataMember]
        public SByte z6;

        [DataMember]
        public SByte z7;

        [DataMember]
        public SByte z8;

        [DataMember]
        public Int16 z9;

        [DataMember]
        public Int16 z91;

        [DataMember]
        public Int16 z92;

        [DataMember]
        public String z93;

        [DataMember]
        public UInt16 z94;

        [DataMember]
        public UInt16 z95;

        [DataMember]
        public UInt16 z96;

        [DataMember]
        public UInt32 z97;

        [DataMember]
        public UInt32 z98;

        [DataMember]
        public UInt32 z99;

        [DataMember]
        public UInt64 z990;

        [DataMember]
        public UInt64 z991;

        [DataMember]
        public UInt64 z992;

        [DataMember]
        public Byte[] z993;

        [DataMember]
        [IgnoreMember]
        public MyEnum1[] enumArrayData = new MyEnum1[] { MyEnum1.red };

        [DataMember]
        [IgnoreMember]
        public XmlQualifiedName xmlQualifiedName = new XmlQualifiedName("WCF", "http://www.microsoft.com");

        [DataMember]
        [IgnoreMember]
        public ValueType timeSpan = TimeSpan.MaxValue;

        [DataMember]
        [IgnoreMember]
        public object obj = new object();

        [DataMember]
        [IgnoreMember]
        public Uri uri = new Uri("http://www.microsoft.com");

        [DataMember]
        [IgnoreMember]
        public Enum enumBase1 = MyEnum1.red;

        [DataMember]
        [IgnoreMember]
        public Array array1 = new object[] { new object(), new object(), new object() };

        [DataMember]
        [IgnoreMember]
        public ValueType valType = new PublicDCStruct(true);

        [DataMember]
        [IgnoreMember]
        public Nullable<DateTimeOffset> nDTO = DateTimeOffset.MaxValue;

        [DataMember]
        [IgnoreMember]
        public List<DateTimeOffset> lDTO = new List<DateTimeOffset>();
    }

    [DataContract]
    [KnownType(typeof(MyEnum1))]
    [KnownType(typeof(PublicDCStruct))]
    public class AllTypes2
    {
        public AllTypes2()
        {
            a = false;
            b = Byte.MaxValue;
            c = Byte.MinValue;

            //char
            //datetime
            e = Decimal.MaxValue;
            f = Decimal.MinusOne;
            g = Decimal.MinValue;
            h = Decimal.One;
            i = Decimal.Zero;
            j = default(Decimal);
            k = default(Double);
            l = Double.Epsilon;
            m = Double.MaxValue;
            n = Double.MinValue;
            o = Double.NaN;
            p = Double.NegativeInfinity;
            q = Double.PositiveInfinity;
            r = default(Single);
            s = Single.Epsilon;
            t = Single.MinValue;
            u = Single.MaxValue;
            v = Single.NaN;
            w = Single.NegativeInfinity;
            x = Single.PositiveInfinity;
            y = default(Int32);
            z = Int32.MaxValue;
            z1 = Int32.MinValue;
            z2 = default(Int64);
            z3 = Int64.MaxValue;
            z4 = Int64.MinValue;
            z5 = new Object();
            z6 = default(SByte);
            z7 = SByte.MaxValue;
            z8 = SByte.MinValue;
            z9 = default(Int16);
            z91 = Int16.MaxValue;
            z92 = Int16.MinValue;
            z93 = "abc";
            z94 = default(UInt16);
            z95 = UInt16.MaxValue;
            z96 = UInt16.MinValue;
            z97 = default(UInt32);
            z98 = UInt32.MaxValue;
            z99 = UInt32.MinValue;
            z990 = default(UInt64);
            z991 = UInt64.MaxValue;
            z992 = UInt64.MinValue;
            z993 = new Byte[] { 1, 2, 3, 4 };
        }

        [DataMember]
        public Boolean a;

        [DataMember]
        public Byte b;

        [DataMember]
        public Byte c;

        [DataMember]
        public Char d = Char.MaxValue;

        [DataMember]
        public DateTime f5 = DateTime.Now;

        [DataMember]
        public Guid guidData = Guid.NewGuid();

        [DataMember]
        public String strData;

        [DataMember]
        public Decimal e;

        [DataMember]
        public Decimal f;

        [DataMember]
        public Decimal g;

        [DataMember]
        public Decimal h;

        [DataMember]
        public Decimal i;

        [DataMember]
        public Decimal j;

        [DataMember]
        public Double k;

        [DataMember]
        public Double l;

        [DataMember]
        public Double m;

        [DataMember]
        public Double n;

        [DataMember]
        public Double o;

        [DataMember]
        public Double p;

        [DataMember]
        public Double q;

        [DataMember]
        public Single r;

        [DataMember]
        public Single s;

        [DataMember]
        public Single t;

        [DataMember]
        public Single u;

        [DataMember]
        public Single v;

        [DataMember]
        public Single w;

        [DataMember]
        public Single x;

        [DataMember]
        public Int32 y;

        [DataMember]
        public Int32 z;

        [DataMember]
        public Int32 z1;

        [DataMember]
        public Int64 z2;

        [DataMember]
        public Int64 z3;

        [DataMember]
        public Int64 z4;

        [DataMember]
        public Object z5;

        [DataMember]
        public SByte z6;

        [DataMember]
        public SByte z7;

        [DataMember]
        public SByte z8;

        [DataMember]
        public Int16 z9;

        [DataMember]
        public Int16 z91;

        [DataMember]
        public Int16 z92;

        [DataMember]
        public String z93;

        [DataMember]
        public UInt16 z94;

        [DataMember]
        public UInt16 z95;

        [DataMember]
        public UInt16 z96;

        [DataMember]
        public UInt32 z97;

        [DataMember]
        public UInt32 z98;

        [DataMember]
        public UInt32 z99;

        [DataMember]
        public UInt64 z990;

        [DataMember]
        public UInt64 z991;

        [DataMember]
        public UInt64 z992;

        [DataMember]
        public Byte[] z993;

        [DataMember]
        [IgnoreMember]
        public MyEnum1[] enumArrayData = new MyEnum1[] { MyEnum1.red };

        [DataMember]
        [IgnoreMember]
        public XmlQualifiedName xmlQualifiedName = new XmlQualifiedName("WCF", "http://www.microsoft.com");

        [DataMember]
        [IgnoreMember]
        public TimeSpan timeSpan = TimeSpan.MaxValue;

        [DataMember]
        [IgnoreMember]
        public object obj = new object();

        [DataMember]
        [IgnoreMember]
        public Uri uri = new Uri("http://www.microsoft.com");

        [DataMember]
        [IgnoreMember]
        public Enum enumBase1 = MyEnum1.red;

        [DataMember]
        [IgnoreMember]
        public Array array1 = new object[] { new object(), new object(), new object() };

        [DataMember]
        [IgnoreMember]
        public ValueType valType = new PublicDCStruct(true);

        [DataMember]
        [IgnoreMember]
        public Nullable<DateTimeOffset> nDTO = DateTimeOffset.MaxValue;

    }

    [DataContract]
    public class DictContainer
    {
        [DataMember]
        public Dictionary<byte[], byte[]> dictionaryData = new Dictionary<byte[], byte[]>();

        public DictContainer()
        {
            dictionaryData.Add(Guid.NewGuid().ToByteArray(), Guid.NewGuid().ToByteArray());
            dictionaryData.Add(Guid.NewGuid().ToByteArray(), Guid.NewGuid().ToByteArray());
            dictionaryData.Add(Guid.NewGuid().ToByteArray(), Guid.NewGuid().ToByteArray());
        }
    }

    [DataContract]
    public class ListContainer
    {
        [DataMember]
        public List<string> listData = new List<string>();
        public ListContainer()
        {
            listData.Add(DateTime.Now.ToShortDateString());
        }
    }

    [DataContract]
    [KnownType(typeof(string))]
    public class ArrayContainer
    {
        [DataMember]
        public ArrayList listData = new ArrayList();
        public ArrayContainer(bool init)
        {
            listData.Add(DateTime.Now.ToShortDateString());
            listData.Add("Test");
            listData.Add(Guid.NewGuid());
        }
    }

    [DataContract]
    [KnownType(typeof(MyPrivateEnum1[]))]
    public class EnumContainer1
    {
        [DataMember]
        public object myPrivateEnum1 = new MyPrivateEnum1[] { MyPrivateEnum1.red };
    }

    [DataContract]
    [KnownType(typeof(MyPrivateEnum2[]))]
    public class EnumContainer2
    {
        [DataMember]
        public object myPrivateEnum2 = new MyPrivateEnum2[] { MyPrivateEnum2.red };
    }

    [DataContract]
    [KnownType(typeof(MyPrivateEnum3[]))]
    public class EnumContainer3
    {
        [DataMember]
        public object myPrivateEnum3 = new MyPrivateEnum3[] { MyPrivateEnum3.red };
    }

    [DataContract]
    public class WithStatic
    {
        public WithStatic()
        {
            sstr = "static string";
            str = "instance string";
        }

        [DataMember]
        public static string sstr;

        [DataMember]
        public string str;
    }

    [DataContract]
    public class PrivateCstor
    {
        PrivateCstor()
        {
            c = 22;
            a = 10;
            b = "string b";
            c = 11;
        }

        public PrivateCstor(int i)
        {
            c = i;
        }

        PrivateCstor(StreamingContext sc)
        {
            c = 33;
        }

        [DataMember]
        public int a;

        [DataMember]
        public string b;

        [DataMember]
        public int c;
    }

    [DataContract]
    public class DerivedFromPriC : PrivateCstor
    {
        public DerivedFromPriC() : base(int.MaxValue) { }
        public DerivedFromPriC(int d)
            : base(d)
        {
            this.d = d;
        }
        [DataMember]
        public int d;
    }

    [DataContract]
    public class EmptyDC
    {
        public EmptyDC()
        {
            a = 10;
        }

        [DataMember]
        public int a;

        public int A
        {
            set { a = value; }
            get { return a; }
        }
    }

    [DataContract(Name = "Enum")]
    public enum MyEnum
    {
        [EnumMember]
        red,
        [EnumMember]
        blue,
        [EnumMember]
        black
    }

    [DataContract(Name = "Enum1")]
    internal enum MyPrivateEnum1
    {
        [EnumMember]
        red,
        [EnumMember]
        blue,
        [EnumMember]
        black
    }

    enum MyPrivateEnum2
    {
        [EnumMember]
        red,
        [EnumMember]
        blue,
        [EnumMember]
        black
    }

    internal enum MyPrivateEnum3
    {
        red,
        blue,
        black
    }

    interface SomeProperties
    {
        int A { set; get; }

        String B { set; get; }
    }

    [DataContract]
    public class Base : SomeProperties
    {
        int a;

        string b;

        [DataMember]
        public virtual int A
        {
            set { a = value; }
            get { return a; }
        }

        [DataMember]
        public virtual string B
        {
            set { b = value; }
            get { return b; }
        }
    }

    [DataContract]
    public class Derived : Base
    {
        int a;

        string b;

        [DataMember]
        override public int A
        {
            set { a = value; }
            get { return a; }
        }

        [DataMember]
        override public string B
        {
            set { b = value; }
            get { return b; }
        }
    }

    [DataContract]
    public enum MyEnum1 : byte
    {
        [EnumMember]
        red,
        [EnumMember]
        blue,
        [EnumMember]
        black
    }

    [DataContract]
    public enum MyEnum2 : sbyte
    {
        [EnumMember]
        red,
        [EnumMember]
        blue,
        [EnumMember]
        black
    }

    [DataContract]
    public enum MyEnum3 : short
    {
        [EnumMember]
        red,
        [EnumMember]
        blue,
        [EnumMember]
        black
    }

    [DataContract]
    public enum MyEnum4 : ushort
    {
        [EnumMember]
        red,
        [EnumMember]
        blue,
        [EnumMember]
        black
    }

    [DataContract]
    public enum MyEnum5 : int
    {
        [EnumMember]
        red,
        [EnumMember]
        blue,
        [EnumMember]
        black
    }

    [DataContract]
    public enum MyEnum6 : uint
    {
        [EnumMember]
        red,
        [EnumMember]
        blue,
        [EnumMember]
        black
    }

    [DataContract]
    public enum MyEnum7 : long
    {
        [EnumMember]
        red,
        [EnumMember]
        blue,
        [EnumMember]
        black
    }

    [DataContract]
    public enum MyEnum8 : ulong
    {
        [EnumMember]
        red,
        [EnumMember]
        blue,
        [EnumMember]
        black
    }

    public class SeasonsEnumContainer
    {
        [IgnoreMember]
        public Seasons1 member1 = Seasons1.Autumn;
        [IgnoreMember]
        public Seasons2 member2 = Seasons2.Spring;
        [IgnoreMember]
        public Seasons3 member3 = Seasons3.Winter;
    }

    [Flags]
    public enum Seasons1 : byte
    {
        None = 0,
        Summer = 1,
        Autumn = 2,
        Winter = 4,
        Spring = 8,
        All = Summer | Autumn | Winter | Spring,
    }

    [Flags]
    public enum Seasons2 : sbyte
    {
        None = 0,
        Summer = 1,
        Autumn = 2,
        Winter = 4,
        Spring = 8,
        All = Summer | Autumn | Winter | Spring,
    }

    [Flags]
    public enum Seasons3 : short
    {
        None = 0,
        Summer = 1,
        Autumn = 2,
        Winter = 4,
        Spring = 8,
        All = Summer | Autumn | Winter | Spring,
    }

    [Flags]
    public enum Seasons4 : ushort
    {
        None = 0,
        Summer = 1,
        Autumn = 2,
        Winter = 4,
        Spring = 8,
        All = Summer | Autumn | Winter | Spring,
    }

    [Flags]
    public enum Seasons5 : int
    {
        None = 0,
        Summer = 1,
        Autumn = 2,
        Winter = 4,
        Spring = 8,
        All = Summer | Autumn | Winter | Spring,
    }

    [Flags]
    public enum Seasons6 : uint
    {
        None = 0,
        Summer = 1,
        Autumn = 2,
        Winter = 4,
        Spring = 8,
        All = Summer | Autumn | Winter | Spring,
    }

    [Flags]
    public enum Seasons7 : long
    {
        None = 0,
        Summer = 1,
        Autumn = 2,
        Winter = 4,
        Spring = 8,
        All = Summer | Autumn | Winter | Spring,
    }

    [Flags]
    public enum Seasons8 : ulong
    {
        None = 0,
        Summer = 1,
        Autumn = 2,
        Winter = 4,
        Spring = 8,
        All = Summer | Autumn | Winter | Spring,
    }

    [DataContract]
    public class list
    {
        [DataMember]
        public int value;

        [DataMember]
        public list next;
    }

    [DataContract]
    public class Arrays
    {
        public Arrays()
        {
            a1 = new int[] { };
            a2 = new int[] { 1 };
            a3 = new int[] { 1, 2, 3, 4 };
            a4 = new int[10000];
        }

        [DataMember]
        public int[] a1;

        [DataMember]
        public int[] a2;

        [DataMember]
        public int[] a3;

        [DataMember]
        public int[] a4;
    }

    [DataContract]
    public class Array2
    {
        [DataMember]
        public int[,] a1 = { { 1, 2, 3 }, { 4, 5, 6 } };
    }

    [DataContract]
    public class Array3
    {
        [DataMember]
        public int[][] a1 = { new int[] { 1, 2, 3 }, new int[] { } };
    }

    [DataContract]
    public class Properties
    {
        int a = 5;

        [DataMember]
        public int A
        {
            set { a = value; }
            get { return a; }
        }
    }

    public struct NotSer
    {
        public int a;
    }

    [DataContract]
    public class HaveNS
    {
        [DataMember]
        public NotSer ns;
    }

    [DataContract]
    public class OutClass
    {
        [DataContract]
        public class NestedClass
        {
            [DataMember]
            public int a = 10;
        }

        [DataMember]
        public NestedClass nc = new NestedClass();
    }

    [DataContract]
    public class Temp
    {
        [DataMember]
        public int a = 10;
    }

    [DataContract]
    public class Array22
    {
        [DataMember]
        public int[] p = new int[] { 1, 2 };
    }


    [DataContract]
    [KnownType(typeof(VT))]
    public class BoxedPrim
    {
        [DataMember]
        public object p = new Boolean();

        [DataMember]
        public object p2 = new VT(10);
    }

    [DataContract]
    public struct VT
    {
        [DataMember]
        public int b;

        public VT(int v)
        {
            b = v;
        }
    }

    [DataContract]
    public class Test
    {
        [DataMember]
        public astruct Data;

        [DataContract]
        public struct astruct
        {
            [DataMember]
            public int a;

            [DataMember]
            public string b;
        }
    }

    public struct MyStruct
    {
        public int value;
        public string globName;

        public MyStruct(bool init)
        {
            value = 10;
            globName = "ﺥﺣﺨﺪﺣﺻﻂﻃ";
        }
    }

    [DataContract]
    public class EnumStructContainer
    {
        [DataMember]
        public object p1 = new VT(10);

        [DataMember]
        public object p2 = new NotSer();

        [DataMember]
        [IgnoreMember]
        public object[] enumArrayData = new object[] { MyEnum1.red, MyEnum1.black, MyEnum1.blue, Seasons1.Autumn, Seasons2.Spring };

        [DataMember]
        public object p3 = new MyStruct();

    }


    [DataContract(Name = "Drawing_using_{0}_T_and_{1}_K_and_{2}_U")]
    public class Drawing<T, K, U>
    {
        public T m1;
        public K m2;
        public U m3;

        public Drawing(T s, K b, U u)
        {
            m1 = s;
            m2 = b;
            m3 = u;
        }
    }
    public class POCOGenericsNullableContainer
    {
        public POCOGenericsNullableContainer() { }

        public object member1 = InstanceCreator.CreateInstanceOf(typeof(List<System.Boolean>), new Random(12345));
        public object member2 = InstanceCreator.CreateInstanceOf(typeof(List<System.Byte>), new Random(12345));
        public object member3 = InstanceCreator.CreateInstanceOf(typeof(List<System.Byte[]>), new Random(12345));
        public object member4 = InstanceCreator.CreateInstanceOf(typeof(List<System.Char>), new Random(12345));
        public object member5 = InstanceCreator.CreateInstanceOf(typeof(List<System.DateTime>), new Random(12345));
        public object member6 = InstanceCreator.CreateInstanceOf(typeof(List<System.DBNull>), new Random(12345));
        public object member7 = InstanceCreator.CreateInstanceOf(typeof(List<System.Decimal>), new Random(12345));
        public object member8 = InstanceCreator.CreateInstanceOf(typeof(List<System.Double>), new Random(12345));
        public object member9 = InstanceCreator.CreateInstanceOf(typeof(List<System.Guid>), new Random(12345));
        public object member10 = InstanceCreator.CreateInstanceOf(typeof(List<System.Int16>), new Random(12345));
        public object member11 = InstanceCreator.CreateInstanceOf(typeof(List<System.Int32>), new Random(12345));
        public object member12 = InstanceCreator.CreateInstanceOf(typeof(List<System.Int64>), new Random(12345));
        public object member13 = InstanceCreator.CreateInstanceOf(typeof(List<System.Nullable<System.Boolean>>), new Random(12345));
        public object member14 = InstanceCreator.CreateInstanceOf(typeof(List<System.Nullable<System.Byte>>), new Random(12345));
        public object member15 = InstanceCreator.CreateInstanceOf(typeof(List<System.Nullable<System.Char>>), new Random(12345));
        public object member16 = InstanceCreator.CreateInstanceOf(typeof(List<System.Nullable<System.DateTime>>), new Random(12345));
        public object member17 = InstanceCreator.CreateInstanceOf(typeof(List<System.Nullable<System.Decimal>>), new Random(12345));
        public object member18 = InstanceCreator.CreateInstanceOf(typeof(List<System.Nullable<System.Double>>), new Random(12345));
        public object member19 = InstanceCreator.CreateInstanceOf(typeof(List<System.Nullable<System.Guid>>), new Random(12345));
        public object member20 = InstanceCreator.CreateInstanceOf(typeof(List<System.Nullable<System.Int16>>), new Random(12345));
        public object member21 = InstanceCreator.CreateInstanceOf(typeof(List<System.Nullable<System.Int32>>), new Random(12345));
        public object member22 = InstanceCreator.CreateInstanceOf(typeof(List<System.Nullable<System.Int64>>), new Random(12345));
        public object member23 = InstanceCreator.CreateInstanceOf(typeof(List<System.Nullable<System.SByte>>), new Random(12345));
        public object member24 = InstanceCreator.CreateInstanceOf(typeof(List<System.Nullable<System.Single>>), new Random(12345));
        public object member25 = InstanceCreator.CreateInstanceOf(typeof(List<System.Nullable<System.TimeSpan>>), new Random(12345));
        public object member26 = InstanceCreator.CreateInstanceOf(typeof(List<System.Nullable<System.UInt16>>), new Random(12345));
        public object member27 = InstanceCreator.CreateInstanceOf(typeof(List<System.Nullable<System.UInt32>>), new Random(12345));
        public object member28 = InstanceCreator.CreateInstanceOf(typeof(List<System.Nullable<System.UInt64>>), new Random(12345));
        public object member29 = InstanceCreator.CreateInstanceOf(typeof(List<System.SByte>), new Random(12345));
        public object member30 = InstanceCreator.CreateInstanceOf(typeof(List<System.Single>), new Random(12345));
        public object member31 = InstanceCreator.CreateInstanceOf(typeof(List<System.String>), new Random(12345));
        public object member32 = InstanceCreator.CreateInstanceOf(typeof(List<System.TimeSpan>), new Random(12345));
        public object member33 = InstanceCreator.CreateInstanceOf(typeof(List<System.UInt16>), new Random(12345));
        public object member34 = InstanceCreator.CreateInstanceOf(typeof(List<System.UInt32>), new Random(12345));
        public object member35 = InstanceCreator.CreateInstanceOf(typeof(List<System.UInt64>), new Random(12345));
        public object member36 = InstanceCreator.CreateInstanceOf(typeof(List<System.Xml.XmlElement>), new Random(12345));
        public object member37 = InstanceCreator.CreateInstanceOf(typeof(List<System.Xml.XmlNode[]>), new Random(12345));
        public object member38 = InstanceCreator.CreateInstanceOf(typeof(List<System.DateTimeOffset>), new Random(12345));
        public object member39 = InstanceCreator.CreateInstanceOf(typeof(List<System.Nullable<System.DateTimeOffset>>), new Random(12345));

        public CustomGeneric2<Person> personInstance = new CustomGeneric2<Person>();
        public object genericOne = new CustomGeneric2<Guid, EmptyDC>();
        public object generictwo = new CustomGeneric2<BoxedPrim, EmptyDC>();
        public object nullableStruct = new Nullable<MyStruct>();
        public object obj = new Drawing<Seasons1, Seasons2, CustomGeneric2<Person>>(Seasons1.Autumn, Seasons2.Summer, new CustomGeneric2<Person>());
    }
}
