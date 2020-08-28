using System;

namespace TinaX.TS.Internal
{
    public class LuaBootstrap : IXBootstrap
    {
        public void OnInit(IXCore core) { }
        public void OnStart(IXCore core)
        {
            if(core.Services.TryGet<ITSInternal>(out var ts))
            {
                try
                {
                    ts.RequireEntryFile();
                }
                catch(Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                }
            }
        }
        public void OnAppRestart() { }

        public void OnQuit() { }

    }
}
