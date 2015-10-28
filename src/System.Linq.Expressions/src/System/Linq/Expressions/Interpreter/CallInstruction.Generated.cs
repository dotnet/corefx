// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace System.Linq.Expressions.Interpreter
{
    internal partial class CallInstruction
    {
        private const int MaxHelpers = 3;
        private const int MaxArgs = 3;

        public virtual object InvokeInstance(object instance, params object[] args)
        {
            switch (args.Length)
            {
                case 0: return Invoke(instance);
                case 1: return Invoke(instance, args[0]);
                case 2: return Invoke(instance, args[0], args[1]);
                case 3: return Invoke(instance, args[0], args[1], args[2]);
                case 4: return Invoke(instance, args[0], args[1], args[2], args[3]);
                case 5: return Invoke(instance, args[0], args[1], args[2], args[3], args[4]);
                case 6: return Invoke(instance, args[0], args[1], args[2], args[3], args[4], args[5]);
                case 7: return Invoke(instance, args[0], args[1], args[2], args[3], args[4], args[5], args[6]);
                case 8: return Invoke(instance, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7]);
                default: throw new InvalidOperationException();
            }
        }

        public virtual object Invoke(params object[] args)
        {
            switch (args.Length)
            {
                case 0: return Invoke();
                case 1: return Invoke(args[0]);
                case 2: return Invoke(args[0], args[1]);
                case 3: return Invoke(args[0], args[1], args[2]);
                case 4: return Invoke(args[0], args[1], args[2], args[3]);
                case 5: return Invoke(args[0], args[1], args[2], args[3], args[4]);
                case 6: return Invoke(args[0], args[1], args[2], args[3], args[4], args[5]);
                case 7: return Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6]);
                case 8: return Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7]);
                case 9: return Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8]);
                default: throw new InvalidOperationException();
            }
        }

        public virtual object Invoke() { throw new InvalidOperationException(); }
        public virtual object Invoke(object arg0) { throw new InvalidOperationException(); }
        public virtual object Invoke(object arg0, object arg1) { throw new InvalidOperationException(); }
        public virtual object Invoke(object arg0, object arg1, object arg2) { throw new InvalidOperationException(); }
        public virtual object Invoke(object arg0, object arg1, object arg2, object arg3) { throw new InvalidOperationException(); }
        public virtual object Invoke(object arg0, object arg1, object arg2, object arg3, object arg4) { throw new InvalidOperationException(); }
        public virtual object Invoke(object arg0, object arg1, object arg2, object arg3, object arg4, object arg5) { throw new InvalidOperationException(); }
        public virtual object Invoke(object arg0, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6) { throw new InvalidOperationException(); }
        public virtual object Invoke(object arg0, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7) { throw new InvalidOperationException(); }
        public virtual object Invoke(object arg0, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8) { throw new InvalidOperationException(); }

#if FEATURE_FAST_CREATE
        /// <summary>
        /// Fast creation works if we have a known primitive types for the entire
        /// method siganture.  If we have any non-primitive types then FastCreate
        /// falls back to SlowCreate which works for all types.
        /// 
        /// Fast creation is fast because it avoids using reflection (MakeGenericType
        /// and Activator.CreateInstance) to create the types.  It does this through
        /// calling a series of generic methods picking up each strong type of the
        /// signature along the way.  When it runs out of types it news up the 
        /// appropriate CallInstruction with the strong-types that have been built up.
        /// 
        /// One relaxation is that for return types which are non-primitive types
        /// we can fallback to object due to relaxed delegates.
        /// </summary>
        private static CallInstruction FastCreate(MethodInfo target, ParameterInfo[] pi)
        {
            Type t = TryGetParameterOrReturnType(target, pi, 0);
            if (t == null)
            {
                return new ActionCallInstruction(target);
            }

            if (t.GetTypeInfo().IsEnum) return SlowCreate(target, pi);
            switch (TypeExtensions.GetTypeCode(t))
            {
                case TypeCode.Object:
                    {
                        if (t != typeof(object) && (IndexIsNotReturnType(0, target, pi) || t.GetTypeInfo().IsValueType))
                        {
                            // if we're on the return type relaxed delegates makes it ok to use object
                            goto default;
                        }
                        return FastCreate<Object>(target, pi);
                    }
                case TypeCode.Int16: return FastCreate<Int16>(target, pi);
                case TypeCode.Int32: return FastCreate<Int32>(target, pi);
                case TypeCode.Int64: return FastCreate<Int64>(target, pi);
                case TypeCode.Boolean: return FastCreate<Boolean>(target, pi);
                case TypeCode.Char: return FastCreate<Char>(target, pi);
                case TypeCode.Byte: return FastCreate<Byte>(target, pi);
                case TypeCode.Decimal: return FastCreate<Decimal>(target, pi);
                case TypeCode.DateTime: return FastCreate<DateTime>(target, pi);
                case TypeCode.Double: return FastCreate<Double>(target, pi);
                case TypeCode.Single: return FastCreate<Single>(target, pi);
                case TypeCode.UInt16: return FastCreate<UInt16>(target, pi);
                case TypeCode.UInt32: return FastCreate<UInt32>(target, pi);
                case TypeCode.UInt64: return FastCreate<UInt64>(target, pi);
                case TypeCode.String: return FastCreate<String>(target, pi);
                case TypeCode.SByte: return FastCreate<SByte>(target, pi);
                default: return SlowCreate(target, pi);
            }
        }

        private static CallInstruction FastCreate<T0>(MethodInfo target, ParameterInfo[] pi)
        {
            Type t = TryGetParameterOrReturnType(target, pi, 1);
            if (t == null)
            {
                if (target.ReturnType == typeof(void))
                {
                    return new ActionCallInstruction<T0>(target);
                }
                return new FuncCallInstruction<T0>(target);
            }

            if (t.GetTypeInfo().IsEnum) return SlowCreate(target, pi);
            switch (TypeExtensions.GetTypeCode(t))
            {
                case TypeCode.Object:
                    {
                        if (t != typeof(object) && (IndexIsNotReturnType(1, target, pi) || t.GetTypeInfo().IsValueType))
                        {
                            // if we're on the return type relaxed delegates makes it ok to use object
                            goto default;
                        }
                        return FastCreate<T0, Object>(target, pi);
                    }
                case TypeCode.Int16: return FastCreate<T0, Int16>(target, pi);
                case TypeCode.Int32: return FastCreate<T0, Int32>(target, pi);
                case TypeCode.Int64: return FastCreate<T0, Int64>(target, pi);
                case TypeCode.Boolean: return FastCreate<T0, Boolean>(target, pi);
                case TypeCode.Char: return FastCreate<T0, Char>(target, pi);
                case TypeCode.Byte: return FastCreate<T0, Byte>(target, pi);
                case TypeCode.Decimal: return FastCreate<T0, Decimal>(target, pi);
                case TypeCode.DateTime: return FastCreate<T0, DateTime>(target, pi);
                case TypeCode.Double: return FastCreate<T0, Double>(target, pi);
                case TypeCode.Single: return FastCreate<T0, Single>(target, pi);
                case TypeCode.UInt16: return FastCreate<T0, UInt16>(target, pi);
                case TypeCode.UInt32: return FastCreate<T0, UInt32>(target, pi);
                case TypeCode.UInt64: return FastCreate<T0, UInt64>(target, pi);
                case TypeCode.String: return FastCreate<T0, String>(target, pi);
                case TypeCode.SByte: return FastCreate<T0, SByte>(target, pi);
                default: return SlowCreate(target, pi);
            }
        }

        private static CallInstruction FastCreate<T0, T1>(MethodInfo target, ParameterInfo[] pi)
        {
            Type t = TryGetParameterOrReturnType(target, pi, 2);
            if (t == null)
            {
                if (target.ReturnType == typeof(void))
                {
                    return new ActionCallInstruction<T0, T1>(target);
                }
                return new FuncCallInstruction<T0, T1>(target);
            }

            if (t.GetTypeInfo().IsEnum) return SlowCreate(target, pi);
            switch (TypeExtensions.GetTypeCode(t))
            {
                case TypeCode.Object:
                    {
                        Debug.Assert(pi.Length == 2);
                        if (t.GetTypeInfo().IsValueType) goto default;

                        return new FuncCallInstruction<T0, T1, Object>(target);
                    }
                case TypeCode.Int16: return new FuncCallInstruction<T0, T1, Int16>(target);
                case TypeCode.Int32: return new FuncCallInstruction<T0, T1, Int32>(target);
                case TypeCode.Int64: return new FuncCallInstruction<T0, T1, Int64>(target);
                case TypeCode.Boolean: return new FuncCallInstruction<T0, T1, Boolean>(target);
                case TypeCode.Char: return new FuncCallInstruction<T0, T1, Char>(target);
                case TypeCode.Byte: return new FuncCallInstruction<T0, T1, Byte>(target);
                case TypeCode.Decimal: return new FuncCallInstruction<T0, T1, Decimal>(target);
                case TypeCode.DateTime: return new FuncCallInstruction<T0, T1, DateTime>(target);
                case TypeCode.Double: return new FuncCallInstruction<T0, T1, Double>(target);
                case TypeCode.Single: return new FuncCallInstruction<T0, T1, Single>(target);
                case TypeCode.UInt16: return new FuncCallInstruction<T0, T1, UInt16>(target);
                case TypeCode.UInt32: return new FuncCallInstruction<T0, T1, UInt32>(target);
                case TypeCode.UInt64: return new FuncCallInstruction<T0, T1, UInt64>(target);
                case TypeCode.String: return new FuncCallInstruction<T0, T1, String>(target);
                case TypeCode.SByte: return new FuncCallInstruction<T0, T1, SByte>(target);
                default: return SlowCreate(target, pi);
            }
        }
#endif

#if FEATURE_DLG_INVOKE
        private static Type GetHelperType(MethodInfo info, Type[] arrTypes)
        {
            Type t;
            if (info.ReturnType == typeof(void))
            {
                switch (arrTypes.Length)
                {
                    case 0: t = typeof(ActionCallInstruction); break;
                    case 1: t = typeof(ActionCallInstruction<>).MakeGenericType(arrTypes); break;
                    case 2: t = typeof(ActionCallInstruction<,>).MakeGenericType(arrTypes); break;
                    default: throw new InvalidOperationException();
                }
            }
            else
            {
                switch (arrTypes.Length)
                {
                    case 1: t = typeof(FuncCallInstruction<>).MakeGenericType(arrTypes); break;
                    case 2: t = typeof(FuncCallInstruction<,>).MakeGenericType(arrTypes); break;
                    case 3: t = typeof(FuncCallInstruction<,,>).MakeGenericType(arrTypes); break;
                    default: throw new InvalidOperationException();
                }
            }
            return t;
        }
#endif
    }

#if FEATURE_DLG_INVOKE
    internal sealed class ActionCallInstruction : CallInstruction
    {
        private readonly Action _target;
        public override int ArgumentCount { get { return 0; } }

        public ActionCallInstruction(Action target)
        {
            _target = target;
        }

        public override int ProducedStack { get { return 0; } }

        public ActionCallInstruction(MethodInfo target)
        {
            _target = (Action)target.CreateDelegate(typeof(Action), target);
        }

        public override object Invoke()
        {
            _target();
            return null;
        }

        public override int Run(InterpretedFrame frame)
        {
            _target();
            frame.StackIndex -= 0;
            return 1;
        }
    }

    internal sealed class ActionCallInstruction<T0> : CallInstruction
    {
        private readonly Action<T0> _target;
        public override int ProducedStack { get { return 0; } }
        public override int ArgumentCount { get { return 1; } }

        public ActionCallInstruction(Action<T0> target)
        {
            _target = target;
        }

        public ActionCallInstruction(MethodInfo target)
        {
            _target = (Action<T0>)target.CreateDelegate(typeof(Action<T0>), target);
        }

        public override object Invoke(object arg0)
        {
            _target(arg0 != null ? (T0)arg0 : default(T0));
            return null;
        }

        public override int Run(InterpretedFrame frame)
        {
            _target((T0)frame.Data[frame.StackIndex - 1]);
            frame.StackIndex -= 1;
            return 1;
        }
    }

    internal sealed class ActionCallInstruction<T0, T1> : CallInstruction
    {
        private readonly Action<T0, T1> _target;
        public override int ProducedStack { get { return 0; } }
        public override int ArgumentCount { get { return 2; } }

        public ActionCallInstruction(Action<T0, T1> target)
        {
            _target = target;
        }

        public ActionCallInstruction(MethodInfo target)
        {
            _target = (Action<T0, T1>)target.CreateDelegate(typeof(Action<T0, T1>), target);
        }

        public override object Invoke(object arg0, object arg1)
        {
            _target(arg0 != null ? (T0)arg0 : default(T0), arg1 != null ? (T1)arg1 : default(T1));
            return null;
        }

        public override int Run(InterpretedFrame frame)
        {
            _target((T0)frame.Data[frame.StackIndex - 2], (T1)frame.Data[frame.StackIndex - 1]);
            frame.StackIndex -= 2;
            return 1;
        }
    }

    internal sealed class FuncCallInstruction<TRet> : CallInstruction
    {
        private readonly Func<TRet> _target;
        public override int ProducedStack { get { return 1; } }
        public override int ArgumentCount { get { return 0; } }

        public FuncCallInstruction(Func<TRet> target)
        {
            _target = target;
        }

        public FuncCallInstruction(MethodInfo target)
        {
            _target = (Func<TRet>)target.CreateDelegate(typeof(Func<TRet>), target);
        }

        public override object Invoke()
        {
            return _target();
        }

        public override int Run(InterpretedFrame frame)
        {
            frame.Data[frame.StackIndex - 0] = _target();
            frame.StackIndex -= -1;
            return 1;
        }
    }

    internal sealed class FuncCallInstruction<T0, TRet> : CallInstruction
    {
        private readonly Func<T0, TRet> _target;
        public override int ProducedStack { get { return 1; } }
        public override int ArgumentCount { get { return 1; } }

        public FuncCallInstruction(Func<T0, TRet> target)
        {
            _target = target;
        }

        public FuncCallInstruction(MethodInfo target)
        {
            _target = (Func<T0, TRet>)target.CreateDelegate(typeof(Func<T0, TRet>), target);
        }

        public override object Invoke(object arg0)
        {
            return _target(arg0 != null ? (T0)arg0 : default(T0));
        }

        public override int Run(InterpretedFrame frame)
        {
            frame.Data[frame.StackIndex - 1] = _target((T0)frame.Data[frame.StackIndex - 1]);
            frame.StackIndex -= 0;
            return 1;
        }
    }

    internal sealed class FuncCallInstruction<T0, T1, TRet> : CallInstruction
    {
        private readonly Func<T0, T1, TRet> _target;
        public override int ProducedStack { get { return 1; } }
        public override int ArgumentCount { get { return 2; } }

        public FuncCallInstruction(Func<T0, T1, TRet> target)
        {
            _target = target;
        }

        public FuncCallInstruction(MethodInfo target)
        {
            _target = (Func<T0, T1, TRet>)target.CreateDelegate(typeof(Func<T0, T1, TRet>), target);
        }

        public override object Invoke(object arg0, object arg1)
        {
            return _target(arg0 != null ? (T0)arg0 : default(T0), arg1 != null ? (T1)arg1 : default(T1));
        }

        public override int Run(InterpretedFrame frame)
        {
            frame.Data[frame.StackIndex - 2] = _target((T0)frame.Data[frame.StackIndex - 2], (T1)frame.Data[frame.StackIndex - 1]);
            frame.StackIndex -= 1;
            return 1;
        }
    }
#endif
}
