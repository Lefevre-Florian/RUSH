using System.Collections.Generic;
using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.Tiles
{
    [RequireComponent(typeof(Animator))]
    public class Teleporter : Tile
    {
        [SerializeField] private Teleporter _Output = default;
        [SerializeField] private int _TeleportationTick = 1;

        [Header("Animation & Juiciness")]
        [SerializeField] private string _TeleportationTrigger = "";

        // Logic comportements
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

        // Animations & Juiciness variables

        private Animator _Animator = null;

        protected override void Init()
        {
            base.Init();

            _Animator = GetComponent<Animator>();
        }

        protected override void OnCollisionComportement()
        {
            Cube.Cube lCube = _Hit.collider.gameObject.GetComponent<Cube.Cube>();

            if (_Output.TeleportationStack.Count != 0 && _Output.TeleportationStack.Contains(lCube) || _Output._TeleportationMemory.Contains(lCube))
                return;

            if(_TeleportationStack.Count == 0)
                _Clock.OnTick += ManageTeleportation;

            TriggerAnimation();

            lCube.Renderer.DisableVisibility();
            lCube.SetActionWait(_TeleportationTick);
            lCube.transform.position = OutputPosition + Vector3.up * 0.5f;

            _TeleportationStack.Enqueue(lCube);
        }

        private void ManageTeleportation()
        {
            _Output.TriggerAnimation();

            Cube.Cube lCube = _TeleportationStack.Dequeue();
            lCube.Renderer.EnableVisibility();

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

        public void TriggerAnimation() => _Animator.SetTrigger(_TeleportationTrigger);

        protected override void Destroy()
        {
            _Animator = null;

            _Clock.OnTick -= ManageTeleportation;
            base.Destroy();
        }
    }
}