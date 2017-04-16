using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Regulus.Remoting.Unity
{
    [ExecuteInEditMode]
    [UnityEditor.InitializeOnLoadAttribute]
    public class AdsorptionStartup
    {
        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            //AdsorptionWindow.OpenCheck();
        }
    }
}
