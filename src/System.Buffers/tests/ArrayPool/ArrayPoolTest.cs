// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.Buffers.ArrayPool.Tests
{
    public class ArrayPoolTest : RemoteExecutorTestBase
    {
        protected const string TrimSwitchName = "DOTNET_SYSTEM_BUFFERS_ARRAYPOOL_TRIMSHARED";

        protected static void RunWithCollectionDisabled(Action action)
        {
            RemoteInvokeOptions options = new RemoteInvokeOptions();
            options.StartInfo.UseShellExecute = false;
            options.StartInfo.EnvironmentVariables.Add(TrimSwitchName, "false");

            RemoteInvoke(action).Dispose();
        }
    }
}
