// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Xml;
using System.Text;
using System;

namespace System.Xml
{
    // Specifies the context that the XmLReader will use for xml fragment
    /// <summary>Provides all the context information required by the <see cref="T:System.Xml.XmlReader" /> to parse an XML fragment.</summary>
    public class XmlParserContext
    {
        private XmlNameTable _nt = null;
        private XmlNamespaceManager _nsMgr = null;
        private String _docTypeName = String.Empty;
        private String _pubId = String.Empty;
        private String _sysId = String.Empty;
        private String _internalSubset = String.Empty;
        private String _xmlLang = String.Empty;
        private XmlSpace _xmlSpace;
        private String _baseURI = String.Empty;
        private Encoding _encoding = null;

        public XmlParserContext(XmlNameTable nt, XmlNamespaceManager nsMgr, String xmlLang, XmlSpace xmlSpace)
        : this(nt, nsMgr, null, null, null, null, String.Empty, xmlLang, xmlSpace)
        {
            // Intentionally Empty
        }

        public XmlParserContext(XmlNameTable nt, XmlNamespaceManager nsMgr, String xmlLang, XmlSpace xmlSpace, Encoding enc)
        : this(nt, nsMgr, null, null, null, null, String.Empty, xmlLang, xmlSpace, enc)
        {
            // Intentionally Empty
        }

        public XmlParserContext(XmlNameTable nt, XmlNamespaceManager nsMgr, String docTypeName,
                  String pubId, String sysId, String internalSubset, String baseURI,
                  String xmlLang, XmlSpace xmlSpace)
        : this(nt, nsMgr, docTypeName, pubId, sysId, internalSubset, baseURI, xmlLang, xmlSpace, null)
        {
            // Intentionally Empty
        }

        public XmlParserContext(XmlNameTable nt, XmlNamespaceManager nsMgr, String docTypeName,
                          String pubId, String sysId, String internalSubset, String baseURI,
                          String xmlLang, XmlSpace xmlSpace, Encoding enc)
        {
            if (nsMgr != null)
            {
                if (nt == null)
                {
                    _nt = nsMgr.NameTable;
                }
                else
                {
                    if ((object)nt != (object)nsMgr.NameTable)
                    {
                        throw new XmlException(SR.Xml_NotSameNametable, string.Empty);
                    }
                    _nt = nt;
                }
            }
            else
            {
                _nt = nt;
            }

            _nsMgr = nsMgr;
            _docTypeName = (null == docTypeName ? String.Empty : docTypeName);
            _pubId = (null == pubId ? String.Empty : pubId);
            _sysId = (null == sysId ? String.Empty : sysId);
            _internalSubset = (null == internalSubset ? String.Empty : internalSubset);
            _baseURI = (null == baseURI ? String.Empty : baseURI);
            _xmlLang = (null == xmlLang ? String.Empty : xmlLang);
            _xmlSpace = xmlSpace;
            _encoding = enc;
        }

        /// <summary>Gets the <see cref="T:System.Xml.XmlNameTable" /> used to atomize strings. For more information on atomized strings, see <see cref="T:System.Xml.XmlNameTable" />.</summary>
        /// <returns>The XmlNameTable.</returns>
        public XmlNameTable NameTable
        {
            get
            {
                return _nt;
            }
            set
            {
                _nt = value;
            }
        }

        /// <summary>Gets or sets the <see cref="T:System.Xml.XmlNamespaceManager" />.</summary>
        /// <returns>The XmlNamespaceManager.</returns>
        public XmlNamespaceManager NamespaceManager
        {
            get
            {
                return _nsMgr;
            }
            set
            {
                _nsMgr = value;
            }
        }

        /// <summary>Gets or sets the name of the document type declaration.</summary>
        /// <returns>The name of the document type declaration.</returns>
        public String DocTypeName
        {
            get
            {
                return _docTypeName;
            }
            set
            {
                _docTypeName = (null == value ? String.Empty : value);
            }
        }

        /// <summary>Gets or sets the public identifier.</summary>
        /// <returns>The public identifier.</returns>
        public String PublicId
        {
            get
            {
                return _pubId;
            }
            set
            {
                _pubId = (null == value ? String.Empty : value);
            }
        }

        /// <summary>Gets or sets the system identifier.</summary>
        /// <returns>The system identifier.</returns>
        public String SystemId
        {
            get
            {
                return _sysId;
            }
            set
            {
                _sysId = (null == value ? String.Empty : value);
            }
        }

        /// <summary>Gets or sets the base URI.</summary>
        /// <returns>The base URI to use to resolve the DTD file.</returns>
        public String BaseURI
        {
            get
            {
                return _baseURI;
            }
            set
            {
                _baseURI = (null == value ? String.Empty : value);
            }
        }

        /// <summary>Gets or sets the internal DTD subset.</summary>
        /// <returns>The internal DTD subset. For example, this property returns everything between the square brackets &lt;!DOCTYPE doc [...]&gt;.</returns>
        public String InternalSubset
        {
            get
            {
                return _internalSubset;
            }
            set
            {
                _internalSubset = (null == value ? String.Empty : value);
            }
        }

        /// <summary>Gets or sets the current xml:lang scope.</summary>
        /// <returns>The current xml:lang scope. If there is no xml:lang in scope, String.Empty is returned.</returns>
        public String XmlLang
        {
            get
            {
                return _xmlLang;
            }
            set
            {
                _xmlLang = (null == value ? String.Empty : value);
            }
        }

        /// <summary>Gets or sets the current xml:space scope.</summary>
        /// <returns>An <see cref="T:System.Xml.XmlSpace" /> value indicating the xml:space scope.</returns>
        public XmlSpace XmlSpace
        {
            get
            {
                return _xmlSpace;
            }
            set
            {
                _xmlSpace = value;
            }
        }

        /// <summary>Gets or sets the encoding type.</summary>
        /// <returns>An <see cref="T:System.Text.Encoding" /> object indicating the encoding type.</returns>
        public Encoding Encoding
        {
            get
            {
                return _encoding;
            }
            set
            {
                _encoding = value;
            }
        }

        internal bool HasDtdInfo
        {
            get
            {
                return (_internalSubset != string.Empty || _pubId != string.Empty || _sysId != string.Empty);
            }
        }
    } // class XmlContext
} // namespace System.Xml



