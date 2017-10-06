// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if XMLSERIALIZERGENERATOR
namespace Microsoft.XmlSerializer.Generator
#else
namespace System.Xml.Serialization
#endif
{
    using System.Reflection;
    using System.Collections;
    using System.IO;
    using System.Xml.Schema;
    using System;
    using System.Collections.Generic;
    using System.Xml.Extensions;
    using System.Xml;

    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
#if XMLSERIALIZERGENERATOR
    internal class XmlSerializerNamespaces
#else
    public class XmlSerializerNamespaces
#endif
    {
        private Dictionary<string, string> _namespaces = null;

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public XmlSerializerNamespaces()
        {
        }


        /// <internalonly/>
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public XmlSerializerNamespaces(XmlSerializerNamespaces namespaces)
        {
            _namespaces = new Dictionary<string, string>(namespaces.Namespaces);
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public XmlSerializerNamespaces(XmlQualifiedName[] namespaces)
        {
            for (int i = 0; i < namespaces.Length; i++)
            {
                XmlQualifiedName qname = namespaces[i];
                Add(qname.Name, qname.Namespace);
            }
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public void Add(string prefix, string ns)
        {
            // parameter value check
            if (prefix != null && prefix.Length > 0)
                XmlConvert.VerifyNCName(prefix);

            if (ns != null && ns.Length > 0)
                ExtensionMethods.ToUri(ns);
            AddInternal(prefix, ns);
        }

        internal void AddInternal(string prefix, string ns)
        {
            Namespaces[prefix] = ns;
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public XmlQualifiedName[] ToArray()
        {
            if (NamespaceList == null)
                return Array.Empty<XmlQualifiedName>();
            return (XmlQualifiedName[])NamespaceList.ToArray(typeof(XmlQualifiedName));
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public int Count
        {
            get { return Namespaces.Count; }
        }

        internal ArrayList NamespaceList
        {
            get
            {
                if (_namespaces == null || _namespaces.Count == 0)
                    return null;
                ArrayList namespaceList = new ArrayList();
                foreach (string key in Namespaces.Keys)
                {
                    namespaceList.Add(new XmlQualifiedName(key, (string)Namespaces[key]));
                }
                return namespaceList;
            }
        }

        internal Dictionary<string, string> Namespaces
        {
            get
            {
                if (_namespaces == null)
                    _namespaces = new Dictionary<string, string>();
                return _namespaces;
            }
            set { _namespaces = value; }
        }

        internal string LookupPrefix(string ns)
        {
            if (string.IsNullOrEmpty(ns))
                return null;
            if (_namespaces == null || _namespaces.Count == 0)
                return null;

            foreach (string prefix in _namespaces.Keys)
            {
                if (!string.IsNullOrEmpty(prefix) && _namespaces[prefix] == ns)
                {
                    return prefix;
                }
            }
            return null;
        }
    }
}

