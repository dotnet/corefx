// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Xml
{
    // Specifies the state of the XmlReader.
    /// <summary>Specifies the state of the reader.</summary>
    public enum ReadState
    {
        // The Read method has not been called yet.
        /// <summary>The Read method has not been called.</summary>
        Initial = 0,

        // Reading is in progress.
        /// <summary>The Read method has been called. Additional methods may be called on the reader.</summary>
        Interactive = 1,

        // An error occurred that prevents the XmlReader from continuing.
        /// <summary>An error occurred that prevents the read operation from continuing.</summary>
        Error = 2,

        // The end of the stream has been reached successfully.
        /// <summary>The end of the file has been reached successfully.</summary>
        EndOfFile = 3,

        // The Close method has been called and the XmlReader is closed.
        /// <summary>The <see cref="M:System.Xml.XmlReader.Close" /> method has been called.</summary>
        Closed = 4
    }
}
