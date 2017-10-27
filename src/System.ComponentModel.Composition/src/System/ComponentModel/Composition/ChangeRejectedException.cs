// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Permissions;
using Microsoft.Internal;

namespace System.ComponentModel.Composition
{
    /// <summary>
    ///     The exception that is thrown when one or more recoverable errors occur during
    ///     composition which results in those changes being rejected.
    /// </summary>
    public class ChangeRejectedException : CompositionException
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ChangeRejectedException"/> class.
        /// </summary>
        public ChangeRejectedException()
            : this((string)null, (Exception)null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChangeRejectedException"/> class.
        /// </summary>
        public ChangeRejectedException(string message)
            : this(message, (Exception)null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChangeRejectedException"/> class.
        /// </summary>
        public ChangeRejectedException(string message, Exception innerException)
            : base(message, innerException, (IEnumerable<CompositionError>)null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChangeRejectedException"/> class.
        /// </summary>
        /// <param name="errors">List of errors that occured while applying the changes.</param>
        public ChangeRejectedException(IEnumerable<CompositionError> errors)
            : base((string)null, (Exception)null, errors)
        {
        }

        /// <summary>
        ///     Gets a message that describes the exception.
        /// </summary>
        /// <value>
        ///     A <see cref="String"/> containing a message that describes the 
        ///     <see cref="ChangeRejectedException"/>.
        /// </value>
        public override string Message
        {
            get
            {
                return string.Format(CultureInfo.CurrentCulture, 
                    SR.CompositionException_ChangesRejected,
                    base.Message);
            }
        }
    }
}
