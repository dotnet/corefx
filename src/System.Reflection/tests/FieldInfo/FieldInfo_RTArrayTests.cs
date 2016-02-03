// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using System;
using System.Reflection;
using System.Collections.Generic;


#pragma warning disable 0414


namespace System.Reflection.Tests
{
    public class FieldInfoRTArrayTests
    {
        //Verify FieldInfo for Array Field
        [Fact]
        public static void TestSetValue_Field1()
        {
            FieldInfo fi = null;
            Type type = typeof(ArrayAsField);
            Object obj = null;


            obj = Activator.CreateInstance(type);
            A[] ATypeWithMixedAB = new A[] { new A(), new FieldInfoArrayB() };
            A[] ATypeWithAllA = new A[] { new A(), new A() };
            A[] ATypeWithAllB = new A[] { new FieldInfoArrayB(), new FieldInfoArrayB() };
            FieldInfoArrayB[] BTypeWithAllB = new FieldInfoArrayB[] { new FieldInfoArrayB(), new FieldInfoArrayB() };
            A[] BTypeWithAllB_Contra = new FieldInfoArrayB[] { new FieldInfoArrayB(), new FieldInfoArrayB() };

            fi = getField(type, "aArray");

            fi.SetValue(obj, ATypeWithMixedAB);
            Assert.True((fi.GetValue(obj)).Equals(ATypeWithMixedAB), "Failed!! Could not set ArrayField aArray using FieldInfo");

            fi.SetValue(obj, ATypeWithAllA);
            Assert.True((fi.GetValue(obj)).Equals(ATypeWithAllA), "Failed!! Could not set ArrayField aArray using FieldInfo");

            fi.SetValue(obj, ATypeWithAllB);
            Assert.True((fi.GetValue(obj)).Equals(ATypeWithAllB), "Failed!! Could not set ArrayField aArray using FieldInfo");

            fi.SetValue(obj, BTypeWithAllB);
            Assert.True((fi.GetValue(obj)).Equals(BTypeWithAllB), "Failed!! Could not set ArrayField aArray using FieldInfo");

            fi.SetValue(obj, BTypeWithAllB_Contra);
            Assert.True((fi.GetValue(obj)).Equals(BTypeWithAllB_Contra), "Failed!! Could not set ArrayField aArray using FieldInfo");
        }


        //Verify FieldInfo for Array Field
        [Fact]
        public static void TestSetValue_Field2()
        {
            FieldInfo fi = null;
            Type type = typeof(ArrayAsField);
            Object obj = null;


            obj = Activator.CreateInstance(type);
            A[] ATypeWithMixedAB = new A[] { new A(), new FieldInfoArrayB() };
            A[] ATypeWithAllA = new A[] { new A(), new A() };
            A[] ATypeWithAllB = new A[] { new FieldInfoArrayB(), new FieldInfoArrayB() };
            FieldInfoArrayB[] BTypeWithAllB = new FieldInfoArrayB[] { new FieldInfoArrayB(), new FieldInfoArrayB() };
            A[] BTypeWithAllB_Contra = new FieldInfoArrayB[] { new FieldInfoArrayB(), new FieldInfoArrayB() };

            fi = getField(type, "bArray");
            Assert.Throws<ArgumentException>(() => fi.SetValue(obj, ATypeWithMixedAB));
            Assert.Throws<ArgumentException>(() => fi.SetValue(obj, ATypeWithAllA));

            Assert.Throws<ArgumentException>(() => fi.SetValue(obj, ATypeWithAllB));

            fi.SetValue(obj, BTypeWithAllB);
            Assert.True((fi.GetValue(obj)).Equals(BTypeWithAllB), "Failed!! Could not set ArrayField bArray using FieldInfo");

            fi.SetValue(obj, BTypeWithAllB_Contra);
            Assert.True((fi.GetValue(obj)).Equals(BTypeWithAllB_Contra), "Failed!! Could not set ArrayField bArray using FieldInfo");
        }



        //Verify FieldInfo for Array Field
        [Fact]
        public static void TestSetValue_Field3()
        {
            FieldInfo fi = null;
            Type type = typeof(ArrayAsField);
            Object obj = Activator.CreateInstance(type);

            I[] mixedMN = new I[] { new M(), new N() };
            int[] mixedInt = new int[] { 200, -200, (byte)30, (ushort)2 };
            byte[] allByte = new byte[] { 2, 3, 4 };

            fi = getField(type, "iArray");
            fi.SetValue(obj, mixedMN);

            Assert.True((fi.GetValue(obj)).Equals(mixedMN), "Failed!! Could not set ArrayField iArray using FieldInfo");
        }


        //Verify FieldInfo for Array Field
        [Fact]
        public static void TestSetValue_Field4()
        {
            FieldInfo fi = null;
            Type type = typeof(ArrayAsField);
            Object obj = Activator.CreateInstance(type);

            I[] mixedMN = new I[] { new M(), new N() };
            int[] mixedInt = new int[] { 200, -200, (byte)30, (ushort)2 };
            byte[] allByte = new byte[] { 2, 3, 4 };

            fi = getField(type, "intArray");
            fi.SetValue(obj, mixedInt);
            Assert.True((fi.GetValue(obj)).Equals(mixedInt), "Failed!! Could not set ArrayField intArray using FieldInfo");

            Assert.Throws<ArgumentException>(() => fi.SetValue(obj, allByte));
        }


        //Verify FieldInfo for Array Field
        [Fact]
        public static void TestSetValue_Field5()
        {
            FieldInfo fi = null;
            Type type = typeof(ArrayAsField);
            Object obj = Activator.CreateInstance(type);

            I[] mixedMN = new I[] { new M(), new N() };
            int[] mixedInt = new int[] { 200, -200, (byte)30, (ushort)2 };
            byte[] allByte = new byte[] { 2, 3, 4 };
            A[] BTypeWithAllB_Contra = new FieldInfoArrayB[] { new FieldInfoArrayB(), new FieldInfoArrayB() };


            fi = getField(type, "objectArray");
            fi.SetValue(obj, mixedMN);
            Assert.True((fi.GetValue(obj)).Equals(mixedMN), "Failed!! Could not set ArrayField objectArray using FieldInfo");

            fi.SetValue(obj, BTypeWithAllB_Contra);

            Assert.True((fi.GetValue(obj)).Equals(BTypeWithAllB_Contra), "Failed!! Could not set ArrayField objectArray using FieldInfo");
            Assert.Throws<ArgumentException>(() => fi.SetValue(obj, mixedInt));
            Assert.Throws<ArgumentException>(() => fi.SetValue(obj, allByte));
        }




        private static FieldInfo getField(string field)
        {
            Type t = typeof(FieldInfoRTArrayTests);
            TypeInfo ti = t.GetTypeInfo();
            IEnumerator<FieldInfo> alldefinedFields = ti.DeclaredFields.GetEnumerator();
            FieldInfo fi = null, found = null;

            while (alldefinedFields.MoveNext())
            {
                fi = alldefinedFields.Current;
                if (fi.Name.Equals(field))
                {
                    //found type
                    found = fi;
                    break;
                }
            }
            return found;
        }


        private static FieldInfo getField(Type type, string field)
        {
            TypeInfo ti = type.GetTypeInfo();
            IEnumerator<FieldInfo> alldefinedFields = ti.DeclaredFields.GetEnumerator();
            FieldInfo fi = null, found = null;

            while (alldefinedFields.MoveNext())
            {
                fi = alldefinedFields.Current;
                if (fi.Name.Equals(field))
                {
                    //found type
                    found = fi;
                    break;
                }
            }
            return found;
        }
    }

    public class A { }
    public class FieldInfoArrayB : A { }

    public interface I { }
    public class M : I { }
    public class N : I { }

    public class ArrayAsField
    {
        public A[] aArray;
        public FieldInfoArrayB[] bArray;

        public I[] iArray;

        public int[] intArray;
        public object[] objectArray;
    }
}
