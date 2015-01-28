// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Reflection.Metadata
{
    public struct CustomModifier<TType>
    {
        private readonly TType type;
        private readonly bool isRequired;

        public CustomModifier(TType type, bool isRequired)
        {
            this.type = type;
            this.isRequired = isRequired;
        }

        public TType Type
        {
            get { return type; }
        }

        public bool IsRequired
        {
            get { return isRequired; }
        }
    }
}
