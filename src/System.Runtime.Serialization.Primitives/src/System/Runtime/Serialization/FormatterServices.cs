// Licensed to the .NET Foundation under one or more agreements.
namespace System.Runtime.Serialization
{
    public static class FormatterServices
    {
    	public static object GetUninitializedObject(Type type)
    	{
    		// On .NET Native, this is not implemented
    		throw new NotImplementedException();
    	}
    }
}