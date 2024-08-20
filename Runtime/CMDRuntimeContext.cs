

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.UIElements;

public class CMDRuntimeContext
{
    private static CMDRuntimeContext Instance_;
    private Dictionary<string, IMethodRepository> DelegationDict = new Dictionary<string, IMethodRepository>();
    private List<ITypeElementRegister> typeElementRegisters = new List<ITypeElementRegister>();
    private Dictionary<Type, IMethodRender> methodRenderDict = new Dictionary<Type, IMethodRender>();
    private Dictionary<Type, FieldInfo[]> typeFieldDict = new Dictionary<Type, FieldInfo[]>();
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

    private FieldInfo[] GetOrCreate(Type type)
    {
        if(!typeFieldDict.TryGetValue(type,out var fields))
        {
             fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
            typeFieldDict.Add(type, fields);
        }

        return fields;
    }

    //绘制复炸的对象.
    public void RenderType(ScrollView selectItemViews, object target)
    {
        selectItemViews.Clear();
        if (target ==null)
        {
            selectItemViews.userData = null;
            return;
        }
        var button = new Button();
        button.text = "Save";
       
        selectItemViews.Add(button);
        var container = new ParamRendererContainer();
        var tmpType = target.GetType();
        container.MethodName = target.GetType().Name;
        var fields = GetOrCreate(tmpType);
        var factory = Factory;
        for (int i = 0; i < fields.Length; i++)
        {
            var info = fields[i];
            var renderer = factory.GetRender(info.FieldType, info.Name);
            if (renderer != null)
            {
                try
                {
                    renderer.SetValueAction?.Invoke(info.GetValue(target));
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogError($"Not OK:info:{info},:fieldName:{info.Name}:{ex}");
                }
               
                selectItemViews.Add(renderer.element);
            }
            else
            {
                UnityEngine.Debug.LogError($"No Render Found:{info.FieldType}:name:{info.Name}");
            }
            container.list.Add(renderer);
        }

        button.clicked += () =>
        {
            for (int i = 0; i < fields.Length; i++)
            {
                var info = fields[i];
                var renderer = container.list[i];
                info.SetValue(target, renderer.ToValueFunc(renderer));
            }
        };
        selectItemViews.userData = container;
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

    public TypeElementRenderer UnHandleType(System.Type type, Dictionary<System.Type, System.Func<System.Type, string, TypeElementRenderer>> creatorDict, string label)
    {
        foreach (var item in DelegationDict.Values)
        {
            var render = item.UnHandleType(type, creatorDict, label);
            if(render!= null)
            {
                return render;
            }
        }
        return null;
    }
}
