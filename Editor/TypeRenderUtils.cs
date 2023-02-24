using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Intepreter;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class TypeRenderView
{
    public System.Type type;
    public Dictionary<VisualElement, FieldInfo> fieldDict = new Dictionary<VisualElement, FieldInfo>();

    public static TypeRenderView CreateTypeView(System.Type type)
    {
        var view = new TypeRenderView();
        view.type = type;
        return view;
    }

    internal void Add(FieldInfo item, VisualElement subView)
    {
        fieldDict[subView] = item;
    }
}

public static class TypeRenderUtils
{
    public static System.Object MakeObjectData(Foldout foldout, TypeRenderView typeRender)
    {
        var obj = System.Activator.CreateInstance(typeRender.type);

        foreach (var item in typeRender.fieldDict)
        {
            if (item.Key is IntegerField intField)
            {
                item.Value.SetValue(obj, intField.value);
            }
            else if (item.Key is TextField text)
            {
                item.Value.SetValue(obj, text.value);
            }
            else if (item.Key is Vector3Field vec3Field)
            {
                item.Value.SetValue(obj, vec3Field.value);
            }
        }
        return obj;
    }

    public static void MakeParams(ScrollView view, object[] ps)
    {
        for (int i = 1; i < view.childCount; i++)
        {
            var tmpView = view.ElementAt(i);
            if (tmpView is IntegerField intField) // 是否为enum;
            {
                ps[i - 1] = (intField.value);
            }
            else if (tmpView is TextField text)
            {
                ps[i - 1] = (text.value);
            }
            else if (tmpView is Vector3Field vec3Field)
            {
                ps[i - 1] = vec3Field.value;
            }
            else if (tmpView is Foldout fold)
            {
                var tmpType = fold.userData as TypeRenderView;
                if (tmpType != null)
                {
                    ps[i - 1] = MakeObjectData(fold, tmpType);
                }
            }
        }
    }

    public static void RenderParams(ScrollView selectItemViews, List<ILRuntime.CLR.TypeSystem.IType> parameterInfos)
    {
        selectItemViews.Clear();
        for (int i = 0; i < parameterInfos.Count; i++)
        {
            var info = parameterInfos[i];
           
            if (GetILTypeView(info.ReflectionType, info.Name,out var view))
            {
                selectItemViews.Add(view);
            }
        }
    }

    public static void RenderParams(ScrollView selectItemViews, ParameterInfo[] parameterInfos)
    {
        selectItemViews.Clear();
        for (int i = 0; i < parameterInfos.Length; i++)
        {
            var info = parameterInfos[i];
            var paramAttr = info.GetCustomAttribute<ParamAttribute>();

            if (GetTypeView(info.ParameterType, info.Name, paramAttr?.DataProvider, out var view))
            {
                selectItemViews.Add(view);
            }
        }
    }


    private static VisualElement RenderILType(System.Type parameterType, string paramName)
    {
        var foldout = new Foldout();
        if(parameterType is ILRuntime.Reflection.ILRuntimeType ilRuntimeType)
        {
            foldout.text = paramName;
            var ilType = ilRuntimeType.ILType;

            var fieldCount = ilType.TotalFieldCount;
          
            UnityEngine.Debug.LogError($"RenderILType:{ilType},fieldCount:{fieldCount},reflectType:{ilType.ReflectionType}");
            for (int i = 0; i < fieldCount; i++)
            {
                var tmpFieldType = ilType.GetField(i, out var fd);
                if (tmpFieldType is CLRType clrType)
                {
                    if (GetTypeView(clrType.TypeForCLR, fd.Name, null, out var subView))
                    {
                        foldout.Add(subView);
                    }
                }else if (tmpFieldType.TypeForCLR == typeof(ILTypeInstance))
                {
                    foldout.Add(RenderILType2(tmpFieldType, fd.Name));
                }
            }
        }

        return foldout;
    }


    private static VisualElement RenderILType2(IType ilType, string paramName)
    {
        var foldout = new Foldout();
        if (ilType is ILRuntime.CLR.TypeSystem.ILType ilRuntimeType)
        {
            foldout.text = paramName;

            var fieldCount = ilType.TotalFieldCount;

            for (int i = 0; i < fieldCount; i++)
            {
                var tmpFieldType = ilRuntimeType.GetField(i, out var fd);
                if (tmpFieldType is CLRType clrType)
                {
                    if (GetTypeView(clrType.TypeForCLR, fd.Name, null, out var subView))
                    {
                        foldout.Add(subView);
                    }
                }
                else if (tmpFieldType.TypeForCLR == typeof(ILTypeInstance))
                {
                    foldout.Add(RenderILType2(tmpFieldType, fd.Name));
                }
            }
        }

        return foldout;
    }

    private static bool GetILTypeView(System.Type parameterType, string paramName,out VisualElement element)
    {
        //UnityEngine.Debug.LogError($"{paramName} |{ parameterType.UnderlyingSystemType } == {parameterType == parameterType.UnderlyingSystemType}::{typeof(ILRuntime.CLR.TypeSystem.ILType)}");
        if (parameterType == typeof(ILType))
        {
            element = (new UnityEngine.UIElements.TextField(parameterType.Name));
            return true;
        }else if (parameterType.UnderlyingSystemType == typeof(ILTypeInstance))
        {
            element = null;
            try
            {
                element = RenderILType(parameterType, paramName);
            }
            catch (Exception exce) {
                UnityEngine.Debug.LogError(exce.ToString());
            }
            if (element == null)
            { 
               element = (new UnityEngine.UIElements.TextField(parameterType.Name));
            }
           
            return true;
        }
        else if (parameterType != parameterType.UnderlyingSystemType)
        {
            return GetTypeView(parameterType.UnderlyingSystemType, paramName, null, out element);
        }
        else
        {
            return GetTypeView(parameterType, paramName, null, out element);
        }
    }

    private static bool GetTypeView(System.Type parameterType, string paramName, System.Type dataProvider, out VisualElement element)
    {
        UnityEngine.Debug.LogError($"GetTypeView:{paramName}:paramType:{parameterType} , { parameterType.UnderlyingSystemType} == {parameterType == parameterType.UnderlyingSystemType}");
        if (parameterType == typeof(int))
        {
            if (dataProvider != null && dataProvider.IsEnum)
            {
                var defaultVal = System.Activator.CreateInstance(dataProvider);
                element = new EnumField(paramName, (Enum)defaultVal);
            }
            else
            {
                element = (new IntegerField(paramName));
            }

            return true;
        }
        else if (parameterType == typeof(uint))
        {
            element = (new IntegerField(paramName));
            return true;
        }
        else if (parameterType == typeof(float))
        {
            element = (new FloatField(paramName));
            return true;
        }
        else if (parameterType == typeof(double))
        {
            element = (new DoubleField(paramName));
            return true;
        }
        else if (parameterType == typeof(string))
        {
            element = (new UnityEngine.UIElements.TextField(paramName));
            return true;
        }
        else if (parameterType == typeof(Vector3))
        {
            element = (new Vector3Field(paramName));
            return true;
        }
        else if (parameterType == typeof(Vector2))
        {
            element = (new Vector2Field());
            return true;
        }
        else if (parameterType.IsAssignableFrom(typeof(UnityEngine.Object)))
        {
            element = (new ObjectField());
            return true;
        }
        else if (typeof(IParamData).IsAssignableFrom(parameterType))
        {
            var foldout = new Foldout();
            foldout.text = paramName;

            foreach (var item in parameterType.GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                var paramAttr = item.GetCustomAttribute<ParamAttribute>();

                if (paramAttr != null)
                {
                    if (GetTypeView(item.FieldType, item.Name, null, out var subView))
                    {
                        foldout.Add(subView);
                    }
                }
                else
                {
                    if (GetTypeView(item.FieldType, item.Name, null, out var subView))
                    {
                        foldout.Add(subView);
                    }
                }

            }
            element = (foldout);
            return true;
        }
        else
        {
            element = null;
            return false;
        }
    }
}