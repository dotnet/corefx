// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.DirectoryServices;
using System.Text;
using System.Net;
using Microsoft.Win32.SafeHandles;

namespace System.DirectoryServices.AccountManagement
{
    internal class SidList
    {
        internal SidList(List<byte[]> sidListByteFormat) : this(sidListByteFormat, null, null)
        {
        }

        internal SidList(List<byte[]> sidListByteFormat, string target, NetCred credentials)
        {
            GlobalDebug.WriteLineIf(GlobalDebug.Info, "SidList", "SidList: processing {0} ByteFormat SIDs", sidListByteFormat.Count);
            GlobalDebug.WriteLineIf(GlobalDebug.Info, "SidList", "SidList: Targetting {0} ", (target != null) ? target : "local store");

            // Build the list of SIDs to resolve
            IntPtr hUser = IntPtr.Zero;

            int sidCount = sidListByteFormat.Count;
            IntPtr[] pSids = new IntPtr[sidCount];

            for (int i = 0; i < sidCount; i++)
            {
                pSids[i] = Utils.ConvertByteArrayToIntPtr(sidListByteFormat[i]);
            }

            try
            {
                if (credentials != null)
                {
                    Utils.BeginImpersonation(credentials, out hUser);
                }

                TranslateSids(target, pSids);
            }
            finally
            {
                if (hUser != IntPtr.Zero)
                    Utils.EndImpersonation(hUser);
            }
        }

        internal SidList(UnsafeNativeMethods.SID_AND_ATTR[] sidAndAttr)
        {
            GlobalDebug.WriteLineIf(GlobalDebug.Info, "SidList", "SidList: processing {0} Sid+Attr SIDs", sidAndAttr.Length);

            // Build the list of SIDs to resolve
            int sidCount = sidAndAttr.Length;
            IntPtr[] pSids = new IntPtr[sidCount];

            for (int i = 0; i < sidCount; i++)
            {
                pSids[i] = sidAndAttr[i].pSid;
            }

            TranslateSids(null, pSids);
        }

        protected void TranslateSids(string target, IntPtr[] pSids)
        {
            GlobalDebug.WriteLineIf(GlobalDebug.Info, "AuthZSet", "SidList: processing {0} SIDs", pSids.Length);

            // if there are no SIDs to translate return
            if (pSids.Length == 0)
            {
                return;
            }

            // Build the list of SIDs to resolve
            int sidCount = pSids.Length;

            // Translate the SIDs in bulk
            SafeLsaPolicyHandle policyHandle = null;
            SafeLsaMemoryHandle referencedDomainsHandle = null;
            SafeLsaMemoryHandle namesHandle = null;

            try
            {
                // Get the policy handle
                var oa = new Interop.OBJECT_ATTRIBUTES();
                uint err = Interop.Advapi32.LsaOpenPolicy(
                                target,
                                ref oa,
                                Interop.Advapi32.POLICY_LOOKUP_NAMES,
                                out policyHandle);
                if (err != 0)
                {
                    GlobalDebug.WriteLineIf(GlobalDebug.Warn, "AuthZSet", "SidList: couldn't get policy handle, err={0}", err);

                    throw new PrincipalOperationException(SR.Format(
                                                                SR.AuthZErrorEnumeratingGroups,
                                                                Interop.Advapi32.LsaNtStatusToWinError(err)));
                }

                Debug.Assert(!policyHandle.IsInvalid);

                // Translate the SIDs
                err = Interop.Advapi32.LsaLookupSids(
                                    policyHandle,
                                    sidCount,
                                    pSids,
                                    out referencedDomainsHandle,
                                    out namesHandle);

                // ignore error STATUS_SOME_NOT_MAPPED = 0x00000107 and 
                // STATUS_NONE_MAPPED = 0xC0000073
                if (err != 0 &&
                        err != 263 &&
                        err != 0xC0000073)
                {
                    GlobalDebug.WriteLineIf(GlobalDebug.Warn, "AuthZSet", "SidList: LsaLookupSids failed, err={0}", err);

                    throw new PrincipalOperationException(SR.Format(
                                                                SR.AuthZErrorEnumeratingGroups,
                                                                Interop.Advapi32.LsaNtStatusToWinError(err)));
                }

                // Get the group names in managed form
                var names = new Interop.LSA_TRANSLATED_NAME[sidCount];
                IntPtr pCurrentName = namesHandle.DangerousGetHandle();

                for (int i = 0; i < sidCount; i++)
                {
                    names[i] = Marshal.PtrToStructure<Interop.LSA_TRANSLATED_NAME>(pCurrentName);
                    pCurrentName = new IntPtr(pCurrentName.ToInt64() + Marshal.SizeOf<Interop.LSA_TRANSLATED_NAME>());
                }

                // Get the domain names in managed form

                // Extract LSA_REFERENCED_DOMAIN_LIST.Entries            
                Interop.LSA_REFERENCED_DOMAIN_LIST referencedDomains = Marshal.PtrToStructure<Interop.LSA_REFERENCED_DOMAIN_LIST>(referencedDomainsHandle.DangerousGetHandle());
                int domainCount = referencedDomains.Entries;

                // Extract LSA_REFERENCED_DOMAIN_LIST.Domains, by iterating over the array and marshalling
                // each native LSA_TRUST_INFORMATION into a managed LSA_TRUST_INFORMATION.
                var domains = new Interop.LSA_TRUST_INFORMATION[domainCount];
                IntPtr pCurrentDomain = referencedDomains.Domains;

                for (int i = 0; i < domainCount; i++)
                {
                    domains[i] = Marshal.PtrToStructure<Interop.LSA_TRUST_INFORMATION>(pCurrentDomain);
                    pCurrentDomain = new IntPtr(pCurrentDomain.ToInt64() + Marshal.SizeOf<Interop.LSA_TRUST_INFORMATION>());
                }

                GlobalDebug.WriteLineIf(GlobalDebug.Info, "AuthZSet", "SidList: got {0} groups in {1} domains", sidCount, domainCount);

                // Build the list of entries
                Debug.Assert(names.Length == sidCount);

                for (int i = 0; i < names.Length; i++)
                {
                    Interop.LSA_TRANSLATED_NAME name = names[i];

                    // Build an entry.  Note that LSA_UNICODE_STRING.length is in bytes,
                    // while PtrToStringUni expects a length in characters.
                    SidListEntry entry = new SidListEntry();

                    Debug.Assert(name.Name.Length % 2 == 0);
                    entry.name = Marshal.PtrToStringUni(name.Name.Buffer, name.Name.Length / 2);

                    // Get the domain associated with this name
                    Debug.Assert(name.DomainIndex < domains.Length);
                    if (name.DomainIndex >= 0)
                    {
                        Interop.LSA_TRUST_INFORMATION domain = domains[name.DomainIndex];
                        Debug.Assert(domain.Name.Length % 2 == 0);
                        entry.sidIssuerName = Marshal.PtrToStringUni(domain.Name.Buffer, domain.Name.Length / 2);
                    }

                    entry.pSid = pSids[i];

                    _entries.Add(entry);
                }
            }
            finally
            {
                policyHandle?.Dispose();
                referencedDomainsHandle?.Dispose();
                namesHandle?.Dispose();
            }
        }

        private List<SidListEntry> _entries = new List<SidListEntry>();

        public SidListEntry this[int index]
        {
            get { return _entries[index]; }
        }

        public int Length
        {
            get { return _entries.Count; }
        }

        public void RemoveAt(int index)
        {
            _entries[index].Dispose();
            _entries.RemoveAt(index);
        }

        public void Clear()
        {
            foreach (SidListEntry sl in _entries)
                sl.Dispose();

            _entries.Clear();
        }
    }

    internal class SidListEntry : IDisposable
    {
        public IntPtr pSid = IntPtr.Zero;
        public string name;
        public string sidIssuerName;

        public virtual void Dispose()
        {
            if (pSid != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(pSid);
                pSid = IntPtr.Zero;
            }
        }
    }
}
