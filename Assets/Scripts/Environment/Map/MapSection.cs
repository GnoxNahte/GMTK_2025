using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using VInspector;

public class MapSection : MonoBehaviour
{
    [SerializeField] private MapGeneratorParams mapParams;
    [SerializeField] private MapSectionData writeSectionData;
    
    // Quickly replacing all tiles with another tile when loading
    [SerializeField] private TileBase replaceTileFrom;
    [SerializeField] private TileBase replaceTileTo;

#if UNITY_EDITOR
    [Button]
    public void SaveData()
    {
        Undo.RecordObject(writeSectionData, "Saved Map Section");
        SaveData(writeSectionData);
        EditorUtility.SetDirty(writeSectionData);
        AssetDatabase.SaveAssets();
    }

    private void SaveData(MapSectionData data)
    {
        if (!data)
        {
            Debug.LogError("No Map Data to write to");
            return;
        }

        // Load tilemap
        Tilemap tilemap = GetComponent<Tilemap>();
        tilemap.CompressBounds();
        
        BoundsInt bounds = tilemap.cellBounds;
        if (bounds.size.x <= 0 || bounds.size.y <= 0)
        {
            // Debug.LogError("Removing all tiles?");
            return;
        }
        
        bounds.xMin = 0;
        bounds.yMin = 0;
        bounds.yMax = MapSectionData.Height;

        data.Tiles = tilemap.GetTilesBlock(bounds);
        
        // Load environment objects
        var envObjs = GetComponentsInChildren<EnvironmentObjectBase>();
        List<MapSectionData.EnvironmentObjData> envData = new List<MapSectionData.EnvironmentObjData>(envObjs.Length);
        foreach (var obj in envObjs)
        {
            // Skip any object that is alr in the tilemap
            if (obj.Type == EnvironmentObjectBase.EnvType.Spikes ||
                obj.Type == EnvironmentObjectBase.EnvType.Spring ||
                obj.Type == EnvironmentObjectBase.EnvType.Stomper)
                continue;
            
            envData.Add(new MapSectionData.EnvironmentObjData(obj.Type, obj.transform.position));
        }

        data.EnvironmentObjs = envData.ToArray();
    }

    [Button]
    public void LoadData() => EditorLoadData(writeSectionData);

    private void EditorLoadData(MapSectionData data)
    {
        Tilemap tilemap = GetComponent<Tilemap>();
        if (data != null)
        {
            ResetTilemap(tilemap);
        }

        MapSectionData dataToLoad = data;
        if (replaceTileFrom != null && replaceTileTo != null && replaceTileFrom != replaceTileTo)
        {
            dataToLoad  = Instantiate(data);
            for (int i = 0; i < dataToLoad.Tiles.Length; i++)
            {
                if (dataToLoad.Tiles[i] == replaceTileFrom)
                    dataToLoad.Tiles[i] = replaceTileTo;
            }
        }
        
        LoadData(tilemap, dataToLoad, mapParams, null);
    }
#endif

    public static void LoadData(Tilemap tilemap, MapSectionData data, MapGeneratorParams mapParams, Dictionary<EnvironmentObjectBase.EnvType, ObjectPool> envPool, TileBase sourceTile = null, TileBase overrideTile = null, int xOffset = 0)
    {
        if (!data || data.Tiles == null || data.Tiles.Length == 0)
        {
            Debug.LogError("No Map Data to load from");
            return;
        }

        // Set tiles
        BoundsInt bounds = new BoundsInt(xOffset, 0, 0, data.Width, MapSectionData.Height, 1);
        
        TileBase[] tiles = data.Tiles;
        // // Array.Copy(data.Tiles, tiles, data.Tiles.Length);
        // if (sourceTile && overrideTile && sourceTile != overrideTile)
        // {
        //     for (int i = 0; i < tiles.Length; i++)
        //     {
        //         if (tiles[i] == sourceTile)
        //             tiles[i] = overrideTile;
        //     }
        // }
        // // else
        // // {
        // //     tiles = data.Tiles;
        // // }
        tilemap.SetTilesBlock(bounds, tiles);
        
        // Set Environment objects
        Vector2 offset = Vector2.right * xOffset;
        foreach (var obj in data.EnvironmentObjs)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Instantiate(mapParams.PrefabsDict[obj.Type], obj.Position + offset, Quaternion.identity, tilemap.transform);
                continue;
            }
#endif
            GameObject go = envPool[obj.Type].Get(obj.Position + offset);
            go.GetComponent<EnvironmentObjectBase>().SetPool(envPool[obj.Type]);
            go.transform.parent = tilemap.transform;
        }
    }

    [Button]
    public void ResetTilemapEditor() => ResetTilemap(GetComponent<Tilemap>());
    public static void ResetTilemap(Tilemap tilemap)
    {
        tilemap.ClearAllTiles();

        // Kill all children, Destroy last child first
        for (int i = tilemap.transform.childCount; i > 0; --i)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                DestroyImmediate(tilemap.transform.GetChild(0).gameObject);
                continue;
            }
#endif
            GameObject go = tilemap.transform.GetChild(0).gameObject;
            go.GetComponent<EnvironmentObjectBase>().Release();
        }
    }

    private void OnDrawGizmos()
    // private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(new Vector3(-0.5f, MapSectionData.Height * 0.5f), new Vector3(1f, MapSectionData.Height));
        Gizmos.DrawCube(new Vector3(25f, -0.5f), new Vector3(50f, 1f));
        Gizmos.DrawCube(new Vector3(25f, MapSectionData.Height + 0.5f), new Vector3(50f, 1f));
    }
}
