using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace XPathTests.Common
{
    public static class FileHelper
    {
        public static Stream CreateStreamFromFile(string xml)
        {
            var xmlPath = Utils.ResourceFilesPath + xml;
            Stream s = typeof(FileHelper).GetTypeInfo().Assembly.GetManifestResourceStream(xmlPath);
            if (s == null)
            {
                throw new Exception("Couldn't find resource.");
            }
            return s;
        }
    }
}
