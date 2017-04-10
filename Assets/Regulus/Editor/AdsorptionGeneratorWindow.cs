using System.Linq;

using System.Collections.Generic;

using UnityEditor;
using System.Reflection;
using UnityEngine;
using System.CodeDom.Compiler;
using Microsoft.CSharp;

using Regulus.Utility;

public class AdsorptionGeneratorWindow : EditorWindow , IGUIDrawer
{


    
    private readonly Regulus.Utility.StageMachine _Machine;

    private IGUIDrawer _Drawer;
    public AdsorptionGeneratorWindow()
    {
        _Machine = new StageMachine();


        _Drawer = this;
        
    }

    private void _ToInput()
    {
        var stage = new AdsorptionGeneratorInput();
        _Drawer = stage;
        stage.DoneEvent += _ToWatingCompile;
        _Machine.Push(stage);
    }

    private void _ToWatingCompile()
    {
        var stage = new AdsorptionGeneratorWaitCompile();
        _Drawer = stage;
        stage.DoneEvent += _ToCollectLostMethods;
        _Machine.Push(stage);
    }

    private void _ToCollectLostMethods()
    {
        var stage = new AdsorptionGeneratorCollectLostMethods();
        _Drawer = stage;
        stage.DoneEvent += _ToErrorLogs;
        _Machine.Push(stage);
    }

    private void _ToErrorLogs(AdsorptionGeneratorCollectLostMethods.Error[] errors)
    {
        var stage = new AdsorptionGeneratorErrorLog(errors);
        _Drawer = stage;
        stage.DoneEvent += _ToInput;
        _Machine.Push(stage);
    }

    [MenuItem("Regulus/Adsorption/Create")]
    public static void OpenCreate()
    {
        var wnd = EditorWindow.GetWindow<AdsorptionGeneratorWindow>();
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
        var wnd = EditorWindow.GetWindow<AdsorptionGeneratorWindow>();
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
        
    }
}