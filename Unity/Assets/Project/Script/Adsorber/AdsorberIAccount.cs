                    
namespace Regulus.Project.Chat.Common.Adsorber
{
    
    public class AdsorberIAccount : Regulus.Remoting.Unity.Adsorber<IAccount>
    {
        [System.Serializable]
        public class UnityEnableEvent : UnityEngine.Events.UnityEvent<bool> {}
        public UnityEnableEvent EnableEvent;
        IAccount _IAccount;                        
        public AdsorberIAccount()
        {
                                
        }

        public override void Supply(IAccount gpi)
        {
            _IAccount = gpi;
            
            EnableEvent.Invoke(true);
        }

        public override void Unsupply(IAccount gpi)
        {
            EnableEvent.Invoke(false);
            
            _IAccount = null;
        }
        
        public void Login(System.String user_name)
        {
            if(_IAccount != null)
            {
                _IAccount.Login(user_name);
            }
        }
        
        
    }
}
                    