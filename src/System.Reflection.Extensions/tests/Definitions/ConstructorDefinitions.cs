// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#pragma warning disable 3026

namespace System.Reflection.Tests
{
    public static class ConstructorTestStaticClass
    {
        public static int Members = 3;
        public static int MembersEverything = 9;
        static ConstructorTestStaticClass() { }
    }

    public class ConstructorTestClassWithMultipleConstructors
    {
        public static int Members = 9;
        public static int MembersEverything = 15;

        static ConstructorTestClassWithMultipleConstructors() { }

        private ConstructorTestClassWithMultipleConstructors(String s) { }

        protected ConstructorTestClassWithMultipleConstructors(int i) { }

        public ConstructorTestClassWithMultipleConstructors() { }
        public ConstructorTestClassWithMultipleConstructors(TimeSpan ts) { }
        public ConstructorTestClassWithMultipleConstructors(Object o1, Object o2) { }
        public ConstructorTestClassWithMultipleConstructors(Object obj0, Int32 i4) { }
    }

    public class ConstructorTestBaseClass
    {
        public static int Members = 5;
        public static int MembersEverything = 11;

        static ConstructorTestBaseClass() { }
        public ConstructorTestBaseClass() { }
        public ConstructorTestBaseClass(Int16 i2) { }
    }

    public class ConstructorTestSubClass : ConstructorTestBaseClass
    {
        public new static int Members = 5; //.cctor is added
        public new static int MembersEverything = 11;

        public ConstructorTestSubClass(String s) { }
        public ConstructorTestSubClass(Int16 i2) { }
    }
}
