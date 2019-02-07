// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.ReflectionModel;
using System.Reflection;

namespace System.ComponentModel.Composition.AttributedModel
{
    internal class AttributedExportDefinition : ExportDefinition
    {
        private readonly AttributedPartCreationInfo _partCreationInfo;
        private readonly MemberInfo _member;
        private readonly ExportAttribute _exportAttribute;
        private readonly Type _typeIdentityType;

        private IDictionary<string, object> _metadata;

        public AttributedExportDefinition(AttributedPartCreationInfo partCreationInfo, MemberInfo member, ExportAttribute exportAttribute, Type typeIdentityType, string contractName)
            : base(contractName, null)
        {
            _partCreationInfo = partCreationInfo ?? throw new ArgumentNullException(nameof(partCreationInfo));
            _member = member ?? throw new ArgumentNullException(nameof(member));
            _exportAttribute = exportAttribute ?? throw new ArgumentNullException(nameof(exportAttribute));
            _typeIdentityType = typeIdentityType;
        }

        public override IDictionary<string, object> Metadata
        {
            get
            {
                if (_metadata == null)
                {
                    _member.TryExportMetadataForMember(out IDictionary<string, object> metadata);

                    string typeIdentity = _exportAttribute.IsContractNameSameAsTypeIdentity() ?
                        ContractName :
                        _member.GetTypeIdentityFromExport(_typeIdentityType);

                    metadata.Add(CompositionConstants.ExportTypeIdentityMetadataName, typeIdentity);

                    IDictionary<string, object> partMetadata = _partCreationInfo.GetMetadata();
                    if (partMetadata != null && partMetadata.ContainsKey(CompositionConstants.PartCreationPolicyMetadataName))
                    {
                        metadata.Add(CompositionConstants.PartCreationPolicyMetadataName, partMetadata[CompositionConstants.PartCreationPolicyMetadataName]);
                    }

                    if ((_typeIdentityType != null) && (_member.MemberType != MemberTypes.Method) && _typeIdentityType.ContainsGenericParameters)
                    {
                        metadata.Add(CompositionConstants.GenericExportParametersOrderMetadataName, GenericServices.GetGenericParametersOrder(_typeIdentityType));
                    }

                    _metadata = metadata;
                }
                return _metadata;
            }
        }
    }

}
