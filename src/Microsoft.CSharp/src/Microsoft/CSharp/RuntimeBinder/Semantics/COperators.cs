// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using Microsoft.CSharp.RuntimeBinder.Syntax;

namespace Microsoft.CSharp.RuntimeBinder.Semantics
{
    internal static class Operators
    {
        private sealed class OPINFO
        {
            public OPINFO(TokenKind t, PredefinedName pn, ExpressionKind e, int c)
            {
                iToken = t;
                methodName = pn;
                expressionKind = e;
            }
            public readonly TokenKind iToken;
            public readonly PredefinedName methodName;
            public readonly ExpressionKind expressionKind;
        }

        private static readonly OPINFO[] s_rgOpInfo = {
            new OPINFO(TokenKind.Unknown         , PredefinedName.PN_COUNT               , ExpressionKind.ExpressionKindCount                                 , 0),
            new OPINFO(TokenKind.Equal           , PredefinedName.PN_COUNT               , ExpressionKind.ExpressionKindCount                                 , 2),
            new OPINFO(TokenKind.PlusEqual       , PredefinedName.PN_COUNT               , ExpressionKind.MultiOffset + (int)ExpressionKind.Add               , 2),
            new OPINFO(TokenKind.MinusEqual      , PredefinedName.PN_COUNT               , ExpressionKind.MultiOffset + (int)ExpressionKind.Subtract          , 2),
            new OPINFO(TokenKind.SplatEqual      , PredefinedName.PN_COUNT               , ExpressionKind.MultiOffset + (int)ExpressionKind.Multiply          , 2),
            new OPINFO(TokenKind.SlashEqual      , PredefinedName.PN_COUNT               , ExpressionKind.MultiOffset + (int)ExpressionKind.Divide            , 2),
            new OPINFO(TokenKind.PercentEqual    , PredefinedName.PN_COUNT               , ExpressionKind.MultiOffset + (int)ExpressionKind.Modulo            , 2),
            new OPINFO(TokenKind.AndEqual        , PredefinedName.PN_COUNT               , ExpressionKind.MultiOffset + (int)ExpressionKind.BitwiseAnd        , 2),
            new OPINFO(TokenKind.HatEqual        , PredefinedName.PN_COUNT               , ExpressionKind.MultiOffset + (int)ExpressionKind.BitwiseExclusiveOr, 2),
            new OPINFO(TokenKind.BarEqual        , PredefinedName.PN_COUNT               , ExpressionKind.MultiOffset + (int)ExpressionKind.BitwiseOr         , 2),
            new OPINFO(TokenKind.LeftShiftEqual  , PredefinedName.PN_COUNT               , ExpressionKind.MultiOffset + (int)ExpressionKind.LeftShirt         , 2),
            new OPINFO(TokenKind.RightShiftEqual , PredefinedName.PN_COUNT               , ExpressionKind.MultiOffset + (int)ExpressionKind.RightShift        , 2),
            new OPINFO(TokenKind.Question        , PredefinedName.PN_COUNT               , ExpressionKind.ExpressionKindCount                                 , 2),
            new OPINFO(TokenKind.QuestionQuestion, PredefinedName.PN_COUNT               , ExpressionKind.ExpressionKindCount                                 , 2),
            new OPINFO(TokenKind.LogicalOr       , PredefinedName.PN_COUNT               , ExpressionKind.LogicalOr                                           , 2),
            new OPINFO(TokenKind.LogicalAnd      , PredefinedName.PN_COUNT               , ExpressionKind.LogicalAnd                                          , 2),
            new OPINFO(TokenKind.Bar             , PredefinedName.PN_OPBITWISEOR         , ExpressionKind.BitwiseOr                                           , 2),
            new OPINFO(TokenKind.Hat             , PredefinedName.PN_OPXOR               , ExpressionKind.BitwiseExclusiveOr                                  , 2),
            new OPINFO(TokenKind.Ampersand       , PredefinedName.PN_OPBITWISEAND        , ExpressionKind.BitwiseAnd                                          , 2),
            new OPINFO(TokenKind.EqualEqual      , PredefinedName.PN_OPEQUALITY          , ExpressionKind.Eq                                                  , 2),
            new OPINFO(TokenKind.NotEqual        , PredefinedName.PN_OPINEQUALITY        , ExpressionKind.NotEq                                               , 2),
            new OPINFO(TokenKind.LessThan        , PredefinedName.PN_OPLESSTHAN          , ExpressionKind.LessThan                                            , 2),
            new OPINFO(TokenKind.LessThanEqual   , PredefinedName.PN_OPLESSTHANOREQUAL   , ExpressionKind.LessThanOrEqual                                     , 2),
            new OPINFO(TokenKind.GreaterThan     , PredefinedName.PN_OPGREATERTHAN       , ExpressionKind.GreaterThan                                         , 2),
            new OPINFO(TokenKind.GreaterThanEqual, PredefinedName.PN_OPGREATERTHANOREQUAL, ExpressionKind.GreaterThanOrEqual                                  , 2),
            new OPINFO(TokenKind.Is              , PredefinedName.PN_COUNT               , ExpressionKind.ExpressionKindCount                                 , 2),
            new OPINFO(TokenKind.As              , PredefinedName.PN_COUNT               , ExpressionKind.ExpressionKindCount                                 , 2),
            new OPINFO(TokenKind.LeftShift       , PredefinedName.PN_OPLEFTSHIFT         , ExpressionKind.LeftShirt                                           , 2),
            new OPINFO(TokenKind.RightShift      , PredefinedName.PN_OPRIGHTSHIFT        , ExpressionKind.RightShift                                          , 2),
            new OPINFO(TokenKind.Plus            , PredefinedName.PN_OPPLUS              , ExpressionKind.Add                                                 , 2),
            new OPINFO(TokenKind.Minus           , PredefinedName.PN_OPMINUS             , ExpressionKind.Subtract                                            , 2),
            new OPINFO(TokenKind.Splat           , PredefinedName.PN_OPMULTIPLY          , ExpressionKind.Multiply                                            , 2),
            new OPINFO(TokenKind.Slash           , PredefinedName.PN_OPDIVISION          , ExpressionKind.Divide                                              , 2),
            new OPINFO(TokenKind.Percent         , PredefinedName.PN_OPMODULUS           , ExpressionKind.Modulo                                              , 2),
            new OPINFO(TokenKind.Unknown         , PredefinedName.PN_COUNT               , ExpressionKind.ExpressionKindCount                                 , 1),
            new OPINFO(TokenKind.Plus            , PredefinedName.PN_OPUNARYPLUS         , ExpressionKind.UnaryPlus                                           , 1),
            new OPINFO(TokenKind.Minus           , PredefinedName.PN_OPUNARYMINUS        , ExpressionKind.Negate                                              , 1),
            new OPINFO(TokenKind.Tilde           , PredefinedName.PN_OPCOMPLEMENT        , ExpressionKind.BitwiseNot                                          , 1),
            new OPINFO(TokenKind.Bang            , PredefinedName.PN_OPNEGATION          , ExpressionKind.LogicalNot                                          , 1),
            new OPINFO(TokenKind.PlusPlus        , PredefinedName.PN_OPINCREMENT         , ExpressionKind.Add                                                 , 1),
            new OPINFO(TokenKind.MinusMinus      , PredefinedName.PN_OPDECREMENT         , ExpressionKind.Subtract                                            , 1),
            new OPINFO(TokenKind.TypeOf          , PredefinedName.PN_COUNT               , ExpressionKind.TypeOf                                              , 1),
            new OPINFO(TokenKind.Checked         , PredefinedName.PN_COUNT               , ExpressionKind.ExpressionKindCount                                 , 1),
            new OPINFO(TokenKind.Unchecked       , PredefinedName.PN_COUNT               , ExpressionKind.ExpressionKindCount                                 , 1),
            new OPINFO(TokenKind.MakeRef         , PredefinedName.PN_COUNT               , ExpressionKind.ExpressionKindCount                                 , 1),
            new OPINFO(TokenKind.RefValue        , PredefinedName.PN_COUNT               , ExpressionKind.ExpressionKindCount                                 , 2),
            new OPINFO(TokenKind.RefType         , PredefinedName.PN_COUNT               , ExpressionKind.ExpressionKindCount                                 , 1),
            new OPINFO(TokenKind.ArgList         , PredefinedName.PN_COUNT               , ExpressionKind.ExpressionKindCount                                 , 0),
            new OPINFO(TokenKind.Unknown         , PredefinedName.PN_COUNT               , ExpressionKind.ExpressionKindCount                                 , 2),
            new OPINFO(TokenKind.Splat           , PredefinedName.PN_COUNT               , ExpressionKind.ExpressionKindCount                                 , 1),
            new OPINFO(TokenKind.Ampersand       , PredefinedName.PN_COUNT               , ExpressionKind.ExpressionKindCount                                 , 1),
            new OPINFO(TokenKind.Colon           , PredefinedName.PN_COUNT               , ExpressionKind.ExpressionKindCount                                 , 2),
            new OPINFO(TokenKind.This            , PredefinedName.PN_COUNT               , ExpressionKind.ExpressionKindCount                                 , 0),
            new OPINFO(TokenKind.Base            , PredefinedName.PN_COUNT               , ExpressionKind.ExpressionKindCount                                 , 0),
            new OPINFO(TokenKind.Null            , PredefinedName.PN_COUNT               , ExpressionKind.ExpressionKindCount                                 , 0),
            new OPINFO(TokenKind.True            , PredefinedName.PN_OPTRUE              , ExpressionKind.ExpressionKindCount                                 , 1),
            new OPINFO(TokenKind.False           , PredefinedName.PN_OPFALSE             , ExpressionKind.ExpressionKindCount                                 , 1),
            new OPINFO(TokenKind.Unknown         , PredefinedName.PN_COUNT               , ExpressionKind.ExpressionKindCount                                 , 0),
            new OPINFO(TokenKind.Unknown         , PredefinedName.PN_COUNT               , ExpressionKind.ExpressionKindCount                                 , 0),
            new OPINFO(TokenKind.Unknown         , PredefinedName.PN_COUNT               , ExpressionKind.ExpressionKindCount                                 , 0),
            new OPINFO(TokenKind.PlusPlus        , PredefinedName.PN_COUNT               , ExpressionKind.Add                                                 , 1),
            new OPINFO(TokenKind.MinusMinus      , PredefinedName.PN_COUNT               , ExpressionKind.Subtract                                            , 1),
            new OPINFO(TokenKind.Dot             , PredefinedName.PN_COUNT               , ExpressionKind.ExpressionKindCount                                 , 2),
            new OPINFO(TokenKind.Implicit        , PredefinedName.PN_OPIMPLICITMN        , ExpressionKind.ExpressionKindCount                                 , 1),
            new OPINFO(TokenKind.Explicit        , PredefinedName.PN_OPEXPLICITMN        , ExpressionKind.ExpressionKindCount                                 , 1),
            new OPINFO(TokenKind.Unknown         , PredefinedName.PN_OPEQUALS            , ExpressionKind.ExpressionKindCount                                 , 2),
            new OPINFO(TokenKind.Unknown         , PredefinedName.PN_OPCOMPARE           , ExpressionKind.ExpressionKindCount                                 , 2),
            new OPINFO(TokenKind.Unknown         , PredefinedName.PN_COUNT               , ExpressionKind.ExpressionKindCount                                 , 0)
        };

        private static OPINFO GetInfo(OperatorKind op) => s_rgOpInfo[(int)op];

        public static OperatorKind OperatorOfMethodName(Name name)
        {
            Debug.Assert(name != null);

            for (OperatorKind i = OperatorKind.OP_NONE; i < OperatorKind.OP_LAST; i = (i + 1))
            {
                if (HasMethodName(i) && (name == NameManager.GetPredefinedName(GetMethodName(i))))
                {
                    return i;
                }
            }

            return OperatorKind.OP_NONE;
        }

        private static bool HasMethodName(OperatorKind op)
        {
            //Debug.Assert(IsValid(op));
            return GetMethodName(op) != PredefinedName.PN_COUNT;
        }

        private static PredefinedName GetMethodName(OperatorKind op)
        {
            //Debug.Assert(IsValid(op));
            return GetInfo(op).methodName;
        }

        public static bool HasDisplayName(OperatorKind op)
        {
            //Debug.Assert(IsValid(op));
            return GetInfo(op).iToken != TokenKind.Unknown;
        }
        public static string GetDisplayName(OperatorKind op)
        {
            Debug.Assert(HasDisplayName(op));
            return TokenFacts.GetText(GetInfo(op).iToken);
        }
        public static ExpressionKind GetExpressionKind(OperatorKind op)
        {
            //Debug.Assert(IsValid(op));
            return GetInfo(op).expressionKind;
        }
    }
}
