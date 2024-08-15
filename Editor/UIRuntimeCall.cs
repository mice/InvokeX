using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using System.Reflection;


public class UIRuntimeCallV : EditorWindow
{
    private TabbedMenuController menuController;
    private StyleSheet styleSheet;
    ContainerData<IMethodInfoData> CacheContainer = null;
    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;
        try
        {
            _CreateUI(root);
        }
        catch (Exception exce)
        {
            UnityEngine.Debug.LogError(exce.ToString());
        }
    }

    private void _LoadULLAndUSS(VisualElement root)
    {
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(System.IO.Path.Combine(UIRuntimeCallSetting.Fold, "UIRuntimeCall.uxml"));
        VisualElement labelFromUXML = visualTree.CloneTree();
        root.Add(labelFromUXML);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(System.IO.Path.Combine(UIRuntimeCallSetting.Fold, "UIRuntimeCall.uss"));
        root.styleSheets.Add(styleSheet);
    }


    private void _CreateTabs(VisualElement root, List<string> tabs , List<bool> tabType)
    {
        var tabsContainer = root.Q<VisualElement>("tabs");
        var tabContentContainer = root.Q<VisualElement>("tabContent");
        tabsContainer.Clear();
        tabContentContainer.Clear();

        InitTab(tabs, tabsContainer, tabContentContainer);
        menuController = new TabbedMenuController(root);
        menuController.RegisterTabCallbacks();

        for (int i = 0; i < tabType.Count; i++)
        {

            if (i != 0 && tabType[i])
            {
                new ContainerData<MethodCLR>(styleSheet, OnSelectItem).InitContainer(root, tabs[i], GetCLR);
            }
            else if (i == 0 && tabType[i])
            {
                CacheContainer = new ContainerData<IMethodInfoData>(styleSheet, OnSelectItem2).InitContainer(root, tabs[i], GetCollectMethod);
            }
            else
            {
#if !DISABLE_ILRUNTIME
                new ContainerData<MethodIL>(styleSheet, OnSelectItem).InitContainer(root, tabs[i], GetIL);
#endif
            }
        }
    }

    private void _CreateUI(VisualElement root)
    {
        try
        {
            // Import UXML
            _LoadULLAndUSS(root);
        }
        catch (Exception exce)
        {
            UnityEngine.Debug.LogError(exce.ToString());
        }
        var callDataProvider = new CallDataProvider();
        var collectCallManager = CollectCallManager.Instance;
        collectCallManager.DataUpdateAction = ResetCollectMethodsContainer;
        collectCallManager.GetData();

        try
        {
            var btn_show_levelUp = root.Q<Button>("btn_test");
            if (btn_show_levelUp != null)
                btn_show_levelUp.clicked += callDataProvider.Test;
        }
        catch (Exception exce)
        {
            UnityEngine.Debug.LogError(exce.ToString());
        }

        try
        {
            callDataProvider.InitData();
        }
        catch (Exception exce)
        {
            UnityEngine.Debug.LogError(exce.ToString());
        }

        try
        {
            var btns = root.Q<VisualElement>("btns");
            CreateBtns(btns,callDataProvider.actionList);
        }
        catch(Exception exce)
        {
            UnityEngine.Debug.LogError(exce.ToString());
        }

        try
        {
            var tabs = new List<string>();
            var tabTypes = new List<bool>();
            callDataProvider.CreateTabs(tabs,tabTypes);
            _CreateTabs(root, tabs, tabTypes);
        }
        catch (Exception exce)
        {
            UnityEngine.Debug.LogError(exce.ToString());
        }
    }

    void ResetCollectMethodsContainer() {
        CacheContainer.ResetContainer("CollectMethods", GetCollectMethod);
    }

    private void CreateBtns(VisualElement btns,Dictionary<string,System.Action> actionList)
    {
        foreach(var item in actionList)
        {
            var btn = new Button();
            btn.AddToClassList("cmdBtn");
            btn.text = item.Key;
            btn.clickable.clicked += item.Value;
            btns.Add(btn);
        }
    }

    private void GetCLR(string tab, List<MethodCLR> list)
    {
        var targetMgr = RuntimeCallManager.Instance;
        var methodTable = new Dictionary<string, System.Reflection.MethodBase>();
        targetMgr.GetMethodDictionary(tab, methodTable);
        foreach(var item in methodTable.Values)
        {
            list.Add(new MethodCLR(item));
        }
    }

    private void GetCollectMethod(string typeName,List<IMethodInfoData> list)
    {
        var collectMgr = CollectCallManager.Instance;

        RuntimeCallManager.Instance.GetCollectMethodDictionary(collectMgr.CollectData.MethodParameters, list);
    
#if !DISABLE_ILRUNTIME
        ILRuntimeCallManager.Instance.GetCollectMethodDictionary(collectMgr.CollectData.MethodParameters, list);
#endif
    }

#if !DISABLE_ILRUNTIME
    private void GetIL(string tab, List<MethodIL> list)
    {
        if (!Application.isPlaying)
            return;

        var targetMgr = ILRuntimeCallManager.Instance;
        var methodTable = new Dictionary<string, ILRuntime.CLR.Method.IMethod>();
        targetMgr.GetMethodDictionary(tab, methodTable);

        foreach(var method in methodTable)
        {
            list.Add(new MethodIL(method.Value));
        }
    }
#endif

    private void OnSelectItem(string tab,System.Object selections)
    {
        var scrollView = rootVisualElement.Q<ScrollView>("typeContainer");
        if (scrollView != null)
        {
            System.Object target = selections;
            scrollView.Clear();

#if UNITY_2021_3
            if (selections is IEnumerable<object> tmpSelections)
            {
                foreach(var item in tmpSelections)
                {
                    target = item;
                    break;
                }
            }
#endif
            if(target is IMethodInfoData methodInfo)
            {
                if (methodInfo.ParamCount > 0)
                {
                    TypeRenderUtils.RenderMethod(scrollView, methodInfo);
                }

                var button = new Button();
                button.text = "Submit";
                button.clicked += () => OnClickSubmit(tab, methodInfo);
                scrollView.Insert(0, button);

                var collectButton = new Button();
                collectButton.text = "Collect";
                collectButton.clicked += () => OnClickCollectMethoed(tab, methodInfo);
                scrollView.Insert(0, collectButton);
            }
        }
    }

    private void OnSelectItem2(string tab, System.Object selections)
    {
        var scrollView = rootVisualElement.Q<ScrollView>("typeContainer");
        if (scrollView != null)
        {
            System.Object target = selections;
#if UNITY_2021_3
            if (selections is IEnumerable<object> tmpSelections)
            {
                foreach (var item in tmpSelections)
                {
                    target = item;
                    break;
                }
            }
#endif
            if(target is IMethodInfoData methodInfo)
            {
                scrollView.Clear();
                if (methodInfo.ParamCount > 0)
                {
                    var collectCallMgr = CollectCallManager.Instance;
                    object[] Params = new object[0];
                    if (collectCallMgr.CollectData.ContainsMethod(methodInfo.Name))
                    {
                        Params = methodInfo.FromJson(collectCallMgr.CollectData.MethodParameters[methodInfo.Name].Item2);
                    }

                    TypeRenderUtils.RenderMethodAndParams(scrollView, methodInfo, Params);
                }


                var button = new Button();
                button.text = "Submit";
                button.clicked += () => OnClickCollectSubmit(tab, methodInfo);
                scrollView.Insert(0, button);
            }
        }
    }

    private object[] MakeParams(IMethodInfoData target, ParamRendererContainer renderContainer)
    {
        object[] arr;
        if (target.ParamCount == 0)
        {
            arr = Array.Empty<object>();
        }
        else
        {
            arr = new object[target.ParamCount];
            renderContainer.MakeParams(arr);
        }
        return arr;
    }

    private void OnClickSubmit(string sub, IMethodInfoData target)
    {
        try
        {
            var scrollView = rootVisualElement.Q<ScrollView>("typeContainer");
            //&& scrollView.userData is ParamRendererContainer renderContainer
            if (scrollView != null )
            {
                object[] arr = Array.Empty<object>();
                if (scrollView.userData != null && scrollView.userData is ParamRendererContainer renderContainer)
                    arr = MakeParams(target, renderContainer);

                var typeName = target.TargetTypeName;

                //var arr = MakeParams(target, renderContainer);
                if (target is MethodCLR method)
                {
                    RuntimeCallManager.Instance.Invoke(typeName, method.Name, arr);

                }
#if !DISABLE_ILRUNTIME
                else if (target is MethodIL methodYYY)
                {
                    ILRuntimeCallManager.Instance.Invoke(sub, methodYYY.Name, arr);
                }
#endif

                var parametersStr = target.ToJson(arr);
                var collectCallManager = CollectCallManager.Instance;
                collectCallManager.AddCollectMethod(target.Name, typeName, parametersStr, target.TypeName);
            }
        }catch(Exception exce)
        {
            Debug.LogError(exce.ToString());
        }
    }


    private void OnClickCollectSubmit(string sub, IMethodInfoData target)
    {
        try
        {
            var scrollView = rootVisualElement.Q<ScrollView>("typeContainer");
            if (scrollView != null  )
            {
                object [] arr =Array.Empty<object>();
                if (scrollView.userData!=null && scrollView.userData is ParamRendererContainer renderContainer)
                     arr = MakeParams(target, renderContainer);

                var collectCallManager = CollectCallManager.Instance;
                var methodName = target.Name;
                var typeName = collectCallManager.CollectData.MethodParameters[methodName].Item1;

                var parametersStr = target.ToJson(arr);
                collectCallManager.AddCollectMethod(methodName, typeName, parametersStr, target.TypeName);

                if (target is MethodCLR method)
                {
                    RuntimeCallManager.Instance.Invoke(typeName, methodName, arr);
                }
#if !DISABLE_ILRUNTIME
                else if (target is MethodIL methodYYY)
                {
                    if (string.Equals(typeName, "CPlayerMsgCallerProxy"))
                        typeName = "Protocal";

                    ILRuntimeCallManager.Instance.Invoke(typeName, methodYYY.Name, arr);
                }
#endif
            }
        }
        catch (Exception exce)
        {
            Debug.LogError(exce.ToString());
        }
    }

    private void OnClickCollectMethoed(string sub, IMethodInfoData target )
    {  //string methoed,int[] intParams
        try
        {
            var scrollView = rootVisualElement.Q<ScrollView>("typeContainer");
            if (scrollView != null) 
            {
                object[] arr = Array.Empty<object>();
                if (scrollView.userData !=null && scrollView.userData is ParamRendererContainer renderContainer)
                     arr = MakeParams(target, renderContainer);

                var collectCallManager = CollectCallManager.Instance;
                var typeName = target.TargetTypeName;
                var parametersStr = target.ToJson(arr);
               
                collectCallManager.CollectMethod(target.Name, typeName, parametersStr, target.TypeName);
            }
        }
        catch (Exception exce)
        {
            Debug.LogError(exce.ToString());
        }
    }

    private void InitTab(List<String> tabs,VisualElement tabsContainer,VisualElement tabContentContainer)
    {
        for (int i = 0; i < tabs.Count; i++)
        {
            var ele = new UnityEngine.UIElements.Button();
            var tab = tabs[i];
            ele.name = tab + "Tab";
            ele.text = tab;
            if (i == 0)
            {
                ele.AddToClassList("currentlySelectedTab");
            }
            ele.AddToClassList("tab");
            tabsContainer.Add(ele);

            var cont = new UnityEngine.UIElements.VisualElement();
            cont.name = tab + "Content";
            cont.AddToClassList("tabContent");
            cont.AddToClassList(i == 0 ? "selectedContent" : "unselectedContent");
            tabContentContainer.Add(cont);
        }
    }
}



public class ContainerData<T>
    where T:IMethodInfoData
{
    public StyleSheet styleSheet;
    private List<T> list;
    ListView contListView;
    VisualElement rootVE;

    public string tab;
    public System.Action<string,System.Object> OnSelectItem;
    public ContainerData(StyleSheet styleSheet, System.Action<string,System.Object> onSelectItem)
    {
        this.styleSheet = styleSheet;
        this.OnSelectItem = onSelectItem;
    }

    public ContainerData<T> InitContainer(VisualElement root, string tab, System.Action<string, List<T>> GetT)
    {
        this.tab = tab;
        rootVE = root;
        var gmContainer = root.Q<VisualElement>(tab + "Content");
        ListView listView = new UnityEngine.UIElements.ListView();
        listView.AddToClassList("TabContainerListView");
        listView.name = tab + "ListView";
        listView.itemHeight = 32;
        listView.makeItem = _MakeItem;
        bool isCollect = string.Equals(tab, "CollectMethods");

        if (isCollect)
            listView.bindItem = _BindItem2;
        else
            listView.bindItem = _BindItem;

#if UNITY_2019_4
        listView.onItemChosen += _OnMethodChoose;
#endif
#if UNITY_2021_3
        listView.onItemsChosen += _OnMethodChoose;
#endif

        list = new List<T>();
        GetT(tab, list);

        listView.itemsSource = list;
        listView.styleSheets.Add(styleSheet);
        listView.userData = list;
        contListView = listView;

        gmContainer?.Add(listView);
        return this;
    }

    public void ResetContainer(string tab, System.Action<string, List<T>> GetT) {
        contListView.Clear();
        contListView.AddToClassList("TabContainerListView");
        contListView.name = tab + "ListView";
        contListView.itemHeight = 32;
        contListView.makeItem = _MakeItem;
        contListView.bindItem = _BindItem2;

#if UNITY_2019_4
        contListView.onItemChosen += _OnMethodChoose;
#endif
#if UNITY_2021_3
        contListView.onItemsChosen += _OnMethodChoose;
#endif
        list = new List<T>();
        GetT(tab, list);
        contListView.itemsSource = list;
        contListView.styleSheets.Add(styleSheet);
        contListView.userData = list;
    }

    private VisualElement _MakeItem()
    {
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UIRuntimeCallSetting.Fold + "MethodItemCell.uxml");
        VisualElement labelFromUXML = visualTree.CloneTree();
        labelFromUXML.AddToClassList("gmitem");
        if (styleSheet != null)
            labelFromUXML.styleSheets.Add(styleSheet);
        return labelFromUXML;
    }

    private void _BindItem(VisualElement ele, int index)
    {
        if (index < list.Count)
        {
            var label = ele.Q<Label>();
            if (label!=null)
            {
                label.text = list[index].Name;
            }

            var btn = ele.Q<Button>();
            if (btn != null) btn.visible = false;

            var Testbtn = ele.Q<Button>("Run", "CarryBtn");
            if (Testbtn!=null)
            {
                Testbtn.visible = list[index].ParamCount == 0;
                if (Testbtn.visible)
                {
                    Testbtn.clickable.clickedWithEventInfo -= Clickable_clicked;
                    Testbtn.clickable.clickedWithEventInfo += Clickable_clicked;
                    Testbtn.userData = index;
                }
            }
            else
            {  
                Testbtn = new Button();
                var strName = "Run";
                Testbtn.name = strName;
                Testbtn.text = strName;
                Testbtn.visible = list[index].ParamCount == 0;
                if (Testbtn.visible)
                {
                    ele.Add(Testbtn);
                    Testbtn.AddToClassList("CarryBtn");
                    Testbtn.clickable.clickedWithEventInfo -= Clickable_clicked;
                    Testbtn.clickable.clickedWithEventInfo += Clickable_clicked;
                    Testbtn.userData = index;
                }
            }
            var BtnName = "Collect";
            var BtnPattern = "CollectPattern";
            var CollectBtn = ele.Q<Button>(BtnName, BtnPattern);
            if (CollectBtn != null)
            {
                CollectBtn.visible = list[index].ParamCount == 0;
                if (CollectBtn.visible) CollectBtn.userData = index;
            }
            else
            {
                CollectBtn = new Button();
                CollectBtn.name = BtnName;
                CollectBtn.text = BtnName;
                CollectBtn.visible = list[index].ParamCount == 0;
                if (CollectBtn.visible)
                {
                    ele.Add(CollectBtn);
                    CollectBtn.AddToClassList(BtnPattern);
                    CollectBtn.clickable.clickedWithEventInfo -= _Clickable_CollectMethod;
                    CollectBtn.clickable.clickedWithEventInfo += _Clickable_CollectMethod;
                    CollectBtn.userData = index;
                }
            }
        }
    }

    private void _BindItem2(VisualElement ele, int index)
    {
        if (index < list.Count)
        {
            var label = ele.Q<Label>();
            if (label != null)
            {
                label.text = list[index].Name;
            }

            var btn = ele.Q<Button>();
            if (btn != null) btn.visible = false;
            var BtnStrName = "Run";
            var BtnStrPattern = "CarryBtn";
            var CarryBtn = ele.Q<Button>(BtnStrName, BtnStrPattern);
            if (CarryBtn != null)
            {
                CarryBtn.visible = list[index].ParamCount == 0;
                if (CarryBtn.visible)
                {
                    CarryBtn.clickable.clickedWithEventInfo -= Clickable_clicked;
                    CarryBtn.clickable.clickedWithEventInfo += Clickable_clicked;
                    CarryBtn.userData = index;
                }
            }
            else
            {
                CarryBtn = new Button();
                CarryBtn.name = BtnStrName;
                CarryBtn.text = BtnStrName;
                CarryBtn.visible = list[index].ParamCount == 0;
                if (CarryBtn.visible)
                {
                    ele.Add(CarryBtn);
                    CarryBtn.AddToClassList(BtnStrPattern);
                    CarryBtn.clickable.clickedWithEventInfo -= Clickable_clicked;
                    CarryBtn.clickable.clickedWithEventInfo += Clickable_clicked;
                    CarryBtn.userData = index;
                }
            }

            var BtnName = "UnCollect";
            var BtnPattern = "CollectPattern";
            var CollectBtn = ele.Q<Button>(BtnName, BtnPattern);
            if (CollectBtn != null)
            {
                if (CollectBtn.visible) CollectBtn.userData = index;
            }
            else
            {
                CollectBtn = new Button();
                CollectBtn.name = BtnName;
                CollectBtn.text = BtnName;
                if (CollectBtn.visible)
                {
                    ele.Add(CollectBtn);
                    CollectBtn.AddToClassList(BtnPattern);
                    CollectBtn.clickable.clickedWithEventInfo -= _Clickable_UnCollectMethod;
                    CollectBtn.clickable.clickedWithEventInfo += _Clickable_UnCollectMethod;
                    CollectBtn.userData = index;
                }
            }
        }
    }

    private void Clickable_clicked(EventBase e)
    {
        var index = System.Convert.ToInt32(((VisualElement)(e.currentTarget)).userData);
        if (index < list.Count)
        {
            var methodInfo = list[index];
            var typeName = methodInfo.TypeName;
            var collectCallManager = CollectCallManager.Instance;
            collectCallManager.AddCollectMethod(methodInfo.Name, methodInfo.TargetTypeName, string.Empty, typeName);

            if (string.Equals(typeName, "CPlayerMsgCallerProxy"))
                typeName = "Protocal";

            if (list[index] is MethodCLR methodCLR)
            {
                var targetMgr = RuntimeCallManager.Instance;
                targetMgr.Invoke(typeName, methodCLR.Name, Array.Empty<object>());

            }
#if !DISABLE_ILRUNTIME
            else if(list[index] is MethodIL methodIL)
            {
                var mgr = ILRuntimeCallManager.Instance;
                mgr.Invoke(typeName, methodIL.Name);
            }
#endif
        }
    }

    private void _Clickable_CollectMethod(EventBase eventBase) {
        var index = System.Convert.ToInt32(((VisualElement)(eventBase.currentTarget)).userData);
        if (index < list.Count)
        {
            var methodInfo = list[index];
            var typeName = methodInfo.TargetTypeName;

            CollectCallManager.Instance.CollectMethod(methodInfo.Name, typeName, string.Empty, methodInfo.TypeName);
        }
    }

    private void _Clickable_UnCollectMethod(EventBase eventBase)
    {
        var index = System.Convert.ToInt32(((VisualElement)(eventBase.currentTarget)).userData);
        if (index < list.Count)
        {
            VisualElement rootVisualElement = rootVE;
            var scrollView = rootVE.Q<ScrollView>("typeContainer");
            if (scrollView != null)
            {
                scrollView.Clear();
            }
            
            CollectCallManager.Instance.DeleteCollectMethod(list[index].Name);
        }
    }

    private void _OnMethodChoose(System.Object obj)
    {
        if (obj != null)
        {
            OnSelectItem?.Invoke(this.tab,obj);
        }
    }
}