// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using Xunit;

namespace System.Net.Test.Common
{
    internal static class RemoteServerQuery
    {
        internal static async Task Run(Func<Task> testCode, Type remoteServerFailExceptionType, string servername, bool mustFail = true)
        {
            try
            {
                await testCode();
            }
            catch (Exception actualException)
            {
                if (remoteServerFailExceptionType.Equals(actualException.GetType()))
                {
                    if (mustFail)
                        Assert.True(false, $"External issue with remote server: {servername}");

                    return;
                }

                throw;
            }
        }

        internal static async Task<TResult> Run<TResult>(Func<Task<TResult>> testCode, Type remoteServerFailExceptionType, string servername, bool mustFail = true)
        {
            try
            {
                return await testCode();
            }
            catch (Exception actualException)
            {
                if (remoteServerFailExceptionType.Equals(actualException.GetType()))
                {
                    if (mustFail)
                        Assert.True(false, $"External issue with remote server: {servername}, test threw {remoteServerFailExceptionType}");

                    return default(TResult);
                }

                throw;
            }
        }

        internal static async Task ThrowsAsync<T>(Func<Task> testCode, Type remoteServerFailExceptionType, string servername, bool mustFail = true)
            where T : Exception
        {
            try
            {
                await testCode();
                Assert.True(false, $"Expected: {typeof(T)}, Actual: No exception thrown.");
            }
            catch (Exception actualException)
            {
                Type actualExceptionType = actualException.GetType();
                if (typeof(T).Equals(actualExceptionType))
                {
                    return;
                }
                else if (remoteServerFailExceptionType.Equals(actualExceptionType))
                {
                    if (mustFail)
                        Assert.True(false, $"External issue with remote server: {servername}");

                    return;
                }

                throw;
            }
        }
    }
}
