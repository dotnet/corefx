// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Dynamic.Utils;

namespace System.Linq.Expressions.Interpreter
{
    internal abstract class NotEqualInstruction : Instruction
    {
        // Perf: EqualityComparer<T> but is 3/2 to 2 times slower.
        private static Instruction s_reference, s_Boolean, s_SByte, s_Int16, s_Char, s_Int32, s_Int64, s_Byte, s_UInt16, s_UInt32, s_UInt64, s_Single, s_Double;
        private static Instruction s_SByteLiftedToNull, s_Int16LiftedToNull, s_CharLiftedToNull, s_Int32LiftedToNull, s_Int64LiftedToNull, s_ByteLiftedToNull, s_UInt16LiftedToNull, s_UInt32LiftedToNull, s_UInt64LiftedToNull, s_SingleLiftedToNull, s_DoubleLiftedToNull;

        public override int ConsumedStack => 2;
        public override int ProducedStack => 1;
        public override string InstructionName => "NotEqual";

        private NotEqualInstruction() { }

        private sealed class NotEqualBoolean : NotEqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                object right = frame.Pop();
                object left = frame.Peek();
                frame.Replace(left == null | right == null ? left != right : (bool)left != (bool)right);
                return 1;
            }
        }

        private sealed class NotEqualSByte : NotEqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                object right = frame.Pop();
                object left = frame.Peek();
                frame.Replace(left == null | right == null ? left != right : (sbyte)left != (sbyte)right);
                return 1;
            }
        }

        private sealed class NotEqualInt16 : NotEqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                object right = frame.Pop();
                object left = frame.Peek();
                frame.Replace(left == null | right == null ? left != right : (short)left != (short)right);
                return 1;
            }
        }

        private sealed class NotEqualChar : NotEqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                object right = frame.Pop();
                object left = frame.Peek();
                frame.Replace(left == null | right == null ? left != right : (char)left != (char)right);
                return 1;
            }
        }

        private sealed class NotEqualInt32 : NotEqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                object right = frame.Pop();
                object left = frame.Peek();
                frame.Replace(left == null | right == null ? left != right : (int)left != (int)right);
                return 1;
            }
        }

        private sealed class NotEqualInt64 : NotEqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                object right = frame.Pop();
                object left = frame.Peek();
                frame.Replace(left == null | right == null ? left != right : (long)left != (long)right);
                return 1;
            }
        }

        private sealed class NotEqualByte : NotEqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                object right = frame.Pop();
                object left = frame.Peek();
                frame.Replace(left == null | right == null ? left != right : (byte)left != (byte)right);
                return 1;
            }
        }

        private sealed class NotEqualUInt16 : NotEqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                object right = frame.Pop();
                object left = frame.Peek();
                frame.Replace(left == null | right == null ? left != right : (ushort)left != (ushort)right);
                return 1;
            }
        }

        private sealed class NotEqualUInt32 : NotEqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                object right = frame.Pop();
                object left = frame.Peek();
                frame.Replace(left == null | right == null ? left != right : (uint)left != (uint)right);
                return 1;
            }
        }

        private sealed class NotEqualUInt64 : NotEqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                object right = frame.Pop();
                object left = frame.Peek();
                frame.Replace(left == null | right == null ? left != right : (ulong)left != (ulong)right);
                return 1;
            }
        }

        private sealed class NotEqualSingle : NotEqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                object right = frame.Pop();
                object left = frame.Peek();
                frame.Replace(left == null | right == null ? left != right : (float)left != (float)right);
                return 1;
            }
        }

        private sealed class NotEqualDouble : NotEqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                object right = frame.Pop();
                object left = frame.Peek();
                frame.Replace(left == null | right == null ? left != right : (double)left != (double)right);
                return 1;
            }
        }

        private sealed class NotEqualReference : NotEqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                object right = frame.Pop();
                frame.Replace(frame.Peek() != right);
                return 1;
            }
        }

        private sealed class NotEqualSByteLiftedToNull : NotEqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                object right = frame.Pop();
                if (right == null)
                {
                    frame.Replace(null);
                }
                else
                {
                    object left = frame.Peek();
                    if (left != null)
                    {
                        frame.Replace((sbyte)left != (sbyte)right);
                    }
                }

                return 1;
            }
        }

        private sealed class NotEqualInt16LiftedToNull : NotEqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                object right = frame.Pop();
                if (right == null)
                {
                    frame.Replace(null);
                }
                else
                {
                    object left = frame.Peek();
                    if (left != null)
                    {
                        frame.Replace((short)left != (short)right);
                    }
                }

                return 1;
            }
        }

        private sealed class NotEqualCharLiftedToNull : NotEqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                object right = frame.Pop();
                if (right == null)
                {
                    frame.Replace(null);
                }
                else
                {
                    object left = frame.Peek();
                    if (left != null)
                    {
                        frame.Replace((char)left != (char)right);
                    }
                }

                return 1;
            }
        }

        private sealed class NotEqualInt32LiftedToNull : NotEqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                object right = frame.Pop();
                if (right == null)
                {
                    frame.Replace(null);
                }
                else
                {
                    object left = frame.Peek();
                    if (left != null)
                    {
                        frame.Replace((int)left != (int)right);
                    }
                }

                return 1;
            }
        }

        private sealed class NotEqualInt64LiftedToNull : NotEqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                object right = frame.Pop();
                if (right == null)
                {
                    frame.Replace(null);
                }
                else
                {
                    object left = frame.Peek();
                    if (left != null)
                    {
                        frame.Replace((long)left != (long)right);
                    }
                }

                return 1;
            }
        }

        private sealed class NotEqualByteLiftedToNull : NotEqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                object right = frame.Pop();
                if (right == null)
                {
                    frame.Replace(null);
                }
                else
                {
                    object left = frame.Peek();
                    if (left != null)
                    {
                        frame.Replace((byte)left != (byte)right);
                    }
                }

                return 1;
            }
        }

        private sealed class NotEqualUInt16LiftedToNull : NotEqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                object right = frame.Pop();
                if (right == null)
                {
                    frame.Replace(null);
                }
                else
                {
                    object left = frame.Peek();
                    if (left != null)
                    {
                        frame.Replace((ushort)left != (ushort)right);
                    }
                }

                return 1;
            }
        }

        private sealed class NotEqualUInt32LiftedToNull : NotEqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                object right = frame.Pop();
                if (right == null)
                {
                    frame.Replace(null);
                }
                else
                {
                    object left = frame.Peek();
                    if (left != null)
                    {
                        frame.Replace((uint)left != (uint)right);
                    }
                }

                return 1;
            }
        }

        private sealed class NotEqualUInt64LiftedToNull : NotEqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                object right = frame.Pop();
                if (right == null)
                {
                    frame.Replace(null);
                }
                else
                {
                    object left = frame.Peek();
                    if (left != null)
                    {
                        frame.Replace((ulong)left != (ulong)right);
                    }
                }

                return 1;
            }
        }

        private sealed class NotEqualSingleLiftedToNull : NotEqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                object right = frame.Pop();
                if (right == null)
                {
                    frame.Replace(null);
                }
                else
                {
                    object left = frame.Peek();
                    if (left != null)
                    {
                        frame.Replace((float)left != (float)right);
                    }
                }

                return 1;
            }
        }

        private sealed class NotEqualDoubleLiftedToNull : NotEqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                object right = frame.Pop();
                if (right == null)
                {
                    frame.Replace(null);
                }
                else
                {
                    object left = frame.Peek();
                    if (left != null)
                    {
                        frame.Replace((double)left != (double)right);
                    }
                }

                return 1;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        public static Instruction Create(Type type, bool liftedToNull)
        {
            if (liftedToNull)
            {
                switch (type.GetNonNullableType().GetTypeCode())
                {
                    case TypeCode.Boolean: return ExclusiveOrInstruction.Create(type);
                    case TypeCode.SByte: return s_SByteLiftedToNull ?? (s_SByteLiftedToNull = new NotEqualSByteLiftedToNull());
                    case TypeCode.Int16: return s_Int16LiftedToNull ?? (s_Int16LiftedToNull = new NotEqualInt16LiftedToNull());
                    case TypeCode.Char: return s_CharLiftedToNull ?? (s_CharLiftedToNull = new NotEqualCharLiftedToNull());
                    case TypeCode.Int32: return s_Int32LiftedToNull ?? (s_Int32LiftedToNull = new NotEqualInt32LiftedToNull());
                    case TypeCode.Int64: return s_Int64LiftedToNull ?? (s_Int64LiftedToNull = new NotEqualInt64LiftedToNull());
                    case TypeCode.Byte: return s_ByteLiftedToNull ?? (s_ByteLiftedToNull = new NotEqualByteLiftedToNull());
                    case TypeCode.UInt16: return s_UInt16LiftedToNull ?? (s_UInt16LiftedToNull = new NotEqualUInt16LiftedToNull());
                    case TypeCode.UInt32: return s_UInt32LiftedToNull ?? (s_UInt32LiftedToNull = new NotEqualUInt32LiftedToNull());
                    case TypeCode.UInt64: return s_UInt64LiftedToNull ?? (s_UInt64LiftedToNull = new NotEqualUInt64LiftedToNull());
                    case TypeCode.Single: return s_SingleLiftedToNull ?? (s_SingleLiftedToNull = new NotEqualSingleLiftedToNull());
                    default:
                        Debug.Assert(type.GetNonNullableType().GetTypeCode() == TypeCode.Double);
                        return s_DoubleLiftedToNull ?? (s_DoubleLiftedToNull = new NotEqualDoubleLiftedToNull());
                }
            }
            else
            {
                switch (type.GetNonNullableType().GetTypeCode())
                {
                    case TypeCode.Boolean: return s_Boolean ?? (s_Boolean = new NotEqualBoolean());
                    case TypeCode.SByte: return s_SByte ?? (s_SByte = new NotEqualSByte());
                    case TypeCode.Int16: return s_Int16 ?? (s_Int16 = new NotEqualInt16());
                    case TypeCode.Char: return s_Char ?? (s_Char = new NotEqualChar());
                    case TypeCode.Int32: return s_Int32 ?? (s_Int32 = new NotEqualInt32());
                    case TypeCode.Int64: return s_Int64 ?? (s_Int64 = new NotEqualInt64());
                    case TypeCode.Byte: return s_Byte ?? (s_Byte = new NotEqualByte());
                    case TypeCode.UInt16: return s_UInt16 ?? (s_UInt16 = new NotEqualUInt16());
                    case TypeCode.UInt32: return s_UInt32 ?? (s_UInt32 = new NotEqualUInt32());
                    case TypeCode.UInt64: return s_UInt64 ?? (s_UInt64 = new NotEqualUInt64());
                    case TypeCode.Single: return s_Single ?? (s_Single = new NotEqualSingle());
                    case TypeCode.Double: return s_Double ?? (s_Double = new NotEqualDouble());
                    default:
                        // Nullable only valid if one operand is constant null, so this assert is slightly too broad.
                        Debug.Assert(type.IsNullableOrReferenceType());
                        return s_reference ?? (s_reference = new NotEqualReference());
                }
            }
        }
    }
}

