namespace System.Configuration
{

    using Xunit;

    public class SettingElementTests
    {

        [Fact]
        public void TestForInequality()
        {
            SettingElement ElementOne = new SettingElement("NotEqualOne", SettingsSerializeAs.String);
            SettingElement ElementTwo = new SettingElement("NotEqualTwo", SettingsSerializeAs.String);
            Assert.False(ElementOne.Equals(ElementTwo));
        }

        [Fact]
        public void TestForEquality()
        {
            SettingElement ElementOne = new SettingElement("TheExactSameName", SettingsSerializeAs.String);
            SettingElement ElementTwo = new SettingElement("TheExactSameName", SettingsSerializeAs.String);
            Assert.True(ElementOne.Equals(ElementTwo));
        }

        [Fact]
        public void AssureDefaultSettingSerializationIsString()
        {
            SettingElement Element = new SettingElement();
            Assert.Equal(Element.SerializeAs, SettingsSerializeAs.String);
        }

        [Fact]
        public void AssureDefaultNameIsEmptyString()
        {
            SettingElement Element = new SettingElement();
            Assert.Equal(Element.Name, string.Empty);
        }

        [Fact]
        public void AssureDefaultValueIsNull()
        {
            SettingElement Element = new SettingElement();
            Assert.Equal(Element.Value.CurrentConfiguration, null);
        }
    
        [Fact]
        public void DefaultSettingElementExceptionForGetHashCode()
        {
            SettingElement Element = new SettingElement();
            Assert.Throws<NullReferenceException>(() => Element.GetHashCode());
        }

        [Fact]
        public void NonDefaultValueHasNonNullHashCode()
        {
            SettingElement Element = new SettingElement("Test", SettingsSerializeAs.Xml)
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
