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

                KillTree();
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
