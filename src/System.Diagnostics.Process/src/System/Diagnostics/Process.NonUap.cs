// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

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
                EnsureState(State.Associated | State.IsLocal | State.HaveNonExitedId);

                if (IsSelfOrDescendant(GetCurrentProcess()))
                    throw new InvalidOperationException(SR.KillEntireProcessTree_DisallowedBecauseTreeContainsCallingProcess);

                IEnumerable<Exception> result = KillTree();

                if (result.Any())
                    throw new AggregateException(SR.KillEntireProcessTree_TerminationIncomplete, result);
            }
        }

        private bool IsSelfOrDescendant(Process processOfInterest)
        {
            if (SafePredicateTest(() => Equals(processOfInterest)))
                return true;

            Process[] allProcesses = GetProcesses();

            try
            {
                var descendantProcesses = new Queue<Process>();
                Process currentDescendant = this;

                do
                {
                    foreach (Process process in allProcesses)
                    {
                        if (SafePredicateTest(() => !currentDescendant.IsParentOf(process)))
                            continue;

                        if (SafePredicateTest(() => processOfInterest.Equals(process)))
                            return true;

                        descendantProcesses.Enqueue(process);
                    }
                } while (descendantProcesses.TryDequeue(out currentDescendant));
            }
            finally
            {
                foreach (Process process in allProcesses)
                {
                    process.Dispose();
                }
            }

            return false;
        }

        private bool SafePredicateTest(Func<bool> predicate)
        {
            try
            {
                return predicate();
            }
            catch (Exception e) when (e is InvalidOperationException || e is Win32Exception)
            {
                // InvalidOperationException signifies conditions such as the process already being dead.
                // Win32Exception signifies issues such as insufficient permissions to get details on the process.
                // In either case, the predicate couldn't be applied so return the fallback result. 
                return false;
            }
        }
    }
}
