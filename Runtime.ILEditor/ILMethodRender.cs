#if !DISABLE_ILRUNTIME
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class ILMethodRender :  IMethodRender
{
    public ITypeElementRendererFactory Factory { get; set; }

    public ILMethodRender(ITypeElementRendererFactory factory)
    {
        this.Factory = factory;
    }

    public void RenderMethodAndParams(ScrollView selectItemViews, IMethodInfoData method, object[] parameters)
    {
        RenderMethodAndParams(selectItemViews,method as MethodIL,parameters);
    }

    public void RenderMethod(ScrollView selectItemViews, IMethodInfoData method)
    {
        RenderILMethod(selectItemViews, method as MethodIL);
    }

    public void RenderILMethod(ScrollView selectItemViews, MethodIL methodIL)
    {
        selectItemViews.Clear();
        List<ILRuntime.CLR.TypeSystem.IType> parameterInfos = methodIL.Data.Parameters;
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

    private void _RenderSetILMethodAndParams(ScrollView selectItemViews, MethodIL ilMethod,object[] Parameters)
    {
        selectItemViews.Clear();
        List<ILRuntime.CLR.TypeSystem.IType> parameterInfos = ilMethod.Data.Parameters;
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

    private TypeElementRenderer GetILTypeView(System.Type parameterType, string paramName)
    {
        var factory = Factory;
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
    }
}
#endif