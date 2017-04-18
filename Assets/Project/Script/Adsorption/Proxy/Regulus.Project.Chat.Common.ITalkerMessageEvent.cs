
    using System;  
    using System.Collections.Generic;
    
    namespace Regulus.Project.Chat.Common.Event.ITalker 
    { 
        public class MessageEvent : Regulus.Remoting.IEventProxyCreator
        {

            Type _Type;
            string _Name;
            
            public MessageEvent()
            {
                _Name = "MessageEvent";
                _Type = typeof(Regulus.Project.Chat.Common.ITalker);                   
            
            }
            Delegate Regulus.Remoting.IEventProxyCreator.Create(Guid soul_id, Action<Guid, string, object[]> invoke_Event)
            {                
                var closure = new Regulus.Remoting.GenericEventClosure<System.String>(soul_id , _Name , invoke_Event);                
                return new Action<System.String>(closure.Run);
            }
        

            Type Regulus.Remoting.IEventProxyCreator.GetType()
            {
                return _Type;
            }            

            string Regulus.Remoting.IEventProxyCreator.GetName()
            {
                return _Name;
            }            
        }
    }
                