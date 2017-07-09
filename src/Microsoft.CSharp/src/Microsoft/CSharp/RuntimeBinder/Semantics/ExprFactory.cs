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
        private TypeManager Types => _globalSymbolContext.GetTypes();

        private BSYMMGR GlobalSymbols => _globalSymbolContext.GetGlobalSymbols();

        public ExprCall CreateCall(EXPRFLAG nFlags, CType pType, Expr pOptionalArguments, ExprMemberGroup pMemberGroup, MethWithInst MWI)
        {
            return new ExprCall(pType, nFlags, pOptionalArguments, pMemberGroup, MWI);
        }

        public ExprField CreateField(CType pType, Expr pOptionalObject, FieldWithType FWT, bool isLValue)
        {
            return new ExprField(pType, pOptionalObject, FWT, isLValue);
        }

        public ExprFuncPtr CreateFunctionPointer(EXPRFLAG nFlags, CType pType, Expr pObject, MethWithInst MWI)
        {
            return new ExprFuncPtr(pType, nFlags, pObject, MWI);
        }

        public ExprArrayInit CreateArrayInit(CType pType, Expr pOptionalArguments, Expr pOptionalArgumentDimensions, int[] pDimSizes, int dimSize)
        {
            return new ExprArrayInit(pType, pOptionalArguments, pOptionalArgumentDimensions, pDimSizes, dimSize);
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
            return new ExprMemberGroup(Types.GetMethGrpType(), nFlags, pName, pTypeArgs, symKind, pTypePar, pMPS, pObject, memberLookupResults);
        }

        public ExprMemberGroup CreateMemGroup(
                Expr pObject,
                MethPropWithInst mwi)
        {
            Name pName = mwi.Sym?.name;
            MethodOrPropertySymbol methProp = mwi.MethProp();

            CType pType = mwi.GetType() ?? (CType)Types.GetErrorSym();

            return CreateMemGroup(0, pName, mwi.TypeArgs, methProp?.getKind() ?? SYMKIND.SK_MethodSymbol, mwi.GetType(), methProp, pObject, new CMemberLookupResults(GlobalSymbols.AllocParams(1, new CType[] { pType }), pName));
        }

        public ExprUserDefinedConversion CreateUserDefinedConversion(Expr arg, Expr call, MethWithInst mwi)
        {
            return new ExprUserDefinedConversion(arg, call, mwi);
        }

        public ExprCast CreateCast(CType pType, Expr pArg)
        {
            return CreateCast(0, CreateClass(pType), pArg);
        }

        public ExprCast CreateCast(EXPRFLAG nFlags, ExprClass pType, Expr pArg)
        {
            return new ExprCast(nFlags, pType, pArg);
        }

        public ExprReturn CreateReturn(Expr pOptionalObject)
        {
            return new ExprReturn(pOptionalObject);
        }

        public ExprLocal CreateLocal(LocalVariableSymbol pLocal)
        {
            return new ExprLocal(pLocal);
        }

        public ExprBoundLambda CreateAnonymousMethod(AggregateType delegateType, Scope argumentScope)
        {
            return new ExprBoundLambda(delegateType, argumentScope);
        }

        public ExprHoistedLocalExpr CreateHoistedLocalInExpression()
        {
            return new ExprHoistedLocalExpr(Types.GetOptPredefAgg(PredefinedType.PT_EXPRESSION).getThisType());
        }

        public ExprMethodInfo CreateMethodInfo(MethPropWithInst mwi)
        {
            return CreateMethodInfo(mwi.Meth(), mwi.GetType(), mwi.TypeArgs);
        }

        public ExprMethodInfo CreateMethodInfo(MethodSymbol method, AggregateType methodType, TypeArray methodParameters)
        {
            return new ExprMethodInfo(
                Types.GetOptPredefAgg(method.IsConstructor() ? PredefinedType.PT_CONSTRUCTORINFO : PredefinedType.PT_METHODINFO).getThisType(),
                method, methodType, methodParameters);
        }

        public ExprPropertyInfo CreatePropertyInfo(PropertySymbol prop, AggregateType propertyType)
        {
            return new ExprPropertyInfo(
                Types.GetOptPredefAgg(PredefinedType.PT_PROPERTYINFO).getThisType(), prop, propertyType);
        }

        public ExprFieldInfo CreateFieldInfo(FieldSymbol field, AggregateType fieldType)
        {
            return new ExprFieldInfo(field, fieldType, Types.GetOptPredefAgg(PredefinedType.PT_FIELDINFO).getThisType());
        }

        private ExprTypeOf CreateTypeOf(ExprClass pSourceType)
        {
            return new ExprTypeOf(Types.GetReqPredefAgg(PredefinedType.PT_TYPE).getThisType(), pSourceType);
        }
        public ExprTypeOf CreateTypeOf(CType pSourceType)
        {
            return CreateTypeOf(CreateClass(pSourceType));
        }

        public ExprUserLogicalOp CreateUserLogOp(CType pType, Expr pCallTF, ExprCall pCallOp)
        {
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
            return new ExprConcat(op1, op2);
        }

        public ExprConstant CreateStringConstant(string str)
        {
            return CreateConstant(Types.GetReqPredefAgg(PredefinedType.PT_STRING).getThisType(), ConstVal.Get(str));
        }

        public ExprMultiGet CreateMultiGet(EXPRFLAG nFlags, CType pType, ExprMulti pOptionalMulti)
        {
            return new ExprMultiGet(pType, nFlags, pOptionalMulti);
        }

        public ExprMulti CreateMulti(EXPRFLAG nFlags, CType pType, Expr pLeft, Expr pOp)
        {
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
                        CType nullType = Types.GetNullType();

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
                    return CreateConstant(pType, ConstVal.GetDefaultValue(pType.constValKind()));
                case FUNDTYPE.FT_STRUCT:
                    if (pType.isPredefType(PredefinedType.PT_DECIMAL))
                    {
                        goto case FUNDTYPE.FT_R8;
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
            return CreateConstant(Types.GetReqPredefAgg(PredefinedType.PT_INT).getThisType(), ConstVal.Get(x));
        }
        public ExprConstant CreateBoolConstant(bool b)
        {
            return CreateConstant(Types.GetReqPredefAgg(PredefinedType.PT_BOOL).getThisType(), ConstVal.Get(b));
        }

        public ExprBlock CreateBlock(ExprStatement pOptionalStatements)
        {
            return new ExprBlock(pOptionalStatements);
        }

        public ExprArrayIndex CreateArrayIndex(Expr pArray, Expr pIndex)
        {
            CType pType = pArray.Type;
            if (pType is ArrayType arr)
            {
                pType = arr.GetElementType();
            }
            else if (pType == null)
            {
                pType = Types.GetReqPredefAgg(PredefinedType.PT_INT).getThisType();
            }

            return new ExprArrayIndex(pType, pArray, pIndex);
        }

        public ExprBinOp CreateBinop(ExpressionKind exprKind, CType pType, Expr p1, Expr p2)
        {
            return new ExprBinOp(exprKind, pType, p1, p2);
        }

        public ExprUnaryOp CreateUnaryOp(ExpressionKind exprKind, CType pType, Expr pOperand)
        {
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
            return new ExprBinOp(exprKind, pType, p1, p2, call, pmpwi);
        }

        public ExprUnaryOp CreateUserDefinedUnaryOperator(ExpressionKind exprKind, CType pType, Expr pOperand, ExprCall call, MethPropWithInst pmpwi)
        {
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
            return CreateConstant(Types.GetNullType(), default(ConstVal));
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
            return new ExprClass(pType);
        }
    }
}

