// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Permissions;
using System.Collections;

namespace System.ServiceProcess.ServiceController
{
    [Serializable]
    public class ServiceControllerPermissionEntryCollection : CollectionBase
    {
        ServiceControllerPermission owner;
        internal ServiceControllerPermissionEntryCollection(ServiceControllerPermission owner, ResourcePermissionBaseEntry[] entries)
        {
            this.owner = owner;
            for (int index = 0; index < entries.Length; ++index)
                this.InnerList.Add(new ServiceControllerPermissionEntry(entries[index]));
        }

        public ServiceControllerPermissionEntry this[int index]
        {
            get
            {
                return (ServiceControllerPermissionEntry)List[index];
            }
            set
            {
                List[index] = value;
            }
        }

        public int Add(ServiceControllerPermissionEntry value) => List.Add(value);

        public void AddRange(ServiceControllerPermissionEntry[] value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            for (int i = 0; ((i) < (value.Length)); i = ((i) + (1)))
            {
                this.Add(value[i]);
            }
        }
    
        public void AddRange(ServiceControllerPermissionEntryCollection value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            int currentCount = value.Count;
            for (int i = 0; i < currentCount; i = ((i) + (1)))
            {
                this.Add(value[i]);
            }
        }

        public bool Contains(ServiceControllerPermissionEntry value) => List.Contains(value);

        public void CopyTo(ServiceControllerPermissionEntry[] array, int index)
        {
            List.CopyTo(array, index);
        }

        public int IndexOf(ServiceControllerPermissionEntry value) => List.IndexOf(value);
        
        public void Insert(int index, ServiceControllerPermissionEntry value)
        {
            List.Insert(index, value);
        }

        public void Remove(ServiceControllerPermissionEntry value)
        {
            List.Remove(value);
        }
        
        protected override void OnClear()
        {
            this.owner.Clear();
        }
        
        protected override void OnInsert(int index, object value)
        {
            this.owner.AddPermissionAccess((ServiceControllerPermissionEntry)value);
        }

        protected override void OnRemove(int index, object value)
        {
            this.owner.RemovePermissionAccess((ServiceControllerPermissionEntry)value);
        }

        protected override void OnSet(int index, object oldValue, object newValue)
        {
            this.owner.RemovePermissionAccess((ServiceControllerPermissionEntry)oldValue);
            this.owner.AddPermissionAccess((ServiceControllerPermissionEntry)newValue);
        }
    }
}
