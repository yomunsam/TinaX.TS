using System.Threading.Tasks;
using TinaX.Services;
using TinaX.TS.Const;

namespace TinaX.TS
{
    [XServiceProviderOrder(100)]
    public class TypeScriptProvider : IXServiceProvider
    {
        public string ServiceName => TSConst.ServiceName;

        public Task<XException> OnInit(IXCore core) => Task.FromResult<XException>(null);



        public void OnServiceRegister(IXCore core)
        {
            core.Services.Singleton<ITypeScript, TypeScriptManager>()
                .SetAlias<Internal.ITSInternal>();
        }


        public Task<XException> OnStart(IXCore core)
            => core.GetService<Internal.ITSInternal>().Start();

        public void OnQuit() { }

        public Task OnRestart() => Task.CompletedTask;
        
    }
}
