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
    private string _Namespace;

    public AdsorptionGeneratorWindow()
    {
        _Namespace = string.Empty;
        _OutputPath = string.Empty;
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
        EditorGUILayout.LabelField("Catch namespace");
        _Namespace = EditorGUILayout.TextField(_Namespace);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(_OutputPath);
        if (GUILayout.Button("Output"))
        {
            _OutputPath = EditorUtility.OpenFolderPanel("Select Output Dir", Application.dataPath, Application.dataPath);
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Generate"))
        {
            _Generate();
        }

        EditorGUILayout.EndVertical();


    }

    private void _Generate()
    {
        foreach (var type in _GetTypes())
        {
            if (type.IsInterface)
            {
                
                var code = string.Format(
                    @"                    
namespace {0}.Adsorber
{{
    
    public class Adsorber{1} : Regulus.Remoting.Unity.Adsorber<{1}>
    {{
        [System.Serializable]
        public class UnityEnableEvent : UnityEngine.Events.UnityEvent<bool> {{}}
        public UnityEnableEvent EnableEvent;
        {1} _{1};                        
        public Adsorber{1}()
        {{
                                
        }}

        public override void Supply({1} gpi)
        {{
            _{1} = gpi;
            {5}
            EnableEvent.Invoke(true);
        }}

        public override void Unsupply({1} gpi)
        {{
            EnableEvent.Invoke(false);
            {6}
            _{1} = null;
        }}
        {2}
        {3}
        {4}
    }}
}}
                    ", _Namespace , type.Name , _GenerateMethods(type) , _GenerateReturnEvents(type) , _GenerateEvents(type) , _GetBindEvents(type , "+=") , _GetBindEvents(type , "-="));

                System.IO.File.WriteAllText(_OutputPath+"\\" + "Adsorber" + type.Name+".cs" , code );
            }
        }
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
        
        return string.Format("_{0}.{1} {2} _On{1};" , type.Name , event_info.Name , op_code);
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
        public class Unity{0}Result : UnityEngine.Events.UnityEvent<{1}> {{ }}
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
        return string.Join(",", (from arg in generic_arguments select arg.ToString()).ToArray());
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
            MessageEvent.Invoke({2});
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
        public class Unity{0} : UnityEngine.Events.UnityEvent<{1}> {{ }}
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
        }}", method_info.Name, _GetParamsDefine(method_info), type.Name, _GetParams(method_info));
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
        }}" , method_info.Name , _GetParamsDefine(method_info) , type.Name , _GetParams(method_info) );

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

    private IEnumerable<Type> _GetTypes()
    {
        var assembles = AppDomain.CurrentDomain.GetAssemblies();

        foreach (var assemble in assembles)
        {
            var types = assemble.GetTypes();
            foreach (var type in types)
            {
                if (type.Namespace == _Namespace)
                {
                    yield return type;
                }
            }
        }
    }


}
