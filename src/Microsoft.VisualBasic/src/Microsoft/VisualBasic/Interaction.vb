' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Namespace Microsoft.VisualBasic

    Partial Public Module Interaction

        Public Enum MsgBoxResult
            Ok = 1
            Cancel = 2
            Abort = 3
            Retry = 4
            Ignore = 5
            Yes = 6
            No = 7
        End Enum

        <Flags()>
        Public Enum MsgBoxStyle
            'You may BitOr one value from each group
            'Button group: Lower 4 bits, &H00F 
            OkOnly = &H0I
            OkCancel = &H1I
            AbortRetryIgnore = &H2I
            YesNoCancel = &H3I
            YesNo = &H4I
            RetryCancel = &H5I

            'Icon Group: Middle 4 bits &H0F0
            Critical = &H10I     'Same as Windows.Forms.MessageBox.IconError
            Question = &H20I     'Same As Windows.MessageBox.IconQuestion
            Exclamation = &H30I  'Same As Windows.MessageBox.IconExclamation
            Information = &H40I  'Same As Windows.MessageBox.IconInformation

            'Default Group: High 4 bits &HF00
            DefaultButton1 = 0
            DefaultButton2 = &H100I
            DefaultButton3 = &H200I
            'UNSUPPORTED IN VB7
            'DefaultButton4 = &H300I

            ApplicationModal = &H0I
            SystemModal = &H1000I

            MsgBoxHelp = &H4000I
            MsgBoxRight = &H80000I
            MsgBoxRtlReading = &H100000I
            MsgBoxSetForeground = &H10000I
        End Enum

        Sub New()
        End Sub

        Friend Function IIf(Of T)(ByVal condition As Boolean, ByVal truePart As T, ByVal falsePart As T) As T
            If condition Then
                Return truePart
            End If

            Return falsePart
        End Function
    End Module
End Namespace

