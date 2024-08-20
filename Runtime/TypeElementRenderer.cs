using System;
using UnityEngine.UIElements;

public partial class TypeElementRenderer
{
    public Type type;
    public Type expliciteType;//特别类型的附加类型.
    public VisualElement element;
    public System.Func<TypeElementRenderer, System.Object> ToValueFunc;//method类型可以返回object,但是object类型,需要的是override对应的属性.
    public System.Action<System.Object> SetValueAction;
}