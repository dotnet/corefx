using System.Xml.XPath;

namespace XPathTests.Common
{
    public interface ICreateNavigator
    {
        XPathNavigator CreateNavigatorFromFile(string fileName);
        XPathNavigator CreateNavigator(string xml);
    }
}