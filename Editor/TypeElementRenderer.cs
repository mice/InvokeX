using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public partial class TypeElementRenderer
{
    public Type type;
    public VisualElement element;
    public System.Func<TypeElementRenderer, System.Object> ToValueFunc;

    public static TypeElementRenderer ByteRenderer(System.Type targetType, string paramName)
    {
        var renderer = new TypeElementRenderer();
        renderer.type = typeof(sbyte);
        renderer.element = new IntegerField(paramName);
        renderer.ToValueFunc = (r) =>
        {
            return (sbyte)((IntegerField)renderer.element).value;
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
        return renderer;
    }

    public static TypeElementRenderer IntRenderer(System.Type targetType,string paramName)
    {
        var renderer = new TypeElementRenderer();
        renderer.type = typeof(int);
        renderer.element = new IntegerField(paramName);
        renderer.ToValueFunc = (r) =>
        {
            return ((IntegerField)renderer.element).value;
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
        return renderer;
    }

    public static TypeElementRenderer FloatRenderer(System.Type targetType, string paramName)
    {
        var renderer = new TypeElementRenderer();
        renderer.type = typeof(float);
        renderer.element = new FloatField(paramName);
        renderer.ToValueFunc = (r) =>
        {
            return (float)((LongField)renderer.element).value;
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
            return (Double)((DoubleField)renderer.element).value;
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
        var sizeElementView = (IntegerField)sizeElement.element;
        sizeElementView.RegisterValueChangedCallback(t=> {
            var newCount = t.newValue + 1;
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
            var obj = Array.CreateInstance(subType, sizeElementView.value);
            for (int i = 0; i < list.Count; i++)
            {
                obj.SetValue(list[i].ToValueFunc(list[i]), i );
            }
            return obj;
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
        var sizeElementView = (IntegerField)sizeElement.element;
        sizeElementView.RegisterValueChangedCallback(t => {
            var newCount = t.newValue + 1;
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
        return renderer;
    }


    public static TypeElementRenderer UObjectRenderer(System.Type targetType, string paramName)
    {
        var renderer = new TypeElementRenderer();
        renderer.type = typeof(int);
        renderer.element = new ObjectField(paramName);
        renderer.ToValueFunc = (r) =>
        {
            return ((ObjectField)renderer.element).value;
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
        return renderer;
    }

    


    public static TypeElementRendererFactory factory;
}
