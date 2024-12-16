using CubivoxCore.Networking.DataFormat;
using CubivoxCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using CubivoxCore.Players;

namespace CubivoxServer.Protocol.ClientBound
{
    public class ClientTransportPacket : ClientBoundPacket
    {
        private ControllerKey controllerKey;
        private object[] parameters;
        public ClientTransportPacket(ControllerKey controllerKey, object[] parameters)
        {
            this.controllerKey = controllerKey;
            this.parameters = parameters;
        }

        public void WritePacket(NetworkStream stream)
        {
            string key = controllerKey.ToString();
            if (key.Length >= short.MaxValue)
            {
                stream.WriteByte(0);
                stream.WriteByte(0);
                return;
            }

            byte[] data = TransportFormat.WriteObjects(parameters);

            if (data == null)
            {
                stream.WriteByte(0);
                stream.WriteByte(0);
                return;
            }

            if (data.Length > TransportFormat.MaxSize)
            {
                stream.WriteByte(0);
                stream.WriteByte(0);
                return;
            }

            stream.Write(BitConverter.GetBytes((short)key.Length));
            stream.Write(Encoding.ASCII.GetBytes(key));
            stream.Write(BitConverter.GetBytes(data.Length));
            stream.Write(data);
        }

        byte Packet.GetType()
        {
            return 0x0A;
        }
    }
}
