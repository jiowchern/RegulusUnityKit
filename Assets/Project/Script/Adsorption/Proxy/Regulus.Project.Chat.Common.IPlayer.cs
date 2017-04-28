   
    using System;  
    
    using System.Collections.Generic;
    
    namespace Regulus.Project.Chat.Common.Ghost 
    { 
        public class CIPlayer : Regulus.Project.Chat.Common.IPlayer , Regulus.Remoting.IGhost
        {
            bool _HaveReturn ;
            Regulus.Remoting.IGhostRequest _Requester;
            Guid _GhostIdName;
            Regulus.Remoting.ReturnValueQueue _Queue;
            readonly Regulus.Serialization.ISerializer _Serializer ;
            public CIPlayer(Regulus.Remoting.IGhostRequest requester , Guid id,Regulus.Remoting.ReturnValueQueue queue, bool have_return , Regulus.Serialization.ISerializer serializer)
            {
                _Serializer = serializer;

                _Requester = requester;
                _HaveReturn = have_return ;
                _GhostIdName = id;
                _Queue = queue;
            }

            void Regulus.Remoting.IGhost.OnEvent(string name_event, byte[][] args)
            {
                Regulus.Remoting.AgentCore.CallEvent(name_event , "CIPlayer" , this , args, _Serializer);
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
                Regulus.Remoting.AgentCore.UpdateProperty(name , "CIPlayer" , this , value , _Serializer);
            }
            
                Regulus.Remoting.Value<System.Boolean> Regulus.Project.Chat.Common.IPlayer.Talk(System.String message)
                {                    

                        
                    var data = new Regulus.Remoting.PackageCallMethod();
                    data.EntityId = _GhostIdName;
                    data.MethodName ="Talk";
                    
    var returnValue = new Regulus.Remoting.Value<System.Boolean>();
    var returnId = _Queue.PushReturnValue(returnValue);    
    data.ReturnId = returnId;

                    var paramList = new System.Collections.Generic.List<byte[]>();

    var messageBytes = _Serializer.Serialize(message);  
    paramList.Add(messageBytes);

data.MethodParams = paramList.ToArray();
                    _Requester.Request(Regulus.Remoting.ClientToServerOpCode.CallMethod , data.ToBuffer(_Serializer));

                    return returnValue;
                }
 

                void Regulus.Project.Chat.Common.IPlayer.Exit()
                {                    

                        
                    var data = new Regulus.Remoting.PackageCallMethod();
                    data.EntityId = _GhostIdName;
                    data.MethodName ="Exit";
                    
                    var paramList = new System.Collections.Generic.List<byte[]>();

data.MethodParams = paramList.ToArray();
                    _Requester.Request(Regulus.Remoting.ClientToServerOpCode.CallMethod , data.ToBuffer(_Serializer));

                    
                }



            
        }
    }
