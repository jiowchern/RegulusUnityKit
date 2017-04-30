
using System;

using Regulus.Utility;

using UnityEngine;


namespace Regulus.Project.Chat.Common{ 
    public class Agent : MonoBehaviour
    {
        public readonly Regulus.Remoting.Unity.Distributor Distributor;


        private readonly Regulus.Utility.Updater _Updater;

        private readonly Regulus.Remoting.IAgent _Agent;
        public string Name;
        public Agent()
        {
            var protocol = new Regulus.Project.Chat.Common.Provider() as Regulus.Remoting.IProtocol;
            _Agent = Regulus.Remoting.Ghost.Native.Agent.Create(protocol.GetGPIProvider() , protocol.GetSerialize());
            Distributor = new Regulus.Remoting.Unity.Distributor(_Agent);
            _Updater = new Updater();

        }

        void Start()   
        {
            _Updater.Add(_Agent);
        }
        // Use this for initialization
        public void Connect(string ip,int port)
        {
            _Agent.Connect(ip, port).OnValue += _ConnectResult;
        }

        private void _ConnectResult(bool obj)
        {
            ConnectEvent.Invoke(obj);
        }

        void OnDestroy()
        {
            _Updater.Shutdown();
        }

       
        // Update is called once per frame
        void Update()
        {
            _Updater.Working();
        }
        [Serializable]
        public class UnityAgentConnectEvent : UnityEngine.Events.UnityEvent<bool>{}

        public UnityAgentConnectEvent ConnectEvent;
    }
}
