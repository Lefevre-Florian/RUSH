using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.Tiles
{
    public class ConvoyerTile : DirectionalTiles
    {

        protected override void OnCollisionComportement()
        {
            Cube.Cube lCube = _Hit.collider.gameObject.GetComponent<Cube.Cube>();
            lCube.SetActionSlideMove(GetDirection());
        }

    }
}
