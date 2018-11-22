﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.Reflection.Context.Virtual
{
    internal class VirtualParameter : ParameterInfo
    {
        public VirtualParameter(MemberInfo member, Type parameterType, string name, int position)
        {
            Debug.Assert(position >= -1);

            ClassImpl = parameterType ?? throw new ArgumentNullException(nameof(parameterType));
            MemberImpl = member ?? throw new ArgumentNullException(nameof(member));
            NameImpl = name;
            PositionImpl = position;
        }

        internal static ParameterInfo[] CloneParameters(MemberInfo member, ParameterInfo[] parameters, bool skipLastParameter)
        {
            int length = parameters.Length;
            if (skipLastParameter)
            {
                length--;
            }

            ParameterInfo[] clonedParameters = new ParameterInfo[length];

            for (int i = 0; i < length; i++)
            {
                ParameterInfo parameter = parameters[i];
                clonedParameters[i] = new VirtualParameter(member, parameter.ParameterType, parameter.Name, parameter.Position);
            }

            return clonedParameters;
        }

        public override bool Equals(object obj)
        {
            // Do we need to compare Name and ParameterType?
            return obj is VirtualParameter other &&
                Member == other.Member &&
                Position == other.Position &&
                ParameterType == other.ParameterType;
        }

        public override int GetHashCode()
        {
            return Member.GetHashCode() ^
                Position.GetHashCode() ^
                ParameterType.GetHashCode();
        }
	}
}
