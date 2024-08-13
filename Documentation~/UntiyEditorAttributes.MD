List of Attributes:

    Header
    Space
    Tooltip
    Range
    TextArea
    Multiline
    SerializeField
    HideInInspector
    ContextMenuItem
    ContextMenu
    HelpURL
    RequireComponent
    SelectionBase
    AddComponentMenu
    ExecuteInEditMode

Custom:
- Label
- Button
- IntList
- ReadOnly


22.5.1 General

A small number of attributes affect the language in some way. These attributes include:

    System.AttributeUsageAttribute (§22.5.2), which is used to describe the ways in which an attribute class can be used.
    System.Diagnostics.ConditionalAttribute (§22.5.3), is a multi-use attribute class which is used to define conditional methods and conditional attribute classes. This attribute indicates a condition by testing a conditional compilation symbol.
    System.ObsoleteAttribute (§22.5.4), which is used to mark a member as obsolete.
    System.Runtime.CompilerServices.AsyncMethodBuilderAttribute (§22.5.5), which is used to establish a task builder for an async method.
    System.Runtime.CompilerServices.CallerLineNumberAttribute (§22.5.6.2), System.Runtime.CompilerServices.CallerFilePathAttribute (§22.5.6.3), and System.Runtime.CompilerServices.CallerMemberNameAttribute (§22.5.6.4), which are used to supply information about the calling context to optional parameters.

An execution environment may provide additional implementation-specific attributes that affect the execution of a C# program.

参考:
https://github.com/v0lt13/EditorAttributes.git