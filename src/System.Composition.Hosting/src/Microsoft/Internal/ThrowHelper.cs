﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// -----------------------------------------------------------------------
// Copyright © Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------

using System;
using System.Composition;
using System.Composition.Hosting;
using System.Diagnostics;
using System.Composition.Hosting.Properties;

namespace Microsoft.Internal
{
    static class ThrowHelper
    {
        static Exception LogException(Exception e)
        {
            Debug.WriteLine(Resources.Diagnostic_ThrowingException, e.ToString());
            return e;
        }

        static public ArgumentNullException ArgumentNullException(string argument)
        {
            var e = new ArgumentNullException(argument);
            LogException(e);
            return e;
        }

        static public ArgumentException ArgumentException(string message)
        {
            var e = new ArgumentException(message);
            LogException(e);
            return e;
        }

        static public CompositionFailedException CardinalityMismatch_TooManyExports(string exportKey)
        {
            var e = new CompositionFailedException(string.Format(Resources.CardinalityMismatch_TooManyExports, exportKey));
            LogException(e);
            return e;
        }

        static public CompositionFailedException CompositionException(string message)
        {
            var e = new CompositionFailedException(message);
            LogException(e);
            return e;
        }

        internal static Exception NotImplemented_MetadataCycles()
        {
            var e = new NotImplementedException(Resources.NotImplemented_MetadataCycles);
            LogException(e);
            return e;
        }
    }
}