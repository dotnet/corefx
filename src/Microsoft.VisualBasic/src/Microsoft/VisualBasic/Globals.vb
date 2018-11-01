' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Namespace Microsoft.VisualBasic
    Public Enum AppWinStyle As Short
        Hide = 0
        NormalFocus = 1
        MinimizedFocus = 2
        MaximizedFocus = 3
        NormalNoFocus = 4
        MinimizedNoFocus = 6
    End Enum

    Public Enum CompareMethod
        [Binary] = 0
        [Text] = 1
    End Enum

    Public Enum DateFormat
        GeneralDate = 0
        LongDate = 1
        ShortDate = 2
        LongTime = 3
        ShortTime = 4
    End Enum

    Public Enum FirstDayOfWeek
        System = 0
        Sunday = 1
        Monday = 2
        Tuesday = 3
        Wednesday = 4
        Thursday = 5
        Friday = 6
        Saturday = 7
    End Enum

    <Flags()> Public Enum FileAttribute
        [Normal] = 0
        [ReadOnly] = 1
        [Hidden] = 2
        [System] = 4
        [Volume] = 8
        [Directory] = 16
        [Archive] = 32
    End Enum

    Public Enum FirstWeekOfYear
        System = 0
        Jan1 = 1
        FirstFourDays = 2
        FirstFullWeek = 3
    End Enum

    <Flags()> Public Enum VbStrConv
        [None] = 0
        [Uppercase] = 1
        [Lowercase] = 2
        [ProperCase] = 3
        [Wide] = 4
        [Narrow] = 8
        [Katakana] = 16
        [Hiragana] = 32
        '[Unicode]      = 64 'OBSOLETE
        '[FromUnicode]   = 128 'OBSOLETE
        [SimplifiedChinese] = 256
        [TraditionalChinese] = 512
        [LinguisticCasing] = 1024
    End Enum

    Public Enum TriState
        [False] = 0
        [True] = -1
        [UseDefault] = -2
    End Enum

    Public Enum DateInterval
        [Year] = 0
        [Quarter] = 1
        [Month] = 2
        [DayOfYear] = 3
        [Day] = 4
        [WeekOfYear] = 5
        [Weekday] = 6
        [Hour] = 7
        [Minute] = 8
        [Second] = 9
    End Enum

    Public Enum DueDate
        EndOfPeriod = 0
        BegOfPeriod = 1
    End Enum

    Public Enum OpenMode
        [Input] = 1
        [Output] = 2
        [Random] = 4
        [Append] = 8
        [Binary] = 32
    End Enum

    Friend Enum OpenModeTypes
        [Input] = 1
        [Output] = 2
        [Random] = 4
        [Append] = 8
        [Binary] = 32
        [Any] = -1
    End Enum

    Public Enum OpenAccess
        [Default] = -1
        [Read] = System.IO.FileAccess.Read
        [ReadWrite] = System.IO.FileAccess.ReadWrite
        [Write] = System.IO.FileAccess.Write
    End Enum

    Public Enum OpenShare
        [Default] = -1
        [Shared] = System.IO.FileShare.ReadWrite
        [LockRead] = System.IO.FileShare.Write
        [LockReadWrite] = System.IO.FileShare.None
        [LockWrite] = System.IO.FileShare.Read
    End Enum

    <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)> _
    Public Structure TabInfo
        Public Column As Short
    End Structure

    <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)> _
    Public Structure SpcInfo
        Public Count As Short
    End Structure

    Public Module Globals
        Public ReadOnly Property ScriptEngine() As String
            Get
                Return "VB"
            End Get
        End Property

        Public ReadOnly Property ScriptEngineMajorVersion() As Integer
            Get
                Return CompilerServices._Version.Major
            End Get
        End Property

        Public ReadOnly Property ScriptEngineMinorVersion() As Integer
            Get
                Return CompilerServices._Version.Minor
            End Get
        End Property

        Public ReadOnly Property ScriptEngineBuildVersion() As Integer
            Get
                Return CompilerServices._Version.Build
            End Get
        End Property
    End Module
End Namespace
