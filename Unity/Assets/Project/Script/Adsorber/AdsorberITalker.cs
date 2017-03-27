                    
namespace Regulus.Project.Chat.Common.Adsorber
{
    
    public class AdsorberITalker : Regulus.Remoting.Unity.Adsorber<ITalker>
    {
        [System.Serializable]
        public class UnityEnableEvent : UnityEngine.Events.UnityEvent<bool> {}
        public UnityEnableEvent EnableEvent;
        ITalker _ITalker;                        
        public AdsorberITalker()
        {
                                
        }

        public override void Supply(ITalker gpi)
        {
            _ITalker = gpi;
            _ITalker.MessageEvent += _OnMessageEvent;
            EnableEvent.Invoke(true);
        }

        public override void Unsupply(ITalker gpi)
        {
            EnableEvent.Invoke(false);
            _ITalker.MessageEvent -= _OnMessageEvent;
            _ITalker = null;
        }
        
        public void Talk(System.String message)
        {
            if(_ITalker != null)
            {
                _ITalker.Talk(message).OnValue += ( result ) =>{ TalkResult.Invoke(result);};
            }
        }

        public void Exit()
        {
            if(_ITalker != null)
            {
                _ITalker.Exit();
            }
        }
        
        [System.Serializable]
        public class UnityTalkResult : UnityEngine.Events.UnityEvent<System.Boolean> { }
        public UnityTalkResult TalkResult;
        
        
        [System.Serializable]
        public class UnityMessageEvent : UnityEngine.Events.UnityEvent<System.String,System.String> { }
        public UnityMessageEvent MessageEvent;
        
        
        private void _OnMessageEvent(System.String arg0,System.String arg1)
        {
            MessageEvent.Invoke(arg0,arg1);
        }
        
    }
}
                    