// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.Composition.Factories;
using System.ComponentModel.Composition.Primitives;
using Xunit;

namespace System.ComponentModel.Composition.Hosting
{
    public class SerializableCompositionElementTests
    {
        [Fact]
        public void FromICompositionElement_NullAsElementArgument_ShouldReturnNull()
        {
            var result = SerializableCompositionElement.FromICompositionElement((ICompositionElement)null);

            Assert.Null(result);
        }

        [Fact]
        public void FromICompositionElement_ValueAsElementArgument_ShouldSetDisplayNameProperty()
        {
            var expectations = Expectations.GetDisplayNames();

            foreach (var e in expectations)
            {
                var element = ElementFactory.Create(e);

                var result = SerializableCompositionElement.FromICompositionElement(element);

                Assert.Equal(e, result.DisplayName);
            }
        }

        [Fact]
        public void FormICompositionElement_ValueWithNullOriginAsElementArgument_ShouldSetOriginPropertyToNull()
        {
            var element = ElementFactory.Create((ICompositionElement)null);
            Assert.Null(element.Origin);

            var result = SerializableCompositionElement.FromICompositionElement(element);

            Assert.Null(element.Origin);
        }

        [Fact]
        public void FromICompositionElement_ValueAsElementArgument_ShouldSetOriginProperty()
        {
            var expectations = Expectations.GetCompositionElements();

            foreach (var e in expectations)
            {
                var element = ElementFactory.Create(e);

                var result = SerializableCompositionElement.FromICompositionElement(element);

                ICompositionElement expected = e, actual = result.Origin;
                while (expected != null && actual != null)
                {
                    Assert.Equal(e.DisplayName, result.Origin.DisplayName);
                    expected = expected.Origin;
                    actual = actual.Origin;
                }

                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void ToString_ShouldReturnDisplayNameProperty()
        {
            var expectations = Expectations.GetDisplayNames();

            foreach (var e in expectations)
            {
                var element = ElementFactory.Create(e);

                var result = SerializableCompositionElement.FromICompositionElement(element);

                Assert.Equal(e, result.ToString());
            }
        }
        
        private static SerializableCompositionElement CreateSerializableCompositionElement(string displayName)
        {
            var element = ElementFactory.Create(displayName);

            return CreateSerializableCompositionElement(element);
        }

        private static SerializableCompositionElement CreateSerializableCompositionElement(ICompositionElement element)
        {
            return (SerializableCompositionElement)SerializableCompositionElement.FromICompositionElement(element);
        }
    }
}
