

using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class CMDRuntimeContext
{
    private static CMDRuntimeContext Instance_;
    private Dictionary<string, IMethodRepository> DelegationDict = new Dictionary<string, IMethodRepository>();
    private List<ITypeElementRegister> typeElementRegisters = new List<ITypeElementRegister>();
    private Dictionary<Type, IMethodRender> methodRenderDict = new Dictionary<Type, IMethodRender>();
    public TypeElementRendererFactory Factory { get; private set; }
    public static CMDRuntimeContext Instance
    {
        get
        {
            if (Instance_ != null)
            {
                return Instance_;
            }
            Instance_ = new CMDRuntimeContext();
            return Instance_;
        }
    }

    public void Register<T>(ITypeElementRegister typeElementRegister, IMethodRender methodRender,
        IMethodRepository invoker
        ) where T : IMethodInfoData
    {
        typeElementRegisters.Add(typeElementRegister);
        methodRenderDict[typeof(T)] = methodRender;
        var tmpType = typeof(T).Name;
        if (!DelegationDict.ContainsKey(tmpType))
        {
            DelegationDict.Add(tmpType, invoker);
        }
    }

    public void Init()
    {
        Factory = new TypeElementRendererFactory()
            .Init(typeElementRegisters);
       
        foreach (var item in methodRenderDict.Values)
        {
            item.Factory = Factory;
        }
    }

    public void GetMethodList(string methodType,string tab, List<IMethodInfoData> list) 
    {
        var typeName = methodType;
        if(DelegationDict.TryGetValue(typeName,out var repo))
        {
            repo.GetMethodList(tab, list);
        }
     
    }

    public void GetCollectMethodDictionary(SerDict<string, KVPair> methodDict, List<IMethodInfoData> list)
    {
        foreach(var val in DelegationDict.Values)
        {
            val.GetCollectMethodDictionary(methodDict, list);
        }
    }

    public void Invoke(IMethodInfoData method)
    {
        if(DelegationDict.TryGetValue(method.TypeName,out var invoker))
        {
            invoker.Invoke(method);
        }
    }

    public void Invoke(IMethodInfoData method, object[] param)
    {
        if (DelegationDict.TryGetValue(method.TypeName, out var invoker))
        {
            invoker.Invoke(method, param);
        }
    }


    public void RenderMethod(ScrollView selectItemViews, IMethodInfoData method)
    {
        if (methodRenderDict.TryGetValue(method.GetType(), out var render))
        {
            render.RenderMethod(selectItemViews, method);
        }
    }

    public void RenderMethodAndParams(ScrollView selectItemViews, IMethodInfoData method, object[] parameters)
    {
        if (methodRenderDict.TryGetValue(method.GetType(), out var render))
        {
            render.RenderMethodAndParams(selectItemViews, method, parameters);
        }
    }
}
