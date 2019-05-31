// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Xml.Serialization
{
    using System.IO;
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Xml;

    public delegate void XmlAttributeEventHandler(object sender, XmlAttributeEventArgs e);

    public class XmlAttributeEventArgs : EventArgs
    {
        private object _o;
        private XmlAttribute _attr;
        private string _qnames;
        private int _lineNumber;
        private int _linePosition;


        internal XmlAttributeEventArgs(XmlAttribute attr, int lineNumber, int linePosition, object o, string qnames)
        {
            _attr = attr;
            _o = o;
            _qnames = qnames;
            _lineNumber = lineNumber;
            _linePosition = linePosition;
        }

        public object ObjectBeingDeserialized
        {
            get { return _o; }
        }

        public XmlAttribute Attr
        {
            get { return _attr; }
        }

        /// <summary>
        /// Gets the current line number.
        /// </summary>
        public int LineNumber
        {
            get { return _lineNumber; }
        }

        /// <summary>
        /// Gets the current line position.
        /// </summary>
        public int LinePosition
        {
            get { return _linePosition; }
        }

        /// <summary>
        /// List the qnames of attributes expected in the current context.
        /// </summary>
        public string ExpectedAttributes
        {
            get { return _qnames == null ? string.Empty : _qnames; }
        }
    }

    public delegate void XmlElementEventHandler(object sender, XmlElementEventArgs e);

    public class XmlElementEventArgs : EventArgs
    {
        private object _o;
        private XmlElement _elem;
        private string _qnames;
        private int _lineNumber;
        private int _linePosition;

        internal XmlElementEventArgs(XmlElement elem, int lineNumber, int linePosition, object o, string qnames)
        {
            _elem = elem;
            _o = o;
            _qnames = qnames;
            _lineNumber = lineNumber;
            _linePosition = linePosition;
        }

        public object ObjectBeingDeserialized
        {
            get { return _o; }
        }

        public XmlElement Element
        {
            get { return _elem; }
        }

        public int LineNumber
        {
            get { return _lineNumber; }
        }

        public int LinePosition
        {
            get { return _linePosition; }
        }

        /// <summary>
        /// List of qnames of elements expected in the current context.
        /// </summary>
        public string ExpectedElements
        {
            get { return _qnames == null ? string.Empty : _qnames; }
        }
    }

    public delegate void XmlNodeEventHandler(object sender, XmlNodeEventArgs e);


    public class XmlNodeEventArgs : EventArgs
    {
        private object _o;
        private XmlNode _xmlNode;
        private int _lineNumber;
        private int _linePosition;


        internal XmlNodeEventArgs(XmlNode xmlNode, int lineNumber, int linePosition, object o)
        {
            _o = o;
            _xmlNode = xmlNode;
            _lineNumber = lineNumber;
            _linePosition = linePosition;
        }

        public object ObjectBeingDeserialized
        {
            get { return _o; }
        }

        public XmlNodeType NodeType
        {
            get { return _xmlNode.NodeType; }
        }

        public string Name
        {
            get { return _xmlNode.Name; }
        }

        public string LocalName
        {
            get { return _xmlNode.LocalName; }
        }

        public string NamespaceURI
        {
            get { return _xmlNode.NamespaceURI; }
        }

        public string Text
        {
            get { return _xmlNode.Value; }
        }

        /// <summary>
        /// Gets the current line number.
        /// </summary>
        public int LineNumber
        {
            get { return _lineNumber; }
        }

        /// <summary>
        /// Gets the current line position.
        /// </summary>
        public int LinePosition
        {
            get { return _linePosition; }
        }
    }

    public delegate void UnreferencedObjectEventHandler(object sender, UnreferencedObjectEventArgs e);

    public class UnreferencedObjectEventArgs : EventArgs
    {
        private object _o;
        private string _id;

        public UnreferencedObjectEventArgs(object o, string id)
        {
            _o = o;
            _id = id;
        }

        public object UnreferencedObject
        {
            get { return _o; }
        }

        public string UnreferencedId
        {
            get { return _id; }
        }
    }
}
