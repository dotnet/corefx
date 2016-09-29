// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Reflection;

namespace System.Security.Principal
{
    public class IdentityReferenceCollection : ICollection<IdentityReference>
    {
        #region Private members

        //
        // Container enumerated by this collection
        //

        private readonly List<IdentityReference> _Identities;

        #endregion

        #region Constructors

        //
        // Creates an empty collection of default size
        //

        public IdentityReferenceCollection()
            : this(0)
        {
        }

        //
        // Creates an empty collection of given initial size
        //

        public IdentityReferenceCollection(int capacity)
        {
            _Identities = new List<IdentityReference>(capacity);
        }

        #endregion

        #region ICollection<IdentityReference> implementation

        public void CopyTo(IdentityReference[] array, int offset)
        {
            _Identities.CopyTo(0, array, offset, Count);
        }

        public int Count
        {
            get
            {
                return _Identities.Count;
            }
        }

        bool ICollection<IdentityReference>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public void Add(IdentityReference identity)
        {
            if (identity == null)
            {
                throw new ArgumentNullException(nameof(identity));
            }
            Contract.EndContractBlock();

            _Identities.Add(identity);
        }

        public bool Remove(IdentityReference identity)
        {
            if (identity == null)
            {
                throw new ArgumentNullException(nameof(identity));
            }
            Contract.EndContractBlock();

            if (Contains(identity))
            {
                return _Identities.Remove(identity);
            }

            return false;
        }

        public void Clear()
        {
            _Identities.Clear();
        }

        public bool Contains(IdentityReference identity)
        {
            if (identity == null)
            {
                throw new ArgumentNullException(nameof(identity));
            }
            Contract.EndContractBlock();

            return _Identities.Contains(identity);
        }

        #endregion
        
        #region IEnumerable<IdentityReference> implementation

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<IdentityReference> GetEnumerator()
        {
            return new IdentityReferenceEnumerator(this);
        }

        #endregion

        #region Public methods

        public IdentityReference this[int index]
        {
            get
            {
                return _Identities[index];
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                Contract.EndContractBlock();

                _Identities[index] = value;
            }
        }

        internal List<IdentityReference> Identities
        {
            get
            {
                return _Identities;
            }
        }

        public IdentityReferenceCollection Translate(Type targetType)
        {
            return Translate(targetType, false);
        }

        public IdentityReferenceCollection Translate(Type targetType, bool forceSuccess)
        {
            if (targetType == null)
            {
                throw new ArgumentNullException(nameof(targetType));
            }

            //
            // Target type must be a subclass of IdentityReference
            //

            if (!targetType.GetTypeInfo().IsSubclassOf(typeof(IdentityReference)))
            {
                throw new ArgumentException(SR.IdentityReference_MustBeIdentityReference, nameof(targetType));
            }
            Contract.EndContractBlock();

            //
            // if the source collection is empty, just return an empty collection
            //
            if (Identities.Count == 0)
            {
                return new IdentityReferenceCollection();
            }

            int SourceSidsCount = 0;
            int SourceNTAccountsCount = 0;

            //
            // First, see how many of each of the source types we have.
            // The cases where source type == target type require no conversion.
            //

            for (int i = 0; i < Identities.Count; i++)
            {
                Type type = Identities[i].GetType();

                if (type == targetType)
                {
                    continue;
                }
                else if (type == typeof(SecurityIdentifier))
                {
                    SourceSidsCount += 1;
                }
                else if (type == typeof(NTAccount))
                {
                    SourceNTAccountsCount += 1;
                }
                else
                {
                    //
                    // Rare case that we have defined a type of identity reference and not included it in the code logic above.  
                    // To avoid this we do not allow IdentityReference to be subclassed outside of the BCL.
                    // 
                    Debug.Assert(false, "Source type is an IdentityReference type which has not been included in translation logic.");
                    throw new NotSupportedException();
                }
            }

            bool Homogeneous = false;
            IdentityReferenceCollection SourceSids = null;
            IdentityReferenceCollection SourceNTAccounts = null;

            if (SourceSidsCount == Count)
            {
                Homogeneous = true;
                SourceSids = this;
            }
            else if (SourceSidsCount > 0)
            {
                SourceSids = new IdentityReferenceCollection(SourceSidsCount);
            }

            if (SourceNTAccountsCount == Count)
            {
                Homogeneous = true;
                SourceNTAccounts = this;
            }
            else if (SourceNTAccountsCount > 0)
            {
                SourceNTAccounts = new IdentityReferenceCollection(SourceNTAccountsCount);
            }
            //
            // Repackage only if the source is not homogeneous (contains different source types)
            //

            IdentityReferenceCollection Result = null;

            if (!Homogeneous)
            {
                Result = new IdentityReferenceCollection(Identities.Count);

                for (int i = 0; i < Identities.Count; i++)
                {
                    IdentityReference id = this[i];

                    Type type = id.GetType();

                    if (type == targetType)
                    {
                        continue;
                    }
                    else if (type == typeof(SecurityIdentifier))
                    {
                        SourceSids.Add(id);
                    }
                    else if (type == typeof(NTAccount))
                    {
                        SourceNTAccounts.Add(id);
                    }
                    else
                    {
                        //
                        // Rare case that we have defined a type of identity reference and not included it in the code logic above.  
                        // To avoid this we do not allow IdentityReference to be subclassed outside of the BCL.
                        // 
                        Debug.Assert(false, "Source type is an IdentityReference type which has not been included in translation logic.");
                        throw new NotSupportedException();
                    }
                }
            }

            bool someFailed = false;
            IdentityReferenceCollection TargetSids = null, TargetNTAccounts = null;

            if (SourceSidsCount > 0)
            {
                TargetSids = SecurityIdentifier.Translate(SourceSids, targetType, out someFailed);

                if (Homogeneous && !(forceSuccess && someFailed))
                {
                    Result = TargetSids;
                }
            }

            if (SourceNTAccountsCount > 0)
            {
                TargetNTAccounts = NTAccount.Translate(SourceNTAccounts, targetType, out someFailed);

                if (Homogeneous && !(forceSuccess && someFailed))
                {
                    Result = TargetNTAccounts;
                }
            }

            if (forceSuccess && someFailed)
            {
                //
                // Need to throw an exception here and provide information regarding 
                // which identity references could not be translated to the target type
                //

                Result = new IdentityReferenceCollection();

                if (TargetSids != null)
                {
                    foreach (IdentityReference id in TargetSids)
                    {
                        if (id.GetType() != targetType)
                        {
                            Result.Add(id);
                        }
                    }
                }

                if (TargetNTAccounts != null)
                {
                    foreach (IdentityReference id in TargetNTAccounts)
                    {
                        if (id.GetType() != targetType)
                        {
                            Result.Add(id);
                        }
                    }
                }

                throw new IdentityNotMappedException(SR.IdentityReference_IdentityNotMapped, Result);
            }
            else if (!Homogeneous)
            {
                SourceSidsCount = 0;
                SourceNTAccountsCount = 0;

                Result = new IdentityReferenceCollection(Identities.Count);

                for (int i = 0; i < Identities.Count; i++)
                {
                    IdentityReference id = this[i];

                    Type type = id.GetType();

                    if (type == targetType)
                    {
                        Result.Add(id);
                    }
                    else if (type == typeof(SecurityIdentifier))
                    {
                        Result.Add(TargetSids[SourceSidsCount++]);
                    }
                    else if (type == typeof(NTAccount))
                    {
                        Result.Add(TargetNTAccounts[SourceNTAccountsCount++]);
                    }
                    else
                    {
                        //
                        // Rare case that we have defined a type of identity reference and not included it in the code logic above.  
                        // To avoid this we do not allow IdentityReference to be subclassed outside of the BCL.
                        // 
                        Debug.Assert(false, "Source type is an IdentityReference type which has not been included in translation logic.");
                        throw new NotSupportedException();
                    }
                }
            }

            return Result;
        }
        #endregion
    }

    internal class IdentityReferenceEnumerator : IEnumerator<IdentityReference>, IDisposable
    {
        #region Private members

        //
        // Current enumeration index
        //

        private int _current;

        //
        // Parent collection
        //

        private readonly IdentityReferenceCollection _collection;

        #endregion

        #region Constructors

        internal IdentityReferenceEnumerator(IdentityReferenceCollection collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            Contract.EndContractBlock();

            _collection = collection;
            _current = -1;
        }

        #endregion

        #region IEnumerator implementation

        /// <internalonly/>
        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public IdentityReference Current
        {
            get
            {
                return _collection.Identities[_current];
            }
        }

        public bool MoveNext()
        {
            _current++;

            return (_current < _collection.Count);
        }

        public void Reset()
        {
            _current = -1;
        }

        public void Dispose()
        {
        }
        #endregion
    }
}
