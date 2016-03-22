// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic.Utils;
using System.Reflection;
using System.Reflection.Emit;


namespace System.Linq.Expressions.Compiler
{
    internal static class ILGen
    {
        internal static void Emit(this ILGenerator il, OpCode opcode, MethodBase methodBase)
        {
            Debug.Assert(methodBase is MethodInfo || methodBase is ConstructorInfo);

            var ctor = methodBase as ConstructorInfo;
            if ((object)ctor != null)
            {
                il.Emit(opcode, ctor);
            }
            else
            {
                il.Emit(opcode, (MethodInfo)methodBase);
            }
        }

        #region Instruction helpers

        internal static void EmitLoadArg(this ILGenerator il, int index)
        {
            Debug.Assert(index >= 0);

            switch (index)
            {
                case 0:
                    il.Emit(OpCodes.Ldarg_0);
                    break;
                case 1:
                    il.Emit(OpCodes.Ldarg_1);
                    break;
                case 2:
                    il.Emit(OpCodes.Ldarg_2);
                    break;
                case 3:
                    il.Emit(OpCodes.Ldarg_3);
                    break;
                default:
                    if (index <= Byte.MaxValue)
                    {
                        il.Emit(OpCodes.Ldarg_S, (byte)index);
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldarg, index);
                    }
                    break;
            }
        }

        internal static void EmitLoadArgAddress(this ILGenerator il, int index)
        {
            Debug.Assert(index >= 0);

            if (index <= Byte.MaxValue)
            {
                il.Emit(OpCodes.Ldarga_S, (byte)index);
            }
            else
            {
                il.Emit(OpCodes.Ldarga, index);
            }
        }

        internal static void EmitStoreArg(this ILGenerator il, int index)
        {
            Debug.Assert(index >= 0);

            if (index <= Byte.MaxValue)
            {
                il.Emit(OpCodes.Starg_S, (byte)index);
            }
            else
            {
                il.Emit(OpCodes.Starg, index);
            }
        }

        /// <summary>
        /// Emits a Ldind* instruction for the appropriate type
        /// </summary>
        internal static void EmitLoadValueIndirect(this ILGenerator il, Type type)
        {
            ContractUtils.RequiresNotNull(type, nameof(type));

            if (type.GetTypeInfo().IsValueType)
            {
                if (type == typeof(int))
                {
                    il.Emit(OpCodes.Ldind_I4);
                }
                else if (type == typeof(uint))
                {
                    il.Emit(OpCodes.Ldind_U4);
                }
                else if (type == typeof(short))
                {
                    il.Emit(OpCodes.Ldind_I2);
                }
                else if (type == typeof(ushort))
                {
                    il.Emit(OpCodes.Ldind_U2);
                }
                else if (type == typeof(long) || type == typeof(ulong))
                {
                    il.Emit(OpCodes.Ldind_I8);
                }
                else if (type == typeof(char))
                {
                    il.Emit(OpCodes.Ldind_I2);
                }
                else if (type == typeof(bool))
                {
                    il.Emit(OpCodes.Ldind_I1);
                }
                else if (type == typeof(float))
                {
                    il.Emit(OpCodes.Ldind_R4);
                }
                else if (type == typeof(double))
                {
                    il.Emit(OpCodes.Ldind_R8);
                }
                else
                {
                    il.Emit(OpCodes.Ldobj, type);
                }
            }
            else
            {
                il.Emit(OpCodes.Ldind_Ref);
            }
        }


        /// <summary>
        /// Emits a Stind* instruction for the appropriate type.
        /// </summary>
        internal static void EmitStoreValueIndirect(this ILGenerator il, Type type)
        {
            ContractUtils.RequiresNotNull(type, nameof(type));

            if (type.GetTypeInfo().IsValueType)
            {
                if (type == typeof(int))
                {
                    il.Emit(OpCodes.Stind_I4);
                }
                else if (type == typeof(short))
                {
                    il.Emit(OpCodes.Stind_I2);
                }
                else if (type == typeof(long) || type == typeof(ulong))
                {
                    il.Emit(OpCodes.Stind_I8);
                }
                else if (type == typeof(char))
                {
                    il.Emit(OpCodes.Stind_I2);
                }
                else if (type == typeof(bool))
                {
                    il.Emit(OpCodes.Stind_I1);
                }
                else if (type == typeof(float))
                {
                    il.Emit(OpCodes.Stind_R4);
                }
                else if (type == typeof(double))
                {
                    il.Emit(OpCodes.Stind_R8);
                }
                else
                {
                    il.Emit(OpCodes.Stobj, type);
                }
            }
            else
            {
                il.Emit(OpCodes.Stind_Ref);
            }
        }

        // Emits the Ldelem* instruction for the appropriate type

        internal static void EmitLoadElement(this ILGenerator il, Type type)
        {
            ContractUtils.RequiresNotNull(type, nameof(type));

            if (!type.GetTypeInfo().IsValueType)
            {
                il.Emit(OpCodes.Ldelem_Ref);
            }
            else if (type.GetTypeInfo().IsEnum)
            {
                il.Emit(OpCodes.Ldelem, type);
            }
            else
            {
                switch (type.GetTypeCode())
                {
                    case TypeCode.Boolean:
                    case TypeCode.SByte:
                        il.Emit(OpCodes.Ldelem_I1);
                        break;
                    case TypeCode.Byte:
                        il.Emit(OpCodes.Ldelem_U1);
                        break;
                    case TypeCode.Int16:
                        il.Emit(OpCodes.Ldelem_I2);
                        break;
                    case TypeCode.Char:
                    case TypeCode.UInt16:
                        il.Emit(OpCodes.Ldelem_U2);
                        break;
                    case TypeCode.Int32:
                        il.Emit(OpCodes.Ldelem_I4);
                        break;
                    case TypeCode.UInt32:
                        il.Emit(OpCodes.Ldelem_U4);
                        break;
                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                        il.Emit(OpCodes.Ldelem_I8);
                        break;
                    case TypeCode.Single:
                        il.Emit(OpCodes.Ldelem_R4);
                        break;
                    case TypeCode.Double:
                        il.Emit(OpCodes.Ldelem_R8);
                        break;
                    default:
                        il.Emit(OpCodes.Ldelem, type);
                        break;
                }
            }
        }

        /// <summary>
        /// Emits a Stelem* instruction for the appropriate type.
        /// </summary>
        internal static void EmitStoreElement(this ILGenerator il, Type type)
        {
            ContractUtils.RequiresNotNull(type, nameof(type));

            if (type.GetTypeInfo().IsEnum)
            {
                il.Emit(OpCodes.Stelem, type);
                return;
            }
            switch (type.GetTypeCode())
            {
                case TypeCode.Boolean:
                case TypeCode.SByte:
                case TypeCode.Byte:
                    il.Emit(OpCodes.Stelem_I1);
                    break;
                case TypeCode.Char:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                    il.Emit(OpCodes.Stelem_I2);
                    break;
                case TypeCode.Int32:
                case TypeCode.UInt32:
                    il.Emit(OpCodes.Stelem_I4);
                    break;
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    il.Emit(OpCodes.Stelem_I8);
                    break;
                case TypeCode.Single:
                    il.Emit(OpCodes.Stelem_R4);
                    break;
                case TypeCode.Double:
                    il.Emit(OpCodes.Stelem_R8);
                    break;
                default:
                    if (type.GetTypeInfo().IsValueType)
                    {
                        il.Emit(OpCodes.Stelem, type);
                    }
                    else
                    {
                        il.Emit(OpCodes.Stelem_Ref);
                    }
                    break;
            }
        }

        internal static void EmitType(this ILGenerator il, Type type)
        {
            ContractUtils.RequiresNotNull(type, nameof(type));

            il.Emit(OpCodes.Ldtoken, type);
            il.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle"));
        }

        #endregion

        #region Fields, properties and methods

        internal static void EmitFieldAddress(this ILGenerator il, FieldInfo fi)
        {
            ContractUtils.RequiresNotNull(fi, nameof(fi));

            if (fi.IsStatic)
            {
                il.Emit(OpCodes.Ldsflda, fi);
            }
            else
            {
                il.Emit(OpCodes.Ldflda, fi);
            }
        }

        internal static void EmitFieldGet(this ILGenerator il, FieldInfo fi)
        {
            ContractUtils.RequiresNotNull(fi, nameof(fi));

            if (fi.IsStatic)
            {
                il.Emit(OpCodes.Ldsfld, fi);
            }
            else
            {
                il.Emit(OpCodes.Ldfld, fi);
            }
        }

        internal static void EmitFieldSet(this ILGenerator il, FieldInfo fi)
        {
            ContractUtils.RequiresNotNull(fi, nameof(fi));

            if (fi.IsStatic)
            {
                il.Emit(OpCodes.Stsfld, fi);
            }
            else
            {
                il.Emit(OpCodes.Stfld, fi);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
        internal static void EmitNew(this ILGenerator il, ConstructorInfo ci)
        {
            ContractUtils.RequiresNotNull(ci, nameof(ci));

            if (ci.DeclaringType.GetTypeInfo().ContainsGenericParameters)
            {
                throw Error.IllegalNewGenericParams(ci.DeclaringType);
            }

            il.Emit(OpCodes.Newobj, ci);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
        internal static void EmitNew(this ILGenerator il, Type type, Type[] paramTypes)
        {
            ContractUtils.RequiresNotNull(type, nameof(type));
            ContractUtils.RequiresNotNull(paramTypes, nameof(paramTypes));

            ConstructorInfo ci = type.GetConstructor(paramTypes);
            if (ci == null) throw Error.TypeDoesNotHaveConstructorForTheSignature();
            il.EmitNew(ci);
        }

        #endregion

        #region Constants

        internal static void EmitNull(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldnull);
        }

        internal static void EmitString(this ILGenerator il, string value)
        {
            ContractUtils.RequiresNotNull(value, nameof(value));
            il.Emit(OpCodes.Ldstr, value);
        }

        internal static void EmitBoolean(this ILGenerator il, bool value)
        {
            if (value)
            {
                il.Emit(OpCodes.Ldc_I4_1);
            }
            else
            {
                il.Emit(OpCodes.Ldc_I4_0);
            }
        }

        internal static void EmitChar(this ILGenerator il, char value)
        {
            il.EmitInt(value);
            il.Emit(OpCodes.Conv_U2);
        }

        internal static void EmitByte(this ILGenerator il, byte value)
        {
            il.EmitInt(value);
            il.Emit(OpCodes.Conv_U1);
        }

        internal static void EmitSByte(this ILGenerator il, sbyte value)
        {
            il.EmitInt(value);
            il.Emit(OpCodes.Conv_I1);
        }

        internal static void EmitShort(this ILGenerator il, short value)
        {
            il.EmitInt(value);
            il.Emit(OpCodes.Conv_I2);
        }

        internal static void EmitUShort(this ILGenerator il, ushort value)
        {
            il.EmitInt(value);
            il.Emit(OpCodes.Conv_U2);
        }

        internal static void EmitInt(this ILGenerator il, int value)
        {
            OpCode c;
            switch (value)
            {
                case -1:
                    c = OpCodes.Ldc_I4_M1;
                    break;
                case 0:
                    c = OpCodes.Ldc_I4_0;
                    break;
                case 1:
                    c = OpCodes.Ldc_I4_1;
                    break;
                case 2:
                    c = OpCodes.Ldc_I4_2;
                    break;
                case 3:
                    c = OpCodes.Ldc_I4_3;
                    break;
                case 4:
                    c = OpCodes.Ldc_I4_4;
                    break;
                case 5:
                    c = OpCodes.Ldc_I4_5;
                    break;
                case 6:
                    c = OpCodes.Ldc_I4_6;
                    break;
                case 7:
                    c = OpCodes.Ldc_I4_7;
                    break;
                case 8:
                    c = OpCodes.Ldc_I4_8;
                    break;
                default:
                    if (value >= -128 && value <= 127)
                    {
                        il.Emit(OpCodes.Ldc_I4_S, (sbyte)value);
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldc_I4, value);
                    }
                    return;
            }
            il.Emit(c);
        }

        internal static void EmitUInt(this ILGenerator il, uint value)
        {
            il.EmitInt((int)value);
            il.Emit(OpCodes.Conv_U4);
        }

        internal static void EmitLong(this ILGenerator il, long value)
        {
            il.Emit(OpCodes.Ldc_I8, value);

            //
            // Now, emit convert to give the constant type information.
            //
            // Otherwise, it is treated as unsigned and overflow is not
            // detected if it's used in checked ops.
            //
            il.Emit(OpCodes.Conv_I8);
        }

        internal static void EmitULong(this ILGenerator il, ulong value)
        {
            il.Emit(OpCodes.Ldc_I8, (long)value);
            il.Emit(OpCodes.Conv_U8);
        }

        internal static void EmitDouble(this ILGenerator il, double value)
        {
            il.Emit(OpCodes.Ldc_R8, value);
        }

        internal static void EmitSingle(this ILGenerator il, float value)
        {
            il.Emit(OpCodes.Ldc_R4, value);
        }

        // matches TryEmitConstant
        internal static bool CanEmitConstant(object value, Type type)
        {
            if (value == null || CanEmitILConstant(type))
            {
                return true;
            }

            Type t = value as Type;
            if (t != null && ShouldLdtoken(t))
            {
                return true;
            }

            MethodBase mb = value as MethodBase;
            if (mb != null && ShouldLdtoken(mb))
            {
                return true;
            }

            return false;
        }

        // matches TryEmitILConstant
        private static bool CanEmitILConstant(Type type)
        {
            switch (type.GetTypeCode())
            {
                case TypeCode.Boolean:
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Char:
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Decimal:
                case TypeCode.String:
                    return true;
            }
            return false;
        }

        internal static void EmitConstant(this ILGenerator il, object value)
        {
            Debug.Assert(value != null);
            EmitConstant(il, value, value.GetType());
        }


        //
        // Note: we support emitting more things as IL constants than
        // Linq does
        internal static void EmitConstant(this ILGenerator il, object value, Type type)
        {
            if (value == null)
            {
                // Smarter than the Linq implementation which uses the initobj
                // pattern for all value types (works, but requires a local and
                // more IL)
                il.EmitDefault(type);
                return;
            }

            // Handle the easy cases
            if (il.TryEmitILConstant(value, type))
            {
                return;
            }

            // Check for a few more types that we support emitting as constants
            Type t = value as Type;
            if (t != null && ShouldLdtoken(t))
            {
                il.EmitType(t);
                if (type != typeof(Type))
                {
                    il.Emit(OpCodes.Castclass, type);
                }
                return;
            }

            MethodBase mb = value as MethodBase;
            if (mb != null && ShouldLdtoken(mb))
            {
                il.Emit(OpCodes.Ldtoken, mb);
                Type dt = mb.DeclaringType;
                if (dt != null && dt.GetTypeInfo().IsGenericType)
                {
                    il.Emit(OpCodes.Ldtoken, dt);
                    il.Emit(OpCodes.Call, typeof(MethodBase).GetMethod("GetMethodFromHandle", new Type[] { typeof(RuntimeMethodHandle), typeof(RuntimeTypeHandle) }));
                }
                else
                {
                    il.Emit(OpCodes.Call, typeof(MethodBase).GetMethod("GetMethodFromHandle", new Type[] { typeof(RuntimeMethodHandle) }));
                }
                if (type != typeof(MethodBase))
                {
                    il.Emit(OpCodes.Castclass, type);
                }
                return;
            }

            throw ContractUtils.Unreachable;
        }

        internal static bool ShouldLdtoken(Type t)
        {
            return t.GetTypeInfo() is TypeBuilder || t.IsGenericParameter || t.GetTypeInfo().IsVisible;
        }

        internal static bool ShouldLdtoken(MethodBase mb)
        {
            // Can't ldtoken on a DynamicMethod
            if (mb is DynamicMethod)
            {
                return false;
            }

            Type dt = mb.DeclaringType;
            return dt == null || ShouldLdtoken(dt);
        }


        private static bool TryEmitILConstant(this ILGenerator il, object value, Type type)
        {
            switch (type.GetTypeCode())
            {
                case TypeCode.Boolean:
                    il.EmitBoolean((bool)value);
                    return true;
                case TypeCode.SByte:
                    il.EmitSByte((sbyte)value);
                    return true;
                case TypeCode.Int16:
                    il.EmitShort((short)value);
                    return true;
                case TypeCode.Int32:
                    il.EmitInt((int)value);
                    return true;
                case TypeCode.Int64:
                    il.EmitLong((long)value);
                    return true;
                case TypeCode.Single:
                    il.EmitSingle((float)value);
                    return true;
                case TypeCode.Double:
                    il.EmitDouble((double)value);
                    return true;
                case TypeCode.Char:
                    il.EmitChar((char)value);
                    return true;
                case TypeCode.Byte:
                    il.EmitByte((byte)value);
                    return true;
                case TypeCode.UInt16:
                    il.EmitUShort((ushort)value);
                    return true;
                case TypeCode.UInt32:
                    il.EmitUInt((uint)value);
                    return true;
                case TypeCode.UInt64:
                    il.EmitULong((ulong)value);
                    return true;
                case TypeCode.Decimal:
                    il.EmitDecimal((decimal)value);
                    return true;
                case TypeCode.String:
                    il.EmitString((string)value);
                    return true;
                default:
                    return false;
            }
        }

        #endregion

        #region Linq Conversions

        internal static void EmitConvertToType(this ILGenerator il, Type typeFrom, Type typeTo, bool isChecked)
        {
            if (TypeUtils.AreEquivalent(typeFrom, typeTo))
            {
                return;
            }

            if (typeFrom == typeof(void) || typeTo == typeof(void))
            {
                throw ContractUtils.Unreachable;
            }

            bool isTypeFromNullable = TypeUtils.IsNullableType(typeFrom);
            bool isTypeToNullable = TypeUtils.IsNullableType(typeTo);

            Type nnExprType = TypeUtils.GetNonNullableType(typeFrom);
            Type nnType = TypeUtils.GetNonNullableType(typeTo);

            if (typeFrom.GetTypeInfo().IsInterface || // interface cast
               typeTo.GetTypeInfo().IsInterface ||
               typeFrom == typeof(object) || // boxing cast
               typeTo == typeof(object) ||
               typeFrom == typeof(System.Enum) ||
               typeFrom == typeof(System.ValueType) ||
               TypeUtils.IsLegalExplicitVariantDelegateConversion(typeFrom, typeTo))
            {
                il.EmitCastToType(typeFrom, typeTo);
            }
            else if (isTypeFromNullable || isTypeToNullable)
            {
                il.EmitNullableConversion(typeFrom, typeTo, isChecked);
            }
            else if (!(TypeUtils.IsConvertible(typeFrom) && TypeUtils.IsConvertible(typeTo)) // primitive runtime conversion
                     &&
                     (nnExprType.IsAssignableFrom(nnType) || // down cast
                     nnType.IsAssignableFrom(nnExprType))) // up cast
            {
                il.EmitCastToType(typeFrom, typeTo);
            }
            else if (typeFrom.IsArray && typeTo.IsArray)
            {
                // See DevDiv Bugs #94657.
                il.EmitCastToType(typeFrom, typeTo);
            }
            else
            {
                il.EmitNumericConversion(typeFrom, typeTo, isChecked);
            }
        }


        private static void EmitCastToType(this ILGenerator il, Type typeFrom, Type typeTo)
        {
            if (!typeFrom.GetTypeInfo().IsValueType && typeTo.GetTypeInfo().IsValueType)
            {
                il.Emit(OpCodes.Unbox_Any, typeTo);
            }
            else if (typeFrom.GetTypeInfo().IsValueType && !typeTo.GetTypeInfo().IsValueType)
            {
                il.Emit(OpCodes.Box, typeFrom);
                if (typeTo != typeof(object))
                {
                    il.Emit(OpCodes.Castclass, typeTo);
                }
            }
            else if (!typeFrom.GetTypeInfo().IsValueType && !typeTo.GetTypeInfo().IsValueType)
            {
                il.Emit(OpCodes.Castclass, typeTo);
            }
            else
            {
                throw Error.InvalidCast(typeFrom, typeTo);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private static void EmitNumericConversion(this ILGenerator il, Type typeFrom, Type typeTo, bool isChecked)
        {
            bool isFromUnsigned = TypeUtils.IsUnsigned(typeFrom);
            bool isFromFloatingPoint = TypeUtils.IsFloatingPoint(typeFrom);
            if (typeTo == typeof(Single))
            {
                if (isFromUnsigned)
                    il.Emit(OpCodes.Conv_R_Un);
                il.Emit(OpCodes.Conv_R4);
            }
            else if (typeTo == typeof(Double))
            {
                if (isFromUnsigned)
                    il.Emit(OpCodes.Conv_R_Un);
                il.Emit(OpCodes.Conv_R8);
            }
            else
            {
                TypeCode tc = typeTo.GetTypeCode();
                if (isChecked)
                {
                    // Overflow checking needs to know if the source value on the IL stack is unsigned or not.
                    if (isFromUnsigned)
                    {
                        switch (tc)
                        {
                            case TypeCode.SByte:
                                il.Emit(OpCodes.Conv_Ovf_I1_Un);
                                break;
                            case TypeCode.Int16:
                                il.Emit(OpCodes.Conv_Ovf_I2_Un);
                                break;
                            case TypeCode.Int32:
                                il.Emit(OpCodes.Conv_Ovf_I4_Un);
                                break;
                            case TypeCode.Int64:
                                il.Emit(OpCodes.Conv_Ovf_I8_Un);
                                break;
                            case TypeCode.Byte:
                                il.Emit(OpCodes.Conv_Ovf_U1_Un);
                                break;
                            case TypeCode.UInt16:
                            case TypeCode.Char:
                                il.Emit(OpCodes.Conv_Ovf_U2_Un);
                                break;
                            case TypeCode.UInt32:
                                il.Emit(OpCodes.Conv_Ovf_U4_Un);
                                break;
                            case TypeCode.UInt64:
                                il.Emit(OpCodes.Conv_Ovf_U8_Un);
                                break;
                            default:
                                throw Error.UnhandledConvert(typeTo);
                        }
                    }
                    else
                    {
                        switch (tc)
                        {
                            case TypeCode.SByte:
                                il.Emit(OpCodes.Conv_Ovf_I1);
                                break;
                            case TypeCode.Int16:
                                il.Emit(OpCodes.Conv_Ovf_I2);
                                break;
                            case TypeCode.Int32:
                                il.Emit(OpCodes.Conv_Ovf_I4);
                                break;
                            case TypeCode.Int64:
                                il.Emit(OpCodes.Conv_Ovf_I8);
                                break;
                            case TypeCode.Byte:
                                il.Emit(OpCodes.Conv_Ovf_U1);
                                break;
                            case TypeCode.UInt16:
                            case TypeCode.Char:
                                il.Emit(OpCodes.Conv_Ovf_U2);
                                break;
                            case TypeCode.UInt32:
                                il.Emit(OpCodes.Conv_Ovf_U4);
                                break;
                            case TypeCode.UInt64:
                                il.Emit(OpCodes.Conv_Ovf_U8);
                                break;
                            default:
                                throw Error.UnhandledConvert(typeTo);
                        }
                    }
                }
                else
                {
                    switch (tc)
                    {
                        case TypeCode.SByte:
                            il.Emit(OpCodes.Conv_I1);
                            break;
                        case TypeCode.Byte:
                            il.Emit(OpCodes.Conv_U1);
                            break;
                        case TypeCode.Int16:
                            il.Emit(OpCodes.Conv_I2);
                            break;
                        case TypeCode.UInt16:
                        case TypeCode.Char:
                            il.Emit(OpCodes.Conv_U2);
                            break;
                        case TypeCode.Int32:
                            il.Emit(OpCodes.Conv_I4);
                            break;
                        case TypeCode.UInt32:
                            il.Emit(OpCodes.Conv_U4);
                            break;
                        case TypeCode.Int64:
                            if (isFromUnsigned)
                            {
                                il.Emit(OpCodes.Conv_U8);
                            }
                            else
                            {
                                il.Emit(OpCodes.Conv_I8);
                            }
                            break;
                        case TypeCode.UInt64:
                            if (isFromUnsigned || isFromFloatingPoint)
                            {
                                il.Emit(OpCodes.Conv_U8);
                            }
                            else
                            {
                                il.Emit(OpCodes.Conv_I8);
                            }
                            break;
                        default:
                            throw Error.UnhandledConvert(typeTo);
                    }
                }
            }
        }

        private static void EmitNullableToNullableConversion(this ILGenerator il, Type typeFrom, Type typeTo, bool isChecked)
        {
            Debug.Assert(TypeUtils.IsNullableType(typeFrom));
            Debug.Assert(TypeUtils.IsNullableType(typeTo));
            Label labIfNull = default(Label);
            Label labEnd = default(Label);
            LocalBuilder locFrom = null;
            LocalBuilder locTo = null;
            locFrom = il.DeclareLocal(typeFrom);
            il.Emit(OpCodes.Stloc, locFrom);
            locTo = il.DeclareLocal(typeTo);
            // test for null
            il.Emit(OpCodes.Ldloca, locFrom);
            il.EmitHasValue(typeFrom);
            labIfNull = il.DefineLabel();
            il.Emit(OpCodes.Brfalse_S, labIfNull);
            il.Emit(OpCodes.Ldloca, locFrom);
            il.EmitGetValueOrDefault(typeFrom);
            Type nnTypeFrom = TypeUtils.GetNonNullableType(typeFrom);
            Type nnTypeTo = TypeUtils.GetNonNullableType(typeTo);
            il.EmitConvertToType(nnTypeFrom, nnTypeTo, isChecked);
            // construct result type
            ConstructorInfo ci = typeTo.GetConstructor(new Type[] { nnTypeTo });
            il.Emit(OpCodes.Newobj, ci);
            il.Emit(OpCodes.Stloc, locTo);
            labEnd = il.DefineLabel();
            il.Emit(OpCodes.Br_S, labEnd);
            // if null then create a default one
            il.MarkLabel(labIfNull);
            il.Emit(OpCodes.Ldloca, locTo);
            il.Emit(OpCodes.Initobj, typeTo);
            il.MarkLabel(labEnd);
            il.Emit(OpCodes.Ldloc, locTo);
        }


        private static void EmitNonNullableToNullableConversion(this ILGenerator il, Type typeFrom, Type typeTo, bool isChecked)
        {
            Debug.Assert(!TypeUtils.IsNullableType(typeFrom));
            Debug.Assert(TypeUtils.IsNullableType(typeTo));
            LocalBuilder locTo = null;
            locTo = il.DeclareLocal(typeTo);
            Type nnTypeTo = TypeUtils.GetNonNullableType(typeTo);
            il.EmitConvertToType(typeFrom, nnTypeTo, isChecked);
            ConstructorInfo ci = typeTo.GetConstructor(new Type[] { nnTypeTo });
            il.Emit(OpCodes.Newobj, ci);
            il.Emit(OpCodes.Stloc, locTo);
            il.Emit(OpCodes.Ldloc, locTo);
        }


        private static void EmitNullableToNonNullableConversion(this ILGenerator il, Type typeFrom, Type typeTo, bool isChecked)
        {
            Debug.Assert(TypeUtils.IsNullableType(typeFrom));
            Debug.Assert(!TypeUtils.IsNullableType(typeTo));
            if (typeTo.GetTypeInfo().IsValueType)
                il.EmitNullableToNonNullableStructConversion(typeFrom, typeTo, isChecked);
            else
                il.EmitNullableToReferenceConversion(typeFrom);
        }


        private static void EmitNullableToNonNullableStructConversion(this ILGenerator il, Type typeFrom, Type typeTo, bool isChecked)
        {
            Debug.Assert(TypeUtils.IsNullableType(typeFrom));
            Debug.Assert(!TypeUtils.IsNullableType(typeTo));
            Debug.Assert(typeTo.GetTypeInfo().IsValueType);
            LocalBuilder locFrom = null;
            locFrom = il.DeclareLocal(typeFrom);
            il.Emit(OpCodes.Stloc, locFrom);
            il.Emit(OpCodes.Ldloca, locFrom);
            il.EmitGetValue(typeFrom);
            Type nnTypeFrom = TypeUtils.GetNonNullableType(typeFrom);
            il.EmitConvertToType(nnTypeFrom, typeTo, isChecked);
        }


        private static void EmitNullableToReferenceConversion(this ILGenerator il, Type typeFrom)
        {
            Debug.Assert(TypeUtils.IsNullableType(typeFrom));
            // We've got a conversion from nullable to Object, ValueType, Enum, etc.  Just box it so that
            // we get the nullable semantics.  
            il.Emit(OpCodes.Box, typeFrom);
        }


        private static void EmitNullableConversion(this ILGenerator il, Type typeFrom, Type typeTo, bool isChecked)
        {
            bool isTypeFromNullable = TypeUtils.IsNullableType(typeFrom);
            bool isTypeToNullable = TypeUtils.IsNullableType(typeTo);
            Debug.Assert(isTypeFromNullable || isTypeToNullable);
            if (isTypeFromNullable && isTypeToNullable)
                il.EmitNullableToNullableConversion(typeFrom, typeTo, isChecked);
            else if (isTypeFromNullable)
                il.EmitNullableToNonNullableConversion(typeFrom, typeTo, isChecked);
            else
                il.EmitNonNullableToNullableConversion(typeFrom, typeTo, isChecked);
        }


        internal static void EmitHasValue(this ILGenerator il, Type nullableType)
        {
            MethodInfo mi = nullableType.GetMethod("get_HasValue", BindingFlags.Instance | BindingFlags.Public);
            Debug.Assert(nullableType.GetTypeInfo().IsValueType);
            il.Emit(OpCodes.Call, mi);
        }


        internal static void EmitGetValue(this ILGenerator il, Type nullableType)
        {
            MethodInfo mi = nullableType.GetMethod("get_Value", BindingFlags.Instance | BindingFlags.Public);
            Debug.Assert(nullableType.GetTypeInfo().IsValueType);
            il.Emit(OpCodes.Call, mi);
        }


        internal static void EmitGetValueOrDefault(this ILGenerator il, Type nullableType)
        {
            MethodInfo mi = nullableType.GetMethod("GetValueOrDefault", System.Type.EmptyTypes);
            Debug.Assert(nullableType.GetTypeInfo().IsValueType);
            il.Emit(OpCodes.Call, mi);
        }

        #endregion

        #region Arrays

        /// <summary>
        /// Emits an array of constant values provided in the given list.
        /// The array is strongly typed.
        /// </summary>
        internal static void EmitArray<T>(this ILGenerator il, IList<T> items)
        {
            ContractUtils.RequiresNotNull(items, nameof(items));

            il.EmitInt(items.Count);
            il.Emit(OpCodes.Newarr, typeof(T));
            for (int i = 0; i < items.Count; i++)
            {
                il.Emit(OpCodes.Dup);
                il.EmitInt(i);
                il.EmitConstant(items[i], typeof(T));
                il.EmitStoreElement(typeof(T));
            }
        }

        /// <summary>
        /// Emits an array of values of count size.  The items are emitted via the callback
        /// which is provided with the current item index to emit.
        /// </summary>
        internal static void EmitArray(this ILGenerator il, Type elementType, int count, Action<int> emit)
        {
            ContractUtils.RequiresNotNull(elementType, nameof(elementType));
            ContractUtils.RequiresNotNull(emit, nameof(emit));
            Debug.Assert(count >= 0);

            il.EmitInt(count);
            il.Emit(OpCodes.Newarr, elementType);
            for (int i = 0; i < count; i++)
            {
                il.Emit(OpCodes.Dup);
                il.EmitInt(i);

                emit(i);

                il.EmitStoreElement(elementType);
            }
        }

        /// <summary>
        /// Emits an array construction code.  
        /// The code assumes that bounds for all dimensions
        /// are already emitted.
        /// </summary>
        internal static void EmitArray(this ILGenerator il, Type arrayType)
        {
            ContractUtils.RequiresNotNull(arrayType, nameof(arrayType));
            Debug.Assert(arrayType.IsArray);

            if (arrayType.IsVector())
            {
                il.Emit(OpCodes.Newarr, arrayType.GetElementType());
            }
            else
            {
                int rank = arrayType.GetArrayRank();

                Type[] types = new Type[rank];
                for (int i = 0; i < rank; i++)
                {
                    types[i] = typeof(int);
                }
                il.EmitNew(arrayType, types);
            }
        }

        #endregion

        #region Support for emitting constants

        internal static void EmitDecimal(this ILGenerator il, decimal value)
        {
            if (Decimal.Truncate(value) == value)
            {
                if (Int32.MinValue <= value && value <= Int32.MaxValue)
                {
                    int intValue = Decimal.ToInt32(value);
                    il.EmitInt(intValue);
                    il.EmitNew(typeof(Decimal).GetConstructor(new Type[] { typeof(int) }));
                }
                else if (Int64.MinValue <= value && value <= Int64.MaxValue)
                {
                    long longValue = Decimal.ToInt64(value);
                    il.EmitLong(longValue);
                    il.EmitNew(typeof(Decimal).GetConstructor(new Type[] { typeof(long) }));
                }
                else
                {
                    il.EmitDecimalBits(value);
                }
            }
            else
            {
                il.EmitDecimalBits(value);
            }
        }

        private static void EmitDecimalBits(this ILGenerator il, decimal value)
        {
            int[] bits = Decimal.GetBits(value);
            il.EmitInt(bits[0]);
            il.EmitInt(bits[1]);
            il.EmitInt(bits[2]);
            il.EmitBoolean((bits[3] & 0x80000000) != 0);
            il.EmitByte((byte)(bits[3] >> 16));
            il.EmitNew(typeof(decimal).GetConstructor(new Type[] { typeof(int), typeof(int), typeof(int), typeof(bool), typeof(byte) }));
        }

        /// <summary>
        /// Emits default(T)
        /// Semantics match C# compiler behavior
        /// </summary>
        internal static void EmitDefault(this ILGenerator il, Type type)
        {
            switch (type.GetTypeCode())
            {
                case TypeCode.Object:
                case TypeCode.DateTime:
                    if (type.GetTypeInfo().IsValueType)
                    {
                        // Type.GetTypeCode on an enum returns the underlying
                        // integer TypeCode, so we won't get here.
                        Debug.Assert(!type.GetTypeInfo().IsEnum);

                        // This is the IL for default(T) if T is a generic type
                        // parameter, so it should work for any type. It's also
                        // the standard pattern for structs.
                        LocalBuilder lb = il.DeclareLocal(type);
                        il.Emit(OpCodes.Ldloca, lb);
                        il.Emit(OpCodes.Initobj, type);
                        il.Emit(OpCodes.Ldloc, lb);
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldnull);
                    }
                    break;

                case TypeCode.Empty:
                case TypeCode.String:
                    il.Emit(OpCodes.Ldnull);
                    break;

                case TypeCode.Boolean:
                case TypeCode.Char:
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                    il.Emit(OpCodes.Ldc_I4_0);
                    break;

                case TypeCode.Int64:
                case TypeCode.UInt64:
                    il.Emit(OpCodes.Ldc_I4_0);
                    il.Emit(OpCodes.Conv_I8);
                    break;

                case TypeCode.Single:
                    il.Emit(OpCodes.Ldc_R4, default(Single));
                    break;

                case TypeCode.Double:
                    il.Emit(OpCodes.Ldc_R8, default(Double));
                    break;

                case TypeCode.Decimal:
                    il.Emit(OpCodes.Ldc_I4_0);
                    il.Emit(OpCodes.Newobj, typeof(Decimal).GetConstructor(new Type[] { typeof(int) }));
                    break;

                default:
                    throw ContractUtils.Unreachable;
            }
        }

        #endregion
    }
}
