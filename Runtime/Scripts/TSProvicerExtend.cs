using TinaX.TS;

namespace TinaX.Services
{
    public static class TypeScriptProvicerExtend
    {
        public static IXCore UseTypeScriptRuntime(this IXCore core)
        {
            core.RegisterServiceProvider(new TypeScriptProvider());
            return core;
        }
    }
}
