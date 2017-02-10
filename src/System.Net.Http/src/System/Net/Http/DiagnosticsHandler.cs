﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http
{
    /// <summary>
    /// DiagnosticHandler notifies DiagnosticSource subscribers about outgoing Http requests
    /// </summary>
    internal sealed class DiagnosticsHandler : DelegatingHandler
    {
        /// <summary>
        /// DiagnosticHandler constructor
        /// </summary>
        /// <param name="innerHandler">Inner handler: Windows or Unix implementation of HttpMessageHandler. 
        /// Note that DiagnosticHandler is the latest in the pipeline </param>
        public DiagnosticsHandler(HttpMessageHandler innerHandler) : base(innerHandler)
        {
        }

        internal static bool IsEnabled()
        {
            return s_diagnosticListener.IsEnabled(DiagnosticsHandlerLoggingStrings.ResponseWriteName);
        }

        protected internal override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            //HttpClientHandler is responsible to call DelegatingHandler.IsEnabled() before forwarding request here.
            //This code will not be reached if ResponseWriteName is not enabled, unless consumer unsubscribes
            //from DiagnosticListener right after the check. So some requests happening right after subscription starts
            //might not be instrumented. Similarly, when consumer unsubscribes, extra requests might be instumented

            Guid loggingRequestId = Guid.NewGuid();
            if (s_diagnosticListener.IsEnabled(DiagnosticsHandlerLoggingStrings.RequestWriteName))
            {
                LogHttpRequest(request,  loggingRequestId);
            }
            Task<HttpResponseMessage> responseTask = base.SendAsync(request, cancellationToken);

            LogHttpResponse(responseTask, loggingRequestId);
            return responseTask;
        }

        #region private

        private static readonly DiagnosticListener s_diagnosticListener =
            new DiagnosticListener(DiagnosticsHandlerLoggingStrings.DiagnosticListenerName);

        private static void LogHttpRequest(HttpRequestMessage request, Guid loggingRequestId)
        {
            long timestamp = Stopwatch.GetTimestamp();

            s_diagnosticListener.Write(
                DiagnosticsHandlerLoggingStrings.RequestWriteName,
                new
                {
                    Request = request,
                    LoggingRequestId = loggingRequestId,
                    Timestamp = timestamp
                }
            );
        }

        private static void LogHttpResponse(Task<HttpResponseMessage> responseTask, Guid loggingRequestId)
        {
            responseTask.ContinueWith(
                (t, s) =>
                {
                    long timestamp = Stopwatch.GetTimestamp();

                    s_diagnosticListener.Write(
                        DiagnosticsHandlerLoggingStrings.ResponseWriteName,
                        new
                        {
                            Response = t.Status == TaskStatus.RanToCompletion ? t.Result : null,
                            LoggingRequestId = (Guid)s,
                            TimeStamp = timestamp,
                            Exception = t.Exception,
                            RequestTaskStatus = t.Status
                        }
                    );
                }, loggingRequestId, CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.Default);
        }

        #endregion
    }
}
