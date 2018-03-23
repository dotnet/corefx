// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

#if !netstandard
using Internal.Runtime.CompilerServices;
#endif

namespace System.Runtime.CompilerServices
{
    /// <summary>Provides an awaitable type that enables configured awaits on a <see cref="ValueTask"/>.</summary>
    [StructLayout(LayoutKind.Auto)]
    public readonly struct ConfiguredValueTaskAwaitable
    {
        /// <summary>The wrapped <see cref="Task"/>.</summary>
        private readonly ValueTask _value;

        /// <summary>Initializes the awaitable.</summary>
        /// <param name="value">The wrapped <see cref="ValueTask"/>.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ConfiguredValueTaskAwaitable(ValueTask value) => _value = value;

        /// <summary>Returns an awaiter for this <see cref="ConfiguredValueTaskAwaitable"/> instance.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ConfiguredValueTaskAwaiter GetAwaiter() => new ConfiguredValueTaskAwaiter(_value);

        /// <summary>Provides an awaiter for a <see cref="ConfiguredValueTaskAwaitable"/>.</summary>
        [StructLayout(LayoutKind.Auto)]
        public readonly struct ConfiguredValueTaskAwaiter : ICriticalNotifyCompletion
#if CORECLR
            , IValueTaskAwaiter
#endif
        {
            /// <summary>The value being awaited.</summary>
            private readonly ValueTask _value;

            /// <summary>Initializes the awaiter.</summary>
            /// <param name="value">The value to be awaited.</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal ConfiguredValueTaskAwaiter(ValueTask value) => _value = value;

            /// <summary>Gets whether the <see cref="ConfiguredValueTaskAwaitable"/> has completed.</summary>
            public bool IsCompleted
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _value.IsCompleted;
            }

            /// <summary>Gets the result of the ValueTask.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [StackTraceHidden]
            public void GetResult() => _value.ThrowIfCompletedUnsuccessfully();

            /// <summary>Schedules the continuation action for the <see cref="ConfiguredValueTaskAwaitable"/>.</summary>
            public void OnCompleted(Action continuation)
            {
                if (_value.ObjectIsTask)
                {
                    _value.UnsafeGetTask().ConfigureAwait(_value.ContinueOnCapturedContext).GetAwaiter().OnCompleted(continuation);
                }
                else if (_value._obj != null)
                {
                    _value.UnsafeGetValueTaskSource().OnCompleted(ValueTaskAwaiter.s_invokeActionDelegate, continuation, _value._token,
                        ValueTaskSourceOnCompletedFlags.FlowExecutionContext |
                            (_value.ContinueOnCapturedContext ? ValueTaskSourceOnCompletedFlags.UseSchedulingContext : ValueTaskSourceOnCompletedFlags.None));
                }
                else
                {
                    ValueTask.CompletedTask.ConfigureAwait(_value.ContinueOnCapturedContext).GetAwaiter().OnCompleted(continuation);
                }
            }

            /// <summary>Schedules the continuation action for the <see cref="ConfiguredValueTaskAwaitable"/>.</summary>
            public void UnsafeOnCompleted(Action continuation)
            {
                if (_value.ObjectIsTask)
                {
                    _value.UnsafeGetTask().ConfigureAwait(_value.ContinueOnCapturedContext).GetAwaiter().UnsafeOnCompleted(continuation);
                }
                else if (_value._obj != null)
                {
                    _value.UnsafeGetValueTaskSource().OnCompleted(ValueTaskAwaiter.s_invokeActionDelegate, continuation, _value._token,
                        _value.ContinueOnCapturedContext ? ValueTaskSourceOnCompletedFlags.UseSchedulingContext : ValueTaskSourceOnCompletedFlags.None);
                }
                else
                {
                    ValueTask.CompletedTask.ConfigureAwait(_value.ContinueOnCapturedContext).GetAwaiter().UnsafeOnCompleted(continuation);
                }
            }

#if CORECLR
            void IValueTaskAwaiter.AwaitUnsafeOnCompleted(IAsyncStateMachineBox box)
            {
                if (_value.ObjectIsTask)
                {
                    TaskAwaiter.UnsafeOnCompletedInternal(_value.UnsafeGetTask(), box, _value.ContinueOnCapturedContext);
                }
                else if (_value._obj != null)
                {
                    _value.UnsafeGetValueTaskSource().OnCompleted(ValueTaskAwaiter.s_invokeAsyncStateMachineBox, box, _value._token,
                        _value.ContinueOnCapturedContext ? ValueTaskSourceOnCompletedFlags.UseSchedulingContext : ValueTaskSourceOnCompletedFlags.None);
                }
                else
                {
                    TaskAwaiter.UnsafeOnCompletedInternal(Task.CompletedTask, box, _value.ContinueOnCapturedContext);
                }
            }
#endif
        }
    }

    /// <summary>Provides an awaitable type that enables configured awaits on a <see cref="ValueTask{TResult}"/>.</summary>
    /// <typeparam name="TResult">The type of the result produced.</typeparam>
    [StructLayout(LayoutKind.Auto)]
    public readonly struct ConfiguredValueTaskAwaitable<TResult>
    {
        /// <summary>The wrapped <see cref="ValueTask{TResult}"/>.</summary>
        private readonly ValueTask<TResult> _value;

        /// <summary>Initializes the awaitable.</summary>
        /// <param name="value">The wrapped <see cref="ValueTask{TResult}"/>.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ConfiguredValueTaskAwaitable(ValueTask<TResult> value) => _value = value;

        /// <summary>Returns an awaiter for this <see cref="ConfiguredValueTaskAwaitable{TResult}"/> instance.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ConfiguredValueTaskAwaiter GetAwaiter() => new ConfiguredValueTaskAwaiter(_value);

        /// <summary>Provides an awaiter for a <see cref="ConfiguredValueTaskAwaitable{TResult}"/>.</summary>
        [StructLayout(LayoutKind.Auto)]
        public readonly struct ConfiguredValueTaskAwaiter : ICriticalNotifyCompletion
#if CORECLR
            , IValueTaskAwaiter
#endif
        {
            /// <summary>The value being awaited.</summary>
            private readonly ValueTask<TResult> _value;

            /// <summary>Initializes the awaiter.</summary>
            /// <param name="value">The value to be awaited.</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal ConfiguredValueTaskAwaiter(ValueTask<TResult> value) => _value = value;

            /// <summary>Gets whether the <see cref="ConfiguredValueTaskAwaitable{TResult}"/> has completed.</summary>
            public bool IsCompleted
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _value.IsCompleted;
            }

            /// <summary>Gets the result of the ValueTask.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [StackTraceHidden]
            public TResult GetResult() => _value.Result;

            /// <summary>Schedules the continuation action for the <see cref="ConfiguredValueTaskAwaitable{TResult}"/>.</summary>
            public void OnCompleted(Action continuation)
            {
                if (_value.ObjectIsTask)
                {
                    _value.UnsafeGetTask().ConfigureAwait(_value.ContinueOnCapturedContext).GetAwaiter().OnCompleted(continuation);
                }
                else if (_value._obj != null)
                {
                    _value.UnsafeGetValueTaskSource().OnCompleted(ValueTaskAwaiter.s_invokeActionDelegate, continuation, _value._token,
                        ValueTaskSourceOnCompletedFlags.FlowExecutionContext |
                            (_value.ContinueOnCapturedContext ? ValueTaskSourceOnCompletedFlags.UseSchedulingContext : ValueTaskSourceOnCompletedFlags.None));
                }
                else
                {
                    ValueTask.CompletedTask.ConfigureAwait(_value.ContinueOnCapturedContext).GetAwaiter().OnCompleted(continuation);
                }
            }

            /// <summary>Schedules the continuation action for the <see cref="ConfiguredValueTaskAwaitable{TResult}"/>.</summary>
            public void UnsafeOnCompleted(Action continuation)
            {
                if (_value.ObjectIsTask)
                {
                    _value.UnsafeGetTask().ConfigureAwait(_value.ContinueOnCapturedContext).GetAwaiter().UnsafeOnCompleted(continuation);
                }
                else if (_value._obj != null)
                {
                    _value.UnsafeGetValueTaskSource().OnCompleted(ValueTaskAwaiter.s_invokeActionDelegate, continuation, _value._token,
                        _value.ContinueOnCapturedContext ? ValueTaskSourceOnCompletedFlags.UseSchedulingContext : ValueTaskSourceOnCompletedFlags.None);
                }
                else
                {
                    ValueTask.CompletedTask.ConfigureAwait(_value.ContinueOnCapturedContext).GetAwaiter().UnsafeOnCompleted(continuation);
                }
            }

#if CORECLR
            void IValueTaskAwaiter.AwaitUnsafeOnCompleted(IAsyncStateMachineBox box)
            {
                if (_value.ObjectIsTask)
                {
                    TaskAwaiter.UnsafeOnCompletedInternal(_value.UnsafeGetTask(), box, _value.ContinueOnCapturedContext);
                }
                else if (_value._obj != null)
                {
                    _value.UnsafeGetValueTaskSource().OnCompleted(ValueTaskAwaiter.s_invokeAsyncStateMachineBox, box, _value._token,
                        _value.ContinueOnCapturedContext ? ValueTaskSourceOnCompletedFlags.UseSchedulingContext : ValueTaskSourceOnCompletedFlags.None);
                }
                else
                {
                    TaskAwaiter.UnsafeOnCompletedInternal(Task.CompletedTask, box, _value.ContinueOnCapturedContext);
                }
            }
#endif
        }
    }
}
