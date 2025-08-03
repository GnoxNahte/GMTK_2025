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
        public AudioClip BGM;
    }

    [ShowInInspector]
    public static int SelectedLevel = 0;

    [ShowInInspector] public static int MaxLevel = 3;
    [SerializeField] private int forceLevel = 0;
    
    [SerializeField] private LevelData[] levelData;
    [SerializeField] private Vector2 playerStartPos;

    public static readonly int[] RequiredSpirit = new[] { 75, 150, 250, 600, 750, };

    // Returns the player and the level data
    public (GameObject, LevelData) LoadLevel()
    {
#if UNITY_EDITOR
        if (forceLevel >= 0)
            SelectedLevel = forceLevel;
#endif
        LevelData data = levelData[SelectedLevel];
        GameObject player = Instantiate(data.CharacterPrefab, playerStartPos, Quaternion.identity);
        
        // Background
        Instantiate(data.BackgroundPrefab, Vector3.zero, Quaternion.identity);
        
        GameObject audioSourceGO = new GameObject("BGM", typeof(AudioSource));
        audioSourceGO.GetComponent<AudioSource>().clip = data.BGM;
        audioSourceGO.GetComponent<AudioSource>().volume = 0.2f;
        audioSourceGO.GetComponent<AudioSource>().Play();
        
        return (player, data);
    }
}
