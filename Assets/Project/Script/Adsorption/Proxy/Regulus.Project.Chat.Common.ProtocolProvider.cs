
            using System;  
            using System.Collections.Generic;
            
            namespace Regulus.Project.Chat.Common{ 
                public class ProtocolProvider : Regulus.Remoting.IProtocol
                {
                    Regulus.Remoting.GPIProvider _GPIProvider;
                    Regulus.Remoting.EventProvider _EventProvider;
                    Regulus.Serialization.ISerializer _Serializer;
                    public ProtocolProvider()
                    {
                        var types = new Dictionary<Type, Type>();
                        types.Add(typeof(Regulus.Project.Chat.Common.IAccount) , typeof(Regulus.Project.Chat.Common.Ghost.CIAccount) );
types.Add(typeof(Regulus.Project.Chat.Common.IPlayer) , typeof(Regulus.Project.Chat.Common.Ghost.CIPlayer) );
types.Add(typeof(Regulus.Project.Chat.Common.ITalker) , typeof(Regulus.Project.Chat.Common.Ghost.CITalker) );
                        _GPIProvider = new Regulus.Remoting.GPIProvider(types);

                        var eventClosures = new List<Regulus.Remoting.IEventProxyCreator>();
                        eventClosures.Add(new Regulus.Project.Chat.Common.Event.ITalker.MessageEvent() );
                        _EventProvider = new Regulus.Remoting.EventProvider(eventClosures);

                        _Serializer = new Regulus.Serialization.Serializer(new Regulus.Serialization.DescriberBuilder(typeof(System.String),typeof(System.Boolean)));
                    }


                    Regulus.Remoting.GPIProvider Regulus.Remoting.IProtocol.GetGPIProvider()
                    {
                        return _GPIProvider;
                    }

                    Regulus.Remoting.EventProvider Regulus.Remoting.IProtocol.GetEventProvider()
                    {
                        return _EventProvider;
                    }

                    Regulus.Serialization.ISerializer Regulus.Remoting.IProtocol.GetSerialize()
                    {
                        return _Serializer;
                    }
                    
                }
            }
            