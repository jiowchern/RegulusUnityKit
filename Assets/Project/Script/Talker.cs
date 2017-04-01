using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Regulus.Project.Chat.Common;

using UnityEngine;

public class Talker : UnityEngine.MonoBehaviour
{
    public Chat Chat;

    public string Name;

    public void Supply(ITalker talker)
    {
        Name = talker.Name;
        Chat.Join(talker.Name);
    }

    public void Enable(bool enable)
    {
        if (enable == false)
        {
            Chat.Leave(Name);
            _Leave();
        }
        
    }


    void _Leave()
    {
        GameObject.Destroy(gameObject);
    }

    public void Message(string message)
    {
        Chat.Message(Name , message);
    }
}
