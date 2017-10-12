//------------------------------------------------------------------------------
// <copyright from='1997' to='2001' company='Microsoft Corporation'>           
//    Copyright (c) Microsoft Corporation. All Rights Reserved.                
//    Information Contained Herein is Proprietary and Confidential.            
// </copyright>                                                                
//------------------------------------------------------------------------------

namespace System.Management.Instrumentation
{
	using System;
	using System.Reflection;
	using System.Globalization;

	sealed class AssemblyNameUtility
	{
		private static string BinToString(byte [] rg)
		{
			if(rg == null)
				return "";
			string sz = "";
			for(int i=0;i<rg.GetLength(0);i++)
			{
				sz += String.Format("{0:x2}", rg[i]);
			}
			return sz;
		}

		public static string UniqueToAssemblyMinorVersion(Assembly assembly)
		{
        	AssemblyName an = assembly.GetName(true);
			return an.Name + "_SN_"+BinToString(an.GetPublicKeyToken()) + "_Version_"+an.Version.Major +"."+ an.Version.Minor; // +"."+ an.Version.Build+"."+an.Version.Revision;
		}

		public static string UniqueToAssemblyFullVersion(Assembly assembly)
		{
       		AssemblyName an = assembly.GetName(true);
			return an.Name + "_SN_"+BinToString(an.GetPublicKeyToken()) + "_Version_"+an.Version.Major +"."+ an.Version.Minor+"."+ an.Version.Build+"."+an.Version.Revision;
		}


        static string UniqueToAssemblyVersion(Assembly assembly)
        {
            AssemblyName an = assembly.GetName(true);
            return an.Name + "_SN_"+BinToString(an.GetPublicKeyToken()) + "_Version_"+an.Version.Major +"."+ an.Version.Minor+"."+an.Version.Build +"."+ an.Version.Revision;
        }

        public static string UniqueToAssemblyBuild(Assembly assembly)
		{
            return UniqueToAssemblyVersion(assembly) + "_Mvid_"+MetaDataInfo.GetMvid(assembly).ToString().ToLower(CultureInfo.InvariantCulture);
		}
	}
}
