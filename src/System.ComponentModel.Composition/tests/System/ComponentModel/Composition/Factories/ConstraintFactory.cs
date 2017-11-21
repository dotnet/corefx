// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel.Composition;
using System.Linq.Expressions;
using System.ComponentModel.Composition.Primitives;

namespace System.ComponentModel.Composition.Factories
{
    internal static class ConstraintFactory
    {
        public static Expression<Func<ExportDefinition, bool>> Create(string contractName)
        {
            return definition => definition.ContractName.Equals(contractName);
        }
    }
}
