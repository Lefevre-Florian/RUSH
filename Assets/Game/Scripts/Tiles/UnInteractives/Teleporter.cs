using System.Collections.Generic;
using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.Tiles
{
    public class Teleporter : Tile
    {
        [SerializeField] private Teleporter _Output = default;
        [SerializeField] private int _TeleportationTick = 1;

        public Vector3 OutputPosition
        {
            get { return _Output.transform.position; }
            private set { _Output.transform.position = value; }
        }

        private Queue<Cube.Cube> _TeleportationStack = new Queue<Cube.Cube>();
        private Queue<Cube.Cube> _TeleportationMemory = new Queue<Cube.Cube>();

        public Queue<Cube.Cube> TeleportationStack
        {
            get { return _TeleportationStack; }
            private set { _TeleportationStack = value; }
        }

        private int _InternalTick = 0;

        protected override void OnCollisionComportement()
        {
            Cube.Cube lCube = _Hit.collider.gameObject.GetComponent<Cube.Cube>();

            if (_Output.TeleportationStack.Count != 0 && _Output.TeleportationStack.Contains(lCube) || _Output._TeleportationMemory.Contains(lCube))
                return;

            if(_TeleportationStack.Count == 0)
                _Clock.OnTick += ManageTeleportation;

            lCube.GetComponent<MeshRenderer>().enabled = false;
            lCube.SetActionWait(_TeleportationTick);
            lCube.transform.position = OutputPosition + Vector3.up * 0.5f;

            _TeleportationStack.Enqueue(lCube);
        }

        private void ManageTeleportation()
        {
            Cube.Cube lCube = _TeleportationStack.Dequeue();
            lCube.GetComponent<MeshRenderer>().enabled = true;

            _TeleportationMemory.Enqueue(lCube);

            if (_TeleportationMemory.Count != 0)
                _Clock.OnTick += CleanQueue;

            if (_TeleportationStack.Count == 0)
                _Clock.OnTick -= ManageTeleportation;
        }

        private void CleanQueue()
        {
            if(++_InternalTick % (_TeleportationTick + 1) == 0)
                _TeleportationMemory.Dequeue();

            if (_TeleportationMemory.Count == 0)
            {
                _Clock.OnTick -= CleanQueue;
                _InternalTick = 0;
            }
                
        }

        protected override void Destroy()
        {
            _Clock.OnTick -= ManageTeleportation;
            base.Destroy();
        }
    }
}