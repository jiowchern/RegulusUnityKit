using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.Chat.Common
{
    public interface ITalker
    {
        event System.Action<string, string> MessageEvent;
        Regulus.Remoting.Value<bool> Talk(string message);

        void Exit();
    }
}
