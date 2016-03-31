﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xunit;

namespace System.Collections.Tests
{
    public class List_Generic_Tests_string : List_Generic_Tests<string>
    {
        protected override string CreateT(int seed)
        {
            int stringLength = seed % 10 + 5;
            Random rand = new Random(seed);
            byte[] bytes = new byte[stringLength];
            rand.NextBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
    }

    public class List_Generic_Tests_int : List_Generic_Tests<int>
    {
        protected override int CreateT(int seed)
        {
            Random rand = new Random(seed);
            return rand.Next();
        }
    }

    public class List_Generic_Tests_string_ReadOnly : List_Generic_Tests<string>
    {
        protected override string CreateT(int seed)
        {
            int stringLength = seed % 10 + 5;
            Random rand = new Random(seed);
            byte[] bytes = new byte[stringLength];
            rand.NextBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        protected override bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        protected override IList<string> GenericIListFactory(int setLength)
        {
            return GenericListFactory(setLength).AsReadOnly();
        }

        protected override IList<string> GenericIListFactory()
        {
            return GenericListFactory().AsReadOnly();
        }

        protected override IEnumerable<ModifyEnumerable> ModifyEnumerables { get { return new List<ModifyEnumerable>(); } }
    }

    public class List_Generic_Tests_int_ReadOnly : List_Generic_Tests<int>
    {
        protected override int CreateT(int seed)
        {
            Random rand = new Random(seed);
            return rand.Next();
        }

        protected override bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        protected override IList<int> GenericIListFactory(int setLength)
        {
            return GenericListFactory(setLength).AsReadOnly();
        }

        protected override IList<int> GenericIListFactory()
        {
            return GenericListFactory().AsReadOnly();
        }
        protected override IEnumerable<ModifyEnumerable> ModifyEnumerables { get { return new List<ModifyEnumerable>(); } }
    }
}
