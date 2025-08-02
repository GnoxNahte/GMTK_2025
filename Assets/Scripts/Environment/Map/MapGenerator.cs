using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using VInspector;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private int minLength;
    [SerializeField] private MapGeneratorParams mapParams;
    [SerializeField] private GameObject tilemapPrefab;
    [SerializeField] private int tilemapCount;
    [SerializeField] private int seed;
    [SerializeField] private int startOffset;

    private Dictionary<EnvironmentObjectBase.EnvType, ObjectPool> _envPool = null;
    private Player _player;
    private Tilemap[] _tilemaps;
    private MapSectionData[] _sections = null;

    private int _currTilemap = 0;
    private int _currDist = 0;
    private int _currBagIndex = 0;
    private int[] _sectionBag = null; // Similar to tetris bag

    public void Init(Player player)
    {
        _player = player;

        _currDist = startOffset;
        Random.InitState(seed);
        
        InitPool();
        // Get all Sections
        _sections = Resources.LoadAll<MapSectionData>("MapData");
        
        // Create tilemap bags, similar to tetris bag
        _sectionBag = new int[_sections.Length];
        for (int i = 0; i < _sectionBag.Length; i++)
            _sectionBag[i] = i;
        GenerateBag();
        
        // Create tilemaps
        _tilemaps = new Tilemap[tilemapCount];
        for (int i = 0; i < tilemapCount; i++)
            _tilemaps[i] = Instantiate(tilemapPrefab, transform).GetComponent<Tilemap>();
    }

    private void InitPool()
    {
        _envPool = new Dictionary<EnvironmentObjectBase.EnvType, ObjectPool>(mapParams.PrefabsDict.Count);

        foreach (var prefab in mapParams.PrefabsDict)
        {
            GameObject poolGO = new GameObject(prefab.Key + " Pool", typeof(ObjectPool));
            ObjectPool pool = poolGO.GetComponent<ObjectPool>();
            pool.SetPrefab(prefab.Value);
            pool.Init();
            _envPool.Add(prefab.Key, pool);
        }
    }

    private void Update()
    {
        if (_currDist - _player.transform.position.x < minLength)
        {
            _currTilemap = (_currTilemap + 1) % _tilemaps.Length;
            GenerateMap(_tilemaps[_currTilemap++]);
        }
    }

    private void GenerateMap(Tilemap tilemap)
    {
        MapSection.ResetTilemap(tilemap);

        int currLength = 0;
        while (currLength < minLength)
        {
            int sectionIndex = _sectionBag[_currBagIndex++];
            if (_currBagIndex >= _sectionBag.Length)
            {
                _currBagIndex = 0;
                GenerateBag();
            }
            MapSectionData section = _sections[sectionIndex];
            MapSection.LoadData(tilemap, section, mapParams, _envPool, _currDist + currLength);
            currLength += section.Width + mapParams.EndPadding; 
        }

        _currDist += currLength;
    }
    
    [Button]
    [OnValueChanged("seed", "minLength")]
    public void EditorGenerateMap()
    {
        return;
        Random.InitState(seed);
        
        // Get all Sections
        _sections = Resources.LoadAll<MapSectionData>("MapData");
        
        _currDist = 0;
        Tilemap tilemap = GetComponentInChildren<Tilemap>();
        GenerateMap(tilemap);
    }

    private void GenerateBag()
    {
        // Check with the last few indexes, to make sure it doesn't repeat too often
        const int lastCheck = 2;
        int[] lastIndexes = null;
        if (_sectionBag.Length >= lastCheck)
        {
            lastIndexes = new int[lastCheck];
            for (int i = 1; i < lastCheck; i++)
            {
                lastIndexes[i] = _sectionBag[i];
            }
        }
        
        Utility.Shuffle(_sectionBag);

        if (lastIndexes == null)
            return;
        
        const int tryCount = 3;
        for (int i = 0; i < tryCount; i++)
        {
            bool ifSuccess = true;
            // Check through the first X elements, 
            // If the first X elements contain the indexes of the previous array, 
            // Re-shuffle it. 
            // Try to re-shuffle until it hits the try count.
            // X - lastCheck count
            for (int j = 0; j < lastCheck; j++)
            {
                if (lastIndexes.Contains(_sectionBag[j]))
                {
                    Utility.Shuffle(_sectionBag);
                    ifSuccess = false;
                }
            }

            if (ifSuccess)
                break;
        }
    }
}
