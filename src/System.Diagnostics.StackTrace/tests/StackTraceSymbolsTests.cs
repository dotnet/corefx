// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using Xunit;

namespace System.Diagnostics.SymbolStore.Tests
{
    public class StackTraceSymbolsTests
    {
        [Fact]
        public void StackTraceSymbolsDoNotLockFile()
        {
            var asmPath = typeof(StackTraceSymbolsTests).Assembly.Location;
            var pdbPath = Path.ChangeExtension(asmPath, ".pdb");
            var targetPath = Path.ChangeExtension(pdbPath, ".moved");

            Assert.True(File.Exists(pdbPath));

            try 
            {
                throw new Exception();
            }
            catch (Exception ex)
            {
                // Force the PDB to be loaded
                Assert.NotNull(ex.StackTrace);
            }

            File.Move(pdbPath, targetPath);
            Assert.True(File.Exists(targetPath));
        }
    }
}
