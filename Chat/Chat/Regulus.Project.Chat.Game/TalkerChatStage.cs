using System;

using Regulus.Project.Chat.Common;
using Regulus.Remoting;
using Regulus.Utility;

namespace Regulus.Project.Chat.Game
{
    internal class TalkerChatStage : IStage , Common.ITalker
    {
        private readonly ISoulBinder _SoulBinder;

        private readonly Room _Room;

        private readonly string _Name;

        public Action DoneEvent;

        public TalkerChatStage(ISoulBinder soul_binder, Room room , string name)
        {
            _SoulBinder = soul_binder;
            _Room = room;
            _Name = name;
        }

        void IStage.Enter()
        {            
            _SoulBinder.Bind<ITalker>(this);
            _Room.MessageEvent += _MessageEvent;
        }

        void IStage.Leave()
        {
            _Room.MessageEvent -= _MessageEvent;
            _SoulBinder.Unbind<ITalker>(this);
        }

        void IStage.Update()
        {
            
        }

        private event Action<string, string> _MessageEvent;
        event Action<string, string> ITalker.MessageEvent
        {
            add { _MessageEvent += value; }
            remove { _MessageEvent -= value; }
        }

        Regulus.Remoting.Value<bool> ITalker.Talk(string message)
        {
            _Room.Talk(_Name , message);

            return true;
        }

        public void Exit()
        {
            DoneEvent();
        }
    }
}