' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Namespace Global.Microsoft.VisualBasic
    <Global.System.Diagnostics.DebuggerNonUserCode()>
    Public Module Constants
        Public Const vbCrLf As String = ChrW(13) & ChrW(10)
        <Global.System.Obsolete("For a carriage return and line feed, use vbCrLf.  For the current platform's newline, use System.Environment.NewLine.")>
        Public Const vbNewLine As String = ChrW(13) & ChrW(10)
        Public Const vbCr As String = ChrW(13)
        Public Const vbLf As String = ChrW(10)
        Public Const vbBack As String = ChrW(8)
        Public Const vbFormFeed As String = ChrW(12)
        Public Const vbTab As String = ChrW(9)
        Public Const vbVerticalTab As String = ChrW(11)
        Public Const vbNullChar As String = ChrW(0)
        Public Const vbNullString As String = Nothing

        'vbCompareMethod enum values
        Public Const vbBinaryCompare As CompareMethod = CompareMethod.Binary
        Public Const vbTextCompare As CompareMethod = CompareMethod.Text

        'MsgBoxResult
        Public Const vbOK As MsgBoxResult = MsgBoxResult.OK
        Public Const vbCancel As MsgBoxResult = MsgBoxResult.Cancel
        Public Const vbAbort As MsgBoxResult = MsgBoxResult.Abort
        Public Const vbRetry As MsgBoxResult = MsgBoxResult.Retry
        Public Const vbIgnore As MsgBoxResult = MsgBoxResult.Ignore
        Public Const vbYes As MsgBoxResult = MsgBoxResult.Yes
        Public Const vbNo As MsgBoxResult = MsgBoxResult.No

        'MsgBoxStyle
        'You may BitOr one value from each group
        Public Const vbOKOnly As MsgBoxStyle = MsgBoxStyle.OKOnly
        Public Const vbOKCancel As MsgBoxStyle = MsgBoxStyle.OKCancel
        Public Const vbAbortRetryIgnore As MsgBoxStyle = MsgBoxStyle.AbortRetryIgnore
        Public Const vbYesNoCancel As MsgBoxStyle = MsgBoxStyle.YesNoCancel
        Public Const vbYesNo As MsgBoxStyle = MsgBoxStyle.YesNo
        Public Const vbRetryCancel As MsgBoxStyle = MsgBoxStyle.RetryCancel
        Public Const vbCritical As MsgBoxStyle = MsgBoxStyle.Critical
        Public Const vbQuestion As MsgBoxStyle = MsgBoxStyle.Question
        Public Const vbExclamation As MsgBoxStyle = MsgBoxStyle.Exclamation
        Public Const vbInformation As MsgBoxStyle = MsgBoxStyle.Information
        Public Const vbDefaultButton1 As MsgBoxStyle = MsgBoxStyle.DefaultButton1
        Public Const vbDefaultButton2 As MsgBoxStyle = MsgBoxStyle.DefaultButton2
        Public Const vbDefaultButton3 As MsgBoxStyle = MsgBoxStyle.DefaultButton3
        Public Const vbApplicationModal As MsgBoxStyle = MsgBoxStyle.ApplicationModal
        Public Const vbSystemModal As MsgBoxStyle = MsgBoxStyle.SystemModal
        Public Const vbMsgBoxHelp As MsgBoxStyle = MsgBoxStyle.MsgBoxHelp
        Public Const vbMsgBoxRight As MsgBoxStyle = MsgBoxStyle.MsgBoxRight
        Public Const vbMsgBoxRtlReading As MsgBoxStyle = MsgBoxStyle.MsgBoxRtlReading
        Public Const vbMsgBoxSetForeground As MsgBoxStyle = MsgBoxStyle.MsgBoxSetForeground
    End Module
End Namespace
