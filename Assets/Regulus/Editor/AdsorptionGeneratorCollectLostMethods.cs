using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

using Regulus.Remoting.Unity;
using Regulus.Utility;

using UnityEngine;

internal class AdsorptionGeneratorCollectLostMethods : IStage, IGUIDrawer
{
    public class Error
    {
        public readonly string Message;

        public Error(string message, string type, string method, string path)
        {
            Message = message;
            Type = type;
            Method = method;
            Path = path;
        }

        public readonly string Type;

        public readonly string Method;

        public readonly string Path;
    }
    public class Method
    {
        public readonly MethodInfo Info;

        public readonly string Path;

        public Method(MethodInfo method_info, string path)
        {
            Info = method_info;
            Path = path;
        }
    }

    public class MethodComparer : IEqualityComparer<Method>
    {
        bool IEqualityComparer<Method>.Equals(Method x, Method y)
        {
            return x.Info == y.Info;
        }

        int IEqualityComparer<Method>.GetHashCode(Method obj)
        {
            return obj.GetHashCode();
        }
    }
    public event Action<Error[]> DoneEvent;

    private readonly Queue<Method> _Methods;


    private readonly int _StepAmount;

    private readonly List<Error> _Errors;

    public AdsorptionGeneratorCollectLostMethods()
    {
        _Errors = new List<Error>();
        var methods = new HashSet<Method>(new MethodComparer());
        var assets = UnityEditor.AssetDatabase.FindAssets("t:script");
        foreach (var asset in assets)
        {
            var path = UnityEditor.AssetDatabase.GUIDToAssetPath(asset);

            var chars = System.IO.Path.GetFileName(path).TakeWhile(t => t != '.');
            var file = new string(chars.ToArray());
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType(file);
                if (type == null)
                {
                    continue;
                }
                foreach (var methodInfo in type.GetMethods())
                {
                    if(methodInfo.IsPublic
                        && methodInfo.DeclaringType != typeof(UnityEngine.MonoBehaviour)
                        && methodInfo.DeclaringType != typeof(UnityEngine.Behaviour)
                        && methodInfo.DeclaringType != typeof(UnityEngine.Component)
                        && methodInfo.DeclaringType != typeof(UnityEngine.Object))

                        methods.Add(new Method(methodInfo , path));
                }
                
            }
        }

        _Methods = new Queue<Method>(methods);

        _StepAmount = _Methods.Count;
    }

    

    void IStage.Enter()
    {

        
    }

    

    void IStage.Leave()
    {
        
    }

    void IStage.Update()
    {
        
        if (_Methods.Count > 0)
        {
            var method = _Methods.Dequeue();
            var methodInfo = method.Info;
            
            var attrs = methodInfo.GetCustomAttributes(typeof(AdsorberAttribute), true);
            foreach (AdsorberAttribute attr in attrs)
            {
                string message = string.Empty;
                var match = attr.Match(methodInfo);
                if (match == AdsorberAttribute.MATCH.OK)
                {
                    continue;
                } 
                else if(match == AdsorberAttribute.MATCH.DIFFERENT_METHOD_PARAMS_LENGTH)
                {
                    message = string.Format("Parameter matching error. Please check length.{0}.{1} and {3}.{2}", attr.Adsorber.Name, attr.Target, methodInfo.Name, methodInfo.DeclaringType.Name);
                }
                else if (match == AdsorberAttribute.MATCH.DIFFERENT_METHOD_PARAMS_TYPE)
                {
                    message = string.Format("Parameter matching error. Please check type. (Adsorber){0}.{1} (Source){3}.{2}", attr.Adsorber.Name, attr.Target, methodInfo.Name, methodInfo.DeclaringType.Name);
                }
                else if (match == AdsorberAttribute.MATCH.LOST_METHOD)
                {
                    message = string.Format("Could not find method name {1}.{0}.", attr.Target, attr.Adsorber.Name);
                }

                _Errors.Add( new Error(message , attr.Adsorber.Name , attr.Target , method.Path));

            }
            
        }
        else
        {
            DoneEvent(_Errors.ToArray());
        }
        
        
    }

    void IGUIDrawer.Draw()
    {
        var progress = string.Format("{0}/{1}", _StepAmount - _Methods.Count, _StepAmount);
        //var message = string.Format("{0}.{1} ... ", methodInfo.DeclaringType.Name ,  methodInfo.Name);
        var rect = UnityEditor.EditorGUILayout.BeginVertical();
        UnityEditor.EditorGUI.ProgressBar(rect, 1f - _Methods.Count / (float)_StepAmount, progress);
        GUILayout.Space(16);
        UnityEditor.EditorGUILayout.EndVertical();
    }
}