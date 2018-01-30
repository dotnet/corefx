// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security;
using System.Text;
using System.Threading;

using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;

using Internal.Runtime.CompilerServices;

using REG_TZI_FORMAT = Interop.Kernel32.REG_TZI_FORMAT;
using TIME_ZONE_INFORMATION = Interop.Kernel32.TIME_ZONE_INFORMATION;
using TIME_DYNAMIC_ZONE_INFORMATION = Interop.Kernel32.TIME_DYNAMIC_ZONE_INFORMATION;

namespace System
{
    public sealed partial class TimeZoneInfo
    {
        // registry constants for the 'Time Zones' hive
        //
        private const string TimeZonesRegistryHive = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Time Zones";
        private const string DisplayValue = "Display";
        private const string DaylightValue = "Dlt";
        private const string StandardValue = "Std";
        private const string MuiDisplayValue = "MUI_Display";
        private const string MuiDaylightValue = "MUI_Dlt";
        private const string MuiStandardValue = "MUI_Std";
        private const string TimeZoneInfoValue = "TZI";
        private const string FirstEntryValue = "FirstEntry";
        private const string LastEntryValue = "LastEntry";

        private const int MaxKeyLength = 255;

#pragma warning disable 0420
        private sealed partial class CachedData
        {
            private static TimeZoneInfo GetCurrentOneYearLocal()
            {
                // load the data from the OS
                TIME_ZONE_INFORMATION timeZoneInformation;
                uint result = Interop.Kernel32.GetTimeZoneInformation(out timeZoneInformation);
                return result == Interop.Kernel32.TIME_ZONE_ID_INVALID ?
                    CreateCustomTimeZone(LocalId, TimeSpan.Zero, LocalId, LocalId) :
                    GetLocalTimeZoneFromWin32Data(timeZoneInformation, dstDisabled: false);
            }

            private volatile OffsetAndRule _oneYearLocalFromUtc;

            public OffsetAndRule GetOneYearLocalFromUtc(int year)
            {
                OffsetAndRule oneYearLocFromUtc = _oneYearLocalFromUtc;
                if (oneYearLocFromUtc == null || oneYearLocFromUtc.Year != year)
                {
                    TimeZoneInfo currentYear = GetCurrentOneYearLocal();
                    AdjustmentRule rule = currentYear._adjustmentRules == null ? null : currentYear._adjustmentRules[0];
                    oneYearLocFromUtc = new OffsetAndRule(year, currentYear.BaseUtcOffset, rule);
                    _oneYearLocalFromUtc = oneYearLocFromUtc;
                }
                return oneYearLocFromUtc;
            }
        }
#pragma warning restore 0420

        private sealed class OffsetAndRule
        {
            public readonly int Year;
            public readonly TimeSpan Offset;
            public readonly AdjustmentRule Rule;

            public OffsetAndRule(int year, TimeSpan offset, AdjustmentRule rule)
            {
                Year = year;
                Offset = offset;
                Rule = rule;
            }
        }

        /// <summary>
        /// Returns a cloned array of AdjustmentRule objects
        /// </summary>
        public AdjustmentRule[] GetAdjustmentRules()
        {
            if (_adjustmentRules == null)
            {
                return Array.Empty<AdjustmentRule>();
            }

            return (AdjustmentRule[])_adjustmentRules.Clone();
        }

        private static void PopulateAllSystemTimeZones(CachedData cachedData)
        {
            Debug.Assert(Monitor.IsEntered(cachedData));

            using (RegistryKey reg = Registry.LocalMachine.OpenSubKey(TimeZonesRegistryHive, writable: false))
            {
                if (reg != null)
                {
                    foreach (string keyName in reg.GetSubKeyNames())
                    {
                        TimeZoneInfo value;
                        Exception ex;
                        TryGetTimeZone(keyName, false, out value, out ex, cachedData);  // populate the cache
                    }
                }
            }
        }

        private TimeZoneInfo(in TIME_ZONE_INFORMATION zone, bool dstDisabled)
        {
            string standardName = zone.GetStandardName();
            if (standardName.Length == 0)
            {
                _id = LocalId;  // the ID must contain at least 1 character - initialize _id to "Local"
            }
            else
            {
                _id = standardName;
            }
            _baseUtcOffset = new TimeSpan(0, -(zone.Bias), 0);

            if (!dstDisabled)
            {
                // only create the adjustment rule if DST is enabled
                REG_TZI_FORMAT regZone = new REG_TZI_FORMAT(zone);
                AdjustmentRule rule = CreateAdjustmentRuleFromTimeZoneInformation(regZone, DateTime.MinValue.Date, DateTime.MaxValue.Date, zone.Bias);
                if (rule != null)
                {
                    _adjustmentRules = new[] { rule };
                }
            }

            ValidateTimeZoneInfo(_id, _baseUtcOffset, _adjustmentRules, out _supportsDaylightSavingTime);
            _displayName = standardName;
            _standardDisplayName = standardName;
            _daylightDisplayName = zone.GetDaylightName();
        }

        /// <summary>
        /// Helper function to check if the current TimeZoneInformation struct does not support DST.
        /// This check returns true when the DaylightDate == StandardDate.
        /// This check is only meant to be used for "Local".
        /// </summary>
        private static bool CheckDaylightSavingTimeNotSupported(in TIME_ZONE_INFORMATION timeZone) =>
            timeZone.DaylightDate.Equals(timeZone.StandardDate);

        /// <summary>
        /// Converts a REG_TZI_FORMAT struct to an AdjustmentRule.
        /// </summary>
        private static AdjustmentRule CreateAdjustmentRuleFromTimeZoneInformation(in REG_TZI_FORMAT timeZoneInformation, DateTime startDate, DateTime endDate, int defaultBaseUtcOffset)
        {
            bool supportsDst = timeZoneInformation.StandardDate.Month != 0;

            if (!supportsDst)
            {
                if (timeZoneInformation.Bias == defaultBaseUtcOffset)
                {
                    // this rule will not contain any information to be used to adjust dates. just ignore it
                    return null;
                }

                return AdjustmentRule.CreateAdjustmentRule(
                    startDate,
                    endDate,
                    TimeSpan.Zero, // no daylight saving transition
                    TransitionTime.CreateFixedDateRule(DateTime.MinValue, 1, 1),
                    TransitionTime.CreateFixedDateRule(DateTime.MinValue.AddMilliseconds(1), 1, 1),
                    new TimeSpan(0, defaultBaseUtcOffset - timeZoneInformation.Bias, 0),  // Bias delta is all what we need from this rule
                    noDaylightTransitions: false);
            }

            //
            // Create an AdjustmentRule with TransitionTime objects
            //
            TransitionTime daylightTransitionStart;
            if (!TransitionTimeFromTimeZoneInformation(timeZoneInformation, out daylightTransitionStart, readStartDate: true))
            {
                return null;
            }

            TransitionTime daylightTransitionEnd;
            if (!TransitionTimeFromTimeZoneInformation(timeZoneInformation, out daylightTransitionEnd, readStartDate: false))
            {
                return null;
            }

            if (daylightTransitionStart.Equals(daylightTransitionEnd))
            {
                // this happens when the time zone does support DST but the OS has DST disabled
                return null;
            }

            return AdjustmentRule.CreateAdjustmentRule(
                startDate,
                endDate,
                new TimeSpan(0, -timeZoneInformation.DaylightBias, 0),
                daylightTransitionStart,
                daylightTransitionEnd,
                new TimeSpan(0, defaultBaseUtcOffset - timeZoneInformation.Bias, 0),
                noDaylightTransitions: false);
        }

        /// <summary>
        /// Helper function that searches the registry for a time zone entry
        /// that matches the TimeZoneInformation struct.
        /// </summary>
        private static string FindIdFromTimeZoneInformation(in TIME_ZONE_INFORMATION timeZone, out bool dstDisabled)
        {
            dstDisabled = false;

            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(TimeZonesRegistryHive, writable: false))
            {
                if (key == null)
                {
                    return null;
                }

                foreach (string keyName in key.GetSubKeyNames())
                {
                    if (TryCompareTimeZoneInformationToRegistry(timeZone, keyName, out dstDisabled))
                    {
                        return keyName;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Helper function for retrieving the local system time zone.
        /// May throw COMException, TimeZoneNotFoundException, InvalidTimeZoneException.
        /// Assumes cachedData lock is taken.
        /// </summary>
        /// <returns>A new TimeZoneInfo instance.</returns>
        private static TimeZoneInfo GetLocalTimeZone(CachedData cachedData)
        {
            Debug.Assert(Monitor.IsEntered(cachedData));

            //
            // Try using the "kernel32!GetDynamicTimeZoneInformation" API to get the "id"
            //
            var dynamicTimeZoneInformation = new TIME_DYNAMIC_ZONE_INFORMATION();

            // call kernel32!GetDynamicTimeZoneInformation...
            uint result = Interop.Kernel32.GetDynamicTimeZoneInformation(out dynamicTimeZoneInformation);
            if (result == Interop.Kernel32.TIME_ZONE_ID_INVALID)
            {
                // return a dummy entry
                return CreateCustomTimeZone(LocalId, TimeSpan.Zero, LocalId, LocalId);
            }

            // check to see if we can use the key name returned from the API call
            string dynamicTimeZoneKeyName = dynamicTimeZoneInformation.GetTimeZoneKeyName();
            if (dynamicTimeZoneKeyName.Length != 0)
            {
                TimeZoneInfo zone;
                Exception ex;

                if (TryGetTimeZone(dynamicTimeZoneKeyName, dynamicTimeZoneInformation.DynamicDaylightTimeDisabled != 0, out zone, out ex, cachedData) == TimeZoneInfoResult.Success)
                {
                    // successfully loaded the time zone from the registry
                    return zone;
                }
            }

            var timeZoneInformation = new TIME_ZONE_INFORMATION(dynamicTimeZoneInformation);

            // the key name was not returned or it pointed to a bogus entry - search for the entry ourselves
            string id = FindIdFromTimeZoneInformation(timeZoneInformation, out bool dstDisabled);

            if (id != null)
            {
                TimeZoneInfo zone;
                Exception ex;
                if (TryGetTimeZone(id, dstDisabled, out zone, out ex, cachedData) == TimeZoneInfoResult.Success)
                {
                    // successfully loaded the time zone from the registry
                    return zone;
                }
            }

            // We could not find the data in the registry.  Fall back to using
            // the data from the Win32 API
            return GetLocalTimeZoneFromWin32Data(timeZoneInformation, dstDisabled);
        }

        /// <summary>
        /// Helper function used by 'GetLocalTimeZone()' - this function wraps a bunch of
        /// try/catch logic for handling the TimeZoneInfo private constructor that takes
        /// a TIME_ZONE_INFORMATION structure.
        /// </summary>
        private static TimeZoneInfo GetLocalTimeZoneFromWin32Data(in TIME_ZONE_INFORMATION timeZoneInformation, bool dstDisabled)
        {
            // first try to create the TimeZoneInfo with the original 'dstDisabled' flag
            try
            {
                return new TimeZoneInfo(timeZoneInformation, dstDisabled);
            }
            catch (ArgumentException) { }
            catch (InvalidTimeZoneException) { }

            // if 'dstDisabled' was false then try passing in 'true' as a last ditch effort
            if (!dstDisabled)
            {
                try
                {
                    return new TimeZoneInfo(timeZoneInformation, dstDisabled: true);
                }
                catch (ArgumentException) { }
                catch (InvalidTimeZoneException) { }
            }

            // the data returned from Windows is completely bogus; return a dummy entry
            return CreateCustomTimeZone(LocalId, TimeSpan.Zero, LocalId, LocalId);
        }

        /// <summary>
        /// Helper function for retrieving a TimeZoneInfo object by <time_zone_name>.
        /// This function wraps the logic necessary to keep the private
        /// SystemTimeZones cache in working order
        ///
        /// This function will either return a valid TimeZoneInfo instance or
        /// it will throw 'InvalidTimeZoneException' / 'TimeZoneNotFoundException'.
        /// </summary>
        public static TimeZoneInfo FindSystemTimeZoneById(string id)
        {
            // Special case for Utc as it will not exist in the dictionary with the rest
            // of the system time zones.  There is no need to do this check for Local.Id
            // since Local is a real time zone that exists in the dictionary cache
            if (string.Equals(id, UtcId, StringComparison.OrdinalIgnoreCase))
            {
                return Utc;
            }

            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            if (id.Length == 0 || id.Length > MaxKeyLength || id.Contains('\0'))
            {
                throw new TimeZoneNotFoundException(SR.Format(SR.TimeZoneNotFound_MissingData, id));
            }

            TimeZoneInfo value;
            Exception e;

            TimeZoneInfoResult result;

            CachedData cachedData = s_cachedData;

            lock (cachedData)
            {
                result = TryGetTimeZone(id, false, out value, out e, cachedData);
            }

            if (result == TimeZoneInfoResult.Success)
            {
                return value;
            }
            else if (result == TimeZoneInfoResult.InvalidTimeZoneException)
            {
                throw new InvalidTimeZoneException(SR.Format(SR.InvalidTimeZone_InvalidRegistryData, id), e);
            }
            else if (result == TimeZoneInfoResult.SecurityException)
            {
                throw new SecurityException(SR.Format(SR.Security_CannotReadRegistryData, id), e);
            }
            else
            {
                throw new TimeZoneNotFoundException(SR.Format(SR.TimeZoneNotFound_MissingData, id), e);
            }
        }

        // DateTime.Now fast path that avoids allocating an historically accurate TimeZoneInfo.Local and just creates a 1-year (current year) accurate time zone
        internal static TimeSpan GetDateTimeNowUtcOffsetFromUtc(DateTime time, out bool isAmbiguousLocalDst)
        {
            bool isDaylightSavings = false;
            isAmbiguousLocalDst = false;
            TimeSpan baseOffset;
            int timeYear = time.Year;

            OffsetAndRule match = s_cachedData.GetOneYearLocalFromUtc(timeYear);
            baseOffset = match.Offset;

            if (match.Rule != null)
            {
                baseOffset = baseOffset + match.Rule.BaseUtcOffsetDelta;
                if (match.Rule.HasDaylightSaving)
                {
                    isDaylightSavings = GetIsDaylightSavingsFromUtc(time, timeYear, match.Offset, match.Rule, null, out isAmbiguousLocalDst, Local);
                    baseOffset += (isDaylightSavings ? match.Rule.DaylightDelta : TimeSpan.Zero /* FUTURE: rule.StandardDelta */);
                }
            }
            return baseOffset;
        }

        /// <summary>
        /// Converts a REG_TZI_FORMAT struct to a TransitionTime
        /// - When the argument 'readStart' is true the corresponding daylightTransitionTimeStart field is read
        /// - When the argument 'readStart' is false the corresponding dayightTransitionTimeEnd field is read
        /// </summary>
        private static bool TransitionTimeFromTimeZoneInformation(in REG_TZI_FORMAT timeZoneInformation, out TransitionTime transitionTime, bool readStartDate)
        {
            //
            // SYSTEMTIME -
            //
            // If the time zone does not support daylight saving time or if the caller needs
            // to disable daylight saving time, the wMonth member in the SYSTEMTIME structure
            // must be zero. If this date is specified, the DaylightDate value in the
            // TIME_ZONE_INFORMATION structure must also be specified. Otherwise, the system
            // assumes the time zone data is invalid and no changes will be applied.
            //
            bool supportsDst = (timeZoneInformation.StandardDate.Month != 0);

            if (!supportsDst)
            {
                transitionTime = default(TransitionTime);
                return false;
            }

            //
            // SYSTEMTIME -
            //
            // * FixedDateRule -
            //   If the Year member is not zero, the transition date is absolute; it will only occur one time
            //
            // * FloatingDateRule -
            //   To select the correct day in the month, set the Year member to zero, the Hour and Minute
            //   members to the transition time, the DayOfWeek member to the appropriate weekday, and the
            //   Day member to indicate the occurence of the day of the week within the month (first through fifth).
            //
            //   Using this notation, specify the 2:00a.m. on the first Sunday in April as follows:
            //   Hour      = 2,
            //   Month     = 4,
            //   DayOfWeek = 0,
            //   Day       = 1.
            //
            //   Specify 2:00a.m. on the last Thursday in October as follows:
            //   Hour      = 2,
            //   Month     = 10,
            //   DayOfWeek = 4,
            //   Day       = 5.
            //
            if (readStartDate)
            {
                //
                // read the "daylightTransitionStart"
                //
                if (timeZoneInformation.DaylightDate.Year == 0)
                {
                    transitionTime = TransitionTime.CreateFloatingDateRule(
                                     new DateTime(1,    /* year  */
                                                  1,    /* month */
                                                  1,    /* day   */
                                                  timeZoneInformation.DaylightDate.Hour,
                                                  timeZoneInformation.DaylightDate.Minute,
                                                  timeZoneInformation.DaylightDate.Second,
                                                  timeZoneInformation.DaylightDate.Milliseconds),
                                     timeZoneInformation.DaylightDate.Month,
                                     timeZoneInformation.DaylightDate.Day,   /* Week 1-5 */
                                     (DayOfWeek)timeZoneInformation.DaylightDate.DayOfWeek);
                }
                else
                {
                    transitionTime = TransitionTime.CreateFixedDateRule(
                                     new DateTime(1,    /* year  */
                                                  1,    /* month */
                                                  1,    /* day   */
                                                  timeZoneInformation.DaylightDate.Hour,
                                                  timeZoneInformation.DaylightDate.Minute,
                                                  timeZoneInformation.DaylightDate.Second,
                                                  timeZoneInformation.DaylightDate.Milliseconds),
                                     timeZoneInformation.DaylightDate.Month,
                                     timeZoneInformation.DaylightDate.Day);
                }
            }
            else
            {
                //
                // read the "daylightTransitionEnd"
                //
                if (timeZoneInformation.StandardDate.Year == 0)
                {
                    transitionTime = TransitionTime.CreateFloatingDateRule(
                                     new DateTime(1,    /* year  */
                                                  1,    /* month */
                                                  1,    /* day   */
                                                  timeZoneInformation.StandardDate.Hour,
                                                  timeZoneInformation.StandardDate.Minute,
                                                  timeZoneInformation.StandardDate.Second,
                                                  timeZoneInformation.StandardDate.Milliseconds),
                                     timeZoneInformation.StandardDate.Month,
                                     timeZoneInformation.StandardDate.Day,   /* Week 1-5 */
                                     (DayOfWeek)timeZoneInformation.StandardDate.DayOfWeek);
                }
                else
                {
                    transitionTime = TransitionTime.CreateFixedDateRule(
                                     new DateTime(1,    /* year  */
                                                  1,    /* month */
                                                  1,    /* day   */
                                                  timeZoneInformation.StandardDate.Hour,
                                                  timeZoneInformation.StandardDate.Minute,
                                                  timeZoneInformation.StandardDate.Second,
                                                  timeZoneInformation.StandardDate.Milliseconds),
                                     timeZoneInformation.StandardDate.Month,
                                     timeZoneInformation.StandardDate.Day);
                }
            }

            return true;
        }

        /// <summary>
        /// Helper function that takes:
        ///  1. A string representing a <time_zone_name> registry key name.
        ///  2. A REG_TZI_FORMAT struct containing the default rule.
        ///  3. An AdjustmentRule[] out-parameter.
        /// </summary>
        private static bool TryCreateAdjustmentRules(string id, in REG_TZI_FORMAT defaultTimeZoneInformation, out AdjustmentRule[] rules, out Exception e, int defaultBaseUtcOffset)
        {
            rules = null;
            e = null;

            try
            {
                // Optional, Dynamic Time Zone Registry Data
                // -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
                //
                // HKLM
                //     Software
                //         Microsoft
                //             Windows NT
                //                 CurrentVersion
                //                     Time Zones
                //                         <time_zone_name>
                //                             Dynamic DST
                // * "FirstEntry" REG_DWORD "1980"
                //                           First year in the table. If the current year is less than this value,
                //                           this entry will be used for DST boundaries
                // * "LastEntry"  REG_DWORD "2038"
                //                           Last year in the table. If the current year is greater than this value,
                //                           this entry will be used for DST boundaries"
                // * "<year1>"    REG_BINARY REG_TZI_FORMAT
                // * "<year2>"    REG_BINARY REG_TZI_FORMAT
                // * "<year3>"    REG_BINARY REG_TZI_FORMAT
                //
                using (RegistryKey dynamicKey = Registry.LocalMachine.OpenSubKey(TimeZonesRegistryHive + "\\" + id + "\\Dynamic DST", writable: false))
                {
                    if (dynamicKey == null)
                    {
                        AdjustmentRule rule = CreateAdjustmentRuleFromTimeZoneInformation(
                            defaultTimeZoneInformation, DateTime.MinValue.Date, DateTime.MaxValue.Date, defaultBaseUtcOffset);
                        if (rule != null)
                        {
                            rules = new[] { rule };
                        }
                        return true;
                    }

                    //
                    // loop over all of the "<time_zone_name>\Dynamic DST" hive entries
                    //
                    // read FirstEntry  {MinValue      - (year1, 12, 31)}
                    // read MiddleEntry {(yearN, 1, 1) - (yearN, 12, 31)}
                    // read LastEntry   {(yearN, 1, 1) - MaxValue       }

                    // read the FirstEntry and LastEntry key values (ex: "1980", "2038")
                    int first = (int)dynamicKey.GetValue(FirstEntryValue, -1, RegistryValueOptions.None);
                    int last = (int)dynamicKey.GetValue(LastEntryValue, -1, RegistryValueOptions.None);

                    if (first == -1 || last == -1 || first > last)
                    {
                        return false;
                    }

                    // read the first year entry
                    REG_TZI_FORMAT dtzi;

                    if (!TryGetTimeZoneEntryFromRegistry(dynamicKey, first.ToString(CultureInfo.InvariantCulture), out dtzi))
                    {
                        return false;
                    }

                    if (first == last)
                    {
                        // there is just 1 dynamic rule for this time zone.
                        AdjustmentRule rule = CreateAdjustmentRuleFromTimeZoneInformation(dtzi, DateTime.MinValue.Date, DateTime.MaxValue.Date, defaultBaseUtcOffset);
                        if (rule != null)
                        {
                            rules = new[] { rule };
                        }
                        return true;
                    }

                    List<AdjustmentRule> rulesList = new List<AdjustmentRule>(1);

                    // there are more than 1 dynamic rules for this time zone.
                    AdjustmentRule firstRule = CreateAdjustmentRuleFromTimeZoneInformation(
                        dtzi,
                        DateTime.MinValue.Date,        // MinValue
                        new DateTime(first, 12, 31),   // December 31, <FirstYear>
                        defaultBaseUtcOffset);

                    if (firstRule != null)
                    {
                        rulesList.Add(firstRule);
                    }

                    // read the middle year entries
                    for (int i = first + 1; i < last; i++)
                    {
                        if (!TryGetTimeZoneEntryFromRegistry(dynamicKey, i.ToString(CultureInfo.InvariantCulture), out dtzi))
                        {
                            return false;
                        }
                        AdjustmentRule middleRule = CreateAdjustmentRuleFromTimeZoneInformation(
                            dtzi,
                            new DateTime(i, 1, 1),    // January  01, <Year>
                            new DateTime(i, 12, 31),  // December 31, <Year>
                            defaultBaseUtcOffset);

                        if (middleRule != null)
                        {
                            rulesList.Add(middleRule);
                        }
                    }

                    // read the last year entry
                    if (!TryGetTimeZoneEntryFromRegistry(dynamicKey, last.ToString(CultureInfo.InvariantCulture), out dtzi))
                    {
                        return false;
                    }
                    AdjustmentRule lastRule = CreateAdjustmentRuleFromTimeZoneInformation(
                        dtzi,
                        new DateTime(last, 1, 1),    // January  01, <LastYear>
                        DateTime.MaxValue.Date,      // MaxValue
                        defaultBaseUtcOffset);

                    if (lastRule != null)
                    {
                        rulesList.Add(lastRule);
                    }

                    // convert the List to an AdjustmentRule array
                    if (rulesList.Count != 0)
                    {
                        rules = rulesList.ToArray();
                    }
                } // end of: using (RegistryKey dynamicKey...
            }
            catch (InvalidCastException ex)
            {
                // one of the RegistryKey.GetValue calls could not be cast to an expected value type
                e = ex;
                return false;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                e = ex;
                return false;
            }
            catch (ArgumentException ex)
            {
                e = ex;
                return false;
            }
            return true;
        }

        private unsafe static bool TryGetTimeZoneEntryFromRegistry(RegistryKey key, string name, out REG_TZI_FORMAT dtzi)
        {
            byte[] regValue = key.GetValue(name, null, RegistryValueOptions.None) as byte[];
            if (regValue == null || regValue.Length != sizeof(REG_TZI_FORMAT))
            {
                dtzi = default;
                return false;
            }
            fixed (byte * pBytes = &regValue[0])
                dtzi = *(REG_TZI_FORMAT *)pBytes;
            return true;
        }

        /// <summary>
        /// Helper function that compares the StandardBias and StandardDate portion a
        /// TimeZoneInformation struct to a time zone registry entry.
        /// </summary>
        private static bool TryCompareStandardDate(in TIME_ZONE_INFORMATION timeZone, in REG_TZI_FORMAT registryTimeZoneInfo) =>
            timeZone.Bias == registryTimeZoneInfo.Bias &&
            timeZone.StandardBias == registryTimeZoneInfo.StandardBias &&
            timeZone.StandardDate.Equals(registryTimeZoneInfo.StandardDate);

        /// <summary>
        /// Helper function that compares a TimeZoneInformation struct to a time zone registry entry.
        /// </summary>
        private static bool TryCompareTimeZoneInformationToRegistry(in TIME_ZONE_INFORMATION timeZone, string id, out bool dstDisabled)
        {
            dstDisabled = false;

            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(TimeZonesRegistryHive + "\\" + id, writable: false))
            {
                if (key == null)
                {
                    return false;
                }

                REG_TZI_FORMAT registryTimeZoneInfo;
                if (!TryGetTimeZoneEntryFromRegistry(key, TimeZoneInfoValue, out registryTimeZoneInfo))
                {
                    return false;
                }

                //
                // first compare the bias and standard date information between the data from the Win32 API
                // and the data from the registry...
                //
                bool result = TryCompareStandardDate(timeZone, registryTimeZoneInfo);

                if (!result)
                {
                    return false;
                }

                result = dstDisabled || CheckDaylightSavingTimeNotSupported(timeZone) ||
                    //
                    // since Daylight Saving Time is not "disabled", do a straight comparision between
                    // the Win32 API data and the registry data ...
                    //
                    (timeZone.DaylightBias == registryTimeZoneInfo.DaylightBias &&
                    timeZone.DaylightDate.Equals(registryTimeZoneInfo.DaylightDate));

                // Finally compare the "StandardName" string value...
                //
                // we do not compare "DaylightName" as this TimeZoneInformation field may contain
                // either "StandardName" or "DaylightName" depending on the time of year and current machine settings
                //
                if (result)
                {
                    string registryStandardName = key.GetValue(StandardValue, string.Empty, RegistryValueOptions.None) as string;
                    result = string.Equals(registryStandardName, timeZone.GetStandardName(), StringComparison.Ordinal);
                }
                return result;
            }
        }

        /// <summary>
        /// Helper function for retrieving a localized string resource via MUI.
        /// The function expects a string in the form: "@resource.dll, -123"
        ///
        /// "resource.dll" is a language-neutral portable executable (LNPE) file in
        /// the %windir%\system32 directory.  The OS is queried to find the best-fit
        /// localized resource file for this LNPE (ex: %windir%\system32\en-us\resource.dll.mui).
        /// If a localized resource file exists, we LoadString resource ID "123" and
        /// return it to our caller.
        /// </summary>
        private static string TryGetLocalizedNameByMuiNativeResource(string resource)
        {
            if (string.IsNullOrEmpty(resource))
            {
                return string.Empty;
            }

            // parse "@tzres.dll, -100"
            //
            // filePath   = "C:\Windows\System32\tzres.dll"
            // resourceId = -100
            //
            string[] resources = resource.Split(',');
            if (resources.Length != 2)
            {
                return string.Empty;
            }

            string filePath;
            int resourceId;

            // get the path to Windows\System32
            string system32 = Environment.SystemDirectory;

            // trim the string "@tzres.dll" => "tzres.dll"
            string tzresDll = resources[0].TrimStart('@');

            try
            {
                filePath = Path.Combine(system32, tzresDll);
            }
            catch (ArgumentException)
            {
                // there were probably illegal characters in the path
                return string.Empty;
            }

            if (!int.TryParse(resources[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out resourceId))
            {
                return string.Empty;
            }
            resourceId = -resourceId;

            try
            {
                StringBuilder fileMuiPath = StringBuilderCache.Acquire(Interop.Kernel32.MAX_PATH);
                fileMuiPath.Length = Interop.Kernel32.MAX_PATH;
                int fileMuiPathLength = Interop.Kernel32.MAX_PATH;
                int languageLength = 0;
                long enumerator = 0;

                bool succeeded = Interop.Kernel32.GetFileMUIPath(
                                        Interop.Kernel32.MUI_PREFERRED_UI_LANGUAGES,
                                        filePath, null /* language */, ref languageLength,
                                        fileMuiPath, ref fileMuiPathLength, ref enumerator);
                if (!succeeded)
                {
                    StringBuilderCache.Release(fileMuiPath);
                    return string.Empty;
                }
                return TryGetLocalizedNameByNativeResource(StringBuilderCache.GetStringAndRelease(fileMuiPath), resourceId);
            }
            catch (EntryPointNotFoundException)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Helper function for retrieving a localized string resource via a native resource DLL.
        /// The function expects a string in the form: "C:\Windows\System32\en-us\resource.dll"
        ///
        /// "resource.dll" is a language-specific resource DLL.
        /// If the localized resource DLL exists, LoadString(resource) is returned.
        /// </summary>
        private static string TryGetLocalizedNameByNativeResource(string filePath, int resource)
        {
            using (SafeLibraryHandle handle =
                       Interop.Kernel32.LoadLibraryEx(filePath, IntPtr.Zero, Interop.Kernel32.LOAD_LIBRARY_AS_DATAFILE))
            {
                if (!handle.IsInvalid)
                {
                    const int LoadStringMaxLength = 500;

                    StringBuilder localizedResource = StringBuilderCache.Acquire(LoadStringMaxLength);

                    int result = Interop.User32.LoadString(handle, resource,
                                     localizedResource, LoadStringMaxLength);

                    if (result != 0)
                    {
                        return StringBuilderCache.GetStringAndRelease(localizedResource);
                    }
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Helper function for retrieving the DisplayName, StandardName, and DaylightName from the registry
        ///
        /// The function first checks the MUI_ key-values, and if they exist, it loads the strings from the MUI
        /// resource dll(s).  When the keys do not exist, the function falls back to reading from the standard
        /// key-values
        /// </summary>
        private static void GetLocalizedNamesByRegistryKey(RegistryKey key, out string displayName, out string standardName, out string daylightName)
        {
            displayName = string.Empty;
            standardName = string.Empty;
            daylightName = string.Empty;

            // read the MUI_ registry keys
            string displayNameMuiResource = key.GetValue(MuiDisplayValue, string.Empty, RegistryValueOptions.None) as string;
            string standardNameMuiResource = key.GetValue(MuiStandardValue, string.Empty, RegistryValueOptions.None) as string;
            string daylightNameMuiResource = key.GetValue(MuiDaylightValue, string.Empty, RegistryValueOptions.None) as string;

            // try to load the strings from the native resource DLL(s)
            if (!string.IsNullOrEmpty(displayNameMuiResource))
            {
                displayName = TryGetLocalizedNameByMuiNativeResource(displayNameMuiResource);
            }

            if (!string.IsNullOrEmpty(standardNameMuiResource))
            {
                standardName = TryGetLocalizedNameByMuiNativeResource(standardNameMuiResource);
            }

            if (!string.IsNullOrEmpty(daylightNameMuiResource))
            {
                daylightName = TryGetLocalizedNameByMuiNativeResource(daylightNameMuiResource);
            }

            // fallback to using the standard registry keys
            if (string.IsNullOrEmpty(displayName))
            {
                displayName = key.GetValue(DisplayValue, string.Empty, RegistryValueOptions.None) as string;
            }
            if (string.IsNullOrEmpty(standardName))
            {
                standardName = key.GetValue(StandardValue, string.Empty, RegistryValueOptions.None) as string;
            }
            if (string.IsNullOrEmpty(daylightName))
            {
                daylightName = key.GetValue(DaylightValue, string.Empty, RegistryValueOptions.None) as string;
            }
        }

        /// <summary>
        /// Helper function that takes a string representing a <time_zone_name> registry key name
        /// and returns a TimeZoneInfo instance.
        /// </summary>
        private static TimeZoneInfoResult TryGetTimeZoneFromLocalMachine(string id, out TimeZoneInfo value, out Exception e)
        {
            e = null;

            // Standard Time Zone Registry Data
            // -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
            // HKLM
            //     Software
            //         Microsoft
            //             Windows NT
            //                 CurrentVersion
            //                     Time Zones
            //                         <time_zone_name>
            // * STD,         REG_SZ "Standard Time Name"
            //                       (For OS installed zones, this will always be English)
            // * MUI_STD,     REG_SZ "@tzres.dll,-1234"
            //                       Indirect string to localized resource for Standard Time,
            //                       add "%windir%\system32\" after "@"
            // * DLT,         REG_SZ "Daylight Time Name"
            //                       (For OS installed zones, this will always be English)
            // * MUI_DLT,     REG_SZ "@tzres.dll,-1234"
            //                       Indirect string to localized resource for Daylight Time,
            //                       add "%windir%\system32\" after "@"
            // * Display,     REG_SZ "Display Name like (GMT-8:00) Pacific Time..."
            // * MUI_Display, REG_SZ "@tzres.dll,-1234"
            //                       Indirect string to localized resource for the Display,
            //                       add "%windir%\system32\" after "@"
            // * TZI,         REG_BINARY REG_TZI_FORMAT
            //
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(TimeZonesRegistryHive + "\\" + id, writable: false))
            {
                if (key == null)
                {
                    value = null;
                    return TimeZoneInfoResult.TimeZoneNotFoundException;
                }

                REG_TZI_FORMAT defaultTimeZoneInformation;
                if (!TryGetTimeZoneEntryFromRegistry(key, TimeZoneInfoValue, out defaultTimeZoneInformation))
                {
                    // the registry value could not be cast to a byte array
                    value = null;
                    return TimeZoneInfoResult.InvalidTimeZoneException;
                }

                AdjustmentRule[] adjustmentRules;
                if (!TryCreateAdjustmentRules(id, defaultTimeZoneInformation, out adjustmentRules, out e, defaultTimeZoneInformation.Bias))
                {
                    value = null;
                    return TimeZoneInfoResult.InvalidTimeZoneException;
                }

                GetLocalizedNamesByRegistryKey(key, out string displayName, out string standardName, out string daylightName);

                try
                {
                    value = new TimeZoneInfo(
                        id,
                        new TimeSpan(0, -(defaultTimeZoneInformation.Bias), 0),
                        displayName,
                        standardName,
                        daylightName,
                        adjustmentRules,
                        disableDaylightSavingTime: false);

                    return TimeZoneInfoResult.Success;
                }
                catch (ArgumentException ex)
                {
                    // TimeZoneInfo constructor can throw ArgumentException and InvalidTimeZoneException
                    value = null;
                    e = ex;
                    return TimeZoneInfoResult.InvalidTimeZoneException;
                }
                catch (InvalidTimeZoneException ex)
                {
                    // TimeZoneInfo constructor can throw ArgumentException and InvalidTimeZoneException
                    value = null;
                    e = ex;
                    return TimeZoneInfoResult.InvalidTimeZoneException;
                }
            }
        }
    }
}
