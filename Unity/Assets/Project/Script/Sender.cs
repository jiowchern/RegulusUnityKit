using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sender : MonoBehaviour {

    public UnityEngine.UI.Text Message;
    public Regulus.Project.Chat.Common.Adsorber.AdsorberITalker Talker;
	public void Send()
    {
        Talker.Talk(Message.text);
    }
}
