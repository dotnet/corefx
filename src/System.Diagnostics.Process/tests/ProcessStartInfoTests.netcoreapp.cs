// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Diagnostics.Tests
{
    public partial class ProcessStartInfoTests
    {
        [Fact]
        public void UnintializedArgumentList()
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            Assert.Equal(0, psi.ArgumentList.Count);

            psi = new ProcessStartInfo("filename", "-arg1 -arg2");
            Assert.Equal(0, psi.ArgumentList.Count);
        }

        [Fact]
        public void InitializeWithArgumentList()
        {
            ProcessStartInfo psi = new ProcessStartInfo("filename");
            psi.ArgumentList.Add("arg1");
            psi.ArgumentList.Add("arg2");

            Assert.Equal(2, psi.ArgumentList.Count);
            Assert.Equal("arg1", psi.ArgumentList[0]);
            Assert.Equal("arg2", psi.ArgumentList[1]);
        }

        [Fact]
        public void InitializeWithArgumentList_Null()
        {
           // ProcessStartInfo psi = new ProcessStartInfo("filename", (IReadOnlyCollection<string>)null);
           // Assert.Equal(0, psi.ArgumentList.Count);
        }
    }
}
