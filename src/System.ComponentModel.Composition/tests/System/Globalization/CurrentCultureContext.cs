// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Globalization;
using System.Threading;

namespace System.Globalization
{
    public class CurrentCultureContext : IDisposable
    {
        private CultureInfo _previousCulture;

        public CurrentCultureContext(CultureInfo culture)
        {
            _previousCulture = Thread.CurrentThread.CurrentCulture;

            Thread.CurrentThread.CurrentCulture = culture;
        }

        public void Dispose()
        {
            if (_previousCulture != null)
            {
                Thread.CurrentThread.CurrentCulture = _previousCulture;
            }
        }
    }
}
