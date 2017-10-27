// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Internal.Collections;

namespace System.ComponentModel.Composition
{
    internal struct CompositionResult<T>
    {
        private readonly IEnumerable<CompositionError> _errors;
        private readonly T _value;
        
        public CompositionResult(T value)
            : this(value, (CompositionError[])null)
        {
        }

        public CompositionResult(params CompositionError[] errors)
            : this(default(T), (IEnumerable<CompositionError>)errors)
        {
        }

        public CompositionResult(IEnumerable<CompositionError> errors)
            : this(default(T), errors)
        {
        }

        internal CompositionResult(T value, IEnumerable<CompositionError> errors)
        {
            this._errors = errors;
            this._value = value;
        }

        public bool Succeeded
        {
            get { return this._errors == null || !this._errors.FastAny(); }
        }

        public IEnumerable<CompositionError> Errors
        {
            get { return this._errors ?? Enumerable.Empty<CompositionError>(); }
        }

        /// <summary>
        ///     Gets the value from the result, throwing a CompositionException if there are any errors.
        /// </summary>
        public T Value
        {
            get 
            {
                ThrowOnErrors();

                return this._value; 
            }
        }

        internal CompositionResult<TValue> ToResult<TValue>()
        {
            return new CompositionResult<TValue>(this._errors);
        }

        internal CompositionResult ToResult()
        {
            return new CompositionResult(this._errors);
        }

        private void ThrowOnErrors()
        {
            if (!this.Succeeded)
            {
                throw new CompositionException(this._errors);
            }
        }
    }
}
