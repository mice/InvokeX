

using System;
using System.Collections.Generic;


public class RuntimeContext
{
    private static RuntimeContext Instance_;
    private Dictionary<string, IMethodRepository> DelegationDict = new Dictionary<string, IMethodRepository>();
 
    public static RuntimeContext Instance
    {
        get
        {
            if (Instance_ != null)
            {
                return Instance_;
            }
            Instance_ = new RuntimeContext();
            return Instance_;
        }
    }

    public void Register<T>(ITypeElementRegister typeElementRegister, IMethodRender methodRender,
        IMethodRepository invoker
        ) where T : IMethodInfoData
    {
        TypeRenderUtils.Register<T>(NativeTypeElementRegister.Instance, new CLRMethodRender(null));
        var tmpType = typeof(T).Name;
        if (!DelegationDict.ContainsKey(tmpType))
        {
            DelegationDict.Add(tmpType, invoker);
        }
        else
        {
            UnityEngine.Debug.LogWarning("Dup Add!");
        }
    }

    public void Init()
    {
        TypeRenderUtils.Init();
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
}
