' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.
Option Explicit On
Option Strict On

' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Imports System.Reflection
Imports System.Runtime.InteropServices
Namespace Microsoft.VisualBasic.Tests.VB
    Partial Public Module PathFeatures
        Private Enum State
            Uninitialized
            [True]
            [False]
        End Enum

        ' Note that this class is using APIs that allow it to run on all platforms (including Core 5.0)
        ' That is why we have .GetTypeInfo(), don't use the Registry, etc...

        Private s_osEnabled As State
        Private s_onCore As State

        ''' <summary>
        ''' Returns true if you can use long paths, including long DOS style paths (e.g. over 260 without \\?\).
        ''' </summary>
        Public Function AreAllLongPathsAvailable() As Boolean
            ' We have support built-in for all platforms in Core
            If RunningOnCoreLib Then
                Return True
            End If

            ' Otherwise we're running on Windows, see if we've got the capability in .NET, and that the feature is enabled in the OS
            Return Not AreLongPathsBlocked() AndAlso AreOsLongPathsEnabled()
        End Function

        Public Function IsUsingLegacyPathNormalization() As Boolean
            Return HasLegacyIoBehavior("UseLegacyPathHandling")
        End Function

        ''' <summary>
        ''' Returns true if > MAX_PATH (260) character paths are blocked.
        ''' Note that this doesn't reflect that you can actually use long paths without device syntax when on Windows.
        ''' Use AreAllLongPathsAvailable() to see that you can use long DOS style paths if on Windows.
        ''' </summary>
        Public Function AreLongPathsBlocked() As Boolean
            Return HasLegacyIoBehavior("BlockLongPaths")
        End Function

        Private Function HasLegacyIoBehavior(propertyName As String) As Boolean
            ' Core doesn't have legacy behaviors
            If RunningOnCoreLib Then
                Return False
            End If

            Dim t As Type = GetType(Object).GetTypeInfo().Assembly.GetType("System.AppContextSwitches")
            Dim p = t.GetProperty(propertyName, BindingFlags.Static Or BindingFlags.Public)

            ' If the switch actually exists use it, otherwise we predate the switch and are effectively on
            Return CBool(If(p?.GetValue(Nothing), True))
        End Function

        Private ReadOnly Property RunningOnCoreLib() As Boolean
            Get
                ' Not particularly elegant
                If s_onCore = State.Uninitialized Then
                    s_onCore = If(GetType(Object).GetTypeInfo().Assembly.GetName().Name = "System.Private.CoreLib", State.True, State.False)
                End If

                Return s_onCore = State.True
            End Get
        End Property

        Private Function AreOsLongPathsEnabled() As Boolean
            If s_osEnabled = State.Uninitialized Then
                ' No official way to check yet this is good enough for tests
                Try
                    s_osEnabled = If(RtlAreLongPathsEnabled(), State.True, State.False)
                Catch
                    s_osEnabled = State.False
                End Try
            End If

            Return s_osEnabled = State.True
        End Function
    End Module
End Namespace
