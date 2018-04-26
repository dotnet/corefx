﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace System.Reflection.Context.Delegation
{
    internal class DelegatingCustomAttributeData : CustomAttributeData
	{
        public DelegatingCustomAttributeData(CustomAttributeData attribute)
        {
            Contract.Requires(null != attribute);

            UnderlyingAttribute = attribute;
        }

        public CustomAttributeData UnderlyingAttribute { get; }

        public override ConstructorInfo Constructor
        {
            get { return UnderlyingAttribute.Constructor; }
        }

        public override IList<CustomAttributeTypedArgument> ConstructorArguments
        {
            get { return UnderlyingAttribute.ConstructorArguments; }
        }

        public override IList<CustomAttributeNamedArgument> NamedArguments
        {
            get { return UnderlyingAttribute.NamedArguments; }
        }

        public override string ToString()
        {
            return UnderlyingAttribute.ToString();
        }
    }
}
