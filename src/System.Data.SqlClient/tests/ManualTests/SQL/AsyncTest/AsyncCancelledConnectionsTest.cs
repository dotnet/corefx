// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace System.Data.SqlClient.ManualTesting.Tests
{
    public class AsyncCancelledConnectionsTest
    {
        private readonly ITestOutputHelper _output;
        private const int NumberOfTasks = 100;  // How many attempts to poison the connection pool we will try
        private const int NumberOfNonPoisoned = 10;  // Number of normal requests for each attempt 

        public AsyncCancelledConnectionsTest(ITestOutputHelper output)
        {
            this._output = output;
        }

        [ActiveIssue(490)] // This test seems to fail regularly in pipelines due to deadlocks. But it's still useful for local testing.
        [ConditionalFact(typeof(DataTestUtility), nameof(DataTestUtility.AreConnStringsSetup))]
        public void CancelAsyncConnections()
        {
            string connectionString = DataTestUtility.TcpConnStr;
            _watch = Stopwatch.StartNew();
            _random = new Random(4); // chosen via fair dice role.
            ParallelLoopResult results = new ParallelLoopResult();
            try
            {
                // Setup a timer so that we can see what is going on while our tasks run
                using (new Timer(TimerCallback, state: null, dueTime: TimeSpan.FromSeconds(5), period: TimeSpan.FromSeconds(5)))
                {
                    results = Parallel.For(
                        fromInclusive: 0,
                        toExclusive: NumberOfTasks,
                        (int i) => DoManyAsync(connectionString).GetAwaiter().GetResult());
                }
            }
            catch (Exception ex)
            {
                _output.WriteLine(ex.ToString());
            }
            while (!results.IsCompleted)
            {
                Thread.Sleep(50);
            }
            DisplaySummary();
            foreach (var detail in _exceptionDetails)
            {
                _output.WriteLine(detail);
            }
            Assert.Empty(_exceptionDetails);
        }

        // Display one row every 5'ish seconds
        private void TimerCallback(object state)
        {
            lock (_lockObject)
            {
                DisplaySummary();
            }
        }

        private void DisplaySummary()
        {
            int count;
            lock (_exceptionDetails)
            {
                count = _exceptionDetails.Count;
            }

            _output.WriteLine($"{_watch.Elapsed} {_continue} Started:{_start} Done:{_done} InFlight:{_inFlight} RowsRead:{_rowsRead} ResultRead:{_resultRead} PoisonedEnded:{_poisonedEnded} nonPoisonedExceptions:{_nonPoisonedExceptions} PoisonedCleanupExceptions:{_poisonCleanUpExceptions} Count:{count} Found:{_found}");
        }

        // This is the the main body that our Tasks run
        private async Task DoManyAsync(string connectionString)
        {
            Interlocked.Increment(ref _start);
            Interlocked.Increment(ref _inFlight);

            // First poison
            await DoOneAsync(connectionString, poison: true);

            for (int i = 0; i < NumberOfNonPoisoned && _continue; i++)
            {
                // now run some without poisoning
                await DoOneAsync(connectionString);
            }

            Interlocked.Decrement(ref _inFlight);
            Interlocked.Increment(ref _done);
        }

        // This will do our work, open a connection, and run a query (that returns 4 results sets)
        // if we are poisoning we will 
        //   1 - Interject some sleeps in the sql statement so that it will run long enough that we can cancel it
        //   2 - Setup a time bomb task that will cancel the command a random amount of time later
        private async Task DoOneAsync(string connectionString, bool poison = false)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < 4; i++)
                    {
                        builder.AppendLine("SELECT name FROM sys.tables");
                        if (poison && i < 3)
                        {
                            builder.AppendLine("WAITFOR DELAY '00:00:01'");
                        }
                    }

                    int rowsRead = 0;
                    int resultRead = 0;

                    try
                    {
                        await connection.OpenAsync();
                        using (var command = connection.CreateCommand())
                        {
                            Task timeBombTask = default;
                            try
                            {
                                // Setup our time bomb
                                if (poison)
                                {
                                    timeBombTask = TimeBombAsync(command);
                                }

                                command.CommandText = builder.ToString();

                                // Attempt to read all of the data
                                using (var reader = await command.ExecuteReaderAsync())
                                {
                                    try
                                    {
                                        do
                                        {
                                            resultRead++;
                                            while (await reader.ReadAsync() && _continue)
                                            {
                                                rowsRead++;
                                            }
                                        }
                                        while (await reader.NextResultAsync() && _continue);
                                    }
                                    catch when (poison)
                                    {
                                        //  This looks a little strange, we failed to read above so this should fail too
                                        //  But consider the case where this code is elsewhere (in the Dispose method of a class holding this logic)
                                        try
                                        {
                                            while (await reader.NextResultAsync())
                                            {
                                            }
                                        }
                                        catch
                                        {
                                            Interlocked.Increment(ref _poisonCleanUpExceptions);
                                        }

                                        throw;
                                    }
                                }
                            }
                            finally
                            {
                                // Make sure to clean up our time bomb
                                // It is unlikely, but the timebomb may get delayed in the Task Queue
                                // And we don't want it running after we dispose the command
                                if (timeBombTask != default)
                                {
                                    await timeBombTask;
                                }
                            }
                        }
                    }
                    finally
                    {
                        Interlocked.Add(ref _rowsRead, rowsRead);
                        Interlocked.Add(ref _resultRead, resultRead);
                        if (poison)
                        {
                            Interlocked.Increment(ref _poisonedEnded);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (!poison)
                {
                    Interlocked.Increment(ref _nonPoisonedExceptions);

                    string details = ex.ToString();
                    details = details.Substring(0, Math.Min(200, details.Length));
                    lock (_exceptionDetails)
                    {
                        _exceptionDetails.Add(details);
                    }
                }

                if (ex.Message.Contains("The MARS TDS header contained errors."))
                {
                    _continue = false;
                    if (_found == 0) // This check is not really safe we may list more than one.
                    {
                        lock (_lockObject)
                        {
                            // You will notice that poison will be likely be false here, it is the normal commands that suffer
                            // Once we have successfully poisoned the connection pool, we may start to see some other request to poison fail just like the normal requests
                            _output.WriteLine($"{poison} {DateTime.UtcNow.ToString("O")}");
                            _output.WriteLine(ex.ToString());
                        }
                    }
                    Interlocked.Increment(ref _found);
                }
            }
        }

        private async Task TimeBombAsync(SqlCommand command)
        {
            await SleepAsync(100, 3000);
            command.Cancel();
        }

        private async Task SleepAsync(int minMs, int maxMs)
        {
            int delayMs;
            lock (_random)
            {
                delayMs = _random.Next(minMs, maxMs);
            }
            await Task.Delay(delayMs);
        }

        private Stopwatch _watch;

        private int _inFlight;
        private int _start;
        private int _done;
        private int _rowsRead;
        private int _resultRead;
        private int _nonPoisonedExceptions;
        private int _poisonedEnded;
        private int _poisonCleanUpExceptions;
        private bool _continue = true;
        private int _found;
        private Random _random;
        private object _lockObject = new object();

        private HashSet<string> _exceptionDetails = new HashSet<string>();
    }
}
