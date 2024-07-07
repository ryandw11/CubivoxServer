using CubivoxCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubivoxServer.Utils
{
    public class Isolater
    {
        public static void Isolate(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Cubivox.GetInstance().GetLogger().Warn("An internal error has occured!");
                Cubivox.GetInstance().GetLogger().Warn(ex.Message);
                if( ex.StackTrace != null )
                {
                    Cubivox.GetInstance().GetLogger().Warn(ex.StackTrace);
                }
            }
        }
    }
}
