// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Globalization;

namespace System.Net
{
    //
    // CookieCollection
    //
    //  A list of cookies maintained in Sorted order. Only one cookie with matching Name/Domain/Path 
    //

    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public class CookieCollection : ICollection
    {
        // fields
        internal enum Stamp
        {
            Check = 0,
            Set = 1,
            SetToUnused = 2,
            SetToMaxUsed = 3,
        }

        internal int m_version;
        ArrayList m_list = new ArrayList();

        DateTime m_TimeStamp = DateTime.MinValue;
        bool m_has_other_versions;

        bool m_IsReadOnly;

        // constructors

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public CookieCollection()
        {
            m_IsReadOnly = true;
        }


        internal CookieCollection(bool IsReadOnly)
        {
            m_IsReadOnly = IsReadOnly;
        }

        // properties

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public bool IsReadOnly
        {
            get
            {
                return m_IsReadOnly;
            }
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public Cookie this[int index]
        {
            get
            {
                if (index < 0 || index >= m_list.Count)
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                return (Cookie)(m_list[index]);
            }
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public Cookie this[string name]
        {
            get
            {
                foreach (Cookie c in m_list)
                {
                    if (string.Compare(c.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        return c;
                    }
                }
                return null;
            }
        }

        // methods

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public void Add(Cookie cookie)
        {
            if (cookie == null)
            {
                throw new ArgumentNullException("cookie");
            }
            ++m_version;
            int idx = IndexOf(cookie);
            if (idx == -1)
            {
                m_list.Add(cookie);
            }
            else
            {
                m_list[idx] = cookie;
            }
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public void Add(CookieCollection cookies)
        {
            if (cookies == null)
            {
                throw new ArgumentNullException("cookies");
            }
            //if (cookies == this) {
            //    cookies = new CookieCollection(cookies);
            //}
            foreach (Cookie cookie in cookies)
            {
                Add(cookie);
            }
        }

        // ICollection interface

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public int Count
        {
            get
            {
                return m_list.Count;
            }
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public object SyncRoot
        {
            get
            {
                return this;
            }
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public void CopyTo(Array array, int index)
        {
            m_list.CopyTo(array, index);
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public void CopyTo(Cookie[] array, int index)
        {
            m_list.CopyTo(array, index);
        }


        internal DateTime TimeStamp(Stamp how)
        {
            switch (how)
            {
                case Stamp.Set:
                    m_TimeStamp = DateTime.Now;
                    break;
                case Stamp.SetToMaxUsed:
                    m_TimeStamp = DateTime.MaxValue;
                    break;
                case Stamp.SetToUnused:
                    m_TimeStamp = DateTime.MinValue;
                    break;
                case Stamp.Check:
                default:
                    break;
            }
            return m_TimeStamp;
        }


        // This is for internal cookie container usage
        // For others not that m_has_other_versions gets changed ONLY in InternalAdd
        internal bool IsOtherVersionSeen
        {
            get
            {
                return m_has_other_versions;
            }
        }

        // If isStrict == false, assumes that incoming cookie is unique
        // If isStrict == true, replace the cookie if found same with newest Variant
        // returns 1 if added, 0 if replaced or rejected
        internal int InternalAdd(Cookie cookie, bool isStrict)
        {
            int ret = 1;
            if (isStrict)
            {
                IComparer comp = Cookie.GetComparer();
                int idx = 0;
                foreach (Cookie c in m_list)
                {
                    if (comp.Compare(cookie, c) == 0)
                    {
                        ret = 0;    //will replace or reject
                        //Cookie2 spec requires that new Variant cookie overwrite the old one
                        if (c.Variant <= cookie.Variant)
                        {
                            m_list[idx] = cookie;
                        }
                        break;
                    }
                    ++idx;
                }
                if (idx == m_list.Count)
                {
                    m_list.Add(cookie);
                }
            }
            else
            {
                m_list.Add(cookie);
            }
            if (cookie.Version != Cookie.MaxSupportedVersion)
            {
                m_has_other_versions = true;
            }
            return ret;
        }


        internal int IndexOf(Cookie cookie)
        {
            IComparer comp = Cookie.GetComparer();
            int idx = 0;
            foreach (Cookie c in m_list)
            {
                if (comp.Compare(cookie, c) == 0)
                {
                    return idx;
                }
                ++idx;
            }
            return -1;
        }

        internal void RemoveAt(int idx)
        {
            m_list.RemoveAt(idx);
        }


        // IEnumerable interface

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public IEnumerator GetEnumerator()
        {
            return new CookieCollectionEnumerator(this);
        }

#if DEBUG
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        internal void Dump()
        {
            GlobalLog.Print("CookieCollection:");
            foreach (Cookie cookie in this)
            {
                cookie.Dump();
            }
        }
#endif

        //Not used anymore delete ?
        private class CookieCollectionEnumerator : IEnumerator
        {
            CookieCollection m_cookies;
            int m_count;
            int m_index = -1;
            int m_version;

            internal CookieCollectionEnumerator(CookieCollection cookies)
            {
                m_cookies = cookies;
                m_count = cookies.Count;
                m_version = cookies.m_version;
            }

            // IEnumerator interface

            object IEnumerator.Current
            {
                get
                {
                    if (m_index < 0 || m_index >= m_count)
                    {
                        throw new InvalidOperationException(SR.InvalidOperation_EnumOpCantHappen);
                    }
                    if (m_version != m_cookies.m_version)
                    {
                        throw new InvalidOperationException(SR.InvalidOperation_EnumFailedVersion);
                    }
                    return m_cookies[m_index];
                }
            }

            bool IEnumerator.MoveNext()
            {
                if (m_version != m_cookies.m_version)
                {
                    throw new InvalidOperationException(SR.InvalidOperation_EnumFailedVersion);
                }
                if (++m_index < m_count)
                {
                    return true;
                }
                m_index = m_count;
                return false;
            }

            void IEnumerator.Reset()
            {
                m_index = -1;
            }
        }
    }
}
