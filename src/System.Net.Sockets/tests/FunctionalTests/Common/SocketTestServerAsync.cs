using System.Collections.Generic;
using System.Net.Test.Common;
using System.Threading;

using Xunit.Abstractions;

namespace System.Net.Sockets.Tests
{   // Code taken from https://msdn.microsoft.com/en-us/library/system.net.sockets.socketasynceventargs.aspx

    // Implements the connection logic for the socket server.   
    // After accepting a connection, all data read from the client  
    // is sent back to the client. The read and echo back to the client pattern  
    // is continued until the client disconnects. 
    public class SocketTestServerAsync : SocketTestServer
    {
        private ITestOutputHelper _log;

        private int m_maxNumConnections;   // the maximum number of connections the sample is designed to handle simultaneously  
        private int m_receiveBufferSize;// buffer size to use for each socket I/O operation 
        private BufferManager m_bufferManager;  // represents a large reusable set of buffers for all socket operations 
        private const int opsToPreAlloc = 2;    // read, write (don't alloc buffer space for accepts)
        private Socket listenSocket;            // the socket used to listen for incoming connection requests 
                                        // pool of reusable SocketAsyncEventArgs objects for write, read and accept socket operations
        private SocketAsyncEventArgsPool m_readWritePool;
        private int m_totalBytesRead;           // counter of the total # bytes received by the server 
        private int m_numConnectedSockets;      // the total number of clients connected to the server 
        private Semaphore m_maxNumberAcceptedClientsSemaphore;

        private object listenSocketLock = new object();

        public SocketTestServerAsync(int numConnections, int receiveBufferSize, EndPoint localEndPoint)
        {
            _log = VerboseTestLogging.GetInstance();
            m_totalBytesRead = 0;
            m_numConnectedSockets = 0;
            m_maxNumConnections = numConnections;
            m_receiveBufferSize = receiveBufferSize;
            // allocate buffers such that the maximum number of sockets can have one outstanding read and  
            //write posted to the socket simultaneously  
            m_bufferManager = new BufferManager(receiveBufferSize * numConnections * opsToPreAlloc,
                receiveBufferSize);

            m_readWritePool = new SocketAsyncEventArgsPool(numConnections);
            m_maxNumberAcceptedClientsSemaphore = new Semaphore(numConnections, numConnections);
            Init();
            Start(localEndPoint);
        }

        protected override void Dispose(bool disposing)
        {
            _log.WriteLine(this.GetHashCode() + " Dispose (m_numConnectedSockets={0})", m_numConnectedSockets);
            if (disposing && (listenSocket != null))
            {
                lock (listenSocketLock)
                {
                    if (listenSocket != null)
                    {
                        listenSocket.Dispose();
                        listenSocket = null;
                    }
                }
            }
        }

        // Initializes the server by preallocating reusable buffers and  
        // context objects.  These objects do not need to be preallocated  
        // or reused, but it is done this way to illustrate how the API can  
        // easily be used to create reusable objects to increase server performance. 
        // 
        private void Init()
        {
            // Allocates one large byte buffer which all I/O operations use a piece of.  This gaurds  
            // against memory fragmentation
            m_bufferManager.InitBuffer();

            // preallocate pool of SocketAsyncEventArgs objects
            SocketAsyncEventArgs readWriteEventArg;

            for (int i = 0; i < m_maxNumConnections; i++)
            {
                //Pre-allocate a set of reusable SocketAsyncEventArgs
                readWriteEventArg = new SocketAsyncEventArgs();
                readWriteEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
                readWriteEventArg.UserToken = new AsyncUserToken();

                // assign a byte buffer from the buffer pool to the SocketAsyncEventArg object
                m_bufferManager.SetBuffer(readWriteEventArg);

                // add SocketAsyncEventArg to the pool
                m_readWritePool.Push(readWriteEventArg);
            }

        }

        // Starts the server such that it is listening for  
        // incoming connection requests.     
        // 
        // <param name="localEndPoint">The endpoint which the server will listening 
        // for connection requests on</param> 
        private void Start(EndPoint localEndPoint)
        {
            // create the socket which listens for incoming connections
            listenSocket = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listenSocket.Bind(localEndPoint);
            // start the server with a listen backlog of 100 connections
            listenSocket.Listen(100);

            // post accepts on the listening socket
            StartAccept(null);
        }

        // Begins an operation to accept a connection request from the client  
        // 
        // <param name="acceptEventArg">The context object to use when issuing 
        // the accept operation on the server's listening socket</param> 
        private void StartAccept(SocketAsyncEventArgs acceptEventArg)
        {
            if (acceptEventArg == null)
            {
                acceptEventArg = new SocketAsyncEventArgs();
                acceptEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptEventArg_Completed);
            }
            else
            {
                // socket must be cleared since the context object is being reused
                acceptEventArg.AcceptSocket = null;
            }

            _log.WriteLine(this.GetHashCode() + " StartAccept(m_numConnectedSockets={0})", m_numConnectedSockets);
            m_maxNumberAcceptedClientsSemaphore.WaitOne();

            if (listenSocket == null)
            {
                return;
            }

            bool willRaiseEvent = false;

            lock (listenSocketLock)
            {
                if (listenSocket != null)
                {
                    willRaiseEvent = listenSocket.AcceptAsync(acceptEventArg);
                }
                else
                {
                    return;
                }
            }

            if (!willRaiseEvent)
            {
                ProcessAccept(acceptEventArg);
            }
        }

        // This method is the callback method associated with Socket.AcceptAsync  
        // operations and is invoked when an accept operation is complete 
        // 
        private void AcceptEventArg_Completed(object sender, SocketAsyncEventArgs e)
        {
            ProcessAccept(e);
        }

        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success)
            {
                return;
            }

            Interlocked.Increment(ref m_numConnectedSockets);
            _log.WriteLine(this.GetHashCode() + " ProcessAccept(m_numConnectedSockets={0})", m_numConnectedSockets);

            // Get the socket for the accepted client connection and put it into the ReadEventArg object user token.
            SocketAsyncEventArgs readEventArgs = m_readWritePool.Pop();

            ((AsyncUserToken)readEventArgs.UserToken).Socket = e.AcceptSocket;

            // As soon as the client is connected, post a receive to the connection 
            bool willRaiseEvent = e.AcceptSocket.ReceiveAsync(readEventArgs);
            if (!willRaiseEvent)
            {
                _log.WriteLine(this.GetHashCode() + " ProcessAccept -> ProcessReceive");
                ProcessReceive(readEventArgs);
            }

            // Accept the next connection request
            StartAccept(e);
        }

        // This method is called whenever a receive or send operation is completed on a socket  
        // 
        // <param name="e">SocketAsyncEventArg associated with the completed receive operation</param>
        private void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
            // determine which type of operation just completed and call the associated handler 
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                ProcessReceive(e);
                break;
                case SocketAsyncOperation.Send:
                ProcessSend(e);
                break;
                default:
                throw new ArgumentException("The last operation completed on the socket was not a receive or send");
            }

        }

        // This method is invoked when an asynchronous receive operation completes.  
        // If the remote host closed the connection, then the socket is closed.   
        // If data was received then the data is echoed back to the client. 
        // 
        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            _log.WriteLine(
                this.GetHashCode() + " ProcessReceive(bytesTransferred={0}, SocketError={1}, m_numConnectedSockets={2})", 
                e.BytesTransferred,
                e.SocketError,
                m_numConnectedSockets);

            // check if the remote host closed the connection
            AsyncUserToken token = (AsyncUserToken)e.UserToken;
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                //increment the count of the total bytes receive by the server
                Interlocked.Add(ref m_totalBytesRead, e.BytesTransferred);
                _log.WriteLine(this.GetHashCode() + " The server has read a total of {0} bytes", m_totalBytesRead);

                //echo the data received back to the client
                e.SetBuffer(e.Offset, e.BytesTransferred);
                bool willRaiseEvent = token.Socket.SendAsync(e);
                if (!willRaiseEvent)
                {
                    ProcessSend(e);
                }

            }
            else
            {
                CloseClientSocket(e);
            }
        }

        // This method is invoked when an asynchronous send operation completes.   
        // The method issues another receive on the socket to read any additional  
        // data sent from the client 
        // 
        // <param name="e"></param>
        private void ProcessSend(SocketAsyncEventArgs e)
        {
            _log.WriteLine(
                this.GetHashCode() + " ProcessSend(SocketError={0}, m_numConnectedSockets={1})",
                e.SocketError,
                m_numConnectedSockets);

            if (e.SocketError == SocketError.Success)
            {
                // done echoing data back to the client
                AsyncUserToken token = (AsyncUserToken)e.UserToken;
                // read the next block of data send from the client 
                bool willRaiseEvent = token.Socket.ReceiveAsync(e);
                if (!willRaiseEvent)
                {
                    _log.WriteLine(this.GetHashCode() + " ProcessSend -> ProcessReceive");
                    ProcessReceive(e);
                }
            }
            else
            {
                _log.WriteLine(this.GetHashCode() + " ProcessSend -> CloseClientSocket");
                CloseClientSocket(e);
            }
        }

        private void CloseClientSocket(SocketAsyncEventArgs e)
        {
            AsyncUserToken token = e.UserToken as AsyncUserToken;

            _log.WriteLine(
                this.GetHashCode() + " CloseClientSocket(m_numConnectedSockets={0}, SocketError={1})",
                m_numConnectedSockets,
                e.SocketError);

            // close the socket associated with the client 
            try
            {
                token.Socket.Shutdown(SocketShutdown.Send);
            }
            // throws if client process has already closed 
            catch (Exception ex)
            {
                _log.WriteLine(
                    this.GetHashCode() + " CloseClientSocket Exception:{0}",
                    ex);
            }
            token.Socket.Dispose();

            // decrement the counter keeping track of the total number of clients connected to the server
            Interlocked.Decrement(ref m_numConnectedSockets);
            m_maxNumberAcceptedClientsSemaphore.Release();
            _log.WriteLine(
                this.GetHashCode() + " CloseClientSocket(m_numConnectedSockets={0})", 
                m_numConnectedSockets);

            // Free the SocketAsyncEventArg so they can be reused by another client
            m_readWritePool.Push(e);
        }
    }

    internal class AsyncUserToken
    {
        public Socket Socket { get; set; }
    }

    // Represents a collection of reusable SocketAsyncEventArgs objects.   
    internal class SocketAsyncEventArgsPool
    {
        Stack<SocketAsyncEventArgs> m_pool;

        // Initializes the object pool to the specified size 
        // 
        // The "capacity" parameter is the maximum number of 
        // SocketAsyncEventArgs objects the pool can hold 
        public SocketAsyncEventArgsPool(int capacity)
        {
            m_pool = new Stack<SocketAsyncEventArgs>(capacity);
        }

        // Add a SocketAsyncEventArg instance to the pool 
        // 
        //The "item" parameter is the SocketAsyncEventArgs instance 
        // to add to the pool 
        public void Push(SocketAsyncEventArgs item)
        {
            if (item == null) { throw new ArgumentNullException("Items added to a SocketAsyncEventArgsPool cannot be null"); }
            lock (m_pool)
            {
                m_pool.Push(item);
            }
        }

        // Removes a SocketAsyncEventArgs instance from the pool 
        // and returns the object removed from the pool 
        public SocketAsyncEventArgs Pop()
        {
            lock (m_pool)
            {
                return m_pool.Pop();
            }
        }

        // The number of SocketAsyncEventArgs instances in the pool 
        public int Count
        {
            get { return m_pool.Count; }
        }

    }

    // This class creates a single large buffer which can be divided up  
    // and assigned to SocketAsyncEventArgs objects for use with each  
    // socket I/O operation.   
    // This enables bufffers to be easily reused and guards against  
    // fragmenting heap memory. 
    //  
    // The operations exposed on the BufferManager class are not thread safe. 
    internal class BufferManager
    {
        int m_numBytes;                 // the total number of bytes controlled by the buffer pool 
        byte[] m_buffer;                // the underlying byte array maintained by the Buffer Manager
        Stack<int> m_freeIndexPool;     //  
        int m_currentIndex;
        int m_bufferSize;

        public BufferManager(int totalBytes, int bufferSize)
        {
            m_numBytes = totalBytes;
            m_currentIndex = 0;
            m_bufferSize = bufferSize;
            m_freeIndexPool = new Stack<int>();
        }

        // Allocates buffer space used by the buffer pool 
        public void InitBuffer()
        {
            // create one big large buffer and divide that  
            // out to each SocketAsyncEventArg object
            m_buffer = new byte[m_numBytes];
        }

        // Assigns a buffer from the buffer pool to the  
        // specified SocketAsyncEventArgs object 
        // 
        // <returns>true if the buffer was successfully set, else false</returns> 
        public bool SetBuffer(SocketAsyncEventArgs args)
        {

            if (m_freeIndexPool.Count > 0)
            {
                args.SetBuffer(m_buffer, m_freeIndexPool.Pop(), m_bufferSize);
            }
            else
            {
                if ((m_numBytes - m_bufferSize) < m_currentIndex)
                {
                    return false;
                }
                args.SetBuffer(m_buffer, m_currentIndex, m_bufferSize);
                m_currentIndex += m_bufferSize;
            }
            return true;
        }

        // Removes the buffer from a SocketAsyncEventArg object.   
        // This frees the buffer back to the buffer pool 
        public void FreeBuffer(SocketAsyncEventArgs args)
        {
            m_freeIndexPool.Push(args.Offset);
            args.SetBuffer(null, 0, 0);
        }

    }
}
