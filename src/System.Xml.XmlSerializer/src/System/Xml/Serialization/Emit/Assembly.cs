// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using System.Reflection;

#if !NET_NATIVE
namespace System.Xml.Serialization.Emit
{
    internal abstract class Assembly
    {
        [Obsolete("TODO", error: false)]
        public virtual IEnumerable<CustomAttributeData> CustomAttributes
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public abstract IEnumerable<TypeInfo> DefinedTypes
        {
            get;
        }

        [Obsolete("TODO", error: false)]
        public virtual IEnumerable<Type> ExportedTypes
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public virtual string FullName
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public virtual bool IsDynamic
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public virtual Module ManifestModule
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public abstract IEnumerable<Module> Modules
        {
            get;
        }

        [Obsolete("TODO", error: false)]
        public virtual ManifestResourceInfo GetManifestResourceInfo(string resourceName)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public virtual string[] GetManifestResourceNames()
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public virtual Stream GetManifestResourceStream(string name)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public virtual AssemblyName GetName()
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public virtual Type GetType(string name)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public static Assembly Load(AssemblyName assemblyRef)
        {
            throw new NotImplementedException();
        }
    }
}
#endif