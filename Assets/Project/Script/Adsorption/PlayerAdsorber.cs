                    
namespace Regulus.Project.Chat.Common.Adsorption
{
    
    public class PlayerAdsorber : Regulus.Remoting.Unity.Adsorber<IPlayer>
    {
        [System.Serializable]
        public class UnityEnableEvent : UnityEngine.Events.UnityEvent<bool> {}
        public UnityEnableEvent EnableEvent;
        [System.Serializable]
        public class UnitySupplyEvent : UnityEngine.Events.UnityEvent<IPlayer> {}
        public UnitySupplyEvent SupplyEvent;
        IPlayer _Player;                        
        public PlayerAdsorber()
        {
                                
        }

        public override IPlayer GetGPI()
        {
            return _Player;
        }
        public override void Supply(IPlayer gpi)
        {
            _Player = gpi;
            
            EnableEvent.Invoke(true);
            SupplyEvent.Invoke(gpi);
        }

        public override void Unsupply(IPlayer gpi)
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
                    