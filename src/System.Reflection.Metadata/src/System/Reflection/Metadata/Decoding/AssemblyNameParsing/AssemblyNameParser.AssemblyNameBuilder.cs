// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Diagnostics;

namespace System.Reflection.Metadata.Decoding
{
    partial class AssemblyNameParser
    {
        private class AssemblyNameBuilder
        {
            private string _name;
            // TODO: Replace this with something a little more efficient
            private readonly Dictionary<string, string> _components = new Dictionary<string, string>(StringComparers.AssemblyNameComponent);

            public AssemblyNameBuilder()
            {
            }

            public bool IsSet(string componentName)
            {
                Debug.Assert(!String.IsNullOrEmpty(componentName));

                return _components.ContainsKey(componentName);
            }

            public void SetName(string name)
            {
                Debug.Assert(!String.IsNullOrEmpty(name));

                _name = name;
            }

            public void SetComponent(string componentName, string componentValue)
            {
                Debug.Assert(!String.IsNullOrEmpty(componentName));

                _components.Add(componentName, componentValue);                
            }

            public AssemblyNameComponents ToAssemblyName()
            {
                return new AssemblyNameComponents(_name, _components);
            }
        }
    }
}
