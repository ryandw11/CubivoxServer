using CubivoxCore.Players;

namespace CubivoxServer.Networking.Transports
{
    internal interface InvocableTransport
    {
        internal void InternalInvoke(Player player, params object[] parameters);
    }
}
