namespace System.Diagnostics {
  public partial class DiagnosticListener : System.Diagnostics.DiagnosticSource, System.IDisposable, System.IObservable<System.Collections.Generic.KeyValuePair<string, object>> {
    public DiagnosticListener(string name) { }
    public static System.Diagnostics.DiagnosticListener DefaultListener { get { return default(System.Diagnostics.DiagnosticListener); } }
    public string Name { [System.Runtime.CompilerServices.CompilerGeneratedAttribute]get { return default(string); } }
    public static IObservable<DiagnosticListener> AllListeners { get { return default(IObservable<DiagnosticListener>); } } 
    public virtual void Dispose() { }
    public override bool IsEnabled(string name) { return default(bool); }
    public System.IDisposable Subscribe(System.IObserver<System.Collections.Generic.KeyValuePair<string, object>> observer) { return default(System.IDisposable); }
    public virtual System.IDisposable Subscribe(System.IObserver<System.Collections.Generic.KeyValuePair<string, object>> observer, System.Predicate<string> isEnabled) { return default(System.IDisposable); }
    public override void Write(string name, object parameters) { }
  }
  public abstract partial class DiagnosticSource {
    protected DiagnosticSource() { }
    public static System.Diagnostics.DiagnosticSource DefaultSource { get { return default(System.Diagnostics.DiagnosticSource); } }
    public abstract bool IsEnabled(string name);
    public abstract void Write(string name, object value);
  }
}

