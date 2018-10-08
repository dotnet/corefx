Imports Xunit

Namespace Microsoft.VisualBasic.Tests.VB
    Public NotInheritable Class MsgBoxTests
        Sub New()
        End Sub

        Public Shared ReadOnly Property ManualTestsEnabled() As Boolean
            Get
                Return Not String.IsNullOrEmpty(Environment.GetEnvironmentVariable("MANUAL_TESTS"))
            End Get
        End Property

        <ConditionalFact(NameOf(ManualTestsEnabled))>
        <PlatformSpecific(TestPlatforms.Windows)>
        Public Sub MsgBoxTestWindows()
            ' Check default buttons
            Assert.Equal(MsgBox("Hit Enter", MsgBoxStyle.OkCancel Or MsgBoxStyle.DefaultButton1), MsgBoxResult.Ok)
            Assert.Equal(MsgBox("Hit Enter", MsgBoxStyle.OkCancel Or MsgBoxStyle.DefaultButton2), MsgBoxResult.Cancel)
            Assert.Equal(MsgBox("Hit Enter", MsgBoxStyle.AbortRetryIgnore Or MsgBoxStyle.DefaultButton3), MsgBoxResult.Ignore)

            Assert.Equal(MsgBox("Select Abort", MsgBoxStyle.AbortRetryIgnore), MsgBoxResult.Abort)
            Assert.Equal(MsgBox("Select Retry", MsgBoxStyle.AbortRetryIgnore), MsgBoxResult.Retry)
            Assert.Equal(MsgBox("Select Ignore", MsgBoxStyle.AbortRetryIgnore), MsgBoxResult.Ignore)

            Assert.Equal(MsgBox("Select OK", MsgBoxStyle.OkCancel), MsgBoxResult.Ok)
            Assert.Equal(MsgBox("Select Cancel", MsgBoxStyle.OkCancel), MsgBoxResult.Cancel)

            ' Check Icons
            Assert.Equal(MsgBox("Select OK if Exclamation icon visible", MsgBoxStyle.OkCancel Or MsgBoxStyle.Exclamation), MsgBoxResult.Ok)
            Assert.Equal(MsgBox("Select OK if Information icon visible", MsgBoxStyle.OkCancel Or MsgBoxStyle.Information), MsgBoxResult.Ok)
            Assert.Equal(MsgBox("Select OK if Question icon visible", MsgBoxStyle.OkCancel Or MsgBoxStyle.Question), MsgBoxResult.Ok)

            'Default buttons
            Assert.Equal(MsgBox("Select OK"), MsgBoxResult.Ok)

            ' Check Title
            Assert.Equal(MsgBox($"Select OK if Title = 'Microsoft.VisualBasic.Tests'", MsgBoxStyle.OkCancel), MsgBoxResult.Ok)
            Assert.Equal(MsgBox($"Select OK if Title = 'Hello'", MsgBoxStyle.OkCancel, "Hello"), MsgBoxResult.Ok)
        End Sub

        <Fact>
        <PlatformSpecific(TestPlatforms.AnyUnix)>
        Public Sub MsgBoxTestUnix()
            Assert.Throws(Of PlatformNotSupportedException)(Sub() MsgBox("Select OK"))
        End Sub
    End Class
End Namespace
