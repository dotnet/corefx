using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit.Sdk;

namespace System.Diagnostics.Tests
{
    internal static class Helpers
    {
        public static async Task RetryWithBackoff(Action action, int delayInMilliseconds = 10, int times = 10)
        {
            for (; times > 0; times--)
            {
                try
                {
                    action();
                    return;
                }
                catch (XunitException) when (times > 1)
                {
                    await Task.Delay(delayInMilliseconds);
                    delayInMilliseconds *= 2;
                }
            }
        }
    }
}
