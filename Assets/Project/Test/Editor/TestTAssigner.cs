using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using NUnit.Framework;


using Regulus.Remoting;

namespace Regulus.Remoting.Unity
{
    public class TestTNotifier
    {

        public interface IGPI
        {

        }

        class GPI : IGPI { }

        public class TestAdsorber : Adsorber<IGPI>
        {

            public int SupplyCount;
            public int UnsupplyCount;
            public override void Supply(IGPI gpi)
            {
                SupplyCount++;
            }

            public override void Unsupply(IGPI gpi)
            {
                UnsupplyCount++;
            }

            public override IGPI GetGPI()
            {
                return null;
            }
        }

        public class TestNotifier<T> : Regulus.Remoting.INotifier<T>
        {

            private readonly List<T> _Gpis;

            public TestNotifier()
            {
                _Gpis = new List<T>();
            }

            T[] INotifier<T>.Ghosts
            {
                get { return _Gpis.ToArray(); }
            }

            T[] INotifier<T>.Returns
            {
                get { return _Gpis.ToArray(); }
            }

            private event Action<T> _Return;
            event Action<T> INotifier<T>.Return
            {
                add { _Return += value; }
                remove { _Return += value; }
            }

            private event Action<T> _Supply;
            event Action<T> INotifier<T>.Supply
            {
                add { _Supply += value; }
                remove { _Supply -= value; }
            }

            private event Action<T> _Unsupply;
            event Action<T> INotifier<T>.Unsupply
            {
                add { _Unsupply += value; }
                remove { _Unsupply -= value; }
            }

            public void Add(T gpi)
            {
                _Gpis.Add(gpi);
                _Supply(gpi);
            }

            public void Remove(T gpi)
            {
                _Gpis.Remove(gpi);
                _Unsupply(gpi);
            }
        }


        [Test]
        public void Register1()
        {

            var gpi = new GPI();
            var testNotifier = new TestNotifier<IGPI>();
            var assigner = new Assigner<IGPI>(testNotifier);

            var obj = new UnityEngine.GameObject();
            var testAdsorber = obj.AddComponent<TestAdsorber>();
            assigner.Register(testAdsorber);


            testNotifier.Add(gpi);

            Assert.AreEqual(1, testAdsorber.SupplyCount);
        }

        [Test]
        public void Register2()
        {

            var gpi = new GPI();
            var testNotifier = new TestNotifier<IGPI>();
            var assigner = new Assigner<IGPI>(testNotifier);

            var obj = new UnityEngine.GameObject();
            var testAdsorber = obj.AddComponent<TestAdsorber>();
            assigner.Register(testAdsorber);

            testNotifier.Add(gpi);

            assigner.Unregister(testAdsorber);
            assigner.Register(testAdsorber);

            Assert.AreEqual(2, testAdsorber.SupplyCount);
        }
        [Test]
        public void Register3()
        {
            var gpi = new GPI();
            var testNotifier = new TestNotifier<IGPI>();
            var assigner = new Assigner<IGPI>(testNotifier);

            var obj = new UnityEngine.GameObject();
            var testAdsorber = obj.AddComponent<TestAdsorber>();
            assigner.Register(testAdsorber);
            testNotifier.Add(gpi);
            testNotifier.Remove(gpi);

            Assert.AreEqual(1, testAdsorber.SupplyCount);
            Assert.AreEqual(1, testAdsorber.UnsupplyCount);
        }

        [Test]
        public void Register4()
        {
            var gpi = new GPI();
            var testNotifier = new TestNotifier<IGPI>();
            var assigner = new Assigner<IGPI>(testNotifier);

            var obj = new UnityEngine.GameObject();
            var testAdsorber = obj.AddComponent<TestAdsorber>();
            assigner.Register(testAdsorber);
            testNotifier.Add(gpi);
            testNotifier.Add(gpi);
            testNotifier.Remove(gpi);

            Assert.AreEqual(1, testAdsorber.SupplyCount);
            Assert.AreEqual(1, testAdsorber.UnsupplyCount);
        }


        [Test]
        public void Register6()
        {
            var gpi = new GPI();
            var testNotifier = new TestNotifier<IGPI>();
            var assigner = new Assigner<IGPI>(testNotifier);

            var obj = new UnityEngine.GameObject();
            var testAdsorber1 = obj.AddComponent<TestAdsorber>();
            var testAdsorber2 = obj.AddComponent<TestAdsorber>();

            testNotifier.Add(gpi);
            assigner.Register(testAdsorber1);
            assigner.Unregister(testAdsorber1);

            assigner.Register(testAdsorber2);
            assigner.Unregister(testAdsorber2);
            testNotifier.Remove(gpi);

            Assert.AreEqual(1, testAdsorber1.SupplyCount);
            Assert.AreEqual(1, testAdsorber1.UnsupplyCount);
            Assert.AreEqual(1, testAdsorber2.SupplyCount);
            Assert.AreEqual(1, testAdsorber2.UnsupplyCount);
        }

        [Test]
        public void Register5()
        {
            var gpi = new GPI();
            var testNotifier = new TestNotifier<IGPI>();
            var assigner = new Assigner<IGPI>(testNotifier);

            var obj = new UnityEngine.GameObject();
            var testAdsorber1 = obj.AddComponent<TestAdsorber>();
            assigner.Register(testAdsorber1);

            testNotifier.Add(gpi);            
            testNotifier.Remove(gpi);

            testNotifier.Add(gpi);            
            testNotifier.Remove(gpi);

            assigner.Unregister(testAdsorber1);

            Assert.AreEqual(2, testAdsorber1.SupplyCount);
            Assert.AreEqual(2, testAdsorber1.UnsupplyCount);
            
        }

        [Test]
        public void Register7()
        {
            var gpi = new GPI();
            var testNotifier = new TestNotifier<IGPI>();
            var assigner = new Assigner<IGPI>(testNotifier);            
            testNotifier.Add(gpi);

            var obj1 = new UnityEngine.GameObject();
            var testAdsorber1 = obj1.AddComponent<TestAdsorber>();


            assigner.Register(testAdsorber1);
            assigner.Unregister(testAdsorber1);

            var obj2 = new UnityEngine.GameObject();
            var testAdsorber2 = obj2.AddComponent<TestAdsorber>();


            assigner.Register(testAdsorber2);
            assigner.Unregister(testAdsorber2);


            testNotifier.Remove(gpi);

            Assert.AreEqual(1, testAdsorber1.SupplyCount);
            Assert.AreEqual(1, testAdsorber1.UnsupplyCount);
            Assert.AreEqual(1, testAdsorber2.SupplyCount);
            Assert.AreEqual(1, testAdsorber2.UnsupplyCount);

        }

    }

}
