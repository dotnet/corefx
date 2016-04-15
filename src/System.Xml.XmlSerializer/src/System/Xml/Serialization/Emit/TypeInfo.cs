// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#if !NET_NATIVE
namespace System.Xml.Serialization.Emit
{
    internal abstract class TypeInfo : Type
    {
        public override TypeInfo GetTypeInfo() => this;

        [Obsolete("TODO", error: false)]
        public virtual IEnumerable<ConstructorInfo> DeclaredConstructors
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public virtual IEnumerable<EventInfo> DeclaredEvents
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public virtual IEnumerable<FieldInfo> DeclaredFields
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public virtual IEnumerable<MemberInfo> DeclaredMembers
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public virtual IEnumerable<MethodInfo> DeclaredMethods
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public virtual IEnumerable<TypeInfo> DeclaredNestedTypes
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public virtual IEnumerable<PropertyInfo> DeclaredProperties
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public virtual Type[] GenericTypeParameters
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public virtual IEnumerable<Type> ImplementedInterfaces
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public abstract TypeAttributes Attributes
        {
            get;
        }

        public abstract Assembly Assembly { get; }

        [Obsolete("TODO", error: false)]
        public abstract Type BaseType
        {
            get;
        }

        [Obsolete("TODO", error: false)]
        public abstract bool ContainsGenericParameters
        {
            get;
        }

        [Obsolete("TODO", error: false)]
        public abstract MethodBase DeclaringMethod
        {
            get;
        }

        [Obsolete("TODO", error: false)]
        public abstract GenericParameterAttributes GenericParameterAttributes
        {
            get;
        }

        [Obsolete("TODO", error: false)]
        public abstract Guid GUID
        {
            get;
        }

        [Obsolete("TODO", error: false)]
        public bool IsAbstract
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public bool IsAnsiClass
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public bool IsAutoClass
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public bool IsAutoLayout
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public bool IsClass
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public abstract bool IsEnum
        {
            get;
        }

        [Obsolete("TODO", error: false)]
        public bool IsExplicitLayout
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public abstract bool IsGenericType
        {
            get;
        }

        [Obsolete("TODO", error: false)]
        public abstract bool IsGenericTypeDefinition
        {
            get;
        }

        [Obsolete("TODO", error: false)]
        public bool IsImport
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public bool IsInterface
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public bool IsLayoutSequential
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public bool IsMarshalByRef
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public bool IsNestedAssembly
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public bool IsNestedFamANDAssem
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public bool IsNestedFamily
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public bool IsNestedFamORAssem
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public bool IsNestedPrivate
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public bool IsNestedPublic
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public bool IsNotPublic
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public bool IsPrimitive
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public bool IsPublic
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public bool IsSealed
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public bool IsVisible
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public abstract bool IsSerializable
        {
            get;
        }

        [Obsolete("TODO", error: false)]
        public bool IsSpecialName
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public bool IsUnicodeClass
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public bool IsValueType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual Type AsType() => this;

        [Obsolete("TODO", error: false)]
        public virtual EventInfo GetDeclaredEvent(string name)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public virtual FieldInfo GetDeclaredField(string name)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public virtual MethodInfo GetDeclaredMethod(string name)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public virtual IEnumerable<MethodInfo> GetDeclaredMethods(string name)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public virtual TypeInfo GetDeclaredNestedType(string name)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public virtual PropertyInfo GetDeclaredProperty(string name)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public virtual bool IsAssignableFrom(TypeInfo typeInfo)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public abstract Type[] GetGenericParameterConstraints();

        [Obsolete("TODO", error: false)]
        public virtual bool IsSubclassOf(Type c)
        {
            throw new NotImplementedException();
        }
    }
}
#endif