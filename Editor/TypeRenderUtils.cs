using System.Collections.Generic;
using System.Reflection;
using UnityEngine.UIElements;

public static partial class TypeRenderUtils
{
    public static void RenderParams(ScrollView selectItemViews, ParameterInfo[] parameterInfos, string methodName = "UnNamed")
    {
        selectItemViews.Clear();
        var container = new ParamRendererContainer();
        container.MethodName = methodName;
        var factory = CMDRuntimeContext.Instance.Factory;
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
}