﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.Security.Cryptography
{
    /// <summary>
    /// An elliptic curve.
    /// </summary>
    /// <remarks>
    /// The CurveType property determines whether the curve is a named curve or an explicit curve 
    /// which is either a prime curve or a characteristic-2 curve.
    /// </remarks>
    [DebuggerDisplay("ECCurve: {Oid}")]
    public partial struct ECCurve
    {
        /// <summary>
        /// Coefficient A. Applies only to Explicit curves.
        /// </summary>
        public byte[] A;

        /// <summary>
        /// Coefficient B. Applies only to Explicit curves.
        /// </summary>
        public byte[] B;

        /// <summary>
        /// Base Point. Applies only to Explicit curves.
        /// </summary>
        public ECPoint G;

        /// <summary>
        /// Order of the group generated by G = (x,y). Applies only to Explicit curves.
        /// </summary>
        public byte[] Order;

        /// <summary>
        /// Cofactor (optional). Applies only to Explicit curves.
        /// </summary>
        public byte[] Cofactor;

        /// <summary>
        /// Seed of the curve (optional). Applies only to Explicit curves.
        /// </summary>
        public byte[] Seed;

        /// <summary>
        /// Curve Type.
        /// </summary>
        public ECCurveType CurveType;

        /// <summary>
        /// The hash algorithm used to generate A and B from the Seed. Applies only to Explicit curves.
        /// </summary>
        public HashAlgorithmName? Hash;

        /// <summary>
        /// The binary polynomial. Applies only to Characteristic2 curves.
        /// </summary>
        public byte[] Polynomial;

        /// <summary>
        /// The prime specifying the base field. Applies only to Prime curves.
        /// </summary>
        public byte[] Prime;

        private Oid _oid;
        /// <summary>
        /// The Oid representing the named curve. Applies only to Named curves.
        /// </summary>
        /// <remarks>A clone is returned, not the current instance.</remarks>
        public Oid Oid
        {
            get
            {
                // Ensure _oid remains immutable
                return new Oid(_oid.Value, _oid.FriendlyName);
            }
            private set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(Oid));

                if (string.IsNullOrEmpty(value.Value) && string.IsNullOrEmpty(value.FriendlyName))
                    throw new ArgumentException(SR.Cryptography_InvalidCurveOid);

                _oid = value;
            }
        }

        /// <summary>
        /// Create a curve without having to make a copy of the Oid
        /// </summary>
        private static ECCurve Create(Oid oid)
        {
            ECCurve curve = new ECCurve();
            curve.CurveType = ECCurveType.Named;
            curve.Oid = oid;
            return curve;
        }

        /// <summary>
        /// Create a curve from the given cref="Oid".
        /// </summary>
        /// <param name="curveOid">The Oid to use.</param>
        /// <returns>An ECCurve representing a named curve.</returns>
        public static ECCurve CreateFromOid(Oid curveOid)
        {
            // Make a copy since Oid is mutable
            return Create(new Oid(curveOid.Value, curveOid.FriendlyName));
        }

        /// <summary>
        /// Create a curve from the given cref="Oid" friendly name.
        /// </summary>
        /// <param name="oidFriendlyName">The Oid friendly name to use.</param>
        /// <returns>An ECCurve representing a named curve.</returns>
        public static ECCurve CreateFromFriendlyName(string oidFriendlyName)
        {
            if (oidFriendlyName == null)
            {
                throw new ArgumentNullException(nameof(oidFriendlyName));
            }
            return ECCurve.CreateFromValueAndName(null, oidFriendlyName);
        }

        /// <summary>
        /// Create a curve from the given cref="Oid" value.
        /// </summary>
        /// <param name="oidValue">The Oid value to use.</param>
        /// <returns>An ECCurve representing a named curve.</returns>
        public static ECCurve CreateFromValue(string oidValue)
        {
            if (oidValue == null)
            {
                throw new ArgumentNullException(nameof(oidValue));
            }
            return ECCurve.CreateFromValueAndName(oidValue, null);
        }

        private static ECCurve CreateFromValueAndName(string oidValue, string oidFriendlyName)
        {
            Oid oid = null;

            if (oidValue == null && oidFriendlyName != null)
            {
                try
                {
                    oid = Oid.FromFriendlyName(oidFriendlyName, OidGroup.PublicKeyAlgorithm);
                }
                catch (CryptographicException)
                {
                }
            }

            oid ??= new Oid(oidValue, oidFriendlyName);
            return ECCurve.Create(oid);
        }

        public bool IsPrime
        {
            get
            {
                return CurveType == ECCurve.ECCurveType.PrimeShortWeierstrass ||
                    CurveType == ECCurve.ECCurveType.PrimeMontgomery ||
                    CurveType == ECCurve.ECCurveType.PrimeTwistedEdwards;
            }
        }

        public bool IsCharacteristic2
        {
            get
            {
                return CurveType == ECCurve.ECCurveType.Characteristic2;
            }
        }

        public bool IsExplicit
        {
            get
            {
                return IsPrime || IsCharacteristic2;
            }
        }

        public bool IsNamed
        {
            get
            {
                return CurveType == ECCurve.ECCurveType.Named;
            }
        }

        /// <summary>
        /// Validate the current curve.
        /// </summary>
        /// <exception cref="CryptographicException">
        ///     if the curve parameters are not valid for the current CurveType.
        /// </exception>
        public void Validate()
        {
            if (IsNamed)
            {
                if (HasAnyExplicitParameters())
                {
                    throw new CryptographicException(SR.Cryptography_InvalidECNamedCurve);
                }

                if (Oid == null ||
                    (string.IsNullOrEmpty(Oid.FriendlyName) && string.IsNullOrEmpty(Oid.Value)))
                {
                    throw new CryptographicException(SR.Cryptography_InvalidCurveOid);
                }
            }
            else if (IsExplicit)
            {
                bool hasErrors = false;

                if (A == null ||
                    B == null || B.Length != A.Length ||
                    G.X == null || G.X.Length != A.Length ||
                    G.Y == null || G.Y.Length != A.Length ||
                    Order == null || Order.Length == 0 ||
                    Cofactor == null || Cofactor.Length == 0)
                {
                    hasErrors = true;
                }

                if (IsPrime)
                {
                    if (!hasErrors)
                    {
                        if (Prime == null || Prime.Length != A.Length)
                        {
                            hasErrors = true;
                        }
                    }

                    if (hasErrors)
                        throw new CryptographicException(SR.Cryptography_InvalidECPrimeCurve);
                }
                else if (IsCharacteristic2)
                {
                    if (!hasErrors)
                    {
                        if (Polynomial == null || Polynomial.Length == 0)
                        {
                            hasErrors = true;
                        }
                    }

                    if (hasErrors)
                        throw new CryptographicException(SR.Cryptography_InvalidECCharacteristic2Curve);
                }
            }
            else
            {
                // Implicit; if there are any values, throw
                Debug.Assert(CurveType == ECCurveType.Implicit);
                if (HasAnyExplicitParameters() || Oid != null)
                {
                    throw new CryptographicException(SR.Format(SR.Cryptography_CurveNotSupported, CurveType.ToString()));
                }
            }
        }

        private bool HasAnyExplicitParameters()
        {
            return (A != null ||
                B != null ||
                G.X != null ||
                G.Y != null ||
                Order != null ||
                Cofactor != null ||
                Prime != null ||
                Polynomial != null ||
                Seed != null ||
                Hash != null);
        }
    }
}
