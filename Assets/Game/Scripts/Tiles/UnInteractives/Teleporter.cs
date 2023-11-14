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

        public Queue<Cube.Cube> TeleportationStack
        {
            get { return _TeleportationStack; }
            private set { _TeleportationStack = value; }
        }

        protected override void OnCollisionComportement()
        {
            Cube.Cube lCube = _Hit.collider.gameObject.GetComponent<Cube.Cube>();

            if (_Output.TeleportationStack.Contains(lCube))
                return;

            if(_TeleportationStack.Count == 0)
                _Clock.OnTick += ManageTeleportation;

            lCube.GetComponent<MeshRenderer>().enabled = false;
            lCube.SetActionWait(_TeleportationTick);
            lCube.transform.position = OutputPosition + Vector3.up * 0.5f;
            Debug.Log(lCube.transform.position);

            _TeleportationStack.Enqueue(lCube);
        }

        private void ManageTeleportation()
        {
            Cube.Cube lCube = _TeleportationStack.Dequeue();
            lCube.GetComponent<MeshRenderer>().enabled = true;
            lCube.SetActionMove();

            if (_TeleportationStack.Count == 0)
                _Clock.OnTick -= ManageTeleportation;
        }

        protected override void Destroy()
        {
            _Clock.OnTick -= ManageTeleportation;
            base.Destroy();
        }
    }
}