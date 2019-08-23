// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using HttpStress;

/// <summary>
/// Simple HttpClient stress app that launches Kestrel in-proc and runs many concurrent requests of varying types against it.
/// </summary>
public static class Program
{

    public static async Task Main(string[] args)
    {
        if (!TryParseCli(args, out Configuration config))
        {
            return;
        }

        await Run(config);
    }

    private static bool TryParseCli(string[] args, out Configuration config)
    {
        var cmd = new RootCommand();
        cmd.AddOption(new Option("-n", "Max number of requests to make concurrently.") { Argument = new Argument<int>("numWorkers", Environment.ProcessorCount) });
        cmd.AddOption(new Option("-serverUri", "Stress suite server uri.") { Argument = new Argument<Uri>("serverUri", new Uri("https://localhost:5001")) });
        cmd.AddOption(new Option("-runMode", "Stress suite execution mode. Defaults to Both.") { Argument = new Argument<RunMode>("runMode", RunMode.both) });
        cmd.AddOption(new Option("-maxContentLength", "Max content length for request and response bodies.") { Argument = new Argument<int>("numBytes", 1000) });
        cmd.AddOption(new Option("-maxRequestUriSize", "Max query string length support by the server.") { Argument = new Argument<int>("numChars", 5000) });
        cmd.AddOption(new Option("-maxRequestHeaderCount", "Maximum number of headers to place in request") { Argument = new Argument<int>("numHeaders", 90) });
        cmd.AddOption(new Option("-maxRequestHeaderTotalSize", "Max request header total size.") { Argument = new Argument<int>("numBytes", 1000) });
        cmd.AddOption(new Option("-http", "HTTP version (1.1 or 2.0)") { Argument = new Argument<Version>("version", HttpVersion.Version20) });
        cmd.AddOption(new Option("-connectionLifetime", "Max connection lifetime length (milliseconds).") { Argument = new Argument<int?>("connectionLifetime", null) });
        cmd.AddOption(new Option("-ops", "Indices of the operations to use") { Argument = new Argument<int[]>("space-delimited indices", null) });
        cmd.AddOption(new Option("-xops", "Indices of the operations to exclude") { Argument = new Argument<int[]>("space-delimited indices", null) });
        cmd.AddOption(new Option("-trace", "Enable Microsoft-System-Net-Http tracing.") { Argument = new Argument<string>("\"console\" or path") });
        cmd.AddOption(new Option("-aspnetlog", "Enable ASP.NET warning and error logging.") { Argument = new Argument<bool>("enable", false) });
        cmd.AddOption(new Option("-listOps", "List available options.") { Argument = new Argument<bool>("enable", false) });
        cmd.AddOption(new Option("-seed", "Seed for generating pseudo-random parameters for a given -n argument.") { Argument = new Argument<int?>("seed", null) });
        cmd.AddOption(new Option("-numParameters", "Max number of query parameters or form fields for a request.") { Argument = new Argument<int>("queryParameters", 1) });
        cmd.AddOption(new Option("-cancelRate", "Number between 0 and 1 indicating rate of client-side request cancellation attempts. Defaults to 0.1.") { Argument = new Argument<double>("probability", 0.1) });
        cmd.AddOption(new Option("-httpSys", "Use http.sys instead of Kestrel.") { Argument = new Argument<bool>("enable", false) });
        cmd.AddOption(new Option("-winHttp", "Use WinHttpHandler for the stress client.") { Argument = new Argument<bool>("enable", false) });
        cmd.AddOption(new Option("-displayInterval", "Client stats display interval in seconds. Defaults to 5 seconds.") { Argument = new Argument<int>("seconds", 5) });
        cmd.AddOption(new Option("-clientTimeout", "Default HttpClient timeout in seconds. Defaults to 10 seconds.") { Argument = new Argument<int>("seconds", 10) });

        ParseResult cmdline = cmd.Parse(args);
        if (cmdline.Errors.Count > 0)
        {
            foreach (ParseError error in cmdline.Errors)
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
            RunMode = cmdline.ValueForOption<RunMode>("-runMode"),
            ServerUri = cmdline.ValueForOption<Uri>("-serverUri"),
            ListOperations = cmdline.ValueForOption<bool>("-listOps"),

            HttpVersion = cmdline.ValueForOption<Version>("-http"),
            UseWinHttpHandler = cmdline.ValueForOption<bool>("-winHttp"),
            ConcurrentRequests = cmdline.ValueForOption<int>("-n"),
            RandomSeed = cmdline.ValueForOption<int?>("-seed") ?? new Random().Next(),
            MaxContentLength = cmdline.ValueForOption<int>("-maxContentLength"),
            MaxRequestUriSize = cmdline.ValueForOption<int>("-maxRequestUriSize"),
            MaxRequestHeaderCount = cmdline.ValueForOption<int>("-maxRequestHeaderCount"),
            MaxRequestHeaderTotalSize = cmdline.ValueForOption<int>("-maxRequestHeaderTotalSize"),
            OpIndices = cmdline.ValueForOption<int[]>("-ops"),
            ExcludedOpIndices = cmdline.ValueForOption<int[]>("-xops"),
            MaxParameters = cmdline.ValueForOption<int>("-numParameters"),
            DisplayInterval = TimeSpan.FromSeconds(cmdline.ValueForOption<int>("-displayInterval")),
            DefaultTimeout = TimeSpan.FromSeconds(cmdline.ValueForOption<int>("-clientTimeout")),
            ConnectionLifetime = cmdline.ValueForOption<double?>("-connectionLifetime").Select(TimeSpan.FromMilliseconds),
            CancellationProbability = Math.Max(0, Math.Min(1, cmdline.ValueForOption<double>("-cancelRate"))),

            UseHttpSys = cmdline.ValueForOption<bool>("-httpSys"),
            LogAspNet = cmdline.ValueForOption<bool>("-aspnetlog"),
            LogPath = cmdline.HasOption("-trace") ? cmdline.ValueForOption<string>("-trace") : null
        };

        return true;
    }

    private static async Task Run(Configuration config)
    {
        (string name, Func<RequestContext, Task> op)[] clientOperations =
            ClientOperations.Operations
                // annotate the operation name with its index
                .Select((op, i) => ($"{i.ToString().PadLeft(2)}: {op.name}", op.operation))
                .ToArray();

        if ((config.RunMode & RunMode.both) == 0)
        {
            Console.Error.WriteLine("Must specify a valid run mode");
            return;
        }

        if (!config.ServerUri.Scheme.StartsWith("http"))
        {
            Console.Error.WriteLine("Invalid server uri");
            return;
        }

        if (config.ListOperations)
        {
            for (int i = 0; i < clientOperations.Length; i++)
            {
                Console.WriteLine(clientOperations[i].name);
            }
            return;
        }

        // derive client operations based on arguments
        (string name, Func<RequestContext, Task> op)[] usedClientOperations = (config.OpIndices, config.ExcludedOpIndices) switch
        {
            (null, null) => clientOperations,
            (int[] incl, null) => incl.Select(i => clientOperations[i]).ToArray(),
            (_, int[] excl) =>
                Enumerable
                .Range(0, clientOperations.Length)
                .Except(excl)
                .Select(i => clientOperations[i])
                .ToArray(),
        };

        Console.WriteLine("       .NET Core: " + Path.GetFileName(Path.GetDirectoryName(typeof(object).Assembly.Location)));
        Console.WriteLine("    ASP.NET Core: " + Path.GetFileName(Path.GetDirectoryName(typeof(WebHost).Assembly.Location)));
        Console.WriteLine(" System.Net.Http: " + GetSysNetHttpAssemblyInfo());
        Console.WriteLine("          Server: " + (config.UseHttpSys ? "http.sys" : "Kestrel"));
        Console.WriteLine("      Server URL: " + config.ServerUri);
        Console.WriteLine("         Tracing: " + (config.LogPath == null ? (object)false : config.LogPath.Length == 0 ? (object)true : config.LogPath));
        Console.WriteLine("     ASP.NET Log: " + config.LogAspNet);
        Console.WriteLine("     Concurrency: " + config.ConcurrentRequests);
        Console.WriteLine("  Content Length: " + config.MaxContentLength);
        Console.WriteLine("   HTTP2 Version: " + config.HttpVersion);
        Console.WriteLine("        Lifetime: " + (config.ConnectionLifetime.HasValue ? $"{config.ConnectionLifetime.Value.TotalMilliseconds}ms" : "(infinite)"));
        Console.WriteLine("      Operations: " + string.Join(", ", usedClientOperations.Select(o => o.name)));
        Console.WriteLine("     Random Seed: " + config.RandomSeed);
        Console.WriteLine("    Cancellation: " + 100 * config.CancellationProbability + "%");
        Console.WriteLine("Max Content Size: " + config.MaxContentLength);
        Console.WriteLine("Query Parameters: " + config.MaxParameters);
        Console.WriteLine();


        StressServer server = null;
        if (config.RunMode.HasFlag(RunMode.server))
        {
            // Start the Kestrel web server in-proc.
            Console.WriteLine($"Starting {(config.UseHttpSys ? "http.sys" : "Kestrel")} server.");
            server = new StressServer(config);
            Console.WriteLine($"Server started at {server.ServerUri}");
        }

        StressClient client = null;
        if (config.RunMode.HasFlag(RunMode.client))
        {
            // Start the client.
            Console.WriteLine($"Starting {config.ConcurrentRequests} client workers.");

            client = new StressClient(usedClientOperations, config);
            client.Start();
        }

        await AwaitCancelKeyPress();

        client?.Stop();
        client?.PrintFinalReport();
    }

    private static async Task AwaitCancelKeyPress()
    {
        var tcs = new TaskCompletionSource<bool>();
        Console.CancelKeyPress += (sender,args) => { Console.Error.WriteLine("Keyboard interrupt"); args.Cancel = true; tcs.TrySetResult(false); };
        await tcs.Task;
    }

    private static S? Select<T, S>(this T? value, Func<T, S> mapper) where T : struct where S : struct
    {
        return value != null ? new S?(mapper(value.Value)) : null;
    }

    private static string GetSysNetHttpAssemblyInfo()
    {
        string location = typeof(System.Net.Http.HttpClient).Assembly.Location;
        return $"{location}, last modified {new FileInfo(location).LastWriteTime}";
    }
}
