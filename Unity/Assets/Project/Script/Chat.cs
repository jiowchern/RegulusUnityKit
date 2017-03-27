using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chat : MonoBehaviour
{
	
	public GameObject ItemPrefab;
	public RectTransform Root;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Message(string talker_name , string message)
	{
		var text = _CreateItem();
		text.text = talker_name + ":" + message;
	}

	public void Send(string message)
	{
		var text = _CreateItem();
		text.text = message;
	}

	private Text _CreateItem()
	{
		var obj = Object.Instantiate(ItemPrefab, Root);
		return obj.GetComponent<UnityEngine.UI.Text>();
	}
}
