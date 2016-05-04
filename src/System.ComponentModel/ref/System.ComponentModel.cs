// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------


namespace System
{
    /// <summary>
    /// Defines a mechanism for retrieving a service object; that is, an object that provides custom
    /// support to other objects.
    /// </summary>
    public partial interface IServiceProvider
    {
        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>
        /// A service object of type <paramref name="serviceType" />.-or- null if there is no service
        /// object of type <paramref name="serviceType" />.
        /// </returns>
        object GetService(System.Type serviceType);
    }
}
namespace System.ComponentModel
{
    /// <summary>
    /// Provides data for a cancelable event.
    /// </summary>
    public partial class CancelEventArgs : System.EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CancelEventArgs" /> class
        /// with the <see cref="Cancel" /> property set to false.
        /// </summary>
        public CancelEventArgs() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="CancelEventArgs" /> class
        /// with the <see cref="Cancel" /> property set to the
        /// given value.
        /// </summary>
        /// <param name="cancel">true to cancel the event; otherwise, false.</param>
        public CancelEventArgs(bool cancel) { }
        /// <summary>
        /// Gets or sets a value indicating whether the event should be canceled.
        /// </summary>
        /// <returns>
        /// true if the event should be canceled; otherwise, false.
        /// </returns>
        public bool Cancel { get { return default(bool); } set { } }
    }
    /// <summary>
    /// Defines the mechanism for querying the object for changes and resetting of the changed status.
    /// </summary>
    public partial interface IChangeTracking
    {
        /// <summary>
        /// Gets the object's changed status.
        /// </summary>
        /// <returns>
        /// true if the object’s content has changed since the last call to
        /// <see cref="AcceptChanges" />; otherwise, false.
        /// </returns>
        bool IsChanged { get; }
        /// <summary>
        /// Resets the object’s state to unchanged by accepting the modifications.
        /// </summary>
        void AcceptChanges();
    }
    /// <summary>
    /// Provides functionality to commit or rollback changes to an object that is used as a data source.
    /// </summary>
    public partial interface IEditableObject
    {
        /// <summary>
        /// Begins an edit on an object.
        /// </summary>
        void BeginEdit();
        /// <summary>
        /// Discards changes since the last <see cref="BeginEdit" />
        /// call.
        /// </summary>
        void CancelEdit();
        /// <summary>
        /// Pushes changes since the last <see cref="BeginEdit" />
        /// or <see cref="ComponentModel.IBindingList.AddNew" /> call into the underlying
        /// object.
        /// </summary>
        void EndEdit();
    }
    /// <summary>
    /// Provides support for rolling back the changes
    /// </summary>
    public partial interface IRevertibleChangeTracking : System.ComponentModel.IChangeTracking
    {
        /// <summary>
        /// Resets the object’s state to unchanged by rejecting the modifications.
        /// </summary>
        void RejectChanges();
    }
}
