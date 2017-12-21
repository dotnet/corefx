// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace System.Net.Sockets.Tests
{
    // Provides a dummy socket server that accepts connections and echos data sent.
    public class DummySocketServer : IDisposable
    {
        private Socket socket;
        
        public DummySocketServer(EndPoint endpoint)
        {
            socket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(endpoint);
            socket.Listen(5);
            socket.BeginAccept(OnAccept, null);
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                socket.Close();
            }
        }
        
        private void OnAccept(IAsyncResult result)
        {
            Socket client = null;
            try
            {
                client = socket.EndAccept(result);
            }
            catch (SocketException e)
            {
                if (e.SocketErrorCode == SocketError.OperationAborted)
                {
                    return;
                }
                throw;
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            
            ClientState state = new ClientState(client);
            try
            {
                state.Socket.BeginReceive(state.ReceiveBuffer, 0, state.ReceiveBuffer.Length, SocketFlags.None, OnReceive, state);
            }
            catch (SocketException)
            {
            }
            
            try
            {
                socket.BeginAccept(OnAccept, null);
            }
            catch (ObjectDisposedException)
            {
            }
        }
        
        private void OnReceive(IAsyncResult result)
        {
            ClientState state = (ClientState)result.AsyncState;
            try
            {
                int bytesReceived = state.Socket.EndReceive(result);
                if (bytesReceived == 0)
                {
                    state.Socket.Close();
                    return;
                }
                state.Socket.Send(state.ReceiveBuffer, 0, bytesReceived, SocketFlags.None);
                state.Socket.BeginReceive(state.ReceiveBuffer, 0, state.ReceiveBuffer.Length, SocketFlags.None, OnReceive, state);
            }
            catch (SocketException)
            {
                state.Socket.Close();
                return;
            }
        }
        
        private class ClientState
        {
            private Socket socket;
            private byte[] receiveBuffer;
            
            public ClientState(Socket socket)
            {
                this.socket = socket;
                this.receiveBuffer = new byte[1024];
            }
            
            public Socket Socket
            {
                get { return socket; }
            }
            
            public byte[] ReceiveBuffer
            {
                get { return receiveBuffer; }
            }
        }
    }
}