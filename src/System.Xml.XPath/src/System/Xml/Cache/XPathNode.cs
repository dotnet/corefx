﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Xml.XPath;

namespace MS.Internal.Xml.Cache
{
    /// <summary>
    /// Implementation of a Node in the XPath/XQuery data model.
    /// 1.  All nodes are stored in variable-size pages (max 65536 nodes/page) of XPathNode structures.
    /// 2.  Pages are sequentially numbered.  Nodes are allocated in strict document order.
    /// 3.  Node references take the form of a (page, index) pair.
    /// 4.  Each node explicitly stores a parent and a sibling reference.
    /// 5.  If a node has one or more attributes and/or non-collapsed content children, then its first
    ///     child is stored in the next slot.  If the node is in the last slot of a page, then its first
    ///     child is stored in the first slot of the next page.
    /// 6.  Attributes are linked together at the start of the child list.
    /// 7.  Namespaces are allocated in totally separate pages.  Elements are associated with
    ///     declared namespaces via a hashtable map in the document.
    /// 8.  Name parts are always non-null (string.Empty for nodes without names)
    /// 9.  XPathNodeInfoAtom contains all information that is common to many nodes in a
    ///     document, and therefore is atomized to save space.  This includes the document, the name,
    ///     the child, sibling, parent, and value pages, and the schema type.
    /// 10. The node structure is 20 bytes in length.  Out-of-line overhead is typically 2-4 bytes per node.
    /// </summary>
    internal struct XPathNode
    {
        private XPathNodeInfoAtom _info;                           // Atomized node information
        private ushort _idxSibling;                     // Page index of sibling node
        private ushort _idxParent;                      // Page index of parent node
        private ushort _idxSimilar;                     // Page index of next node in document order that has local name with same hashcode
        private ushort _posOffset;                      // Line position offset of node (added to LinePositionBase)
        private uint _props;                          // Node properties (broken down into bits below)
        private string _value;                          // String value of node

        private const uint NodeTypeMask = 0xF;
        private const uint HasAttributeBit = 0x10;
        private const uint HasContentChildBit = 0x20;
        private const uint HasElementChildBit = 0x40;
        private const uint HasCollapsedTextBit = 0x80;
        private const uint AllowShortcutTagBit = 0x100;    // True if this is an element that allows shortcut tag syntax
        private const uint HasNmspDeclsBit = 0x200;        // True if this is an element with namespace declarations declared on it

        private const uint LineNumberMask = 0x00FFFC00;    // 14 bits for line number offset (0 - 16K)
        private const int LineNumberShift = 10;
        private const int CollapsedPositionShift = 24;    // 8 bits for collapsed text position offset (0 - 256)

#if DEBUG
        public const int MaxLineNumberOffset = 0x20;
        public const int MaxLinePositionOffset = 0x20;
        public const int MaxCollapsedPositionOffset = 0x10;
#else
        public const int MaxLineNumberOffset = 0x3FFF;
        public const int MaxLinePositionOffset = 0xFFFF;
        public const int MaxCollapsedPositionOffset = 0xFF;
#endif

        /// <summary>
        /// Returns the type of this node
        /// </summary>
        public XPathNodeType NodeType
        {
            get { return (XPathNodeType)(this._props & NodeTypeMask); }
        }

        /// <summary>
        /// Returns the namespace prefix of this node.  If this node has no prefix, then the empty string
        /// will be returned (never null).
        /// </summary>
        public string Prefix
        {
            get { return this._info.Prefix; }
        }

        /// <summary>
        /// Returns the local name of this node.  If this node has no name, then the empty string
        /// will be returned (never null).
        /// </summary>
        public string LocalName
        {
            get { return this._info.LocalName; }
        }

        /// <summary>
        /// Returns the name of this node.  If this node has no name, then the empty string
        /// will be returned (never null).
        /// </summary>
        public string Name
        {
            get
            {
                if (Prefix.Length == 0)
                {
                    return LocalName;
                }
                else
                {
                    return string.Concat(Prefix, ":", LocalName);
                }
            }
        }

        /// <summary>
        /// Returns the namespace part of this node's name.  If this node has no name, then the empty string
        /// will be returned (never null).
        /// </summary>
        public string NamespaceUri
        {
            get { return this._info.NamespaceUri; }
        }

        /// <summary>
        /// Returns this node's document.
        /// </summary>
        public XPathDocument Document
        {
            get { return this._info.Document; }
        }

        /// <summary>
        /// Returns this node's base Uri.  This is string.Empty for all node kinds except Element, Root, and PI.
        /// </summary>
        public string BaseUri
        {
            get { return this._info.BaseUri; }
        }

        /// <summary>
        /// Returns this node's source line number.
        /// </summary>
        public int LineNumber
        {
            get { return this._info.LineNumberBase + (int)((this._props & LineNumberMask) >> LineNumberShift); }
        }

        /// <summary>
        /// Return this node's source line position.
        /// </summary>
        public int LinePosition
        {
            get { return this._info.LinePositionBase + (int)this._posOffset; }
        }

        /// <summary>
        /// If this node is an element with collapsed text, then return the source line position of the node (the
        /// source line number is the same as LineNumber).
        /// </summary>
        public int CollapsedLinePosition
        {
            get
            {
                Debug.Assert(HasCollapsedText, "Do not call CollapsedLinePosition unless HasCollapsedText is true.");
                return LinePosition + (int)(this._props >> CollapsedPositionShift);
            }
        }

        /// <summary>
        /// Returns information about the node page.  Only the 0th node on each page has this property defined.
        /// </summary>
        public XPathNodePageInfo PageInfo
        {
            get { return this._info.PageInfo; }
        }

        /// <summary>
        /// Returns the root node of the current document.  This always succeeds.
        /// </summary>
        public int GetRoot(out XPathNode[] pageNode)
        {
            return this._info.Document.GetRootNode(out pageNode);
        }

        /// <summary>
        /// Returns the parent of this node.  If this node has no parent, then 0 is returned.
        /// </summary>
        public int GetParent(out XPathNode[] pageNode)
        {
            pageNode = this._info.ParentPage;
            return this._idxParent;
        }

        /// <summary>
        /// Returns the next sibling of this node.  If this node has no next sibling, then 0 is returned.
        /// </summary>
        public int GetSibling(out XPathNode[] pageNode)
        {
            pageNode = this._info.SiblingPage;
            return this._idxSibling;
        }

        /// <summary>
        /// Returns the next element in document order that has the same local name hashcode as this element.
        /// If there are no similar elements, then 0 is returned.
        /// </summary>
        public int GetSimilarElement(out XPathNode[] pageNode)
        {
            pageNode = this._info.SimilarElementPage;
            return this._idxSimilar;
        }

        /// <summary>
        /// Returns true if this node's name matches the specified localName and namespaceName.  Assume
        /// that localName has been atomized, but namespaceName has not.
        /// </summary>
        public bool NameMatch(string localName, string namespaceName)
        {
            Debug.Assert(localName == null || (object)Document.NameTable.Get(localName) == (object)localName, "localName must be atomized.");

            return (object)this._info.LocalName == (object)localName &&
                   this._info.NamespaceUri == namespaceName;
        }

        /// <summary>
        /// Returns true if this is an Element node with a name that matches the specified localName and
        /// namespaceName.  Assume that localName has been atomized, but namespaceName has not.
        /// </summary>
        public bool ElementMatch(string localName, string namespaceName)
        {
            Debug.Assert(localName == null || (object)Document.NameTable.Get(localName) == (object)localName, "localName must be atomized.");

            return NodeType == XPathNodeType.Element &&
                   (object)this._info.LocalName == (object)localName &&
                   this._info.NamespaceUri == namespaceName;
        }

        /// <summary>
        /// Return true if this node is an xmlns:xml node.
        /// </summary>
        public bool IsXmlNamespaceNode
        {
            get
            {
                string localName = this._info.LocalName;
                return NodeType == XPathNodeType.Namespace && localName.Length == 3 && localName == "xml";
            }
        }

        /// <summary>
        /// Returns true if this node has a sibling.
        /// </summary>
        public bool HasSibling
        {
            get { return this._idxSibling != 0; }
        }

        /// <summary>
        /// Returns true if this node has a collapsed text node as its only content-typed child.
        /// </summary>
        public bool HasCollapsedText
        {
            get { return (this._props & HasCollapsedTextBit) != 0; }
        }

        /// <summary>
        /// Returns true if this node has at least one attribute.
        /// </summary>
        public bool HasAttribute
        {
            get { return (this._props & HasAttributeBit) != 0; }
        }

        /// <summary>
        /// Returns true if this node has at least one content-typed child (attributes and namespaces
        /// don't count).
        /// </summary>
        public bool HasContentChild
        {
            get { return (this._props & HasContentChildBit) != 0; }
        }

        /// <summary>
        /// Returns true if this node has at least one element child.
        /// </summary>
        public bool HasElementChild
        {
            get { return (this._props & HasElementChildBit) != 0; }
        }

        /// <summary>
        /// Returns true if this is an attribute or namespace node.
        /// </summary>
        public bool IsAttrNmsp
        {
            get
            {
                XPathNodeType xptyp = NodeType;
                return xptyp == XPathNodeType.Attribute || xptyp == XPathNodeType.Namespace;
            }
        }

        /// <summary>
        /// Returns true if this is a text or whitespace node.
        /// </summary>
        public bool IsText
        {
            get { return XPathNavigator.IsText(NodeType); }
        }

        /// <summary>
        /// Returns true if this node has local namespace declarations associated with it.  Since all
        /// namespace declarations are stored out-of-line in the owner Document, this property
        /// can be consulted in order to avoid a lookup in the common case where this node has no
        /// local namespace declarations.
        /// </summary>
        public bool HasNamespaceDecls
        {
            get { return (this._props & HasNmspDeclsBit) != 0; }
            set
            {
                if (value) this._props |= HasNmspDeclsBit;
                else unchecked { this._props &= (byte)~((uint)HasNmspDeclsBit); }
            }
        }

        /// <summary>
        /// Returns true if this node is an empty element that allows shortcut tag syntax.
        /// </summary>
        public bool AllowShortcutTag
        {
            get { return (this._props & AllowShortcutTagBit) != 0; }
        }

        /// <summary>
        /// Cached hashcode computed over the local name of this element.
        /// </summary>
        public int LocalNameHashCode
        {
            get { return this._info.LocalNameHashCode; }
        }

        /// <summary>
        /// Return the precomputed String value of this node (null if no value exists, i.e. document node, element node with complex content, etc).
        /// </summary>
        public string Value
        {
            get { return this._value; }
        }


        //-----------------------------------------------
        // Node construction
        //-----------------------------------------------

        /// <summary>
        /// Constructs the 0th XPathNode in each page, which contains only page information.
        /// </summary>
        public void Create(XPathNodePageInfo pageInfo)
        {
            this._info = new XPathNodeInfoAtom(pageInfo);
        }

        /// <summary>
        /// Constructs a XPathNode.  Later, the idxSibling and value fields may be fixed up.
        /// </summary>
        public void Create(XPathNodeInfoAtom info, XPathNodeType xptyp, int idxParent)
        {
            Debug.Assert(info != null && idxParent <= UInt16.MaxValue);
            this._info = info;
            this._props = (uint)xptyp;
            this._idxParent = (ushort)idxParent;
        }

        /// <summary>
        /// Set this node's line number information.
        /// </summary>
        public void SetLineInfoOffsets(int lineNumOffset, int linePosOffset)
        {
            Debug.Assert(lineNumOffset >= 0 && lineNumOffset <= MaxLineNumberOffset, "Line number offset too large or small: " + lineNumOffset);
            Debug.Assert(linePosOffset >= 0 && linePosOffset <= MaxLinePositionOffset, "Line position offset too large or small: " + linePosOffset);
            this._props |= ((uint)lineNumOffset << LineNumberShift);
            this._posOffset = (ushort)linePosOffset;
        }

        /// <summary>
        /// Set the position offset of this element's collapsed text.
        /// </summary>
        public void SetCollapsedLineInfoOffset(int posOffset)
        {
            Debug.Assert(posOffset >= 0 && posOffset <= MaxCollapsedPositionOffset, "Collapsed text line position offset too large or small: " + posOffset);
            this._props |= ((uint)posOffset << CollapsedPositionShift);
        }

        /// <summary>
        /// Set this node's value.
        /// </summary>
        public void SetValue(string value)
        {
            this._value = value;
        }

        /// <summary>
        /// Create an empty element value.
        /// </summary>
        public void SetEmptyValue(bool allowShortcutTag)
        {
            Debug.Assert(NodeType == XPathNodeType.Element);
            this._value = string.Empty;
            if (allowShortcutTag)
                this._props |= AllowShortcutTagBit;
        }

        /// <summary>
        /// Create a collapsed text node on this element having the specified value.
        /// </summary>
        public void SetCollapsedValue(string value)
        {
            Debug.Assert(NodeType == XPathNodeType.Element);
            this._value = value;
            this._props |= HasContentChildBit | HasCollapsedTextBit;
        }

        /// <summary>
        /// This method is called when a new child is appended to this node's list of attributes and children.
        /// The type of the new child is used to determine how various parent properties should be set.
        /// </summary>
        public void SetParentProperties(XPathNodeType xptyp)
        {
            if (xptyp == XPathNodeType.Attribute)
            {
                this._props |= HasAttributeBit;
            }
            else
            {
                this._props |= HasContentChildBit;
                if (xptyp == XPathNodeType.Element)
                    this._props |= HasElementChildBit;
            }
        }

        /// <summary>
        /// Link this node to its next sibling.  If "pageSibling" is different than the one stored in the InfoAtom, re-atomize.
        /// </summary>
        public void SetSibling(XPathNodeInfoTable infoTable, XPathNode[] pageSibling, int idxSibling)
        {
            Debug.Assert(pageSibling != null && idxSibling != 0 && idxSibling <= UInt16.MaxValue, "Bad argument");
            Debug.Assert(this._idxSibling == 0, "SetSibling should not be called more than once.");
            this._idxSibling = (ushort)idxSibling;

            if (pageSibling != this._info.SiblingPage)
            {
                // Re-atomize the InfoAtom
                this._info = infoTable.Create(this._info.LocalName, this._info.NamespaceUri, this._info.Prefix, this._info.BaseUri,
                                             this._info.ParentPage, pageSibling, this._info.SimilarElementPage,
                                             this._info.Document, this._info.LineNumberBase, this._info.LinePositionBase);
            }
        }

        /// <summary>
        /// Link this element to the next element in document order that shares a local name having the same hash code.
        /// If "pageSimilar" is different than the one stored in the InfoAtom, re-atomize.
        /// </summary>
        public void SetSimilarElement(XPathNodeInfoTable infoTable, XPathNode[] pageSimilar, int idxSimilar)
        {
            Debug.Assert(pageSimilar != null && idxSimilar != 0 && idxSimilar <= UInt16.MaxValue, "Bad argument");
            Debug.Assert(this._idxSimilar == 0, "SetSimilarElement should not be called more than once.");
            this._idxSimilar = (ushort)idxSimilar;

            if (pageSimilar != this._info.SimilarElementPage)
            {
                // Re-atomize the InfoAtom
                this._info = infoTable.Create(this._info.LocalName, this._info.NamespaceUri, this._info.Prefix, this._info.BaseUri,
                                             this._info.ParentPage, this._info.SiblingPage, pageSimilar,
                                             this._info.Document, this._info.LineNumberBase, this._info.LinePositionBase);
            }
        }
    }


    /// <summary>
    /// A reference to a XPathNode is composed of two values: the page on which the node is located, and the node's
    /// index in the page.
    /// </summary>
    internal struct XPathNodeRef
    {
        private XPathNode[] _page;
        private int _idx;

        public XPathNodeRef(XPathNode[] page, int idx)
        {
            this._page = page;
            this._idx = idx;
        }

        public XPathNode[] Page
        {
            get { return this._page; }
        }

        public int Index
        {
            get { return this._idx; }
        }

        public override int GetHashCode()
        {
            return XPathNodeHelper.GetLocation(this._page, this._idx);
        }
    }
}
