// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
    // We need this wrapper object to:
    //      1. Calculate position
    //      2. Protect internal query.Current from user who may call MoveNext().
    internal class XPathSelectionIterator : ResetableIterator
    {
        private XPathNavigator _nav;
        private Query _query;
        private int _position;

        internal XPathSelectionIterator(XPathNavigator nav, Query query)
        {
            this._nav = nav.Clone();
            this._query = query;
        }

        protected XPathSelectionIterator(XPathSelectionIterator it)
        {
            this._nav = it._nav.Clone();
            this._query = (Query)it._query.Clone();
            this._position = it._position;
        }

        public override void Reset()
        {
            this._query.Reset();
        }

        public override bool MoveNext()
        {
            XPathNavigator n = _query.Advance();
            if (n != null)
            {
                _position++;
                if (!_nav.MoveTo(n))
                {
                    _nav = n.Clone();
                }
                return true;
            }
            return false;
        }

        public override int Count { get { return _query.Count; } }
        public override XPathNavigator Current { get { return _nav; } }
        public override int CurrentPosition { get { return _position; } }
        public override XPathNodeIterator Clone() { return new XPathSelectionIterator(this); }
    }
}
