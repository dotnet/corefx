' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Imports Microsoft.VisualBasic.CompilerServices

Namespace Microsoft.VisualBasic

    Public NotInheritable Class Interaction
        Sub New()
        End Sub

        Friend Shared Function IIf(Of T)(ByVal condition As Boolean, ByVal truePart As T, ByVal falsePart As T) As T
            If condition Then
                Return truePart
            End If

            Return falsePart
        End Function

        Public Shared Function MsgBox(ByVal Prompt As Object, Optional ByVal Buttons As MsgBoxStyle = MsgBoxStyle.OkOnly, Optional ByVal Title As Object = Nothing) As MsgBoxResult
            Dim sPrompt As String = Nothing
            Dim sTitle As String
            'Dim vbhost As CompilerServices.IVbHost
            Dim ParentWindow As Integer = Nothing

            'vbhost = CompilerServices.HostServices.VBHost
            'If Not vbhost Is Nothing Then
            '    ParentWindow = vbhost.GetParentWindow()
            'End If

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
                    sPrompt = DirectCast(Conversions.ChangeType(Prompt, GetType(String)), String)
                End If
            Catch ex As StackOverflowException
                Throw ex
            Catch ex As OutOfMemoryException
                Throw ex
            Catch ex As System.Threading.ThreadAbortException
                Throw ex
            Catch
                Throw New ArgumentException(SR.GetResourceString(SR.Argument_InvalidValueType2, "Prompt"), "String")
            End Try

            Try
                If Title Is Nothing Then
                    'If vbhost Is Nothing Then
                    '    sTitle = GetTitleFromAssembly(System.Reflection.Assembly.GetCallingAssembly())
                    'Else
                    '    sTitle = vbhost.GetWindowTitle()
                    'End If
                    sTitle = ""
                Else
                    sTitle = CStr(Title) 'allows the title to be an expression, e.g. msgbox(prompt, Title:=1+5)
                End If
            Catch ex As StackOverflowException
                Throw ex
            Catch ex As OutOfMemoryException
                Throw ex
            Catch ex As Threading.ThreadAbortException
                Throw ex
            Catch
                Throw New ArgumentException(SR.GetResourceString(SR.Argument_InvalidValueType2, "Title"), "String")
            End Try

            Return CType(NativeMethods.MessageBox(ParentWindow, sPrompt, sTitle, CUInt(Buttons)), MsgBoxResult)
        End Function
    End Class
End Namespace

