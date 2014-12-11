namespace System
{
    internal class DisposableConsoleColor : IDisposable
    {
        private readonly Action _resetPreviousConsoleSettings;

        public DisposableConsoleColor(Action resetPreviousConsoleSettings )
        {
            _resetPreviousConsoleSettings = resetPreviousConsoleSettings;
        }

        public void Dispose()
        {
            _resetPreviousConsoleSettings();
        }
    }
}