// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;

namespace System
{
    public sealed class AppDomainSetup
    {
        private string _applicationBaseValue;
 
        internal AppDomainSetup()
        {
            ApplicationBase = AppContext.BaseDirectory;
            TargetFrameworkName = AppContext.TargetFrameworkName;
        }

        public string ApplicationBase
        {
            get
            {
                return _applicationBaseValue;
            }

            set
            {
                _applicationBaseValue = (value == null || value.Length == 0) ? null : Path.GetFullPath(value);
            }
        }

        public string TargetFrameworkName { get; }
    }
}
