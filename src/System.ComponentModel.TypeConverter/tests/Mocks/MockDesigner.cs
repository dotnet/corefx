﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.Design;

namespace System.ComponentModel.Tests
{
    internal class MockDesigner : IDesigner
    {
        private string StringProperty { get; set; }

        public bool StringPropertyHasBeenSet => StringProperty != null;

        private bool ShouldSerializeStringProperty()
        {
            ShouldSerializeStringPropertyCalled = true;
            return true;
        }

        public bool ShouldSerializeStringPropertyCalled { get; private set; }

        private void ResetStringProperty()
        {
            ResetStringPropertyCalled = true;
        }

        public bool ResetStringPropertyCalled { get; private set; }

        public IComponent Component => throw new NotImplementedException();

        public DesignerVerbCollection Verbs => throw new NotImplementedException();

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void DoDefaultAction()
        {
            throw new NotImplementedException();
        }

        public void Initialize(IComponent component)
        {
            throw new NotImplementedException();
        }
    }
}
