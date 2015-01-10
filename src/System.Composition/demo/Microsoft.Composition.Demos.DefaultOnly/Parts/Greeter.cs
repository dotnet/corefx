// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Composition.Demos.DefaultOnly.Parts
{
    [Export]
    public class Greeter
    {
        private IMessage _message;
        private ILogger _logger;

        [ImportingConstructor]
        public Greeter(IMessage message, ILogger logger)
        {
            _message = message;
            _logger = logger;
        }

        public void Greet()
        {
            _logger.Write(_message.Current);
        }
    }
}
