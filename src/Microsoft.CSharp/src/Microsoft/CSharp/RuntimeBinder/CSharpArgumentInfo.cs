// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;

namespace Microsoft.CSharp.RuntimeBinder
{
    /// <summary>
    /// Represents information about C# dynamic operations that are specific to particular arguments at a call site.
    /// Instances of this class are generated by the C# compiler.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class CSharpArgumentInfo
    {
        // Create a singleton static instance.
        internal static readonly CSharpArgumentInfo None = new CSharpArgumentInfo(CSharpArgumentInfoFlags.None, null);

        /// <summary>
        /// The flags for the argument.
        /// </summary>
        internal CSharpArgumentInfoFlags Flags { get { return _flags; } }
        private CSharpArgumentInfoFlags _flags;

        /// <summary>
        /// The name of the argument, if named; otherwise null.
        /// </summary>
        internal string Name { get { return _name; } }
        private string _name;

        private CSharpArgumentInfo(CSharpArgumentInfoFlags flags, string name)
        {
            _flags = flags;
            _name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpArgumentInfo"/> class.
        /// </summary>
        /// <param name="flags">The flags for the argument.</param>
        /// <param name="name">The name of the argument, if named; otherwise null.</param>
        public static CSharpArgumentInfo Create(CSharpArgumentInfoFlags flags, string name)
        {
            return new CSharpArgumentInfo(flags, name);
        }

        // Accessor helpers.
        internal bool UseCompileTimeType { get { return (Flags & CSharpArgumentInfoFlags.UseCompileTimeType) != 0; } }
        internal bool LiteralConstant { get { return (Flags & CSharpArgumentInfoFlags.Constant) != 0; } }
        internal bool NamedArgument { get { return (Flags & CSharpArgumentInfoFlags.NamedArgument) != 0; } }
        internal bool IsByRef { get { return (Flags & CSharpArgumentInfoFlags.IsRef) != 0; } }
        internal bool IsOut { get { return (Flags & CSharpArgumentInfoFlags.IsOut) != 0; } }
        internal bool IsStaticType { get { return (Flags & CSharpArgumentInfoFlags.IsStaticType) != 0; } }
    }
}
