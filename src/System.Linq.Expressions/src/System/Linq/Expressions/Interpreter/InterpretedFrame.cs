// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Dynamic.Utils;

namespace System.Linq.Expressions.Interpreter
{
    internal sealed class InterpretedFrame
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        [ThreadStatic]
        public static InterpretedFrame CurrentFrame;

        internal readonly Interpreter Interpreter;
        internal InterpretedFrame _parent;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2105:ArrayFieldsShouldNotBeReadOnly")]
        private int[] _continuations;
        private int _continuationIndex;
        private int _pendingContinuation;
        private object _pendingValue;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2105:ArrayFieldsShouldNotBeReadOnly")]
        public readonly object[] Data;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2105:ArrayFieldsShouldNotBeReadOnly")]
        public readonly IStrongBox[] Closure;

        public int StackIndex;
        public int InstructionIndex;

#if FEATURE_THREAD_ABORT
        // When a ThreadAbortException is raised from interpreted code this is the first frame that caught it.
        // No handlers within this handler re-abort the current thread when left.
        public ExceptionHandler CurrentAbortHandler;
#endif

        internal InterpretedFrame(Interpreter interpreter, IStrongBox[] closure)
        {
            Interpreter = interpreter;
            StackIndex = interpreter.LocalCount;
            Data = new object[StackIndex + interpreter.Instructions.MaxStackDepth];

            int c = interpreter.Instructions.MaxContinuationDepth;
            if (c > 0)
            {
                _continuations = new int[c];
            }

            Closure = closure;

            _pendingContinuation = -1;
            _pendingValue = Interpreter.NoValue;
        }

        public DebugInfo GetDebugInfo(int instructionIndex)
        {
            return DebugInfo.GetMatchingDebugInfo(Interpreter._debugInfos, instructionIndex);
        }

        public string Name
        {
            get { return Interpreter._name; }
        }

        #region Data Stack Operations

        public void Push(object value)
        {
            Data[StackIndex++] = value;
        }

        public void Push(bool value)
        {
            Data[StackIndex++] = value ? ScriptingRuntimeHelpers.True : ScriptingRuntimeHelpers.False;
        }

        public void Push(int value)
        {
            Data[StackIndex++] = ScriptingRuntimeHelpers.Int32ToObject(value);
        }

        public void Push(byte value)
        {
            Data[StackIndex++] = value;
        }

        public void Push(sbyte value)
        {
            Data[StackIndex++] = value;
        }

        public void Push(Int16 value)
        {
            Data[StackIndex++] = value;
        }

        public void Push(UInt16 value)
        {
            Data[StackIndex++] = value;
        }

        public object Pop()
        {
            return Data[--StackIndex];
        }

        internal void SetStackDepth(int depth)
        {
            StackIndex = Interpreter.LocalCount + depth;
        }

        public object Peek()
        {
            return Data[StackIndex - 1];
        }

        public void Dup()
        {
            int i = StackIndex;
            Data[i] = Data[i - 1];
            StackIndex = i + 1;
        }

        #endregion

        #region Stack Trace

        public InterpretedFrame Parent
        {
            get { return _parent; }
        }

        public static bool IsInterpretedFrame(MethodBase method)
        {
            //ContractUtils.RequiresNotNull(method, "method");
            return method.DeclaringType == typeof(Interpreter) && method.Name == "Run";
        }

        public IEnumerable<InterpretedFrameInfo> GetStackTraceDebugInfo()
        {
            var frame = this;
            do
            {
                yield return new InterpretedFrameInfo(frame.Name, frame.GetDebugInfo(frame.InstructionIndex));
                frame = frame.Parent;
            } while (frame != null);
        }

        internal void SaveTraceToException(Exception exception)
        {
            if (exception.Data[typeof(InterpretedFrameInfo)] == null)
            {
                exception.Data[typeof(InterpretedFrameInfo)] = new List<InterpretedFrameInfo>(GetStackTraceDebugInfo()).ToArray();
            }
        }

        public static InterpretedFrameInfo[] GetExceptionStackTrace(Exception exception)
        {
            return exception.Data[typeof(InterpretedFrameInfo)] as InterpretedFrameInfo[];
        }

#if DEBUG
        internal string[] Trace
        {
            get
            {
                var trace = new List<string>();
                var frame = this;
                do
                {
                    trace.Add(frame.Name);
                    frame = frame.Parent;
                } while (frame != null);
                return trace.ToArray();
            }
        }
#endif

        internal InterpretedFrame Enter()
        {
            var currentFrame = CurrentFrame;
            CurrentFrame = this;
            return _parent = currentFrame;
        }

        internal void Leave(InterpretedFrame prevFrame)
        {
            CurrentFrame = prevFrame;
        }

        #endregion

        #region Continuations

        internal bool IsJumpHappened()
        {
            return _pendingContinuation >= 0;
        }

        public void RemoveContinuation()
        {
            _continuationIndex--;
        }

        public void PushContinuation(int continuation)
        {
            _continuations[_continuationIndex++] = continuation;
        }

        public int YieldToCurrentContinuation()
        {
            var target = Interpreter._labels[_continuations[_continuationIndex - 1]];
            SetStackDepth(target.StackDepth);
            return target.Index - InstructionIndex;
        }

        /// <summary>
        /// Get called from the LeaveFinallyInstruction
        /// </summary>
        public int YieldToPendingContinuation()
        {
            Debug.Assert(_pendingContinuation >= 0);
            RuntimeLabel pendingTarget = Interpreter._labels[_pendingContinuation];

            // the current continuation might have higher priority (continuationIndex is the depth of the current continuation):
            if (pendingTarget.ContinuationStackDepth < _continuationIndex)
            {
                RuntimeLabel currentTarget = Interpreter._labels[_continuations[_continuationIndex - 1]];
                SetStackDepth(currentTarget.StackDepth);
                return currentTarget.Index - InstructionIndex;
            }

            SetStackDepth(pendingTarget.StackDepth);
            if (_pendingValue != Interpreter.NoValue)
            {
                Data[StackIndex - 1] = _pendingValue;
            }

            // Set the _pendingContinuation and _pendingValue to the default values if we finally gets to the Goto target
            _pendingContinuation = -1;
            _pendingValue = Interpreter.NoValue;
            return pendingTarget.Index - InstructionIndex;
        }

        internal void PushPendingContinuation()
        {
            Push(_pendingContinuation);
            Push(_pendingValue);

            _pendingContinuation = -1;
            _pendingValue = Interpreter.NoValue;
        }

        internal void PopPendingContinuation()
        {
            _pendingValue = Pop();
            _pendingContinuation = (int)Pop();
        }

        public int Goto(int labelIndex, object value, bool gotoExceptionHandler)
        {
            // TODO: we know this at compile time (except for compiled loop):
            RuntimeLabel target = Interpreter._labels[labelIndex];
            Debug.Assert(!gotoExceptionHandler || (gotoExceptionHandler && _continuationIndex == target.ContinuationStackDepth),
                "When it's time to jump to the exception handler, all previous finally blocks should already be processed");

            if (_continuationIndex == target.ContinuationStackDepth)
            {
                SetStackDepth(target.StackDepth);
                if (value != Interpreter.NoValue)
                {
                    Data[StackIndex - 1] = value;
                }
                return target.Index - InstructionIndex;
            }

            // if we are in the middle of executing jump we forget the previous target and replace it by a new one:
            _pendingContinuation = labelIndex;
            _pendingValue = value;
            return YieldToCurrentContinuation();
        }
        #endregion

    }
}
