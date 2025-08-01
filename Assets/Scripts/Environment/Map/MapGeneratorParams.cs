using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Map Params", menuName = "ScriptableObjects/Map Params")]
public class MapGeneratorParams : ScriptableObject
{
    [System.Serializable] 
    public class EnvPrefabPair
    {
        public EnvironmentObjectBase.EnvType Type;
        public GameObject Prefab;
    }

    public int EndPadding;
    
    [SerializeField] private EnvPrefabPair[] prefabs;

    private Dictionary<EnvironmentObjectBase.EnvType, GameObject> _prefabsDict;
    public Dictionary<EnvironmentObjectBase.EnvType, GameObject> PrefabsDict
    {
        get
        {
            if (_prefabsDict == null)
            {
                _prefabsDict = new Dictionary<EnvironmentObjectBase.EnvType, GameObject>();
                foreach (var prefab in prefabs)
                    _prefabsDict.Add(prefab.Type, prefab.Prefab);
            }
            
            return _prefabsDict;
        }
    }
}
