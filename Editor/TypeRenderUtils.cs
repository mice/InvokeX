using ILRuntime.CLR.Method;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEditor.UIElements;
using UnityEngine.UIElements;


public static partial class TypeRenderUtils
{
    private static List<ITypeElementRegister> typeElementRegisters = new List<ITypeElementRegister>();
    public static TypeElementRendererFactory factory = new TypeElementRendererFactory().Init(typeElementRegisters);
    private static Dictionary<System.Type, IMethodRender> methodRenderDict = new Dictionary<System.Type, IMethodRender>();
    public static void Init()
    {
        factory = new TypeElementRendererFactory()
            .Init(typeElementRegisters);

        var clrMethodRender = new CLRMethodRender(factory);

        methodRenderDict[typeof(MethodCLR)] = clrMethodRender;
        foreach(var item in methodRenderDict.Values)
        {
            item.Factory = factory;
        }
    }


    public static void Register<T>(ITypeElementRegister typeElementRegister, IMethodRender methodRender) where T:IMethodInfoData
    {
        if (!typeElementRegisters.Contains(typeElementRegister))
        {
            typeElementRegisters.Add(typeElementRegister);
        }
        else
        {
            UnityEngine.Debug.LogWarning("Dup Add!");
        }

        methodRenderDict[typeof(MethodCLR)] = methodRender;
    }

    public static void RenderParams(ScrollView selectItemViews, ParameterInfo[] parameterInfos,string methodName = "UnNamed")
    {
        selectItemViews.Clear();
        var container = new ParamRendererContainer();
        container.MethodName = methodName;
        for (int i = 0; i < parameterInfos.Length; i++)
        {
            var info = parameterInfos[i];
            var paramAttr = info.GetCustomAttribute<ParamAttribute>();
            var renderer = factory.GetRender(info.ParameterType, info.Name);
            if (renderer != null)
            {
                selectItemViews.Add(renderer.element);
            }
            else
            {
                UnityEngine.Debug.LogError($"No Render Found:{info.ParameterType}:name:{info.Name}");
            }
            container.list.Add(renderer);
        }

        selectItemViews.userData = container;
    }


    public static void RenderMethod(ScrollView selectItemViews, IMethodInfoData method)
    {
        if(methodRenderDict.TryGetValue(method.GetType(),out var render))
        {
            render.RenderMethod(selectItemViews, method);
        }
    }

    public static void RenderMethodAndParams(ScrollView selectItemViews, IMethodInfoData method, object[] parameters)
    {
        if (methodRenderDict.TryGetValue(method.GetType(), out var render))
        {
            render.RenderMethodAndParams(selectItemViews, method, parameters);
        }
    }
}