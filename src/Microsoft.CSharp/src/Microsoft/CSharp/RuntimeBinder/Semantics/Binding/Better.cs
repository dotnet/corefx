// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.CSharp.RuntimeBinder.Syntax;

namespace Microsoft.CSharp.RuntimeBinder.Semantics
{
    internal partial class ExpressionBinder
    {
        ////////////////////////////////////////////////////////////////////////////////
        // This table is used to implement the last set of 'better' conversion rules
        // when there are no implicit conversions between T1(down) and T2 (across)
        // Use all the simple types plus 1 more for Object
        // See CLR section 7.4.1.3

        static private readonly byte[][] s_betterConversionTable =
        {
            //          BYTE    SHORT   INT     LONG    FLOAT   DOUBLE  DECIMAL CHAR    BOOL    SBYTE   USHORT  UINT    ULONG   IPTR     UIPTR    OBJECT
            new byte[] /* BYTE*/   {0,     0,      0,      0,      0,      0,      0,      0,      0,      2,      0,      0,      0,      0,       0,       0},
            new byte[] /* SHORT*/  {0,     0,      0,      0,      0,      0,      0,      0,      0,      0,      1,      1,      1,      0,       0,       0},
            new byte[] /* INT*/    {0,     0,      0,      0,      0,      0,      0,      0,      0,      0,      0,      1,      1,      0,       0,       0},
            new byte[] /* LONG*/   {0,     0,      0,      0,      0,      0,      0,      0,      0,      0,      0,      0,      1,      0,       0,       0},
            new byte[] /* FLOAT*/  {0,     0,      0,      0,      0,      0,      0,      0,      0,      0,      0,      0,      0,      0,       0,       0},
            new byte[] /* DOUBLE*/ {0,     0,      0,      0,      0,      0,      0,      0,      0,      0,      0,      0,      0,      0,       0,       0},
            new byte[] /* DECIMAL*/{0,     0,      0,      0,      0,      0,      0,      0,      0,      0,      0,      0,      0,      0,       0,       0},
            new byte[] /* CHAR*/   {0,     0,      0,      0,      0,      0,      0,      0,      0,      0,      0,      0,      0,      0,       0,       0},
            new byte[] /* BOOL*/   {0,     0,      0,      0,      0,      0,      0,      0,      0,      0,      0,      0,      0,      0,       0,       0},
            new byte[] /* SBYTE*/  {1,     0,      0,      0,      0,      0,      0,      0,      0,      0,      1,      1,      1,      0,       0,       0},
            new byte[] /* USHORT*/ {0,     2,      0,      0,      0,      0,      0,      0,      0,      2,      0,      0,      0,      0,       0,       0},
            new byte[] /* UINT*/   {0,     2,      2,      0,      0,      0,      0,      0,      0,      2,      0,      0,      0,      0,       0,       0},
            new byte[] /* ULONG*/  {0,     2,      2,      2,      0,      0,      0,      0,      0,      2,      0,      0,      0,      0,       0,       0},
            new byte[] /* IPTR*/   {0,     0,      0,      0,      0,      0,      0,      0,      0,      0,      0,      0,      0,      0,       0,       0},
            new byte[] /* UIPTR*/  {0,     0,      0,      0,      0,      0,      0,      0,      0,      0,      0,      0,      0,      0,       0,       0},
            new byte[] /* OBJECT*/ {0,     0,      0,      0,      0,      0,      0,      0,      0,      0,      0,      0,      0,      0,       0,       0}
        };

        protected BetterType WhichMethodIsBetterTieBreaker(
            CandidateFunctionMember node1,
            CandidateFunctionMember node2,
            CType pTypeThrough,
            ArgInfos args)
        {
            MethPropWithInst mpwi1 = node1.mpwi;
            MethPropWithInst mpwi2 = node2.mpwi;

            // Same signatures. If they have different lifting numbers, the smaller number wins.
            // Otherwise, if one is generic and the other isn't then the non-generic wins.
            // Otherwise, if one is expanded and the other isn't then the non-expanded wins.
            // Otherwise, if one has fewer modopts than the other then it wins.
            if (node1.ctypeLift != node2.ctypeLift)
            {
                return node1.ctypeLift < node2.ctypeLift ? BetterType.Left : BetterType.Right;
            }

            // Non-generic wins.
            if (mpwi1.TypeArgs.size != 0)
            {
                if (mpwi2.TypeArgs.size == 0)
                {
                    return BetterType.Right;
                }
            }
            else if (mpwi2.TypeArgs.size != 0)
            {
                return BetterType.Left;
            }

            // Non-expanded wins
            if (node1.fExpanded)
            {
                if (!node2.fExpanded)
                {
                    return BetterType.Right;
                }
            }
            else if (node2.fExpanded)
            {
                return BetterType.Left;
            }

            // See if one's parameter types (un-instantiated) are more specific.
            BetterType nT = GetGlobalSymbols().CompareTypes(
               RearrangeNamedArguments(mpwi1.MethProp().Params, mpwi1, pTypeThrough, args),
               RearrangeNamedArguments(mpwi2.MethProp().Params, mpwi2, pTypeThrough, args));
            if (nT == BetterType.Left || nT == BetterType.Right)
            {
                return nT;
            }

            // Fewer modopts wins.
            if (mpwi1.MethProp().modOptCount != mpwi2.MethProp().modOptCount)
            {
                return mpwi1.MethProp().modOptCount < mpwi2.MethProp().modOptCount ? BetterType.Left : BetterType.Right;
            }

            // Bona-fide tie.
            return BetterType.Neither;
        }

        ////////////////////////////////////////////////////////////////////////////////

        // Find the index of a name on a list.
        // There is no failure case; we require that the name actually
        // be on the list

        private static int FindName(List<Name> names, Name name)
        {
            int index = names.IndexOf(name);
            Debug.Assert(index != -1);
            return index;
        }

        ////////////////////////////////////////////////////////////////////////////////
        // We need to rearange the method parameters so that the type of any specified named argument
        // appears in the same place as the named argument. Consider the example below:
        //    Foo(int x = 4, string y = "", long l = 4)
        //    Foo(string y = "", string x="", long l = 5)
        // and the call site:
        //    Foo(y:"a")
        // After rearranging the parameter types we will have:
        //   (string, int, long) and (string, string, long)
        // By rearranging the arguments as such we make sure that any specified named arguments appear in the same position for both
        // methods and we also maintain the relative order of the other parameters (the type long appears after int in the above example)

        private TypeArray RearrangeNamedArguments(TypeArray pta, MethPropWithInst mpwi,
            CType pTypeThrough, ArgInfos args)
        {
            if (!args.fHasExprs)
            {
                return pta;
            }

#if DEBUG
            // We never have a named argument that is in a position in the argument
            // list past the end of what would be the formal parameter list.
            for (int i = pta.size; i < args.carg; i++)
            {
                Debug.Assert(!args.prgexpr[i].isNamedArgumentSpecification());
            }
#endif

            CType type = pTypeThrough != null ? pTypeThrough : mpwi.GetType();
            CType[] typeList = new CType[pta.size];
            MethodOrPropertySymbol methProp = GroupToArgsBinder.FindMostDerivedMethod(GetSymbolLoader(), mpwi.MethProp(), type);

            // We initialize the new type array with the parameters for the method. 
            for (int iParam = 0; iParam < pta.size; iParam++)
            {
                typeList[iParam] = pta.Item(iParam);
            }

            // We then go over the specified arguments and put the type for any named argument in the right position in the array.
            for (int iParam = 0; iParam < args.carg; iParam++)
            {
                EXPR arg = args.prgexpr[iParam];
                if (arg.isNamedArgumentSpecification())
                {
                    // We find the index of the type of the argument in the method parameter list and store that in a temp
                    int index = FindName(methProp.ParameterNames, arg.asNamedArgumentSpecification().Name);
                    CType tempType = pta.Item(index);

                    // Starting from the current position in the type list up until the location of the type of the optional argument
                    //  We shift types by one:
                    //   before: (int, string, long)
                    //   after: (string, int, long)
                    // We only touch the types between the current position and the position of the type we need to move
                    for (int iShift = iParam; iShift < index; iShift++)
                    {
                        typeList[iShift + 1] = typeList[iShift];
                    }

                    typeList[iParam] = tempType;
                }
            }

            return GetSymbolLoader().getBSymmgr().AllocParams(pta.size, typeList);
        }

        ////////////////////////////////////////////////////////////////////////////////
        // Determine which method is better for the purposes of overload resolution.
        // Better means: as least as good in all params, and better in at least one param.
        // Better w/r to a param means is an ordering, from best down:
        // 1) same type as argument
        // 2) implicit conversion from argument to formal type
        // Because of user defined conversion opers this relation is not transitive.
        //
        // If there is a tie because of identical signatures, the tie may be broken by the
        // following rules:
        // 1) If one is generic and the other isn't, the non-generic wins.
        // 2) Otherwise if one is expanded (params) and the other isn't, the non-expanded wins.
        // 3) Otherwise if one has more specific parameter types (at the declaration) it wins:
        //    This occurs if at least on parameter type is more specific and no parameter type is
        //    less specific.
        //* A type parameter is less specific than a non-type parameter.
        //* A constructed type is more specific than another constructed type if at least
        //      one type argument is more specific and no type argument is less specific than
        //      the corresponding type args in the other.
        // 4) Otherwise if one has more modopts than the other does, the smaller number of modopts wins.
        //
        // Returns Left if m1 is better, Right if m2 is better, or Neither/Same

        protected BetterType WhichMethodIsBetter(
            CandidateFunctionMember node1,
            CandidateFunctionMember node2,
            CType pTypeThrough,
            ArgInfos args)
        {
            MethPropWithInst mpwi1 = node1.mpwi;
            MethPropWithInst mpwi2 = node2.mpwi;

            // Substitutions should have already been done on these!
            TypeArray pta1 = RearrangeNamedArguments(node1.@params, mpwi1, pTypeThrough, args);
            TypeArray pta2 = RearrangeNamedArguments(node2.@params, mpwi2, pTypeThrough, args);

            // If the parameter types for both candidate methods are identical,
            // use the tie breaking rules.

            if (pta1 == pta2)
            {
                return WhichMethodIsBetterTieBreaker(node1, node2, pTypeThrough, args);
            }

            //  Otherwise, do a parameter-by-parameter comparison:
            //
            // Given an argument list A with a set of argument expressions {E1, ... En} and
            // two applicable function members Mp and Mq with parameter types {P1,... Pn} and
            // {Q1, ... Qn}, Mp is defined to be a better function member than Mq if:
            //* for each argument the implicit conversion from Ex to Qx is not better than
            //   the implicit conversion from Ex to Px.
            //* for at least one argument, the conversion from Ex to Px is better than the
            //   conversion from Ex to Qx.

            BetterType betterMethod = BetterType.Neither;
            CType type1 = pTypeThrough != null ? pTypeThrough : mpwi1.GetType();
            CType type2 = pTypeThrough != null ? pTypeThrough : mpwi2.GetType();
            MethodOrPropertySymbol methProp1 = ExpressionBinder.GroupToArgsBinder.FindMostDerivedMethod(GetSymbolLoader(), mpwi1.MethProp(), type1);
            MethodOrPropertySymbol methProp2 = ExpressionBinder.GroupToArgsBinder.FindMostDerivedMethod(GetSymbolLoader(), mpwi2.MethProp(), type2);
            List<Name> names1 = methProp1.ParameterNames;
            List<Name> names2 = methProp2.ParameterNames;

            for (int i = 0; i < args.carg; i++)
            {
                EXPR arg = args.fHasExprs ? args.prgexpr[i] : null;
                CType argType = args.types.Item(i);
                CType p1 = pta1.Item(i);
                CType p2 = pta2.Item(i);

                // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                // RUNTIME BINDER ONLY CHANGE
                // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //
                // We need to consider conversions from the actual runtime type
                // since we could have private interfaces that we are converting

                if (arg.RuntimeObjectActualType != null)
                {
                    argType = arg.RuntimeObjectActualType;
                }

                // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                // END RUNTIME BINDER ONLY CHANGE
                // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                BetterType betterConversion = WhichConversionIsBetter(arg, argType, p1, p2);

                if (betterMethod == BetterType.Right && betterConversion == BetterType.Left)
                {
                    betterMethod = BetterType.Neither;
                    break;
                }
                else if (betterMethod == BetterType.Left && betterConversion == BetterType.Right)
                {
                    betterMethod = BetterType.Neither;
                    break;
                }
                else if (betterMethod == BetterType.Neither)
                {
                    if (betterConversion == BetterType.Right || betterConversion == BetterType.Left)
                    {
                        betterMethod = betterConversion;
                    }
                }
            }

            // We may have different sizes if we had optional parameters. If thats the case,
            // the one with fewer parameters wins (ie less optional parameters) unless it is
            // expanded. If so, the one with more parameters wins (ie option beats expanded).
            if (pta1.size != pta2.size && betterMethod == BetterType.Neither)
            {
                if (node1.fExpanded && !node2.fExpanded)
                {
                    return BetterType.Right;
                }
                else if (node2.fExpanded && !node1.fExpanded)
                {
                    return BetterType.Left;
                }

                // Here, if both methods needed to use optionals to fill in the signatures,
                // then we are ambiguous. Otherwise, take the one that didn't need any 
                // optionals.

                if (pta1.size == args.carg)
                {
                    return BetterType.Left;
                }
                else if (pta2.size == args.carg)
                {
                    return BetterType.Right;
                }
                return BetterType.Neither;
            }

            return betterMethod;
        }

        protected BetterType WhichConversionIsBetter(EXPR arg, CType argType,
            CType p1, CType p2)
        {
            Debug.Assert(argType != null);
            Debug.Assert(p1 != null);
            Debug.Assert(p2 != null);

            // 7.4.2.3 Better Conversion From Expression
            //
            // Given an implicit  conversion C1 that converts from an expression E to a type T1
            // and an implicit conversion C2 that converts from an expression E to a type T2, the
            // better conversion of the two conversions is determined as follows:
            //* if T1 and T2 are the same type, neither conversion is better.
            //* If E has a type S and the conversion from S to T1 is better than the conversion from
            //   S to T2 then C1 is the better conversion.
            //* If E has a type S and the conversion from S to T2 is better than the conversion from
            //   S to T1 then C2 is the better conversion.
            //* If E is a lambda expression or anonymous method for which an inferred return type X
            //   exists and T1 is a delegate type and T2 is a delegate type and T1 and T2 have identical
            //   parameter lists:
            //  * If T1 is a delegate of return type Y1 and T2 is a delegate of return type Y2 and the
            //     conversion from X to Y1 is better than the conversion from X to Y2, then C1 is the
            //     better return type.
            //  * If T1 is a delegate of return type Y1 and T2 is a delegate of return type Y2 and the
            //     conversion from X to Y2 is better than the conversion from X to Y1, then C2 is the
            //     better return type.

            if (p1 == p2)
            {
                return BetterType.Same;
            }
            return WhichConversionIsBetter(argType, p1, p2);
        }

        public BetterType WhichConversionIsBetter(CType argType,
            CType p1, CType p2)
        {
            // 7.4.2.4 Better conversion from type
            //
            // Given a conversion C1 that converts from a type S to a type T1 and a conversion C2
            // that converts from a type S to a type T2, the better conversion of the two conversions
            // is determined as follows:
            //* If T1 and T2 are the same type, neither conversion is better
            //* If S is T1, C1 is the better conversion.
            //* If S is T2, C2 is the better conversion.
            //* If an implicit conversion from T1 to T2 exists and no implicit conversion from T2 to
            //   T1 exists, C1 is the better conversion.
            //* If an implicit conversion from T2 to T1 exists and no implicit conversion from T1 to
            //   T2 exists, C2 is the better conversion.
            //
            // [Otherwise, see table above for better integral type conversions.]

            if (p1 == p2)
            {
                return BetterType.Same;
            }

            if (argType == p1)
            {
                return BetterType.Left;
            }

            if (argType == p2)
            {
                return BetterType.Right;
            }

            bool a2b = canConvert(p1, p2);
            bool b2a = canConvert(p2, p1);

            if (a2b && !b2a)
            {
                return BetterType.Left;
            }
            if (b2a && !a2b)
            {
                return BetterType.Right;
            }

            Debug.Assert(b2a == a2b);

            if (p1.isPredefined() && p2.isPredefined() &&
                p1.getPredefType() <= PredefinedType.PT_OBJECT && p2.getPredefType() <= PredefinedType.PT_OBJECT)
            {
                int c = s_betterConversionTable[(int)p1.getPredefType()][(int)p2.getPredefType()];
                if (c == 1)
                {
                    return BetterType.Left;
                }
                else if (c == 2)
                {
                    return BetterType.Right;
                }
            }

            return BetterType.Neither;
        }

        ////////////////////////////////////////////////////////////////////////////////
        // Determine best method for overload resolution. Returns null if no best 
        // method, in which case two tying methods are returned for error reporting.

        protected CandidateFunctionMember FindBestMethod(
            List<CandidateFunctionMember> list,
            CType pTypeThrough,
            ArgInfos args,
            out CandidateFunctionMember methAmbig1,
            out CandidateFunctionMember methAmbig2)
        {
            Debug.Assert(list.Any());
            Debug.Assert(list.First().mpwi != null);
            Debug.Assert(list.Count > 0);

            // select the best method:
            /*
            Effectively, we pick the best item from a set using a non-transitive ranking function
            So, pick the first item (candidate) and compare against next (contender), if there is
                no next, goto phase 2
            If first is better, move to next contender, if none proceed to phase 2
            If second is better, make the contender the candidate and make the item following
                contender into the new contender, if there is none, goto phase 2
            If neither, make contender+1 into candidate and contender+2 into contender, if possible,
                otherwise, if contender was last, return null, otherwise if new candidate is last,
                goto phase 2
            Phase 2: compare all items before candidate to candidate
                If candidate always better, return it, otherwise return null

        */
            // Record two method that are ambiguous for error reporting.
            CandidateFunctionMember ambig1 = null;
            CandidateFunctionMember ambig2 = null;
            bool ambiguous = false;
            CandidateFunctionMember candidate = list[0];
            for (int i = 1; i < list.Count; i++)
            {
                CandidateFunctionMember contender = list[i];
                Debug.Assert(candidate != contender);

                BetterType result = WhichMethodIsBetter(candidate, contender, pTypeThrough, args);
                if (result == BetterType.Left)
                {
                    ambiguous = false;
                    continue;  // (meaning m1 is better...)
                }
                else if (result == BetterType.Right)
                {
                    ambiguous = false;
                    candidate = contender;
                }
                else
                {
                    // in case of tie we don't want to bother with the contender who tied...
                    ambig1 = candidate;
                    ambig2 = contender;

                    i++;
                    if (i < list.Count)
                    {
                        contender = list[i];
                        candidate = contender;
                    }
                    else
                    {
                        ambiguous = true;
                    }
                }
            }
            if (ambiguous)
                goto AMBIG;

            // Now, compare the candidate with items previous to it...
            foreach (CandidateFunctionMember contender in list)
            {
                if (contender == candidate)
                {
                    // We hit our winner, so its good enough...
                    methAmbig1 = null;
                    methAmbig2 = null;
                    return candidate;
                }
                BetterType result = WhichMethodIsBetter(contender, candidate, pTypeThrough, args);
                if (result == BetterType.Right)
                { // meaning m2 is better
                    continue;
                }
                else if (result == BetterType.Same || result == BetterType.Neither)
                {
                    ambig1 = candidate;
                    ambig2 = contender;
                }
                break;
            }

        AMBIG:
            // an ambig call. Return two of the ambiguous set.
            if (ambig1 != null && ambig2 != null)
            {
                methAmbig1 = ambig1;
                methAmbig2 = ambig2;
            }
            else
            {
                // For some reason, we have an ambiguity but never had a tie.
                // This can easily happen in a circular graph of candidate methods.
                methAmbig1 = list.First();
                methAmbig2 = list.Skip(1).First();
            }

            return null;
        }
    }
}
