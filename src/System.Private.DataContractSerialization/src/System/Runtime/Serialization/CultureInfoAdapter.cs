// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Runtime.Serialization
{
    [DataContract(Name = "CultureInfo", Namespace = "http://schemas.datacontract.org/2004/07/System.Globalization")]
    public struct CultureInfoAdapter
    {
        [DataMember(Name = "calendar", IsRequired = true)]
        public string Calendar { get; set; }

        [DataMember(Name = "compareInfo", IsRequired = true)]
        public string CompareInfo { get; set; }

        [DataMember(Name = "dateTimeInfo", IsRequired = true)]
        public string DateTimeInfo { get; set; }

        [DataMember(Name = "m_isReadOnly", IsRequired = true)]
        public bool IsReadOnly { get; set; }

        [DataMember(Name = "m_name", IsRequired = true)]
        public string Name { get; set; }

        [DataMember(Name = "m_useUserOverride", IsRequired = true)]
        public bool UseUserOverride { get; set; }

        [DataMember(Name = "numInfo", IsRequired = true)]
        public string NumInfo { get; set; }

        [DataMember(Name = "textInfo", IsRequired = true)]
        public string TextInfo { get; set; }

        public static CultureInfo GetCultureInfo(CultureInfoAdapter value)
        {
            return new CultureInfo(value.Name);
        }

        public static CultureInfoAdapter GetCultureInfoAdapter(CultureInfo value)
        {
            return new CultureInfoAdapter()
            {
                Calendar = null,
                CompareInfo = null,
                IsReadOnly = false,
                Name = value.ToString(),
                UseUserOverride = true,
                NumInfo = null,
                TextInfo = null
            };
        }
    }
}
