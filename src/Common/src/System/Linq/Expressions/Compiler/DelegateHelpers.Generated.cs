// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic.Utils;

namespace System.Linq.Expressions.Compiler
{
    internal static partial class DelegateHelpers
    {
        private static TypeInfo _DelegateCache = new TypeInfo();

        private const int MaximumArity = 17;

        internal class TypeInfo
        {
            public Type DelegateType;
            public Dictionary<Type, TypeInfo> TypeChain;
        }

        private static TypeInfo NextTypeInfo(Type initialArg, TypeInfo curTypeInfo)
        {
            Type lookingUp = initialArg;
            TypeInfo nextTypeInfo;
            if (curTypeInfo.TypeChain == null)
            {
                curTypeInfo.TypeChain = new Dictionary<Type, TypeInfo>();
            }

            if (!curTypeInfo.TypeChain.TryGetValue(lookingUp, out nextTypeInfo))
            {
                nextTypeInfo = new TypeInfo();
                if (TypeUtils.CanCache(lookingUp))
                {
                    curTypeInfo.TypeChain[lookingUp] = nextTypeInfo;
                }
            }
            return nextTypeInfo;
        }

#if !FEATURE_COMPILE

        public delegate object VBCallSiteDelegate0<T>(T callSite, object instance);
        public delegate object VBCallSiteDelegate1<T>(T callSite, object instance, ref object arg1);
        public delegate object VBCallSiteDelegate2<T>(T callSite, object instance, ref object arg1, ref object arg2);
        public delegate object VBCallSiteDelegate3<T>(T callSite, object instance, ref object arg1, ref object arg2, ref object arg3);
        public delegate object VBCallSiteDelegate4<T>(T callSite, object instance, ref object arg1, ref object arg2, ref object arg3, ref object arg4);
        public delegate object VBCallSiteDelegate5<T>(T callSite, object instance, ref object arg1, ref object arg2, ref object arg3, ref object arg4, ref object arg5);
        public delegate object VBCallSiteDelegate6<T>(T callSite, object instance, ref object arg1, ref object arg2, ref object arg3, ref object arg4, ref object arg5, ref object arg6);
        public delegate object VBCallSiteDelegate7<T>(T callSite, object instance, ref object arg1, ref object arg2, ref object arg3, ref object arg4, ref object arg5, ref object arg6, ref object arg7);


        private static Type TryMakeVBStyledCallSite(Type[] types)
        {
            // Shape of VB CallSiteDelegates is CallSite * (instance : obj) * [arg-n : byref obj] -> obj
            // array of arguments should contain at least 3 elements (callsite, instance and return type)
            if (types.Length < 3 || types[0].IsByRef || types[1] != typeof(object) || types[types.Length - 1] != typeof(object))
            {
                return null;
            }

            // check if all arguments starting from the second has type byref<obj>
            for (int i = 2; i < types.Length - 2; ++i)
            {
                Type t = types[i];
                if (!t.IsByRef || t.GetElementType() != typeof(object))
                {
                    return null;
                }
            }

            switch (types.Length - 1)
            {
                case 2: return typeof(VBCallSiteDelegate0<>).MakeGenericType(types[0]);
                case 3: return typeof(VBCallSiteDelegate1<>).MakeGenericType(types[0]);
                case 4: return typeof(VBCallSiteDelegate2<>).MakeGenericType(types[0]);
                case 5: return typeof(VBCallSiteDelegate3<>).MakeGenericType(types[0]);
                case 6: return typeof(VBCallSiteDelegate4<>).MakeGenericType(types[0]);
                case 7: return typeof(VBCallSiteDelegate5<>).MakeGenericType(types[0]);
                case 8: return typeof(VBCallSiteDelegate6<>).MakeGenericType(types[0]);
                case 9: return typeof(VBCallSiteDelegate7<>).MakeGenericType(types[0]);
                default: return null;
            }
        }
#endif 

        /// <summary>
        /// Creates a new delegate, or uses a func/action
        /// Note: this method does not cache
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        internal static Type MakeNewDelegate(Type[] types)
        {
            Debug.Assert(types != null && types.Length > 0);

            // Can only used predefined delegates if we have no byref types and
            // the arity is small enough to fit in Func<...> or Action<...>
            bool needCustom;

            if (types.Length > MaximumArity)
            {
                needCustom = true;
            }
            else
            {
                needCustom = false;

                for (int i = 0; i < types.Length; i++)
                {
                    if (types[i].IsByRef)
                    {
                        needCustom = true;
                        break;
                    }
                }
            }

            if (needCustom)
            {
#if FEATURE_COMPILE
                return MakeNewCustomDelegate(types);
#else
                return TryMakeVBStyledCallSite(types) ?? MakeNewCustomDelegate(types);
#endif 
            }

            Type result;
            if (types[types.Length - 1] == typeof(void))
            {
                result = GetActionType(types.RemoveLast());
            }
            else
            {
                result = GetFuncType(types);
            }
            Debug.Assert(result != null);
            return result;
        }

        internal static Type GetFuncType(Type[] types)
        {
            switch (types.Length)
            {
                case 1: return typeof(Func<>).MakeGenericType(types);
                case 2: return typeof(Func<,>).MakeGenericType(types);
                case 3: return typeof(Func<,,>).MakeGenericType(types);
                case 4: return typeof(Func<,,,>).MakeGenericType(types);
                case 5: return typeof(Func<,,,,>).MakeGenericType(types);
                case 6: return typeof(Func<,,,,,>).MakeGenericType(types);
                case 7: return typeof(Func<,,,,,,>).MakeGenericType(types);
                case 8: return typeof(Func<,,,,,,,>).MakeGenericType(types);
                case 9: return typeof(Func<,,,,,,,,>).MakeGenericType(types);
                case 10: return typeof(Func<,,,,,,,,,>).MakeGenericType(types);
                case 11: return typeof(Func<,,,,,,,,,,>).MakeGenericType(types);
                case 12: return typeof(Func<,,,,,,,,,,,>).MakeGenericType(types);
                case 13: return typeof(Func<,,,,,,,,,,,,>).MakeGenericType(types);
                case 14: return typeof(Func<,,,,,,,,,,,,,>).MakeGenericType(types);
                case 15: return typeof(Func<,,,,,,,,,,,,,,>).MakeGenericType(types);
                case 16: return typeof(Func<,,,,,,,,,,,,,,,>).MakeGenericType(types);
                case 17: return typeof(Func<,,,,,,,,,,,,,,,,>).MakeGenericType(types);

                default: return null;
            }
        }

        internal static Type GetActionType(Type[] types)
        {
            switch (types.Length)
            {
                case 0: return typeof(Action);

                case 1: return typeof(Action<>).MakeGenericType(types);
                case 2: return typeof(Action<,>).MakeGenericType(types);
                case 3: return typeof(Action<,,>).MakeGenericType(types);
                case 4: return typeof(Action<,,,>).MakeGenericType(types);
                case 5: return typeof(Action<,,,,>).MakeGenericType(types);
                case 6: return typeof(Action<,,,,,>).MakeGenericType(types);
                case 7: return typeof(Action<,,,,,,>).MakeGenericType(types);
                case 8: return typeof(Action<,,,,,,,>).MakeGenericType(types);
                case 9: return typeof(Action<,,,,,,,,>).MakeGenericType(types);
                case 10: return typeof(Action<,,,,,,,,,>).MakeGenericType(types);
                case 11: return typeof(Action<,,,,,,,,,,>).MakeGenericType(types);
                case 12: return typeof(Action<,,,,,,,,,,,>).MakeGenericType(types);
                case 13: return typeof(Action<,,,,,,,,,,,,>).MakeGenericType(types);
                case 14: return typeof(Action<,,,,,,,,,,,,,>).MakeGenericType(types);
                case 15: return typeof(Action<,,,,,,,,,,,,,,>).MakeGenericType(types);
                case 16: return typeof(Action<,,,,,,,,,,,,,,,>).MakeGenericType(types);

                default: return null;
            }
        }
    }
}
