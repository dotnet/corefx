// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Security.Permissions;

namespace System.ComponentModel
{
    /// <summary>
    ///     Represents the method that will handle the AddingNew event on a list,
    ///     and provide the new object to be added to the list.
    /// </summary>
    public delegate void AddingNewEventHandler(object sender, AddingNewEventArgs e);
}
