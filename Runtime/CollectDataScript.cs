using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CollectData
{
    [SerializeField] public SerDict<string, (string,string,bool)> MethodParameters = new SerDict<string, (string, string,bool)>();

    public bool  ContainsMethod(string methodName) {
        return MethodParameters.ContainsKey(methodName);
    }

    public void AddMethodAndParameters(string methodName,string typeName, string parameters,bool isIL) {
        if (!MethodParameters.ContainsKey(methodName))
            MethodParameters.Add(methodName, (typeName, parameters, isIL));
        else
        {
            MethodParameters[methodName] = (typeName, parameters, isIL);
        }
    }

    public void DeleteMethod(string methodName) {
        if (ContainsMethod(methodName))
            MethodParameters.Remove(methodName);
        else
            Debug.LogError($"MethodParameters is not Contains {methodName} ");
    }
}

[System.Serializable]
public class KVPair
{
    public string key;
    public string value;
}


[System.Serializable]
public class SerPlayerData {
    [SerializeField] public string methodName;
    [SerializeField] public string typeName;
    [SerializeField] public string methodParameters;
    [SerializeField] public bool isIL;
    public SerPlayerData() { }
    public SerPlayerData( string _methodName, string _typeName, string _methodParameters, bool _isIL )
    {
        methodName = _methodName;
        typeName = _typeName;
        methodParameters = _methodParameters;
        isIL = _isIL;
    }
}

[System.Serializable]
public class SerDict<Tkey, TValue> : Dictionary<Tkey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField] public List<Tkey> list_key=new List<Tkey>();
    [SerializeField] public List<TValue> list_value=new List<TValue>();

    //序列化之后
    public void OnAfterDeserialize()
    {
        for (int i = 0; i < list_key.Count; i++)
        {
            base.Add(list_key[i], list_value[i]);
        }
    }

    //序列化之前
    public void OnBeforeSerialize()
    {
        list_key = new List<Tkey>(Count);
        list_value = new List<TValue>(Count);
        foreach (var item in base.Keys)
        {
            list_key.Add(item);
            list_value.Add(base[item]);
        }
    }

}