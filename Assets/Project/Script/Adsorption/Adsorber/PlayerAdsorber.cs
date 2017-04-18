                    

namespace Regulus.Project.Chat.Common.Adsorption
{
    using System.Linq;
        
    public class PlayerAdsorber : UnityEngine.MonoBehaviour , Regulus.Remoting.Unity.Adsorber<IPlayer>
    {
        private readonly Regulus.Utility.StageMachine _Machine;        
        
        public string Agent;

        private Regulus.Project.Chat.Common.Adsorption.Agent _Agent;

        [System.Serializable]
        public class UnityEnableEvent : UnityEngine.Events.UnityEvent<bool> {}
        public UnityEnableEvent EnableEvent;
        [System.Serializable]
        public class UnitySupplyEvent : UnityEngine.Events.UnityEvent<Regulus.Project.Chat.Common.IPlayer> {}
        public UnitySupplyEvent SupplyEvent;
        Regulus.Project.Chat.Common.IPlayer _Player;                        
       
        public PlayerAdsorber()
        {
            _Machine = new Regulus.Utility.StageMachine();
        }

        void Start()
        {
            _Machine.Push(new Regulus.Utility.SimpleStage(_ScanEnter, _ScanLeave, _ScanUpdate));
        }

        private void _ScanUpdate()
        {
            var agents = UnityEngine.GameObject.FindObjectsOfType<Regulus.Project.Chat.Common.Adsorption.Agent>();
            _Agent = agents.FirstOrDefault(d => string.IsNullOrEmpty(d.Name) == false && d.Name == Agent);
            if(_Agent != null)
            {
                _Machine.Push(new Regulus.Utility.SimpleStage(_DispatchEnter, _DispatchLeave));
            }            
        }

        private void _DispatchEnter()
        {
            _Agent.Distributor.Attach<IPlayer>(this);
        }

        private void _DispatchLeave()
        {
            _Agent.Distributor.Detach<IPlayer>(this);
        }

        private void _ScanLeave()
        {

        }


        private void _ScanEnter()
        {

        }

        void Update()
        {
            _Machine.Update();
        }

        void OnDestroy()
        {
            _Machine.Termination();
        }

        public Regulus.Project.Chat.Common.IPlayer GetGPI()
        {
            return _Player;
        }
        public void Supply(Regulus.Project.Chat.Common.IPlayer gpi)
        {
            _Player = gpi;
            
            EnableEvent.Invoke(true);
            SupplyEvent.Invoke(gpi);
        }

        public void Unsupply(Regulus.Project.Chat.Common.IPlayer gpi)
        {
            EnableEvent.Invoke(false);
            
            _Player = null;
        }
        
        public void Talk(System.String message)
        {
            if(_Player != null)
            {
                _Player.Talk(message).OnValue += ( result ) =>{ TalkResult.Invoke(result);};
            }
        }

        public void Exit()
        {
            if(_Player != null)
            {
                _Player.Exit();
            }
        }
        
        [System.Serializable]
        public class UnityTalkResult : UnityEngine.Events.UnityEvent<System.Boolean> { }
        public UnityTalkResult TalkResult;
        
        
    }
}
                    