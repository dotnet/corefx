﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace OLEDB.Test.ModuleCore
{
    ////////////////////////////////////////////////////////////////
    // CTestSkippedException
    //
    // Since this is a very common exception and makes it so you
    // can catch only the skipped exceptions this is quite useful
    //
    ////////////////////////////////////////////////////////////////
    public class CTestSkippedException : CTestException
    {
        //Constructor
        public CTestSkippedException(string message)
            : this(message, false, true, null)
        {
        }

        public CTestSkippedException(string message, object actual, object expected, Exception inner)
            : base(CTestBase.TEST_SKIPPED, message, actual, expected, inner)
        {
        }
    }

    ////////////////////////////////////////////////////////////////
    // CTestFailedException
    //
    ////////////////////////////////////////////////////////////////
    public class CTestFailedException : CTestException
    {
        //Constructor
        public CTestFailedException(string message)
            : this(message, false, true, null)
        {
        }

        public CTestFailedException(string message, object actual, object expected, Exception inner)
            : base(CTestBase.TEST_FAIL, message, actual, expected, inner)
        {
        }
    }

    ////////////////////////////////////////////////////////////////
    // CTestException
    //
    // For all other cases, you can just setup the status directly
    ////////////////////////////////////////////////////////////////
    public class CTestException : Exception
    {
        //Data
        public int Result;
        public object Actual;
        public object Expected;

        //Constructor
        public CTestException(string message)
            : this(CTestBase.TEST_FAIL, message)
        {
        }

        public CTestException(int result, string message)
            : this(result, message, false, true, null)
        {
        }

        public CTestException(int result, string message, object actual, object expected, Exception inner)
            : base(message, inner)
        {
            //Note: iResult is the variation result (ie: TEST_PASS, TEST_FAIL, etc...)
            //Setup the exception
            Result = result;
            Actual = actual;
            Expected = expected;
        }
    }
}
