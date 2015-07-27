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
        private ArrayList _list = new ArrayList();

        private DateTime _timeStamp = DateTime.MinValue;
        private bool _has_other_versions;

        private bool _isReadOnly;

        // constructors

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public CookieCollection()
        {
            _isReadOnly = true;
        }


        internal CookieCollection(bool IsReadOnly)
        {
            _isReadOnly = IsReadOnly;
        }

        // properties

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public bool IsReadOnly
        {
            get
            {
                return _isReadOnly;
            }
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public Cookie this[int index]
        {
            get
            {
                if (index < 0 || index >= _list.Count)
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                return (Cookie)(_list[index]);
            }
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public Cookie this[string name]
        {
            get
            {
                foreach (Cookie c in _list)
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
                _list.Add(cookie);
            }
            else
            {
                _list[idx] = cookie;
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
                return _list.Count;
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
            _list.CopyTo(array, index);
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public void CopyTo(Cookie[] array, int index)
        {
            _list.CopyTo(array, index);
        }


        internal DateTime TimeStamp(Stamp how)
        {
            switch (how)
            {
                case Stamp.Set:
                    _timeStamp = DateTime.Now;
                    break;
                case Stamp.SetToMaxUsed:
                    _timeStamp = DateTime.MaxValue;
                    break;
                case Stamp.SetToUnused:
                    _timeStamp = DateTime.MinValue;
                    break;
                case Stamp.Check:
                default:
                    break;
            }
            return _timeStamp;
        }


        // This is for internal cookie container usage
        // For others not that m_has_other_versions gets changed ONLY in InternalAdd
        internal bool IsOtherVersionSeen
        {
            get
            {
                return _has_other_versions;
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
                foreach (Cookie c in _list)
                {
                    if (comp.Compare(cookie, c) == 0)
                    {
                        ret = 0;    //will replace or reject
                        //Cookie2 spec requires that new Variant cookie overwrite the old one
                        if (c.Variant <= cookie.Variant)
                        {
                            _list[idx] = cookie;
                        }
                        break;
                    }
                    ++idx;
                }
                if (idx == _list.Count)
                {
                    _list.Add(cookie);
                }
            }
            else
            {
                _list.Add(cookie);
            }
            if (cookie.Version != Cookie.MaxSupportedVersion)
            {
                _has_other_versions = true;
            }
            return ret;
        }


        internal int IndexOf(Cookie cookie)
        {
            IComparer comp = Cookie.GetComparer();
            int idx = 0;
            foreach (Cookie c in _list)
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
            _list.RemoveAt(idx);
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
            private CookieCollection _cookies;
            private int _count;
            private int _index = -1;
            private int _version;

            internal CookieCollectionEnumerator(CookieCollection cookies)
            {
                _cookies = cookies;
                _count = cookies.Count;
                _version = cookies.m_version;
            }

            // IEnumerator interface

            object IEnumerator.Current
            {
                get
                {
                    if (_index < 0 || _index >= _count)
                    {
                        throw new InvalidOperationException(SR.InvalidOperation_EnumOpCantHappen);
                    }
                    if (_version != _cookies.m_version)
                    {
                        throw new InvalidOperationException(SR.InvalidOperation_EnumFailedVersion);
                    }
                    return _cookies[_index];
                }
            }

            bool IEnumerator.MoveNext()
            {
                if (_version != _cookies.m_version)
                {
                    throw new InvalidOperationException(SR.InvalidOperation_EnumFailedVersion);
                }
                if (++_index < _count)
                {
                    return true;
                }
                _index = _count;
                return false;
            }

            void IEnumerator.Reset()
            {
                _index = -1;
            }
        }
    }
}
