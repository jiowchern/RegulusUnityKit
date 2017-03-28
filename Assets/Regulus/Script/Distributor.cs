using System;
using System.Collections;
using System.Collections.Generic;

using Regulus.Remoting;

using UnityEngine;

namespace Regulus.Remoting.Unity
{
    public abstract class Distributor : MonoBehaviour
    {
        

        private readonly Dictionary<int, Assigner> _Notifiers;

        protected Distributor()
        {            
            _Notifiers = new Dictionary<int, Assigner>();
        }

        protected abstract IAgent _GetAgent();        
        public void Attach<T>(Adsorber<T> adsorber)
        {
            var notifier = _GetAgent().QueryNotifier<T>();
            var notifier1 = _QueryNotifier(notifier);
            notifier1.Register(adsorber);
        }

        public void Detach<T>(Adsorber<T> adsorber)
        {
            var notifier = _GetAgent().QueryNotifier<T>();
            var notifier1 = _QueryNotifier(notifier);
            notifier1.Unregister(adsorber);
        }

        private Assigner _QueryNotifier<T>(INotifier<T> notifier)
        {
            var hash = notifier.GetHashCode();
            Assigner outAssigner;
            if (_Notifiers.TryGetValue(hash, out outAssigner))
            {
                return outAssigner;
            }

            outAssigner = new Assigner<T>(notifier);
            _Notifiers.Add(hash, outAssigner);
            return outAssigner;
        }
    }
}
