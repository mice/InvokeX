﻿using System;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// 需要处理Interface的插入.
/// </summary>
public class RuntimeCallManager:IMethodInvoker, IMethodRepository
{
    internal class RuntimeMethodTable
    {
        public Dictionary<string, MethodBase> dict = new Dictionary<string, MethodBase>();
        private HashSet<Type> types = new HashSet<Type>();

        public void RegisterType(Type type)
        {
            types.Add(type);
        }

        public static RuntimeMethodTable FromStatic(Type type)
        {
            var methodTable = new RuntimeMethodTable();

            var methods = type.GetMethods(System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            foreach (var method in methods)
            {
                //这个肯定是不行的没考虑 params
                methodTable.dict[method.Name] = method;
            }
            return methodTable;
        }

        public static RuntimeMethodTable CreateFrom(Type type)
        {
            var methodTable = new RuntimeMethodTable();

            var methods = type.GetMethods(System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            foreach (var method in methods)
            {
                //这个肯定是不行的没考虑 params
                methodTable.dict[method.Name] = method;
            }
            return methodTable;
        }

        public void Invoke(System.Object target, string v2, System.Object v3, System.Object v4)
        {
            if (dict.TryGetValue(v2, out var _method))
            {
                _method.Invoke(target, new System.Object[] { v3, v4 });
            }
        }

        public void Invoke(System.Object target, string v2, System.Object v3)
        {
            if (dict.TryGetValue(v2, out var _method))
            {
                _method.Invoke(target, new System.Object[] { v3 });
            }
        }

        public void Invoke(System.Object target, string v2, System.Object[] param)
        {
            if (dict.TryGetValue(v2, out var _method))
            {
                _method.Invoke(target, param);
            }
        }
    }

    public static RuntimeCallManager Instance { get; } = new RuntimeCallManager();
    private static Dictionary<string, (Type,System.Object)> targetCallDict = new Dictionary<string, (Type, System.Object)>();
    private static Dictionary<Type, RuntimeMethodTable> typeMethodDict = new Dictionary<Type, RuntimeMethodTable>();

    public void GetTypeDictionary(Dictionary<string, Type> typeDict)
    {
        foreach (var type in targetCallDict.Keys)
        {
            if (!typeDict.TryGetValue(type,out var _target))
            {
                typeDict[type] = targetCallDict[type].Item1;
            }
        }
    }

    public void GetMethodList(string typeName,List<IMethodInfoData> list)
    {
        var methodTable = new Dictionary<string, System.Reflection.MethodBase>();
        GetMethodDictionary(typeName, methodTable);
        foreach (var item in methodTable.Values)
        {
            list.Add(new CLRMethod(item));
        }
    }

    public void GetMethodDictionary(string typeName,Dictionary<string, MethodBase> typeDict)
    {
        if (targetCallDict.TryGetValue(typeName, out var _target) && typeMethodDict.TryGetValue(_target.Item1, out var methodTable))
        {
            foreach(var item in methodTable.dict)
            {
                typeDict[item.Key] = item.Value;
            }
        }
    }

    public void GetCollectMethodDictionary(SerDict<string, KVPair> methodDict, List<IMethodInfoData> list)
    {
        var methodTable = new Dictionary<string, MethodBase>();
        _GetCollectMethodDictionary(methodDict,methodTable);
        foreach (var item in methodTable.Values)
        {
            list.Add(new CLRMethod(item));
        }
    }
       

    private void _GetCollectMethodDictionary(SerDict<string, KVPair> methodDict, Dictionary<string, MethodBase> typeDict)
    {
        var logMethName = string.Empty;
    
        foreach (var methodItem in methodDict)
        {
            var typeName = methodItem.Value.Item1;
            if (string.Equals(typeName, "CPlayerMsgCallerProxy"))
                typeName = "Protocal";

            if (!methodItem.Value.IsIL() && targetCallDict.TryGetValue(typeName, out var _target) && typeMethodDict.TryGetValue(_target.Item1, out var methodTable))
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

    public void AddStatic(Type type)
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

    public void AddInstance(string instName,System.Object obj)
    {
        var type = obj.GetType();
        if(targetCallDict.TryGetValue(instName, out var _type))
        {
            if (type != _type.Item1)
            {
                UnityEngine.Debug.LogError("Error:::");
            }
            targetCallDict[instName] = (type, obj);
        }
        else
        {
            targetCallDict.Add(instName, (type,obj));
            typeMethodDict[type] = RuntimeMethodTable.CreateFrom(type);
        }
    }

    public void Invoke(string v1, string v2, string v3)
    {
        UnityEngine.Debug.LogError($"Call Target{v1},Func:{v2}:Params:{v3}");
        if (targetCallDict.TryGetValue(v1, out var _target) && typeMethodDict.TryGetValue(_target.Item1, out var methodTable))
        {
            methodTable.Invoke(_target.Item2, v2, v3);
        }
    }

    public void Invoke(string v1, string v2, string v3, string v4)
    {
        UnityEngine.Debug.LogError($"Call Target{v1},Func:{v2}:Params:{v3},{v4}");
        if(targetCallDict.TryGetValue(v1,out var _target) && typeMethodDict.TryGetValue(_target.Item1, out var methodTable))
        {
            methodTable.Invoke(_target.Item2, v2, v3, v4);
        }
    }

    public void Invoke(string v1, string v2, System.Object[] param)
    {
        UnityEngine.Debug.LogError($"Call Target{v1},Func:{v2}:Params:{param}");
        if (targetCallDict.TryGetValue(v1, out var _target) && typeMethodDict.TryGetValue(_target.Item1, out var methodTable))
        {
            methodTable.Invoke(_target.Item2, v2, param);
        }
    }

    public void Invoke(IMethodInfoData method)
    {
        Invoke(method.TargetTypeName, method.Name, Array.Empty<object>());
    }

    public void Invoke(IMethodInfoData method, object[] param)
    {
        Invoke(method.TargetTypeName, method.Name, param);
    }

    public MethodBase GetMethodData(string v1, string v2)
    {
        if (targetCallDict.TryGetValue(v1, out var _target) && typeMethodDict.TryGetValue(_target.Item1, out var methodTable))
        {
            return methodTable.dict[v2];
        }
        return null;
    }

    public TypeElementRenderer UnHandleType(System.Type type, Dictionary<Type, System.Func<System.Type, string, TypeElementRenderer>> creatorDict, string label)
    {
        if (type.IsPrimitive)
        {
            return null;
        }

        if (type.IsInterface)
        {
            return null;
        }

        if (type.IsSerializable)
        {
            var mapType = typeof(IParamData);
            if(creatorDict.TryGetValue(mapType, out var result))
            {
                return result.Invoke(type,label);
            }
        }
        return null;
    }
}
