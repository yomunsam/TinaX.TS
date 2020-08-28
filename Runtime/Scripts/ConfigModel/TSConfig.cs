using UnityEngine;

namespace TinaX.TS.Internal
{
    public class TSConfig : ScriptableObject
    {
        public bool Enable = true;

        public string JsFileExtensionName = ".js.txt";


        public string FrameworkInternalJsFolderLoadPath = "Assets/TinaX/TS/TS"; //内置资源文件夹在加载时的根目录路径

        public string EntryJsFileLoadPath; //入口文件地址
    }
}
