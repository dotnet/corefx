// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Runtime.Serialization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    public sealed class CollectionDataContractAttribute : Attribute
    {
        private string _name;
        private string _ns;
        private string _itemName;
        private string _keyName;
        private string _valueName;
        private bool _isReference;
        private bool _isNameSetExplicitly;
        private bool _isNamespaceSetExplicitly;
        private bool _isReferenceSetExplicitly;
        private bool _isItemNameSetExplicitly;
        private bool _isKeyNameSetExplicitly;
        private bool _isValueNameSetExplicitly;

        public CollectionDataContractAttribute()
        {
        }

        public string Namespace
        {
            get { return _ns; }
            set
            {
                _ns = value;
                _isNamespaceSetExplicitly = true;
            }
        }

        public bool IsNamespaceSetExplicitly
        {
            get { return _isNamespaceSetExplicitly; }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                _isNameSetExplicitly = true;
            }
        }

        public bool IsNameSetExplicitly
        {
            get { return _isNameSetExplicitly; }
        }

        public string ItemName
        {
            get { return _itemName; }
            set
            {
                _itemName = value;
                _isItemNameSetExplicitly = true;
            }
        }

        public bool IsItemNameSetExplicitly
        {
            get { return _isItemNameSetExplicitly; }
        }

        public string KeyName
        {
            get { return _keyName; }
            set
            {
                _keyName = value;
                _isKeyNameSetExplicitly = true;
            }
        }

        public bool IsReference
        {
            get { return _isReference; }
            set
            {
                _isReference = value;
                _isReferenceSetExplicitly = true;
            }
        }

        public bool IsReferenceSetExplicitly
        {
            get { return _isReferenceSetExplicitly; }
        }

        public bool IsKeyNameSetExplicitly
        {
            get { return _isKeyNameSetExplicitly; }
        }

        public string ValueName
        {
            get { return _valueName; }
            set
            {
                _valueName = value;
                _isValueNameSetExplicitly = true;
            }
        }

        public bool IsValueNameSetExplicitly
        {
            get { return _isValueNameSetExplicitly; }
        }
    }
}
