' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Imports System
Imports System.ComponentModel

Namespace Microsoft.VisualBasic
    ''' <summary>
    ''' This attribute is put on an empty 'container class' that the compiler then fills with
    ''' properties that return instances of all the types found in the project which derive
    ''' from the TypeToCollect argument.
    ''' 
    ''' This is how My.Forms is built, for instance.
    ''' </summary>
    ''' <remarks>
    ''' WARNING: Do not rename this attribute or move it out of this module.  Otherwise there
    ''' are compiler changes that will need to be made
    ''' </remarks>
    <AttributeUsage(AttributeTargets.Class, AllowMultiple:=False, Inherited:=False)> _
    <EditorBrowsable(EditorBrowsableState.Advanced)> _
    Public NotInheritable Class MyGroupCollectionAttribute : Inherits Attribute

        Private m_NameOfBaseTypeToCollect As String ' Do not rename - the runtime relies on this name.
        Private m_NameOfCreateMethod As String ' Do not rename - the runtime relies on this name.
        Private m_NameOfDisposeMethod As String ' Do not rename - the runtime relies on this name.
        Private m_DefaultInstanceAlias As String ' Do not rename - the runtime relies on this name.

        ''' <summary>
        ''' </summary>
        ''' <param name="TypeToCollect">Compiler will generate accessors for classes that derived from this type</param>
        ''' <param name="CreateInstanceMethodName">Name of the factory method to create the instances</param>
        ''' <param name="DisposeInstanceMethodName">Name of the method that will dispose of the instances</param>
        ''' <param name="DefaultInstanceAlias">"Name of the My.* method to call to get the default instance for the types in the container</param>
        Public Sub New(ByVal typeToCollect As String, ByVal createInstanceMethodName As String, _
                                ByVal disposeInstanceMethodName As String, ByVal defaultInstanceAlias As String)

            m_NameOfBaseTypeToCollect = typeToCollect
            m_NameOfCreateMethod = createInstanceMethodName
            m_NameOfDisposeMethod = disposeInstanceMethodName
            m_DefaultInstanceAlias = defaultInstanceAlias

        End Sub

        ''' <summary>
        ''' The name of the base type we are trying to collect
        ''' </summary>
        Public ReadOnly Property MyGroupName() As String
            Get
                Return m_NameOfBaseTypeToCollect
            End Get
        End Property

        ''' <summary>
        ''' Name of the factory method to create the instances
        ''' </summary>
        Public ReadOnly Property CreateMethod() As String
            Get
                Return m_NameOfCreateMethod
            End Get
        End Property

        ''' <summary>
        ''' Name of the method that will dispose of the instances
        ''' </summary>
        Public ReadOnly Property DisposeMethod() As String
            Get
                Return m_NameOfDisposeMethod
            End Get
        End Property

        ''' <summary>
        ''' Provides the name of the My.* methods to call to get the 'default instance' 
        ''' </summary>
        Public ReadOnly Property DefaultInstanceAlias() As String
            Get
                Return m_DefaultInstanceAlias
            End Get
        End Property
    End Class
End Namespace
