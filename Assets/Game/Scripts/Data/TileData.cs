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
    }
}
