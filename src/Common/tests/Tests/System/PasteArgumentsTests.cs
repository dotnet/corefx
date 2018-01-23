using System;

using Xunit;

namespace Tests.System
{
    public class PasteArgumentsTests
    {
        [Theory]
        [InlineData(@"app.exe arg1 arg2", new[] {"app.exe", "arg1", "arg2"})]
        [InlineData(@"""app name.exe"" arg1 arg2", new[] {"app name.exe", "arg1", "arg2"})]
        [InlineData(@"app.exe \\ arg2", new[] {"app.exe", @"\\", "arg2"})] 
        [InlineData(@"app.exe ""\"""" arg2", new[] {"app.exe", @"""", "arg2"})]  // literal double quotation mark character
        [InlineData(@"app.exe ""\\\"""" arg2", new[] {"app.exe", @"\""", "arg2"})]    // 2N+1 backslashes before quote rule 
        [InlineData(@"app.exe ""\\\\\"""" arg2", new[] {"app.exe", @"\\""", "arg2"})]  // 2N backslashes before quote rule 
        public void Pastes(string pasteExpected ,string[] arguments)
        {
            Assert.Equal(pasteExpected, PasteArguments.Paste(arguments, true));
        }
    }
}
