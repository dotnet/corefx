using System;
using System.Resources;
using System.Reflection;
using System.Globalization;

namespace System.Management
{
    internal sealed class RC
    {
        // Private constructor to prevent instances of this class from being created
        RC(){}

        // ResourceManager instance
        static readonly ResourceManager resMgr = new ResourceManager(Assembly.GetExecutingAssembly().GetName().Name ,Assembly.GetExecutingAssembly(),null);

        // Public method to get string from resources
        public static string GetString(string strToGet)
        {
            return resMgr.GetString(strToGet,CultureInfo.CurrentCulture);
        }

    }
}