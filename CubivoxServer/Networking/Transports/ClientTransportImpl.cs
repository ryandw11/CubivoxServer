using CubivoxCore.Networking;
using CubivoxCore.Players;
using CubivoxCore;
using CubivoxServer.Protocol.ClientBound;
using CubivoxServer.Players;
using CubivoxCore.Networking.DataFormat;

namespace CubivoxServer.Networking.Transports
{
    // Server -> Client
    public sealed class ClientTransportImpl<T> : ClientTransport<T> where T : Delegate
    {

        public ClientTransportImpl(ControllerKey key) : base(key)
        {
        }

        public override void Invoke(Player player, params object[] parameters)
        {
            TransportFormat.AssertTypeMatch(GetParameterTypes(), parameters, false);

            ClientTransportPacket transportPacket = new ClientTransportPacket(mKey, parameters);
            ServerPlayer serverPlayer = (ServerPlayer)player;
            serverPlayer.SendPacket(transportPacket);
        }

        public override void Register(T method)
        {
            // Client Only
        }
    }
}
