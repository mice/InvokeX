using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.UIElements;

public interface IElementRendererFactory
{
    Dictionary<Type, System.Func<System.Type, string, TypeElementRenderer>> creatorDict { get; }
    void RegisterType(Type type, System.Func<System.Type, string, TypeElementRenderer> creator);
    TypeElementRenderer GetRender(Type type, string label);
}


public interface ITypeElementRegister
{
    void Register(ITypeElementRendererFactory factory, Action<Type, System.Func<System.Type, string, TypeElementRenderer>> register);
}

public interface ITypeElementRendererFactory
{
    TypeElementRenderer GetRender(Type type, string label);
}

/// <summary>
/// 作为一个marker
/// </summary>
public interface IParamData
{

}

public interface IMethodInfoData
{
    String Name { get; }
    String TypeName { get; }
    string TargetTypeName { get; }
    int ParamCount { get; }
    ParameterInfo[] GetParameters();

    string ToJson(object[] objs);
    object[] FromJson(string stringObjs);
}


public interface IMethodRender
{
    ITypeElementRendererFactory Factory { get; set; }
    void RenderMethod(ScrollView selectItemViews, IMethodInfoData method);
    void RenderMethodAndParams(ScrollView selectItemViews, IMethodInfoData method,object[] parameters);
}