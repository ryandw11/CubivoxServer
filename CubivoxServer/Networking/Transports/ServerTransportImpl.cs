using CubivoxCore;
using CubivoxCore.Networking;
using CubivoxCore.Players;

namespace CubivoxServer.Networking.Transports
{
    // Client -> Server
    public sealed class ServerTransportImpl<T> : ServerTransport<T>, InvocableTransport where T : Delegate
    {
        private Delegate? mDelegate;

        public ServerTransportImpl(ControllerKey key) : base(key)
        {
        }

        public override void Invoke(params object[] parameters)
        {
            // Client Only
        }

        public override void Register(T method)
        {
            if (mDelegate == null)
            {
                mDelegate = method;
            }
            else
            {
                mDelegate = Delegate.Combine(mDelegate, method);
            }
        }

        void InvocableTransport.InternalInvoke(Player player, params object[] parameters)
        {
            object[] args = new object[parameters.Length + 1];
            args[0] = player;
            for(int i = 1; i < args.Length; ++i )
            {
                args[i] = parameters[i-1];
            }
            mDelegate?.DynamicInvoke(args);
        }
    }
}
