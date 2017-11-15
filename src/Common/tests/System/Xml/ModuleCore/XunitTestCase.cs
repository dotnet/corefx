﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OLEDB.Test.ModuleCore
{
    public class XunitTestCase
    {
        public string DisplayName { get; set; }
        public Func<tagVARIATION_STATUS> Test { get; set; }

        public XunitTestCase(string displayName, Func<tagVARIATION_STATUS> test)
        {
            DisplayName = displayName;
            Test = test;
        }

        public override string ToString()
        {
            return DisplayName;
        }

        public void Run()
        {
            try
            {
                tagVARIATION_STATUS result = (tagVARIATION_STATUS)Test();
                Assert.NotEqual(tagVARIATION_STATUS.eVariationStatusFailed, result);
            }
            catch (CTestSkippedException) { }
        }
    }
}
