using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class MethodRenderBase
{
    public TypeElementRendererFactory factory;
    public abstract void RenderMethodAndParams(ScrollView selectItemViews, IMethodInfoData method, object[] parameters);

    public abstract void RenderMethod(ScrollView selectItemViews, IMethodInfoData method);

  
}