// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Text;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.Hosting
{
    public partial class CompositionContainer
    {
        private class CompositionServiceShim : ICompositionService
        {
            CompositionContainer _innerContainer = null;

            public CompositionServiceShim(CompositionContainer innerContainer)
            {
                Assumes.NotNull(innerContainer);
                this._innerContainer = innerContainer;
            }

            void ICompositionService.SatisfyImportsOnce(ComposablePart part)
            {
                this._innerContainer.SatisfyImportsOnce(part);
            }
        }
    }
}
