// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Internal;

namespace System.Composition.Convention
{
    /// <summary>
    /// Configures an import associated with a part.
    /// </summary>
    public sealed class ImportConventionBuilder
    {
        private static readonly Type[] _SupportedImportManyTypes = new[] { typeof(IList<>), typeof(ICollection<>), typeof(IEnumerable<>) };

        private string _contractName;
        private bool? _asMany;
        private bool _allowDefault;
        private Func<Type, string> _getContractNameFromPartType;
        private List<Tuple<string, object>> _metadataConstraintItems;
        private List<Tuple<string, Func<Type, object>>> _metadataConstraintItemFuncs;

        internal ImportConventionBuilder() { }

        /// <summary>
        /// Specify the contract name for the import.
        /// </summary>
        /// <param name="contractName"></param>
        /// <returns>An import builder allowing further configuration.</returns>
        public ImportConventionBuilder AsContractName(string contractName)
        {
            Requires.NotNullOrEmpty(contractName, "contractName");
            this._contractName = contractName;
            return this;
        }

        /// <summary>
        /// Specify the contract name for the export.
        /// </summary>
        /// <param name="getContractNameFromPartType">A Func to retrieve the contract name from the part typeThe contract name.</param>
        /// <returns>An export builder allowing further configuration.</returns>
        public ImportConventionBuilder AsContractName(Func<Type, string> getContractNameFromPartType)
        {
            Requires.NotNull(getContractNameFromPartType, "getContractNameFromPartType");
            this._getContractNameFromPartType = getContractNameFromPartType;
            return this;
        }

        /// <summary>
        /// Configure the import to receive all exports of the contract.
        /// </summary>
        /// <returns>An import builder allowing further configuration.</returns>
        public ImportConventionBuilder AsMany()
        {
            return AsMany(true);
        }

        /// <summary>
        /// Configure the import to receive all exports of the contract.
        /// </summary>
        /// <param name="isMany">True if the import receives all values; otherwise false.</param>
        /// <returns>An import builder allowing further configuration.</returns>
        public ImportConventionBuilder AsMany(bool isMany)
        {
            this._asMany = isMany;
            return this;
        }

        /// <summary>
        /// Allow the import to receive the default value for its type if
        /// the contract cannot be supplied by another part.
        /// </summary>
        /// <returns>An import builder allowing further configuration.</returns>
        public ImportConventionBuilder AllowDefault()
        {
            this._allowDefault = true;
            return this;
        }

        /// <summary>
        /// Add an import constraint
        /// </summary>
        /// <param name="name">The name of the constraint item.</param>
        /// <param name="value">The value to match.</param>
        /// <returns>An import builder allowing further configuration.</returns>
        public ImportConventionBuilder AddMetadataConstraint(string name, object value)
        {
            Requires.NotNullOrEmpty(name, "name");
            if (this._metadataConstraintItems == null)
            {
                this._metadataConstraintItems = new List<Tuple<string, object>>();
            }
            this._metadataConstraintItems.Add(Tuple.Create(name, value));
            return this;
        }

        /// <summary>
        /// Add an import constraint
        /// </summary>
        /// <param name="name">The name of the constraint item.</param>
        /// <param name="getConstraintValueFromPartType">A function that calculates the value to match.</param>
        /// <returns>An export builder allowing further configuration.</returns>
        public ImportConventionBuilder AddMetadataConstraint(string name, Func<Type, object> getConstraintValueFromPartType)
        {
            Requires.NotNullOrEmpty(name, "name");
            Requires.NotNull(getConstraintValueFromPartType, "getConstraintValueFromPartType");

            if (this._metadataConstraintItemFuncs == null)
            {
                this._metadataConstraintItemFuncs = new List<Tuple<string, Func<Type, object>>>();
            }
            this._metadataConstraintItemFuncs.Add(Tuple.Create(name, getConstraintValueFromPartType));
            return this;
        }

        internal void BuildAttributes(Type type, ref List<Attribute> attributes)
        {
            Attribute importAttribute;

            var contractName = (this._getContractNameFromPartType != null) ? this._getContractNameFromPartType(type) : this._contractName;

            // Infer from Type when not explicitly set.
            var asMany = _asMany ?? IsSupportedImportManyType(type.GetTypeInfo());
            if (!asMany)
            {
                importAttribute = new ImportAttribute(contractName)
                {
                    AllowDefault = this._allowDefault
                };
            }
            else
            {
                importAttribute = new ImportManyAttribute(contractName);
            }
            if (attributes == null)
            {
                attributes = new List<Attribute>();
            }
            attributes.Add(importAttribute);


            //Add metadata attributes from direct specification
            if (this._metadataConstraintItems != null)
            {
                foreach (var item in this._metadataConstraintItems)
                {
                    attributes.Add(new ImportMetadataConstraintAttribute(item.Item1, item.Item2));
                }
            }

            //Add metadata attributes from func specification
            if (this._metadataConstraintItemFuncs != null)
            {
                foreach (var item in this._metadataConstraintItemFuncs)
                {
                    var name = item.Item1;
                    var value = (item.Item2 != null) ? item.Item2(type) : null;
                    attributes.Add(new ImportMetadataConstraintAttribute(name, value));
                }
            }
            return;
        }

        bool IsSupportedImportManyType(TypeInfo typeInfo)
        {
            return typeInfo.IsArray ||
                (typeInfo.IsGenericTypeDefinition && _SupportedImportManyTypes.Contains(typeInfo.AsType())) ||
                (typeInfo.AsType().IsConstructedGenericType && _SupportedImportManyTypes.Contains(typeInfo.GetGenericTypeDefinition()));
        }
    }
}
