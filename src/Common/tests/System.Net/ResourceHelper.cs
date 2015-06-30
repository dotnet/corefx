using CoreFXTestLibrary;
using System;
using System.Reflection;
using System.Resources;

namespace NCLTest.Common 
{
    public class ResourceHelper
    {
        private ResourceManager _resourceManager = null;

        // Hiding ctor: use the GetInstance factory function instead.
        private ResourceHelper(ResourceManager rm)
        {
            _resourceManager = rm;
        }

        public static ResourceHelper GetInstance(Type t)
        {
            Assembly asm = t.GetTypeInfo().Assembly;
            var rm = new ResourceManager(asm.GetType("FxResources." + asm.GetName().Name + ".SR"));
            return new ResourceHelper(rm);
        }

        public string GetExceptionMessage(string resourceName, params object[] parameters)
        {
            string errorMessageTemplate = _resourceManager.GetString(resourceName);

            if (string.IsNullOrEmpty(errorMessageTemplate))
            {
                Assert.Fail(string.Format("Localized error message '{0}' does not exist.",
                    resourceName));
            }

            return string.Format(errorMessageTemplate, parameters);
        }
    }
}
