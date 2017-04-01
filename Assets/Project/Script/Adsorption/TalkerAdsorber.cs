                    
namespace Regulus.Project.Chat.Common.Adsorption
{
    
    public class TalkerAdsorber : Regulus.Remoting.Unity.Adsorber<ITalker>
    {
        [System.Serializable]
        public class UnityEnableEvent : UnityEngine.Events.UnityEvent<bool> {}
        public UnityEnableEvent EnableEvent;
        [System.Serializable]
        public class UnitySupplyEvent : UnityEngine.Events.UnityEvent<ITalker> {}
        public UnitySupplyEvent SupplyEvent;
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
            SupplyEvent.Invoke(gpi);
        }

        public override void Unsupply(ITalker gpi)
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
                    