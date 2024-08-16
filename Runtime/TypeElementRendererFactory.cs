using System;
using System.Collections.Generic;

public class TypeElementRendererFactory:ITypeElementRendererFactory
{
    private Dictionary<Type, System.Func<System.Type, string, TypeElementRenderer>> creatorDict = new Dictionary<Type, Func<System.Type, string, TypeElementRenderer>>();

    public TypeElementRendererFactory Init(List<ITypeElementRegister> typeElementRegisters)
    {
        foreach (var typeElementRegister in typeElementRegisters)
        {
            typeElementRegister.Register(this, RegisterType);
        }
        return this;
    }

    public void RegisterType(Type type, System.Func<System.Type, string, TypeElementRenderer> creator)
    {
        creatorDict[type] = creator;
    }

    public TypeElementRenderer GetRender(Type type, string label)
    {
        if (creatorDict.TryGetValue(type, out var creator))
        {
            return creator.Invoke(type, label);
        }

        if (type.IsArray)
        {
            var subType = type.GetElementType();
            var arrayType = typeof(System.Array);
            if (!subType.IsArray) //多维数组,不支持
            {
                if (creatorDict.TryGetValue(arrayType, out var arrayCreator))
                {
                    return arrayCreator.Invoke(type, label);
                }
                else
                {
                    UnityEngine.Debug.LogError($"OnlySupport:Array[T];{type}:{type.FullName}:tmpType:{subType};;;;{subType} == {type}");
                }
            }
        }

        if (type.IsGenericType)
        {
            var subType = type.GetGenericArguments()[0];
            var genericListType = typeof(System.Collections.Generic.List<>);
            var targetType = genericListType.MakeGenericType(subType);
            if (type.FullName.StartsWith("System.Collections.Generic.List`1"))
            {
                if (creatorDict.TryGetValue(genericListType, out var arrayCreator))
                {
                    return arrayCreator.Invoke(type, label);
                }
                else
                {
                    UnityEngine.Debug.LogError($"OnlySupport:List<T>;{type}:{type.FullName}:tmpType:{genericListType};;;;{targetType} == {type}");
                }
            }
            else
            {
                UnityEngine.Debug.LogError($"OnlySupport:List<T>;{type}:{type.FullName}:tmpType:{genericListType};;;;{targetType} == {type}");
            }
        }

        var uObjectType = typeof(UnityEngine.Object);
        if (type.IsAssignableFrom(uObjectType))
        {
            if (creatorDict.TryGetValue(uObjectType, out var uObjCreator))
            {
                return uObjCreator.Invoke(type, label);
            }
            return null;
        }

        if (typeof(IParamData).IsAssignableFrom(type))
        {
            if (creatorDict.TryGetValue(typeof(IParamData), out var uObjCreator))
            {
                return uObjCreator.Invoke(type, label);
            }
        }

        var context = CMDRuntimeContext.Instance;
        return context.UnHandleType(type, creatorDict,label);
    }
}

