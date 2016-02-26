// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Security.Cryptography.EcDsa.Tests
{
    public class CurveDef
    {
        public CurveDef() { }
        public ECCurve Curve;
        public ECCurve.ECCurveType CurveType;
        public int KeySize;
        public bool IncludePrivate;
        public bool RequiredOnPlatform;

        public bool IsCurveValidOnPlatform
        {
            get
            {
                // Assume curve is valid if required; tests will fail if not present
                return RequiredOnPlatform || ECDsaFactory.IsCurveValid(Curve.Oid);
            }
        }

        public bool IsCurveTypeEqual(ECCurve.ECCurveType actual)
        {
            if (CurveType == actual)
                return true;

            // Montgomery curves can be expressed as Weierstrass, and some curves can be either depending on the 
            // platform so use whichever the platform says it is for the given curve
            if (CurveType == ECCurve.ECCurveType.PrimeMontgomery && actual == ECCurve.ECCurveType.PrimeShortWeierstrass)
                return true;

            return false;
        }
    }
}
