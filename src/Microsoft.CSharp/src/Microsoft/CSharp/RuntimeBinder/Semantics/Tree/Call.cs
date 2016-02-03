// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.CSharp.RuntimeBinder.Semantics
{
    internal class EXPRCALL : EXPR
    {
        private EXPR _OptionalArguments;
        public EXPR GetOptionalArguments() { return _OptionalArguments; }
        public void SetOptionalArguments(EXPR value) { _OptionalArguments = value; }

        private EXPRMEMGRP _MemberGroup;
        public EXPRMEMGRP GetMemberGroup() { return _MemberGroup; }
        public void SetMemberGroup(EXPRMEMGRP value) { _MemberGroup = value; }

        public MethWithInst mwi;

        public PREDEFMETH PredefinedMethod;

        public NullableCallLiftKind nubLiftKind;
        public EXPR pConversions;
        public EXPR castOfNonLiftedResultToLiftedType;
    }
}
