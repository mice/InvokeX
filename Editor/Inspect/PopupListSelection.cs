using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public interface ISelectionData
{
    int Count { get; }
    string Label(int index);
    bool IsSelected(int index);
    void Select(int index, bool isSelected);
    void CompleteSelection();
}

public class StringSelectData : SelectData<String>
{

}

public class SelectData<T> : ISelectionData
{
    public List<T> options = new List<T>();
    public List<T> selections = new List<T>();
    private System.Action<List<T>> onCompleteSelection;
    private Func<T, string> labelF;
    int ISelectionData.Count => options.Count;
    string ISelectionData.Label(int index) => labelF != null ? labelF.Invoke(options[index]) : (options[index]).ToString();
    bool ISelectionData.IsSelected(int index) => selections.Contains(options[index]);
    void ISelectionData.Select(int index, bool isSelected)
    {
        if (isSelected)
        {
            selections.Add(options[index]);
        }
        else
        {
            selections.Remove(options[index]);
        }
    }

    private void Clear()
    {
        this.options.Clear();
        this.selections.Clear();
        this.onCompleteSelection = null;
    }

    public void SetLabelF(Func<T, string> labelF)
    {
        this.labelF = labelF;
    }

    public void Init(List<T> options, IList<T> selections,
       Action<List<T>> completeSelection)
    {
        this.options.Clear();
        this.options.AddRange(options);

        this.selections.Clear();
        this.selections.AddRange(selections);
        this.onCompleteSelection = completeSelection;
    }

    public void CompleteSelection()
    {
        onCompleteSelection?.Invoke(selections);
        this.Clear();
    }
}

public class PopupListSelection : PopupWindowContent
{
    private ISelectionData SelectionData;
    private Vector2 scrollPos;
    public override Vector2 GetWindowSize()
    {
        return new Vector2(200, 150);
    }

    public void Init(ISelectionData selectionData)
    {
        this.SelectionData = selectionData;
    }

    public override void OnGUI(Rect rect)
    {
        GUILayout.Label("Popup Options Example", EditorStyles.boldLabel);
        GUILayout.BeginVertical();
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        for (int i = 0; i < SelectionData.Count; i++)
        {
            bool selected = SelectionData.IsSelected(i);
            var newSelected = EditorGUILayout.Toggle(SelectionData.Label(i), selected);
            if (newSelected != selected)
            {
                SelectionData.Select(i, newSelected);
            }
        }
        EditorGUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    public override void OnOpen()
    {
        Debug.Log("Popup opened: " + this);
    }

    public override void OnClose()
    {
        SelectionData?.CompleteSelection();
        Clear();
    }

    private void Clear()
    {
        SelectionData = null;
    }
}