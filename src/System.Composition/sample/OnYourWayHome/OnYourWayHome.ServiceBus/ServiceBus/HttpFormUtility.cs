//---------------------------------------------------------------------------------
// Copyright (c), Microsoft Corporation
//---------------------------------------------------------------------------------

namespace OnYourWayHome.ServiceBus
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    internal static class HttpFormUtility
    {
        public static string EncodeAsHttpForm(this IDictionary<string, string> values)
        {
            StringBuilder result = new StringBuilder();
            foreach (var item in values)
            {
                result.AppendFormat("{0}={1}&", Uri.EscapeDataString(item.Key), Uri.EscapeDataString(item.Value));
            }

            result = result.Remove(result.Length - 1, 1);

            return result.ToString();
        }

        public static IDictionary<string, string> DecodeHttpForm(this string httpForm)
        {
            var result = new Dictionary<string, string>();
            foreach (var claim in httpForm.Split('&'))
            {
                string[] pair = claim.Split('=');
                if (pair.Length == 2)
                {
                    result.Add(Uri.UnescapeDataString(pair[0]), Uri.UnescapeDataString(pair[1]));
                }
            }

            return result;
        }
    }
}
