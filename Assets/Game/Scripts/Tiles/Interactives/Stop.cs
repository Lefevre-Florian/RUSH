using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.Tiles
{
    public class Stop : Tile
    {
        [SerializeField] private int _WaitDuration = 2;

        public int Wait { get { return _WaitDuration; } private set { _WaitDuration = value; } }

        protected override void OnCollisionComportement()
        {
            Cube.Cube lCube = _Hit.collider.gameObject.GetComponent<Cube.Cube>();
            lCube.SetActionWait(_WaitDuration);
        }


    }

}