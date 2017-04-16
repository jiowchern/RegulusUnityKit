using System.Linq;

using System.Collections.Generic;

using UnityEditor;
using System.Reflection;
using UnityEngine;
using System.CodeDom.Compiler;
using Microsoft.CSharp;

using Regulus.Utility;


[ExecuteInEditMode]
public class AdsorptionWindow : EditorWindow , IGUIDrawer
{
    private readonly Regulus.Utility.StageMachine _Machine;

    private IGUIDrawer _Drawer;
    public AdsorptionWindow()
    {
        _Machine = new StageMachine();
         
        

        _Drawer = this;
        
    }

    private void _ToInput()
    {
        var stage = new AdsorptionWindowCreate();
        _Drawer = stage;
        stage.DoneEvent += _ToWatingCompile;
        _Machine.Push(stage);
    }

    private void _ToWatingCompile()
    {
        var stage = new AdsorptionWindowWaitCompile();
        _Drawer = stage;
        stage.DoneEvent += _ToCollectLostMethods;
        _Machine.Push(stage);
    }

    private void _ToCollectLostMethods()
    {
        var stage = new AdsorptionWindowCollectLostMethods();
        _Drawer = stage;
        stage.DoneEvent += _ToErrorLogs;
        _Machine.Push(stage);
    }

    private void _ToErrorLogs(AdsorptionWindowCollectLostMethods.Error[] errors)
    {
        var stage = new AdsorptionErrorLog(errors);
        _Drawer = stage;
        stage.DoneEvent += _ToInput;
        _Machine.Push(stage);
    }

    [MenuItem("Regulus/Adsorption/Create")]
    public static void OpenCreate()
    {
        var wnd = EditorWindow.GetWindow<AdsorptionWindow>();
        wnd.Create();
    }

    private void Create()
    {
        _ToInput();
        base.Show();
    }

    [MenuItem("Regulus/Adsorption/Check")]
    public static void OpenCheck() 
    {
        var wnd = EditorWindow.GetWindow<AdsorptionWindow>();
        wnd.Check();
    }

    private void Check()
    {
        _ToCollectLostMethods();
        Show();
    }

    public void OnGUI()
    {
        
        
        _Drawer.Draw();
    }

    void Update()
    {
        _Machine.Update();
    }

    void IGUIDrawer.Draw()
    {
        EditorGUILayout.BeginVertical();
        if (GUILayout.Button("Create"))
        {
            Create();            
        }
        if (GUILayout.Button("Check"))
        {
            Check();
        }
        EditorGUILayout.EndVertical();
    }
}