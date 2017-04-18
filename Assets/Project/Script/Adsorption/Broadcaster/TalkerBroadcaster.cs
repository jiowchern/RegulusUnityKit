
using System;

using System.Linq;

using Regulus.Utility;

using UnityEngine;
using UnityEngine.Events;

namespace Regulus.Project.Chat.Common.Adsorption
{
    public class TalkerBroadcaster : UnityEngine.MonoBehaviour 
    {
        public string Agent;        
        Regulus.Remoting.INotifier<ITalker> _Notifier;

        private readonly Regulus.Utility.StageMachine _Machine;

        public TalkerBroadcaster()
        {
            _Machine = new StageMachine();
        } 

        void Start()
        {
            _ToScan();
        }

        private void _ToScan()
        {
            var stage = new Regulus.Utility.SimpleStage(_ScanEnter , _ScaneLeave , _ScaneUpdate);

            _Machine.Push(stage);
        }


        private void _ScaneUpdate()
        {
            var agents = GameObject.FindObjectsOfType<Regulus.Project.Chat.Common.Adsorption.Agent>();
            var agent = agents.FirstOrDefault(d => d.Name == Agent);
            if (agent != null)
            {
                _Notifier = agent.Distributor.QueryNotifier<Regulus.Project.Chat.Common.ITalker>();

                _ToInitial();                                
            }
        }

        private void _ToInitial()
        {
            var stage = new Regulus.Utility.SimpleStage(_Initial);
            _Machine.Push(stage);
        }

        private void _Initial()
        {
            _Notifier.Supply += _Supply;
            _Notifier.Unsupply += _Unsupply;
        }

        private void _ScaneLeave()
        {
            
        }

        private void _ScanEnter()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            _Machine.Update();
        }

        void OnDestroy()
        {
            if (_Notifier != null)
            {
                _Notifier.Supply -= _Supply;
                _Notifier.Unsupply -= _Unsupply;
            }
                
            _Machine.Termination();
        }

        private void _Unsupply(ITalker obj)
        {
            UnsupplyEvent.Invoke(obj);
        }

        private void _Supply(ITalker obj)
        {
            SupplyEvent.Invoke(obj);
        }

        [Serializable]
        public class UnityBroadcastEvent : UnityEvent<ITalker>{}

        public UnityBroadcastEvent SupplyEvent;
        public UnityBroadcastEvent UnsupplyEvent;
    }
}
