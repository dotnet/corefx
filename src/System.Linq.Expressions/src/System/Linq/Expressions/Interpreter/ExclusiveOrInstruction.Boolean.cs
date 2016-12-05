// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Linq.Expressions.Interpreter
{
    internal partial class ExclusiveOrInstruction
    {
        private sealed class ExclusiveOrBoolean : ExclusiveOrInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                object left = frame.Pop();
                object right = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(null);
                    return 1;
                }
                frame.Push(((bool)left) ^ ((bool)right));
                return 1;
            }
        }
    }
}