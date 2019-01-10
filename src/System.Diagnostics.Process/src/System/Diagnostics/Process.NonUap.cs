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

                if (IsInTreeOf(GetCurrentProcess()))
                    throw new InvalidOperationException(SR.KillEntireProcessTree_DisallowedBecauseTreeContainsCallingProcess);

                IEnumerable<Exception> result = KillTree();

                if (result.Any())
                    throw new AggregateException(SR.KillEntireProcessTree_TerminationIncomplete, result);
            }
        }

        private bool IsInTreeOf(Process processOfInterest)
        {
            if (SafePredicateTest(() => Equals(processOfInterest)))
                return true;

            Process[] processes = GetProcesses();

            try
            {
                var queue = new Queue<Process>();
                Process current = this;

                do
                {
                    IEnumerable<Process> immediateChildren = processes
                        .Where(p => SafePredicateTest(() => current.IsParentOf(p)));

                    if (immediateChildren.Any(c => SafePredicateTest(() => c.Equals(processOfInterest))))
                        return true;

                    foreach (Process child in immediateChildren)
                        queue.Enqueue(child);
                } while (queue.TryDequeue(out current));
            }
            finally
            {
                foreach (Process process in processes)
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
