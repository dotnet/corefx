// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Linq;
using System.Reflection;

namespace System.ComponentModel.Composition.ReflectionModel
{
    public struct LazyMemberInfo
    {
        private MemberInfo[] _accessors;
        private readonly Func<MemberInfo[]> _accessorsCreator;

        public LazyMemberInfo(MemberInfo member)
        {
            if (member == null)
            {
                throw new ArgumentNullException(nameof(member));
            }
            EnsureSupportedMemberType(member.MemberType, nameof(member));

            _accessorsCreator = null;
            MemberType = member.MemberType;

            switch (MemberType)
            {
                case MemberTypes.Property:
                    PropertyInfo property = (PropertyInfo)member;
                    if (property == null)
                    {
                        throw new Exception(SR.Diagnostic_InternalExceptionMessage);
                    }
                    _accessors = new MemberInfo[] { property.GetGetMethod(true), property.GetSetMethod(true) };
                    break;
                case MemberTypes.Event:
                    EventInfo event_ = (EventInfo)member;
                    _accessors = new MemberInfo[] { event_.GetRaiseMethod(true), event_.GetAddMethod(true), event_.GetRemoveMethod(true) };
                    break;
                default:
                    _accessors = new MemberInfo[] { member };
                    break;
            }
        }

        public LazyMemberInfo(MemberTypes memberType, params MemberInfo[] accessors)
        {
            EnsureSupportedMemberType(memberType, nameof(memberType));
            if (accessors == null)
            {
                throw new ArgumentNullException(nameof(accessors));
            }

            if (!AreAccessorsValid(memberType, accessors, out string errorMessage))
            {
                throw new ArgumentException(errorMessage, nameof(accessors));
            }

            MemberType = memberType;
            _accessors = accessors;
            _accessorsCreator = null;
        }

        public LazyMemberInfo(MemberTypes memberType, Func<MemberInfo[]> accessorsCreator)
        {
            EnsureSupportedMemberType(memberType, nameof(memberType));

            MemberType = memberType;
            _accessors = null;
            _accessorsCreator = accessorsCreator ?? throw new ArgumentNullException(nameof(accessorsCreator));
        }

        public MemberTypes MemberType { get; }

        public MemberInfo[] GetAccessors()
        {
            if ((_accessors == null) && (_accessorsCreator != null))
            {
                MemberInfo[] accessors = _accessorsCreator.Invoke();

                if (!AreAccessorsValid(MemberType, accessors, out string errorMessage))
                {
                    throw new InvalidOperationException(errorMessage);
                }

                _accessors = accessors;
            }

            return _accessors;
        }

        public override int GetHashCode()
        {
            if (_accessorsCreator != null)
            {
                return MemberType.GetHashCode() ^ _accessorsCreator.GetHashCode();
            }
            else if (_accessors == null || _accessors[0] == null)
            {
                throw new Exception(SR.Diagnostic_InternalExceptionMessage);
            }

            return MemberType.GetHashCode() ^ _accessors[0].GetHashCode();
        }

        public override bool Equals(object obj)
        {
            LazyMemberInfo that = (LazyMemberInfo)obj;

            // Difefrent member types mean different members
            if (MemberType != that.MemberType)
            {
                return false;
            }

            // if any of the lazy memebers create accessors in a delay-loaded fashion, we simply compare the creators
            if ((_accessorsCreator != null) || (that._accessorsCreator != null))
            {
                return Equals(_accessorsCreator, that._accessorsCreator);
            }

            // we are dealing with explicitly passed accessors in both cases
            if (_accessors == null || that._accessors == null)
            {
                throw new Exception(SR.Diagnostic_InternalExceptionMessage);
            }
            return _accessors.SequenceEqual(that._accessors);
        }

        public static bool operator ==(LazyMemberInfo left, LazyMemberInfo right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(LazyMemberInfo left, LazyMemberInfo right)
        {
            return !left.Equals(right);
        }
        private static void EnsureSupportedMemberType(MemberTypes memberType, string argument)
        {
            MemberTypes supportedTypes = MemberTypes.TypeInfo | MemberTypes.NestedType | MemberTypes.Constructor | MemberTypes.Field | MemberTypes.Method | MemberTypes.Property | MemberTypes.Event;
            if ((memberType & supportedTypes) != memberType || (memberType & (memberType - 1)) != 0)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, SR.ArgumentOutOfRange_InvalidEnumInSet, argument, memberType, supportedTypes.ToString()), argument);
            }
        }

        private static bool AreAccessorsValid(MemberTypes memberType, MemberInfo[] accessors, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (accessors == null)
            {
                errorMessage = SR.LazyMemberInfo_AccessorsNull;
                return false;
            }

            if (accessors.All(accessor => accessor == null))
            {
                errorMessage = SR.LazyMemberInfo_NoAccessors;
                return false;
            }

            switch (memberType)
            {
                case MemberTypes.Property:
                    if (accessors.Length != 2)
                    {
                        errorMessage = SR.LazyMemberInfo_InvalidPropertyAccessors_Cardinality;
                        return false;
                    }

                    if (accessors.Where(accessor => (accessor != null) && (accessor.MemberType != MemberTypes.Method)).Any())
                    {
                        errorMessage = SR.LazyMemberinfo_InvalidPropertyAccessors_AccessorType;
                        return false;
                    }

                    break;

                case MemberTypes.Event:
                    if (accessors.Length != 3)
                    {
                        errorMessage = SR.LazyMemberInfo_InvalidEventAccessors_Cardinality;
                        return false;
                    }

                    if (accessors.Where(accessor => (accessor != null) && (accessor.MemberType != MemberTypes.Method)).Any())
                    {
                        errorMessage = SR.LazyMemberinfo_InvalidEventAccessors_AccessorType;
                        return false;
                    }

                    break;

                default:
                    if (
                        (accessors.Length != 1) ||
                        ((accessors.Length == 1) && (accessors[0].MemberType != memberType)))
                    {
                        errorMessage = string.Format(CultureInfo.CurrentCulture, SR.LazyMemberInfo_InvalidAccessorOnSimpleMember, memberType);
                        return false;
                    }

                    break;
            }
            return true;
        }
    }
}
