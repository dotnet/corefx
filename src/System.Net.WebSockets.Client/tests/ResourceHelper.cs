// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Reflection;

using Xunit;

namespace System.Net.WebSockets.Client.Tests
{
    public class ResourceHelper
    {
        public static string GetExceptionMessage(string resourceName, params object[] parameters)
        {
            if (PlatformDetection.IsNetNative)
            {
                // .NET Native does not include the full exception message. Instead, the exception text
                // consists of the 'resourceName' along with a link to follow for more information.
                return string.Concat(resourceName, ". For more information, visit http://go.microsoft.com/fwlink/?LinkId=623485");
            }

            Type srType = typeof(SR);
            PropertyInfo property = srType.GetRuntimeProperties().Single(p => p.Name == resourceName);

            string errorMessageTemplate = (string)property.GetMethod.Invoke(null, null);

            Assert.False(string.IsNullOrEmpty(errorMessageTemplate),
                         string.Format("Localized error message '{0}' does not exist.", resourceName));

            return string.Format(errorMessageTemplate, parameters);
        }
    }
}
