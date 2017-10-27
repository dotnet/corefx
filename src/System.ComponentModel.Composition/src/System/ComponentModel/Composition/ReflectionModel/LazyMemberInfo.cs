// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.ReflectionModel
{
    public struct LazyMemberInfo
    {
        private readonly MemberTypes _memberType;
        private MemberInfo[] _accessors;
        private readonly Func<MemberInfo[]> _accessorsCreator;

        public LazyMemberInfo(MemberInfo member)
        {
            Requires.NotNull(member, "member");
            EnsureSupportedMemberType(member.MemberType, "member");

            this._accessorsCreator = null;
            this._memberType = member.MemberType;
            
            switch(this._memberType)
            {
                case MemberTypes.Property:
                    PropertyInfo property = (PropertyInfo)member;
                    Assumes.NotNull(property);
                    this._accessors = new MemberInfo[] { property.GetGetMethod(true), property.GetSetMethod(true) };
                    break;
                case MemberTypes.Event:
                    EventInfo event_ = (EventInfo)member;
                    this._accessors = new MemberInfo[] { event_.GetRaiseMethod(true), event_.GetAddMethod(true), event_.GetRemoveMethod(true) };
                    break;
                default:
                    this._accessors = new MemberInfo[] { member };
                    break;
            }
        }

        public LazyMemberInfo(MemberTypes memberType, params MemberInfo[] accessors)
        {
            EnsureSupportedMemberType(memberType, "memberType");
            Requires.NotNull(accessors, "accessors");
            
            string errorMessage;
            if (!LazyMemberInfo.AreAccessorsValid(memberType, accessors, out errorMessage))
            {
                throw new ArgumentException(errorMessage, "accessors");
            }

            this._memberType = memberType;
            this._accessors = accessors;
            this._accessorsCreator = null;
        }

        public LazyMemberInfo(MemberTypes memberType, Func<MemberInfo[]> accessorsCreator)
        {
            EnsureSupportedMemberType(memberType, "memberType");
            Requires.NotNull(accessorsCreator, "accessorsCreator");

            this._memberType = memberType;
            this._accessors = null;
            this._accessorsCreator = accessorsCreator;
        }

        public MemberTypes MemberType
        {
            get { return this._memberType; }
        }

        public MemberInfo[] GetAccessors()
        {
            if ((this._accessors == null) && (this._accessorsCreator != null))
            {
                MemberInfo[] accessors = this._accessorsCreator.Invoke();

                string errorMessage;
                if (!LazyMemberInfo.AreAccessorsValid(this.MemberType, accessors, out errorMessage))
                {
                    throw new InvalidOperationException(errorMessage);
                }

                this._accessors = accessors;
            }

            return this._accessors;
        }

        public override int GetHashCode()
        {
            if (this._accessorsCreator != null)
            {
                return this.MemberType.GetHashCode() ^ this._accessorsCreator.GetHashCode();
            }
            else
            {
                Assumes.NotNull(this._accessors);
                Assumes.NotNull(this._accessors[0]);
                return this.MemberType.GetHashCode() ^ this._accessors[0].GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            LazyMemberInfo that = (LazyMemberInfo)obj;

            // Difefrent member types mean different members
            if (this._memberType != that._memberType)
            {
                return false;
            }

            // if any of the lazy memebers create accessors in a delay-loaded fashion, we simply compare the creators
            if ((this._accessorsCreator != null) || (that._accessorsCreator != null))
            {
                return object.Equals(this._accessorsCreator, that._accessorsCreator);
            }

            // we are dealing with explicitly passed accessors in both cases
            Assumes.NotNull(this._accessors);
            Assumes.NotNull(that._accessors);
            return this._accessors.SequenceEqual(that._accessors);
        }

        public static bool operator ==(LazyMemberInfo left, LazyMemberInfo right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(LazyMemberInfo left, LazyMemberInfo right)
        {
            return !left.Equals(right);
        }

        [ContractArgumentValidator]
        private static void EnsureSupportedMemberType(MemberTypes memberType, string argument)
        {
            MemberTypes supportedTypes = MemberTypes.TypeInfo | MemberTypes.NestedType | MemberTypes.Constructor | MemberTypes.Field | MemberTypes.Method | MemberTypes.Property | MemberTypes.Event;
            Requires.IsInMembertypeSet(memberType, argument, supportedTypes);
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
