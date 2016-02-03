// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// This RegexNode class is internal to the Regex package.
// It is built into a parsed tree for a regular expression.

// Implementation notes:
//
// Since the node tree is a temporary data structure only used
// during compilation of the regexp to integer codes, it's
// designed for clarity and convenience rather than
// space efficiency.
//
// RegexNodes are built into a tree, linked by the _children list.
// Each node also has a _parent and _ichild member indicating
// its parent and which child # it is in its parent's list.
//
// RegexNodes come in as many types as there are constructs in
// a regular expression, for example, "concatenate", "alternate",
// "one", "rept", "group". There are also node types for basic
// peephole optimizations, e.g., "onerep", "notsetrep", etc.
//
// Because perl 5 allows "lookback" groups that scan backwards,
// each node also gets a "direction". Normally the value of
// boolean _backward = false.
//
// During parsing, top-level nodes are also stacked onto a parse
// stack (a stack of trees). For this purpose we have a _next
// pointer. [Note that to save a few bytes, we could overload the
// _parent pointer instead.]
//
// On the parse stack, each tree has a "role" - basically, the
// nonterminal in the grammar that the parser has currently
// assigned to the tree. That code is stored in _role.
//
// Finally, some of the different kinds of nodes have data.
// Two integers (for the looping constructs) are stored in
// _operands, an an object (either a string or a set)
// is stored in _data

using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace System.Text.RegularExpressions
{
    internal sealed class RegexNode
    {
        // RegexNode types

        // The following are leaves, and correspond to primitive operations

        internal const int Oneloop = RegexCode.Oneloop;                 // c,n      a*
        internal const int Notoneloop = RegexCode.Notoneloop;           // c,n      .*
        internal const int Setloop = RegexCode.Setloop;                 // set,n    \d*

        internal const int Onelazy = RegexCode.Onelazy;                 // c,n      a*?
        internal const int Notonelazy = RegexCode.Notonelazy;           // c,n      .*?
        internal const int Setlazy = RegexCode.Setlazy;                 // set,n    \d*?

        internal const int One = RegexCode.One;                         // char     a
        internal const int Notone = RegexCode.Notone;                   // char     . [^a]
        internal const int Set = RegexCode.Set;                         // set      [a-z] \w \s \d

        internal const int Multi = RegexCode.Multi;                     // string   abcdef
        internal const int Ref = RegexCode.Ref;                         // index    \1

        internal const int Bol = RegexCode.Bol;                         //          ^
        internal const int Eol = RegexCode.Eol;                         //          $
        internal const int Boundary = RegexCode.Boundary;               //          \b
        internal const int Nonboundary = RegexCode.Nonboundary;         //          \B
        internal const int ECMABoundary = RegexCode.ECMABoundary;       // \b
        internal const int NonECMABoundary = RegexCode.NonECMABoundary; // \B
        internal const int Beginning = RegexCode.Beginning;             //          \A
        internal const int Start = RegexCode.Start;                     //          \G
        internal const int EndZ = RegexCode.EndZ;                       //          \Z
        internal const int End = RegexCode.End;                         //          \z

        // Interior nodes do not correspond to primitive operations, but
        // control structures compositing other operations

        // Concat and alternate take n children, and can run forward or backwards

        internal const int Nothing = 22;                                //          []
        internal const int Empty = 23;                                  //          ()

        internal const int Alternate = 24;                              //          a|b
        internal const int Concatenate = 25;                            //          ab

        internal const int Loop = 26;                                   // m,x      * + ? {,}
        internal const int Lazyloop = 27;                               // m,x      *? +? ?? {,}?

        internal const int Capture = 28;                                // n        ()
        internal const int Group = 29;                                  //          (?:)
        internal const int Require = 30;                                //          (?=) (?<=)
        internal const int Prevent = 31;                                //          (?!) (?<!)
        internal const int Greedy = 32;                                 //          (?>) (?<)
        internal const int Testref = 33;                                //          (?(n) | )
        internal const int Testgroup = 34;                              //          (?(...) | )

        // RegexNode data members

        internal int _type;

        internal List<RegexNode> _children;

        internal String _str;
        internal char _ch;
        internal int _m;
        internal int _n;
        internal readonly RegexOptions _options;

        internal RegexNode _next;

        internal RegexNode(int type, RegexOptions options)
        {
            _type = type;
            _options = options;
        }

        internal RegexNode(int type, RegexOptions options, char ch)
        {
            _type = type;
            _options = options;
            _ch = ch;
        }

        internal RegexNode(int type, RegexOptions options, String str)
        {
            _type = type;
            _options = options;
            _str = str;
        }

        internal RegexNode(int type, RegexOptions options, int m)
        {
            _type = type;
            _options = options;
            _m = m;
        }

        internal RegexNode(int type, RegexOptions options, int m, int n)
        {
            _type = type;
            _options = options;
            _m = m;
            _n = n;
        }

        internal bool UseOptionR()
        {
            return (_options & RegexOptions.RightToLeft) != 0;
        }

        internal RegexNode ReverseLeft()
        {
            if (UseOptionR() && _type == Concatenate && _children != null)
            {
                _children.Reverse(0, _children.Count);
            }

            return this;
        }

        /// <summary>
        /// Pass type as OneLazy or OneLoop
        /// </summary>
        internal void MakeRep(int type, int min, int max)
        {
            _type += (type - One);
            _m = min;
            _n = max;
        }

        /// <summary>
        /// Removes redundant nodes from the subtree, and returns a reduced subtree.
        /// </summary>
        internal RegexNode Reduce()
        {
            RegexNode n;

            switch (Type())
            {
                case Alternate:
                    n = ReduceAlternation();
                    break;

                case Concatenate:
                    n = ReduceConcatenation();
                    break;

                case Loop:
                case Lazyloop:
                    n = ReduceRep();
                    break;

                case Group:
                    n = ReduceGroup();
                    break;

                case Set:
                case Setloop:
                    n = ReduceSet();
                    break;

                default:
                    n = this;
                    break;
            }

            return n;
        }

        /// <summary>
        /// Simple optimization. If a concatenation or alternation has only
        /// one child strip out the intermediate node. If it has zero children,
        /// turn it into an empty.
        /// </summary>
        internal RegexNode StripEnation(int emptyType)
        {
            switch (ChildCount())
            {
                case 0:
                    return new RegexNode(emptyType, _options);
                case 1:
                    return Child(0);
                default:
                    return this;
            }
        }

        /// <summary>
        /// Simple optimization. Once parsed into a tree, noncapturing groups
        /// serve no function, so strip them out.
        /// </summary>
        internal RegexNode ReduceGroup()
        {
            RegexNode u;

            for (u = this; u.Type() == Group;)
                u = u.Child(0);

            return u;
        }

        /// <summary>
        /// Nested repeaters just get multiplied with each other if they're not
        /// too lumpy
        /// </summary>
        internal RegexNode ReduceRep()
        {
            RegexNode u;
            RegexNode child;
            int type;
            int min;
            int max;

            u = this;
            type = Type();
            min = _m;
            max = _n;

            for (; ;)
            {
                if (u.ChildCount() == 0)
                    break;

                child = u.Child(0);

                // multiply reps of the same type only
                if (child.Type() != type)
                {
                    int childType = child.Type();

                    if (!(childType >= Oneloop && childType <= Setloop && type == Loop ||
                          childType >= Onelazy && childType <= Setlazy && type == Lazyloop))
                        break;
                }

                // child can be too lumpy to blur, e.g., (a {100,105}) {3} or (a {2,})?
                // [but things like (a {2,})+ are not too lumpy...]
                if (u._m == 0 && child._m > 1 || child._n < child._m * 2)
                    break;

                u = child;
                if (u._m > 0)
                    u._m = min = ((Int32.MaxValue - 1) / u._m < min) ? Int32.MaxValue : u._m * min;
                if (u._n > 0)
                    u._n = max = ((Int32.MaxValue - 1) / u._n < max) ? Int32.MaxValue : u._n * max;
            }

            return min == Int32.MaxValue ? new RegexNode(Nothing, _options) : u;
        }

        /// <summary>
        /// Simple optimization. If a set is a singleton, an inverse singleton,
        /// or empty, it's transformed accordingly.
        /// </summary>
        internal RegexNode ReduceSet()
        {
            // Extract empty-set, one and not-one case as special

            if (RegexCharClass.IsEmpty(_str))
            {
                _type = Nothing;
                _str = null;
            }
            else if (RegexCharClass.IsSingleton(_str))
            {
                _ch = RegexCharClass.SingletonChar(_str);
                _str = null;
                _type += (One - Set);
            }
            else if (RegexCharClass.IsSingletonInverse(_str))
            {
                _ch = RegexCharClass.SingletonChar(_str);
                _str = null;
                _type += (Notone - Set);
            }

            return this;
        }

        /// <summary>
        /// Basic optimization. Single-letter alternations can be replaced
        /// by faster set specifications, and nested alternations with no
        /// intervening operators can be flattened:
        ///
        /// a|b|c|def|g|h -> [a-c]|def|[gh]
        /// apple|(?:orange|pear)|grape -> apple|orange|pear|grape
        /// </summary>
        internal RegexNode ReduceAlternation()
        {
            // Combine adjacent sets/chars

            bool wasLastSet;
            bool lastNodeCannotMerge;
            RegexOptions optionsLast;
            RegexOptions optionsAt;
            int i;
            int j;
            RegexNode at;
            RegexNode prev;

            if (_children == null)
                return new RegexNode(RegexNode.Nothing, _options);

            wasLastSet = false;
            lastNodeCannotMerge = false;
            optionsLast = 0;

            for (i = 0, j = 0; i < _children.Count; i++, j++)
            {
                at = _children[i];

                if (j < i)
                    _children[j] = at;

                for (; ;)
                {
                    if (at._type == Alternate)
                    {
                        for (int k = 0; k < at._children.Count; k++)
                            at._children[k]._next = this;

                        _children.InsertRange(i + 1, at._children);
                        j--;
                    }
                    else if (at._type == Set || at._type == One)
                    {
                        // Cannot merge sets if L or I options differ, or if either are negated.
                        optionsAt = at._options & (RegexOptions.RightToLeft | RegexOptions.IgnoreCase);


                        if (at._type == Set)
                        {
                            if (!wasLastSet || optionsLast != optionsAt || lastNodeCannotMerge || !RegexCharClass.IsMergeable(at._str))
                            {
                                wasLastSet = true;
                                lastNodeCannotMerge = !RegexCharClass.IsMergeable(at._str);
                                optionsLast = optionsAt;
                                break;
                            }
                        }
                        else if (!wasLastSet || optionsLast != optionsAt || lastNodeCannotMerge)
                        {
                            wasLastSet = true;
                            lastNodeCannotMerge = false;
                            optionsLast = optionsAt;
                            break;
                        }


                        // The last node was a Set or a One, we're a Set or One and our options are the same.
                        // Merge the two nodes.
                        j--;
                        prev = _children[j];

                        RegexCharClass prevCharClass;
                        if (prev._type == RegexNode.One)
                        {
                            prevCharClass = new RegexCharClass();
                            prevCharClass.AddChar(prev._ch);
                        }
                        else
                        {
                            prevCharClass = RegexCharClass.Parse(prev._str);
                        }

                        if (at._type == RegexNode.One)
                        {
                            prevCharClass.AddChar(at._ch);
                        }
                        else
                        {
                            RegexCharClass atCharClass = RegexCharClass.Parse(at._str);
                            prevCharClass.AddCharClass(atCharClass);
                        }

                        prev._type = RegexNode.Set;
                        prev._str = prevCharClass.ToStringClass();
                    }
                    else if (at._type == RegexNode.Nothing)
                    {
                        j--;
                    }
                    else
                    {
                        wasLastSet = false;
                        lastNodeCannotMerge = false;
                    }
                    break;
                }
            }

            if (j < i)
                _children.RemoveRange(j, i - j);

            return StripEnation(RegexNode.Nothing);
        }

        /// <summary>
        /// Basic optimization. Adjacent strings can be concatenated.
        ///
        /// (?:abc)(?:def) -> abcdef
        /// </summary>
        internal RegexNode ReduceConcatenation()
        {
            // Eliminate empties and concat adjacent strings/chars

            bool wasLastString;
            RegexOptions optionsLast;
            RegexOptions optionsAt;
            int i;
            int j;

            if (_children == null)
                return new RegexNode(RegexNode.Empty, _options);

            wasLastString = false;
            optionsLast = 0;

            for (i = 0, j = 0; i < _children.Count; i++, j++)
            {
                RegexNode at;
                RegexNode prev;

                at = _children[i];

                if (j < i)
                    _children[j] = at;

                if (at._type == RegexNode.Concatenate &&
                    ((at._options & RegexOptions.RightToLeft) == (_options & RegexOptions.RightToLeft)))
                {
                    for (int k = 0; k < at._children.Count; k++)
                        at._children[k]._next = this;

                    _children.InsertRange(i + 1, at._children);
                    j--;
                }
                else if (at._type == RegexNode.Multi ||
                         at._type == RegexNode.One)
                {
                    // Cannot merge strings if L or I options differ
                    optionsAt = at._options & (RegexOptions.RightToLeft | RegexOptions.IgnoreCase);

                    if (!wasLastString || optionsLast != optionsAt)
                    {
                        wasLastString = true;
                        optionsLast = optionsAt;
                        continue;
                    }

                    prev = _children[--j];

                    if (prev._type == RegexNode.One)
                    {
                        prev._type = RegexNode.Multi;
                        prev._str = Convert.ToString(prev._ch, CultureInfo.InvariantCulture);
                    }

                    if ((optionsAt & RegexOptions.RightToLeft) == 0)
                    {
                        if (at._type == RegexNode.One)
                            prev._str += at._ch.ToString();
                        else
                            prev._str += at._str;
                    }
                    else
                    {
                        if (at._type == RegexNode.One)
                            prev._str = at._ch.ToString() + prev._str;
                        else
                            prev._str = at._str + prev._str;
                    }
                }
                else if (at._type == RegexNode.Empty)
                {
                    j--;
                }
                else
                {
                    wasLastString = false;
                }
            }

            if (j < i)
                _children.RemoveRange(j, i - j);

            return StripEnation(RegexNode.Empty);
        }

        internal RegexNode MakeQuantifier(bool lazy, int min, int max)
        {
            RegexNode result;

            if (min == 0 && max == 0)
                return new RegexNode(RegexNode.Empty, _options);

            if (min == 1 && max == 1)
                return this;

            switch (_type)
            {
                case RegexNode.One:
                case RegexNode.Notone:
                case RegexNode.Set:

                    MakeRep(lazy ? RegexNode.Onelazy : RegexNode.Oneloop, min, max);
                    return this;

                default:
                    result = new RegexNode(lazy ? RegexNode.Lazyloop : RegexNode.Loop, _options, min, max);
                    result.AddChild(this);
                    return result;
            }
        }

        internal void AddChild(RegexNode newChild)
        {
            RegexNode reducedChild;

            if (_children == null)
                _children = new List<RegexNode>(4);

            reducedChild = newChild.Reduce();

            _children.Add(reducedChild);
            reducedChild._next = this;
        }
        internal RegexNode Child(int i)
        {
            return _children[i];
        }

        internal int ChildCount()
        {
            return _children == null ? 0 : _children.Count;
        }

        internal int Type()
        {
            return _type;
        }

#if DEBUG
        internal static String[] TypeStr = new String[] {
            "Onerep", "Notonerep", "Setrep",
            "Oneloop", "Notoneloop", "Setloop",
            "Onelazy", "Notonelazy", "Setlazy",
            "One", "Notone", "Set",
            "Multi", "Ref",
            "Bol", "Eol", "Boundary", "Nonboundary",
            "ECMABoundary", "NonECMABoundary",
            "Beginning", "Start", "EndZ", "End",
            "Nothing", "Empty",
            "Alternate", "Concatenate",
            "Loop", "Lazyloop",
            "Capture", "Group", "Require", "Prevent", "Greedy",
            "Testref", "Testgroup"};

        internal String Description()
        {
            StringBuilder ArgSb = new StringBuilder();

            ArgSb.Append(TypeStr[_type]);

            if ((_options & RegexOptions.ExplicitCapture) != 0)
                ArgSb.Append("-C");
            if ((_options & RegexOptions.IgnoreCase) != 0)
                ArgSb.Append("-I");
            if ((_options & RegexOptions.RightToLeft) != 0)
                ArgSb.Append("-L");
            if ((_options & RegexOptions.Multiline) != 0)
                ArgSb.Append("-M");
            if ((_options & RegexOptions.Singleline) != 0)
                ArgSb.Append("-S");
            if ((_options & RegexOptions.IgnorePatternWhitespace) != 0)
                ArgSb.Append("-X");
            if ((_options & RegexOptions.ECMAScript) != 0)
                ArgSb.Append("-E");

            switch (_type)
            {
                case Oneloop:
                case Notoneloop:
                case Onelazy:
                case Notonelazy:
                case One:
                case Notone:
                    ArgSb.Append("(Ch = " + RegexCharClass.CharDescription(_ch) + ")");
                    break;
                case Capture:
                    ArgSb.Append("(index = " + _m.ToString(CultureInfo.InvariantCulture) + ", unindex = " + _n.ToString(CultureInfo.InvariantCulture) + ")");
                    break;
                case Ref:
                case Testref:
                    ArgSb.Append("(index = " + _m.ToString(CultureInfo.InvariantCulture) + ")");
                    break;
                case Multi:
                    ArgSb.Append("(String = " + _str + ")");
                    break;
                case Set:
                case Setloop:
                case Setlazy:
                    ArgSb.Append("(Set = " + RegexCharClass.SetDescription(_str) + ")");
                    break;
            }

            switch (_type)
            {
                case Oneloop:
                case Notoneloop:
                case Onelazy:
                case Notonelazy:
                case Setloop:
                case Setlazy:
                case Loop:
                case Lazyloop:
                    ArgSb.Append("(Min = " + _m.ToString(CultureInfo.InvariantCulture) + ", Max = " + (_n == Int32.MaxValue ? "inf" : Convert.ToString(_n, CultureInfo.InvariantCulture)) + ")");
                    break;
            }

            return ArgSb.ToString();
        }

        internal const String Space = "                                ";

        internal void Dump()
        {
            List<int> Stack = new List<int>();
            RegexNode CurNode;
            int CurChild;

            CurNode = this;
            CurChild = 0;

            Debug.WriteLine(CurNode.Description());

            for (; ;)
            {
                if (CurNode._children != null && CurChild < CurNode._children.Count)
                {
                    Stack.Add(CurChild + 1);
                    CurNode = CurNode._children[CurChild];
                    CurChild = 0;

                    int Depth = Stack.Count;
                    if (Depth > 32)
                        Depth = 32;

                    Debug.WriteLine(Space.Substring(0, Depth) + CurNode.Description());
                }
                else
                {
                    if (Stack.Count == 0)
                        break;

                    CurChild = Stack[Stack.Count - 1];
                    Stack.RemoveAt(Stack.Count - 1);
                    CurNode = CurNode._next;
                }
            }
        }
#endif
    }
}
