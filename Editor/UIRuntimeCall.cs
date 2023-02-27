using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using System.Reflection;
using ILRuntime.CLR.Method;

/// <summary>
//public static void ShowUIRuntime()
//{
//    var wnd = EditorWindow.GetWindow<UIRuntimeCall>(true, nameof(UIRuntimeCall));
//    wnd.minSize = new Vector2(1200, 300);
//}
/// </summary>
public class UIRuntimeCall : EditorWindow
{
    private TabbedMenuController menuController;
    private StyleSheet styleSheet;

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
            if (tabType[i])
            {
                new ContainerData<MethodCLR>(styleSheet, OnSelectItem).InitContainer(root, tabs[i], GetCLR);
            }
            else
            {
                new ContainerData<MethodIL>(styleSheet, OnSelectItem).InitContainer(root, tabs[i], GetIL);
            }
        }
    }

    protected virtual CallDataProvider GetDataProvider()
    {
        return new CallDataProvider();
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
        var callDataProvider = GetDataProvider();

        try
        {
            var btn_show_levelUp = root.Q<Button>("btn_show_levelUp");
            if (btn_show_levelUp != null)
                btn_show_levelUp.clicked += callDataProvider.ShowTextLevelUp;
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

    private void GetIL(string tab, List<MethodIL> list)
    {
        if (!Application.isPlaying)
            return;
     
        var targetMgr = ILRuntimeCallManager.Instance;
        var methodTable = new Dictionary<string, IMethod>();
        targetMgr.GetMethodDictionary(tab, methodTable);

        foreach(var method in methodTable)
        {
            list.Add(new MethodIL(method.Value));
        }
    }

    private void OnSelectItem(string tab,System.Object target)
    {
        var scrollView = rootVisualElement.Q<ScrollView>("typeContainer");
        if (scrollView != null)
        {
            if (target is MethodCLR methodCLR)
            {
                if (methodCLR.ParamCount > 0)
                {
                    TypeRenderUtils.RenderMethod(scrollView, methodCLR);
                }
                else
                {
                    scrollView.Clear();
                }
                var button = new Button();
                button.text = "确认";
                button.clicked += () => OnClickSubmit(tab, methodCLR);
                scrollView.Insert(0, button);
            }
            else if (target is MethodIL methodIL)
            {
                if (methodIL.ParamCount > 0)
                {
                    TypeRenderUtils.RenderILParams(scrollView, methodIL.Data.Parameters);
                }
                else
                {
                    scrollView.Clear();
                }

                var button = new Button();
                button.text = "确认";
                button.clicked += () => OnClickSubmit(tab, methodIL);
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
            if (scrollView != null && scrollView.userData is ParamRendererContainer renderContainer)
            {
                var arr = MakeParams(target, renderContainer);

                if (target is MethodCLR method)
                {
                    RuntimeCallManager.Instance.Invoke(sub, method.Name, arr);
                }
                else if (target is MethodIL methodYYY)
                {
                    var mgr = ILRuntimeCallManager.Instance;
                    mgr.Invoke(sub, methodYYY.Name, arr);
                }
            }
        }catch(Exception exce)
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

public class MethodCLR : IMethodInfoData
{
    public string Name => Data.Name;
    public int ParamCount { get; private set; }
    public MethodBase Data;

    public MethodCLR(MethodBase method) {
        this.Data = method;
        this.ParamCount = method.GetParameters().Length;
    }

    public ParameterInfo[] GetParameters()
    {
        return Data.GetParameters();
    }
}

public class MethodIL : IMethodInfoData
{
    public string Name => Data.Name;
    public int ParamCount { get; private set; }
    public IMethod Data;
   
    public MethodIL(IMethod method)
    {
        this.Data = method;
        this.ParamCount = method.ParameterCount;
    }

    public ParameterInfo[] GetParameters()
    {
        return Array.Empty<ParameterInfo>();
    }
}

public class ContainerData<T>
    where T:IMethodInfoData
{
    public StyleSheet styleSheet;
    private List<T> list;
    public string tab;
    public System.Action<string,System.Object> OnSelectItem;
    public ContainerData(StyleSheet styleSheet, System.Action<string,System.Object> onSelectItem)
    {
        this.styleSheet = styleSheet;
        this.OnSelectItem = onSelectItem;
    }

    public ContainerData<T> InitContainer(VisualElement root, string tab, System.Action<string,List<T>> GetT)
    {
        this.tab = tab;
        var gmContainer = root.Q<VisualElement>(tab + "Content");
        ListView listView = new UnityEngine.UIElements.ListView();
        listView.AddToClassList("TabContainerListView");
        listView.name = tab + "ListView";
        listView.itemHeight = 32;
        listView.makeItem = _MakeItem;
        listView.bindItem = _BindItem;
        listView.onItemChosen += _OnMethodChoose;

        list = new List<T>();
        GetT(tab, list);
        listView.itemsSource = list;
        listView.styleSheets.Add(styleSheet);
        listView.userData = list;
        gmContainer?.Add(listView);
        return this;
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
            if (btn != null)
            {
                btn.visible = list[index].ParamCount == 0;
            }
            btn.clickable.clickedWithEventInfo += Clickable_clicked;
            btn.userData = index;
        }
    }

    private void Clickable_clicked(EventBase e)
    {
        var index = System.Convert.ToInt32(((VisualElement)(e.currentTarget)).userData);
        if (index < list.Count)
        {
            if(typeof(T) == typeof(MethodCLR))
            {
                var targetMgr = RuntimeCallManager.Instance;
                targetMgr.Invoke(tab, list[index].Name, Array.Empty<object>());
            }
            else if(typeof(T)== typeof(MethodIL))
            {
                var mgr = ILRuntimeCallManager.Instance;
                mgr.Invoke(tab, list[index].Name);
            }
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

public class TabbedMenuController
{
    /* Define member variables*/
    private const string tabClassName = "tab";
    private const string currentlySelectedTabClassName = "currentlySelectedTab";
    private const string unselectedContentClassName = "unselectedContent";
    // Tab and tab content have the same prefix but different suffix
    // Define the suffix of the tab name
    private const string tabNameSuffix = "Tab";
    // Define the suffix of the tab content name
    private const string contentNameSuffix = "Content";

    private readonly VisualElement root;

    public TabbedMenuController(VisualElement root)
    {
        this.root = root;
    }

    public void RegisterTabCallbacks()
    {
        UQueryBuilder<Button> tabs = GetAllTabs();
        tabs.ForEach((Button tab) =>
        {
            tab.RegisterCallback<MouseUpEvent>(TabOnClick);
        });
    }

    /* Method for the tab on-click event: 
       - If it is not selected, find other tabs that are selected, unselect them 
       - Then select the tab that was clicked on
    */
    private void TabOnClick(MouseUpEvent evt)
    {
        Button clickedTab = evt.currentTarget as Button;
        if (!TabIsCurrentlySelected(clickedTab))
        {
            GetAllTabs().Where(
                (tab) => tab != clickedTab && TabIsCurrentlySelected(tab)
            ).ForEach(UnselectTab);
            SelectTab(clickedTab);
        }
    }

    //Method that returns a Boolean indicating whether a tab is currently selected
    private static bool TabIsCurrentlySelected(Button tab)
    {
        return tab.ClassListContains(currentlySelectedTabClassName);
    }

    private UQueryBuilder<Button> GetAllTabs()
    {
        return root.Query<Button>(className: tabClassName);
    }

    /* Method for the selected tab: 
       -  Takes a tab as a parameter and adds the currentlySelectedTab class
       -  Then finds the tab content and removes the unselectedContent class */
    private void SelectTab(Button tab)
    {
        tab.AddToClassList(currentlySelectedTabClassName);
        VisualElement content = FindContent(tab);
        content.RemoveFromClassList(unselectedContentClassName);
    }

    /* Method for the unselected tab: 
       -  Takes a tab as a parameter and removes the currentlySelectedTab class
       -  Then finds the tab content and adds the unselectedContent class */
    private void UnselectTab(Button tab)
    {
        tab.RemoveFromClassList(currentlySelectedTabClassName);
        VisualElement content = FindContent(tab);
        content.AddToClassList(unselectedContentClassName);
    }

    // Method to generate the associated tab content name by for the given tab name
    private static string GenerateContentName(Button tab) =>
        tab.name.Replace(tabNameSuffix, contentNameSuffix);

    // Method that takes a tab as a parameter and returns the associated content element
    private VisualElement FindContent(Button tab)
    {
        return root.Q(GenerateContentName(tab));
    }
}