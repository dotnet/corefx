// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;

namespace System.Reflection.Metadata
{
    public struct DocumentHandle : IEquatable<DocumentHandle>
    {
        private const uint tokenType = TokenTypeIds.Document;
        private readonly uint _rowId;

        private DocumentHandle(uint rowId)
        {
            Debug.Assert(TokenTypeIds.IsValidRowId(rowId));
            _rowId = rowId;
        }

        internal static DocumentHandle FromRowId(uint rowId)
        {
            return new DocumentHandle(rowId);
        }

        public static implicit operator Handle(DocumentHandle handle)
        {
            return new Handle(handle._rowId | tokenType);
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

        internal uint RowId { get { return _rowId; } }

        public static bool operator ==(DocumentHandle left, DocumentHandle right)
        {
            return left._rowId == right._rowId;
        }

        public override bool Equals(object obj)
        {
            return obj is DocumentHandle && ((DocumentHandle)obj)._rowId == _rowId;
        }

        public bool Equals(DocumentHandle other)
        {
            return _rowId == other._rowId;
        }

        public override int GetHashCode()
        {
            return _rowId.GetHashCode();
        }

        public static bool operator !=(DocumentHandle left, DocumentHandle right)
        {
            return left._rowId != right._rowId;
        }
    }

    public struct MethodBodyHandle : IEquatable<MethodBodyHandle>
    {
        private const uint tokenType = TokenTypeIds.MethodBody;
        private readonly uint _rowId;

        private MethodBodyHandle(uint rowId)
        {
            Debug.Assert(TokenTypeIds.IsValidRowId(rowId));
            _rowId = rowId;
        }

        internal static MethodBodyHandle FromRowId(uint rowId)
        {
            return new MethodBodyHandle(rowId);
        }

        public static implicit operator Handle(MethodBodyHandle handle)
        {
            return new Handle(handle._rowId | tokenType);
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

        internal uint RowId { get { return _rowId; } }

        public static bool operator ==(MethodBodyHandle left, MethodBodyHandle right)
        {
            return left._rowId == right._rowId;
        }

        public override bool Equals(object obj)
        {
            return obj is MethodBodyHandle && ((MethodBodyHandle)obj)._rowId == _rowId;
        }

        public bool Equals(MethodBodyHandle other)
        {
            return _rowId == other._rowId;
        }

        public override int GetHashCode()
        {
            return _rowId.GetHashCode();
        }

        public static bool operator !=(MethodBodyHandle left, MethodBodyHandle right)
        {
            return left._rowId != right._rowId;
        }
    }

    public struct LocalScopeHandle : IEquatable<LocalScopeHandle>
    {
        private const uint tokenType = TokenTypeIds.LocalScope;
        private readonly uint _rowId;

        private LocalScopeHandle(uint rowId)
        {
            Debug.Assert(TokenTypeIds.IsValidRowId(rowId));
            _rowId = rowId;
        }

        internal static LocalScopeHandle FromRowId(uint rowId)
        {
            return new LocalScopeHandle(rowId);
        }

        public static implicit operator Handle(LocalScopeHandle handle)
        {
            return new Handle(handle._rowId | tokenType);
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

        internal uint RowId { get { return _rowId; } }

        public static bool operator ==(LocalScopeHandle left, LocalScopeHandle right)
        {
            return left._rowId == right._rowId;
        }

        public override bool Equals(object obj)
        {
            return obj is LocalScopeHandle && ((LocalScopeHandle)obj)._rowId == _rowId;
        }

        public bool Equals(LocalScopeHandle other)
        {
            return _rowId == other._rowId;
        }

        public override int GetHashCode()
        {
            return _rowId.GetHashCode();
        }

        public static bool operator !=(LocalScopeHandle left, LocalScopeHandle right)
        {
            return left._rowId != right._rowId;
        }
    }

    public struct LocalVariableHandle : IEquatable<LocalVariableHandle>
    {
        private const uint tokenType = TokenTypeIds.LocalVariable;
        private readonly uint _rowId;

        private LocalVariableHandle(uint rowId)
        {
            Debug.Assert(TokenTypeIds.IsValidRowId(rowId));
            _rowId = rowId;
        }

        internal static LocalVariableHandle FromRowId(uint rowId)
        {
            return new LocalVariableHandle(rowId);
        }

        public static implicit operator Handle(LocalVariableHandle handle)
        {
            return new Handle(handle._rowId | tokenType);
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

        internal uint RowId { get { return _rowId; } }

        public static bool operator ==(LocalVariableHandle left, LocalVariableHandle right)
        {
            return left._rowId == right._rowId;
        }

        public override bool Equals(object obj)
        {
            return obj is LocalVariableHandle && ((LocalVariableHandle)obj)._rowId == _rowId;
        }

        public bool Equals(LocalVariableHandle other)
        {
            return _rowId == other._rowId;
        }

        public override int GetHashCode()
        {
            return _rowId.GetHashCode();
        }

        public static bool operator !=(LocalVariableHandle left, LocalVariableHandle right)
        {
            return left._rowId != right._rowId;
        }
    }

    public struct LocalConstantHandle : IEquatable<LocalConstantHandle>
    {
        private const uint tokenType = TokenTypeIds.LocalConstant;
        private readonly uint _rowId;

        private LocalConstantHandle(uint rowId)
        {
            Debug.Assert(TokenTypeIds.IsValidRowId(rowId));
            _rowId = rowId;
        }

        internal static LocalConstantHandle FromRowId(uint rowId)
        {
            return new LocalConstantHandle(rowId);
        }

        public static implicit operator Handle(LocalConstantHandle handle)
        {
            return new Handle(handle._rowId | tokenType);
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

        internal uint RowId { get { return _rowId; } }

        public static bool operator ==(LocalConstantHandle left, LocalConstantHandle right)
        {
            return left._rowId == right._rowId;
        }

        public override bool Equals(object obj)
        {
            return obj is LocalConstantHandle && ((LocalConstantHandle)obj)._rowId == _rowId;
        }

        public bool Equals(LocalConstantHandle other)
        {
            return _rowId == other._rowId;
        }

        public override int GetHashCode()
        {
            return _rowId.GetHashCode();
        }

        public static bool operator !=(LocalConstantHandle left, LocalConstantHandle right)
        {
            return left._rowId != right._rowId;
        }
    }

    public struct ImportScopeHandle : IEquatable<ImportScopeHandle>
    {
        private const uint tokenType = TokenTypeIds.ImportScope;
        private readonly uint _rowId;

        private ImportScopeHandle(uint rowId)
        {
            Debug.Assert(TokenTypeIds.IsValidRowId(rowId));
            _rowId = rowId;
        }

        internal static ImportScopeHandle FromRowId(uint rowId)
        {
            return new ImportScopeHandle(rowId);
        }

        public static implicit operator Handle(ImportScopeHandle handle)
        {
            return new Handle(handle._rowId | tokenType);
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

        internal uint RowId { get { return _rowId; } }

        public static bool operator ==(ImportScopeHandle left, ImportScopeHandle right)
        {
            return left._rowId == right._rowId;
        }

        public override bool Equals(object obj)
        {
            return obj is ImportScopeHandle && ((ImportScopeHandle)obj)._rowId == _rowId;
        }

        public bool Equals(ImportScopeHandle other)
        {
            return _rowId == other._rowId;
        }

        public override int GetHashCode()
        {
            return _rowId.GetHashCode();
        }

        public static bool operator !=(ImportScopeHandle left, ImportScopeHandle right)
        {
            return left._rowId != right._rowId;
        }
    }

    public struct AsyncMethodHandle : IEquatable<AsyncMethodHandle>
    {
        private const uint tokenType = TokenTypeIds.AsyncMethod;
        private readonly uint _rowId;

        private AsyncMethodHandle(uint rowId)
        {
            Debug.Assert(TokenTypeIds.IsValidRowId(rowId));
            _rowId = rowId;
        }

        internal static AsyncMethodHandle FromRowId(uint rowId)
        {
            return new AsyncMethodHandle(rowId);
        }

        public static implicit operator Handle(AsyncMethodHandle handle)
        {
            return new Handle(handle._rowId | tokenType);
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

        internal uint RowId { get { return _rowId; } }

        public static bool operator ==(AsyncMethodHandle left, AsyncMethodHandle right)
        {
            return left._rowId == right._rowId;
        }

        public override bool Equals(object obj)
        {
            return obj is AsyncMethodHandle && ((AsyncMethodHandle)obj)._rowId == _rowId;
        }

        public bool Equals(AsyncMethodHandle other)
        {
            return _rowId == other._rowId;
        }

        public override int GetHashCode()
        {
            return _rowId.GetHashCode();
        }

        public static bool operator !=(AsyncMethodHandle left, AsyncMethodHandle right)
        {
            return left._rowId != right._rowId;
        }
    }

    public struct CustomDebugInformationHandle : IEquatable<CustomDebugInformationHandle>
    {
        private const uint tokenType = TokenTypeIds.CustomDebugInformation;
        private readonly uint _rowId;

        private CustomDebugInformationHandle(uint rowId)
        {
            Debug.Assert(TokenTypeIds.IsValidRowId(rowId));
            _rowId = rowId;
        }

        internal static CustomDebugInformationHandle FromRowId(uint rowId)
        {
            return new CustomDebugInformationHandle(rowId);
        }

        public static implicit operator Handle(CustomDebugInformationHandle handle)
        {
            return new Handle(handle._rowId | tokenType);
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

        internal uint RowId { get { return _rowId; } }

        public static bool operator ==(CustomDebugInformationHandle left, CustomDebugInformationHandle right)
        {
            return left._rowId == right._rowId;
        }

        public override bool Equals(object obj)
        {
            return obj is CustomDebugInformationHandle && ((CustomDebugInformationHandle)obj)._rowId == _rowId;
        }

        public bool Equals(CustomDebugInformationHandle other)
        {
            return _rowId == other._rowId;
        }

        public override int GetHashCode()
        {
            return _rowId.GetHashCode();
        }

        public static bool operator !=(CustomDebugInformationHandle left, CustomDebugInformationHandle right)
        {
            return left._rowId != right._rowId;
        }
    }
}
