// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Reflection.Tests
{
    public class Util
    {
        public static string ObjectToString(object o)
        {
            if (o == null)
                return string.Empty;

            if (!(o is Array))
                return o.ToString();

            string s = string.Empty;
            Array a = (Array)o;
            for (int i = 0; i < a.Length; i += 1)
            {
                if (i > 0)
                    s = s + ", ";

                if (a.GetValue(i) is Array)
                    s = s + ObjectToString((Array)a.GetValue(i));
                else
                    s = s + a.GetValue(i).ToString();
            }
            return s;
        }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    public class Attr : Attribute
    {
        public Attr(int i)
        {
            value = i;
            sValue = null;
        }

        public Attr(int i, string s)
        {
            value = i;
            sValue = s;
        }

        public override string ToString()
        {
            return "Attr ToString : " + value.ToString() + " " + sValue;
        }

        public string name;
        public int value;
        public string sValue;
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    public class Int32Attr : Attribute
    {
        public Int32Attr(int i)
        {
            value = i;
            arrayValue = null;
        }

        public Int32Attr(int i, int[] a)
        {
            value = i;
            arrayValue = a;
        }

        public string name;
        public readonly int value;
        public int field;
        public readonly int[] arrayValue;
        public int[] arrayField;
        private int _property = 0;
        public int property { get { return _property; } set { _property = value; } }
        private int[] _arrayProperty;
        public int[] arrayProperty { get { return _arrayProperty; } set { _arrayProperty = value; } }

        public override string ToString()
        {
            return GetType().ToString() + " - name : " + name
                                        + "; value : " + Util.ObjectToString(value)
                                        + "; field : " + Util.ObjectToString(field)
                                        + "; property : " + Util.ObjectToString(property)
                                        + "; array value : " + Util.ObjectToString(arrayValue)
                                        + "; array field : " + Util.ObjectToString(arrayField)
                                        + "; array property : " + Util.ObjectToString(arrayProperty);
        }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    public class Int64Attr : Attribute
    {
        public Int64Attr(long l)
        {
            value = l;
            arrayValue = null;
        }

        public Int64Attr(long l, long[] a)
        {
            value = l;
            arrayValue = a;
        }

        public string name;
        public readonly long value;
        public long field;
        public readonly long[] arrayValue;
        public long[] arrayField;
        private long _property = 0;
        public long property { get { return _property; } set { _property = value; } }
        private long[] _arrayProperty;
        public long[] arrayProperty { get { return _arrayProperty; } set { _arrayProperty = value; } }

        public override string ToString()
        {
            return GetType().ToString() + " - name : " + name
                                        + "; value : " + Util.ObjectToString(value)
                                        + "; field : " + Util.ObjectToString(field)
                                        + "; property : " + Util.ObjectToString(property)
                                        + "; array value : " + Util.ObjectToString(arrayValue)
                                        + "; array field : " + Util.ObjectToString(arrayField)
                                        + "; array property : " + Util.ObjectToString(arrayProperty);
        }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    public class EnumAttr : Attribute
    {
        public EnumAttr(MyEnum e)
        {
            value = e;
            arrayValue = null;
        }

        public EnumAttr(MyEnum e, MyEnum[] a)
        {
            value = e;
            arrayValue = a;
        }

        public string name;
        public readonly MyEnum value;
        public MyEnum field;
        public readonly MyEnum[] arrayValue;
        public MyEnum[] arrayField;
        private MyEnum _property = 0;
        public MyEnum property { get { return _property; } set { _property = value; } }
        private MyEnum[] _arrayProperty;
        public MyEnum[] arrayProperty { get { return _arrayProperty; } set { _arrayProperty = value; } }

        public override string ToString()
        {
            return GetType().ToString() + " - name : " + name
                                        + "; value : " + Util.ObjectToString(value)
                                        + "; field : " + Util.ObjectToString(field)
                                        + "; property : " + Util.ObjectToString(property)
                                        + "; array value : " + Util.ObjectToString(arrayValue)
                                        + "; array field : " + Util.ObjectToString(arrayField)
                                        + "; array property : " + Util.ObjectToString(arrayProperty);
        }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    public class StringAttr : Attribute
    {
        public StringAttr(string s)
        {
            value = s;
            arrayValue = null;
        }

        public StringAttr(string s, string[] a)
        {
            value = s;
            arrayValue = a;
        }

        public string name;
        public readonly string value;
        public string field;
        public readonly string[] arrayValue;
        public string[] arrayField;
        private string _property;
        public string property { get { return _property; } set { _property = value; } }
        private string[] _arrayProperty;
        public string[] arrayProperty { get { return _arrayProperty; } set { _arrayProperty = value; } }

        public override string ToString()
        {
            return GetType().ToString() + " - name : " + name
                                        + "; value : " + Util.ObjectToString(value)
                                        + "; field : " + Util.ObjectToString(field)
                                        + "; property : " + Util.ObjectToString(property)
                                        + "; array value : " + Util.ObjectToString(arrayValue)
                                        + "; array field : " + Util.ObjectToString(arrayField)
                                        + "; array property : " + Util.ObjectToString(arrayProperty);
        }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    public class TypeAttr : Attribute
    {
        public TypeAttr(Type type)
        {
            value = type;
            arrayValue = null;
        }

        public TypeAttr(Type type, Type[] array)
        {
            value = type;
            arrayValue = array;
        }

        public string name;
        public readonly Type value;
        public Type field;
        public readonly Type[] arrayValue;
        public Type[] arrayField;
        private Type _property;
        public Type property { get { return _property; } set { _property = value; } }
        private Type[] _arrayProperty;
        public Type[] arrayProperty { get { return _arrayProperty; } set { _arrayProperty = value; } }

        public override string ToString()
        {
            return GetType().ToString() + " - name : " + name
                                        + "; value : " + Util.ObjectToString(value)
                                        + "; field : " + Util.ObjectToString(field)
                                        + "; property : " + Util.ObjectToString(property)
                                        + "; array value : " + Util.ObjectToString(arrayValue)
                                        + "; array field : " + Util.ObjectToString(arrayField)
                                        + "; array property : " + Util.ObjectToString(arrayProperty);
        }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    public class ObjectAttr : Attribute
    {
        public ObjectAttr(object v)
        {
            value = v;
            arrayValue = null;
        }

        public ObjectAttr(object v, object[] a)
        {
            value = v;
            arrayValue = a;
        }

        public string name;
        public readonly object value;
        public object field;
        public readonly object[] arrayValue;
        public object[] arrayField;
        private object _property = 0;
        public object property { get { return _property; } set { _property = value; } }
        private object[] _arrayProperty;
        public object[] arrayProperty { get { return _arrayProperty; } set { _arrayProperty = value; } }

        public override string ToString()
        {
            return GetType().ToString() + " - name : " + name
                                        + "; value : " + Util.ObjectToString(value)
                                        + "; field : " + Util.ObjectToString(field)
                                        + "; property : " + Util.ObjectToString(property)
                                        + "; array value : " + Util.ObjectToString(arrayValue)
                                        + "; array field : " + Util.ObjectToString(arrayField)
                                        + "; array property : " + Util.ObjectToString(arrayProperty);
        }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    public class NullAttr : Attribute
    {
        public NullAttr(string s, Type type, int[] array) { }

        public NullAttr(string s) { }

        public string name;

        public string stringField;
        public Type typeField;
        public int[] arrayField;
        public string stringProperty { get { return null; } set { } }
        public Type typeProperty { get { return null; } set { } }
        public int[] arrayProperty { get { return null; } set { } }
    }
}
