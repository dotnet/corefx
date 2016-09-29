// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ComponentModel
{
    /// <summary>
    ///    <para>
    ///       Specifies identifiers that indicate the nature of
    ///       the refresh, for use in refreshing the design time view.
    ///    </para>
    /// </summary>
    public enum RefreshProperties
    {
        /// <summary>
        ///    <para>Indicates to use the no refresh mode.</para>
        /// </summary>
        None,
        /// <summary>
        ///    <para>Indicates to use the refresh all refresh mode.</para>
        /// </summary>
        All,
        /// <summary>
        ///    <para>Indicates to use the repaint refresh mode.</para>
        /// </summary>
        Repaint,
    }
}
