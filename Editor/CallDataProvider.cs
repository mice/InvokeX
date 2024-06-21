using System;
using System.Collections.Generic;
using UnityEngine;

public interface ICallDataBuilder
{
    void Build(Dictionary<string, Type> typeDict, Dictionary<string, System.Action> actionList);
}

public class CallDataProvider
{
    public Dictionary<string, Type> typeDict = new Dictionary<string, Type>();
    public Dictionary<string, System.Action> actionList = new Dictionary<string, Action>();
    public static ICallDataBuilder Builder;
    public void Test()
    {
        UnityEngine.Debug.LogError("Test");
    }

    //public void ShowTextLevelUp()
    //{
    //    if (!Application.isPlaying)
    //        return;
        
    //    QKILRuntimeEngine.Instance.ILInvoke("ViewUtils", "ShowLvUp");
    //}

    //public void ShowFunctionOpen()
    //{
    //    if (!Application.isPlaying)
    //        return;

    //    QKILRuntimeEngine.Instance.ILInvoke("ViewUtils", "ShowFunctionOpen");
    //}

    //public void ShowCelebrateOpenLog()
    //{
    //    if (!Application.isPlaying)
    //        return;

    //    var appdomain = QKILRuntimeEngine.Instance.Appdomain;
    //    QKILRuntimeEngine.Instance.ILInvoke("ViewUtils", "DumpCelebrate");
    //}

    public void InitData()
    {
        if (Builder != null)
        {
            Builder.Build(typeDict, actionList);
        }
        //InitBtns();
        //InitTypes();
        //InitILCall();
    }

    //private void InitBtns()
    //{
    //    actionList.Clear();
    //    actionList.Add("升级", ShowTextLevelUp);
    //    actionList.Add("功能开放", ShowFunctionOpen);
    //    actionList.Add("活动时间", ShowCelebrateOpenLog);
    //}

    public void CreateTabs(List<string> tabs, List<bool> tabTypes)
    {
        tabs.Clear();
        tabTypes.Clear();

        AddILTab("CollectMethods", tabs, tabTypes);
        tabTypes[tabTypes.Count - 1] = true;

        tabs.AddRange(typeDict.Keys);
        tabTypes.Add(true);

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

    //private void InitTypes()
    //{
    //    var targetMgr = RuntimeCallManager.Instance;
    //    targetMgr.AddInstance(nameof(GMScripts), new GMScripts());
    //    targetMgr.GetTypeDictionary(typeDict);
    //}

}
