// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Microsoft.CSharp.RuntimeBinder.Syntax;

namespace Microsoft.CSharp.RuntimeBinder.Semantics
{
    internal static class Operators
    {
        private sealed class OperatorInfo
        {
            public readonly TokenKind TokenKind;
            public readonly string Name;
            public readonly ExpressionKind ExpressionKind;

            public OperatorInfo(TokenKind kind, string name, ExpressionKind e)
            {
                TokenKind = kind;
                Name = name;
                ExpressionKind = e;
            }
        }

        private static readonly OperatorInfo[] s_operatorInfos = {
            new OperatorInfo(TokenKind.Unknown         , null                      , ExpressionKind.ExpressionKindCount                                 ),
            new OperatorInfo(TokenKind.Equal           , null                      , ExpressionKind.ExpressionKindCount                                 ),
            new OperatorInfo(TokenKind.PlusEqual       , null                      , ExpressionKind.MultiOffset + (int)ExpressionKind.Add               ),
            new OperatorInfo(TokenKind.MinusEqual      , null                      , ExpressionKind.MultiOffset + (int)ExpressionKind.Subtract          ),
            new OperatorInfo(TokenKind.SplatEqual      , null                      , ExpressionKind.MultiOffset + (int)ExpressionKind.Multiply          ),
            new OperatorInfo(TokenKind.SlashEqual      , null                      , ExpressionKind.MultiOffset + (int)ExpressionKind.Divide            ),
            new OperatorInfo(TokenKind.PercentEqual    , null                      , ExpressionKind.MultiOffset + (int)ExpressionKind.Modulo            ),
            new OperatorInfo(TokenKind.AndEqual        , null                      , ExpressionKind.MultiOffset + (int)ExpressionKind.BitwiseAnd        ),
            new OperatorInfo(TokenKind.HatEqual        , null                      , ExpressionKind.MultiOffset + (int)ExpressionKind.BitwiseExclusiveOr),
            new OperatorInfo(TokenKind.BarEqual        , null                      , ExpressionKind.MultiOffset + (int)ExpressionKind.BitwiseOr         ),
            new OperatorInfo(TokenKind.LeftShiftEqual  , null                      , ExpressionKind.MultiOffset + (int)ExpressionKind.LeftShirt         ),
            new OperatorInfo(TokenKind.RightShiftEqual , null                      , ExpressionKind.MultiOffset + (int)ExpressionKind.RightShift        ),
            new OperatorInfo(TokenKind.Question        , null                      , ExpressionKind.ExpressionKindCount                                 ),
            new OperatorInfo(TokenKind.QuestionQuestion, null                      , ExpressionKind.ExpressionKindCount                                 ),
            new OperatorInfo(TokenKind.LogicalOr       , null                      , ExpressionKind.LogicalOr                                           ),
            new OperatorInfo(TokenKind.LogicalAnd      , null                      , ExpressionKind.LogicalAnd                                          ),
            new OperatorInfo(TokenKind.Bar             , "op_BitwiseOr"            , ExpressionKind.BitwiseOr                                           ),
            new OperatorInfo(TokenKind.Hat             , "op_ExclusiveOr"          , ExpressionKind.BitwiseExclusiveOr                                  ),
            new OperatorInfo(TokenKind.Ampersand       , "op_BitwiseAnd"           , ExpressionKind.BitwiseAnd                                          ),
            new OperatorInfo(TokenKind.EqualEqual      , "op_Equality"             , ExpressionKind.Eq                                                  ),
            new OperatorInfo(TokenKind.NotEqual        , "op_Inequality"           , ExpressionKind.NotEq                                               ),
            new OperatorInfo(TokenKind.LessThan        , "op_LessThan"             , ExpressionKind.LessThan                                            ),
            new OperatorInfo(TokenKind.LessThanEqual   , "op_LessThanOrEqual"      , ExpressionKind.LessThanOrEqual                                     ),
            new OperatorInfo(TokenKind.GreaterThan     , "op_GreaterThan"          , ExpressionKind.GreaterThan                                         ),
            new OperatorInfo(TokenKind.GreaterThanEqual, "op_GreaterThanOrEqual"   , ExpressionKind.GreaterThanOrEqual                                  ),
            new OperatorInfo(TokenKind.Is              , null                      , ExpressionKind.ExpressionKindCount                                 ),
            new OperatorInfo(TokenKind.As              , null                      , ExpressionKind.ExpressionKindCount                                 ),
            new OperatorInfo(TokenKind.LeftShift       , "op_LeftShift"            , ExpressionKind.LeftShirt                                           ),
            new OperatorInfo(TokenKind.RightShift      , "op_RightShift"           , ExpressionKind.RightShift                                          ),
            new OperatorInfo(TokenKind.Plus            , "op_Addition"             , ExpressionKind.Add                                                 ),
            new OperatorInfo(TokenKind.Minus           , "op_Subtraction"          , ExpressionKind.Subtract                                            ),
            new OperatorInfo(TokenKind.Splat           , "op_Multiply"             , ExpressionKind.Multiply                                            ),
            new OperatorInfo(TokenKind.Slash           , "op_Division"             , ExpressionKind.Divide                                              ),
            new OperatorInfo(TokenKind.Percent         , "op_Modulus"              , ExpressionKind.Modulo                                              ),
            new OperatorInfo(TokenKind.Unknown         , null                      , ExpressionKind.ExpressionKindCount                                 ),
            new OperatorInfo(TokenKind.Plus            , "op_UnaryPlus"            , ExpressionKind.UnaryPlus                                           ),
            new OperatorInfo(TokenKind.Minus           , "op_UnaryNegation"        , ExpressionKind.Negate                                              ),
            new OperatorInfo(TokenKind.Tilde           , "op_OnesComplement"       , ExpressionKind.BitwiseNot                                          ),
            new OperatorInfo(TokenKind.Bang            , "op_LogicalNot"           , ExpressionKind.LogicalNot                                          ),
            new OperatorInfo(TokenKind.PlusPlus        , "op_Increment"            , ExpressionKind.Add                                                 ),
            new OperatorInfo(TokenKind.MinusMinus      , "op_Decrement"            , ExpressionKind.Subtract                                            ),
            new OperatorInfo(TokenKind.TypeOf          , null                      , ExpressionKind.TypeOf                                              ),
            new OperatorInfo(TokenKind.Checked         , null                      , ExpressionKind.ExpressionKindCount                                 ),
            new OperatorInfo(TokenKind.Unchecked       , null                      , ExpressionKind.ExpressionKindCount                                 ),
            new OperatorInfo(TokenKind.MakeRef         , null                      , ExpressionKind.ExpressionKindCount                                 ),
            new OperatorInfo(TokenKind.RefValue        , null                      , ExpressionKind.ExpressionKindCount                                 ),
            new OperatorInfo(TokenKind.RefType         , null                      , ExpressionKind.ExpressionKindCount                                 ),
            new OperatorInfo(TokenKind.ArgList         , null                      , ExpressionKind.ExpressionKindCount                                 ),
            new OperatorInfo(TokenKind.Unknown         , null                      , ExpressionKind.ExpressionKindCount                                 ),
            new OperatorInfo(TokenKind.Splat           , null                      , ExpressionKind.ExpressionKindCount                                 ),
            new OperatorInfo(TokenKind.Ampersand       , null                      , ExpressionKind.ExpressionKindCount                                 ),
            new OperatorInfo(TokenKind.Colon           , null                      , ExpressionKind.ExpressionKindCount                                 ),
            new OperatorInfo(TokenKind.This            , null                      , ExpressionKind.ExpressionKindCount                                 ),
            new OperatorInfo(TokenKind.Base            , null                      , ExpressionKind.ExpressionKindCount                                 ),
            new OperatorInfo(TokenKind.Null            , null                      , ExpressionKind.ExpressionKindCount                                 ),
            new OperatorInfo(TokenKind.True            , "op_True"                 , ExpressionKind.ExpressionKindCount                                 ),
            new OperatorInfo(TokenKind.False           , "op_False"                , ExpressionKind.ExpressionKindCount                                 ),
            new OperatorInfo(TokenKind.Unknown         , null                      , ExpressionKind.ExpressionKindCount                                 ),
            new OperatorInfo(TokenKind.Unknown         , null                      , ExpressionKind.ExpressionKindCount                                 ),
            new OperatorInfo(TokenKind.Unknown         , null                      , ExpressionKind.ExpressionKindCount                                 ),
            new OperatorInfo(TokenKind.PlusPlus        , null                      , ExpressionKind.Add                                                 ),
            new OperatorInfo(TokenKind.MinusMinus      , null                      , ExpressionKind.Subtract                                            ),
            new OperatorInfo(TokenKind.Dot             , null                      , ExpressionKind.ExpressionKindCount                                 ),
            new OperatorInfo(TokenKind.Implicit        , "op_Implicit"             , ExpressionKind.ExpressionKindCount                                 ),
            new OperatorInfo(TokenKind.Explicit        , "op_Explicit"             , ExpressionKind.ExpressionKindCount                                 ),
            new OperatorInfo(TokenKind.Unknown         , "op_Equals"                  , ExpressionKind.ExpressionKindCount                                 ),
            new OperatorInfo(TokenKind.Unknown         , "op_Compare"                 , ExpressionKind.ExpressionKindCount                                 ),
            new OperatorInfo(TokenKind.Unknown         , null                      , ExpressionKind.ExpressionKindCount                                 )
        };

        private static Dictionary<string, string> s_operatorsByName;

        private static Dictionary<string, string> GetOperatorByName()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>(28)
            {
                {"op_Equals", "equals"},
                {"op_Compare", "compare" }
            };

            foreach (OperatorInfo opInfo in s_operatorInfos)
            {
                TokenKind token = opInfo.TokenKind;
                if (token != TokenKind.Unknown && opInfo.Name != null)
                {
                    dict.Add(opInfo.Name, TokenFacts.GetText(token));
                }
            }

            return dict;
        }

        private static OperatorInfo GetInfo(OperatorKind op) => s_operatorInfos[(int)op];

        public static string OperatorOfMethodName(string name) =>
            (s_operatorsByName ?? (s_operatorsByName = GetOperatorByName()))[name];

        public static string GetDisplayName(OperatorKind op) => TokenFacts.GetText(GetInfo(op).TokenKind);

        public static ExpressionKind GetExpressionKind(OperatorKind op) => GetInfo(op).ExpressionKind;
    }
}
