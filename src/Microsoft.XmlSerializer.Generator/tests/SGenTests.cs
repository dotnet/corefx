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
            int n = Sgen.Main(new string[0]);
            Assert.Equal(0, n);
        }

        [Fact]
        public static void GeneralBaselineTest()
        {
            string codefile = "System.Xml.XmlSerializer.Sgen.Data.XmlSerializers.cs";
            if (File.Exists(codefile))
            {
                File.Delete(codefile);
            }

            Assert.True(File.Exists("System.Xml.XmlSerializer.Sgen.Data.dll"), string.Format("Cannot find file {0}.", "System.Xml.XmlSerializer.Sgen.Data.dll"));
            int n = Sgen.Main(new string[] { "System.Xml.XmlSerializer.Sgen.Data.dll", "/force", "/casesensitive"});
            Assert.Equal(0, n);
            Assert.True(File.Exists(codefile), string.Format("Fail to generate {0}.", codefile));
            string basefile = Path.Combine(Directory.GetCurrentDirectory(), "baseline", "System.Xml.XmlSerializer.Sgen.Data.XmlSerializers.cs");
            Assert.True(File.Exists(basefile), string.Format("Missing baseline file {0}.", basefile));


            List<string> allbaseclasses = GetAllClassdNames(basefile);
            List<string> allclasses = GetAllClassdNames(codefile);
            CompareList(allbaseclasses, allclasses);
            List<string> allbasemethods = GetAllMethodNames(basefile);
            List<string> allmethods = GetAllMethodNames(codefile);
            CompareList(allbasemethods, allmethods);
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
                if (line == String.Empty)
                {
                    continue;
                }

                newalllines.Add(line.Trim());
            }

            return newalllines;
        }

        public static List<string> GetAllMethodNames(string strFileName)
        {
            List<string> methodNames = new List<string>();
            var strMethodLines = GetAllLines(strFileName)
                            .Where(a => (a.Contains("protected") ||
                                         a.Contains("private") ||
                                         a.Contains("public")) &&
                                         !a.Contains("class"));
            foreach (var item in strMethodLines)
            {
                if (item.IndexOf("(") != -1)
                {
                    string strTemp = String.Join("", item.Substring(0, item.IndexOf("(")).Reverse());
                    methodNames.Add(String.Join("", strTemp.Substring(0, strTemp.IndexOf(" ")).Reverse()));
                }
            }

            methodNames.Sort();
            return methodNames;
        }

        public static List<string> GetAllClassdNames(string strFileName)
        {
            List<string> classdNames = new List<string>();
            var strMethodLines = GetAllLines(strFileName)
            .Where(a => (a.Contains("protected") ||
            a.Contains("private") ||
            a.Contains("public")) &&
            a.Contains("class"));
            foreach (var item in strMethodLines)
            {
                if (item.IndexOf("class") != -1)
                {
                    string strTemp = String.Join("", item.Substring(0, item.IndexOf(":")).Reverse());
                    classdNames.Add(String.Join("", strTemp.Substring(0, strTemp.IndexOf("ssalc")).Reverse()));
                }
            }

            classdNames.Sort();
            return classdNames;
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
