// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System
{
    // Remove Shim once System.Runtime.Hosting has been implemented.
    internal class AppDomain
    {
        private static AppDomain currentDomain = new AppDomain();

        public static AppDomain CurrentDomain
        {
            get { return currentDomain; }
        }

        public bool IsFinalizingForUnload()
        {
            return false;
        }
    }
}

