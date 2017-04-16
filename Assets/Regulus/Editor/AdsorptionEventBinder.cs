using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

using UnityEngine;

namespace Regulus.Remoting.Unity
{

    [ExecuteInEditMode]
    public class AdsorptionEventBinder<T> 
    {
        private readonly T _Source;

        public AdsorptionEventBinder(T source)
        {
            _Source = source;
        }

        
        public void Bind(GameObject attach_object)
        {
            var adsorberType = typeof(T);

            var sourceInstance = attach_object;
            var components = sourceInstance.GetComponents<Component>();
            foreach (var component in components)
            {
                var type = component.GetType();



                foreach (var sourceMethod in _GetSourceMethods(type))
                {
                    foreach (AdsorberAttribute adsorberListenerAttributese in sourceMethod.GetCustomAttributes(typeof(AdsorberAttribute), false))
                    {

                        if (adsorberListenerAttributese.Adsorber != adsorberType)
                        {
                            continue;
                        }
                        _AttachEvent(adsorberListenerAttributese.Target, sourceMethod, component);
                    }


                }

            }

        }

        private void _AttachEvent(string event_name, MethodInfo source_method, Component source_instance)
        {
            var info = _FindUnityEventField(event_name, source_method, source_instance);
            if (info == null)
                return;

            var targetFieldInstance = info.GetValue(_Source);
            
            var argTypes = source_method.GetParameters().Select(e => e.ParameterType).ToArray();
            var unityAction = _CreateUnityAction(source_instance,source_method, argTypes);

            var addMethod = typeof(UnityEditor.Events.UnityEventTools).GetMethods().Where(m => m.Name == "AddPersistentListener" && m.GetGenericArguments().Length == argTypes.Length && m.GetParameters().Length == 2).Select(m => m).First();
            if (addMethod.IsGenericMethod)
            {
                var callMethod = addMethod.MakeGenericMethod(argTypes);
                callMethod.Invoke(null,new object[]{targetFieldInstance,unityAction});
            }
            else
            {
                var callMethod = addMethod;
                callMethod.Invoke(null,new object[]{targetFieldInstance,unityAction});
            }

        }

        private static FieldInfo _FindUnityEventField(string event_name , MethodInfo source_method , Component source_instance)
        {
            var adsorberType = typeof (T);
            var info = (from i in _GetEvents(adsorberType) where i.Name == event_name select i).FirstOrDefault();

            if (info == null)
            {
                Debug.LogErrorFormat("Could not find method name {1}.{0}.", event_name, adsorberType.Name);
                return null;
            }

            var eventArgs = info.FieldType.GetMethod("Invoke").GetParameters();
            var methodArgs = source_method.GetParameters();

            if (methodArgs.Length != eventArgs.Length)
            {
                Debug.LogErrorFormat("Parameter matching error. Please check length.{0}.{1} and {3}.{2}",adsorberType.Name,info.Name,source_method.Name,source_instance.GetType().Name);
                return null;
            }


            for (int i = 0; i < methodArgs.Length; i++)
            {
                if (methodArgs[i].ParameterType != eventArgs[i].ParameterType)
                {
                    Debug.LogErrorFormat("Parameter matching error. Please check type. (Adsorber){0}.{1} (Source){3}.{2}", adsorberType.Name, info.Name, source_method.Name, source_instance.GetType().Name);
                    return null;
                }
            }
            
            return info;
        }

        private System.Delegate _CreateUnityAction(Component source_instance, MethodInfo source_method, Type[] event_args)
        {
            var type = _GetUnityActionType(event_args.Length);

            if (event_args.Length == 0)
                return Delegate.CreateDelegate(type, source_instance, source_method);
            else
            {
                return Delegate.CreateDelegate(type.MakeGenericType(event_args), source_instance, source_method);
            }
        }

        private Type _GetUnityActionType(int length)
        {
            if (length == 0)
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

        private static IEnumerable<FieldInfo> _GetEvents(Type adsorber_type)
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
                    if (baseType == typeof(UnityEngine.Events.UnityEvent))
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
    }
}
