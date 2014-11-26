﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// -----------------------------------------------------------------------
// Copyright © Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Composition.Hosting.Core;
using System.Composition.Hosting.Util;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Internal;

namespace System.Composition.Hosting.Providers.Metadata
{
    static class MetadataViewProvider
    {
        private static readonly MethodInfo _GetMetadataValueMethod = typeof(MetadataViewProvider).GetTypeInfo().GetDeclaredMethod("GetMetadataValue");

        // While not called through the composition pipeline, we use the dependency mechanism to surface errors
        // with appropriate context information.
        public static Func<IDictionary<string, object>, TMetadata> GetMetadataViewProvider<TMetadata>()
        {
            if (typeof(TMetadata) == typeof(IDictionary<string, object>))
                return m => (TMetadata)m;

            if (!typeof(TMetadata).GetTypeInfo().IsClass)
                throw new CompositionFailedException(string.Format(Properties.Resources.MetadataViewProvider_InvalidViewImplementation, typeof(TMetadata).Name));

            var ti = typeof(TMetadata).GetTypeInfo();
            var dictionaryConstructor = ti.DeclaredConstructors.SingleOrDefault(ci =>
            {
                var ps = ci.GetParameters();
                return ci.IsPublic && ps.Length == 1 && ps[0].ParameterType == typeof(IDictionary<string, object>);
            });

            if (dictionaryConstructor != null)
            {
                var providerArg = Expression.Parameter(typeof(IDictionary<string, object>), "metadata");
                return Expression.Lambda<Func<IDictionary<string, object>, TMetadata>>(
                        Expression.New(dictionaryConstructor, providerArg),
                        providerArg)
                    .Compile();
            }

            var parameterlessConstructor = ti.DeclaredConstructors.SingleOrDefault(ci => ci.IsPublic && ci.GetParameters().Length == 0);
            if (parameterlessConstructor != null)
            {
                var providerArg = Expression.Parameter(typeof(IDictionary<string, object>), "metadata");
                var resultVar = Expression.Variable(typeof(TMetadata), "result");

                var blockExprs = new List<Expression>();
                blockExprs.Add(Expression.Assign(resultVar, Expression.New(parameterlessConstructor)));

                foreach (var prop in typeof(TMetadata).GetTypeInfo().DeclaredProperties
                    .Where(prop =>
                        prop.GetMethod != null && prop.GetMethod.IsPublic && !prop.GetMethod.IsStatic &&
                        prop.SetMethod != null && prop.SetMethod.IsPublic && !prop.SetMethod.IsStatic))
                {
                    var dva = Expression.Constant(prop.GetCustomAttribute<DefaultValueAttribute>(false), typeof(DefaultValueAttribute));
                    var name = Expression.Constant(prop.Name, typeof(string));
                    var m = _GetMetadataValueMethod.MakeGenericMethod(prop.PropertyType);
                    var assign = Expression.Assign(
                        Expression.Property(resultVar, prop),
                        Expression.Call(null, m, providerArg, name, dva));
                    blockExprs.Add(assign);
                }

                blockExprs.Add(resultVar);

                return Expression.Lambda<Func<IDictionary<string, object>, TMetadata>>(
                        Expression.Block(new[] { resultVar }, blockExprs), providerArg)
                    .Compile();
            }

            throw new CompositionFailedException(string.Format(Properties.Resources.MetadataViewProvider_InvalidViewImplementation, typeof(TMetadata).Name));
        }

        static TValue GetMetadataValue<TValue>(IDictionary<string, object> metadata, string name, DefaultValueAttribute defaultValue)
        {
            object result;
            if (metadata.TryGetValue(name, out result))
                return (TValue)result;

            if (defaultValue != null)
                return (TValue)defaultValue.Value;

            // This could be significantly improved by describing the target metadata property.
            var message = string.Format(Properties.Resources.MetadataViewProvider_MissingMetadata, name);
            throw ThrowHelper.CompositionException(message);
        }
    }
}
