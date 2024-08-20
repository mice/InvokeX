using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using XInspect;



public class NativeTypeElementRegister : ITypeElementRegister
{
    public static NativeTypeElementRegister Instance { get; private set; } = new NativeTypeElementRegister();
    public void Register(ITypeElementRendererFactory factory, Action<Type, System.Func<System.Type, string, TypeElementRenderer>> RegisterType)
    {
        NativeTypeElementRegister.factory = factory;
        RegisterType(typeof(sbyte), ByteRenderer);
        RegisterType(typeof(byte), UByteRenderer);
        RegisterType(typeof(short), ShortRenderer);
        RegisterType(typeof(ushort), UShortRenderer);
        RegisterType(typeof(int), IntRenderer);
        RegisterType(typeof(uint), UIntRenderer);
        RegisterType(typeof(long), LongRenderer);
        RegisterType(typeof(ulong), ULongRenderer);
        RegisterType(typeof(float), FloatRenderer);
        RegisterType(typeof(double), DoubleRenderer);


        RegisterType(typeof(bool), BoolRenderer);
        RegisterType(typeof(string), StringRenderer);

        RegisterType(typeof(UnityEngine.Color), ColorRenderer);
        RegisterType(typeof(UnityEngine.Vector2), Vec2Renderer);
        RegisterType(typeof(UnityEngine.Vector2Int), Vec2IntRenderer);
        RegisterType(typeof(UnityEngine.Vector3), Vec3Renderer);
        RegisterType(typeof(UnityEngine.Vector3Int), Vec3IntRenderer);
        RegisterType(typeof(UnityEngine.Vector4), Vec4Renderer);
        RegisterType(typeof(UnityEngine.Rect), RectRenderer);
        RegisterType(typeof(UnityEngine.RectInt), RectIntRenderer);

        RegisterType(typeof(System.Array), ArrayRenderer);
        RegisterType(typeof(System.Enum), EnumRenderer);
        RegisterType(typeof(System.Collections.Generic.List<>), ListRenderer);
        RegisterType(typeof(UnityEngine.Object), UObjectRenderer);
        RegisterType(typeof(IParamData), IParamDataRenderer);
    }

    private static Regex intDigit = new Regex("^[0-9]+$",RegexOptions.Singleline);
    public static TypeElementRenderer ByteRenderer(System.Type targetType, string paramName)
    {
        var renderer = new TypeElementRenderer();
        renderer.type = typeof(sbyte);
        var tField = new IntXField(paramName);
        renderer.element = tField;

        renderer.ToValueFunc = (r) =>
        {
            int.TryParse(tField.value, out var sValue);
            return System.Convert.ToSByte(sValue);
        };

        renderer.SetValueAction = (obj) =>
        {
            tField.value = System.Convert.ToInt32(obj).ToString();
        };

        return renderer;
    }

    public static TypeElementRenderer UByteRenderer(System.Type targetType, string paramName)
    {
        var renderer = new TypeElementRenderer();
        renderer.type = typeof(byte);
        var tField = new IntegerField(paramName);
        renderer.element = tField;

        renderer.ToValueFunc = (r) =>
        {
            return (byte)tField.value;
        };
        renderer.SetValueAction = (obj) =>
        {
            tField.value = System.Convert.ToInt32(obj);
        };
        return renderer;
    }

    public static TypeElementRenderer ShortRenderer(System.Type targetType, string paramName)
    {
        var renderer = new TypeElementRenderer();
        renderer.type = typeof(short);
        var tField = new IntegerField(paramName);
        renderer.element = tField;
        renderer.ToValueFunc = (r) =>
        {
            return (short)tField.value;
        };

        renderer.SetValueAction = (obj) =>
        {
            tField.value = System.Convert.ToInt32(obj);
        };
        return renderer;
    }

    public static TypeElementRenderer UShortRenderer(System.Type targetType, string paramName)
    {
        var renderer = new TypeElementRenderer();
        renderer.type = typeof(ushort);
        var tField = new IntegerField(paramName);
        renderer.element = tField;
        renderer.ToValueFunc = (r) =>
        {
            return (ushort)tField.value;
        };
        renderer.SetValueAction = (obj) =>
        {
            tField.value = System.Convert.ToInt32(obj);
        };
        return renderer;
    }

    public static TypeElementRenderer IntRenderer(System.Type targetType, string paramName)
    {
        var renderer = new TypeElementRenderer();
        renderer.type = typeof(int);
        var fieldView = new IntXField(paramName);
        renderer.element = fieldView;
        renderer.ToValueFunc = (r) =>
        {
            int.TryParse(fieldView.value, out var intValue);
            return intValue;
        };

        renderer.SetValueAction = (obj) =>
        {
            fieldView.value = System.Convert.ToInt32(obj).ToString();
        };
        return renderer;
    }

    public static TypeElementRenderer UIntRenderer(System.Type targetType, string paramName)
    {
        var renderer = new TypeElementRenderer();
        renderer.type = typeof(uint);
        var fieldView = new IntegerField(paramName);
        renderer.element = fieldView;
        renderer.ToValueFunc = (r) =>
        {
            return (uint)fieldView.value;
        };
        renderer.SetValueAction = (obj) =>
        {
            fieldView.value = System.Convert.ToInt32(obj);
        };
        return renderer;
    }

    public static TypeElementRenderer LongRenderer(System.Type targetType, string paramName)
    {
        var renderer = new TypeElementRenderer();
        renderer.type = typeof(long);
        var fieldView = new LongField(paramName);
        renderer.element = fieldView;
        renderer.ToValueFunc = (r) =>
        {
            return fieldView.value;
        };

        renderer.SetValueAction = (obj) =>
        {
            fieldView.value = System.Convert.ToInt64(obj);
        };
        return renderer;
    }

    public static TypeElementRenderer ULongRenderer(System.Type targetType, string paramName)
    {
        var renderer = new TypeElementRenderer();
        renderer.type = typeof(ulong);
        var fieldView = new LongField(paramName);
        renderer.element = fieldView;
        renderer.ToValueFunc = (r) =>
        {
            return (ulong)fieldView.value;
        };

        renderer.SetValueAction = (obj) =>
        {
            fieldView.value = System.Convert.ToInt64(obj);
        };
        return renderer;
    }

    public static TypeElementRenderer FloatRenderer(System.Type targetType, string paramName)
    {
        var renderer = new TypeElementRenderer();
        renderer.type = typeof(float);
        var fieldView = new FloatField(paramName);
        renderer.element = fieldView;
        renderer.ToValueFunc = (r) =>
        {
            return (float)fieldView.value;
        };
        renderer.SetValueAction = (obj) =>
        {
            fieldView.value = System.Convert.ToSingle(obj);
        };
        return renderer;
    }

    public static TypeElementRenderer DoubleRenderer(System.Type targetType, string paramName)
    {
        var renderer = new TypeElementRenderer();
        renderer.type = typeof(Double);
        var fieldView = new DoubleField(paramName);
        renderer.element = fieldView;
        renderer.ToValueFunc = (r) =>
        {
            return fieldView.value;
        };
        renderer.SetValueAction = (obj) =>
        {
            fieldView.value = System.Convert.ToDouble(obj);
        };
        return renderer;
    }

    public static TypeElementRenderer BoolRenderer(System.Type targetType, string paramName)
    {
        var renderer = new TypeElementRenderer();
        renderer.type = typeof(bool);
        var fieldView = new Toggle(paramName);
        renderer.element = fieldView;
        renderer.ToValueFunc = (r) =>
        {
            return fieldView.value;
        };
        renderer.SetValueAction = (obj) =>
        {
            fieldView.value = System.Convert.ToBoolean(obj);
        };
        return renderer;
    }

    public static TypeElementRenderer StringRenderer(System.Type targetType, string paramName)
    {
        var renderer = new TypeElementRenderer();
        renderer.type = typeof(string);
        var fieldView = new TextField(paramName);
        renderer.element = fieldView;
        renderer.ToValueFunc = (r) =>
        {
            return fieldView.text;
        };

        renderer.SetValueAction = (obj) =>
        {
            fieldView.value = System.Convert.ToString(obj);
        };
        return renderer;
    }

    public static TypeElementRenderer ColorRenderer(System.Type targetType, string paramName)
    {
        var renderer = new TypeElementRenderer();
        renderer.type = typeof(Color);
        var fieldView = new ColorField(paramName);
        renderer.element = fieldView;
        renderer.ToValueFunc = (r) =>
        {
            return fieldView.value;
        };
        renderer.SetValueAction = (obj) =>
        {
            fieldView.value = (Color)obj;
        };
        return renderer;
    }

    public static TypeElementRenderer Vec2Renderer(System.Type targetType, string paramName)
    {
        var renderer = new TypeElementRenderer();
        renderer.type = typeof(Vector2);
        var fieldView = new Vector2Field(paramName);
        renderer.element = fieldView;
        renderer.ToValueFunc = (r) =>
        {
            return fieldView.value;
        };

        renderer.SetValueAction = (obj) =>
        {
            fieldView.value = (Vector2)obj;
        };
        return renderer;
    }

    public static TypeElementRenderer Vec3Renderer(System.Type targetType, string paramName)
    {
        var renderer = new TypeElementRenderer();
        renderer.type = typeof(Vector3);
        var fieldView = new Vector3Field(paramName);
        renderer.element = fieldView;
        renderer.ToValueFunc = (r) =>
        {
            return fieldView.value;
        };

        renderer.SetValueAction = (obj) =>
        {
            fieldView.value = (Vector3)obj;
        };
        return renderer;
    }

    public static TypeElementRenderer Vec4Renderer(System.Type targetType, string paramName)
    {
        var renderer = new TypeElementRenderer();
        renderer.type = typeof(Vector4);
        var fieldView = new Vector4Field(paramName);
        renderer.element = fieldView;
        renderer.ToValueFunc = (r) =>
        {
            return fieldView.value;
        };

        renderer.SetValueAction = (obj) =>
        {
            fieldView.value = (Vector4)obj;
        };
        return renderer;
    }

    public static TypeElementRenderer Vec2IntRenderer(System.Type targetType, string paramName)
    {
        var renderer = new TypeElementRenderer();
        renderer.type = typeof(Vector2Int);
        var fieldView = new Vector2IntField(paramName);
        renderer.element = fieldView;
        renderer.ToValueFunc = (r) =>
        {
            return fieldView.value;
        };

        renderer.SetValueAction = (obj) =>
        {
            fieldView.value = (Vector2Int)obj;
        };

        return renderer;
    }

    public static TypeElementRenderer Vec3IntRenderer(System.Type targetType, string paramName)
    {
        var renderer = new TypeElementRenderer();
        renderer.type = typeof(Vector3Int);
        var fieldView = new Vector3IntField(paramName);
        renderer.element = fieldView;
        renderer.ToValueFunc = (r) =>
        {
            return fieldView.value;
        };

        renderer.SetValueAction = (obj) =>
        {
            fieldView.value = (Vector3Int)obj;
        };
        return renderer;
    }

    public static TypeElementRenderer RectRenderer(System.Type targetType, string paramName)
    {
        var renderer = new TypeElementRenderer();
        renderer.type = typeof(Rect);
        var fieldView = new RectField(paramName);
        renderer.element = fieldView;
        renderer.ToValueFunc = (r) =>
        {
            return fieldView.value;
        };

        renderer.SetValueAction = (obj) =>
        {
            fieldView.value = (Rect)obj;
        };
        return renderer;
    }

    public static TypeElementRenderer RectIntRenderer(System.Type targetType, string paramName)
    {
        var renderer = new TypeElementRenderer();
        renderer.type = typeof(RectInt);
        var fieldView = new RectIntField(paramName);
        renderer.element = fieldView;
        renderer.ToValueFunc = (r) =>
        {
            return fieldView.value;
        };

        renderer.SetValueAction = (obj) =>
        {
            fieldView.value = (RectInt)obj;
        };
        return renderer;
    }

    public static TypeElementRenderer EnumRenderer(System.Type targetType, string paramName)
    {
        var renderer = new TypeElementRenderer();
        renderer.type = targetType;
        var values = Enum.GetValues(targetType);
        var fieldView = new EnumField(paramName, values.GetValue(0) as Enum);
      
        renderer.element = fieldView;
        renderer.ToValueFunc = (r) =>
        {
            return fieldView.value;
        };

        renderer.SetValueAction = (obj) =>
        {
            fieldView.value = (Enum)obj;
        };
        return renderer;
    }

    public static TypeElementRenderer ArrayRenderer(System.Type targetType, string paramName)
    {
        var renderer = new ListElementRenderer();
        renderer.InitArray(targetType,paramName,factory);
        return renderer;
    }

    public static TypeElementRenderer ListRenderer(System.Type targetType, string paramName)
    {
        var renderer = new ListElementRenderer();
        renderer.InitList(targetType, paramName, factory);
        return renderer;
    }


    public static TypeElementRenderer UObjectRenderer(System.Type targetType, string paramName)
    {
        var renderer = new TypeElementRenderer();
        renderer.type = typeof(UnityEngine.Object);
        renderer.element = new ObjectField(paramName);
        renderer.ToValueFunc = (r) =>
        {
            return ((ObjectField)renderer.element).value;
        };

        renderer.SetValueAction = (obj) =>
        {
            ((ObjectField)renderer.element).value = obj as UnityEngine.Object;
        };
        return renderer;
    }

    public static TypeElementRenderer IParamDataRenderer(System.Type targetType, string paramName)
    {
        var renderer = new SerializeElementRenderer();
        renderer.InitObject(targetType, paramName, factory);
        return renderer;
    }

    public static ITypeElementRendererFactory factory;
}
