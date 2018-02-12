// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// This RegexWriter class is internal to the Regex package.
// It builds a block of regular expression codes (RegexCode)
// from a RegexTree parse tree.

// Implementation notes:
//
// This step is as simple as walking the tree and emitting
// sequences of codes.
//

using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace System.Text.RegularExpressions
{
    internal ref struct RegexWriter
    {
        private const int BeforeChild = 64;
        private const int AfterChild = 128;

        public ValueListBuilder<int> Emitted;
        public ValueListBuilder<int> IntStack;
        public readonly Dictionary<string, int> StringHash;
        public readonly List<string> StringTable;
        public Hashtable Caps;
        public int TrackCount { get; private set; }

        private RegexWriter(Span<int> emittedSpan, Span<int> intStackSpan)
        {
            Emitted = new ValueListBuilder<int>(emittedSpan);
            IntStack = new ValueListBuilder<int>(intStackSpan);
            StringHash = new Dictionary<string, int>();
            StringTable = new List<string>();
            Caps = null;
            TrackCount = 0;
        }

        /// <summary>
        /// The top level RegexCode generator. It does a depth-first walk
        /// through the tree and calls EmitFragment to emits code before
        /// and after each child of an interior node, and at each leaf.
        /// </summary>
        public static RegexCode Write(RegexTree tree)
        {
            // Distribution of common patterns indicates an average amount of 56 op codes.
            Span<int> emittedSpan = stackalloc int[56];
            Span<int> intStackSpan = stackalloc int[32];
            RegexWriter writer = new RegexWriter(emittedSpan, intStackSpan);

            // construct sparse capnum mapping if some numbers are unused
            int capsize;
            if (tree._capnumlist == null || tree._captop == tree._capnumlist.Length)
            {
                capsize = tree._captop;
                writer.Caps = null;
            }
            else
            {
                capsize = tree._capnumlist.Length;
                writer.Caps = tree._caps;
                for (int i = 0; i < tree._capnumlist.Length; i++)
                    writer.Caps[tree._capnumlist[i]] = i;
            }

            RegexNode curNode = tree._root;
            int curChild = 0;

            writer.Emit(RegexCode.Lazybranch, 0);

            for (; ; )
            {
                if (curNode._children == null)
                {
                    writer.EmitFragment(curNode._type, curNode, 0);
                }
                else if (curChild < curNode._children.Count)
                {
                    writer.EmitFragment(curNode._type | BeforeChild, curNode, curChild);

                    curNode = curNode._children[curChild];
                    writer.IntStack.Append(curChild);
                    curChild = 0;
                    continue;
                }

                if (writer.IntStack.Length == 0)
                    break;

                curChild = writer.IntStack.Pop();
                curNode = curNode._next;

                writer.EmitFragment(curNode._type | AfterChild, curNode, curChild);
                curChild++;
            }

            writer.PatchJump(0, writer.Emitted.Length);
            writer.Emit(RegexCode.Stop);

            RegexPrefix fcPrefix = RegexFCD.FirstChars(tree);
            RegexPrefix prefix = RegexFCD.Prefix(tree);
            bool rtl = ((tree._options & RegexOptions.RightToLeft) != 0);

            CultureInfo culture = (tree._options & RegexOptions.CultureInvariant) != 0 ? CultureInfo.InvariantCulture : CultureInfo.CurrentCulture;
            RegexBoyerMoore bmPrefix;

            if (prefix != null && prefix.Prefix.Length > 0)
                bmPrefix = new RegexBoyerMoore(prefix.Prefix, prefix.CaseInsensitive, rtl, culture);
            else
                bmPrefix = null;

            int anchors = RegexFCD.Anchors(tree);
            int[] emitted = writer.Emitted.AsReadOnlySpan().ToArray();

            // Cleaning up and returning the borrowed arrays
            writer.Emitted.Dispose();
            writer.IntStack.Dispose();

            var retVal = new RegexCode(emitted, writer.StringTable, writer.TrackCount, writer.Caps, capsize, bmPrefix, fcPrefix, anchors, rtl);

#if DEBUG
            if (tree.Debug)
            {
                tree.Dump();
                retVal.Dump();
            }
#endif

            return retVal;
        }

        /// <summary>
        /// Fixes up a jump instruction at the specified offset
        /// so that it jumps to the specified jumpDest.
        /// </summary>
        private void PatchJump(int offset, int jumpDest)
        {
            Emitted[offset + 1] = jumpDest;
        }

        /// <summary>
        /// Emits a zero-argument operation. Note that the emit
        /// functions all run in two modes: they can emit code, or
        /// they can just count the size of the code.
        /// </summary>
        private void Emit(int op)
        {
            if (RegexCode.OpcodeBacktracks(op))
                TrackCount++;

            Emitted.Append(op);
        }

        /// <summary>
        /// Emits a one-argument operation.
        /// </summary>
        private void Emit(int op, int opd1)
        {
            if (RegexCode.OpcodeBacktracks(op))
                TrackCount++;

            Emitted.Append(op);
            Emitted.Append(opd1);
        }

        /// <summary>
        /// Emits a two-argument operation.
        /// </summary>
        private void Emit(int op, int opd1, int opd2)
        {
            if (RegexCode.OpcodeBacktracks(op))
                TrackCount++;

            Emitted.Append(op);
            Emitted.Append(opd1);
            Emitted.Append(opd2);
        }

        /// <summary>
        /// Returns an index in the string table for a string;
        /// uses a hashtable to eliminate duplicates.
        /// </summary>
        private int StringCode(string str)
        {
            if (str == null)
                str = string.Empty;

            int i;
            if (!StringHash.TryGetValue(str, out i))
            {
                i = StringTable.Count;
                StringHash[str] = i;
                StringTable.Add(str);
            }

            return i;
        }

        /// <summary>
        /// When generating code on a regex that uses a sparse set
        /// of capture slots, we hash them to a dense set of indices
        /// for an array of capture slots. Instead of doing the hash
        /// at match time, it's done at compile time, here.
        /// </summary>
        private int MapCapnum(int capnum)
        {
            if (capnum == -1)
                return -1;

            if (Caps != null)
                return (int)Caps[capnum];
            else
                return capnum;
        }

        /// <summary>
        /// The main RegexCode generator. It does a depth-first walk
        /// through the tree and calls EmitFragment to emits code before
        /// and after each child of an interior node, and at each leaf.
        /// </summary>
        private void EmitFragment(int nodetype, RegexNode node, int curIndex)
        {
            int bits = 0;

            if (nodetype <= RegexNode.Ref)
            {
                if (node.UseOptionR())
                    bits |= RegexCode.Rtl;
                if ((node._options & RegexOptions.IgnoreCase) != 0)
                    bits |= RegexCode.Ci;
            }

            switch (nodetype)
            {
                case RegexNode.Concatenate | BeforeChild:
                case RegexNode.Concatenate | AfterChild:
                case RegexNode.Empty:
                    break;

                case RegexNode.Alternate | BeforeChild:
                    if (curIndex < node._children.Count - 1)
                    {
                        IntStack.Append(Emitted.Length);
                        Emit(RegexCode.Lazybranch, 0);
                    }
                    break;

                case RegexNode.Alternate | AfterChild:
                    {
                        if (curIndex < node._children.Count - 1)
                        {
                            int LBPos = IntStack.Pop();
                            IntStack.Append(Emitted.Length);
                            Emit(RegexCode.Goto, 0);
                            PatchJump(LBPos, Emitted.Length);
                        }
                        else
                        {
                            int I;
                            for (I = 0; I < curIndex; I++)
                            {
                                PatchJump(IntStack.Pop(), Emitted.Length);
                            }
                        }
                        break;
                    }

                case RegexNode.Testref | BeforeChild:
                    switch (curIndex)
                    {
                        case 0:
                            Emit(RegexCode.Setjump);
                            IntStack.Append(Emitted.Length);
                            Emit(RegexCode.Lazybranch, 0);
                            Emit(RegexCode.Testref, MapCapnum(node._m));
                            Emit(RegexCode.Forejump);
                            break;
                    }
                    break;

                case RegexNode.Testref | AfterChild:
                    switch (curIndex)
                    {
                        case 0:
                            {
                                int Branchpos = IntStack.Pop();
                                IntStack.Append(Emitted.Length);
                                Emit(RegexCode.Goto, 0);
                                PatchJump(Branchpos, Emitted.Length);
                                Emit(RegexCode.Forejump);
                                if (node._children.Count > 1)
                                    break;
                                // else fallthrough
                                goto case 1;
                            }
                        case 1:
                            PatchJump(IntStack.Pop(), Emitted.Length);
                            break;
                    }
                    break;

                case RegexNode.Testgroup | BeforeChild:
                    switch (curIndex)
                    {
                        case 0:
                            Emit(RegexCode.Setjump);
                            Emit(RegexCode.Setmark);
                            IntStack.Append(Emitted.Length);
                            Emit(RegexCode.Lazybranch, 0);
                            break;
                    }
                    break;

                case RegexNode.Testgroup | AfterChild:
                    switch (curIndex)
                    {
                        case 0:
                            Emit(RegexCode.Getmark);
                            Emit(RegexCode.Forejump);
                            break;
                        case 1:
                            int Branchpos = IntStack.Pop();
                            IntStack.Append(Emitted.Length);
                            Emit(RegexCode.Goto, 0);
                            PatchJump(Branchpos, Emitted.Length);
                            Emit(RegexCode.Getmark);
                            Emit(RegexCode.Forejump);

                            if (node._children.Count > 2)
                                break;
                            // else fallthrough
                            goto case 2;
                        case 2:
                            PatchJump(IntStack.Pop(), Emitted.Length);
                            break;
                    }
                    break;

                case RegexNode.Loop | BeforeChild:
                case RegexNode.Lazyloop | BeforeChild:

                    if (node._n < int.MaxValue || node._m > 1)
                        Emit(node._m == 0 ? RegexCode.Nullcount : RegexCode.Setcount, node._m == 0 ? 0 : 1 - node._m);
                    else
                        Emit(node._m == 0 ? RegexCode.Nullmark : RegexCode.Setmark);

                    if (node._m == 0)
                    {
                        IntStack.Append(Emitted.Length);
                        Emit(RegexCode.Goto, 0);
                    }
                    IntStack.Append(Emitted.Length);
                    break;

                case RegexNode.Loop | AfterChild:
                case RegexNode.Lazyloop | AfterChild:
                    {
                        int StartJumpPos = Emitted.Length;
                        int Lazy = (nodetype - (RegexNode.Loop | AfterChild));

                        if (node._n < int.MaxValue || node._m > 1)
                            Emit(RegexCode.Branchcount + Lazy, IntStack.Pop(), node._n == int.MaxValue ? int.MaxValue : node._n - node._m);
                        else
                            Emit(RegexCode.Branchmark + Lazy, IntStack.Pop());

                        if (node._m == 0)
                            PatchJump(IntStack.Pop(), StartJumpPos);
                    }
                    break;

                case RegexNode.Group | BeforeChild:
                case RegexNode.Group | AfterChild:
                    break;

                case RegexNode.Capture | BeforeChild:
                    Emit(RegexCode.Setmark);
                    break;

                case RegexNode.Capture | AfterChild:
                    Emit(RegexCode.Capturemark, MapCapnum(node._m), MapCapnum(node._n));
                    break;

                case RegexNode.Require | BeforeChild:
                    // NOTE: the following line causes lookahead/lookbehind to be
                    // NON-BACKTRACKING. It can be commented out with (*)
                    Emit(RegexCode.Setjump);


                    Emit(RegexCode.Setmark);
                    break;

                case RegexNode.Require | AfterChild:
                    Emit(RegexCode.Getmark);

                    // NOTE: the following line causes lookahead/lookbehind to be
                    // NON-BACKTRACKING. It can be commented out with (*)
                    Emit(RegexCode.Forejump);

                    break;

                case RegexNode.Prevent | BeforeChild:
                    Emit(RegexCode.Setjump);
                    IntStack.Append(Emitted.Length);
                    Emit(RegexCode.Lazybranch, 0);
                    break;

                case RegexNode.Prevent | AfterChild:
                    Emit(RegexCode.Backjump);
                    PatchJump(IntStack.Pop(), Emitted.Length);
                    Emit(RegexCode.Forejump);
                    break;

                case RegexNode.Greedy | BeforeChild:
                    Emit(RegexCode.Setjump);
                    break;

                case RegexNode.Greedy | AfterChild:
                    Emit(RegexCode.Forejump);
                    break;

                case RegexNode.One:
                case RegexNode.Notone:
                    Emit(node._type | bits, node._ch);
                    break;

                case RegexNode.Notoneloop:
                case RegexNode.Notonelazy:
                case RegexNode.Oneloop:
                case RegexNode.Onelazy:
                    if (node._m > 0)
                        Emit(((node._type == RegexNode.Oneloop || node._type == RegexNode.Onelazy) ?
                              RegexCode.Onerep : RegexCode.Notonerep) | bits, node._ch, node._m);
                    if (node._n > node._m)
                        Emit(node._type | bits, node._ch, node._n == int.MaxValue ?
                             int.MaxValue : node._n - node._m);
                    break;

                case RegexNode.Setloop:
                case RegexNode.Setlazy:
                    if (node._m > 0)
                        Emit(RegexCode.Setrep | bits, StringCode(node._str), node._m);
                    if (node._n > node._m)
                        Emit(node._type | bits, StringCode(node._str),
                             (node._n == int.MaxValue) ? int.MaxValue : node._n - node._m);
                    break;

                case RegexNode.Multi:
                    Emit(node._type | bits, StringCode(node._str));
                    break;

                case RegexNode.Set:
                    Emit(node._type | bits, StringCode(node._str));
                    break;

                case RegexNode.Ref:
                    Emit(node._type | bits, MapCapnum(node._m));
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
                    Emit(node._type);
                    break;

                default:
                    throw new ArgumentException(SR.Format(SR.UnexpectedOpcode, nodetype.ToString(CultureInfo.CurrentCulture)));
            }
        }
    }
}
