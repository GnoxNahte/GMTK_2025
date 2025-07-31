using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapSection : MonoBehaviour
{
    [Serializable] 
    public class EnvPrefabPair
    {
        public EnvironmentObjectBase.EnvType Type;
        public GameObject Prefab;
    }
    
    [SerializeField] private MapSectionData writeData;
    [SerializeField] private EnvPrefabPair[] prefabs;
    
    // Just for easy access
    private Dictionary<EnvironmentObjectBase.EnvType, GameObject> _prefabsDict;
    const int HEIGHT = 12;
    
    [ContextMenu("Get Map Data")]
    public void GetData()
    {
        if (!writeData)
        {
            Debug.LogError("No Map Data to write to");
            return;
        }

        // Load tilemap
        Tilemap tilemap = GetComponent<Tilemap>();
        tilemap.CompressBounds();
        
        BoundsInt bounds = tilemap.cellBounds;
        bounds.xMin = 0;
        bounds.yMin = 0;
        bounds.yMax = HEIGHT;

        writeData.Tiles = tilemap.GetTilesBlock(bounds);
        
        // Load environment objects
        var envObjs = GetComponentsInChildren<EnvironmentObjectBase>();
        writeData.EnvironmentObjs = envObjs.Select(obj => new MapSectionData.EnvironmentObjData(obj.Type, obj.transform.position)).ToArray();
    }

    [ContextMenu("Load Map Data")]
    public void LoadData()
    {
        if (!writeData)
        {
            Debug.LogError("No Map Data to write to");
            return;
        }

        GeneratePrefabDict();

        Tilemap tilemap = GetComponent<Tilemap>();
        BoundsInt bounds = new BoundsInt(0, 0, 0, writeData.Tiles.Length / HEIGHT, HEIGHT, 1);
        tilemap.SetTilesBlock(bounds, writeData.Tiles);

        foreach (var obj in writeData.EnvironmentObjs)
            Instantiate(_prefabsDict[obj.Type], obj.Position, Quaternion.identity, transform);
    }

    [ContextMenu("Reset Tilemap")]
    public void ResetTilemap()
    {
        Tilemap tilemap = GetComponent<Tilemap>();
        tilemap.ClearAllTiles();

        // Kill all children, Destroy last child first
        for (int i = transform.childCount; i > 0; --i)
        {
            if (Application.isPlaying)
                Destroy(transform.GetChild(0).gameObject);
            else
                DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }

    private void GeneratePrefabDict()
    {
        _prefabsDict = new Dictionary<EnvironmentObjectBase.EnvType, GameObject>();

        foreach (var prefab in prefabs)
            _prefabsDict.Add(prefab.Type, prefab.Prefab);
    }

    private void OnDrawGizmos()
    // private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(new Vector3(-0.5f, HEIGHT * 0.5f), new Vector3(1f, HEIGHT));
        Gizmos.DrawCube(new Vector3(15f, -0.5f), new Vector3(30f, 1f));

        // if (writeData != null)
        // {
        //     Gizmos.color = new Color(0, 0, 1, 0.5f);
        //     Gizmos.DrawCube(writeData.Bounds.center, writeData.Bounds.size);
        // }
    }
}
