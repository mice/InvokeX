using System;
using System.Collections.Generic;
using UnityEngine;

public class CollectCallManager
{

    const string MARK_KEY = "_marked_list";
    const string SAVE_KEY = "_collect_list";

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

    public List<string> marked = new List<string>();
    CollectData methodParameters = new CollectData();
    public CollectData CollectData { get { return methodParameters; } }

    public void GetData() {

        methodParameters.MethodParameters.Clear();
        marked.Clear();
        ReadMarked();
        ReadContents();
    }

    private void ReadMarked()
    {
        marked.Clear();
        var content = PlayerPrefs.GetString(MARK_KEY);
        if (content == null || content.Length == 0)
            return;
        marked.AddRange(content.Split('|'));
    }

    private void WriteMarked()
    {
        var content = string.Empty;
        if (marked.Count > 0)
        {
            content = string.Join("|", marked);
        }

        PlayerPrefs.SetString(MARK_KEY, content);
    }

    public void CollectMethod(string methodName, string typeName, string parameters, bool isIL)
    {
        marked.Remove(methodName);
        marked.Add(methodName);
        
        methodParameters.AddMethodAndParameters(methodName, typeName, parameters, isIL);
        SaveCollectData();
        DataUpdateAction?.Invoke();
    }

    public void AddCollectMethod(string methodName, string typeName, string parameters, bool isIL) {
       
        methodParameters.AddMethodAndParameters(methodName, typeName, parameters, isIL);
        SaveCollectData();
        DataUpdateAction?.Invoke();
    }

    public void DeleteCollectMethod(string methodName) {
        methodParameters.DeleteMethod(methodName);
        SaveCollectData();
        DataUpdateAction?.Invoke();
    }

    public void ReadContents()
    {
        var content = PlayerPrefs.GetString(SAVE_KEY);
        if (content.Length > 0)
        {
            methodParameters.FromStringData(content);
        }
    }

    private void WriteContents()
    {
        PlayerPrefs.SetString(SAVE_KEY, methodParameters.ToJsonData());
    }

    public void SaveCollectData() {
        WriteMarked();
        WriteContents();
        PlayerPrefs.Save();
    }
}
