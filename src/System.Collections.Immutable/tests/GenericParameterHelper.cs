// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Collections.Immutable.Tests
{
    public class GenericParameterHelper
    {
        public GenericParameterHelper()
        {
            this.Data = new Random().Next();
        }

        public GenericParameterHelper(int data)
        {
            this.Data = data;
        }

        public int Data { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as GenericParameterHelper;
            if (other != null)
            {
                return this.Data == other.Data;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.Data;
        }
    }
}
