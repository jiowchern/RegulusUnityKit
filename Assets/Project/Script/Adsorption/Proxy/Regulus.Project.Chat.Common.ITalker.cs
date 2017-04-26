   
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
            readonly Regulus.Serialization.ISerializer _Serializer ;
            public CITalker(Regulus.Remoting.IGhostRequest requester , Guid id,Regulus.Remoting.ReturnValueQueue queue, bool have_return , Regulus.Serialization.ISerializer serializer)
            {
                _Serializer = serializer;

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
            

                System.String _Name;
                System.String Regulus.Project.Chat.Common.ITalker.Name { get{ return _Name;} }

                System.Action<System.String> _MessageEvent;
                event System.Action<System.String> Regulus.Project.Chat.Common.ITalker.MessageEvent
                {
                    add { _MessageEvent += value;}
                    remove { _MessageEvent -= value;}
                }
                
            
        }
    }
