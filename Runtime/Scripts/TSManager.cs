using System.Threading.Tasks;
using Puerts;
using TinaX.Core.Localization;
using TinaX.Services;
using TinaX.TS.Const;
using TinaX.TS.Internal;
using UnityEngine;

namespace TinaX.TS
{
    public class TypeScriptManager : ITypeScript, ITSInternal
    {
        private const string c_InternalJsSign = @"{tinax}";
        private const string c_Internal_Js_Extension = ".js.txt";
        private const string c_InternalJsEntryPath = @"{tinax}.init.init";

        private IXCore m_Core;

        private TSConfig m_Config;
        private TinaX.Services.IAssetService m_Assets;

        private JsEnv m_JsEnv;
        //private LuaEnv.CustomLoader m_Loader;
        private static float m_lastGCTime = 0;
        private const float m_GCInterval = 1; //1 second

        private string m_Internal_Js_Folder_Load_Path;
        private string m_Internal_Js_Folder_Load_Path_withSlash;

        private string m_JsExtension;
        private bool m_Inited;
        private TinaX.Systems.ITimeTicket m_UpdateTicket;

        /// <summary>
        /// 预先加载的入口文件内容
        /// </summary>
        private string m_EntryCodeContent;
        //private LuaFunction m_EntryFunc;

        //private CustomLoadHandlerManager m_CustomLoadHandlerManager = new CustomLoadHandlerManager();

        public TypeScriptManager(IAssetService buildInAssets)
        {
            m_Assets = buildInAssets;
            m_Core = XCore.GetMainInstance();
            //m_JsVM = new JsEnv(new XTSLoader(buildInAssets, c_InternalLuaSign, ));
        }

        ~TypeScriptManager()
        {
            m_UpdateTicket?.Unregister();
        }

        public JsEnv JsEnv => m_JsEnv;
        public bool Inited => m_Inited;

        public async Task<XException> Start()
        {
            if (m_Inited) return null;
            m_Config = XConfig.GetConfig<TSConfig>(TSConst.ConfigPath_Resources);
            if (m_Config == null)
                return new XException($"[{TSConst.ServiceName}] Connot found config file.");

            if (!m_Config.Enable) return null;

            //框架内部Js文件加载路径
            m_Internal_Js_Folder_Load_Path = m_Config.FrameworkInternalJsFolderLoadPath;
            if (!m_Internal_Js_Folder_Load_Path.IsNullOrEmpty())
            {
                if (m_Internal_Js_Folder_Load_Path.EndsWith("/"))
                    m_Internal_Js_Folder_Load_Path = m_Internal_Js_Folder_Load_Path.Substring(0, m_Internal_Js_Folder_Load_Path.Length - 1);
                m_Internal_Js_Folder_Load_Path_withSlash = m_Internal_Js_Folder_Load_Path + "/";
            }
            //Js文件加载名称
            m_JsExtension = m_Config.JsFileExtensionName;
            if (!m_JsExtension.StartsWith("."))
                m_JsExtension = "." + m_JsExtension;

            if(m_Assets == null)
                return new XException($"[{TSConst.ServiceName}]" + (m_Core.IsCmnHans() ? "没有任何服务实现了Framework中的内置的资产加载接口" : "No service implements the built-in asset loading interface in Framework"));

            //初始化Js的运行环境
            m_JsEnv = new JsEnv(new XTSLoader(m_Assets, c_InternalJsSign, c_Internal_Js_Extension, m_Internal_Js_Folder_Load_Path, m_JsExtension));

            //try
            //{
            //    await InitInternalEntry();
            //}
            //catch(XException e)
            //{
            //    return e;
            //}

            //准备好入口文件
            if(!m_Config.EntryJsFileLoadPath.IsNullOrEmpty())
            {
                try
                {
                    TextAsset entry_ta = await m_Assets.LoadAsync<TextAsset>(m_Config.EntryJsFileLoadPath);
                    m_EntryCodeContent = entry_ta.text;
                    //m_EntryFunc = m_JsEnv.LoadString<LuaFunction>(entry_ta.bytes, m_Config.EntryLuaFileLoadPath);
                    m_Assets.Release(entry_ta);
                }
                catch(XException e)
                {
                    return e;
                }
            }

            if(m_UpdateTicket != null)
                m_UpdateTicket.Unregister();
            m_UpdateTicket = TimeMachine.RegisterUpdate(OnUpdate);

            m_Inited = true;
            return null;
        }

        //public ILua ConfigureCustomLoadHandler(Action<CustomLoadHandlerManager> options)
        //{
        //    options?.Invoke(m_CustomLoadHandlerManager);
        //    return this;
        //}



        //public void LoadStringAsync(string require_text, Action<LuaFunction,Exception> callback)
        //{
        //    string final_path = require_text;
        //    LuaRequireToPath(ref final_path);
        //    m_Assets.LoadAsync(final_path, typeof(TextAsset), (asset, error) =>
        //    {
        //        if (error != null)
        //        {
        //            callback?.Invoke(null, error);
        //        }
        //        else
        //        {
        //            var func = m_JsEnv.LoadString<LuaFunction>(((TextAsset)asset).bytes, final_path);
        //            callback?.Invoke(func, null);
        //            m_Assets.Release(asset);
        //        }
        //    });
        //}

        public void RequireEntryFile()
        {
            if (!m_Config.EntryJsFileLoadPath.IsNullOrEmpty() && !m_EntryCodeContent.IsNullOrEmpty())
            {
                Debug.Log("喵喵喵");
                m_JsEnv.Eval($"require('Assets/App/TypeScript/HelloWorld')", m_Config.EntryJsFileLoadPath);
                //m_JsEnv.Eval(m_EntryCodeContent, m_Config.EntryJsFileLoadPath);
            }
        }

        //private byte[] LoadLuaFiles(ref string fileName)
        //{
        //    if(m_CustomLoadHandlerManager.TryGetHandler(fileName,out var handler)) //自定义加载
        //    {
        //        return handler?.Invoke();
        //    }


        //    LuaRequireToPath(ref fileName);

        //    //使用同步接口加载资源
        //    try
        //    {
        //        var ta = m_Assets.Load<TextAsset>(fileName);
        //        byte[] code = ta.bytes;
        //        m_Assets.Release(ta);
        //        return code;
        //    }
        //    catch
        //    {
        //        Debug.LogWarning("Load Lua file failed: " + fileName);
        //        return null;
        //    }
        //}

        //private async Task InitInternalEntry()
        //{
        //    try
        //    {
        //        string final_path = c_InternalJsEntryPath;
        //        LuaRequireToPath(ref final_path);
        //        TextAsset ta = await m_Assets.LoadAsync<TextAsset>(final_path);
        //        object[] obj_result = m_JsEnv.DoString(ta.bytes, final_path);
        //        LuaTable table = (LuaTable)obj_result[0];
        //        List<string> init_list = table.Cast<List<string>>();

        //        List<Task> list_task = new List<Task>();
        //        foreach (var item in init_list)
        //        {
        //            list_task.Add(require_init_file(item));
        //        }
        //        await Task.WhenAll(list_task);
        //        m_Assets.Release(ta);
        //    }
        //    catch(XException e)
        //    {
        //        throw e;
        //    }
        //    catch(Exception e)
        //    {
        //        Debug.LogException(e);
        //    }
        //}

        //private async Task require_init_file(string req_str)
        //{
        //    string final_path = req_str;
        //    LuaRequireToPath(ref final_path);
        //    TextAsset ta = await m_Assets.LoadAsync<TextAsset>(final_path);
        //    m_JsEnv.DoString(ta.bytes, final_path);
        //    m_Assets.Release(ta);
        //}

        private void OnUpdate()
        {
            if (Time.time - m_lastGCTime > m_GCInterval)
            {
                m_JsEnv.Tick();
                m_lastGCTime = Time.time;
            }
        }

        

        
    }
}
