// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;

namespace System.Runtime.Serialization.Formatters.Tests
{
    public static class EqualityExtensions
    {
        public static bool IsEqual(this WeakReference @this, WeakReference other)
        {
            if (@this == null || other == null)
                return false;

            if (@this.TrackResurrection != other.TrackResurrection)
                return false;

            // When WeakReference is deserialized, the object it wraps may blip into and out of
            // existence before we get a chance to compare it, since there are no strong references
            // to it such that it can then be immediately collected.  Therefore, if we can get both
            // values, great, compare them.  Otherwise, consider them equal.
            object a = @this.Target;
            object b = other.Target;
            return a != null && b != null ? Equals(a, b) : true;
        }

        public static bool IsEqual<T>(this WeakReference<T> @this, WeakReference<T> other)
            where T : class
        {
            if (@this == null || other == null)
                return false;

            // When WeakReference is deserialized, the object it wraps may blip into and out of
            // existence before we get a chance to compare it, since there are no strong references
            // to it such that it can then be immediately collected.  Therefore, if we can get both
            // values, great, compare them.  Otherwise, consider them equal.
            return @this.TryGetTarget(out T thisTarget) && other.TryGetTarget(out T otherTarget) ?
                Equals(thisTarget, otherTarget) :
                true;
        }

        public static bool IsEqual<T>(this Lazy<T> @this, Lazy<T> other)
        {
            // Force value creation for lazy original object
            T thisVal = @this.Value;
            T otherVal = other.Value;

            return @this != null &&
                other != null &&
                @this.IsValueCreated == other.IsValueCreated &&
                Object.Equals(thisVal, otherVal);
        }

        public static bool IsEqual(this StreamingContext @this, StreamingContext other)
        {
            return @this.State == @other.State &&
                Object.Equals(@this.Context, other.Context);
        }

        public static bool IsEqual(this CookieContainer @this, CookieContainer other)
        {
            return @this != null &&
                other != null &&
                @this.Capacity == other.Capacity &&
                @this.Count == other.Count &&
                @this.MaxCookieSize == other.MaxCookieSize &&
                @this.PerDomainCapacity == other.PerDomainCapacity;
        }

        public static bool IsEqual(this DataSet @this, DataSet other)
        {
            return @this != null &&
                @other != null &&
                @this.DataSetName == other.DataSetName &&
                @this.Namespace == other.Namespace &&
                @this.Prefix == other.Prefix &&
                @this.CaseSensitive == other.CaseSensitive &&
                @this.Locale.LCID == other.Locale.LCID &&
                @this.EnforceConstraints == other.EnforceConstraints &&
                @this.ExtendedProperties?.Count == other.ExtendedProperties?.Count;
        }

        public static bool IsEqual(this DataTable @this, DataTable other)
        {
            return @this != null &&
                other != null &&
                @this.TableName == other.TableName &&
                @this.Namespace == other.Namespace &&
                @this.Prefix == other.Prefix &&
                @this.CaseSensitive == other.CaseSensitive &&
                @this.Locale.LCID == other.Locale.LCID &&
                @this.MinimumCapacity == other.MinimumCapacity;
        }

        public static bool IsEqual(this Comparer @this, Comparer other)
        {
            if(@this == null || other == null)
            {
                return false;
            }

            // The compareInfos are internal and get reflection blocked on .NET Native, so use
            // GetObjectData to get them
            SerializationInfo thisInfo = new SerializationInfo(typeof(Comparer), new FormatterConverter());
            @this.GetObjectData(thisInfo, new StreamingContext());
            CompareInfo thisCompareInfo = (CompareInfo)thisInfo.GetValue("CompareInfo", typeof(CompareInfo));

            SerializationInfo otherInfo = new SerializationInfo(typeof(Comparer), new FormatterConverter());
            other.GetObjectData(otherInfo, new StreamingContext());
            CompareInfo otherCompareInfo = (CompareInfo)otherInfo.GetValue("CompareInfo", typeof(CompareInfo));
            
            return Object.Equals(thisCompareInfo, otherCompareInfo);
        }


        public static bool IsEqual(this DictionaryEntry @this, DictionaryEntry other)
        {
            return Object.Equals(@this.Key, other.Key) && Object.Equals(@this.Value, other.Value);
        }

        public static bool IsEqual(this StringDictionary @this, StringDictionary other)
        {
            return @this != null &&
                other != null &&
                @this.Count == other.Count;
        }

        public static bool IsEqual(this ArrayList @this, ArrayList other)
        {
            if (!(@this != null &&
                other != null &&
                @this.Capacity == other.Capacity &&
                @this.Count == other.Count &&
                @this.IsFixedSize == other.IsFixedSize &&
                @this.IsReadOnly == other.IsReadOnly &&
                @this.IsSynchronized == other.IsSynchronized))
                return false;

            for (int i = 0; i < @this.Count; i++)
            {
                if (!BinaryFormatterTests.CheckEquals(@this[i], other[i]))
                    return false;
            }

            return true;
        }

        //public static bool IsEqual(this BitArray @this, BitArray other)
        //{
        //    if (!(@this != null &&
        //        other != null &&
        //        @this.Length == other.Length &&
        //        @this.Count == other.Count &&
        //        @this.SyncRoot == other.SyncRoot &&
        //        @this.IsReadOnly == other.IsReadOnly &&
        //        @this.IsSynchronized == other.IsSynchronized))
        //        return false;
        //    throw new NotImplementedException("finish me");
        //    for (int i = 0; i < @this.Count; i++)
        //    {
        //        if (@this[i] != other[i])
        //            return false;
        //    }

        //    return true;
        //}

        //public static bool IsEqual(this Dictionary<int, string> @this, Dictionary<int, string> other)
        //{
        //    if (!(@this != null &&
        //        other != null &&
        //        @this.Comparer == other.Comparer &&
        //        @this.Count == other.Count &&
        //        @this.Keys == other.Keys &&
        //        @this.Values == other.Values))
        //        return false;
        //    throw new NotImplementedException("finish me");
        //    for (int i = 0; i < @this.Count; i++)
        //    {
        //        if (@this[i] != other[i])
        //            return false;
        //    }

        //    return true;
        //}

        //public static bool IsEqual(this HashSet<Point> @this, HashSet<Point> other)
        //{
        //    return @this != null &&
        //        other != null &&
        //        @this.Count == other.Count &&
        //        @this.Comparer == other.Comparer;
        //    throw new NotImplementedException("finish me");
        //}

        //public static bool IsEqual(this LinkedList<Point> @this, LinkedList<Point> other)
        //{
        //    return @this != null &&
        //        other != null &&
        //        @this.Count == other.Count &&
        //        @this.First == other.First &&
        //        @this.Last == other.Last;
        //    throw new NotImplementedException("finish me");
        //}

        public static bool IsEqual(this List<int> @this, List<int> other)
        {
            if (!(@this != null &&
                other != null &&
                @this.Capacity == other.Capacity &&
                @this.Count == other.Count))
                return false;

            for (int i = 0; i < @this.Count; i++)
            {
                if (@this[i] != other[i])
                    return false;
            }

            return true;
        }

        //public static bool IsEqual(this Queue<int> @this, Queue<int> other)
        //{
        //    return @this != null &&
        //        other != null &&
        //        @this.Count == other.Count;
        //    throw new NotImplementedException("finish me");
        //}

        //public static bool IsEqual(this SortedList<int, Point> @this, SortedList<int, Point> other)
        //{
        //    if (!(@this != null &&
        //        other != null &&
        //        @this.Capacity == other.Capacity &&
        //        @this.Comparer == other.Comparer &&
        //        @this.Count == other.Count &&
        //        @this.Keys == other.Keys &&
        //        @this.Values == other.Values))
        //        return false;

        //    for (int i = 0; i < @this.Count; i++)
        //    {
        //        if (@this[i] != other[i])
        //            return false;
        //    }
        //    throw new NotImplementedException("finish me");
        //    return true;
        //}

        //public static bool IsEqual(this SortedSet<Point> @this, SortedSet<Point> other)
        //{
        //    return @this != null &&
        //        other != null &&
        //        @this.Count == other.Count &&
        //        @this.Comparer == other.Comparer &&
        //        @this.Min == other.Min &&
        //        @this.Max == other.Max;
        //    throw new NotImplementedException("finish me");
        //}

        //public static bool IsEqual(this Stack<Point> @this, Stack<Point> other)
        //{
        //    return @this != null &&
        //        other != null &&
        //        @this.Count == other.Count;
        //    throw new NotImplementedException("finish me");
        //}

        //public static bool IsEqual(this Hashtable @this, Hashtable other)
        //{
        //    if (!(@this != null &&
        //        other != null &&
        //        @this.IsReadOnly == other.IsReadOnly &&
        //        @this.IsFixedSize == other.IsFixedSize &&
        //        @this.IsSynchronized == other.IsSynchronized &&
        //        @this.Keys == other.Keys &&
        //        @this.Values == other.Values &&
        //        @this.SyncRoot == other.SyncRoot &&
        //        @this.Count == other.Count))
        //        return false;
        //    throw new NotImplementedException("finish me");
        //    for (int i = 0; i < @this.Count; i++)
        //    {
        //        if (@this[i] != other[i])
        //            return false;
        //    }

        //    return true;
        //}

        public static bool IsEqual(this Collection<int> @this, Collection<int> other)
        {
            if (!(@this != null &&
                other != null &&
                @this.Count == other.Count))
                return false;

            for (int i = 0; i < @this.Count; i++)
            {
                if (@this[i] != other[i])
                    return false;
            }

            return true;
        }

        public static bool IsEqual(this ObservableCollection<int> @this, ObservableCollection<int> other)
        {
            if (!(@this != null &&
                other != null &&
                @this.Count == other.Count))
                return false;

            for (int i = 0; i < @this.Count; i++)
            {
                if (@this[i] != other[i])
                    return false;
            }

            return true;
        }

        public static bool IsEqual(this ReadOnlyCollection<int> @this, ReadOnlyCollection<int> other)
        {
            if (!(@this != null &&
                other != null &&
                @this.Count == other.Count))
                return false;

            for (int i = 0; i < @this.Count; i++)
            {
                if (@this[i] != other[i])
                    return false;
            }

            return true;
        }

        //public static bool IsEqual(this ReadOnlyDictionary<int, string> @this, ReadOnlyDictionary<int, string> other)
        //{
        //    if (!(@this != null &&
        //        other != null &&
        //        @this.Keys == other.Keys &&
        //        @this.Values == other.Values &&
        //        @this.Count == other.Count))
        //        return false;
        //    throw new NotImplementedException("finish me");
        //    for (int i = 0; i < @this.Count; i++)
        //    {
        //        if (@this[i] != other[i])
        //            return false;
        //    }

        //    return true;
        //}

        public static bool IsEqual(this ReadOnlyObservableCollection<int> @this, ReadOnlyObservableCollection<int> other)
        {
            if (!(@this != null &&
                other != null &&
                @this.Count == other.Count))
                return false;

            for (int i = 0; i < @this.Count; i++)
            {
                if (@this[i] != other[i])
                    return false;
            }

            return true;
        }

        //public static bool IsEqual(this Queue @this, Queue other)
        //{
        //    return @this != null &&
        //        other != null &&
        //        @this.Count == other.Count &&
        //        @this.IsSynchronized == other.IsSynchronized &&
        //        @this.SyncRoot == other.SyncRoot;
        //}

        //public static bool IsEqual(this SortedList @this, SortedList other)
        //{
        //    if (!(@this != null &&
        //        other != null &&
        //        @this.Capacity == other.Capacity &&
        //        @this.Count == other.Count &&
        //        @this.Keys == other.Keys &&
        //        @this.Values == other.Values &&
        //        @this.IsReadOnly == other.IsReadOnly &&
        //        @this.IsFixedSize == other.IsFixedSize &&
        //        @this.IsSynchronized == other.IsSynchronized &&
        //        @this.SyncRoot == other.SyncRoot))
        //        return false;
        //    throw new NotImplementedException("finish me");
        //    for (int i = 0; i < @this.Count; i++)
        //    {
        //        if (@this[i] != other[i])
        //            return false;
        //    }

        //    return true;
        //}

        //public static bool IsEqual(this HybridDictionary @this, HybridDictionary other)
        //{
        //    if (!(@this != null &&
        //        other != null &&
        //        @this.Count == other.Count &&
        //        @this.Keys == other.Keys &&
        //        @this.IsReadOnly == other.IsReadOnly &&
        //        @this.IsFixedSize == other.IsFixedSize &&
        //        @this.IsSynchronized == other.IsSynchronized &&
        //        @this.SyncRoot == other.SyncRoot &&
        //        @this.Values == other.Values))
        //        return false;
        //    throw new NotImplementedException("finish me");
        //    for (int i = 0; i < @this.Count; i++)
        //    {
        //        if (@this[i] != other[i])
        //            return false;
        //    }

        //    return true;
        //}

        //public static bool IsEqual(this ListDictionary @this, ListDictionary other)
        //{
        //    if (!(@this != null &&
        //        other != null &&
        //        @this.Count == other.Count &&
        //        @this.Keys == other.Keys &&
        //        @this.IsReadOnly == other.IsReadOnly &&
        //        @this.IsFixedSize == other.IsFixedSize &&
        //        @this.IsSynchronized == other.IsSynchronized &&
        //        @this.SyncRoot == other.SyncRoot &&
        //        @this.Values == other.Values))
        //        return false;
        //    throw new NotImplementedException("finish me");
        //    for (int i = 0; i < @this.Count; i++)
        //    {
        //        if (@this[i] != other[i])
        //            return false;
        //    }

        //    return true;
        //}

        //public static bool IsEqual(this NameValueCollection @this, NameValueCollection other)
        //{
        //    if (!(@this != null &&
        //        other != null &&
        //        @this.AllKeys == other.AllKeys &&
        //        @this.Count == other.Count &&
        //        @this.Keys == other.Keys))
        //        return false;
        //    throw new NotImplementedException("finish me");
        //    for (int i = 0; i < @this.Count; i++)
        //    {
        //        if (@this[i] != other[i])
        //            return false;
        //    }

        //    return true;
        //}

        //public static bool IsEqual(this OrderedDictionary @this, OrderedDictionary other)
        //{
        //    if (!(@this != null &&
        //        other != null &&
        //        @this.Count == other.Count &&
        //        @this.IsReadOnly == other.IsReadOnly &&
        //        @this.Keys == other.Keys &&
        //        @this.Values == other.Values))
        //        return false;
        //    throw new NotImplementedException("finish me");
        //    for (int i = 0; i < @this.Count; i++)
        //    {
        //        if (@this[i] != other[i])
        //            return false;
        //    }

        //    return true;
        //}

        public static bool IsEqual(this StringCollection @this, StringCollection other)
        {
            if (!(@this != null &&
                other != null &&
                @this.Count == other.Count &&
                @this.IsReadOnly == other.IsReadOnly &&
                @this.IsSynchronized == other.IsSynchronized))
                return false;

            for (int i = 0; i < @this.Count; i++)
            {
                if (!@this[i].Equals(other[i]))
                    return false;
            }

            return true;
        }

        //public static bool IsEqual(this Stack @this, Stack other)
        //{
        //    return @this != null &&
        //        other != null &&
        //        @this.Count == other.Count &&
        //        @this.IsSynchronized == other.IsSynchronized &&
        //        @this.SyncRoot == other.SyncRoot;
        //    throw new NotImplementedException("finish me");
        //}

        //public static bool IsEqual(this BindingList<int> @this, BindingList<int> other)
        //{
        //    if (!(@this != null &&
        //        other != null &&
        //        @this.RaiseListChangedEvents == other.RaiseListChangedEvents &&
        //        @this.AllowNew == other.AllowNew &&
        //        @this.AllowEdit == other.AllowEdit &&
        //        @this.AllowRemove == other.AllowRemove &&
        //        @this.Count == other.Count))
        //        return false;

        //    for (int i = 0; i < @this.Count; i++)
        //    {
        //        if (@this[i] != other[i])
        //            return false;
        //    }

        //    return true;
        //}

        public static bool IsEqual(this BindingList<Point> @this, BindingList<Point> other)
        {
            if (!(@this != null &&
                other != null &&
                @this.RaiseListChangedEvents == other.RaiseListChangedEvents &&
                @this.AllowNew == other.AllowNew &&
                @this.AllowEdit == other.AllowEdit &&
                @this.AllowRemove == other.AllowRemove &&
                @this.Count == other.Count))
                return false;

            for (int i = 0; i < @this.Count; i++)
            {
                if (!IsEqual(@this[i], other[i]))
                    return false;
            }

            return true;
        }

        //public static bool IsEqual(this PropertyCollection @this, PropertyCollection other)
        //{
        //    if (!(@this != null &&
        //        other != null &&
        //        @this.IsReadOnly == other.IsReadOnly &&
        //        @this.IsFixedSize == other.IsFixedSize &&
        //        @this.IsSynchronized == other.IsSynchronized &&
        //        @this.Keys == other.Keys &&
        //        @this.Values == other.Values &&
        //        @this.SyncRoot == other.SyncRoot &&
        //        @this.Count == other.Count))
        //        return false;

        //    for (int i = 0; i < @this.Count; i++)
        //    {
        //        if (@this[i] != other[i])
        //            return false;
        //    }

        //    return true;
        //}

        public static bool IsEqual(this CompareInfo @this, CompareInfo other)
        {
            return @this != null &&
                other != null &&
                @this.Name == other.Name &&
                @this.LCID == other.LCID &&
                @this.Version == other.Version;
        }

        public static bool IsEqual(this SortVersion @this, SortVersion other)
        {
            return @this != null &&
                other != null &&
                @this.FullVersion == other.FullVersion &&
                @this.SortId == other.SortId;
        }

        //public static bool IsEqual(this Cookie @this, Cookie other)
        //{
        //    return @this != null &&
        //        other != null &&
        //        @this.Comment == other.Comment &&
        //        IsEqual(@this.CommentUri, other.CommentUri) &&
        //        @this.HttpOnly == other.HttpOnly &&
        //        @this.Discard == other.Discard &&
        //        @this.Domain == other.Domain &&
        //        @this.Expired == other.Expired &&
        //        @this.Expires == other.Expires &&
        //        @this.Name == other.Name &&
        //        @this.Path == other.Path &&
        //        @this.Port == other.Port &&
        //        @this.Secure == other.Secure &&
        //        @this.TimeStamp == other.TimeStamp &&
        //        @this.Value == other.Value &&
        //        @this.Version == other.Version;
        //}

        //public static bool IsEqual(this CookieCollection @this, CookieCollection other)
        //{
        //    if (!(@this != null &&
        //        other != null &&
        //        @this.IsReadOnly == other.IsReadOnly &&
        //        @this.Count == other.Count &&
        //        @this.IsSynchronized == other.IsSynchronized &&
        //        @this.SyncRoot == other.SyncRoot))
        //        return false;
        //    for (int i = 0; i < @this.Count; i++)
        //    {
        //        if (!IsEqual(@this[i], other[i]))
        //            return false;
        //    }

        //    return true;
        //}

        public static bool IsEqual(this BasicISerializableObject @this, BasicISerializableObject other)
        {
            return @this != null &&
                other != null;
        }

        public static bool IsEqual(this DerivedISerializableWithNonPublicDeserializationCtor @this, DerivedISerializableWithNonPublicDeserializationCtor other)
        {
            return @this != null &&
                other != null;
        }

        //public static bool IsEqual(this Graph<int> @this, Graph<int> other)
        //{
        //    Graph can have cycles
        //    if (!(@this != null &&
        //        other != null &&
        //        @this.Value == other.Value &&
        //        @this.Links.Length == other.Links.Length))
        //        return false;

        //    for (int i = 0; i < @this.Links.Length; i++)
        //    {
        //        if (!IsEqual(@this.Links[i], other.Links[i]))
        //            return false;
        //    }

        //    return true;
        //}

        //public static bool IsEqual(this ObjectWithArrays @this, ObjectWithArrays other)
        //{
        //    return @this != null &&
        //        other != null &&
        //        @this.IntArray.SequenceEqual(other.IntArray) &&
        //        @this.StringArray.SequenceEqual(other.StringArray) &&
        //        @this.TreeArray.SequenceEqual(other.TreeArray) &&
        //        @this.ByteArray.SequenceEqual(other.ByteArray); // &&
        //        //@this.JaggedArray.SequenceEqual(other.JaggedArray) &&
        //        //@this.MultiDimensionalArray.Equals(other.MultiDimensionalArray);
        //   throw new NotImplementedException("finish me");
        //}

        public static bool IsEqual(this ObjectWithIntStringUShortUIntULongAndCustomObjectFields @this, ObjectWithIntStringUShortUIntULongAndCustomObjectFields other)
        {
            return @this != null &&
                other != null &&
                @this.Member1 == other.Member1 &&
                @this.Member2 == other.Member2 &&
                @this._member3 == other._member3 &&
                IsEqual(@this.Member4, other.Member4) &&
                IsEqual(@this.Member4shared, other.Member4shared) &&
                IsEqual(@this.Member5, other.Member5) &&
                @this.Member6 == other.Member6 &&
                @this.str1 == other.str1 &&
                @this.str2 == other.str2 &&
                @this.str3 == other.str3 &&
                @this.str4 == other.str4 &&
                @this.u16 == other.u16 &&
                @this.u32 == other.u32 &&
                @this.u64 == other.u64;
        }

        public static bool IsEqual(this Point @this, Point other)
        {
            return @this != null &&
                other != null &&
                @this.X == other.X &&
                @this.Y == other.Y;
        }

        public static bool IsEqual(this SealedObjectWithIntStringFields @this, SealedObjectWithIntStringFields other)
        {
            return @this != null &&
                other != null &&
                @this.Member1 == other.Member1 &&
                @this.Member2 == other.Member2 &&
                @this.Member3 == other.Member3;
        }

        public static bool IsEqual(this SimpleKeyedCollection @this, SimpleKeyedCollection other)
        {
            if (!(@this != null &&
                other != null &&
                //@this.Comparer.Equals(other.Comparer) &&
                @this.Count == other.Count))
                return false;

            Collection<Point> a = @this;
            Collection<Point> b = other;
            for (int i = 0; i < a.Count; i++)
            {
                if (!IsEqual(a[i], b[i]))
                    return false;
            }

            return true;
        }

        public static bool IsEqual(this Tree<Colors> @this, Tree<Colors> other)
        {
            if (@this == null && other == null)
                return true;

            return @this != null &&
                other != null &&
                @this.Value == other.Value &&
                IsEqual(@this.Left, other.Left) &&
                IsEqual(@this.Right, other.Right);
        }

        public static bool IsEqual(this TimeZoneInfo @this, TimeZoneInfo other)
        {
            return @this != null &&
                other != null &&
                @this.Id == other.Id &&
                @this.DisplayName == other.DisplayName &&
                @this.StandardName == other.StandardName &&
                @this.DaylightName == other.DaylightName &&
                @this.BaseUtcOffset == other.BaseUtcOffset &&
                @this.SupportsDaylightSavingTime == other.SupportsDaylightSavingTime;
        }

        public static bool IsEqual(this Tuple<int> @this, Tuple<int> other)
        {
            return @this != null &&
                other != null &&
                @this.Item1 == other.Item1;
        }

        public static bool IsEqual(this Tuple<int, string> @this, Tuple<int, string> other)
        {
            return @this != null &&
                other != null &&
                @this.Item1 == other.Item1 &&
                @this.Item2 == other.Item2;
        }

        public static bool IsEqual(this Tuple<int, string, uint> @this, Tuple<int, string, uint> other)
        {
            return @this != null &&
                other != null &&
                @this.Item1 == other.Item1 &&
                @this.Item2 == other.Item2 &&
                @this.Item3 == other.Item3;
        }

        public static bool IsEqual(this Tuple<int, string, uint, long> @this, Tuple<int, string, uint, long> other)
        {
            return @this != null &&
                other != null &&
                @this.Item1 == other.Item1 &&
                @this.Item2 == other.Item2 &&
                @this.Item3 == other.Item3 &&
                @this.Item4 == other.Item4;
        }

        public static bool IsEqual(this Tuple<int, string, uint, long, double> @this, Tuple<int, string, uint, long, double> other)
        {
            return @this != null &&
                other != null &&
                @this.Item1 == other.Item1 &&
                @this.Item2 == other.Item2 &&
                @this.Item3 == other.Item3 &&
                @this.Item4 == other.Item4 &&
                @this.Item5 == other.Item5;
        }

        public static bool IsEqual(this Tuple<int, string, uint, long, double, float> @this, Tuple<int, string, uint, long, double, float> other)
        {
            return @this != null &&
                other != null &&
                @this.Item1 == other.Item1 &&
                @this.Item2 == other.Item2 &&
                @this.Item3 == other.Item3 &&
                @this.Item4 == other.Item4 &&
                @this.Item5 == other.Item5 &&
                @this.Item6 == other.Item6;
        }

        public static bool IsEqual(this Tuple<int, string, uint, long, double, float, decimal> @this, Tuple<int, string, uint, long, double, float, decimal> other)
        {
            return @this != null &&
                other != null &&
                @this.Item1 == other.Item1 &&
                @this.Item2 == other.Item2 &&
                @this.Item3 == other.Item3 &&
                @this.Item4 == other.Item4 &&
                @this.Item5 == other.Item5 &&
                @this.Item6 == other.Item6 &&
                @this.Item7 == other.Item7;
        }

        public static bool IsEqual(this Tuple<int, string, uint, long, double, float, decimal, Tuple<Tuple<int>>> @this, Tuple<int, string, uint, long, double, float, decimal, Tuple<Tuple<int>>> other)
        {
            return @this != null &&
                other != null &&
                @this.Item1 == other.Item1 &&
                @this.Item2 == other.Item2 &&
                @this.Item3 == other.Item3 &&
                @this.Item4 == other.Item4 &&
                @this.Item5 == other.Item5 &&
                @this.Item6 == other.Item6 &&
                @this.Item7 == other.Item7 &&
                @this.Rest.Item1.Item1 == other.Rest.Item1.Item1;
        }

        //public static bool IsEqual(this Uri @this, Uri other)
        //{
        //    return @this != null &&
        //        other != null &&
        //        @this.AbsolutePath == other.AbsolutePath &&
        //        @this.AbsoluteUri == other.AbsoluteUri &&
        //        @this.LocalPath == other.LocalPath &&
        //        @this.Authority == other.Authority &&
        //        @this.HostNameType == other.HostNameType &&
        //        @this.IsDefaultPort == other.IsDefaultPort &&
        //        @this.IsFile == other.IsFile &&
        //        @this.IsLoopback == other.IsLoopback &&
        //        @this.PathAndQuery == other.PathAndQuery &&
        //        @this.Segments.SequenceEqual(other.Segments) &&
        //        @this.IsUnc == other.IsUnc &&
        //        @this.Host == other.Host &&
        //        @this.Port == other.Port &&
        //        @this.Query == other.Query &&
        //        @this.Fragment == other.Fragment &&
        //        @this.Scheme == other.Scheme &&
        //        @this.OriginalString == other.OriginalString &&
        //        @this.DnsSafeHost == other.DnsSafeHost &&
        //        @this.IdnHost == other.IdnHost &&
        //        @this.IsAbsoluteUri == other.IsAbsoluteUri &&
        //        @this.UserEscaped == other.UserEscaped &&
        //        @this.UserInfo == other.UserInfo;
        //}

        public static bool IsEqual(this Version @this, Version other)
        {
            return @this != null &&
                other != null &&
                @this.Major == other.Major &&
                @this.Minor == other.Minor &&
                @this.Build == other.Build &&
                @this.Revision == other.Revision &&
                @this.MajorRevision == other.MajorRevision &&
                @this.MinorRevision == other.MinorRevision;
        }
    }
}
