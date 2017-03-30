﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;

namespace System.Security.Permissions
{
    public sealed class KeyContainerPermissionAccessEntryCollection : ICollection
    {
        public KeyContainerPermissionAccessEntry this[int index] { get { return null; } }
        public int Count { get; }
        public int Add(KeyContainerPermissionAccessEntry accessEntry) { return 0; }
        public void Clear() { }
        public int IndexOf(KeyContainerPermissionAccessEntry accessEntry) { return 0; }
        public void Remove(KeyContainerPermissionAccessEntry accessEntry) { }
        public KeyContainerPermissionAccessEntryEnumerator GetEnumerator() { return null; }        
        public void CopyTo(KeyContainerPermissionAccessEntry[] array, int index) { }
        public void CopyTo(Array array, int index) { throw new NotImplementedException(); }
        IEnumerator IEnumerable.GetEnumerator() { throw new NotImplementedException(); }
        public bool IsSynchronized { get; }
        public object SyncRoot { get; }
    }
}
