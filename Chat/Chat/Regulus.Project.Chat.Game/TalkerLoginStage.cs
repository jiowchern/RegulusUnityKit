using System;
using System.Security.Policy;

using Regulus.Project.Chat.Common;
using Regulus.Remoting;
using Regulus.Utility;

namespace Regulus.Project.Chat.Game
{
    internal class TalkerLoginStage : IStage , Common.IAccount
    {
        private readonly ISoulBinder _SoulBinder;

        public Action<string> DoneEvent;

        public TalkerLoginStage(ISoulBinder soul_binder)
        {
            _SoulBinder = soul_binder;
        }

        void IStage.Enter()
        {
            _SoulBinder.Bind<IAccount>(this);
        }

        void IStage.Leave()
        {
            _SoulBinder.Unbind<IAccount>(this);
        }

        void IStage.Update()
        {
            
        }

        void IAccount.Login(string user_name)
        {
            DoneEvent(user_name);            
        }
    }
}