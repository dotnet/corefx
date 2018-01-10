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
        [NonSerialized]
        private bool _onSerializingFired;

        [NonSerialized]
        private bool _onDeserializedFired;

        public bool OnSerializingFired
        {
            get => _onSerializingFired;
        }
        
        public bool OnDeserializedFired
        {
            get => _onDeserializedFired;
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
           _onSerializingFired = true;
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            _onDeserializedFired = true;
        }
    }
}
