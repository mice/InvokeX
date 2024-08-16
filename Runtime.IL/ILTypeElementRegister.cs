#if !DISABLE_ILRUNTIME
using ILRuntime.CLR.TypeSystem;
using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

public partial class ILTypeElementRegister:ITypeElementRegister
{
    public static ILTypeElementRegister Instance { get; private set; } = new ILTypeElementRegister();

    public static ITypeElementRendererFactory factory;
    public void Register(ITypeElementRendererFactory factory, Action<Type, Func<Type, string, TypeElementRenderer>> register)
    {
        ILTypeElementRegister.factory = factory;
        register(typeof(ILRuntime.Runtime.Intepreter.ILTypeInstance), ILTypeElementRegister.ILTypeRender);
    }

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

            
            for (int i = 0; i < fieldCount; i++)
            {
                var tmpFieldType = ilType.GetField(i, out var fd);
                if (tmpFieldType is CLRType clrType)
                {
                    if (!clrType.IsGenericInstance 
                        ||(clrType.FullName.StartsWith("System.Collections.Generic.List`1")
                            && clrType.GenericArguments[0].Value is CLRType))
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
                    else
                    {
                        if (clrType.FullName.StartsWith("System.Collections.Generic.List`1"))
                        {
                            var subRender = _RenderILListType(clrType.TypeForCLR, clrType.GenericArguments[0].Value.ReflectionType, fd.Name);
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
                        else
                        {
                            UnityEngine.Debug.LogError($"OnlySupport:List<T>;{clrType}:{clrType.FullName}");
                            continue;
                        }
                    }
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
                else if(tmpFieldType.IsArray)
                {
                    var subRender = factory.GetRender(tmpFieldType.ReflectionType, fd.Name);//Clr 语义丢失了.
                   
                    if (subRender != null)
                    {
                        foldout.Add(subRender.element);
                    }
                    else
                    {
                        UnityEngine.Debug.LogError($"array To type:{ilType} index:{i},fieldType:{tmpFieldType}===clr:{tmpFieldType.TypeForCLR}");
                    }
                    list.Add(subRender);
                }
                else
                {
                    UnityEngine.Debug.LogError($"Field To type:{ilType} index:{i},fieldType:{tmpFieldType}");
                }
            }
        }
        else if (parameterType.FullName == "ILRuntime.Runtime.Intepreter.ILTypeInstance")
        {
            var appDomain = ILRuntimeCallManager.AppDomain;
            IType tmpType = appDomain.GetType(parameterType.FullName);
            var  subSubTarget = appDomain.Instantiate(parameterType.FullName,System.Array.Empty<object>());
            var subSubType = subSubTarget?.GetType();
            if (tmpType != null && tmpType is ILRuntime.CLR.TypeSystem.ILType subIlRuntimeType)
            {
                __RenderILType2(foldout, list, tmpType, subIlRuntimeType);
            }
            else
            {
                UnityEngine.Debug.LogError($"ILTypeRender Null: ILRuntime.Runtime.Intepreter.ILTypeInstance");
            }
        }
        else
        {
            UnityEngine.Debug.LogError($"ILTypeRender Null:");
        }

        renderer.element = foldout;
        renderer.ToValueFunc = t =>
        {
            var appDomain = ILRuntimeCallManager.AppDomain;
            var obj = appDomain.Instantiate(parameterType.FullName);
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
            __RenderILType2(foldout, list, ilType, ilRuntimeType);
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

    private static void __RenderILType2(Foldout foldout, List<TypeElementRenderer> list,
        IType ilType,ILRuntime.CLR.TypeSystem.ILType ilRuntimeType)
    {
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

    private static TypeElementRenderer _RenderILListType(Type targetType, Type subType,string paramName)
    {
        var renderer = new TypeElementRenderer();
        renderer.type = typeof(System.Collections.Generic.List<>);
        var foldout = new Foldout();
        foldout.text = paramName;
        
        var list = new List<TypeElementRenderer>();

       

        var sizeElement = factory.GetRender(typeof(int), "Size");
        var sizeElementView = (IntXField)sizeElement.element;
        sizeElementView.RegisterValueChangedCallback(t => {
            if (int.TryParse( t.newValue,out var newValue))
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
        return renderer;
    }
}
#endif