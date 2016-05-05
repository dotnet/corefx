// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------


namespace System.Diagnostics
{
    /// <summary>
    /// Directs tracing or debugging output to a text writer, such as a stream writer, or to a stream,
    /// such as a file stream.
    /// </summary>
    public partial class DelimitedListTraceListener : System.Diagnostics.TextWriterTraceListener
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DelimitedListTraceListener" />
        /// class that writes to the specified output stream.
        /// </summary>
        /// <param name="stream">The <see cref="IO.Stream" /> to receive the output.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream" /> is null.</exception>
        public DelimitedListTraceListener(System.IO.Stream stream) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="DelimitedListTraceListener" />
        /// class that writes to the specified output stream and has the specified name.
        /// </summary>
        /// <param name="stream">The <see cref="IO.Stream" /> to receive the output.</param>
        /// <param name="name">The name of the new instance of the trace listener.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream" /> is null.</exception>
        public DelimitedListTraceListener(System.IO.Stream stream, string name) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="DelimitedListTraceListener" />
        /// class that writes to the specified text writer.
        /// </summary>
        /// <param name="writer">The <see cref="IO.TextWriter" /> to receive the output.</param>
        /// <exception cref="ArgumentNullException"><paramref name="writer" /> is null.</exception>
        public DelimitedListTraceListener(System.IO.TextWriter writer) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="DelimitedListTraceListener" />
        /// class that writes to the specified text writer and has the specified name.
        /// </summary>
        /// <param name="writer">The <see cref="IO.TextWriter" /> to receive the output.</param>
        /// <param name="name">The name of the new instance of the trace listener.</param>
        /// <exception cref="ArgumentNullException"><paramref name="writer" /> is null.</exception>
        public DelimitedListTraceListener(System.IO.TextWriter writer, string name) { }
        /// <summary>
        /// Gets or sets the delimiter for the delimited list.
        /// </summary>
        /// <returns>
        /// The delimiter for the delimited list.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <see cref="Delimiter" /> is set to null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <see cref="Delimiter" /> is set to an empty
        /// string ("").
        /// </exception>
        public string Delimiter { get { return default(string); } set { } }
        /// <summary>
        /// Writes trace information, a data object, and event information to the output file or stream.
        /// </summary>
        /// <param name="eventCache">
        /// A <see cref="Diagnostics.TraceEventCache" /> object that contains the current process
        /// ID, thread ID, and stack trace information.
        /// </param>
        /// <param name="source">
        /// A name used to identify the output, typically the name of the application that generated the
        /// trace event.
        /// </param>
        /// <param name="eventType">
        /// One of the <see cref="Diagnostics.TraceEventType" /> values specifying the type of
        /// event that has caused the trace.
        /// </param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="data">A data object to write to the output file or stream.</param>
        public override void TraceData(System.Diagnostics.TraceEventCache eventCache, string source, System.Diagnostics.TraceEventType eventType, int id, object data) { }
        /// <summary>
        /// Writes trace information, an array of data objects, and event information to the output file
        /// or stream.
        /// </summary>
        /// <param name="eventCache">
        /// A <see cref="Diagnostics.TraceEventCache" /> object that contains the current process
        /// ID, thread ID, and stack trace information.
        /// </param>
        /// <param name="source">
        /// A name used to identify the output, typically the name of the application that generated the
        /// trace event.
        /// </param>
        /// <param name="eventType">
        /// One of the <see cref="Diagnostics.TraceEventType" /> values specifying the type of
        /// event that has caused the trace.
        /// </param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="data">An array of data objects to write to the output file or stream.</param>
        public override void TraceData(System.Diagnostics.TraceEventCache eventCache, string source, System.Diagnostics.TraceEventType eventType, int id, params object[] data) { }
        /// <summary>
        /// Writes trace information, a message, and event information to the output file or stream.
        /// </summary>
        /// <param name="eventCache">
        /// A <see cref="Diagnostics.TraceEventCache" /> object that contains the current process
        /// ID, thread ID, and stack trace information.
        /// </param>
        /// <param name="source">
        /// A name used to identify the output, typically the name of the application that generated the
        /// trace event.
        /// </param>
        /// <param name="eventType">
        /// One of the <see cref="Diagnostics.TraceEventType" /> values specifying the type of
        /// event that has caused the trace.
        /// </param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="message">The trace message to write to the output file or stream.</param>
        public override void TraceEvent(System.Diagnostics.TraceEventCache eventCache, string source, System.Diagnostics.TraceEventType eventType, int id, string message) { }
        /// <summary>
        /// Writes trace information, a formatted array of objects, and event information to the output
        /// file or stream.
        /// </summary>
        /// <param name="eventCache">
        /// A <see cref="Diagnostics.TraceEventCache" /> object that contains the current process
        /// ID, thread ID, and stack trace information.
        /// </param>
        /// <param name="source">
        /// A name used to identify the output, typically the name of the application that generated the
        /// trace event.
        /// </param>
        /// <param name="eventType">
        /// One of the <see cref="Diagnostics.TraceEventType" /> values specifying the type of
        /// event that has caused the trace.
        /// </param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="format">
        /// A format string that contains zero or more format items that correspond to objects in the
        /// <paramref name="args" /> array.
        /// </param>
        /// <param name="args">An array containing zero or more objects to format.</param>
        public override void TraceEvent(System.Diagnostics.TraceEventCache eventCache, string source, System.Diagnostics.TraceEventType eventType, int id, string format, params object[] args) { }
    }
    /// <summary>
    /// Directs tracing or debugging output to a <see cref="IO.TextWriter" /> or to a
    /// <see cref="IO.Stream" />, such as <see cref="IO.FileStream" />.
    /// </summary>
    public partial class TextWriterTraceListener : System.Diagnostics.TraceListener
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextWriterTraceListener" />
        /// class with <see cref="IO.TextWriter" /> as the output recipient.
        /// </summary>
        public TextWriterTraceListener() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="TextWriterTraceListener" />
        /// class, using the stream as the recipient of the debugging and tracing output.
        /// </summary>
        /// <param name="stream">
        /// A <see cref="IO.Stream" /> that represents the stream the
        /// <see cref="TextWriterTraceListener" /> writes to.
        /// </param>
        /// <exception cref="ArgumentNullException">The stream is null.</exception>
        public TextWriterTraceListener(System.IO.Stream stream) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="TextWriterTraceListener" />
        /// class with the specified name, using the stream as the recipient of the debugging and tracing
        /// output.
        /// </summary>
        /// <param name="stream">
        /// A <see cref="IO.Stream" /> that represents the stream the
        /// <see cref="TextWriterTraceListener" /> writes to.
        /// </param>
        /// <param name="name">The name of the new instance.</param>
        /// <exception cref="ArgumentNullException">The stream is null.</exception>
        public TextWriterTraceListener(System.IO.Stream stream, string name) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="TextWriterTraceListener" />
        /// class using the specified writer as recipient of the tracing or debugging output.
        /// </summary>
        /// <param name="writer">
        /// A <see cref="IO.TextWriter" /> that receives the output from the
        /// <see cref="TextWriterTraceListener" />.
        /// </param>
        /// <exception cref="ArgumentNullException">The writer is null.</exception>
        public TextWriterTraceListener(System.IO.TextWriter writer) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="TextWriterTraceListener" />
        /// class with the specified name, using the specified writer as recipient of the tracing or
        /// debugging output.
        /// </summary>
        /// <param name="writer">
        /// A <see cref="IO.TextWriter" /> that receives the output from the
        /// <see cref="TextWriterTraceListener" />.
        /// </param>
        /// <param name="name">The name of the new instance.</param>
        /// <exception cref="ArgumentNullException">The writer is null.</exception>
        public TextWriterTraceListener(System.IO.TextWriter writer, string name) { }
        /// <summary>
        /// Gets or sets the text writer that receives the tracing or debugging output.
        /// </summary>
        /// <returns>
        /// A <see cref="IO.TextWriter" /> that represents the writer that receives the tracing
        /// or debugging output.
        /// </returns>
        public System.IO.TextWriter Writer { get { return default(System.IO.TextWriter); } set { } }
        /// <summary>
        /// Disposes this <see cref="TextWriterTraceListener" /> object.
        /// </summary>
        /// <param name="disposing">
        /// true to release managed resources; if false,
        /// <see cref="Dispose(Boolean)" /> has no effect.
        /// </param>
        protected override void Dispose(bool disposing) { }
        /// <summary>
        /// Flushes the output buffer for the <see cref="Writer" />.
        /// </summary>
        public override void Flush() { }
        /// <summary>
        /// Writes a message to this instance's <see cref="Writer" />.
        /// </summary>
        /// <param name="message">A message to write.</param>
        public override void Write(string message) { }
        /// <summary>
        /// Writes a message to this instance's <see cref="Writer" />
        /// followed by a line terminator. The default line terminator is a carriage return followed
        /// by a line feed (\r\n).
        /// </summary>
        /// <param name="message">A message to write.</param>
        public override void WriteLine(string message) { }
    }
}
