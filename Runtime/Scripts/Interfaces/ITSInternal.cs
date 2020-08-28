using System.Threading.Tasks;
using Puerts;

namespace TinaX.TS.Internal
{
    public interface ITSInternal
    {
        bool Inited { get; }
        JsEnv JsEnv { get; }

        Task<XException> Start();
        void RequireEntryFile();
    }
}
