using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using System.Reflection;


#if !DISABLE_ILRUNTIME
public class MethodIL : IMethodInfoData
{
    public string Name => Data.Name;
    public int ParamCount { get; private set; }
    public ILRuntime.CLR.Method.IMethod Data;

    public MethodIL(ILRuntime.CLR.Method.IMethod method)
    {
        this.Data = method;
        this.ParamCount = method.ParameterCount;
    }

    public ParameterInfo[] GetParameters()
    {
        return Array.Empty<ParameterInfo>();
    }

    public string ToJson(object[] arr)
    {
        if (arr.Length > 0)
        {
            SerDict<string, string> data = new SerDict<string, string>();
            var infos = Data.Parameters;
            for (int i = 0; i < arr.Length; i++)
            {
                data[infos[i].Name] = arr[i].ToString();
            }

            var content = JsonUtility.ToJson(data);
            return content;
        }

        return string.Empty;
    }

    public object[] FromJson(string stringObjs)
    {
        object[] objs = new object[0];
        if (!string.IsNullOrEmpty(stringObjs))
        {
            SerDict<string, string> data = JsonUtility.FromJson<SerDict<string, string>>(stringObjs);
            objs = new object[data.Count];
            var infos = Data.Parameters;
            for (int i = 0; i < data.Count; i++)
            {
                objs[i] = data[infos[i].Name] as object;
            }
        }

        return objs;
    }
}
#endif