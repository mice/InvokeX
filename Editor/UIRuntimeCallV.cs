using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using System.Collections.Generic;
using System.Reflection;
using ILRuntime.CLR.Method;

public static class UIRuntimeSetting
{
    public static string Fold = "Assets/Scripts/RuntimeCall/Editor/";
}

public class UIRuntimeCallV : EditorWindow
{
    
    [MenuItem("Window/UIElements/UIRuntimeCallX")]
    public static void ShowExample()
    {
        var wnd = EditorWindow.GetWindow<UIRuntimeCallV>(true, nameof(UIRuntimeCallV));
        wnd.minSize = new Vector2(1200, 300);
    }

    private TabbedMenuController menuController;
    private StyleSheet styleSheet;
    private Dictionary<string, Type> typeDict = new Dictionary<string, Type>();

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
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(System.IO.Path.Combine(UIRuntimeSetting.Fold, "UIRuntimeCall.uxml"));
        VisualElement labelFromUXML = visualTree.CloneTree();
        root.Add(labelFromUXML);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(System.IO.Path.Combine(UIRuntimeSetting.Fold, "UIRuntimeCall.uss"));
        root.styleSheets.Add(styleSheet);
    }

    private void _CreateTabs(VisualElement root)
    {
        var tabs = new List<string>();

        tabs.Clear();
        tabs.AddRange(typeDict.Keys);
        tabs.Add("TestLog");
        tabs.Add("DLog");

        var tabsContainer = root.Q<VisualElement>("tabs");
        var tabContentContainer = root.Q<VisualElement>("tabContent");
        tabsContainer.Clear();
        tabContentContainer.Clear();

        InitTab(tabs, tabsContainer, tabContentContainer);
        menuController = new TabbedMenuController(root);
        menuController.RegisterTabCallbacks();
        new ContainerData<MethodCLR>(styleSheet, OnSelectItem).InitContainer(root, nameof(GMScripts), GetT);
        new ContainerData<MethodCLR>(styleSheet, OnSelectItem).InitContainer(root, nameof(ProtocalX), GetT);
        new ContainerData<MethodCLR>(styleSheet, OnSelectItem).InitContainer(root, nameof(ViewUtils), GetT);
        new ContainerData<MethodIL>(styleSheet, OnSelectItem).InitContainer(root, "TestLog", GetY);
        new ContainerData<MethodIL>(styleSheet, OnSelectItem).InitContainer(root, "DLog", GetY);
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

        try
        {
            var btn_show_levelUp = root.Q<Button>("btn_show_levelUp");
            if (btn_show_levelUp != null)
                btn_show_levelUp.clicked += _showTextLevelUp;
        }
        catch (Exception exce)
        {
            UnityEngine.Debug.LogError(exce.ToString());
        }

        try
        {
            InitTypes();
            InitILCall();
        }
        catch (Exception exce)
        {
            UnityEngine.Debug.LogError(exce.ToString());
        }

        try
        {
            _CreateTabs(root);
        }
        catch (Exception exce)
        {
            UnityEngine.Debug.LogError(exce.ToString());
        }
    }

    private void InitILCall()
    {
        var appMain = GameObject.FindObjectOfType<AppMain>();
        if (appMain == null || appMain.ilRuntime == null)
            return;
        
        var appdomain = appMain.ilRuntime.AppDomain;
        var mgr = ILRuntimeCallManager.Instance;
        if (appdomain != null)
        {
            mgr.SetDomain(appdomain);
        }

        if(appdomain.LoadedTypes.TryGetValue("TestLog",out var ilType))
        {
            var logInstance = appdomain.Instantiate("TestLog");
            mgr.AddInstance("TestLog", logInstance);
        }

        if (appdomain.LoadedTypes.TryGetValue("DLog", out var ilType2))
        {
            mgr.AddStatic((ILRuntime.CLR.TypeSystem.ILType)ilType2);
        }
    }

    private void GetT(string tab, List<MethodCLR> list)
    {
        var targetMgr = RuntimeCallManager.Instance;
        var methodTable = new Dictionary<string, System.Reflection.MethodBase>();
        targetMgr.GetMethodDictionary(tab, methodTable);
        foreach(var item in methodTable.Values)
        {
            list.Add(new MethodCLR(item));
        }
    }

    private void GetY(string tab, List<MethodIL> list)
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
        if(target is MethodCLR method)
        {
            var scrollView = rootVisualElement.Q<ScrollView>("typeContainer");
            if (scrollView != null)
            {
                TypeRenderUtils.RenderMethod(scrollView, method);
                var button = new Button();
                button.text = "确认";
                button.clicked += () => OnClickSubmit(tab,target);
                scrollView.Insert(0, button);
            }
        }else if(target is MethodIL methodYYY)
        {
            var scrollView = rootVisualElement.Q<ScrollView>("typeContainer");
            if (scrollView != null)
            {
                if (methodYYY.ParamCount > 0)
                {
                    TypeRenderUtils.RenderILParams(scrollView, methodYYY.Data.Parameters);
                }
             
                var button = new Button();
                button.text = "确认";
                button.clicked += () => OnClickSubmit(tab, target);
                scrollView.Insert(0, button);
            }
        }
    }

    private void OnClickSubmit(string sub,System.Object target)
    {
        try
        {
            if (target is MethodCLR method)
            {
                var scrollView = rootVisualElement.Q<ScrollView>("typeContainer");
                if (scrollView != null)
                {
                    if (scrollView.userData is ParamRendererContainer renderContainer)
                    {
                        var arr = new object[method.ParamCount];
                        renderContainer.MakeParams(arr);
                        RuntimeCallManager.Instance.Invoke(sub, method.Name, arr);
                    }
                }
            }
            else if (target is MethodIL methodYYY)
            {
                var scrollView = rootVisualElement.Q<ScrollView>("typeContainer");
                if (scrollView != null)
                {
                    if (scrollView.userData is ParamRendererContainer renderContainer)
                    {
                        var arr = new object[methodYYY.ParamCount];
                        renderContainer.MakeParams(arr);
                        var mgr = ILRuntimeCallManager.Instance;
                        mgr.Invoke(sub, methodYYY.Name, arr);
                    }
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

    private void InitTypes()
    {
        var targetMgr = RuntimeCallManager.Instance;
        targetMgr.AddInstance(nameof(GMScripts), new GMScripts());
        targetMgr.AddInstance(nameof(ProtocalX), new ProtocalX());
        targetMgr.AddStatic(typeof(ViewUtils));
        targetMgr.GetTypeDictionary(typeDict);
    }

    private void _showTextLevelUp()
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
}

public interface IMethodInfoData
{
    String Name { get; }

    int ParamCount { get; }
    ParameterInfo[] GetParameters();
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
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UIRuntimeSetting.Fold + "MethodItemCell.uxml");
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