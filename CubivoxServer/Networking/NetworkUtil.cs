using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace CubivoxServer.Networking
{
    public class NetworkUtil
    {
        /// <summary>
        /// Fill a byte buffer full with data from the NetworkStream.
        /// </summary>
        /// <param name="buffer">The byte buffer to fill.</param>
        /// <param name="stream">The network stream.</param>
        /// <returns>The asynchronous task.</returns>
        public static async Task FillBufferFromNetwork(byte[] buffer, NetworkStream stream)
        {
            int buffIndex = 0;
            while (buffIndex < buffer.Length)
            {
                buffIndex += await stream.ReadAsync(buffer, buffIndex, buffer.Length - buffIndex);
            }
        }
    }
}
