// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Reflection.Metadata
{
    public struct CustomModifier<TType>
    {
        private readonly TType _type;
        private readonly bool _isRequired;

        public CustomModifier(TType type, bool isRequired)
        {
            _type = type;
            _isRequired = isRequired;
        }

        public TType Type
        {
            get { return _type; }
        }

        public bool IsRequired
        {
            get { return _isRequired; }
        }
    }
}
