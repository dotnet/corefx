// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Xml.Schema;
using System.Reflection;


namespace System.Xml.Serialization
{
    /// <include file='doc\XmlChoiceIdentifierAttribute.uex' path='docs/doc[@for="XmlChoiceIdentifierAttribute"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = false)]
    public class XmlChoiceIdentifierAttribute : System.Attribute
    {
        private string _name;
        private MemberInfo _memberInfo;

        /// <include file='doc\XmlChoiceIdentifierAttribute.uex' path='docs/doc[@for="XmlChoiceIdentifierAttribute.XmlChoiceIdentifierAttribute"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public XmlChoiceIdentifierAttribute()
        {
        }

        /// <include file='doc\XmlChoiceIdentifierAttribute.uex' path='docs/doc[@for="XmlChoiceIdentifierAttribute.XmlChoiceIdentifierAttribute1"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public XmlChoiceIdentifierAttribute(string name)
        {
            _name = name;
        }

        /// <include file='doc\XmlChoiceIdentifierAttribute.uex' path='docs/doc[@for="XmlChoiceIdentifierAttribute.Name"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public string MemberName
        {
            get { return _name == null ? string.Empty : _name; }
            set { _name = value; }
        }

        internal MemberInfo MemberInfo
        {
            get { return _memberInfo; }
            set { _memberInfo = value; }
        }
    }
}
