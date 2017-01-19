﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Do not edit this file manually. This file is auto-generated by OrInstruction.tt.

using System.Dynamic.Utils;
using System.Reflection;

namespace System.Linq.Expressions.Interpreter
{
    internal abstract partial class OrInstruction : Instruction
    {
        private static Instruction s_SByte, s_Int16, s_Int32, s_Int64, s_Byte, s_UInt16, s_UInt32, s_UInt64, s_Boolean;

        public override int ConsumedStack => 2;
        public override int ProducedStack => 1;
        public override string InstructionName => "Or";

        private OrInstruction() { }

        private sealed class OrSByte : OrInstruction
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
                frame.Push((sbyte)((sbyte)left | (sbyte)right));
                return 1;
            }
        }

        private sealed class OrInt16 : OrInstruction
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
                frame.Push((short)((short)left | (short)right));
                return 1;
            }
        }

        private sealed class OrInt32 : OrInstruction
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
                frame.Push((int)left | (int)right);
                return 1;
            }
        }

        private sealed class OrInt64 : OrInstruction
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
                frame.Push((long)left | (long)right);
                return 1;
            }
        }

        private sealed class OrByte : OrInstruction
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
                frame.Push((byte)((byte)left | (byte)right));
                return 1;
            }
        }

        private sealed class OrUInt16 : OrInstruction
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
                frame.Push((ushort)((ushort)left | (ushort)right));
                return 1;
            }
        }

        private sealed class OrUInt32 : OrInstruction
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
                frame.Push((uint)left | (uint)right);
                return 1;
            }
        }

        private sealed class OrUInt64 : OrInstruction
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
                frame.Push((ulong)left | (ulong)right);
                return 1;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        public static Instruction Create(Type type)
        {
            // Boxed enums can be unboxed as their underlying types:
            Type underlyingType = type.GetTypeInfo().IsEnum ? Enum.GetUnderlyingType(type) : type.GetNonNullableType();

            switch (underlyingType.GetTypeCode())
            {
                case TypeCode.SByte: return s_SByte ?? (s_SByte = new OrSByte());
                case TypeCode.Int16: return s_Int16 ?? (s_Int16 = new OrInt16());
                case TypeCode.Int32: return s_Int32 ?? (s_Int32 = new OrInt32());
                case TypeCode.Int64: return s_Int64 ?? (s_Int64 = new OrInt64());
                case TypeCode.Byte: return s_Byte ?? (s_Byte = new OrByte());
                case TypeCode.UInt16: return s_UInt16 ?? (s_UInt16 = new OrUInt16());
                case TypeCode.UInt32: return s_UInt32 ?? (s_UInt32 = new OrUInt32());
                case TypeCode.UInt64: return s_UInt64 ?? (s_UInt64 = new OrUInt64());
                case TypeCode.Boolean: return s_Boolean ?? (s_Boolean = new OrBoolean());
                default:
                    throw Error.ExpressionNotSupportedForType("Or", type);
            }
        }
    }
}
