using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace YAMS
{
    public class Wrapper
    {
        //Listens on the external port and deals with all incoming data
        public class Listen
        {
            public TcpListener Listener;
            private Thread listenThread;

            public List<Client> Clients = new List<Client>();

            public Listen(IPAddress ipListen, int intPort)
            {
                //Create our listener on the correct IP and port
                this.Listener = new TcpListener(ipListen, intPort);
                this.listenThread = new Thread(new ThreadStart(Connect));
                this.listenThread.Start();
            }

            private void Connect()
            {
                this.Listener.Start();

                while (true)
                {
                    //If there is a new connection ready, create a new client
                    if (this.Listener.Pending()) new YAMS.Client(this.Listener.AcceptSocket(), this);
                }
            }

            public void Dispose()
            {
                this.listenThread.Abort();
                this.Listener.Stop();
            }
        }

        //Sends messages to the server from each client
        public class Shout
        {

            IPAddress ip = IPAddress.Parse(YAMS.Database.GetSetting("server-ip", "MC"));
            int port = Convert.ToInt32(YAMS.Database.GetSetting("server-port", "MC"));
            Socket sock;

            Thread dataReader;

            //Sends packet to the server
            public Shout()
            {
                this.sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.sock.Connect(this.ip, this.port);

                this.sock.SendTimeout = 10000;
                this.sock.ReceiveTimeout = 10000;

                dataReader = new Thread(new ThreadStart(Read));
                dataReader.Start();
            }

            public void Read()
            {
                byte[] packet = new byte[1];

                while (this.sock.Connected)
                {
                    packet = new byte[1];

                }
            }
        }
        
    }
}
