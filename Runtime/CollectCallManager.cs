using System;
using System.Collections.Generic;
using UnityEngine;

public class CollectCallManager 
{
    static CollectCallManager _Instance;
    public static CollectCallManager Instance {
        get {
            if (_Instance == null) {
                _Instance = new CollectCallManager();
                _Instance.GetData();
            }
             
            return _Instance;
        }
    }

    public Action DataUpdateAction = null;

    public List<string> SaveKeys = new List<string>() {
            "PlayerPrefsMethod1", "PlayerPrefsMethod2",
            "PlayerPrefsMethod3", "PlayerPrefsMethod4",  "PlayerPrefsMethod5",
    };

    public Dictionary<string, string> ReverseKeys = new Dictionary<string, string>();

    CollectData methodParameters =new CollectData();
    public CollectData CollectData { get { return methodParameters; } }

    public void GetData() {

        methodParameters.MethodParameters.Clear();
        ReverseKeys.Clear();
        for (int i = 0; i < SaveKeys.Count; i++)
        {
            string json = PlayerPrefs.GetString(SaveKeys[i]);
            if (json != null)
            {
                var serPlayerData = JsonUtility.FromJson<SerPlayerData>(json);
                if (serPlayerData != null)
                {
                    methodParameters.MethodParameters.Add(serPlayerData.methodName, (serPlayerData.typeName, serPlayerData.methodParameters,serPlayerData.isIL));

                    if (!ReverseKeys.ContainsKey(serPlayerData.methodName))
                        ReverseKeys.Add(serPlayerData.methodName, SaveKeys[i]);
                    else
                        ReverseKeys[serPlayerData.methodName] = SaveKeys[i];

                    if (!ReverseKeys.ContainsKey(SaveKeys[i]))
                        ReverseKeys.Add(SaveKeys[i], serPlayerData.methodName);
                    else
                        ReverseKeys[SaveKeys[i]] = serPlayerData.methodName;
                }
            }
        }
    }

    public void CollectMethod(string methodName, string typeName, string parameters,bool isIL)
    {
        if (!methodParameters.ContainsMethod(methodName)) {

            if (methodParameters.MethodParameters.Count < SaveKeys.Count)
            {
                methodParameters.AddMethodAndParameters(methodName, typeName, parameters, isIL);
            }
            else
            {
                var lastKey = SaveKeys[0];
                if (ReverseKeys.ContainsKey(lastKey))
                {
                    methodParameters.DeleteMethod(ReverseKeys[lastKey]);
                    methodParameters.AddMethodAndParameters(methodName, typeName, parameters, isIL);
                }
            }

            SaveCollectData();
            DataUpdateAction?.Invoke();
        }
        else
        {
            methodParameters.AddMethodAndParameters(methodName, typeName, parameters, isIL);
            SaveCollectData();
            DataUpdateAction?.Invoke();
        }
    }

    public void AddCollectMethod(string methodName, string typeName, string parameters, bool isIL) {
        if (!methodParameters.ContainsMethod(methodName))
        {
            if (methodParameters.MethodParameters.Count < SaveKeys.Count)
            {
                methodParameters.AddMethodAndParameters(methodName, typeName, parameters, isIL);
            }
            else
            {
                var lastKey = SaveKeys[SaveKeys.Count - 1];
                if (ReverseKeys.ContainsKey(lastKey))
                {
                    methodParameters.DeleteMethod(ReverseKeys[lastKey]);
                    SaveCollectData();
                    methodParameters.AddMethodAndParameters(methodName, typeName, parameters, isIL);
                }
            }
            SaveCollectData();
            DataUpdateAction?.Invoke();
        }
        else
        {
            methodParameters.AddMethodAndParameters(methodName, typeName, parameters, isIL);
            SaveCollectData();
            DataUpdateAction?.Invoke();
        }
    }

    public void DeleteCollectMethod(string methodName) {
        methodParameters.DeleteMethod(methodName);
        SaveCollectData();
        DataUpdateAction?.Invoke();  
    }

    public void SaveCollectData() {
        ReverseKeys.Clear();
        int i = 0;
        foreach (var Method in methodParameters.MethodParameters)
        {
            var methodData = new SerPlayerData(Method.Key, Method.Value.Item1, Method.Value.Item2,Method.Value.Item3);
            string json = JsonUtility.ToJson(methodData);
            PlayerPrefs.SetString(SaveKeys[i], json);

            if (!ReverseKeys.ContainsKey(methodData.methodName))
                ReverseKeys.Add(methodData.methodName, SaveKeys[i]);
            else
                ReverseKeys[methodData.methodName] = SaveKeys[i];

            if (!ReverseKeys.ContainsKey(SaveKeys[i]))
                ReverseKeys.Add(SaveKeys[i], methodData.methodName);
            else
                ReverseKeys[SaveKeys[i]] = methodData.methodName;

            i += 1;
        }

        if (i < SaveKeys.Count)
        {
            for (int j = i; j < SaveKeys.Count; j++)
            {
                PlayerPrefs.DeleteKey(SaveKeys[j]);
            }
        }
    }
}
