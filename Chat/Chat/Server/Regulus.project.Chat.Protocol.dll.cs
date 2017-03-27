   
    using System;  
    
    using System.Collections.Generic;
    
    namespace Regulus.Project.Chat.Common.Ghost 
    { 
        public class CIAccount : Regulus.Project.Chat.Common.IAccount , Regulus.Remoting.IGhost
        {
            bool _HaveReturn ;
            Regulus.Remoting.IGhostRequest _Requester;
            Guid _GhostIdName;
            Regulus.Remoting.ReturnValueQueue _Queue;
            public CIAccount(Regulus.Remoting.IGhostRequest requester , Guid id,Regulus.Remoting.ReturnValueQueue queue, bool have_return )
            {
                _Requester = requester;
                _HaveReturn = have_return ;
                _GhostIdName = id;
                _Queue = queue;
            }

            void Regulus.Remoting.IGhost.OnEvent(string name_event, byte[][] args)
            {
                Regulus.Remoting.AgentCore.CallEvent(name_event , "CIAccount" , this , args);
            }

            Guid Regulus.Remoting.IGhost.GetID()
            {
                return _GhostIdName;
            }

            bool Regulus.Remoting.IGhost.IsReturnType()
            {
                return _HaveReturn;
            }

            void Regulus.Remoting.IGhost.OnProperty(string name, byte[] value)
            {
                Regulus.Remoting.AgentCore.UpdateProperty(name , "CIAccount" , this , value);
            }
            
                void Regulus.Project.Chat.Common.IAccount.Login(System.String user_name)
                {                    

                        
                    var data = new Regulus.Remoting.PackageCallMethod();
                    data.EntityId = _GhostIdName;
                    data.MethodName ="Login";
                    
                    var paramList = new System.Collections.Generic.List<byte[]>();

    var user_nameBytes = Regulus.TypeHelper.Serializer<System.String>(user_name);    
    paramList.Add(user_nameBytes);

data.MethodParams = paramList.ToArray();
                    _Requester.Request(Regulus.Remoting.ClientToServerOpCode.CallMethod , data.ToBuffer());

                    
                }



            
        }
    }

   
    using System;  
    
    using System.Collections.Generic;
    
    namespace Regulus.Project.Chat.Common.Ghost 
    { 
        public class CITalker : Regulus.Project.Chat.Common.ITalker , Regulus.Remoting.IGhost
        {
            bool _HaveReturn ;
            Regulus.Remoting.IGhostRequest _Requester;
            Guid _GhostIdName;
            Regulus.Remoting.ReturnValueQueue _Queue;
            public CITalker(Regulus.Remoting.IGhostRequest requester , Guid id,Regulus.Remoting.ReturnValueQueue queue, bool have_return )
            {
                _Requester = requester;
                _HaveReturn = have_return ;
                _GhostIdName = id;
                _Queue = queue;
            }

            void Regulus.Remoting.IGhost.OnEvent(string name_event, byte[][] args)
            {
                Regulus.Remoting.AgentCore.CallEvent(name_event , "CITalker" , this , args);
            }

            Guid Regulus.Remoting.IGhost.GetID()
            {
                return _GhostIdName;
            }

            bool Regulus.Remoting.IGhost.IsReturnType()
            {
                return _HaveReturn;
            }

            void Regulus.Remoting.IGhost.OnProperty(string name, byte[] value)
            {
                Regulus.Remoting.AgentCore.UpdateProperty(name , "CITalker" , this , value);
            }
            
                Regulus.Remoting.Value<System.Boolean> Regulus.Project.Chat.Common.ITalker.Talk(System.String message)
                {                    

                        
                    var data = new Regulus.Remoting.PackageCallMethod();
                    data.EntityId = _GhostIdName;
                    data.MethodName ="Talk";
                    
    var returnValue = new Regulus.Remoting.Value<System.Boolean>();
    var returnId = _Queue.PushReturnValue(returnValue);    
    data.ReturnId = returnId;

                    var paramList = new System.Collections.Generic.List<byte[]>();

    var messageBytes = Regulus.TypeHelper.Serializer<System.String>(message);    
    paramList.Add(messageBytes);

data.MethodParams = paramList.ToArray();
                    _Requester.Request(Regulus.Remoting.ClientToServerOpCode.CallMethod , data.ToBuffer());

                    return returnValue;
                }
 

                void Regulus.Project.Chat.Common.ITalker.Exit()
                {                    

                        
                    var data = new Regulus.Remoting.PackageCallMethod();
                    data.EntityId = _GhostIdName;
                    data.MethodName ="Exit";
                    
                    var paramList = new System.Collections.Generic.List<byte[]>();

data.MethodParams = paramList.ToArray();
                    _Requester.Request(Regulus.Remoting.ClientToServerOpCode.CallMethod , data.ToBuffer());

                    
                }



                System.Action<System.String,System.String> _MessageEvent;
                event System.Action<System.String,System.String> Regulus.Project.Chat.Common.ITalker.MessageEvent
                {
                    add { _MessageEvent += value;}
                    remove { _MessageEvent -= value;}
                }
                
            
        }
    }


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
                var closure = new Regulus.Remoting.GenericEventClosure<System.String,System.String>(soul_id , _Name , invoke_Event);                
                return new Action<System.String,System.String>(closure.Run);
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
                

            using System;  
            using System.Collections.Generic;
            
            namespace Regulus.Project.Chat{ 
                public class Protocol : Regulus.Remoting.IProtocol
                {
                    Regulus.Remoting.GPIProvider _GPIProvider;
                    Regulus.Remoting.EventProvider _EventProvider;
                    public Protocol()
                    {
                        var types = new Dictionary<Type, Type>();
                        types.Add(typeof(Regulus.Project.Chat.Common.IAccount) , typeof(Regulus.Project.Chat.Common.Ghost.CIAccount) );
types.Add(typeof(Regulus.Project.Chat.Common.ITalker) , typeof(Regulus.Project.Chat.Common.Ghost.CITalker) );
                        _GPIProvider = new Regulus.Remoting.GPIProvider(types);

                        var eventClosures = new List<Regulus.Remoting.IEventProxyCreator>();
                        eventClosures.Add(new Regulus.Project.Chat.Common.Event.ITalker.MessageEvent() );
                        _EventProvider = new Regulus.Remoting.EventProvider(eventClosures);
                    }


                    Regulus.Remoting.GPIProvider Regulus.Remoting.IProtocol.GetGPIProvider()
                    {
                        return _GPIProvider;
                    }

                    Regulus.Remoting.EventProvider Regulus.Remoting.IProtocol.GetEventProvider()
                    {
                        return _EventProvider;
                    }
                    
                }
            }
            
