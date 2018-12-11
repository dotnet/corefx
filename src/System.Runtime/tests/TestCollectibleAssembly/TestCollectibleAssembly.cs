// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace TestCollectibleAssembly
{
    public class MyTestClass
    {
        public bool MyField = false;
        public int MyProperty => 1;
        public int MyMethod() { return 1 * 1; }
    }

    public class MyGenericTestClass<T>
    {
        public T MyGenericField;
        public T MyGenericProperty { get; set; }
        public T MyGenericMethod(T arg1) { return arg1; }
        public T MyGenericMethod<V>(T arg1, V arg2) { return arg1; }
    }

    public class UseGenerics
    {
        public MyGenericTestClass<int> myGenericTest { get; set; }
    }
}