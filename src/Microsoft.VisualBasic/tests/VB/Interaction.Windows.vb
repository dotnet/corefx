' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.
Option Explicit On
Option Strict On

Imports System.Runtime.InteropServices
Imports System.Security
Imports System.Runtime.CompilerServices

Namespace Microsoft.VisualBasic

    Partial Public Module Interaction
        <DllImport("user32.dll", CharSet:=CharSet.Unicode, EntryPoint:="MessageBoxW", SetLastError:=False)>
        Friend Function MessageBox(hWnd As IntPtr, lpText As String, lpCaption As String, uType As UInteger) As Integer
        End Function

        Private Function GetTitleFromAssembly(ByVal CallingAssembly As Reflection.Assembly) As String
            Dim Title As String

            'Get the Assembly name of the calling assembly
            'Assembly.GetName requires PathDiscovery permission so we try this first
            'and if it throws we catch the security exception and parse the name
            'from the full assembly name
            Try
                Title = CallingAssembly.GetName().Name
            Catch ex As SecurityException
                Dim FullName As String = CallingAssembly.FullName
                'Find the text up to the first comma. Note, this fails if the assembly has a comma in its name
                Dim FirstCommaLocation As Integer = FullName.IndexOf(","c)
                If FirstCommaLocation >= 0 Then
                    Title = FullName.Substring(0, FirstCommaLocation)
                Else
                    'The name is not in the format we're expecting so return an empty string
                    Title = ""
                End If
            End Try

            Return Title
        End Function

        Public Function MsgBox(ByVal Prompt As Object, Optional ByVal Buttons As MsgBoxStyle = MsgBoxStyle.OkOnly, Optional ByVal Title As Object = Nothing) As MsgBoxResult
            Dim sPrompt As String = Nothing
            Dim sTitle As String

            'Only allow legal button combinations to be set, one choice from each group
            'These bit constants are defined in System.Windows.Forms.MessageBox
            'Low-order 4 bits (0x000f), legal values: 0, 1, 2, 3, 4, 5
            '     next 4 bits (0x00f0), legal values: 0, &H10, &H20, &H30, &H40
            '     next 4 bits (0x0f00), legal values: 0, &H100, &H200
            If ((Buttons And &HFI) > MsgBoxStyle.RetryCancel) OrElse ((Buttons And &HF0I) > MsgBoxStyle.Information) _
                OrElse ((Buttons And &HF00I) > MsgBoxStyle.DefaultButton3) Then
                Buttons = MsgBoxStyle.OkOnly
            End If

            Try
                If Not Prompt Is Nothing Then
                    sPrompt = CType(Prompt, String)
                End If
            Catch ex As StackOverflowException
                Throw ex
            Catch ex As OutOfMemoryException
                Throw ex
            Catch ex As System.Threading.ThreadAbortException
                Throw ex
            Catch ex As Exception
                Throw New ArgumentException("Argument 'Prompt' cannot be converted to type 'String'.")
            End Try

            Try
                If Title Is Nothing Then
                    sTitle = GetTitleFromAssembly(System.Reflection.Assembly.GetCallingAssembly())
                Else
                    sTitle = CStr(Title) 'allows the title to be an expression, e.g. msgbox(prompt, Title:=1+5)
                End If
            Catch ex As StackOverflowException
                Throw ex
            Catch ex As OutOfMemoryException
                Throw ex
            Catch ex As Threading.ThreadAbortException
                Throw ex
            Catch ex As Exception
                Throw New ArgumentException("Argument 'Title' cannot be converted to type 'String'.")
            End Try

            Return CType(MessageBox(CType(0, IntPtr), sPrompt, sTitle, CUInt(Buttons)), MsgBoxResult)
        End Function
    End Module
End Namespace
