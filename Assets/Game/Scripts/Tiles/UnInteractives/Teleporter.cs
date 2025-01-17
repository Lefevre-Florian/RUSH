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
            m_Clock.OnReset += Restore;
        }

        protected override void OnCollisionComportement()
        {
            Cube.Cube lCube = m_Hit.collider.gameObject.GetComponent<Cube.Cube>();

            if (_Output.TeleportationStack.Count != 0 && _Output.TeleportationStack.Contains(lCube) || _Output._TeleportationMemory.Contains(lCube))
                return;

            if(_TeleportationStack.Count == 0)
                m_Clock.OnTick += ManageTeleportation;

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
                m_Clock.OnTick += CleanQueue;

            if (_TeleportationStack.Count == 0)
                m_Clock.OnTick -= ManageTeleportation;
        }

        private void CleanQueue()
        {
            if(++_InternalTick % (_TeleportationTick + 1) == 0)
                _TeleportationMemory.Dequeue();

            if (_TeleportationMemory.Count == 0)
            {
                m_Clock.OnTick -= CleanQueue;
                _InternalTick = 0;
            }
                
        }

        private void Restore()
        {
            _TeleportationStack.Clear();
            if (_TeleportationMemory.Count != 0)
                _TeleportationMemory.Clear();

            m_Clock.OnTick -= CleanQueue;
            m_Clock.OnTick -= ManageTeleportation;
        }

        public void TriggerAnimation() => _Animator.SetTrigger(_TeleportationTrigger);

        protected override void Destroy()
        {
            _Animator = null;

            Restore();
            m_Clock.OnReset -= Restore;

            base.Destroy();
        }
    }
}