using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using VInspector;

public class MapSection : MonoBehaviour
{
    [SerializeField] private MapGeneratorParams mapParams;
    [SerializeField] private MapSectionData writeSectionData;

    private MapSectionData _backupData; // In case want to revert

#if UNITY_EDITOR
    [Button]
    public void SaveData()
    {
        Undo.RecordObject(writeSectionData, "Saved Map Section");
        SaveData(writeSectionData);
        EditorUtility.SetDirty(writeSectionData);
        AssetDatabase.SaveAssets();
    }
#endif

    private void BackupData()
    {
        if (_backupData == null)
            _backupData = ScriptableObject.CreateInstance<MapSectionData>();
        
        SaveData(_backupData);
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
        data.EnvironmentObjs = envObjs.Select(obj => new MapSectionData.EnvironmentObjData(obj.Type, obj.transform.position)).ToArray();
    }

    [Button]
    public void LoadData() => EditorLoadData(writeSectionData);
    [Button]
    public void LoadBackupData() => EditorLoadData(_backupData);

    private void EditorLoadData(MapSectionData data)
    {
        Tilemap tilemap = GetComponent<Tilemap>();
        if (data != null)
        {
            BackupData();
            ResetTilemap(tilemap);
        }
        LoadData(tilemap, data, mapParams);
    }

    public static void LoadData(Tilemap tilemap, MapSectionData data, MapGeneratorParams mapParams, int xOffset = 0)
    {
        if (!data || data.Tiles == null || data.Tiles.Length == 0)
        {
            Debug.LogError("No Map Data to load from");
            return;
        }

        BoundsInt bounds = new BoundsInt(xOffset, 0, 0, data.Width, MapSectionData.Height, 1);
        tilemap.SetTilesBlock(bounds, data.Tiles);
        
        Vector2 offset = Vector2.right * xOffset;
        foreach (var obj in data.EnvironmentObjs)
            Instantiate(mapParams.PrefabsDict[obj.Type], obj.Position + offset, Quaternion.identity, tilemap.transform);
    }

    [Button]
    public void ResetTilemapEditor() => ResetTilemap(GetComponent<Tilemap>());
    public static void ResetTilemap(Tilemap tilemap)
    {
        tilemap.ClearAllTiles();

        // Kill all children, Destroy last child first
        for (int i = tilemap.transform.childCount; i > 0; --i)
        {
            if (Application.isPlaying)
                Destroy(tilemap.transform.GetChild(0).gameObject);
            else
                DestroyImmediate(tilemap.transform.GetChild(0).gameObject);
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
