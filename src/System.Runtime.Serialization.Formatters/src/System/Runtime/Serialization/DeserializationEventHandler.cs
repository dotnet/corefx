// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Runtime.Serialization
{
    [Serializable]
    internal delegate void DeserializationEventHandler(object sender);

    [Serializable]
    internal delegate void SerializationEventHandler(StreamingContext context);
}
