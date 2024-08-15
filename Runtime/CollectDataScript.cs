using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CollectData
{
    [SerializeField] public SerDict<string, KVPair> MethodParameters = new SerDict<string, KVPair>();

    public bool  ContainsMethod(string methodName) {
        return MethodParameters.ContainsKey(methodName);
    }

    public void AddMethodAndParameters(string methodName,string typeName, string parameters,string methodTypeName) {
        if (!MethodParameters.ContainsKey(methodName))
            MethodParameters.Add(methodName, new KVPair(typeName, parameters, methodTypeName));
        else
        {
            MethodParameters[methodName] = new KVPair(typeName, parameters, methodTypeName);
        }
    }

    public void DeleteMethod(string methodName) {
        if (ContainsMethod(methodName))
            MethodParameters.Remove(methodName);
        else
            Debug.LogError($"MethodParameters is not Contains {methodName} ");
    }

    public void FromStringData(string content)
    {
        try
        {
            MethodParameters = JsonUtility.FromJson<SerDict<string, KVPair>>(content);
        }catch(System.Exception exc)
        {
            UnityEngine.Debug.LogError(exc.Message);
            MethodParameters = new SerDict<string, KVPair>();
        }
    }

    public string ToJsonData()
    {
        var content = JsonUtility.ToJson(MethodParameters);
        return content;
    }
}

[System.Serializable]
public class KVPair
{
    public string key;
    public string value;
    public string methodTypeName;
    public string Item1 => key;
    public string Item2 => value;
    public bool IsIL() => methodTypeName == "MethodIL";
    public KVPair(string key,string value, string methodTypeName)
    {
        this.key = key;
        this.value = value;
        this.methodTypeName = methodTypeName;
    }
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