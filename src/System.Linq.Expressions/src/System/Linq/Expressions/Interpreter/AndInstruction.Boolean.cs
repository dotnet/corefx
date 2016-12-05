// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Linq.Expressions.Interpreter
{
    internal partial class AndInstruction
    {
        private sealed class AndBoolean : AndInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                object right = frame.Pop();
                object left = frame.Pop();
                if (left == null)
                {
                    if (right == null)
                    {
                        frame.Push(null);
                    }
                    else
                    {
                        frame.Push((bool)right ? null : Utils.BoxedFalse);
                    }
                    return 1;
                }
                else if (right == null)
                {
                    frame.Push((bool)left ? null : Utils.BoxedFalse);
                    return 1;
                }
                frame.Push(((bool)left) & ((bool)right));
                return 1;
            }
        }
    }
}