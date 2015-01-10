//---------------------------------------------------------------------------------
// Copyright (c), Microsoft Corporation
//---------------------------------------------------------------------------------

namespace OnYourWayHome.ServiceBus
{
    using System;
    using System.IO;
    using System.Net;
    using System.Security;
    using System.Threading;

    internal abstract class RestAsyncResult<T> : AsyncResult<T>
    {
        protected abstract Uri Uri { get; }

        protected abstract string Method { get; }

        public override void BeginInvoke(AsyncCallback callback, object state)
        {
            base.BeginInvoke(callback, state);

            ThreadPool.QueueUserWorkItem(s =>
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(this.Uri);
                request.Method = this.Method;

                if (!request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
                {
                    this.OnCreateRequest(request);
                }
                else
                {
                    this.OnSendRequest(request, null);
                }
            });
        }

        protected virtual void OnCreateRequest(HttpWebRequest request)
        {
            request.BeginGetRequestStream(this.GetRequestStreamCompleted, request);
        }

        protected virtual void OnSendRequest(HttpWebRequest request, Stream requestStream)
        {
            request.BeginGetResponse(this.GetResponseCompleted, request);
        }

        protected virtual void OnReceiveResponse(HttpWebRequest request, HttpWebResponse response)
        {
            response.Dispose();
        }

        protected override void SetCompleted(Exception exception, bool completedSyncronously)
        {
            // Silverlight WebExceptions hide the actual Http Status Code and Message, we'll
            // wrap them in a more useful HttpWebException
            WebException webException = exception as WebException;
            if (webException != null)
            {
                HttpWebResponse response = webException.Response as HttpWebResponse;
                if (response != null)
                {
                    var message = webException.Message;
                    if (response.ContentLength > 0)
                    {
                        try
                        {
                            using (var streamReader = new StreamReader(response.GetResponseStream()))
                            {
                                message = streamReader.ReadToEnd();
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }

                    string exceptionMessage = string.Format("{0} ({1}): {2}", (int)response.StatusCode, response.StatusCode, message);
                    exception = new HttpWebException(response.StatusCode, exceptionMessage, webException);
                }
            }

            //// A silverlight SecurityException when trying to access the network often indicates
            //// that a Client Access Policy is not published
            //if (exception != null && exception is SecurityException)
            //{
            //    exception = new SecurityException("A security error occurred - this may be because a Client Access Policy is not published for " + this.Uri, exception);
            //}

            base.SetCompleted(exception, completedSyncronously);
        }

        private void GetRequestStreamCompleted(IAsyncResult ar)
        {
            HttpWebRequest request;
            Stream requestStream;

            try
            {
                request = (HttpWebRequest)ar.AsyncState;
                requestStream = request.EndGetRequestStream(ar);
                this.OnSendRequest(request, requestStream);
            }
            catch (Exception exception)
            {
                this.SetCompleted(exception, false);
                return;
            }
        }

        private void GetResponseCompleted(IAsyncResult ar)
        {
            HttpWebRequest request;
            HttpWebResponse response;

            try
            {
                request = (HttpWebRequest)ar.AsyncState;
                response = (HttpWebResponse)request.EndGetResponse(ar);
                this.OnReceiveResponse(request, response);
            }
            catch (Exception exception)
            {
                this.SetCompleted(exception, false);
                return;
            }

            this.SetCompleted(null, false);
        }
    }
}
