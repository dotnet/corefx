// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System
{
    /// <summary>
    /// An exception for when a test needs to throw a general exception.
    /// </summary>
    /// The intent of this exception is to be a "specific" thrown exception from a test,
    /// when the actual exception is immaterial to the class under test.
    ///
    /// That is, exception-checking tests don't have to throw/catch the base Exception,
    /// (which might mask something unexpected)
    /// and can verify the "unknown" error is being returned properly.
    /// (such as the inner exceptions of AggregateException)
    internal class DeliberateTestException : Exception
    {
    }
}
