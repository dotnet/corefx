// Licensed to the .NET Foundation under one or more agreements.
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

        protected internal override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            //do not write to diagnostic source if request is invalid or cancelled,
            //let inner handler decide what to do with it
            if (request == null || cancellationToken.IsCancellationRequested)
            {
                return base.SendAsync(request, cancellationToken);
            }

            Guid loggingRequestId = LogHttpRequest(request);
            Task<HttpResponseMessage> responseTask = base.SendAsync(request, cancellationToken);
            LogHttpResponse(responseTask, loggingRequestId);
            return responseTask;
        }

        #region private

        private static readonly DiagnosticListener s_diagnosticListener =
            new DiagnosticListener(DiagnosticsHandlerLoggingStrings.DiagnosticListenerName);

        private static Guid LogHttpRequest(HttpRequestMessage request)
        {
            return s_diagnosticListener.IsEnabled(DiagnosticsHandlerLoggingStrings.RequestWriteName) ?
                LogHttpRequestCore(request) :
                Guid.Empty;
        }

        private static void LogHttpResponse(Task<HttpResponseMessage> responseTask, Guid loggingRequestId)
        {
            if (s_diagnosticListener.IsEnabled(DiagnosticsHandlerLoggingStrings.ResponseWriteName))
            {
                ScheduleLogResponse(responseTask, loggingRequestId);
            }
        }

        private static void ScheduleLogResponse(
            Task<HttpResponseMessage> responseTask, Guid loggingRequestId)
        {
            responseTask.ContinueWith(
                t =>
                {
                    if (!t.IsCanceled)
                    {
                        LogHttpResponseCore(t.IsFaulted ? null : t.Result, t.Exception, loggingRequestId);
                    }
                },
                TaskScheduler.Default);
        }

        private static Guid LogHttpRequestCore(HttpRequestMessage request)
        {
            Guid loggingRequestId = Guid.NewGuid();
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

            return loggingRequestId;
        }

        private static void LogHttpResponseCore(HttpResponseMessage response, Exception exception,
            Guid loggingRequestId)
        {
            // An empty loggingRequestId signifies that the request was not logged, so do
            // not attempt to log response.
            if (loggingRequestId != Guid.Empty)
            {
                long timestamp = Stopwatch.GetTimestamp();

                s_diagnosticListener.Write(
                    DiagnosticsHandlerLoggingStrings.ResponseWriteName,
                    new
                    {
                        Response = response,
                        LoggingRequestId = loggingRequestId,
                        TimeStamp = timestamp,
                        Exception = exception
                    }
                );
            }
        }

        #endregion
    }
}
