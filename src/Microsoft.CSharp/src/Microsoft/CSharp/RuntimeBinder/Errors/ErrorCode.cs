// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.CSharp.RuntimeBinder.Errors
{
    internal enum ErrorCode
    {
        ERR_BadBinaryOps = 19,
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
        ERR_PropertyLacksGet = 154,
        ERR_ObjectProhibited = 176,
        ERR_AssgReadonly = 191,
        ERR_RefReadonly = 192,
        ERR_AssgReadonlyStatic = 198,
        ERR_RefReadonlyStatic = 199,
        ERR_AssgReadonlyProp = 200,
        ERR_RefProperty = 206,
        ERR_UnsafeNeeded = 214,
        ERR_BadBoolOp = 217,
        ERR_MustHaveOpTF = 218,
        ERR_ConstOutOfRangeChecked = 221,
        ERR_AmbigMember = 229,
        ERR_NoImplicitConvCast = 266,
        ERR_InaccessibleGetter = 271,
        ERR_InaccessibleSetter = 272,
        ERR_BadArity = 305,
        ERR_TypeArgsNotAllowed = 307,
        ERR_HasNoTypeVars = 308,
        ERR_NewConstraintNotSatisfied = 310,
        ERR_GenericConstraintNotSatisfiedRefType = 311,
        ERR_GenericConstraintNotSatisfiedNullableEnum = 312,
        ERR_GenericConstraintNotSatisfiedNullableInterface = 313,
        ERR_GenericConstraintNotSatisfiedValType = 315,
        ERR_CantInferMethTypeArgs = 411,
        ERR_RefConstraintNotSatisfied = 452,
        ERR_ValConstraintNotSatisfied = 453,
        ERR_AmbigUDConv = 457,
        ERR_BindToBogus = 570,
        ERR_CantCallSpecialMethod = 571,
        ERR_ConvertToStaticClass = 716,
        ERR_IncrementLvalueExpected = 1059,
        ERR_BadArgCount = 1501,
        ERR_BadArgTypes = 1502,
        ERR_RefLvalueExpected = 1510,
        ERR_BadProtectedAccess = 1540,
        ERR_BindToBogusProp2 = 1545,
        ERR_BindToBogusProp1 = 1546,
        ERR_BadDelArgCount = 1593,
        ERR_BadDelArgTypes = 1594,
        ERR_AssgReadonlyLocal = 1604,
        ERR_RefReadonlyLocal = 1605,
        ERR_ReturnNotLValue = 1612,
        ERR_AssgReadonly2 = 1648,
        ERR_RefReadonly2 = 1649,
        ERR_AssgReadonlyStatic2 = 1650,
        ERR_RefReadonlyStatic2 = 1651,
        ERR_AssgReadonlyLocalCause = 1656,
        ERR_RefReadonlyLocalCause = 1657,
        ERR_BadCtorArgCount = 1729,
        ERR_NonInvocableMemberCalled = 1955,
        ERR_NamedArgumentSpecificationBeforeFixedArgument = 5002,
        ERR_BadNamedArgument = 5003,
        ERR_BadNamedArgumentForDelegateInvoke = 5004,
        ERR_DuplicateNamedArgument = 5005,
        ERR_NamedArgumentUsedInPositional = 5006,
    }
}
