using Puerts;
using TinaX.Services;
using UnityEngine;

namespace TinaX.TS
{
    /// <summary>
    /// TinaX Default TypeScript Loader
    /// </summary>
    public class XTSLoader : ILoader
    {
        private IAssetService m_Assets;

        /// <summary>
        /// 框架内部 js代码标记
        /// 指代框架内部js代码的加载根路径
        /// </summary>
        private string m_InternalJsCodeSign;

        /// <summary>
        /// 框架内部 js 代码文件的后缀名
        /// </summary>
        private string m_InternalJsExtension;

        /// <summary>
        /// 框架内部 js 代码 的加载根路径
        /// </summary>
        private string m_InternalJsFileLoadPath;

        /// <summary>
        /// 加载 js 文件的扩展名
        /// （指业务开发者的js文件，框架内部的js文件不受此管理）
        /// </summary>
        private string m_JsFileExtension;

        /// <summary>
        /// 记录最后一次加载的Js文件的内容
        /// </summary>
        private string m_LastLoadCache_Content { get; set;}

        /// <summary>
        /// 记录最后一次加载的Js文件的路径
        /// </summary>
        private string m_LastLoadCache_Path { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="assets">框架的内置服务接口</param>
        /// <param name="internalJsCodeSign">框架内部js代码标记</param>
        /// <param name="internalJsExtension">框架内部js文件后缀名</param>
        /// <param name="internalJsFileLoadPath">框架内部js文件加载根路径</param>
        /// <param name="jsFileExtension">加载js文件扩展名</param>
        public XTSLoader(IAssetService assets, string internalJsCodeSign, string internalJsExtension, string internalJsFileLoadPath, string jsFileExtension)
        {
            m_Assets = assets;
            m_InternalJsCodeSign = internalJsCodeSign;
            m_InternalJsExtension = internalJsExtension;
            m_InternalJsFileLoadPath = internalJsFileLoadPath;
            m_JsFileExtension = jsFileExtension;
        }

        public bool FileExists(string filepath)
        {
            m_LastLoadCache_Content = string.Empty;
            m_LastLoadCache_Path = string.Empty;

            //检查是不是puerts内部方法
            if (filepath.StartsWith("puerts/"))
            {
                var ta = Resources.Load<TextAsset>(filepath);
                if (ta == null)
                    return false;
                else
                {
                    m_LastLoadCache_Path = filepath; //记录缓存
                    m_LastLoadCache_Content = ta.text;
                    return true;
                }
            }
            //使用同步方式加载资源
            try
            {
                string path = filepath;
                LoadFilePathHandle(ref path);
                var ta = m_Assets.Load<TextAsset>(path);

                if (ta != null)
                {
                    m_LastLoadCache_Path = path;
                    m_LastLoadCache_Content = ta.text;
                    m_Assets.Release(ta);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string ReadFile(string filepath, out string debugpath)
        {
            //检查是不是puerts内部方法
            if (filepath.StartsWith("puerts/"))
            {
                //检查是否有缓存，有的话直接返回
                if (!m_LastLoadCache_Path.IsNullOrEmpty() && m_LastLoadCache_Path.Equals(filepath))
                {
                    debugpath = filepath;
                    return m_LastLoadCache_Content;
                }
                UnityEngine.TextAsset file = (UnityEngine.TextAsset)UnityEngine.Resources.Load(filepath);
                debugpath = filepath;
                return file == null ? null : file.text;
            }
            string path = filepath;
            LoadFilePathHandle(ref path);

            //检查缓存
            if (!m_LastLoadCache_Path.IsNullOrEmpty() && m_LastLoadCache_Path.Equals(path))
            {
                debugpath = filepath; //TODO
                return m_LastLoadCache_Content;
            }

            var ta_js_file = m_Assets.Load<TextAsset>(path);
            string code = ta_js_file.text;
            m_Assets.Release(ta_js_file);

            //TODO：Debug Path
            debugpath = filepath;

            return code;
        }

        /// <summary>
        /// 处理 加载文件 路径相关
        /// </summary>
        /// <param name="fileName"></param>
        public void LoadFilePathHandle(ref string fileName)
        {
            //if (fileName.IndexOf('.') != -1)
            //    fileName = fileName.Replace('.', '/');
            // 点号拼接路径是lua的规则，js/ts应该不需要这玩意

            if (fileName.EndsWith(".js"))
            {
                fileName = fileName.Substring(0, fileName.Length - 3);
            }

            //检查是不是框架内部的文件
            bool framework_file = false;
            //转义
            if (fileName.StartsWith(m_InternalJsCodeSign))
            {
                fileName = fileName.Replace(m_InternalJsCodeSign, m_InternalJsFileLoadPath);
                framework_file = true;
            }

            //后缀
            if (framework_file)
                fileName += m_InternalJsExtension;
            else
                fileName += m_JsFileExtension;

        }
    }
}
