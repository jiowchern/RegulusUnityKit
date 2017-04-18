   
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
