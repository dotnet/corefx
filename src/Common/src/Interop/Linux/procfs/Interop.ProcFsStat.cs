// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;

internal static partial class Interop
{
    internal static partial class procfs
    {
        internal const string RootPath = "/proc/";
        private const string ExeFileName = "/exe";
        private const string CmdLineFileName = "/cmdline";
        private const string StatFileName = "/stat";
        private const string MapsFileName = "/maps";
        private const string StatusFileName = "/status";
        private const string FileDescriptorDirectoryName = "/fd/";
        private const string TaskDirectoryName = "/task/";

        internal const string SelfExeFilePath = RootPath + "self" + ExeFileName;
        internal const string SelfCmdLineFilePath = RootPath + "self" + CmdLineFileName;
        internal const string ProcStatFilePath = RootPath + "stat";

        internal struct ParsedStat
        {
            // Commented out fields are available in the stat data file but
            // are currently not used.  If/when needed, they can be uncommented,
            // and the corresponding entry can be added back to StatParser, replacing
            // the MoveNext() with the appropriate ParseNext* call and assignment.

            internal int pid;
            internal string comm;
            internal char state;
            internal int ppid;
            //internal int pgrp;
            internal int session;
            //internal int tty_nr;
            //internal int tpgid;
            //internal uint flags;
            //internal ulong minflt;
            //internal ulong cminflt;
            //internal ulong majflt;
            //internal ulong cmajflt;
            internal ulong utime;
            internal ulong stime;
            //internal long cutime;
            //internal long cstime;
            //internal long priority;
            internal long nice;
            //internal long num_threads;
            //internal long itrealvalue;
            internal ulong starttime;
            internal ulong vsize;
            internal long rss;
            internal ulong rsslim;
            //internal ulong startcode;
            //internal ulong endcode;
            //internal ulong startstack;
            //internal ulong kstkesp;
            //internal ulong kstkeip;
            //internal ulong signal;
            //internal ulong blocked;
            //internal ulong sigignore;
            //internal ulong sigcatch;
            //internal ulong wchan;
            //internal ulong nswap;
            //internal ulong cnswap;
            //internal int exit_signal;
            //internal int processor;
            //internal uint rt_priority;
            //internal uint policy;
            //internal ulong delayacct_blkio_ticks;
            //internal ulong guest_time;
            //internal long cguest_time;
        }

        internal struct ParsedStatus
        {
#if DEBUG
            internal int Pid;
#endif
            internal ulong VmHWM;
            internal ulong VmRSS;
            internal ulong VmData;
            internal ulong VmSwap;
            internal ulong VmSize;
            internal ulong VmPeak;
        }

        internal struct ParsedMapsModule
        {
            internal string FileName;
            internal KeyValuePair<long, long> AddressRange;
        }

        internal static string GetExeFilePathForProcess(int pid)
        {
            return RootPath + pid.ToString(CultureInfo.InvariantCulture) + ExeFileName;
        }

        internal static string GetCmdLinePathForProcess(int pid)
        {
            return RootPath + pid.ToString(CultureInfo.InvariantCulture) + CmdLineFileName;
        }

        internal static string GetStatFilePathForProcess(int pid)
        {
            return RootPath + pid.ToString(CultureInfo.InvariantCulture) + StatFileName;
        }

        internal static string GetStatusFilePathForProcess(int pid)
        {
            return RootPath + pid.ToString(CultureInfo.InvariantCulture) + StatusFileName;
        }

        internal static string GetMapsFilePathForProcess(int pid)
        {
            return RootPath + pid.ToString(CultureInfo.InvariantCulture) + MapsFileName;
        }

        internal static string GetTaskDirectoryPathForProcess(int pid)
        {
            return RootPath + pid.ToString(CultureInfo.InvariantCulture) + TaskDirectoryName;
        }

        internal static string GetFileDescriptorDirectoryPathForProcess(int pid)
        {
            return RootPath + pid.ToString(CultureInfo.InvariantCulture) + FileDescriptorDirectoryName;
        }

        internal static IEnumerable<ParsedMapsModule> ParseMapsModules(int pid)
        {
            try
            {
                return ParseMapsModulesCore(File.ReadLines(GetMapsFilePathForProcess(pid)));
            }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }

            return Array.Empty<ParsedMapsModule>();
        }

        private static IEnumerable<ParsedMapsModule> ParseMapsModulesCore(IEnumerable<string> lines)
        {
            Debug.Assert(lines != null);

            // Parse each line from the maps file into a ParsedMapsModule result
            foreach (string line in lines)
            {
                // Use a StringParser to avoid string.Split costs
                var parser = new StringParser(line, separator: ' ', skipEmpty: true);

                // Parse the address range
                KeyValuePair<long, long> addressRange =
                    parser.ParseRaw(delegate (string s, ref int start, ref int end)
                    {
                        long startingAddress = 0, endingAddress = 0;
                        int pos = s.IndexOf('-', start, end - start);
                        if (pos > 0)
                        {
                            if (long.TryParse(s.AsSpan(start, pos), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out startingAddress))
                            {
                                long.TryParse(s.AsSpan(pos + 1, end - (pos + 1)), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out endingAddress);
                            }
                        }
                        return new KeyValuePair<long, long>(startingAddress, endingAddress);
                    });

                // Parse the permissions (we only care about entries with 'r' and 'x' set)
                if (!parser.ParseRaw(delegate (string s, ref int start, ref int end)
                {
                    bool sawRead = false, sawExec = false;
                    for (int i = start; i < end; i++)
                    {
                        if (s[i] == 'r')
                            sawRead = true;
                        else if (s[i] == 'x')
                            sawExec = true;
                    }
                    return sawRead & sawExec;
                }))
                {
                    continue;
                }

                // Skip past the offset, dev, and inode fields
                parser.MoveNext();
                parser.MoveNext();
                parser.MoveNext();

                // Parse the pathname
                if (!parser.MoveNext())
                {
                    continue;
                }
                string pathname = parser.ExtractCurrentToEnd();

                // We only get here if a we have a non-empty pathname and
                // the permissions included both readability and executability.
                // Yield the result.
                yield return new ParsedMapsModule { FileName = pathname, AddressRange = addressRange };
            }
        }

        private static string GetStatFilePathForThread(int pid, int tid)
        {
            // Perf note: Calling GetTaskDirectoryPathForProcess will allocate a string,
            // which we then use in another Concat call to produce another string.  The straightforward alternative,
            // though, since we have five input strings, is to use the string.Concat overload that takes a params array.
            // This results in allocating not only the params array but also a defensive copy inside of Concat,
            // which means allocating two five-element arrays.  This two-string approach will result not only in fewer
            // allocations, but also typically in less memory allocated, and it's a bit more maintainable.
            return GetTaskDirectoryPathForProcess(pid) + tid.ToString(CultureInfo.InvariantCulture) + StatFileName;
        }

        internal static bool TryReadStatFile(int pid, out ParsedStat result, ReusableTextReader reusableReader)
        {
            bool b = TryParseStatFile(GetStatFilePathForProcess(pid), out result, reusableReader);
            Debug.Assert(!b || result.pid == pid, "Expected process ID from stat file to match supplied pid");
            return b;
        }

        internal static bool TryReadStatFile(int pid, int tid, out ParsedStat result, ReusableTextReader reusableReader)
        {
            bool b = TryParseStatFile(GetStatFilePathForThread(pid, tid), out result, reusableReader);
            Debug.Assert(!b || result.pid == tid, "Expected thread ID from stat file to match supplied tid");
            return b;
        }

        internal static bool TryReadStatusFile(int pid, out ParsedStatus result, ReusableTextReader reusableReader)
        {
            bool b = TryParseStatusFile(GetStatusFilePathForProcess(pid), out result, reusableReader);
#if DEBUG
            Debug.Assert(!b || result.Pid == pid, "Expected process ID from status file to match supplied pid");
#endif
            return b;
        }

        internal static bool TryParseStatFile(string statFilePath, out ParsedStat result, ReusableTextReader reusableReader)
        {
            if (!TryReadFile(statFilePath, reusableReader, out string statFileContents))
            {
                // Between the time that we get an ID and the time that we try to read the associated stat
                // file(s), the process could be gone.
                result = default(ParsedStat);
                return false;
            }

            var parser = new StringParser(statFileContents, ' ');
            var results = default(ParsedStat);

            results.pid = parser.ParseNextInt32();
            results.comm = parser.MoveAndExtractNextInOuterParens();
            results.state = parser.ParseNextChar();
            results.ppid = parser.ParseNextInt32();
            parser.MoveNextOrFail(); // pgrp
            results.session = parser.ParseNextInt32();
            parser.MoveNextOrFail(); // tty_nr
            parser.MoveNextOrFail(); // tpgid
            parser.MoveNextOrFail(); // flags
            parser.MoveNextOrFail(); // majflt
            parser.MoveNextOrFail(); // cmagflt
            parser.MoveNextOrFail(); // minflt
            parser.MoveNextOrFail(); // cminflt
            results.utime = parser.ParseNextUInt64();
            results.stime = parser.ParseNextUInt64();
            parser.MoveNextOrFail(); // cutime
            parser.MoveNextOrFail(); // cstime
            parser.MoveNextOrFail(); // priority
            results.nice = parser.ParseNextInt64();
            parser.MoveNextOrFail(); // num_threads
            parser.MoveNextOrFail(); // itrealvalue
            results.starttime = parser.ParseNextUInt64();
            results.vsize = parser.ParseNextUInt64();
            results.rss = parser.ParseNextInt64();
            results.rsslim = parser.ParseNextUInt64();

            // The following lines are commented out as there's no need to parse through
            // the rest of the entry (we've gotten all of the data we need).  Should any
            // of these fields be needed in the future, uncomment all of the lines up
            // through and including the one that's needed.  For now, these are being left
            // commented to document what's available in the remainder of the entry.

            //parser.MoveNextOrFail(); // startcode
            //parser.MoveNextOrFail(); // endcode
            //parser.MoveNextOrFail(); // startstack
            //parser.MoveNextOrFail(); // kstkesp
            //parser.MoveNextOrFail(); // kstkeip
            //parser.MoveNextOrFail(); // signal
            //parser.MoveNextOrFail(); // blocked
            //parser.MoveNextOrFail(); // sigignore
            //parser.MoveNextOrFail(); // sigcatch
            //parser.MoveNextOrFail(); // wchan
            //parser.MoveNextOrFail(); // nswap
            //parser.MoveNextOrFail(); // cnswap
            //parser.MoveNextOrFail(); // exit_signal
            //parser.MoveNextOrFail(); // processor
            //parser.MoveNextOrFail(); // rt_priority
            //parser.MoveNextOrFail(); // policy
            //parser.MoveNextOrFail(); // delayacct_blkio_ticks
            //parser.MoveNextOrFail(); // guest_time
            //parser.MoveNextOrFail(); // cguest_time

            result = results;
            return true;
        }

        internal static bool TryParseStatusFile(string statusFilePath, out ParsedStatus result, ReusableTextReader reusableReader)
        {
            if (!TryReadFileAsSpan(statusFilePath, reusableReader, out ReadOnlySpan<char> statusFileContents))
            {
                // Between the time that we get an ID and the time that we try to read the associated stat
                // file(s), the process could be gone.
                result = default(ParsedStatus);
                return false;
            }

            var results = default(ParsedStatus);
            while (!statusFileContents.IsEmpty)
            {
                int startIndex = statusFileContents.IndexOf(':');
                int endIndex = statusFileContents.IndexOf('\n');

                // Reached end of file
                if (endIndex == -1)
                {
                    break;
                }

                ReadOnlySpan<char> title = statusFileContents.Slice(0, startIndex);
                ReadOnlySpan<char> value = default;
                // endIndex - startIndex - 1 --> To slice till just before '\n'
                // endIndex - startIndex - 4 --> To slice till just before ' kB\n'
                switch (title.ToString())
                {
#if DEBUG
                    case "Pid":
                        value = statusFileContents.Slice(startIndex + 1, endIndex - startIndex - 1);
                        int.TryParse(value, out results.Pid);
                        break;
#endif
                    case "VmHWM":
                        value = statusFileContents.Slice(startIndex + 1, endIndex - startIndex - 4);
                        ulong.TryParse(value, out results.VmHWM);
                        break;

                    case "VmRSS":
                        value = statusFileContents.Slice(startIndex + 1, endIndex - startIndex - 4);
                        ulong.TryParse(value, out results.VmRSS);
                        break;

                    case "VmData":
                        value = statusFileContents.Slice(startIndex + 1, endIndex - startIndex - 4);
                        ulong.TryParse(value, out results.VmData);
                        break;

                    case "VmSwap":
                        value = statusFileContents.Slice(startIndex + 1, endIndex - startIndex - 4);
                        ulong.TryParse(value, out results.VmSwap);
                        break;

                    case "VmSize":
                        value = statusFileContents.Slice(startIndex + 1, endIndex - startIndex - 4);
                        ulong.TryParse(value, out results.VmSize);
                        break;

                    case "VmPeak":
                        value = statusFileContents.Slice(startIndex + 1, endIndex - startIndex - 4);
                        ulong.TryParse(value, out results.VmPeak);
                        break;

                    case "VmStk":
                        value = statusFileContents.Slice(startIndex + 1, endIndex - startIndex - 4);
                        ulong.TryParse(value, out ulong VmStk);
                        // Add stack value to VmData to calculate memory usage
                        results.VmData += VmStk;
                        break;

                    default:
                        endIndex = statusFileContents.IndexOf('\n');
                        break;
                }
                // Slice off the parsed part and parse on the rest.
                statusFileContents = statusFileContents.Slice(endIndex + 1, statusFileContents.Length - endIndex - 1);
            }
            results.VmData *= 1024;
            results.VmPeak *= 1024;
            results.VmSize *= 1024;
            results.VmSwap *= 1024;
            results.VmRSS  *= 1024;
            results.VmHWM  *= 1024;
            result = results;
            return true;
        }

        private static bool TryReadFile(string filePath, ReusableTextReader reusableReader, out string fileContents)
        {
            try
            {
                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 1, useAsync: false))
                {
                    fileContents = reusableReader.ReadAllText(fileStream);
                    return true;
                }
            }
            catch (IOException)
            {
                fileContents = null;
                return false;
            }
        }

        private static bool TryReadFileAsSpan(string filePath, ReusableTextReader reusableReader, out ReadOnlySpan<char> fileContents)
        {
            try
            {
                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 1, useAsync: false))
                {
                    fileContents = reusableReader.ReadAllText(fileStream).AsSpan();
                    return true;
                }
            }
            catch (IOException)
            {
                fileContents = null;
                return false;
            }
        }
    }
}
