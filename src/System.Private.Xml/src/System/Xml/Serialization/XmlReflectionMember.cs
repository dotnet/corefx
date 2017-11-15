// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Xml.Serialization;


#if XMLSERIALIZERGENERATOR
namespace Microsoft.XmlSerializer.Generator
#else
namespace System.Xml.Serialization
#endif
{
    ///<internalonly/>
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
#if XMLSERIALIZERGENERATOR
    internal class XmlReflectionMember
#else
    public class XmlReflectionMember
#endif
    {
        private string _memberName;
        private Type _type;
        private XmlAttributes _xmlAttributes = new XmlAttributes();
        private SoapAttributes _soapAttributes = new SoapAttributes();
        private bool _isReturnValue;
        private bool _overrideIsNullable;

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public Type MemberType
        {
            get { return _type; }
            set { _type = value; }
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public XmlAttributes XmlAttributes
        {
            get { return _xmlAttributes; }
            set { _xmlAttributes = value; }
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public SoapAttributes SoapAttributes
        {
            get { return _soapAttributes; }
            set { _soapAttributes = value; }
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public string MemberName
        {
            get { return _memberName == null ? string.Empty : _memberName; }
            set { _memberName = value; }
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public bool IsReturnValue
        {
            get { return _isReturnValue; }
            set { _isReturnValue = value; }
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public bool OverrideIsNullable
        {
            get { return _overrideIsNullable; }
            set { _overrideIsNullable = value; }
        }
    }
}
