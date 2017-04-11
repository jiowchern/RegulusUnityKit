using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Regulus.Utility;

using UnityEngine;
using UnityEngine.Events;

namespace Regulus.Remoting.Unity
{
    public  abstract  class Broadcaster<T> : MonoBehaviour
    {
        public string Distributor;        
        Regulus.Remoting.INotifier<T> _Notifier;

        private readonly Regulus.Utility.StageMachine _Machine;

        

        public Broadcaster()
        {
            _Machine = new StageMachine();
        } 
        // Use this for initialization
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
            var distributors = GameObject.FindObjectsOfType<Distributor>();
            var distributor = distributors.FirstOrDefault(d => d.Name == Distributor);
            if (distributor != null)
            {
                _Notifier = distributor._GetAgent().QueryNotifier<T>();

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
        
        protected abstract void _Unsupply(T obj);


        protected abstract void _Supply(T obj);



    }
}

