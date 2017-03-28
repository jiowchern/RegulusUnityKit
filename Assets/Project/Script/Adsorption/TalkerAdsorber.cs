                    
namespace Regulus.Project.Chat.Common.Adsorption
{
    
    public class TalkerAdsorber : Regulus.Remoting.Unity.Adsorber<ITalker>
    {
        [System.Serializable]
        public class UnityEnableEvent : UnityEngine.Events.UnityEvent<bool> {}
        public UnityEnableEvent EnableEvent;
        ITalker _Talker;                        
        public TalkerAdsorber()
        {
                                
        }

        public override ITalker GetGPI()
        {
            return _Talker;
        }
        public override void Supply(ITalker gpi)
        {
            _Talker = gpi;
            _Talker.MessageEvent += _OnMessageEvent;
            EnableEvent.Invoke(true);
        }

        public override void Unsupply(ITalker gpi)
        {
            EnableEvent.Invoke(false);
            _Talker.MessageEvent -= _OnMessageEvent;
            _Talker = null;
        }
        
        public void Talk(System.String message)
        {
            if(_Talker != null)
            {
                _Talker.Talk(message).OnValue += ( result ) =>{ TalkResult.Invoke(result);};
            }
        }

        public void Exit()
        {
            if(_Talker != null)
            {
                _Talker.Exit();
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
                    