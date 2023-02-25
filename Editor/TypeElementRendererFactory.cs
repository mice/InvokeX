using System;
using System.Collections.Generic;

public class TypeElementRendererFactory
{
    private Dictionary<Type, System.Func<System.Type, string, TypeElementRenderer>> creatorDict = new Dictionary<Type, Func<System.Type, string, TypeElementRenderer>>();


    public TypeElementRendererFactory Init()
    {
        RegisterType(typeof(int), TypeElementRenderer.IntRenderer);
        RegisterType(typeof(uint), TypeElementRenderer.UIntRenderer);
        RegisterType(typeof(long), TypeElementRenderer.LongRenderer);
        RegisterType(typeof(ulong), TypeElementRenderer.ULongRenderer);
        RegisterType(typeof(float), TypeElementRenderer.FloatRenderer);
        RegisterType(typeof(double), TypeElementRenderer.DoubleRenderer);
        RegisterType(typeof(string), TypeElementRenderer.StringRenderer);
        RegisterType(typeof(UnityEngine.Color), TypeElementRenderer.ColorRenderer);
        RegisterType(typeof(UnityEngine.Vector2), TypeElementRenderer.Vec2Renderer);
        RegisterType(typeof(UnityEngine.Vector3), TypeElementRenderer.Vec3Renderer);
        RegisterType(typeof(UnityEngine.Vector4), TypeElementRenderer.Vec4Renderer);
        RegisterType(typeof(System.Array), TypeElementRenderer.ArrayRenderer);
        RegisterType(typeof(System.Collections.Generic.List<>), TypeElementRenderer.ListRenderer);
        RegisterType(typeof(UnityEngine.Object), TypeElementRenderer.UObjectRenderer);
        RegisterType(typeof(IParamData), TypeElementRenderer.IParamDataRenderer);

        RegisterType(typeof(ILRuntime.Runtime.Intepreter.ILTypeInstance), TypeElementRenderer.ILTypeRender);
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

        //---
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
            }
        }

        if (type.IsGenericType)
        {
            var subType = type.GetGenericArguments()[0];
            var genericListType = typeof(System.Collections.Generic.List<>);
            var targetType = genericListType.MakeGenericType(subType);
            if(type == targetType)
            {
                if (creatorDict.TryGetValue(genericListType, out var arrayCreator))
                {
                    return arrayCreator.Invoke(type, label);
                }
            }
            UnityEngine.Debug.LogError($"XXX;{type}:tmpType:{genericListType};;;;{targetType} == {type}");
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

        if(type.UnderlyingSystemType == typeof(ILRuntime.Runtime.Intepreter.ILTypeInstance))
        {
            if (creatorDict.TryGetValue(typeof(ILRuntime.Runtime.Intepreter.ILTypeInstance), out var uObjCreator))
            {
                return uObjCreator.Invoke(type, label);
            }
        }

        var ilTypeType = typeof(ILRuntime.CLR.TypeSystem.ILType);
        var ilRuntimeTypeType = typeof(ILRuntime.Reflection.ILRuntimeType);
        var ilInstType = typeof(ILRuntime.Runtime.Intepreter.ILTypeInstance);
        UnityEngine.Debug.LogError($"type:{type},ilType:{ilTypeType},ilRuntimeType:{ilRuntimeTypeType},ilInstType:{ilInstType}");
        return null;
    }
}

