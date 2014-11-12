using XPathTests.Common;

namespace XPathTests.Common
{
    public static partial class Utils
    {
        private readonly static ICreateNavigator _navigatorCreator = new CreateNavigatorFromXmlDocument();
        public readonly static string ResourceFilesPath = "System.Xml.XPath.XmlDocument.Tests.TestData.";
    }
}
