                    

namespace Regulus.Project.Chat.Common.Adsorption
{
    using System.Linq;
        
    public class AccountAdsorber : UnityEngine.MonoBehaviour , Regulus.Remoting.Unity.Adsorber<IAccount>
    {
        private readonly Regulus.Utility.StageMachine _Machine;        
        
        public string Agent;

        private Regulus.Project.Chat.Common.Adsorption.Agent _Agent;

        [System.Serializable]
        public class UnityEnableEvent : UnityEngine.Events.UnityEvent<bool> {}
        public UnityEnableEvent EnableEvent;
        [System.Serializable]
        public class UnitySupplyEvent : UnityEngine.Events.UnityEvent<Regulus.Project.Chat.Common.IAccount> {}
        public UnitySupplyEvent SupplyEvent;
        Regulus.Project.Chat.Common.IAccount _Account;                        
       
        public AccountAdsorber()
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
            _Agent.Distributor.Attach<IAccount>(this);
        }

        private void _DispatchLeave()
        {
            _Agent.Distributor.Detach<IAccount>(this);
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

        public Regulus.Project.Chat.Common.IAccount GetGPI()
        {
            return _Account;
        }
        public void Supply(Regulus.Project.Chat.Common.IAccount gpi)
        {
            _Account = gpi;
            
            EnableEvent.Invoke(true);
            SupplyEvent.Invoke(gpi);
        }

        public void Unsupply(Regulus.Project.Chat.Common.IAccount gpi)
        {
            EnableEvent.Invoke(false);
            
            _Account = null;
        }
        
        public void Login(System.String user_name)
        {
            if(_Account != null)
            {
                _Account.Login(user_name);
            }
        }
        
        
    }
}
                    