// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Reflection;

namespace System.ComponentModel.Composition.ReflectionModel
{
    internal class ReflectionParameter : ReflectionItem
    {
        public ReflectionParameter(ParameterInfo parameter)
        {
            UnderlyingParameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
        }

        public ParameterInfo UnderlyingParameter { get; }

        public override string Name => UnderlyingParameter.Name;

        public override string GetDisplayName()
        {
            return string.Format(CultureInfo.CurrentCulture,
                "{0} (Parameter=\"{1}\")", // NOLOC
                UnderlyingParameter.Member.GetDisplayName(),
                UnderlyingParameter.Name);
        }

        public override Type ReturnType => UnderlyingParameter.ParameterType;

        public override ReflectionItemType ItemType => ReflectionItemType.Parameter;
    }
}
