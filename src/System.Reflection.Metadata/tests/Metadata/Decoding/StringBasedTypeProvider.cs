// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;

namespace System.Reflection.Metadata.Decoding
{
    internal class StringBasedTypeProvider : ITypeNameParserTypeProvider<string>
    {
        public StringBasedTypeProvider()
        {
        }

        public string GetTypeFromName(AssemblyNameComponents? assemblyName, string declaringTypeFullName, ImmutableArray<string> nestedTypeNames)
        {

            string typeName = string.Empty;

            if (assemblyName != null)
            {
                typeName += "|" + assemblyName.Value.Name + "(name)";

                foreach (var component in assemblyName.Value.Components)
                {
                    typeName += component.Key + "(componentName)";
                    typeName += component.Value + "(componentValue)";
                }

                typeName += "|";
            }

            typeName += declaringTypeFullName + "(simple)";

            foreach (string nestedTypeName in nestedTypeNames)
            {
                typeName += "-" + nestedTypeName;
            }

            return typeName;
        }

        public string GetGenericInstantiation(string genericType, ImmutableArray<string> typeArguments)
        {
            return genericType + "<" + String.Join(",", typeArguments) + ">";
        }

        public string GetArrayType(string elementType, ArrayShape shape)
        {
            return elementType + "{" + shape.Rank + "}";
        }

        public string GetByReferenceType(string elementType)
        {
            return elementType + "(reference)";
        }

        public string GetSZArrayType(string elementType)
        {
            return elementType + "{}";
        }

        public string GetPointerType(string elementType)
        {
            return elementType + "(pointer)";
        }
    }
}
