using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.Chat.Common
{
    public interface ITest
    {
        void Method1();
        Regulus.Remoting.Value<int> Method2();

        event Action<int, float> Event1;
    }
}
