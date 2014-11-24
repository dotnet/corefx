// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using System.Xml.XPath;

namespace MS.Internal.Xml.Cache
{
    /// <summary>
    /// Although the XPath data model does not differentiate between text and whitespace, Managed Xml 1.0
    /// does.  Therefore, when building from an XmlReader, we must preserve these designations in order
    /// to remain backwards-compatible.
    /// </summary>
    internal enum TextBlockType
    {
        None = 0,
        Text = XPathNodeType.Text,
        SignificantWhitespace = XPathNodeType.SignificantWhitespace,
        Whitespace = XPathNodeType.Whitespace,
    };


    /// <summary>
    /// Implementation of XmlRawWriter that builds nodes in an XPathDocument.
    /// </summary>
    internal sealed class XPathDocumentBuilder : XmlRawWriter
    {
        private NodePageFactory _nodePageFact;   // Creates non-namespace node pages
        private NodePageFactory _nmspPageFact;   // Creates namespace node pages
        private TextBlockBuilder _textBldr;      // Concatenates adjacent text blocks

        private Stack<XPathNodeRef> _stkNmsp;    // In-scope namespaces
        private XPathNodeInfoTable _infoTable;   // Atomization table for shared node information
        private XPathDocument _doc;              // Currently building document
        private IXmlLineInfo _lineInfo;          // Line information provider
        private XmlNameTable _nameTable;         // Atomization table for all names in the document
        private bool _atomizeNames;              // True if all names should be atomized (false if they are pre-atomized)

        private XPathNode[] _pageNmsp;           // Page of last in-scope namespace node
        private int _idxNmsp;                    // Page index of last in-scope namespace node
        private XPathNode[] _pageParent;         // Page of last parent-type node (Element or Root)
        private int _idxParent;                  // Page index of last parent-type node (Element or Root)
        private XPathNode[] _pageSibling;        // Page of previous sibling node (may be null if no previous sibling)
        private int _idxSibling;                 // Page index of previous sibling node
        private int _lineNumBase;                // Line number from which offsets are computed
        private int _linePosBase;                // Line position from which offsets are computed

        private XPathNodeRef[] _elemNameIndex;   // Elements with the same name are linked together so that they can be searched quickly

        private const int ElementIndexSize = 64;

        /// <summary>
        /// Create a new XPathDocumentBuilder which creates nodes in "doc".
        /// </summary>
        public XPathDocumentBuilder(XPathDocument doc, IXmlLineInfo lineInfo, string baseUri, XPathDocument.LoadFlags flags)
        {
            // Allocate the initial node (for non-namespaces) page, and the initial namespace page
            this._nodePageFact.Init(256);
            this._nmspPageFact.Init(16);

            this._stkNmsp = new Stack<XPathNodeRef>();

            Initialize(doc, lineInfo, baseUri, flags);
        }

        /// <summary>
        /// Start construction of a new document.  This must be called before any other methods are called.
        /// It may also be called after Close(), in order to build further documents.
        /// </summary>
        public void Initialize(XPathDocument doc, IXmlLineInfo lineInfo, string baseUri, XPathDocument.LoadFlags flags)
        {
            XPathNode[] page;
            int idx;

            this._doc = doc;
            this._nameTable = doc.NameTable;
            this._atomizeNames = (flags & XPathDocument.LoadFlags.AtomizeNames) != 0;
            this._idxParent = this._idxSibling = 0;
            this._elemNameIndex = new XPathNodeRef[ElementIndexSize];

            // Prepare line number information
            this._textBldr.Initialize(lineInfo);
            this._lineInfo = lineInfo;
            this._lineNumBase = 0;
            this._linePosBase = 0;

            // Allocate the atomization table
            this._infoTable = new XPathNodeInfoTable();

            // Allocate singleton collapsed text node
            idx = NewNode(out page, XPathNodeType.Text, string.Empty, string.Empty, string.Empty, string.Empty);
            this._doc.SetCollapsedTextNode(page, idx);

            // Allocate xmlns:xml namespace node
            this._idxNmsp = NewNamespaceNode(out this._pageNmsp, this._nameTable.Add("xml"), this._nameTable.Add(XmlConst.ReservedNsXml), null, 0);
            this._doc.SetXmlNamespaceNode(this._pageNmsp, this._idxNmsp);

            if ((flags & XPathDocument.LoadFlags.Fragment) == 0)
            {
                // This tree has a document root node
                this._idxParent = NewNode(out this._pageParent, XPathNodeType.Root, string.Empty, string.Empty, string.Empty, baseUri);
                this._doc.SetRootNode(this._pageParent, this._idxParent);
            }
            else
            {
                // This tree is an XQuery fragment (no document root node), so root will be next node in the current page
                this._doc.SetRootNode(this._nodePageFact.NextNodePage, this._nodePageFact.NextNodeIndex);
            }
        }


        //-----------------------------------------------
        // XmlWriter interface
        //-----------------------------------------------

        /// <summary>
        /// XPathDocument ignores the DocType information.
        /// </summary>
        public override void WriteDocType(string name, string pubid, string sysid, string subset)
        {
        }

        /// <summary>
        /// Shortcut for calling WriteStartElement with elemType == null.
        /// </summary>
        public override void WriteStartElement(string prefix, string localName, string ns)
        {
            this.WriteStartElement(prefix, localName, ns, string.Empty);
        }

        /// <summary>
        /// Build an element node and attach it to its parent, if one exists.  Make the element the new parent node.
        /// </summary>
        public void WriteStartElement(string prefix, string localName, string ns, string baseUri)
        {
            int hash;
            Debug.Assert(prefix != null && localName != null && ns != null && localName.Length != 0 && baseUri != null);

            if (this._atomizeNames)
            {
                prefix = this._nameTable.Add(prefix);
                localName = this._nameTable.Add(localName);
                ns = this._nameTable.Add(ns);
            }

            AddSibling(XPathNodeType.Element, localName, ns, prefix, baseUri);
            this._pageParent = this._pageSibling;
            this._idxParent = this._idxSibling;
            this._idxSibling = 0;

            // Link elements with the same name together
            hash = (this._pageParent[this._idxParent].LocalNameHashCode & (ElementIndexSize - 1));
            this._elemNameIndex[hash] = LinkSimilarElements(this._elemNameIndex[hash].Page, this._elemNameIndex[hash].Index, this._pageParent, this._idxParent);
        }

        /// <summary>
        /// Must be called when an element node's children have been fully enumerated.
        /// </summary>
        public override void WriteEndElement()
        {
            WriteEndElement(true);
        }

        /// <summary>
        /// Must be called when an element node's children have been fully enumerated.
        /// </summary>
        public override void WriteFullEndElement()
        {
            WriteEndElement(false);
        }

        /// <summary>
        /// Must be called when an element node's children have been fully enumerated.
        /// </summary>
        internal override void WriteEndElement(string prefix, string localName, string namespaceName)
        {
            WriteEndElement(true);
        }

        /// <summary>
        /// Must be called when an element node's children have been fully enumerated.
        /// </summary>
        internal override void WriteFullEndElement(string prefix, string localName, string namespaceName)
        {
            WriteEndElement(false);
        }

        /// <summary>
        /// Must be called when an element node's children have been fully enumerated.
        /// </summary>
        public void WriteEndElement(bool allowShortcutTag)
        {
            XPathNodeRef nodeRef;
            Debug.Assert(this._pageParent[this._idxParent].NodeType == XPathNodeType.Element);

            // If element has no content-typed children except for the one about to be added, then
            // its value is the same as its only text child's.
            if (!this._pageParent[this._idxParent].HasContentChild)
            {
                switch (this._textBldr.TextType)
                {
                    case TextBlockType.Text:
                        // Collapsed text node can be created if text line number information can be encoded efficiently in parent node
                        if (this._lineInfo != null)
                        {
                            // If collapsed text node is not on same line as parent, don't collapse text
                            if (this._textBldr.LineNumber != this._pageParent[this._idxParent].LineNumber)
                                goto case TextBlockType.Whitespace;

                            // If position is not within 256 of parent, don't collapse text
                            int posDiff = this._textBldr.LinePosition - this._pageParent[this._idxParent].LinePosition;
                            if (posDiff < 0 || posDiff > XPathNode.MaxCollapsedPositionOffset)
                                goto case TextBlockType.Whitespace;

                            // Set collapsed node line position offset
                            this._pageParent[this._idxParent].SetCollapsedLineInfoOffset(posDiff);
                        }

                        // Set collapsed node text
                        this._pageParent[this._idxParent].SetCollapsedValue(this._textBldr.ReadText());
                        break;

                    case TextBlockType.SignificantWhitespace:
                    case TextBlockType.Whitespace:
                        // Create separate whitespace node
                        CachedTextNode();
                        this._pageParent[this._idxParent].SetValue(this._pageSibling[this._idxSibling].Value);
                        break;

                    default:
                        // Empty value, so don't create collapsed text node
                        this._pageParent[this._idxParent].SetEmptyValue(allowShortcutTag);
                        break;
                }
            }
            else
            {
                if (this._textBldr.HasText)
                {
                    // Element's last child (one of several) is a text or whitespace node
                    CachedTextNode();
                }
            }

            // If namespaces were added to this element,
            if (this._pageParent[this._idxParent].HasNamespaceDecls)
            {
                // Add it to the document's element --> namespace mapping
                this._doc.AddNamespace(this._pageParent, this._idxParent, this._pageNmsp, this._idxNmsp);

                // Restore the previous namespace chain
                nodeRef = this._stkNmsp.Pop();
                this._pageNmsp = nodeRef.Page;
                this._idxNmsp = nodeRef.Index;
            }

            // Make parent of this element the current element
            this._pageSibling = this._pageParent;
            this._idxSibling = this._idxParent;
            this._idxParent = this._pageParent[this._idxParent].GetParent(out this._pageParent);
        }

        /// <summary>
        /// Shortcut for calling WriteStartAttribute with attrfType == null.
        /// </summary>
        public override void WriteStartAttribute(string prefix, string localName, string namespaceName)
        {
            Debug.Assert(!prefix.Equals("xmlns"));
            Debug.Assert(this._idxParent == 0 || this._pageParent[this._idxParent].NodeType == XPathNodeType.Element);
            Debug.Assert(this._idxSibling == 0 || this._pageSibling[this._idxSibling].NodeType == XPathNodeType.Attribute);

            if (this._atomizeNames)
            {
                prefix = this._nameTable.Add(prefix);
                localName = this._nameTable.Add(localName);
                namespaceName = this._nameTable.Add(namespaceName);
            }

            AddSibling(XPathNodeType.Attribute, localName, namespaceName, prefix, string.Empty);
        }

        /// <summary>
        /// Attach the attribute's text or typed value to the previously constructed attribute node.
        /// </summary>
        public override void WriteEndAttribute()
        {
            Debug.Assert(this._pageSibling[this._idxSibling].NodeType == XPathNodeType.Attribute);

            this._pageSibling[this._idxSibling].SetValue(this._textBldr.ReadText());
        }

        /// <summary>
        /// Map CData text into regular text.
        /// </summary>
        public override void WriteCData(string text)
        {
            WriteString(text, TextBlockType.Text);
        }

        /// <summary>
        /// Construct comment node.
        /// </summary>
        public override void WriteComment(string text)
        {
            AddSibling(XPathNodeType.Comment, string.Empty, string.Empty, string.Empty, string.Empty);
            this._pageSibling[this._idxSibling].SetValue(text);
        }

        /// <summary>
        /// Shortcut for calling WriteProcessingInstruction with baseUri = string.Empty.
        /// </summary>
        public override void WriteProcessingInstruction(string name, string text)
        {
            this.WriteProcessingInstruction(name, text, string.Empty);
        }

        /// <summary>
        /// Construct pi node.
        /// </summary>
        public void WriteProcessingInstruction(string name, string text, string baseUri)
        {
            if (this._atomizeNames)
                name = this._nameTable.Add(name);

            AddSibling(XPathNodeType.ProcessingInstruction, name, string.Empty, string.Empty, baseUri);
            this._pageSibling[this._idxSibling].SetValue(text);
        }

        /// <summary>
        /// Write a whitespace text block.
        /// </summary>
        public override void WriteWhitespace(string ws)
        {
            WriteString(ws, TextBlockType.Whitespace);
        }

        /// <summary>
        /// Write an attribute or element text block.
        /// </summary>
        public override void WriteString(string text)
        {
            WriteString(text, TextBlockType.Text);
        }

        public override void WriteChars(char[] buffer, int index, int count)
        {
            WriteString(new string(buffer, index, count), TextBlockType.Text);
        }

        /// <summary>
        /// Map RawText to Text.  This will lose entitization and won't roundtrip.
        /// </summary>
        public override void WriteRaw(string data)
        {
            WriteString(data, TextBlockType.Text);
        }

        public override void WriteRaw(char[] buffer, int index, int count)
        {
            WriteString(new string(buffer, index, count), TextBlockType.Text);
        }

        /// <summary>
        /// Write an element text block with the specified text type (whitespace, significant whitespace, or text).
        /// </summary>
        public void WriteString(string text, TextBlockType textType)
        {
            this._textBldr.WriteTextBlock(text, textType);
        }

        /// <summary>
        /// Cache does not handle entity references.
        /// </summary>
        public override void WriteEntityRef(string name)
        {
            throw NotImplemented.ByDesign;
        }

        /// <summary>
        /// Don't entitize, since the cache cannot represent character entities.
        /// </summary>
        public override void WriteCharEntity(char ch)
        {
            WriteString(new string(ch, 1), TextBlockType.Text);
        }

        /// <summary>
        /// Don't entitize, since the cache cannot represent character entities.
        /// </summary>
        public override void WriteSurrogateCharEntity(char lowChar, char highChar)
        {
            char[] chars = { highChar, lowChar };
            WriteString(new string(chars), TextBlockType.Text);
        }

        /// <summary>
        /// Signals the end of tree construction.
        /// </summary>
        internal void CloseWithoutDisposing()
        {
            XPathNode[] page;
            int idx;

            // If cached text exists, then create a text node at the top-level
            if (this._textBldr.HasText)
                CachedTextNode();

            // If document does not yet contain nodes, then an empty text node must have been created
            idx = this._doc.GetRootNode(out page);
            if (idx == this._nodePageFact.NextNodeIndex && page == this._nodePageFact.NextNodePage)
            {
                AddSibling(XPathNodeType.Text, string.Empty, string.Empty, string.Empty, string.Empty);
                this._pageSibling[this._idxSibling].SetValue(string.Empty);
            }
        }

        /// <summary>
        /// Since output is not forwarded to another object, this does nothing.
        /// </summary>
        public override void Flush()
        {
        }


        //-----------------------------------------------
        // XmlRawWriter interface
        //-----------------------------------------------

        /// <summary>
        /// Write the xml declaration.  This must be the first call after Open.
        /// </summary>
        internal override void WriteXmlDeclaration(XmlStandalone standalone)
        {
            // Ignore the xml declaration when building the cache
        }

        internal override void WriteXmlDeclaration(string xmldecl)
        {
            // Ignore the xml declaration when building the cache
        }

        /// <summary>
        /// Called as element node's children are about to be enumerated.
        /// </summary>
        internal override void StartElementContent()
        {
            Debug.Assert(this._pageParent[this._idxParent].NodeType == XPathNodeType.Element);
        }

        /// <summary>
        /// Build a namespace declaration node.  Attach it to an element parent, if one was previously constructed.
        /// All namespace declarations are linked together in an in-scope namespace tree.
        /// </summary>
        internal override void WriteNamespaceDeclaration(string prefix, string namespaceName)
        {
            XPathNode[] pageTemp, pageOverride, pageNew, pageOrig, pageCopy;
            int idxTemp, idxOverride, idxNew, idxOrig, idxCopy;
            Debug.Assert(this._idxSibling == 0 || this._pageSibling[this._idxSibling].NodeType == XPathNodeType.Attribute);
            Debug.Assert(!prefix.Equals("xmlns") && !namespaceName.Equals(XmlConst.ReservedNsXmlNs));
            Debug.Assert(this._idxParent == 0 || this._idxNmsp != 0);
            Debug.Assert(this._idxParent == 0 || this._pageParent[this._idxParent].NodeType == XPathNodeType.Element);

            if (this._atomizeNames)
                prefix = this._nameTable.Add(prefix);

            namespaceName = this._nameTable.Add(namespaceName);

            // Does the new namespace override a previous namespace node?
            pageOverride = this._pageNmsp;
            idxOverride = this._idxNmsp;
            while (idxOverride != 0)
            {
                if ((object)pageOverride[idxOverride].LocalName == (object)prefix)
                {
                    // Need to clone all namespaces up until the overridden node in order to bypass it
                    break;
                }
                idxOverride = pageOverride[idxOverride].GetSibling(out pageOverride);
            }

            // Create new namespace node and add it to front of namespace list
            idxNew = NewNamespaceNode(out pageNew, prefix, namespaceName, this._pageParent, this._idxParent);

            if (idxOverride != 0)
            {
                // Bypass overriden node by cloning nodes in list leading to it
                pageOrig = this._pageNmsp;
                idxOrig = this._idxNmsp;
                pageCopy = pageNew;
                idxCopy = idxNew;

                while (idxOrig != idxOverride || pageOrig != pageOverride)
                {
                    // Make a copy of the original namespace node
                    idxTemp = pageOrig[idxOrig].GetParent(out pageTemp);
                    idxTemp = NewNamespaceNode(out pageTemp, pageOrig[idxOrig].LocalName, pageOrig[idxOrig].Value, pageTemp, idxTemp);

                    // Attach copy to chain of copied nodes
                    pageCopy[idxCopy].SetSibling(this._infoTable, pageTemp, idxTemp);

                    // Position on the new copy
                    pageCopy = pageTemp;
                    idxCopy = idxTemp;

                    // Get next original sibling
                    idxOrig = pageOrig[idxOrig].GetSibling(out pageOrig);
                }

                // Link farther up in the original chain, just past the last overriden node
                idxOverride = pageOverride[idxOverride].GetSibling(out pageOverride);

                if (idxOverride != 0)
                    pageCopy[idxCopy].SetSibling(this._infoTable, pageOverride, idxOverride);
                else
                    Debug.Assert(prefix.Equals("xml"), "xmlns:xml namespace declaration should always be present in the list.");
            }
            else if (this._idxParent != 0)
            {
                // Link new node directly to last in-scope namespace.  No overrides necessary.
                pageNew[idxNew].SetSibling(this._infoTable, this._pageNmsp, this._idxNmsp);
            }
            else
            {
                // Floating namespace, so make this the root of the tree
                this._doc.SetRootNode(pageNew, idxNew);
            }

            if (this._idxParent != 0)
            {
                // If this is the first namespace on the current element,
                if (!this._pageParent[this._idxParent].HasNamespaceDecls)
                {
                    // Then save the last in-scope namespace on a stack so that EndElementNode can restore it.
                    this._stkNmsp.Push(new XPathNodeRef(this._pageNmsp, this._idxNmsp));

                    // Mark element parent as having namespace nodes declared on it
                    this._pageParent[this._idxParent].HasNamespaceDecls = true;
                }

                // New namespace is now last in-scope namespace
                this._pageNmsp = pageNew;
                this._idxNmsp = idxNew;
            }
        }


        //-----------------------------------------------
        // Custom Build Helper Methods
        //-----------------------------------------------

        /// <summary>
        /// Link "prev" element with "next" element, which has a "similar" name.  This increases the performance of searches by element name.
        /// </summary>
        private XPathNodeRef LinkSimilarElements(XPathNode[] pagePrev, int idxPrev, XPathNode[] pageNext, int idxNext)
        {
            // Set link on previous element
            if (pagePrev != null)
                pagePrev[idxPrev].SetSimilarElement(this._infoTable, pageNext, idxNext);

            // Add next element to index
            return new XPathNodeRef(pageNext, idxNext);
        }

        /// <summary>
        /// Helper method that constructs a new Namespace XPathNode.
        /// </summary>
        private int NewNamespaceNode(out XPathNode[] page, string prefix, string namespaceUri, XPathNode[] pageElem, int idxElem)
        {
            XPathNode[] pageNode;
            int idxNode, lineNumOffset, linePosOffset;
            XPathNodeInfoAtom info;
            Debug.Assert(pageElem == null || pageElem[idxElem].NodeType == XPathNodeType.Element);

            // Allocate a page slot for the new XPathNode
            this._nmspPageFact.AllocateSlot(out pageNode, out idxNode);

            // Compute node's line number information
            ComputeLineInfo(false, out lineNumOffset, out linePosOffset);

            // Obtain a XPathNodeInfoAtom object for this node
            info = this._infoTable.Create(prefix, string.Empty, string.Empty, string.Empty,
                                         pageElem, pageNode, null,
                                         this._doc, this._lineNumBase, this._linePosBase);

            // Initialize the new node
            pageNode[idxNode].Create(info, XPathNodeType.Namespace, idxElem);
            pageNode[idxNode].SetValue(namespaceUri);
            pageNode[idxNode].SetLineInfoOffsets(lineNumOffset, linePosOffset);

            page = pageNode;
            return idxNode;
        }

        /// <summary>
        /// Helper method that constructs a new XPathNode.
        /// </summary>
        private int NewNode(out XPathNode[] page, XPathNodeType xptyp, string localName, string namespaceUri, string prefix, string baseUri)
        {
            XPathNode[] pageNode;
            int idxNode, lineNumOffset, linePosOffset;
            XPathNodeInfoAtom info;
            Debug.Assert(xptyp != XPathNodeType.Namespace);

            // Allocate a page slot for the new XPathNode
            this._nodePageFact.AllocateSlot(out pageNode, out idxNode);

            // Compute node's line number information
            ComputeLineInfo(XPathNavigator.IsText(xptyp), out lineNumOffset, out linePosOffset);

            // Obtain a XPathNodeInfoAtom object for this node
            info = this._infoTable.Create(localName, namespaceUri, prefix, baseUri,
                                         this._pageParent, pageNode, pageNode,
                                         this._doc, this._lineNumBase, this._linePosBase);

            // Initialize the new node
            pageNode[idxNode].Create(info, xptyp, this._idxParent);
            pageNode[idxNode].SetLineInfoOffsets(lineNumOffset, linePosOffset);

            page = pageNode;
            return idxNode;
        }

        /// <summary>
        /// Compute current node's line number information.
        /// </summary>
        private void ComputeLineInfo(bool isTextNode, out int lineNumOffset, out int linePosOffset)
        {
            int lineNum, linePos;

            if (this._lineInfo == null)
            {
                lineNumOffset = 0;
                linePosOffset = 0;
                return;
            }

            // Get line number info from TextBlockBuilder if current node is a text node
            if (isTextNode)
            {
                lineNum = this._textBldr.LineNumber;
                linePos = this._textBldr.LinePosition;
            }
            else
            {
                Debug.Assert(this._lineInfo.HasLineInfo(), "HasLineInfo should have been checked before this.");
                lineNum = this._lineInfo.LineNumber;
                linePos = this._lineInfo.LinePosition;
            }

            lineNumOffset = lineNum - this._lineNumBase;
            if (lineNumOffset < 0 || lineNumOffset > XPathNode.MaxLineNumberOffset)
            {
                this._lineNumBase = lineNum;
                lineNumOffset = 0;
            }

            linePosOffset = linePos - this._linePosBase;
            if (linePosOffset < 0 || linePosOffset > XPathNode.MaxLinePositionOffset)
            {
                this._linePosBase = linePos;
                linePosOffset = 0;
            }
        }

        /// <summary>
        /// Add a sibling node.  If no previous sibling exists, add the node as the first child of the parent.
        /// If no parent exists, make this node the root of the document.
        /// </summary>
        private void AddSibling(XPathNodeType xptyp, string localName, string namespaceUri, string prefix, string baseUri)
        {
            XPathNode[] pageNew;
            int idxNew;
            Debug.Assert(xptyp != XPathNodeType.Root && xptyp != XPathNodeType.Namespace);

            if (this._textBldr.HasText)
                CachedTextNode();

            idxNew = NewNode(out pageNew, xptyp, localName, namespaceUri, prefix, baseUri);

            // this.idxParent is only 0 for the top-most node
            if (this._idxParent != 0)
            {
                // Set properties on parent
                this._pageParent[this._idxParent].SetParentProperties(xptyp);

                if (this._idxSibling == 0)
                {
                    // This is the first child of the parent (so should be allocated immediately after parent)
                    Debug.Assert(this._idxParent + 1 == idxNew || idxNew == 1);
                }
                else
                {
                    // There is already a previous sibling
                    this._pageSibling[this._idxSibling].SetSibling(this._infoTable, pageNew, idxNew);
                }
            }

            this._pageSibling = pageNew;
            this._idxSibling = idxNew;
        }

        /// <summary>
        /// Creates a text node from cached text parts.
        /// </summary>
        private void CachedTextNode()
        {
            TextBlockType textType;
            string text;
            Debug.Assert(this._textBldr.HasText || (this._idxSibling == 0 && this._idxParent == 0), "Cannot create empty text node unless it's a top-level text node.");
            Debug.Assert(this._idxSibling == 0 || !this._pageSibling[this._idxSibling].IsText, "Cannot create adjacent text nodes.");

            // Create a text node
            textType = this._textBldr.TextType;
            text = this._textBldr.ReadText();
            AddSibling((XPathNodeType)textType, string.Empty, string.Empty, string.Empty, string.Empty);
            this._pageSibling[this._idxSibling].SetValue(text);
        }

        /// <summary>
        /// Allocates pages of nodes for the XPathDocumentBuilder.  The initial pages and arrays are
        /// fairly small.  As each page fills, a new page that is twice as big is allocated.
        /// The max size of a page is 65536 nodes, since XPathNode indexes are 16-bits.
        /// </summary>
        private struct NodePageFactory
        {
            private XPathNode[] _page;
            private XPathNodePageInfo _pageInfo;
            private int _pageSize;

            /// <summary>
            /// Allocates and returns the initial node page.
            /// </summary>
            public void Init(int initialPageSize)
            {
                // 0th slot: Index 0 is reserved to mean "null node".  Only use 0th slot to store PageInfo.
                this._pageSize = initialPageSize;
                this._page = new XPathNode[this._pageSize];
                this._pageInfo = new XPathNodePageInfo(null, 1);
                this._page[0].Create(this._pageInfo);
            }

            /// <summary>
            /// Return the page on which the next node will be allocated.
            /// </summary>
            public XPathNode[] NextNodePage
            {
                get { return this._page; }
            }

            /// <summary>
            /// Return the page index that the next node will be given.
            /// </summary>
            public int NextNodeIndex
            {
                get { return this._pageInfo.NodeCount; }
            }

            /// <summary>
            /// Allocate the next slot in the current node page.  Return a reference to the page and the index
            /// of the allocated slot.
            /// </summary>
            public void AllocateSlot(out XPathNode[] page, out int idx)
            {
                page = this._page;
                idx = this._pageInfo.NodeCount;

                // Allocate new page if necessary
                if (++this._pageInfo.NodeCount >= this._page.Length)
                {
                    if (this._pageSize < (1 << 16))
                    {
                        // New page shouldn't contain more slots than 16 bits can address
                        this._pageSize *= 2;
                    }
                    this._page = new XPathNode[this._pageSize];
                    this._pageInfo.NextPage = this._page;
                    this._pageInfo = new XPathNodePageInfo(page, this._pageInfo.PageNumber + 1);
                    this._page[0].Create(this._pageInfo);
                }
            }
        }

        /// <summary>
        /// This class concatenates adjacent text blocks and tracks TextBlockType and line number information.
        /// </summary>
        private struct TextBlockBuilder
        {
            private IXmlLineInfo _lineInfo;
            private TextBlockType _textType;
            private string _text;
            private int _lineNum, _linePos;

            /// <summary>
            /// Constructor.
            /// </summary>
            public void Initialize(IXmlLineInfo lineInfo)
            {
                this._lineInfo = lineInfo;
                this._textType = TextBlockType.None;
            }

            /// <summary>
            /// Return the type of the cached text block.
            /// </summary>
            public TextBlockType TextType
            {
                get { return this._textType; }
            }

            /// <summary>
            /// Returns true if text has been cached.
            /// </summary>
            public bool HasText
            {
                get { return this._textType != TextBlockType.None; }
            }

            /// <summary>
            /// Returns the line number of the last text block to be cached.
            /// </summary>
            public int LineNumber
            {
                get { return this._lineNum; }
            }

            /// <summary>
            /// Returns the line position of the last text block to be cached.
            /// </summary>
            public int LinePosition
            {
                get { return this._linePos; }
            }

            /// <summary>
            /// Append a text block with the specified type.
            /// </summary>
            public void WriteTextBlock(string text, TextBlockType textType)
            {
                Debug.Assert((int)XPathNodeType.Text < (int)XPathNodeType.SignificantWhitespace);
                Debug.Assert((int)XPathNodeType.SignificantWhitespace < (int)XPathNodeType.Whitespace);

                if (text.Length != 0)
                {
                    if (this._textType == TextBlockType.None)
                    {
                        this._text = text;
                        this._textType = textType;

                        if (this._lineInfo != null)
                        {
                            this._lineNum = this._lineInfo.LineNumber;
                            this._linePos = this._lineInfo.LinePosition;
                        }
                    }
                    else
                    {
                        this._text = string.Concat(this._text, text);

                        // Determine whether text is Text, Whitespace, or SignificantWhitespace
                        if ((int)textType < (int)this._textType)
                            this._textType = textType;
                    }
                }
            }

            /// <summary>
            /// Read all cached text, or string.Empty if no text has been cached, and clear the text block type.
            /// </summary>
            public string ReadText()
            {
                if (this._textType == TextBlockType.None)
                    return string.Empty;

                this._textType = TextBlockType.None;
                return this._text;
            }
        }
    }
}
