// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Linq
{
    [Flags]
    internal enum Check
    {
        None = 0,
        Array = 1,
        ICollection = Array << 1,
        ICollectionOfT = ICollection << 1,
        IIListProvider = ICollectionOfT << 1,
        IListOfT = IIListProvider << 1,
        IPartition = IListOfT << 1,
        List = IPartition << 1,
        All = Array | ICollection | ICollectionOfT | IIListProvider | IListOfT | IPartition | List
    }
}
