using System.Collections;
using System.Collections.Generic;

using Regulus.Project.Chat.Common;

using UnityEngine;

public class TalkerFactory : Regulus.Remoting.Unity.Broadcaster<ITalker>
{
    public Chat Chat;
    public GameObject TalkerSource;
    protected override void _Unsupply(ITalker gpi)
    {
        
    }

    protected override void _Supply(ITalker gpi)
    {
        var obj = GameObject.Instantiate(TalkerSource);
        var talker = obj.GetComponent<Talker>();
        talker.Chat = this.Chat;
    }
}
