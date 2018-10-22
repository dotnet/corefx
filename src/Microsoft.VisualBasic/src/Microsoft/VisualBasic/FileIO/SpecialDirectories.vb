' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.
Option Explicit On
Option Strict On

Imports System
Imports System.Environment
Imports System.Security
Imports Microsoft.VisualBasic.CompilerServices.Utils
Imports ExUtils = Microsoft.VisualBasic.CompilerServices.ExceptionUtils

Namespace Microsoft.VisualBasic.FileIO

    ''' <summary>
    ''' This class contains properties that will return the Special Directories
    ''' specific to the current user (My Documents, My Music ...) and those specific
    ''' to the current Application that a developer expects to be able to find quickly.
    ''' </summary>
    Partial Public NotInheritable Class SpecialDirectories
        Public Sub New()
        End Sub

        Private Shared ReadOnly Separators() As Char = {
            IO.Path.DirectorySeparatorChar,
            IO.Path.AltDirectorySeparatorChar
            }

        ''' <summary>
        ''' Return the directory that serves as a common repository for user's personal documents.
        ''' </summary>
        ''' <value>A String containing the path to the user's personal documents.</value>
        ''' <exception cref="IO.DirectoryNotFoundException">If the system does not have the notion of My Documents directory.</exception>
        ''' <remarks>This directory is usually: C:\Documents and Settings\[UserName]\My Documents.</remarks>
        Public Shared ReadOnly Property MyDocuments() As String
            Get
                Return GetDirectoryPath(Environment.GetFolderPath(SpecialFolder.Personal), SR.IO_SpecialDirectory_MyDocuments)
            End Get
        End Property


        ''' <summary>
        ''' Return the "My Music" directory.
        ''' </summary>
        ''' <value>A String containing the path to the user's "My Music" directory.</value>
        ''' <exception cref="IO.DirectoryNotFoundException">If the system does not have the notion of My Music directory.</exception>
        ''' <remarks>This directory is C:\Documents and Settings\[UserName]\My Music</remarks>
        Public Shared ReadOnly Property MyMusic() As String
            Get
                Return GetDirectoryPath(Environment.GetFolderPath(SpecialFolder.MyMusic), SR.IO_SpecialDirectory_MyMusic)
            End Get
        End Property

        ''' <summary>
        ''' Return the "My Pictures" directory.
        ''' </summary>
        ''' <value>A String containing the path to the user's "My Pictures" directory.</value>
        ''' <exception cref="IO.DirectoryNotFoundException">If the system does not have the notion of My Pictures directory.</exception>
        ''' <remarks>This directory is C:\Documents and Settings\[UserName]\My Pictures.</remarks>
        Public Shared ReadOnly Property MyPictures() As String
            Get
                Return GetDirectoryPath(Environment.GetFolderPath(SpecialFolder.MyPictures), SR.IO_SpecialDirectory_MyPictures)
            End Get
        End Property

        ''' <summary>
        ''' Return the current user's Desktop directory.
        ''' </summary>
        ''' <value>A String containing the path to the current user's Desktop directory.</value>
        ''' <remarks>This directory is C:\Document and Settings\[UserName]\Desktop.</remarks>
        Public Shared ReadOnly Property Desktop() As String
            Get
                Return GetDirectoryPath(Environment.GetFolderPath(SpecialFolder.Desktop), SR.IO_SpecialDirectory_Desktop)
            End Get
        End Property

        ''' <summary>
        ''' Returns the directory used to store program shortcuts from Start Menu for current user.
        ''' </summary>
        ''' <value>A String containing the path to the Start Menu \ Programs directory.</value>
        ''' <remarks>This directory is C:\Document and Settings\[UserName]\Start Menu\Programs.</remarks>
        Public Shared ReadOnly Property Programs() As String
            Get
                Return GetDirectoryPath(Environment.GetFolderPath(SpecialFolder.Programs), SR.IO_SpecialDirectory_Programs)
            End Get
        End Property

        ''' <summary>
        ''' Return the program files directory.
        ''' </summary>
        ''' <value>A String containing the path to the default program directories.</value>
        ''' <remarks>This directory is C:\Program Files.</remarks>
        Public Shared ReadOnly Property ProgramFiles() As String
            Get
                Return GetDirectoryPath(Environment.GetFolderPath(SpecialFolder.ProgramFiles), SR.IO_SpecialDirectory_ProgramFiles)
            End Get
        End Property

        ''' <summary>
        ''' Return the directory that contain temporary files for the current user.
        ''' </summary>
        ''' <value>A String containing the path to the temporary directory for the current user.</value>
        ''' <remarks>
        ''' According to Win32 API document, GetTempPath should always return a value even if TEMP and TMP = "".
        ''' Also, this is not updated if TEMP or TMP is changed in Windows. The reason is
        '''     each process has its own copy of the environment variables and this copy is not updated.
        ''' </remarks>
        Public Shared ReadOnly Property Temp() As String
            Get
                Return GetDirectoryPath(IO.Path.GetTempPath(), SR.IO_SpecialDirectory_Temp)
            End Get
        End Property

        ''' <summary>
        ''' Returns the directory that serves as a common repository for data files
        ''' from your application used only by the current user.
        ''' </summary>
        ''' <value>A String containing the path to the directory your application can use to store data for the current user.</value>
        ''' <remarks>
        ''' We chose to use UserAppDataPath instead of LocalUserAppDataPath since this directory
        ''' will work with Roaming User as well.
        ''' </remarks>
        Public Shared ReadOnly Property CurrentUserApplicationData() As String
            Get
                Dim ApplicationData As String = GetDirectoryPath(Environment.GetFolderPath(SpecialFolder.ApplicationData, SpecialFolderOption.Create), SR.IO_SpecialDirectory_UserAppData).Trim.TrimEnd(Separators).Trim
                If ApplicationData.Length = 0 Then
                    Throw ExUtils.GetDirectoryNotFoundException(SR.IO_SpecialDirectoryNotExist, GetResourceString(SR.IO_SpecialDirectory_UserAppData) & $" SpecialFolder.ApplicationData returned String.Empty.")
                End If
                If Not IO.Directory.Exists(ApplicationData) Then
                    Throw ExUtils.GetDirectoryNotFoundException(SR.IO_SpecialDirectoryNotExist, GetResourceString(SR.IO_SpecialDirectory_UserAppData) & $" SpecialFolder.ApplicationData could not create '{ApplicationData}'")
                End If
                Try
                    Return CreateValidFullPath(ApplicationData)
                Catch ex As Exception
                    Throw ExUtils.GetDirectoryNotFoundException(SR.IO_SpecialDirectoryNotExist, GetResourceString(SR.IO_SpecialDirectory_UserAppData) & $" '{ApplicationData}' with real exception {ex.Message}")
                End Try
            End Get
        End Property

        ''' <summary>
        ''' Returns the directory that serves as a common repository for data files
        ''' from your application used by all users.
        ''' </summary>
        ''' <value>A String containing the path to the directory your application can use to store data for all users.</value>
        ''' <remarks>
        ''' If a path does not exist, one is created in the following format
        ''' C:\Documents and Settings\All Users\Application Data\[CompanyName]\[ProductName]\[ProductVersion]
        '''
        ''' See above for reason why we don't use System.Environment.GetFolderPath(*).
        ''' </remarks>
        Public Shared ReadOnly Property AllUsersApplicationData() As String
            Get
                Return CreateValidFullPath(GetDirectoryPath(GetFolderPath(SpecialFolder.CommonApplicationData), SR.IO_SpecialDirectory_AllUserAppData))
            End Get
        End Property

        ''' <summary>
        ''' Return a normalized from a directory path and throw exception if directory path is "".
        ''' </summary>
        ''' <param name="Directory">The special directory's path got back from FX. "" if it does not exist.</param>
        ''' <param name="DirectoryNameResID">The resource ID of the special directory's localized name.</param>
        ''' <returns>A String containing the path to the special directory if success.</returns>
        Private Shared Function GetDirectoryPath(ByVal Directory As String, ByVal DirectoryNameResID As String) As String
            If Directory.Trim.TrimEnd(Separators).TrimEnd.Length = 0 Then
                Throw ExUtils.GetDirectoryNotFoundException(SR.IO_SpecialDirectoryNotExist, GetResourceString(DirectoryNameResID))
            End If
            Return FileSystem.NormalizePath(Directory)
        End Function

        Private Shared Function CreateValidFullPath(FullPath As String) As String
            For Each d As String In GetCompanyProductVersionList()
                FullPath = IO.Path.Combine(FullPath, d)
                If IO.Directory.Exists(FullPath) Then
                    Continue For
                End If
                IO.Directory.CreateDirectory(FullPath)
            Next

            Return FullPath
        End Function

        ''' <summary>
        ''' This takes a full assembly name which includes version and other information and extracts just the name upto the comma
        ''' </summary>
        ''' <param name="AssemblyFullName"></param>
        ''' <returns></returns>
        Private Shared Function GetTitleFromAssemblyFullName(AssemblyFullName As String) As String
            'Find the text up to the first comma. Note, this fails if the assembly has a comma in its name
            Dim FirstCommaLocation As Integer = AssemblyFullName.IndexOf(","c)
            If FirstCommaLocation >= 0 Then
                Return AssemblyFullName.Substring(0, FirstCommaLocation)
            End If
            'The name is not in the format we're expecting so return an empty string
            Return ""
        End Function

        ''' <summary>
        ''' If a path does not exist, one is created in the following format
        ''' C:\Documents and Settings\[UserName]\Application Data\[CompanyName]\[ProductName]\[ProductVersion]
        ''' The first function separates applications by CompanyName, ProductName, ProductVersion.
        ''' The only catch is that CompanyName, ProductName has to be specified in the AssemblyInfo.vb file,
        ''' otherwise the name of the assembly will be used instead (which still has a level of separation).
        ''' </summary>
        ''' <returns>[CompanyName]\[ProductName]\[ProductVersion] </returns>
        Private Shared Function GetCompanyProductVersionList() As List(Of String)
            Dim PathList As New List(Of String)
            Try
                Dim assm As Reflection.Assembly = Reflection.Assembly.GetEntryAssembly()
                If assm Is Nothing Then
                    assm = Reflection.Assembly.GetExecutingAssembly
                    PathList.Add(MakeValidFileName(GetTitleFromAssemblyFullName(assm.FullName)))
                    Return PathList
                End If
                Dim at As Type = GetType(Reflection.AssemblyCompanyAttribute)
                Dim r() As Object = assm.GetCustomAttributes(at, False)
                Dim ct As Reflection.AssemblyCompanyAttribute = (DirectCast(r(0), Reflection.AssemblyCompanyAttribute))
                If Not String.IsNullOrWhiteSpace(ct.Company) Then
                    PathList.Add(MakeValidFileName(ct.Company))
                End If
                If Not String.IsNullOrWhiteSpace(assm.FullName) Then
                    PathList.Add(MakeValidFileName(assm.FullName))
                End If
                If Not String.IsNullOrWhiteSpace(assm.GetName().Version.ToString) Then
                    PathList.Add(MakeValidFileName(assm.GetName().Version.ToString))
                End If
            Catch
            End Try
            Return PathList
        End Function

        ''' <summary>
        ''' Remove invalid characters, leading "." and leading and training spaces.
        ''' </summary>
        ''' <param name="InputName"></param>
        ''' <returns></returns>
        Private Shared Function MakeValidFileName(InputName As String) As String
            Dim invalidFileChars() As Char = IO.Path.GetInvalidFileNameChars()
            For Each c As Char In InputName
                InputName = InputName.Replace(c.ToString(), "")
            Next c
            Return InputName.Trim.TrimStart("."c).TrimStart
        End Function
    End Class
End Namespace
