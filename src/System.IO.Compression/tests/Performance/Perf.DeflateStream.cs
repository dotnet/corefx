// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Xunit;
using Microsoft.Xunit.Performance;
using System.Collections.Generic;

namespace System.IO.Compression.Tests
{
    public class Perf_DeflateStream : PerfTestBase
    {
        /// <summary>
        /// Yields an Enumerable list of paths to GZTestData files
        /// </summary>
        protected static IEnumerable<string> GZTestFiles()
        {
            yield return "GZTestData/GZTestDocument.txt.gz";
            yield return "GZTestData/GZTestDocument.docx.gz";
        }

        [Benchmark]
        [MemberData("GZTestFiles")]
        public async void Decompress(string testFilePath)
        {
            int _bufferSize = 1024;
            var bytes = new Byte[_bufferSize];
            foreach (var iteration in Benchmark.Iterations)
            {
                var gzStream = await LocalMemoryStream.readAppFileAsync(testFilePath);
                var strippedMs = StripHeaderAndFooter.Strip(gzStream);
                var zip = new DeflateStream(strippedMs, CompressionMode.Decompress);

                int retCount = -1;
                using (iteration.StartMeasurement())
                {
                    while (retCount != 0)
                    {
                        retCount = zip.Read(bytes, 0, _bufferSize);
                    }
                }
            }
        }

        [Benchmark]
        [InlineData(100)]
        [InlineData(1000)]
        public void Compress(int numberOfWrites)
        {
            var bytes = Text.Encoding.UTF8.GetBytes(CreateString(100));
            foreach (var iteration in Benchmark.Iterations)
            {
                Stream output = File.Create(GetTestFilePath());
                var zip = new DeflateStream(output, CompressionMode.Compress);

                int writeCount = 0;
                using (iteration.StartMeasurement())
                {
                    while (writeCount < numberOfWrites)
                    {
                        zip.Write(bytes, 0, bytes.Length);
                    }
                }
            }
        }
    }
}
