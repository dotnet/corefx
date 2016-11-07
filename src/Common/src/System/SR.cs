// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Resources;
using System.Resources.Diagnostics;
using System.Runtime.CompilerServices;

namespace System
{
    internal partial class SR
    {
        private static ResourceManager s_resourceManager;

        private static ResourceManager ResourceManager
        {
            [ExcludeFromCodeCoverage]
            get
            {
                if (SR.s_resourceManager == null)
                {
                    SR.s_resourceManager = new ResourceManager(SR.ResourceType);
                }

                return SR.s_resourceManager;
            }
        }

        // This method is used to decide if we need to append the exception message parameters to the message when calling SR.Format. 
        // It is hardcoded to return false, but on some platforms an IL transform may rewrite it to instead return true.
        [ExcludeFromCodeCoverage]
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static bool UsingResourceKeys()
        {
            return false;
        }

        [ExcludeFromCodeCoverage]
        internal static string GetResourceString(string resourceKey, string defaultString)
        {
            string resourceString = null;
            try { resourceString = ResourceManager.GetString(resourceKey); }
            catch (MissingManifestResourceException) { }

            if (defaultString != null && resourceKey.Equals(resourceString, StringComparison.Ordinal))
            {
                return defaultString;
            }

            return resourceString;
        }

        [ExcludeFromCodeCoverage]
        internal static string Format(string resourceFormat, params object[] args)
        {
            if (args != null)
            {
                if (UsingResourceKeys())
                {
                    return resourceFormat + string.Join(", ", args);
                }

                return string.Format(resourceFormat, args);
            }

            return resourceFormat;
        }

        [ExcludeFromCodeCoverage]
        internal static string Format(string resourceFormat, object p1)
        {
            if (UsingResourceKeys())
            {
                return string.Join(", ", resourceFormat, p1);
            }

            return string.Format(resourceFormat, p1);
        }

        [ExcludeFromCodeCoverage]
        internal static string Format(string resourceFormat, object p1, object p2)
        {
            if (UsingResourceKeys())
            {
                return string.Join(", ", resourceFormat, p1, p2);
            }

            return string.Format(resourceFormat, p1, p2);
        }

        [ExcludeFromCodeCoverage]
        internal static string Format(string resourceFormat, object p1, object p2, object p3)
        {
            if (UsingResourceKeys())
            {
                return string.Join(", ", resourceFormat, p1, p2, p3);
            }

            return string.Format(resourceFormat, p1, p2, p3);
        }
    }
}

namespace System.Resources.Diagnostics
{
    [AttributeUsage(AttributeTargets.All)]
    [ExcludeFromCodeCoverage]
    internal sealed class ExcludeFromCodeCoverageAttribute : Attribute
    {
        // The code in the partial SR above is injected into all assemblies with resources, regardless of
        // whether those assemblies use this functionality.  As such, it should be excluded from code coverage.
        // The code coverage tools are configured to ignore any code attributed with an attribute
        // named ExcludeFromCodeCoverage, regardless of its namespace.  We have it in a specialized namespace
        // so as to avoid conflicts with other ExcludeFromCodeCoverageAttribute types used elsewhere in corefx.
        // It's applied to the individual members in SR rather than to SR as a whole because we still
        // want code coverage to include the individual resource keys; doing so helps to highlight whether
        // we're exercising all error paths, whether any resource strings are stale and can be removed, etc.
    }
}
