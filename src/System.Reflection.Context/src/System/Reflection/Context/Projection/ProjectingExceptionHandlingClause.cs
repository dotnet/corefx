// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection.Context.Delegation;
using System.Diagnostics.Contracts;

namespace System.Reflection.Context.Projection
{
    // Recursively 'projects' any assemblies, modules, types and members returned by a given exception handling clause
    internal class ProjectingExceptionHandlingClause : DelegatingExceptionHandlingClause
    {
        private readonly Projector _projector;

        public ProjectingExceptionHandlingClause(ExceptionHandlingClause clause, Projector projector)
            : base(clause)
        {
            Contract.Requires(null != projector);

            _projector = projector;
        }

        public override Type CatchType
        {
            get { return _projector.ProjectType(base.CatchType); }
        }
    }
}
