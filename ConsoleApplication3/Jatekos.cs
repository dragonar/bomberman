using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Concurrent;

namespace ConsoleApplication3
{
    class Jatekos
    {
        public uint ID;
        public String Nev;
        public Color Szin;
        public uint x;
        public uint y;
        public uint Sebesseg;
        public bool Ele;
        public uint Maxbombaszam;
        public uint Actbombaszam;
        public uint Rendzs;

        public ConcurrentQueue<string> Uzisor;

        public TcpClient tcp;
        public Thread thread;
    }
}
