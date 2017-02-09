// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.DirectoryServices.Interop
{
#pragma warning disable BCL0015 // CoreFxPort
    using System.Runtime.InteropServices;
    using System;
    using System.Security;
    using System.Security.Permissions;
    using System.Collections;
    using System.IO;
    using System.Text;

    [StructLayout(LayoutKind.Explicit)]
    internal struct Variant
    {
        [FieldOffset(0)]
        public ushort varType;
        [FieldOffset(2)]
        public ushort reserved1;
        [FieldOffset(4)]
        public ushort reserved2;
        [FieldOffset(6)]
        public ushort reserved3;
        [FieldOffset(8)]
        public short boolvalue;
        [FieldOffset(8)]
        public IntPtr ptr1;
        [FieldOffset(12)]
        public IntPtr ptr2;
    }

    [
    SuppressUnmanagedCodeSecurityAttribute()
    ]
    internal class UnsafeNativeMethods
    {
        [DllImport(ExternDll.Activeds, ExactSpelling = true, EntryPoint = "ADsOpenObject", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
        private static extern int IntADsOpenObject(string path, string userName, string password, int flags, [In, Out] ref Guid iid, [Out, MarshalAs(UnmanagedType.Interface)] out object ppObject);
        public static int ADsOpenObject(string path, string userName, string password, int flags, [In, Out] ref Guid iid, [Out, MarshalAs(UnmanagedType.Interface)] out object ppObject)
        {
            try
            {
                return IntADsOpenObject(path, userName, password, flags, ref iid, out ppObject);
            }
            catch (EntryPointNotFoundException)
            {
                throw new InvalidOperationException(SR.DSAdsiNotInstalled);
            }
        }

        public interface IAds
        {
            string Name
            {
                [return: MarshalAs(UnmanagedType.BStr)]
                get;
            }

            string Class
            {
                [return: MarshalAs(UnmanagedType.BStr)]
                get;
            }

            string GUID
            {
                [return: MarshalAs(UnmanagedType.BStr)]
                get;
            }

            string ADsPath
            {
                [return: MarshalAs(UnmanagedType.BStr)]
                get;
            }

            string Parent
            {
                [return: MarshalAs(UnmanagedType.BStr)]
                get;
            }

            string Schema
            {
                [return: MarshalAs(UnmanagedType.BStr)]
                get;
            }

            void GetInfo();

            void SetInfo();

            Object Get(
                [In, MarshalAs(UnmanagedType.BStr)]
                string bstrName);

            void Put(
                [In, MarshalAs(UnmanagedType.BStr)]
                string bstrName,
                [In]
                Object vProp);

            [PreserveSig]
            int GetEx(
                [In, MarshalAs(UnmanagedType.BStr)]
                String bstrName,
                [Out]
                out object value);

            void PutEx(
                [In, MarshalAs(UnmanagedType.U4)]
                int lnControlCode,
                [In, MarshalAs(UnmanagedType.BStr)]
                string bstrName,
                [In]
                Object vProp);

            void GetInfoEx(
                [In]
                Object vProperties,
                [In, MarshalAs(UnmanagedType.U4)]
                int lnReserved);
        }

        public interface IAdsContainer
        {
            int Count
            {
                [return: MarshalAs(UnmanagedType.U4)]
                get;
            }

            object _NewEnum
            {
                [return: MarshalAs(UnmanagedType.Interface)]
                get;
            }

            object Filter
            {
                get;
                set;
            }

            object Hints
            {
                get;
                set;
            }

            [return: MarshalAs(UnmanagedType.Interface)]
            object GetObject(
                [In, MarshalAs(UnmanagedType.BStr)]
                string className,
                [In, MarshalAs(UnmanagedType.BStr)]
                string relativeName);

            [return: MarshalAs(UnmanagedType.Interface)]
            object Create(
                [In, MarshalAs(UnmanagedType.BStr)]
                string className,
                [In, MarshalAs(UnmanagedType.BStr)]
                string relativeName);

            void Delete(
                [In, MarshalAs(UnmanagedType.BStr)]
                string className,
                [In, MarshalAs(UnmanagedType.BStr)]
                string relativeName);

            [return: MarshalAs(UnmanagedType.Interface)]
            object CopyHere(
                [In, MarshalAs(UnmanagedType.BStr)]
                string sourceName,
                [In, MarshalAs(UnmanagedType.BStr)]
                string newName);

            [return: MarshalAs(UnmanagedType.Interface)]
            object MoveHere(
                [In, MarshalAs(UnmanagedType.BStr)]
                string sourceName,
                [In, MarshalAs(UnmanagedType.BStr)]
                string newName);
        }

        public interface IAdsDeleteOps
        {
            void DeleteObject(int flags);
        }

        //
        // PropertyValue as a co-class that implements the IAdsPropertyValue interface
        //
        [ComImport, Guid("7b9e38b0-a97c-11d0-8534-00c04fd8d503")]
        public class PropertyValue
        {
        }

        public interface IADsLargeInteger
        {
            int HighPart
            {
                get;
                set;
            }
            int LowPart
            {
                get;
                set;
            }
        }

        public interface IAdsPropertyValue
        {
            void Clear();

            int ADsType
            {
                get;
                set;
            }

            string DNString
            {
                [return: MarshalAs(UnmanagedType.BStr)]
                get;
                [param: MarshalAs(UnmanagedType.BStr)]
                set;
            }

            string CaseExactString
            {
                [return: MarshalAs(UnmanagedType.BStr)]
                get;
                [param: MarshalAs(UnmanagedType.BStr)]
                set;
            }

            string CaseIgnoreString
            {
                [return: MarshalAs(UnmanagedType.BStr)]
                get;
                [param: MarshalAs(UnmanagedType.BStr)]
                set;
            }

            string PrintableString
            {
                [return: MarshalAs(UnmanagedType.BStr)]
                get;
                [param: MarshalAs(UnmanagedType.BStr)]
                set;
            }

            string NumericString
            {
                [return: MarshalAs(UnmanagedType.BStr)]
                get;
                [param: MarshalAs(UnmanagedType.BStr)]
                set;
            }

            bool Boolean
            {
                get;
                set;
            }

            int Integer
            {
                get;
                set;
            }

            object OctetString
            {
                get;

                set;
            }

            object SecurityDescriptor
            {
                get;

                set;
            }

            object LargeInteger
            {
                get;

                set;
            }

            object UTCTime
            {
                get;

                set;
            }
        }

        //
        // PropertyEntry as a co-class that implements the IAdsPropertyEntry interface
        //
        public class PropertyEntry
        {
        }

        public interface IAdsPropertyEntry
        {
            void Clear();

            string Name
            {
                [return: MarshalAs(UnmanagedType.BStr)]
                get;
                [param: MarshalAs(UnmanagedType.BStr)]
                set;
            }

            int ADsType
            {
                get;
                set;
            }

            int ControlCode
            {
                get;
                set;
            }

            object Values
            {
                get;
                set;
            }
        }

        public interface IAdsPropertyList
        {
            int PropertyCount
            {
                [return: MarshalAs(UnmanagedType.U4)]
                get;
            }

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int Next([Out] out object nextProp);

            void Skip([In] int cElements);

            void Reset();

            object Item([In] object varIndex);

            object GetPropertyItem([In, MarshalAs(UnmanagedType.BStr)] string bstrName, int ADsType);

            void PutPropertyItem([In] object varData);

            void ResetPropertyItem([In] object varEntry);

            void PurgePropertyList();
        }

        [ComImport, Guid("109BA8EC-92F0-11D0-A790-00C04FD8D5A8"), System.Runtime.InteropServices.InterfaceTypeAttribute(System.Runtime.InteropServices.ComInterfaceType.InterfaceIsIUnknown)]
        public interface IDirectorySearch
        {
            void SetSearchPreference(
                [In]
                IntPtr /*ads_searchpref_info * */pSearchPrefs,
                //ads_searchpref_info[] pSearchPrefs,
                int dwNumPrefs);

            void ExecuteSearch(
                [In, MarshalAs(UnmanagedType.LPWStr)]
                string pszSearchFilter,
                [In, MarshalAs(UnmanagedType.LPArray)]
                string[] pAttributeNames,
                [In]
                int dwNumberAttributes,
                [Out]
                out IntPtr hSearchResult);

            void AbandonSearch([In] IntPtr hSearchResult);

            [return: MarshalAs(UnmanagedType.U4)]
            [PreserveSig]
            int GetFirstRow([In] IntPtr hSearchResult);

            [return: MarshalAs(UnmanagedType.U4)]
            [PreserveSig]
            int GetNextRow([In] IntPtr hSearchResult);

            [return: MarshalAs(UnmanagedType.U4)]
            [PreserveSig]
            int GetPreviousRow([In] IntPtr hSearchResult);

            [return: MarshalAs(UnmanagedType.U4)]
            [PreserveSig]
            int GetNextColumnName(
                [In] IntPtr hSearchResult,
                [Out]
                IntPtr ppszColumnName);

            void GetColumn(
                [In] IntPtr hSearchResult,
                [In] //, MarshalAs(UnmanagedType.LPWStr)]
                IntPtr /* char * */ szColumnName,
                [In]
                IntPtr pSearchColumn);

            void FreeColumn(
                [In]
                IntPtr pSearchColumn);

            void CloseSearchHandle([In] IntPtr hSearchResult);
        }

        public interface IAdsObjectOptions
        {
            object GetOption(int flag);

            void SetOption(int flag, [In] object varValue);
        }

        // for boolean type, the default marshaller does not work, so need to have specific marshaller. For other types, use the
        // default marshaller which is more efficient        

        public interface IAdsObjectOptions2
        {
            [PreserveSig]
            int GetOption(int flag, [Out] out object value);

            void SetOption(int option, Variant value);
        }

        // IDirecorySearch return codes  
        internal const int S_ADS_NOMORE_ROWS = 0x00005012;
        internal const int INVALID_FILTER = unchecked((int)0x8007203E);
        internal const int SIZE_LIMIT_EXCEEDED = unchecked((int)0x80072023);
    }
}

