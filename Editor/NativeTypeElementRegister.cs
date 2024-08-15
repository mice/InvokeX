using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NativeTypeElementRegister : ITypeElementRegister
{
    public static NativeTypeElementRegister Instance { get; private set; } = new NativeTypeElementRegister();
    public void Register(ITypeElementRendererFactory factory, Action<Type, System.Func<System.Type, string, TypeElementRenderer>> RegisterType)
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
