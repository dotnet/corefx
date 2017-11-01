// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.ComponentModel.Composition.AttributedModel;
using System.Reflection;
using Microsoft.Internal;
using System.Threading;

namespace System.ComponentModel.Composition.ReflectionModel
{
    internal class ReflectionType : ReflectionMember
    {
        private Type _type;

        public ReflectionType(Type type)
        {
            Assumes.NotNull(type);

            this._type = type;
        }

        public override MemberInfo UnderlyingMember
        {
            get { return this._type; }
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool RequiresInstance
        {
            get { return true; }
        }

        public override Type ReturnType
        {
            get { return this._type; }
        }

        public override ReflectionItemType ItemType
        {
            get { return ReflectionItemType.Type; }
        }

        public override object GetValue(object instance)
        {
            return instance;
        }
    }
}
