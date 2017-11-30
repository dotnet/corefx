// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace System.IO.Tests
{
    public class EnumerableTests : FileSystemTest
    {
        [Fact]
        [ActiveIssue(25613, TestPlatforms.AnyUnix)]
        public void FileEnumeratorIsThreadSafe()
        {
            string directory = Directory.CreateDirectory(GetTestFilePath()).FullName;
            for (int i = 0; i < 100; i++)
                File.Create(Path.Combine(directory, GetTestFileName())).Dispose();

            new ThreadSafeRepro().Execute(directory);
        }

        class ThreadSafeRepro
        {
            volatile IEnumerator<string> _enumerator;

            void Enumerate(IEnumerator<string> s)
            {
                while (s.MoveNext())
                { }
                s.Dispose();
            }

            public void Execute(string directory)
            {
                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;

                void Work()
                {
                    do
                    {
                        IEnumerator<string> x = _enumerator;
                        if (x != null)
                            Enumerate(x);
                    } while (!token.IsCancellationRequested);
                    token.ThrowIfCancellationRequested();
                }

                new Task(Work, token).Start();
                new Task(Work, token).Start();
                for (int i = 0; i < 1000; i++)
                {
                    _enumerator = Directory.EnumerateFiles(directory).GetEnumerator();
                    Enumerate(_enumerator);
                }
                source.Cancel();
            }
        }
    }
}
