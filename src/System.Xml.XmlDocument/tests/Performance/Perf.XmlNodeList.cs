﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Xml;
using Microsoft.Xunit.Performance;

namespace XmlDocumentTests.XmlNodeListTests
{
    public class Perf_XmlNodeList
    {
        private const int innerIterations = 10000;

        [Benchmark]
        public void Enumerator()
        {
            // Setup
            var doc = new XmlDocument();
            doc.LoadXml("<a><sub1/><sub1/><sub2/><sub1/><sub2/><sub1/><sub2/><sub1/><sub2/><sub1/><sub2/><sub2/></a>");
            XmlNodeList list = doc.DocumentElement.ChildNodes;

            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    for (int i = 0; i < innerIterations; i++)
                        foreach (var element in list) { }
        }

        [Benchmark]
        public void GetCount()
        {
            // Setup
            var doc = new XmlDocument();
            doc.LoadXml("<a><sub1/><sub2/></a>");
            XmlNodeList list = doc.DocumentElement.ChildNodes;
            int count;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterations; i++)
                    {
                        count = list.Count; count = list.Count; count = list.Count;
                        count = list.Count; count = list.Count; count = list.Count;
                        count = list.Count; count = list.Count; count = list.Count;
                    }
                }
            }
        }
    }
}
