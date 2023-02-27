using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class CallDataProvider
{
    public Dictionary<string, Type> typeDict = new Dictionary<string, Type>();

    public virtual void ShowTextLevelUp()
    {
        if (!Application.isPlaying)
            return;
        //var appdomain = QKILRuntimeEngine.Instance.Appdomain;

        //ILRuntime.CLR.TypeSystem.IType t = appdomain.LoadedTypes["ViewUtils"];
        //Type type = t.ReflectionType;

        //if (type!=null)
        //{
        //    appdomain.Invoke("ViewUtils", "ShowLvUp", null);
        //    //appdomain.Invoke("ViewUtils", "ShowFunctionOpen", null);
        //}
    }

    public void InitData()
    {
        InitTypes();
        InitILCall();
    }

    public virtual void CreateTabs(List<string> tabs, List<bool> tabTypes)
    {
        tabs.Clear();
        tabTypes.Clear();
        tabs.AddRange(typeDict.Keys);
        for (int i = 0; i < tabs.Count; i++)
        {
            tabTypes.Add(true);
        }
        tabs.Add("TestLog");
        tabTypes.Add(false);
        tabs.Add("DLog");
        tabTypes.Add(false);
    }

    protected virtual void InitTypes()
    {
        //var targetMgr = RuntimeCallManager.Instance;
        //targetMgr.AddInstance(nameof(GMScripts), new GMScripts());
        //targetMgr.AddInstance(nameof(ProtocalX), new ProtocalX());
        //targetMgr.AddStatic(typeof(ViewUtils));
        //targetMgr.GetTypeDictionary(typeDict);
    }

    protected virtual ILRuntime.Runtime.Enviorment.AppDomain GetAppDomain()
    {
        if (!Application.isPlaying)
            return null;
        // if (QKILRuntimeEngine.Instance != null)
        // {
        //     return QKILRuntimeEngine.Instance.Appdomain;
        // }

        //var appMain = GameObject.FindObjectOfType<AppMain>();
        //if (appMain == null || appMain.ilRuntime == null)
        //    return null;

        //return appMain.ilRuntime.AppDomain;
        return null;
    }

    protected virtual void InitILCall()
    {
        var appdomain = GetAppDomain();
        var mgr = ILRuntimeCallManager.Instance;
        if (appdomain != null)
        {
            mgr.SetDomain(appdomain);
            AddILCall(appdomain, mgr);
        }
    }

    protected virtual void AddILCall(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILRuntimeCallManager mgr)
    {
        //if (appdomain.LoadedTypes.TryGetValue("TestLog", out var ilType))
        //{
        //    var logInstance = appdomain.Instantiate("TestLog");
        //    if (logInstance != null)
        //        mgr.AddInstance("TestLog", logInstance);
        //}

        //if (appdomain.LoadedTypes.TryGetValue("DLog", out var ilType2))
        //{
        //    mgr.AddStatic((ILRuntime.CLR.TypeSystem.ILType)ilType2);
        //}
    }
}
