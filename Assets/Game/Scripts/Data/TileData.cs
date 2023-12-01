using System;
using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.Data
{
    [Serializable]
    public struct TileData
    {
        public Vectors direction;
        public GameObject prefab;
        public int quantity;

        public Material Material { get { return prefab.GetComponentInChildren<Renderer>().sharedMaterial; } private set { } }
    }
}
