// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace System.Xml.Linq.Tests
{
    //////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Tests for XObject class.
    /// </summary>
    //////////////////////////////////////////////////////////////////////////////////////////////////////
    public class XObjectTests
    {
        public class SkipNotifyTests
        {
            [Fact]
            public void NoXObjectChangeAnnotation()
            {
                var sut = new FakeXObject();
                sut.AddAnnotation(new object());
                sut.AddAnnotation(new object());
                Assert.True(sut.SkipNotify());
            }

            [Fact]
            public void XObjectChangeAnnotation()
            {
                var sut = new FakeXObject();
                sut.AddAnnotation(new object());
                sut.AddAnnotation(new object());
                sut.AddAnnotation(new XObjectChangeAnnotation());
                Assert.False(sut.SkipNotify());
            }
        }

        private class FakeXObject : XObject
        {
            public override XmlNodeType NodeType
            {
                get
                {
                    return XmlNodeType.None;
                }
            }
        }
    }
}
