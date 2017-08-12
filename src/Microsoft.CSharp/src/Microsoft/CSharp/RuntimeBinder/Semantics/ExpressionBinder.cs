// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.CSharp.RuntimeBinder.Errors;
using Microsoft.CSharp.RuntimeBinder.Syntax;

namespace Microsoft.CSharp.RuntimeBinder.Semantics
{
    // Used by bindUserDefinedConversion
    internal sealed class UdConvInfo
    {
        public MethWithType mwt;
        public bool fSrcImplicit;
        public bool fDstImplicit;
    }

    //////////////////////////////////////////////////////////////////////////////////////////////
    // Small wrapper for passing around argument information for the various BindGrpTo methods
    // It is used because most things only need the type, but in the case of METHGRPs and ANONMETHs
    // the expr is also needed to determine if a conversion is possible
    internal sealed class ArgInfos
    {
        public int carg;
        public TypeArray types;
        public bool fHasExprs;
        public List<Expr> prgexpr;
    }

    internal enum ConstCastResult
    {
        Success,      // Constant can be cast to type
        Failure,      // Constant cannot be cast to type
        CheckFailure  // Constant cannot be cast to type because of overflow in checked context
                      // (Note that this only happens when the conversion is explicit; implicit
                      // conversions never overflow, that's why they're implicit.)
    }

    internal enum AggCastResult
    {
        Success, // We found a conversion, stop looking
        Failure, // This conversion doesn't work, keep looking
        Abort    // No possible conversion can work, stop looking
    }

    internal enum UnaryOperatorSignatureFindResult
    {
        Match,
        Continue,
        Return
    }


    internal enum UnaOpKind
    {
        Plus,
        Minus,
        Tilde,
        Bang,
        IncDec,
        Lim
    }

    [Flags]
    internal enum UnaOpMask
    {
        None = 0,
        Plus = 1 << UnaOpKind.Plus,
        Minus = 1 << UnaOpKind.Minus,
        Tilde = 1 << UnaOpKind.Tilde,
        Bang = 1 << UnaOpKind.Bang,
        IncDec = 1 << UnaOpKind.IncDec,
        // The different combinations needed in operators.cs
        Signed = Plus | Minus | Tilde,
        Unsigned = Plus | Tilde,
        Real = Plus | Minus,
        Bool = Bang,
    }

    [Flags]
    internal enum OpSigFlags
    {
        None = 0,
        Convert = 0x01,    // Convert the operands before calling the bind method
        CanLift = 0x02,    // Operator has a lifted form
        AutoLift = 0x04,   // Standard nullable lifting
        // The different combinations needed in operators.cs
        Value = Convert | CanLift | AutoLift,
        Reference = Convert,
        BoolBit = Convert | CanLift,
    }

    [Flags]
    internal enum LiftFlags
    {
        None = 0,
        Lift1 = 0x01,
        Lift2 = 0x02,
        Convert1 = 0x04,
        Convert2 = 0x08,
    }

    internal enum CheckLvalueKind
    {
        Assignment,
        OutParameter,
        Increment,
    }

    internal enum BinOpFuncKind
    {
        BoolBinOp,
        BoolBitwiseOp,
        DecBinOp,
        DelBinOp,
        EnumBinOp,
        IntBinOp,
        PtrBinOp,
        PtrCmpOp,
        RealBinOp,
        RefCmpOp,
        ShiftOp,
        StrBinOp,
        StrCmpOp,
        None
    }

    internal enum UnaOpFuncKind
    {
        BoolUnaOp,
        DecUnaOp,
        EnumUnaOp,
        IntUnaOp,
        RealUnaOp,
        LiftedIncOpCore,
        None
    }

    internal sealed partial class ExpressionBinder
    {
        // ExpressionBinder - General Rules
        // 
        // Express the Contract
        // 
        // Use assertions and naming guidelines to express the contract for methods. 
        // The most common issue is whether an argument may be null or not. If an 
        // argument may not be null, then the method must ASSERT that before any other 
        // code. If an argument may be null then the name of the argument should 
        // include 'Optional'. The exception to this rule is the input parse tree 
        // parameter. If the parse tree may be null, then the method name should 
        // include an 'Opt' suffix. For example bindArgumentList should really be 
        // named bindArgumentListOpt. Abbreviations should be avoided, but the 'Opt' 
        // suffix gets an exception because it is used consistently in the language 
        // spec.
        // 
        // 
        // Error Tolerant
        // 
        // Do not rely on the input parse tree being complete. Erroneous code may 
        // result in parse trees with required children missing, or with unexpected 
        // structure. Find out what the invariants are for the parse tree being 
        // consumed and code defensively.
        // 
        // Similarly, the result of binding children nodes may not be 'OK'. The child 
        // node may have contained some semantic errors and the binding code in the 
        // parent must cope gracefully with the result. For example, an EXPRMEMGRP may 
        // contain no members.
        // 
        // 
        // Error Recovery
        // 
        // Always attempt to bind children nodes even if errors have already been 
        // detected in other children. Always build a new node representing a 'best 
        // guess' at the semantics of the parse tree. Never discard the results of 
        // binding a child node even if the binding has errors. Since a new node is 
        // always produced there should always be a place to add bindings with errors 
        // to the result. 
        // 
        // This ensures that full semantic information for all nodes is produced - 
        // that the expression binder always produces a 'best guess' for every 
        // expression in source.
        // 
        // 
        // Error Reporting
        // 
        // If a child expression has an error, then no new error's for the parent 
        // should be reported, unless there is no way that the new error was caused 
        // by any child errors. If child nodes don't have any errors, and the new 
        // node does have an error, then at least one error must be reported. These 
        // rules ensure that an error is always reported for erroneous code, and that 
        // only the most meaningful error is reported from a set of cascading errors.
        // 
        // 
        // Map Back To the Source
        // 
        // When constructing new expression nodes, attach the appropriate parse tree 
        // node. The attached parse tree is used for:
        //             - error location reporting
        //             - debug sequence points 
        //                         - stepping
        //                         - local variable scopes
        //             - finding the most meaningful expression for a parse tree node
        // 
        // 
        // Meaning not Implementation
        // 
        // The expression trees resulting from the initial binding pass should 
        // represent the semantics of the input source code. There should be a 
        // direct mapping between the newly constructed expression and the input 
        // parse tree.
        // 
        // The Whidbey codebase had the habit of producing expressions in the initial 
        // binding which were closer in representation to the generated IL than the 
        // input source code. In Orcas all transformations which may lose semantic 
        // information about the source must be done after the initial expression 
        // binding phase. 
        // 
        // Special Cases
        // 
        //     -   Constant folding - Constant folding is a semantic losing 
        //         transformation which must be performed to complete expression 
        //         analysis. When creating a folded constant, create an expression 
        //         node representing the unfolded expression, then pass this as a 
        //         child expression of a new constant expression.
        //     -   Color Color - The new Type or Instance expression covers this case. 
        //         It is produced from bindSimpleName.
        //     -   Method Group - Method groups should be preserved in expression trees. 
        //         This includes as children of Call expressions, and delegate construction 
        //         nodes.
        //     -   Type Binding - This is a big one. Whenever a type is bound in an 
        //         expression, the binding of the component parts of the type must be 
        //         preserved. This includes every identifier in a dotted type or 
        //         namespace name, as well as the type binding information for the 
        //         type arguments of constructed types. The semantic information for 
        //         intermediate type binding results is represented by an 
        //         EXPRTYPEORNAMESPACE. Types can be bound in several places in 
        //         expressions:
        //             - Sizeof
        //             - Typeof
        //             - New
        //             - Is/As
        //             - Cast
        //             - Type Arguments supplied to generic method calls.
        //             - Left hand side of a dot operator.
        //             - Parameter types in anonymous methods and lambdas.
        //             - Local Variables
        // 
        // 
        // Want to eventually Have's
        // 
        // Only build the new node once all children have been built.
        // Factory should require all children as arguments.
        // Factory method sets the "Do Children have Errors?" bit - not done manually.
        // Once constructed Expression trees are not mutated - doesn't work easily for statements unfortunately.

        private delegate Expr PfnBindBinOp(ExpressionKind ek, EXPRFLAG flags, Expr op1, Expr op2);
        private delegate Expr PfnBindUnaOp(ExpressionKind ek, EXPRFLAG flags, Expr op);

        private BindingContext Context;
        public BindingContext GetContext() { return Context; }
        private CNullable m_nullable;

        private static void VSFAIL(string s)
        {
            Debug.Assert(false, s);
        }

        public ExpressionBinder(BindingContext context)
        {
            Context = context;
            m_nullable = new CNullable(GetSymbolLoader(), GetErrorContext(), GetExprFactory());
            g_binopSignatures = new BinOpSig[]
            {
                new BinOpSig (PredefinedType.PT_INT,        PredefinedType.PT_INT,      BinOpMask.Integer,  8, BindIntBinOp,            OpSigFlags.Value,       BinOpFuncKind.IntBinOp      ),
                new BinOpSig (PredefinedType.PT_UINT,       PredefinedType.PT_UINT,     BinOpMask.Integer,  7, BindIntBinOp,            OpSigFlags.Value,       BinOpFuncKind.IntBinOp      ),
                new BinOpSig (PredefinedType.PT_LONG,       PredefinedType.PT_LONG,     BinOpMask.Integer,  6, BindIntBinOp,            OpSigFlags.Value,       BinOpFuncKind.IntBinOp      ),
                new BinOpSig (PredefinedType.PT_ULONG,      PredefinedType.PT_ULONG,    BinOpMask.Integer,  5, BindIntBinOp,            OpSigFlags.Value,       BinOpFuncKind.IntBinOp      ),
                /* ERROR */
                new BinOpSig (PredefinedType.PT_ULONG,      PredefinedType.PT_LONG,     BinOpMask.Integer,  4, null,                    OpSigFlags.Value,       BinOpFuncKind.None          ),
                /* ERROR */
                new BinOpSig (PredefinedType.PT_LONG,       PredefinedType.PT_ULONG,    BinOpMask.Integer,  3, null,                    OpSigFlags.Value,       BinOpFuncKind.None          ),
                new BinOpSig (PredefinedType.PT_FLOAT,      PredefinedType.PT_FLOAT,    BinOpMask.Real,     1, BindRealBinOp,           OpSigFlags.Value,       BinOpFuncKind.RealBinOp     ),
                new BinOpSig (PredefinedType.PT_DOUBLE,     PredefinedType.PT_DOUBLE,   BinOpMask.Real,     0, BindRealBinOp,           OpSigFlags.Value,       BinOpFuncKind.RealBinOp     ),
                new BinOpSig (PredefinedType.PT_DECIMAL,    PredefinedType.PT_DECIMAL,  BinOpMask.Real,     0, BindDecBinOp,            OpSigFlags.Value,       BinOpFuncKind.DecBinOp      ),
                new BinOpSig (PredefinedType.PT_STRING,     PredefinedType.PT_STRING,   BinOpMask.Equal,    0, BindStrCmpOp,            OpSigFlags.Reference,   BinOpFuncKind.StrCmpOp      ),
                new BinOpSig (PredefinedType.PT_STRING,     PredefinedType.PT_STRING,   BinOpMask.Add,      2, BindStrBinOp,            OpSigFlags.Reference,   BinOpFuncKind.StrBinOp      ),
                new BinOpSig (PredefinedType.PT_STRING,     PredefinedType.PT_OBJECT,   BinOpMask.Add,      1, BindStrBinOp,            OpSigFlags.Reference,   BinOpFuncKind.StrBinOp      ),
                new BinOpSig (PredefinedType.PT_OBJECT,     PredefinedType.PT_STRING,   BinOpMask.Add,      0, BindStrBinOp,            OpSigFlags.Reference,   BinOpFuncKind.StrBinOp      ),
                new BinOpSig (PredefinedType.PT_INT,        PredefinedType.PT_INT,      BinOpMask.Shift,    3, BindShiftOp,             OpSigFlags.Value,       BinOpFuncKind.ShiftOp       ),
                new BinOpSig (PredefinedType.PT_UINT,       PredefinedType.PT_INT,      BinOpMask.Shift,    2, BindShiftOp,             OpSigFlags.Value,       BinOpFuncKind.ShiftOp       ),
                new BinOpSig (PredefinedType.PT_LONG,       PredefinedType.PT_INT,      BinOpMask.Shift,    1, BindShiftOp,             OpSigFlags.Value,       BinOpFuncKind.ShiftOp       ),
                new BinOpSig (PredefinedType.PT_ULONG,      PredefinedType.PT_INT,      BinOpMask.Shift,    0, BindShiftOp,             OpSigFlags.Value,       BinOpFuncKind.ShiftOp       ),
                new BinOpSig (PredefinedType.PT_BOOL,       PredefinedType.PT_BOOL,     BinOpMask.BoolNorm, 0, BindBoolBinOp,           OpSigFlags.Value,       BinOpFuncKind.BoolBinOp     ),
                // Make boolean logical operators liftable so that they don't give funny short circuiting semantics.
                // This is for DDBugs 677075.
                new BinOpSig (PredefinedType.PT_BOOL,       PredefinedType.PT_BOOL,     BinOpMask.Logical,  0, BindBoolBinOp,           OpSigFlags.BoolBit,     BinOpFuncKind.BoolBinOp     ),
                new BinOpSig (PredefinedType.PT_BOOL,       PredefinedType.PT_BOOL,     BinOpMask.Bitwise,  0, BindLiftedBoolBitwiseOp, OpSigFlags.BoolBit,     BinOpFuncKind.BoolBitwiseOp ),
            };
            g_rguos = new UnaOpSig[]
            {
                new UnaOpSig( PredefinedType.PT_INT,        UnaOpMask.Signed,   7, BindIntUnaOp,    UnaOpFuncKind.IntUnaOp  ),
                new UnaOpSig( PredefinedType.PT_UINT,       UnaOpMask.Unsigned, 6, BindIntUnaOp,    UnaOpFuncKind.IntUnaOp  ),
                new UnaOpSig( PredefinedType.PT_LONG,       UnaOpMask.Signed,   5, BindIntUnaOp,    UnaOpFuncKind.IntUnaOp  ),
                new UnaOpSig( PredefinedType.PT_ULONG,      UnaOpMask.Unsigned, 4, BindIntUnaOp,    UnaOpFuncKind.IntUnaOp  ),
                /* ERROR */
                new UnaOpSig( PredefinedType.PT_ULONG,      UnaOpMask.Minus,    3, null,            UnaOpFuncKind.None      ),
                new UnaOpSig( PredefinedType.PT_FLOAT,      UnaOpMask.Real,     1, BindRealUnaOp,   UnaOpFuncKind.RealUnaOp ),
                new UnaOpSig( PredefinedType.PT_DOUBLE,     UnaOpMask.Real,     0, BindRealUnaOp,   UnaOpFuncKind.RealUnaOp ),
                new UnaOpSig( PredefinedType.PT_DECIMAL,    UnaOpMask.Real,     0, BindDecUnaOp,    UnaOpFuncKind.DecUnaOp  ),
                new UnaOpSig( PredefinedType.PT_BOOL,       UnaOpMask.Bool,     0, BindBoolUnaOp,   UnaOpFuncKind.BoolUnaOp ),
                new UnaOpSig( PredefinedType.PT_INT,        UnaOpMask.IncDec,   6, null,            UnaOpFuncKind.None      ),
                new UnaOpSig( PredefinedType.PT_UINT,       UnaOpMask.IncDec,   5, null,            UnaOpFuncKind.None      ),
                new UnaOpSig( PredefinedType.PT_LONG,       UnaOpMask.IncDec,   4, null,            UnaOpFuncKind.None      ),
                new UnaOpSig( PredefinedType.PT_ULONG,      UnaOpMask.IncDec,   3, null,            UnaOpFuncKind.None      ),
                new UnaOpSig( PredefinedType.PT_FLOAT,      UnaOpMask.IncDec,   1, null,            UnaOpFuncKind.None      ),
                new UnaOpSig( PredefinedType.PT_DOUBLE,     UnaOpMask.IncDec,   0, null,            UnaOpFuncKind.None      ),
                new UnaOpSig( PredefinedType.PT_DECIMAL,    UnaOpMask.IncDec,   0, null,            UnaOpFuncKind.None      ),
            };
        }

        private SymbolLoader GetSymbolLoader() { return SymbolLoader; }

        private SymbolLoader SymbolLoader
        {
            get
            {
                return Context.SymbolLoader;
            }
        }

        private CSemanticChecker SemanticChecker
        {
            get
            {
                return Context.SemanticChecker;
            }
        }
        public CSemanticChecker GetSemanticChecker() { return SemanticChecker; }

        private ErrorHandling ErrorContext
        {
            get
            {
                return SymbolLoader.ErrorContext;
            }
        }
        private ErrorHandling GetErrorContext() { return ErrorContext; }

        private BSYMMGR GetGlobalSymbols()
        {
            return GetSymbolLoader().getBSymmgr();
        }

        private TypeManager GetTypes() { return TypeManager; }

        private TypeManager TypeManager { get { return SymbolLoader.TypeManager; } }

        private ExprFactory GetExprFactory() { return ExprFactory; }

        private ExprFactory ExprFactory { get { return Context.ExprFactory; } }

        private AggregateType GetPredefindType(PredefinedType pt)
        {
            Debug.Assert(pt != PredefinedType.PT_VOID); // use getVoidType()

            return GetSymbolLoader().GetPredefindType(pt);
        }

        private CType VoidType { get { return GetSymbolLoader().GetTypeManager().GetVoid(); } }

        private CType getVoidType() { return VoidType; }

        private Expr GenerateAssignmentConversion(Expr op1, Expr op2, bool allowExplicit)
        {
            if (allowExplicit)
            {
                return mustCastCore(op2, GetExprFactory().CreateClass(op1.Type), 0);
            }
            else
            {
                return mustConvertCore(op2, GetExprFactory().CreateClass(op1.Type));
            }
        }

        ////////////////////////////////////////////////////////////////////////////////
        // Bind the simple assignment operator =.

        public Expr BindAssignment(Expr op1, Expr op2, bool allowExplicit)
        {
            Debug.Assert(op1 is ExprCast
                || op1 is ExprArrayIndex
                || op1 is ExprCall
                || op1 is ExprProperty 
                || op1 is ExprClass
                || op1 is ExprField);

            if (!checkLvalue(op1, CheckLvalueKind.Assignment))
            {
                ExprAssignment rval = GetExprFactory().CreateAssignment(op1, op2);
                rval.SetError();
                return rval;
            }

            op2 = GenerateAssignmentConversion(op1, op2, allowExplicit);
            return GenerateOptimizedAssignment(op1, op2);
        }

        internal Expr BindArrayIndexCore(BindingFlag bindFlags, Expr pOp1, Expr pOp2)
        {
            Expr pExpr;
            bool bIsError = false;
            if (!pOp1.IsOK || !pOp2.IsOK)
            {
                bIsError = true;
            }

            CType pIntType = GetPredefindType(PredefinedType.PT_INT);

            // Array indexing must occur on an array type.
            if (!(pOp1.Type is ArrayType pArrayType))
            {
                Debug.Assert(!(pOp1.Type is PointerType));
                pExpr = bindIndexer(pOp1, pOp2, bindFlags);
                if (bIsError)
                {
                    pExpr.SetError();
                }
                return pExpr;
            }

            checkUnsafe(pArrayType.GetElementType()); // added to the binder so we don't bind to pointer ops
            // Check the rank of the array against the number of indices provided, and
            // convert the indexes to ints

            CType pDestType = chooseArrayIndexType(pOp2);

            if (null == pDestType)
            {
                // using int as the type will allow us to give a better error...
                pDestType = pIntType;
            }

            int rank = pArrayType.rank;
            int cIndices = 0;

            Expr transformedIndices = pOp2.Map(GetExprFactory(),
                (Expr x) =>
                {
                    cIndices++;
                    Expr pTemp = mustConvert(x, pDestType);
                    if (pDestType == pIntType)
                        return pTemp;
                    ExprClass exprType = GetExprFactory().CreateClass(pDestType);
                    return GetExprFactory().CreateCast(EXPRFLAG.EXF_INDEXEXPR, exprType, pTemp);
                });

            if (cIndices != rank)
            {
                throw ErrorContext.Error(ErrorCode.ERR_BadIndexCount, rank);
            }

            // Allocate a new expression, the type is the element type of the array.
            // Array index operations are always lvalues.
            pExpr = GetExprFactory().CreateArrayIndex(pOp1, transformedIndices);
            pExpr.Flags |= EXPRFLAG.EXF_LVALUE | EXPRFLAG.EXF_ASSGOP;

            if (bIsError)
            {
                pExpr.SetError();
            }

            return pExpr;
        }

        private Expr bindIndexer(Expr pObject, Expr args, BindingFlag bindFlags)
        {
            CType type = pObject.Type;

            if (!(type is AggregateType) && !(type is TypeParameterType))
            {
                throw ErrorContext.Error(ErrorCode.ERR_BadIndexLHS, type);
            }

            Name pName = NameManager.GetPredefinedName(PredefinedName.PN_INDEXERINTERNAL);

            MemberLookup mem = new MemberLookup();
            if (!mem.Lookup(GetSemanticChecker(), type, pObject, ContextForMemberLookup(), pName, 0,
                            MemLookFlags.Indexer))
            {
                throw mem.ReportErrors();
            }

            Debug.Assert(mem.SymFirst() is IndexerSymbol);

            ExprMemberGroup grp = GetExprFactory().CreateMemGroup((EXPRFLAG)mem.GetFlags(),
                pName, BSYMMGR.EmptyTypeArray(), mem.SymFirst().getKind(), mem.GetSourceType(), null/*pMPS*/, mem.GetObject(), mem.GetResults());

            Expr pResult = BindMethodGroupToArguments(bindFlags, grp, args);
            IExprWithObject exprWithObject = pResult as IExprWithObject;
            Debug.Assert(exprWithObject != null);
            if (exprWithObject?.OptionalObject == null)
            {
                // We must be in an error scenario where the object was not allowed. 
                // This can happen if the user tries to access the indexer off the
                // type and not an instance or if the incorrect type/number of arguments 
                // were passed for binding.
                if (exprWithObject != null)
                {
                    exprWithObject.OptionalObject = pObject;
                }

                pResult.SetError();
            }
            return pResult;
        }

        ////////////////////////////////////////////////////////////////////////////////
        // Create a cast node with the given expression flags. 
        private void bindSimpleCast(Expr exprSrc, ExprClass typeDest, out Expr pexprDest)
        {
            bindSimpleCast(exprSrc, typeDest, out pexprDest, 0);
        }

        private void bindSimpleCast(Expr exprSrc, ExprClass exprTypeDest, out Expr pexprDest, EXPRFLAG exprFlags)
        {
            Debug.Assert(exprTypeDest != null);
            Debug.Assert(exprTypeDest.Type != null);
            CType typeDest = exprTypeDest.Type;
            pexprDest = null;
            // If the source is a constant, and cast is really simple (no change in fundamental
            // type, no flags), then create a new constant node with the new type instead of
            // creating a cast node. This allows compile-time constants to be easily recognized.
            Expr exprConst = exprSrc.GetConst();

            // Make the cast expr anyway, and if we find that we have a constant, then set the cast expr
            // as the original tree for the constant. Otherwise, return the cast expr.

            ExprCast exprCast = GetExprFactory().CreateCast(exprFlags, exprTypeDest, exprSrc);
            if (Context.CheckedNormal)
            {
                exprCast.Flags |= EXPRFLAG.EXF_CHECKOVERFLOW;
            }

            // Check if we have a compile time constant. If we do, create a constant for it and set the
            // original tree to the cast.

            if (exprConst is ExprConstant constant && exprFlags == 0 &&
                exprSrc.Type.fundType() == typeDest.fundType() &&
                (!exprSrc.Type.isPredefType(PredefinedType.PT_STRING) || constant.Val.IsNullRef))
            {
                ExprConstant expr = GetExprFactory().CreateConstant(typeDest, constant.Val);
                pexprDest = expr;
                return;
            }

            pexprDest = exprCast;
            Debug.Assert(exprCast.Argument != null);
            return;
        }

        ////////////////////////////////////////////////////////////////////////////////
        // Binds a call to a method, return type is an error or an EXPRCALL.
        //
        // tree      - ParseTree for error messages
        // pObject    - pObject to call method on
        // pmwi      - Meth to bind to. This will be morphed when we remap to an override.
        // args      - arguments
        // exprFlags - Flags to put on the generated expr

        private ExprCall BindToMethod(MethWithInst mwi, Expr pArguments, ExprMemberGroup pMemGroup, MemLookFlags flags)
        {
            Debug.Assert(mwi.Sym is MethodSymbol && (!mwi.Meth().isOverride || mwi.Meth().isHideByName));
            Debug.Assert(pMemGroup != null);

            bool fConstrained;
            Expr pObject = pMemGroup.OptionalObject;
            CType callingObjectType = pObject?.Type;
            PostBindMethod(ref mwi, pObject);
            pObject = AdjustMemberObject(mwi, pObject, out fConstrained);
            pMemGroup.OptionalObject = pObject;

            CType pReturnType = null;
            if ((flags & (MemLookFlags.Ctor | MemLookFlags.NewObj)) == (MemLookFlags.Ctor | MemLookFlags.NewObj))
            {
                pReturnType = mwi.Ats;
            }
            else
            {
                pReturnType = GetTypes().SubstType(mwi.Meth().RetType, mwi.GetType(), mwi.TypeArgs);
            }

            ExprCall pResult = GetExprFactory().CreateCall(0, pReturnType, pArguments, pMemGroup, mwi);

            if (!pResult.IsOK)
            {
                return pResult;
            }

            // Set the return type and flags for constructors.
            if ((flags & MemLookFlags.Ctor) != 0)
            {
                if ((flags & MemLookFlags.NewObj) != 0)
                {
                    pResult.Flags |= EXPRFLAG.EXF_NEWOBJCALL | EXPRFLAG.EXF_CANTBENULL;
                }
                else
                {
                    Debug.Assert(pResult.Type == getVoidType());
                }
            }

            if (fConstrained && pObject != null)
            {
                // Use the constrained prefix.
                pResult.Flags |= EXPRFLAG.EXF_CONSTRAINED;
            }

            verifyMethodArgs(pResult, callingObjectType);

            return pResult;
        }

        ////////////////////////////////////////////////////////////////////////////////
        // Construct the Expr node which corresponds to a field expression
        // for a given field and pObject pointer.

        internal Expr BindToField(Expr pOptionalObject, FieldWithType fwt, BindingFlag bindFlags)
        {
            Debug.Assert(fwt.GetType() != null && fwt.Field().getClass() == fwt.GetType().getAggregate());

            CType pFieldType = GetTypes().SubstType(fwt.Field().GetType(), fwt.GetType());
            if (pOptionalObject != null && !pOptionalObject.IsOK)
            {
                ExprField pField = GetExprFactory().CreateField(pFieldType, pOptionalObject, fwt, false);
                pField.SetError();
                return pField;
            }

            bool pfConstrained;
            pOptionalObject = AdjustMemberObject(fwt, pOptionalObject, out pfConstrained);

            checkUnsafe(pFieldType); // added to the binder so we don't bind to pointer ops

            bool isLValue = pOptionalObject?.Type is PointerType || objectIsLvalue(pOptionalObject);

            // Exception: a readonly field is not an lvalue unless we're in the constructor/static constructor appropriate
            // for the field.
            if (fwt.Field().isReadOnly)
            {
                isLValue = false;
            }

            AggregateType fieldType = null;
            // If this field is the backing field of a WindowsRuntime event then we need to bind to its
            // invocationlist property which is a delegate containing all the handlers.
            if (fwt.Field().isEvent && fwt.Field().getEvent(GetSymbolLoader()) != null
                && fwt.Field().getEvent(GetSymbolLoader()).IsWindowsRuntimeEvent)
            {
                fieldType = fwt.Field().GetType() as AggregateType;
                if (fieldType != null)
                {
                    // Access event backing field (EventRegistrationTokenTable<T>) using
                    // EventRegistrationTokenTable<T>.GetOrCreateEventRegistrationTokenTable()
                    // to ensure non-null
                    pFieldType = GetTypes().GetParameterModifier(pFieldType, false);
                }
            }

            ExprField pResult = GetExprFactory()
                .CreateField(pFieldType, pOptionalObject, fwt, isLValue);

            if (pFieldType is ErrorType)
            {
                pResult.SetError();
            }

            Debug.Assert(BindingFlag.BIND_MEMBERSET == (BindingFlag)EXPRFLAG.EXF_MEMBERSET);
            pResult.Flags |= (EXPRFLAG)(bindFlags & BindingFlag.BIND_MEMBERSET);

            if (fieldType != null)
            {
                Name getOrCreateMethodName =
                    NameManager.GetPredefinedName(PredefinedName.PN_GETORCREATEEVENTREGISTRATIONTOKENTABLE);
                GetSymbolLoader()
                    .RuntimeBinderSymbolTable.PopulateSymbolTableWithName(
                        getOrCreateMethodName.Text, null, fieldType.AssociatedSystemType);
                MethodSymbol getOrCreateMethod =
                    GetSymbolLoader()
                        .LookupAggMember(getOrCreateMethodName, fieldType.getAggregate(), symbmask_t.MASK_MethodSymbol)
                         as MethodSymbol;

                MethPropWithInst getOrCreatempwi = new MethPropWithInst(getOrCreateMethod, fieldType);
                ExprMemberGroup getOrCreateGrp = GetExprFactory().CreateMemGroup(null, getOrCreatempwi);

                Expr getOrCreateCall = BindToMethod(
                    new MethWithInst(getOrCreatempwi), pResult, getOrCreateGrp, (MemLookFlags)MemLookFlags.None);

                AggregateSymbol fieldTypeSymbol = fieldType.GetOwningAggregate();
                Name invocationListName = NameManager.GetPredefinedName(PredefinedName.PN_INVOCATIONLIST);

                // InvocationList might not be populated in the symbol table as no one would have called it.
                GetSymbolLoader()
                    .RuntimeBinderSymbolTable.PopulateSymbolTableWithName(
                        invocationListName.Text, null, fieldType.AssociatedSystemType);
                PropertySymbol invocationList =
                    GetSymbolLoader()
                        .LookupAggMember(invocationListName, fieldTypeSymbol, symbmask_t.MASK_PropertySymbol)
                         as PropertySymbol;

                MethPropWithInst mpwi = new MethPropWithInst(invocationList, fieldType);
                ExprMemberGroup memGroup = GetExprFactory().CreateMemGroup(getOrCreateCall, mpwi);

                PropWithType pwt = new PropWithType(invocationList, fieldType);
                Expr propertyExpr = BindToProperty(getOrCreateCall, pwt, bindFlags, null, null, memGroup);
                return propertyExpr;
            }

            return pResult;
        }

        ////////////////////////////////////////////////////////////////////////////////

        internal Expr BindToProperty(Expr pObject, PropWithType pwt, BindingFlag bindFlags, Expr args, AggregateType pOtherType, ExprMemberGroup pMemGroup)
        {
            Debug.Assert(pwt.Sym != null &&
                    pwt.Sym is PropertySymbol &&
                    pwt.GetType() != null &&
                    pwt.Prop().getClass() == pwt.GetType().getAggregate());
            Debug.Assert(pwt.Prop().Params.Count == 0 || pwt.Prop() is IndexerSymbol);
            Debug.Assert(pOtherType == null ||
                    !(pwt.Prop() is IndexerSymbol) &&
                    pOtherType.getAggregate() == pwt.Prop().RetType.getAggregate());

            bool fConstrained;
            MethWithType mwtGet;
            MethWithType mwtSet;
            Expr pObjectThrough = null;

            // We keep track of the type of the pObject which we're doing the call through so that we can report 
            // protection access errors later, either below when binding the get, or later when checking that
            // the setter is actually an lvalue.
            pObjectThrough = pObject;

            PostBindProperty(pwt, pObject, out mwtGet, out mwtSet);

            if (mwtGet &&
                    (!mwtSet ||
                     mwtSet.GetType() == mwtGet.GetType() ||
                     GetSymbolLoader().HasBaseConversion(mwtGet.GetType(), mwtSet.GetType())
                     )
                )
            {
                pObject = AdjustMemberObject(mwtGet, pObject, out fConstrained);
            }
            else if (mwtSet)
            {
                pObject = AdjustMemberObject(mwtSet, pObject, out fConstrained);
            }
            else
            {
                pObject = AdjustMemberObject(pwt, pObject, out fConstrained);
            }
            pMemGroup.OptionalObject = pObject;

            CType pReturnType = GetTypes().SubstType(pwt.Prop().RetType, pwt.GetType());
            Debug.Assert(pOtherType == pReturnType || pOtherType == null);

            if (pObject != null && !pObject.IsOK)
            {
                ExprProperty pResult = GetExprFactory().CreateProperty(pReturnType, pObjectThrough, args, pMemGroup, pwt, null);
                pResult.SetError();
                return pResult;
            }

            // if we are doing a get on this thing, and there is no get, and
            // most importantly, we are not leaving the arguments to be bound by the array index
            // then error...
            if ((bindFlags & BindingFlag.BIND_RVALUEREQUIRED) != 0)
            {
                if (!mwtGet)
                {
                    if (pOtherType != null)
                    {
                        return GetExprFactory().CreateClass(pOtherType);
                    }

                    throw ErrorContext.Error(ErrorCode.ERR_PropertyLacksGet, pwt);
                }
                else
                {
                    CType type = null;
                    if (pObjectThrough != null)
                    {
                        type = pObjectThrough.Type;
                    }

                    ACCESSERROR error = SemanticChecker.CheckAccess2(mwtGet.Meth(), mwtGet.GetType(), ContextForMemberLookup(), type);
                    if (error != ACCESSERROR.ACCESSERROR_NOERROR)
                    {
                        // if the get exists, but is not accessible, give an error.
                        if (pOtherType != null)
                        {
                            return GetExprFactory().CreateClass(pOtherType);
                        }

                        if (error == ACCESSERROR.ACCESSERROR_NOACCESSTHRU)
                        {
                            throw ErrorContext.Error(ErrorCode.ERR_BadProtectedAccess, pwt, type, ContextForMemberLookup());
                        }
                        else
                        {
                            throw ErrorContext.Error(ErrorCode.ERR_InaccessibleGetter, pwt);
                        }
                    }
                }
            }

            ExprProperty result = GetExprFactory().CreateProperty(pReturnType, pObjectThrough, args, pMemGroup, pwt, mwtSet);
            if (fConstrained && pObject != null)
            {
                // Use the constrained prefix.
                result.Flags |= EXPRFLAG.EXF_CONSTRAINED;
            }

            if (result.OptionalArguments != null)
            {
                verifyMethodArgs(result, pObjectThrough?.Type);
            }

            if (mwtSet && objectIsLvalue(result.MemberGroup.OptionalObject))
            {
                result.Flags |= EXPRFLAG.EXF_LVALUE;
            }
            if (pOtherType != null)
            {
                result.Flags |= EXPRFLAG.EXF_SAMENAMETYPE;
            }

            return result;
        }

        internal Expr bindUDUnop(ExpressionKind ek, Expr arg)
        {
            Name pName = ekName(ek);
            Debug.Assert(pName != null);

            CType typeSrc = arg.Type;

        LAgain:
            switch (typeSrc.GetTypeKind())
            {
                case TypeKind.TK_NullableType:
                    typeSrc = typeSrc.StripNubs();
                    goto LAgain;
                case TypeKind.TK_TypeParameterType:
                    typeSrc = ((TypeParameterType)typeSrc).GetEffectiveBaseClass();
                    goto LAgain;
                case TypeKind.TK_AggregateType:
                    if (!typeSrc.isClassType() && !typeSrc.isStructType() || ((AggregateType)typeSrc).getAggregate().IsSkipUDOps())
                        return null;
                    break;
                default:
                    return null;
            }

            ArgInfos info = new ArgInfos();

            info.carg = 1;
            FillInArgInfoFromArgList(info, arg);

            List<CandidateFunctionMember> methFirstList = new List<CandidateFunctionMember>();
            MethodSymbol methCur = null;
            AggregateType atsCur = (AggregateType)typeSrc;

            for (; ;)
            {
                // Find the next operator.
                methCur = methCur == null
                    ? GetSymbolLoader().LookupAggMember(pName, atsCur.getAggregate(), symbmask_t.MASK_MethodSymbol) as MethodSymbol
                    : GetSymbolLoader().LookupNextSym(methCur, atsCur.getAggregate(), symbmask_t.MASK_MethodSymbol) as MethodSymbol;

                if (methCur == null)
                {
                    // Find the next type.
                    // If we've found some applicable methods in a class then we don't need to look any further.
                    if (!methFirstList.IsEmpty())
                        break;
                    atsCur = atsCur.GetBaseClass();
                    if (atsCur == null)
                        break;
                    continue;
                }

                // Only look at operators with 1 args.
                if (!methCur.isOperator || methCur.Params.Count != 1)
                    continue;
                Debug.Assert(methCur.typeVars.Count == 0);

                TypeArray paramsCur = GetTypes().SubstTypeArray(methCur.Params, atsCur);
                CType typeParam = paramsCur[0];
                NullableType nubParam;

                if (canConvert(arg, typeParam))
                {
                    methFirstList.Add(new CandidateFunctionMember(
                                    new MethPropWithInst(methCur, atsCur, BSYMMGR.EmptyTypeArray()),
                                    paramsCur,
                                    0,
                                    false));
                }
                else if (GetSymbolLoader().FCanLift() && typeParam.IsNonNubValType() &&
                         GetTypes().SubstType(methCur.RetType, atsCur).IsNonNubValType() &&
                         canConvert(arg, nubParam = GetTypes().GetNullable(typeParam)))
                {
                    methFirstList.Add(new CandidateFunctionMember(
                                    new MethPropWithInst(methCur, atsCur, BSYMMGR.EmptyTypeArray()),
                                    GetGlobalSymbols().AllocParams(1, new CType[] { nubParam }),
                                    1,
                                    false));
                }
            }

            if (methFirstList.IsEmpty())
                return null;

            CandidateFunctionMember pmethAmbig1;
            CandidateFunctionMember pmethAmbig2;
            CandidateFunctionMember pmethBest = FindBestMethod(methFirstList, null, info, out pmethAmbig1, out pmethAmbig2);

            if (pmethBest == null)
            {
                // No winner, so its an ambiguous call...
                throw ErrorContext.Error(ErrorCode.ERR_AmbigCall, pmethAmbig1.mpwi, pmethAmbig2.mpwi);
            }

            ExprCall call;

            if (pmethBest.ctypeLift != 0)
            {
                call = BindLiftedUDUnop(arg, pmethBest.@params[0], pmethBest.mpwi);
            }
            else
            {
                call = BindUDUnopCall(arg, pmethBest.@params[0], pmethBest.mpwi);
            }

            return GetExprFactory().CreateUserDefinedUnaryOperator(ek, call.Type, arg, call, pmethBest.mpwi);
        }

        private ExprCall BindLiftedUDUnop(Expr arg, CType typeArg, MethPropWithInst mpwi)
        {
            CType typeRaw = typeArg.StripNubs();
            if (!(arg.Type is NullableType) || !canConvert(arg.Type.StripNubs(), typeRaw, CONVERTTYPE.NOUDC))
            {
                // Convert then lift.
                arg = mustConvert(arg, typeArg);
            }
            Debug.Assert(arg.Type is NullableType);

            CType typeRet = GetTypes().SubstType(mpwi.Meth().RetType, mpwi.GetType());
            if (!(typeRet is NullableType))
            {
                typeRet = GetTypes().GetNullable(typeRet);
            }

            // First bind the non-lifted version for errors.
            Expr nonLiftedArg = mustCast(arg, typeRaw);
            ExprCall nonLiftedResult = BindUDUnopCall(nonLiftedArg, typeRaw, mpwi);

            ExprMemberGroup pMemGroup = GetExprFactory().CreateMemGroup(null, mpwi);
            ExprCall call = GetExprFactory().CreateCall(0, typeRet, arg, pMemGroup, null);
            call.MethWithInst = new MethWithInst(mpwi);
            call.CastOfNonLiftedResultToLiftedType = mustCast(nonLiftedResult, typeRet, 0);
            call.NullableCallLiftKind = NullableCallLiftKind.Operator;
            return call;
        }

        private ExprCall BindUDUnopCall(Expr arg, CType typeArg, MethPropWithInst mpwi)
        {
            CType typeRet = GetTypes().SubstType(mpwi.Meth().RetType, mpwi.GetType());
            checkUnsafe(typeRet); // added to the binder so we don't bind to pointer ops
            ExprMemberGroup pMemGroup = GetExprFactory().CreateMemGroup(null, mpwi);
            ExprCall call = GetExprFactory().CreateCall(0, typeRet, mustConvert(arg, typeArg), pMemGroup, null);
            call.MethWithInst = new MethWithInst(mpwi);
            verifyMethodArgs(call, mpwi.GetType());
            return call;
        }

        ////////////////////////////////////////////////////////////////////////////////
        // Given a method group or indexer group, bind it to the arguments for an 
        // invocation. This method can change the arguments to bind with Extension 
        // Methods
        private bool BindMethodGroupToArgumentsCore(out GroupToArgsBinderResult pResults, BindingFlag bindFlags, ExprMemberGroup grp, ref Expr args, int carg, bool bHasNamedArgumentSpecifiers)
        {
            ArgInfos pargInfo = new ArgInfos {carg = carg};
            FillInArgInfoFromArgList(pargInfo, args);

            ArgInfos pOriginalArgInfo = new ArgInfos {carg = carg};
            FillInArgInfoFromArgList(pOriginalArgInfo, args);

            GroupToArgsBinder binder = new GroupToArgsBinder(this, bindFlags, grp, pargInfo, pOriginalArgInfo, bHasNamedArgumentSpecifiers, null/*atsDelegate*/);
            bool retval = binder.Bind(bReportErrors: true);

            pResults = binder.GetResultsOfBind();
            return retval;
        }

        ////////////////////////////////////////////////////////////////////////////////
        // Given a method group or indexer group, bind it to the arguments for an 
        // invocation.
        internal Expr BindMethodGroupToArguments(BindingFlag bindFlags, ExprMemberGroup grp, Expr args)
        {
            Debug.Assert(grp.SymKind == SYMKIND.SK_MethodSymbol || grp.SymKind == SYMKIND.SK_PropertySymbol && ((grp.Flags & EXPRFLAG.EXF_INDEXER) != 0));

            // Count the args.
            bool argTypeErrors;
            int carg = CountArguments(args, out argTypeErrors);
            // We need to store the object because BindMethodGroupToArgumentsCore will 
            // null it out in the case of an extension method, which is then consumed
            // by BindToMethod. After that, we want to set the object back.
            Expr pObject = grp.OptionalObject;

            // If we weren't given a pName, then we couldn't bind the method pName, so we should
            // just bail out of here.

            if (grp.Name == null)
            {
                ExprCall rval = GetExprFactory().CreateCall(0, GetTypes().GetErrorSym(), args, grp, null);
                rval.SetError();
                return rval;
            }

            // If we have named arguments specified, make sure we have them all appearing after 
            // fixed arguments.
            bool seenNamed = VerifyNamedArgumentsAfterFixed(args);

            GroupToArgsBinderResult result;
            if (!BindMethodGroupToArgumentsCore(out result, bindFlags, grp, ref args, carg, seenNamed))
            {
                Debug.Assert(false, "Why didn't BindMethodGroupToArgumentsCore throw an error?");
                return null;
            }

            Expr exprRes;
            MethPropWithInst mpwiBest = result.GetBestResult();

            if (grp.SymKind == SYMKIND.SK_PropertySymbol)
            {
                Debug.Assert((grp.Flags & EXPRFLAG.EXF_INDEXER) != 0);
                //PropWithType pwt = new PropWithType(mpwiBest.Prop(), mpwiBest.GetType());

                exprRes = BindToProperty(grp.OptionalObject, new PropWithType(mpwiBest), bindFlags, args, null/*typeOther*/, grp);
            }
            else
            {
                exprRes = BindToMethod(new MethWithInst(mpwiBest), args, grp, (MemLookFlags)grp.Flags);
            }
            return exprRes;
        }

        /////////////////////////////////////////////////////////////////////////////////

        private bool VerifyNamedArgumentsAfterFixed(Expr args)
        {
            Expr list = args;
            bool seenNamed = false;
            while (list != null)
            {
                Expr arg;
                if (list is ExprList next)
                {
                    arg = next.OptionalElement;
                    list = next.OptionalNextListNode;
                }
                else
                {
                    arg = list;
                    list = null;
                }

                Debug.Assert(arg != null);
                if (arg is ExprNamedArgumentSpecification)
                {
                    seenNamed = true;
                }
                else
                {
                    if (seenNamed)
                    {
                        throw GetErrorContext().Error(ErrorCode.ERR_NamedArgumentSpecificationBeforeFixedArgument);
                    }
                }
            }

            return seenNamed;
        }

        ////////////////////////////////////////////////////////////////////////////////
        // Report a bad operator types error to the user.
        private ExprOperator BadOperatorTypesError(ExpressionKind ek, Expr pOperand1, Expr pOperand2)
        {
            return BadOperatorTypesError(ek, pOperand1, pOperand2, null);
        }

        private ExprOperator BadOperatorTypesError(ExpressionKind ek, Expr pOperand1, Expr pOperand2, CType pTypeErr)
        {
            // This is a hack, but we need to store the operation somewhere... the first argument's as 
            // good a place as any.
            string strOp = pOperand1.ErrorString;

            pOperand1 = UnwrapExpression(pOperand1);

            if (pOperand1 != null)
            {
                if (pOperand2 != null)
                {
                    pOperand2 = UnwrapExpression(pOperand2);
                    if (pOperand1.Type != null &&
                            !(pOperand1.Type is ErrorType) &&
                            pOperand2.Type != null &&
                            !(pOperand2.Type is ErrorType))
                    {
                        throw ErrorContext.Error(ErrorCode.ERR_BadBinaryOps, strOp, pOperand1.Type, pOperand2.Type);
                    }
                }
                else if (pOperand1.Type != null && !(pOperand1.Type is ErrorType))
                {
                    throw ErrorContext.Error(ErrorCode.ERR_BadUnaryOp, strOp, pOperand1.Type);
                }
            }

            if (pTypeErr == null)
            {
                pTypeErr = GetPredefindType(PredefinedType.PT_OBJECT);
            }

            ExprOperator rval = GetExprFactory().CreateOperator(ek, pTypeErr, pOperand1, pOperand2);
            rval.SetError();
            return rval;
        }

        private Expr UnwrapExpression(Expr pExpression)
        {
            while (pExpression is ExprWrap wrap)
            {
                Expr wrapped = wrap.OptionalExpression;
                if (wrapped == null)
                {
                    break;
                }

                pExpression = wrapped;
            }

            return pExpression;
        }

        private static ErrorCode GetStandardLvalueError(CheckLvalueKind kind)
        {
            switch (kind)
            {
                default:
                    VSFAIL("bad kind");
                    return ErrorCode.ERR_AssgLvalueExpected;
                case CheckLvalueKind.Assignment:
                    return ErrorCode.ERR_AssgLvalueExpected;
                case CheckLvalueKind.OutParameter:
                    return ErrorCode.ERR_RefLvalueExpected;
                case CheckLvalueKind.Increment:
                    return ErrorCode.ERR_IncrementLvalueExpected;
            }
        }

        private void CheckLvalueProp(ExprProperty prop)
        {
            Debug.Assert(prop != null);
            Debug.Assert(prop.isLvalue());

            // We have an lvalue property.  Give an error if this is an inaccessible property.

            CType type = null;
            if (prop.OptionalObjectThrough != null)
            {
                type = prop.OptionalObjectThrough.Type;
            }

            CheckPropertyAccess(prop.MethWithTypeSet, prop.PropWithTypeSlot, type);
        }

        private void CheckPropertyAccess(MethWithType mwt, PropWithType pwtSlot, CType type)
        {
            switch (SemanticChecker.CheckAccess2(mwt.Meth(), mwt.GetType(), ContextForMemberLookup(), type))
            {
                case ACCESSERROR.ACCESSERROR_NOACCESSTHRU:
                    throw ErrorContext.Error(ErrorCode.ERR_BadProtectedAccess, pwtSlot, type, ContextForMemberLookup());
                case ACCESSERROR.ACCESSERROR_NOACCESS:
                    throw ErrorContext.Error(mwt.Meth().isSetAccessor() ? ErrorCode.ERR_InaccessibleSetter : ErrorCode.ERR_InaccessibleGetter, pwtSlot);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////
        // A false return means not to process the expr any further - it's totally out
        // of place. For example - a method group or an anonymous method.
        private bool checkLvalue(Expr expr, CheckLvalueKind kind)
        {
            if (!expr.IsOK)
                return false;
            if (expr.isLvalue())
            {
                if (expr is ExprProperty prop)
                {
                    CheckLvalueProp(prop);
                }
                markFieldAssigned(expr);
                return true;
            }

            switch (expr.Kind)
            {
                case ExpressionKind.Property:
                    if (kind == CheckLvalueKind.OutParameter)
                    {
                        // passing a property as ref or out
                        throw ErrorContext.Error(ErrorCode.ERR_RefProperty);
                    }

                    ExprProperty prop = (ExprProperty)expr;
                    if (!prop.MethWithTypeSet)
                    {
                        // Assigning to a property without a setter.
                        // If we have
                        // bool? b = true; (bool)b = false;
                        // then this is realized immediately as 
                        // b.Value = false; 
                        // and no ExpressionKind.EK_CAST is generated. We'd rather not give a "you're writing
                        // to a read-only property" error in the case where the property access
                        // is not explicit in the source code.  Fortunately in this case the
                        // cast is still hanging around in the parse tree, so we can look for it.

                        // POSSIBLE ERROR: It would be nice to also give this error for other situations
                        // POSSIBLE ERROR: in which the user is attempting to assign to a value, such as
                        // POSSIBLE ERROR: an explicit (bool)b.Value = false;
                        // POSSIBLE ERROR: Unfortunately we cannot use this trick in that situation because
                        // POSSIBLE ERROR: we've already discarded the OperatorKind.OP_CAST node.  (This is an SyntaxKind.Dot).

                        // SPEC VIOLATION: More generally:
                        // SPEC VIOLATION: The spec states that the result of any cast is a value, not a
                        // SPEC VIOLATION: variable. Unfortunately we do not correctly implement this
                        // SPEC VIOLATION: and we probably should not start implementing it because this
                        // SPEC VIOLATION: would be a breaking change.  We currently discard "no op" casts
                        // SPEC VIOLATION: very aggressively rather than generating an ExpressionKind.EK_CAST node.

                        throw ErrorContext.Error(ErrorCode.ERR_AssgReadonlyProp, prop.PropWithTypeSlot);
                    }
                    break;

                case ExpressionKind.BoundLambda:
                case ExpressionKind.Constant:
                    throw ErrorContext.Error(GetStandardLvalueError(kind));

                case ExpressionKind.MemberGroup:
                    ErrorCode err = (kind == CheckLvalueKind.OutParameter) ? ErrorCode.ERR_RefReadonlyLocalCause : ErrorCode.ERR_AssgReadonlyLocalCause;
                    throw ErrorContext.Error(err, ((ExprMemberGroup)expr).Name, new ErrArgIds(MessageID.MethodGroup));
            }

            TryReportLvalueFailure(expr, kind);
            return true;
        }

        private void PostBindMethod(ref MethWithInst pMWI, Expr pObject)
        {
            MethWithInst mwiOrig = pMWI;

            // If it is virtual, find a remap of the method to something more specific.  This
            // may alter where the method is found.
            if (pObject != null && pObject.Type.isSimpleType())
            {
                RemapToOverride(GetSymbolLoader(), pMWI, pObject.Type);
            }

            if (pMWI.Meth().RetType != null)
            {
                checkUnsafe(pMWI.Meth().RetType);

                // We need to check unsafe on the parameters as well, since we cannot check in conversion.
                TypeArray pParams = pMWI.Meth().Params;

                for (int i = 0; i < pParams.Count; i++)
                {
                    // This is an optimization: don't call this in the vast majority of cases
                    CType type = pParams[i];

                    if (type.isUnsafe())
                    {
                        checkUnsafe(type);
                    }
                }
            }
        }

        private void PostBindProperty(PropWithType pwt, Expr pObject, out MethWithType pmwtGet, out MethWithType pmwtSet)
        {
            pmwtGet = new MethWithType();
            pmwtSet = new MethWithType();
            // Get the accessors.
            if (pwt.Prop().GetterMethod != null)
            {
                pmwtGet.Set(pwt.Prop().GetterMethod, pwt.GetType());
            }
            else
            {
                pmwtGet.Clear();
            }

            if (pwt.Prop().SetterMethod != null)
            {
                pmwtSet.Set(pwt.Prop().SetterMethod, pwt.GetType());
            }
            else
            {
                pmwtSet.Clear();
            }

            if (pwt.Prop().RetType != null)
            {
                checkUnsafe(pwt.Prop().RetType);
            }
        }

        private Expr AdjustMemberObject(SymWithType swt, Expr pObject, out bool pfConstrained)
        {
            // Assert that the type is present and is an instantiation of the member's parent.
            Debug.Assert(swt.GetType() != null && swt.GetType().getAggregate() == swt.Sym.parent as AggregateSymbol);
            bool bIsMatchingStatic = IsMatchingStatic(swt, pObject);
            pfConstrained = false;

            bool isStatic = swt.Sym.isStatic;

            // If our static doesn't match, bail out of here.
            if (!bIsMatchingStatic)
            {
                if (isStatic)
                {
                    // If we have a mismatched static, a static method, and the binding flag
                    // that tells us we're binding simple names, then insert a type here instead.
                    if ((pObject.Flags & EXPRFLAG.EXF_SIMPLENAME) != 0)
                    {
                        // We've made the static match now.
                        return null;
                    }

                    throw ErrorContext.Error(ErrorCode.ERR_ObjectProhibited, swt);
                }

                throw ErrorContext.Error(ErrorCode.ERR_ObjectRequired, swt);
            }

            // At this point, all errors for static invocations have been reported, and
            // the object has been nulled out. So return out of here.
            if (isStatic)
            {
                return null;
            }

            // If we're in a constructor, then bail.
            if ((swt.Sym is MethodSymbol) && swt.Meth().IsConstructor())
            {
                return pObject;
            }

            if (pObject == null)
            {
                return null;
            }

            CType typeObj = pObject.Type;
            CType typeTmp;

            if (typeObj is NullableType nubTypeObj && (typeTmp = nubTypeObj.GetAts()) != swt.GetType())
            {
                typeObj = typeTmp;
            }

            if (typeObj is TypeParameterType || typeObj is AggregateType)
            {
                AggregateSymbol aggCalled = swt.Sym.parent as AggregateSymbol;
                Debug.Assert(swt.GetType().getAggregate() == aggCalled);

                // If we're invoking code on a struct-valued field, mark the struct as assigned (to
                // avoid warning CS0649).
                if (pObject is ExprField field && !field.FieldWithType.Field().isAssigned && !(swt.Sym is FieldSymbol) &&
                    typeObj.isStructType() && !typeObj.isPredefined())
                {
                    field.FieldWithType.Field().isAssigned = true;
                }

                if (pfConstrained &&
                    (typeObj is TypeParameterType ||
                     typeObj.isStructType() && swt.GetType().IsRefType() && swt.Sym.IsVirtual()))
                {
                    // For calls on type parameters or virtual calls on struct types (not enums),
                    // use the constrained prefix.
                    pfConstrained = true;
                }

                Expr objNew = tryConvert(pObject, swt.GetType(), CONVERTTYPE.NOUDC);

                // This check ensures that we do not bind to methods in an outer class
                // which are visible, but whose this pointer is of an incorrect type...
                // ... also handles case of calling an pObject method on a RefAny value.
                // WE don't give a great message for this, but it'll do.
                if (objNew == null)
                {
                    throw ErrorContext.Error(ErrorCode.ERR_WrongNestedThis, swt.GetType(), pObject.Type);
                }

                pObject = objNew;
            }

            return pObject;
        }
        /////////////////////////////////////////////////////////////////////////////////

        private bool IsMatchingStatic(SymWithType swt, Expr pObject)
        {
            Symbol pSym = swt.Sym;

            // Instance constructors are always ok, static constructors are never ok.
            if (pSym is MethodSymbol meth && meth.IsConstructor())
            {
                return !meth.isStatic;
            }

            bool isStatic = swt.Sym.isStatic;

            if (isStatic)
            {
                // If we're static and we don't have an object then we're ok.
                if (pObject == null)
                {
                    return true;
                }

                if ((pObject.Flags & EXPRFLAG.EXF_SAMENAMETYPE) == 0)
                {
                    return false;
                }
            }
            else if (pObject == null)
            {
                // We're not static, and we don't have an object. This is ok in certain scenarios:
                return false;
            }

            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////
        // this determines whether the expression as an pObject of a prop or field is an
        // lvalue

        private bool objectIsLvalue(Expr pObject)
        {
            return (
                       pObject == null ||  // statics are always lvalues
                       (((pObject.Flags & EXPRFLAG.EXF_LVALUE) != 0) && (pObject.Kind != ExpressionKind.Property)) ||
                       // things marked as lvalues have props/fields which are lvalues, with one exception:  props of structs
                       // do not have fields/structs as lvalues

                       !pObject.Type.isStructOrEnum()
                   // non-struct types are lvalues (such as non-struct method returns)
                   );
        }
        ////////////////////////////////////////////////////////////////////////////////
        // For a base call we need to remap from the virtual to the specific override 
        // to invoke.  This is also used to map a virtual on pObject (like ToString) to 
        // the specific override when the pObject is a simple type (int, bool, char, 
        // etc). In these cases it is safe to assume that any override won't later be 
        // removed.... We start searching from "typeObj" up the superclass hierarchy 
        // until we find a method with an exact signature match.

        private static void RemapToOverride(SymbolLoader symbolLoader, SymWithType pswt, CType typeObj)
        {
            // For a property/indexer we remap the accessors, not the property/indexer.
            // Since every event has both accessors we remap the event instead of the accessors.
            Debug.Assert(pswt && (pswt.Sym is MethodSymbol || pswt.Sym is EventSymbol || pswt.Sym is MethodOrPropertySymbol));
            Debug.Assert(typeObj != null);

            // Don't remap static or interface methods.
            if (typeObj is NullableType nubTypeObj)
            {
                typeObj = nubTypeObj.GetAts();
            }

            // Don't remap non-virtual members
            if (!(typeObj is AggregateType atsObj) || atsObj.isInterfaceType() || !pswt.Sym.IsVirtual())
            {
                return;
            }

            symbmask_t mask = pswt.Sym.mask();

            // Search for an override version of the method.
            while (atsObj != null && atsObj.getAggregate() != pswt.Sym.parent)
            {
                for (Symbol symT = symbolLoader.LookupAggMember(pswt.Sym.name, atsObj.getAggregate(), mask);
                     symT != null;
                     symT = symbolLoader.LookupNextSym(symT, atsObj.getAggregate(), mask))
                {
                    if (symT.IsOverride() && (symT.SymBaseVirtual() == pswt.Sym || symT.SymBaseVirtual() == pswt.Sym.SymBaseVirtual()))
                    {
                        pswt.Set(symT, atsObj);
                        return;
                    }
                }
                atsObj = atsObj.GetBaseClass();
            }
        }

        private void verifyMethodArgs(IExprWithArgs call, CType callingObjectType)
        {
            Debug.Assert(call != null);
            Expr argsPtr = call.OptionalArguments;
            SymWithType swt = call.GetSymWithType();
            MethodOrPropertySymbol mp = swt.Sym as MethodOrPropertySymbol;
            TypeArray pTypeArgs = (call as ExprCall)?.MethWithInst.TypeArgs;
            Expr newArgs;
            AdjustCallArgumentsForParams(callingObjectType, swt.GetType(), mp, pTypeArgs, argsPtr, out newArgs);
            call.OptionalArguments = newArgs;
        }

        private void AdjustCallArgumentsForParams(CType callingObjectType, CType type, MethodOrPropertySymbol mp, TypeArray pTypeArgs, Expr argsPtr, out Expr newArgs)
        {
            Debug.Assert(mp != null);
            Debug.Assert(mp.Params != null);
            newArgs = null;
            Expr newArgsTail = null;

            MethodOrPropertySymbol mostDerivedMethod = GroupToArgsBinder.FindMostDerivedMethod(GetSymbolLoader(), mp, callingObjectType);

            int paramCount = mp.Params.Count;
            TypeArray @params = mp.Params;
            int iDst = 0;
            MethodSymbol m = mp as MethodSymbol;
            int argCount = ExpressionIterator.Count(argsPtr);

            if (m != null && m.isVarargs)
            {
                paramCount--; // we don't care about the vararg sentinel
            }

            bool bDontFixParamArray = false;

            ExpressionIterator it = new ExpressionIterator(argsPtr);

            if (argsPtr == null)
            {
                if (mp.isParamArray)
                    goto FIXUPPARAMLIST;
                return;
            }
            for (; !it.AtEnd(); it.MoveNext())
            {
                Expr indir = it.Current();
                // this will splice the optional arguments into the list

                if (indir.Type is ParameterModifierType)
                {
                    if (paramCount != 0)
                        paramCount--;
                    GetExprFactory().AppendItemToList(indir, ref newArgs, ref newArgsTail);
                }
                else if (paramCount != 0)
                {
                    if (paramCount == 1 && mp.isParamArray && argCount > mp.Params.Count)
                    {
                        // we arrived at the last formal, and we have more than one actual, so
                        // we need to put the rest in an array...
                        goto FIXUPPARAMLIST;
                    }

                    Expr argument = indir;
                    Expr rval;
                    if (argument is ExprNamedArgumentSpecification named)
                    {
                        int index = 0;
                        // If we're named, look for the type of the matching name.
                        foreach (Name i in mostDerivedMethod.ParameterNames)
                        {
                            if (i == named.Name)
                            {
                                break;
                            }
                            index++;
                        }
                        Debug.Assert(index != mp.Params.Count);
                        CType substDestType = GetTypes().SubstType(@params[index], type, pTypeArgs);

                        // If we cant convert the argument and we're the param array argument, then deal with it.
                        if (!canConvert(named.Value, substDestType) &&
                            mp.isParamArray && index == mp.Params.Count - 1)
                        {
                            // We have a param array, but we're not at the end yet. This will happen
                            // with named arguments when the user specifies a name for the param array,
                            // and its not an actual array.
                            // 
                            // For example:
                            // void Foo(int y, params int[] x);
                            // ...
                            // Foo(x:1, y:1);
                            CType arrayType = (ArrayType)GetTypes().SubstType(mp.Params[mp.Params.Count - 1], type, pTypeArgs);

                            // Use an EK_ARRINIT even in the empty case so empty param arrays in attributes work.
                            ExprArrayInit arrayInit = GetExprFactory().CreateArrayInit(arrayType, null, null, new[] { 0 }, 1);
                            arrayInit.GeneratedForParamArray = true;
                            arrayInit.OptionalArguments = named.Value;

                            named.Value = arrayInit;
                            bDontFixParamArray = true;
                        }
                        else
                        {
                            // Otherwise, force the conversion and get errors if needed.
                            named.Value = tryConvert(named.Value, substDestType);
                        }
                        rval = argument;
                    }
                    else
                    {
                        CType substDestType = GetTypes().SubstType(@params[iDst], type, pTypeArgs);
                        rval = tryConvert(indir, substDestType);
                    }

                    if (rval == null)
                    {
                        // the last arg failed to fix up, so it must fixup into the array element
                        // if we have a param array (we will be passing a 1 element array...)
                        if (mp.isParamArray && paramCount == 1 && argCount >= mp.Params.Count)
                        {
                            goto FIXUPPARAMLIST;
                        }
                        else
                        {
                            // This is either the error case that the args are of the wrong type, 
                            // or that we have some optional arguments being used. Either way,
                            // we won't need to expand the param array.
                            return;
                        }
                    }
                    Debug.Assert(rval != null);
                    indir = rval;
                    GetExprFactory().AppendItemToList(rval, ref newArgs, ref newArgsTail);
                    paramCount--;
                }
                // note that destype might not be valid if we are in varargs, but then we won't ever use it...
                iDst++;

                if (paramCount != 0 && mp.isParamArray && iDst == argCount)
                {
                    // we run out of actuals, but we still have formals, so this is an empty array being passed
                    // into the last param...
                    indir = null;
                    it.MoveNext();
                    goto FIXUPPARAMLIST;
                }
            }

            return;

        FIXUPPARAMLIST:
            if (bDontFixParamArray)
            {
                // We've already fixed the param array for named arguments.
                return;
            }

            // we need to create an array and put it as the last arg...
            CType substitutedArrayType = GetTypes().SubstType(mp.Params[mp.Params.Count - 1], type, pTypeArgs);
            if (!(substitutedArrayType is ArrayType subArr) || !subArr.IsSZArray)
            {
                // Invalid type for params array parameter. Happens in LAF scenarios, e.g.
                //
                // void Foo(int i, params int ar = null) { }
                // ...
                // Foo(1);
                return;
            }

            CType elementType = subArr.GetElementType();

            // Use an EK_ARRINIT even in the empty case so empty param arrays in attributes work.
            ExprArrayInit exprArrayInit = GetExprFactory().CreateArrayInit(substitutedArrayType, null, null, new[] { 0 }, 1);
            exprArrayInit.GeneratedForParamArray = true;

            if (it.AtEnd())
            {
                exprArrayInit.DimensionSize = 0;
                exprArrayInit.DimensionSizes[0] = 0;
                exprArrayInit.OptionalArguments = null;
                if (argsPtr == null)
                {
                    argsPtr = exprArrayInit;
                }
                else
                {
                    argsPtr = GetExprFactory().CreateList(argsPtr, exprArrayInit);
                }
                GetExprFactory().AppendItemToList(exprArrayInit, ref newArgs, ref newArgsTail);
            }
            else
            {
                // Go through the list - for each argument, do the conversion and append it to the new list.
                Expr newList = null;
                Expr newListTail = null;
                int count = 0;

                for (; !it.AtEnd(); it.MoveNext())
                {
                    Expr expr = it.Current();
                    count++;

                    if (expr is ExprNamedArgumentSpecification named)
                    {
                        named.Value = tryConvert(named.Value, elementType);
                    }
                    else
                    {
                        expr = tryConvert(expr, elementType);
                    }
                    GetExprFactory().AppendItemToList(expr, ref newList, ref newListTail);
                }

                exprArrayInit.DimensionSize = count;
                exprArrayInit.DimensionSizes[0] = count;
                exprArrayInit.OptionalArguments = newList;
                GetExprFactory().AppendItemToList(exprArrayInit, ref newArgs, ref newArgsTail);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////
        // Sets the isAssigned bit

        private void markFieldAssigned(Expr expr)
        {
            if (0 != (expr.Flags & EXPRFLAG.EXF_LVALUE) && expr is ExprField field)
            {
                FieldSymbol symbol;
                do
                {
                    symbol = field.FieldWithType.Field();
                    symbol.isAssigned = true;
                    expr = field.OptionalObject;
                }
                while (symbol.getClass().IsStruct() && !symbol.isStatic && expr != null && (field = expr as ExprField) != null);
            }
        }

        private static readonly PredefinedType[] s_rgptIntOp =
        {
            PredefinedType.PT_INT,
            PredefinedType.PT_UINT,
            PredefinedType.PT_LONG,
            PredefinedType.PT_ULONG
        };



        internal CType chooseArrayIndexType(Expr args)
        {
            // first, select the allowable types
            for (int ipt = 0; ipt < s_rgptIntOp.Length; ipt++)
            {
                CType type = GetPredefindType(s_rgptIntOp[ipt]);
                foreach (Expr arg in args.ToEnumerable())
                {
                    if (!canConvert(arg, type))
                    {
                        goto NEXTI;
                    }
                }
                return type;
            NEXTI:
                ;
            }
            return null;
        }

        internal void FillInArgInfoFromArgList(ArgInfos argInfo, Expr args)
        {
            CType[] prgtype = new CType[argInfo.carg];
            argInfo.fHasExprs = true;
            argInfo.prgexpr = new List<Expr>();

            int iarg = 0;
            for (Expr list = args; list != null; iarg++)
            {
                Expr arg;
                if (list is ExprList next)
                {
                    arg = next.OptionalElement;
                    list = next.OptionalNextListNode;
                }
                else
                {
                    arg = list;
                    list = null;
                }

                Debug.Assert(arg != null);

                if (arg.Type != null)
                {
                    prgtype[iarg] = arg.Type;
                }
                else
                {
                    prgtype[iarg] = GetTypes().GetErrorSym();
                }
                argInfo.prgexpr.Add(arg);
            }

            Debug.Assert(iarg <= argInfo.carg);
            argInfo.types = GetGlobalSymbols().AllocParams(iarg, prgtype);
        }

        private bool TryGetExpandedParams(TypeArray @params, int count, out TypeArray ppExpandedParams)
        {
            CType[] prgtype;
            if (count < @params.Count - 1)
            {
                // The user has specified less arguments than our parameters, but we still
                // need to return our set of types without the param array. This is in the 
                // case that all the parameters are optional.
                prgtype = new CType[@params.Count - 1];
                @params.CopyItems(0, @params.Count - 1, prgtype);
                ppExpandedParams = GetGlobalSymbols().AllocParams(@params.Count - 1, prgtype);
                return true;
            }

            prgtype = new CType[count];
            @params.CopyItems(0, @params.Count - 1, prgtype);

            CType type = @params[@params.Count - 1];
            CType elementType = null;

            if (!(type is ArrayType arr))
            {
                ppExpandedParams = null;
                // If we don't have an array sym, we don't have expanded parameters.
                return false;
            }

            // At this point, we have an array sym.
            elementType = arr.GetElementType();

            for (int itype = @params.Count - 1; itype < count; itype++)
            {
                prgtype[itype] = elementType;
            }

            ppExpandedParams = GetGlobalSymbols().AllocParams(prgtype);

            return true;
        }

        // Is the method/property callable. Not if it's an override or not user-callable.
        public static bool IsMethPropCallable(MethodOrPropertySymbol sym, bool requireUC)
        {
            // The hide-by-pName option for binding other languages takes precedence over general
            // rules of not binding to overrides.
            return (!sym.isOverride || sym.isHideByName) && (!requireUC || sym.isUserCallable());
        }

        private bool isConvInTable(List<UdConvInfo> convTable, MethodSymbol meth, AggregateType ats, bool fSrc, bool fDst)
        {
            foreach (UdConvInfo conv in convTable)
            {
                if (conv.mwt.Meth() == meth &&
                    conv.mwt.GetType() == ats &&
                    conv.fSrcImplicit == fSrc &&
                    conv.fDstImplicit == fDst)
                {
                    return true;
                }
            }
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////
        // Check to see if an integral constant is within range of a integral 
        // destination type.
        private static bool isConstantInRange(ExprConstant exprSrc, CType typeDest)
        {
            return isConstantInRange(exprSrc, typeDest, false);
        }

        private static bool isConstantInRange(ExprConstant exprSrc, CType typeDest, bool realsOk)
        {
            FUNDTYPE ftSrc = exprSrc.Type.fundType();
            FUNDTYPE ftDest = typeDest.fundType();

            if (ftSrc > FUNDTYPE.FT_LASTINTEGRAL || ftDest > FUNDTYPE.FT_LASTINTEGRAL)
            {
                if (!realsOk)
                {
                    return false;
                }
                else if (ftSrc > FUNDTYPE.FT_LASTNUMERIC || ftDest > FUNDTYPE.FT_LASTNUMERIC)
                {
                    return false;
                }
            }

            // if converting to a float type, this always succeeds...
            if (ftDest > FUNDTYPE.FT_LASTINTEGRAL)
            {
                return true;
            }

            // if converting from float to an integral type, we need to check whether it fits
            if (ftSrc > FUNDTYPE.FT_LASTINTEGRAL)
            {
                double dvalue = exprSrc.Val.DoubleVal;

                switch (ftDest)
                {
                    case FUNDTYPE.FT_I1:
                        if (dvalue > -0x81 && dvalue < 0x80)
                            return true;
                        break;
                    case FUNDTYPE.FT_I2:
                        if (dvalue > -0x8001 && dvalue < 0x8000)
                            return true;
                        break;
                    case FUNDTYPE.FT_I4:
                        if (dvalue > I64(-0x80000001) && dvalue < I64(0x80000000))
                            return true;
                        break;
                    case FUNDTYPE.FT_I8:
                        // 0x7FFFFFFFFFFFFFFFFFFF is rounded to 0x800000000000000000000 in 64 bit double precision
                        // floating point representation. The conversion back to ulong is not possible.
                        if (dvalue >= -9223372036854775808.0 && dvalue < 9223372036854775808.0)
                        {
                            return true;
                        }
                        break;
                    case FUNDTYPE.FT_U1:
                        if (dvalue > -1 && dvalue < 0x100)
                            return true;
                        break;
                    case FUNDTYPE.FT_U2:
                        if (dvalue > -1 && dvalue < 0x10000)
                            return true;
                        break;
                    case FUNDTYPE.FT_U4:
                        if (dvalue > -1 && dvalue < I64(0x100000000))
                            return true;
                        break;
                    case FUNDTYPE.FT_U8:
                        // 0xFFFFFFFFFFFFFFFFFFFF is rounded to 0x100000000000000000000 in 64 bit double precision
                        // floating point representation. The conversion back to ulong is not possible.
                        if (dvalue > -1.0 && dvalue < 18446744073709551616.0)
                        {
                            return true;
                        }
                        break;
                    default:
                        break;
                }
                return false;
            }

            // U8 src is unsigned, so deal with values > MAX_LONG here.
            if (ftSrc == FUNDTYPE.FT_U8)
            {
                ulong value = exprSrc.UInt64Value;

                switch (ftDest)
                {
                    case FUNDTYPE.FT_I1:
                        if (value <= (ulong)sbyte.MaxValue)
                            return true;
                        break;
                    case FUNDTYPE.FT_I2:
                        if (value <= (ulong)short.MaxValue)
                            return true;
                        break;
                    case FUNDTYPE.FT_I4:
                        if (value <= int.MaxValue)
                            return true;
                        break;
                    case FUNDTYPE.FT_I8:
                        if (value <= long.MaxValue)
                            return true;
                        break;
                    case FUNDTYPE.FT_U1:
                        if (value <= byte.MaxValue)
                            return true;
                        break;
                    case FUNDTYPE.FT_U2:
                        if (value <= ushort.MaxValue)
                            return true;
                        break;
                    case FUNDTYPE.FT_U4:
                        if (value <= uint.MaxValue)
                            return true;
                        break;
                    case FUNDTYPE.FT_U8:
                        return true;
                    default:
                        break;
                }
            }
            else
            {
                long value = exprSrc.Int64Value;

                switch (ftDest)
                {
                    case FUNDTYPE.FT_I1:
                        if (value >= -128 && value <= 127)
                            return true;
                        break;
                    case FUNDTYPE.FT_I2:
                        if (value >= -0x8000 && value <= 0x7fff)
                            return true;
                        break;
                    case FUNDTYPE.FT_I4:
                        if (value >= I64(-0x80000000) && value <= I64(0x7fffffff))
                            return true;
                        break;
                    case FUNDTYPE.FT_I8:
                        return true;
                    case FUNDTYPE.FT_U1:
                        if (value >= 0 && value <= 0xff)
                            return true;
                        break;
                    case FUNDTYPE.FT_U2:
                        if (value >= 0 && value <= 0xffff)
                            return true;
                        break;
                    case FUNDTYPE.FT_U4:
                        if (value >= 0 && value <= I64(0xffffffff))
                            return true;
                        break;
                    case FUNDTYPE.FT_U8:
                        if (value >= 0)
                            return true;
                        break;
                    default:
                        break;
                }
            }
            return false;
        }

        private static readonly PredefinedName[] s_EK2NAME =
        {
            PredefinedName.PN_OPEQUALS,
            PredefinedName.PN_OPCOMPARE,
            PredefinedName.PN_OPTRUE,
            PredefinedName.PN_OPFALSE,
            PredefinedName.PN_OPINCREMENT,
            PredefinedName.PN_OPDECREMENT,
            PredefinedName.PN_OPNEGATION,
            PredefinedName.PN_OPEQUALITY,
            PredefinedName.PN_OPINEQUALITY,
            PredefinedName.PN_OPLESSTHAN,
            PredefinedName.PN_OPLESSTHANOREQUAL,
            PredefinedName.PN_OPGREATERTHAN,
            PredefinedName.PN_OPGREATERTHANOREQUAL,
            PredefinedName.PN_OPPLUS,
            PredefinedName.PN_OPMINUS,
            PredefinedName.PN_OPMULTIPLY,
            PredefinedName.PN_OPDIVISION,
            PredefinedName.PN_OPMODULUS,
            PredefinedName.PN_OPUNARYMINUS,
            PredefinedName.PN_OPUNARYPLUS,
            PredefinedName.PN_OPBITWISEAND,
            PredefinedName.PN_OPBITWISEOR,
            PredefinedName.PN_OPXOR,
            PredefinedName.PN_OPCOMPLEMENT,
            PredefinedName.PN_OPLEFTSHIFT,
            PredefinedName.PN_OPRIGHTSHIFT,
        };

        private Name ekName(ExpressionKind ek)
        {
            Debug.Assert(ek >= ExpressionKind.FirstOp && (ek - ExpressionKind.FirstOp) < (int)s_EK2NAME.Length);
            return NameManager.GetPredefinedName(s_EK2NAME[ek - ExpressionKind.FirstOp]);
        }

        private void checkUnsafe(CType type)
        {
            if (type == null || type.isUnsafe())
            {
                throw ErrorContext.Error(ErrorCode.ERR_UnsafeNeeded);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////
        private AggregateDeclaration ContextForMemberLookup()
        {
            return Context.ContextForMemberLookup;
        }

        private ExprWrap WrapShortLivedExpression(Expr expr)
        {
            return GetExprFactory().CreateWrap(expr);
        }

        private ExprAssignment GenerateOptimizedAssignment(Expr op1, Expr op2)
        {
            return GetExprFactory().CreateAssignment(op1, op2);
        }

        internal static int CountArguments(Expr args, out bool typeErrors)
        {
            int carg = 0;
            typeErrors = false;
            for (Expr list = args; list != null; carg++)
            {
                Expr arg;

                if (list is ExprList next)
                {
                    arg = next.OptionalElement;
                    list = next.OptionalNextListNode;
                }
                else
                {
                    arg = list;
                    list = null;
                }

                Debug.Assert(arg != null);

                if (arg.Type == null || arg.Type is ErrorType)
                {
                    typeErrors = true;
                }
            }
            return carg;
        }
    }
}

