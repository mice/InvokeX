using System;
using System.Collections.Generic;
using System.Reflection;

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

public interface IMethodInfoData
{
    String Name { get; }
    int ParamCount { get; }
    ParameterInfo[] GetParameters();

    string ToJson(object[] objs);
    object[] FromJson(string stringObjs);
}