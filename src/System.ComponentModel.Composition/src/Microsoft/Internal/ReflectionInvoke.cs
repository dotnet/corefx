using System;
using System.Reflection;
using System.Security;
using System.Security.Permissions;

namespace Microsoft.Internal
{
    internal static class ReflectionInvoke
    {
        public static object SafeCreateInstance(this Type type, params object[] arguments)
        {
            DemandMemberAccessIfNeeded(type);

            return Activator.CreateInstance(type, arguments);
        }

        public static object SafeInvoke(this ConstructorInfo constructor, params object[] arguments)
        {
            DemandMemberAccessIfNeeded(constructor);

            return constructor.Invoke(arguments);
        }

        public static object SafeInvoke(this MethodInfo method, object instance, params object[] arguments)
        {
            DemandMemberAccessIfNeeded(method);

            return method.Invoke(instance, arguments);
        }

        public static object SafeGetValue(this FieldInfo field, object instance)
        {
            DemandMemberAccessIfNeeded(field);

            return field.GetValue(instance);
        }

        public static void SafeSetValue(this FieldInfo field, object instance, object value)
        {
            DemandMemberAccessIfNeeded(field);

            field.SetValue(instance, value);
        }

#if FEATURE_CAS_APTCA
        private static readonly ReflectionPermission _memberAccess = new ReflectionPermission(ReflectionPermissionFlag.MemberAccess);
        private static readonly ReflectionPermission _restrictedMemberAccess = new ReflectionPermission(ReflectionPermissionFlag.RestrictedMemberAccess);
        
        public static void DemandMemberAccessIfNeeded(MethodInfo method)
        {
            if (!method.IsVisible())
            {
                DemandMemberAccess(method);
            }
        }

        private static void DemandMemberAccessIfNeeded(FieldInfo field)
        {
            if (!field.IsVisible())
            {
                DemandMemberAccess(field);
            }
        }

        public static void DemandMemberAccessIfNeeded(Type type)
        {
            // Consult UnderlyingSystemType this is the type that Activator.CreateInstance creates            
            if (!type.UnderlyingSystemType.IsVisible)
            {
                DemandMemberAccess(type);
            }
        }

        private static void DemandMemberAccessIfNeeded(ConstructorInfo constructor)
        {
            if (!constructor.IsVisible())
            {
                DemandMemberAccess(constructor);
            }
        }

        [System.Security.SecuritySafeCritical]
        private static void DemandMemberAccess(MemberInfo target)
        {
            try
            {
                _memberAccess.Demand();
            }
            catch (SecurityException)
            {   // The caller doesn't have member access, but let's see whether they have access to
                // members of assemblies with less or equal permissions (this mimics Reflection's behavior)

                DemandRestrictedMemberAccess(target);
            }
        }

        [System.Security.SecuritySafeCritical]
        private static void DemandRestrictedMemberAccess(MemberInfo target)
        {
            Assembly targetAssembly = target.Assembly();

            PermissionSet targetGrantSet = UnsafePermissionSet(targetAssembly);
            targetGrantSet.AddPermission(_restrictedMemberAccess);
            targetGrantSet.Demand();
        }

        [SecuritySafeCritical] // PermissionSet is [SecurityCritical]
        private static PermissionSet UnsafePermissionSet(Assembly assembly)
        {
            return assembly.PermissionSet;
        }
#else
        public static void DemandMemberAccessIfNeeded(MethodInfo method) { }
        private static void DemandMemberAccessIfNeeded(ConstructorInfo constructor) { }
        private static void DemandMemberAccessIfNeeded(FieldInfo field) { }
        public static void DemandMemberAccessIfNeeded(Type type) { }
#endif //FEATURE_CAS_APTCA
    }
}