// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*============================================================
**
**
** Purpose:
** This public class describes the metadata for a specific Keyword
** defined by a Provider. An instance of this class is obtained from
** a ProviderMetadata object.
**
============================================================*/

using System.Collections.Generic;

namespace System.Diagnostics.Eventing.Reader
{
    /// <summary>
    /// Describes the metadata for a specific Keyword defined by a Provider.
    /// An instance of this class is obtained from a ProviderMetadata object.
    /// </summary>
    public sealed class EventKeyword
    {
        private long _value;
        private string _name;
        private string _displayName;
        private bool _dataReady;
        private ProviderMetadata _pmReference;
        private object _syncObject;

        //called from EventMetadata
        internal EventKeyword(long value, ProviderMetadata pmReference)
        {
            _value = value;
            _pmReference = pmReference;
            _syncObject = new object();
        }

        //called from ProviderMetadata
        internal EventKeyword(string name, long value, string displayName)
        {
            _value = value;
            _name = name;
            _displayName = displayName;
            _dataReady = true;
            _syncObject = new object();
        }

        internal void PrepareData()
        {
            if (_dataReady == true) return;

            lock (_syncObject)
            {
                if (_dataReady == true) return;

                IEnumerable<EventKeyword> result = _pmReference.Keywords;

                _name = null;
                _displayName = null;
                _dataReady = true;

                foreach (EventKeyword key in result)
                {
                    if (key.Value == _value)
                    {
                        _name = key.Name;
                        _displayName = key.DisplayName;
                        break;
                    }
                }
            }
        }

        public string Name
        {
            get
            {
                PrepareData();
                return _name;
            }
        }

        public long Value
        {
            get
            {
                return _value;
            }
        }

        public string DisplayName
        {
            get
            {
                PrepareData();
                return _displayName;
            }
        }
    }
}
