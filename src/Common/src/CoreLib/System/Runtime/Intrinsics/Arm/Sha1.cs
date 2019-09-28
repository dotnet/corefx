// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

namespace System.Runtime.Intrinsics.Arm
{
    /// <summary>
    /// This class provides access to the ARM SHA1 hardware instructions via intrinsics
    /// </summary>
    [Intrinsic]
    [CLSCompliant(false)]
    public abstract class Sha1 : ArmBase
    {
        internal Sha1() { }

        public static new bool IsSupported { get => IsSupported; }

        /// <summary>
        /// uint32_t vsha1h_u32 (uint32_t hash_e)
        ///   A32: SHA1H.32 Qd, Qm
        ///   A64: SHA1H Sd, Sn
        /// </summary>
        public static uint FixedRotate(uint hash_e) => FixedRotate(hash_e);

        /// <summary>
        /// uint32x4_t vsha1cq_u32 (uint32x4_t hash_abcd, uint32_t hash_e, uint32x4_t wk)
        ///   A32: SHA1C.32 Qd, Qn, Qm
        ///   A64: SHA1C Qd, Sn, Vm.4S
        /// </summary>
        public static Vector128<uint> HashUpdateChoose(Vector128<uint> hash_abcd, uint hash_e, Vector128<uint> wk) => HashUpdateChoose(hash_abcd, hash_e, wk);

        /// <summary>
        /// uint32x4_t vsha1mq_u32 (uint32x4_t hash_abcd, uint32_t hash_e, uint32x4_t wk)
        ///   A32: SHA1M.32 Qd, Qn, Qm
        ///   A64: SHA1M Qd, Sn, Vm.4S
        /// </summary>
        public static Vector128<uint> HashUpdateMajority(Vector128<uint> hash_abcd, uint hash_e, Vector128<uint> wk) => HashUpdateMajority(hash_abcd, hash_e, wk);

        /// <summary>
        /// uint32x4_t vsha1pq_u32 (uint32x4_t hash_abcd, uint32_t hash_e, uint32x4_t wk)
        ///   A32: SHA1P.32 Qd, Qn, Qm
        ///   A64: SHA1P Qd, Sn, Vm.4S
        /// </summary>
        public static Vector128<uint> HashUpdateParity(Vector128<uint> hash_abcd, uint hash_e, Vector128<uint> wk) => HashUpdateParity(hash_abcd, hash_e, wk);

        /// <summary>
        /// uint32x4_t vsha1su0q_u32 (uint32x4_t w0_3, uint32x4_t w4_7, uint32x4_t w8_11)
        ///   A32: SHA1SU0.32 Qd, Qn, Qm
        ///   A64: SHA1SU0 Vd.4S, Vn.4S, Vm.4S
        /// </summary>
        public static Vector128<uint> ScheduleUpdate0(Vector128<uint> w0_3, Vector128<uint> w4_7, Vector128<uint> w8_11) => ScheduleUpdate0(w0_3, w4_7, w8_11);

        /// <summary>
        /// uint32x4_t vsha1su1q_u32 (uint32x4_t tw0_3, uint32x4_t w12_15)
        ///   A32: SHA1SU1.32 Qd, Qm
        ///   A64: SHA1SU1 Vd.4S, Vn.4S
        /// </summary>
        public static Vector128<uint> ScheduleUpdate1(Vector128<uint> tw0_3, Vector128<uint> w12_15) => ScheduleUpdate1(tw0_3, w12_15);
    }
}
