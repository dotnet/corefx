// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Globalization.Tests
{
    public abstract class GregorianCalendarTestBase : CalendarTestBase
    {
        public abstract GregorianCalendarTypes CalendarType { get; }

        public override Calendar Calendar => new GregorianCalendar(CalendarType);
    }

    public class GregorianCalendarDefaultTests : GregorianCalendarTestBase
    {
        public override Calendar Calendar => new GregorianCalendar();

        public override GregorianCalendarTypes CalendarType => GregorianCalendarTypes.Localized;
    }

    public class GregorianCalendarArabicTests : GregorianCalendarTestBase
    {
        public override GregorianCalendarTypes CalendarType => GregorianCalendarTypes.Arabic;
    }

    public class GregorianCalendarLocalizedTests : GregorianCalendarTestBase
    {
        public override GregorianCalendarTypes CalendarType => GregorianCalendarTypes.Localized;
    }

    public class GregorianCalendarMiddleEastFrenchTests : GregorianCalendarTestBase
    {
        public override GregorianCalendarTypes CalendarType => GregorianCalendarTypes.MiddleEastFrench;
    }

    public class GregorianCalendarTransliteratedEnglishTests : GregorianCalendarTestBase
    {
        public override GregorianCalendarTypes CalendarType => GregorianCalendarTypes.TransliteratedEnglish;
    }

    public class GregorianCalendarTransliteratedFrenchTests : GregorianCalendarTestBase
    {
        public override GregorianCalendarTypes CalendarType => GregorianCalendarTypes.TransliteratedFrench;
    }

    public class GregorianCalendarUSEnglishTests : GregorianCalendarTestBase
    {
        public override GregorianCalendarTypes CalendarType => GregorianCalendarTypes.USEnglish;
    }
}
