// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using Xunit;
using Microsoft.XmlSerializer.Generator;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Microsoft.XmlSerializer.Generator.Tests
{
    public static class SgenTests
    {
        [Fact]
        public static void BasicTest()
        {
            int n = Sgen.Main(null);
            Assert.Equal(0, n);
        }

        [Fact]
        public static void GeneralBaselineTest()
        {
            string codefile = "System.Xml.XmlSerializer.Sgen.Tests.XmlSerializers.cs";
            if (File.Exists(codefile))
            {
                File.Delete(codefile);
            }

            int n = Sgen.Main(new string[] { @"..\..\System.Xml.XmlSerializer.Sgen.Tests\netstandard\System.Xml.XmlSerializer.Sgen.Tests.dll", "/force", "/out:..\\..\\Microsoft.XmlSerializer.Generator.Tests\\netstandard\\" });
            Assert.Equal(0, n);
            if(!File.Exists(codefile))
            {
                throw new FileNotFoundException(string.Format("Fail to generate {0}.", codefile));
            }

            string basefile = @"baseline\" + codefile;
            if(!File.Exists(basefile))
            {
                throw new FileNotFoundException(string.Format("Missing baseline file {0}.", basefile));
            }

            List<string> allbaselines = GetAllLines(basefile);
            List<string> alllines = GetAllLines(codefile);
            CompareList(allbaselines, alllines);
        }

        private static List<String> GetAllLines(string strFileName)
        {
            List<string> methodNames = new List<string>();
            var alllines = File.ReadAllLines(strFileName);
            var newalllines = new List<string>();
            foreach (var line in alllines)
            {
                if (line == String.Empty || line.Contains("#pragma"))
                {
                    continue;
                }

                newalllines.Add(line.Trim());
            }

            return newalllines;
        }

        private static void CompareList(List<string> expectedlist, List<string> actuallist)
        {
            if (expectedlist == null)
            {
                throw new InvalidDataException("The expected list is null");
            }

            if (actuallist == null)
            {
                throw new InvalidDataException("The actual list is null");
            }

            Assert.Equal(expectedlist.Count(), actuallist.Count());

            expectedlist.Sort();
            actuallist.Sort();

            for (int i = 0; i < expectedlist.Count(); ++i)
            {
                if(expectedlist[i] != actuallist[i])
                {
                    if(expectedlist[i].Contains("XmlSerializerVersionAttribute") && expectedlist[i].Contains("ParentAssemblyId")
                        && actuallist[i].Contains("XmlSerializerVersionAttribute") && actuallist[i].Contains("ParentAssemblyId"))
                    {
                        continue;
                    }
                    else
                    {
                        Assert.True(false, string.Format("The generated file doesn't match with the baseline file, Expected {0}, Actual {1}", expectedlist[i], actuallist[i]));
                    }
                }
            }
        }
    }
}
