' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Imports System

Namespace Microsoft.VisualBasic
    ''' <summary>
    ''' ComClass is used by the VB compiler to mark a public class 
    ''' that will be exposed via COM interop.
    ''' </summary>
    <AttributeUsage(AttributeTargets.Class, Inherited:=False, AllowMultiple:=False)>
    Public NotInheritable Class ComClassAttribute
        Inherits Attribute

        Private m_ClassID As String ' Do not rename - the runtime relies on this name.
        Private m_InterfaceID As String ' Do not rename - the runtime relies on this name.
        Private m_EventID As String ' Do not rename - the runtime relies on this name.
        Private m_InterfaceShadows As Boolean = False ' Do not rename - the runtime relies on this name.

        Public Sub New()
        End Sub

        Public Sub New(ByVal _ClassID As String)
            m_ClassID = _ClassID
        End Sub

        Public Sub New(ByVal _ClassID As String, ByVal _InterfaceID As String)
            m_ClassID = _ClassID
            m_InterfaceID = _InterfaceID
        End Sub

        Public Sub New(ByVal _ClassID As String, ByVal _InterfaceID As String, ByVal _EventId As String)
            m_ClassID = _ClassID
            m_InterfaceID = _InterfaceID
            m_EventID = _EventId
        End Sub

        Public ReadOnly Property ClassID() As String
            Get
                Return m_ClassID
            End Get
        End Property

        Public ReadOnly Property InterfaceID() As String
            Get
                Return m_InterfaceID
            End Get
        End Property

        Public ReadOnly Property EventID() As String
            Get
                Return m_EventID
            End Get
        End Property

        Public Property InterfaceShadows() As Boolean
            Get
                Return m_InterfaceShadows
            End Get
            Set(ByVal Value As Boolean)
                m_InterfaceShadows = Value
            End Set
        End Property
    End Class
End Namespace
