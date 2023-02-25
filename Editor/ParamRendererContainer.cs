using System.Collections.Generic;

public class ParamRendererContainer
{
    public string MethodName;
    public List<TypeElementRenderer> list = new List<TypeElementRenderer>();
    public ParamRendererContainer()
    {

    }

    public void MakeParams(object[] ps)
    {
        for (int i = 0; i < list.Count; i++)
        {
            ps[i] = list[i].ToValueFunc.Invoke(list[i]);
        }
    }
}