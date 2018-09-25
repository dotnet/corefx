// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.Diagnostics
{
    public partial class Process : IDisposable
    {
        public void Kill(bool entireProcessTree)
        {
            if (!entireProcessTree)
            {
                Kill();
            }
            else
            {
                // Ensures that an InvalidOperationException is thrown if the process hasn't started yet -- present to mimic the behavior of Kill()
                EnsureState(State.Associated);

                try
                {
                    KillTree();
                }
                catch (Exception e)
                {
                    // This method is to be fail-safe so exceptions should not escape from it. 
                    //
                    // Some exceptions that could be encountered will only be thrown in race or difficult to simulate situations, making 
                    // it impractical to use tests to trigger these exceptions and verify that they are caught. Without such tests, it's possible 
                    // that code changes elsewhere could change the list of exceptions this method might encounter this method being updated. 
                    //
                    // To ensure this method honors its 'fail-safe' contract, it catches all exceptions when in production. In development, 
                    // unexpected exceptions cause failures, encouraging the developer to update the explicit catches, as appropriate.

                    Debug.Fail("Unexpected exception encountered", e.ToString());
                }
            }
        }

        private void KillChildren(IReadOnlyList<Process> children)
        {
            try
            {
                foreach (Process childProcess in children)
                {
                    childProcess.KillTree();
                }
            }
            finally
            {
                foreach (Process childProcess in children)
                {
                    childProcess.Dispose();
                }
            }
        }
    }
}
