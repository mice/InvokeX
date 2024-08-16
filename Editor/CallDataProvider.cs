using System;
using System.Collections.Generic;
using UnityEngine;

public interface ICallDataBuilder
{
    void Build(Dictionary<(string, string), Type> typeDict, Dictionary<string, System.Action> actionList);
}

/// <summary>
/// 这个抽象层级有问题.
/// </summary>
public class CallDataProvider
{
    private Dictionary<(string,string), Type> typeDict = new Dictionary<(string, string), Type>();
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

    public void CreateTabs(List<string> tabs, List<string> tabTypes)
    {
        tabs.Clear();
        tabTypes.Clear();

        AddILTab("CollectMethods","", tabs, tabTypes);

       
        foreach(var item in typeDict.Keys)
        {
            AddILTab(item.Item2, item.Item1, tabs, tabTypes);
        }
    }

    private void AddILTab(string tab, string methodType,List<string> tabs, List<string> tabTypes)
    {
        tabs.Add(tab);
        tabTypes.Add(methodType);
    }
}
