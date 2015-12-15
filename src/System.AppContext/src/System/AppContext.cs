// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using Windows.ApplicationModel;

namespace System
{
    public static class AppContext
    {
        [Flags]
        private enum SwitchValueState
        {
            HasFalseValue = 0x1,
            HasTrueValue = 0x2,
            HasLookedForOverride = 0x4, // Not used on .NET Native
            UnknownValue = 0x8 // Not used on .NET Native
        }

        private static Dictionary<string, SwitchValueState> s_switchMap = new Dictionary<string, SwitchValueState>();
        private static readonly object s_syncLock = new object();

        public static string BaseDirectory
        {
            get
            {
                return Package.Current.InstalledLocation.Path;
            }
        }

        public static string TargetFrameworkName
        {
            get
            {
                // Forward the value that is set on the current domain.
                return AppDomain.CurrentDomain.SetupInformation.TargetFrameworkName;
            }
        }

        /// <summary>
        /// Try to get the value of the switch.
        /// </summary>
        /// <param name="switchName">The name of the switch</param>
        /// <param name="isEnabled">A variable where to place the value of the switch</param>
        /// <returns>A return value of true represents that the switch was set and <paramref name="isEnabled"/> contains the value of the switch</returns>
        public static bool TryGetSwitch(string switchName, out bool isEnabled)
        {
            if (switchName == null)
                throw new ArgumentNullException("switchName");
            if (switchName.Length == 0)
                throw new ArgumentException(SR.Argument_EmptyName, "switchName");

            // By default, the switch is not enabled.
            isEnabled = false;

            SwitchValueState switchValue;
            lock (s_switchMap)
            {
                if (s_switchMap.TryGetValue(switchName, out switchValue))
                {
                    // We get the value of isEnabled from the value that we stored in the dictionary
                    isEnabled = (switchValue & SwitchValueState.HasTrueValue) == SwitchValueState.HasTrueValue;
                    return true;
                }
            }

            return false; // we did not find a value for the switch
        }

        /// <summary>
        /// Assign a switch a value
        /// </summary>
        /// <param name="switchName">The name of the switch</param>
        /// <param name="isEnabled">The value to assign</param>
        public static void SetSwitch(string switchName, bool isEnabled)
        {
            if (switchName == null)
                throw new ArgumentNullException("switchName");
            if (switchName.Length == 0)
                throw new ArgumentException(SR.Argument_EmptyName, "switchName");

            lock (s_syncLock)
            {
                // Store the new value and the fact that we checked in the dictionary
                s_switchMap[switchName] = (isEnabled ? SwitchValueState.HasTrueValue : SwitchValueState.HasFalseValue);
            }
        }
    }
}