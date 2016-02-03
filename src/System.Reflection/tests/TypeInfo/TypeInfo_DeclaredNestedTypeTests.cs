// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

#pragma warning disable 0414
#pragma warning disable 0067
#pragma warning disable 3026
#pragma warning disable 3005

namespace System.Reflection.Tests
{
    public class TypeInfoDeclaredNestedTypeTests
    {
        // Verify NestedTypes
        [Fact]
        public static void TestNestedTypes1()
        {
            VerifyNestedType(typeof(TestNest), "NestPublic", true);
        }


        // Verify NestedTypes
        [Fact]
        public static void TestNestedTypes2()
        {
            VerifyNestedType(typeof(TestNest), "NestPublic2", true);
        }

        // Verify NestedTypes
        [Fact]
        public static void TestNestedTypes3()
        {
            VerifyNestedType(typeof(TestNestDerived), "NestPublic", true);
        }

        // Verify NestedTypes
        [Fact]
        public static void TestNestedTypes4()
        {
            VerifyNestedType(typeof(TestNestDerived), "NestPublic3", true);
        }


        // Verify NestedTypes
        [Fact]
        public static void TestNestedTypes5()
        {
            VerifyNestedType(typeof(TestNestDerived), "NESTPUBLIC3", true);
        }

        // Verify NestedTypes
        [Fact]
        public static void TestNestedTypes6()
        {
            VerifyNestedType(typeof(TestMultiNest), "Nest1", true);
        }

        // Verify NestedTypes
        [Fact]
        public static void TestNestedTypes7()
        {
            VerifyNestedType(typeof(TestMultiNest.Nest1), "Nest2", true);
        }

        // Verify NestedTypes
        [Fact]
        public static void TestNestedTypes8()
        {
            VerifyNestedType(typeof(TestMultiNest.Nest1.Nest2), "Nest3", true);
        }

        // Verify NestedTypes
        [Fact]
        public static void TestNestedTypes9()
        {
            VerifyNestedType(typeof(TestNest), "NoSuchType", false);
        }





        //private helper methods
        private static void VerifyNestedType(Type t, String name, bool present)
        {
            //Fix to initialize Reflection
            String str = typeof(Object).Name;

            TypeInfo ti = t.GetTypeInfo();

            if (present)
            {
                Assert.True(ti.DeclaredNestedTypes.Any(item => item.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase)));
            }
            else if (!present)
            {
                Assert.All(ti.DeclaredNestedTypes, item => Assert.False(item.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase)));
            }
        }
    }

    //Metadata for Reflection
    public class TestNest
    {
        public static int iDeclaredNests = 6;

        public class NestPublic { }
        public class NestPublic2 { }
        private class NestPrivate { }		// private, so not inherited
        internal class NestInternal { }		// internal members are not inherited
        protected class NestProtected { }
        private class NestAssemblyPrivate { }		// same as private, so not inherited
    }

    public class TestNestDerived : TestNest
    {
        new public static int iDeclaredNests = 4;

        public new class NestPublic { }
        public class NestPublic3 { }
        public class NESTPUBLIC3 { }
        private class NestPrivate2 { }
    }

    public class TestMultiNest
    {
        public class Nest1
        {
            public class Nest2
            {
                public class Nest3
                {
                    // nest ends
                }
            }
        }
    }
}
