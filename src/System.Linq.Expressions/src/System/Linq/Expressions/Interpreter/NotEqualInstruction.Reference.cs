// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Linq.Expressions.Interpreter
{
    internal abstract partial class NotEqualInstruction : Instruction
    {
        private sealed class NotEqualReference : NotEqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                frame.Push(frame.Pop() != frame.Pop());
                return 1;
            }
        }
    }
}
