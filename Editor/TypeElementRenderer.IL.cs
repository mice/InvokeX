using ILRuntime.CLR.TypeSystem;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


public partial class TypeElementRenderer
{
    public static TypeElementRenderer ILTypeRender(System.Type parameterType, string paramName)
    {
        var renderer = new TypeElementRenderer();
        renderer.type = typeof(ILRuntime.CLR.TypeSystem.ILType);
        var list = new List<TypeElementRenderer>();
        var foldout = new Foldout();
        if (parameterType is ILRuntime.Reflection.ILRuntimeType ilRuntimeType)
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
                    var subRender = factory.GetRender(clrType.TypeForCLR, fd.Name);
                    if (subRender != null)
                    {
                        foldout.Add(subRender.element);
                    }
                    else
                    {
                        UnityEngine.Debug.LogError($"Field To type:{ilType} index:{i},fieldType:{tmpFieldType}===clr:{clrType.TypeForCLR}");
                    }
                    list.Add(subRender);
                    
                }
                else if (tmpFieldType.TypeForCLR == typeof(ILRuntime.Runtime.Intepreter.ILTypeInstance))
                {
                    var subRender = RenderILType2(tmpFieldType, fd.Name);
                    if (subRender != null)
                    {
                        foldout.Add(subRender.element);
                    }
                    list.Add(subRender);
                }
                else
                {
                    UnityEngine.Debug.LogError($"Field To type:{ilType} index:{i},fieldType:{tmpFieldType}");
                }
            }
        }

        renderer.element = foldout;
        renderer.ToValueFunc = t =>
        {
            var appDomain = ILRuntimeCallManager.AppDomain;
            var obj = appDomain.Instantiate(parameterType.Name);
            if (parameterType is ILRuntime.Reflection.ILRuntimeType ilRuntimeType2)
            {
                foldout.text = paramName;
                var ilType = ilRuntimeType2.ILType;
                var fieldCount = ilType.TotalFieldCount;
                for (int i = 0; i < fieldCount; i++)
                {
                    obj[i] = list[i].ToValueFunc(list[i]);
                }
            }
            return obj;
        };
        return renderer;
    }



    private static TypeElementRenderer RenderILType2(IType ilType, string paramName)
    {
        var renderer = new TypeElementRenderer();
        renderer.type = typeof(ILRuntime.CLR.TypeSystem.ILType);
        var foldout = new Foldout();
        var list = new List<TypeElementRenderer>();
        if (ilType is ILRuntime.CLR.TypeSystem.ILType ilRuntimeType)
        {
            foldout.text = paramName;

            var fieldCount = ilType.TotalFieldCount;

            for (int i = 0; i < fieldCount; i++)
            {
                var tmpFieldType = ilRuntimeType.GetField(i, out var fd);
                if (tmpFieldType is CLRType clrType)
                {
                    var subRender = factory.GetRender(clrType.TypeForCLR, fd.Name);
                    if (subRender != null)
                    {
                        foldout.Add(subRender.element);
                    }
                    list.Add(subRender);
                }
                else if (tmpFieldType.TypeForCLR == typeof(ILRuntime.Runtime.Intepreter.ILTypeInstance))
                {
                    var subRender = RenderILType2(tmpFieldType, fd.Name);
                    if (subRender != null)
                    {
                        foldout.Add(subRender.element);
                    }
                    list.Add(subRender);
                }
            }
        }
        renderer.element = foldout;
        renderer.ToValueFunc = t =>
        {
            var appDomain = ILRuntimeCallManager.AppDomain;
            var obj = appDomain.Instantiate(ilType.FullName);
            if (ilType is ILRuntime.CLR.TypeSystem.ILType ilRuntimeType2)
            {
                foldout.text = paramName;
                var fieldCount = ilType.TotalFieldCount;
                for (int i = 0; i < fieldCount; i++)
                {
                    obj[i] = list[i].ToValueFunc(list[i]);
                }
            }
            return obj;
        };
        return renderer;
    }
}