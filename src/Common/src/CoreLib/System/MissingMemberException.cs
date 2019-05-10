// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;

namespace System
{
    [Serializable]
    [System.Runtime.CompilerServices.TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    public partial class MissingMemberException : MemberAccessException
    {
        public MissingMemberException()
            : base(SR.Arg_MissingMemberException)
        {
            HResult = HResults.COR_E_MISSINGMEMBER;
        }

        public MissingMemberException(string? message)
            : base(message)
        {
            HResult = HResults.COR_E_MISSINGMEMBER;
        }

        public MissingMemberException(string? message, Exception? inner)
            : base(message, inner)
        {
            HResult = HResults.COR_E_MISSINGMEMBER;
        }

        public MissingMemberException(string? className, string? memberName)
        {
            ClassName = className;
            MemberName = memberName;
        }

        protected MissingMemberException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ClassName = info.GetString("MMClassName");
            MemberName = info.GetString("MMMemberName");
            Signature = (byte[]?)info.GetValue("MMSignature", typeof(byte[]));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("MMClassName", ClassName, typeof(string));
            info.AddValue("MMMemberName", MemberName, typeof(string));
            info.AddValue("MMSignature", Signature, typeof(byte[]));
        }

        public override string Message
        {
            get
            {
                if (ClassName == null)
                {
                    return base.Message;
                }
                else
                {
                    // do any desired fixups to classname here.
                    return SR.Format(SR.MissingMember_Name, ClassName + "." + MemberName + (Signature != null ? " " + FormatSignature(Signature) : string.Empty));
                }
            }
        }

        // If ClassName != null, GetMessage will construct on the fly using it
        // and the other variables. This allows customization of the
        // format depending on the language environment.
        protected string? ClassName;
        protected string? MemberName;
        protected byte[]? Signature;
    }
}
