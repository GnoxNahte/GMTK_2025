using UnityEngine;
using UnityEngine.Tilemaps;
using VInspector;

public class LevelManager : MonoBehaviour
{
    [System.Serializable]
    public class LevelData
    {
        public GameObject CharacterPrefab;
        public GameObject BackgroundPrefab;
        public TileBase SourceOverrideTile;
        public TileBase OverrideTile;
    }

    [ShowInInspector]
    public static int SelectedLevel = 0;

    [ShowInInspector] public static int MaxLevel = 3;
    [SerializeField] private int forceLevel = 0;
    
    [SerializeField] private LevelData[] levelData;
    [SerializeField] private Vector2 playerStartPos;

    // Returns the player and the level data
    public (GameObject, LevelData) LoadLevel(MapGenerator mapGenerator)
    {
#if UNITY_EDITOR
        if (forceLevel >= 0)
            SelectedLevel = forceLevel;
#endif
        LevelData data = levelData[SelectedLevel];
        GameObject player = Instantiate(data.CharacterPrefab, playerStartPos, Quaternion.identity);
        
        // Background
        Instantiate(data.BackgroundPrefab, Vector3.zero, Quaternion.identity);
        
        return (player, data);
    }
}
