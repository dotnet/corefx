// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace System.ComponentModel.Design.Serialization
{
    /// <summary>
    /// A member relationship service is used by a serializer to announce that one
    /// property is related to a property on another object. Consider a code
    /// based serialization scheme where code is of the following form:
    /// 
    /// object1.Property1 = object2.Property2
    /// 
    /// Upon interpretation of this code, Property1 on object1 will be
    /// set to the return value of object2.Property2. But the relationship
    /// between these two objects is lost. Serialization schemes that
    /// wish to maintain this relationship may install a MemberRelationshipService
    /// into the serialization manager. When an object is deserialized
    /// this service will be notified of these relationships. It is up to the service
    /// to act on these notifications if it wishes. During serialization, the
    /// service is also consulted. If a relationship exists the same
    /// relationship is maintained by the serializer.
    /// </summary>
    public abstract class MemberRelationshipService
    {
        private readonly Dictionary<RelationshipEntry, RelationshipEntry> _relationships = new Dictionary<RelationshipEntry, RelationshipEntry>();

        /// <summary>
        /// Returns the current relationship associated with the source, or MemberRelationship.Empty if
        /// there is no relationship. Also sets a relationship between two objects. Empty
        /// can also be passed as the property value, in which case the relationship will
        /// be cleared.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1043:UseIntegralOrStringArgumentForIndexers")]
        public MemberRelationship this[MemberRelationship source]
        {
            get
            {
                if (source.Owner == null)
                {
                    throw new ArgumentNullException(nameof(MemberRelationship.Owner));
                }
                if (source.Member == null)
                {
                    throw new ArgumentNullException(nameof(MemberRelationship.Member));
                }

                return GetRelationship(source);
            }
            set
            {
                if (source.Owner == null)
                {
                    throw new ArgumentNullException(nameof(MemberRelationship.Owner));
                }
                if (source.Member == null)
                {
                    throw new ArgumentNullException(nameof(MemberRelationship.Member));
                }

                SetRelationship(source, value);
            }
        }

        /// <summary>
        /// Returns the current relationship associated with the source, or null if
        /// there is no relationship. Also sets a relationship between two objects. Null
        /// can be passed as the property value, in which case the relationship will
        /// be cleared.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1023:IndexersShouldNotBeMultidimensional")]
        public MemberRelationship this[object sourceOwner, MemberDescriptor sourceMember]
        {
            get
            {
                if (sourceOwner == null)
                {
                    throw new ArgumentNullException(nameof(sourceOwner));
                }
                if (sourceMember == null)
                {
                    throw new ArgumentNullException(nameof(sourceMember));
                }

                return GetRelationship(new MemberRelationship(sourceOwner, sourceMember));
            }
            set
            {
                if (sourceOwner == null)
                {
                    throw new ArgumentNullException(nameof(sourceOwner));
                }
                if (sourceMember == null)
                {
                    throw new ArgumentNullException(nameof(sourceMember));
                }

                SetRelationship(new MemberRelationship(sourceOwner, sourceMember), value);
            }
        }

        /// <summary>
        /// This is the implementation API for returning relationships. The default implementation stores the 
        /// relationship in a table. Relationships are stored weakly, so they do not keep an object alive.
        /// </summary>
        protected virtual MemberRelationship GetRelationship(MemberRelationship source)
        {
            RelationshipEntry retVal;

            if (_relationships != null && _relationships.TryGetValue(new RelationshipEntry(source), out retVal) && retVal._owner.IsAlive)
            {
                return new MemberRelationship(retVal._owner.Target, retVal._member);
            }

            return MemberRelationship.Empty;
        }

        /// <summary>
        /// This is the implementation API for returning relationships. The default implementation stores the 
        /// relationship in a table. Relationships are stored weakly, so they do not keep an object alive. Empty can be
        /// passed in for relationship to remove the relationship.
        /// </summary>
        protected virtual void SetRelationship(MemberRelationship source, MemberRelationship relationship)
        {
            if (!relationship.IsEmpty && !SupportsRelationship(source, relationship))
            {
                string sourceName = TypeDescriptor.GetComponentName(source.Owner);
                string relName = TypeDescriptor.GetComponentName(relationship.Owner);
                if (sourceName == null)
                {
                    sourceName = source.Owner.ToString();
                }
                if (relName == null)
                {
                    relName = relationship.Owner.ToString();
                }
                throw new ArgumentException(SR.Format(SR.MemberRelationshipService_RelationshipNotSupported, sourceName, source.Member.Name, relName, relationship.Member.Name));
            }

            _relationships[new RelationshipEntry(source)] = new RelationshipEntry(relationship);
        }

        /// <summary>
        /// Returns true if the provided relationship is supported.
        /// </summary>
        public abstract bool SupportsRelationship(MemberRelationship source, MemberRelationship relationship);

        /// <summary>
        /// Used as storage in our relationship table
        /// </summary>
        private struct RelationshipEntry
        {
            internal WeakReference _owner;
            internal MemberDescriptor _member;
            private readonly int _hashCode;

            internal RelationshipEntry(MemberRelationship rel)
            {
                _owner = new WeakReference(rel.Owner);
                _member = rel.Member;
                _hashCode = rel.Owner == null ? 0 : rel.Owner.GetHashCode();
            }

            public override bool Equals(object o)
            {
                if (o is RelationshipEntry)
                {
                    RelationshipEntry e = (RelationshipEntry)o;
                    return this == e;
                }

                return false;
            }

            public static bool operator ==(RelationshipEntry re1, RelationshipEntry re2)
            {
                object owner1 = (re1._owner.IsAlive ? re1._owner.Target : null);
                object owner2 = (re2._owner.IsAlive ? re2._owner.Target : null);
                return owner1 == owner2 && re1._member.Equals(re2._member);
            }

            public static bool operator !=(RelationshipEntry re1, RelationshipEntry re2)
            {
                return !(re1 == re2);
            }

            public override int GetHashCode() => _hashCode;
        }
    }

    /// <summary>
    /// This class represents a single relationship between an object and a member.
    /// </summary>
    public readonly struct MemberRelationship
    {
        private readonly object _owner;
        private readonly MemberDescriptor _member;

        public static readonly MemberRelationship Empty = new MemberRelationship();

        /// <summary>
        /// Creates a new member relationship.
        /// </summary>
        public MemberRelationship(object owner, MemberDescriptor member)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _member = member ?? throw new ArgumentNullException(nameof(member));
        }

        /// <summary>
        /// Returns true if this relationship is empty.
        /// </summary>
        public bool IsEmpty => Owner == null;

        /// <summary>
        /// The member in this relationship.
        /// </summary>
        public MemberDescriptor Member => _member;

        /// <summary>
        /// The object owning the member.
        /// </summary>
        public object Owner => _owner;

        /// <summary>
        /// Infrastructure support to make this a first class struct
        /// </summary>
        public override bool Equals(object obj)
        {
            if (!(obj is MemberRelationship rel))
            {
                return false;
            }

            return rel.Owner == Owner && rel.Member == Member;
        }

        /// <summary>
        /// Infrastructure support to make this a first class struct
        /// </summary>
        public override int GetHashCode()
        {
            if (Owner == null)
            {
                return base.GetHashCode();
            }

            return Owner.GetHashCode() ^ Member.GetHashCode();
        }
        /// <summary>
        /// Infrastructure support to make this a first class struct
        /// </summary>
        public static bool operator ==(MemberRelationship left, MemberRelationship right)
        {
            return left.Owner == right.Owner && left.Member == right.Member;
        }

        /// <summary>
        /// Infrastructure support to make this a first class struct
        /// </summary>
        public static bool operator !=(MemberRelationship left, MemberRelationship right)
        {
            return !(left == right);
        }
    }
}
