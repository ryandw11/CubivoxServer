using CubivoxCore.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubivoxServer.Loggers
{
    public class ServerLogger : Logger
    {
        private string modName;

        public ServerLogger(string modName)
        {
            this.modName = modName;
        }

        public void Debug(string message)
        {
            Console.WriteLine($"[{modName}] DEBUG: " + message);
        }

        public void Error(string message)
        {
            Console.WriteLine($"[{modName}] ERROR: " + message);
        }

        public void Info(string message)
        {
            Console.WriteLine($"[{modName}] INFO: " + message);
        }

        public void Warn(string message)
        {
            Console.WriteLine($"[{modName}] WARN: " + message);
        }
    }
}
