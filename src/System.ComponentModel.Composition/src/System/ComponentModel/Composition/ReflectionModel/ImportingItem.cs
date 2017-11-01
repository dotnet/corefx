// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Globalization;
using System.Linq;
using Microsoft.Internal;
using Microsoft.Internal.Collections;

namespace System.ComponentModel.Composition.ReflectionModel
{
    internal abstract class ImportingItem
    {
        private readonly ContractBasedImportDefinition _definition;
        private readonly ImportType _importType;

        protected ImportingItem(ContractBasedImportDefinition definition, ImportType importType)
        {
            Assumes.NotNull(definition);

            this._definition = definition;
            this._importType = importType;
        }

        public ContractBasedImportDefinition Definition
        {
            get { return this._definition; }
        }

        public ImportType ImportType
        {
            get { return this._importType; }
        }

        public object CastExportsToImportType(Export[] exports)
        {
            if (this.Definition.Cardinality == ImportCardinality.ZeroOrMore)
            {
                return CastExportsToCollectionImportType(exports);
            }
            else
            {
                return CastExportsToSingleImportType(exports);
            }
        }

        private object CastExportsToCollectionImportType(Export[] exports)
        {
            Assumes.NotNull(exports);

            // Element type could be null if the actually import type of the member is not a collection
            // This particular case will end up failing when we set the member.
            Type elementType = this.ImportType.ElementType ?? typeof(object);

            Array array = Array.CreateInstance(elementType, exports.Length);

            for (int i = 0; i < array.Length; i++)
            {
                object value = CastSingleExportToImportType(elementType, exports[i]);

                array.SetValue(value, i);
            }

            return array;
        }

        private object CastExportsToSingleImportType(Export[] exports)
        {
            Assumes.NotNull(exports);
            Assumes.IsTrue(exports.Length < 2);

            if (exports.Length == 0)
            {   
                return null;
            }

            return CastSingleExportToImportType(this.ImportType.ActualType, exports[0]);
        }

        private object CastSingleExportToImportType(Type type, Export export)
        {
            if (this.ImportType.CastExport != null)
            {
                return this.ImportType.CastExport(export);
            }

            return Cast(type, export);
        }

        private object Cast(Type type, Export export)
        {
            object value = export.Value;

            object result;
            if (!ContractServices.TryCast(type, value, out result))
            {
                throw new ComposablePartException(
                    String.Format(CultureInfo.CurrentCulture,
                        SR.ReflectionModel_ImportNotAssignableFromExport,
                        export.ToElement().DisplayName,
                        type.FullName),
                    this.Definition.ToElement());
            }

            return result;
        }
    }
}