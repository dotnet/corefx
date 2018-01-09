// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.Serialization;

namespace NetStandardLib
{
    [Serializable]
    public class SerializableType
    {
        public bool OnSerializingFired { get; private set; }
        
        public bool OnDeserializedFired { get; private set; }

        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
           OnSerializingFired = true;
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            OnDeserializedFired = true;
        }
    }
}
