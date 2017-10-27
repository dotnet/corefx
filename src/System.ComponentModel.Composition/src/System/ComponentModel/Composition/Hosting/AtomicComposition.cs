// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.Hosting
{
    /// <summary>
    /// AtomicComposition provides lightweight atomicCompositional semantics to enable temporary
    /// state to be managed for a series of nested atomicCompositions.  Each atomicComposition maintains
    /// queryable state along with a sequence of actions necessary to complete the state when
    /// the atomicComposition is no longer in danger of being rolled back.  State is completed or
    /// rolled back when the atomicComposition is disposed, depending on the state of the
    /// CompleteOnDipose property which defaults to false.  The using(...) pattern in C# is a
    /// convenient mechanism for defining atomicComposition scopes.
    /// 
    /// The least obvious aspects of AtomicComposition deal with nesting.
    /// 
    /// Firstly, no complete actions are actually performed until the outermost atomicComposition is
    /// completed.  Completeting or rolling back nested atomicCompositions serves only to change which
    /// actions would be completed the outer atomicComposition.
    /// 
    /// Secondly, state is added in the form of queries associated with an object key.  The
    /// key represents a unique object the state is being held on behalf of.  The quieries are
    /// accessed throught the Query methods which provide automatic chaining to execute queries
    /// across the target atomicComposition and its inner atomicComposition as appropriate.
    /// 
    /// Lastly, when a nested atomicComposition is created for a given outer the outer atomicComposition is locked.
    /// It remains locked until the inner atomicComposition is disposed or completeed preventing the addition of
    /// state, actions or other inner atomicCompositions.
    /// </summary>
    public class AtomicComposition : IDisposable
    {
        private readonly AtomicComposition _outerAtomicComposition;
        private KeyValuePair<object, object>[] _values;
        private int _valueCount = 0;
        private List<Action> _completeActionList;
        private List<Action> _revertActionList;
        private bool _isDisposed = false;
        private bool _isCompleted = false;
        private bool _containsInnerAtomicComposition = false;

        public AtomicComposition()
            : this(null)
        {
        }

        public AtomicComposition(AtomicComposition outerAtomicComposition)
        {
            // Lock the inner atomicComposition so that we can assume nothing changes except on
            // the innermost scope, and thereby optimize the query path
            if (outerAtomicComposition != null)
            {
                this._outerAtomicComposition = outerAtomicComposition;
                this._outerAtomicComposition.ContainsInnerAtomicComposition = true;
            }
        }

        public void SetValue(object key, object value)
        {
            ThrowIfDisposed();
            ThrowIfCompleted();
            ThrowIfContainsInnerAtomicComposition();

            Requires.NotNull(key, "key");

            SetValueInternal(key, value);
        }

        public bool TryGetValue<T>(object key, out T value) 
        {
            return TryGetValue(key, false, out value);
        }

        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters")]
        public bool TryGetValue<T>(object key, bool localAtomicCompositionOnly, out T value) 
        {
            ThrowIfDisposed();
            ThrowIfCompleted();

            Requires.NotNull(key, "key");

            return TryGetValueInternal(key, localAtomicCompositionOnly, out value);
        }

        public void AddCompleteAction(Action completeAction)
        {
            ThrowIfDisposed();
            ThrowIfCompleted();
            ThrowIfContainsInnerAtomicComposition();

            Requires.NotNull(completeAction, "completeAction");

            if (this._completeActionList == null)
            {
                this._completeActionList = new List<Action>();
            }
            this._completeActionList.Add(completeAction);
        }

        public void AddRevertAction(Action revertAction)
        {
            ThrowIfDisposed();
            ThrowIfCompleted();
            ThrowIfContainsInnerAtomicComposition();

            Requires.NotNull(revertAction, "revertAction");

            if (this._revertActionList == null)
            {
                this._revertActionList = new List<Action>();
            }
            this._revertActionList.Add(revertAction);
        }

        public void Complete()
        {
            ThrowIfDisposed();
            ThrowIfCompleted();

            if (this._outerAtomicComposition == null)
            {   // Execute all the complete actions
                FinalComplete();
            }
            else
            {   // Copy the actions and state to the outer atomicComposition
                CopyComplete();
            }

            this._isCompleted = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            ThrowIfDisposed();
            this._isDisposed = true;

            if (this._outerAtomicComposition != null)
            {
                this._outerAtomicComposition.ContainsInnerAtomicComposition = false;
            }

            // Revert is always immediate and involves forgetting information and
            // exceuting any appropriate revert actions
            if (!this._isCompleted)
            {
                if (this._revertActionList != null)
                {
                    List<Exception> exceptions = null;

                    // Execute the revert actions in reverse order to ensure
                    // everything incrementally rollsback its state.
                    for (int i = this._revertActionList.Count - 1; i >= 0; i--)
                    {
                        Action action = this._revertActionList[i];
                        try
                        {
                            action();
                        }
                        catch(CompositionException)
                        {
                            // This can only happen after preview is completed, so ... abandon remainder of events is correct
                            throw;
                        }
                        catch(Exception e)
                        {
                            if (exceptions == null)
                            {
                                //If any exceptions leak through the actions we will swallow them for now
                                // complete processing the list
                                // and we will throw InvalidOperationException with an AggregateException as it's innerException
                                exceptions = new List<Exception>();
                            }
                            exceptions.Add(e);
                        }
                    }
                    this._revertActionList = null;
                    if(exceptions != null)
                    {
                        throw new InvalidOperationException(SR.InvalidOperation_RevertAndCompleteActionsMustNotThrow, new AggregateException(exceptions));
                    }
                }
            }
        }

        private void FinalComplete()
        {
            // Completeting the outer most scope is easy, just execute all the actions
            if (this._completeActionList != null)
            {
                List<Exception> exceptions = null;

                foreach (Action action in this._completeActionList)
                {
                    try
                    {
                        action();
                    }
                    catch(CompositionException)
                    {
                        // This can only happen after preview is completed, so ... abandon remainder of events is correct
                        throw;
                    }
                    catch(Exception e)
                    {
                        if (exceptions == null)
                        {
                            //If any exceptions leak through the actions we will swallow them for now complete processing the list
                            // and we will throw InvalidOperationException with an AggregateException as it's innerException
                            exceptions = new List<Exception>();
                        }
                        exceptions.Add(e);
                    }
                }
                this._completeActionList = null;
                if(exceptions != null)
                {
                    throw new InvalidOperationException(SR.InvalidOperation_RevertAndCompleteActionsMustNotThrow, new AggregateException(exceptions));
                }
            }
        }

        private void CopyComplete()
        {
            Assumes.NotNull(this._outerAtomicComposition);

            this._outerAtomicComposition.ContainsInnerAtomicComposition = false;

            // Inner scopes are much odder, because completeting them means coalescing them into the
            // outer scope - the complete or revert actions are deferred until the outermost scope completes
            // or any intermediate rolls back
            if (this._completeActionList != null)
            {
                foreach (Action action in this._completeActionList)
                {
                    this._outerAtomicComposition.AddCompleteAction(action);
                }
            }

            if (this._revertActionList != null)
            {
                foreach (Action action in this._revertActionList)
                {
                    this._outerAtomicComposition.AddRevertAction(action);
                }
            }

            // We can copy over existing atomicComposition entries because they're either already chained or
            // overwrite by design and can now be completed or rolled back together
            for (var index = 0; index < this._valueCount; index++)
            {
                this._outerAtomicComposition.SetValueInternal(
                    this._values[index].Key, this._values[index].Value);
            }
        }

        private bool ContainsInnerAtomicComposition
        {
            set
            {
                if (value == true && this._containsInnerAtomicComposition == true)
                {
                    throw new InvalidOperationException(SR.AtomicComposition_AlreadyNested);
                }
                this._containsInnerAtomicComposition = value;
            }
        }

        private bool TryGetValueInternal<T>(object key, bool localAtomicCompositionOnly, out T value) 
        {
            for (var index = 0; index < this._valueCount; index++)
            {
                if (this._values[index].Key == key)
                {
                    value = (T)this._values[index].Value;
                    return true;
                }
            }

            // If there's no atomicComposition available then recurse until we hit the outermost
            // scope, where upon we go ahead and return null
            if (!localAtomicCompositionOnly && this._outerAtomicComposition != null)
            {
                return this._outerAtomicComposition.TryGetValueInternal<T>(key, localAtomicCompositionOnly, out value);
            }

            value = default(T);
            return false;
        }

        private void SetValueInternal(object key, object value)
        {
            // Handle overwrites quickly
            for (var index = 0; index < this._valueCount; index++)
            {
                if (this._values[index].Key == key)
                {
                    this._values[index] = new KeyValuePair<object,object>(key, value);
                    return;
                }
            }

            // Expand storage when needed
            if (this._values == null || this._valueCount == this._values.Length)
            {
                var newQueries = new KeyValuePair<object, object>[this._valueCount == 0 ? 5 : this._valueCount * 2];
                if (this._values != null)
                {
                    Array.Copy(this._values, newQueries, this._valueCount);
                }
                this._values = newQueries;
            }

            // Store a new entry
            this._values[_valueCount] = new KeyValuePair<object, object>(key, value);
            this._valueCount++;
            return;
        }

        [DebuggerStepThrough]
        [ContractArgumentValidator]
        [SuppressMessage("Microsoft.Contracts", "CC1053", Justification = "Suppressing warning because this validator has no public contract")]
        private void ThrowIfContainsInnerAtomicComposition()
        {
            if (this._containsInnerAtomicComposition)
            {
                throw new InvalidOperationException(SR.AtomicComposition_PartOfAnotherAtomicComposition);
            }
        }

        [DebuggerStepThrough]
        [ContractArgumentValidator]
        [SuppressMessage("Microsoft.Contracts", "CC1053", Justification = "Suppressing warning because this validator has no public contract")]
        private void ThrowIfCompleted()
        {
            if (this._isCompleted)
            {
                throw new InvalidOperationException(SR.AtomicComposition_AlreadyCompleted);
            }
        }

        [DebuggerStepThrough]
        [ContractArgumentValidator]
        [SuppressMessage("Microsoft.Contracts", "CC1053", Justification = "Suppressing warning because this validator has no public contract")]
        private void ThrowIfDisposed()
        {
            if (this._isDisposed)
            {
                throw ExceptionBuilder.CreateObjectDisposed(this);
            }
        }
    }
}
