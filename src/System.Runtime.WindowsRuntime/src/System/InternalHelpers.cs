// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace System
{
    internal static class InternalHelpers
    {
        internal static void SetErrorCode(this Exception ex, int code)
        {
            ex.HResult = code;
        }
    }
}

namespace Internal.Threading.Tasks
{

    //
    // An internal contract that exposes just enough async debugger support needed by the AsTask() extension methods in the WindowsRuntimeSystemExtensions class.
    //
    internal static class AsyncCausalitySupport
    {
        private static Action<Task> _addToActiveTasks;
        private static Action<Task> _removeFromActiveTasks;
        private static Func<bool> _loggingOn;
        private static Action<Task, string> _traceOperationCreation;
        private static Action<Task> _traceOperationCompletedSuccess;
        private static Action<Task> _traceOperationCompletedError;

// Initialize all static fields in 'AsyncCausalitySupport' when those fields are declared and remove the explicit static constructor
#pragma warning disable CA1810
        static AsyncCausalitySupport()
        {
            Type privateType = typeof(object).Assembly.GetType("Internal.Threading.Tasks.AsyncCausalitySupport");
            _addToActiveTasks = (Action<Task>)privateType.GetMethod(nameof(AddToActiveTasks), BindingFlags.Static | BindingFlags.Public).CreateDelegate(typeof(Action<Task>));
            _removeFromActiveTasks = (Action<Task>)privateType.GetMethod(nameof(RemoveFromActiveTasks), BindingFlags.Static | BindingFlags.Public).CreateDelegate(typeof(Action<Task>));
            _loggingOn = (Func<bool>)privateType.GetProperty(nameof(LoggingOn), BindingFlags.Static | BindingFlags.Public).GetMethod.CreateDelegate(typeof(Func<bool>));
            _traceOperationCreation = (Action<Task, string>)privateType.GetMethod(nameof(TraceOperationCreation), BindingFlags.Static | BindingFlags.Public).CreateDelegate(typeof(Action<Task, string>));
            _traceOperationCompletedSuccess = (Action<Task>)privateType.GetMethod(nameof(TraceOperationCompletedSuccess), BindingFlags.Static | BindingFlags.Public).CreateDelegate(typeof(Action<Task>));
            _traceOperationCompletedError = (Action<Task>)privateType.GetMethod(nameof(TraceOperationCompletedError), BindingFlags.Static | BindingFlags.Public).CreateDelegate(typeof(Action<Task>));
        }
#pragma warning restore CA1810

        public static void AddToActiveTasks(Task task)
        {
            _addToActiveTasks(task);
        }

        public static void RemoveFromActiveTasks(Task task)
        {
            _removeFromActiveTasks(task);
        }

        public static bool LoggingOn => _loggingOn();

        public static void TraceOperationCreation(Task task, string operationName)
        {
            _traceOperationCreation(task, operationName);
        }

        public static void TraceOperationCompletedSuccess(Task task)
        {
            _traceOperationCompletedSuccess(task);
        }

        public static void TraceOperationCompletedError(Task task)
        {
            _traceOperationCompletedError(task);
        }
    }
}

#nullable enable

// TODO: Remove once the shared partition is also updated and use the definition in the shared partition.
namespace Internal.Resources
{
    internal interface IWindowsRuntimeResourceManager
    {
        bool Initialize(string libpath, string reswFilename, out string? packageSimpleName, out string? encodedResWFilename);

        string GetString(string stringName, string? startingCulture, string? neutralResourcesCulture);

        System.Globalization.CultureInfo? GlobalResourceContextBestFitCultureInfo
        {
            get;
        }

        bool SetGlobalResourceContextDefaultCulture(System.Globalization.CultureInfo ci);
    }
}
