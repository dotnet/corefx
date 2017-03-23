// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace System.Tests
{
    public static partial class ArrayTests
    {
        
        [Fact]
        public static unsafe void GetValue_ArrayOfPointers_ThrowsNotSupportedException()
        {
            Assert.Throws<NotSupportedException>(() => new int*[2].GetValue(0));
        }

        [Fact]
        public static unsafe void SetValue_ArrayOfPointers_ThrowsNotSupportedException()
        {
            Assert.Throws<NotSupportedException>(() => new int*[2].SetValue(null, 0));
        }

        [Fact]
        public static unsafe void Copy_PointerArrayToNonPointerArray_ThrowsArrayTypeMismatchException()
        {
            Copy_SourceAndDestinationNeverConvertible_ThrowsArrayTypeMismatchException(new int[1], new int*[1]);
            Copy_SourceAndDestinationNeverConvertible_ThrowsArrayTypeMismatchException(new int*[1], new int[1]);
        }

        [Fact]
        public static unsafe void GetEnumerator_ArrayOfPointers_ThrowsNotSupportedException()
        {
            Array nonEmptyArray = new int*[2];
            Assert.Throws<NotSupportedException>(() => { foreach (object obj in nonEmptyArray) { } });

            Array emptyArray = new int*[0];
            foreach (object obj in emptyArray) { }
        }

        [Fact]
        public static unsafe void IndexOf_ArrayOfPointers_ThrowsNotSupportedException()
        {
            Assert.Throws<NotSupportedException>(() => Array.IndexOf((Array)new int*[2], null));
            Assert.Equal(-1, Array.IndexOf((Array)new int*[0], null));
        }

        [Fact]
        public static unsafe void LastIndexOf_ArrayOfPointers_ThrowsNotSupportedException()
        {
            Assert.Throws<NotSupportedException>(() => Array.LastIndexOf((Array)new int*[2], null));
            Assert.Equal(-1, Array.LastIndexOf((Array)new int*[0], null));
        }
        
        [Fact]
        public static unsafe void Reverse_ArrayOfPointers_ThrowsNotSupportedException()
        {
            Assert.Throws<NotSupportedException>(() => Array.Reverse((Array)new int*[2]));
            Array.Reverse((Array)new int*[0]);
            Array.Reverse((Array)new int*[1]);
        }
    }
}

