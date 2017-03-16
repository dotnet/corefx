// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Reflection.Metadata.Decoding
{
    public static class TypeNameParser
    {
        public static TType Parse<TType>(string typeName, ITypeNameParserTypeProvider<TType> typeProvider)
        {
            if (typeName == null)
                throw new ArgumentNullException("typeName");

            if (typeName.Length == 0)
                throw new ArgumentException(null, "typeName");

            if (typeProvider == null)
                throw new ArgumentNullException("typeName");

            return TypeNameParser<TType>.ParseType(typeName, typeProvider);
        }
    }
}
