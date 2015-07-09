// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Xunit;

namespace ManagedTests.DynamicCSharp.Conformance.dynamic.dynamicType.overloadResolution.indexer.getter.Oneclass.Twoparam.accessibility005.accessibility005
{
    using ManagedTests.DynamicCSharp.Conformance.dynamic.dynamicType.overloadResolution.indexer.getter.Oneclass.Twoparam.accessibility005.accessibility005;

    public class Test
    {
        private class Base
        {
            public int this[int x]
            {
                get
                {
                    return 1;
                }
            }
        }

        
        public static void DynamicCSharpRunTest()
        {
            Assert.Equal(0, MainMethod(null));
        }

        public static int MainMethod(string[] args)
        {
            Base b = new Base();
            dynamic x = int.MaxValue;
            var rez = b[x];
            if (rez == 1)
                return 0;
            return 1;
        }
    }
    // </Code>
}



namespace ManagedTests.DynamicCSharp.Conformance.dynamic.dynamicType.overloadResolution.indexer.getter.Oneclass.Twoparam.accessibility006.accessibility006
{
    using ManagedTests.DynamicCSharp.Conformance.dynamic.dynamicType.overloadResolution.indexer.getter.Oneclass.Twoparam.accessibility006.accessibility006;

    public class Test
    {
        protected class Base
        {
            public int this[int x]
            {
                get
                {
                    return 1;
                }
            }
        }

        
        public static void DynamicCSharpRunTest()
        {
            Assert.Equal(0, MainMethod(null));
        }

        public static int MainMethod(string[] args)
        {
            Base b = new Base();
            dynamic x = int.MaxValue;
            var rez = b[x];
            if (rez == 1)
                return 0;
            return 1;
        }
    }
    // </Code>
}



namespace ManagedTests.DynamicCSharp.Conformance.dynamic.dynamicType.overloadResolution.indexer.getter.Oneclass.Twoparam.accessibility007.accessibility007
{
    using ManagedTests.DynamicCSharp.Conformance.dynamic.dynamicType.overloadResolution.indexer.getter.Oneclass.Twoparam.accessibility007.accessibility007;

    public class Test
    {
        internal class Base
        {
            public int this[int x]
            {
                get
                {
                    return 1;
                }
            }
        }

        
        public static void DynamicCSharpRunTest()
        {
            Assert.Equal(0, MainMethod(null));
        }

        public static int MainMethod(string[] args)
        {
            Base b = new Base();
            dynamic x = int.MaxValue;
            var rez = b[x];
            if (rez == 1)
                return 0;
            return 1;
        }
    }
    // </Code>
}



namespace ManagedTests.DynamicCSharp.Conformance.dynamic.dynamicType.overloadResolution.indexer.getter.Oneclass.Twoparam.accessibility011.accessibility011
{
    using ManagedTests.DynamicCSharp.Conformance.dynamic.dynamicType.overloadResolution.indexer.getter.Oneclass.Twoparam.accessibility011.accessibility011;

    public class Test
    {
        public class Higher
        {
            private class Base
            {
                public int this[int x]
                {
                    get
                    {
                        return 1;
                    }
                }
            }

            
            public static void DynamicCSharpRunTest()
            {
                Assert.Equal(0, MainMethod(null));
            }

            public static int MainMethod(string[] args)
            {
                Base b = new Base();
                dynamic x = int.MaxValue;
                var rez = b[x];
                if (rez == 1)
                    return 0;
                return 1;
            }
        }
    }
    // </Code>
}
