

using Regulus.Project.Chat.Common;
//using Regulus.Project.Chat.Common.Adsorption;
using Regulus.Remoting.Unity;

using UnityEngine;


public class Talker : UnityEngine.MonoBehaviour
{
    public Chat Chat;

    public string Name;


    //[Adsorber(typeof(TalkerAdsorber), "SupplyEvent")]
    public void Supply(ITalker talker)
    {        
        Name = talker.Name;
        Chat.Join(talker.Name);
    }


    //[Adsorber(typeof(TalkerAdsorber), "EnableEvent")]
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
    //[Adsorber(typeof(TalkerAdsorber) , "MessageEvent")]
    public void Message(string message)
    {
        Chat.Message(Name , message);
    }


    //[Adsorber(typeof(TalkerAdsorber), "TestEvent")]
    public void Test(int a)
    {
        //Chat.Message(Name , message);
    }
}
