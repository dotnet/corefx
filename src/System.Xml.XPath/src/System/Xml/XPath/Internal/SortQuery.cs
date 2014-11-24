// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace MS.Internal.Xml.XPath
{
    internal sealed class SortQuery : Query
    {
        private List<SortKey> _results;
        private XPathSortComparer _comparer;
        private Query _qyInput;

        public SortQuery(Query qyInput)
        {
            Debug.Assert(qyInput != null, "Sort Query needs an input query tree to work on");
            this._results = new List<SortKey>();
            this._comparer = new XPathSortComparer();
            this._qyInput = qyInput;
            count = 0;
        }
        private SortQuery(SortQuery other) : base(other)
        {
            this._results = new List<SortKey>(other._results);
            this._comparer = other._comparer.Clone();
            this._qyInput = Clone(other._qyInput);
            count = 0;
        }

        public override void Reset() { count = 0; }

        public override void SetXsltContext(XsltContext xsltContext)
        {
            _qyInput.SetXsltContext(xsltContext);
            if (
                _qyInput.StaticType != XPathResultType.NodeSet &&
                _qyInput.StaticType != XPathResultType.Any
            )
            {
                throw XPathException.Create(SR.Xp_NodeSetExpected);
            }
        }

        private void BuildResultsList()
        {
            Int32 numSorts = this._comparer.NumSorts;

            Debug.Assert(numSorts > 0, "Why was the sort query created?");

            XPathNavigator eNext;
            while ((eNext = _qyInput.Advance()) != null)
            {
                SortKey key = new SortKey(numSorts, /*originalPosition:*/this._results.Count, eNext.Clone());

                for (Int32 j = 0; j < numSorts; j++)
                {
                    key[j] = this._comparer.Expression(j).Evaluate(_qyInput);
                }

                _results.Add(key);
            }
            _results.Sort(this._comparer);
        }

        public override object Evaluate(XPathNodeIterator context)
        {
            _qyInput.Evaluate(context);
            this._results.Clear();
            BuildResultsList();
            count = 0;
            return this;
        }

        public override XPathNavigator Advance()
        {
            Debug.Assert(0 <= count && count <= _results.Count);
            if (count < this._results.Count)
            {
                return this._results[count++].Node;
            }
            return null;
        }

        public override XPathNavigator Current
        {
            get
            {
                Debug.Assert(0 <= count && count <= _results.Count);
                if (count == 0)
                {
                    return null;
                }
                return _results[count - 1].Node;
            }
        }

        internal void AddSort(Query evalQuery, IComparer comparer)
        {
            this._comparer.AddSort(evalQuery, comparer);
        }

        public override XPathNodeIterator Clone() { return new SortQuery(this); }

        public override XPathResultType StaticType { get { return XPathResultType.NodeSet; } }
        public override int CurrentPosition { get { return count; } }
        public override int Count { get { return _results.Count; } }
        public override QueryProps Properties { get { return QueryProps.Cached | QueryProps.Position | QueryProps.Count; } }

        public override void PrintQuery(XmlWriter w)
        {
            w.WriteStartElement(this.GetType().Name);
            _qyInput.PrintQuery(w);
            w.WriteElementString("XPathSortComparer", "... PrintTree() not implemented ...");
            w.WriteEndElement();
        }
    } // class SortQuery

    internal sealed class SortKey
    {
        private Int32 _numKeys;
        private object[] _keys;
        private int _originalPosition;
        private XPathNavigator _node;

        public SortKey(int numKeys, int originalPosition, XPathNavigator node)
        {
            this._numKeys = numKeys;
            this._keys = new object[numKeys];
            this._originalPosition = originalPosition;
            this._node = node;
        }

        public object this[int index]
        {
            get { return this._keys[index]; }
            set { this._keys[index] = value; }
        }

        public int NumKeys { get { return this._numKeys; } }
        public int OriginalPosition { get { return this._originalPosition; } }
        public XPathNavigator Node { get { return this._node; } }
    } // class SortKey

    internal sealed class XPathSortComparer : IComparer<SortKey>
    {
        private const int minSize = 3;
        private Query[] _expressions;
        private IComparer[] _comparers;
        private int _numSorts;

        public XPathSortComparer(int size)
        {
            if (size <= 0) size = minSize;
            this._expressions = new Query[size];
            this._comparers = new IComparer[size];
        }
        public XPathSortComparer() : this(minSize) { }

        public void AddSort(Query evalQuery, IComparer comparer)
        {
            Debug.Assert(this._expressions.Length == this._comparers.Length);
            Debug.Assert(0 < this._expressions.Length);
            Debug.Assert(0 <= _numSorts && _numSorts <= this._expressions.Length);
            // Ajust array sizes if needed.
            if (_numSorts == this._expressions.Length)
            {
                Query[] newExpressions = new Query[_numSorts * 2];
                IComparer[] newComparers = new IComparer[_numSorts * 2];
                for (int i = 0; i < _numSorts; i++)
                {
                    newExpressions[i] = this._expressions[i];
                    newComparers[i] = this._comparers[i];
                }
                this._expressions = newExpressions;
                this._comparers = newComparers;
            }
            Debug.Assert(_numSorts < this._expressions.Length);

            // Fixup expression to handle node-set return type:
            if (evalQuery.StaticType == XPathResultType.NodeSet || evalQuery.StaticType == XPathResultType.Any)
            {
                evalQuery = new StringFunctions(Function.FunctionType.FuncString, new Query[] { evalQuery });
            }

            this._expressions[_numSorts] = evalQuery;
            this._comparers[_numSorts] = comparer;
            _numSorts++;
        }

        public int NumSorts { get { return _numSorts; } }

        public Query Expression(int i)
        {
            return this._expressions[i];
        }

        int IComparer<SortKey>.Compare(SortKey x, SortKey y)
        {
            Debug.Assert(x != null && y != null, "Oops!! what happened?");
            int result = 0;
            for (int i = 0; i < x.NumKeys; i++)
            {
                result = this._comparers[i].Compare(x[i], y[i]);
                if (result != 0)
                {
                    return result;
                }
            }

            // if after all comparisions, the two sort keys are still equal, preserve the doc order
            return x.OriginalPosition - y.OriginalPosition;
        }

        internal XPathSortComparer Clone()
        {
            XPathSortComparer clone = new XPathSortComparer(this._numSorts);

            for (int i = 0; i < this._numSorts; i++)
            {
                clone._comparers[i] = this._comparers[i];
                clone._expressions[i] = (Query)this._expressions[i].Clone(); // Expressions should be cloned because Query should be cloned
            }
            clone._numSorts = this._numSorts;
            return clone;
        }
    } // class XPathSortComparer
} // namespace
