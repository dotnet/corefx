' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Option Explicit On
Option Strict Off

Imports System.Collections
Imports System.Collections.Generic
Imports System.Threading

Namespace Microsoft.VisualBasic.MyServices.Internal

    '''**************************************************************************
    ''' ;SkuSafeHttpContext
    ''' <summary>
    ''' Returns the current HTTPContext or nothing if we are not running in a 
    ''' web context.
    ''' </summary>
    ''' <remarks>
    ''' With the FX dividing into Client and Full skus, we may not always have
    ''' access to the System.Web types.  So we have to test for the presence
    ''' of System.Web.Httpcontext before trying to access it.
    ''' </remarks>
    <Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Never)>
    Friend Class SkuSafeHttpContext
        Public Shared ReadOnly Property Current() As Object
            Get
                'Return the equivalent of System.Web.HttpContext.Current
                If m_HttpContextCurrent IsNot Nothing Then
                    Return m_HttpContextCurrent.GetValue(Nothing, Nothing)
                Else
                    Return Nothing
                End If
            End Get
        End Property

        '''**************************************************************************
        ''' ;InitContext
        ''' <summary>
        ''' Initialize the field that holds the type that allows us to access
        ''' System.Web.HttpContext
        ''' </summary>
        Private Shared Function InitContext() As System.Reflection.PropertyInfo
            Dim HttpContextType As System.Type
            HttpContextType = System.Type.GetType("System.Web.HttpContext,System.Web,Version=0.0.0.0,Culture=neutral,PublicKeyToken=B03F5F7F11D50A3A")

            If HttpContextType IsNot Nothing Then
                Return HttpContextType.GetProperty("Current")
            Else
                Return Nothing
            End If
        End Function

        'This class isn't meant to be constructed.
        Private Sub New()
        End Sub

        Private Shared m_HttpContextCurrent As System.Reflection.PropertyInfo = InitContext()
    End Class

    '''**************************************************************************
    ''' ;ContextValue
    ''' <summary>
    ''' Stores an object in a context appropriate for the environment we are 
    ''' running in (web/windows)
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <remarks>
    ''' "Thread appropriate" means that if we are running on ASP.Net the object will be stored in the 
    ''' context of the current request (meaning the object is stored per request on the web).  Otherwise, 
    ''' the object is stored per CallContext.  Note that an instance of this class can only be associated
    ''' with the one item to be stored/retrieved at a time.
    ''' </remarks>
    <Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Never)>
    Public Class ContextValue(Of T)
        Public Sub New()
            m_ContextKey = System.Guid.NewGuid.ToString
        End Sub

        '''**************************************************************************
        ''' ;Value
        ''' <summary>
        ''' Get the object from the correct thread-appropriate location
        ''' </summary>
        ''' <value></value>
        Public Property Value() As T 'No Synclocks required because we are operating upon instance data and the object is not shared across threads
            Get
                Dim items As IDictionary = GetDictionary()
                Return DirectCast(items(m_ContextKey), T) 'Note, IDictionary(key) can return Nothing and that's ok
            End Get
            Set(ByVal value As T)
                Dim items As IDictionary = GetDictionary()
                items(m_ContextKey) = value
            End Set
        End Property

        Private Shared Function GetDictionary() As IDictionary
            Dim context = SkuSafeHttpContext.Current()
            If context IsNot Nothing Then
                Return context.Items
            End If
            If s_ThreadLocal Is Nothing Then
                Interlocked.CompareExchange(s_ThreadLocal, New ThreadLocal(Of IDictionary)(Function() New Dictionary(Of String, T)), Nothing)
            End If
            Return s_ThreadLocal.Value
        End Function

        Private ReadOnly m_ContextKey As String 'An item is stored in the dictionary by a guid which this string maintains

        Private Shared s_ThreadLocal As ThreadLocal(Of IDictionary)

    End Class 'ContextValue

End Namespace
