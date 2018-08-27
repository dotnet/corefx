' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Option Explicit On
Option Strict On

Imports System
Imports System.ComponentModel
Imports System.Globalization
Imports System.Security

Imports Microsoft.VisualBasic.CompilerServices.Utils

Namespace Microsoft.VisualBasic.FileIO

    '''************************************************************************
    ''';MalformedLineException
    ''' <summary>
    '''  Indicates a line cannot be parsed into fields
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()>
    Public Class MalformedLineException
        Inherits Exception

        '==PUBLIC*************************************************************

        '''********************************************************************
        ''';New
        ''' <summary>
        '''  Creates a new exception with no properties set
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
            MyBase.New()
        End Sub

        '''********************************************************************
        ''';New
        ''' <summary>
        '''  Creates a new exception, setting Message and LineNumber
        ''' </summary>
        ''' <param name="message">The message for the exception</param>
        ''' <param name="lineNumber">The number of the line that is malformed</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal message As String, ByVal lineNumber As Long)
            MyBase.New(message)
            Me.LineNumber = lineNumber
        End Sub

        '''********************************************************************
        ''';New
        ''' <summary>
        '''  Creates a new exception, setting Message
        ''' </summary>
        ''' <param name="message">The message for the exception</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal message As String)
            MyBase.New(message)
        End Sub

        '''********************************************************************
        ''';New
        ''' <summary>
        '''  Creates a new exception, setting Message, LineNumber, and InnerException
        ''' </summary>
        ''' <param name="message">The message for the exception</param>
        ''' <param name="lineNumber">The number of the line that is malformed</param>
        ''' <param name="innerException">The inner exception for the exception</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal message As String, ByVal lineNumber As Long, ByVal innerException As Exception)
            MyBase.New(message, innerException)
            Me.LineNumber = lineNumber
        End Sub

        '''*********************************************************************
        ''';New
        ''' <summary>
        '''  Creates a new exception, setting Message and InnerException
        ''' </summary>
        ''' <param name="message">The message for the exception</param>
        ''' <param name="innerException">The inner exception for the exception</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal message As String, ByVal innerException As Exception)
            MyBase.New(message, innerException)
        End Sub

        '''*********************************************************************
        ''';New
        ''' <summary>
        '''  Constructor used for serialization
        ''' </summary>
        ''' <param name="info"></param>
        ''' <param name="context"></param>
        ''' <remarks></remarks>
        <EditorBrowsable(EditorBrowsableState.Advanced)>
        Protected Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)
            MyBase.New(info, context)

            If info IsNot Nothing Then ' Fix FxCop violation ValidateArgumentsOfPublicMethods.
                LineNumber = info.GetInt32(LINE_NUMBER_PROPERTY)
            Else
                LineNumber = -1
            End If
        End Sub

        '''********************************************************************
        ''';LineNumber
        ''' <summary>
        '''  The number of the offending line
        ''' </summary>
        ''' <value>The line number</value>
        ''' <remarks></remarks>
        <EditorBrowsable(EditorBrowsableState.Always)>
        Public Property LineNumber() As Long

        '''********************************************************************
        ''';GetObjectData
        ''' <summary>
        '''  Supports serialization
        ''' </summary>
        ''' <param name="info"></param>
        ''' <param name="context"></param>
        ''' <remarks></remarks>
        <EditorBrowsable(EditorBrowsableState.Advanced)>
        Public Overrides Sub GetObjectData(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)
            If info IsNot Nothing Then ' Fix FxCop violation ValidateArgumentsOfPublicMethods.
                info.AddValue(LINE_NUMBER_PROPERTY, LineNumber, GetType(Long))
            End If

            MyBase.GetObjectData(info, context)
        End Sub

        '''***************************************************************
        ''';ToString
        ''' <summary>
        '''  Appends extra data to string so that it's available when the exception is caught as an Exception
        ''' </summary>
        ''' <returns>The base ToString plus the Line Number</returns>
        ''' <remarks></remarks>
        Public Overrides Function ToString() As String
            Return MyBase.ToString() & " " & GetResourceString(SR.TextFieldParser_MalformedExtraData, LineNumber.ToString(CultureInfo.InvariantCulture))
        End Function

        ' Name of property used for serialization
        Private Const LINE_NUMBER_PROPERTY As String = "LineNumber"

    End Class
End Namespace
