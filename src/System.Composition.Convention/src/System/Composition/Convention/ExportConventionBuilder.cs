// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;
using System.Reflection;

using Microsoft.Internal;

namespace System.Composition.Convention
{
    /// <summary>
    /// Configures an export associated with a part.
    /// </summary>
    public sealed class ExportConventionBuilder
    {
        private string _contractName;
        private Type _contractType;
        private List<Tuple<string, object>> _metadataItems;
        private List<Tuple<string, Func<Type, object>>> _metadataItemFuncs;
        private Func<Type, string> _getContractNameFromPartType;

        internal ExportConventionBuilder() { }

        /// <summary>
        /// Specify the contract type for the export.
        /// </summary>
        /// <typeparam name="T">The contract type.</typeparam>
        /// <returns>An export builder allowing further configuration.</returns>
        public ExportConventionBuilder AsContractType<T>()
        {
            return AsContractType(typeof(T));
        }

        /// <summary>
        /// Specify the contract type for the export.
        /// </summary>
        /// <param name="type">The contract type.</param>
        /// <returns>An export builder allowing further configuration.</returns>
        public ExportConventionBuilder AsContractType(Type type)
        {
            Requires.NotNull(type, "type");
            this._contractType = type;
            return this;
        }

        /// <summary>
        /// Specify the contract name for the export.
        /// </summary>
        /// <param name="contractName">The contract name.</param>
        /// <returns>An export builder allowing further configuration.</returns>
        public ExportConventionBuilder AsContractName(string contractName)
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
        public ExportConventionBuilder AsContractName(Func<Type, string> getContractNameFromPartType)
        {
            Requires.NotNull(getContractNameFromPartType, "getContractNameFromPartType");
            this._getContractNameFromPartType = getContractNameFromPartType;
            return this;
        }

        /// <summary>
        /// Add export metadata to the export.
        /// </summary>
        /// <param name="name">The name of the metadata item.</param>
        /// <param name="value">The value of the metadata item.</param>
        /// <returns>An export builder allowing further configuration.</returns>
        public ExportConventionBuilder AddMetadata(string name, object value)
        {
            Requires.NotNullOrEmpty(name, "name");
            if (this._metadataItems == null)
            {
                this._metadataItems = new List<Tuple<string, object>>();
            }
            this._metadataItems.Add(Tuple.Create(name, value));
            return this;
        }

        /// <summary>
        /// Add export metadata to the export.
        /// </summary>
        /// <param name="name">The name of the metadata item.</param>
        /// <param name="getValueFromPartType">A function that calculates the metadata value based on the type.</param>
        /// <returns>An export builder allowing further configuration.</returns>
        public ExportConventionBuilder AddMetadata(string name, Func<Type, object> getValueFromPartType)
        {
            Requires.NotNullOrEmpty(name, "name");
            Requires.NotNull(getValueFromPartType, "getValueFromPartType");
            if (this._metadataItemFuncs == null)
            {
                this._metadataItemFuncs = new List<Tuple<string, Func<Type, object>>>();
            }
            this._metadataItemFuncs.Add(Tuple.Create(name, getValueFromPartType));
            return this;
        }

        internal void BuildAttributes(Type type, ref List<Attribute> attributes)
        {
            if (attributes == null)
            {
                attributes = new List<Attribute>();
            }

            var contractName = (this._getContractNameFromPartType != null) ? this._getContractNameFromPartType(type) : this._contractName;
            attributes.Add(new ExportAttribute(contractName, this._contractType));

            //Add metadata attributes from direct specification
            if (this._metadataItems != null)
            {
                foreach (var item in this._metadataItems)
                {
                    attributes.Add(new ExportMetadataAttribute(item.Item1, item.Item2));
                }
            }

            //Add metadata attributes from func specification
            if (this._metadataItemFuncs != null)
            {
                foreach (var item in this._metadataItemFuncs)
                {
                    var name = item.Item1;
                    var value = (item.Item2 != null) ? item.Item2(type) : null;
                    attributes.Add(new ExportMetadataAttribute(name, value));
                }
            }
        }
    }
}
