﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// -----------------------------------------------------------------------
// Copyright © Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Composition.Convention;

namespace System.Composition.TypedParts.Util
{
    class DirectAttributeContext : AttributedModelProvider
    {
        public override IEnumerable<Attribute> GetCustomAttributes(Type reflectedType, Reflection.MemberInfo member)
        {
            if (reflectedType == null) throw new ArgumentNullException("reflectedType");
            if (member == null) throw new ArgumentNullException("member");

            if (!(member is TypeInfo) && member.DeclaringType != reflectedType)
                return new Attribute[0];

            return member.GetCustomAttributes(false);
        }

        public override IEnumerable<Attribute> GetCustomAttributes(Type reflectedType, Reflection.ParameterInfo parameter)
        {
            if (reflectedType == null) throw new ArgumentNullException("reflectedType");
            if (parameter == null) throw new ArgumentNullException("parameter");
            return parameter.GetCustomAttributes(false);
        }
    }
}
