using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.Tiles
{
    public class Stop : MonoBehaviour
    {
        [SerializeField] private int _WaitDuration = 2;

        public int Wait { get { return _WaitDuration; } private set { _WaitDuration = value; } }
    }

}