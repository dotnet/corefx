// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// -----------------------------------------------------------------------
// Copyright © Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------

using System.Diagnostics;
using System.Composition.Hosting.Properties;


namespace Microsoft.Internal
{
    static class Requires
    {
        [DebuggerStepThrough]
        static public void ArgumentNotNull<T>(T argument, string argumentName)
        {
            if (argument == null)
            {
                throw ThrowHelper.ArgumentNullException(argumentName);
            }
        }
    }
}