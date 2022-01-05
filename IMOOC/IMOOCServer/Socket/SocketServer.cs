using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace IMOOCServer
{
    public class SocketServer
    {
        private int m_numConnections;   // the maximum number of connections the sample is designed to handle simultaneously 
        private int m_receiveBufferSize;// buffer size to use for each socket I/O operation 
        BufferManager m_bufferManager;  // represents a large reusable set of buffers for all socket operations
        const int opsToPreAlloc = 2;    // read, write (don't alloc buffer space for accepts)
        Socket listenSocket;            // the socket used to listen for incoming connection requests
                                        // pool of reusable SocketAsyncEventArgs objects for write, read and accept socket operations
        SocketAsyncEventArgsPool m_readWritePool;
        List<SocketAsyncEventArgs> teacherSocketList;
        List<SocketAsyncEventArgs> studentSocketList;
        PacketQueue m_packets;
        bool firstReceive;
        int m_totalBytesRead;           // counter of the total # bytes received by the server
        int m_numConnectedSockets;      // the total number of clients connected to the server 
        Semaphore m_maxNumberAcceptedClients;
        public event EventHandler<TeacherNumChangedEventArgs> TeacherNumChanged;
        public event EventHandler<StudentNumChangedEventArgs> StudentNumChanged;

        // Create an uninitialized server instance.  
        // To start the server listening for connection requests
        // call the Init method followed by Start method 
        //
        // <param name="numConnections">the maximum number of connections the sample is designed to handle simultaneously</param>
        // <param name="receiveBufferSize">buffer size to use for each socket I/O operation</param>
        public SocketServer(int numConnections, int receiveBufferSize)
        {
            m_totalBytesRead = 0;
            m_numConnectedSockets = 0;
            m_numConnections = numConnections;
            m_receiveBufferSize = receiveBufferSize;
            // allocate buffers such that the maximum number of sockets can have one outstanding read and 
            //write posted to the socket simultaneously  
            m_bufferManager = new BufferManager(receiveBufferSize * numConnections * opsToPreAlloc,
                receiveBufferSize);

            m_readWritePool = new SocketAsyncEventArgsPool(numConnections);
            m_maxNumberAcceptedClients = new Semaphore(numConnections, numConnections);

        }

        // Initializes the server by preallocating reusable buffers and 
        // context objects.  These objects do not need to be preallocated 
        // or reused, but it is done this way to illustrate how the API can 
        // easily be used to create reusable objects to increase server performance.
        //
        public void Init()
        {
            // Allocates one large byte buffer which all I/O operations use a piece of.  This gaurds 
            // against memory fragmentation
            m_bufferManager.InitBuffer();

            // preallocate pool of SocketAsyncEventArgs objects
            SocketAsyncEventArgs readWriteEventArg;

            for (int i = 0; i < m_numConnections; i++)
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

            teacherSocketList = new List<SocketAsyncEventArgs>();
            studentSocketList = new List<SocketAsyncEventArgs>();
            m_packets = new PacketQueue();
            firstReceive = true;
        }

        // Starts the server such that it is listening for 
        // incoming connection requests.    
        //
        // <param name="localEndPoint">The endpoint which the server will listening 
        // for connection requests on</param>
        public void Start(IPEndPoint localEndPoint)
        {
            // create the socket which listens for incoming connections
            listenSocket = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listenSocket.Bind(localEndPoint);
            // start the server with a listen backlog of 10 connections
            listenSocket.Listen(10);

            // post accepts on the listening socket
            StartAccept(null);

            //Console.WriteLine("{0} connected sockets with one outstanding receive posted to each....press any key", m_outstandingReadCount);
            //Console.WriteLine("Press any key to terminate the server process....");
            //Console.ReadKey();
        }


        // Begins an operation to accept a connection request from the client 
        //
        // <param name="acceptEventArg">The context object to use when issuing 
        // the accept operation on the server's listening socket</param>
        public void StartAccept(SocketAsyncEventArgs acceptEventArg)
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

            m_maxNumberAcceptedClients.WaitOne();
            bool willRaiseEvent = listenSocket.AcceptAsync(acceptEventArg);
            if (!willRaiseEvent)
            {
                ProcessAccept(acceptEventArg);
            }
        }

        // This method is the callback method associated with Socket.AcceptAsync 
        // operations and is invoked when an accept operation is complete
        //
        void AcceptEventArg_Completed(object sender, SocketAsyncEventArgs e)
        {
            ProcessAccept(e);
        }

        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            Interlocked.Increment(ref m_numConnectedSockets);
            //Console.WriteLine("Client connection accepted. There are {0} clients connected to the server",
            //    m_numConnectedSockets);

            // Get the socket for the accepted client connection and put it into the 
            //ReadEventArg object user token
            SocketAsyncEventArgs readEventArgs = m_readWritePool.Pop();
            ((AsyncUserToken)readEventArgs.UserToken).Socket = e.AcceptSocket;

            // As soon as the client is connected, post a receive to the connection
            bool willRaiseEvent = e.AcceptSocket.ReceiveAsync(readEventArgs);
            if (!willRaiseEvent)
            {
                ProcessReceive(readEventArgs);
            }

            //Accept the next connection request
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
            // check if the remote host closed the connection
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                //increment the count of the total bytes receive by the server
                Interlocked.Add(ref m_totalBytesRead, e.BytesTransferred);
                //Console.WriteLine("The server has read a total of {0} bytes", m_totalBytesRead);


                //clerify the client's identity
                var data = Encoding.UTF8.GetString(e.Buffer, e.Offset, e.BytesTransferred);
                if (data == "teacher")
                {
                    teacherSocketList.Add(e);
                    OnTeacherNumChanged(new TeacherNumChangedEventArgs(1));
                    if (!((AsyncUserToken)e.UserToken).Socket.ReceiveAsync(e))
                    {
                        ProcessReceive(e);
                    }
                }
                else if (data == "student")
                {
                    studentSocketList.Add(e);
                    OnStudentNumChanged(new StudentNumChangedEventArgs(1));
                    ((AsyncUserToken)e.UserToken).Socket.NoDelay = true;
                    ((AsyncUserToken)e.UserToken).IsSend = false;
                }
                else
                {
                    m_packets.SetPacket(e.Buffer, e.Offset, e.BytesTransferred);
                    if (firstReceive)
                    {
                        firstReceive = false;
                        StudentArgsSend();
                    }
                    if (!((AsyncUserToken)e.UserToken).Socket.ReceiveAsync(e))
                    {
                        ProcessReceive(e);
                    }
                }

            }
            else
            {
                CloseClientSocket(e);
            }
        }

        private bool StudentListIsSend()
        {
            for (int i = 0; i < studentSocketList.Count; i++)
            {
                if (((AsyncUserToken)studentSocketList[i].UserToken).IsSend)
                {
                    return true;
                }
            }
            return false;
        }

        // This method is invoked when an asynchronous send operation completes.  
        // The method issues another receive on the socket to read any additional 
        // data sent from the client
        //
        // <param name="e"></param>
        private void ProcessSend(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                // done echoing data back to the client
                AsyncUserToken token = (AsyncUserToken)e.UserToken;
                //read the next block of data send from the client
                //bool willRaiseEvent = token.Socket.ReceiveAsync(e);
                //if (!willRaiseEvent)
                //{
                //    ProcessReceive(e);
                //}
                token.IsSend = false;
                if (!StudentListIsSend())
                {
                    StudentArgsSend();
                }
            }
            else
            {
                CloseClientSocket(e);
            }
        }

        private void StudentArgsSend()
        {
            if (m_packets.GetPacket(studentSocketList))
            {
                for (int i = 0; i < studentSocketList.Count; i++)
                {
                    var token = (AsyncUserToken)studentSocketList[i].UserToken;
                    if (!(token.Socket.SendAsync(studentSocketList[i])))
                    {
                        ProcessSend(studentSocketList[i]);
                    }
                }
            }
            else
            {
                firstReceive = true;
            }
        }

        private void CloseClientSocket(SocketAsyncEventArgs e)
        {
            AsyncUserToken token = e.UserToken as AsyncUserToken;

            // close the socket associated with the client
            try
            {
                token.Socket.Shutdown(SocketShutdown.Send);
            }
            // throws if client process has already closed
            catch (Exception) { }
            token.Socket.Close();

            // decrement the counter keeping track of the total number of clients connected to the server
            Interlocked.Decrement(ref m_numConnectedSockets);
            m_maxNumberAcceptedClients.Release();

            if (teacherSocketList.Remove(e))
            {
                OnTeacherNumChanged(new TeacherNumChangedEventArgs(-1));
            }
            if (studentSocketList.Remove(e))
            {
                OnStudentNumChanged(new StudentNumChangedEventArgs(-1));
            }

            //Console.WriteLine("A client has been disconnected from the server.
            //There are {0} clients connected to the server", m_numConnectedSockets);

            // Free the SocketAsyncEventArg so they can be reused by another client
            m_readWritePool.Push(e);
        }


        // Wrap event invocations inside a protected virtual method
        // to allow derived classes to override the event invocation behavior
        protected virtual void OnTeacherNumChanged(TeacherNumChangedEventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            EventHandler<TeacherNumChangedEventArgs> handler = TeacherNumChanged;

            // Event will be null if there are no subscribers
            if (handler != null)
            {
                // Use the () operator to raise the event.
                handler(this, e);
            }
        }

        protected virtual void OnStudentNumChanged(StudentNumChangedEventArgs e)
        {
            EventHandler<StudentNumChangedEventArgs> handler = StudentNumChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

    }

    public class TeacherNumChangedEventArgs : EventArgs
    {
        public int TeacherNumChanged { get; private set; }

        public TeacherNumChangedEventArgs(int teacherChange)
        {
            TeacherNumChanged = teacherChange;

        }
    }

    public class StudentNumChangedEventArgs : EventArgs
    {
        public int StudentNumChanged { get; private set; }

        public StudentNumChangedEventArgs(int studentChange)
        {
            StudentNumChanged = studentChange;
        }
    }

    public class AsyncUserToken
    {
        public Socket Socket;
        public bool IsSend;
    }
}
