// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebStyleLifetimeDemo.Extension
{
    public static class Boundaries
    {
        // Depends on the context of an executing
        // HTTP web request.
        public const string HttpRequest = "HttpRequest";

        // Depends on there being only one coherent view of
        // backing data.
        public const string DataConsistency = "DataConsistency";

        // Depends on there being a single identified user.
        public const string UserIdentity = "UserIdentity";
    }
}
