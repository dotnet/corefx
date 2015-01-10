﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// -----------------------------------------------------------------------
// Copyright © Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Composition.Convention;
using System.Composition.Hosting;
using System.Composition.Hosting.Core;
using System.Composition.Runtime;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Composition.TypedParts.ActivationFeatures
{
    /// <summary>
    /// Modifies activators of parts that have <see cref="OnImportsSatisfiedAttribute"/> so that
    /// their [OnImportsSatisfied] method is correctly called.
    /// </summary>
    class OnImportsSatisfiedFeature : ActivationFeature
    {
        private AttributedModelProvider _attributeContext;

        public OnImportsSatisfiedFeature(AttributedModelProvider attributeContext)
        {
            if (attributeContext == null) throw new ArgumentNullException("attributeContext");
            _attributeContext = attributeContext;
        }

        public override CompositeActivator RewriteActivator(
            TypeInfo partType,
            CompositeActivator activator,
            IDictionary<string, object> partMetadata,
            IEnumerable<CompositionDependency> dependencies)
        {
            var result = activator;

            var partTypeAsType = partType.AsType();
            var importsSatisfiedMethods = partTypeAsType.GetRuntimeMethods()
                .Where(mi => _attributeContext.GetDeclaredAttribute<OnImportsSatisfiedAttribute>(mi.DeclaringType, mi) != null);

            foreach (var m in importsSatisfiedMethods)
            {
                if (!(m.IsPublic || m.IsAssembly) | m.IsStatic || m.ReturnType != typeof(void) ||
                    m.IsGenericMethodDefinition || m.GetParameters().Length != 0)
                {
                    var message = string.Format(
                        Properties.Resources.OnImportsSatisfiedFeature_AttributeError,
                        partType, m.Name);
                    throw new CompositionFailedException(message);
                }

                var ois = Expression.Parameter(typeof(object), "ois");
                var call = Expression.Lambda<Action<object>>(
                    Expression.Call(Expression.Convert(ois, partType.AsType()), m), ois).Compile();

                var prev = result;
                result = (c, o) =>
                {
                    var psn = prev(c, o);
                    o.AddPostCompositionAction(() => call(psn));
                    return psn;
                };
            }

            return result;
        }
    }
}
