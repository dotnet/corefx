// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------

namespace Microsoft.VisualBasic
{
    public enum AppWinStyle : short
    {
        Hide = 0,
        MaximizedFocus = 3,
        MinimizedFocus = 2,
        MinimizedNoFocus = 6,
        NormalFocus = 1,
        NormalNoFocus = 4,
    }
    public enum CallType
    {
        Method = 1,
        Get = 2,
        Let = 4,
        Set = 8,
    }
    public sealed partial class Collection : System.Collections.ICollection, System.Collections.IEnumerable, System.Collections.IList
    {
        public Collection() { }
        public int Count { get { throw null; } }
        public object this[int Index] { get { throw null; } }
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public object this[object Index] { get { throw null; } }
        public object this[string Key] { get { throw null; } }
        int System.Collections.ICollection.Count { get { throw null; } }
        bool System.Collections.ICollection.IsSynchronized { get { throw null; } }
        object System.Collections.ICollection.SyncRoot { get { throw null; } }
        bool System.Collections.IList.IsFixedSize { get { throw null; } }
        bool System.Collections.IList.IsReadOnly { get { throw null; } }
        object System.Collections.IList.this[int index] { get { throw null; } set { } }
        public void Add(object Item, string Key = null, object Before = null, object After = null) { }
        public void Clear() { }
        public bool Contains(string Key) { throw null; }
        public System.Collections.IEnumerator GetEnumerator() { throw null; }
        public void Remove(int Index) { }
        public void Remove(string Key) { }
        void System.Collections.ICollection.CopyTo(System.Array array, int index) { }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { throw null; }
        int System.Collections.IList.Add(object value) { throw null; }
        void System.Collections.IList.Clear() { }
        bool System.Collections.IList.Contains(object value) { throw null; }
        int System.Collections.IList.IndexOf(object value) { throw null; }
        void System.Collections.IList.Insert(int index, object value) { }
        void System.Collections.IList.Remove(object value) { }
        void System.Collections.IList.RemoveAt(int index) { }
    }
    [System.AttributeUsageAttribute(System.AttributeTargets.Class, Inherited=false, AllowMultiple=false)]
    public sealed partial class ComClassAttribute : System.Attribute
    {
        public ComClassAttribute() { }
        public ComClassAttribute(string _ClassID) { }
        public ComClassAttribute(string _ClassID, string _InterfaceID) { }
        public ComClassAttribute(string _ClassID, string _InterfaceID, string _EventId) { }
        public string ClassID { get { throw null; } }
        public string EventID { get { throw null; } }
        public string InterfaceID { get { throw null; } }
        public bool InterfaceShadows { get { throw null; } set { } }
    }
    public enum CompareMethod
    {
        Binary = 0,
        Text = 1,
    }
    [Microsoft.VisualBasic.CompilerServices.StandardModuleAttribute]
    public sealed partial class Constants
    {
        internal Constants() { }
        public const MsgBoxResult vbAbort = MsgBoxResult.Abort;
        public const MsgBoxStyle vbAbortRetryIgnore = MsgBoxStyle.AbortRetryIgnore;
        public const MsgBoxStyle vbApplicationModal = MsgBoxStyle.ApplicationModal;
        public const FileAttribute vbArchive = FileAttribute.Archive;
        public const VariantType vbArray = VariantType.Array;
        public const string vbBack = "\b";
        public const Microsoft.VisualBasic.CompareMethod vbBinaryCompare = Microsoft.VisualBasic.CompareMethod.Binary;
        public const VariantType vbBoolean = VariantType.Boolean;
        public const VariantType vbByte = VariantType.Byte;
        public const MsgBoxResult vbCancel = MsgBoxResult.Cancel;
        public const string vbCr = "\r";
        public const MsgBoxStyle vbCritical = MsgBoxStyle.Critical;
        public const string vbCrLf = "\r\n";
        public const VariantType vbCurrency = VariantType.Currency;
        public const VariantType vbDate = VariantType.Date;
        public const VariantType vbDecimal = VariantType.Decimal;
        public const MsgBoxStyle vbDefaultButton1 = MsgBoxStyle.ApplicationModal;
        public const MsgBoxStyle vbDefaultButton2 = MsgBoxStyle.DefaultButton2;
        public const MsgBoxStyle vbDefaultButton3 = MsgBoxStyle.DefaultButton3;
        public const FileAttribute vbDirectory = FileAttribute.Directory;
        public const VariantType vbDouble = VariantType.Double;
        public const VariantType vbEmpty = VariantType.Empty;
        public const MsgBoxStyle vbExclamation = MsgBoxStyle.Exclamation;
        public const TriState vbFalse = TriState.False;
        public const FirstWeekOfYear vbFirstFourDays = FirstWeekOfYear.FirstFourDays;
        public const FirstWeekOfYear vbFirstFullWeek = FirstWeekOfYear.FirstFullWeek;
        public const FirstWeekOfYear vbFirstJan1 = FirstWeekOfYear.Jan1;
        public const string vbFormFeed = "\f";
        public const FirstDayOfWeek vbFriday = FirstDayOfWeek.Friday;
        public const DateFormat vbGeneralDate = DateFormat.GeneralDate;
        public const CallType vbGet = CallType.Get;
        public const AppWinStyle vbHide = AppWinStyle.Hide;
        public const FileAttribute vbHidden = FileAttribute.Hidden;
        public const VbStrConv vbHiragana = VbStrConv.Hiragana;
        public const MsgBoxStyle vbInformation = MsgBoxStyle.Information;
        public const MsgBoxResult vbIgnore = MsgBoxResult.Ignore;
        public const VariantType vbInteger = VariantType.Integer;
        public const VbStrConv vbKatakana = VbStrConv.Katakana;
        public const CallType vbLet = CallType.Let;
        public const string vbLf = "\n";
        public const VbStrConv vbLinguisticCasing = VbStrConv.LinguisticCasing;
        public const VariantType vbLong = VariantType.Long;
        public const DateFormat vbLongDate = DateFormat.LongDate;
        public const DateFormat vbLongTime = DateFormat.LongTime;
        public const VbStrConv vbLowerCase = VbStrConv.Lowercase;
        public const CallType vbMethod = CallType.Method;
        public const AppWinStyle vbMaximizedFocus = AppWinStyle.MaximizedFocus;
        public const AppWinStyle vbMinimizedFocus = AppWinStyle.MinimizedFocus;
        public const AppWinStyle vbMinimizedNoFocus = AppWinStyle.MinimizedNoFocus;
        public const FirstDayOfWeek vbMonday = FirstDayOfWeek.Monday;
        public const MsgBoxStyle vbMsgBoxHelp = MsgBoxStyle.MsgBoxHelp;
        public const MsgBoxStyle vbMsgBoxRight = MsgBoxStyle.MsgBoxRight;
        public const MsgBoxStyle vbMsgBoxRtlReading = MsgBoxStyle.MsgBoxRtlReading;
        public const MsgBoxStyle vbMsgBoxSetForeground = MsgBoxStyle.MsgBoxSetForeground;
        [System.ObsoleteAttribute("For a carriage return and line feed, use vbCrLf.  For the current platform's newline, use System.Environment.NewLine.")]
        public const VbStrConv vbNarrow = VbStrConv.Narrow;
        public const string vbNewLine = "\r\n";
        public const MsgBoxResult vbNo = MsgBoxResult.No;
        public const FileAttribute vbNormal = FileAttribute.Normal;
        public const AppWinStyle vbNormalFocus = AppWinStyle.NormalFocus;
        public const AppWinStyle vbNormalNoFocus = AppWinStyle.NormalNoFocus;
        public const VariantType vbNull = VariantType.Null;
        public const string vbNullChar = "\0";
        public const string vbNullString = null;
        public const VariantType vbObject = VariantType.Object;
        public const int vbObjectError = unchecked((int)0x80040000);
        public const MsgBoxResult vbOK = MsgBoxResult.Ok;
        public const MsgBoxStyle vbOKCancel = MsgBoxStyle.OkCancel;
        public const MsgBoxStyle vbOKOnly = MsgBoxStyle.ApplicationModal;
        public const VbStrConv vbProperCase = VbStrConv.ProperCase;
        public const MsgBoxStyle vbQuestion = MsgBoxStyle.Question;
        public const FileAttribute vbReadOnly = FileAttribute.ReadOnly;
        public const MsgBoxResult vbRetry = MsgBoxResult.Retry;
        public const MsgBoxStyle vbRetryCancel = MsgBoxStyle.RetryCancel;
        public const FirstDayOfWeek vbSaturday = FirstDayOfWeek.Saturday;
        public const CallType vbSet = CallType.Set;
        public const DateFormat vbShortDate = DateFormat.ShortDate;
        public const DateFormat vbShortTime = DateFormat.ShortTime;
        public const VbStrConv vbSimplifiedChinese = VbStrConv.SimplifiedChinese;
        public const VariantType vbSingle = VariantType.Single;
        public const VariantType vbString = VariantType.String;
        public const FirstDayOfWeek vbSunday = FirstDayOfWeek.Sunday;
        public const FileAttribute vbSystem = FileAttribute.System;
        public const MsgBoxStyle vbSystemModal = MsgBoxStyle.SystemModal;
        public const string vbTab = "\t";
        public const Microsoft.VisualBasic.CompareMethod vbTextCompare = Microsoft.VisualBasic.CompareMethod.Text;
        public const FirstDayOfWeek vbThursday = FirstDayOfWeek.Thursday;
        public const VbStrConv vbTraditionalChinese = VbStrConv.TraditionalChinese;
        public const TriState vbTrue = TriState.True;
        public const FirstDayOfWeek vbTuesday = FirstDayOfWeek.Tuesday;
        public const VbStrConv vbUpperCase = VbStrConv.Uppercase;
        public const TriState vbUseDefault = TriState.UseDefault;
        public const VariantType vbUserDefinedType = VariantType.UserDefinedType;
        public const FirstWeekOfYear vbUseSystem = FirstWeekOfYear.System;
        public const FirstDayOfWeek vbUseSystemDayOfWeek = FirstDayOfWeek.System;
        public const VariantType vbVariant = VariantType.Variant;
        public const string vbVerticalTab = "\v";
        public const FileAttribute vbVolume = FileAttribute.Volume;
        public const FirstDayOfWeek vbWednesday = FirstDayOfWeek.Wednesday;
        public const VbStrConv vbWide = VbStrConv.Wide;
        public const MsgBoxResult vbYes = MsgBoxResult.Yes;
        public const MsgBoxStyle vbYesNo = MsgBoxStyle.YesNo;
        public const MsgBoxStyle vbYesNoCancel = MsgBoxStyle.YesNoCancel;
    }
    public sealed partial class ControlChars
    {
        public const char Back = '\b';
        public const char Cr = '\r';
        public const string CrLf = "\r\n";
        public const char FormFeed = '\f';
        public const char Lf = '\n';
        public const string NewLine = "\r\n";
        public const char NullChar = '\0';
        public const char Quote = '"';
        public const char Tab = '\t';
        public const char VerticalTab = '\v';
        public ControlChars() { }
    }
    [Microsoft.VisualBasic.CompilerServices.StandardModuleAttribute]
    public sealed partial class Conversion
    {
        internal Conversion() { }
        public static TargetType CTypeDynamic<TargetType>(object Expression) { throw null; }
        public static object CTypeDynamic(object Expression, System.Type TargetType) { throw null; }
        public static string ErrorToString() { throw null; }
        public static string ErrorToString(int ErrorNumber) { throw null; }
        public static int Fix(int Number) { throw null; }
        public static long Fix(long Number) { throw null; }
        public static double Fix(double Number) { throw null; }
        public static float Fix(float Number) { throw null; }
        public static decimal Fix(decimal Number) { throw null; }
        public static object Fix(object Number) { throw null; }
        public static short Fix(short Number) { throw null; }
        public static string Hex(object Number) { throw null; }
        [System.CLSCompliant(false)]
        public static string Hex(ulong Number) { throw null; }
        public static string Hex(long Number) { throw null; }
        public static string Hex(int Number) { throw null; }
        public static string Hex(short Number) { throw null; }
        public static string Hex(byte Number) { throw null; }
        [System.CLSCompliant(false)]
        public static string Hex(uint Number) { throw null; }
        [System.CLSCompliant(false)]
        public static string Hex(ushort Number) { throw null; }
        [System.CLSCompliant(false)]
        public static string Hex(sbyte Number) { throw null; }
        public static short Int(short Number) { throw null; }
        public static int Int(int Number) { throw null; }
        public static object Int(object Number) { throw null; }
        public static double Int(double Number) { throw null; }
        public static float Int(float Number) { throw null; }
        public static long Int(long Number) { throw null; }
        public static decimal Int(decimal Number) { throw null; }
        public static string Oct(object Number) { throw null; }
        [System.CLSCompliant(false)]
        public static string Oct(ulong Number) { throw null; }
        [System.CLSCompliant(false)]
        public static string Oct(uint Number) { throw null; }
        public static string Oct(long Number) { throw null; }
        public static string Oct(short Number) { throw null; }
        public static string Oct(byte Number) { throw null; }
        [System.CLSCompliant(false)]
        public static string Oct(sbyte Number) { throw null; }
        public static string Oct(int Number) { throw null; }
        [System.CLSCompliant(false)]
        public static string Oct(ushort Number) { throw null; }
        public static string Str(object Number) { throw null; }
        public static double Val(string InputStr) { throw null; }
        public static int Val(char Expression) { throw null; }
        public static double Val(object Expression) { throw null; }
    }
    [Microsoft.VisualBasic.CompilerServices.StandardModuleAttribute]
    public sealed partial class DateAndTime
    {
        internal DateAndTime() { }
        public static System.DateTime DateAdd(DateInterval Interval, double Number, System.DateTime DateValue) { throw null; }
        public static System.DateTime DateAdd(string Interval, double Number, object DateValue) { throw null; }
        public static long DateDiff(DateInterval Interval, System.DateTime Date1, System.DateTime Date2, FirstDayOfWeek DayOfWeek = FirstDayOfWeek.Sunday, FirstWeekOfYear WeekOfYear = FirstWeekOfYear.Jan1) { throw null; }
        public static long DateDiff(string Interval, object Date1, object Date2, FirstDayOfWeek DayOfWeek = FirstDayOfWeek.Sunday, FirstWeekOfYear WeekOfYear = FirstWeekOfYear.Jan1) { throw null; }
        public static int DatePart(DateInterval Interval, System.DateTime DateValue, FirstDayOfWeek FirstDayOfWeekValue = FirstDayOfWeek.Sunday, FirstWeekOfYear FirstWeekOfYearValue = FirstWeekOfYear.Jan1) { throw null; }
        public static int DatePart(string Interval, object DateValue, FirstDayOfWeek DayOfWeek = FirstDayOfWeek.Sunday, FirstWeekOfYear WeekOfYear = FirstWeekOfYear.Jan1) { throw null; }
        public static System.DateTime DateSerial(int Year, int Month, int Day) { throw null; }
        public static string DateString { get { throw null; } set { } }
        public static System.DateTime DateValue(string StringDate) { throw null; }
        public static int Day(System.DateTime DateValue) { throw null; }
        public static int Hour(System.DateTime TimeValue) { throw null; }
        public static int Minute(System.DateTime TimeValue) { throw null; }
        public static int Month(System.DateTime DateValue) { throw null; }
        public static string MonthName(int Month, bool Abbreviate = false) { throw null; }
        public static System.DateTime Now { get { throw null; } }
        public static int Second(System.DateTime TimeValue) { throw null; }
        public static System.DateTime TimeOfDay { get { throw null; } set { } }
        public static double Timer { get { throw null; } }
        public static System.DateTime TimeSerial(int Hour, int Minute, int Second) { throw null; }
        public static string TimeString { get { throw null; } set { } }
        public static System.DateTime TimeValue(string StringTime) { throw null; }
        public static System.DateTime Today { get { throw null; } set { } }
        public static int Weekday(System.DateTime DateValue, FirstDayOfWeek DayOfWeek = FirstDayOfWeek.Sunday) { throw null; }
        public static string WeekdayName(int Weekday, bool Abbreviate = false, FirstDayOfWeek FirstDayOfWeekValue = FirstDayOfWeek.System) { throw null; }
        public static int Year(System.DateTime DateValue) { throw null; }
    }
    public enum DateFormat
    {
        GeneralDate = 0,
        LongDate = 1,
        LongTime = 3,
        ShortDate = 2,
        ShortTime = 4,
    }
    public enum DateInterval
    {
        DayOfYear = 3,
        Day = 4,
        Hour = 7,
        Minute = 8,
        Month = 2,
        Quarter = 1,
        Second = 9,
        Weekday = 6,
        WeekOfYear = 5,
        Year = 0,
    }
    public enum DueDate
    {
        BegOfPeriod = 1,
        EndOfPeriod = 0,
    }
    public sealed partial class ErrObject
    {
        internal ErrObject() { }
        public void Clear() { }
        public string Description { get { throw null; } set { } }
        public int Erl { get { throw null; } }
        public System.Exception GetException() { throw null; }
        public int LastDllError { get { throw null; } }
        public int Number { get { throw null; } set { } }
        public void Raise(int Number, object Source = null, object Description = null, object HelpFile = null, object HelpContext = null) { }
    }
    [System.Flags]
    public enum FileAttribute
    {
        Archive = 32,
        Directory = 16,
        Hidden = 2,
        Normal = 0,
        ReadOnly = 1,
        System = 4,
        Volume = 8,
    }
    [Microsoft.VisualBasic.CompilerServices.StandardModuleAttribute]
    public sealed class FileSystem
    {
        internal FileSystem() { }
        public static void ChDir(string Path){ throw null; }
        public static void ChDrive(char Drive){ throw null; }
        public static void ChDrive(string Drive){ throw null; }
        public static string CurDir(){ throw null; }
        public static string CurDir(char Drive){ throw null; }
        public static string Dir(){ throw null; }
        public static string Dir(string PathName, FileAttribute Attributes = FileAttribute.Normal){ throw null; }
        public static bool EOF(int FileNumber){ throw null; }
        public static OpenMode FileAttr(int FileNumber){ throw null; }
        public static void FileClose(params int[] FileNumbers) { }
        public static void FileCopy(string Source, string Destination) { }
        public static System.DateTime FileDateTime(string PathName){ throw null; }
        public static void FileGet(int FileNumber, ref bool Value, long RecordNumber = -1) { }
        public static void FileGet(int FileNumber, ref byte Value, long RecordNumber = -1) { }
        public static void FileGet(int FileNumber, ref char Value, long RecordNumber = -1) { }
        public static void FileGet(int FileNumber, ref decimal Value, long RecordNumber = -1) { }
        public static void FileGet(int FileNumber, ref double Value, long RecordNumber = -1) { }
        public static void FileGet(int FileNumber, ref float Value, long RecordNumber = -1) { }
        public static void FileGet(int FileNumber, ref int Value, long RecordNumber = -1) { }
        public static void FileGet(int FileNumber, ref long Value, long RecordNumber = -1) { }
        public static void FileGet(int FileNumber, ref short Value, long RecordNumber = -1) { }
        public static void FileGet(int FileNumber, ref string Value, long RecordNumber = -1, bool StringIsFixedLength = false) { }
        public static void FileGet(int FileNumber, ref System.Array Value, long RecordNumber = -1, bool ArrayIsDynamic = false, bool StringIsFixedLength = false) { }
        public static void FileGet(int FileNumber, ref System.DateTime Value, long RecordNumber = -1) { }
        public static void FileGet(int FileNumber, ref System.ValueType Value, long RecordNumber = -1) { }
        public static void FileGetObject(int FileNumber, ref object Value, long RecordNumber = -1) { }
        public static long FileLen(string PathName) { throw null; }
        public static void FileOpen(int FileNumber, string FileName, OpenMode Mode, OpenAccess Access = OpenAccess.Default, OpenShare Share = OpenShare.Default, int RecordLength = -1) { }
        public static void FilePut(int FileNumber, bool Value, long RecordNumber = -1) { }
        public static void FilePut(int FileNumber, byte Value, long RecordNumber = -1) { }
        public static void FilePut(int FileNumber, char Value, long RecordNumber = -1) { }
        public static void FilePut(int FileNumber, decimal Value, long RecordNumber = -1) { }
        public static void FilePut(int FileNumber, double Value, long RecordNumber = -1) { }
        public static void FilePut(int FileNumber, float Value, long RecordNumber = -1) { }
        public static void FilePut(int FileNumber, int Value, long RecordNumber = -1) { }
        public static void FilePut(int FileNumber, long Value, long RecordNumber = -1) { }
        [System.ObsoleteAttribute("This member has been deprecated. Please use FilePutObject to write Object types, or coerce FileNumber and RecordNumber to Integer for writing non-Object types. http://go.microsoft.com/fwlink/?linkid=14202")]
        public static void FilePut(object FileNumber, object Value, object RecordNumber/* = -1*/) { }
        public static void FilePut(int FileNumber, short Value, long RecordNumber = -1) { }
        public static void FilePut(int FileNumber, string Value, long RecordNumber = -1, bool StringIsFixedLength = false) { }
        public static void FilePut(int FileNumber, System.Array Value, long RecordNumber = -1, bool ArrayIsDynamic = false, bool StringIsFixedLength = false) { }
        public static void FilePut(int FileNumber, System.DateTime Value, long RecordNumber = -1) { }
        public static void FilePut(int FileNumber, System.ValueType Value, long RecordNumber = -1) { }
        public static void FilePutObject(int FileNumber, object Value, long RecordNumber = -1) { }
        public static void FileWidth(int FileNumber, int RecordWidth) { }
        public static int FreeFile(){ throw null; }
        public static FileAttribute GetAttr(string PathName){ throw null; }
        public static void Input(int FileNumber, ref bool Value) { }
        public static void Input(int FileNumber, ref byte Value) { }
        public static void Input(int FileNumber, ref char Value) { }
        public static void Input(int FileNumber, ref decimal Value) { }
        public static void Input(int FileNumber, ref double Value) { }
        public static void Input(int FileNumber, ref float Value) { }
        public static void Input(int FileNumber, ref int Value) { }
        public static void Input(int FileNumber, ref long Value) { }
        public static void Input(int FileNumber, ref object Value) { }
        public static void Input(int FileNumber, ref short Value) { }
        public static void Input(int FileNumber, ref string Value) { }
        public static void Input(int FileNumber, ref System.DateTime Value) { }
        public static string InputString(int FileNumber, int CharCount){ throw null; }
        public static void Kill(string PathName) { }
        public static string LineInput(int FileNumber){ throw null; }
        public static long Loc(int FileNumber){ throw null; }
        public static void Lock(int FileNumber){ throw null; }
        public static void Lock(int FileNumber, long FromRecord, long ToRecord) { }
        public static void Lock(int FileNumber, long Record) { }
        public static long LOF(int FileNumber){ throw null; }
        public static void MkDir(string Path) { }
        public static void Print(int FileNumber, params object[] Output) { }
        public static void PrintLine(int FileNumber, params object[] Output) { }
        public static void Rename(string OldPath, string NewPath) { }
        public static void Reset() { }
        public static void RmDir(string Path) { }
        public static void Seek(int FileNumber, long Position) { }
        public static long Seek(int FileNumber) { throw null; }
        public static void SetAttr(string PathName, FileAttribute Attributes) { }
        public static SpcInfo SPC(short Count){ throw null; }
        public static TabInfo TAB(){ throw null; }
        public static TabInfo TAB(short Column){ throw null; }
        public static void Unlock(int FileNumber) { }
        public static void Unlock(int FileNumber, long Record) { }
        public static void Unlock(int FileNumber, long FromRecord, long ToRecord) { }
        public static void Write(int FileNumber, params object[] Output) { }
        public static void WriteLine(int FileNumber, params object[] Output) { }
    }
    [Microsoft.VisualBasic.CompilerServices.StandardModuleAttribute]
    public sealed class Financial
    {
        internal Financial() { }
        public static double DDB(double Cost, double Salvage, double Life, double Period, double Factor = 2.0) { throw null; }
        public static double FV(double Rate, double NPer, double Pmt, double PV = 0, DueDate Due = DueDate.EndOfPeriod) { throw null; }
        public static double IPmt(double Rate, double Per, double NPer, double PV, double FV = 0, DueDate Due = DueDate.EndOfPeriod) { throw null; }
        public static double IRR(ref double[] ValueArray, double Guess = 0.1) { throw null; }
        public static double MIRR(ref double[] ValueArray, double FinanceRate, double ReinvestRate) { throw null; }
        public static double NPer(double Rate, double Pmt, double PV, double FV = 0, DueDate Due = DueDate.EndOfPeriod) { throw null; }
        public static double NPV(double Rate, ref double[] ValueArray) { throw null; }
        public static double Pmt(double Rate, double NPer, double PV, double FV = 0, DueDate Due = DueDate.EndOfPeriod) { throw null; }
        public static double PPmt(double Rate, double Per, double NPer, double PV, double FV = 0, DueDate Due = DueDate.EndOfPeriod) { throw null; }
        public static double PV(double Rate, double NPer, double Pmt, double FV = 0, DueDate Due = DueDate.EndOfPeriod) { throw null; }
        public static double Rate(double NPer, double Pmt, double PV, double FV = 0, DueDate Due = DueDate.EndOfPeriod, double Guess = 0.1) { throw null; }
        public static double SLN(double Cost, double Salvage, double Life) { throw null; }
        public static double SYD(double Cost, double Salvage, double Life, double Period) { throw null; }
    }
    public enum FirstDayOfWeek
    {
        Friday = 6,
        Monday = 2,
        Saturday = 7,
        Sunday = 1,
        System = 0,
        Thursday = 5,
        Tuesday = 3,
        Wednesday = 4,
    }
    public enum FirstWeekOfYear
    {
        FirstFourDays = 2,
        FirstFullWeek = 3,
        Jan1 = 1,
        System = 0,
    }
    [System.AttributeUsageAttribute(System.AttributeTargets.Class, AllowMultiple=false, Inherited=false)]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
    public sealed partial class HideModuleNameAttribute : System.Attribute
    {
        public HideModuleNameAttribute() { }
    }
    [Microsoft.VisualBasic.CompilerServices.StandardModuleAttribute]
    public sealed partial class Information
    {
        internal Information() { }
        public static ErrObject Err() { throw null; }
        public static bool IsArray(object VarName) { throw null; }
        public static bool IsDate(object Expression) { throw null; }
        public static bool IsDBNull(object Expression) { throw null; }
        public static bool IsError(object Expression) { throw null; }
        public static bool IsNothing(object Expression) { throw null; }
        public static bool IsNumeric(object Expression) { throw null; }
        public static bool IsReference(object Expression) { throw null; }
        public static int LBound(System.Array Array, int Rank = 1) { throw null; }
        public static int QBColor(int Color) { throw null; }
        public static int RGB(int Red, int Green, int Blue) { throw null; }
        public static string SystemTypeName(string VbName) { throw null; }
        public static string TypeName(object VarName) { throw null; }
        public static int UBound(System.Array Array, int Rank = 1) { throw null; }
        public static Microsoft.VisualBasic.VariantType VarType(object VarName) { throw null; }
        public static string VbTypeName(string UrtName) { throw null; }
    }
    [Microsoft.VisualBasic.CompilerServices.StandardModuleAttribute]
    public sealed partial class Interaction
    {
        internal Interaction() { }
        public static void Beep() { }
        public static object CallByName(object ObjectRef, string ProcName, CallType UseCallType, params object[] Args) { throw null; }
        public static object CreateObject(string ProgId, string ServerName = "") { throw null; }
        public static object IIf(bool Expression, object TruePart, object FalsePart) { throw null; }
    }
    public enum MsgBoxResult
    {
        Abort = 3,
        Cancel = 2,
        Ignore = 5,
        Ok = 1,
        Retry = 4,
        No = 7,
        Yes = 6,
    }
    [System.FlagsAttribute]
    public enum MsgBoxStyle
    {
        AbortRetryIgnore = 2,
        ApplicationModal = 0,
        Critical = 16,
        DefaultButton1 = 0,
        DefaultButton2 = 256,
        DefaultButton3 = 512,
        Exclamation = 48,
        Information = 64,
        MsgBoxHelp = 16384,
        MsgBoxRight = 524288,
        MsgBoxRtlReading = 1048576,
        MsgBoxSetForeground = 65536,
        OkCancel = 1,
        OkOnly = 0,
        Question = 32,
        RetryCancel = 5,
        SystemModal = 4096,
        YesNo = 4,
        YesNoCancel = 3,
    }
    [System.AttributeUsageAttribute(System.AttributeTargets.Class, AllowMultiple=false, Inherited=false)]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    public sealed partial class MyGroupCollectionAttribute : System.Attribute
    {
        public MyGroupCollectionAttribute(string typeToCollect, string createInstanceMethodName, string disposeInstanceMethodName, string defaultInstanceAlias) { }
        public string CreateMethod { get { throw null; } }
        public string DefaultInstanceAlias { get { throw null; } }
        public string DisposeMethod { get { throw null; } }
        public string MyGroupName { get { throw null; } }
    }
    public enum OpenAccess
    {
        Default = -1,
        Read = 1,
        ReadWrite = 3,
        Write = 2,
    }
    public enum OpenMode
    {
        Append = 8,
        Binary = 32,
        Input = 1,
        Output = 2,
        Random = 4,
    }
    public enum OpenShare
    {
        Default = -1,
        LockRead = 2,
        LockReadWrite = 0,
        LockWrite = 1,
        Shared = 3
    }
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
    public struct SpcInfo
    {
        public short Count;
    }
    [Microsoft.VisualBasic.CompilerServices.StandardModuleAttribute]
    public sealed partial class Strings
    {
        internal Strings() { }
        public static int Asc(char String) { throw null; }
        public static int Asc(string String) { throw null; }
        public static int AscW(char String) { throw null; }
        public static int AscW(string String) { throw null; }
        public static char Chr(int CharCode) { throw null; }
        public static char ChrW(int CharCode) { throw null; }
        public static string[] Filter(object[] Source, string Match, bool Include = true, [Microsoft.VisualBasic.CompilerServices.OptionCompareAttribute]Microsoft.VisualBasic.CompareMethod Compare = Microsoft.VisualBasic.CompareMethod.Binary) { throw null; }
        public static string[] Filter(string[] Source, string Match, bool Include = true, [Microsoft.VisualBasic.CompilerServices.OptionCompareAttribute]Microsoft.VisualBasic.CompareMethod Compare = Microsoft.VisualBasic.CompareMethod.Binary) { throw null; }
        public static string Format(object Expression, string Style = "") { throw null; }
        public static string FormatCurrency(object Expression, int NumDigitsAfterDecimal = -1, TriState IncludeLeadingDigit = TriState.UseDefault, TriState UseParensForNegativeNumbers = TriState.UseDefault, TriState GroupDigits = TriState.UseDefault) { throw null; }
        public static string FormatDateTime(System.DateTime Expression, DateFormat NamedFormat = DateFormat.GeneralDate) { throw null; }
        public static string FormatNumber(object Expression, int NumDigitsAfterDecimal = -1, TriState IncludeLeadingDigit = TriState.UseDefault, TriState UseParensForNegativeNumbers = TriState.UseDefault, TriState GroupDigits = TriState.UseDefault) { throw null; }
        public static string FormatPercent(object Expression, int NumDigitsAfterDecimal = -1, TriState IncludeLeadingDigit = TriState.UseDefault, TriState UseParensForNegativeNumbers = TriState.UseDefault, TriState GroupDigits = TriState.UseDefault) { throw null; }
        public static char GetChar(string str, int Index) { throw null; }
        public static int InStr(int StartPos, string String1, string String2, [Microsoft.VisualBasic.CompilerServices.OptionCompareAttribute]Microsoft.VisualBasic.CompareMethod Compare = Microsoft.VisualBasic.CompareMethod.Binary) { throw null; }
        public static int InStr(string String1, string String2, [Microsoft.VisualBasic.CompilerServices.OptionCompareAttribute]Microsoft.VisualBasic.CompareMethod Compare = Microsoft.VisualBasic.CompareMethod.Binary) { throw null; }
        public static int InStrRev(string StringCheck, string StringMatch, int Start = -1, [Microsoft.VisualBasic.CompilerServices.OptionCompareAttribute]Microsoft.VisualBasic.CompareMethod Compare = Microsoft.VisualBasic.CompareMethod.Binary) { throw null; }
        public static string Join(object[] SourceArray, string Delimiter = " ") { throw null; }
        public static string Join(string[] SourceArray, string Delimiter = " ") { throw null; }
        public static char LCase(char Value) { throw null; }
        public static string LCase(string Value) { throw null; }
        public static string Left(string str, int Length) { throw null; }
        public static int Len(bool Expression) { throw null; }
        public static int Len(byte Expression) { throw null; }
        public static int Len(char Expression) { throw null; }
        public static int Len(System.DateTime Expression) { throw null; }
        public static int Len(decimal Expression) { throw null; }
        public static int Len(double Expression) { throw null; }
        public static int Len(short Expression) { throw null; }
        public static int Len(int Expression) { throw null; }
        public static int Len(long Expression) { throw null; }
        public static int Len(object Expression) { throw null; }
        [System.CLSCompliantAttribute(false)]
        public static int Len(sbyte Expression) { throw null; }
        public static int Len(float Expression) { throw null; }
        public static int Len(string Expression) { throw null; }
        [System.CLSCompliantAttribute(false)]
        public static int Len(ushort Expression) { throw null; }
        [System.CLSCompliantAttribute(false)]
        public static int Len(uint Expression) { throw null; }
        [System.CLSCompliantAttribute(false)]
        public static int Len(ulong Expression) { throw null; }
        public static string LSet(string Source, int Length) { throw null; }
        public static string LTrim(string str) { throw null; }
        public static string Mid(string str, int Start) { throw null; }
        public static string Mid(string str, int Start, int Length) { throw null; }
        public static string Replace(string Expression, string Find, string Replacement, int Start = 1, int Count = -1, [Microsoft.VisualBasic.CompilerServices.OptionCompareAttribute] CompareMethod Compare = CompareMethod.Binary) { throw null; }
        public static string Right(string str, int Length) { throw null; }
        public static string RSet(string Source, int Length) { throw null; }
        public static string RTrim(string str) { throw null; }
        public static string Space(int Number) { throw null; }
        public static string[] Split(string Expression, string Delimiter = " ", int Limit = -1, [Microsoft.VisualBasic.CompilerServices.OptionCompareAttribute] CompareMethod Compare = CompareMethod.Binary) { throw null; }
        public static int StrComp(string String1, string String2, CompareMethod Compare = CompareMethod.Binary) { throw null; }
        public static string StrConv(string str, Microsoft.VisualBasic.VbStrConv Conversion, int LocaleID = 0) { throw null; }
        public static string StrDup(int Number, char Character) { throw null; }
        public static object StrDup(int Number, object Character) { throw null; }
        public static string StrDup(int Number, string Character) { throw null; }
        public static string StrReverse(string Expression) { throw null; }
        public static string Trim(string str) { throw null; }
        public static char UCase(char Value) { throw null; }
        public static string UCase(string Value) { throw null; }
    }
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
    public struct TabInfo
    {
        public short Column;
    }
    public enum TriState
    {
        False = 0,
        True = -1,
        UseDefault = -2,
    }
    public enum VariantType
    {
        Empty = 0,
        Null = 1,
        Short = 2,
        Integer = 3,
        Single = 4,
        Double = 5,
        Currency = 6,
        Date = 7,
        String = 8,
        Object = 9,
        Error = 10,
        Boolean = 11,
        Variant = 12,
        DataObject = 13,
        Decimal = 14,
        Byte = 17,
        Char = 18,
        Long = 20,
        UserDefinedType = 36,
        Array = 8192,
    }
    [System.AttributeUsageAttribute(System.AttributeTargets.Field, Inherited=false, AllowMultiple=false)]
    public sealed partial class VBFixedArrayAttribute : System.Attribute
    {
        public VBFixedArrayAttribute(int UpperBound1) { }
        public VBFixedArrayAttribute(int UpperBound1, int UpperBound2) { }
        public int[] Bounds { get { throw null; } }
        public int Length { get { throw null; } }
    }
    [System.AttributeUsageAttribute(System.AttributeTargets.Field, Inherited=false, AllowMultiple=false)]
    public sealed partial class VBFixedStringAttribute : System.Attribute
    {
        public VBFixedStringAttribute(int Length) { }
        public int Length { get { throw null; } }
    }
    [Microsoft.VisualBasic.CompilerServices.StandardModuleAttribute]
    public sealed partial class VBMath
    {
        internal VBMath() { }
        public static void Randomize() { }
        public static void Randomize(double Number) { }
        public static float Rnd() { throw null; }
        public static float Rnd(float Number) { throw null; }
    }
    [System.FlagsAttribute]
    public enum VbStrConv
    {
        Hiragana = 32,
        Katakana = 16,
        LinguisticCasing = 1024,
        Lowercase = 2,
        Narrow = 8,
        None = 0,
        ProperCase = 3,
        SimplifiedChinese = 256,
        TraditionalChinese = 512,
        Uppercase = 1,
        Wide = 4,
    }
}
namespace Microsoft.VisualBasic.ApplicationServices
{
    public partial class ApplicationBase
    {
        public ApplicationBase() { }
        public void ChangeCulture(string cultureName) { throw null; }
        public void ChangeUICulture(string cultureName) { throw null; }
        public System.Globalization.CultureInfo Culture { get { throw null; } }
        public string GetEnvironmentVariable(string name) { throw null; }
        public AssemblyInfo Info { get { throw null; } }
        public System.Globalization.CultureInfo UICulture { get { throw null; } }
    }
    public class AssemblyInfo
    {
        public AssemblyInfo(System.Reflection.Assembly currentAssembly) { }
        public string AssemblyName { get { throw null; } }
        public string CompanyName { get { throw null; } }
        public string Copyright { get { throw null; } }
        public string Description { get { throw null; } }
        public string DirectoryPath { get { throw null; } }
        public System.Collections.ObjectModel.ReadOnlyCollection<System.Reflection.Assembly> LoadedAssemblies { get { throw null; } }
        public string ProductName { get { throw null; } }
        public string StackTrace { get { throw null; } }
        public string Title { get { throw null; } }
        public string Trademark { get { throw null; } }
        public System.Version Version { get { throw null; } }
        public long WorkingSet { get { throw null; } }
    }
    public class ConsoleApplicationBase : ApplicationBase
    {
        public ConsoleApplicationBase() { }
        public System.Collections.ObjectModel.ReadOnlyCollection<string> CommandLineArgs { get { throw null; } }
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        protected System.Collections.ObjectModel.ReadOnlyCollection<string> InternalCommandLine { set { } }
    }
    public partial class StartupEventArgs : System.ComponentModel.CancelEventArgs
    {
        public StartupEventArgs(System.Collections.ObjectModel.ReadOnlyCollection<string> args) { }
        public System.Collections.ObjectModel.ReadOnlyCollection<string> CommandLine { get { throw null; } }
    }
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    public partial class StartupNextInstanceEventArgs : System.EventArgs
    {
        public StartupNextInstanceEventArgs(System.Collections.ObjectModel.ReadOnlyCollection<string> args, bool bringToForegroundFlag) { }
        public bool BringToForeground { get { throw null; } set { } }
        public System.Collections.ObjectModel.ReadOnlyCollection<string> CommandLine { get { throw null; } }
    }
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    public partial class UnhandledExceptionEventArgs : System.Threading.ThreadExceptionEventArgs
    {
        public UnhandledExceptionEventArgs(bool exitApplication, System.Exception exception) : base (default(System.Exception)) { }
        public bool ExitApplication { get { throw null; } set { } }
    }
    public partial class User
    {
        public User() { }
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.Security.Principal.IPrincipal CurrentPrincipal { get { throw null; } set { } }
        protected virtual System.Security.Principal.IPrincipal InternalPrincipal { get { throw null; } set { } }
        public bool IsAuthenticated { get { throw null; } }
        public bool IsInRole(string role) { throw null; }
        public string Name { get { throw null; } }
    }
}
namespace Microsoft.VisualBasic.CompilerServices
{
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
    public sealed partial class BooleanType
    {
        internal BooleanType() { }
        public static bool FromObject(object Value) { throw null; }
        public static bool FromString(string Value) { throw null; }
    }
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
    public sealed partial class ByteType
    {
        internal ByteType() { }
        public static byte FromObject(object Value) { throw null; }
        public static byte FromString(string Value) { throw null; }
    }
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
    public sealed partial class CharArrayType
    {
        internal CharArrayType() { }
        public static char[] FromObject(object Value) { throw null; }
        public static char[] FromString(string Value) { throw null; }
    }
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
    public sealed partial class CharType
    {
        internal CharType() { }
        public static char FromObject(object Value) { throw null; }
        public static char FromString(string Value) { throw null; }
    }
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
    public sealed partial class Conversions
    {
        internal Conversions() { }
        public static object ChangeType(object Expression, System.Type TargetType) { throw null; }
        [System.ObsoleteAttribute("do not use this method", true)]
        public static object FallbackUserDefinedConversion(object Expression, System.Type TargetType) { throw null; }
        public static string FromCharAndCount(char Value, int Count) { throw null; }
        public static string FromCharArray(char[] Value) { throw null; }
        public static string FromCharArraySubset(char[] Value, int StartIndex, int Length) { throw null; }
        public static bool ToBoolean(object Value) { throw null; }
        public static bool ToBoolean(string Value) { throw null; }
        public static byte ToByte(object Value) { throw null; }
        public static byte ToByte(string Value) { throw null; }
        public static char ToChar(object Value) { throw null; }
        public static char ToChar(string Value) { throw null; }
        public static char[] ToCharArrayRankOne(object Value) { throw null; }
        public static char[] ToCharArrayRankOne(string Value) { throw null; }
        public static System.DateTime ToDate(object Value) { throw null; }
        public static System.DateTime ToDate(string Value) { throw null; }
        public static decimal ToDecimal(bool Value) { throw null; }
        public static decimal ToDecimal(object Value) { throw null; }
        public static decimal ToDecimal(string Value) { throw null; }
        public static double ToDouble(object Value) { throw null; }
        public static double ToDouble(string Value) { throw null; }
        public static T ToGenericParameter<T>(object Value) { throw null; }
        public static int ToInteger(object Value) { throw null; }
        public static int ToInteger(string Value) { throw null; }
        public static long ToLong(object Value) { throw null; }
        public static long ToLong(string Value) { throw null; }
        [System.CLSCompliantAttribute(false)]
        public static sbyte ToSByte(object Value) { throw null; }
        [System.CLSCompliantAttribute(false)]
        public static sbyte ToSByte(string Value) { throw null; }
        public static short ToShort(object Value) { throw null; }
        public static short ToShort(string Value) { throw null; }
        public static float ToSingle(object Value) { throw null; }
        public static float ToSingle(string Value) { throw null; }
        public static string ToString(bool Value) { throw null; }
        public static string ToString(byte Value) { throw null; }
        public static string ToString(char Value) { throw null; }
        public static string ToString(System.DateTime Value) { throw null; }
        public static string ToString(decimal Value) { throw null; }
        public static string ToString(decimal Value, System.Globalization.NumberFormatInfo NumberFormat) { throw null; }
        public static string ToString(double Value) { throw null; }
        public static string ToString(double Value, System.Globalization.NumberFormatInfo NumberFormat) { throw null; }
        public static string ToString(short Value) { throw null; }
        public static string ToString(int Value) { throw null; }
        public static string ToString(long Value) { throw null; }
        public static string ToString(object Value) { throw null; }
        public static string ToString(float Value) { throw null; }
        public static string ToString(float Value, System.Globalization.NumberFormatInfo NumberFormat) { throw null; }
        [System.CLSCompliantAttribute(false)]
        public static string ToString(uint Value) { throw null; }
        [System.CLSCompliantAttribute(false)]
        public static string ToString(ulong Value) { throw null; }
        [System.CLSCompliantAttribute(false)]
        public static uint ToUInteger(object Value) { throw null; }
        [System.CLSCompliantAttribute(false)]
        public static uint ToUInteger(string Value) { throw null; }
        [System.CLSCompliantAttribute(false)]
        public static ulong ToULong(object Value) { throw null; }
        [System.CLSCompliantAttribute(false)]
        public static ulong ToULong(string Value) { throw null; }
        [System.CLSCompliantAttribute(false)]
        public static ushort ToUShort(object Value) { throw null; }
        [System.CLSCompliantAttribute(false)]
        public static ushort ToUShort(string Value) { throw null; }
    }
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
    public sealed partial class DateType
    {
        internal DateType() { }
        public static System.DateTime FromObject(object Value) { throw null; }
        public static System.DateTime FromString(string Value) { throw null; }
        public static System.DateTime FromString(string Value, System.Globalization.CultureInfo culture) { throw null; }
    }
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
    public sealed partial class DecimalType
    {
        internal DecimalType() { }
        public static decimal FromBoolean(bool Value) { throw null; }
        public static decimal FromObject(object Value) { throw null; }
        public static decimal FromObject(object Value, System.Globalization.NumberFormatInfo NumberFormat) { throw null; }
        public static decimal FromString(string Value) { throw null; }
        public static decimal FromString(string Value, System.Globalization.NumberFormatInfo NumberFormat) { throw null; }
        public static decimal Parse(string Value, System.Globalization.NumberFormatInfo NumberFormat) { throw null; }
    }
    [System.AttributeUsageAttribute(System.AttributeTargets.Class, AllowMultiple=false, Inherited=false)]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
    public sealed partial class DesignerGeneratedAttribute : System.Attribute
    {
        public DesignerGeneratedAttribute() { }
    }
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
    public sealed partial class DoubleType
    {
        internal DoubleType() { }
        public static double FromObject(object Value) { throw null; }
        public static double FromObject(object Value, System.Globalization.NumberFormatInfo NumberFormat) { throw null; }
        public static double FromString(string Value) { throw null; }
        public static double FromString(string Value, System.Globalization.NumberFormatInfo NumberFormat) { throw null; }
        public static double Parse(string Value) { throw null; }
        public static double Parse(string Value, System.Globalization.NumberFormatInfo NumberFormat) { throw null; }
    }
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
    public sealed partial class IncompleteInitialization : System.Exception
    {
        public IncompleteInitialization() { }
    }
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
    public sealed partial class IntegerType
    {
        internal IntegerType() { }
        public static int FromObject(object Value) { throw null; }
        public static int FromString(string Value) { throw null; }
    }
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
    public sealed partial class LateBinding
    {
        internal LateBinding() { }
        [System.Diagnostics.DebuggerHiddenAttribute]
        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static void LateCall(object o, System.Type objType, string name, object[] args, string[] paramnames, bool[] CopyBack) { throw null; }
        [System.Diagnostics.DebuggerHiddenAttribute]
        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static object LateGet(object o, System.Type objType, string name, object[] args, string[] paramnames, bool[] CopyBack) { throw null; }
        [System.Diagnostics.DebuggerHiddenAttribute]
        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static object LateIndexGet(object o, object[] args, string[] paramnames) { throw null; }
        [System.Diagnostics.DebuggerHiddenAttribute]
        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static void LateIndexSet(object o, object[] args, string[] paramnames) { throw null; }
        [System.Diagnostics.DebuggerHiddenAttribute]
        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static void LateIndexSetComplex(object o, object[] args, string[] paramnames, bool OptimisticSet, bool RValueBase) { throw null; }
        [System.Diagnostics.DebuggerHiddenAttribute]
        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static void LateSet(object o, System.Type objType, string name, object[] args, string[] paramnames) { throw null; }
        [System.Diagnostics.DebuggerHiddenAttribute]
        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static void LateSetComplex(object o, System.Type objType, string name, object[] args, string[] paramnames, bool OptimisticSet, bool RValueBase) { throw null; }
    }
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
    public sealed partial class LikeOperator
    {
        internal LikeOperator() { }
        public static object LikeObject(object Source, object Pattern, Microsoft.VisualBasic.CompareMethod CompareOption) { throw null; }
        public static bool LikeString(string Source, string Pattern, Microsoft.VisualBasic.CompareMethod CompareOption) { throw null; }
    }
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
    public sealed partial class LongType
    {
        internal LongType() { }
        public static long FromObject(object Value) { throw null; }
        public static long FromString(string Value) { throw null; }
    }
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
    public sealed partial class NewLateBinding
    {
        internal NewLateBinding() { }
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.ObsoleteAttribute("do not use this method", true)]
        public static object FallbackCall(object Instance, string MemberName, object[] Arguments, string[] ArgumentNames, bool IgnoreReturn) { throw null; }
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.ObsoleteAttribute("do not use this method", true)]
        public static object FallbackGet(object Instance, string MemberName, object[] Arguments, string[] ArgumentNames) { throw null; }
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.ObsoleteAttribute("do not use this method", true)]
        public static void FallbackIndexSet(object Instance, object[] Arguments, string[] ArgumentNames) { }
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.ObsoleteAttribute("do not use this method", true)]
        public static void FallbackIndexSetComplex(object Instance, object[] Arguments, string[] ArgumentNames, bool OptimisticSet, bool RValueBase) { }
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.ObsoleteAttribute("do not use this method", true)]
        public static object FallbackInvokeDefault1(object Instance, object[] Arguments, string[] ArgumentNames, bool ReportErrors) { throw null; }
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.ObsoleteAttribute("do not use this method", true)]
        public static object FallbackInvokeDefault2(object Instance, object[] Arguments, string[] ArgumentNames, bool ReportErrors) { throw null; }
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.ObsoleteAttribute("do not use this method", true)]
        public static void FallbackSet(object Instance, string MemberName, object[] Arguments) { }
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.ObsoleteAttribute("do not use this method", true)]
        public static void FallbackSetComplex(object Instance, string MemberName, object[] Arguments, bool OptimisticSet, bool RValueBase) { }
        public static object LateCall(object Instance, System.Type Type, string MemberName, object[] Arguments, string[] ArgumentNames, System.Type[] TypeArguments, bool[] CopyBack, bool IgnoreReturn) { throw null; }
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public static object LateCallInvokeDefault(object Instance, object[] Arguments, string[] ArgumentNames, bool ReportErrors) { throw null; }
        public static object LateGet(object Instance, System.Type Type, string MemberName, object[] Arguments, string[] ArgumentNames, System.Type[] TypeArguments, bool[] CopyBack) { throw null; }
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public static object LateGetInvokeDefault(object Instance, object[] Arguments, string[] ArgumentNames, bool ReportErrors) { throw null; }
        public static object LateIndexGet(object Instance, object[] Arguments, string[] ArgumentNames) { throw null; }
        public static void LateIndexSet(object Instance, object[] Arguments, string[] ArgumentNames) { }
        public static void LateIndexSetComplex(object Instance, object[] Arguments, string[] ArgumentNames, bool OptimisticSet, bool RValueBase) { }
        public static void LateSet(object Instance, System.Type Type, string MemberName, object[] Arguments, string[] ArgumentNames, System.Type[] TypeArguments) { }
        public static void LateSet(object Instance, System.Type Type, string MemberName, object[] Arguments, string[] ArgumentNames, System.Type[] TypeArguments, bool OptimisticSet, bool RValueBase, Microsoft.VisualBasic.CallType CallType) { }
        public static void LateSetComplex(object Instance, System.Type Type, string MemberName, object[] Arguments, string[] ArgumentNames, System.Type[] TypeArguments, bool OptimisticSet, bool RValueBase) { }
    }
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
    public sealed partial class ObjectFlowControl
    {
        internal ObjectFlowControl() { }
        public static void CheckForSyncLockOnValueType(object Expression) { }
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public sealed partial class ForLoopControl
        {
            internal ForLoopControl() { }
            public static bool ForLoopInitObj(object Counter, object Start, object Limit, object StepValue, ref object LoopForResult, ref object CounterResult) { throw null; }
            public static bool ForNextCheckDec(decimal count, decimal limit, decimal StepValue) { throw null; }
            public static bool ForNextCheckObj(object Counter, object LoopObj, ref object CounterResult) { throw null; }
            public static bool ForNextCheckR4(float count, float limit, float StepValue) { throw null; }
            public static bool ForNextCheckR8(double count, double limit, double StepValue) { throw null; }
        }
    }
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
    public sealed partial class ObjectType
    {
        public ObjectType() { }
        public static object AddObj(object o1, object o2) { throw null; }
        public static object BitAndObj(object obj1, object obj2) { throw null; }
        public static object BitOrObj(object obj1, object obj2) { throw null; }
        public static object BitXorObj(object obj1, object obj2) { throw null; }
        public static object DivObj(object o1, object o2) { throw null; }
        public static object GetObjectValuePrimitive(object o) { throw null; }
        public static object IDivObj(object o1, object o2) { throw null; }
        public static bool LikeObj(object vLeft, object vRight, CompareMethod CompareOption) { throw null; }
        public static object ModObj(object o1, object o2) { throw null; }
        public static object MulObj(object o1, object o2) { throw null; }
        public static object NegObj(object obj) { throw null; }
        public static object NotObj(object obj) { throw null; }
        public static int ObjTst(object o1, object o2, bool TextCompare) { throw null; }
        public static object PlusObj(object obj) { throw null; }
        public static object PowObj(object obj1, object obj2) { throw null; }
        public static object ShiftLeftObj(object o1, int amount) { throw null; }
        public static object ShiftRightObj(object o1, int amount) { throw null; }
        public static object StrCatObj(object vLeft, object vRight) { throw null; }
        public static object SubObj(object o1, object o2) { throw null; }
        public static object XorObj(object obj1, object obj2) { throw null; }
    }
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
    public sealed partial class Operators
    {
        internal Operators() { }
        public static object AddObject(object Left, object Right) { throw null; }
        public static object AndObject(object Left, object Right) { throw null; }
        public static object CompareObjectEqual(object Left, object Right, bool TextCompare) { throw null; }
        public static object CompareObjectGreater(object Left, object Right, bool TextCompare) { throw null; }
        public static object CompareObjectGreaterEqual(object Left, object Right, bool TextCompare) { throw null; }
        public static object CompareObjectLess(object Left, object Right, bool TextCompare) { throw null; }
        public static object CompareObjectLessEqual(object Left, object Right, bool TextCompare) { throw null; }
        public static object CompareObjectNotEqual(object Left, object Right, bool TextCompare) { throw null; }
        public static int CompareString(string Left, string Right, bool TextCompare) { throw null; }
        public static object ConcatenateObject(object Left, object Right) { throw null; }
        public static bool ConditionalCompareObjectEqual(object Left, object Right, bool TextCompare) { throw null; }
        public static bool ConditionalCompareObjectGreater(object Left, object Right, bool TextCompare) { throw null; }
        public static bool ConditionalCompareObjectGreaterEqual(object Left, object Right, bool TextCompare) { throw null; }
        public static bool ConditionalCompareObjectLess(object Left, object Right, bool TextCompare) { throw null; }
        public static bool ConditionalCompareObjectLessEqual(object Left, object Right, bool TextCompare) { throw null; }
        public static bool ConditionalCompareObjectNotEqual(object Left, object Right, bool TextCompare) { throw null; }
        public static object DivideObject(object Left, object Right) { throw null; }
        public static object ExponentObject(object Left, object Right) { throw null; }
        [System.ObsoleteAttribute("do not use this method", true)]
        public static object FallbackInvokeUserDefinedOperator(object vbOp, object[] arguments) { throw null; }
        public static object IntDivideObject(object Left, object Right) { throw null; }
        public static object LeftShiftObject(object Operand, object Amount) { throw null; }
        public static object ModObject(object Left, object Right) { throw null; }
        public static object MultiplyObject(object Left, object Right) { throw null; }
        public static object NegateObject(object Operand) { throw null; }
        public static object NotObject(object Operand) { throw null; }
        public static object OrObject(object Left, object Right) { throw null; }
        public static object PlusObject(object Operand) { throw null; }
        public static object RightShiftObject(object Operand, object Amount) { throw null; }
        public static object SubtractObject(object Left, object Right) { throw null; }
        public static object XorObject(object Left, object Right) { throw null; }
    }
    [System.AttributeUsageAttribute(System.AttributeTargets.Parameter, Inherited=false, AllowMultiple=false)]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
    public sealed partial class OptionCompareAttribute : System.Attribute
    {
        public OptionCompareAttribute() { }
    }
    [System.AttributeUsageAttribute(System.AttributeTargets.Class, Inherited=false, AllowMultiple=false)]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
    public sealed partial class OptionTextAttribute : System.Attribute
    {
        public OptionTextAttribute() { }
    }
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
    public sealed partial class ProjectData
    {
        internal ProjectData() { }
        public static void ClearProjectError() { }
        public static System.Exception CreateProjectError(int hr) { throw null; }
        public static void EndApp() { }
        public static void SetProjectError(System.Exception ex) { }
        public static void SetProjectError(System.Exception ex, int lErl) { }
    }
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
    public sealed partial class ShortType
    {
        internal ShortType() { }
        public static short FromObject(object Value) { throw null; }
        public static short FromString(string Value) { throw null; }
    }
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
    public sealed partial class SingleType
    {
        internal SingleType() { }
        public static float FromObject(object Value) { throw null; }
        public static float FromObject(object Value, System.Globalization.NumberFormatInfo NumberFormat) { throw null; }
        public static float FromString(string Value) { throw null; }
        public static float FromString(string Value, System.Globalization.NumberFormatInfo NumberFormat) { throw null; }
    }
    [System.AttributeUsageAttribute(System.AttributeTargets.Class, Inherited=false, AllowMultiple=false)]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
    public sealed partial class StandardModuleAttribute : System.Attribute
    {
        public StandardModuleAttribute() { }
    }
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
    public sealed partial class StaticLocalInitFlag
    {
        public short State;
        public StaticLocalInitFlag() { }
    }
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
    public sealed partial class StringType
    {
        internal StringType() { }
        public static string FromBoolean(bool Value) { throw null; }
        public static string FromByte(byte Value) { throw null; }
        public static string FromChar(char Value) { throw null; }
        public static string FromDate(System.DateTime Value) { throw null; }
        public static string FromDecimal(decimal Value) { throw null; }
        public static string FromDecimal(decimal Value, System.Globalization.NumberFormatInfo NumberFormat) { throw null; }
        public static string FromDouble(double Value) { throw null; }
        public static string FromDouble(double Value, System.Globalization.NumberFormatInfo NumberFormat) { throw null; }
        public static string FromInteger(int Value) { throw null; }
        public static string FromLong(long Value) { throw null; }
        public static string FromObject(object Value) { throw null; }
        public static string FromShort(short Value) { throw null; }
        public static string FromSingle(float Value) { throw null; }
        public static string FromSingle(float Value, System.Globalization.NumberFormatInfo NumberFormat) { throw null; }
        public static void MidStmtStr(ref string sDest, int StartPosition, int MaxInsertLength, string sInsert) { throw null; }
        public static int StrCmp(string sLeft, string sRight, bool TextCompare) { throw null; }
        public static bool StrLike(string Source, string Pattern, Microsoft.VisualBasic.CompareMethod CompareOption) { throw null; }
        public static bool StrLikeBinary(string Source, string Pattern) { throw null; }
        public static bool StrLikeText(string Source, string Pattern) { throw null; }
    }
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
    public sealed partial class Utils
    {
        internal Utils() { }
        public static System.Array CopyArray(System.Array arySrc, System.Array aryDest) { throw null; }
    }
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
    public sealed partial class Versioned
    {
        internal Versioned() { }
        public static object CallByName(object Instance, string MethodName, CallType UseCallType, params object[] Arguments) { throw null; }
        public static bool IsNumeric(object Expression) { throw null; }
        public static string SystemTypeName(string VbName) { throw null; }
        public static string TypeName(object Expression) { throw null; }
        public static string VbTypeName(string SystemName) { throw null; }
    }
}
namespace Microsoft.VisualBasic.Devices
{
    public partial class Audio
    {
        public Audio() { }
    }
    public partial class Clock
    {
        public Clock() { }
        public System.DateTime GmtTime { get { throw null; } }
        public System.DateTime LocalTime { get { throw null; } }
        public int TickCount { get { throw null; } }
    }
    public partial class Computer : ServerComputer
    {
        public Computer() { }
        public Audio Audio { get { throw null; } }
        public Microsoft.VisualBasic.MyServices.ClipboardProxy Clipboard { get { throw null; } }
        public Keyboard Keyboard { get { throw null; } }
        public Mouse Mouse { get { throw null; } }
        public Ports Ports { get { throw null; } }
    }
    public partial class ComputerInfo
    {
        public ComputerInfo() { }
        [System.CLSCompliant(false)]
        public ulong AvailablePhysicalMemory { get { throw null; } }
        [System.CLSCompliant(false)]
        public ulong AvailableVirtualMemory { get { throw null; } }
        public System.Globalization.CultureInfo InstalledUICulture { get { throw null; } }
        public string OSPlatform { get { throw null; } }
        public string OSVersion { get { throw null; } }
        [System.CLSCompliant(false)]
        public ulong TotalPhysicalMemory { get { throw null; } }
        [System.CLSCompliant(false)]
        public ulong TotalVirtualMemory { get { throw null; } }
    }
    public partial class Keyboard
    {
        public Keyboard() { }
    }
    public partial class Mouse
    {
        public Mouse() { }
    }
    public partial class Network
    {
        public Network() { }
    }
    public partial class NetworkAvailableEventArgs : System.EventArgs
    {
        public NetworkAvailableEventArgs(bool networkAvailable) { }
        public bool IsNetworkAvailable { get { throw null; } }
    }
    public partial class Ports
    {
        public Ports() { }
    }
    public partial class ServerComputer
    {
        public ServerComputer() { }
        public Clock Clock { get { throw null; } }
        public Microsoft.VisualBasic.MyServices.FileSystemProxy FileSystem { get { throw null; } }
        public ComputerInfo Info { get { throw null; } }
        public string Name { get { throw null; } }
        public Network Network { get { throw null; } }
        public Microsoft.VisualBasic.MyServices.RegistryProxy Registry { get { throw null; } }
    }
}
namespace Microsoft.VisualBasic.FileIO
{
    public enum DeleteDirectoryOption
    {
        ThrowIfDirectoryNonEmpty = 4,
        DeleteAllContents = 5,
    }
    public enum FieldType
    {
        Delimited = 0,
        FixedWidth = 1,
    }
    public partial class FileSystem
    {
        public FileSystem() { }
        public static string CombinePath(string baseDirectory, string relativePath) { throw null; }
        public static void CopyDirectory(string sourceDirectoryName, string destinationDirectoryName) { }
        public static void CopyDirectory(string sourceDirectoryName, string destinationDirectoryName, bool overwrite) { }
        public static void CopyDirectory(string sourceDirectoryName, string destinationDirectoryName, Microsoft.VisualBasic.FileIO.UIOption showUI) { }
        public static void CopyDirectory(string sourceDirectoryName, string destinationDirectoryName, Microsoft.VisualBasic.FileIO.UIOption showUI, Microsoft.VisualBasic.FileIO.UICancelOption onUserCancel) { }
        public static void CopyFile(string sourceFileName, string destinationFileName) { }
        public static void CopyFile(string sourceFileName, string destinationFileName, bool overwrite) { }
        public static void CopyFile(string sourceFileName, string destinationFileName, Microsoft.VisualBasic.FileIO.UIOption showUI) { }
        public static void CopyFile(string sourceFileName, string destinationFileName, Microsoft.VisualBasic.FileIO.UIOption showUI, Microsoft.VisualBasic.FileIO.UICancelOption onUserCancel) { }
        public static void CreateDirectory(string directory) { }
        public static string CurrentDirectory { get { throw null; } set { } }
        public static void DeleteDirectory(string directory, Microsoft.VisualBasic.FileIO.DeleteDirectoryOption onDirectoryNotEmpty) { }
        public static void DeleteDirectory(string directory, Microsoft.VisualBasic.FileIO.UIOption showUI, Microsoft.VisualBasic.FileIO.RecycleOption recycle) { }
        public static void DeleteDirectory(string directory, Microsoft.VisualBasic.FileIO.UIOption showUI, Microsoft.VisualBasic.FileIO.RecycleOption recycle, Microsoft.VisualBasic.FileIO.UICancelOption onUserCancel) { }
        public static void DeleteFile(string file) { }
        public static void DeleteFile(string file, Microsoft.VisualBasic.FileIO.UIOption showUI, Microsoft.VisualBasic.FileIO.RecycleOption recycle) { }
        public static void DeleteFile(string file, Microsoft.VisualBasic.FileIO.UIOption showUI, Microsoft.VisualBasic.FileIO.RecycleOption recycle, Microsoft.VisualBasic.FileIO.UICancelOption onUserCancel) { }
        public static bool DirectoryExists(string directory) { throw null; }
        public static System.Collections.ObjectModel.ReadOnlyCollection<System.IO.DriveInfo> Drives { get { throw null; } }
        public static bool FileExists(string file) { throw null; }
        public static System.Collections.ObjectModel.ReadOnlyCollection<string> FindInFiles(string directory, string containsText, bool ignoreCase, Microsoft.VisualBasic.FileIO.SearchOption searchType) { throw null; }
        public static System.Collections.ObjectModel.ReadOnlyCollection<string> FindInFiles(string directory, string containsText, bool ignoreCase, Microsoft.VisualBasic.FileIO.SearchOption searchType, params string[] fileWildcards) { throw null; }
        public static System.Collections.ObjectModel.ReadOnlyCollection<string> GetDirectories(string directory) { throw null; }
        public static System.Collections.ObjectModel.ReadOnlyCollection<string> GetDirectories(string directory, Microsoft.VisualBasic.FileIO.SearchOption searchType, params string[] wildcards) { throw null; }
        public static System.IO.DirectoryInfo GetDirectoryInfo(string directory) { throw null; }
        public static System.IO.DriveInfo GetDriveInfo(string drive) { throw null; }
        public static System.IO.FileInfo GetFileInfo(string file) { throw null; }
        public static System.Collections.ObjectModel.ReadOnlyCollection<string> GetFiles(string directory) { throw null; }
        public static System.Collections.ObjectModel.ReadOnlyCollection<string> GetFiles(string directory, Microsoft.VisualBasic.FileIO.SearchOption searchType, params string[] wildcards) { throw null; }
        public static string GetName(string path) { throw null; }
        public static string GetParentPath(string path) { throw null; }
        public static string GetTempFileName() { throw null; }
        public static void MoveDirectory(string sourceDirectoryName, string destinationDirectoryName) { }
        public static void MoveDirectory(string sourceDirectoryName, string destinationDirectoryName, bool overwrite) { }
        public static void MoveDirectory(string sourceDirectoryName, string destinationDirectoryName, Microsoft.VisualBasic.FileIO.UIOption showUI) { }
        public static void MoveDirectory(string sourceDirectoryName, string destinationDirectoryName, Microsoft.VisualBasic.FileIO.UIOption showUI, Microsoft.VisualBasic.FileIO.UICancelOption onUserCancel) { }
        public static void MoveFile(string sourceFileName, string destinationFileName) { }
        public static void MoveFile(string sourceFileName, string destinationFileName, bool overwrite) { }
        public static void MoveFile(string sourceFileName, string destinationFileName, Microsoft.VisualBasic.FileIO.UIOption showUI) { }
        public static void MoveFile(string sourceFileName, string destinationFileName, Microsoft.VisualBasic.FileIO.UIOption showUI, Microsoft.VisualBasic.FileIO.UICancelOption onUserCancel) { }
        public static Microsoft.VisualBasic.FileIO.TextFieldParser OpenTextFieldParser(string file) { throw null; }
        public static Microsoft.VisualBasic.FileIO.TextFieldParser OpenTextFieldParser(string file, params int[] fieldWidths) { throw null; }
        public static Microsoft.VisualBasic.FileIO.TextFieldParser OpenTextFieldParser(string file, params string[] delimiters) { throw null; }
        public static System.IO.StreamReader OpenTextFileReader(string file) { throw null; }
        public static System.IO.StreamReader OpenTextFileReader(string file, System.Text.Encoding encoding) { throw null; }
        public static System.IO.StreamWriter OpenTextFileWriter(string file, bool append) { throw null; }
        public static System.IO.StreamWriter OpenTextFileWriter(string file, bool append, System.Text.Encoding encoding) { throw null; }
        public static byte[] ReadAllBytes(string file) { throw null; }
        public static string ReadAllText(string file) { throw null; }
        public static string ReadAllText(string file, System.Text.Encoding encoding) { throw null; }
        public static void RenameDirectory(string directory, string newName) { }
        public static void RenameFile(string file, string newName) { }
        public static void WriteAllBytes(string file, byte[] data, bool append) { }
        public static void WriteAllText(string file, string text, bool append) { }
        public static void WriteAllText(string file, string text, bool append, System.Text.Encoding encoding) { }
    }
    public partial class MalformedLineException : System.Exception
    {
        public MalformedLineException() { }
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        protected MalformedLineException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
        public MalformedLineException(string message) { }
        public MalformedLineException(string message, System.Exception innerException) { }
        public MalformedLineException(string message, long lineNumber) { }
        public MalformedLineException(string message, long lineNumber, System.Exception innerException) { }
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Always)]
        public long LineNumber { get { throw null; } set { } }
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
        public override string ToString() { throw null; }
    }
    public enum RecycleOption
    {
        DeletePermanently = 2,
        SendToRecycleBin = 3,
    }
    public enum SearchOption
    {
        SearchTopLevelOnly = 2,
        SearchAllSubDirectories = 3,
    }
    public partial class SpecialDirectories
    {
        public SpecialDirectories() { }
        public static string AllUsersApplicationData { get { throw null; } }
        public static string CurrentUserApplicationData { get { throw null; } }
        public static string Desktop { get { throw null; } }
        public static string MyDocuments { get { throw null; } }
        public static string MyMusic { get { throw null; } }
        public static string MyPictures { get { throw null; } }
        public static string ProgramFiles { get { throw null; } }
        public static string Programs { get { throw null; } }
        public static string Temp { get { throw null; } }
    }
    public partial class TextFieldParser : System.IDisposable
    {
        public TextFieldParser(System.IO.Stream stream) { }
        public TextFieldParser(System.IO.Stream stream, System.Text.Encoding defaultEncoding) { }
        public TextFieldParser(System.IO.Stream stream, System.Text.Encoding defaultEncoding, bool detectEncoding) { }
        public TextFieldParser(System.IO.Stream stream, System.Text.Encoding defaultEncoding, bool detectEncoding, bool leaveOpen) { }
        public TextFieldParser(System.IO.TextReader reader) { }
        public TextFieldParser(string path) { }
        public TextFieldParser(string path, System.Text.Encoding defaultEncoding) { }
        public TextFieldParser(string path, System.Text.Encoding defaultEncoding, bool detectEncoding) { }
        ~TextFieldParser() { }
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public void Close() { }
        public string[] CommentTokens { get { throw null; } set { } }
        public string[] Delimiters { get { throw null; } set { } }
        protected virtual void Dispose(bool disposing) { }
        void System.IDisposable.Dispose() { }
        public bool EndOfData { get { throw null; } }
        public string ErrorLine { get { throw null; } }
        public long ErrorLineNumber { get { throw null; } }
        public int[] FieldWidths { get { throw null; } set { } }
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public bool HasFieldsEnclosedInQuotes { get { throw null; } set { } }
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public long LineNumber { get { throw null; } }
        public string PeekChars(int numberOfChars) { throw null; }
        public string[] ReadFields() { throw null; }
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public string ReadLine() { throw null; }
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public string ReadToEnd() { throw null; }
        public void SetDelimiters(params string[] delimiters) { }
        public void SetFieldWidths(params int[] fieldWidths) { }
        public Microsoft.VisualBasic.FileIO.FieldType TextFieldType { get { throw null; } set { } }
        public bool TrimWhiteSpace { get { throw null; } set { } }
    }
    public enum UICancelOption
    {
        DoNothing = 2,
        ThrowException = 3,
    }
    public enum UIOption
    {
        OnlyErrorDialogs = 2,
        AllDialogs = 3,
    }
}
namespace Microsoft.VisualBasic.MyServices
{
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
    public partial class ClipboardProxy
    {
        internal ClipboardProxy() { }
    }
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
    public partial class FileSystemProxy
    {
        internal FileSystemProxy() { }
        public string CombinePath(string baseDirectory, string relativePath) { throw null; }
        public void CopyDirectory(string sourceDirectoryName, string destinationDirectoryName) { }
        public void CopyDirectory(string sourceDirectoryName, string destinationDirectoryName, bool overwrite) { }
        public void CopyDirectory(string sourceDirectoryName, string destinationDirectoryName, Microsoft.VisualBasic.FileIO.UIOption showUI) { }
        public void CopyDirectory(string sourceDirectoryName, string destinationDirectoryName, Microsoft.VisualBasic.FileIO.UIOption showUI, Microsoft.VisualBasic.FileIO.UICancelOption onUserCancel) { }
        public void CopyFile(string sourceFileName, string destinationFileName) { }
        public void CopyFile(string sourceFileName, string destinationFileName, bool overwrite) { }
        public void CopyFile(string sourceFileName, string destinationFileName, Microsoft.VisualBasic.FileIO.UIOption showUI) { }
        public void CopyFile(string sourceFileName, string destinationFileName, Microsoft.VisualBasic.FileIO.UIOption showUI, Microsoft.VisualBasic.FileIO.UICancelOption onUserCancel) { }
        public void CreateDirectory(string directory) { }
        public string CurrentDirectory { get { throw null; } set { } }
        public void DeleteDirectory(string directory, Microsoft.VisualBasic.FileIO.DeleteDirectoryOption onDirectoryNotEmpty) { }
        public void DeleteDirectory(string directory, Microsoft.VisualBasic.FileIO.UIOption showUI, Microsoft.VisualBasic.FileIO.RecycleOption recycle) { }
        public void DeleteDirectory(string directory, Microsoft.VisualBasic.FileIO.UIOption showUI, Microsoft.VisualBasic.FileIO.RecycleOption recycle, Microsoft.VisualBasic.FileIO.UICancelOption onUserCancel) { }
        public void DeleteFile(string file) { }
        public void DeleteFile(string file, Microsoft.VisualBasic.FileIO.UIOption showUI, Microsoft.VisualBasic.FileIO.RecycleOption recycle) { }
        public void DeleteFile(string file, Microsoft.VisualBasic.FileIO.UIOption showUI, Microsoft.VisualBasic.FileIO.RecycleOption recycle, Microsoft.VisualBasic.FileIO.UICancelOption onUserCancel) { }
        public bool DirectoryExists(string directory) { throw null; }
        public System.Collections.ObjectModel.ReadOnlyCollection<System.IO.DriveInfo> Drives { get { throw null; } }
        public bool FileExists(string file) { throw null; }
        public System.Collections.ObjectModel.ReadOnlyCollection<string> FindInFiles(string directory, string containsText, bool ignoreCase, FileIO.SearchOption searchType) { throw null; }
        public System.Collections.ObjectModel.ReadOnlyCollection<string> FindInFiles(string directory, string containsText, bool ignoreCase, FileIO.SearchOption searchType, params string[] fileWildcards) { throw null; }
        public System.Collections.ObjectModel.ReadOnlyCollection<string> GetDirectories(string directory) { throw null; }
        public System.Collections.ObjectModel.ReadOnlyCollection<string> GetDirectories(string directory, FileIO.SearchOption searchType, params string[] wildcards) { throw null; }
        public System.IO.DirectoryInfo GetDirectoryInfo(string directory) { throw null; }
        public System.IO.DriveInfo GetDriveInfo(string drive) { throw null; }
        public System.IO.FileInfo GetFileInfo(string file) { throw null; }
        public System.Collections.ObjectModel.ReadOnlyCollection<string> GetFiles(string directory) { throw null; }
        public System.Collections.ObjectModel.ReadOnlyCollection<string> GetFiles(string directory, FileIO.SearchOption searchType, params string[] wildcards) { throw null; }
        public string GetName(string path) { throw null; }
        public string GetParentPath(string path) { throw null; }
        public string GetTempFileName() { throw null; }
        public void MoveDirectory(string sourceDirectoryName, string destinationDirectoryName) { }
        public void MoveDirectory(string sourceDirectoryName, string destinationDirectoryName, bool overwrite) { }
        public void MoveDirectory(string sourceDirectoryName, string destinationDirectoryName, Microsoft.VisualBasic.FileIO.UIOption showUI) { }
        public void MoveDirectory(string sourceDirectoryName, string destinationDirectoryName, Microsoft.VisualBasic.FileIO.UIOption showUI, Microsoft.VisualBasic.FileIO.UICancelOption onUserCancel) { }
        public void MoveFile(string sourceFileName, string destinationFileName) { }
        public void MoveFile(string sourceFileName, string destinationFileName, bool overwrite) { }
        public void MoveFile(string sourceFileName, string destinationFileName, Microsoft.VisualBasic.FileIO.UIOption showUI) { }
        public void MoveFile(string sourceFileName, string destinationFileName, Microsoft.VisualBasic.FileIO.UIOption showUI, Microsoft.VisualBasic.FileIO.UICancelOption onUserCancel) { }
        public Microsoft.VisualBasic.FileIO.TextFieldParser OpenTextFieldParser(string file) { throw null; }
        public Microsoft.VisualBasic.FileIO.TextFieldParser OpenTextFieldParser(string file, params int[] fieldWidths) { throw null; }
        public Microsoft.VisualBasic.FileIO.TextFieldParser OpenTextFieldParser(string file, params string[] delimiters) { throw null; }
        public System.IO.StreamReader OpenTextFileReader(string file) { throw null; }
        public System.IO.StreamReader OpenTextFileReader(string file, System.Text.Encoding encoding) { throw null; }
        public System.IO.StreamWriter OpenTextFileWriter(string file, bool append) { throw null; }
        public System.IO.StreamWriter OpenTextFileWriter(string file, bool append, System.Text.Encoding encoding) { throw null; }
        public byte[] ReadAllBytes(string file) { throw null; }
        public string ReadAllText(string file) { throw null; }
        public string ReadAllText(string file, System.Text.Encoding encoding) { throw null; }
        public void RenameDirectory(string directory, string newName) { }
        public void RenameFile(string file, string newName) { }
        public SpecialDirectoriesProxy SpecialDirectories { get { throw null; } }
        public void WriteAllBytes(string file, byte[] data, bool append) { }
        public void WriteAllText(string file, string text, bool append) { }
        public void WriteAllText(string file, string text, bool append, System.Text.Encoding encoding) { }
    }
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
    public partial class RegistryProxy
    {
        internal RegistryProxy() { }
    }
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
    public partial class SpecialDirectoriesProxy
    {
        internal SpecialDirectoriesProxy() { }
        public string AllUsersApplicationData { get { throw null; } }
        public string CurrentUserApplicationData { get { throw null; } }
        public string Desktop { get { throw null; } }
        public string MyDocuments { get { throw null; } }
        public string MyMusic { get { throw null; } }
        public string MyPictures { get { throw null; } }
        public string Programs { get { throw null; } }
        public string ProgramFiles { get { throw null; } }
        public string Temp { get { throw null; } }
    }
}
