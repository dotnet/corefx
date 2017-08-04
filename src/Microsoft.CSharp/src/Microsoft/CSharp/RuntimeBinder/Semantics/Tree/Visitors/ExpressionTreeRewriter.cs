// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using Microsoft.CSharp.RuntimeBinder.Syntax;

namespace Microsoft.CSharp.RuntimeBinder.Semantics
{
    internal sealed class ExpressionTreeRewriter : ExprVisitorBase
    {
        public static Expr Rewrite(Expr expr, ExprFactory expressionFactory, SymbolLoader symbolLoader)
        {
            ExpressionTreeRewriter rewriter = new ExpressionTreeRewriter(expressionFactory, symbolLoader);
            return rewriter.Visit(expr);
        }

        private ExprFactory expressionFactory;
        private SymbolLoader symbolLoader;
        private ExprBoundLambda currentAnonMeth;

        private ExprFactory GetExprFactory() { return expressionFactory; }

        private SymbolLoader GetSymbolLoader() { return symbolLoader; }

        private ExpressionTreeRewriter(ExprFactory expressionFactory, SymbolLoader symbolLoader)
        {
            this.expressionFactory = expressionFactory;
            this.symbolLoader = symbolLoader;
        }

        protected override Expr Dispatch(Expr expr)
        {
            Debug.Assert(expr != null);

            Expr result = base.Dispatch(expr);
            if (result == expr)
            {
                throw Error.InternalCompilerError();
            }
            return result;
        }

        /////////////////////////////////////////////////////////////////////////////////
        // Statement types.
        protected override Expr VisitASSIGNMENT(ExprAssignment assignment)
        {
            Debug.Assert(assignment != null);

            // For assignments, we either have a member assignment or an indexed assignment.
            //Debug.Assert(assignment.GetLHS().isPROP() || assignment.GetLHS().isFIELD() || assignment.GetLHS().isARRAYINDEX() || assignment.GetLHS().isLOCAL());
            Expr lhs;
            if (assignment.LHS is ExprProperty prop)
            {
                if (prop.OptionalArguments== null)
                {
                    // Regular property.
                    lhs = Visit(prop);
                }
                else
                {
                    // Indexed assignment. Here we need to find the instance of the object, create the 
                    // PropInfo for the thing, and get the array of expressions that make up the index arguments.
                    //
                    // The LHS becomes Expression.Property(instance, indexerInfo, arguments).
                    Expr instance = Visit(prop.MemberGroup.OptionalObject);
                    Expr propInfo = GetExprFactory().CreatePropertyInfo(prop.PropWithTypeSlot.Prop(), prop.PropWithTypeSlot.Ats);
                    Expr arguments = GenerateParamsArray(
                        GenerateArgsList(prop.OptionalArguments),
                        PredefinedType.PT_EXPRESSION);

                    lhs = GenerateCall(PREDEFMETH.PM_EXPRESSION_PROPERTY, instance, propInfo, arguments);
                }
            }
            else
            {
                lhs = Visit(assignment.LHS);
            }

            Expr rhs = Visit(assignment.RHS);
            return GenerateCall(PREDEFMETH.PM_EXPRESSION_ASSIGN, lhs, rhs);
        }
        protected override Expr VisitMULTIGET(ExprMultiGet pExpr)
        {
            return Visit(pExpr.OptionalMulti.Left);
        }
        protected override Expr VisitMULTI(ExprMulti pExpr)
        {
            Expr rhs = Visit(pExpr.Operator);
            Expr lhs = Visit(pExpr.Left);
            return GenerateCall(PREDEFMETH.PM_EXPRESSION_ASSIGN, lhs, rhs);
        }

        /////////////////////////////////////////////////////////////////////////////////
        // Expression types.

        protected override Expr VisitBOUNDLAMBDA(ExprBoundLambda anonmeth)
        {
            Debug.Assert(anonmeth != null);

            ExprBoundLambda prevAnonMeth = currentAnonMeth;
            currentAnonMeth = anonmeth;
            MethodSymbol lambdaMethod = GetPreDefMethod(PREDEFMETH.PM_EXPRESSION_LAMBDA);

            CType delegateType = anonmeth.DelegateType;
            TypeArray lambdaTypeParams = GetSymbolLoader().getBSymmgr().AllocParams(1, new CType[] { delegateType });
            AggregateType expressionType = GetSymbolLoader().GetPredefindType(PredefinedType.PT_EXPRESSION);
            MethWithInst mwi = new MethWithInst(lambdaMethod, expressionType, lambdaTypeParams);
            Expr createParameters = CreateWraps(anonmeth);
            Expr body = RewriteLambdaBody(anonmeth);
            Expr parameters = RewriteLambdaParameters(anonmeth);
            Expr args = GetExprFactory().CreateList(body, parameters);
            CType typeRet = GetSymbolLoader().GetTypeManager().SubstType(mwi.Meth().RetType, mwi.GetType(), mwi.TypeArgs);
            ExprMemberGroup pMemGroup = GetExprFactory().CreateMemGroup(null, mwi);
            ExprCall call = GetExprFactory().CreateCall(0, typeRet, args, pMemGroup, mwi);
            Expr callLambda = call;

            call.PredefinedMethod = PREDEFMETH.PM_EXPRESSION_LAMBDA;

            currentAnonMeth = prevAnonMeth;
            if (createParameters != null)
            {
                callLambda = GetExprFactory().CreateSequence(createParameters, callLambda);
            }
            Expr expr = DestroyWraps(anonmeth, callLambda);
            // If we are already inside an expression tree rewrite and this is an expression tree lambda
            // then it needs to be quoted.
            if (currentAnonMeth != null)
            {
                expr = GenerateCall(PREDEFMETH.PM_EXPRESSION_QUOTE, expr);
            }
            return expr;
        }
        protected override Expr VisitCONSTANT(ExprConstant expr)
        {
            Debug.Assert(expr != null);
            return GenerateConstant(expr);
        }
        protected override Expr VisitLOCAL(ExprLocal local)
        {
            Debug.Assert(local != null);
            Debug.Assert(!local.Local.isThis);
            // this is true for all parameters of an expression lambda
            if (local.Local.wrap != null)
            {
                return local.Local.wrap;
            }
            Debug.Assert(local.Local.fUsedInAnonMeth);
            return GetExprFactory().CreateHoistedLocalInExpression();
        }
        protected override Expr VisitFIELD(ExprField expr)
        {
            Debug.Assert(expr != null);
            Expr pObject;
            if (expr.OptionalObject== null)
            {
                pObject = GetExprFactory().CreateNull();
            }
            else
            {
                pObject = Visit(expr.OptionalObject);
            }
            ExprFieldInfo pFieldInfo = GetExprFactory().CreateFieldInfo(expr.FieldWithType.Field(), expr.FieldWithType.GetType());
            return GenerateCall(PREDEFMETH.PM_EXPRESSION_FIELD, pObject, pFieldInfo);
        }
        protected override Expr VisitUSERDEFINEDCONVERSION(ExprUserDefinedConversion expr)
        {
            Debug.Assert(expr != null);
            return GenerateUserDefinedConversion(expr, expr.Argument);
        }
        protected override Expr VisitCAST(ExprCast pExpr)
        {
            Debug.Assert(pExpr != null);

            Expr pArgument = pExpr.Argument;

            // If we have generated an identity cast or reference cast to a base class
            // we can omit the cast.
            if (pArgument.Type == pExpr.Type ||
                    GetSymbolLoader().IsBaseClassOfClass(pArgument.Type, pExpr.Type) ||
                    CConversions.FImpRefConv(GetSymbolLoader(), pArgument.Type, pExpr.Type))
            {
                return Visit(pArgument);
            }

            // If we have a cast to PredefinedType.PT_G_EXPRESSION and the thing that we're casting is 
            // a EXPRBOUNDLAMBDA that is an expression tree, then just visit the expression tree.
            if (pExpr.Type != null &&
                    pExpr.Type.isPredefType(PredefinedType.PT_G_EXPRESSION) &&
                    pArgument is ExprBoundLambda)
            {
                return Visit(pArgument);
            }

            Expr result = GenerateConversion(pArgument, pExpr.Type, pExpr.isChecked());
            if ((pExpr.Flags & EXPRFLAG.EXF_UNBOXRUNTIME) != 0)
            {
                // Propagate the unbox flag to the call for the ExpressionTreeCallRewriter.
                result.Flags |= EXPRFLAG.EXF_UNBOXRUNTIME;
            }
            return result;
        }
        protected override Expr VisitCONCAT(ExprConcat expr)
        {
            Debug.Assert(expr != null);
            PREDEFMETH pdm;
            if (expr.FirstArgument.Type.isPredefType(PredefinedType.PT_STRING) && expr.SecondArgument.Type.isPredefType(PredefinedType.PT_STRING))
            {
                pdm = PREDEFMETH.PM_STRING_CONCAT_STRING_2;
            }
            else
            {
                pdm = PREDEFMETH.PM_STRING_CONCAT_OBJECT_2;
            }
            Expr p1 = Visit(expr.FirstArgument);
            Expr p2 = Visit(expr.SecondArgument);
            MethodSymbol method = GetPreDefMethod(pdm);
            Expr methodInfo = GetExprFactory().CreateMethodInfo(method, GetSymbolLoader().GetPredefindType(PredefinedType.PT_STRING), null);
            return GenerateCall(PREDEFMETH.PM_EXPRESSION_ADD_USER_DEFINED, p1, p2, methodInfo);
        }
        protected override Expr VisitBINOP(ExprBinOp expr)
        {
            Debug.Assert(expr != null);
            if (expr.UserDefinedCallMethod != null)
            {
                return GenerateUserDefinedBinaryOperator(expr);
            }
            else
            {
                return GenerateBuiltInBinaryOperator(expr);
            }
        }
        protected override Expr VisitUNARYOP(ExprUnaryOp pExpr)
        {
            Debug.Assert(pExpr != null);
            if (pExpr.UserDefinedCallMethod != null)
            {
                return GenerateUserDefinedUnaryOperator(pExpr);
            }
            else
            {
                return GenerateBuiltInUnaryOperator(pExpr);
            }
        }
        protected override Expr VisitARRAYINDEX(ExprArrayIndex pExpr)
        {
            Debug.Assert(pExpr != null);
            Expr arr = Visit(pExpr.Array);
            Expr args = GenerateIndexList(pExpr.Index);
            if (args is ExprList)
            {
                Expr Params = GenerateParamsArray(args, PredefinedType.PT_EXPRESSION);
                return GenerateCall(PREDEFMETH.PM_EXPRESSION_ARRAYINDEX2, arr, Params);
            }
            return GenerateCall(PREDEFMETH.PM_EXPRESSION_ARRAYINDEX, arr, args);
        }

        protected override Expr VisitCALL(ExprCall expr)
        {
            Debug.Assert(expr != null);
            switch (expr.NullableCallLiftKind)
            {
                default:
                    break;
                case NullableCallLiftKind.NullableIntermediateConversion:
                case NullableCallLiftKind.NullableConversion:
                case NullableCallLiftKind.NullableConversionConstructor:
                    return GenerateConversion(expr.OptionalArguments, expr.Type, expr.isChecked());
                case NullableCallLiftKind.NotLiftedIntermediateConversion:
                case NullableCallLiftKind.UserDefinedConversion:
                    return GenerateUserDefinedConversion(expr.OptionalArguments, expr.Type, expr.MethWithInst);
            }

            if (expr.MethWithInst.Meth().IsConstructor())
            {
                return GenerateConstructor(expr);
            }

            ExprMemberGroup memberGroup = expr.MemberGroup;
            if (memberGroup.IsDelegate)
            {
                return GenerateDelegateInvoke(expr);
            }

            Expr pObject;
            if (expr.MethWithInst.Meth().isStatic || expr.MemberGroup.OptionalObject== null)
            {
                pObject = GetExprFactory().CreateNull();
            }
            else
            {
                pObject = expr.MemberGroup.OptionalObject;

                // If we have, say, an int? which is the object of a call to ToString
                // then we do NOT want to generate ((object)i).ToString() because that
                // will convert a null-valued int? to a null object.  Rather what we want
                // to do is box it to a ValueType and call ValueType.ToString.
                //
                // To implement this we say that if the object of the call is an implicit boxing cast
                // then just generate the object, not the cast.  If the cast is explicit in the
                // source code then it will be an EXPLICITCAST and we will visit it normally.
                //
                // It might be better to rewrite the expression tree API so that it
                // can handle in the general case all implicit boxing conversions. Right now it 
                // requires that all arguments to a call that need to be boxed be explicitly boxed.

                if (pObject != null && pObject is ExprCast cast && cast.IsBoxingCast)
                {
                    pObject = cast.Argument;
                }
                pObject = Visit(pObject);
            }
            Expr methodInfo = GetExprFactory().CreateMethodInfo(expr.MethWithInst);
            Expr args = GenerateArgsList(expr.OptionalArguments);
            Expr Params = GenerateParamsArray(args, PredefinedType.PT_EXPRESSION);
            PREDEFMETH pdm = PREDEFMETH.PM_EXPRESSION_CALL;
            Debug.Assert(!expr.MethWithInst.Meth().isVirtual || expr.MemberGroup.OptionalObject != null);

            return GenerateCall(pdm, pObject, methodInfo, Params);
        }
        protected override Expr VisitPROP(ExprProperty expr)
        {
            Debug.Assert(expr != null);
            Expr pObject;
            if (expr.PropWithTypeSlot.Prop().isStatic || expr.MemberGroup.OptionalObject== null)
            {
                pObject = GetExprFactory().CreateNull();
            }
            else
            {
                pObject = Visit(expr.MemberGroup.OptionalObject);
            }
            Expr propInfo = GetExprFactory().CreatePropertyInfo(expr.PropWithTypeSlot.Prop(), expr.PropWithTypeSlot.GetType());
            if (expr.OptionalArguments != null)
            {
                // It is an indexer property.  Turn it into a virtual method call.
                Expr args = GenerateArgsList(expr.OptionalArguments);
                Expr Params = GenerateParamsArray(args, PredefinedType.PT_EXPRESSION);
                return GenerateCall(PREDEFMETH.PM_EXPRESSION_PROPERTY, pObject, propInfo, Params);
            }
            return GenerateCall(PREDEFMETH.PM_EXPRESSION_PROPERTY, pObject, propInfo);
        }
        protected override Expr VisitARRINIT(ExprArrayInit expr)
        {
            Debug.Assert(expr != null);
            // POSSIBLE ERROR: Multi-d should be an error?
            Expr pTypeOf = CreateTypeOf(((ArrayType)expr.Type).GetElementType());
            Expr args = GenerateArgsList(expr.OptionalArguments);
            Expr Params = GenerateParamsArray(args, PredefinedType.PT_EXPRESSION);
            return GenerateCall(PREDEFMETH.PM_EXPRESSION_NEWARRAYINIT, pTypeOf, Params);
        }
        protected override Expr VisitZEROINIT(ExprZeroInit expr)
        {
            Debug.Assert(expr != null);
            Debug.Assert(expr.OptionalArgument == null);

            if (expr.IsConstructor)
            {
                // We have a parameterless "new MyStruct()" which has been realized as a zero init.
                ExprTypeOf pTypeOf = CreateTypeOf(expr.Type);
                return GenerateCall(PREDEFMETH.PM_EXPRESSION_NEW_TYPE, pTypeOf);
            }
            return GenerateConstant(expr);
        }
        protected override Expr VisitTYPEOF(ExprTypeOf expr)
        {
            Debug.Assert(expr != null);
            return GenerateConstant(expr);
        }

        private Expr GenerateDelegateInvoke(ExprCall expr)
        {
            Debug.Assert(expr != null);
            ExprMemberGroup memberGroup = expr.MemberGroup;
            Debug.Assert(memberGroup.IsDelegate);
            Expr oldObject = memberGroup.OptionalObject;
            Debug.Assert(oldObject != null);
            Expr pObject = Visit(oldObject);
            Expr args = GenerateArgsList(expr.OptionalArguments);
            Expr Params = GenerateParamsArray(args, PredefinedType.PT_EXPRESSION);
            return GenerateCall(PREDEFMETH.PM_EXPRESSION_INVOKE, pObject, Params);
        }

        private Expr GenerateBuiltInBinaryOperator(ExprBinOp expr)
        {
            Debug.Assert(expr != null);
            PREDEFMETH pdm;

            switch (expr.Kind)
            {
                case ExpressionKind.LeftShirt: pdm = PREDEFMETH.PM_EXPRESSION_LEFTSHIFT; break;
                case ExpressionKind.RightShift: pdm = PREDEFMETH.PM_EXPRESSION_RIGHTSHIFT; break;
                case ExpressionKind.BitwiseExclusiveOr: pdm = PREDEFMETH.PM_EXPRESSION_EXCLUSIVEOR; break;
                case ExpressionKind.BitwiseOr: pdm = PREDEFMETH.PM_EXPRESSION_OR; break;
                case ExpressionKind.BitwiseAnd: pdm = PREDEFMETH.PM_EXPRESSION_AND; break;
                case ExpressionKind.LogicalAnd: pdm = PREDEFMETH.PM_EXPRESSION_ANDALSO; break;
                case ExpressionKind.LogicalOr: pdm = PREDEFMETH.PM_EXPRESSION_ORELSE; break;
                case ExpressionKind.StringEq: pdm = PREDEFMETH.PM_EXPRESSION_EQUAL; break;
                case ExpressionKind.Eq: pdm = PREDEFMETH.PM_EXPRESSION_EQUAL; break;
                case ExpressionKind.StringNotEq: pdm = PREDEFMETH.PM_EXPRESSION_NOTEQUAL; break;
                case ExpressionKind.NotEq: pdm = PREDEFMETH.PM_EXPRESSION_NOTEQUAL; break;
                case ExpressionKind.GreaterThanOrEqual: pdm = PREDEFMETH.PM_EXPRESSION_GREATERTHANOREQUAL; break;
                case ExpressionKind.LessThanOrEqual: pdm = PREDEFMETH.PM_EXPRESSION_LESSTHANOREQUAL; break;
                case ExpressionKind.LessThan: pdm = PREDEFMETH.PM_EXPRESSION_LESSTHAN; break;
                case ExpressionKind.GreaterThan: pdm = PREDEFMETH.PM_EXPRESSION_GREATERTHAN; break;
                case ExpressionKind.Modulo: pdm = PREDEFMETH.PM_EXPRESSION_MODULO; break;
                case ExpressionKind.Divide: pdm = PREDEFMETH.PM_EXPRESSION_DIVIDE; break;
                case ExpressionKind.Multiply:
                    pdm = expr.isChecked() ? PREDEFMETH.PM_EXPRESSION_MULTIPLYCHECKED : PREDEFMETH.PM_EXPRESSION_MULTIPLY;
                    break;
                case ExpressionKind.Subtract:
                    pdm = expr.isChecked() ? PREDEFMETH.PM_EXPRESSION_SUBTRACTCHECKED : PREDEFMETH.PM_EXPRESSION_SUBTRACT;
                    break;
                case ExpressionKind.Add:
                    pdm = expr.isChecked() ? PREDEFMETH.PM_EXPRESSION_ADDCHECKED : PREDEFMETH.PM_EXPRESSION_ADD;
                    break;

                default:
                    throw Error.InternalCompilerError();
            }
            Expr origL = expr.OptionalLeftChild;
            Expr origR = expr.OptionalRightChild;
            Debug.Assert(origL != null);
            Debug.Assert(origR != null);
            CType typeL = origL.Type;
            CType typeR = origR.Type;

            Expr newL = Visit(origL);
            Expr newR = Visit(origR);

            bool didEnumConversion = false;
            CType convertL = null;
            CType convertR = null;

            if (typeL.isEnumType())
            {
                // We have already inserted casts if not lifted, so we should never see an enum.
                Debug.Assert(expr.IsLifted);
                convertL = GetSymbolLoader().GetTypeManager().GetNullable(typeL.underlyingEnumType());
                typeL = convertL;
                didEnumConversion = true;
            }
            else if (typeL is NullableType nubL && nubL.UnderlyingType.isEnumType())
            {
                Debug.Assert(expr.IsLifted);
                convertL = GetSymbolLoader().GetTypeManager().GetNullable(nubL.UnderlyingType.underlyingEnumType());
                typeL = convertL;
                didEnumConversion = true;
            }
            if (typeR.isEnumType())
            {
                Debug.Assert(expr.IsLifted);
                convertR = GetSymbolLoader().GetTypeManager().GetNullable(typeR.underlyingEnumType());
                typeR = convertR;
                didEnumConversion = true;
            }
            else if (typeR is NullableType nubR && nubR.UnderlyingType.isEnumType())
            {
                Debug.Assert(expr.IsLifted);
                convertR = GetSymbolLoader().GetTypeManager().GetNullable(nubR.UnderlyingType.underlyingEnumType());
                typeR = convertR;
                didEnumConversion = true;
            }
            if (typeL is NullableType nubL2 && nubL2.UnderlyingType == typeR)
            {
                convertR = typeL;
            }
            if (typeR is NullableType nubR2 && nubR2.UnderlyingType == typeL)
            {
                convertL = typeR;
            }

            if (convertL != null)
            {
                newL = GenerateCall(PREDEFMETH.PM_EXPRESSION_CONVERT, newL, CreateTypeOf(convertL));
            }
            if (convertR != null)
            {
                newR = GenerateCall(PREDEFMETH.PM_EXPRESSION_CONVERT, newR, CreateTypeOf(convertR));
            }

            Expr call = GenerateCall(pdm, newL, newR);

            if (didEnumConversion && expr.Type.StripNubs().isEnumType())
            {
                call = GenerateCall(PREDEFMETH.PM_EXPRESSION_CONVERT, call, CreateTypeOf(expr.Type));
            }

            return call;
        }

        private Expr GenerateBuiltInUnaryOperator(ExprUnaryOp expr)
        {
            Debug.Assert(expr != null);
            PREDEFMETH pdm;
            switch (expr.Kind)
            {
                case ExpressionKind.UnaryPlus:
                    return Visit(expr.Child);
                case ExpressionKind.BitwiseNot: pdm = PREDEFMETH.PM_EXPRESSION_NOT; break;
                case ExpressionKind.LogicalNot: pdm = PREDEFMETH.PM_EXPRESSION_NOT; break;
                case ExpressionKind.Negate:
                    pdm = expr.isChecked() ? PREDEFMETH.PM_EXPRESSION_NEGATECHECKED : PREDEFMETH.PM_EXPRESSION_NEGATE;
                    break;
                default:
                    throw Error.InternalCompilerError();
            }
            Expr origOp = expr.Child;

            return GenerateBuiltInUnaryOperator(pdm, origOp, expr);
        }

        private Expr GenerateBuiltInUnaryOperator(PREDEFMETH pdm, Expr pOriginalOperator, Expr pOperator)
        {
            Expr op = Visit(pOriginalOperator);
            bool isNullableEnum = pOriginalOperator.Type is NullableType nub && nub.underlyingType().isEnumType();
            if (isNullableEnum)
            {
                Debug.Assert(pOperator.Kind == ExpressionKind.BitwiseNot); // The only built-in unary operator defined on nullable enum.
                CType underlyingType = pOriginalOperator.Type.StripNubs().underlyingEnumType();
                CType nullableType = GetSymbolLoader().GetTypeManager().GetNullable(underlyingType);
                op = GenerateCall(PREDEFMETH.PM_EXPRESSION_CONVERT, op, CreateTypeOf(nullableType));
            }

            Expr call = GenerateCall(pdm, op);
            if (isNullableEnum)
            {
                call = GenerateCall(PREDEFMETH.PM_EXPRESSION_CONVERT, call, CreateTypeOf(pOperator.Type));
            }

            return call;
        }

        private Expr GenerateUserDefinedBinaryOperator(ExprBinOp expr)
        {
            Debug.Assert(expr != null);
            PREDEFMETH pdm;

            switch (expr.Kind)
            {
                case ExpressionKind.LogicalOr: pdm = PREDEFMETH.PM_EXPRESSION_ORELSE_USER_DEFINED; break;
                case ExpressionKind.LogicalAnd: pdm = PREDEFMETH.PM_EXPRESSION_ANDALSO_USER_DEFINED; break;
                case ExpressionKind.LeftShirt: pdm = PREDEFMETH.PM_EXPRESSION_LEFTSHIFT_USER_DEFINED; break;
                case ExpressionKind.RightShift: pdm = PREDEFMETH.PM_EXPRESSION_RIGHTSHIFT_USER_DEFINED; break;
                case ExpressionKind.BitwiseExclusiveOr: pdm = PREDEFMETH.PM_EXPRESSION_EXCLUSIVEOR_USER_DEFINED; break;
                case ExpressionKind.BitwiseOr: pdm = PREDEFMETH.PM_EXPRESSION_OR_USER_DEFINED; break;
                case ExpressionKind.BitwiseAnd: pdm = PREDEFMETH.PM_EXPRESSION_AND_USER_DEFINED; break;
                case ExpressionKind.Modulo: pdm = PREDEFMETH.PM_EXPRESSION_MODULO_USER_DEFINED; break;
                case ExpressionKind.Divide: pdm = PREDEFMETH.PM_EXPRESSION_DIVIDE_USER_DEFINED; break;
                case ExpressionKind.StringEq:
                case ExpressionKind.StringNotEq:
                case ExpressionKind.DelegateEq:
                case ExpressionKind.DelegateNotEq:
                case ExpressionKind.Eq:
                case ExpressionKind.NotEq:
                case ExpressionKind.GreaterThanOrEqual:
                case ExpressionKind.GreaterThan:
                case ExpressionKind.LessThanOrEqual:
                case ExpressionKind.LessThan:
                    return GenerateUserDefinedComparisonOperator(expr);
                case ExpressionKind.DelegateSubtract:
                case ExpressionKind.Subtract:
                    pdm = expr.isChecked() ? PREDEFMETH.PM_EXPRESSION_SUBTRACTCHECKED_USER_DEFINED : PREDEFMETH.PM_EXPRESSION_SUBTRACT_USER_DEFINED;
                    break;
                case ExpressionKind.DelegateAdd:
                case ExpressionKind.Add:
                    pdm = expr.isChecked() ? PREDEFMETH.PM_EXPRESSION_ADDCHECKED_USER_DEFINED : PREDEFMETH.PM_EXPRESSION_ADD_USER_DEFINED;
                    break;
                case ExpressionKind.Multiply:
                    pdm = expr.isChecked() ? PREDEFMETH.PM_EXPRESSION_MULTIPLYCHECKED_USER_DEFINED : PREDEFMETH.PM_EXPRESSION_MULTIPLY_USER_DEFINED;
                    break;
                default:
                    throw Error.InternalCompilerError();
            }
            Expr p1 = expr.OptionalLeftChild;
            Expr p2 = expr.OptionalRightChild;
            Expr udcall = expr.OptionalUserDefinedCall;
            if (udcall != null)
            {
                Debug.Assert(udcall.Kind == ExpressionKind.Call || udcall.Kind == ExpressionKind.UserLogicalOp);
                if (udcall is ExprCall ascall)
                {
                    ExprList args = (ExprList)ascall.OptionalArguments;
                    Debug.Assert(args.OptionalNextListNode.Kind != ExpressionKind.List);
                    p1 = args.OptionalElement;
                    p2 = args.OptionalNextListNode;
                }
                else
                {
                    ExprUserLogicalOp userLogOp = udcall as ExprUserLogicalOp;
                    Debug.Assert(userLogOp != null);
                    ExprList args = (ExprList)userLogOp.OperatorCall.OptionalArguments;
                    Debug.Assert(args.OptionalNextListNode.Kind != ExpressionKind.List);
                    p1 = ((ExprWrap)args.OptionalElement).OptionalExpression;
                    p2 = args.OptionalNextListNode;
                }
            }
            p1 = Visit(p1);
            p2 = Visit(p2);
            FixLiftedUserDefinedBinaryOperators(expr, ref p1, ref p2);
            Expr methodInfo = GetExprFactory().CreateMethodInfo(expr.UserDefinedCallMethod);
            Expr call = GenerateCall(pdm, p1, p2, methodInfo);
            // Delegate add/subtract generates a call to Combine/Remove, which returns System.Delegate,
            // not the operand delegate CType.  We must cast to the delegate CType.
            if (expr.Kind == ExpressionKind.DelegateSubtract || expr.Kind == ExpressionKind.DelegateAdd)
            {
                Expr pTypeOf = CreateTypeOf(expr.Type);
                return GenerateCall(PREDEFMETH.PM_EXPRESSION_CONVERT, call, pTypeOf);
            }
            return call;
        }

        private Expr GenerateUserDefinedUnaryOperator(ExprUnaryOp expr)
        {
            Debug.Assert(expr != null);
            PREDEFMETH pdm;
            Expr arg = expr.Child;
            ExprCall call = (ExprCall)expr.OptionalUserDefinedCall;
            if (call != null)
            {
                // Use the actual argument of the call; it may contain user-defined
                // conversions or be a bound lambda, and that will not be in the original
                // argument stashed away in the left child of the operator.
                arg = call.OptionalArguments;
            }
            Debug.Assert(arg != null && arg.Kind != ExpressionKind.List);
            switch (expr.Kind)
            {
                case ExpressionKind.True:
                case ExpressionKind.False:
                    return Visit(call);
                case ExpressionKind.UnaryPlus:
                    pdm = PREDEFMETH.PM_EXPRESSION_UNARYPLUS_USER_DEFINED;
                    break;
                case ExpressionKind.BitwiseNot: pdm = PREDEFMETH.PM_EXPRESSION_NOT_USER_DEFINED; break;
                case ExpressionKind.LogicalNot: pdm = PREDEFMETH.PM_EXPRESSION_NOT_USER_DEFINED; break;
                case ExpressionKind.DecimalNegate:
                case ExpressionKind.Negate:
                    pdm = expr.isChecked() ? PREDEFMETH.PM_EXPRESSION_NEGATECHECKED_USER_DEFINED : PREDEFMETH.PM_EXPRESSION_NEGATE_USER_DEFINED;
                    break;

                case ExpressionKind.Inc:
                case ExpressionKind.Dec:
                case ExpressionKind.DecimalInc:
                case ExpressionKind.DecimalDec:
                    pdm = PREDEFMETH.PM_EXPRESSION_CALL;
                    break;

                default:
                    throw Error.InternalCompilerError();
            }
            Expr op = Visit(arg);
            Expr methodInfo = GetExprFactory().CreateMethodInfo(expr.UserDefinedCallMethod);

            if (expr.Kind == ExpressionKind.Inc || expr.Kind == ExpressionKind.Dec ||
                expr.Kind == ExpressionKind.DecimalInc || expr.Kind == ExpressionKind.DecimalDec)
            {
                return GenerateCall(pdm, null, methodInfo, GenerateParamsArray(op, PredefinedType.PT_EXPRESSION));
            }
            return GenerateCall(pdm, op, methodInfo);
        }

        private Expr GenerateUserDefinedComparisonOperator(ExprBinOp expr)
        {
            Debug.Assert(expr != null);
            PREDEFMETH pdm;

            switch (expr.Kind)
            {
                case ExpressionKind.StringEq: pdm = PREDEFMETH.PM_EXPRESSION_EQUAL_USER_DEFINED; break;
                case ExpressionKind.StringNotEq: pdm = PREDEFMETH.PM_EXPRESSION_NOTEQUAL_USER_DEFINED; break;
                case ExpressionKind.DelegateEq: pdm = PREDEFMETH.PM_EXPRESSION_EQUAL_USER_DEFINED; break;
                case ExpressionKind.DelegateNotEq: pdm = PREDEFMETH.PM_EXPRESSION_NOTEQUAL_USER_DEFINED; break;
                case ExpressionKind.Eq: pdm = PREDEFMETH.PM_EXPRESSION_EQUAL_USER_DEFINED; break;
                case ExpressionKind.NotEq: pdm = PREDEFMETH.PM_EXPRESSION_NOTEQUAL_USER_DEFINED; break;
                case ExpressionKind.LessThanOrEqual: pdm = PREDEFMETH.PM_EXPRESSION_LESSTHANOREQUAL_USER_DEFINED; break;
                case ExpressionKind.LessThan: pdm = PREDEFMETH.PM_EXPRESSION_LESSTHAN_USER_DEFINED; break;
                case ExpressionKind.GreaterThanOrEqual: pdm = PREDEFMETH.PM_EXPRESSION_GREATERTHANOREQUAL_USER_DEFINED; break;
                case ExpressionKind.GreaterThan: pdm = PREDEFMETH.PM_EXPRESSION_GREATERTHAN_USER_DEFINED; break;
                default:
                    throw Error.InternalCompilerError();
            }
            Expr p1 = expr.OptionalLeftChild;
            Expr p2 = expr.OptionalRightChild;
            if (expr.OptionalUserDefinedCall != null)
            {
                ExprCall udcall = (ExprCall)expr.OptionalUserDefinedCall;
                ExprList args = (ExprList)udcall.OptionalArguments;
                Debug.Assert(args.OptionalNextListNode.Kind != ExpressionKind.List);

                p1 = args.OptionalElement;
                p2 = args.OptionalNextListNode;
            }
            p1 = Visit(p1);
            p2 = Visit(p2);
            FixLiftedUserDefinedBinaryOperators(expr, ref p1, ref p2);
            Expr lift = GetExprFactory().CreateBoolConstant(false); // We never lift to null in C#.
            Expr methodInfo = GetExprFactory().CreateMethodInfo(expr.UserDefinedCallMethod);
            return GenerateCall(pdm, p1, p2, lift, methodInfo);
        }

        private Expr RewriteLambdaBody(ExprBoundLambda anonmeth)
        {
            Debug.Assert(anonmeth != null);
            Debug.Assert(anonmeth.OptionalBody != null);
            Debug.Assert(anonmeth.OptionalBody.OptionalStatements != null);
            // There ought to be no way to get an empty statement block successfully converted into an expression tree.
            Debug.Assert(anonmeth.OptionalBody.OptionalStatements.OptionalNextStatement== null);

            ExprBlock body = anonmeth.OptionalBody;

            // The most likely case:
            if (body.OptionalStatements is ExprReturn ret)
            {
                Debug.Assert(ret.OptionalObject != null);
                return Visit(ret.OptionalObject);
            }
            // This can only if it is a void delegate and this is a void expression, such as a call to a void method
            // or something like Expression<Action<Foo>> e = (Foo f) => f.MyEvent += MyDelegate;

            throw Error.InternalCompilerError();
        }

        private Expr RewriteLambdaParameters(ExprBoundLambda anonmeth)
        {
            Debug.Assert(anonmeth != null);

            // new ParameterExpression[2] {Parameter(typeof(type1), name1), Parameter(typeof(type2), name2)}

            Expr paramArrayInitializerArgs = null;
            Expr paramArrayInitializerArgsTail = paramArrayInitializerArgs;

            for (Symbol sym = anonmeth.ArgumentScope; sym != null; sym = sym.nextChild)
            {
                if (!(sym is LocalVariableSymbol local))
                {
                    continue;
                }

                if (local.isThis)
                {
                    continue;
                }
                GetExprFactory().AppendItemToList(local.wrap, ref paramArrayInitializerArgs, ref paramArrayInitializerArgsTail);
            }

            return GenerateParamsArray(paramArrayInitializerArgs, PredefinedType.PT_PARAMETEREXPRESSION);
        }

        private Expr GenerateConversion(Expr arg, CType CType, bool bChecked)
        {
            return GenerateConversionWithSource(Visit(arg), CType, bChecked || arg.isChecked());
        }

        private Expr GenerateConversionWithSource(Expr pTarget, CType pType, bool bChecked)
        {
            PREDEFMETH pdm = bChecked ? PREDEFMETH.PM_EXPRESSION_CONVERTCHECKED : PREDEFMETH.PM_EXPRESSION_CONVERT;
            Expr pTypeOf = CreateTypeOf(pType);
            return GenerateCall(pdm, pTarget, pTypeOf);
        }

        private Expr GenerateValueAccessConversion(Expr pArgument)
        {
            Debug.Assert(pArgument != null);
            CType pStrippedTypeOfArgument = pArgument.Type.StripNubs();
            Expr pStrippedTypeExpr = CreateTypeOf(pStrippedTypeOfArgument);
            return GenerateCall(PREDEFMETH.PM_EXPRESSION_CONVERT, Visit(pArgument), pStrippedTypeExpr);
        }

        private Expr GenerateUserDefinedConversion(Expr arg, CType type, MethWithInst method)
        {
            Expr target = Visit(arg);
            return GenerateUserDefinedConversion(arg, type, target, method);
        }

        private Expr GenerateUserDefinedConversion(Expr arg, CType CType, Expr target, MethWithInst method)
        {
            // The user-defined explicit conversion from enum? to decimal or decimal? requires
            // that we convert the enum? to its nullable underlying CType.
            if (isEnumToDecimalConversion(arg.Type, CType))
            {
                // Special case: If we have enum? to decimal? then we need to emit 
                // a conversion from enum? to its nullable underlying CType first.
                // This is unfortunate; we ought to reorganize how conversions are 
                // represented in the Expr tree so that this is more transparent.

                // converting an enum to its underlying CType never fails, so no need to check it.
                CType underlyingType = arg.Type.StripNubs().underlyingEnumType();
                CType nullableType = GetSymbolLoader().GetTypeManager().GetNullable(underlyingType);
                Expr typeofNubEnum = CreateTypeOf(nullableType);
                target = GenerateCall(PREDEFMETH.PM_EXPRESSION_CONVERT, target, typeofNubEnum);
            }

            // If the methodinfo does not return the target CType AND this is not a lifted conversion
            // from one value CType to another, then we need to wrap the whole thing in another conversion,
            // e.g. if we have a user-defined conversion from int to S? and we have (S)myint, then we need to generate 
            // Convert(Convert(myint, typeof(S?), op_implicit), typeof(S))

            CType pMethodReturnType = GetSymbolLoader().GetTypeManager().SubstType(method.Meth().RetType,
                method.GetType(), method.TypeArgs);
            bool fDontLiftReturnType = (pMethodReturnType == CType || (IsNullableValueType(arg.Type) && IsNullableValueType(CType)));

            Expr typeofInner = CreateTypeOf(fDontLiftReturnType ? CType : pMethodReturnType);
            Expr methodInfo = GetExprFactory().CreateMethodInfo(method);
            PREDEFMETH pdmInner = arg.isChecked() ? PREDEFMETH.PM_EXPRESSION_CONVERTCHECKED_USER_DEFINED : PREDEFMETH.PM_EXPRESSION_CONVERT_USER_DEFINED;
            Expr callUserDefinedConversion = GenerateCall(pdmInner, target, typeofInner, methodInfo);

            if (fDontLiftReturnType)
            {
                return callUserDefinedConversion;
            }

            PREDEFMETH pdmOuter = arg.isChecked() ? PREDEFMETH.PM_EXPRESSION_CONVERTCHECKED : PREDEFMETH.PM_EXPRESSION_CONVERT;
            Expr typeofOuter = CreateTypeOf(CType);
            return GenerateCall(pdmOuter, callUserDefinedConversion, typeofOuter);
        }

        private Expr GenerateUserDefinedConversion(ExprUserDefinedConversion pExpr, Expr pArgument)
        {
            Expr pCastCall = pExpr.UserDefinedCall;
            Expr pCastArgument = pExpr.Argument;
            Expr pConversionSource;

            if (!isEnumToDecimalConversion(pArgument.Type, pExpr.Type)
                && IsNullableValueAccess(pCastArgument, pArgument))
            {
                // We have an implicit conversion of nullable CType to the value CType, generate a convert node for it.
                pConversionSource = GenerateValueAccessConversion(pArgument);
            }
            else
            {
                ExprCall call = pCastCall as ExprCall;
                Expr pUDConversion = call?.PConversions;
                if (pUDConversion != null)
                {
                    if (pUDConversion is ExprCall convCall)
                    {
                        Expr pUDConversionArgument = convCall.OptionalArguments;
                        if (IsNullableValueAccess(pUDConversionArgument, pArgument))
                        {
                            pConversionSource = GenerateValueAccessConversion(pArgument);
                        }
                        else
                        {
                            pConversionSource = Visit(pUDConversionArgument);
                        }

                        return GenerateConversionWithSource(pConversionSource, pCastCall.Type, call.isChecked());
                    }

                    // This can happen if we have a UD conversion from C to, say, int, 
                    // and we have an explicit cast to decimal?. The conversion should
                    // then be bound as two chained user-defined conversions.
                    Debug.Assert(pUDConversion is ExprUserDefinedConversion);

                    // Just recurse.
                    return GenerateUserDefinedConversion((ExprUserDefinedConversion)pUDConversion, pArgument);
                }

                pConversionSource = Visit(pCastArgument);
            }

            return GenerateUserDefinedConversion(pCastArgument, pExpr.Type, pConversionSource, pExpr.UserDefinedCallMethod);
        }

        private Expr GenerateParameter(string name, CType CType)
        {
            GetSymbolLoader().GetPredefindType(PredefinedType.PT_STRING);  // force an ensure state
            ExprConstant nameString = GetExprFactory().CreateStringConstant(name);
            ExprTypeOf pTypeOf = CreateTypeOf(CType);
            return GenerateCall(PREDEFMETH.PM_EXPRESSION_PARAMETER, pTypeOf, nameString);
        }

        private MethodSymbol GetPreDefMethod(PREDEFMETH pdm)
        {
            return GetSymbolLoader().getPredefinedMembers().GetMethod(pdm);
        }

        private ExprTypeOf CreateTypeOf(CType CType)
        {
            return GetExprFactory().CreateTypeOf(CType);
        }

        private Expr CreateWraps(ExprBoundLambda anonmeth)
        {
            Expr sequence = null;
            for (Symbol sym = anonmeth.ArgumentScope.firstChild; sym != null; sym = sym.nextChild)
            {
                if (!(sym is LocalVariableSymbol local))
                {
                    continue;
                }

                if (local.isThis)
                {
                    continue;
                }
                Debug.Assert(anonmeth.OptionalBody != null);
                Expr create = GenerateParameter(local.name.Text, local.GetType());
                local.wrap = GetExprFactory().CreateWrap(create);
                Expr save = GetExprFactory().CreateSave(local.wrap);
                if (sequence == null)
                {
                    sequence = save;
                }
                else
                {
                    sequence = GetExprFactory().CreateSequence(sequence, save);
                }
            }

            return sequence;
        }

        private Expr DestroyWraps(ExprBoundLambda anonmeth, Expr sequence)
        {
            for (Symbol sym = anonmeth.ArgumentScope; sym != null; sym = sym.nextChild)
            {
                if (!(sym is LocalVariableSymbol local))
                {
                    continue;
                }

                if (local.isThis)
                {
                    continue;
                }
                Debug.Assert(local.wrap != null);
                Debug.Assert(anonmeth.OptionalBody != null);
                Expr freeWrap = GetExprFactory().CreateWrap(local.wrap);
                sequence = GetExprFactory().CreateReverseSequence(sequence, freeWrap);
            }
            return sequence;
        }

        private Expr GenerateConstructor(ExprCall expr)
        {
            Debug.Assert(expr != null);
            Debug.Assert(expr.MethWithInst.Meth().IsConstructor());

            // Realize a call to new DELEGATE(obj, FUNCPTR) as though it actually was
            // (DELEGATE)CreateDelegate(typeof(DELEGATE), obj, GetMethInfoFromHandle(FUNCPTR))

            if (IsDelegateConstructorCall(expr))
            {
                return GenerateDelegateConstructor(expr);
            }
            Expr constructorInfo = GetExprFactory().CreateMethodInfo(expr.MethWithInst);
            Expr args = GenerateArgsList(expr.OptionalArguments);
            Expr Params = GenerateParamsArray(args, PredefinedType.PT_EXPRESSION);
            return GenerateCall(PREDEFMETH.PM_EXPRESSION_NEW, constructorInfo, Params);
        }

        private Expr GenerateDelegateConstructor(ExprCall expr)
        {
            // In:
            //
            // new DELEGATE(obj, &FUNC)
            //
            // Out:
            //
            //  Cast(
            //      Call(
            //          null,
            //          (MethodInfo)GetMethodFromHandle(&CreateDelegate),
            //          new Expression[3]{
            //              Constant(typeof(DELEGATE)),
            //              transformed-object,
            //              Constant((MethodInfo)GetMethodFromHandle(&FUNC)}),
            //      typeof(DELEGATE))
            //

            Debug.Assert(expr != null);
            Debug.Assert(expr.MethWithInst.Meth().IsConstructor());
            Debug.Assert(expr.Type.isDelegateType());

            ExprList origArgs = (ExprList)expr.OptionalArguments;
            Debug.Assert(origArgs != null);
            Expr target = origArgs.OptionalElement;
            Debug.Assert(origArgs.OptionalNextListNode.Kind == ExpressionKind.FunctionPointer);
            ExprFuncPtr funcptr = (ExprFuncPtr)origArgs.OptionalNextListNode;
            Debug.Assert(funcptr != null);
            MethodSymbol createDelegateMethod = GetPreDefMethod(PREDEFMETH.PM_METHODINFO_CREATEDELEGATE_TYPE_OBJECT);
            AggregateType delegateType = GetSymbolLoader().GetPredefindType(PredefinedType.PT_DELEGATE);
            MethWithInst mwi = new MethWithInst(createDelegateMethod, delegateType);

            Expr instance = GenerateConstant(GetExprFactory().CreateMethodInfo(funcptr.MethWithInst));
            Expr methinfo = GetExprFactory().CreateMethodInfo(mwi);
            Expr param1 = GenerateConstant(CreateTypeOf(expr.Type));
            Expr param2 = Visit(target);
            Expr paramsList = GetExprFactory().CreateList(param1, param2);
            Expr Params = GenerateParamsArray(paramsList, PredefinedType.PT_EXPRESSION);
            Expr call = GenerateCall(PREDEFMETH.PM_EXPRESSION_CALL, instance, methinfo, Params);
            Expr pTypeOf = CreateTypeOf(expr.Type);
            return GenerateCall(PREDEFMETH.PM_EXPRESSION_CONVERT, call, pTypeOf);
        }

        private Expr GenerateArgsList(Expr oldArgs)
        {
            Expr newArgs = null;
            Expr newArgsTail = newArgs;
            for (ExpressionIterator it = new ExpressionIterator(oldArgs); !it.AtEnd(); it.MoveNext())
            {
                Expr oldArg = it.Current();
                GetExprFactory().AppendItemToList(Visit(oldArg), ref newArgs, ref newArgsTail);
            }
            return newArgs;
        }

        private Expr GenerateIndexList(Expr oldIndices)
        {
            CType intType = symbolLoader.GetPredefindType(PredefinedType.PT_INT);

            Expr newIndices = null;
            Expr newIndicesTail = newIndices;
            for (ExpressionIterator it = new ExpressionIterator(oldIndices); !it.AtEnd(); it.MoveNext())
            {
                Expr newIndex = it.Current();
                if (newIndex.Type != intType)
                {
                    ExprClass exprType = expressionFactory.CreateClass(intType);
                    newIndex = expressionFactory.CreateCast(EXPRFLAG.EXF_INDEXEXPR, exprType, newIndex);
                    newIndex.Flags |= EXPRFLAG.EXF_CHECKOVERFLOW;
                }
                Expr rewrittenIndex = Visit(newIndex);
                expressionFactory.AppendItemToList(rewrittenIndex, ref newIndices, ref newIndicesTail);
            }
            return newIndices;
        }

        private Expr GenerateConstant(Expr expr)
        {
            EXPRFLAG flags = 0;

            AggregateType pObject = GetSymbolLoader().GetPredefindType(PredefinedType.PT_OBJECT);

            if (expr.Type is NullType)
            {
                ExprTypeOf pTypeOf = CreateTypeOf(pObject);
                return GenerateCall(PREDEFMETH.PM_EXPRESSION_CONSTANT_OBJECT_TYPE, expr, pTypeOf);
            }

            AggregateType stringType = GetSymbolLoader().GetPredefindType(PredefinedType.PT_STRING);
            if (expr.Type != stringType)
            {
                flags = EXPRFLAG.EXF_BOX;
            }

            ExprClass objectType = GetExprFactory().CreateClass(pObject);
            ExprCast cast = GetExprFactory().CreateCast(flags, objectType, expr);
            ExprTypeOf pTypeOf2 = CreateTypeOf(expr.Type);

            return GenerateCall(PREDEFMETH.PM_EXPRESSION_CONSTANT_OBJECT_TYPE, cast, pTypeOf2);
        }

        private ExprCall GenerateCall(PREDEFMETH pdm, Expr arg1)
        {
            MethodSymbol method = GetPreDefMethod(pdm);
            // this should be enforced in an earlier pass and the transform pass should not 
            // be handling this error
            if (method == null)
                return null;
            AggregateType expressionType = GetSymbolLoader().GetPredefindType(PredefinedType.PT_EXPRESSION);
            MethWithInst mwi = new MethWithInst(method, expressionType);
            ExprMemberGroup pMemGroup = GetExprFactory().CreateMemGroup(null, mwi);
            ExprCall call = GetExprFactory().CreateCall(0, mwi.Meth().RetType, arg1, pMemGroup, mwi);
            call.PredefinedMethod = pdm;
            return call;
        }

        private ExprCall GenerateCall(PREDEFMETH pdm, Expr arg1, Expr arg2)
        {
            MethodSymbol method = GetPreDefMethod(pdm);
            if (method == null)
                return null;
            AggregateType expressionType = GetSymbolLoader().GetPredefindType(PredefinedType.PT_EXPRESSION);
            Expr args = GetExprFactory().CreateList(arg1, arg2);
            MethWithInst mwi = new MethWithInst(method, expressionType);
            ExprMemberGroup pMemGroup = GetExprFactory().CreateMemGroup(null, mwi);
            ExprCall call = GetExprFactory().CreateCall(0, mwi.Meth().RetType, args, pMemGroup, mwi);
            call.PredefinedMethod = pdm;
            return call;
        }

        private ExprCall GenerateCall(PREDEFMETH pdm, Expr arg1, Expr arg2, Expr arg3)
        {
            MethodSymbol method = GetPreDefMethod(pdm);
            if (method == null)
                return null;
            AggregateType expressionType = GetSymbolLoader().GetPredefindType(PredefinedType.PT_EXPRESSION);
            Expr args = GetExprFactory().CreateList(arg1, arg2, arg3);
            MethWithInst mwi = new MethWithInst(method, expressionType);
            ExprMemberGroup pMemGroup = GetExprFactory().CreateMemGroup(null, mwi);
            ExprCall call = GetExprFactory().CreateCall(0, mwi.Meth().RetType, args, pMemGroup, mwi);
            call.PredefinedMethod = pdm;
            return call;
        }

        private ExprCall GenerateCall(PREDEFMETH pdm, Expr arg1, Expr arg2, Expr arg3, Expr arg4)
        {
            MethodSymbol method = GetPreDefMethod(pdm);
            if (method == null)
                return null;
            AggregateType expressionType = GetSymbolLoader().GetPredefindType(PredefinedType.PT_EXPRESSION);
            Expr args = GetExprFactory().CreateList(arg1, arg2, arg3, arg4);
            MethWithInst mwi = new MethWithInst(method, expressionType);
            ExprMemberGroup pMemGroup = GetExprFactory().CreateMemGroup(null, mwi);
            ExprCall call = GetExprFactory().CreateCall(0, mwi.Meth().RetType, args, pMemGroup, mwi);
            call.PredefinedMethod = pdm;
            return call;
        }

        private ExprArrayInit GenerateParamsArray(Expr args, PredefinedType pt)
        {
            int parameterCount = ExpressionIterator.Count(args);
            AggregateType paramsArrayElementType = GetSymbolLoader().GetPredefindType(pt);
            ArrayType paramsArrayType = GetSymbolLoader().GetTypeManager().GetArray(paramsArrayElementType, 1, true);
            ExprConstant paramsArrayArg = GetExprFactory().CreateIntegerConstant(parameterCount);
            return GetExprFactory().CreateArrayInit(paramsArrayType, args, paramsArrayArg, new int[] { parameterCount }, parameterCount);
        }

        private void FixLiftedUserDefinedBinaryOperators(ExprBinOp expr, ref Expr pp1, ref Expr pp2)
        {
            // If we have lifted T1 op T2 to T1? op T2?, and we have an expression T1 op T2? or T1? op T2 then
            // we need to ensure that the unlifted actual arguments are promoted to their nullable CType.
            Debug.Assert(expr != null);
            Debug.Assert(pp1 != null);
            Debug.Assert(pp1 != null);
            Debug.Assert(pp2 != null);
            Debug.Assert(pp2 != null);
            MethodSymbol method = expr.UserDefinedCallMethod.Meth();
            Expr orig1 = expr.OptionalLeftChild;
            Expr orig2 = expr.OptionalRightChild;
            Debug.Assert(orig1 != null && orig2 != null);
            Expr new1 = pp1;
            Expr new2 = pp2;
            CType fptype1 = method.Params[0];
            CType fptype2 = method.Params[1];
            CType aatype1 = orig1.Type;
            CType aatype2 = orig2.Type;
            // Is the operator even a candidate for lifting?
            if (!(fptype1 is AggregateType fat1)
                || !fat1.getAggregate().IsValueType()
                || !(fptype2 is AggregateType fat2)
                || !fat2.getAggregate().IsValueType())
            {
                return;
            }

            CType nubfptype1 = GetSymbolLoader().GetTypeManager().GetNullable(fptype1);
            CType nubfptype2 = GetSymbolLoader().GetTypeManager().GetNullable(fptype2);
            // If we have null op X, or T1 op T2?, or T1 op null, lift first arg to T1?
            if (aatype1 is NullType || aatype1 == fptype1 && (aatype2 == nubfptype2 || aatype2 is NullType))
            {
                new1 = GenerateCall(PREDEFMETH.PM_EXPRESSION_CONVERT, new1, CreateTypeOf(nubfptype1));
            }

            // If we have X op null, or T1? op T2, or null op T2, lift second arg to T2?
            if (aatype2 is NullType || aatype2 == fptype2 && (aatype1 == nubfptype1 || aatype1 is NullType))
            {
                new2 = GenerateCall(PREDEFMETH.PM_EXPRESSION_CONVERT, new2, CreateTypeOf(nubfptype2));
            }
            pp1 = new1;
            pp2 = new2;
        }

        private bool IsNullableValueType(CType pType) =>
            pType is NullableType && pType.StripNubs() is AggregateType agg && agg.getAggregate().IsValueType();

        private bool IsNullableValueAccess(Expr pExpr, Expr pObject)
        {
            Debug.Assert(pExpr != null);
            return pExpr is ExprProperty prop && prop.MemberGroup.OptionalObject == pObject && pObject.Type is NullableType;
        }

        private bool IsDelegateConstructorCall(Expr pExpr)
        {
            Debug.Assert(pExpr != null);
            if (!(pExpr is ExprCall pCall))
            {
                return false;
            }

            return pCall.MethWithInst.Meth() != null &&
                pCall.MethWithInst.Meth().IsConstructor() &&
                pCall.Type.isDelegateType() &&
                pCall.OptionalArguments != null &&
                pCall.OptionalArguments is ExprList list &&
                list.OptionalNextListNode.Kind == ExpressionKind.FunctionPointer;
        }
        private static bool isEnumToDecimalConversion(CType argtype, CType desttype) =>
            argtype.StripNubs().isEnumType() && desttype.StripNubs().isPredefType(PredefinedType.PT_DECIMAL);
    }
}
