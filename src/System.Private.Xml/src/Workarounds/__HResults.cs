﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System
{
    // Note: FACILITY_URT is defined as 0x13 (0x8013xxxx).  Within that
    // range, 0x1yyy is for Runtime errors (used for Security, Metadata, etc).
    // In that subrange, 0x15zz and 0x16zz have been allocated for classlib-type 
    // HResults. Also note that some of our HResults have to map to certain 
    // COM HR's, etc.

    // Another arbitrary decision...  Feel free to change this, as long as you
    // renumber the HResults yourself (and update rexcep.h).
    // Reflection will use 0x1600 -> 0x161f.  IO will use 0x1620 -> 0x163f.
    // Security will use 0x1640 -> 0x165f

    // There are __HResults files in the IO, Remoting, Reflection & 
    // Security/Util directories as well, so choose your HResults carefully.

    using System;
    internal static class __HResults
    {
        internal const int RO_E_CLOSED = unchecked((int)0x80000013);
        internal const int E_BOUNDS = unchecked((int)0x8000000B);
        internal const int E_CHANGED_STATE = unchecked((int)0x8000000C);
        internal const int E_FAIL = unchecked((int)0x80004005);
        internal const int E_POINTER = unchecked((int)0x80004003);
        internal const int E_NOTIMPL = unchecked((int)0x80004001);
        internal const int REGDB_E_CLASSNOTREG = unchecked((int)0x80040154);
        internal const int COR_E_AMBIGUOUSMATCH = unchecked((int)0x8000211D);
        internal const int COR_E_APPDOMAINUNLOADED = unchecked((int)0x80131014);
        internal const int COR_E_APPLICATION = unchecked((int)0x80131600);
        internal const int COR_E_ARGUMENT = unchecked((int)0x80070057);
        internal const int COR_E_ARGUMENTOUTOFRANGE = unchecked((int)0x80131502);
        internal const int COR_E_ARITHMETIC = unchecked((int)0x80070216);
        internal const int COR_E_ARRAYTYPEMISMATCH = unchecked((int)0x80131503);
        internal const int COR_E_BADIMAGEFORMAT = unchecked((int)0x8007000B);
        internal const int COR_E_TYPEUNLOADED = unchecked((int)0x80131013);
        internal const int COR_E_CANNOTUNLOADAPPDOMAIN = unchecked((int)0x80131015);
        internal const int COR_E_COMEMULATE = unchecked((int)0x80131535);
        internal const int COR_E_CONTEXTMARSHAL = unchecked((int)0x80131504);
        internal const int COR_E_DATAMISALIGNED = unchecked((int)0x80131541);
        internal const int COR_E_TIMEOUT = unchecked((int)0x80131505);
        internal const int COR_E_CUSTOMATTRIBUTEFORMAT = unchecked((int)0x80131605);
        internal const int COR_E_DIVIDEBYZERO = unchecked((int)0x80020012); // DISP_E_DIVBYZERO
        internal const int COR_E_DUPLICATEWAITOBJECT = unchecked((int)0x80131529);
        internal const int COR_E_EXCEPTION = unchecked((int)0x80131500);
        internal const int COR_E_EXECUTIONENGINE = unchecked((int)0x80131506);
        internal const int COR_E_FIELDACCESS = unchecked((int)0x80131507);
        internal const int COR_E_FORMAT = unchecked((int)0x80131537);
        internal const int COR_E_INDEXOUTOFRANGE = unchecked((int)0x80131508);
        internal const int COR_E_INSUFFICIENTMEMORY = unchecked((int)0x8013153D);
        internal const int COR_E_INSUFFICIENTEXECUTIONSTACK = unchecked((int)0x80131578);
        internal const int COR_E_INVALIDCAST = unchecked((int)0x80004002);
        internal const int COR_E_INVALIDCOMOBJECT = unchecked((int)0x80131527);
        internal const int COR_E_INVALIDFILTERCRITERIA = unchecked((int)0x80131601);
        internal const int COR_E_INVALIDOLEVARIANTTYPE = unchecked((int)0x80131531);
        internal const int COR_E_INVALIDOPERATION = unchecked((int)0x80131509);
        internal const int COR_E_INVALIDPROGRAM = unchecked((int)0x8013153A);
        internal const int COR_E_KEYNOTFOUND = unchecked((int)0x80131577);
        internal const int COR_E_MARSHALDIRECTIVE = unchecked((int)0x80131535);
        internal const int COR_E_MEMBERACCESS = unchecked((int)0x8013151A);
        internal const int COR_E_METHODACCESS = unchecked((int)0x80131510);
        internal const int COR_E_MISSINGFIELD = unchecked((int)0x80131511);
        internal const int COR_E_MISSINGMANIFESTRESOURCE = unchecked((int)0x80131532);
        internal const int COR_E_MISSINGMEMBER = unchecked((int)0x80131512);
        internal const int COR_E_MISSINGMETHOD = unchecked((int)0x80131513);
        internal const int COR_E_MISSINGSATELLITEASSEMBLY = unchecked((int)0x80131536);
        internal const int COR_E_MULTICASTNOTSUPPORTED = unchecked((int)0x80131514);
        internal const int COR_E_NOTFINITENUMBER = unchecked((int)0x80131528);
        internal const int COR_E_PLATFORMNOTSUPPORTED = unchecked((int)0x80131539);
        internal const int COR_E_NOTSUPPORTED = unchecked((int)0x80131515);
        internal const int COR_E_NULLREFERENCE = unchecked((int)0x80004003);
        internal const int COR_E_OBJECTDISPOSED = unchecked((int)0x80131622);
        internal const int COR_E_OPERATIONCANCELED = unchecked((int)0x8013153B);
        internal const int COR_E_OUTOFMEMORY = unchecked((int)0x8007000E);
        internal const int COR_E_OVERFLOW = unchecked((int)0x80131516);
        internal const int COR_E_RANK = unchecked((int)0x80131517);
        internal const int COR_E_REFLECTIONTYPELOAD = unchecked((int)0x80131602);
        internal const int COR_E_RUNTIMEWRAPPED = unchecked((int)0x8013153E);
        internal const int COR_E_SAFEARRAYRANKMISMATCH = unchecked((int)0x80131538);
        internal const int COR_E_SAFEARRAYTYPEMISMATCH = unchecked((int)0x80131533);
        internal const int COR_E_SAFEHANDLEMISSINGATTRIBUTE = unchecked((int)0x80131623);
        internal const int COR_E_SECURITY = unchecked((int)0x8013150A);
        internal const int COR_E_SERIALIZATION = unchecked((int)0x8013150C);
        internal const int COR_E_SEMAPHOREFULL = unchecked((int)0x8013152B);
        internal const int COR_E_WAITHANDLECANNOTBEOPENED = unchecked((int)0x8013152C);
        internal const int COR_E_ABANDONEDMUTEX = unchecked((int)0x8013152D);
        internal const int COR_E_STACKOVERFLOW = unchecked((int)0x800703E9);
        internal const int COR_E_SYNCHRONIZATIONLOCK = unchecked((int)0x80131518);
        internal const int COR_E_SYSTEM = unchecked((int)0x80131501);
        internal const int COR_E_TARGET = unchecked((int)0x80131603);
        internal const int COR_E_TARGETINVOCATION = unchecked((int)0x80131604);
        internal const int COR_E_TARGETPARAMCOUNT = unchecked((int)0x8002000e);
        internal const int COR_E_THREADABORTED = unchecked((int)0x80131530);
        internal const int COR_E_THREADINTERRUPTED = unchecked((int)0x80131519);
        internal const int COR_E_THREADSTATE = unchecked((int)0x80131520);
        internal const int COR_E_THREADSTOP = unchecked((int)0x80131521);
        internal const int COR_E_THREADSTART = unchecked((int)0x80131525);
        internal const int COR_E_TYPEACCESS = unchecked((int)0x80131543);
        internal const int COR_E_TYPEINITIALIZATION = unchecked((int)0x80131534);
        internal const int COR_E_TYPELOAD = unchecked((int)0x80131522);
        internal const int COR_E_ENTRYPOINTNOTFOUND = unchecked((int)0x80131523);
        internal const int COR_E_DLLNOTFOUND = unchecked((int)0x80131524);
        internal const int COR_E_UNAUTHORIZEDACCESS = unchecked((int)0x80070005);
        internal const int COR_E_UNSUPPORTEDFORMAT = unchecked((int)0x80131523);
        internal const int COR_E_VERIFICATION = unchecked((int)0x8013150D);
        internal const int COR_E_HOSTPROTECTION = unchecked((int)0x80131640);
        internal const int CORSEC_E_MIN_GRANT_FAIL = unchecked((int)0x80131417);
        internal const int CORSEC_E_NO_EXEC_PERM = unchecked((int)0x80131418);
        internal const int CORSEC_E_POLICY_EXCEPTION = unchecked((int)0x80131416);
        internal const int CORSEC_E_XMLSYNTAX = unchecked((int)0x80131418);
        internal const int NTE_FAIL = unchecked((int)0x80090020);
        internal const int CORSEC_E_CRYPTO = unchecked((int)0x80131430);
        internal const int CORSEC_E_CRYPTO_UNEX_OPER = unchecked((int)0x80131431);
        internal const int DISP_E_OVERFLOW = unchecked((int)0x8002000a);
        internal const int FUSION_E_REF_DEF_MISMATCH = unchecked((int)0x80131040);
        internal const int FUSION_E_INVALID_NAME = unchecked((int)0x80131047);
        internal const int TYPE_E_TYPEMISMATCH = unchecked((int)0x80028ca0);
    }
}

