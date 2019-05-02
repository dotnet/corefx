// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json.Serialization.Policies;

namespace System.Text.Json.Serialization.Converters
{
    // This converter returns enumerables in the System.Collections.Immutable namespace.
    internal sealed class DefaultImmutableConverter : JsonEnumerableConverter
    {
        public const string ImmutableNamespace = "System.Collections.Immutable";

        private const string ImmutableListGenericTypeName = "System.Collections.Immutable.ImmutableList`1";
        private const string ImmutableListGenericInterfaceTypeName = "System.Collections.Immutable.IImmutableList`1";

        private const string ImmutableArrayGenericTypeName = "System.Collections.Immutable.ImmutableArray`1";

        private const string ImmutableStackGenericTypeName = "System.Collections.Immutable.ImmutableStack`1";
        private const string ImmutableStackGenericInterfaceTypeName = "System.Collections.Immutable.IImmutableStack`1";

        private const string ImmutableQueueGenericTypeName = "System.Collections.Immutable.ImmutableQueue`1";
        private const string ImmutableQueueGenericInterfaceTypeName = "System.Collections.Immutable.IImmutableQueue`1";

        private const string ImmutableSortedSetGenericTypeName = "System.Collections.Immutable.ImmutableSortedSet`1";

        private const string ImmutableHashSetGenericTypeName = "System.Collections.Immutable.ImmutableHashSet`1";
        private const string ImmutableSetGenericInterfaceTypeName = "System.Collections.Immutable.IImmutableSet`1";

        // This method is invoked with reflection in CreateFromList.
        private static IEnumerable CreateFromListInternal<T>(MethodInfo createImmutableMethod, IList sourceList)
        {
            List<T> list = new List<T>();

            foreach (object item in sourceList)
            {
                if (item is T itemT)
                {
                    list.Add(itemT);
                }
            }

            return (IEnumerable)createImmutableMethod.Invoke(null, new object[] { list } );
        }

        public override IEnumerable CreateFromList(Type enumerableType, Type elementType, IList sourceList)
        {
            Debug.Assert(enumerableType.IsGenericType);

            Type underlyingType = enumerableType.GetGenericTypeDefinition();
            Type constructingType;
            string name = underlyingType.FullName;

            switch (name)
            {
                case ImmutableListGenericTypeName:
                case ImmutableListGenericInterfaceTypeName:
                    constructingType = typeof(ImmutableList);
                    break;
                case ImmutableArrayGenericTypeName:
                    constructingType = typeof(ImmutableArray);
                    break;
                case ImmutableStackGenericTypeName:
                case ImmutableStackGenericInterfaceTypeName:
                    constructingType = typeof(ImmutableStack);
                    break;
                case ImmutableQueueGenericTypeName:
                case ImmutableQueueGenericInterfaceTypeName:
                    constructingType = typeof(ImmutableQueue);
                    break;
                case ImmutableSortedSetGenericTypeName:
                    constructingType = typeof(ImmutableSortedSet);
                    break;
                case ImmutableHashSetGenericTypeName:
                case ImmutableSetGenericInterfaceTypeName:
                    constructingType = typeof(ImmutableHashSet);
                    break;
                default:
                    return sourceList;
            }

            MethodInfo[] constructingTypeMethods = constructingType.GetMethods();
            MethodInfo constructingMethod = null;

            foreach (MethodInfo method in constructingTypeMethods)
            {
                if (method.Name == "CreateRange" && method.GetParameters().Length == 1)
                {
                    constructingMethod = method;
                    break;
                }
            }

            if (constructingMethod != null)
            {
                if (elementType == null)
                {
                    Type[] args = enumerableType.GetGenericArguments();
                    Debug.Assert(args.Length == 1);

                    elementType = args[0];
                }

                MethodInfo mi = typeof(DefaultImmutableConverter).GetMethod("CreateFromListInternal", BindingFlags.NonPublic | BindingFlags.Static);

                if (mi != null)
                {
                    MethodInfo createImmutableMethod = constructingMethod.MakeGenericMethod(elementType);

                    MethodInfo createFromListInternalMethod = mi.MakeGenericMethod(elementType);
                    return (IEnumerable)createFromListInternalMethod.Invoke(null, new object[] { createImmutableMethod, sourceList });
                }
            }

            return sourceList;
        }
    }
}
