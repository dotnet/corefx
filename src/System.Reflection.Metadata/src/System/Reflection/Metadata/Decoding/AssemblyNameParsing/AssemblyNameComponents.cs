// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace System.Reflection.Metadata.Decoding
{
    public struct AssemblyNameComponents
    {
        private readonly string _name;
        private readonly IReadOnlyDictionary<string, string> _components;

        public AssemblyNameComponents(string name, IReadOnlyDictionary<string, string> components)
        {
            _name = name;
            _components = components;
        }

        public string Name
        {
            get { return _name; }
        }

        public IReadOnlyDictionary<string, string> Components
        {
            get { return _components; }
        }
    }
}
