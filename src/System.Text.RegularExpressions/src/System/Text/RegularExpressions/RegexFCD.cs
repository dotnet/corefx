// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// This RegexFCD class is internal to the Regex package.
// It builds a bunch of FC information (RegexFC) about
// the regex for optimization purposes.

// Implementation notes:
//
// This step is as simple as walking the tree and emitting
// sequences of codes.

using System.Buffers;
using System.Collections.Generic;
using System.Globalization;

namespace System.Text.RegularExpressions
{
    internal ref struct RegexFCD
    {
        private const int StackBufferSize = 32;
        private const int BeforeChild = 64;
        private const int AfterChild = 128;

        // where the regex can be pegged

        public const int Beginning = 0x0001;
        public const int Bol = 0x0002;
        public const int Start = 0x0004;
        public const int Eol = 0x0008;
        public const int EndZ = 0x0010;
        public const int End = 0x0020;
        public const int Boundary = 0x0040;
        public const int ECMABoundary = 0x0080;

        private readonly ValueListBuilder<RegexFC> _fcStack;
        private readonly ValueListBuilder<int> _intStack;
        private bool _skipAllChildren;      // don't process any more children at the current level
        private bool _skipchild;            // don't process the current child.
        private bool _failed;

        private RegexFCD(Span<RegexFC> fcStack, Span<int> intStack)
        {
            _fcStack = new ValueListBuilder<RegexFC>(fcStack);
            _intStack = new ValueListBuilder<int>(intStack);
            _failed = false;
            _skipchild = false;
            _skipAllChildren = false;
        }

        /// <summary>
        /// This is the one of the only two functions that should be called from outside.
        /// It takes a RegexTree and computes the set of chars that can start it.
        /// </summary>
        public static RegexPrefix? FirstChars(RegexTree t)
        {
            // Create/rent buffers
            RegexFC[] fcSpan = ArrayPool<RegexFC>.Shared.Rent(StackBufferSize);
            Span<int> intSpan = stackalloc int[StackBufferSize];

            RegexFCD s = new RegexFCD(fcSpan, intSpan);
            RegexFC? fc = s.RegexFCFromRegexTree(t);
            s.Dispose();

            // Return rented buffer
            ArrayPool<RegexFC>.Shared.Return(fcSpan);

            if (fc == null || fc.Value._nullable)
                return null;

            CultureInfo culture = ((t.Options & RegexOptions.CultureInvariant) != 0) ? CultureInfo.InvariantCulture : CultureInfo.CurrentCulture;

            return new RegexPrefix(fc.Value.GetFirstChars(culture), fc.Value.CaseInsensitive);
        }

        /// <summary>
        /// This is a related computation: it takes a RegexTree and computes the
        /// leading substring if it see one. It's quite trivial and gives up easily.
        /// </summary>
        public static RegexPrefix Prefix(RegexTree tree)
        {
            RegexNode curNode = tree.Root;
            RegexNode concatNode = null;
            int nextChild = 0;

            for (; ;)
            {
                switch (curNode.NType)
                {
                    case RegexNode.Concatenate:
                        if (curNode.ChildCount() > 0)
                        {
                            concatNode = curNode;
                            nextChild = 0;
                        }
                        break;

                    case RegexNode.Greedy:
                    case RegexNode.Capture:
                        curNode = curNode.Child(0);
                        concatNode = null;
                        continue;

                    case RegexNode.Oneloop:
                    case RegexNode.Onelazy:
                        if (curNode.M > 0)
                        {
                            string pref = string.Empty.PadRight(curNode.M, curNode.Ch);
                            return new RegexPrefix(pref, 0 != (curNode.Options & RegexOptions.IgnoreCase));
                        }
                        else
                            return RegexPrefix.Empty;

                    case RegexNode.One:
                        return new RegexPrefix(curNode.Ch.ToString(), 0 != (curNode.Options & RegexOptions.IgnoreCase));

                    case RegexNode.Multi:
                        return new RegexPrefix(curNode.Str, 0 != (curNode.Options & RegexOptions.IgnoreCase));

                    case RegexNode.Bol:
                    case RegexNode.Eol:
                    case RegexNode.Boundary:
                    case RegexNode.ECMABoundary:
                    case RegexNode.Beginning:
                    case RegexNode.Start:
                    case RegexNode.EndZ:
                    case RegexNode.End:
                    case RegexNode.Empty:
                    case RegexNode.Require:
                    case RegexNode.Prevent:
                        break;

                    default:
                        return RegexPrefix.Empty;
                }

                if (concatNode == null || nextChild >= concatNode.ChildCount())
                    return RegexPrefix.Empty;

                curNode = concatNode.Child(nextChild++);
            }
        }

        /// <summary>
        /// Yet another related computation: it takes a RegexTree and computes 
        /// the leading anchors that it encounters.
        /// </summary>
        public static int Anchors(RegexTree tree)
        {
            RegexNode curNode;
            RegexNode concatNode = null;
            int nextChild = 0;
            int result = 0;

            curNode = tree.Root;

            for (; ;)
            {
                switch (curNode.NType)
                {
                    case RegexNode.Concatenate:
                        if (curNode.ChildCount() > 0)
                        {
                            concatNode = curNode;
                            nextChild = 0;
                        }
                        break;

                    case RegexNode.Greedy:
                    case RegexNode.Capture:
                        curNode = curNode.Child(0);
                        concatNode = null;
                        continue;

                    case RegexNode.Bol:
                    case RegexNode.Eol:
                    case RegexNode.Boundary:
                    case RegexNode.ECMABoundary:
                    case RegexNode.Beginning:
                    case RegexNode.Start:
                    case RegexNode.EndZ:
                    case RegexNode.End:
                        return result | AnchorFromType(curNode.NType);

                    case RegexNode.Empty:
                    case RegexNode.Require:
                    case RegexNode.Prevent:
                        break;

                    default:
                        return result;
                }

                if (concatNode == null || nextChild >= concatNode.ChildCount())
                    return result;

                curNode = concatNode.Child(nextChild++);
            }
        }

        /// <summary>
        /// Convert anchor type to anchor bit.
        /// </summary>
        private static int AnchorFromType(int type)
        {
            switch (type)
            {
                case RegexNode.Bol: return Bol;
                case RegexNode.Eol: return Eol;
                case RegexNode.Boundary: return Boundary;
                case RegexNode.ECMABoundary: return ECMABoundary;
                case RegexNode.Beginning: return Beginning;
                case RegexNode.Start: return Start;
                case RegexNode.EndZ: return EndZ;
                case RegexNode.End: return End;
                default: return 0;
            }
        }

#if DEBUG
        public static string AnchorDescription(int anchors)
        {
            StringBuilder sb = new StringBuilder();

            if (0 != (anchors & Beginning))
                sb.Append(", Beginning");
            if (0 != (anchors & Start))
                sb.Append(", Start");
            if (0 != (anchors & Bol))
                sb.Append(", Bol");
            if (0 != (anchors & Boundary))
                sb.Append(", Boundary");
            if (0 != (anchors & ECMABoundary))
                sb.Append(", ECMABoundary");
            if (0 != (anchors & Eol))
                sb.Append(", Eol");
            if (0 != (anchors & End))
                sb.Append(", End");
            if (0 != (anchors & EndZ))
                sb.Append(", EndZ");

            if (sb.Length >= 2)
                return (sb.ToString(2, sb.Length - 2));

            return "None";
        }
#endif

        /// <summary>
        /// Return rented buffers. Clear RegexFC buffer manually as only
        /// the reference to the RegexCharClass needs to be cleared.
        /// </summary>
        public void Dispose()
        {
            for (int i = 0; i < _fcStack.Length; i++)
                _fcStack[i].Dispose();

            _fcStack.Dispose();
            _intStack.Dispose();
        }

        /// <summary>
        /// The main FC computation. It does a shortcutted depth-first walk
        /// through the tree and calls CalculateFC to emits code before
        /// and after each child of an interior node, and at each leaf.
        /// </summary>
        private RegexFC? RegexFCFromRegexTree(RegexTree tree)
        {
            RegexNode curNode = tree.Root;
            int curChild = 0;

            for (; ;)
            {
                if (curNode.Children == null)
                {
                    // This is a leaf node
                    CalculateFC(curNode.NType, curNode, 0);
                }
                else if (curChild < curNode.Children.Count && !_skipAllChildren)
                {
                    // This is an interior node, and we have more children to analyze
                    CalculateFC(curNode.NType | BeforeChild, curNode, curChild);

                    if (!_skipchild)
                    {
                        curNode = curNode.Children[curChild];
                        // this stack is how we get a depth first walk of the tree.
                        _intStack.Append(curChild);
                        curChild = 0;
                    }
                    else
                    {
                        curChild++;
                        _skipchild = false;
                    }
                    continue;
                }

                // This is an interior node where we've finished analyzing all the children, or
                // the end of a leaf node.
                _skipAllChildren = false;

                if (_intStack.Length == 0)
                    break;

                curChild = _intStack.Pop();
                curNode = curNode.Next;

                CalculateFC(curNode.NType | AfterChild, curNode, curChild);
                if (_failed)
                    return null;

                curChild++;
            }

            if (_fcStack.Length == 0)
                return null;

            return _fcStack.Pop();
        }

        /// <summary>
        /// Called in Beforechild to prevent further processing of the current child
        /// </summary>
        private void SkipChild()
        {
            _skipchild = true;
        }

        /// <summary>
        /// FC computation and shortcut cases for each node type
        /// </summary>
        private void CalculateFC(int NodeType, RegexNode node, int CurIndex)
        {
            bool ci = false;
            bool rtl = false;

            if (NodeType <= RegexNode.Ref)
            {
                if ((node.Options & RegexOptions.IgnoreCase) != 0)
                    ci = true;
                if ((node.Options & RegexOptions.RightToLeft) != 0)
                    rtl = true;
            }

            switch (NodeType)
            {
                case RegexNode.Concatenate | BeforeChild:
                case RegexNode.Alternate | BeforeChild:
                case RegexNode.Testref | BeforeChild:
                case RegexNode.Loop | BeforeChild:
                case RegexNode.Lazyloop | BeforeChild:
                    break;

                case RegexNode.Testgroup | BeforeChild:
                    if (CurIndex == 0)
                        SkipChild();
                    break;

                case RegexNode.Empty:
                    _fcStack.Append(new RegexFC(true));
                    break;

                case RegexNode.Concatenate | AfterChild:
                    if (CurIndex != 0)
                    {
                        RegexFC child = _fcStack.Pop();
                        RegexFC cumul = _fcStack[_fcStack.Length - 1];

                        _failed = !cumul.AddFC(child, true);
                    }

                    if (!_fcStack[_fcStack.Length - 1]._nullable)
                        _skipAllChildren = true;
                    break;

                case RegexNode.Testgroup | AfterChild:
                    if (CurIndex > 1)
                    {
                        RegexFC child = _fcStack.Pop();
                        RegexFC cumul = _fcStack[_fcStack.Length - 1];

                        _failed = !cumul.AddFC(child, false);
                    }
                    break;

                case RegexNode.Alternate | AfterChild:
                case RegexNode.Testref | AfterChild:
                    if (CurIndex != 0)
                    {
                        RegexFC child = _fcStack.Pop();
                        RegexFC cumul = _fcStack[_fcStack.Length - 1];

                        _failed = !cumul.AddFC(child, false);
                    }
                    break;

                case RegexNode.Loop | AfterChild:
                case RegexNode.Lazyloop | AfterChild:
                    if (node.M == 0)
                        _fcStack[_fcStack.Length - 1]._nullable = true;
                    break;

                case RegexNode.Group | BeforeChild:
                case RegexNode.Group | AfterChild:
                case RegexNode.Capture | BeforeChild:
                case RegexNode.Capture | AfterChild:
                case RegexNode.Greedy | BeforeChild:
                case RegexNode.Greedy | AfterChild:
                    break;

                case RegexNode.Require | BeforeChild:
                case RegexNode.Prevent | BeforeChild:
                    SkipChild();
                    _fcStack.Append(new RegexFC(true));
                    break;

                case RegexNode.Require | AfterChild:
                case RegexNode.Prevent | AfterChild:
                    break;

                case RegexNode.One:
                case RegexNode.Notone:
                    _fcStack.Append(new RegexFC(node.Ch, NodeType == RegexNode.Notone, false, ci));
                    break;

                case RegexNode.Oneloop:
                case RegexNode.Onelazy:
                    _fcStack.Append(new RegexFC(node.Ch, false, node.M == 0, ci));
                    break;

                case RegexNode.Notoneloop:
                case RegexNode.Notonelazy:
                    _fcStack.Append(new RegexFC(node.Ch, true, node.M == 0, ci));
                    break;

                case RegexNode.Multi:
                    if (node.Str.Length == 0)
                        _fcStack.Append(new RegexFC(true));
                    else if (!rtl)
                        _fcStack.Append(new RegexFC(node.Str[0], false, false, ci));
                    else
                        _fcStack.Append(new RegexFC(node.Str[node.Str.Length - 1], false, false, ci));
                    break;

                case RegexNode.Set:
                    _fcStack.Append(new RegexFC(node.Str, false, ci));
                    break;

                case RegexNode.Setloop:
                case RegexNode.Setlazy:
                    _fcStack.Append(new RegexFC(node.Str, node.M == 0, ci));
                    break;

                case RegexNode.Ref:
                    _fcStack.Append(new RegexFC(RegexCharClass.AnyClass, true, false));
                    break;

                case RegexNode.Nothing:
                case RegexNode.Bol:
                case RegexNode.Eol:
                case RegexNode.Boundary:
                case RegexNode.Nonboundary:
                case RegexNode.ECMABoundary:
                case RegexNode.NonECMABoundary:
                case RegexNode.Beginning:
                case RegexNode.Start:
                case RegexNode.EndZ:
                case RegexNode.End:
                    _fcStack.Append(new RegexFC(true));
                    break;

                default:
                    throw new ArgumentException(SR.Format(SR.UnexpectedOpcode, NodeType.ToString(CultureInfo.CurrentCulture)));
            }
        }
    }

    internal struct RegexFC
    {
        private RegexCharClass _cc;
        public bool _nullable;

        public RegexFC(bool nullable)
        {
            _cc = new RegexCharClass();
            _nullable = nullable;
            CaseInsensitive = false;
        }

        public RegexFC(char ch, bool not, bool nullable, bool caseInsensitive)
        {
            _cc = new RegexCharClass();

            if (not)
            {
                if (ch > 0)
                    _cc.AddRange('\0', (char)(ch - 1));
                if (ch < 0xFFFF)
                    _cc.AddRange((char)(ch + 1), '\uFFFF');
            }
            else
            {
                _cc.AddRange(ch, ch);
            }

            CaseInsensitive = caseInsensitive;
            _nullable = nullable;
        }

        public RegexFC(string charClass, bool nullable, bool caseInsensitive)
        {
            _cc = RegexCharClass.Parse(charClass);

            _nullable = nullable;
            CaseInsensitive = caseInsensitive;
        }

        public bool AddFC(RegexFC fc, bool concatenate)
        {
            if (!_cc.CanMerge || !fc._cc.CanMerge)
            {
                return false;
            }

            if (concatenate)
            {
                if (!_nullable)
                    return true;

                if (!fc._nullable)
                    _nullable = false;
            }
            else
            {
                if (fc._nullable)
                    _nullable = true;
            }

            CaseInsensitive |= fc.CaseInsensitive;
            _cc.AddCharClass(fc._cc);
            return true;
        }

        public bool CaseInsensitive { get; private set; }

        public string GetFirstChars(CultureInfo culture)
        {
            if (CaseInsensitive)
                _cc.AddLowercase(culture);

            return _cc.ToStringClass();
        }

        public void Dispose()
        {
            _cc = null;
        }
    }
}
