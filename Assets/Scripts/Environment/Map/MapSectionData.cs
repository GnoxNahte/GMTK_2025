using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Map Section Data", menuName = "ScriptableObjects/Map Section Data")]
public class MapSectionData : ScriptableObject
{
    [Serializable]
    public class EnvironmentObjData
    {
        public EnvironmentObjectBase.EnvType Type;
        public Vector2 Position;

        public EnvironmentObjData(EnvironmentObjectBase.EnvType type, Vector2 position)
        {
            Type = type;
            Position = position;
        }
    }
    
    public TileBase[] Tiles;
    public EnvironmentObjData[] EnvironmentObjs;
    
    public const int Height = 15;
    public int Width => Tiles.Length / Height;
}
