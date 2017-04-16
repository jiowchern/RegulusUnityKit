using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

using NUnit.Framework;

using Regulus.Project.Chat.Common;

using Regulus.Remoting.Unity;

using UnityEditor;

using UnityEngine;

using Object = UnityEngine.Object;

//[CustomEditor(typeof(TalkerAdsorber))]
public  class TalkerAdsorberInspector : UnityEditor.Editor
{
    private Object _AttachObject;

    public override void OnInspectorGUI()
    {
        

        var adsorber = this.target as Adsorber<ITalker>;
        var gpi = adsorber.GetGPI();
        _DrawBinder();
        _DrawGPI(gpi);

        DrawDefaultInspector();
    }

    private void _DrawBinder()
    {
        EditorGUILayout.BeginHorizontal();
        
        var attechObject = EditorGUILayout.ObjectField("Attach", _AttachObject, typeof (GameObject), true);
        
        if (attechObject != null && _AttachObject != attechObject)
        {
            _AttachObject = null;
            _Attech(attechObject as GameObject);
        }
        else
        {
            _AttachObject = attechObject;
        }
        EditorGUILayout.EndHorizontal();
    }

    private void _Attech(GameObject attach_object)
    {
        var adsorberType = typeof (int/*todo:TalkerAdsorber*/);
        
        var sourceInstance = attach_object;
        var components = sourceInstance.GetComponents<Component>();
        foreach (var component in components)
        {
            var type = component.GetType();

            

            foreach (var sourceMethod in _GetSourceMethods(type))
            {
                foreach (AdsorberAttribute adsorberListenerAttributese in sourceMethod.GetCustomAttributes(typeof(AdsorberAttribute ), false))
                {

                    if (adsorberListenerAttributese.Adsorber != adsorberType)
                    {
                        continue;
                    }
                    _AttechEvent(adsorberListenerAttributese.Target, sourceMethod, component);
                }


            }
            
        }

    }

    

    private void _AttechEvent(string event_name, MethodInfo source_method, Component source_instance)
    {
        var adsorberType = typeof(int/*todo : TalkerAdsorber*/);
        var info = (from i in _GetEvents(adsorberType) where i.Name == event_name select i).FirstOrDefault();

        if (info == null)
        {
            Debug.LogErrorFormat("Could not find method name {1}.{0}.", event_name , adsorberType.Name);
        }
        var eventInfo = info;
        
            
        var invokeMethod = eventInfo.FieldType.GetMethod("Invoke");
        var eventArgs = invokeMethod.GetParameters();
            
        var addMethod = typeof(UnityEditor.Events.UnityEventTools).GetMethods().Where(m => m.Name == "AddPersistentListener" && m.GetGenericArguments().Length == eventArgs.Length && m.GetParameters().Length == 2).Select(m=> m).First();
            
        var methodArgs = source_method.GetParameters();
        if (methodArgs.Length != eventArgs.Length)
        {
            Debug.LogErrorFormat("Parameter matching error. Please check length.{0}.{1} and {3}.{2}", adsorberType.Name, eventInfo.Name, source_method.Name, source_instance.GetType().Name);
            return;
        }
                

        if (methodArgs.All(m => eventArgs.All(e => e.ParameterType == m.ParameterType)))
        {
            var targetFieldInstance = eventInfo.GetValue(target);


            var argTypes = eventArgs.Select(e => e.ParameterType).ToArray();
            var unityAction = _CreateUnityAction(
                source_instance,
                source_method, argTypes);


            if (addMethod.IsGenericMethod)
            {
                var callMethod = addMethod.MakeGenericMethod(argTypes);

                callMethod.Invoke(
                    null,
                    new object[]
                    {
                    targetFieldInstance,
                    unityAction
                    });
            }
            else
            {
                var callMethod = addMethod;

                callMethod.Invoke(
                    null,
                    new object[]
                    {
                    targetFieldInstance,
                    unityAction
                    });
                    
            }
                
        }
        else
        {
            Debug.LogErrorFormat("Parameter matching error. Please check type. (Adsorber){0}.{1} (Source){3}.{2}", adsorberType.Name , eventInfo.Name , source_method.Name , source_instance.GetType().Name);
        }
       
        
    }

    

    private System.Delegate _CreateUnityAction(Component source_instance, MethodInfo source_method, Type[] event_args)
    {
        var type = _GetUnityActionType(event_args.Length);
        
        if(event_args.Length == 0)
            return Delegate.CreateDelegate(type , source_instance, source_method);
        else
        {
            return Delegate.CreateDelegate(type.MakeGenericType(event_args), source_instance, source_method);
        }
    }

    private Type _GetUnityActionType(int length)
    {
        if(length == 0)
            return typeof(UnityEngine.Events.UnityAction);
        if (length == 1)
            return typeof(UnityEngine.Events.UnityAction<>);
        if (length == 2)
            return typeof(UnityEngine.Events.UnityAction<,>);
        if (length == 3)
            return typeof(UnityEngine.Events.UnityAction<,,>);
        if (length == 4)
            return typeof(UnityEngine.Events.UnityAction<,,,>);
        
        throw new Exception("UnityAction does not support more than 5 parameters");
    }

    private IEnumerable<FieldInfo> _GetEvents(Type adsorber_type)
    {
        var fields = adsorber_type.GetFields();
        foreach (var fieldInfo in fields)
        {
            var baseType = fieldInfo.FieldType.BaseType;
            if (baseType.IsGenericType)
            {
                var genericType = baseType.GetGenericTypeDefinition();
                if (genericType == typeof(UnityEngine.Events.UnityEvent<>))
                
                    yield return fieldInfo;
            }
            else
            {
                if ( baseType == typeof(UnityEngine.Events.UnityEvent))
                    yield return fieldInfo;

            }

        }
    }

    private IEnumerable<MethodInfo> _GetMethods(Type adsorber_type)
    {
        foreach (var methodInfo in adsorber_type.GetMethods())
        {
            if (methodInfo.IsPublic && methodInfo.IsStatic == false && methodInfo.IsSpecialName == false)
                yield return methodInfo;
        }
    }

    IEnumerable<MethodInfo> _GetSourceMethods(Type type)
    {

        
        var methods = type.GetMethods();

        foreach (var methodInfo in methods)
        {
            if (methodInfo.IsPublic && methodInfo.IsStatic == false)
            {
                yield return methodInfo;
            }
        }
    }

    private void _DrawGPI<TGPI>(TGPI gpi)
    {
        if (gpi == null)
        {
            EditorGUILayout.LabelField("Empty"); 
            return;
        }

        var type = typeof (TGPI);
        foreach (var propertyInfo in type.GetProperties())
        {
            if (propertyInfo.PropertyType.IsArray)
            {
                var collection = propertyInfo.PropertyType as ICollection;
                EditorGUILayout.LabelField(propertyInfo.Name , "count" + collection.Count);
                /*for (int i = 0; i < collection.Count; i++)
                {
                    EditorGUILayout.LabelField(propertyInfo.GetValue(gpi, new object[] {i} ).ToString());
                }*/
            }
            else
            {
                
                EditorGUILayout.LabelField(propertyInfo.Name , propertyInfo.GetValue(gpi, null).ToString());
            }
                
        }

    }
}

