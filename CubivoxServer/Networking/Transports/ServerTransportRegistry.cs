using CubivoxCore.Exceptions;
using CubivoxCore.Networking;
using CubivoxCore.Players;
using CubivoxCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CubivoxCore.Networking.DataFormat;

namespace CubivoxServer.Networking.Transports
{
    public sealed class ServerTransportRegistry : TransportRegistry
    {
        // Server -> Client
        public override void Transport<T>(Player player, params object[] parameters)
        {
            IClientTransport? transport;
            if (mClientTransports.TryGetValue(typeof(T), out transport))
            {
                transport.Invoke(player, parameters);
            }
        }

        // Client -> Server
        public override void Transport<T>(params object[] parameters)
        {
            throw new InvalidEnvironmentException("This method can only be called on a client!");
        }

        protected override ClientTransport<T> CreateClientTransport<T>(ControllerKey key)
        {
            return new ClientTransportImpl<T>(key);
        }

        protected override ServerTransport<T> CreateServerTransport<T>(ControllerKey key)
        {
            return new ServerTransportImpl<T>(key);
        }

        internal bool InvokeTransport(Player player, ControllerKey key, byte[] data)
        {
            Type type;
            if (mTypeMap.TryGetValue(key, out type))
            {
                IServerTransport transport = mServerTransports[type];
                var serverTransport = (InvocableTransport)transport;

                Type[] allParamTypes = transport.GetParameterTypes();
                Type[] filteredParamType = new Type[allParamTypes.Length - 1];
                Array.Copy(allParamTypes, 1, filteredParamType, 0, allParamTypes.Length - 1);

                var objs = TransportFormat.ReadObjects(filteredParamType, data);

                if (objs == null)
                {
                    return false;
                }

                serverTransport.InternalInvoke(player, objs.ToArray());

                return true;
            }

            return false;
        }
    }
}
