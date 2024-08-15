#if !DISABLE_ILRUNTIME
using System.Collections.Generic;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using UnityEngine;

public class ILRuntimeCallManager
{
    internal class RuntimeMethodTable
    {
        public Dictionary<string, IMethod> dict = new Dictionary<string, IMethod>();

        internal static RuntimeMethodTable FromStatic(ILType type)
        {
            var methodTable = new RuntimeMethodTable();

            var methods = type.GetMethods();
            foreach (var method in methods)
            {
                //这个肯定是不行的没考虑 params
                methodTable.dict[method.Name] = method;
            }
            return methodTable;
        }

        public static RuntimeMethodTable CreateFrom(ILType type)
        {
            var methodTable = new RuntimeMethodTable();

            var methods = type.GetMethods();
            foreach (var method in methods)
            {
                //这个肯定是不行的没考虑 params
                methodTable.dict[method.Name] = method;
            }
            return methodTable;
        }

        public void Invoke(AppDomain appDomain, System.Object target, string v2, System.Object v3, System.Object v4)
        {
            if (dict.TryGetValue(v2, out var _method))
            {
                appDomain.Invoke(_method, target, new System.Object[] { v3, v4 });
            }
        }

        public void Invoke(AppDomain appDomain,System.Object target, string v2, System.Object v3)
        {
            if (dict.TryGetValue(v2, out var _method))
            {
                appDomain.Invoke(_method,target, new System.Object[] { v3 });
            }
        }

        public void Invoke(AppDomain appDomain, System.Object target, string v2)
        {
            if (dict.TryGetValue(v2, out var _method))
            {
                appDomain.Invoke(_method, target, System.Array.Empty<object>());
            }
        }

        public void Invoke(AppDomain appDomain, System.Object target, string v2, System.Object[] param)
        {
            if (dict.TryGetValue(v2, out var _method))
            {
                appDomain.Invoke(_method, target, param);
            }
        }
    }

    public static ILRuntimeCallManager Instance { get; } = new ILRuntimeCallManager();
    private static Dictionary<string, (ILType, System.Object)> targetCallDict = new Dictionary<string, (ILType, System.Object)>();
    private static Dictionary<ILType, RuntimeMethodTable> typeMethodDict = new Dictionary<ILType, RuntimeMethodTable>();
    public static AppDomain AppDomain => appDomain;
    private static AppDomain appDomain;
    public void SetDomain(AppDomain appDomain_)
    {
        appDomain = appDomain_;
    }

    public void AddStatic(ILType type)
    {
        var instName = type.Name;
        if (targetCallDict.TryGetValue(instName, out var _type))
        {
            if (type != _type.Item1)
            {
                UnityEngine.Debug.LogError("Error:::");
            }
            targetCallDict[instName] = (type, null);
        }
        else
        {
            targetCallDict.Add(instName, (type, null));
            typeMethodDict[type] = RuntimeMethodTable.FromStatic(type);
        }
    }

    public void AddInstance(string instName, ILTypeInstance obj)
    {
        var type = obj.Type;
        if (targetCallDict.TryGetValue(instName, out var _type))
        {
            if (type != _type.Item1)
            {
                UnityEngine.Debug.LogError("Error:::");
            }
            targetCallDict[instName] = (type, obj);
        }
        else
        {
            targetCallDict.Add(instName, (type, obj));
            typeMethodDict[type] = RuntimeMethodTable.CreateFrom(type);
        }
    }

    public void GetMethodDictionary(string typeName,Dictionary<string,IMethod> methodDict)
    {
        if (targetCallDict.TryGetValue(typeName, out var _target) && typeMethodDict.TryGetValue(_target.Item1, out var methodTable))
        {
            foreach(var methodPair in methodTable.dict)
            {
                methodDict[methodPair.Key] = methodPair.Value;
            }
        }
    }

    public void GetCollectMethodDictionary(SerDict<string, KVPair> methodDict, List<IMethodInfoData> list)
    {
        var ilMethodTable = new Dictionary<string, ILRuntime.CLR.Method.IMethod>();
        _GetCollectMethodDictionary(methodDict, ilMethodTable);

        foreach (var item in ilMethodTable.Values)
        {
            list.Add(new MethodIL(item));
        }
    }

    private void _GetCollectMethodDictionary(SerDict<string, KVPair> methodDict, Dictionary<string, IMethod> typeDict)
    {
        var logMethName = string.Empty;

        foreach (var methodItem in methodDict)
        {
            var typeName = methodItem.Value.Item1;
            if (string.Equals(typeName, "CPlayerMsgCallerProxy"))
                typeName = "Protocal";

            if (methodItem.Value.IsIL()&& targetCallDict.TryGetValue(typeName, out var _target) && typeMethodDict.TryGetValue(_target.Item1, out var methodTable))
            {
                var methodName = methodItem.Key;
                var methodTableDict = methodTable.dict;
                if (methodTableDict.ContainsKey(methodName))
                {
                    typeDict[methodName] = methodTableDict[methodName];
                }
                else
                {
                    UnityEngine.Debug.LogError($"methodTableDict  not Contains{methodName}");
                    logMethName = methodName;
                    var collectCallManager = CollectCallManager.Instance;
                    collectCallManager.DeleteCollectMethod(logMethName);
                }
            }
        }
    }

    public void Invoke(string typeName,string methodName)
    {
        if (targetCallDict.TryGetValue(typeName, out var _target) && typeMethodDict.TryGetValue(_target.Item1, out var methodTable))
        {
            methodTable.Invoke(appDomain, _target.Item2, methodName);
        }
    }

    public void Invoke(string typeName,string methodName, object[] paramArr)
    {
        if (targetCallDict.TryGetValue(typeName, out var _target) && typeMethodDict.TryGetValue(_target.Item1, out var methodTable))
        {
            methodTable.Invoke(appDomain, _target.Item2, methodName, paramArr);
        }
    }
}
#endif