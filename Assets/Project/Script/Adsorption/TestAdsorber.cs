                    
namespace Regulus.Project.Chat.Common.Adsorption
{
    
    public class TestAdsorber : Regulus.Remoting.Unity.Adsorber<ITest>
    {
        [System.Serializable]
        public class UnityEnableEvent : UnityEngine.Events.UnityEvent<bool> {}
        public UnityEnableEvent EnableEvent;
        ITest _Test;                        
        public TestAdsorber()
        {
                                
        }

        public override ITest GetGPI()
        {
            return _Test;
        }
        public override void Supply(ITest gpi)
        {
            _Test = gpi;
            _Test.Event1 += _OnEvent1;
            EnableEvent.Invoke(true);
        }

        public override void Unsupply(ITest gpi)
        {
            EnableEvent.Invoke(false);
            _Test.Event1 -= _OnEvent1;
            _Test = null;
        }
        
        public void Method1()
        {
            if(_Test != null)
            {
                _Test.Method1();
            }
        }

        public void Method2()
        {
            if(_Test != null)
            {
                _Test.Method2().OnValue += ( result ) =>{ Method2Result.Invoke(result);};
            }
        }
        
        [System.Serializable]
        public class UnityMethod2Result : UnityEngine.Events.UnityEvent<System.Int32> { }
        public UnityMethod2Result Method2Result;
        
        
        [System.Serializable]
        public class UnityEvent1 : UnityEngine.Events.UnityEvent<System.Int32,System.Single> { }
        public UnityEvent1 Event1;
        
        
        private void _OnEvent1(System.Int32 arg0,System.Single arg1)
        {
            Event1.Invoke(arg0,arg1);
        }
        
    }
}
                    