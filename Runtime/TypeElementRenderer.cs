using System;
using UnityEngine.UIElements;

public partial class TypeElementRenderer
{
    public Type type;
    public VisualElement element;
    public System.Func<TypeElementRenderer, System.Object> ToValueFunc;
    public System.Action<System.Object> SetValueAction;
}