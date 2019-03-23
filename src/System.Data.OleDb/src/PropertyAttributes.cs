// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Data {
    // We can't remove this enum, since we already shipped it in v1.0
    
    [
    Flags(),
    Obsolete("PropertyAttributes has been deprecated.  http://go.microsoft.com/fwlink/?linkid=14202"),
    EditorBrowsable(EditorBrowsableState.Never),
    ]
    public enum PropertyAttributes {
        NotSupported = 0,    // Indicates that the property is not supported by the provider. 

        Required     = 1,    // Indicates that the user must specify a value for this property before the data source is initialized. 

        Optional     = 2,    // Indicates that the user does not need to specify a value for this property before the data source is initialized. 

        Read         = 512,  // Indicates that the user can read the property. 

        Write        = 1024, // Indicates that the user can set the property.
    } 
}
