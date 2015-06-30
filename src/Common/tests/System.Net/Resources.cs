// Copyright (c) Microsoft. All rights reserved.

using System;

namespace NCLTest.Common
{
    public class Resources
    {
        public const string NCLServer = "nclbvt02";
        public const string NCLProxy = "nclproxy02";

        public const string NCLWebSocketServer = "nclweb";
        public static readonly Uri WebSocketEchoService = 
            new Uri("ws://" + NCLWebSocketServer + "/NCLWebSocketServer/EchoWebSocket.ashx");
        public static readonly Uri WebSocketSubProtocolService = 
            new Uri("ws://" + NCLWebSocketServer + "/NCLWebSocketServer/SubProtocol.ashx");
        public static readonly Uri SecureWebSocketEchoService =
            new Uri("wss://" + NCLWebSocketServer + "/NCLWebSocketServer/EchoWebSocket.ashx");
        public static readonly Uri SecureWebSocketEchoServiceInvalidCert =
            new Uri("wss://" + NCLWebSocketServer + ":444/NCLWebSocketServer/EchoWebSocket.ashx");

        public const string NCLHttpWebRequest = "nclweb";
        public static readonly Uri HttpWebRequestEcho = 
            new Uri ("http://" + NCLHttpWebRequest + "/testserver/echo.ashx");
        public static readonly Uri HttpSecureWebRequestEcho = 
            new Uri ("https://" + NCLHttpWebRequest + "/testserver/echo.ashx");
        public static readonly string HttpWebRequestStatusCodeFormat = 
            "http://" + NCLHttpWebRequest + "/testserver/statuscode.ashx?statuscode={0}";
        public static readonly string HttpSecureWebRequestStatusCodeFormat = 
            "https://" + NCLHttpWebRequest + "/testserver/statuscode.ashx?statuscode={0}";
    }
}
