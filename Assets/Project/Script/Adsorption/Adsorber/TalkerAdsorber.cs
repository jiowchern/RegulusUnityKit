                    

namespace Regulus.Project.Chat.Common.Adsorption
{
    using System.Linq;
        
    public class TalkerAdsorber : UnityEngine.MonoBehaviour , Regulus.Remoting.Unity.Adsorber<ITalker>
    {
        private readonly Regulus.Utility.StageMachine _Machine;        
        
        public string Agent;

        private Regulus.Project.Chat.Common.Adsorption.Agent _Agent;

        [System.Serializable]
        public class UnityEnableEvent : UnityEngine.Events.UnityEvent<bool> {}
        public UnityEnableEvent EnableEvent;
        [System.Serializable]
        public class UnitySupplyEvent : UnityEngine.Events.UnityEvent<Regulus.Project.Chat.Common.ITalker> {}
        public UnitySupplyEvent SupplyEvent;
        Regulus.Project.Chat.Common.ITalker _Talker;                        
       
        public TalkerAdsorber()
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
            _Agent.Distributor.Attach<ITalker>(this);
        }

        private void _DispatchLeave()
        {
            _Agent.Distributor.Detach<ITalker>(this);
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

        public Regulus.Project.Chat.Common.ITalker GetGPI()
        {
            return _Talker;
        }
        public void Supply(Regulus.Project.Chat.Common.ITalker gpi)
        {
            _Talker = gpi;
            _Talker.MessageEvent += _OnMessageEvent;
            EnableEvent.Invoke(true);
            SupplyEvent.Invoke(gpi);
        }

        public void Unsupply(Regulus.Project.Chat.Common.ITalker gpi)
        {
            EnableEvent.Invoke(false);
            _Talker.MessageEvent -= _OnMessageEvent;
            _Talker = null;
        }
        
        
        
        [System.Serializable]
        public class UnityMessageEvent : UnityEngine.Events.UnityEvent<System.String> { }
        public UnityMessageEvent MessageEvent;
        
        
        private void _OnMessageEvent(System.String arg0)
        {
            MessageEvent.Invoke(arg0);
        }
        
    }
}
                    