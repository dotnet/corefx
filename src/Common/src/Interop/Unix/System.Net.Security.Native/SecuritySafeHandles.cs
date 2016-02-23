// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace System.Net.Security
{
    internal sealed partial class SafeFreeNegoCredentials : SafeFreeCredentials
    {
        private SafeGssCredHandle _credential;
        private bool _isdefault;
        private readonly string _username;
        private readonly string _domain;
        private bool _isNtlm;

        public bool IsDefault
        {
            get { return _isdefault; }
        }

        public string UserName
        {
            get { return _username; }
        }

        public string Domain
        {
            get { return _domain; }
        }

        public bool IsNtlm
        {
            get { return _isNtlm; }
        }

        public SafeGssCredHandle GssCredential
        {
            get { return _credential; }
        }

        public SafeFreeNegoCredentials(string username, string password, string domain) : base(IntPtr.Zero, true)
        {
            bool ignore = false;
            _credential = SafeGssCredHandle.Create(username, password, domain);
            _credential.DangerousAddRef(ref ignore);
        }

        public override bool IsInvalid
        {
            get { return (null == _credential); }
        }

        protected override bool ReleaseHandle()
        {
            _credential.DangerousRelease();
            _credential = null;
            return true;
        }
    }

    internal sealed partial class SafeDeleteNegoContext : SafeDeleteContext
    {
        private SafeGssNameHandle _targetName;
        private SafeGssContextHandle _context;
        private bool _isNtlm;

        public SafeGssNameHandle TargetName
        {
            get { return _targetName; }
        }

        public SafeGssContextHandle GssContext
        {
            get { return _context; }
        }

        public bool IsNtlm
        {
            get { return _isNtlm; }
        }

        public SafeDeleteNegoContext(SafeFreeNegoCredentials credential, string targetName)
            : base(credential)
        {
            try
            {
                _targetName = SafeGssNameHandle.CreatePrincipal(targetName);
            }
            catch
            {
                Debug.Assert((null != credential), "Null credential in SafeDeleteNegoContext");
                Dispose();
                throw;
            }
        }

        public void SetGssContext(SafeGssContextHandle context, bool isNtlm)
        {
            Debug.Assert(!context.IsInvalid, "Invalid context passed to SafeDeleteNegoContext");
            _context = context;
            _isNtlm = isNtlm;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (null != _context)
                {
                    _context.Dispose();
                    _context = null;
                }

                if (_targetName != null)
                {
                    _targetName.Dispose();
                    _targetName = null;
                }
            }
            base.Dispose(disposing);
        }
    }
}
