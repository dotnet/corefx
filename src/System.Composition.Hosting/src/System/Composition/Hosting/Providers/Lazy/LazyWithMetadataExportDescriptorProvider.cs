﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using System.Composition.Hosting.Core;
using System.Composition.Runtime;
using System.Linq;
using System.Threading;
using System.Composition.Hosting.Util;
using System.Collections.Generic;
using System.Composition.Hosting.Providers.Metadata;

namespace System.Composition.Hosting.Providers.Lazy
{
    internal class LazyWithMetadataExportDescriptorProvider : ExportDescriptorProvider
    {
        private static readonly MethodInfo s_getLazyDefinitionsMethod = typeof(LazyWithMetadataExportDescriptorProvider).GetTypeInfo().GetDeclaredMethod("GetLazyDefinitions");

        public override IEnumerable<ExportDescriptorPromise> GetExportDescriptors(CompositionContract exportKey, DependencyAccessor definitionAccessor)
        {
            if (!exportKey.ContractType.IsConstructedGenericType || exportKey.ContractType.GetGenericTypeDefinition() != typeof(Lazy<,>))
                return NoExportDescriptors;

            var ga = exportKey.ContractType.GenericTypeArguments;
            var gld = s_getLazyDefinitionsMethod.MakeGenericMethod(ga[0], ga[1]);
            var gldm = gld.CreateStaticDelegate<Func<CompositionContract, DependencyAccessor, object>>();
            return (ExportDescriptorPromise[])gldm(exportKey, definitionAccessor);
        }

        private static ExportDescriptorPromise[] GetLazyDefinitions<TValue, TMetadata>(CompositionContract lazyContract, DependencyAccessor definitionAccessor)
        {
            var metadataProvider = MetadataViewProvider.GetMetadataViewProvider<TMetadata>();

            return definitionAccessor.ResolveDependencies("value", lazyContract.ChangeType(typeof(TValue)), false)
                .Select(d => new ExportDescriptorPromise(
                    lazyContract,
                    Formatters.Format(typeof(Lazy<TValue, TMetadata>)),
                    false,
                    () => new[] { d },
                    _ =>
                    {
                        var dsc = d.Target.GetDescriptor();
                        var da = dsc.Activator;
                        return ExportDescriptor.Create((c, o) =>
                        {
                            return new Lazy<TValue, TMetadata>(() => (TValue)CompositionOperation.Run(c, da), metadataProvider(dsc.Metadata));
                        }, dsc.Metadata);
                    }))
                .ToArray();
        }
    }
}
