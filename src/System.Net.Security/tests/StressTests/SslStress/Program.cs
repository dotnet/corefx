// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.CommandLine;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace SslStress
{
    public static class Program
    {
        public enum ExitCode { Success = 0, StressError = 1, CliError = 2 };

        public static async Task<int> Main(string[] args)
        {
            if (!TryParseCli(args, out Configuration? config))
            {
                return (int)ExitCode.CliError;
            }

            return (int)await Run(config);
        }

        private static async Task<ExitCode> Run(Configuration config)
        {
            if ((config.RunMode & RunMode.both) == 0)
            {
                Console.Error.WriteLine("Must specify a valid run mode");
                return ExitCode.CliError;
            }

            static string GetAssemblyInfo(Assembly assembly) => $"{assembly.Location}, modified {new FileInfo(assembly.Location).LastWriteTime}";

            Console.WriteLine("           .NET Core: " + GetAssemblyInfo(typeof(object).Assembly));
            Console.WriteLine(" System.Net.Security: " + GetAssemblyInfo(typeof(System.Net.Security.SslStream).Assembly));
            Console.WriteLine("     Server Endpoint: " + config.ServerEndpoint);
            Console.WriteLine("         Concurrency: " + config.MaxConnections);
            Console.WriteLine("  Max Execution Time: " + ((config.MaxExecutionTime != null) ? config.MaxExecutionTime.Value.ToString() : "infinite"));
            Console.WriteLine("  Max Conn. Lifetime: " + config.ConnectionLifetime);
            Console.WriteLine("         Random Seed: " + config.RandomSeed);
            Console.WriteLine();

            StressServer? server = null;
            if (config.RunMode.HasFlag(RunMode.server))
            {
                // Start the SSL web server in-proc.
                Console.WriteLine($"Starting SSL server.");
                server = new StressServer(config);
                server.Start();

                Console.WriteLine($"Server listening to {server.ServerEndpoint}");
            }

            StressClient? client = null;
            if (config.RunMode.HasFlag(RunMode.client))
            {
                // Start the client.
                Console.WriteLine($"Starting {config.MaxConnections} client workers.");
                Console.WriteLine();

                client = new StressClient(config);
                client.Start();
            }

            await WaitUntilMaxExecutionTimeElapsedOrKeyboardInterrupt(config.MaxExecutionTime);

            try
            {
                if (client != null) await client.StopAsync();
                if (server != null) await server.StopAsync();
            }
            finally
            {
                client?.PrintFinalReport();
            }

            return client?.TotalErrorCount == 0 ? ExitCode.Success : ExitCode.StressError;

            static async Task WaitUntilMaxExecutionTimeElapsedOrKeyboardInterrupt(TimeSpan? maxExecutionTime = null)
            {
                var tcs = new TaskCompletionSource<bool>();
                Console.CancelKeyPress += (sender, args) => { Console.Error.WriteLine("Keyboard interrupt"); args.Cancel = true; tcs.TrySetResult(false); };
                if (maxExecutionTime.HasValue)
                {
                    Console.WriteLine($"Running for a total of {maxExecutionTime.Value.TotalMinutes:0.##} minutes");
                    var cts = new System.Threading.CancellationTokenSource(delay: maxExecutionTime.Value);
                    cts.Token.Register(() => { Console.WriteLine("Max execution time elapsed"); tcs.TrySetResult(false); });
                }

                await tcs.Task;
            }
        }

        private static bool TryParseCli(string[] args, [NotNullWhen(true)] out Configuration? config)
        {
            var cmd = new RootCommand();
            cmd.AddOption(new Option(new[] { "--help", "-h" }, "Display this help text."));
            cmd.AddOption(new Option(new[] { "--mode", "-m" }, "Stress suite execution mode. Defaults to Both.") { Argument = new Argument<RunMode>("runMode", RunMode.both) });
            cmd.AddOption(new Option(new[] { "--num-connections", "-n" }, "Max number of connections to open concurrently.") { Argument = new Argument<int>("connections", Math.Max(Environment.ProcessorCount / 2, 2)) });
            cmd.AddOption(new Option(new[] { "--server-endpoint", "-e" }, "Endpoint to bind to if server, endpoint to listen to if client.") { Argument = new Argument<string>("ipEndpoint", "127.0.0.1:5002") });
            cmd.AddOption(new Option(new[] { "--max-execution-time", "-t" }, "Maximum stress execution time, in minutes. Defaults to infinity.") { Argument = new Argument<double?>("minutes", null) });
            cmd.AddOption(new Option(new[] { "--max-buffer-length", "-l" }, "Maximum buffer length to write on ssl stream. Defaults to 8192.") { Argument = new Argument<int>("bytes", 8192) });
            cmd.AddOption(new Option(new[] { "--connection-lifetime", "-L" }, "Maximum stress execution time, in seconds. Defaults to 120 seconds.") { Argument = new Argument<double>("minutes", 120) });
            cmd.AddOption(new Option(new[] { "--display-interval", "-i" }, "Client stats display interval, in seconds. Defaults to 5 seconds.") { Argument = new Argument<double>("seconds", 5) });
            cmd.AddOption(new Option(new[] { "--seed", "-s" }, "Seed for generating pseudo-random parameters. Also depends on the -n argument.") { Argument = new Argument<int>("seed", (new Random().Next())) });

            ParseResult parseResult = cmd.Parse(args);
            if (parseResult.Errors.Count > 0 || parseResult.HasOption("-h"))
            {
                foreach (ParseError error in parseResult.Errors)
                {
                    Console.WriteLine(error);
                }
                Console.WriteLine();
                new HelpBuilder(new SystemConsole()).Write(cmd);
                config = null;
                return false;
            }

            config = new Configuration()
            {
                RunMode = parseResult.ValueForOption<RunMode>("-m"),
                MaxConnections = parseResult.ValueForOption<int>("-n"),
                ServerEndpoint = IPEndPoint.Parse(parseResult.ValueForOption<string>("-e")),
                MaxExecutionTime = parseResult.ValueForOption<double?>("-M").Select(TimeSpan.FromMinutes),
                MaxBufferLength = parseResult.ValueForOption<int>("-l"),
                ConnectionLifetime = TimeSpan.FromSeconds(parseResult.ValueForOption<double>("-L")),
                DisplayInterval = TimeSpan.FromSeconds(parseResult.ValueForOption<double>("-i")),
                RandomSeed = parseResult.ValueForOption<int>("-s"),
            };

            return true;
        }

        private static S? Select<T, S>(this T? value, Func<T, S> mapper) where T : struct where S : struct
        {
            return value is null ? null : new S?(mapper(value.Value));
        }
    }
}
