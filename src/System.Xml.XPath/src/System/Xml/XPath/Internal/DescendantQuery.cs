// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
    internal class DescendantQuery : DescendantBaseQuery
    {
        private XPathNodeIterator _nodeIterator;

        internal DescendantQuery(Query qyParent, string Name, string Prefix, XPathNodeType Type, bool matchSelf, bool abbrAxis)
            : base(qyParent, Name, Prefix, Type, matchSelf, abbrAxis)
        { }

        public DescendantQuery(DescendantQuery other) : base(other)
        {
            _nodeIterator = Clone(other._nodeIterator);
        }

        public override void Reset()
        {
            _nodeIterator = null;
            base.Reset();
        }

        public override XPathNavigator Advance()
        {
            while (true)
            {
                if (_nodeIterator == null)
                {
                    position = 0;
                    XPathNavigator nav = qyInput.Advance();
                    if (nav == null)
                    {
                        return null;
                    }
                    if (NameTest)
                    {
                        if (TypeTest == XPathNodeType.ProcessingInstruction)
                        {
                            _nodeIterator = new IteratorFilter(nav.SelectDescendants(TypeTest, matchSelf), Name);
                        }
                        else
                        {
                            _nodeIterator = nav.SelectDescendants(Name, Namespace, matchSelf);
                        }
                    }
                    else
                    {
                        _nodeIterator = nav.SelectDescendants(TypeTest, matchSelf);
                    }
                }

                if (_nodeIterator.MoveNext())
                {
                    position++;
                    currentNode = _nodeIterator.Current;
                    return currentNode;
                }
                else
                {
                    _nodeIterator = null;
                }
            }
        }

        public override XPathNodeIterator Clone() { return new DescendantQuery(this); }
    }
}
