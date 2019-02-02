// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ComponentModel
{
    /// <summary>
    /// Provides data for the <see langword='PropertyChanged'/> event.
    /// </summary>
    public class PropertyChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref='System.ComponentModel.PropertyChangedEventArgs'/>
        /// class.
        /// </summary>
        public PropertyChangedEventArgs(string propertyName)
        {
            PropertyName = propertyName;
        }

        /// <summary>
        /// Indicates the name of the property that changed.
        /// </summary>
        public virtual string PropertyName { get; }
    }
}
