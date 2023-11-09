using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.Tiles
{
    public class Teleporter : MonoBehaviour
    {
        [SerializeField] private Transform _Output = default;
        [SerializeField] private int _TeleportationTick = 2;

        public Vector3 OutputPosition
        {
            get { return _Output.position; }
            private set { _Output.position = value; }
        }

        public int TeleportationTick { get { return _TeleportationTick; } private set { _TeleportationTick = value; } }
    }
}