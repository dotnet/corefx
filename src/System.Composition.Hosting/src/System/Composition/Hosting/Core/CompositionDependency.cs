// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Composition.Hosting.Util;
using System.Linq;
using System.Text;
using Microsoft.Internal;

namespace System.Composition.Hosting.Core
{
    /// <summary>
    /// Describes a dependency that a part must have in order to fulfill an
    /// <see cref="ExportDescriptorPromise"/>. Used by the composition engine during
    /// initialization to determine whether the composition can be made, and if not,
    /// what error to provide.
    /// </summary>
    public class CompositionDependency
    {
        private readonly ExportDescriptorPromise _target;
        private readonly bool _isPrerequisite;
        private readonly object _site;
        private readonly CompositionContract _contract;

        // Carrying some information to later use in error messages - 
        // it may be better to just store the message.
        private readonly ExportDescriptorPromise[] _oversuppliedTargets;

        /// <summary>
        /// Construct a dependency on the specified target.
        /// </summary>
        /// <param name="target">The export descriptor promise from another part
        /// that this part is dependent on.</param>
        /// <param name="isPrerequisite">True if the dependency is a prerequisite
        /// that must be satisfied before any exports can be retrieved from the dependent
        /// part; otherwise, false.</param>
        /// <param name="site">A marker used to identify the individual dependency among
        /// those on the dependent part.</param>
        /// <param name="contract">The contract required by the dependency.</param>
        public static CompositionDependency Satisfied(CompositionContract contract, ExportDescriptorPromise target, bool isPrerequisite, object site)
        {
            Requires.NotNull(target, nameof(target));
            Requires.NotNull(site, nameof(site));
            Requires.NotNull(contract, nameof(contract));

            return new CompositionDependency(contract, target, isPrerequisite, site);
        }

        /// <summary>
        /// Construct a placeholder for a missing dependency. Note that this is different
        /// from an optional dependency - a missing dependency is an error.
        /// </summary>
        /// <param name="site">A marker used to identify the individual dependency among
        /// those on the dependent part.</param>
        /// <param name="contract">The contract required by the dependency.</param>
        public static CompositionDependency Missing(CompositionContract contract, object site)
        {
            Requires.NotNull(contract, nameof(contract));
            Requires.NotNull(site, nameof(site));

            return new CompositionDependency(contract, site);
        }

        /// <summary>
        /// Construct a placeholder for an "exactly one" dependency that cannot be
        /// configured because multiple target implementations exist.
        /// </summary>
        /// <param name="site">A marker used to identify the individual dependency among
        /// those on the dependent part.</param>
        /// <param name="targets">The targets found when expecting only one.</param>
        /// <param name="contract">The contract required by the dependency.</param>
        public static CompositionDependency Oversupplied(CompositionContract contract, IEnumerable<ExportDescriptorPromise> targets, object site)
        {
            Requires.NotNull(targets, nameof(targets));
            Requires.NotNull(site, nameof(site));
            Requires.NotNull(contract, nameof(contract));

            return new CompositionDependency(contract, targets, site);
        }

        private CompositionDependency(CompositionContract contract, ExportDescriptorPromise target, bool isPrerequisite, object site)
        {
            _target = target;
            _isPrerequisite = isPrerequisite;
            _site = site;
            _contract = contract;
        }

        private CompositionDependency(CompositionContract contract, object site)
        {
            _contract = contract;
            _site = site;
        }

        private CompositionDependency(CompositionContract contract, IEnumerable<ExportDescriptorPromise> targets, object site)
        {
            _oversuppliedTargets = targets.ToArray();
            _site = site;
            _contract = contract;
        }

        /// <summary>
        /// The export descriptor promise from another part
        /// that this part is dependent on.
        /// </summary>
        public ExportDescriptorPromise Target { get { return _target; } }

        /// <summary>
        /// True if the dependency is a prerequisite
        /// that must be satisfied before any exports can be retrieved from the dependent
        /// part; otherwise, false.
        /// </summary>
        public bool IsPrerequisite { get { return _isPrerequisite; } }

        /// <summary>
        /// A marker used to identify the individual dependency among
        /// those on the dependent part.
        /// </summary>
        public object Site { get { return _site; } }

        /// <summary>
        /// The contract required by the dependency.
        /// </summary>
        public CompositionContract Contract { get { return _contract; } }

        /// <summary>
        /// Creates a human-readable explanation of the dependency.
        /// </summary>
        /// <returns>The dependency represented as a string.</returns>
        public override string ToString()
        {
            if (IsError)
                return Site.ToString();

            return string.Format(Properties.Resources.Dependency_ToStringFormat, Site, Target.Contract, Target.Origin);
        }

        internal bool IsError { get { return _target == null; } }

        internal void DescribeError(StringBuilder message)
        {
            Assumes.IsTrue(IsError, "Dependency is not in an error state.");

            if (_oversuppliedTargets != null)
            {
                var list = Formatters.ReadableList(_oversuppliedTargets.Select(t => string.Format(Properties.Resources.Dependency_QuoteParameter, t.Origin)));
                message.AppendFormat(Properties.Resources.Dependency_TooManyExports, Contract, list);
            }
            else
            {
                message.AppendFormat(Properties.Resources.Dependency_ExportNotFound, Contract);
            }
        }
    }
}
