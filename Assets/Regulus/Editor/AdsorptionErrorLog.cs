using System;
using System.Text.RegularExpressions;

using Regulus.Utility;

using UnityEditorInternal;

using UnityEngine;
using UnityEngine.EventSystems;

internal class AdsorptionErrorLog : IGUIDrawer , IStage
{
    private readonly AdsorptionWindowCollectLostMethods.Error[] _Errors;

    public Action DoneEvent;

    private Vector2 _Position;

    public AdsorptionErrorLog(AdsorptionWindowCollectLostMethods.Error[] errors)
    {
        _Errors = errors;
        _Position = Vector2.zero;
    }

    void IGUIDrawer.Draw()
    {

        _Position = UnityEditor.EditorGUILayout.BeginScrollView(_Position);
        if (_Errors.Length == 0)
        {
            UnityEngine.GUILayout.Label("No Error");
        }

        for (int i = 0; i < _Errors.Length; i++)
        {
            if (UnityEngine.GUILayout.Button(string.Format("{0}", _Errors[i].Message)))
            {
                _Open(_Errors[i]);
            }
        }
        UnityEditor.EditorGUILayout.EndScrollView();
    }

    private void _Open(AdsorptionWindowCollectLostMethods.Error error)
    {
        var pattern = string.Format(@"\[\s*(Regulus.Remoting.Unity.)?Adsorber(Attribute)?\s*\(\s*typeof\s*\(\s*[\w.]*{0}\s*\)\s*,\s*""{1}""\s*\)\s*\]", error.Type , error.Method );

        var rgx = new Regex(pattern);

        var text = System.IO.File.ReadAllText(error.Path);
        var matchs = rgx.Matches(text);
        
        foreach (Match match in matchs)
        {
            var line = _GetLine(match.Index , text);
            var obj = UnityEditor.AssetDatabase.LoadAssetAtPath(error.Path, typeof (TextAsset));
            UnityEditor.AssetDatabase.OpenAsset(obj, line + 1);
        }
    }

    private int _GetLine(int index, string text)
    {
        int line = 0;
        for (int i = 0; i <= index; i++)
        {
            var c = text[i];
            if (c == '\n')
            {
                line++;
            }
        }        

        return line;
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
}