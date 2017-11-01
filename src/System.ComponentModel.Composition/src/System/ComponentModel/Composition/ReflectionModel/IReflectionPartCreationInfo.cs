// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Reflection;
using System.Threading;

namespace System.ComponentModel.Composition.ReflectionModel
{
    internal interface IReflectionPartCreationInfo : ICompositionElement
    {
        Type GetPartType();
        Lazy<Type> GetLazyPartType();
        ConstructorInfo GetConstructor();
        IDictionary<string, object> GetMetadata();
        IEnumerable<ExportDefinition> GetExports();
        IEnumerable<ImportDefinition> GetImports();
        bool IsDisposalRequired { get; }
        bool IsIdentityComparison { get; }
    }
}
