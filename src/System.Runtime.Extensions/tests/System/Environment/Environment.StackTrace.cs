// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace System.Tests
{
    public class EnvironmentStackTrace
    {
        static string s_stackTrace;

        [Fact]
        public void StackTraceTest()
        {
            //arrange
            List<string> knownFrames = new List<string>()
            {
                "System.Tests.EnvironmentStackTrace.StaticFrame(Object obj)",
                "System.Tests.EnvironmentStackTrace.TestClass..ctor()",
                "System.Tests.EnvironmentStackTrace.GenericFrame[T1,T2](T1 t1, T2 t2)",
                "System.Tests.EnvironmentStackTrace.TestFrame()"
            };

            //act
            Task.Run(() => TestFrame()).Wait();

            //assert
            int index = 0;
            foreach (string frame in knownFrames)
            {
                index = s_stackTrace.IndexOf(frame, index);
                Assert.True(index > -1);
                index += frame.Length;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void TestFrame()
        {
            GenericFrame<DateTime, StringBuilder>(DateTime.Now, null);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void GenericFrame<T1, T2>(T1 t1, T2 t2)
        {
            var test = new TestClass();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void StaticFrame(object obj)
        {
            s_stackTrace = Environment.StackTrace;
        }

        class TestClass
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            public TestClass()
            {
                EnvironmentStackTrace.StaticFrame(null);
            }
        }
    }
}
