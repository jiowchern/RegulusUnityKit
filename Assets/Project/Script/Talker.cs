

using Regulus.Project.Chat.Common;
using Regulus.Project.Chat.Common.Adsorption;
using Regulus.Remoting.Unity;

using UnityEngine;


public class Talker : UnityEngine.MonoBehaviour
{
    public Chat Chat;

    public string Name;


    [AdsorberListener(typeof(TalkerAdsorber), "SupplyEvent")]
    public void Supply(ITalker talker)
    {        
        Name = talker.Name;
        Chat.Join(talker.Name);
    }


    [AdsorberListener(typeof(TalkerAdsorber), "EnableEvent")]
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
    [AdsorberListener(typeof(TalkerAdsorber) , "MessageEvent")]
    public void Message(string message)
    {
        Chat.Message(Name , message);
    }


    [AdsorberListener(typeof(TalkerAdsorber), "TestEvent")]
    public void Test(int a)
    {
        //Chat.Message(Name , message);
    }
}
