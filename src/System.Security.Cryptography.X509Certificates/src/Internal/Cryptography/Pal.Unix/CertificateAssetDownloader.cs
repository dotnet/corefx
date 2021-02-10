// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace Internal.Cryptography.Pal
{
    internal static class CertificateAssetDownloader
    {
        private static readonly Func<string, byte[]> s_downloadBytes = CreateDownloadBytesFunc();

        internal static X509Certificate2 DownloadCertificate(string uri, ref TimeSpan remainingDownloadTime)
        {
            byte[] data = DownloadAsset(uri, ref remainingDownloadTime);

            if (data == null)
            {
                return null;
            }

            try
            {
                return new X509Certificate2(data);
            }
            catch (CryptographicException)
            {
                return null;
            }
        }

        internal static SafeX509CrlHandle DownloadCrl(string uri, ref TimeSpan remainingDownloadTime)
        {
            byte[] data = DownloadAsset(uri, ref remainingDownloadTime);

            if (data == null)
            {
                return null;
            }

            // DER-encoded CRL seems to be the most common off of some random spot-checking, so try DER first.
            SafeX509CrlHandle handle = Interop.Crypto.DecodeX509Crl(data, data.Length);

            if (!handle.IsInvalid)
            {
                return handle;
            }

            using (SafeBioHandle bio = Interop.Crypto.CreateMemoryBio())
            {
                Interop.Crypto.CheckValidOpenSslHandle(bio);

                Interop.Crypto.BioWrite(bio, data, data.Length);

                handle = Interop.Crypto.PemReadBioX509Crl(bio);

                // DecodeX509Crl failed, so we need to clear its error.
                // If PemReadBioX509Crl failed, clear that too.
                Interop.Crypto.ErrClearError();

                if (!handle.IsInvalid)
                {
                    return handle;
                }
            }

            return null;
        }

        private static byte[] DownloadAsset(string uri, ref TimeSpan remainingDownloadTime)
        {
            if (remainingDownloadTime > TimeSpan.Zero)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                try
                {
                    return s_downloadBytes(uri);
                }
                catch { }
                finally
                {
                    // TimeSpan.Zero isn't a worrisome value on the subtraction, it only means "no limit" on the original input.
                    remainingDownloadTime -= stopwatch.Elapsed;
                }
            }

            return null;
        }

        private static Func<string, byte[]> CreateDownloadBytesFunc()
        {
            try
            {
                // Use reflection to access System.Net.Http:
                // Since System.Net.Http.dll explicitly depends on System.Security.Cryptography.X509Certificates.dll,
                // the latter can't in turn have an explicit dependency on the former.

                // Get the relevant types needed.
                Type socketsHttpHandlerType = Type.GetType("System.Net.Http.SocketsHttpHandler, System.Net.Http, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", throwOnError: false);
                Type httpClientType = Type.GetType("System.Net.Http.HttpClient, System.Net.Http, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", throwOnError: false);
                Type httpRequestMessageType = Type.GetType("System.Net.Http.HttpRequestMessage, System.Net.Http, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", throwOnError: false);
                Type httpResponseMessageType = Type.GetType("System.Net.Http.HttpResponseMessage, System.Net.Http, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", throwOnError: false);
                Type httpResponseHeadersType = Type.GetType("System.Net.Http.Headers.HttpResponseHeaders, System.Net.Http, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", throwOnError: false);
                Type httpContentType = Type.GetType("System.Net.Http.HttpContent, System.Net.Http, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", throwOnError: false);
                if (socketsHttpHandlerType == null || httpClientType == null || httpRequestMessageType == null || httpResponseMessageType == null || httpResponseHeadersType == null || httpContentType == null)
                {
                    Debug.Fail("Unable to load required type.");
                    return null;
                }

                Type taskOfHttpResponseMessageType = typeof(Task<>).MakeGenericType(httpResponseMessageType);
                Type taskAwaiterOfHttpResponseMessageType = typeof(TaskAwaiter<>).MakeGenericType(httpResponseMessageType);

                // Get the methods on those types.
                PropertyInfo pooledConnectionIdleTimeoutProp = socketsHttpHandlerType.GetProperty("PooledConnectionIdleTimeout");
                PropertyInfo allowAutoRedirectProp = socketsHttpHandlerType.GetProperty("AllowAutoRedirect");
                PropertyInfo requestUriProp = httpRequestMessageType.GetProperty("RequestUri");
                MethodInfo sendAsyncMethod = httpClientType.GetMethod("SendAsync", new Type[] { httpRequestMessageType, typeof(CancellationToken) });
                PropertyInfo responseContentProp = httpResponseMessageType.GetProperty("Content");
                PropertyInfo responseStatusCodeProp = httpResponseMessageType.GetProperty("StatusCode");
                PropertyInfo responseHeadersProp = httpResponseMessageType.GetProperty("Headers");
                PropertyInfo responseHeadersLocationProp = httpResponseHeadersType.GetProperty("Location");
                MethodInfo readAsStreamAsyncMethod = httpContentType.GetMethod("ReadAsStreamAsync", Type.EmptyTypes);
                MethodInfo getAwaiterMethod = taskOfHttpResponseMessageType.GetMethod("GetAwaiter", Type.EmptyTypes);
                MethodInfo getResultMethod = taskAwaiterOfHttpResponseMessageType.GetMethod("GetResult", Type.EmptyTypes);
                if (pooledConnectionIdleTimeoutProp == null || allowAutoRedirectProp == null || requestUriProp == null || sendAsyncMethod == null ||
                    responseContentProp == null || responseStatusCodeProp == null || responseHeadersProp == null || responseHeadersLocationProp == null || readAsStreamAsyncMethod == null ||
                    getAwaiterMethod == null || getResultMethod == null)
                {
                    Debug.Fail("Unable to load required member.");
                    return null;
                }

                // Only keep idle connections around briefly, as a compromise between resource leakage and port exhaustion.
                const int PooledConnectionIdleTimeoutSeconds = 15;
                const int MaxRedirections = 10;

                // Equivalent of:
                // var socketsHttpHandler = new SocketsHttpHandler() {
                //     PooledConnectionIdleTimeout = TimeSpan.FromSeconds(PooledConnectionIdleTimeoutSeconds),
                //     AllowAutoRedirect = false
                // };
                // var httpClient = new HttpClient(socketsHttpHandler);
                object socketsHttpHandler = Activator.CreateInstance(socketsHttpHandlerType);
                pooledConnectionIdleTimeoutProp.SetValue(socketsHttpHandler, TimeSpan.FromSeconds(PooledConnectionIdleTimeoutSeconds));
                allowAutoRedirectProp.SetValue(socketsHttpHandler, false);
                object httpClient = Activator.CreateInstance(httpClientType, new object[] { socketsHttpHandler });

                return (string uriString) =>
                {
                    CancellationToken cancellationToken = CancellationToken.None;
                    Uri uri = new Uri(uriString);

                    if (!IsAllowedScheme(uri.Scheme))
                    {
                        return null;
                    }

                    // Equivalent of:
                    // HttpRequestMessage requestMessage = new HttpRequestMessage() { RequestUri = new Uri(uri) };
                    // HttpResponseMessage responseMessage = httpClient.SendAsync(requestMessage, cancellationToken).GetAwaiter().GetResult();
                    object requestMessage = Activator.CreateInstance(httpRequestMessageType);
                    requestUriProp.SetValue(requestMessage, uri);
                    object taskOfResponseMessage = sendAsyncMethod.Invoke(httpClient, new object[] { requestMessage, cancellationToken });
                    object awaiter = getAwaiterMethod.Invoke(taskOfResponseMessage, null);
                    object responseMessage = getResultMethod.Invoke(awaiter, null);

                    int redirections = 0;
                    Uri redirectUri;
                    bool hasRedirect;
                    while (true)
                    {
                        int statusCode = (int)responseStatusCodeProp.GetValue(responseMessage);
                        object responseHeaders = responseHeadersProp.GetValue(responseMessage);
                        Uri location = (Uri)responseHeadersLocationProp.GetValue(responseHeaders);
                        redirectUri = GetUriForRedirect((Uri)requestUriProp.GetValue(requestMessage), statusCode, location, out hasRedirect);
                        if (redirectUri == null)
                        {
                            break;
                        }

                        ((IDisposable)responseMessage).Dispose();

                        redirections++;
                        if (redirections > MaxRedirections)
                        {
                            return null;
                        }

                        // Equivalent of:
                        // requestMessage = new HttpRequestMessage() { RequestUri = redirectUri };
                        // requestMessage.RequestUri = redirectUri;
                        // responseMessage = httpClient.SendAsync(requestMessage, cancellationToken).GetAwaiter().GetResult();
                        requestMessage = Activator.CreateInstance(httpRequestMessageType);
                        requestUriProp.SetValue(requestMessage, redirectUri);
                        taskOfResponseMessage = sendAsyncMethod.Invoke(httpClient, new object[] { requestMessage, cancellationToken });
                        awaiter = getAwaiterMethod.Invoke(taskOfResponseMessage, null);
                        responseMessage = getResultMethod.Invoke(awaiter, null);
                    }

                    if (hasRedirect && redirectUri == null)
                    {
                        return null;
                    }

                    // Equivalent of:
                    // using Stream responseStream = resp.Content.ReadAsStreamAsync().GetAwaiter().GetResult();
                    object content = responseContentProp.GetValue(responseMessage);
                    using (Stream responseStream = ((Task<Stream>)readAsStreamAsyncMethod.Invoke(content, null)).GetAwaiter().GetResult())
                    {
                        var result = new MemoryStream();
                        responseStream.CopyTo(result);
                        ((IDisposable)responseMessage).Dispose();
                        return result.ToArray();
                    }
                };
            }
            catch
            {
                // We shouldn't have any exceptions, but if we do, ignore them all.
                return null;
            }
        }

        private static Uri GetUriForRedirect(Uri requestUri, int statusCode, Uri location, out bool hasRedirect)
        {
            if (!IsRedirectStatusCode(statusCode))
            {
                hasRedirect = false;
                return null;
            }

            hasRedirect = true;

            if (location == null)
            {
                return null;
            }

            // Ensure the redirect location is an absolute URI.
            if (!location.IsAbsoluteUri)
            {
                location = new Uri(requestUri, location);
            }

            // Per https://tools.ietf.org/html/rfc7231#section-7.1.2, a redirect location without a
            // fragment should inherit the fragment from the original URI.
            string requestFragment = requestUri.Fragment;
            if (!string.IsNullOrEmpty(requestFragment))
            {
                string redirectFragment = location.Fragment;
                if (string.IsNullOrEmpty(redirectFragment))
                {
                    location = new UriBuilder(location) { Fragment = requestFragment }.Uri;
                }
            }

            if (!IsAllowedScheme(location.Scheme))
            {
                return null;
            }

            return location;
        }

        private static bool IsRedirectStatusCode(int statusCode)
        {
            // MultipleChoices (300), Moved (301), Found (302), SeeOther (303), TemporaryRedirect (307), PermanentRedirect (308)
            return (statusCode >= 300 && statusCode <= 303) || statusCode == 307 || statusCode == 308;
        }

        private static bool IsAllowedScheme(string scheme)
        {
            return string.Equals(scheme, "http", StringComparison.OrdinalIgnoreCase);
        }
    }
}
