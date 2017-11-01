// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;

namespace System.ComponentModel.Composition.ReflectionModel
{
    internal class PartCreatorExportDefinition : ExportDefinition
    {
        private readonly ExportDefinition _productDefinition;
        private IDictionary<string, object> _metadata;

        public PartCreatorExportDefinition(ExportDefinition productDefinition)
            : base()
        {
            this._productDefinition = productDefinition;
        }

        public override string ContractName
        {
            get
            {
                return CompositionConstants.PartCreatorContractName;
            }
        }

        public override IDictionary<string, object> Metadata
        {
            get
            {
                if (this._metadata == null)
                {
                    var metadata = new Dictionary<string, object>(this._productDefinition.Metadata);
                    metadata[CompositionConstants.ExportTypeIdentityMetadataName] = CompositionConstants.PartCreatorTypeIdentity;
                    metadata[CompositionConstants.ProductDefinitionMetadataName] = this._productDefinition;

                    this._metadata = metadata.AsReadOnly();
                }
                return this._metadata;
            }
        }

        internal static bool IsProductConstraintSatisfiedBy(ImportDefinition productImportDefinition, ExportDefinition exportDefinition)
        {
            object productValue = null;
            if (exportDefinition.Metadata.TryGetValue(CompositionConstants.ProductDefinitionMetadataName, out productValue))
            {
                ExportDefinition productDefinition = productValue as ExportDefinition;

                if (productDefinition != null)
                {
                    return productImportDefinition.IsConstraintSatisfiedBy(productDefinition);
                }
            }

            return false;
        }
    }
}
