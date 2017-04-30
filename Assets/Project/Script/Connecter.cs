using System.Collections;
using System.Collections.Generic;



using UnityEngine;

public class Connecter : MonoBehaviour
{

    public UnityEngine.UI.Text IPAddress;
    public UnityEngine.UI.Text Port;
    public Regulus.Project.Chat.Common.Agent Agent;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Connect()
    {
        int port;
        if(int.TryParse(Port.text , out port))
            Agent.Connect(IPAddress.text , port);
    }
}
