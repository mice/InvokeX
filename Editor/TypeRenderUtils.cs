using System.Collections.Generic;
using System.Reflection;
using UnityEngine.UIElements;


public static partial class TypeRenderUtils
{
    private static List<ITypeElementRegister> typeElementRegisters = new List<ITypeElementRegister>();
    public static TypeElementRendererFactory factory => RuntimeContext.Instance.Factory;
   

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
}