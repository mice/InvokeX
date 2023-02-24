using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Text;


public class UIRuntimeCallWindow : UnityEditor.EditorWindow
{

    private static bool NeedPlaying = false;
    [MenuItem("Window/UIElements/GM")]
    static void Init()
    {
        if (!NeedPlaying || Application.isPlaying)
        {
            var window = EditorWindow.GetWindow<UIRuntimeCallWindow>(true, "GM测试工具");
            window.minSize = new Vector2(1200, 300);
        }
        else
        {
            UnityEditor.EditorUtility.DisplayDialog("错误", "请先登录游戏", "确定");
        }
    }

    private void InitProtoSections()
    {
        GUILayout.BeginHorizontal(GUILayout.MinHeight(320));
       
            GUILayout.BeginVertical(GUILayout.MinWidth(240), GUILayout.MinHeight(320));
            OnInspectorGUI();
            GUILayout.EndVertical();
      
        GUILayout.EndHorizontal();
    }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(0, 0, 1200, 600));

        GUILayout.BeginVertical();
        GUILayout.Label("GM测试工具");
        InitProtoSections();
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    
    private Vector2 ratio;
    private MessageSendData searchData = new MessageSendData();
    
    private void InitInputSection(Dictionary<string,MethodBase> _findTypies,List<string> searchResult)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("GM工具:");
        var protoName = searchData._searchStr;
        protoName = EditorGUILayout.TextField(protoName, GUILayout.MinWidth(160));
        if (protoName != searchData._searchStr)
        {
            searchData._searchStr = protoName;
            UpdateSearchList(_findTypies, searchData._searchStr, searchResult);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginScrollView(new Vector2(), GUILayout.Height(Mathf.Min(20 * searchResult.Count, 240)));
        foreach (var k in searchResult)
        {
            EditorGUILayout.LabelField(k);
        }
        EditorGUILayout.EndScrollView();
    }

    private static bool IsLabledTypes(Type t)
    {
        return t == typeof(Vector3);
    }

    private void InitTypeSection(List<ValueTuple<string,Type, System.Object>> typePropDict, string selectType)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("type:", GUILayout.MinWidth(60));
        EditorGUILayout.LabelField(selectType);
        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < typePropDict.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            var tmpV = typePropDict[i];
            var tmpKey = tmpV.Item1;
           
            var tmpType = tmpV.Item2;
            if (IsLabledTypes(tmpType))
            {
                if (tmpType == typeof(UnityEngine.Vector3))
                {
                    var tmpVec3 = EditorGUILayout.Vector3Field($"[{tmpKey}]:{i}", tmpV.Item3 is UnityEngine.Vector3 v3 ? v3 : UnityEngine.Vector3.zero);
                    tmpV = (tmpKey, tmpType, tmpVec3);
                }
            }
            else
            {
                EditorGUILayout.LabelField($"[{tmpKey}]:{i}", GUILayout.MinWidth(60));
                if (tmpType == typeof(int))
                {
                    var tmpInt = EditorGUILayout.IntField(Convert.ToInt32(tmpV.Item3));
                    tmpV = (tmpKey, tmpType, tmpInt);
                }
                if (tmpType == typeof(long))
                {
                    var tmpInt = EditorGUILayout.LongField(Convert.ToInt64(tmpV.Item3));
                    tmpV = (tmpKey, tmpType, tmpInt);
                }
                else if (tmpType == typeof(String))
                {
                    var tmpStr = EditorGUILayout.TextField(tmpV.Item3 == null ? String.Empty : tmpV.Item3.ToString());
                    tmpV = (tmpKey, tmpType, tmpStr);
                }
                else if (tmpType == typeof(bool))
                {
                    var tmpBool = EditorGUILayout.Toggle(tmpV.Item3 == null ? false : Convert.ToBoolean(tmpV.Item3));
                    tmpV = (tmpKey, tmpType, tmpBool);
                }
            }
           
            typePropDict[i] = tmpV;
            EditorGUILayout.EndHorizontal();
        }
    }

    public void OnInspectorGUI()
    {
        var _findTypies = searchData._findTypies;
        var searchResult = searchData.searchResult;
        var selectMethod = searchData.selectMethod;
        if (_findTypies == null || _findTypies.Count == 0)
        {
            if (GUILayout.Button("点击开始"))
            {
                _InitTypes(null,_findTypies);
                searchData._searchStr = String.Empty;
                UpdateSearchList(_findTypies, searchData._searchStr, searchResult);
            }
        }
        else
        {
            EditorGUILayout.BeginVertical();
            InitInputSection(_findTypies, searchResult);
            EditorGUILayout.EndVertical();

            if (searchResult.Count >= 1)
            {
                if (selectMethod == null || (selectMethod.Name != searchResult[0]))
                {
                    selectMethod = InitMethod(_findTypies, searchResult[0], searchData);
                    searchData.selectMethod = selectMethod;
                }
            }
        }

        if (selectMethod != null)
        {
            var typePropDict = searchData.typePropDict;
            if (typePropDict.Count>0)
            {
                EditorGUILayout.BeginVertical();
                InitTypeSection(typePropDict, searchData.selectType);
                EditorGUILayout.EndVertical();
            }

            if (GUILayout.Button("Send:"))
            {
                _TestSend();
            }
        }
    }

    /*****/
    private void _TestSend()
    {
        var selectMethod = searchData.selectMethod;
        var typePropDict = searchData.typePropDict;
        if ((!NeedPlaying || Application.isPlaying ) && selectMethod != null)
        {
            var sb = new StringBuilder();
            var objs = new System.Object[typePropDict.Count];
            for (int i = 0; i < typePropDict.Count; i++)
            {
                var item = typePropDict[i];
                sb.AppendLine(item.ToString());
                objs[i] = item.Item3;
            }
        
            UnityEngine.Debug.LogError($"Send:{sb.ToString()}");
            RuntimeCallManager.Instance.Invoke(searchData.selectType,searchData.selectMethod.Name,objs);
        }
    }

    private static void UpdateSearchList(Dictionary<string, MethodBase> typeMethodDict, string key, List<String> results)
    {
        results.Clear();
        if (String.IsNullOrEmpty(key))
        {
            results.AddRange(typeMethodDict.Keys);
        }
        else
        {
            foreach (var k in typeMethodDict.Keys)
            {
                if (k.Contains(key))
                {
                    results.Add(k);
                }
            }
        }
    }

    private MethodBase InitMethod(Dictionary<string, MethodBase> typeMethodDict, string methodName, MessageSendData data)
    {
        var typePropDict = data.typePropDict;
        if(typeMethodDict.TryGetValue(methodName, out var methodData))
        {
            typePropDict.Clear();
            if (data.selectType != null)
            {
                typePropDict.Clear();

                var parameters = methodData.GetParameters();
                for (int i = 0; i < parameters.Length; i++)
                {
                    typePropDict.Add((parameters[i].Name,parameters[i].ParameterType, null));
                }
                if (typePropDict.Count > 0)
                {
                    //Ass(selectType.Name + "_RunTime", typePropDict);
                }
            }
        }
        return methodData;
    }

    private void _InitTypes(Type type,Dictionary<string, MethodBase> typeMethodDict)
    {
        if (typeMethodDict != null && typeMethodDict.Count > 0)
            return;

        var targetMgr = RuntimeCallManager.Instance;
        targetMgr.AddInstance("GM", new GMScripts());
        targetMgr.AddInstance(nameof(ProtocalX), new ProtocalX());
        targetMgr.AddStatic(typeof(ViewUtils));
        targetMgr.GetMethodDictionary("GM",typeMethodDict);
    }

    public class MessageSendData
    {
        public string _searchStr;

        public Dictionary<string, MethodBase> _findTypies = new Dictionary<string, MethodBase>();
        public string selectType = "GM";
        public MethodBase selectMethod;
        public List<string> searchResult = new List<string>();
        public List<ValueTuple<string,Type, System.Object>> typePropDict = new List<ValueTuple<string,Type, System.Object>>();
    }
}