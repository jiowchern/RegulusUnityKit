using System;

namespace Regulus.Project.Chat.Game
{
    internal class Room
    {
        public Action<string, string> MessageEvent;

        public void Talk(string name, string message)
        {
            MessageEvent(name , message);
        }
    }
}