using System;
using System.Collections.Generic;
using UnityEngine;

public interface ICallDataBuilder
{
    void Build(Dictionary<string, Type> typeDict, Dictionary<string, System.Action> actionList);
}

/// <summary>
/// 这个抽象层级有问题.
/// </summary>
public class CallDataProvider
{
    public Dictionary<string, Type> typeDict = new Dictionary<string, Type>();
    public Dictionary<string, System.Action> actionList = new Dictionary<string, Action>();
    public static ICallDataBuilder Builder;
    public void Test()
    {
        UnityEngine.Debug.LogError("Test");
    }

    public void InitData()
    {
        if (Builder != null)
        {
            Builder.Build(typeDict, actionList);
        }
    }

    public void CreateTabs(List<string> tabs, List<bool> tabTypes)
    {
        tabs.Clear();
        tabTypes.Clear();

        AddILTab("CollectMethods", tabs, tabTypes);
        tabTypes[tabTypes.Count - 1] = true;

       
        foreach(var item in typeDict.Keys)
        {
            tabs.Add(item);
            tabTypes.Add(true);
        }
        

        AddILTab("Protocal", tabs, tabTypes);
        AddILTab("ViewUtils", tabs, tabTypes);
        AddILTab("TestUtils", tabs, tabTypes);
        AddILTab("EditorUtils", tabs, tabTypes);
    }

    private void AddILTab(string tab, List<string> tabs, List<bool> tabTypes)
    {
        tabs.Add(tab);
        tabTypes.Add(false);
    }
}
