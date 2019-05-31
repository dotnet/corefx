// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Xml.Serialization
{
    using System.Reflection;
    using System.Collections;
    using System.IO;
    using System.Xml.Schema;
    using System;
    using System.ComponentModel;

    public class SoapAttributeOverrides
    {
        private Hashtable _types = new Hashtable();

        public void Add(Type type, SoapAttributes attributes)
        {
            Add(type, string.Empty, attributes);
        }

        public void Add(Type type, string member, SoapAttributes attributes)
        {
            Hashtable members = (Hashtable)_types[type];
            if (members == null)
            {
                members = new Hashtable();
                _types.Add(type, members);
            }
            else if (members[member] != null)
            {
                throw new InvalidOperationException(SR.Format(SR.XmlMultipleAttributeOverrides, type.FullName, member));
            }
            members.Add(member, attributes);
        }

        public SoapAttributes this[Type type]
        {
            get
            {
                return this[type, string.Empty];
            }
        }

        public SoapAttributes this[Type type, string member]
        {
            get
            {
                Hashtable members = (Hashtable)_types[type];
                if (members == null) return null;
                return (SoapAttributes)members[member];
            }
        }
    }
}

