using CubivoxCore.Networking.DataFormat;
using CubivoxCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using CubivoxServer.Networking;
using CubivoxServer.Networking.Transports;

namespace CubivoxServer.Protocol.ServerBound
{
    public class ServerTransportPacket : ServerBoundPacket
    {
        private ServerTransportRegistry transportRegistery;

        public ServerTransportPacket()
        {
            transportRegistery = (ServerTransportRegistry)Cubivox.GetTransportRegistry();
        }

        public async Task<bool> ProcessPacket(Client client, NetworkStream stream)
        {
            byte[] primitiveBuffer = new byte[4];

            await NetworkUtil.ReadFromNetwork(primitiveBuffer, 2, stream);
            short keyLength = BitConverter.ToInt16(primitiveBuffer, 0);

            byte[] keyBuffer = new byte[keyLength];
            await NetworkUtil.ReadFromNetwork(keyBuffer, keyLength, stream);
            string keyString = Encoding.ASCII.GetString(keyBuffer, 0, keyLength);

            ControllerKey key;

            try
            {
                key = new ControllerKey(keyString);
            }
            catch (ArgumentException)
            {
                return false;
            }

            await NetworkUtil.ReadFromNetwork(primitiveBuffer, 4, stream);
            int dataLength = BitConverter.ToInt32(primitiveBuffer);

            if (dataLength > TransportFormat.MaxSize)
            {
                return false;
            }

            byte[] dataBuffer = new byte[dataLength];
            await NetworkUtil.ReadFromNetwork(dataBuffer, dataLength, stream);

            if (!transportRegistery.InvokeTransport(client.GetServerPlayer()!, key, dataBuffer))
            {
                Cubivox.GetInstance().GetLogger().Error($"An error has occured trying to invoke transport {key}, has it been registered?");
            }

            return true;
        }

        byte Packet.GetType()
        {
            return 0x05;
        }
    }
}
