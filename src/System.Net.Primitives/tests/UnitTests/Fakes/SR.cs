// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System
{
    public static class SR
    {
        public const string InvalidOperation_EnumFailedVersion = "";
        public const string InvalidOperation_EnumOpCantHappen = "";
        public const string net_container_add_cookie = "";
        public const string net_cookie_attribute = "";
        public const string net_cookie_capacity_range = "";
        public const string net_cookie_format = "";
        public const string net_cookie_parse_header = "";
        public const string net_cookie_size = "";
        public const string net_emptystringcall = "";
        public const string net_toosmall = "";
        public const string dns_bad_ip_address = "";


        public static string Format(object arg1, object arg2)
        {
            return Format(arg1, arg2, string.Empty, string.Empty);
        }

        public static string Format(object arg1, object arg2, object arg3)
        {
            return Format(arg1, arg2, arg3, string.Empty);
        }

        public static string Format(object arg1, object arg2, object arg3, object arg4)
        {
            return string.Empty;
        }

        public static string GetString(string resource)
        {
            return resource;
        }
    }
}
