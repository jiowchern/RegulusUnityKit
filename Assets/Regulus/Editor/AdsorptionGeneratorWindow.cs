using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using System.Reflection;
using UnityEngine;
using System.CodeDom.Compiler;
using Microsoft.CSharp;

public class AdsorptionGeneratorWindow : EditorWindow {
    
    private string _OutputPath;
    private string[] _Namespaces;
    private int _NamespaceIndex;
    private string _InputPath;
    private Assembly _Assembly;
    

    public AdsorptionGeneratorWindow()
    {
        _Namespaces = new string[0];
        _NamespaceIndex = 0;
        _OutputPath = string.Empty;
        _InputPath = String.Empty;
    }

    [MenuItem("Regulus/Tool/AdsorptionGenerator")]
    public static void Open()
    {
        var wnd = EditorWindow.GetWindow<AdsorptionGeneratorWindow>();
        wnd.Show();
    }
  

    public void OnGUI()
    {
        
        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(_InputPath);
        if (GUILayout.Button("Open"))
        {            
            _InputPath = EditorUtility.OpenFilePanelWithFilters("Select DLL", Application.dataPath,new [] {"dll","dll"});
            
            _Assembly = Assembly.LoadFile(_InputPath);
            var namesapces = new HashSet<string>(from type in _Assembly.GetTypes() select type.Namespace);
            _Namespaces = namesapces.ToArray();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        _NamespaceIndex = EditorGUILayout.Popup("Namespace", _NamespaceIndex, _Namespaces);       
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(_OutputPath);
        if (GUILayout.Button("Output"))
        {
            _OutputPath = EditorUtility.OpenFolderPanel("Select Output Dir", Application.dataPath, Application.dataPath);
        }
        EditorGUILayout.EndHorizontal();

        if (_NamespaceIndex < _Namespaces.Length && GUILayout.Button("Generate") )
        {
            EditorApplication.LockReloadAssemblies();
            try
            {
                _Generate(_Assembly);                  
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                EditorApplication.UnlockReloadAssemblies();
            }
            AssetDatabase.Refresh();
            Debug.Log("done.");

            _CollectMissing();
        }

        EditorGUILayout.EndVertical();


    }

    private void _CollectMissing()
    {
        //AppDomain.CurrentDomain.GetAssemblies();
    }

    private void _Generate(Assembly assembly)
    {
        foreach (var type in _GetTypes(assembly))
        {
            _BuildCode(type);
        }
    }

    

    private void _BuildCode(Type type)
    {
        var code = string.Format(
            @"                    
namespace {0}.Adsorption
{{
    
    public class {7}Adsorber : Regulus.Remoting.Unity.Adsorber<{1}>
    {{
        [System.Serializable]
        public class UnityEnableEvent : UnityEngine.Events.UnityEvent<bool> {{}}
        public UnityEnableEvent EnableEvent;
        [System.Serializable]
        public class UnitySupplyEvent : UnityEngine.Events.UnityEvent<{1}> {{}}
        public UnitySupplyEvent SupplyEvent;
        {1} _{7};                        
        public {7}Adsorber()
        {{
                                
        }}

        public override {1} GetGPI()
        {{
            return _{7};
        }}
        public override void Supply({1} gpi)
        {{
            _{7} = gpi;
            {5}
            EnableEvent.Invoke(true);
            SupplyEvent.Invoke(gpi);
        }}

        public override void Unsupply({1} gpi)
        {{
            EnableEvent.Invoke(false);
            {6}
            _{7} = null;
        }}
        {2}
        {3}
        {4}
    }}
}}
                    ", _Namespaces[_NamespaceIndex], type.Name, _GenerateMethods(type), _GenerateReturnEvents(type),
            _GenerateEvents(type), _GetBindEvents(type, "+="), _GetBindEvents(type, "-="), _GetClassName(type.Name));
        System.IO.File.WriteAllText(_OutputPath + "\\" + _GetClassName(type.Name) + "Adsorber" + ".cs", code);
    }

    private string _GetClassName(string type_name)
    {
        var className = new string(type_name.Skip(1).ToArray());
        return className;
    }

    private object _GetBindEvents(Type type, string op_code)
    {
        var codes = new List<string>();
        foreach (var eventInfo in type.GetEvents())
        {
            codes.Add(_GetBindEvent(type , eventInfo , op_code ));
        }
        return string.Join("\n", codes.ToArray());
    }

    private string _GetBindEvent(Type type,EventInfo event_info, string op_code)
    {
        
        return string.Format("_{0}.{1} {2} _On{1};" , _GetClassName(type.Name) , event_info.Name , op_code);
    }

    private string _GenerateReturnEvents(Type type)
    {
        var methodInfos = type.GetMethods();
        var codes = new List<string>();
        foreach (var methodInfo in methodInfos)
        {
            if (methodInfo.IsSpecialName)
            {
                continue;
            }

            var returnType = methodInfo.ReturnType;

            if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Regulus.Remoting.Value<>))
            {
                codes.Add(_GenerateMethodReturnEvent(methodInfo));
            }
        }

        return string.Join("\n", codes.ToArray());
    }
    
    private string _GenerateMethodReturnEvent(MethodInfo method_info)
    {
        var genericArguments = method_info.ReturnType.GetGenericArguments();
        var argTypes  = AdsorptionGeneratorWindow._GetArgTypes(genericArguments);        
        string code = String.Format(@"
        [System.Serializable]
        public class Unity{0}Result : UnityEngine.Events.UnityEvent{1} {{ }}
        public Unity{0}Result {0}Result;
        ", method_info.Name , argTypes);
        return code;
    }
    private string _GetArgDefines(Type[] generic_arguments)
    {
        var types = new List<string>();
        for (int i = 0; i < generic_arguments.Length; i++)
        {
            var type = generic_arguments[i];
            types.Add(type.FullName + " arg" + i);
        }
        
        return string.Join(",", types.ToArray());
    }
    private static string _GetArgTypes(Type[] generic_arguments)
    {
        if(generic_arguments.Any())
            return "<" + string.Join(",", (from arg in generic_arguments select arg.ToString()).ToArray())+ ">";
        return String.Empty;
    }

    private string _GenerateEvents(Type type)
    {
        var codes = new List<string>();
        var eventInfos = type.GetEvents();
        foreach (var eventInfo in eventInfos)
        {
            codes.Add(_GenerateEventDefine(eventInfo));
            codes.Add(_GenerateEventFunction(eventInfo));
        }
        return string.Join("\n", codes.ToArray());
    }

    private string _GenerateEventFunction(EventInfo event_info)
    {

        var argTypes = event_info.EventHandlerType.GetGenericArguments();
        var argNames = _GetArgNames(argTypes);
        var argDefines = _GetArgDefines(argTypes);
        string code = String.Format(@"        
        private void _On{0}({1})
        {{
            {0}.Invoke({2});
        }}
        ", event_info.Name, argDefines , argNames);
        return code;
    }

    private string _GetArgNames(Type[] arg_types)
    {
        return string.Join(",", _GenArgNumber(arg_types.Length).ToArray());
    }

    private IEnumerable<string> _GenArgNumber(int length)
    {
        for (int i = 0; i < length; i++)
        {
            yield return "arg" + i;
        }        
    }

    private string _GenerateEventDefine(EventInfo event_info)
    {
        var argTypes = _GetArgTypes(event_info.EventHandlerType.GetGenericArguments());

        string code = String.Format(@"
        [System.Serializable]
        public class Unity{0} : UnityEngine.Events.UnityEvent{1} {{ }}
        public Unity{0} {0};
        ", event_info.Name, argTypes);
        return code;
    }

    private string _GenerateMethods(Type type)
    {
        var methodInfos = type.GetMethods();
        var methodCodes = new List<string>();
        foreach (var methodInfo in methodInfos)
        {
            if (methodInfo.IsSpecialName)
            {
                continue;
            }
            var returnType = methodInfo.ReturnType;
            
            if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof (Regulus.Remoting.Value<>))
            {
                methodCodes.Add(_GenerateMethodHaveReturn(type, methodInfo));
            }
            else
            {
                methodCodes.Add(_GenerateMethodNotReturn(type , methodInfo));
            }
        }
        return string.Join("\n", methodCodes.ToArray());
    }

    private string _GenerateMethodNotReturn(Type type, MethodInfo method_info)
    {
        string code = string.Format(@"
        public void {0}({1})
        {{
            if(_{2} != null)
            {{
                _{2}.{0}({3});
            }}
        }}", method_info.Name, _GetParamsDefine(method_info), _GetClassName(type.Name), _GetParams(method_info));
        return code;
    }

    private string _GenerateMethodHaveReturn(Type type, MethodInfo method_info)
    {        
        string code = string.Format(@"
        public void {0}({1})
        {{
            if(_{2} != null)
            {{
                _{2}.{0}({3}).OnValue += ( result ) =>{{ {0}Result.Invoke(result);}};
            }}
        }}" , method_info.Name , _GetParamsDefine(method_info) , _GetClassName(type.Name) , _GetParams(method_info) );

        return code;
    }

    private object _GetParams(MethodInfo method_info)
    {
        var args = new List<string>();
        foreach (var paramInfo in method_info.GetParameters())
        {
            args.Add(paramInfo.Name);
        }
        return String.Join(",", args.ToArray());
    }

    private string _GetParamsDefine(MethodInfo method_info)
    {
        var args = new List<string>();
        foreach (var paramInfo in method_info.GetParameters())
        {
            args.Add(paramInfo.ParameterType.ToString() + " " + paramInfo.Name);
        }
        return String.Join(",", args.ToArray());
    }

    private IEnumerable<Type> _GetTypes(Assembly assembly)
    {
        var types = assembly.GetTypes();
        foreach (var type in types)
        {
            if (type.IsInterface && type.Namespace == _Namespaces[_NamespaceIndex])
            {
                yield return type;
            }
        }
    }


}
