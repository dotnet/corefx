// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Indicates that a switch expression that was non-exhaustive failed to match its input
    /// at runtime, e.g. in the C# 8 expression <code>3 switch { 4 => 5 }</code>.
    /// The exception optionally contains an object representing the unmatched value.
    /// </summary>
    [Serializable]
    [System.Runtime.CompilerServices.TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    public sealed class SwitchExpressionException : InvalidOperationException//, ISerializable
    {
        public SwitchExpressionException()
            : base(SR.Arg_SwitchExpressionException) { }

        public SwitchExpressionException(object unmatchedValue)
            : base()
        {
            UnmatchedValue = unmatchedValue;
        }

        private SwitchExpressionException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
            info.AddValue(nameof(UnmatchedValue), UnmatchedValue, typeof(object));
        }

        public object UnmatchedValue { get; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
           base.GetObjectData(info, context);
            info.AddValue(nameof(UnmatchedValue), UnmatchedValue, typeof(object));
        }
    }
}