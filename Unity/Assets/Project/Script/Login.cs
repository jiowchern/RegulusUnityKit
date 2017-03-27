using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Login : MonoBehaviour {

    public UnityEngine.UI.Text Name;
    public Regulus.Project.Chat.Common.Adsorber.AdsorberIAccount Account;
	
    public void Verify()
    {
        Account.Login(Name.text);
        
    }
	
}
