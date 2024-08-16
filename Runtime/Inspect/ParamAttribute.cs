using System;

[AttributeUsage( AttributeTargets.Parameter, AllowMultiple = false)]
public class ParamAttribute:Attribute
{
    public System.Type DataProvider { get; set; }
}


[AttributeUsage(AttributeTargets.Parameter)]
public class ConvertAttribute:Attribute
{
    public String ConvertMethod;
}
