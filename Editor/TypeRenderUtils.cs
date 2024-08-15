using System.Collections.Generic;
using System.Reflection;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;


public static partial class TypeRenderUtils
{
    private static List<ITypeElementRegister> typeElementRegisters = new List<ITypeElementRegister>();
    private static TypeElementRendererFactory factory = new TypeElementRendererFactory().Init(typeElementRegisters);
    public static void Init()
    {
        factory = new TypeElementRendererFactory()
            .Init(typeElementRegisters);
    }

    
    public static void Register(ITypeElementRegister typeElementRegister)
    {
        if (!typeElementRegisters.Contains(typeElementRegister))
        {
            typeElementRegisters.Add(typeElementRegister);
        }
        else
        {
            UnityEngine.Debug.LogWarning("Dup Add!");
        }
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

    /// <summary>
    /// TODO---------fix
    /// 先苟且一下.
    /// 难用当前type确定.最好是在ParamRendererContainer里面提供信息.
    /// </summary>
    /// <param name="selectItemViews"></param>
    /// <param name="parameterInfos"></param>
    /// <param name="Params"></param>
    public static void RenderLogParams(ScrollView selectItemViews, ParameterInfo[] parameterInfos, object[] Params) {
        var children= selectItemViews.Children();
        int i = 0;
        var dd = selectItemViews.userData as ParamRendererContainer;

        var tmpTextBaseType = typeof(TextValueField<>);
        foreach (var child in children)
        {
            var strParams = Params[i].ToString();
            var tmpType = parameterInfos[i].GetType();
            var viewType = child.GetType();
            var ddsub = dd.list[i];
               
            if(child is UnityEngine.UIElements.TextField textField)
            {
                textField.value = strParams;
            }else if (child is UnityEditor.UIElements.IntegerField intField)
            {
                int.TryParse(strParams, out var num);
                intField.value = num;
            }
            i += 1;
        }
    }

    private static void RenderCLRMethod(ScrollView selectItemViews, MethodCLR method)
    {
        ParameterInfo[] parameterInfos = method.GetParameters();
        RenderParams(selectItemViews, parameterInfos);
    }

    private static void RenderClrMethodAndParams(ScrollView selectItemViews, MethodCLR method,object[] Parameters)
    {
        ParameterInfo[] parameterInfos = method.GetParameters();
        RenderParams(selectItemViews, parameterInfos);
        RenderLogParams(selectItemViews,parameterInfos, Parameters);
    }

    public static void RenderMethod(ScrollView selectItemViews, IMethodInfoData method)
    {
        if (method is MethodCLR methodCLR)
        {
            TypeRenderUtils.RenderCLRMethod(selectItemViews, methodCLR);
        }
#if !DISABLE_ILRUNTIME
        else if (method is MethodIL methodIL)
        {

            TypeRenderUtils.RenderILParams(selectItemViews, methodIL.Data.Parameters);
        }
#endif
    }

    public static void RenderMethodAndParams(ScrollView selectItemViews, IMethodInfoData method, object[] Parameters)
    {
        if (method is MethodCLR methodCLR)
        {
            TypeRenderUtils.RenderClrMethodAndParams(selectItemViews, methodCLR, Parameters);
        }
#if !DISABLE_ILRUNTIME
        else if (method is MethodIL methodIL)
        {
            TypeRenderUtils.RenderSetILParams(selectItemViews, methodIL.Data.Parameters, Parameters);
        }
#endif
    }
}