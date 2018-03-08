﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Configuration.Tests
{
    public class SettingElementTests
    {
        [Fact]
        public void TestForInequality()
        {
            var ElementOne = new SettingElement("NotEqualOne", SettingsSerializeAs.String);
            var ElementTwo = new SettingElement("NotEqualTwo", SettingsSerializeAs.String);
            Assert.False(ElementOne.Equals(ElementTwo));
        }

        [Fact]
        public void TestForEquality()
        {
            var ElementOne = new SettingElement("TheExactSameName", SettingsSerializeAs.String);
            var ElementTwo = new SettingElement("TheExactSameName", SettingsSerializeAs.String);
            Assert.True(ElementOne.Equals(ElementTwo));
        }

        [Fact]
        public void DefaultSettingSerializationIsString()
        {
            var Element = new SettingElement();
            Assert.Equal(SettingsSerializeAs.String, Element.SerializeAs);
        }

        [Fact]
        public void DefaultNameIsEmptyString()
        {
            var Element = new SettingElement();
            Assert.Equal(string.Empty, Element.Name);
        }

        [Fact]
        public void DefaultValueIsNull()
        {
            var Element = new SettingElement();
            Assert.Equal(null, Element.Value.CurrentConfiguration);
        }
    
        [Fact]
        public void DefaultSettingElementExceptionForGetHashCode()
        {
            var Element = new SettingElement();
            Assert.Throws<NullReferenceException>(() => Element.GetHashCode());
            //likely a bug
        }

        [Fact]
        public void NonDefaultValueHasNonNullHashCode()
        {
            var Element = new SettingElement("Test", SettingsSerializeAs.Xml)
            {
                Value = new SettingValueElement
                {
                    ValueXml = new ConfigXmlDocument
                    {
                    }
                }
            };
            Assert.NotNull(Element.GetHashCode());
        }
    }
}
