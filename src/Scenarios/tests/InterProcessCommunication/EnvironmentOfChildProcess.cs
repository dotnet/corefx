// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using Xunit;

namespace InterProcessCommunication.Tests
{
    public class EnvironmentOfChildProcess : RemoteExecutorTestBase
    {
        [Fact]
        public void Test()
        {
            var expectedEnv = new HashSet<string>();
            var actualEnv = new HashSet<string>();

            using (var inbound = new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable))
            using (var reader = new StreamReader(inbound))
            using (var remote = RemoteInvoke(GetEnvironmetOfChildProcess, inbound.GetClientHandleAsString()))
            {
                inbound.DisposeLocalCopyOfClientHandle();

                foreach (KeyValuePair<string, string> envVar in remote.Process.StartInfo.Environment)
                {
                    actualEnv.Add(envVar.Key + "=" + envVar.Value);
                }

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    expectedEnv.Add(line);
                }
            }

            // Validate against StartInfo.Environment
            if (!expectedEnv.SetEquals(actualEnv))
            {
                var expected = string.Join(", ", expectedEnv.Except(actualEnv));
                var actual = string.Join(", ", actualEnv.Except(expectedEnv));

                Assert.True(false, string.Format("Expected: {0}{1}Actual: {2}", expected, Environment.NewLine, actual));
            }

            // Validate against current process
            var currentProcEnv = new HashSet<string>();
            foreach (DictionaryEntry envVar in Environment.GetEnvironmentVariables())
            {
                currentProcEnv.Add(envVar.Key + "=" + envVar.Value);
            }

            // Profilers / code coverage tools can add own environment variables but we start
            // child process without them. Thus the set of variables from child process will
            // compose subset of variables from current process.
            // But in case if tests running directly through the Xunit runner, sets will be equal
            // and Assert.ProperSubset will throw. We add null to avoid this.
            currentProcEnv.Add(null);
            Assert.ProperSubset(currentProcEnv, actualEnv);
        }

        private static int GetEnvironmetOfChildProcess(string outHandle)
        {
            using (var outbound = new AnonymousPipeClientStream(PipeDirection.Out, outHandle))
            using (var writer = new StreamWriter(outbound))
            {
                foreach (DictionaryEntry envVar in Environment.GetEnvironmentVariables())
                {
                    writer.WriteLine(envVar.Key + "=" + envVar.Value);
                }
            }

            return SuccessExitCode;
        }
    }
}
