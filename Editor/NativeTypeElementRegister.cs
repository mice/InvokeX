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
            return System.Convert.ToSByte(tField.value);
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
        renderer.element = new IntegerField(paramName);
        renderer.ToValueFunc = (r) =>
        {
            return (byte)((IntegerField)renderer.element).value;
        };
        renderer.SetValueAction = (obj) =>
        {
            ((IntegerField)renderer.element).value = System.Convert.ToInt32(obj);
        };
        return renderer;
    }

    public static TypeElementRenderer ShortRenderer(System.Type targetType, string paramName)
    {
        var renderer = new TypeElementRenderer();
        renderer.type = typeof(short);
        renderer.element = new IntegerField(paramName);
        renderer.ToValueFunc = (r) =>
        {
            return (short)((IntegerField)renderer.element).value;
        };
        renderer.SetValueAction = (obj) =>
        {
            ((IntegerField)renderer.element).value = System.Convert.ToInt32(obj);
        };
        return renderer;
    }

    public static TypeElementRenderer UShortRenderer(System.Type targetType, string paramName)
    {
        var renderer = new TypeElementRenderer();
        renderer.type = typeof(ushort);
        renderer.element = new IntegerField(paramName);
        renderer.ToValueFunc = (r) =>
        {
            return (ushort)((IntegerField)renderer.element).value;
        };
        renderer.SetValueAction = (obj) =>
        {
            ((IntegerField)renderer.element).value = System.Convert.ToInt32(obj);
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
        renderer.element = new IntegerField(paramName);
        renderer.ToValueFunc = (r) =>
        {
            return (uint)((IntegerField)renderer.element).value;
        };
        renderer.SetValueAction = (obj) =>
        {
            ((IntegerField)renderer.element).value = System.Convert.ToInt32(obj);
        };
        return renderer;
    }

    public static TypeElementRenderer LongRenderer(System.Type targetType, string paramName)
    {
        var renderer = new TypeElementRenderer();
        renderer.type = typeof(long);
        renderer.element = new LongField(paramName);
        renderer.ToValueFunc = (r) =>
        {
            return ((LongField)renderer.element).value;
        };
        renderer.SetValueAction = (obj) =>
        {
            ((LongField)renderer.element).value = System.Convert.ToInt64(obj);
        };
        return renderer;
    }

    public static TypeElementRenderer ULongRenderer(System.Type targetType, string paramName)
    {
        var renderer = new TypeElementRenderer();
        renderer.type = typeof(ulong);
        renderer.element = new LongField(paramName);
        renderer.ToValueFunc = (r) =>
        {
            return (ulong)((LongField)renderer.element).value;
        };

        renderer.SetValueAction = (obj) =>
        {
            ((LongField)renderer.element).value = System.Convert.ToInt64(obj);
        };
        return renderer;
    }

    public static TypeElementRenderer FloatRenderer(System.Type targetType, string paramName)
    {
        var renderer = new TypeElementRenderer();
        renderer.type = typeof(float);
        renderer.element = new FloatField(paramName);
        renderer.ToValueFunc = (r) =>
        {
            return (float)((FloatField)renderer.element).value;
        };
        renderer.SetValueAction = (obj) =>
        {
            ((FloatField)renderer.element).value = System.Convert.ToSingle(obj);
        };
        return renderer;
    }

    public static TypeElementRenderer DoubleRenderer(System.Type targetType, string paramName)
    {
        var renderer = new TypeElementRenderer();
        renderer.type = typeof(Double);
        renderer.element = new DoubleField(paramName);
        renderer.ToValueFunc = (r) =>
        {
            return ((DoubleField)renderer.element).value;
        };
        renderer.SetValueAction = (obj) =>
        {
            ((DoubleField)renderer.element).value = System.Convert.ToDouble(obj);
        };
        return renderer;
    }

    public static TypeElementRenderer BoolRenderer(System.Type targetType, string paramName)
    {
        var renderer = new TypeElementRenderer();
        renderer.type = typeof(bool);
        renderer.element = new Toggle(paramName);
        renderer.ToValueFunc = (r) =>
        {
            return (bool)((Toggle)renderer.element).value;
        };
        renderer.SetValueAction = (obj) =>
        {
            ((Toggle)renderer.element).value = System.Convert.ToBoolean(obj);
        };
        return renderer;
    }

    public static TypeElementRenderer StringRenderer(System.Type targetType, string paramName)
    {
        var renderer = new TypeElementRenderer();
        renderer.type = typeof(string);
        var textElment = new TextField(paramName);
        renderer.element = textElment;
        renderer.ToValueFunc = (r) =>
        {
            return ((TextField)renderer.element).text;
        };

        renderer.SetValueAction = (obj) =>
        {
            ((TextField)renderer.element).value = System.Convert.ToString(obj);
        };
        return renderer;
    }

    public static TypeElementRenderer ColorRenderer(System.Type targetType, string paramName)
    {
        var renderer = new TypeElementRenderer();
        renderer.type = typeof(Color);
        renderer.element = new ColorField(paramName);
        renderer.ToValueFunc = (r) =>
        {
            return ((ColorField)renderer.element).value;
        };
        renderer.SetValueAction = (obj) =>
        {
            ((ColorField)renderer.element).value = (Color)obj;
        };
        return renderer;
    }

    public static TypeElementRenderer Vec2Renderer(System.Type targetType, string paramName)
    {
        var renderer = new TypeElementRenderer();
        renderer.type = typeof(Vector2);
        renderer.element = new Vector2Field(paramName);
        renderer.ToValueFunc = (r) =>
        {
            return ((Vector2Field)renderer.element).value;
        };

        renderer.SetValueAction = (obj) =>
        {
            ((Vector2Field)renderer.element).value = (Vector2)obj;
        };
        return renderer;
    }

    public static TypeElementRenderer Vec3Renderer(System.Type targetType, string paramName)
    {
        var renderer = new TypeElementRenderer();
        renderer.type = typeof(Vector3);
        renderer.element = new Vector3Field(paramName);
        renderer.ToValueFunc = (r) =>
        {
            return ((Vector3Field)renderer.element).value;
        };

        renderer.SetValueAction = (obj) =>
        {
            ((Vector3Field)renderer.element).value = (Vector3)obj;
        };
        return renderer;
    }

    public static TypeElementRenderer Vec4Renderer(System.Type targetType, string paramName)
    {
        var renderer = new TypeElementRenderer();
        renderer.type = typeof(Vector4);
        renderer.element = new Vector4Field(paramName);
        renderer.ToValueFunc = (r) =>
        {
            return ((Vector4Field)renderer.element).value;
        };

        renderer.SetValueAction = (obj) =>
        {
            ((Vector4Field)renderer.element).value = (Vector4)obj;
        };
        return renderer;
    }

    public static TypeElementRenderer Vec2IntRenderer(System.Type targetType, string paramName)
    {
        var renderer = new TypeElementRenderer();
        renderer.type = typeof(Vector2Int);
        renderer.element = new Vector2IntField(paramName);
        renderer.ToValueFunc = (r) =>
        {
            return ((Vector2IntField)renderer.element).value;
        };

        renderer.SetValueAction = (obj) =>
        {
            ((Vector2IntField)renderer.element).value = (Vector2Int)obj;
        };

        return renderer;
    }

    public static TypeElementRenderer Vec3IntRenderer(System.Type targetType, string paramName)
    {
        var renderer = new TypeElementRenderer();
        renderer.type = typeof(Vector3Int);
        renderer.element = new Vector3IntField(paramName);
        renderer.ToValueFunc = (r) =>
        {
            return ((Vector3IntField)renderer.element).value;
        };

        renderer.SetValueAction = (obj) =>
        {
            ((Vector3IntField)renderer.element).value = (Vector3Int)obj;
        };
        return renderer;
    }

    public static TypeElementRenderer RectRenderer(System.Type targetType, string paramName)
    {
        var renderer = new TypeElementRenderer();
        renderer.type = typeof(Rect);

        renderer.element = new RectField(paramName);
        renderer.ToValueFunc = (r) =>
        {
            return ((RectField)renderer.element).value;
        };

        renderer.SetValueAction = (obj) =>
        {
            ((RectField)renderer.element).value = (Rect)obj;
        };
        return renderer;
    }

    public static TypeElementRenderer RectIntRenderer(System.Type targetType, string paramName)
    {
        var renderer = new TypeElementRenderer();
        renderer.type = typeof(RectInt);

        renderer.element = new RectIntField(paramName);
        renderer.ToValueFunc = (r) =>
        {
            return ((RectIntField)renderer.element).value;
        };

        renderer.SetValueAction = (obj) =>
        {
            ((RectIntField)renderer.element).value = (RectInt)obj;
        };
        return renderer;
    }

    public static TypeElementRenderer ArrayRenderer(System.Type targetType, string paramName)
    {
        var renderer = new TypeElementRenderer();
        renderer.type = typeof(System.Array);
        var foldout = new Foldout();
        foldout.text = paramName;
        var subType = targetType.GetElementType();
        var list = new List<TypeElementRenderer>();

        var sizeElement = factory.GetRender(typeof(int), "Size");
        var sizeElementView = (IntXField)sizeElement.element;
        //同样的pattern
        sizeElementView.RegisterValueChangedCallback(t => {
            if (int.TryParse(t.newValue, out var newValue))
            {

            }
            var newCount = newValue + 1;
            var oldCount = foldout.childCount;
            if (newCount > oldCount)
            {
                for (int i = oldCount; i < newCount; i++)
                {
                    var typeRender = factory.GetRender(subType, "Element" + (i));
                    list.Add(typeRender);
                    foldout.Add(typeRender.element);
                }
            }
            else
            {
                while (foldout.childCount > newCount)
                {
                    foldout.RemoveAt(foldout.childCount - 1);
                    list.RemoveAt(list.Count - 1);
                }
            }
        });

        //reactive;
        foldout.Add(sizeElement.element);
        foldout.userData = list;
        renderer.element = foldout;
        renderer.ToValueFunc = (r) =>
        {
            var newSize = int.Parse(sizeElementView.value);
            var obj = Array.CreateInstance(subType, newSize);
            for (int i = 0; i < list.Count; i++)
            {
                obj.SetValue(list[i].ToValueFunc(list[i]), i);
            }
            return obj;
        };

        renderer.SetValueAction = (obj) =>
        {
            throw new NotImplementedException();
        };

        return renderer;
    }

    public static TypeElementRenderer ListRenderer(System.Type targetType, string paramName)
    {
        var renderer = new TypeElementRenderer();
        renderer.type = typeof(System.Collections.Generic.List<>);
        var foldout = new Foldout();
        foldout.text = paramName;
        var subType = targetType.GetGenericArguments()[0];
        var list = new List<TypeElementRenderer>();

        var sizeElement = factory.GetRender(typeof(int), "Size");
        var sizeElementView = (IntXField)sizeElement.element;
        sizeElementView.RegisterValueChangedCallback(t => {
            if (int.TryParse(t.newValue, out var newValue))
            {

            }
            var newCount = newValue + 1;
            var oldCount = foldout.childCount;
            if (newCount > oldCount)
            {
                for (int i = oldCount; i < newCount; i++)
                {
                    var typeRender = factory.GetRender(subType, "Element" + (i));
                    list.Add(typeRender);
                    foldout.Add(typeRender.element);
                }
            }
            else
            {
                while (foldout.childCount > newCount)
                {
                    foldout.RemoveAt(foldout.childCount - 1);
                    list.RemoveAt(list.Count - 1);
                }
            }
        });

        //reactive;
        foldout.Add(sizeElement.element);
        foldout.userData = list;
        renderer.element = foldout;
        renderer.ToValueFunc = (r) =>
        {
            var obj = System.Activator.CreateInstance(targetType) as System.Collections.IList;
            for (int i = 0; i < list.Count; i++)
            {
                obj.Add(list[i].ToValueFunc(list[i]));
            }
            return obj;
        };


        renderer.SetValueAction = (obj) =>
        {
            throw new NotImplementedException();
        };

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
        var renderer = new TypeElementRenderer();
        renderer.type = typeof(IParamData);
        var foldout = new Foldout();
        foldout.text = paramName;
        var list = new List<TypeElementRenderer>();
        var fields = targetType.GetFields(BindingFlags.Instance | BindingFlags.Public);
        foreach (var item in fields)
        {
            var paramAttr = item.GetCustomAttribute<ParamAttribute>();
            // 没有handle paramAttr;
            var fRenderer = factory.GetRender(item.FieldType, item.Name);
            if (fRenderer != null)
            {
                foldout.Add(fRenderer.element);
            }
            list.Add(fRenderer);
        }

        foldout.userData = list;
        renderer.element = foldout;
        renderer.ToValueFunc = (r) =>
        {
            var obj = System.Activator.CreateInstance(targetType);
            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                var tmpValue = list[i].ToValueFunc(list[i]);
                field.SetValue(obj, tmpValue);
            }
            return obj;
        };

        renderer.SetValueAction = (obj) =>
        {
            throw new NotImplementedException();
        };

        return renderer;
    }

    public static ITypeElementRendererFactory factory;
}
