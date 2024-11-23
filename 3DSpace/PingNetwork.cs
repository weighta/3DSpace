using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _3DSpace
{
    public class PingNetwork
    {
        const int pingWait = 1000;
        public string[] pingList = new string[2] { "www.google.com", "hypixel.net" };
        public long[] result;
        PingReply[] pingReply;
        Ping[] ping;
        public bool isPinging;
        public PingNetwork()
        {
            result = new long[pingList.Length];
            pingReply = new PingReply[pingList.Length];
            ping = new Ping[pingList.Length];
            for (int i = 0; i < ping.Length; i++) ping[i] = new Ping();
            Thread pingThread = new Thread(new ParameterizedThreadStart(ping_Thread));
            pingThread.Start();
        }
        void ping_Thread(object x)
        {
            while (true)
            {
                if (isPinging)
                {
                    _Ping();
                }
                Thread.Sleep(pingWait);
            }
        }
        public void _Ping()
        {
            for (int i = 0; i < pingList.Length; i++)
            {
                result[i] = _ping(i);
            }
        }
        public long _ping(int i)
        {
            pingReply[i] = ping[i].Send(pingList[i], pingWait);
            return pingReply[i].RoundtripTime;
        }
    }
}
