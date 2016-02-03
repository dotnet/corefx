// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.onedynamicparam001.onedynamicparam001
{
    using ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.onedynamicparam001.onedynamicparam001;

    // <Title> Tests overload resolution for 1 class and 2 operators</Title>
    // <Description>
    // </Description>
    // <RelatedBugs></RelatedBugs>
    //<Expects Status=success></Expects>
    // <Code>
    public class Target
    {
        public int Field;
        public static implicit operator Target(float p1)
        {
            return new Target()
            {
                Field = 1
            }

            ;
        }

        public static implicit operator Target(string p1)
        {
            return new Target()
            {
                Field = 2
            }

            ;
        }
    }

    public class Test
    {
        [Fact]
        public static void DynamicCSharpRunTest()
        {
            Assert.Equal(0, MainMethod(null));
        }

        public static int MainMethod(string[] args)
        {
            dynamic d = 4;
            Target t = d;
            if (t.Field == 1)
                return 0;
            return 1;
        }
    }
    // </Code>
}



namespace ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.onedynamicparam004.onedynamicparam004
{
    using ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.onedynamicparam004.onedynamicparam004;

    // <Title> Tests overload resolution for 1 class and 2 methods</Title>
    // <Description>
    // </Description>
    // <RelatedBugs></RelatedBugs>
    //<Expects Status=success></Expects>
    // <Code>
    public class Target
    {
        public int Field;
        public static explicit operator Target(string x)
        {
            return new Target()
            {
                Field = 1
            }

            ;
        }

        public static explicit operator Target(int x)
        {
            return new Target()
            {
                Field = 2
            }

            ;
        }

        public static explicit operator Target(ulong x)
        {
            return new Target()
            {
                Field = 3
            }

            ;
        }
    }

    public class Test
    {
        [Fact]
        public static void DynamicCSharpRunTest()
        {
            Assert.Equal(0, MainMethod(null));
        }

        public static int MainMethod(string[] args)
        {
            dynamic d = 4;
            Target t = (Target)d;
            if (t.Field == 2)
                return 0;
            return 1;
        }
    }
    // </Code>
}



namespace ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.onedynamicparam005.onedynamicparam005
{
    using ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.onedynamicparam005.onedynamicparam005;

    // <Title> Tests overload resolution for 1 class and 2 methods</Title>
    // <Description>
    // </Description>
    // <RelatedBugs></RelatedBugs>
    //<Expects Status=success></Expects>
    // <Code>
    public class Target
    {
        public int Field = 4;
        public static implicit operator int (Target p1)
        {
            return p1;
        }

        public static implicit operator string (Target p1)
        {
            return string.Empty;
        }
    }

    public class Test
    {
        [Fact]
        public static void DynamicCSharpRunTest()
        {
            Assert.Equal(0, MainMethod(null));
        }

        public static int MainMethod(string[] args)
        {
            Target t = new Target()
            {
                Field = 5
            }

            ;
            dynamic d = t;
            if (d.Field == 5)
                return 0;
            return 1;
        }
    }
    // </Code>
}



namespace ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.oneparam013.oneparam013
{
    using ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.oneparam013.oneparam013;

    // <Title> Tests overload resolution for 1 class and 2 methods</Title>
    // <Description>
    // </Description>
    // <RelatedBugs></RelatedBugs>
    //<Expects Status=success></Expects>
    // <Code>
    public class Target
    {
        public int Field;
        public static explicit operator decimal (Target p1)
        {
            p1.Field = 2;
            return (decimal)p1;
        }

        public static explicit operator int (Target p2)
        {
            return int.MinValue;
        }
    }

    public class Test
    {
        [Fact]
        public static void DynamicCSharpRunTest()
        {
            Assert.Equal(0, MainMethod(null));
        }

        public static int MainMethod(string[] args)
        {
            dynamic d = new Target()
            {
                Field = 1
            }

            ;
            int x = (int)d;
            if (x == int.MinValue)
                return 0;
            return 1;
        }
    }
    // </Code>
}



namespace ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.oneparam014.oneparam014
{
    using ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.oneparam014.oneparam014;

    // <Title> Tests overload resolution for 1 class and 2 methods</Title>
    // <Description>
    // </Description>
    // <RelatedBugs></RelatedBugs>
    //<Expects Status=success></Expects>
    // <Code>
    public class Target
    {
        public static implicit operator string (Target p2)
        {
            return "foo";
        }

        public static explicit operator float (Target p2)
        {
            return 12.3f;
        }
    }

    public class Test
    {
        [Fact]
        public static void DynamicCSharpRunTest()
        {
            Assert.Equal(0, MainMethod(null));
        }

        public static int MainMethod(string[] args)
        {
            dynamic d = new Target();
            string s = d;
            if (s == "foo")
                return 0;
            return 1;
        }
    }
    // </Code>
}



namespace ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.oneparam015.oneparam015
{
    using ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.oneparam015.oneparam015;

    // <Title> Tests overload resolution for 1 class and 2 methods</Title>
    // <Description>
    // </Description>
    // <RelatedBugs></RelatedBugs>
    //<Expects Status=success></Expects>
    // <Code>
    public class Target
    {
        public static explicit operator string (Target p1)
        {
            return "foo";
        }

        public static explicit operator short (Target p2)
        {
            return 1;
        }
    }

    public class Test
    {
        [Fact]
        public static void DynamicCSharpRunTest()
        {
            Assert.Equal(0, MainMethod(null));
        }

        public static int MainMethod(string[] args)
        {
            dynamic d = new Target();
            int s = (short)d;
            if (s == 1)
                return 0;
            return 1;
        }
    }
    // </Code>
}



namespace ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.oneparam016.oneparam016
{
    using ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.oneparam016.oneparam016;

    // <Title> Tests overload resolution for 1 class and 2 methods</Title>
    // <Description>
    // </Description>
    // <RelatedBugs></RelatedBugs>
    //<Expects Status=success></Expects>
    // <Code>
    public class Target
    {
        public static implicit operator int (Target p2)
        {
            return int.MinValue;
        }

        public static explicit operator short (Target p2)
        {
            return short.MinValue;
        }

        public static implicit operator long (Target p2)
        {
            return long.MaxValue;
        }
    }

    public class Test
    {
        [Fact]
        public static void DynamicCSharpRunTest()
        {
            Assert.Equal(0, MainMethod(null));
        }

        public static int MainMethod(string[] args)
        {
            dynamic d = new Target();
            long s = d;
            if (s == long.MaxValue)
                return 0;
            return 1;
        }
    }
    // </Code>
}



namespace ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.oneparamdifftypesconv001.oneparamdifftypesconv001
{
    using ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.oneparamdifftypesconv001.oneparamdifftypesconv001;

    // <Title> Tests overload resolution for 1 class and 2 methods</Title>
    // <Description>
    // </Description>
    // <RelatedBugs></RelatedBugs>
    //<Expects Status=success></Expects>
    // <Code>
    public class Target
    {
        public int Field;
        public static implicit operator Target(int x)
        {
            return new Target()
            {
                Field = 1
            }

            ;
        }

        public static implicit operator Target(long x)
        {
            return new Target()
            {
                Field = 2
            }

            ;
        }
    }

    public class Test
    {
        [Fact]
        public static void DynamicCSharpRunTest()
        {
            Assert.Equal(0, MainMethod(null));
        }

        public static int MainMethod(string[] args)
        {
            dynamic s = (uint)3;
            Target x = s;
            if (x.Field == 2)
                return 0;
            return 1;
        }
    }
    // </Code>
}



namespace ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.oneparamdifftypesconv002.oneparamdifftypesconv002
{
    using ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.oneparamdifftypesconv002.oneparamdifftypesconv002;

    // <Title> Tests overload resolution for 1 class and 2 methods</Title>
    // <Description>
    // </Description>
    // <RelatedBugs></RelatedBugs>
    //<Expects Status=success></Expects>
    // <Code>
    public class Target
    {
        public int Field;
        public static implicit operator Target(int x)
        {
            return new Target()
            {
                Field = 1
            }

            ;
        }

        public static implicit operator Target(long x)
        {
            return new Target()
            {
                Field = 2
            }

            ;
        }
    }

    public class Test
    {
        [Fact]
        public static void DynamicCSharpRunTest()
        {
            Assert.Equal(0, MainMethod(null));
        }

        public static int MainMethod(string[] args)
        {
            dynamic s = (short)3;
            Target x = s;
            if (x.Field == 1)
                return 0;
            return 1;
        }
    }
    // </Code>
}



namespace ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.oneparamdifftypesconv003.oneparamdifftypesconv003
{
    using ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.oneparamdifftypesconv003.oneparamdifftypesconv003;

    // <Title> Tests overload resolution for 1 class and 2 methods</Title>
    // <Description>
    // </Description>
    // <RelatedBugs></RelatedBugs>
    //<Expects Status=success></Expects>
    // <Code>
    public class Target
    {
        public static explicit operator float (Target x)
        {
            return float.Epsilon;
        }

        public static explicit operator float? (Target x)
        {
            return float.NegativeInfinity;
        }
    }

    public class Test
    {
        [Fact]
        public static void DynamicCSharpRunTest()
        {
            Assert.Equal(0, MainMethod(null));
        }

        public static int MainMethod(string[] args)
        {
            dynamic d = new Target();
            float x = (float)d;
            if (x == float.Epsilon)
                return 0;
            return 1;
        }
    }
    // </Code>
}



namespace ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.oneparamdifftypesconv004.oneparamdifftypesconv004
{
    using ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.oneparamdifftypesconv004.oneparamdifftypesconv004;

    // <Title> Tests overload resolution for 1 class and 2 methods</Title>
    // <Description>
    // </Description>
    // <RelatedBugs></RelatedBugs>
    //<Expects Status=success></Expects>
    // <Code>
    public class Target
    {
        public static explicit operator float (Target x)
        {
            return float.Epsilon;
        }

        public static explicit operator float? (Target x)
        {
            return null;
        }
    }

    public class Test
    {
        [Fact]
        public static void DynamicCSharpRunTest()
        {
            Assert.Equal(0, MainMethod(null));
        }

        public static int MainMethod(string[] args)
        {
            dynamic d = new Target();
            float? f = (float?)d;
            if (f == null)
                return 0;
            return 1;
        }
    }
    // </Code>
}



namespace ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.oneparamdifftypesconv005.oneparamdifftypesconv005
{
    using ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.oneparamdifftypesconv005.oneparamdifftypesconv005;

    // <Title> Tests overload resolution for 1 class and 2 methods</Title>
    // <Description>
    // </Description>
    // <RelatedBugs></RelatedBugs>
    //<Expects Status=success></Expects>
    // <Code>
    public class Target
    {
        public static implicit operator long (Target x)
        {
            return long.MaxValue;
        }

        public static implicit operator int? (Target x)
        {
            return int.MinValue;
        }
    }

    public class Test
    {
        [Fact]
        public static void DynamicCSharpRunTest()
        {
            Assert.Equal(0, MainMethod(null));
        }

        public static int MainMethod(string[] args)
        {
            dynamic d = new Target();
            int? s = d;
            if (s == int.MinValue)
                return 0;
            return 1;
        }
    }
    // </Code>
}



namespace ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.oneparamdifftypesconv006.oneparamdifftypesconv006
{
    using ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.oneparamdifftypesconv006.oneparamdifftypesconv006;

    // <Title> Tests overload resolution for 1 class and 2 methods</Title>
    // <Description>
    // </Description>
    // <RelatedBugs></RelatedBugs>
    //<Expects Status=success></Expects>
    // <Code>
    public enum myEnum
    {
        First,
        Second,
        Third
    }

    public class Target
    {
        public static implicit operator int (Target x)
        {
            return int.MinValue;
        }

        public static implicit operator myEnum(Target x)
        {
            return myEnum.Second;
        }
    }

    public class Test
    {
        [Fact]
        public static void DynamicCSharpRunTest()
        {
            Assert.Equal(0, MainMethod(null));
        }

        public static int MainMethod(string[] args)
        {
            dynamic d = new Target();
            myEnum e = d;
            if (e == myEnum.Second)
                return 0;
            return 1;
        }
    }
    // </Code>
}



namespace ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.oneparamdifftypesconv007.oneparamdifftypesconv007
{
    using ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.oneparamdifftypesconv007.oneparamdifftypesconv007;

    // <Title> Tests overload resolution for 1 class and 2 methods</Title>
    // <Description>
    // </Description>
    // <RelatedBugs></RelatedBugs>
    //<Expects Status=success></Expects>
    // <Code>
    public enum myEnum
    {
        First,
        Second,
        Third
    }

    public class Target
    {
        public int Field;
        public static explicit operator Target(int x)
        {
            return new Target()
            {
                Field = 1
            }

            ;
        }

        public static explicit operator Target(myEnum x)
        {
            return new Target()
            {
                Field = 2
            }

            ;
        }
    }

    public class Test
    {
        [Fact]
        public static void DynamicCSharpRunTest()
        {
            Assert.Equal(0, MainMethod(null));
        }

        public static int MainMethod(string[] args)
        {
            dynamic i = 1;
            Target x = (Target)i;
            if (x.Field == 1)
                return 0;
            return 1;
        }
    }
    // </Code>
}



namespace ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.oneparamdifftypesconv008.oneparamdifftypesconv008
{
    using ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.oneparamdifftypesconv008.oneparamdifftypesconv008;

    // <Title> Tests overload resolution for 1 class and 2 methods</Title>
    // <Description>
    // </Description>
    // <RelatedBugs></RelatedBugs>
    //<Expects Status=success></Expects>
    // <Code>
    public enum myEnum
    {
        First,
        Second,
        Third
    }

    public class Target
    {
        public int Field;
        public static implicit operator Target(int x)
        {
            return new Target()
            {
                Field = 1
            }

            ;
        }

        public static implicit operator Target(myEnum x)
        {
            return new Target()
            {
                Field = 2
            }

            ;
        }
    }

    public class Test
    {
        [Fact]
        public static void DynamicCSharpRunTest()
        {
            Assert.Equal(0, MainMethod(null));
        }

        public static int MainMethod(string[] args)
        {
            dynamic d = 0;
            Target x = d;
            if (x.Field == 1)
                return 0;
            return 1;
        }
    }
    // </Code>
}



namespace ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.oneparamdifftypesconv009.oneparamdifftypesconv009
{
    using ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.oneparamdifftypesconv009.oneparamdifftypesconv009;
    // <Title> Tests overload resolution for 1 class and 2 methods</Title>
    // <Description>
    // </Description>
    // <RelatedBugs></RelatedBugs>
    //<Expects Status=success></Expects>
    // <Code>
    using System;

    public struct myStruct
    {
        public bool Ok
        {
            get;
            set;
        }
    }

    public class Target
    {
        public int Field;
        public static explicit operator Target(myStruct x)
        {
            return new Target()
            {
                Field = 1
            }

            ;
        }

        public static explicit operator Target(ValueType x)
        {
            return new Target()
            {
                Field = 2
            }

            ;
        }
    }

    public class Test
    {
        [Fact]
        public static void DynamicCSharpRunTest()
        {
            Assert.Equal(0, MainMethod(null));
        }

        public static int MainMethod(string[] args)
        {
            dynamic s = new myStruct()
            {
                Ok = false
            }

            ;
            Target x = (Target)s;
            if (x.Field == 1)
                return 0;
            return 1;
        }
    }
    // </Code>
}



namespace ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.oneparamdifftypesconv010.oneparamdifftypesconv010
{
    using ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.oneparamdifftypesconv010.oneparamdifftypesconv010;
    // <Title> Tests overload resolution for 1 class and 2 methods</Title>
    // <Description>
    // </Description>
    // <RelatedBugs></RelatedBugs>
    //<Expects Status=success></Expects>
    // <Code>
    using System;

    public struct myStruct
    {
        public bool Ok
        {
            get;
            set;
        }
    }

    public class Target
    {
        static public int Status = 0;
        public static implicit operator Target(ValueType x)
        {
            Target.Status = 1;
            return null;
        }

        public static implicit operator Target(int x)
        {
            Target.Status = 2;
            return null;
        }
    }

    public class Test
    {
        [Fact]
        public static void DynamicCSharpRunTest()
        {
            Assert.Equal(0, MainMethod(null));
        }

        public static int MainMethod(string[] args)
        {
            dynamic s = new myStruct()
            {
                Ok = false
            }

            ;
            Target x = s;
            if (Target.Status == 1)
                return 0;
            return 1;
        }
    }
    // </Code>
}



namespace ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.oneparamdifftypesconv011.oneparamdifftypesconv011
{
    using ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.oneparamdifftypesconv011.oneparamdifftypesconv011;

    // <Title> Tests overload resolution for 1 class and 2 methods</Title>
    // <Description>
    // </Description>
    // <RelatedBugs></RelatedBugs>
    //<Expects Status=success></Expects>
    // <Code>
    public struct myStruct
    {
        public bool Ok
        {
            get;
            set;
        }
    }

    public class Target
    {
        public int Field;
        public static implicit operator Target(myStruct x)
        {
            return new Target()
            {
                Field = 1
            }

            ;
        }

        public static implicit operator Target(myStruct? x)
        {
            return new Target()
            {
                Field = 2
            }

            ;
        }
    }

    public class Test
    {
        [Fact]
        public static void DynamicCSharpRunTest()
        {
            Assert.Equal(0, MainMethod(null));
        }

        public static int MainMethod(string[] args)
        {
            dynamic s = new myStruct()
            {
                Ok = false
            }

            ;
            Target x = s;
            if (x.Field == 1)
                return 0;
            return 1;
        }
    }
    // </Code>
}



namespace ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.oneparamdifftypesconv012.oneparamdifftypesconv012
{
    using ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.oneparamdifftypesconv012.oneparamdifftypesconv012;

    // <Title> Tests overload resolution for 1 class and 2 methods</Title>
    // <Description>
    // </Description>
    // <RelatedBugs></RelatedBugs>
    //<Expects Status=success></Expects>
    // <Code>
    public struct MyStruct
    {
        public bool Ok
        {
            get;
            set;
        }
    }

    public class Target
    {
        public int Field;
        public static explicit operator Target(MyStruct x)
        {
            return new Target()
            {
                Field = 1
            }

            ;
        }

        public static explicit operator Target(MyStruct? x)
        {
            return new Target()
            {
                Field = 2
            }

            ;
        }
    }

    public class Test
    {
        [Fact]
        public static void DynamicCSharpRunTest()
        {
            Assert.Equal(0, MainMethod(null));
        }

        public static int MainMethod(string[] args)
        {
            MyStruct? ms = null;
            dynamic d = (Target)ms;
            if (d.Field == 2)
                return 0;
            return 1;
        }
    }
    // </Code>
}



namespace ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.oneparamdifftypesconv013.oneparamdifftypesconv013
{
    using ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.oneparamdifftypesconv013.oneparamdifftypesconv013;

    // <Title> Tests overload resolution for 1 class and 2 methods</Title>
    // <Description>
    // </Description>
    // <RelatedBugs></RelatedBugs>
    //<Expects Status=success></Expects>
    // <Code>
    public class Base
    {
    }

    public class Derived : Base
    {
    }

    public class Target
    {
        public int Field;
        public static explicit operator Target(Base x)
        {
            return new Target()
            {
                Field = 1
            }

            ;
        }

        public static explicit operator Target(Derived x)
        {
            return new Target()
            {
                Field = 2
            }

            ;
        }
    }

    public class Test
    {
        [Fact]
        public static void DynamicCSharpRunTest()
        {
            Assert.Equal(0, MainMethod(null));
        }

        public static int MainMethod(string[] args)
        {
            Base b = new Base();
            dynamic d = (Target)b;
            if (d.Field == 1)
                return 0;
            return 1;
        }
    }
    // </Code>
}



namespace ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.oneparamdifftypesconv014.oneparamdifftypesconv014
{
    using ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.oneparamdifftypesconv014.oneparamdifftypesconv014;

    // <Title> Tests overload resolution for 1 class and 2 methods</Title>
    // <Description>
    // </Description>
    // <RelatedBugs></RelatedBugs>
    //<Expects Status=success></Expects>
    // <Code>
    public class Base
    {
    }

    public class Derived : Base
    {
    }

    public class Target
    {
        public static int Status;
        public static implicit operator Base(Target x)
        {
            Target.Status = 1;
            return new Base();
        }

        public static implicit operator Derived(Target x)
        {
            Target.Status = 2;
            return new Derived();
        }
    }

    public class Test
    {
        [Fact]
        public static void DynamicCSharpRunTest()
        {
            Assert.Equal(0, MainMethod(null));
        }

        public static int MainMethod(string[] args)
        {
            dynamic d = new Target();
            Derived b = d;
            if (Target.Status == 2)
                return 0;
            return 1;
        }
    }
    // </Code>
}



namespace ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.oneparamdifftypesconv015.oneparamdifftypesconv015
{
    using ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.oneparamdifftypesconv015.oneparamdifftypesconv015;

    // <Title> Tests overload resolution for 1 class and 2 methods</Title>
    // <Description>
    // </Description>
    // <RelatedBugs></RelatedBugs>
    //<Expects Status=success></Expects>
    // <Code>
    public class Base
    {
    }

    public class Derived : Base
    {
    }

    public class Target
    {
        public int Field;
        public static implicit operator Target(Base x)
        {
            return new Target()
            {
                Field = 1
            }

            ;
        }

        public static implicit operator Target(Derived x)
        {
            return new Target()
            {
                Field = 2
            }

            ;
        }
    }

    public class Test
    {
        [Fact]
        public static void DynamicCSharpRunTest()
        {
            Assert.Equal(0, MainMethod(null));
        }

        public static int MainMethod(string[] args)
        {
            Base b = new Derived();
            dynamic d = b;
            Target x = d;
            if (x.Field == 2)
                return 0;
            return 1;
        }
    }
    // </Code>
}



namespace ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.oneparamdifftypesconv017.oneparamdifftypesconv017
{
    using ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.oneparamdifftypesconv017.oneparamdifftypesconv017;

    // <Title> Tests overload resolution for 1 class and 2 methods</Title>
    // <Description>
    // </Description>
    // <RelatedBugs></RelatedBugs>
    //<Expects Status=success></Expects>
    // <Code>
    public interface I
    {
    }

    public class Base : I
    {
    }

    public class Derived : Base
    {
    }

    public class Target
    {
        public int Field;
        public static explicit operator Target(Derived x)
        {
            return new Target()
            {
                Field = 2
            }

            ;
        }

        public static explicit operator Target(Base x)
        {
            return new Target()
            {
                Field = 1
            }

            ;
        }
    }

    public class Test
    {
        [Fact]
        public static void DynamicCSharpRunTest()
        {
            Assert.Equal(0, MainMethod(null));
        }

        public static int MainMethod(string[] args)
        {
            I b = new Derived();
            dynamic d = b;
            Target x = (Target)d;
            if (x.Field == 2)
                return 0;
            return 1;
        }
    }
    // </Code>
}



namespace ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.oneparamdifftypesconv018.oneparamdifftypesconv018
{
    using ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.oneparamdifftypesconv018.oneparamdifftypesconv018;

    // <Title> Tests overload resolution for 1 class and 2 methods</Title>
    // <Description>
    // </Description>
    // <RelatedBugs></RelatedBugs>
    //<Expects Status=success></Expects>
    // <Code>
    public interface I
    {
    }

    public class Base : I
    {
    }

    public class Base2 : I
    {
    }

    public class Target
    {
        public int Field;
        public static implicit operator Target(Base2 x)
        {
            return new Target()
            {
                Field = 1
            }

            ;
        }

        public static implicit operator Target(Base x)
        {
            return new Target()
            {
                Field = 2
            }

            ;
        }
    }

    public class Test
    {
        [Fact]
        public static void DynamicCSharpRunTest()
        {
            Assert.Equal(0, MainMethod(null));
        }

        public static int MainMethod(string[] args)
        {
            Base b = new Base();
            dynamic d = (I)b;
            Target x = d;
            if (x.Field == 2)
                return 0;
            return 1;
        }
    }
    // </Code>
}



namespace ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.oneparamdifftypesnoconv001.oneparamdifftypesnoconv001
{
    using ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.oneparamdifftypesnoconv001.oneparamdifftypesnoconv001;

    // <Title> Tests overload resolution for 1 class and 2 methods</Title>
    // <Description>
    // </Description>
    // <RelatedBugs></RelatedBugs>
    //<Expects Status=success></Expects>
    // <Code>
    public class Target
    {
        public static explicit operator bool (Target x)
        {
            return false;
        }

        public static explicit operator string (Target x)
        {
            return "foo";
        }
    }

    public class Test
    {
        [Fact]
        public static void DynamicCSharpRunTest()
        {
            Assert.Equal(0, MainMethod(null));
        }

        public static int MainMethod(string[] args)
        {
            dynamic d = new Target();
            bool b = (bool)d;
            if (b == false)
                return 0;
            return 1;
        }
    }
    // </Code>
}



namespace ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.oneparamdifftypesnoconv002.oneparamdifftypesnoconv002
{
    using ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.oneparamdifftypesnoconv002.oneparamdifftypesnoconv002;

    // <Title> Tests overload resolution for 1 class and 2 methods</Title>
    // <Description>
    // </Description>
    // <RelatedBugs></RelatedBugs>
    //<Expects Status=success></Expects>
    // <Code>
    public class Target
    {
        public static bool operator +(Target x, string s)
        {
            return true;
        }

        public static implicit operator string (Target x)
        {
            return string.Empty;
        }
    }

    public class Test
    {
        [Fact]
        public static void DynamicCSharpRunTest()
        {
            Assert.Equal(0, MainMethod(null));
        }

        public static int MainMethod(string[] args)
        {
            dynamic d = new Target();
            string s = d;
            if (s == string.Empty)
                return 0;
            return 1;
        }
    }
    // </Code>
}



namespace ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.oneparamdifftypesuserconv001.oneparamdifftypesuserconv001
{
    using ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.oneparamdifftypesuserconv001.oneparamdifftypesuserconv001;

    // <Title> Tests overload resolution for 1 class and 2 methods</Title>
    // <Description>
    // </Description>
    // <RelatedBugs></RelatedBugs>
    //<Expects Status=success></Expects>
    // <Code>
    public class Base
    {
        public static implicit operator decimal (Base b)
        {
            return 1m;
        }
    }

    public class Target
    {
        public static implicit operator decimal (Target x)
        {
            return decimal.One;
        }

        public static implicit operator Base(Target x)
        {
            return new Base();
        }
    }

    public class Test
    {
        [Fact]
        public static void DynamicCSharpRunTest()
        {
            Assert.Equal(0, MainMethod(null));
        }

        public static int MainMethod(string[] args)
        {
            dynamic d = new Target();
            decimal dec = d;
            if (dec == decimal.One)
                return 0;
            return 1;
        }
    }
    // </Code>
}



namespace ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.errorverifier.errorverifier
{
    using ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.errorverifier.errorverifier;
    using ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.oneparamdifftypesuserconv002.oneparamdifftypesuserconv002;
    using System;
    using System.Collections;
    using System.IO;
    using System.Globalization;
    using System.Reflection;
    using System.Resources;
    using Microsoft.CSharp.RuntimeBinder;

    public enum ErrorElementId
    {
        None,
        SK_METHOD, // method
        SK_CLASS, // type
        SK_NAMESPACE, // namespace
        SK_FIELD, // field
        SK_PROPERTY, // property
        SK_UNKNOWN, // element
        SK_VARIABLE, // variable
        SK_EVENT, // event
        SK_TYVAR, // type parameter
        SK_ALIAS, // using alias
        ERRORSYM, // <error>
        NULL, // <null>
        GlobalNamespace, // <global namespace>
        MethodGroup, // method group
        AnonMethod, // anonymous method
        Lambda, // lambda expression
        AnonymousType, // anonymous type
    }

    public enum ErrorMessageId
    {
        None,
        BadBinaryOps, // Operator '{0}' cannot be applied to operands of type '{1}' and '{2}'
        IntDivByZero, // Division by constant zero
        BadIndexLHS, // Cannot apply indexing with [] to an expression of type '{0}'
        BadIndexCount, // Wrong number of indices inside []; expected '{0}'
        BadUnaryOp, // Operator '{0}' cannot be applied to operand of type '{1}'
        NoImplicitConv, // Cannot implicitly convert type '{0}' to '{1}'
        NoExplicitConv, // Cannot convert type '{0}' to '{1}'
        ConstOutOfRange, // Constant value '{0}' cannot be converted to a '{1}'
        AmbigBinaryOps, // Operator '{0}' is ambiguous on operands of type '{1}' and '{2}'
        AmbigUnaryOp, // Operator '{0}' is ambiguous on an operand of type '{1}'
        ValueCantBeNull, // Cannot convert null to '{0}' because it is a non-nullable value type
        WrongNestedThis, // Cannot access a non-static member of outer type '{0}' via nested type '{1}'
        NoSuchMember, // '{0}' does not contain a definition for '{1}'
        ObjectRequired, // An object reference is required for the non-static field, method, or property '{0}'
        AmbigCall, // The call is ambiguous between the following methods or properties: '{0}' and '{1}'
        BadAccess, // '{0}' is inaccessible due to its protection level
        MethDelegateMismatch, // No overload for '{0}' matches delegate '{1}'
        AssgLvalueExpected, // The left-hand side of an assignment must be a variable, property or indexer
        NoConstructors, // The type '{0}' has no constructors defined
        BadDelegateConstructor, // The delegate '{0}' does not have a valid constructor
        PropertyLacksGet, // The property or indexer '{0}' cannot be used in this context because it lacks the get accessor
        ObjectProhibited, // Member '{0}' cannot be accessed with an instance reference; qualify it with a type name instead
        AssgReadonly, // A readonly field cannot be assigned to (except in a constructor or a variable initializer)
        RefReadonly, // A readonly field cannot be passed ref or out (except in a constructor)
        AssgReadonlyStatic, // A static readonly field cannot be assigned to (except in a static constructor or a variable initializer)
        RefReadonlyStatic, // A static readonly field cannot be passed ref or out (except in a static constructor)
        AssgReadonlyProp, // Property or indexer '{0}' cannot be assigned to -- it is read only
        AbstractBaseCall, // Cannot call an abstract base member: '{0}'
        RefProperty, // A property or indexer may not be passed as an out or ref parameter
        ManagedAddr, // Cannot take the address of, get the size of, or declare a pointer to a managed type ('{0}')
        FixedNotNeeded, // You cannot use the fixed statement to take the address of an already fixed expression
        UnsafeNeeded, // Dynamic calls cannot be used in conjunction with pointers
        BadBoolOp, // In order to be applicable as a short circuit operator a user-defined logical operator ('{0}') must have the same return type as the type of its 2 parameters
        MustHaveOpTF, // The type ('{0}') must contain declarations of operator true and operator false
        CheckedOverflow, // The operation overflows at compile time in checked mode
        ConstOutOfRangeChecked, // Constant value '{0}' cannot be converted to a '{1}' (use 'unchecked' syntax to override)
        AmbigMember, // Ambiguity between '{0}' and '{1}'
        SizeofUnsafe, // '{0}' does not have a predefined size, therefore sizeof can only be used in an unsafe context (consider using System.Runtime.InteropServices.Marshal.SizeOf)
        FieldInitRefNonstatic, // A field initializer cannot reference the non-static field, method, or property '{0}'
        CallingFinalizeDepracated, // Destructors and object.Finalize cannot be called directly. Consider calling IDisposable.Dispose if available.
        CallingBaseFinalizeDeprecated, // Do not directly call your base class Finalize method. It is called automatically from your destructor.
        BadCastInFixed, // The right hand side of a fixed statement assignment may not be a cast expression
        NoImplicitConvCast, // Cannot implicitly convert type '{0}' to '{1}'. An explicit conversion exists (are you missing a cast?)
        InaccessibleGetter, // The property or indexer '{0}' cannot be used in this context because the get accessor is inaccessible
        InaccessibleSetter, // The property or indexer '{0}' cannot be used in this context because the set accessor is inaccessible
        BadArity, // Using the generic {1} '{0}' requires '{2}' type arguments
        BadTypeArgument, // The type '{0}' may not be used as a type argument
        TypeArgsNotAllowed, // The {1} '{0}' cannot be used with type arguments
        HasNoTypeVars, // The non-generic {1} '{0}' cannot be used with type arguments
        NewConstraintNotSatisfied, // '{2}' must be a non-abstract type with a public parameterless constructor in order to use it as parameter '{1}' in the generic type or method '{0}'
        GenericConstraintNotSatisfiedRefType, // The type '{3}' cannot be used as type parameter '{2}' in the generic type or method '{0}'. There is no implicit reference conversion from '{3}' to '{1}'.
        GenericConstraintNotSatisfiedNullableEnum, // The type '{3}' cannot be used as type parameter '{2}' in the generic type or method '{0}'. The nullable type '{3}' does not satisfy the constraint of '{1}'.
        GenericConstraintNotSatisfiedNullableInterface, // The type '{3}' cannot be used as type parameter '{2}' in the generic type or method '{0}'. The nullable type '{3}' does not satisfy the constraint of '{1}'. Nullable types can not satisfy any interface constraints.
        GenericConstraintNotSatisfiedTyVar, // The type '{3}' cannot be used as type parameter '{2}' in the generic type or method '{0}'. There is no boxing conversion or type parameter conversion from '{3}' to '{1}'.
        GenericConstraintNotSatisfiedValType, // The type '{3}' cannot be used as type parameter '{2}' in the generic type or method '{0}'. There is no boxing conversion from '{3}' to '{1}'.
        TypeVarCantBeNull, // Cannot convert null to type parameter '{0}' because it could be a non-nullable value type. Consider using 'default({0})' instead.
        BadRetType, // '{1} {0}' has the wrong return type
        CantInferMethTypeArgs, // The type arguments for method '{0}' cannot be inferred from the usage. Try specifying the type arguments explicitly.
        MethGrpToNonDel, // Cannot convert method group '{0}' to non-delegate type '{1}'. Did you intend to invoke the method?
        RefConstraintNotSatisfied, // The type '{2}' must be a reference type in order to use it as parameter '{1}' in the generic type or method '{0}'
        ValConstraintNotSatisfied, // The type '{2}' must be a non-nullable value type in order to use it as parameter '{1}' in the generic type or method '{0}'
        CircularConstraint, // Circular constraint dependency involving '{0}' and '{1}'
        BaseConstraintConflict, // Type parameter '{0}' inherits conflicting constraints '{1}' and '{2}'
        ConWithValCon, // Type parameter '{1}' has the 'struct' constraint so '{1}' cannot be used as a constraint for '{0}'
        AmbigUDConv, // Ambiguous user defined conversions '{0}' and '{1}' when converting from '{2}' to '{3}'
        PredefinedTypeNotFound, // Predefined type '{0}' is not defined or imported
        PredefinedTypeBadType, // Predefined type '{0}' is declared incorrectly
        BindToBogus, // '{0}' is not supported by the language
        CantCallSpecialMethod, // '{0}': cannot explicitly call operator or accessor
        BogusType, // '{0}' is a type not supported by the language
        MissingPredefinedMember, // Missing compiler required member '{0}.{1}'
        LiteralDoubleCast, // Literal of type double cannot be implicitly converted to type '{1}'; use an '{0}' suffix to create a literal of this type
        UnifyingInterfaceInstantiations, // '{0}' cannot implement both '{1}' and '{2}' because they may unify for some type parameter substitutions
        ConvertToStaticClass, // Cannot convert to static type '{0}'
        GenericArgIsStaticClass, // '{0}': static types cannot be used as type arguments
        PartialMethodToDelegate, // Cannot create delegate from method '{0}' because it is a partial method without an implementing declaration
        IncrementLvalueExpected, // The operand of an increment or decrement operator must be a variable, property or indexer
        NoSuchMemberOrExtension, // '{0}' does not contain a definition for '{1}' and no extension method '{1}' accepting a first argument of type '{0}' could be found (are you missing a using directive or an assembly reference?)
        ValueTypeExtDelegate, // Extension methods '{0}' defined on value type '{1}' cannot be used to create delegates
        BadArgCount, // No overload for method '{0}' takes '{1}' arguments
        BadArgTypes, // The best overloaded method match for '{0}' has some invalid arguments
        BadArgType, // Argument '{0}': cannot convert from '{1}' to '{2}'
        RefLvalueExpected, // A ref or out argument must be an assignable variable
        BadProtectedAccess, // Cannot access protected member '{0}' via a qualifier of type '{1}'; the qualifier must be of type '{2}' (or derived from it)
        BindToBogusProp2, // Property, indexer, or event '{0}' is not supported by the language; try directly calling accessor methods '{1}' or '{2}'
        BindToBogusProp1, // Property, indexer, or event '{0}' is not supported by the language; try directly calling accessor method '{1}'
        BadDelArgCount, // Delegate '{0}' does not take '{1}' arguments
        BadDelArgTypes, // Delegate '{0}' has some invalid arguments
        AssgReadonlyLocal, // Cannot assign to '{0}' because it is read-only
        RefReadonlyLocal, // Cannot pass '{0}' as a ref or out argument because it is read-only
        ReturnNotLValue, // Cannot modify the return value of '{0}' because it is not a variable
        BadArgExtraRef, // Argument '{0}' should not be passed with the '{1}' keyword
        // DelegateOnConditional, // Cannot create delegate with '{0}' because it has a Conditional attribute (REMOVED)
        BadArgRef, // Argument '{0}' must be passed with the '{1}' keyword
        AssgReadonly2, // Members of readonly field '{0}' cannot be modified (except in a constructor or a variable initializer)
        RefReadonly2, // Members of readonly field '{0}' cannot be passed ref or out (except in a constructor)
        AssgReadonlyStatic2, // Fields of static readonly field '{0}' cannot be assigned to (except in a static constructor or a variable initializer)
        RefReadonlyStatic2, // Fields of static readonly field '{0}' cannot be passed ref or out (except in a static constructor)
        AssgReadonlyLocalCause, // Cannot assign to '{0}' because it is a '{1}'
        RefReadonlyLocalCause, // Cannot pass '{0}' as a ref or out argument because it is a '{1}'
        ThisStructNotInAnonMeth, // Anonymous methods, lambda expressions, and query expressions inside structs cannot access instance members of 'this'. Consider copying 'this' to a local variable outside the anonymous method, lambda expression or query expression and using the local instead.
        DelegateOnNullable, // Cannot bind delegate to '{0}' because it is a member of 'System.Nullable<T>'
        BadCtorArgCount, // '{0}' does not contain a constructor that takes '{1}' arguments
        BadExtensionArgTypes, // '{0}' does not contain a definition for '{1}' and the best extension method overload '{2}' has some invalid arguments
        BadInstanceArgType, // Instance argument: cannot convert from '{0}' to '{1}'
        BadArgTypesForCollectionAdd, // The best overloaded Add method '{0}' for the collection initializer has some invalid arguments
        InitializerAddHasParamModifiers, // The best overloaded method match '{0}' for the collection initializer element cannot be used. Collection initializer 'Add' methods cannot have ref or out parameters.
        NonInvocableMemberCalled, // Non-invocable member '{0}' cannot be used like a method.
        NamedArgumentSpecificationBeforeFixedArgument, // Named argument specifications must appear after all fixed arguments have been specified
        BadNamedArgument, // The best overload for '{0}' does not have a parameter named '{1}'
        BadNamedArgumentForDelegateInvoke, // The delegate '{0}' does not have a parameter named '{1}'
        DuplicateNamedArgument, // Named argument '{0}' cannot be specified multiple times
        NamedArgumentUsedInPositional, // Named argument '{0}' specifies a parameter for which a positional argument has already been given
    }

    public enum RuntimeErrorId
    {
        None,
        // RuntimeBinderInternalCompilerException
        InternalCompilerError, // An unexpected exception occurred while binding a dynamic operation
        // ArgumentException
        BindRequireArguments, // Cannot bind call with no calling object
        // RuntimeBinderException
        BindCallFailedOverloadResolution, // Overload resolution failed
        // ArgumentException
        BindBinaryOperatorRequireTwoArguments, // Binary operators must be invoked with two arguments
        // ArgumentException
        BindUnaryOperatorRequireOneArgument, // Unary operators must be invoked with one argument
        // RuntimeBinderException
        BindPropertyFailedMethodGroup, // The name '{0}' is bound to a method and cannot be used like a property
        // RuntimeBinderException
        BindPropertyFailedEvent, // The event '{0}' can only appear on the left hand side of += or -=
        // RuntimeBinderException
        BindInvokeFailedNonDelegate, // Cannot invoke a non-delegate type
        // ArgumentException
        BindImplicitConversionRequireOneArgument, // Implicit conversion takes exactly one argument
        // ArgumentException
        BindExplicitConversionRequireOneArgument, // Explicit conversion takes exactly one argument
        // ArgumentException
        BindBinaryAssignmentRequireTwoArguments, // Binary operators cannot be invoked with one argument
        // RuntimeBinderException
        BindBinaryAssignmentFailedNullReference, // Cannot perform member assignment on a null reference
        // RuntimeBinderException
        NullReferenceOnMemberException, // Cannot perform runtime binding on a null reference
        // RuntimeBinderException
        BindCallToConditionalMethod, // Cannot dynamically invoke method '{0}' because it has a Conditional attribute
        // RuntimeBinderException
        BindToVoidMethodButExpectResult, // Cannot implicitly convert type 'void' to 'object'
        // EE?
        EmptyDynamicView, // No further information on this object could be discovered
        // MissingMemberException
        GetValueonWriteOnlyProperty, // Write Only properties are not supported
    }
}



namespace ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.oneparamdifftypesuserconv002.oneparamdifftypesuserconv002
{
    using ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.errorverifier.errorverifier;
    using ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.oneparamdifftypesuserconv002.oneparamdifftypesuserconv002;
    // <Title> Tests overload resolution for 1 class and 2 methods</Title>
    // <Description>
    // </Description>
    // <RelatedBugs></RelatedBugs>
    //<Expects Status=success></Expects>
    // <Code>
    using System;

    public class Base
    {
        public int Field;
        public static implicit operator int (Base b)
        {
            if (b.Field == 2)
                return 1;
            return 0;
        }

        public static implicit operator long (Base b)
        {
            if (b.Field == 3)
                return 2;
            return 0;
        }
    }

    public class Target
    {
        public static implicit operator Base(Target x)
        {
            return new Base()
            {
                Field = 2
            }

            ;
        }

        public static implicit operator Target(ValueType x)
        {
            return new Target();
        }
    }

    public class Test
    {
        [Fact]
        public static void DynamicCSharpRunTest()
        {
            Assert.Equal(0, MainMethod(null));
        }

        public static int MainMethod(string[] args)
        {
            dynamic d = new Target();
            try
            {
                int x = d; // ? Target->Base->int, not support.
            }
            catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException ex)
            {
                return 0;
            }

            return 1;
        }
    }
    // </Code>
}



namespace ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.oneparamdifftypesuserconv003.oneparamdifftypesuserconv003
{
    using ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.oneparamdifftypesuserconv003.oneparamdifftypesuserconv003;

    // <Title> Tests overload resolution for 1 class and 2 methods</Title>
    // <Description>
    // </Description>
    // <RelatedBugs></RelatedBugs>
    //<Expects Status=success></Expects>
    // <Code>
    public class Base
    {
    }

    public class Derived : Base
    {
        public static implicit operator decimal (Derived b)
        {
            return decimal.Zero;
        }
    }

    public class Target
    {
        public static implicit operator decimal (Target x)
        {
            return decimal.One;
        }

        public static implicit operator Base(Target x)
        {
            return new Base();
        }
    }

    public class Test
    {
        [Fact]
        public static void DynamicCSharpRunTest()
        {
            Assert.Equal(0, MainMethod(null));
        }

        public static int MainMethod(string[] args)
        {
            dynamic d = new Target();
            decimal dec = d;
            if (dec == decimal.One)
                return 0;
            return 1;
        }
    }
    // </Code>
}



namespace ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.genericparam101n.genericparam101n
{
    using ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.errorverifier.errorverifier;
    using ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Operators.Oneclass2operates.conversion.genericparam101n.genericparam101n;
    // <Title> Tests overload resolution for 1 class and 2 operators</Title>
    // <Description></Description>
    // <RelatedBugs></RelatedBugs>
    // <Expects Status=success></Expects>
    // <Code>
    using System;

    public class A<T>
    {
        public static explicit operator int (A<T> x)
        {
            return 0;
        }

        public static explicit operator T(A<T> x)
        {
            return default(T);
        }
    }

    public class P
    {
        [Fact]
        public static void DynamicCSharpRunTest()
        {
            Assert.Equal(0, MainMethod());
        }

        public static int MainMethod()
        {
            try
            {
                dynamic y = new A<int>();
                int x = y;
            }
            catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException ex)
            {
                return 0;
            }

            return 1;
        }
    }
    // </Code>
}
