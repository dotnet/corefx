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
    internal sealed class SafeFreeNegoCredentials : SafeFreeCredentials
    {
        private SafeGssCredHandle _credential;
        private readonly bool _isNtlm;
        private readonly string _userName;
        private readonly bool _isDefault;

        public SafeGssCredHandle GssCredential
        {
            get { return _credential; }
        }

        public bool IsNtlm
        {
            get { return _isNtlm; }
        }

        public string UserName
        {
            get { return _userName; }
        }

        public bool IsDefault
        {
            get { return _isDefault; }
        }

        public SafeFreeNegoCredentials(bool isNtlm, string username, string password, string domain)
            : base(IntPtr.Zero, true)
        {
            Debug.Assert(username != null && password != null, "Username and Password can not be null");
            const char At = '@';
            const char Backwhack = '\\';

            int index = username.IndexOf(Backwhack);
            if (index > 0 && username.IndexOf(Backwhack, index + 1) < 0 && string.IsNullOrEmpty(domain))
            {
                domain = username.Substring(0, index);
                username = username.Substring(index + 1);
            }

            //remove any leading and trailing whitespace
            if (domain != null)
            {
                domain = domain.Trim();
            }

            if (username != null)
            {
                username = username.Trim();
            }

            if ((username.IndexOf(At) < 0) && !string.IsNullOrEmpty(domain))
            {
                username += At + domain;
            }

            bool ignore = false;
            _isNtlm = isNtlm;
            _userName = username;
            _isDefault = string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password);
            _credential = SafeGssCredHandle.Create(username, password, isNtlm);
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

    internal sealed class SafeDeleteNegoContext : SafeDeleteContext
    {
        private SafeGssNameHandle _targetName;
        private SafeGssContextHandle _context;
        private bool _isNtlm;

        public SafeGssNameHandle TargetName
        {
            get { return _targetName; }
        }

        public bool IsNtlm
        {
            get { return _isNtlm; }
        }

        public SafeGssContextHandle GssContext
        {
            get { return _context; }
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

        public void SetGssContext(SafeGssContextHandle context)
        {
            Debug.Assert(!context.IsInvalid, "Invalid context passed to SafeDeleteNegoContext");
            _context = context;
        }

        public void SetAuthenticationPackage(int isNtlm)
        {
            Debug.Assert(isNtlm == 0 || isNtlm == 1, "Invalid isNtlm value passed to SafeDeleteNegoContext");
            _isNtlm = isNtlm == 1;
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
