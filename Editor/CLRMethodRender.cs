using System.Reflection;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class CLRMethodRender : IMethodRender
{
    public ITypeElementRendererFactory Factory { get; set; }
    private ITypeElementRendererFactory factory => Factory;
    public CLRMethodRender(ITypeElementRendererFactory factory)
    {
        this.Factory = factory;
    }
    public void RenderMethod(ScrollView selectItemViews, IMethodInfoData method)
    {
        ParameterInfo[] parameterInfos = method.GetParameters();
        TypeRenderUtils.RenderParams(selectItemViews, parameterInfos);
    }

    public void RenderMethodAndParams(ScrollView selectItemViews, IMethodInfoData method, object[] parameters)
    {
        ParameterInfo[] parameterInfos = method.GetParameters();
        TypeRenderUtils.RenderParams(selectItemViews, parameterInfos);
        RenderLogParams(selectItemViews, parameterInfos, parameters);
    }


    /// <summary>
    /// TODO---------fix
    /// 先苟且一下.
    /// 难用当前type确定.最好是在ParamRendererContainer里面提供信息.
    /// </summary>
    /// <param name="selectItemViews"></param>
    /// <param name="parameterInfos"></param>
    /// <param name="Params"></param>
    private void RenderLogParams(ScrollView selectItemViews, ParameterInfo[] parameterInfos, object[] Params)
    {
        var children = selectItemViews.Children();
        int i = 0;
        var dd = selectItemViews.userData as ParamRendererContainer;

        var tmpTextBaseType = typeof(TextValueField<>);
        foreach (var child in children)
        {
            var strParams = Params[i].ToString();
            var tmpType = parameterInfos[i].GetType();
            var viewType = child.GetType();
            var ddsub = dd.list[i];

            if (child is UnityEngine.UIElements.TextField textField)
            {
                textField.value = strParams;
            }
            else if (child is UnityEditor.UIElements.IntegerField intField)
            {
                int.TryParse(strParams, out var num);
                intField.value = num;
            }
            i += 1;
        }
    }
}
