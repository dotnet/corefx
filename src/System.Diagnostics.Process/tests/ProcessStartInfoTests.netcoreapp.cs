// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using Xunit;
using System.Text;
using System.ComponentModel;
using System.Security;
using System.Threading;

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
            List<string> argList = new List<string>();
            argList.Add("arg1");
            argList.Add("arg2");

            ProcessStartInfo psi = new ProcessStartInfo("filename", argList);
            Assert.Equal(2, psi.ArgumentList.Count);
        }

        [Fact]
        public void InitializeWithArgumentList_Null()
        {
            ProcessStartInfo psi = new ProcessStartInfo("filename", (IReadOnlyCollection<string>)null);
            Assert.Equal(0, psi.ArgumentList.Count);
        }
    }
}
