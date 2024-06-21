#if !DISABLE_ILRUNTIME
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public static partial class TypeRenderUtils
{
    public static void RenderILParams(ScrollView selectItemViews, List<ILRuntime.CLR.TypeSystem.IType> parameterInfos)
    {
        selectItemViews.Clear();
        var container = new ParamRendererContainer();
        for (int i = 0; i < parameterInfos.Count; i++)
        {
            var info = parameterInfos[i];
            var renderer = GetILTypeView(info.ReflectionType, info.Name);
            if (renderer != null)
            {
                selectItemViews.Add(renderer.element);
            }
            else
            {
                UnityEngine.Debug.LogError($"No Render Found:{info.ReflectionType}:name:{info.Name}");
            }
            container.list.Add(renderer);
        }
        selectItemViews.userData = container;
    }

    public static void RenderSetILParams(ScrollView selectItemViews, List<ILRuntime.CLR.TypeSystem.IType> parameterInfos,object[] Parameters)
    {
        selectItemViews.Clear();
        var container = new ParamRendererContainer();
        for (int i = 0; i < parameterInfos.Count; i++)
        {
            var info = parameterInfos[i];
            var renderer = GetILTypeView(info.ReflectionType, info.Name);
            var strParams = Parameters[i].ToString();
            int num = 0;
            if (int.TryParse(strParams, out num) && renderer.element is IntegerField)
            {
                var intField = renderer.element as IntegerField;
                intField.value = num;
            }
            else
            {
                var textField = renderer.element as TextField;
                textField.value = strParams;
            }

            if (renderer != null)
            {
                selectItemViews.Add(renderer.element);
            }
            else
            {
                UnityEngine.Debug.LogError($"No Render Found:{info.ReflectionType}:name:{info.Name}");
            }
            container.list.Add(renderer);
        }
        selectItemViews.userData = container;
    }

    private static TypeElementRenderer GetILTypeView(System.Type parameterType, string paramName)
    {
        if (parameterType.UnderlyingSystemType == typeof(ILRuntime.Runtime.Intepreter.ILTypeInstance))
        {
            var renderer = factory.GetRender(parameterType, paramName);
            if (renderer == null)
            {
                UnityEngine.Debug.LogError($"No Render Found ILTypeInstance:{parameterType}:name:{paramName}");
            }
            return renderer;
        }
        else if (parameterType != parameterType.UnderlyingSystemType)
        {
            var renderer = factory.GetRender(parameterType.UnderlyingSystemType, paramName);
            return renderer;
        }
        else
        {
            var renderer = factory.GetRender(parameterType, paramName);
            return renderer;
        }
        //UnityEngine.Debug.LogError($"No Render Found ILType:{parameterType}:name:{paramName}");
    }
}
#endif