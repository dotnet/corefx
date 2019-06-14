// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
    internal class XPathDescendantIterator : XPathAxisIterator
    {
        private int _level = 0;
        private bool _done = false;

        public XPathDescendantIterator(XPathNavigator nav, XPathNodeType type, bool matchSelf) : base(nav, type, matchSelf) { }
        public XPathDescendantIterator(XPathNavigator nav, string name, string namespaceURI, bool matchSelf) : base(nav, name, namespaceURI, matchSelf) { }

        public XPathDescendantIterator(XPathDescendantIterator it) : base(it)
        {
            _level = it._level;
            _done = it._done;
        }

        public override XPathNodeIterator Clone()
        {
            return new XPathDescendantIterator(this);
        }

        public override bool MoveNext()
        {
            if (_done)
            {
                return false;
            }

            if (first)
            {
                first = false;
                if (matchSelf && Matches)
                {
                    position = 1;
                    return true;
                }
            }

            while (true)
            {
                if (nav.MoveToFirstChild())
                {
                    _level++;
                }
                else
                {
                    while (true)
                    {
                        if (_level == 0)
                        {
                            return false;
                        }
                        if (nav.MoveToNext())
                        {
                            break;
                        }
                        nav.MoveToParent();
                        _level--;
                    }
                }

                if (Matches)
                {
                    position++;
                    return true;
                }
            }
        }
    }
}
