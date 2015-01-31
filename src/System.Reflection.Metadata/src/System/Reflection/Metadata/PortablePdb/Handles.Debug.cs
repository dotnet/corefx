// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;

namespace System.Reflection.Metadata
{
    public struct DocumentHandle : IEquatable<DocumentHandle>
    {
        private const uint tokenType = TokenTypeIds.Document;
        private readonly uint rowId;

        private DocumentHandle(uint rowId)
        {
            Debug.Assert(TokenTypeIds.IsValidRowId(rowId));
            this.rowId = rowId;
        }

        internal static DocumentHandle FromRowId(uint rowId)
        {
            return new DocumentHandle(rowId);
        }

        public static implicit operator Handle(DocumentHandle handle)
        {
            return new Handle(handle.rowId | tokenType);
        }

        public static explicit operator DocumentHandle(Handle handle)
        {
            if (handle.TokenType != tokenType)
            {
                Handle.ThrowInvalidCast();
            }

            return new DocumentHandle(handle.RowId);
        }

        public bool IsNil
        {
            get
            {
                return RowId == 0;
            }
        }

        internal uint RowId { get { return rowId; } }

        public static bool operator ==(DocumentHandle left, DocumentHandle right)
        {
            return left.rowId == right.rowId;
        }

        public override bool Equals(object obj)
        {
            return obj is DocumentHandle && ((DocumentHandle)obj).rowId == this.rowId;
        }

        public bool Equals(DocumentHandle other)
        {
            return this.rowId == other.rowId;
        }

        public override int GetHashCode()
        {
            return this.rowId.GetHashCode();
        }

        public static bool operator !=(DocumentHandle left, DocumentHandle right)
        {
            return left.rowId != right.rowId;
        }
    }

    public struct MethodBodyHandle : IEquatable<MethodBodyHandle>
    {
        private const uint tokenType = TokenTypeIds.MethodBody;
        private readonly uint rowId;

        private MethodBodyHandle(uint rowId)
        {
            Debug.Assert(TokenTypeIds.IsValidRowId(rowId));
            this.rowId = rowId;
        }

        internal static MethodBodyHandle FromRowId(uint rowId)
        {
            return new MethodBodyHandle(rowId);
        }

        public static implicit operator Handle(MethodBodyHandle handle)
        {
            return new Handle(handle.rowId | tokenType);
        }

        public static explicit operator MethodBodyHandle(Handle handle)
        {
            if (handle.TokenType != tokenType)
            {
                Handle.ThrowInvalidCast();
            }

            return new MethodBodyHandle(handle.RowId);
        }

        public bool IsNil
        {
            get
            {
                return RowId == 0;
            }
        }

        internal uint RowId { get { return rowId; } }

        public static bool operator ==(MethodBodyHandle left, MethodBodyHandle right)
        {
            return left.rowId == right.rowId;
        }

        public override bool Equals(object obj)
        {
            return obj is MethodBodyHandle && ((MethodBodyHandle)obj).rowId == this.rowId;
        }

        public bool Equals(MethodBodyHandle other)
        {
            return this.rowId == other.rowId;
        }

        public override int GetHashCode()
        {
            return this.rowId.GetHashCode();
        }

        public static bool operator !=(MethodBodyHandle left, MethodBodyHandle right)
        {
            return left.rowId != right.rowId;
        }
    }

    public struct LocalScopeHandle : IEquatable<LocalScopeHandle>
    {
        private const uint tokenType = TokenTypeIds.LocalScope;
        private readonly uint rowId;

        private LocalScopeHandle(uint rowId)
        {
            Debug.Assert(TokenTypeIds.IsValidRowId(rowId));
            this.rowId = rowId;
        }

        internal static LocalScopeHandle FromRowId(uint rowId)
        {
            return new LocalScopeHandle(rowId);
        }

        public static implicit operator Handle(LocalScopeHandle handle)
        {
            return new Handle(handle.rowId | tokenType);
        }

        public static explicit operator LocalScopeHandle(Handle handle)
        {
            if (handle.TokenType != tokenType)
            {
                Handle.ThrowInvalidCast();
            }

            return new LocalScopeHandle(handle.RowId);
        }

        public bool IsNil
        {
            get
            {
                return RowId == 0;
            }
        }

        internal uint RowId { get { return rowId; } }

        public static bool operator ==(LocalScopeHandle left, LocalScopeHandle right)
        {
            return left.rowId == right.rowId;
        }

        public override bool Equals(object obj)
        {
            return obj is LocalScopeHandle && ((LocalScopeHandle)obj).rowId == this.rowId;
        }

        public bool Equals(LocalScopeHandle other)
        {
            return this.rowId == other.rowId;
        }

        public override int GetHashCode()
        {
            return this.rowId.GetHashCode();
        }

        public static bool operator !=(LocalScopeHandle left, LocalScopeHandle right)
        {
            return left.rowId != right.rowId;
        }
    }

    public struct LocalVariableHandle : IEquatable<LocalVariableHandle>
    {
        private const uint tokenType = TokenTypeIds.LocalVariable;
        private readonly uint rowId;

        private LocalVariableHandle(uint rowId)
        {
            Debug.Assert(TokenTypeIds.IsValidRowId(rowId));
            this.rowId = rowId;
        }

        internal static LocalVariableHandle FromRowId(uint rowId)
        {
            return new LocalVariableHandle(rowId);
        }

        public static implicit operator Handle(LocalVariableHandle handle)
        {
            return new Handle(handle.rowId | tokenType);
        }

        public static explicit operator LocalVariableHandle(Handle handle)
        {
            if (handle.TokenType != tokenType)
            {
                Handle.ThrowInvalidCast();
            }

            return new LocalVariableHandle(handle.RowId);
        }

        public bool IsNil
        {
            get
            {
                return RowId == 0;
            }
        }

        internal uint RowId { get { return rowId; } }

        public static bool operator ==(LocalVariableHandle left, LocalVariableHandle right)
        {
            return left.rowId == right.rowId;
        }

        public override bool Equals(object obj)
        {
            return obj is LocalVariableHandle && ((LocalVariableHandle)obj).rowId == this.rowId;
        }

        public bool Equals(LocalVariableHandle other)
        {
            return this.rowId == other.rowId;
        }

        public override int GetHashCode()
        {
            return this.rowId.GetHashCode();
        }

        public static bool operator !=(LocalVariableHandle left, LocalVariableHandle right)
        {
            return left.rowId != right.rowId;
        }
    }

    public struct LocalConstantHandle : IEquatable<LocalConstantHandle>
    {
        private const uint tokenType = TokenTypeIds.LocalConstant;
        private readonly uint rowId;

        private LocalConstantHandle(uint rowId)
        {
            Debug.Assert(TokenTypeIds.IsValidRowId(rowId));
            this.rowId = rowId;
        }

        internal static LocalConstantHandle FromRowId(uint rowId)
        {
            return new LocalConstantHandle(rowId);
        }

        public static implicit operator Handle(LocalConstantHandle handle)
        {
            return new Handle(handle.rowId | tokenType);
        }

        public static explicit operator LocalConstantHandle(Handle handle)
        {
            if (handle.TokenType != tokenType)
            {
                Handle.ThrowInvalidCast();
            }

            return new LocalConstantHandle(handle.RowId);
        }

        public bool IsNil
        {
            get
            {
                return RowId == 0;
            }
        }

        internal uint RowId { get { return rowId; } }

        public static bool operator ==(LocalConstantHandle left, LocalConstantHandle right)
        {
            return left.rowId == right.rowId;
        }

        public override bool Equals(object obj)
        {
            return obj is LocalConstantHandle && ((LocalConstantHandle)obj).rowId == this.rowId;
        }

        public bool Equals(LocalConstantHandle other)
        {
            return this.rowId == other.rowId;
        }

        public override int GetHashCode()
        {
            return this.rowId.GetHashCode();
        }

        public static bool operator !=(LocalConstantHandle left, LocalConstantHandle right)
        {
            return left.rowId != right.rowId;
        }
    }

    public struct ImportScopeHandle : IEquatable<ImportScopeHandle>
    {
        private const uint tokenType = TokenTypeIds.ImportScope;
        private readonly uint rowId;

        private ImportScopeHandle(uint rowId)
        {
            Debug.Assert(TokenTypeIds.IsValidRowId(rowId));
            this.rowId = rowId;
        }

        internal static ImportScopeHandle FromRowId(uint rowId)
        {
            return new ImportScopeHandle(rowId);
        }

        public static implicit operator Handle(ImportScopeHandle handle)
        {
            return new Handle(handle.rowId | tokenType);
        }

        public static explicit operator ImportScopeHandle(Handle handle)
        {
            if (handle.TokenType != tokenType)
            {
                Handle.ThrowInvalidCast();
            }

            return new ImportScopeHandle(handle.RowId);
        }

        public bool IsNil
        {
            get
            {
                return RowId == 0;
            }
        }

        internal uint RowId { get { return rowId; } }

        public static bool operator ==(ImportScopeHandle left, ImportScopeHandle right)
        {
            return left.rowId == right.rowId;
        }

        public override bool Equals(object obj)
        {
            return obj is ImportScopeHandle && ((ImportScopeHandle)obj).rowId == this.rowId;
        }

        public bool Equals(ImportScopeHandle other)
        {
            return this.rowId == other.rowId;
        }

        public override int GetHashCode()
        {
            return this.rowId.GetHashCode();
        }

        public static bool operator !=(ImportScopeHandle left, ImportScopeHandle right)
        {
            return left.rowId != right.rowId;
        }
    }

    public struct AsyncMethodHandle : IEquatable<AsyncMethodHandle>
    {
        private const uint tokenType = TokenTypeIds.AsyncMethod;
        private readonly uint rowId;

        private AsyncMethodHandle(uint rowId)
        {
            Debug.Assert(TokenTypeIds.IsValidRowId(rowId));
            this.rowId = rowId;
        }

        internal static AsyncMethodHandle FromRowId(uint rowId)
        {
            return new AsyncMethodHandle(rowId);
        }

        public static implicit operator Handle(AsyncMethodHandle handle)
        {
            return new Handle(handle.rowId | tokenType);
        }

        public static explicit operator AsyncMethodHandle(Handle handle)
        {
            if (handle.TokenType != tokenType)
            {
                Handle.ThrowInvalidCast();
            }

            return new AsyncMethodHandle(handle.RowId);
        }

        public bool IsNil
        {
            get
            {
                return RowId == 0;
            }
        }

        internal uint RowId { get { return rowId; } }

        public static bool operator ==(AsyncMethodHandle left, AsyncMethodHandle right)
        {
            return left.rowId == right.rowId;
        }

        public override bool Equals(object obj)
        {
            return obj is AsyncMethodHandle && ((AsyncMethodHandle)obj).rowId == this.rowId;
        }

        public bool Equals(AsyncMethodHandle other)
        {
            return this.rowId == other.rowId;
        }

        public override int GetHashCode()
        {
            return this.rowId.GetHashCode();
        }

        public static bool operator !=(AsyncMethodHandle left, AsyncMethodHandle right)
        {
            return left.rowId != right.rowId;
        }
    }

    public struct CustomDebugInformationHandle : IEquatable<CustomDebugInformationHandle>
    {
        private const uint tokenType = TokenTypeIds.CustomDebugInformation;
        private readonly uint rowId;

        private CustomDebugInformationHandle(uint rowId)
        {
            Debug.Assert(TokenTypeIds.IsValidRowId(rowId));
            this.rowId = rowId;
        }

        internal static CustomDebugInformationHandle FromRowId(uint rowId)
        {
            return new CustomDebugInformationHandle(rowId);
        }

        public static implicit operator Handle(CustomDebugInformationHandle handle)
        {
            return new Handle(handle.rowId | tokenType);
        }

        public static explicit operator CustomDebugInformationHandle(Handle handle)
        {
            if (handle.TokenType != tokenType)
            {
                Handle.ThrowInvalidCast();
            }

            return new CustomDebugInformationHandle(handle.RowId);
        }

        public bool IsNil
        {
            get
            {
                return RowId == 0;
            }
        }

        internal uint RowId { get { return rowId; } }

        public static bool operator ==(CustomDebugInformationHandle left, CustomDebugInformationHandle right)
        {
            return left.rowId == right.rowId;
        }

        public override bool Equals(object obj)
        {
            return obj is CustomDebugInformationHandle && ((CustomDebugInformationHandle)obj).rowId == this.rowId;
        }

        public bool Equals(CustomDebugInformationHandle other)
        {
            return this.rowId == other.rowId;
        }

        public override int GetHashCode()
        {
            return this.rowId.GetHashCode();
        }

        public static bool operator !=(CustomDebugInformationHandle left, CustomDebugInformationHandle right)
        {
            return left.rowId != right.rowId;
        }
    }
}
