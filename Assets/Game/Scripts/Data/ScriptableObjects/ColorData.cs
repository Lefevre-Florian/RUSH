using System;
using UnityEngine;

//Author : Lefevre Florian
namespace Com.IsartDigital.Rush.Data
{
    [CreateAssetMenu(fileName = "DefaultColor", menuName = "RUSH/NewColor", order =1)]
    public class ColorData : ScriptableObject
    {
        [SerializeField] private Colors _Color = default;
        [SerializeField] private Texture _Texture = default;

        public Colors Color { get { return _Color; } private set { _Color = value; } }

        public Texture Texture { get { return _Texture; } private set { _Texture = value; } }
    }
}