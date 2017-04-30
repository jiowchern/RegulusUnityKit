using System;


using Regulus.Utility;

using System.Linq;

using System.Collections.Generic;

using UnityEditor;
using System.Reflection;

using UnityEngine;


[ExecuteInEditMode]
internal class AdsorptionWindowCreate : IStage , IGUIDrawer
{
    public event Action DoneEvent;
    private string _OutputPath;
    private string[] _Assemblys;
    private int _AssemblyIndex;

    private string _Assembly;

    private string _AgentName;

    public AdsorptionWindowCreate()
    {
        _Assemblys = new string[0]; 
        _AssemblyIndex = 0;
        _OutputPath = string.Empty;
        _AgentName = "";
    }
    void IStage.Enter()
    {
        
    }

    void IStage.Leave()
    {
        
    }

    void IStage.Update()
    {
        
    }

    private void _Generate(bool dir)
    {

        EditorApplication.LockReloadAssemblies();
        try
        {
            Assembly selected = null;
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.FullName == _GetAssembly())
                {
                    selected = assembly;
                }

            }
            var unityProtocolProvider = new Regulus.Remoting.Unity.AssemblyOutputer(selected,  _AgentName);

            unityProtocolProvider.ErrorMessageEvent += _ErrorMessage;
            if (dir)
                unityProtocolProvider.OutputDir(_OutputPath);
            else
            {
                unityProtocolProvider.OutputDll(_OutputPath + "//protocol.dll" 
                    , _GetUnityEngine() , 
                    _GetAssembly("reguluslibrary.dll") ,
                    _GetAssembly("regulusremoting.dll"),
                    _GetAssembly("Regulus.Protocol.dll") ,
                    _GetAssembly("Regulus.Remoting.Unity.dll"),
                    _GetRegulusRemotingGhost(),
                    _GetRegulusSerialization());
                
            }

            DoneEvent();
        }
        catch (Exception e)
        {
            Debug.Log(e);
            throw e;
        }
        finally
        {
            EditorApplication.UnlockReloadAssemblies();
        }


        

    }

    private void _ErrorMessage(string obj)
    {
        Debug.Log(obj);
    }
    private  Assembly _GetRegulusSerialization()
    {
        return _GetAssembly("Regulus.Serialization.dll");
    }

    private  Assembly _GetRegulusRemotingGhost()
    {
        return _GetAssembly("RegulusRemotingGhostNative.dll");
    }

    private  Assembly _GetRegulusProtocolUnity()
    {
        return _GetAssembly("Regulus.Remoting.Unity.dll");
    }

    private  Assembly _GetRegulusProtocol()
    {
        return _GetAssembly("Regulus.Protocol.dll");
    }

    private  Assembly _GetRegulusLibrary()
    {
        return _GetAssembly("reguluslibrary.dll");
    }

    private  Assembly _GetRegulusRemoting()
    {
        return _GetAssembly("regulusremoting.dll");
    }
    private Assembly _GetUnityEngine()
    {

        return _GetAssembly("unityengine.dll");
    }

    private Assembly _GetAssembly(string filename)
    {
        return (from asm in AppDomain.CurrentDomain.GetAssemblies()
                let fileName = System.IO.Path.GetFileName(asm.Location)
                where fileName.ToLower() == filename.ToLower()
                select asm).First();
    }

    void IGUIDrawer.Draw()
    {
        EditorGUILayout.BeginVertical();

        _AgentName = EditorGUILayout.TextField("Agent", _AgentName);

        EditorGUILayout.BeginHorizontal();
        EditorGUI.BeginChangeCheck();
        _Assembly = EditorGUILayout.TextField(_Assembly);
        if (EditorGUI.EndChangeCheck())
        {
            var namesapces = new HashSet<string>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {

                if (assembly.FullName.IndexOf(_Assembly, StringComparison.Ordinal) >= 0)
                {
                    namesapces.Add(assembly.FullName);
                }
                
            }

            _Assemblys = namesapces.ToArray();
        }
        
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        _AssemblyIndex = EditorGUILayout.Popup("Namespace", _AssemblyIndex, _Assemblys);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Dir");
        EditorGUILayout.LabelField(_OutputPath);
        if (GUILayout.Button("Output"))
        {
            _OutputPath = EditorUtility.SaveFolderPanel("Select Output Path", Application.dataPath , Application.dataPath );
        }
        EditorGUILayout.EndHorizontal();
        

        if (_AgentName.Length > 0 && _AssemblyIndex < _Assemblys.Length && System.IO.Directory.Exists(_OutputPath)    )
        {
            if (GUILayout.Button("Generate Dir"))
            {
                _Generate(true);
            }

            if (GUILayout.Button("Generate Dll"))
            {
                _Generate(false);
            }



        }

        EditorGUILayout.EndVertical();
    }
    private string _GetAssembly()
    {
        return _Assemblys[_AssemblyIndex];
    }
}

    