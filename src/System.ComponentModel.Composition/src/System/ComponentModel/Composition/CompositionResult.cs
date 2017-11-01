// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Internal.Collections;
using System.ComponentModel.Composition.Hosting;

namespace System.ComponentModel.Composition
{
    internal struct CompositionResult
    {
        public static readonly CompositionResult SucceededResult = new CompositionResult();
        private readonly IEnumerable<CompositionError> _errors;

        public CompositionResult(params CompositionError[] errors)
            : this((IEnumerable<CompositionError>)errors)
        {            
        }

        public CompositionResult(IEnumerable<CompositionError> errors)
        {
            this._errors = errors;
        }

        public bool Succeeded
        {
            get { return this._errors == null || !this._errors.FastAny(); }
        }

        public IEnumerable<CompositionError> Errors
        {
            get { return this._errors ?? Enumerable.Empty<CompositionError>(); }
        }

        public CompositionResult MergeResult(CompositionResult result)
        {
            if (this.Succeeded)
            {
                return result;
            }
            if (result.Succeeded)
            {
                return this;
            }
            return MergeErrors(result._errors);
        }

        public CompositionResult MergeError(CompositionError error)
        {
            return MergeErrors(new CompositionError[] { error });
        }

        public CompositionResult MergeErrors(IEnumerable<CompositionError> errors)
        {
            return new CompositionResult(this._errors.ConcatAllowingNull(errors));
        }

        public CompositionResult<T> ToResult<T>(T value)
        {
            return new CompositionResult<T>(value, this._errors); 
        }

        public void ThrowOnErrors()
        {
            ThrowOnErrors(null);
        }

        public void ThrowOnErrors(AtomicComposition atomicComposition)
        {
            if (!this.Succeeded)
            {
                if (atomicComposition == null)
                {
                    throw new CompositionException(this._errors);
                }
                else
                {
                    throw new ChangeRejectedException(this._errors);
                }
            }
        }
    }
}
