// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using Microsoft.CSharp.RuntimeBinder.Syntax;

namespace Microsoft.CSharp.RuntimeBinder.Semantics
{
    internal sealed class ExprFactory
    {
        private readonly GlobalSymbolContext _globalSymbolContext;

        public ExprFactory(GlobalSymbolContext globalSymbolContext)
        {
            Debug.Assert(globalSymbolContext != null);
            _globalSymbolContext = globalSymbolContext;
        }
        private TypeManager GetTypes()
        {
            return _globalSymbolContext.GetTypes();
        }
        private BSYMMGR GetGlobalSymbols()
        {
            return _globalSymbolContext.GetGlobalSymbols();
        }

        public ExprCall CreateCall(EXPRFLAG nFlags, CType pType, Expr pOptionalArguments, ExprMemberGroup pMemberGroup, MethWithInst MWI)
        {
            Debug.Assert(0 == (nFlags &
               ~(
                   EXPRFLAG.EXF_NEWOBJCALL | EXPRFLAG.EXF_CONSTRAINED | EXPRFLAG.EXF_BASECALL |
                   EXPRFLAG.EXF_NEWSTRUCTASSG |
                   EXPRFLAG.EXF_IMPLICITSTRUCTASSG | EXPRFLAG.EXF_MASK_ANY
               )
              ));

            return new ExprCall(pType, nFlags, pOptionalArguments, pMemberGroup, MWI);
        }

        public ExprField CreateField(EXPRFLAG nFlags, CType pType, Expr pOptionalObject, FieldWithType FWT)
        {
            Debug.Assert(0 == (nFlags & ~(EXPRFLAG.EXF_MEMBERSET | EXPRFLAG.EXF_MASK_ANY)));
            return new ExprField(pType, nFlags, pOptionalObject, FWT);
        }

        public ExprFuncPtr CreateFunctionPointer(EXPRFLAG nFlags, CType pType, Expr pObject, MethWithInst MWI)
        {
            Debug.Assert(0 == (nFlags & ~EXPRFLAG.EXF_BASECALL));
            return new ExprFuncPtr(pType, nFlags, pObject, MWI);
        }

        public ExprArrayInit CreateArrayInit(CType pType, Expr pOptionalArguments, Expr pOptionalArgumentDimensions, int[] pDimSizes)
        {
            return new ExprArrayInit(pType, pOptionalArguments, pOptionalArgumentDimensions, pDimSizes);
        }

        public ExprProperty CreateProperty(CType pType, Expr pOptionalObject)
        {
            MethPropWithInst mwi = new MethPropWithInst();
            ExprMemberGroup pMemGroup = CreateMemGroup(pOptionalObject, mwi);
            return CreateProperty(pType, null, null, pMemGroup, null, null);
        }

        public ExprProperty CreateProperty(CType pType, Expr pOptionalObjectThrough, Expr pOptionalArguments, ExprMemberGroup pMemberGroup, PropWithType pwtSlot, MethWithType mwtSet)
        {
            return new ExprProperty(pType, pOptionalObjectThrough, pOptionalArguments, pMemberGroup, pwtSlot, mwtSet);
        }

        public ExprEvent CreateEvent(CType pType, Expr pOptionalObject, EventWithType EWT)
        {
            return new ExprEvent(pType, pOptionalObject, EWT);
        }

        public ExprMemberGroup CreateMemGroup(EXPRFLAG nFlags, Name pName, TypeArray pTypeArgs, SYMKIND symKind, CType pTypePar, MethodOrPropertySymbol pMPS, Expr pObject, CMemberLookupResults memberLookupResults)
        {
            Debug.Assert(0 == (nFlags & ~(
                           EXPRFLAG.EXF_CTOR | EXPRFLAG.EXF_INDEXER | EXPRFLAG.EXF_OPERATOR | EXPRFLAG.EXF_NEWOBJCALL |
                           EXPRFLAG.EXF_BASECALL | EXPRFLAG.EXF_DELEGATE | EXPRFLAG.EXF_USERCALLABLE | EXPRFLAG.EXF_MASK_ANY
                       )
                      ));
            return new ExprMemberGroup(GetTypes().GetMethGrpType(), nFlags, pName, pTypeArgs, symKind, pTypePar, pMPS, pObject, memberLookupResults);
        }

        public ExprMemberGroup CreateMemGroup(
                Expr pObject,
                MethPropWithInst mwi)
        {
            Name pName = mwi.Sym?.name;
            MethodOrPropertySymbol methProp = mwi.MethProp();

            CType pType = mwi.GetType() ?? (CType)GetTypes().GetErrorSym();

            return CreateMemGroup(0, pName, mwi.TypeArgs, methProp?.getKind() ?? SYMKIND.SK_MethodSymbol, mwi.GetType(), methProp, pObject, new CMemberLookupResults(GetGlobalSymbols().AllocParams(1, new CType[] { pType }), pName));
        }

        public ExprUserDefinedConversion CreateUserDefinedConversion(Expr arg, Expr call, MethWithInst mwi)
        {
            Debug.Assert(arg != null);
            Debug.Assert(call != null);
            return new ExprUserDefinedConversion(arg, call, mwi);
        }

        public ExprCast CreateCast(EXPRFLAG nFlags, CType pType, Expr pArg)
        {
            return CreateCast(nFlags, CreateClass(pType), pArg);
        }

        public ExprCast CreateCast(EXPRFLAG nFlags, ExprClass pType, Expr pArg)
        {
            Debug.Assert(pArg != null);
            Debug.Assert(pType != null);
            Debug.Assert(0 == (nFlags & ~(EXPRFLAG.EXF_CAST_ALL | EXPRFLAG.EXF_MASK_ANY)));
            return new ExprCast(nFlags, pType, pArg);
        }

        public ExprReturn CreateReturn(EXPRFLAG nFlags, Expr pOptionalObject)
        {
            Debug.Assert(0 == (nFlags &
                       ~(EXPRFLAG.EXF_ASLEAVE | EXPRFLAG.EXF_FINALLYBLOCKED | EXPRFLAG.EXF_RETURNISYIELD |
                          EXPRFLAG.EXF_ASFINALLYLEAVE | EXPRFLAG.EXF_GENERATEDSTMT | EXPRFLAG.EXF_MARKING |
                          EXPRFLAG.EXF_MASK_ANY
                        )
                      ));
            return new ExprReturn(nFlags, pOptionalObject);
        }

        public ExprLocal CreateLocal(EXPRFLAG nFlags, LocalVariableSymbol pLocal)
        {
            Debug.Assert(0 == (nFlags & ~EXPRFLAG.EXF_MASK_ANY));
            return new ExprLocal(nFlags, pLocal);
        }

        public ExprBoundLambda CreateAnonymousMethod(AggregateType delegateType)
        {
            Debug.Assert(delegateType == null || delegateType.isDelegateType());
            return new ExprBoundLambda(delegateType);
        }

        public ExprUnboundLambda CreateLambda()
        {
            return new ExprUnboundLambda(GetTypes().GetAnonMethType());
        }

        public ExprHoistedLocalExpr CreateHoistedLocalInExpression(ExprLocal localToHoist)
        {
            Debug.Assert(localToHoist != null);
            return new ExprHoistedLocalExpr(GetTypes().GetOptPredefAgg(PredefinedType.PT_EXPRESSION).getThisType());
        }

        public ExprMethodInfo CreateMethodInfo(MethPropWithInst mwi)
        {
            return CreateMethodInfo(mwi.Meth(), mwi.GetType(), mwi.TypeArgs);
        }

        public ExprMethodInfo CreateMethodInfo(MethodSymbol method, AggregateType methodType, TypeArray methodParameters)
        {
            Debug.Assert(method != null);
            Debug.Assert(methodType != null);
            return new ExprMethodInfo(
                GetTypes().GetOptPredefAgg(method.IsConstructor() ? PredefinedType.PT_CONSTRUCTORINFO : PredefinedType.PT_METHODINFO).getThisType(),
                method, methodType, methodParameters);
        }

        public ExprPropertyInfo CreatePropertyInfo(PropertySymbol prop, AggregateType propertyType)
        {
            Debug.Assert(prop != null);
            Debug.Assert(propertyType != null);
            return new ExprPropertyInfo(
                GetTypes().GetOptPredefAgg(PredefinedType.PT_PROPERTYINFO).getThisType(), prop, propertyType);
        }

        public ExprFieldInfo CreateFieldInfo(FieldSymbol field, AggregateType fieldType)
        {
            Debug.Assert(field != null);
            Debug.Assert(fieldType != null);
            return new ExprFieldInfo(field, fieldType, GetTypes().GetOptPredefAgg(PredefinedType.PT_FIELDINFO).getThisType());
        }

        private ExprTypeOf CreateTypeOf(ExprClass pSourceType)
        {
            return new ExprTypeOf(GetTypes().GetReqPredefAgg(PredefinedType.PT_TYPE).getThisType(), pSourceType);
        }
        public ExprTypeOf CreateTypeOf(CType pSourceType)
        {
            return CreateTypeOf(CreateClass(pSourceType));
        }

        public ExprUserLogicalOp CreateUserLogOp(CType pType, Expr pCallTF, ExprCall pCallOp)
        {
            Debug.Assert(pCallTF != null);
            Debug.Assert((pCallOp?.OptionalArguments as ExprList)?.OptionalElement != null);
            return new ExprUserLogicalOp(pType, pCallTF, pCallOp);
        }

        public ExprUserLogicalOp CreateUserLogOpError(CType pType, Expr pCallTF, ExprCall pCallOp)
        {
            ExprUserLogicalOp rval = CreateUserLogOp(pType, pCallTF, pCallOp);
            rval.SetError();
            return rval;
        }

        public ExprConcat CreateConcat(Expr op1, Expr op2)
        {
            Debug.Assert(op1?.Type != null);
            Debug.Assert(op2?.Type != null);
            Debug.Assert(op1.Type.isPredefType(PredefinedType.PT_STRING) || op2.Type.isPredefType(PredefinedType.PT_STRING));
            return new ExprConcat(op1, op2);
        }

        public ExprConstant CreateStringConstant(string str)
        {
            return CreateConstant(GetTypes().GetReqPredefAgg(PredefinedType.PT_STRING).getThisType(), ConstVal.Get(str));
        }

        public ExprMultiGet CreateMultiGet(EXPRFLAG nFlags, CType pType, ExprMulti pOptionalMulti)
        {
            Debug.Assert(0 == (nFlags & ~EXPRFLAG.EXF_MASK_ANY));
            return new ExprMultiGet(pType, nFlags, pOptionalMulti);
        }

        public ExprMulti CreateMulti(EXPRFLAG nFlags, CType pType, Expr pLeft, Expr pOp)
        {
            Debug.Assert(pLeft != null);
            Debug.Assert(pOp != null);
            return new ExprMulti(pType, nFlags, pLeft, pOp);
        }

        ////////////////////////////////////////////////////////////////////////////////
        //
        // Precondition:
        //
        // pType - Non-null
        //
        // This returns a null for reference types and an EXPRZEROINIT for all others.

        public Expr CreateZeroInit(CType pType)
        {
            return CreateZeroInit(CreateClass(pType), null, false);
        }

        private Expr CreateZeroInit(ExprClass pTypeExpr, Expr pOptionalOriginalConstructorCall, bool isConstructor)
        {
            Debug.Assert(pTypeExpr != null);
            CType pType = pTypeExpr.Type;
            bool bIsError = false;

            if (pType.isEnumType())
            {
                // For enum types, we create a constant that has the default value
                // as an object pointer.
                return CreateConstant(pType, ConstVal.Get(Activator.CreateInstance(pType.AssociatedSystemType)));
            }

            switch (pType.fundType())
            {
                case FUNDTYPE.FT_PTR:
                    {
                        CType nullType = GetTypes().GetNullType();

                        // It looks like this if is always false ...
                        if (nullType.fundType() == pType.fundType())
                        {
                            // Create a constant here.
                            return CreateConstant(pType, ConstVal.GetDefaultValue(ConstValKind.IntPtr));
                        }

                        // Just allocate a new node and fill it in.
                        return CreateCast(0, pTypeExpr, CreateNull());
                    }

                case FUNDTYPE.FT_REF:
                case FUNDTYPE.FT_I1:
                case FUNDTYPE.FT_U1:
                case FUNDTYPE.FT_I2:
                case FUNDTYPE.FT_U2:
                case FUNDTYPE.FT_I4:
                case FUNDTYPE.FT_U4:
                case FUNDTYPE.FT_I8:
                case FUNDTYPE.FT_U8:
                case FUNDTYPE.FT_R4:
                case FUNDTYPE.FT_R8:
                    {
                        ExprConstant expr = CreateConstant(pType, ConstVal.GetDefaultValue(pType.constValKind()));
                        ExprConstant exprInOriginal = CreateConstant(pType, ConstVal.GetDefaultValue(pType.constValKind()));
                        exprInOriginal.OptionalConstructorCall = pOptionalOriginalConstructorCall;
                        return expr;
                    }
                case FUNDTYPE.FT_STRUCT:
                    if (pType.isPredefType(PredefinedType.PT_DECIMAL))
                    {
                        ExprConstant expr = CreateConstant(pType, ConstVal.GetDefaultValue(pType.constValKind()));
                        ExprConstant exprOriginal = CreateConstant(pType, ConstVal.GetDefaultValue(pType.constValKind()));
                        exprOriginal.OptionalConstructorCall = pOptionalOriginalConstructorCall;
                        return expr;
                    }

                    break;

                case FUNDTYPE.FT_VAR:
                    break;

                default:
                    bIsError = true;
                    break;
            }

            return new ExprZeroInit(pType, pOptionalOriginalConstructorCall, isConstructor, bIsError);
        }

        public ExprConstant CreateConstant(CType pType, ConstVal constVal)
        {
            return new ExprConstant(pType, constVal);
        }

        public ExprConstant CreateIntegerConstant(int x)
        {
            return CreateConstant(GetTypes().GetReqPredefAgg(PredefinedType.PT_INT).getThisType(), ConstVal.Get(x));
        }
        public ExprConstant CreateBoolConstant(bool b)
        {
            return CreateConstant(GetTypes().GetReqPredefAgg(PredefinedType.PT_BOOL).getThisType(), ConstVal.Get(b));
        }

        public ExprBlock CreateBlock(ExprStatement pOptionalStatements, Scope pOptionalScope)
        {
            return new ExprBlock(pOptionalStatements, pOptionalScope);
        }

        public ExprArrayIndex CreateArrayIndex(Expr pArray, Expr pIndex)
        {
            CType pType = pArray.Type;

            if (pType != null && pType.IsArrayType())
            {
                pType = pType.AsArrayType().GetElementType();
            }
            else if (pType == null)
            {
                pType = GetTypes().GetReqPredefAgg(PredefinedType.PT_INT).getThisType();
            }

            return new ExprArrayIndex(pType, pArray, pIndex);
        }

        public ExprBinOp CreateBinop(ExpressionKind exprKind, CType pType, Expr p1, Expr p2)
        {
            return new ExprBinOp(exprKind, pType, p1, p2);
        }

        public ExprUnaryOp CreateUnaryOp(ExpressionKind exprKind, CType pType, Expr pOperand)
        {
            Debug.Assert(exprKind.IsUnaryOperator());
            Debug.Assert(pOperand != null);
            return new ExprUnaryOp(exprKind, pType, pOperand);
        }

        public ExprOperator CreateOperator(ExpressionKind exprKind, CType pType, Expr pArg1, Expr pOptionalArg2)
        {
            Debug.Assert(pArg1 != null);
            Debug.Assert(exprKind.IsUnaryOperator() ? pOptionalArg2 == null : pOptionalArg2 != null);
            return exprKind.IsUnaryOperator()
                ? (ExprOperator)CreateUnaryOp(exprKind, pType, pArg1)
                : CreateBinop(exprKind, pType, pArg1, pOptionalArg2);
        }


        public ExprBinOp CreateUserDefinedBinop(ExpressionKind exprKind, CType pType, Expr p1, Expr p2, Expr call, MethPropWithInst pmpwi)
        {
            Debug.Assert(p1 != null);
            Debug.Assert(p2 != null);
            Debug.Assert(call != null);
            return new ExprBinOp(exprKind, pType, p1, p2, call, pmpwi);
        }

        public ExprUnaryOp CreateUserDefinedUnaryOperator(ExpressionKind exprKind, CType pType, Expr pOperand, ExprCall call, MethPropWithInst pmpwi)
        {
            Debug.Assert(pType != null);
            Debug.Assert(pOperand != null);
            Debug.Assert(call != null);
            Debug.Assert(pmpwi != null);
            // The call may be lifted, but we do not mark the outer binop as lifted.
            return new ExprUnaryOp(exprKind, pType, pOperand, call, pmpwi);
        }

        public ExprUnaryOp CreateNeg(EXPRFLAG nFlags, Expr pOperand)
        {
            Debug.Assert(pOperand != null);
            ExprUnaryOp pUnaryOp = CreateUnaryOp(ExpressionKind.Negate, pOperand.Type, pOperand);
            pUnaryOp.Flags |= nFlags;
            return pUnaryOp;
        }

        ////////////////////////////////////////////////////////////////////////////////
        // Create a node that evaluates the first, evaluates the second, results in the second.

        public ExprBinOp CreateSequence(Expr p1, Expr p2)
        {
            Debug.Assert(p1 != null);
            Debug.Assert(p2 != null);
            return CreateBinop(ExpressionKind.Sequence, p2.Type, p1, p2);
        }

        ////////////////////////////////////////////////////////////////////////////////
        // Create a node that evaluates the first, evaluates the second, results in the first.

        public ExprBinOp CreateReverseSequence(Expr p1, Expr p2)
        {
            Debug.Assert(p1 != null);
            Debug.Assert(p2 != null);
            return CreateBinop(ExpressionKind.SequenceReverse, p1.Type, p1, p2);
        }

        public ExprAssignment CreateAssignment(Expr pLHS, Expr pRHS)
        {
            return new ExprAssignment(pLHS, pRHS);
        }

        ////////////////////////////////////////////////////////////////////////////////

        public ExprNamedArgumentSpecification CreateNamedArgumentSpecification(Name pName, Expr pValue)
        {
            return new ExprNamedArgumentSpecification(pName, pValue);
        }

        public ExprWrap CreateWrap(Expr pOptionalExpression) => new ExprWrap(pOptionalExpression);

        public ExprBinOp CreateSave(ExprWrap wrap)
        {
            Debug.Assert(wrap != null);
            ExprBinOp expr = CreateBinop(ExpressionKind.Save, wrap.Type, wrap.OptionalExpression, wrap);
            expr.SetAssignment();
            return expr;
        }

        public ExprConstant CreateNull()
        {
            return CreateConstant(GetTypes().GetNullType(), default(ConstVal));
        }

        public void AppendItemToList(
            Expr newItem,
            ref Expr first,
            ref Expr last
        )
        {
            if (newItem == null)
            {
                // Nothing changes.
                return;
            }
            if (first == null)
            {
                Debug.Assert(last == first);
                first = newItem;
                last = newItem;
                return;
            }
            if (first.Kind != ExpressionKind.List)
            {
                Debug.Assert(last == first);
                first = CreateList(first, newItem);
                last = first;
                return;
            }
            Debug.Assert((last as ExprList)?.OptionalNextListNode != null);
            Debug.Assert((last as ExprList).OptionalNextListNode.Kind != ExpressionKind.List);
            ExprList list = (ExprList)last;
            list.OptionalNextListNode = CreateList(list.OptionalNextListNode, newItem);
            last = list.OptionalNextListNode;
        }

        public ExprList CreateList(Expr op1, Expr op2)
        {
            return new ExprList(op1, op2);
        }
        public ExprList CreateList(Expr op1, Expr op2, Expr op3)
        {
            return CreateList(op1, CreateList(op2, op3));
        }
        public ExprList CreateList(Expr op1, Expr op2, Expr op3, Expr op4)
        {
            return CreateList(op1, CreateList(op2, CreateList(op3, op4)));
        }

        public ExprClass CreateClass(CType pType)
        {
            Debug.Assert(pType != null);
            return new ExprClass(pType);
        }
    }
}

