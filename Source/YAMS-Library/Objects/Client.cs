using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;

namespace YAMS
{
    public class Client
    {

        public IPAddress ip;
        public Wrapper.Shout shouter;

        public string name = "";

        public Client(Socket sktClient, Wrapper.Listen listener)
        {
            this.ip = ((IPEndPoint)sktClient.RemoteEndPoint).Address;

            this.shouter = new Wrapper.Shout();
            sktClient.SendTimeout = 10000;
            sktClient.ReceiveTimeout = 10000;




            //Woo a legit client, let's add them to our list
            listener.Clients.Add(this);
        }

    }
}
