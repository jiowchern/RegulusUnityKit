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
    private string[] _Namespaces;
    private int _NamespaceIndex;

    private string _Namesapce;


    public AdsorptionWindowCreate()
    {
        _Namespaces = new string[0];
        _NamespaceIndex = 0;
        _OutputPath = string.Empty;
        
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

    private void _Generate()
    {
        var types = new HashSet<Type>();
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.Namespace == _GetNamespace() )
                    types.Add(type);
            }
        }
        var unityProtocolProvider = new Regulus.Remoting.Unity.AssemblyOutputer(types.ToArray(), _GetUnityEngine() , _GetNamespace());
        unityProtocolProvider.OutputDir(_OutputPath);

    }

    private Assembly _GetUnityEngine()
    {

        return (from asm in AppDomain.CurrentDomain.GetAssemblies()
                let fileName = System.IO.Path.GetFileName(asm.Location)
                where fileName.ToLower() == "unityengine.dll"
                select asm).First();
    }

    void IGUIDrawer.Draw()
    {
        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();
        EditorGUI.BeginChangeCheck();
        _Namesapce = EditorGUILayout.TextField(_Namesapce);
        if (EditorGUI.EndChangeCheck())
        {
            var namesapces = new HashSet<string>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if(type.IsInterface && type.Namespace != null)
                        if(type.Namespace.IndexOf(_Namesapce, StringComparison.Ordinal) >= 0)
                            namesapces.Add(type.Namespace);
                }
            }

            _Namespaces = namesapces.ToArray();
        }
        
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        _NamespaceIndex = EditorGUILayout.Popup("Namespace", _NamespaceIndex, _Namespaces);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Dir");
        EditorGUILayout.LabelField(_OutputPath);
        if (GUILayout.Button("Output"))
        {
            _OutputPath = EditorUtility.SaveFolderPanel("Select Output Path", Application.dataPath , Application.dataPath );
        }
        EditorGUILayout.EndHorizontal();
        

        if (_NamespaceIndex < _Namespaces.Length && System.IO.Directory.Exists(_OutputPath)  && GUILayout.Button("Generate")  )
        {
            EditorApplication.LockReloadAssemblies();
            try
            {
                _Generate();
                
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

        EditorGUILayout.EndVertical();
    }
    private string _GetNamespace()
    {
        return _Namespaces[_NamespaceIndex];
    }
}

    