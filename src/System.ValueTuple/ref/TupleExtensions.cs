﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------

using System.ComponentModel;

namespace System
{
    public static class TupleExtensions
    {
        #region Deconstruct
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Deconstruct<T1>(
            this Tuple<T1> value,
            out T1 item1)
        {
            throw null;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Deconstruct<T1, T2>(
            this Tuple<T1, T2> value,
            out T1 item1, out T2 item2)
        {
            throw null;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Deconstruct<T1, T2, T3>(
            this Tuple<T1, T2, T3> value,
            out T1 item1, out T2 item2, out T3 item3)
        {
            throw null;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Deconstruct<T1, T2, T3, T4>(
            this Tuple<T1, T2, T3, T4> value,
            out T1 item1, out T2 item2, out T3 item3, out T4 item4)
        {
            throw null;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Deconstruct<T1, T2, T3, T4, T5>(
            this Tuple<T1, T2, T3, T4, T5> value,
            out T1 item1, out T2 item2, out T3 item3, out T4 item4, out T5 item5)
        {
            throw null;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Deconstruct<T1, T2, T3, T4, T5, T6>(
            this Tuple<T1, T2, T3, T4, T5, T6> value,
            out T1 item1, out T2 item2, out T3 item3, out T4 item4, out T5 item5, out T6 item6)
        {
            throw null;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Deconstruct<T1, T2, T3, T4, T5, T6, T7>(
            this Tuple<T1, T2, T3, T4, T5, T6, T7> value,
            out T1 item1, out T2 item2, out T3 item3, out T4 item4, out T5 item5, out T6 item6, out T7 item7)
        {
            throw null;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Deconstruct<T1, T2, T3, T4, T5, T6, T7, T8>(
            this Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>> value,
            out T1 item1, out T2 item2, out T3 item3, out T4 item4, out T5 item5, out T6 item6, out T7 item7, out T8 item8)
        {
            throw null;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Deconstruct<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
            this Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9>> value,
            out T1 item1, out T2 item2, out T3 item3, out T4 item4, out T5 item5, out T6 item6, out T7 item7, out T8 item8, out T9 item9)
        {
            throw null;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Deconstruct<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
            this Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10>> value,
            out T1 item1, out T2 item2, out T3 item3, out T4 item4, out T5 item5, out T6 item6, out T7 item7, out T8 item8, out T9 item9, out T10 item10)
        {
            throw null;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Deconstruct<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
            this Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11>> value,
            out T1 item1, out T2 item2, out T3 item3, out T4 item4, out T5 item5, out T6 item6, out T7 item7, out T8 item8, out T9 item9, out T10 item10, out T11 item11)
        {
            throw null;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Deconstruct<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
            this Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12>> value,
            out T1 item1, out T2 item2, out T3 item3, out T4 item4, out T5 item5, out T6 item6, out T7 item7, out T8 item8, out T9 item9, out T10 item10, out T11 item11, out T12 item12)
        {
            throw null;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Deconstruct<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
            this Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13>> value,
            out T1 item1, out T2 item2, out T3 item3, out T4 item4, out T5 item5, out T6 item6, out T7 item7, out T8 item8, out T9 item9, out T10 item10, out T11 item11, out T12 item12, out T13 item13)
        {
            throw null;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Deconstruct<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
            this Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14>> value,
            out T1 item1, out T2 item2, out T3 item3, out T4 item4, out T5 item5, out T6 item6, out T7 item7, out T8 item8, out T9 item9, out T10 item10, out T11 item11, out T12 item12, out T13 item13, out T14 item14)
        {
            throw null;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Deconstruct<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
            this Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15>>> value,
            out T1 item1, out T2 item2, out T3 item3, out T4 item4, out T5 item5, out T6 item6, out T7 item7, out T8 item8, out T9 item9, out T10 item10, out T11 item11, out T12 item12, out T13 item13, out T14 item14, out T15 item15)
        {
            throw null;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Deconstruct<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(
            this Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16>>> value,
            out T1 item1, out T2 item2, out T3 item3, out T4 item4, out T5 item5, out T6 item6, out T7 item7, out T8 item8, out T9 item9, out T10 item10, out T11 item11, out T12 item12, out T13 item13, out T14 item14, out T15 item15, out T16 item16)
        {
            throw null;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Deconstruct<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>(
            this Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17>>> value,
            out T1 item1, out T2 item2, out T3 item3, out T4 item4, out T5 item5, out T6 item6, out T7 item7, out T8 item8, out T9 item9, out T10 item10, out T11 item11, out T12 item12, out T13 item13, out T14 item14, out T15 item15, out T16 item16, out T17 item17)
        {
            throw null;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Deconstruct<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18>(
            this Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18>>> value,
            out T1 item1, out T2 item2, out T3 item3, out T4 item4, out T5 item5, out T6 item6, out T7 item7, out T8 item8, out T9 item9, out T10 item10, out T11 item11, out T12 item12, out T13 item13, out T14 item14, out T15 item15, out T16 item16, out T17 item17, out T18 item18)
        {
            throw null;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Deconstruct<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>(
            this Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19>>> value,
            out T1 item1, out T2 item2, out T3 item3, out T4 item4, out T5 item5, out T6 item6, out T7 item7, out T8 item8, out T9 item9, out T10 item10, out T11 item11, out T12 item12, out T13 item13, out T14 item14, out T15 item15, out T16 item16, out T17 item17, out T18 item18, out T19 item19)
        {
            throw null;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Deconstruct<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20>(
            this Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20>>> value,
            out T1 item1, out T2 item2, out T3 item3, out T4 item4, out T5 item5, out T6 item6, out T7 item7, out T8 item8, out T9 item9, out T10 item10, out T11 item11, out T12 item12, out T13 item13, out T14 item14, out T15 item15, out T16 item16, out T17 item17, out T18 item18, out T19 item19, out T20 item20)
        {
            throw null;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Deconstruct<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21>(
            this Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21>>> value,
            out T1 item1, out T2 item2, out T3 item3, out T4 item4, out T5 item5, out T6 item6, out T7 item7, out T8 item8, out T9 item9, out T10 item10, out T11 item11, out T12 item12, out T13 item13, out T14 item14, out T15 item15, out T16 item16, out T17 item17, out T18 item18, out T19 item19, out T20 item20, out T21 item21)
        {
            throw null;
        }
        #endregion

        #region ToValueTuple
        public static ValueTuple<T1>
            ToValueTuple<T1>(
                this Tuple<T1> value)
        {
            throw null;
        }

        public static ValueTuple<T1, T2>
            ToValueTuple<T1, T2>(
                this Tuple<T1, T2> value)
        {
            throw null;
        }

        public static ValueTuple<T1, T2, T3>
            ToValueTuple<T1, T2, T3>(
                this Tuple<T1, T2, T3> value)
        {
            throw null;
        }

        public static ValueTuple<T1, T2, T3, T4>
            ToValueTuple<T1, T2, T3, T4>(
                this Tuple<T1, T2, T3, T4> value)
        {
            throw null;
        }

        public static ValueTuple<T1, T2, T3, T4, T5>
            ToValueTuple<T1, T2, T3, T4, T5>(
                this Tuple<T1, T2, T3, T4, T5> value)
        {
            throw null;
        }

        public static ValueTuple<T1, T2, T3, T4, T5, T6>
            ToValueTuple<T1, T2, T3, T4, T5, T6>(
                this Tuple<T1, T2, T3, T4, T5, T6> value)
        {
            throw null;
        }

        public static ValueTuple<T1, T2, T3, T4, T5, T6, T7>
            ToValueTuple<T1, T2, T3, T4, T5, T6, T7>(
                this Tuple<T1, T2, T3, T4, T5, T6, T7> value)
        {
            throw null;
        }

        public static ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8>>
            ToValueTuple<T1, T2, T3, T4, T5, T6, T7, T8>(
                this Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>> value)
        {
            throw null;
        }

        public static ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9>>
            ToValueTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
                this Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9>> value)
        {
            throw null;
        }

        public static ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10>>
            ToValueTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
                this Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10>> value)
        {
            throw null;
        }

        public static ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11>>
            ToValueTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
                this Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11>> value)
        {
            throw null;
        }

        public static ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12>>
            ToValueTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
                this Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12>> value)
        {
            throw null;
        }

        public static ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13>>
            ToValueTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
                this Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13>> value)
        {
            throw null;
        }

        public static ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14>>
            ToValueTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
                this Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14>> value)
        {
            throw null;
        }

        public static ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14, ValueTuple<T15>>>
            ToValueTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
                this Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15>>> value)
        {
            throw null;
        }

        public static ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14, ValueTuple<T15, T16>>>
            ToValueTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(
                this Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16>>> value)
        {
            throw null;
        }

        public static ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14, ValueTuple<T15, T16, T17>>>
            ToValueTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>(
                this Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17>>> value)
        {
            throw null;
        }

        public static ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14, ValueTuple<T15, T16, T17, T18>>>
            ToValueTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18>(
                this Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18>>> value)
        {
            throw null;
        }

        public static ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14, ValueTuple<T15, T16, T17, T18, T19>>>
            ToValueTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>(
                this Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19>>> value)
        {
            throw null;
        }

        public static ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14, ValueTuple<T15, T16, T17, T18, T19, T20>>>
            ToValueTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20>(
                this Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20>>> value)
        {
            throw null;
        }

        public static ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14, ValueTuple<T15, T16, T17, T18, T19, T20, T21>>>
            ToValueTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21>(
                this Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21>>> value)
        {
            throw null;
        }
        #endregion

        #region ToTuple
        public static Tuple<T1>
            ToTuple<T1>(
                this ValueTuple<T1> value)
        {
            throw null;
        }

        public static Tuple<T1, T2>
            ToTuple<T1, T2>(
                this ValueTuple<T1, T2> value)
        {
            throw null;
        }

        public static Tuple<T1, T2, T3>
            ToTuple<T1, T2, T3>(
                this ValueTuple<T1, T2, T3> value)
        {
            throw null;
        }

        public static Tuple<T1, T2, T3, T4>
            ToTuple<T1, T2, T3, T4>(
                this ValueTuple<T1, T2, T3, T4> value)
        {
            throw null;
        }

        public static Tuple<T1, T2, T3, T4, T5>
            ToTuple<T1, T2, T3, T4, T5>(
                this ValueTuple<T1, T2, T3, T4, T5> value)
        {
            throw null;
        }

        public static Tuple<T1, T2, T3, T4, T5, T6>
            ToTuple<T1, T2, T3, T4, T5, T6>(
                this ValueTuple<T1, T2, T3, T4, T5, T6> value)
        {
            throw null;
        }

        public static Tuple<T1, T2, T3, T4, T5, T6, T7>
            ToTuple<T1, T2, T3, T4, T5, T6, T7>(
                this ValueTuple<T1, T2, T3, T4, T5, T6, T7> value)
        {
            throw null;
        }

        public static Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>>
            ToTuple<T1, T2, T3, T4, T5, T6, T7, T8>(
                this ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8>> value)
        {
            throw null;
        }

        public static Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9>>
            ToTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
                this ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9>> value)
        {
            throw null;
        }

        public static Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10>>
            ToTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
                this ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10>> value)
        {
            throw null;
        }

        public static Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11>>
            ToTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
                this ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11>> value)
        {
            throw null;
        }

        public static Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12>>
            ToTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
                this ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12>> value)
        {
            throw null;
        }

        public static Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13>>
            ToTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
                this ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13>> value)
        {
            throw null;
        }

        public static Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14>>
            ToTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
                this ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14>> value)
        {
            throw null;
        }

        public static Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15>>>
            ToTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
                this ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14, ValueTuple<T15>>> value)
        {
            throw null;
        }

        public static Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16>>>
            ToTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(
                this ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14, ValueTuple<T15, T16>>> value)
        {
            throw null;
        }

        public static Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17>>>
            ToTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>(
                this ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14, ValueTuple<T15, T16, T17>>> value)
        {
            throw null;
        }

        public static Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18>>>
            ToTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18>(
                this ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14, ValueTuple<T15, T16, T17, T18>>> value)
        {
            throw null;
        }

        public static Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19>>>
            ToTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>(
                this ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14, ValueTuple<T15, T16, T17, T18, T19>>> value)
        {
            throw null;
        }

        public static Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20>>>
            ToTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20>(
                this ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14, ValueTuple<T15, T16, T17, T18, T19, T20>>> value)
        {
            throw null;
        }

        public static Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21>>>
            ToTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21>(
                this ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14, ValueTuple<T15, T16, T17, T18, T19, T20, T21>>> value)
        {
            throw null;
        }
        #endregion

        private static ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> CreateLong<T1, T2, T3, T4, T5, T6, T7, TRest>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, TRest rest) where TRest : struct =>
            new ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>(item1, item2, item3, item4, item5, item6, item7, rest);

        private static Tuple<T1, T2, T3, T4, T5, T6, T7, TRest> CreateLongRef<T1, T2, T3, T4, T5, T6, T7, TRest>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, TRest rest) =>
            new Tuple<T1, T2, T3, T4, T5, T6, T7, TRest>(item1, item2, item3, item4, item5, item6, item7, rest);
    }
}