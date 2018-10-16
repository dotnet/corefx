Imports System

Namespace Microsoft.VisualBasic.FileIO
    Partial Public Class FileSystem
        Private Shared Sub ShellCopyOrMove(operation As CopyOrMove, directory As FileOrDirectory, sourceDirectoryFullPath As String, targetDirectoryFullPath As String, showUI As UIOptionInternal, onUserCancel As UICancelOption)
            Throw New PlatformNotSupportedException(SR.NoShellCopyOrMove)
        End Sub

        Private Shared Sub ShellDelete(directoryFullPath As String, showUI As UIOptionInternal, recycle As RecycleOption, onUserCancel As UICancelOption, directory As FileOrDirectory)
            Throw New PlatformNotSupportedException(SR.NoShellDelete)
        End Sub
    End Class
End Namespace
