// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

namespace System.Runtime.Intrinsics.Arm
{
    /// <summary>
    /// This class provides access to the ARM SHA256 hardware instructions via intrinsics
    /// </summary>
    [Intrinsic]
    [CLSCompliant(false)]
    public abstract class Sha256 : ArmBase
    {
        internal Sha256() { }

        public static new bool IsSupported { get => IsSupported; }

        /// <summary>
        /// uint32x4_t vsha256hq_u32 (uint32x4_t hash_abcd, uint32x4_t hash_efgh, uint32x4_t wk)
        ///   A32: SHA256H.32 Qd, Qn, Qm
        ///   A64: SHA256H Qd, Qn, Vm.4S
        /// </summary>
        public static Vector128<uint> HashUpdate1(Vector128<uint> hash_abcd, Vector128<uint> hash_efgh, Vector128<uint> wk) => HashUpdate1(hash_abcd, hash_efgh, wk);

        /// <summary>
        /// uint32x4_t vsha256h2q_u32 (uint32x4_t hash_efgh, uint32x4_t hash_abcd, uint32x4_t wk)
        ///   A32: SHA256H2.32 Qd, Qn, Qm
        ///   A64: SHA256H2 Qd, Qn, Vm.4S
        /// </summary>
        public static Vector128<uint> HashUpdate2(Vector128<uint> hash_efgh, Vector128<uint> hash_abcd, Vector128<uint> wk) => HashUpdate2(hash_efgh, hash_abcd, wk);

        /// <summary>
        /// uint32x4_t vsha256su0q_u32 (uint32x4_t w0_3, uint32x4_t w4_7)
        ///   A32: SHA256SU0.32 Qd, Qm
        ///   A64: SHA256SU0 Vd.4S, Vn.4S
        /// </summary>
        public static Vector128<uint> ScheduleUpdate0(Vector128<uint> w0_3, Vector128<uint> w4_7) => ScheduleUpdate0(w0_3, w4_7);

        /// <summary>
        /// uint32x4_t vsha256su1q_u32 (uint32x4_t w0_3, uint32x4_t w8_11, uint32x4_t w12_15)
        ///   A32: SHA256SU1.32 Qd, Qn, Qm
        ///   A64: SHA256SU1 Vd.4S, Vn.4S, Vm.4S
        /// </summary>
        public static Vector128<uint> ScheduleUpdate1(Vector128<uint> w0_3, Vector128<uint> w8_11, Vector128<uint> w12_15) => ScheduleUpdate1(w0_3, w8_11, w12_15);
    }
}
