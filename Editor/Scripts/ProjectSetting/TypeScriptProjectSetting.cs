using System.IO;
using TinaX;
using TinaX.TS.Const;
using TinaX.TS.Internal;
using TinaXEditor.TS.Const;
using UnityEditor;
using UnityEngine;

namespace TinaXEditor.Lua.Internal
{
    public static class LuaProjectSetting
    {
        private static bool mDataRefreshed = false;
        private static TSConfig mConfig;
        private static string[] _extNames;
        

        [SettingsProvider]
        public static SettingsProvider XRuntimeSetting()
        {
            return new SettingsProvider(TSEditorConst.ProjectSetting_Node, SettingsScope.Project, new string[] { "Nekonya", "TinaX", "TS", "TypeScript", "TinaX.TS", "puerts" })
            {
                label = "X TypeScript",
                guiHandler = (searchContent) =>
                {
                    if (!mDataRefreshed) refreshData();
                    if (mConfig == null)
                    {
                        GUILayout.Space(20);
                        GUILayout.Label(I18Ns.NoConfig);
                        if (GUILayout.Button(I18Ns.BtnCreateConfigFile, Styles.style_btn_normal, GUILayout.MaxWidth(120)))
                        {
                            mConfig = XConfig.CreateConfigIfNotExists<TSConfig>(TSConst.ConfigPath_Resources, AssetLoadType.Resources);
                            refreshData();
                        }
                    }
                    else
                    {
                        GUILayout.Space(20);

                        //Enable Lua
                        mConfig.Enable = EditorGUILayout.ToggleLeft(I18Ns.EnableLua, mConfig.Enable);


                        //Entry File
                        EditorGUILayout.Space();
                        EditorGUILayout.LabelField(I18Ns.EntryFilePath);
                        EditorGUILayout.BeginHorizontal();
                        mConfig.EntryJsFileLoadPath = EditorGUILayout.TextField(mConfig.EntryJsFileLoadPath);
                        if (GUILayout.Button("Select",Styles.style_btn_normal, GUILayout.Width(55)))
                        {
                            var path = EditorUtility.OpenFilePanel("Select JavaScript Entry File", "Assets/", "");
                            if (!path.IsNullOrEmpty())
                            {
                                var root_path = Directory.GetCurrentDirectory().Replace("\\", "/");
                                if (path.StartsWith(root_path))
                                {
                                    path = path.Substring(root_path.Length + 1, path.Length - root_path.Length - 1);
                                    path = path.Replace("\\", "/");
                                    mConfig.EntryJsFileLoadPath = path;
                                }
                                else
                                    Debug.LogError("Invalid Path: " + path);
                            }
                            
                        }
                        EditorGUILayout.EndHorizontal();


                        //Framework Lua Folder Load Path 
                        EditorGUILayout.Space();
                        EditorGUILayout.LabelField(I18Ns.FrameworkInternalLuaFolderLoadPath);
                        EditorGUILayout.BeginHorizontal();
                        mConfig.FrameworkInternalJsFolderLoadPath = EditorGUILayout.TextField(mConfig.FrameworkInternalJsFolderLoadPath);
                        EditorGUILayout.EndHorizontal();


                        //extension
                        EditorGUILayout.Space();
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(I18Ns.LuaExtension, GUILayout.MaxWidth(120));
                        mConfig.JsFileExtensionName = EditorGUILayout.TextField(mConfig.JsFileExtensionName, GUILayout.MaxWidth(120));
                        if(mConfig.JsFileExtensionName != ".js.txt")
                        {
                            if (GUILayout.Button(".lua.txt", Styles.style_btn_normal, GUILayout.Width(75)))
                                mConfig.JsFileExtensionName = ".js.txt";
                        }
                        if (mConfig.JsFileExtensionName != ".js.bytes")
                        {
                            if (GUILayout.Button(".js.bytes", Styles.style_btn_normal, GUILayout.Width(75)))
                                mConfig.JsFileExtensionName = ".js.bytes";
                        }
                        if (mConfig.JsFileExtensionName != ".txt")
                        {
                            if (GUILayout.Button(".txt", Styles.style_btn_normal, GUILayout.Width(75)))
                                mConfig.JsFileExtensionName = ".txt";
                        }

                        EditorGUILayout.EndHorizontal();
                        
                    }
                },
                deactivateHandler = () =>
                {
                    if (mConfig != null)
                    {
                        if (!mConfig.JsFileExtensionName.IsNullOrEmpty())
                        {
                            mConfig.JsFileExtensionName = mConfig.JsFileExtensionName.ToLower();
                            if (!mConfig.JsFileExtensionName.StartsWith("."))
                                mConfig.JsFileExtensionName = "." + mConfig.JsFileExtensionName;
                        }
                        EditorUtility.SetDirty(mConfig);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                }
            };
        }



        private static void refreshData()
        {
            mConfig = XConfig.GetConfig<TSConfig>(TSConst.ConfigPath_Resources, AssetLoadType.Resources, false);



            mDataRefreshed = true;
        }


        static class Styles
        {
            private static GUIStyle _style_btn_normal; //字体比原版稍微大一号
            public static GUIStyle style_btn_normal
            {
                get
                {
                    if (_style_btn_normal == null)
                    {
                        _style_btn_normal = new GUIStyle(GUI.skin.button);
                        _style_btn_normal.fontSize = 13;
                    }
                    return _style_btn_normal;
                }
            }


        }
        static class I18Ns
        {
            private static bool? _isChinese;
            private static bool IsChinese
            {
                get
                {
                    if (_isChinese == null)
                    {
                        _isChinese = (Application.systemLanguage == SystemLanguage.Chinese || Application.systemLanguage == SystemLanguage.ChineseSimplified);
                    }
                    return _isChinese.Value;
                }
            }

            private static bool? _nihongo_desuka;
            private static bool NihongoDesuka
            {
                get
                {
                    if (_nihongo_desuka == null)
                        _nihongo_desuka = (Application.systemLanguage == SystemLanguage.Japanese);
                    return _nihongo_desuka.Value;
                }
            }

            public static string NoConfig
            {
                get
                {
                    if (IsChinese)
                        return "在首次使用TinaX TypeScript的设置工具前，请先创建配置文件";
                    if (NihongoDesuka)
                        return "TinaX Luaセットアップツールを初めて使用する前に、構成ファイルを作成してください";
                    return "Before using the TinaX Lua setup tool for the first time, please create a configuration file";
                }
            }

            public static string BtnCreateConfigFile
            {
                get
                {
                    if (IsChinese)
                        return "创建配置文件";
                    if (NihongoDesuka)
                        return "構成ファイルを作成する";
                    return "Create Configure File";
                }
            }

            public static string EnableLua
            {
                get
                {
                    if (IsChinese)
                        return "启用 TypeScript Runtime";
                    if (NihongoDesuka)
                        return "TypeScriptランタイムを有効にする";
                    return "Enable TypeScript Runtime";
                }
            }

            public static string EntryFilePath
            {
                get
                {
                    if (IsChinese)
                        return "TypeScript启动入口文件加载路径";
                    if (NihongoDesuka)
                        return "TypeScript起動ファイルのロードパス";
                    return "TypeScript startup file load path";
                }
            }

            public static string FrameworkInternalLuaFolder
            {
                get
                {
                    if (IsChinese)
                        return "Framework 内置TypeScript文件根目录";
                    if (NihongoDesuka)
                        return "フレームワークの組み込みTypeScriptファイルのルートディレクトリ";
                    return "Framework built-in TypeScript files root directory";
                }
            }

            public static string FrameworkInternalLuaFolderLoadPath
            {
                get
                {
                    if (IsChinese)
                        return "Framework 内置TypeScript文件根目录在加载时的路径";
                    if (NihongoDesuka)
                        return "ロード時のフレームワーク組み込みTypeScriptファイルのルートパス";
                    return "Framework built-in TypeScript file root path at load time";
                }
            }

            public static string FrameworkInternalLuaFolder_Tips
            {
                get
                {
                    if (IsChinese)
                        return "TinaX.TS 需要将内置的TypeScript文件导入到工程中，并确保Framework内置的资源加载方法可以顺利的加载到它们。";
                    if (NihongoDesuka)
                        return "TinaX.TS は、組み込みのTypeScriptファイルをプロジェクトにインポートし、Frameworkの組み込みリソースの読み込み方法がスムーズにそれらに読み込まれるようにする必要があります。";
                    return "TinaX.TS needs to import the built-in TypeScript files into the project, and ensure that the built-in resource loading method of the Framework can be smoothly loaded into them.";
                }
            }

            public static string ImportBuildinLuaFilesToThisPath
            {
                get
                {
                    if (IsChinese)
                        return "导入内置TypeScript文件到该目录";
                    if (NihongoDesuka)
                        return "ビルトインTypeScriptファイルをこのディレクトリにインポートします";
                    return "Import built-in TypeScript files into this directory";
                }
            }

            public static string LuaExtension
            {
                get
                {
                    if (IsChinese)
                        return "TypeScript文件后缀名";
                    if (NihongoDesuka)
                        return "TypeScriptファイル拡張子";
                    return "TypeScript file extension";
                }
            }
            
            
        }
    }
}
