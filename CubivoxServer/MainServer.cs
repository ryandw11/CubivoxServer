using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubivoxServer
{
    public class MainServer
    {
        static void Main(string[] args)
        {
            ServerCubivox serverCubivox = new ServerCubivox();
            serverCubivox.StartServer(5555);
        }
    }
}
