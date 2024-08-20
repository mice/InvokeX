using System.Collections.Generic;
using UnityEngine.UIElements;
using XInspect;
using System;

public class ListElementRenderer : TypeElementRenderer
{
    private Foldout foldout;
    private ParamRendererContainer container;

    public ListElementRenderer()
    {
        foldout = new Foldout();
        this.element = foldout;
        container = new ParamRendererContainer();
        foldout.userData = container;
    }

    public void InitArray(System.Type targetType,string paramName,ITypeElementRendererFactory factory)
    {
        type = typeof(System.Array);
        expliciteType = targetType;
        foldout.text = paramName;
        var subType = targetType.GetElementType();

        var list = container.list;
        list.Clear();
        InitListSize_(subType, list, factory);

        ToValueFunc = (r) =>
        {
            var obj = Array.CreateInstance(subType, list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                obj.SetValue(list[i].ToValueFunc(list[i]), i);
            }
            return obj;
        };

        SetValueAction = (obj) =>
        {
            if(obj is Array array)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].SetValueAction(array.GetValue(i));
                }
            }
           
        };
    }

    public void InitList(System.Type targetType, string paramName, ITypeElementRendererFactory factory)
    {
        type = typeof(System.Collections.Generic.List<>);
        expliciteType = (System.Type)targetType;
        foldout.text = paramName;
        var subType = targetType.GetGenericArguments()[0];
        var list = container.list;
        list.Clear();
        InitListSize_(subType,list,factory);
        ToValueFunc = (r) =>
        {
            var obj = System.Activator.CreateInstance(targetType) as System.Collections.IList;
            for (int i = 0; i < list.Count; i++)
            {
                obj.Add(list[i].ToValueFunc(list[i]));
            }
            return obj;
        };


        SetValueAction = (obj) =>
        {
            if (obj is System.Collections.IList array)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].SetValueAction(array[i]);
                }
            }
        };

    }

    private void InitListSize_(Type subType, List<TypeElementRenderer> list, ITypeElementRendererFactory factory)
    {
        var sizeElement = factory.GetRender(typeof(int), "Size");
        var sizeElementView = (IntXField)sizeElement.element;
        sizeElementView.RegisterValueChangedCallback(t => {
            if (int.TryParse(t.newValue, out var newValue))
            {

            }
            InitListSize(newValue, subType, list, factory);
        });

        //reactive;
        foldout.Add(sizeElement.element);
    }

    private void InitListSize(int newValue,System.Type subType, List<TypeElementRenderer> list,ITypeElementRendererFactory factory)
    {
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
    }
}
