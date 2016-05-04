// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------


namespace System
{
    /// <summary>
    /// Represents the standard input, output, and error streams for console applications. This class
    /// cannot be inherited.To browse the .NET Framework source code for this type, see the Reference Source.
    /// </summary>
    public static partial class Console
    {
        /// <summary>
        /// Gets or sets the background color of the console.
        /// </summary>
        /// <returns>
        /// A value that specifies the background color of the console; that is, the color that appears
        /// behind each character. The default is black.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// The color specified in a set operation is not a valid member of <see cref="ConsoleColor" />.
        /// </exception>
        /// <exception cref="Security.SecurityException">
        /// The user does not have permission to perform this action.
        /// </exception>
        /// <exception cref="IO.IOException">An I/O error occurred.</exception>
        public static System.ConsoleColor BackgroundColor { get { return default(System.ConsoleColor); } set { } }
        /// <summary>
        /// Plays the sound of a beep through the console speaker.
        /// </summary>
        /// <exception cref="Security.HostProtectionException">
        /// This method was executed on a server, such as SQL Server, that does not permit access to a
        /// user interface.
        /// </exception>
        public static void Beep() { }
        /// <summary>
        /// Plays the sound of a beep of a specified frequency and duration through the console speaker.
        /// </summary>
        /// <param name="frequency">The frequency of the beep, ranging from 37 to 32767 hertz.</param>
        /// <param name="duration">The duration of the beep measured in milliseconds.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="frequency" /> is less than 37 or more than 32767 hertz.-or-<paramref name="duration" />
        /// is less than or equal to zero.
        /// </exception>
        /// <exception cref="Security.HostProtectionException">
        /// This method was executed on a server, such as SQL Server, that does not permit access to the
        /// console.
        /// </exception>
        public static void Beep(int frequency, int duration) { }
        /// <summary>
        /// Gets or sets the height of the buffer area.
        /// </summary>
        /// <returns>
        /// The current height, in rows, of the buffer area.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The value in a set operation is less than or equal to zero.-or- The value in a set operation
        /// is greater than or equal to <see cref="Int16.MaxValue" />.-or- The value in a set
        /// operation is less than <see cref="WindowTop" /> + <see cref="WindowHeight" />.
        /// </exception>
        /// <exception cref="Security.SecurityException">
        /// The user does not have permission to perform this action.
        /// </exception>
        /// <exception cref="IO.IOException">An I/O error occurred.</exception>
        public static int BufferHeight { get { return default(int); } set { } }
        /// <summary>
        /// Gets or sets the width of the buffer area.
        /// </summary>
        /// <returns>
        /// The current width, in columns, of the buffer area.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The value in a set operation is less than or equal to zero.-or- The value in a set operation
        /// is greater than or equal to <see cref="Int16.MaxValue" />.-or- The value in a set
        /// operation is less than <see cref="WindowLeft" /> + <see cref="WindowWidth" />.
        /// </exception>
        /// <exception cref="Security.SecurityException">
        /// The user does not have permission to perform this action.
        /// </exception>
        /// <exception cref="IO.IOException">An I/O error occurred.</exception>
        public static int BufferWidth { get { return default(int); } set { } }
        /// <summary>
        /// Gets a value indicating whether the CAPS LOCK keyboard toggle is turned on or turned off.
        /// </summary>
        /// <returns>
        /// true if CAPS LOCK is turned on; false if CAPS LOCK is turned off.
        /// </returns>
        public static bool CapsLock { get { return default(bool); } }
        /// <summary>
        /// Occurs when the <see cref="ConsoleModifiers.Control" /> modifier key (Ctrl) and either
        /// the <see cref="ConsoleKey.C" /> console key (C) or the Break key are pressed simultaneously
        /// (Ctrl+C or Ctrl+Break).
        /// </summary>
        public static event System.ConsoleCancelEventHandler CancelKeyPress { add { } remove { } }
        /// <summary>
        /// Clears the console buffer and corresponding console window of display information.
        /// </summary>
        /// <exception cref="IO.IOException">An I/O error occurred.</exception>
        public static void Clear() { }
        /// <summary>
        /// Gets or sets the column position of the cursor within the buffer area.
        /// </summary>
        /// <returns>
        /// The current position, in columns, of the cursor.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The value in a set operation is less than zero.-or- The value in a set operation is greater
        /// than or equal to <see cref="BufferWidth" />.
        /// </exception>
        /// <exception cref="Security.SecurityException">
        /// The user does not have permission to perform this action.
        /// </exception>
        /// <exception cref="IO.IOException">An I/O error occurred.</exception>
        public static int CursorLeft { get { return default(int); } set { } }
        /// <summary>
        /// Gets or sets the height of the cursor within a character cell.
        /// </summary>
        /// <returns>
        /// The size of the cursor expressed as a percentage of the height of a character cell. The property
        /// value ranges from 1 to 100.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The value specified in a set operation is less than 1 or greater than 100.
        /// </exception>
        /// <exception cref="Security.SecurityException">
        /// The user does not have permission to perform this action.
        /// </exception>
        /// <exception cref="IO.IOException">An I/O error occurred.</exception>
        public static int CursorSize { get { return default(int); } set { } }
        /// <summary>
        /// Gets or sets the row position of the cursor within the buffer area.
        /// </summary>
        /// <returns>
        /// The current position, in rows, of the cursor.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The value in a set operation is less than zero.-or- The value in a set operation is greater
        /// than or equal to <see cref="BufferHeight" />.
        /// </exception>
        /// <exception cref="Security.SecurityException">
        /// The user does not have permission to perform this action.
        /// </exception>
        /// <exception cref="IO.IOException">An I/O error occurred.</exception>
        public static int CursorTop { get { return default(int); } set { } }
        /// <summary>
        /// Gets or sets a value indicating whether the cursor is visible.
        /// </summary>
        /// <returns>
        /// true if the cursor is visible; otherwise, false.
        /// </returns>
        /// <exception cref="Security.SecurityException">
        /// The user does not have permission to perform this action.
        /// </exception>
        /// <exception cref="IO.IOException">An I/O error occurred.</exception>
        public static bool CursorVisible { get { return default(bool); } set { } }
        /// <summary>
        /// Gets the standard error output stream.
        /// </summary>
        /// <returns>
        /// A <see cref="IO.TextWriter" /> that represents the standard error output stream.
        /// </returns>
        public static System.IO.TextWriter Error { get { return default(System.IO.TextWriter); } }
        /// <summary>
        /// Gets or sets the foreground color of the console.
        /// </summary>
        /// <returns>
        /// A <see cref="ConsoleColor" /> that specifies the foreground color of the console;
        /// that is, the color of each character that is displayed. The default is gray.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// The color specified in a set operation is not a valid member of <see cref="ConsoleColor" />.
        /// </exception>
        /// <exception cref="Security.SecurityException">
        /// The user does not have permission to perform this action.
        /// </exception>
        /// <exception cref="IO.IOException">An I/O error occurred.</exception>
        public static System.ConsoleColor ForegroundColor { get { return default(System.ConsoleColor); } set { } }
        /// <summary>
        /// Gets or sets the encoding the console uses to read input.
        /// </summary>
        /// <returns>
        /// The encoding used to read console input.
        /// </returns>
        /// <exception cref="ArgumentNullException">The property value in a set operation is null.</exception>
        /// <exception cref="IO.IOException">An error occurred during the execution of this operation.</exception>
        /// <exception cref="Security.SecurityException">
        /// Your application does not have permission to perform this operation.
        /// </exception>
        public static System.Text.Encoding InputEncoding { get { return default(System.Text.Encoding); } set { } }
        /// <summary>
        /// Gets a value that indicates whether the error output stream has been redirected from the standard
        /// error stream.
        /// </summary>
        /// <returns>
        /// true if error output is redirected; otherwise, false.
        /// </returns>
        public static bool IsErrorRedirected { get { return false; } }
        /// <summary>
        /// Gets a value that indicates whether input has been redirected from the standard input stream.
        /// </summary>
        /// <returns>
        /// true if input is redirected; otherwise, false.
        /// </returns>
        public static bool IsInputRedirected { get { return false; } }
        /// <summary>
        /// Gets a value that indicates whether output has been redirected from the standard output stream.
        /// </summary>
        /// <returns>
        /// true if output is redirected; otherwise, false.
        /// </returns>
        public static bool IsOutputRedirected { get { return false; } }
        /// <summary>
        /// Gets the standard input stream.
        /// </summary>
        /// <returns>
        /// A <see cref="IO.TextReader" /> that represents the standard input stream.
        /// </returns>
        public static System.IO.TextReader In { get { return default(System.IO.TextReader); } }
        /// <summary>
        /// Gets a value indicating whether a key press is available in the input stream.
        /// </summary>
        /// <returns>
        /// true if a key press is available; otherwise, false.
        /// </returns>
        /// <exception cref="IO.IOException">An I/O error occurred.</exception>
        /// <exception cref="InvalidOperationException">
        /// Standard input is redirected to a file instead of the keyboard.
        /// </exception>
        public static bool KeyAvailable { get { return default(bool); }}
        /// <summary>
        /// Gets the largest possible number of console window columns, based on the current font and screen
        /// resolution.
        /// </summary>
        /// <returns>
        /// The width of the largest possible console window measured in columns.
        /// </returns>
        public static int LargestWindowWidth { get { return default(int); } }
        /// <summary>
        /// Gets the largest possible number of console window rows, based on the current font and screen
        /// resolution.
        /// </summary>
        /// <returns>
        /// The height of the largest possible console window measured in rows.
        /// </returns>
        public static int LargestWindowHeight { get { return default(int); }}
        /// <summary>
        /// Copies a specified source area of the screen buffer to a specified destination area.
        /// </summary>
        /// <param name="sourceLeft">The leftmost column of the source area.</param>
        /// <param name="sourceTop">The topmost row of the source area.</param>
        /// <param name="sourceWidth">The number of columns in the source area.</param>
        /// <param name="sourceHeight">The number of rows in the source area.</param>
        /// <param name="targetLeft">The leftmost column of the destination area.</param>
        /// <param name="targetTop">The topmost row of the destination area.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// One or more of the parameters is less than zero.-or- <paramref name="sourceLeft" /> or
        /// <paramref name="targetLeft" /> is greater than or equal to <see cref="BufferWidth" />.
        /// -or- <paramref name="sourceTop" /> or <paramref name="targetTop" /> is greater than or
        /// equal to <see cref="BufferHeight" />.-or- <paramref name="sourceTop" /> +
        /// <paramref name="sourceHeight" /> is greater than or equal to <see cref="BufferHeight" />.
        /// -or- <paramref name="sourceLeft" /> + <paramref name="sourceWidth" /> is greater than or
        /// equal to <see cref="BufferWidth" />.
        /// </exception>
        /// <exception cref="Security.SecurityException">
        /// The user does not have permission to perform this action.
        /// </exception>
        /// <exception cref="IO.IOException">An I/O error occurred.</exception>
        public static void MoveBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop) { }
        /// <summary>
        /// Copies a specified source area of the screen buffer to a specified destination area.
        /// </summary>
        /// <param name="sourceLeft">The leftmost column of the source area.</param>
        /// <param name="sourceTop">The topmost row of the source area.</param>
        /// <param name="sourceWidth">The number of columns in the source area.</param>
        /// <param name="sourceHeight">The number of rows in the source area.</param>
        /// <param name="targetLeft">The leftmost column of the destination area.</param>
        /// <param name="targetTop">The topmost row of the destination area.</param>
        /// <param name="sourceChar">The character used to fill the source area.</param>
        /// <param name="sourceForeColor">The foreground color used to fill the source area.</param>
        /// <param name="sourceBackColor">The background color used to fill the source area.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// One or more of the parameters is less than zero.-or- <paramref name="sourceLeft" /> or
        /// <paramref name="targetLeft" /> is greater than or equal to <see cref="BufferWidth" />.
        /// -or- <paramref name="sourceTop" /> or <paramref name="targetTop" /> is greater than or
        /// equal to <see cref="BufferHeight" />.-or- <paramref name="sourceTop" /> +
        /// <paramref name="sourceHeight" /> is greater than or equal to <see cref="BufferHeight" />.
        /// -or- <paramref name="sourceLeft" /> + <paramref name="sourceWidth" /> is greater than or
        /// equal to <see cref="BufferWidth" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// One or both of the color parameters is not a member of the <see cref="ConsoleColor" />
        /// enumeration.
        /// </exception>
        /// <exception cref="Security.SecurityException">
        /// The user does not have permission to perform this action.
        /// </exception>
        /// <exception cref="IO.IOException">An I/O error occurred.</exception>
        public static void MoveBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop, char sourceChar, ConsoleColor sourceForeColor, ConsoleColor sourceBackColor) { }
        /// <summary>
        /// Gets a value indicating whether the NUM LOCK keyboard toggle is turned on or turned off.
        /// </summary>
        /// <returns>
        /// true if NUM LOCK is turned on; false if NUM LOCK is turned off.
        /// </returns>
        public static bool NumberLock { get { return default(bool); }}
        /// <summary>
        /// Acquires the standard error stream.
        /// </summary>
        /// <returns>
        /// The standard error stream.
        /// </returns>
        public static System.IO.Stream OpenStandardError() { return default(System.IO.Stream); }
        /// <summary>
        /// Acquires the standard input stream.
        /// </summary>
        /// <returns>
        /// The standard input stream.
        /// </returns>
        public static System.IO.Stream OpenStandardInput() { return default(System.IO.Stream); }
        /// <summary>
        /// Acquires the standard output stream.
        /// </summary>
        /// <returns>
        /// The standard output stream.
        /// </returns>
        public static System.IO.Stream OpenStandardOutput() { return default(System.IO.Stream); }
        /// <summary>
        /// Gets the standard output stream.
        /// </summary>
        /// <returns>
        /// A <see cref="IO.TextWriter" /> that represents the standard output stream.
        /// </returns>
        public static System.IO.TextWriter Out { get { return default(System.IO.TextWriter); } }
        /// <summary>
        /// Gets or sets the encoding the console uses to write output.
        /// </summary>
        /// <returns>
        /// The encoding used to write console output.
        /// </returns>
        /// <exception cref="ArgumentNullException">The property value in a set operation is null.</exception>
        /// <exception cref="IO.IOException">An error occurred during the execution of this operation.</exception>
        /// <exception cref="Security.SecurityException">
        /// Your application does not have permission to perform this operation.
        /// </exception>
        public static System.Text.Encoding OutputEncoding { get { return default(System.Text.Encoding); } set { } }
        /// <summary>
        /// Reads the next character from the standard input stream.
        /// </summary>
        /// <returns>
        /// The next character from the input stream, or negative one (-1) if there are currently no more
        /// characters to be read.
        /// </returns>
        /// <exception cref="IO.IOException">An I/O error occurred.</exception>
        public static int Read() { return default(int); }
        /// <summary>
        /// Obtains the next character or function key pressed by the user. The pressed key is displayed
        /// in the console window.
        /// </summary>
        /// <returns>
        /// An object that describes the <see cref="ConsoleKey" /> constant and Unicode character,
        /// if any, that correspond to the pressed console key. The <see cref="ConsoleKeyInfo" />
        /// object also describes, in a bitwise combination of <see cref="ConsoleModifiers" />
        /// values, whether one or more Shift, Alt, or Ctrl modifier keys was pressed simultaneously
        /// with the console key.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The <see cref="In" /> property is redirected from some stream other than
        /// the console.
        /// </exception>
        public static ConsoleKeyInfo ReadKey() { return default(ConsoleKeyInfo); }
        /// <summary>
        /// Obtains the next character or function key pressed by the user. The pressed key is optionally
        /// displayed in the console window.
        /// </summary>
        /// <param name="intercept">
        /// Determines whether to display the pressed key in the console window. true to not display the
        /// pressed key; otherwise, false.
        /// </param>
        /// <returns>
        /// An object that describes the <see cref="ConsoleKey" /> constant and Unicode character,
        /// if any, that correspond to the pressed console key. The <see cref="ConsoleKeyInfo" />
        /// object also describes, in a bitwise combination of <see cref="ConsoleModifiers" />
        /// values, whether one or more Shift, Alt, or Ctrl modifier keys was pressed simultaneously
        /// with the console key.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The <see cref="In" /> property is redirected from some stream other than
        /// the console.
        /// </exception>
        public static ConsoleKeyInfo ReadKey(bool intercept) { return default(ConsoleKeyInfo); }
        /// <summary>
        /// Reads the next line of characters from the standard input stream.
        /// </summary>
        /// <returns>
        /// The next line of characters from the input stream, or null if no more lines are available.
        /// </returns>
        /// <exception cref="IO.IOException">An I/O error occurred.</exception>
        /// <exception cref="OutOfMemoryException">
        /// There is insufficient memory to allocate a buffer for the returned string.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The number of characters in the next line of characters is greater than <see cref="Int32.MaxValue" />.
        /// </exception>
        public static string ReadLine() { return default(string); }
        /// <summary>
        /// Sets the foreground and background console colors to their defaults.
        /// </summary>
        /// <exception cref="Security.SecurityException">
        /// The user does not have permission to perform this action.
        /// </exception>
        /// <exception cref="IO.IOException">An I/O error occurred.</exception>
        public static void ResetColor() { }
        /// <summary>
        /// Sets the height and width of the screen buffer area to the specified values.
        /// </summary>
        /// <param name="width">The width of the buffer area measured in columns.</param>
        /// <param name="height">The height of the buffer area measured in rows.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="height" /> or <paramref name="width" /> is less than or equal to zero.-or-
        /// <paramref name="height" /> or <paramref name="width" /> is greater than or equal to
        /// <see cref="Int16.MaxValue" />.-or- <paramref name="width" /> is less than <see cref="WindowLeft" />
        /// + <see cref="WindowWidth" />.-or- <paramref name="height" /> is less than
        /// <see cref="WindowTop" /> + <see cref="WindowHeight" />.
        /// </exception>
        /// <exception cref="Security.SecurityException">
        /// The user does not have permission to perform this action.
        /// </exception>
        /// <exception cref="IO.IOException">An I/O error occurred.</exception>
        public static void SetBufferSize(int width, int height) { }
        /// <summary>
        /// Sets the position of the cursor.
        /// </summary>
        /// <param name="left">The column position of the cursor. Columns are numbered from left to right starting at 0.</param>
        /// <param name="top">The row position of the cursor. Rows are numbered from top to bottom starting at 0.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="left" /> or <paramref name="top" /> is less than zero.-or- <paramref name="left" />
        /// is greater than or equal to <see cref="BufferWidth" />.-or- <paramref name="top" />
        /// is greater than or equal to <see cref="BufferHeight" />.
        /// </exception>
        /// <exception cref="Security.SecurityException">
        /// The user does not have permission to perform this action.
        /// </exception>
        /// <exception cref="IO.IOException">An I/O error occurred.</exception>
        public static void SetCursorPosition(int left, int top) { }
        /// <summary>
        /// Sets the <see cref="Error" /> property to the specified <see cref="IO.TextWriter" />
        /// object.
        /// </summary>
        /// <param name="newError">A stream that is the new standard error output.</param>
        /// <exception cref="ArgumentNullException"><paramref name="newError" /> is null.</exception>
        /// <exception cref="Security.SecurityException">The caller does not have the required permission.</exception>
        public static void SetError(System.IO.TextWriter newError) { }
        /// <summary>
        /// Sets the <see cref="In" /> property to the specified <see cref="IO.TextReader" />
        /// object.
        /// </summary>
        /// <param name="newIn">A stream that is the new standard input.</param>
        /// <exception cref="ArgumentNullException"><paramref name="newIn" /> is null.</exception>
        /// <exception cref="Security.SecurityException">The caller does not have the required permission.</exception>
        public static void SetIn(System.IO.TextReader newIn) { }
        /// <summary>
        /// Sets the <see cref="Out" /> property to the specified <see cref="IO.TextWriter" />
        /// object.
        /// </summary>
        /// <param name="newOut">A stream that is the new standard output.</param>
        /// <exception cref="ArgumentNullException"><paramref name="newOut" /> is null.</exception>
        /// <exception cref="Security.SecurityException">The caller does not have the required permission.</exception>
        public static void SetOut(System.IO.TextWriter newOut) { }
        /// <summary>
        /// Sets the position of the console window relative to the screen buffer.
        /// </summary>
        /// <param name="left">The column position of the upper left  corner of the console window.</param>
        /// <param name="top">The row position of the upper left corner of the console window.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="left" /> or <paramref name="top" /> is less than zero.-or- <paramref name="left" />
        /// + <see cref="WindowWidth" /> is greater than <see cref="BufferWidth" />.
        /// -or- <paramref name="top" /> + <see cref="WindowHeight" /> is greater
        /// than <see cref="BufferHeight" />.
        /// </exception>
        /// <exception cref="Security.SecurityException">
        /// The user does not have permission to perform this action.
        /// </exception>
        /// <exception cref="IO.IOException">An I/O error occurred.</exception>
        public static void SetWindowPosition(int left, int top) { }
        /// <summary>
        /// Sets the height and width of the console window to the specified values.
        /// </summary>
        /// <param name="width">The width of the console window measured in columns.</param>
        /// <param name="height">The height of the console window measured in rows.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="width" /> or <paramref name="height" /> is less than or equal to zero.-or-
        /// <paramref name="width" /> plus <see cref="WindowLeft" /> or <paramref name="height" />
        /// plus <see cref="WindowTop" /> is greater than or equal to <see cref="Int16.MaxValue" />.
        /// -or-<paramref name="width" /> or <paramref name="height" /> is greater than the largest
        /// possible window width or height for the current screen resolution and console font.
        /// </exception>
        /// <exception cref="Security.SecurityException">
        /// The user does not have permission to perform this action.
        /// </exception>
        /// <exception cref="IO.IOException">An I/O error occurred.</exception>
        public static void SetWindowSize(int width, int height) { }
        /// <summary>
        /// Gets or sets the title to display in the console title bar.
        /// </summary>
        /// <returns>
        /// The string to be displayed in the title bar of the console. The maximum length of the title
        /// string is 24500 characters.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// In a get operation, the retrieved title is longer than 24500 characters.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// In a set operation, the specified title is longer than 24500 characters.
        /// </exception>
        /// <exception cref="ArgumentNullException">In a set operation, the specified title is null.</exception>
        /// <exception cref="IO.IOException">An I/O error occurred.</exception>
        public static string Title { get { return default(string); } set { } }
        /// <summary>
        /// Gets or sets a value indicating whether the combination of the <see cref="ConsoleModifiers.Control" />
        /// modifier key and <see cref="ConsoleKey.C" /> console key (Ctrl+C) is treated as
        /// ordinary input or as an interruption that is handled by the operating system.
        /// </summary>
        /// <returns>
        /// true if Ctrl+C is treated as ordinary input; otherwise, false.
        /// </returns>
        /// <exception cref="IO.IOException">
        /// Unable to get or set the input mode of the console input buffer.
        /// </exception>
        public static bool TreatControlCAsInput { get { return default(bool); } set { } }
        /// <summary>
        /// Gets or sets the height of the console window area.
        /// </summary>
        /// <returns>
        /// The height of the console window measured in rows.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The value of the <see cref="WindowWidth" /> property or the value of the
        /// <see cref="WindowHeight" /> property is less than or equal to 0.-or-The value
        /// of the <see cref="WindowHeight" /> property plus the value of the <see cref="WindowTop" />
        /// property is greater than or equal to <see cref="Int16.MaxValue" />.-or-The value
        /// of the <see cref="WindowWidth" /> property or the value of the
        /// <see cref="WindowHeight" /> property is greater than the largest possible window width or height for the current screen
        /// resolution and console font.
        /// </exception>
        /// <exception cref="IO.IOException">Error reading or writing information.</exception>
        public static int WindowHeight { get { return default(int); } set { } }
        /// <summary>
        /// Gets or sets the width of the console window.
        /// </summary>
        /// <returns>
        /// The width of the console window measured in columns.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The value of the <see cref="WindowWidth" /> property or the value of the
        /// <see cref="WindowHeight" /> property is less than or equal to 0.-or-The value
        /// of the <see cref="WindowHeight" /> property plus the value of the <see cref="WindowTop" />
        /// property is greater than or equal to <see cref="Int16.MaxValue" />.-or-The value
        /// of the <see cref="WindowWidth" /> property or the value of the
        /// <see cref="WindowHeight" /> property is greater than the largest possible window width or height for the current screen
        /// resolution and console font.
        /// </exception>
        /// <exception cref="IO.IOException">Error reading or writing information.</exception>
        public static int WindowWidth { get { return default(int); } set { } }
        /// <summary>
        /// Gets or sets the leftmost position of the console window area relative to the screen buffer.
        /// </summary>
        /// <returns>
        /// The leftmost console window position measured in columns.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// In a set operation, the value to be assigned is less than zero.-or-As a result of the assignment,
        /// <see cref="WindowLeft" /> plus <see cref="WindowWidth" />
        /// would exceed <see cref="BufferWidth" />.
        /// </exception>
        /// <exception cref="IO.IOException">Error reading or writing information.</exception>
        public static int WindowLeft { get { return default(int); } set { } }
        /// <summary>
        /// Gets or sets the top position of the console window area relative to the screen buffer.
        /// </summary>
        /// <returns>
        /// The uppermost console window position measured in rows.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// In a set operation, the value to be assigned is less than zero.-or-As a result of the assignment,
        /// <see cref="WindowTop" /> plus <see cref="WindowHeight" />
        /// would exceed <see cref="BufferHeight" />.
        /// </exception>
        /// <exception cref="IO.IOException">Error reading or writing information.</exception>
        public static int WindowTop { get { return default(int); } set { } }
        /// <summary>
        /// Writes the text representation of the specified Boolean value to the standard output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="IO.IOException">An I/O error occurred.</exception>
        public static void Write(bool value) { }
        /// <summary>
        /// Writes the specified Unicode character value to the standard output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="IO.IOException">An I/O error occurred.</exception>
        public static void Write(char value) { }
        /// <summary>
        /// Writes the specified array of Unicode characters to the standard output stream.
        /// </summary>
        /// <param name="buffer">A Unicode character array.</param>
        /// <exception cref="IO.IOException">An I/O error occurred.</exception>
        public static void Write(char[] buffer) { }
        /// <summary>
        /// Writes the specified subarray of Unicode characters to the standard output stream.
        /// </summary>
        /// <param name="buffer">An array of Unicode characters.</param>
        /// <param name="index">The starting position in <paramref name="buffer" />.</param>
        /// <param name="count">The number of characters to write.</param>
        /// <exception cref="ArgumentNullException"><paramref name="buffer" /> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> or <paramref name="count" /> is less than zero.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="index" /> plus <paramref name="count" /> specify a position that is not within
        /// <paramref name="buffer" />.
        /// </exception>
        /// <exception cref="IO.IOException">An I/O error occurred.</exception>
        public static void Write(char[] buffer, int index, int count) { }
        /// <summary>
        /// Writes the text representation of the specified <see cref="Decimal" /> value to the
        /// standard output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="IO.IOException">An I/O error occurred.</exception>
        public static void Write(decimal value) { }
        /// <summary>
        /// Writes the text representation of the specified double-precision floating-point value to the
        /// standard output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="IO.IOException">An I/O error occurred.</exception>
        public static void Write(double value) { }
        /// <summary>
        /// Writes the text representation of the specified 32-bit signed integer value to the standard
        /// output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="IO.IOException">An I/O error occurred.</exception>
        public static void Write(int value) { }
        /// <summary>
        /// Writes the text representation of the specified 64-bit signed integer value to the standard
        /// output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="IO.IOException">An I/O error occurred.</exception>
        public static void Write(long value) { }
        /// <summary>
        /// Writes the text representation of the specified object to the standard output stream.
        /// </summary>
        /// <param name="value">The value to write, or null.</param>
        /// <exception cref="IO.IOException">An I/O error occurred.</exception>
        public static void Write(object value) { }
        /// <summary>
        /// Writes the text representation of the specified single-precision floating-point value to the
        /// standard output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="IO.IOException">An I/O error occurred.</exception>
        public static void Write(float value) { }
        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="IO.IOException">An I/O error occurred.</exception>
        public static void Write(string value) { }
        /// <summary>
        /// Writes the text representation of the specified object to the standard output stream using
        /// the specified format information.
        /// </summary>
        /// <param name="format">A composite format string (see Remarks).</param>
        /// <param name="arg0">An object to write using <paramref name="format" />.</param>
        /// <exception cref="IO.IOException">An I/O error occurred.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="format" /> is null.</exception>
        /// <exception cref="FormatException">
        /// The format specification in <paramref name="format" /> is invalid.
        /// </exception>
        public static void Write(string format, object arg0) { }
        /// <summary>
        /// Writes the text representation of the specified objects to the standard output stream using
        /// the specified format information.
        /// </summary>
        /// <param name="format">A composite format string (see Remarks).</param>
        /// <param name="arg0">The first object to write using <paramref name="format" />.</param>
        /// <param name="arg1">The second object to write using <paramref name="format" />.</param>
        /// <exception cref="IO.IOException">An I/O error occurred.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="format" /> is null.</exception>
        /// <exception cref="FormatException">
        /// The format specification in <paramref name="format" /> is invalid.
        /// </exception>
        public static void Write(string format, object arg0, object arg1) { }
        /// <summary>
        /// Writes the text representation of the specified objects to the standard output stream using
        /// the specified format information.
        /// </summary>
        /// <param name="format">A composite format string (see Remarks).</param>
        /// <param name="arg0">The first object to write using <paramref name="format" />.</param>
        /// <param name="arg1">The second object to write using <paramref name="format" />.</param>
        /// <param name="arg2">The third object to write using <paramref name="format" />.</param>
        /// <exception cref="IO.IOException">An I/O error occurred.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="format" /> is null.</exception>
        /// <exception cref="FormatException">
        /// The format specification in <paramref name="format" /> is invalid.
        /// </exception>
        public static void Write(string format, object arg0, object arg1, object arg2) { }
        /// <summary>
        /// Writes the text representation of the specified array of objects to the standard output stream
        /// using the specified format information.
        /// </summary>
        /// <param name="format">A composite format string (see Remarks).</param>
        /// <param name="arg">An array of objects to write using <paramref name="format" />.</param>
        /// <exception cref="IO.IOException">An I/O error occurred.</exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="format" /> or <paramref name="arg" /> is null.
        /// </exception>
        /// <exception cref="FormatException">
        /// The format specification in <paramref name="format" /> is invalid.
        /// </exception>
        public static void Write(string format, params object[] arg) { }
        /// <summary>
        /// Writes the text representation of the specified 32-bit unsigned integer value to the standard
        /// output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="IO.IOException">An I/O error occurred.</exception>
        [System.CLSCompliantAttribute(false)]
        public static void Write(uint value) { }
        /// <summary>
        /// Writes the text representation of the specified 64-bit unsigned integer value to the standard
        /// output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="IO.IOException">An I/O error occurred.</exception>
        [System.CLSCompliantAttribute(false)]
        public static void Write(ulong value) { }
        /// <summary>
        /// Writes the current line terminator to the standard output stream.
        /// </summary>
        /// <exception cref="IO.IOException">An I/O error occurred.</exception>
        public static void WriteLine() { }
        /// <summary>
        /// Writes the text representation of the specified Boolean value, followed by the current line
        /// terminator, to the standard output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="IO.IOException">An I/O error occurred.</exception>
        public static void WriteLine(bool value) { }
        /// <summary>
        /// Writes the specified Unicode character, followed by the current line terminator, value to the
        /// standard output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="IO.IOException">An I/O error occurred.</exception>
        public static void WriteLine(char value) { }
        /// <summary>
        /// Writes the specified array of Unicode characters, followed by the current line terminator,
        /// to the standard output stream.
        /// </summary>
        /// <param name="buffer">A Unicode character array.</param>
        /// <exception cref="IO.IOException">An I/O error occurred.</exception>
        public static void WriteLine(char[] buffer) { }
        /// <summary>
        /// Writes the specified subarray of Unicode characters, followed by the current line terminator,
        /// to the standard output stream.
        /// </summary>
        /// <param name="buffer">An array of Unicode characters.</param>
        /// <param name="index">The starting position in <paramref name="buffer" />.</param>
        /// <param name="count">The number of characters to write.</param>
        /// <exception cref="ArgumentNullException"><paramref name="buffer" /> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> or <paramref name="count" /> is less than zero.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="index" /> plus <paramref name="count" /> specify a position that is not within
        /// <paramref name="buffer" />.
        /// </exception>
        /// <exception cref="IO.IOException">An I/O error occurred.</exception>
        public static void WriteLine(char[] buffer, int index, int count) { }
        /// <summary>
        /// Writes the text representation of the specified <see cref="Decimal" /> value, followed
        /// by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="IO.IOException">An I/O error occurred.</exception>
        public static void WriteLine(decimal value) { }
        /// <summary>
        /// Writes the text representation of the specified double-precision floating-point value, followed
        /// by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="IO.IOException">An I/O error occurred.</exception>
        public static void WriteLine(double value) { }
        /// <summary>
        /// Writes the text representation of the specified 32-bit signed integer value, followed by the
        /// current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="IO.IOException">An I/O error occurred.</exception>
        public static void WriteLine(int value) { }
        /// <summary>
        /// Writes the text representation of the specified 64-bit signed integer value, followed by the
        /// current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="IO.IOException">An I/O error occurred.</exception>
        public static void WriteLine(long value) { }
        /// <summary>
        /// Writes the text representation of the specified object, followed by the current line terminator,
        /// to the standard output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="IO.IOException">An I/O error occurred.</exception>
        public static void WriteLine(object value) { }
        /// <summary>
        /// Writes the text representation of the specified single-precision floating-point value, followed
        /// by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="IO.IOException">An I/O error occurred.</exception>
        public static void WriteLine(float value) { }
        /// <summary>
        /// Writes the specified string value, followed by the current line terminator, to the standard
        /// output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="IO.IOException">An I/O error occurred.</exception>
        public static void WriteLine(string value) { }
        /// <summary>
        /// Writes the text representation of the specified object, followed by the current line terminator,
        /// to the standard output stream using the specified format information.
        /// </summary>
        /// <param name="format">A composite format string (see Remarks).</param>
        /// <param name="arg0">An object to write using <paramref name="format" />.</param>
        /// <exception cref="IO.IOException">An I/O error occurred.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="format" /> is null.</exception>
        /// <exception cref="FormatException">
        /// The format specification in <paramref name="format" /> is invalid.
        /// </exception>
        public static void WriteLine(string format, object arg0) { }
        /// <summary>
        /// Writes the text representation of the specified objects, followed by the current line terminator,
        /// to the standard output stream using the specified format information.
        /// </summary>
        /// <param name="format">A composite format string (see Remarks).</param>
        /// <param name="arg0">The first object to write using <paramref name="format" />.</param>
        /// <param name="arg1">The second object to write using <paramref name="format" />.</param>
        /// <exception cref="IO.IOException">An I/O error occurred.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="format" /> is null.</exception>
        /// <exception cref="FormatException">
        /// The format specification in <paramref name="format" /> is invalid.
        /// </exception>
        public static void WriteLine(string format, object arg0, object arg1) { }
        /// <summary>
        /// Writes the text representation of the specified objects, followed by the current line terminator,
        /// to the standard output stream using the specified format information.
        /// </summary>
        /// <param name="format">A composite format string (see Remarks).</param>
        /// <param name="arg0">The first object to write using <paramref name="format" />.</param>
        /// <param name="arg1">The second object to write using <paramref name="format" />.</param>
        /// <param name="arg2">The third object to write using <paramref name="format" />.</param>
        /// <exception cref="IO.IOException">An I/O error occurred.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="format" /> is null.</exception>
        /// <exception cref="FormatException">
        /// The format specification in <paramref name="format" /> is invalid.
        /// </exception>
        public static void WriteLine(string format, object arg0, object arg1, object arg2) { }
        /// <summary>
        /// Writes the text representation of the specified array of objects, followed by the current line
        /// terminator, to the standard output stream using the specified format information.
        /// </summary>
        /// <param name="format">A composite format string (see Remarks).</param>
        /// <param name="arg">An array of objects to write using <paramref name="format" />.</param>
        /// <exception cref="IO.IOException">An I/O error occurred.</exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="format" /> or <paramref name="arg" /> is null.
        /// </exception>
        /// <exception cref="FormatException">
        /// The format specification in <paramref name="format" /> is invalid.
        /// </exception>
        public static void WriteLine(string format, params object[] arg) { }
        /// <summary>
        /// Writes the text representation of the specified 32-bit unsigned integer value, followed by
        /// the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="IO.IOException">An I/O error occurred.</exception>
        [System.CLSCompliantAttribute(false)]
        public static void WriteLine(uint value) { }
        /// <summary>
        /// Writes the text representation of the specified 64-bit unsigned integer value, followed by
        /// the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="IO.IOException">An I/O error occurred.</exception>
        [System.CLSCompliantAttribute(false)]
        public static void WriteLine(ulong value) { }
    }
    /// <summary>
    /// Provides data for the <see cref="Console.CancelKeyPress" /> event. This class cannot
    /// be inherited.
    /// </summary>
    public sealed partial class ConsoleCancelEventArgs : System.EventArgs
    {
        internal ConsoleCancelEventArgs() { }
        /// <summary>
        /// Gets or sets a value that indicates whether simultaneously pressing the
        /// <see cref="ConsoleModifiers.Control" /> modifier key and the <see cref="ConsoleKey.C" /> console key (Ctrl+C) or the Ctrl+Break
        /// keys terminates the current process. The default is false, which terminates the current process.
        /// </summary>
        /// <returns>
        /// true if the current process should resume when the event handler concludes; false if the current
        /// process should terminate. The default value is false; the current process terminates when the
        /// event handler returns. If true, the current process continues.
        /// </returns>
        public bool Cancel { get { return default(bool); } set { } }
        /// <summary>
        /// Gets the combination of modifier and console keys that interrupted the current process.
        /// </summary>
        /// <returns>
        /// One of the enumeration values that specifies the key combination that interrupted the current
        /// process. There is no default value.
        /// </returns>
        public System.ConsoleSpecialKey SpecialKey { get { return default(System.ConsoleSpecialKey); } }
    }
    /// <summary>
    /// Represents the method that will handle the <see cref="Console.CancelKeyPress" />
    /// event of a <see cref="Console" />.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="ConsoleCancelEventArgs" /> object that contains the event data.</param>
    public delegate void ConsoleCancelEventHandler(object sender, System.ConsoleCancelEventArgs e);
    /// <summary>
    /// Specifies constants that define foreground and background colors for the console.
    /// </summary>
    public enum ConsoleColor
    {
        /// <summary>
        /// The color black.
        /// </summary>
        Black = 0,
        /// <summary>
        /// The color blue.
        /// </summary>
        Blue = 9,
        /// <summary>
        /// The color cyan (blue-green).
        /// </summary>
        Cyan = 11,
        /// <summary>
        /// The color dark blue.
        /// </summary>
        DarkBlue = 1,
        /// <summary>
        /// The color dark cyan (dark blue-green).
        /// </summary>
        DarkCyan = 3,
        /// <summary>
        /// The color dark gray.
        /// </summary>
        DarkGray = 8,
        /// <summary>
        /// The color dark green.
        /// </summary>
        DarkGreen = 2,
        /// <summary>
        /// The color dark magenta (dark purplish-red).
        /// </summary>
        DarkMagenta = 5,
        /// <summary>
        /// The color dark red.
        /// </summary>
        DarkRed = 4,
        /// <summary>
        /// The color dark yellow (ochre).
        /// </summary>
        DarkYellow = 6,
        /// <summary>
        /// The color gray.
        /// </summary>
        Gray = 7,
        /// <summary>
        /// The color green.
        /// </summary>
        Green = 10,
        /// <summary>
        /// The color magenta (purplish-red).
        /// </summary>
        Magenta = 13,
        /// <summary>
        /// The color red.
        /// </summary>
        Red = 12,
        /// <summary>
        /// The color white.
        /// </summary>
        White = 15,
        /// <summary>
        /// The color yellow.
        /// </summary>
        Yellow = 14,
    }
    /// <summary>
    /// Describes the console key that was pressed, including the character represented by the console
    /// key and the state of the SHIFT, ALT, and CTRL modifier keys.
    /// </summary>
    public partial struct ConsoleKeyInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleKeyInfo" /> structure using the
        /// specified character, console key, and modifier keys.
        /// </summary>
        /// <param name="keyChar">The Unicode character that corresponds to the <paramref name="key" /> parameter.</param>
        /// <param name="key">The console key that corresponds to the <paramref name="keyChar" /> parameter.</param>
        /// <param name="shift">true to indicate that a SHIFT key was pressed; otherwise, false.</param>
        /// <param name="alt">true to indicate that an ALT key was pressed; otherwise, false.</param>
        /// <param name="control">true to indicate that a CTRL key was pressed; otherwise, false.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The numeric value of the <paramref name="key" /> parameter is less than 0 or greater than
        /// 255.
        /// </exception>
        public ConsoleKeyInfo(char keyChar, ConsoleKey key, bool shift, bool alt, bool control) { }
        /// <summary>
        /// Gets the Unicode character represented by the current <see cref="ConsoleKeyInfo" />
        /// object.
        /// </summary>
        /// <returns>
        /// An object that corresponds to the console key represented by the current <see cref="ConsoleKeyInfo" />
        /// object.
        /// </returns>
        public char KeyChar { get { return default(char); } }
        /// <summary>
        /// Gets the console key represented by the current <see cref="ConsoleKeyInfo" /> object.
        /// </summary>
        /// <returns>
        /// A value that identifies the console key that was pressed.
        /// </returns>
        public ConsoleKey Key { get { return default(ConsoleKey); } }
        /// <summary>
        /// Gets a bitwise combination of <see cref="ConsoleModifiers" /> values that specifies
        /// one or more modifier keys pressed simultaneously with the console key.
        /// </summary>
        /// <returns>
        /// A bitwise combination of the enumeration values. There is no default value.
        /// </returns>
        public ConsoleModifiers Modifiers { get { return default(ConsoleModifiers); ; } }
        /// <summary>
        /// Gets a value indicating whether the specified <see cref="ConsoleKeyInfo" /> object
        /// is equal to the current <see cref="ConsoleKeyInfo" /> object.
        /// </summary>
        /// <param name="obj">An object to compare to the current <see cref="ConsoleKeyInfo" /> object.</param>
        /// <returns>
        /// true if <paramref name="obj" /> is equal to the current <see cref="ConsoleKeyInfo" />
        /// object; otherwise, false.
        /// </returns>
        public bool Equals(ConsoleKeyInfo obj) { return default(bool); }
        /// <summary>
        /// Gets a value indicating whether the specified object is equal to the current
        /// <see cref="ConsoleKeyInfo" /> object.
        /// </summary>
        /// <param name="value">An object to compare to the current <see cref="ConsoleKeyInfo" /> object.</param>
        /// <returns>
        /// true if <paramref name="value" /> is a <see cref="ConsoleKeyInfo" /> object and is
        /// equal to the current <see cref="ConsoleKeyInfo" /> object; otherwise, false.
        /// </returns>
        public override bool Equals(object value) { return default(bool); }
        /// <summary>
        /// Returns the hash code for the current <see cref="ConsoleKeyInfo" /> object.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer hash code.
        /// </returns>
        public override int GetHashCode() { return default(int); }
        /// <summary>
        /// Indicates whether the specified <see cref="ConsoleKeyInfo" /> objects are equal.
        /// </summary>
        /// <param name="a">The first object to compare.</param>
        /// <param name="b">The second object to compare.</param>
        /// <returns>
        /// true if <paramref name="a" /> is equal to <paramref name="b" />; otherwise, false.
        /// </returns>
        public static bool operator ==(ConsoleKeyInfo a, ConsoleKeyInfo b) { return default(bool); }
        /// <summary>
        /// Indicates whether the specified <see cref="ConsoleKeyInfo" /> objects are not equal.
        /// </summary>
        /// <param name="a">The first object to compare.</param>
        /// <param name="b">The second object to compare.</param>
        /// <returns>
        /// true if <paramref name="a" /> is not equal to <paramref name="b" />; otherwise, false.
        /// </returns>
        public static bool operator !=(ConsoleKeyInfo a, ConsoleKeyInfo b) { return default(bool); }
    }
    /// <summary>
    /// Specifies the standard keys on a console.
    /// </summary>
    public enum ConsoleKey
    {
        /// <summary>
        /// The BACKSPACE key.
        /// </summary>
        Backspace = 0x8,
        /// <summary>
        /// The TAB key.
        /// </summary>
        Tab = 0x9,
        /// <summary>
        /// The CLEAR key.
        /// </summary>
        Clear = 0xC,
        /// <summary>
        /// The ENTER key.
        /// </summary>
        Enter = 0xD,
        /// <summary>
        /// The PAUSE key.
        /// </summary>
        Pause = 0x13,
        /// <summary>
        /// The ESC (ESCAPE) key.
        /// </summary>
        Escape = 0x1B,
        /// <summary>
        /// The SPACEBAR key.
        /// </summary>
        Spacebar = 0x20,
        /// <summary>
        /// The PAGE UP key.
        /// </summary>
        PageUp = 0x21,
        /// <summary>
        /// The PAGE DOWN key.
        /// </summary>
        PageDown = 0x22,
        /// <summary>
        /// The END key.
        /// </summary>
        End = 0x23,
        /// <summary>
        /// The HOME key.
        /// </summary>
        Home = 0x24,
        /// <summary>
        /// The LEFT ARROW key.
        /// </summary>
        LeftArrow = 0x25,
        /// <summary>
        /// The UP ARROW key.
        /// </summary>
        UpArrow = 0x26,
        /// <summary>
        /// The RIGHT ARROW key.
        /// </summary>
        RightArrow = 0x27,
        /// <summary>
        /// The DOWN ARROW key.
        /// </summary>
        DownArrow = 0x28,
        /// <summary>
        /// The SELECT key.
        /// </summary>
        Select = 0x29,
        /// <summary>
        /// The PRINT key.
        /// </summary>
        Print = 0x2A,
        /// <summary>
        /// The EXECUTE key.
        /// </summary>
        Execute = 0x2B,
        /// <summary>
        /// The PRINT SCREEN key.
        /// </summary>
        PrintScreen = 0x2C,
        /// <summary>
        /// The INS (INSERT) key.
        /// </summary>
        Insert = 0x2D,
        /// <summary>
        /// The DEL (DELETE) key.
        /// </summary>
        Delete = 0x2E,
        /// <summary>
        /// The HELP key.
        /// </summary>
        Help = 0x2F,
        /// <summary>
        /// The 0 key.
        /// </summary>
        D0 = 0x30,  // 0 through 9
        /// <summary>
        /// The 1 key.
        /// </summary>
        D1 = 0x31,
        /// <summary>
        /// The 2 key.
        /// </summary>
        D2 = 0x32,
        /// <summary>
        /// The 3 key.
        /// </summary>
        D3 = 0x33,
        /// <summary>
        /// The 4 key.
        /// </summary>
        D4 = 0x34,
        /// <summary>
        /// The 5 key.
        /// </summary>
        D5 = 0x35,
        /// <summary>
        /// The 6 key.
        /// </summary>
        D6 = 0x36,
        /// <summary>
        /// The 7 key.
        /// </summary>
        D7 = 0x37,
        /// <summary>
        /// The 8 key.
        /// </summary>
        D8 = 0x38,
        /// <summary>
        /// The 9 key.
        /// </summary>
        D9 = 0x39,
        /// <summary>
        /// The A key.
        /// </summary>
        A = 0x41,
        /// <summary>
        /// The B key.
        /// </summary>
        B = 0x42,
        /// <summary>
        /// The C key.
        /// </summary>
        C = 0x43,
        /// <summary>
        /// The D key.
        /// </summary>
        D = 0x44,
        /// <summary>
        /// The E key.
        /// </summary>
        E = 0x45,
        /// <summary>
        /// The F key.
        /// </summary>
        F = 0x46,
        /// <summary>
        /// The G key.
        /// </summary>
        G = 0x47,
        /// <summary>
        /// The H key.
        /// </summary>
        H = 0x48,
        /// <summary>
        /// The I key.
        /// </summary>
        I = 0x49,
        /// <summary>
        /// The J key.
        /// </summary>
        J = 0x4A,
        /// <summary>
        /// The K key.
        /// </summary>
        K = 0x4B,
        /// <summary>
        /// The L key.
        /// </summary>
        L = 0x4C,
        /// <summary>
        /// The M key.
        /// </summary>
        M = 0x4D,
        /// <summary>
        /// The N key.
        /// </summary>
        N = 0x4E,
        /// <summary>
        /// The O key.
        /// </summary>
        O = 0x4F,
        /// <summary>
        /// The P key.
        /// </summary>
        P = 0x50,
        /// <summary>
        /// The Q key.
        /// </summary>
        Q = 0x51,
        /// <summary>
        /// The R key.
        /// </summary>
        R = 0x52,
        /// <summary>
        /// The S key.
        /// </summary>
        S = 0x53,
        /// <summary>
        /// The T key.
        /// </summary>
        T = 0x54,
        /// <summary>
        /// The U key.
        /// </summary>
        U = 0x55,
        /// <summary>
        /// The V key.
        /// </summary>
        V = 0x56,
        /// <summary>
        /// The W key.
        /// </summary>
        W = 0x57,
        /// <summary>
        /// The X key.
        /// </summary>
        X = 0x58,
        /// <summary>
        /// The Y key.
        /// </summary>
        Y = 0x59,
        /// <summary>
        /// The Z key.
        /// </summary>
        Z = 0x5A,
        /// <summary>
        /// The Computer Sleep key.
        /// </summary>
        Sleep = 0x5F,
        /// <summary>
        /// The 0 key on the numeric keypad.
        /// </summary>
        NumPad0 = 0x60,
        /// <summary>
        /// The 1 key on the numeric keypad.
        /// </summary>
        NumPad1 = 0x61,
        /// <summary>
        /// The 2 key on the numeric keypad.
        /// </summary>
        NumPad2 = 0x62,
        /// <summary>
        /// The 3 key on the numeric keypad.
        /// </summary>
        NumPad3 = 0x63,
        /// <summary>
        /// The 4 key on the numeric keypad.
        /// </summary>
        NumPad4 = 0x64,
        /// <summary>
        /// The 5 key on the numeric keypad.
        /// </summary>
        NumPad5 = 0x65,
        /// <summary>
        /// The 6 key on the numeric keypad.
        /// </summary>
        NumPad6 = 0x66,
        /// <summary>
        /// The 7 key on the numeric keypad.
        /// </summary>
        NumPad7 = 0x67,
        /// <summary>
        /// The 8 key on the numeric keypad.
        /// </summary>
        NumPad8 = 0x68,
        /// <summary>
        /// The 9 key on the numeric keypad.
        /// </summary>
        NumPad9 = 0x69,
        /// <summary>
        /// The Multiply key (the multiplication key on the numeric keypad).
        /// </summary>
        Multiply = 0x6A,
        /// <summary>
        /// The Add key (the addition key on the numeric keypad).
        /// </summary>
        Add = 0x6B,
        /// <summary>
        /// The Separator key.
        /// </summary>
        Separator = 0x6C,
        /// <summary>
        /// The Subtract key (the subtraction key on the numeric keypad).
        /// </summary>
        Subtract = 0x6D,
        /// <summary>
        /// The Decimal key (the decimal key on the numeric keypad).
        /// </summary>
        Decimal = 0x6E,
        /// <summary>
        /// The Divide key (the division key on the numeric keypad).
        /// </summary>
        Divide = 0x6F,
        /// <summary>
        /// The F1 key.
        /// </summary>
        F1 = 0x70,
        /// <summary>
        /// The F2 key.
        /// </summary>
        F2 = 0x71,
        /// <summary>
        /// The F3 key.
        /// </summary>
        F3 = 0x72,
        /// <summary>
        /// The F4 key.
        /// </summary>
        F4 = 0x73,
        /// <summary>
        /// The F5 key.
        /// </summary>
        F5 = 0x74,
        /// <summary>
        /// The F6 key.
        /// </summary>
        F6 = 0x75,
        /// <summary>
        /// The F7 key.
        /// </summary>
        F7 = 0x76,
        /// <summary>
        /// The F8 key.
        /// </summary>
        F8 = 0x77,
        /// <summary>
        /// The F9 key.
        /// </summary>
        F9 = 0x78,
        /// <summary>
        /// The F10 key.
        /// </summary>
        F10 = 0x79,
        /// <summary>
        /// The F11 key.
        /// </summary>
        F11 = 0x7A,
        /// <summary>
        /// The F12 key.
        /// </summary>
        F12 = 0x7B,
        /// <summary>
        /// The F13 key.
        /// </summary>
        F13 = 0x7C,
        /// <summary>
        /// The F14 key.
        /// </summary>
        F14 = 0x7D,
        /// <summary>
        /// The F15 key.
        /// </summary>
        F15 = 0x7E,
        /// <summary>
        /// The F16 key.
        /// </summary>
        F16 = 0x7F,
        /// <summary>
        /// The F17 key.
        /// </summary>
        F17 = 0x80,
        /// <summary>
        /// The F18 key.
        /// </summary>
        F18 = 0x81,
        /// <summary>
        /// The F19 key.
        /// </summary>
        F19 = 0x82,
        /// <summary>
        /// The F20 key.
        /// </summary>
        F20 = 0x83,
        /// <summary>
        /// The F21 key.
        /// </summary>
        F21 = 0x84,
        /// <summary>
        /// The F22 key.
        /// </summary>
        F22 = 0x85,
        /// <summary>
        /// The F23 key.
        /// </summary>
        F23 = 0x86,
        /// <summary>
        /// The F24 key.
        /// </summary>
        F24 = 0x87,
        /// <summary>
        /// The OEM 1 key (OEM specific).
        /// </summary>
        Oem1 = 0xBA,
        /// <summary>
        /// The OEM Plus key on any country/region keyboard (Windows 2000 or later).
        /// </summary>
        OemPlus = 0xBB,
        /// <summary>
        /// The OEM Comma key on any country/region keyboard (Windows 2000 or later).
        /// </summary>
        OemComma = 0xBC,
        /// <summary>
        /// The OEM Minus key on any country/region keyboard (Windows 2000 or later).
        /// </summary>
        OemMinus = 0xBD,
        /// <summary>
        /// The OEM Period key on any country/region keyboard (Windows 2000 or later).
        /// </summary>
        OemPeriod = 0xBE,
        /// <summary>
        /// The OEM 2 key (OEM specific).
        /// </summary>
        Oem2 = 0xBF,
        /// <summary>
        /// The OEM 3 key (OEM specific).
        /// </summary>
        Oem3 = 0xC0,
        /// <summary>
        /// The OEM 4 key (OEM specific).
        /// </summary>
        Oem4 = 0xDB,
        /// <summary>
        /// The OEM 5 (OEM specific).
        /// </summary>
        Oem5 = 0xDC,
        /// <summary>
        /// The OEM 6 key (OEM specific).
        /// </summary>
        Oem6 = 0xDD,
        /// <summary>
        /// The OEM 7 key (OEM specific).
        /// </summary>
        Oem7 = 0xDE,
        /// <summary>
        /// The OEM 8 key (OEM specific).
        /// </summary>
        Oem8 = 0xDF,
        /// <summary>
        /// The CLEAR key (OEM specific).
        /// </summary>
        OemClear = 0xFE,
    }
    /// <summary>
    /// Represents the SHIFT, ALT, and CTRL modifier keys on a keyboard.
    /// </summary>
    [Flags]
    public enum ConsoleModifiers
    {
        /// <summary>
        /// The left or right ALT modifier key.
        /// </summary>
        Alt = 1,
        /// <summary>
        /// The left or right SHIFT modifier key.
        /// </summary>
        Shift = 2,
        /// <summary>
        /// The left or right CTRL modifier key.
        /// </summary>
        Control = 4
    }
    /// <summary>
    /// Specifies combinations of modifier and console keys that can interrupt the current process.
    /// </summary>
    public enum ConsoleSpecialKey
    {
        /// <summary>
        /// The <see cref="ConsoleModifiers.Control" /> modifier key plus the BREAK console key.
        /// </summary>
        ControlBreak = 1,
        /// <summary>
        /// The <see cref="ConsoleModifiers.Control" /> modifier key plus the <see cref="ConsoleKey.C" />
        /// console key.
        /// </summary>
        ControlC = 0,
    }
}
