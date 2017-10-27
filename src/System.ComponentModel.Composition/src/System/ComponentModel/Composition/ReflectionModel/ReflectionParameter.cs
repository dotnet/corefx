// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Globalization;
using System.Reflection;
using Microsoft.Internal;
using System.Threading;

namespace System.ComponentModel.Composition.ReflectionModel
{
    internal class ReflectionParameter : ReflectionItem
    {
        private readonly ParameterInfo _parameter;

        public ReflectionParameter(ParameterInfo parameter)
        {
            Assumes.NotNull(parameter);

            this._parameter = parameter;
        }

        public ParameterInfo UnderlyingParameter
        {
            get { return this._parameter; }
        }

        public override string Name
        {
            get { return this.UnderlyingParameter.Name; }
        }

        public override string GetDisplayName()
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                "{0} (Parameter=\"{1}\")",  // NOLOC
                this.UnderlyingParameter.Member.GetDisplayName(),
                this.UnderlyingParameter.Name);
        }

        public override Type ReturnType
        {
            get { return this.UnderlyingParameter.ParameterType; }
        }

        public override ReflectionItemType ItemType
        {
            get { return ReflectionItemType.Parameter; }
        }
    }
}
