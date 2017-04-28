using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sender : MonoBehaviour {

    public UnityEngine.UI.Text Message;
    public Regulus.Project.Chat.Common.Adsorption.PlayerAdsorber Player;
	public void Send()
    {
        Player.Talk(Message.text);
    }
}
