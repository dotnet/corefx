// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace System.ComponentModel.Composition.ReflectionModel
{
    internal class PartCreatorParameterImportDefinition : ReflectionParameterImportDefinition, IPartCreatorImportDefinition
    {
        private readonly ContractBasedImportDefinition _productImportDefinition;

        public PartCreatorParameterImportDefinition(
            Lazy<ParameterInfo> importingLazyParameter,
            ICompositionElement origin,
            ContractBasedImportDefinition productImportDefinition)
            : base(importingLazyParameter, CompositionConstants.PartCreatorContractName, CompositionConstants.PartCreatorTypeIdentity,
                productImportDefinition.RequiredMetadata, productImportDefinition.Cardinality, CreationPolicy.Any, MetadataServices.EmptyMetadata, origin)
        {
            if (productImportDefinition == null)
            {
                throw new ArgumentNullException(nameof(productImportDefinition));
            }
            _productImportDefinition = productImportDefinition;
        }

        public ContractBasedImportDefinition ProductImportDefinition { get { return _productImportDefinition; } }
        public override bool IsConstraintSatisfiedBy(ExportDefinition exportDefinition)
        {
            if (!base.IsConstraintSatisfiedBy(exportDefinition))
            {
                return false;
            }
            return PartCreatorExportDefinition.IsProductConstraintSatisfiedBy(_productImportDefinition, exportDefinition);
        }

        public override Expression<Func<ExportDefinition, bool>> Constraint
        {
            get
            {
                return ConstraintServices.CreatePartCreatorConstraint(base.Constraint, _productImportDefinition);
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            
            sb.Append(string.Format("\n\tExportFactory of: {0}", ProductImportDefinition.ToString()));
            
            return sb.ToString();
        }

    }
}
