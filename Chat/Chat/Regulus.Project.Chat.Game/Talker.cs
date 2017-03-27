using Regulus.Framework;
using Regulus.Remoting;
using Regulus.Utility;

namespace Regulus.Project.Chat.Game
{
    internal class Talker : IUpdatable
    {
        private readonly ISoulBinder _SoulBinder;

        private readonly Room _Room;

        private readonly Regulus.Utility.StageMachine _Machine;


        private bool _Enable;
        public Talker(ISoulBinder soul_binder, Room room)
        {
            _Machine = new StageMachine();
            _SoulBinder = soul_binder;
            _Room = room;
            _Enable = true;
            _SoulBinder.BreakEvent += () => { _Enable = false; };
        }

        void IBootable.Launch()
        {
            _ToLogin();
        }

        private void _ToLogin()
        {
            var stage = new TalkerLoginStage(_SoulBinder);
            stage.DoneEvent += _ToChat;
            _Machine.Push(stage);
        }

        private void _ToChat(string name)
        {
            var stage = new TalkerChatStage(_SoulBinder , _Room , name);
            stage.DoneEvent += _ToLogin;
            _Machine.Push(stage);
        }

        void IBootable.Shutdown()
        {
            _Machine.Termination();
        }

        bool IUpdatable.Update()
        {
            _Machine.Update();
            return _Enable;
        }
    }
}