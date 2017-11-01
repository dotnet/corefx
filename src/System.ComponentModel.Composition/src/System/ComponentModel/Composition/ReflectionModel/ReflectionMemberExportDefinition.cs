// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.ReflectionModel;
using System.Globalization;
using System.Reflection;
using Microsoft.Internal;
using Microsoft.Internal.Collections;
using System.Threading;

namespace System.ComponentModel.Composition.ReflectionModel
{
    internal class ReflectionMemberExportDefinition : ExportDefinition, ICompositionElement
    {
        private readonly LazyMemberInfo _member;
        private readonly ExportDefinition _exportDefinition;
        private readonly ICompositionElement _origin;
        private IDictionary<string, object> _metadata;

        public ReflectionMemberExportDefinition(LazyMemberInfo member, ExportDefinition exportDefinition, ICompositionElement origin)
        {
            Assumes.NotNull(exportDefinition);

            this._member = member;
            this._exportDefinition = exportDefinition;
            this._origin = origin;
        }

        public override string ContractName
        {
            get { return this._exportDefinition.ContractName; }
        }

        public LazyMemberInfo ExportingLazyMember
        {
            get { return this._member; }
        }

        public override IDictionary<string, object> Metadata
        {
            get
            {
                if (this._metadata == null)
                {
                    this._metadata = this._exportDefinition.Metadata.AsReadOnly();
                }
                return this._metadata;
            }
        }

        string ICompositionElement.DisplayName
        {
            get { return this.GetDisplayName(); }
        }

        ICompositionElement ICompositionElement.Origin
        {
            get { return this._origin; }
        }

        public override string ToString()
        {
            return this.GetDisplayName();
        }

        public int GetIndex()
        {
            return this.ExportingLazyMember.ToReflectionMember().UnderlyingMember.MetadataToken;
        }

        public ExportingMember ToExportingMember()
        {
            return new ExportingMember(this, this.ToReflectionMember());
        }

        private ReflectionMember ToReflectionMember()
        {
            return this.ExportingLazyMember.ToReflectionMember();
        }

        private string GetDisplayName()
        {
            return string.Format(CultureInfo.CurrentCulture,
                   "{0} (ContractName=\"{1}\")",    // NOLOC
                   this.ToReflectionMember().GetDisplayName(),
                   this.ContractName);
        }
    }
}
