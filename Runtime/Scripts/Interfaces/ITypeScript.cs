using Puerts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinaX.TS.Internal;

namespace TinaX.TS
{
    public interface ITypeScript
    {
        bool Inited { get; }
        JsEnv JsEnv { get; }

        //ITypeScript ConfigureCustomLoadHandler(Action<CustomLoadHandlerManager> options);
        //void LoadStringAsync(string require_text, Action<LuaFunction, Exception> callback);
    }
}
