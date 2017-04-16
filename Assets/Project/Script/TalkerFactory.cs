using System.Collections;
using System.Collections.Generic;

using Regulus.Project.Chat.Common;

using UnityEngine;

public class TalkerFactory : MonoBehaviour
{
    public Chat Chat;
    public GameObject TalkerSource;
    

    public void Supply(ITalker gpi)
    {
        var obj = GameObject.Instantiate(TalkerSource);
        var talker = obj.GetComponent<Talker>();
        talker.Chat = this.Chat;
    }
}
