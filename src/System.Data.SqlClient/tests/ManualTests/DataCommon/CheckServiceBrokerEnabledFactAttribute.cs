// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace System.Data.SqlClient.ManualTesting.Tests
{
    public class CheckServiceBrokerEnabledFactAttribute : FactAttribute
    {
        public CheckServiceBrokerEnabledFactAttribute()
        {
            if (!DataTestUtility.AreConnStringsSetup())
            {
                Skip = "Connection Strings Not Setup";
            }
            else if (!DataTestUtility.IsServiceBrokerEnabled())
            {
                Skip = "Service Broker Not Enabled";
            }
        }
    }
}
