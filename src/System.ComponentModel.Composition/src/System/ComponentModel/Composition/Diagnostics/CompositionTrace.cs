// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.ReflectionModel;
using Microsoft.Internal;
using System.Reflection;

namespace System.ComponentModel.Composition.Diagnostics
{
    internal static class CompositionTrace
    {
        internal static void PartDefinitionResurrected(ComposablePartDefinition definition)
        {
            Assumes.NotNull(definition);

            if (CompositionTraceSource.CanWriteInformation)
            {
                CompositionTraceSource.WriteInformation(CompositionTraceId.Rejection_DefinitionResurrected, 
                                                        SR.CompositionTrace_Rejection_DefinitionResurrected, 
                                                        definition.GetDisplayName());
            }
        }

        internal static void PartDefinitionRejected(ComposablePartDefinition definition, ChangeRejectedException exception)
        {
            Assumes.NotNull(definition, exception);

            if (CompositionTraceSource.CanWriteWarning)
            {
                CompositionTraceSource.WriteWarning(CompositionTraceId.Rejection_DefinitionRejected, 
                                                    SR.CompositionTrace_Rejection_DefinitionRejected, 
                                                    definition.GetDisplayName(), 
                                                    exception.Message);
            }
        }

//#if FEATURE_REFLECTIONFILEIO

        internal static void AssemblyLoadFailed(DirectoryCatalog catalog, string fileName, Exception exception)
        {
            Assumes.NotNull(catalog, exception);
            Assumes.NotNullOrEmpty(fileName);            

            if (CompositionTraceSource.CanWriteWarning)
            {
                CompositionTraceSource.WriteWarning(CompositionTraceId.Discovery_AssemblyLoadFailed, 
                                                    SR.CompositionTrace_Discovery_AssemblyLoadFailed, 
                                                    catalog.GetDisplayName(),
                                                    fileName, 
                                                    exception.Message);
            }
        }

//#endif //FEATURE_REFLECTIONFILEIO

        internal static void DefinitionMarkedWithPartNotDiscoverableAttribute(Type type)
        {
            Assumes.NotNull(type);

            if (CompositionTraceSource.CanWriteInformation)
            {
                CompositionTraceSource.WriteInformation(CompositionTraceId.Discovery_DefinitionMarkedWithPartNotDiscoverableAttribute, 
                                                        SR.CompositionTrace_Discovery_DefinitionMarkedWithPartNotDiscoverableAttribute, 
                                                        type.GetDisplayName());
            }
        }

        internal static void DefinitionMismatchedExportArity(Type type, MemberInfo member)
        {
            Assumes.NotNull(type);
            Assumes.NotNull(member);

            if (CompositionTraceSource.CanWriteInformation)
            {
                CompositionTraceSource.WriteInformation(CompositionTraceId.Discovery_DefinitionMismatchedExportArity,
                                                        SR.CompositionTrace_Discovery_DefinitionMismatchedExportArity,
                                                        type.GetDisplayName(), member.GetDisplayName());
            }
        }

        internal static void DefinitionContainsNoExports(Type type)
        {
            Assumes.NotNull(type);

            if (CompositionTraceSource.CanWriteInformation)
            {
                CompositionTraceSource.WriteInformation(CompositionTraceId.Discovery_DefinitionContainsNoExports,
                                                        SR.CompositionTrace_Discovery_DefinitionContainsNoExports,
                                                        type.GetDisplayName());
            }
        }

        internal static void MemberMarkedWithMultipleImportAndImportMany(ReflectionItem item)
        {
            Assumes.NotNull(item);

            if (CompositionTraceSource.CanWriteError)
            {
                CompositionTraceSource.WriteError(CompositionTraceId.Discovery_MemberMarkedWithMultipleImportAndImportMany,
                                                  SR.CompositionTrace_Discovery_MemberMarkedWithMultipleImportAndImportMany,
                                                  item.GetDisplayName());
            }
        }
    }
}
