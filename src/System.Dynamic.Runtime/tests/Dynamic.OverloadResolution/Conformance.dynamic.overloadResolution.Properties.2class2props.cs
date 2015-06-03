// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Properties.Twoclass2props.hide001.hide001;
using Xunit;

namespace ManagedTests.DynamicCSharp.Conformance.dynamic.overloadResolution.Properties.Twoclass2props.hide001.hide001
{
    // <Title> Tests overload resolution for 2 class and 2 properties</Title>
    // <Description>
    // </Description>
    // <RelatedBugs></RelatedBugs>
    //<Expects Status=success></Expects>
    // <Code>
    //<Expects Status=warning>\(27,16\).*CS0108</Expects>
    public class Base
    {
        public static int Status;
        public string Prop
        {
            get
            {
                Status = 1;
                return "Foo";
            }

            set
            {
            }
        }
    }

    public class Derived : Base
    {
        public int Prop
        {
            get
            {
                Status = 2;
                return 3;
            }

            set
            {
            }
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
            dynamic d = new Derived();
            int x = (int)d.Prop;
            if (Base.Status == 2 && x == 3)
                return 0;
            else
                return 1;
        }
    }
    // </Code>
}