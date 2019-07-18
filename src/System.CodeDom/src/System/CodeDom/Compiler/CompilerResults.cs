// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Specialized;
using System.Reflection;

namespace System.CodeDom.Compiler
{
    public partial class CompilerResults
    {
        public CompilerResults(TempFileCollection tempFiles)
        {
            TempFiles = tempFiles;
        }

        public TempFileCollection TempFiles { get; set; }

        public Assembly CompiledAssembly { get; set; }

        public CompilerErrorCollection Errors { get; } = new CompilerErrorCollection();

        public StringCollection Output { get; } = new StringCollection();

        public string PathToAssembly { get; set; }

        public int NativeCompilerReturnValue { get; set; }
    }
}
