//
// Unit tests for Transform
//
// Author:
//	Sebastien Pouliot  <sebastien@ximian.com>
//
// Copyright (C) 2008 Novell, Inc (http://www.novell.com)
//
// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using Microsoft.CSharp.RuntimeBinder;
using Xunit;
using BindingFlags = System.Reflection.BindingFlags;

namespace System.Security.Cryptography.Xml.Tests
{

    public class ConcreteTransform : Transform
    {

        protected override XmlNodeList GetInnerXml()
        {
            throw new NotImplementedException();
        }

        public override object GetOutput(Type type)
        {
            return new MemoryStream();
        }

        public override object GetOutput()
        {
            throw new NotImplementedException();
        }

        public override Type[] InputTypes
        {
            get { throw new NotImplementedException(); }
        }

        public override void LoadInnerXml(global::System.Xml.XmlNodeList nodeList)
        {
            throw new NotImplementedException();
        }

        public override void LoadInput(object obj)
        {
            throw new NotImplementedException();
        }

        public override Type[] OutputTypes
        {
            get { throw new NotImplementedException(); }
        }
    }

    public class TransformTest
    {
        [Fact]
        public void Constructor()
        {
            ConcreteTransform concreteTransform = new ConcreteTransform();
            Assert.Null(concreteTransform.Context);
            Assert.Null(concreteTransform.Algorithm);
#if !NET45 && !NET451 && !NET452 && !NET46 && !NET461 && !NET462 && !NET463
            Assert.Empty(concreteTransform.PropagatedNamespaces);
#endif
        }

        [Fact]
        public void GetDigestedOutput_Null()
        {
            Assert.Throws<NullReferenceException>(() => new ConcreteTransform().GetDigestedOutput(null));
        }

#if !NET45 && !NET451 && !NET452 && !NET46 && !NET461 && !NET462 && !NET463
        // Use a selection of transforms that do not modify the XML if applied.
        [Theory]
        [InlineData(typeof(XmlDsigC14NTransform))]
        [InlineData(typeof(XmlDsigExcC14NTransform))]
        [InlineData(typeof(XmlDsigC14NWithCommentsTransform))]
        public void PropagatedNamespaces(Type type)
        {
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateElement("foo", "urn:foo"));
            doc.DocumentElement.AppendChild(doc.CreateElement("bar", "urn:bar"));
            Assert.Equal(string.Empty, doc.DocumentElement.GetAttribute("xmlns:f"));
            Transform transform = type.GetConstructor(Type.EmptyTypes).Invoke(new object[0]) as Transform;
            transform.LoadInput(doc);
            transform.PropagatedNamespaces.Add("f", "urn:foo");
            transform.PropagatedNamespaces.Add("b", "urn:bar");
            using (Stream stream = transform.GetOutput(typeof(Stream)) as Stream)
            using (StreamReader streamReader = new StreamReader(stream, Encoding.UTF8))
            {
                string result = streamReader.ReadToEnd();
                Assert.Equal(result,
                    "<foo xmlns=\"urn:foo\"><bar xmlns=\"urn:bar\"></bar></foo>");
                Assert.Equal("urn:foo", doc.DocumentElement.NamespaceURI);
            }
        }
#endif
    }
}
