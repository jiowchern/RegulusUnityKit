using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

using NUnit.Framework;

using Regulus.Project.Chat.Common;
using Regulus.Project.Chat.Common.Adsorption;
using Regulus.Remoting.Unity;

using UnityEditor;

using UnityEngine;

using Object = UnityEngine.Object;

[CustomEditor(typeof(TalkerAdsorber))]
public  class TalkerAdsorberInspector : UnityEditor.Editor
{
    private Object _AttachObject;

    public override void OnInspectorGUI()
    {

        _DrawBinder();
        var adsorber = this.target as Adsorber<ITalker>;
        var gpi = adsorber.GetGPI();        
        _DrawGPI(gpi);

        DrawDefaultInspector();
    }

    private void _DrawBinder()
    {
        EditorGUILayout.BeginHorizontal();
        
        var attachObject = EditorGUILayout.ObjectField("Hook event from", _AttachObject, typeof (GameObject), true);
        
        if (attachObject != null && _AttachObject != attachObject)
        {
            _AttachObject = null;

            var aeb = new AdsorptionEventBinder<TalkerAdsorber>(target as TalkerAdsorber);
            aeb.Bind(attachObject as GameObject);
        }
        else
        {
            _AttachObject = attachObject;
        }
        EditorGUILayout.EndHorizontal();
    }

    

    

    private void _DrawGPI<TGPI>(TGPI gpi)
    {
        if (gpi == null)
        {
            //EditorGUILayout.LabelField("Empty"); 
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

