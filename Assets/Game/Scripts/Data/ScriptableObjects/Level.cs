using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.Data
{
    [CreateAssetMenu(menuName = "RUSH/NewLevel",fileName = "DefaultLevel",order = 0)]
    public class Level : ScriptableObject
    {
        [SerializeField] private TileData[] _Tiles = new TileData[0];
        [SerializeField] private GameObject _Level = null;
        [SerializeField] private GameObject _Model = null;

        [Space(5)]
        [SerializeField][TextArea] private string _Name = "";

        public TileData[] Tile { get { return _Tiles; } private set { _Tiles = value; } }

        public GameObject LevelPrefab { get { return _Level; } private set { _Level = value; } }

        public GameObject Model { get { return _Model; } private set { _Model = value; } }

        public string Name { get { return _Name; } private set { _Name = value; } }
    }
}
