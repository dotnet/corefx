// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.CSharp.RuntimeBinder.Errors
{
    public enum ErrorCode
    {
        ERR_BadBinaryOps = 19,
        ERR_IntDivByZero = 20,
        ERR_BadIndexLHS = 21,
        ERR_BadIndexCount = 22,
        ERR_BadUnaryOp = 23,
        ERR_NoImplicitConv = 29,
        ERR_NoExplicitConv = 30,
        ERR_ConstOutOfRange = 31,
        ERR_AmbigBinaryOps = 34,
        ERR_AmbigUnaryOp = 35,
        ERR_ValueCantBeNull = 37,
        ERR_WrongNestedThis = 38,
        ERR_NoSuchMember = 117,
        ERR_ObjectRequired = 120,
        ERR_AmbigCall = 121,
        ERR_BadAccess = 122,
        ERR_MethDelegateMismatch = 123,
        ERR_AssgLvalueExpected = 131,
        ERR_NoConstructors = 143,
        ERR_BadDelegateConstructor = 148,
        ERR_PropertyLacksGet = 154,
        ERR_ObjectProhibited = 176,
        ERR_AssgReadonly = 191,
        ERR_RefReadonly = 192,
        ERR_AssgReadonlyStatic = 198,
        ERR_RefReadonlyStatic = 199,
        ERR_AssgReadonlyProp = 200,
        ERR_AbstractBaseCall = 205,
        ERR_RefProperty = 206,
        ERR_ManagedAddr = 208,
        ERR_FixedNotNeeded = 213,
        ERR_UnsafeNeeded = 214,
        ERR_BadBoolOp = 217,
        ERR_MustHaveOpTF = 218,
        ERR_CheckedOverflow = 220,
        ERR_ConstOutOfRangeChecked = 221,
        ERR_AmbigMember = 229,
        ERR_SizeofUnsafe = 233,
        ERR_FieldInitRefNonstatic = 236,
        ERR_CallingFinalizeDepracated = 245,
        ERR_CallingBaseFinalizeDeprecated = 250,
        ERR_BadCastInFixed = 254,
        ERR_NoImplicitConvCast = 266,
        ERR_InaccessibleGetter = 271,
        ERR_InaccessibleSetter = 272,
        ERR_BadArity = 305,
        ERR_BadTypeArgument = 306,
        ERR_TypeArgsNotAllowed = 307,
        ERR_HasNoTypeVars = 308,
        ERR_NewConstraintNotSatisfied = 310,
        ERR_GenericConstraintNotSatisfiedRefType = 311,
        ERR_GenericConstraintNotSatisfiedNullableEnum = 312,
        ERR_GenericConstraintNotSatisfiedNullableInterface = 313,
        ERR_GenericConstraintNotSatisfiedTyVar = 314,
        ERR_GenericConstraintNotSatisfiedValType = 315,
        ERR_TypeVarCantBeNull = 403,
        ERR_BadRetType = 407,
        ERR_CantInferMethTypeArgs = 411,
        ERR_MethGrpToNonDel = 428,
        ERR_RefConstraintNotSatisfied = 452,
        ERR_ValConstraintNotSatisfied = 453,
        ERR_CircularConstraint = 454,
        ERR_BaseConstraintConflict = 455,
        ERR_ConWithValCon = 456,
        ERR_AmbigUDConv = 457,
        ERR_PredefinedTypeNotFound = 518,
        ERR_PredefinedTypeBadType = 520,
        ERR_BindToBogus = 570,
        ERR_CantCallSpecialMethod = 571,
        ERR_BogusType = 648,
        ERR_MissingPredefinedMember = 656,
        ERR_LiteralDoubleCast = 664,
        ERR_UnifyingInterfaceInstantiations = 695,
        ERR_ConvertToStaticClass = 716,
        ERR_GenericArgIsStaticClass = 718,
        ERR_PartialMethodToDelegate = 762,
        ERR_IncrementLvalueExpected = 1059,
        ERR_NoSuchMemberOrExtension = 1061,
        ERR_ValueTypeExtDelegate = 1113,
        ERR_BadArgCount = 1501,
        ERR_BadArgTypes = 1502,
        ERR_BadArgType = 1503,
        ERR_RefLvalueExpected = 1510,
        ERR_BadProtectedAccess = 1540,
        ERR_BindToBogusProp2 = 1545,
        ERR_BindToBogusProp1 = 1546,
        ERR_BadDelArgCount = 1593,
        ERR_BadDelArgTypes = 1594,
        ERR_AssgReadonlyLocal = 1604,
        ERR_RefReadonlyLocal = 1605,
        ERR_ReturnNotLValue = 1612,
        ERR_BadArgExtraRef = 1615,
        ERR_BadArgRef = 1620,
        ERR_AssgReadonly2 = 1648,
        ERR_RefReadonly2 = 1649,
        ERR_AssgReadonlyStatic2 = 1650,
        ERR_RefReadonlyStatic2 = 1651,
        ERR_AssgReadonlyLocalCause = 1656,
        ERR_RefReadonlyLocalCause = 1657,
        ERR_ThisStructNotInAnonMeth = 1673,
        ERR_DelegateOnNullable = 1728,
        ERR_BadCtorArgCount = 1729,
        ERR_BadExtensionArgTypes = 1928,
        ERR_BadInstanceArgType = 1929,
        ERR_BadArgTypesForCollectionAdd = 1950,
        ERR_InitializerAddHasParamModifiers = 1954,
        ERR_NonInvocableMemberCalled = 1955,
        ERR_NamedArgumentSpecificationBeforeFixedArgument = 5002,
        ERR_BadNamedArgument = 5003,
        ERR_BadNamedArgumentForDelegateInvoke = 5004,
        ERR_DuplicateNamedArgument = 5005,
        ERR_NamedArgumentUsedInPositional = 5006,
    }
}
