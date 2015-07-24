// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;

namespace System.Security.Cryptography.X509Certificates
{
    public sealed class X509ChainElementEnumerator : IEnumerator
    {
        private readonly X509ChainElementCollection _chainElements;
        private int _current;

        internal X509ChainElementEnumerator(X509ChainElementCollection chainElements)
        {
            _chainElements = chainElements;
            _current = -1;
        }

        public X509ChainElement Current
        {
            get
            {
                return _chainElements[_current];
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public bool MoveNext()
        {
            if (_current == _chainElements.Count - 1)
                return false;
            _current++;
            return true;
        }

        public void Reset()
        {
            _current = -1;
        }
    }
}
