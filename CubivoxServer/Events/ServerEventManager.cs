using CubivoxCore.Attributes;
using CubivoxCore;
using CubivoxCore.Events;
using CubivoxServer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CubivoxServer.Events
{
    public class ServerEventManager : EventManager
    {
        private Dictionary<Type, List<Action<Event>>> events;

        public ServerEventManager() 
        {
            events = new Dictionary<Type, List<Action<Event>>>();
        }

        public void RegisterEvent<T>(Action<T> evt) where T : Event
        {
            Type parameterType = evt.GetMethodInfo().GetParameters()[0].ParameterType;

            if (evt.Method.GetCustomAttributes(typeof(ClientOnly), true).Length > 0)
            {
                if (Cubivox.GetEnvironment() != EnvType.CLIENT)
                {
                    return;
                }
            }
            else if (evt.Method.GetCustomAttributes(typeof(ServerOnly), true).Length > 0)
            {
                if (Cubivox.GetEnvironment() != EnvType.SERVER)
                {
                    return;
                }
            }

            if (!events.ContainsKey(parameterType))
            {
                events[parameterType] = new List<Action<Event>>() { new Action<Event>(o => evt((T)o)) };
            }
            else
            {
                events[parameterType].Add(new Action<Event>(o => evt((T)o)));
            }
        }

        public bool TriggerEvent(Event evt)
        {
            if(!events.ContainsKey(evt.GetType()))
            {
                return true;
            }

            if(evt is CancellableEvent cancellableEvent)
            {
                var delegates = events[cancellableEvent.GetType()];
                foreach (var delegator in delegates)
                {
                    Isolater.Isolate(() => delegator.Invoke(cancellableEvent));

                    if (cancellableEvent.IsCanceled())
                    {
                        return false;
                    }
                }
            }
            else
            {
                var delegates = events[evt.GetType()];
                foreach (var delegator in delegates)
                {
                    Isolater.Isolate(() => delegator.Invoke(evt));
                }
            }

            return true;
        }
    }
}
