// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Xml;
using System.Diagnostics.CodeAnalysis;

namespace System.ServiceModel.Syndication
{
    // NOTE: This class implements Clone so if you add any members, please update the copy ctor
    public class SyndicationPerson : IExtensibleSyndicationObject
    {
        private ExtensibleSyndicationObject _extensions = new ExtensibleSyndicationObject();

        public SyndicationPerson() : this((string)null)
        {
        }

        public SyndicationPerson(string email) : this(email, null, null)
        {
        }

        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "2#", Justification = "The Uri represents a unique category and not a network location")]
        public SyndicationPerson(string email, string name, string uri)
        {
            Name = name;
            Email = email;
            Uri = uri;
        }

        protected SyndicationPerson(SyndicationPerson source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            Email = source.Email;
            Name = source.Name;
            Uri = source.Uri;
            _extensions = source._extensions.Clone();
        }

        public Dictionary<XmlQualifiedName, string> AttributeExtensions => _extensions.AttributeExtensions;

        public SyndicationElementExtensionCollection ElementExtensions => _extensions.ElementExtensions;

        public string Email { get; set; }

        public string Name { get; set; }

        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Scope = "property", Justification = "The Uri represents a unique category and not a network location")]
        public string Uri { get; set; }

        public virtual SyndicationPerson Clone() => new SyndicationPerson(this);

        protected internal virtual bool TryParseAttribute(string name, string ns, string value, string version)
        {
            return false;
        }

        protected internal virtual bool TryParseElement(XmlReader reader, string version)
        {
            return false;
        }

        protected internal virtual void WriteAttributeExtensions(XmlWriter writer, string version)
        {
            _extensions.WriteAttributeExtensions(writer);
        }

        protected internal virtual void WriteElementExtensions(XmlWriter writer, string version)
        {
            _extensions.WriteElementExtensions(writer);
        }

        internal void LoadElementExtensions(XmlReader readerOverUnparsedExtensions, int maxExtensionSize)
        {
            _extensions.LoadElementExtensions(readerOverUnparsedExtensions, maxExtensionSize);
        }

        internal void LoadElementExtensions(XmlBuffer buffer)
        {
            _extensions.LoadElementExtensions(buffer);
        }
    }
}
