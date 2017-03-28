using System.Collections;
using System.Collections.Generic;

using Regulus.Utility;

using UnityEngine;

namespace Regulus.Remoting.Unity
{

    public abstract class Adsorber<T> : MonoBehaviour
    {
        private readonly Regulus.Utility.StageMachine _Machine;        
        public string LinkTag;

        private Distributor _Distributor;

        protected Adsorber()
        {
            _Machine = new StageMachine();
        }

        void Start()
        {
            _Machine.Push(new Regulus.Utility.SimpleStage(_ScanEnter, _ScanLeave, _ScanUpdate));
        }

        private void _ScanUpdate()
        {
            var obj = GameObject.FindGameObjectWithTag(LinkTag);
            if (obj != null)
            {

                _Distributor = obj.GetComponent<Distributor>();
                _Machine.Push(new Regulus.Utility.SimpleStage(_DispatchEnter, _DispatchLeave));
            }
        }

        private void _DispatchEnter()
        {
            _Distributor.Attach<T>(this);
        }

        private void _DispatchLeave()
        {
            _Distributor.Detach<T>(this);
        }

        private void _ScanLeave()
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
            _Machine.Termination();
        }

        public abstract void Supply(T gpi);


        public abstract void Unsupply(T gpi);

        public abstract T GetGPI();
    }


}