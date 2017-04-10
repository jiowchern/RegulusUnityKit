using System;

using Regulus.Utility;

internal class AdsorptionGeneratorWaitCompile : IStage,   IGUIDrawer
{

    public event Action DoneEvent; 
    void IStage.Enter()
    {
                
    }

    void IStage.Leave()
    {
        
    }

    void IStage.Update()
    {
        if (UnityEditor.EditorApplication.isCompiling == false)
        {
            DoneEvent();
        }
    }

    void IGUIDrawer.Draw()
    {
        
    }
}