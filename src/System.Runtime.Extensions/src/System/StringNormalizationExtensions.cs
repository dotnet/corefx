// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text;

namespace System
{
    public static partial class StringNormalizationExtensions
    {
        public static bool IsNormalized(this string strInput)
        {
            return strInput.IsNormalized(NormalizationForm.FormC);
        }

        public static bool IsNormalized(this string strInput, NormalizationForm normalizationForm)
        {
            return strInput.IsNormalized(normalizationForm);
        }

        public static string Normalize(this string strInput)
        {
            // Default to Form C
            return strInput.Normalize(NormalizationForm.FormC);
        }

        public static string Normalize(this string strInput, NormalizationForm normalizationForm)
        {
            return strInput.Normalize(normalizationForm);
        }
    }
}
