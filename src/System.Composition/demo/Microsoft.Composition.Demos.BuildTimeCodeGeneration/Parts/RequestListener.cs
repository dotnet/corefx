// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildTimeCodeGeneration.Parts
{
    // [Export]
    //
    public class RequestListener
    {
        private Lazy<ConsoleLog> _log;

        // [ImportingConstructor]
        //
        public RequestListener(Lazy<ConsoleLog> log)
        {
            _log = log;
        }

        public void HandleRequest()
        {
            _log.Value.Write("Request incoming...");
        }
    }
}
