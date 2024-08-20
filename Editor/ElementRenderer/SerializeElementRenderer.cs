using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
using XInspect;

public class SerializeElementRenderer : TypeElementRenderer
{
    private Foldout foldout;
    private ParamRendererContainer container;

    public SerializeElementRenderer()
    {
        foldout = new Foldout();
        this.element = foldout;
        container = new ParamRendererContainer();
        foldout.userData = container;
    }

    public void InitObject(System.Type targetType, string paramName, ITypeElementRendererFactory factory)
    {
        this.type = typeof(IParamData);
        this.expliciteType = targetType;

        var fields = CMDRuntimeContext.Instance.GetOrCreateFields(targetType);
        foreach (var item in fields)
        {
            var paramAttr = item.GetCustomAttribute<ParamAttribute>();
            // 没有handle paramAttr;
            var fRenderer = factory.GetRender(item.FieldType, item.Name);
            if (fRenderer != null)
            {
                foldout.Add(fRenderer.element);
            }
            container.list.Add(fRenderer);
        }

        foldout.userData = container;
        element = foldout;
        ToValueFunc = (r) =>
        {
            var obj = System.Activator.CreateInstance(targetType);
            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                var tmpValue = container.list[i].ToValueFunc(container.list[i]);
                field.SetValue(obj, tmpValue);
            }
            return obj;
        };

        SetValueAction = (obj) =>
        {
            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                try
                {
                    container.list[i].SetValueAction(field.GetValue(obj));
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogWarning($"XXX:{ex}");
                }
            }
        };

        SaveValueAction = (obj) =>
        {
            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                try
                {
                    var fieldType = field.FieldType;
                    if (fieldType.IsPrimitive)
                    {
                        field.SetValue(obj, container.list[i].ToValueFunc(container.list[i]));
                    }else if (fieldType.IsValueType)
                    {
                        field.SetValue(obj, container.list[i].ToValueFunc(container.list[i]));
                    }else
                    {
                        container.list[i].SetValueAction((object)field.GetValue(obj));
                    }
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogWarning($"XXX:{ex}");
                }
            }
        };
    }
}
