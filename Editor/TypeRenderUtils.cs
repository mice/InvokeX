using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Intepreter;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


public static partial class TypeRenderUtils
{
    private static TypeElementRendererFactory factory = new TypeElementRendererFactory().Init();
    static TypeRenderUtils(){
        TypeElementRendererExt.factory = factory;
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

    public static void RenderMethod(ScrollView selectItemViews, MethodCLR method)
    {
        ParameterInfo[] parameterInfos = method.GetParameters();
        RenderParams(selectItemViews, parameterInfos);
    }
}