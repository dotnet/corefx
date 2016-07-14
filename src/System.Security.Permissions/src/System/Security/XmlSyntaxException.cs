namespace System.Security
{
    public sealed partial class XmlSyntaxException : System.SystemException
    {
        public XmlSyntaxException() { }
        public XmlSyntaxException(int lineNumber) { }
        public XmlSyntaxException(int lineNumber, string message) { }
        public XmlSyntaxException(string message) { }
        public XmlSyntaxException(string message, System.Exception inner) { }
    }
}
