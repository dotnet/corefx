// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Diagnostics;

namespace System.Xml
{
    internal class XmlElementList : XmlNodeList
    {
        private string _asterisk;
        private int _changeCount; //recording the total number that the dom tree has been changed ( insertion and deletetion )
        //the member vars below are saved for further reconstruction        
        private string _name;         //only one of 2 string groups will be initialized depends on which constructor is called.
        private string _localName;
        private string _namespaceURI;
        private XmlNode _rootNode;
        // the memeber vars belwo serves the optimization of accessing of the elements in the list
        private int _curInd;       // -1 means the starting point for a new search round
        private XmlNode _curElem;      // if sets to rootNode, means the starting point for a new search round
        private bool _empty;        // whether the list is empty
        private bool _atomized;     //whether the localname and namespaceuri are aomized
        private int _matchCount;   // cached list count. -1 means it needs reconstruction

        private WeakReference _listener;   // XmlElementListListener

        private XmlElementList(XmlNode parent)
        {
            Debug.Assert(parent != null);
            Debug.Assert(parent.NodeType == XmlNodeType.Element || parent.NodeType == XmlNodeType.Document);
            this._rootNode = parent;
            Debug.Assert(parent.Document != null);
            this._curInd = -1;
            this._curElem = _rootNode;
            this._changeCount = 0;
            this._empty = false;
            this._atomized = true;
            this._matchCount = -1;
            // This can be a regular reference, but it would cause some kind of loop inside the GC
            this._listener = new WeakReference(new XmlElementListListener(parent.Document, this));
        }

        ~XmlElementList()
        {
            Dispose(false);
        }

        internal void ConcurrencyCheck(XmlNodeChangedEventArgs args)
        {
            if (_atomized == false)
            {
                XmlNameTable nameTable = this._rootNode.Document.NameTable;
                this._localName = nameTable.Add(this._localName);
                this._namespaceURI = nameTable.Add(this._namespaceURI);
                this._atomized = true;
            }
            if (IsMatch(args.Node))
            {
                this._changeCount++;
                this._curInd = -1;
                this._curElem = _rootNode;
                if (args.Action == XmlNodeChangedAction.Insert)
                    this._empty = false;
            }
            this._matchCount = -1;
        }

        internal XmlElementList(XmlNode parent, string name) : this(parent)
        {
            Debug.Assert(parent.Document != null);
            XmlNameTable nt = parent.Document.NameTable;
            Debug.Assert(nt != null);
            _asterisk = nt.Add("*");
            this._name = nt.Add(name);
            this._localName = null;
            this._namespaceURI = null;
        }

        internal XmlElementList(XmlNode parent, string localName, string namespaceURI) : this(parent)
        {
            Debug.Assert(parent.Document != null);
            XmlNameTable nt = parent.Document.NameTable;
            Debug.Assert(nt != null);
            _asterisk = nt.Add("*");
            this._localName = nt.Get(localName);
            this._namespaceURI = nt.Get(namespaceURI);
            if ((this._localName == null) || (this._namespaceURI == null))
            {
                this._empty = true;
                this._atomized = false;
                this._localName = localName;
                this._namespaceURI = namespaceURI;
            }
            this._name = null;
        }

        internal int ChangeCount
        {
            get { return _changeCount; }
        }

        // return the next element node that is in PreOrder
        private XmlNode NextElemInPreOrder(XmlNode curNode)
        {
            Debug.Assert(curNode != null);
            //For preorder walking, first try its child
            XmlNode retNode = curNode.FirstChild;
            if (retNode == null)
            {
                //if no child, the next node forward will the be the NextSibling of the first ancestor which has NextSibling
                //so, first while-loop find out such an ancestor (until no more ancestor or the ancestor is the rootNode
                retNode = curNode;
                while (retNode != null
                        && retNode != _rootNode
                        && retNode.NextSibling == null)
                {
                    retNode = retNode.ParentNode;
                }
                //then if such ancestor exists, set the retNode to its NextSibling
                if (retNode != null && retNode != _rootNode)
                    retNode = retNode.NextSibling;
            }
            if (retNode == this._rootNode)
                //if reach the rootNode, consider having walked through the whole tree and no more element after the curNode
                retNode = null;
            return retNode;
        }

        // return the previous element node that is in PreOrder
        private XmlNode PrevElemInPreOrder(XmlNode curNode)
        {
            Debug.Assert(curNode != null);
            //For preorder walking, the previous node will be the right-most node in the tree of PreviousSibling of the curNode
            XmlNode retNode = curNode.PreviousSibling;
            // so if the PreviousSibling is not null, going through the tree down to find the right-most node
            while (retNode != null)
            {
                if (retNode.LastChild == null)
                    break;
                retNode = retNode.LastChild;
            }
            // if no PreviousSibling, the previous node will be the curNode's parentNode
            if (retNode == null)
                retNode = curNode.ParentNode;
            // if the final retNode is rootNode, consider having walked through the tree and no more previous node
            if (retNode == this._rootNode)
                retNode = null;
            return retNode;
        }

        // if the current node a matching element node
        private bool IsMatch(XmlNode curNode)
        {
            if (curNode.NodeType == XmlNodeType.Element)
            {
                if (this._name != null)
                {
                    if (Ref.Equal(this._name, _asterisk) || Ref.Equal(curNode.Name, this._name))
                        return true;
                }
                else
                {
                    if (
                        (Ref.Equal(this._localName, _asterisk) || Ref.Equal(curNode.LocalName, this._localName)) &&
                        (Ref.Equal(this._namespaceURI, _asterisk) || curNode.NamespaceURI == this._namespaceURI)
                    )
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private XmlNode GetMatchingNode(XmlNode n, bool bNext)
        {
            Debug.Assert(n != null);
            XmlNode node = n;
            do
            {
                if (bNext)
                    node = NextElemInPreOrder(node);
                else
                    node = PrevElemInPreOrder(node);
            } while (node != null && !IsMatch(node));
            return node;
        }

        private XmlNode GetNthMatchingNode(XmlNode n, bool bNext, int nCount)
        {
            Debug.Assert(n != null);
            XmlNode node = n;
            for (int ind = 0; ind < nCount; ind++)
            {
                node = GetMatchingNode(node, bNext);
                if (node == null)
                    return null;
            }
            return node;
        }

        //the function is for the enumerator to find out the next available matching element node
        public XmlNode GetNextNode(XmlNode n)
        {
            if (this._empty == true)
                return null;
            XmlNode node = (n == null) ? _rootNode : n;
            return GetMatchingNode(node, true);
        }

        public override XmlNode Item(int index)
        {
            if (_rootNode == null || index < 0)
                return null;

            if (this._empty == true)
                return null;
            if (_curInd == index)
                return _curElem;
            int nDiff = index - _curInd;
            bool bForward = (nDiff > 0);
            if (nDiff < 0)
                nDiff = -nDiff;
            XmlNode node;
            if ((node = GetNthMatchingNode(_curElem, bForward, nDiff)) != null)
            {
                _curInd = index;
                _curElem = node;
                return _curElem;
            }
            return null;
        }

        public override int Count
        {
            get
            {
                if (this._empty == true)
                    return 0;
                if (this._matchCount < 0)
                {
                    int currMatchCount = 0;
                    int currChangeCount = this._changeCount;
                    XmlNode node = _rootNode;
                    while ((node = GetMatchingNode(node, true)) != null)
                    {
                        currMatchCount++;
                    }
                    if (currChangeCount != this._changeCount)
                    {
                        return currMatchCount;
                    }
                    this._matchCount = currMatchCount;
                }
                return this._matchCount;
            }
        }

        public override IEnumerator GetEnumerator()
        {
            if (this._empty == true)
                return new XmlEmptyElementListEnumerator(this); ;
            return new XmlElementListEnumerator(this);
        }

        protected override void PrivateDisposeNodeList()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this._listener != null)
            {
                XmlElementListListener listener = (XmlElementListListener)this._listener.Target;
                if (listener != null)
                {
                    listener.Unregister();
                }
                this._listener = null;
            }
        }
    }

    internal class XmlElementListEnumerator : IEnumerator
    {
        private XmlElementList _list;
        private XmlNode _curElem;
        private int _changeCount; //save the total number that the dom tree has been changed ( insertion and deletetion ) when this enumerator is created

        public XmlElementListEnumerator(XmlElementList list)
        {
            this._list = list;
            this._curElem = null;
            this._changeCount = list.ChangeCount;
        }

        public bool MoveNext()
        {
            if (_list.ChangeCount != this._changeCount)
            {
                //the number mismatch, there is new change(s) happened since last MoveNext() is called.
                throw new InvalidOperationException(SR.Xdom_Enum_ElementList);
            }
            else
            {
                _curElem = _list.GetNextNode(_curElem);
            }
            return _curElem != null;
        }

        public void Reset()
        {
            _curElem = null;
            //reset the number of changes to be synced with current dom tree as well
            this._changeCount = _list.ChangeCount;
        }

        public object Current
        {
            get { return _curElem; }
        }
    }

    internal class XmlEmptyElementListEnumerator : IEnumerator
    {
        public XmlEmptyElementListEnumerator(XmlElementList list)
        {
        }

        public bool MoveNext()
        {
            return false;
        }

        public void Reset()
        {
        }

        public object Current
        {
            get { return null; }
        }
    }

    internal class XmlElementListListener
    {
        private WeakReference _elemList;
        private XmlDocument _doc;
        private XmlNodeChangedEventHandler _nodeChangeHandler = null;

        internal XmlElementListListener(XmlDocument doc, XmlElementList elemList)
        {
            this._doc = doc;
            this._elemList = new WeakReference(elemList);
            this._nodeChangeHandler = new XmlNodeChangedEventHandler(this.OnListChanged);
            doc.NodeInserted += this._nodeChangeHandler;
            doc.NodeRemoved += this._nodeChangeHandler;
        }

        private void OnListChanged(object sender, XmlNodeChangedEventArgs args)
        {
            lock (this)
            {
                if (this._elemList != null)
                {
                    XmlElementList el = (XmlElementList)this._elemList.Target;
                    if (null != el)
                    {
                        el.ConcurrencyCheck(args);
                    }
                    else
                    {
                        this._doc.NodeInserted -= this._nodeChangeHandler;
                        this._doc.NodeRemoved -= this._nodeChangeHandler;
                        this._elemList = null;
                    }
                }
            }
        }

        // This method is called from the finalizer of XmlElementList
        internal void Unregister()
        {
            lock (this)
            {
                if (_elemList != null)
                {
                    this._doc.NodeInserted -= this._nodeChangeHandler;
                    this._doc.NodeRemoved -= this._nodeChangeHandler;
                    this._elemList = null;
                }
            }
        }
    }
}
