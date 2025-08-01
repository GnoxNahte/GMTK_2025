using UnityEngine;
using UnityEngine.Tilemaps;
using VInspector;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private float minLength;
    [SerializeField] private MapGeneratorParams mapParams;
    [SerializeField] private int seed;
    
    [Button]
    [OnValueChanged("seed", "minLength")]
    public void GenerateMap()
    {
        Random.InitState(seed);
        
        Tilemap tilemap = GetComponent<Tilemap>();
        MapSection.ResetTilemap(tilemap);
        
        // Get all Sections
        MapSectionData[] sections = Resources.LoadAll<MapSectionData>("MapData");
        
        int currLen = 0;

        while (currLen < minLength)
        {
            MapSectionData section = sections[Random.Range(0, sections.Length)];
            MapSection.LoadData(tilemap, section, mapParams, currLen);
            currLen += section.Width + mapParams.EndPadding; 
        }
    }
}
