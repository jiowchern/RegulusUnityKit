using System;
using System.CodeDom.Compiler;

using Regulus.Utility;

using System.Linq;

using System.Collections.Generic;

using UnityEditor;
using System.Reflection;
using System.Text;

using Microsoft.CSharp;

using Regulus.Protocol;

using UnityEngine;


[ExecuteInEditMode]
internal class AdsorptionWindowCreate : IStage , IGUIDrawer
{
    public event Action DoneEvent;
    private string _OutputPath;
    private string[] _Namespaces;
    private int _NamespaceIndex;
    private string _InputPath;
    private Assembly _Assembly;

    public AdsorptionWindowCreate()
    {
        _Namespaces = new string[0];
        _NamespaceIndex = 0;
        _OutputPath = string.Empty;
        _InputPath = String.Empty;
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

    private void _Generate(Assembly assembly)
    {

        
        var ns = _GetNamespace();
        var providerName = ns + "ProtocolProvider";

        var codes = new List<string>();
        foreach (var type in _GetTypes(assembly))
        {

            var code = _BuildAdsorberCode(type);
            //("adsorber", _GetClassName(type.Name) + "Adsorber", code);
            codes.Add(code);

            var code2 = _BuildBroadcasterCode(type);
            codes.Add(code2);
            //_ExportCs("Broadcaster", _GetClassName(type.Name) + "Broadcaster", code2);


            // TODO : _BuildInspectorCode(type);
        }

        var agent = _BuildAgentCode();
        codes.Add(agent);

        //_ExportCs("agent", "Agent" , agent);
        var codeBuilder = new CodeBuilder();
        
       /* var protocolPath = _OutputPath + "\\Protocol\\";
        System.IO.Directory.CreateDirectory(protocolPath);
        codeBuilder.ProviderEvent += (code) => { _WriteFile(protocolPath + providerName + ".cs", code); };
        codeBuilder.GpiEvent += (type_name, code) => { _WriteFile(protocolPath + type_name + ".cs", code); };
        codeBuilder.EventEvent += (type_name, event_name, code) => { _WriteFile(protocolPath + type_name + event_name + ".cs", code); };
        codeBuilder.Build(providerName, new[] { ns }, _GetTypes(assembly).ToArray());*/


         codeBuilder.ProviderEvent += (code) => { codes.Add(code); };
         codeBuilder.GpiEvent += (type_name, code) => { codes.Add(code); };
         codeBuilder.EventEvent += (type_name, event_name, code) => { codes.Add(code); };
         codeBuilder.Build(providerName,new[]{ns},_GetTypes(assembly).ToArray());
        _ExportDLL(assembly, _OutputPath , codes);
    }

    private string _BuildAgentCode()
    {
        return string.Format(@"
using System;

using Regulus.Utility;

using UnityEngine;


namespace {0}.Adsorption
{{
    public class Agent : MonoBehaviour
    {{
        public readonly Regulus.Remoting.Unity.Distributor Distributor;


        private readonly Regulus.Utility.Updater _Updater;

        private readonly Regulus.Remoting.IAgent _Agent;
        public string Name;
        public Agent()
        {{
            var protocol = new {0}ProtocolProvider() as Regulus.Remoting.IProtocol;
            _Agent = Regulus.Remoting.Ghost.Native.Agent.Create(protocol.GetGPIProvider());
            Distributor = new Regulus.Remoting.Unity.Distributor(_Agent);
            _Updater = new Updater();

        }}

        void Start()   
        {{
            _Updater.Add(_Agent);
        }}
        // Use this for initialization
        public void Connecter(string ip,int port)
        {{
            _Agent.Connecter(ip, port).OnValue += _ConnectResult;
        }}

        private void _ConnectResult(bool obj)
        {{
            ConnectEvent.Invoke(obj);
        }}

        void OnDestroy()
        {{
            _Updater.Shutdown();
        }}

       
        // Update is called once per frame
        void Update()
        {{
            _Updater.Working();
        }}
        [Serializable]
        public class UnityAgentConnectEvent : UnityEngine.Events.UnityEvent<bool>{{}}

        public UnityAgentConnectEvent ConnectEvent;
    }}
}}
", _GetNamespace());
    }

    private void _WriteFile(string path_s, string code)
    {
        System.IO.File.WriteAllText(path_s, code);
    }

    private void _ExportCs(string dir,string file, string code)
    {
        System.IO.Directory.CreateDirectory(_OutputPath + "\\"+dir +"\\");
        System.IO.File.WriteAllText(_OutputPath + "\\"+dir +"\\"+ file  + ".cs", code);
    }

    private static void _ExportDLL(Assembly assembly, string output_path, List<string> codes)
    {
        var optionsDic = new Dictionary<string, string>
        {
            {"CompilerVersion", "v3.5"}
        };
        var provider = new CSharpCodeProvider(optionsDic);
        var param = new CompilerParameters();

        var assemblies = new[]
        {
            "UnityEngine.dll",            
            "RegulusLibrary.dll",
            "RegulusRemoting.dll",
            "RegulusRemotingGhostNative.dll",
            "Regulus.Remoting.Unity.dll",

        };

        foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
        {
            var path = System.IO.Path.GetFileName(a.Location);
            if (assemblies.Any(a_name => a_name == path))
            {
                param.ReferencedAssemblies.Add(a.Location);
            }
        }

        param.ReferencedAssemblies.Add(assembly.Location);
        param.GenerateExecutable = false;
        param.GenerateInMemory = false;
        param.IncludeDebugInformation = false;
        param.OutputAssembly = output_path;

        var result = provider.CompileAssemblyFromSource(param, codes.ToArray());
        if (result.Errors.Count > 0)
        {
            var msg = new StringBuilder();
            foreach (CompilerError error in result.Errors)
            {
                msg.AppendFormat(
                    "Error ({0}): {1}\n",
                    error.ErrorNumber,
                    error.ErrorText);
            }
            throw new Exception(msg.ToString());
        }
    }
   
    private string _BuildAdsorberCode(Type type)
    {
        var code = string.Format(
            @"                    

namespace {0}.Adsorption
{{
    using System.Linq;
        
    public class {7}Adsorber : UnityEngine.MonoBehaviour , Regulus.Remoting.Unity.Adsorber<{1}>
    {{
        private readonly Regulus.Utility.StageMachine _Machine;        
        
        public string Agent;

        private {0}.Adsorption.Agent _Agent;

        [System.Serializable]
        public class UnityEnableEvent : UnityEngine.Events.UnityEvent<bool> {{}}
        public UnityEnableEvent EnableEvent;
        [System.Serializable]
        public class UnitySupplyEvent : UnityEngine.Events.UnityEvent<{0}.{1}> {{}}
        public UnitySupplyEvent SupplyEvent;
        {0}.{1} _{7};                        
       
        public {7}Adsorber()
        {{
            _Machine = new Regulus.Utility.StageMachine();
        }}

        void Start()
        {{
            _Machine.Push(new Regulus.Utility.SimpleStage(_ScanEnter, _ScanLeave, _ScanUpdate));
        }}

        private void _ScanUpdate()
        {{
            var agents = UnityEngine.GameObject.FindObjectsOfType<{0}.Adsorption.Agent>();
            _Agent = agents.FirstOrDefault(d => string.IsNullOrEmpty(d.Name) == false && d.Name == Agent);
            if(_Agent != null)
            {{
                _Machine.Push(new Regulus.Utility.SimpleStage(_DispatchEnter, _DispatchLeave));
            }}            
        }}

        private void _DispatchEnter()
        {{
            _Agent.Distributor.Attach<{1}>(this);
        }}

        private void _DispatchLeave()
        {{
            _Agent.Distributor.Detach<{1}>(this);
        }}

        private void _ScanLeave()
        {{

        }}


        private void _ScanEnter()
        {{

        }}

        void Update()
        {{
            _Machine.Update();
        }}

        void OnDestroy()
        {{
            _Machine.Termination();
        }}

        public {0}.{1} GetGPI()
        {{
            return _{7};
        }}
        public void Supply({0}.{1} gpi)
        {{
            _{7} = gpi;
            {5}
            EnableEvent.Invoke(true);
            SupplyEvent.Invoke(gpi);
        }}

        public void Unsupply({0}.{1} gpi)
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
        

        return code;
    }


    private string _BuildBroadcasterCode(Type type)
    {
        return string.Format(@"
using System;

using System.Linq;

using Regulus.Utility;

using UnityEngine;
using UnityEngine.Events;

namespace {0}.Adsorption
{{
    public class {2}Broadcaster : UnityEngine.MonoBehaviour 
    {{
        public string Agent;        
        Regulus.Remoting.INotifier<{1}> _Notifier;

        private readonly Regulus.Utility.StageMachine _Machine;

        public {2}Broadcaster()
        {{
            _Machine = new StageMachine();
        }} 

        void Start()
        {{
            _ToScan();
        }}

        private void _ToScan()
        {{
            var stage = new Regulus.Utility.SimpleStage(_ScanEnter , _ScaneLeave , _ScaneUpdate);

            _Machine.Push(stage);
        }}


        private void _ScaneUpdate()
        {{
            var agents = GameObject.FindObjectsOfType<{0}.Adsorption.Agent>();
            var agent = agents.FirstOrDefault(d => d.Name == Agent);
            if (agent != null)
            {{
                _Notifier = agent.Distributor.QueryNotifier<{0}.{1}>();

                _ToInitial();                                
            }}
        }}

        private void _ToInitial()
        {{
            var stage = new Regulus.Utility.SimpleStage(_Initial);
            _Machine.Push(stage);
        }}

        private void _Initial()
        {{
            _Notifier.Supply += _Supply;
            _Notifier.Unsupply += _Unsupply;
        }}

        private void _ScaneLeave()
        {{
            
        }}

        private void _ScanEnter()
        {{
            
        }}

        // Update is called once per frame
        void Update()
        {{
            _Machine.Update();
        }}

        void OnDestroy()
        {{
            if (_Notifier != null)
            {{
                _Notifier.Supply -= _Supply;
                _Notifier.Unsupply -= _Unsupply;
            }}
                
            _Machine.Termination();
        }}

        private void _Unsupply({1} obj)
        {{
            UnsupplyEvent.Invoke(obj);
        }}

        private void _Supply({1} obj)
        {{
            SupplyEvent.Invoke(obj);
        }}

        [Serializable]
        public class UnityBroadcastEvent : UnityEvent<{1}>{{}}

        public UnityBroadcastEvent SupplyEvent;
        public UnityBroadcastEvent UnsupplyEvent;
    }}
}}
", _GetNamespace(), type.Name , _GetClassName(type.Name));
    }

    private object _GetBindEvents(Type type, string op_code)
    {
        var codes = new List<string>();
        foreach (var eventInfo in type.GetEvents())
        {
            codes.Add(_GetBindEvent(type, eventInfo, op_code));
        }
        return string.Join("\n", codes.ToArray());
    }

    private string _GetBindEvent(Type type, EventInfo event_info, string op_code)
    {

        return string.Format("_{0}.{1} {2} _On{1};", _GetClassName(type.Name), event_info.Name, op_code);
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
        var argTypes = _GetArgTypes(genericArguments);
        string code = String.Format(@"
        [System.Serializable]
        public class Unity{0}Result : UnityEngine.Events.UnityEvent{1} {{ }}
        public Unity{0}Result {0}Result;
        ", method_info.Name, argTypes);
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
        if (generic_arguments.Any())
            return "<" + string.Join(",", (from arg in generic_arguments select arg.ToString()).ToArray()) + ">";
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
        ", event_info.Name, argDefines, argNames);
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

            if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Regulus.Remoting.Value<>))
            {
                methodCodes.Add(_GenerateMethodHaveReturn(type, methodInfo));
            }
            else
            {
                methodCodes.Add(_GenerateMethodNotReturn(type, methodInfo));
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
        }}", method_info.Name, _GetParamsDefine(method_info), _GetClassName(type.Name), _GetParams(method_info));

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

    private string _GetClassName(string type_name)
    {
        var className = new string(type_name.Skip(1).ToArray());
        return className;
    }

    void IGUIDrawer.Draw()
    {
        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(_InputPath);
        if (GUILayout.Button("Open"))
        {
            _InputPath = EditorUtility.OpenFilePanelWithFilters("Select DLL", Application.dataPath, new[] { "dll", "dll" });
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
            _OutputPath = EditorUtility.SaveFilePanel("Select output dll" , Application.dataPath , _GetNamespace() + ".Protocol" ,"dll");

            //EditorUtility.SaveFolderPanel("Select Output Path", Application.dataPath , Application.dataPath );
        }
        EditorGUILayout.EndHorizontal();

        if (_NamespaceIndex < _Namespaces.Length && GUILayout.Button("Generate"))
        {
            EditorApplication.LockReloadAssemblies();
            try
            {
                _Generate(_Assembly);
                
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

            try
            {
                UnityEditor.AssetDatabase.Refresh( ImportAssetOptions.ForceUpdate);
            }
            catch (Exception e)
            {
               
            }
        }

        EditorGUILayout.EndVertical();
    }
    private string _GetNamespace()
    {
        return _Namespaces[_NamespaceIndex];
    }
}

    