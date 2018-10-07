Namespace Microsoft.VisualBasic
    Partial Public Module Interaction
        Public Function MsgBox(ByVal Prompt As Object, Optional ByVal Buttons As MsgBoxStyle = MsgBoxStyle.OkOnly, Optional ByVal Title As Object = Nothing) As MsgBoxResult
            Throw New PlatformNotSupportedException("MsgBox")
        End Function
    End Module
End Namespace
