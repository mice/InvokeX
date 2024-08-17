using UnityEngine;
using System;
using System.Reflection;

public class ILMethod : IMethodInfoData
{
    public string Name { get; private set; }
    public string TargetTypeName { get; private set; }
    public string TypeName => nameof(ILMethod);
    public int ParamCount { get; private set; }
#if !DISABLE_ILRUNTIME
    public ILRuntime.CLR.Method.IMethod Data;

    public ILMethod(ILRuntime.CLR.Method.IMethod method)
    {
        this.Data = method;
        this.Name = Data.Name;
        this.TargetTypeName = Data.DeclearingType.Name;
        this.ParamCount = method.ParameterCount;
    }
#else
    public ILMethod(object method)
    {
       
    }
#endif

    public ParameterInfo[] GetParameters()
    {
        return Array.Empty<ParameterInfo>();
    }

    public string ToJson(object[] arr)
    {
#if !DISABLE_ILRUNTIME
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
#endif
        return string.Empty;
    }

    public object[] FromJson(string stringObjs)
    {
        object[] objs = new object[0];
#if !DISABLE_ILRUNTIME
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
#endif
        return objs;
    }
}