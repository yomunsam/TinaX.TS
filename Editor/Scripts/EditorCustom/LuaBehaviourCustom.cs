using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinaXEditor.XComponent.GUICustom;
using UnityEditor;
using TinaX.TS;

namespace TinaXEditor.Lua.EditorCustom
{
    [CustomEditor(typeof(TSBehaviour))]
    public class LuaBehaviourCustom : XComponentCustom
    {
        private TSBehaviour __target;


        public override void OnInspectorGUI()
        {
            if (__target == null)
                __target = (TSBehaviour)target;
            GUILayout.Space(5);
            //Lua Script
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Module Name:", GUILayout.Width(85));
            //__target.LuaScript = (TextAsset)EditorGUILayout.ObjectField(__target.LuaScript, typeof(TextAsset), true);
            __target.ModuleName = EditorGUILayout.TextField(__target.ModuleName);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);
            //Update Time
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Update Order:", GUILayout.Width(120));
            __target.UpdateOrder = EditorGUILayout.IntField(__target.UpdateOrder, GUILayout.Width(35));
            EditorGUILayout.EndHorizontal();

            //LateUpdate Time
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("LateUpdate Order:", GUILayout.Width(120));
            __target.LateUpdateOrder = EditorGUILayout.IntField(__target.LateUpdateOrder, GUILayout.Width(35));
            EditorGUILayout.EndHorizontal();

            //FixedUpdate Time
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("FixedUpdate Order:", GUILayout.Width(120));
            __target.FixedUpdateOrder = EditorGUILayout.IntField(__target.FixedUpdateOrder, GUILayout.Width(35));
            EditorGUILayout.EndHorizontal();

            base.OnInspectorGUI();
        }
    }
}

