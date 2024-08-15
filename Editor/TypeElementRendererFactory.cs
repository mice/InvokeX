using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class NativeTypeElementRegister : ITypeElementRegister
{
    public static NativeTypeElementRegister Instance { get; private set; } = new NativeTypeElementRegister(); 
    public void Register(ITypeElementRendererFactory factory,Action<Type, System.Func<System.Type, string, TypeElementRenderer>> RegisterType)
    {
        TypeElementRendererExt.factory = factory;
        RegisterType(typeof(sbyte), TypeElementRendererExt.ByteRenderer);
        RegisterType(typeof(byte), TypeElementRendererExt.UByteRenderer);
        RegisterType(typeof(short), TypeElementRendererExt.ShortRenderer);
        RegisterType(typeof(ushort), TypeElementRendererExt.UShortRenderer);
        RegisterType(typeof(int), TypeElementRendererExt.IntRenderer);
        RegisterType(typeof(uint), TypeElementRendererExt.UIntRenderer);
        RegisterType(typeof(long), TypeElementRendererExt.LongRenderer);
        RegisterType(typeof(ulong), TypeElementRendererExt.ULongRenderer);
        RegisterType(typeof(float), TypeElementRendererExt.FloatRenderer);
        RegisterType(typeof(double), TypeElementRendererExt.DoubleRenderer);
        RegisterType(typeof(bool), TypeElementRendererExt.BoolRenderer);
        RegisterType(typeof(string), TypeElementRendererExt.StringRenderer);

        RegisterType(typeof(UnityEngine.Color), TypeElementRendererExt.ColorRenderer);
        RegisterType(typeof(UnityEngine.Vector2), TypeElementRendererExt.Vec2Renderer);
        RegisterType(typeof(UnityEngine.Vector2Int), TypeElementRendererExt.Vec2IntRenderer);
        RegisterType(typeof(UnityEngine.Vector3), TypeElementRendererExt.Vec3Renderer);
        RegisterType(typeof(UnityEngine.Vector3Int), TypeElementRendererExt.Vec3IntRenderer);
        RegisterType(typeof(UnityEngine.Vector4), TypeElementRendererExt.Vec4Renderer);
        RegisterType(typeof(UnityEngine.Rect), TypeElementRendererExt.RectRenderer);
        RegisterType(typeof(UnityEngine.RectInt), TypeElementRendererExt.RectIntRenderer);

        RegisterType(typeof(System.Array), TypeElementRendererExt.ArrayRenderer);
        RegisterType(typeof(System.Collections.Generic.List<>), TypeElementRendererExt.ListRenderer);
        RegisterType(typeof(UnityEngine.Object), TypeElementRendererExt.UObjectRenderer);
        RegisterType(typeof(IParamData), TypeElementRendererExt.IParamDataRenderer);
    }
}


public class TypeElementRendererFactory:ITypeElementRendererFactory
{
    private Dictionary<Type, System.Func<System.Type, string, TypeElementRenderer>> creatorDict = new Dictionary<Type, Func<System.Type, string, TypeElementRenderer>>();
    private List<ITypeElementRegister> typeElementRegisters = new List<ITypeElementRegister>();
    public TypeElementRendererFactory Register(ITypeElementRegister typeElementRegister)
    {
        if (!typeElementRegisters.Contains(typeElementRegister))
        {
            typeElementRegisters.Add(typeElementRegister);
        }
        else
        {
            UnityEngine.Debug.LogWarning("Dup Add!");
        }
        return this;
    }

    public TypeElementRendererFactory Init()
    {
        if (typeElementRegisters.Count == 0)
        {
            typeElementRegisters.Add(NativeTypeElementRegister.Instance);
        }
        foreach (var typeElementRegister in typeElementRegisters)
        {
            typeElementRegister.Register(this, RegisterType);
        }

#if !DISABLE_ILRUNTIME
        RegisterType(typeof(ILRuntime.Runtime.Intepreter.ILTypeInstance), TypeElementRendererExt.ILTypeRender);
#endif
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
#if !DISABLE_ILRUNTIME
        if (type.UnderlyingSystemType == typeof(ILRuntime.Runtime.Intepreter.ILTypeInstance))
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
#endif
        return null;
    }
}

