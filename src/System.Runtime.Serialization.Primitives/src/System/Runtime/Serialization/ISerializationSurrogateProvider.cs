// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Runtime.Serialization
{
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
    public interface ISerializationSurrogateProvider
    {
        Type GetSurrogateType(Type type);
        object GetObjectToSerialize(object obj, Type targetType);
        object GetDeserializedObject(object obj, Type targetType);
    }
}
