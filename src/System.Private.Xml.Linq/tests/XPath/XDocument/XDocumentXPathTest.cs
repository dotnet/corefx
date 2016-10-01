// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using XPathTests.Common;

namespace XPathTests.Common
{
    public static partial class Utils
    {
        private readonly static ICreateNavigator _navigatorCreator = new CreateNavigatorComparer();
        public readonly static string ResourceFilesPath = "System.Xml.XPath.XDocument.Tests.TestData.";
    }
}
