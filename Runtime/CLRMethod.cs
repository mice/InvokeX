using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class CLRMethod : IMethodInfoData
{
    public string Name => Data.Name;
    public string TargetTypeName => Data.DeclaringType.Name;
    public string TypeName => nameof(CLRMethod);
    public int ParamCount { get; private set; }
    public MethodBase Data;

    public CLRMethod(MethodBase method)
    {
        this.Data = method;
        this.ParamCount = method.GetParameters().Length;
    }

    public ParameterInfo[] GetParameters()
    {
        return Data.GetParameters();
    }

    public string ToJson(object[] arr)
    {
        if (arr.Length > 0)
        {
            SerDict<string, string> data = new SerDict<string, string>();
            var infos = Data.GetParameters();
            for (int i = 0; i < arr.Length; i++)
            {
                data[infos[i].Name] = arr[i].ToString();
            }

            var content = JsonUtility.ToJson(data); ;
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
            var infos = Data.GetParameters();
            for (int i = 0; i < data.Count; i++)
            {
                objs[i] = data[infos[i].Name] as object;
            }
        }

        return objs;
    }
}