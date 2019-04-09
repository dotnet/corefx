// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------

namespace System.Resources.Binary
{
    public partial class BinaryResourceReader : System.Resources.IResourceReader
    {
        public BinaryResourceReader(System.IO.Stream stream) { }
        public BinaryResourceReader(string fileName) { }
        protected virtual int Version { get { throw null; } }
        public void Close() { }
        protected virtual object DeserializeObject(System.IO.BinaryReader reader, Type type) { throw null; }
        public void Dispose() { }
        public System.Collections.IDictionaryEnumerator GetEnumerator() { throw null; }
        public void GetResourceData(string resourceName, out string resourceType, out byte[] resourceData) { throw null; }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { throw null; }
    }
}
