// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Xunit
{
    public class ActiveIssueAttribute : Attribute
    {
        public ActiveIssueAttribute(int issueNumber, PlatformID platforms = default(PlatformID))
        {
        }
        public ActiveIssueAttribute(string issue, PlatformID platforms = default(PlatformID))
        {
        }
    }
}
