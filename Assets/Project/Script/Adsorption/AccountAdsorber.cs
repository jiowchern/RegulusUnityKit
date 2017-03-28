                    
namespace Regulus.Project.Chat.Common.Adsorption
{
    
    public class AccountAdsorber : Regulus.Remoting.Unity.Adsorber<IAccount>
    {
        [System.Serializable]
        public class UnityEnableEvent : UnityEngine.Events.UnityEvent<bool> {}
        public UnityEnableEvent EnableEvent;
        IAccount _Account;                        
        public AccountAdsorber()
        {
                                
        }

        public override IAccount GetGPI()
        {
            return _Account;
        }
        public override void Supply(IAccount gpi)
        {
            _Account = gpi;
            
            EnableEvent.Invoke(true);
        }

        public override void Unsupply(IAccount gpi)
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
                    