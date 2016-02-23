// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

using Xunit;

namespace System.Reflection.Tests
{
    public class GetTypeTests
    {
        public static IEnumerable<object[]> GetType_TestData()
        {
            yield return new object[] { "non-existent-type", null };
            yield return new object[] { null, typeof(MyClass1) };
            yield return new object[] { "System.Reflection.Tests.mYclAss1", typeof(MyClass1) };
            yield return new object[] { "System.Reflection.Tests.MyNameSPACe1.MyNAMEspace99.MyClASs3+inNer", null };
            yield return new object[] { "System.Reflection.Tests.MyNameSPACe1.MyNAMEspace2.MyClASs399+inNer", null };
            yield return new object[] { "System.Reflection.Tests.MyNameSPACe1.MyNAMEspace2.MyClASs3+inNer99", null };
            yield return new object[] { null, typeof(MyNamespace1.MyNamespace2.MyClass2) };
            yield return new object[] { null, typeof(MyNamespace1.MyNamespace2.MyClass3.iNner) };
            yield return new object[] { "System.Reflection.Tests.MyNameSPACe1.MyNAMEspace2.MyClASs3+inNer", typeof(MyNamespace1.MyNamespace2.MyClass3.iNner) };
            yield return new object[] { null, typeof(MyNamespace1.MyNamespace3.Foo) };
            yield return new object[] { "System.Reflection.Tests.mynamespace1.mynamespace3.foo", typeof(MyNamespace1.MyNamespace3.Foo) };
            yield return new object[] { "System.Reflection.Tests.MYNAMESPACE1.MYNAMESPACE3.FOO", typeof(MyNamespace1.MyNamespace3.Foo) };

            Type type = typeof(MyNamespace1.MynAmespace3.Goo<int>);
            yield return new object[] { type.FullName, type };
            yield return new object[] { type.FullName.ToUpper(), type };
            yield return new object[] { type.FullName.ToLower(), type };
        }

        [Theory]
        [MemberData("GetType_TestData")]
        public void GetType(string typeName, Type expected)
        {
            if (typeName == null)
                typeName = expected.FullName;

            Assembly assembly = typeof(GetTypeTests).GetTypeInfo().Assembly;
            Module module = assembly.ManifestModule;

            string aqn = typeName + ", " + assembly.FullName;
            if (expected == null)
            {
                // Type
                Assert.Null(Type.GetType(typeName));
                Assert.Null(Type.GetType(aqn));

                Assert.Null(Type.GetType(typeName, throwOnError: false));
                Assert.Null(Type.GetType(aqn, throwOnError: false));

                Assert.Throws<TypeLoadException>(() => Type.GetType(typeName, throwOnError: true));
                Assert.Throws<TypeLoadException>(() => Type.GetType(aqn, throwOnError: true));

                Assert.Null(Type.GetType(typeName, throwOnError: false, ignoreCase: false));
                Assert.Null(Type.GetType(typeName, throwOnError: false, ignoreCase: true));
                Assert.Null(Type.GetType(aqn, throwOnError: false, ignoreCase: false));
                Assert.Null(Type.GetType(aqn, throwOnError: false, ignoreCase: true));

                Assert.Throws<TypeLoadException>(() => Type.GetType(typeName, throwOnError: true, ignoreCase: false));
                Assert.Throws<TypeLoadException>(() => Type.GetType(typeName, throwOnError: true, ignoreCase: true));
                Assert.Throws<TypeLoadException>(() => Type.GetType(aqn, throwOnError: true, ignoreCase: false));
                Assert.Throws<TypeLoadException>(() => Type.GetType(aqn, throwOnError: true, ignoreCase: true));

                // Assembly
                Assert.Null(assembly.GetType(typeName));
                Assert.Null(assembly.GetType(aqn));

                Assert.Null(assembly.GetType(typeName, throwOnError: false, ignoreCase: false));
                Assert.Null(assembly.GetType(typeName, throwOnError: false, ignoreCase: true));
                Assert.Null(assembly.GetType(aqn, throwOnError: false, ignoreCase: false));
                Assert.Null(assembly.GetType(aqn, throwOnError: false, ignoreCase: true));

                Assert.Throws<TypeLoadException>(() => assembly.GetType(typeName, throwOnError: true, ignoreCase: false));
                Assert.Throws<TypeLoadException>(() => assembly.GetType(typeName, throwOnError: true, ignoreCase: true));
                Assert.Throws<ArgumentException>(() => assembly.GetType(aqn, throwOnError: true, ignoreCase: false));
                Assert.Throws<ArgumentException>(() => assembly.GetType(aqn, throwOnError: true, ignoreCase: true));

                // Module
                Assert.Null(module.GetType(typeName, throwOnError: false, ignoreCase: false));
                Assert.Null(module.GetType(typeName, throwOnError: false, ignoreCase: true));
                Assert.Null(module.GetType(aqn, throwOnError: false, ignoreCase: false));
                Assert.Null(module.GetType(aqn, throwOnError: false, ignoreCase: true));

                Assert.Throws<TypeLoadException>(() => module.GetType(typeName, throwOnError: true, ignoreCase: false));
                Assert.Throws<TypeLoadException>(() => module.GetType(typeName, throwOnError: true, ignoreCase: true));
                Assert.Throws<ArgumentException>(() => module.GetType(aqn, throwOnError: true, ignoreCase: false));
                Assert.Throws<ArgumentException>(() => module.GetType(aqn, throwOnError: true, ignoreCase: true));
            }
            else if (expected.FullName == typeName)
            {
                // Case-sensitive match.
                // Type
                Assert.Equal(expected, Type.GetType(typeName));
                Assert.Equal(expected, Type.GetType(aqn));

                Assert.Equal(expected, Type.GetType(typeName, throwOnError: false));
                Assert.Equal(expected, Type.GetType(aqn, throwOnError: false));

                Assert.Equal(expected, Type.GetType(typeName, throwOnError: true));
                Assert.Equal(expected, Type.GetType(aqn, throwOnError: true));

                Assert.Equal(expected, Type.GetType(typeName, throwOnError: false, ignoreCase: false));
                AssertCaseInsensitiveMatch(expected, Type.GetType(typeName, throwOnError: false, ignoreCase: true));
                Assert.Equal(expected, Type.GetType(aqn, throwOnError: false, ignoreCase: false));
                AssertCaseInsensitiveMatch(expected, Type.GetType(aqn, throwOnError: false, ignoreCase: true));

                Assert.Equal(expected, Type.GetType(typeName, throwOnError: true, ignoreCase: false));
                AssertCaseInsensitiveMatch(expected, Type.GetType(typeName, throwOnError: true, ignoreCase: true));
                Assert.Equal(expected, Type.GetType(aqn, throwOnError: true, ignoreCase: false));
                AssertCaseInsensitiveMatch(expected, Type.GetType(aqn, throwOnError: true, ignoreCase: true));

                // Assembly
                Assert.Equal(expected, assembly.GetType(typeName));
                Assert.Null(assembly.GetType(aqn));

                Assert.Equal(expected, assembly.GetType(typeName, throwOnError: false, ignoreCase: false));
                AssertCaseInsensitiveMatch(expected, assembly.GetType(typeName, throwOnError: false, ignoreCase: true));
                Assert.Null(assembly.GetType(aqn, throwOnError: false, ignoreCase: false));
                Assert.Null(assembly.GetType(aqn, throwOnError: false, ignoreCase: true));

                Assert.Equal(expected, assembly.GetType(typeName, throwOnError: true, ignoreCase: false));
                AssertCaseInsensitiveMatch(expected, assembly.GetType(typeName, throwOnError: true, ignoreCase: true));
                Assert.Throws<ArgumentException>(() => assembly.GetType(aqn, throwOnError: true, ignoreCase: false));
                Assert.Throws<ArgumentException>(() => assembly.GetType(aqn, throwOnError: true, ignoreCase: true));

                // Module
                Assert.Equal(expected, module.GetType(typeName, throwOnError: false, ignoreCase: false));
                AssertCaseInsensitiveMatch(expected, module.GetType(typeName, throwOnError: false, ignoreCase: true));
                Assert.Null(module.GetType(aqn, throwOnError: false, ignoreCase: false));
                Assert.Null(module.GetType(aqn, throwOnError: false, ignoreCase: true));

                Assert.Equal(expected, module.GetType(typeName, throwOnError: true, ignoreCase: false));
                AssertCaseInsensitiveMatch(expected, module.GetType(typeName, throwOnError: true, ignoreCase: true));
                Assert.Throws<ArgumentException>(() => module.GetType(aqn, throwOnError: true, ignoreCase: false));
                Assert.Throws<ArgumentException>(() => module.GetType(aqn, throwOnError: true, ignoreCase: true));
            }
            else if (expected.FullName.Equals(typeName, StringComparison.OrdinalIgnoreCase))
            {
                // Case-insensitive match.
                // Type
                Assert.Null(Type.GetType(typeName));
                Assert.Null(Type.GetType(aqn));

                Assert.Null(Type.GetType(typeName, throwOnError: false));
                Assert.Null(Type.GetType(aqn, throwOnError: false));

                Assert.Throws<TypeLoadException>(() => Type.GetType(typeName, throwOnError: true));
                Assert.Throws<TypeLoadException>(() => Type.GetType(aqn, throwOnError: true));

                Assert.Null(Type.GetType(typeName, throwOnError: false, ignoreCase: false));
                AssertCaseInsensitiveMatch(expected, Type.GetType(typeName, throwOnError: false, ignoreCase: true));
                Assert.Null(Type.GetType(aqn, throwOnError: false, ignoreCase: false));
                AssertCaseInsensitiveMatch(expected, Type.GetType(aqn, throwOnError: false, ignoreCase: true));

                Assert.Throws<TypeLoadException>(() => Type.GetType(typeName, throwOnError: true, ignoreCase: false));
                AssertCaseInsensitiveMatch(expected, Type.GetType(typeName, throwOnError: true, ignoreCase: true));
                Assert.Throws<TypeLoadException>(() => Type.GetType(aqn, throwOnError: true, ignoreCase: false));
                AssertCaseInsensitiveMatch(expected, Type.GetType(aqn, throwOnError: true, ignoreCase: true));

                // Assembly
                Assert.Null(assembly.GetType(typeName));
                Assert.Null(assembly.GetType(aqn));

                Assert.Null(assembly.GetType(typeName, throwOnError: false, ignoreCase: false));
                AssertCaseInsensitiveMatch(expected, assembly.GetType(typeName, throwOnError: false, ignoreCase: true));
                Assert.Null(assembly.GetType(aqn, throwOnError: false, ignoreCase: false));
                Assert.Null(assembly.GetType(aqn, throwOnError: false, ignoreCase: true));

                Assert.Throws<TypeLoadException>(() => assembly.GetType(typeName, throwOnError: true, ignoreCase: false));
                AssertCaseInsensitiveMatch(expected, assembly.GetType(typeName, throwOnError: true, ignoreCase: true));
                Assert.Throws<ArgumentException>(() => assembly.GetType(aqn, throwOnError: true, ignoreCase: false));
                Assert.Throws<ArgumentException>(() => assembly.GetType(aqn, throwOnError: true, ignoreCase: true));

                // Module
                Assert.Null(module.GetType(typeName, throwOnError: false, ignoreCase: false));
                AssertCaseInsensitiveMatch(expected, module.GetType(typeName, throwOnError: false, ignoreCase: true));
                Assert.Null(module.GetType(aqn, throwOnError: false, ignoreCase: false));
                Assert.Null(module.GetType(aqn, throwOnError: false, ignoreCase: true));

                Assert.Throws<TypeLoadException>(() => module.GetType(typeName, throwOnError: true, ignoreCase: false));
                AssertCaseInsensitiveMatch(expected, module.GetType(typeName, throwOnError: true, ignoreCase: true));
                Assert.Throws<ArgumentException>(() => module.GetType(aqn, throwOnError: true, ignoreCase: false));
                Assert.Throws<ArgumentException>(() => module.GetType(aqn, throwOnError: true, ignoreCase: true));
            }
            else
            {
                throw new InvalidOperationException("TEST ERROR.");
            }
        }

        [Fact]
        public void GetType_CoreAssembly()
        {
            Assert.Equal(typeof(int), Type.GetType("System.Int32", throwOnError: true));
            Assert.Equal(typeof(int), Type.GetType("system.int32", throwOnError: true, ignoreCase: true));
        }

        [Fact]
        public void GetType_EmptyString()
        {
            Assembly assembly = typeof(GetTypeTests).GetTypeInfo().Assembly;
            Module module = assembly.ManifestModule;

            string typeName = "";
            string aqn = ", " + typeof(GetTypeTests).GetTypeInfo().Assembly.FullName;

            // Type
            Assert.Null(Type.GetType(typeName));
            Assert.Null(Type.GetType(aqn));

            Assert.Null(Type.GetType(typeName, throwOnError: false));
            Assert.Null(Type.GetType(aqn, throwOnError: false));

            Assert.Throws<TypeLoadException>(() => Type.GetType(typeName, throwOnError: true));
            Assert.Throws<ArgumentException>(() => Type.GetType(aqn, throwOnError: true));

            Assert.Null(Type.GetType(typeName, throwOnError: false, ignoreCase: false));
            Assert.Null(Type.GetType(typeName, throwOnError: false, ignoreCase: true));
            Assert.Null(Type.GetType(aqn, throwOnError: false, ignoreCase: false));
            Assert.Null(Type.GetType(aqn, throwOnError: false, ignoreCase: true));

            Assert.Throws<TypeLoadException>(() => Type.GetType(typeName, throwOnError: true, ignoreCase: false));
            Assert.Throws<TypeLoadException>(() => Type.GetType(typeName, throwOnError: true, ignoreCase: true));
            Assert.Throws<ArgumentException>(() => Type.GetType(aqn, throwOnError: true, ignoreCase: false));
            Assert.Throws<ArgumentException>(() => Type.GetType(aqn, throwOnError: true, ignoreCase: true));

            // Assembly
            Assert.Throws<ArgumentException>(() => assembly.GetType(typeName));
            Assert.Null(assembly.GetType(aqn));

            Assert.Throws<ArgumentException>(() => assembly.GetType(typeName, throwOnError: false, ignoreCase: false));
            Assert.Throws<ArgumentException>(() => assembly.GetType(typeName, throwOnError: false, ignoreCase: true));
            Assert.Null(assembly.GetType(aqn, throwOnError: false, ignoreCase: false));
            Assert.Null(assembly.GetType(aqn, throwOnError: false, ignoreCase: true));

            Assert.Throws<ArgumentException>(() => assembly.GetType(typeName, throwOnError: true, ignoreCase: false));
            Assert.Throws<ArgumentException>(() => assembly.GetType(typeName, throwOnError: true, ignoreCase: true));
            Assert.Throws<ArgumentException>(() => assembly.GetType(aqn, throwOnError: true, ignoreCase: false));
            Assert.Throws<ArgumentException>(() => assembly.GetType(aqn, throwOnError: true, ignoreCase: true));

            // Module
            Assert.Throws<ArgumentException>(() => module.GetType(typeName, throwOnError: false, ignoreCase: false));
            Assert.Throws<ArgumentException>(() => module.GetType(typeName, throwOnError: false, ignoreCase: true));
            Assert.Null(module.GetType(aqn, throwOnError: false, ignoreCase: false));
            Assert.Null(module.GetType(aqn, throwOnError: false, ignoreCase: true));

            Assert.Throws<ArgumentException>(() => module.GetType(typeName, throwOnError: true, ignoreCase: false));
            Assert.Throws<ArgumentException>(() => module.GetType(typeName, throwOnError: true, ignoreCase: true));
            Assert.Throws<ArgumentException>(() => module.GetType(aqn, throwOnError: true, ignoreCase: false));
            Assert.Throws<ArgumentException>(() => module.GetType(aqn, throwOnError: true, ignoreCase: true));
        }

        private static void AssertCaseInsensitiveMatch(Type expectedResult, Type actualResult)
        {
            // When called with "ignoreCase: true", GetType() may have a choice of matching items. The one that is chosen
            // is an implementation detail (and on the CLR, *very* implementation-dependent as it's influenced by the internal
            // layout of private hash tables.) As a result, we do not expect the same result across desktop and Project N
            // and so the best we can do is compare the names.
            Assert.True(expectedResult.AssemblyQualifiedName.Equals(actualResult.AssemblyQualifiedName, StringComparison.OrdinalIgnoreCase));
        }
    }

    namespace MyNamespace1
    {
        namespace MyNamespace2
        {
            public class MyClass2 { }
            public class MyClass3
            {
                public class Inner { }
                public class inner { }
                public class iNner { }
                public class inNer { }
            }
            public class MyClass4 { }
            public class mYClass4 { }
            public class Myclass4 { }
            public class myCLass4 { }
            public class myClAss4 { }
        }

        namespace MyNamespace3
        {
            public class Foo { }
        }

        namespace MynAmespace3
        {
            public class Foo { }

            public class Goo<T> { }
            public class gOo<T> { }
            public class goO<T> { }
        }

        namespace MyNaMespace3
        {
            public class Foo { }
        }

        namespace MyNamEspace3
        {
            public class Foo { }
        }
    }

    public class MyClass1 { }
}
