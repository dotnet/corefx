﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;

namespace OLEDB.Test.ModuleCore
{
    ////////////////////////////////////////////////////////////////
    // CVariation
    //
    ////////////////////////////////////////////////////////////////
    public delegate int TestFunc();
    public class CVariation : CTestBase
    {
        //Data
        private TestFunc _pfunc = null;
        //Constructor
        public CVariation(CTestCase testcase)
            : this(testcase, null)
        {
        }

        public CVariation(CTestCase testcase, string desc)
            : this(testcase, desc, "Variation_" + (testcase.GetVariationCount() + 1))
        {
            //Delegate
            //By default - if you don't specify the function name to run for this variation
            //it creates Variation_X, where X is the next variation for this testcase...
        }

        public CVariation(CTestCase testcase, string desc, string function)
            : base(function, desc)
        {
            //Note: The parent automatically gets setup on AddVariation so we don't 
            //really need to pass in the testcase, left here for backward compatibilty 
            //of inherited drivers.
        }

        public CVariation(CTestCase testcase, string desc, TestFunc function)
            : base(null, desc)
        {
            //Note: The parent automatically gets setup on AddVariation so we don't 
            //really need to pass in the testcase, left here for backward compatibilty 
            //of inherited drivers.
            _pfunc = function;
        }
        public CVariation(TestFunc func)
            : base(null, null)
        {
            _pfunc = func;
        }

        public CTestCase TestCase
        {
            get { return (CTestCase)Parent; }
            set { Parent = value; }
        }

        protected override CAttrBase CreateAttribute()
        {
            return new Variation();
        }

        public override tagVARIATION_STATUS Execute()
        {
            if (TestCase != null)
            {
                TestCase.CurVariation = this;
            }

            if (_pfunc != null)
            {
                int ret = _pfunc();
                return (tagVARIATION_STATUS)ret;
            }
            throw new Exception("test method is null in CVariation");
        }
    }
}
