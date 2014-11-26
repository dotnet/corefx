// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// -----------------------------------------------------------------------
// Copyright © Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------

namespace Microsoft.Composition.Demos.Web.Http
{
    /// <summary>
    /// Well-known sharing boundary names. The dependency resolver uses
    /// all of these when handling a web request.
    /// </summary>
    public static class Boundaries
    {
        /// <summary>
        /// The boundary within which a current HTTP request is accessible.
        /// </summary>
        public const string HttpRequest = "HttpRequest";

        /// <summary>
        /// The boundary within which a consistent view of persistent data is available.
        /// </summary>
        public const string DataConsistency = "DataConsistency";

        /// <summary>
        /// The boundary within which a single user can be identified.
        /// </summary>
        public const string UserIdentity = "UserIdentity";
    }
}
