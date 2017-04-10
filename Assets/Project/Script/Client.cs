using System;
using System.Collections;
using System.Collections.Generic;

using Regulus.Remoting;
using Regulus.Utility;

using UnityEngine;

public class Client : Regulus.Remoting.Unity.Distributor
{

	public GameObject ConnectUI;
	public UnityEngine.UI.Text IPAddress;
	public UnityEngine.UI.Text Port;

	private readonly Regulus.Utility.Updater _Updater;

	private readonly Regulus.Remoting.IAgent _Agent;
	public Client()
	{	    
		_Updater = new Updater();
		var protocol = new Regulus.Project.Chat.Protocol() as Regulus.Remoting.IProtocol;
		_Agent = Regulus.Remoting.Ghost.Native.Agent.Create(protocol.GetGPIProvider());
	}

	void Start()
	{
		_Updater.Add(_Agent);
	}
	// Use this for initialization
	public void Connect ()
	{        
		int port;
		if (int.TryParse(Port.text, out port))
		{
			_Agent.Connect(IPAddress.text, port).OnValue += _ConnectResult;            
		}
	}

	internal override IAgent _GetAgent()
	{
		return _Agent;        
	}

	void OnDestroy()
	{
		_Updater.Shutdown();
	}

	private void _ConnectResult(bool success)
	{
		ConnectUI.SetActive(!success);
	}

	// Update is called once per frame
	void Update ()
	{
		_Updater.Working();
	}
}
