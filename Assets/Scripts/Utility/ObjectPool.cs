using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VInspector;

// Game Object Pool. Written by GnoxNahte
public class ObjectPool : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] private int startCount;
    [ShowInInspector, ReadOnly] List<GameObject> objs;
    [ShowInInspector, ReadOnly] List<GameObject> inactiveObjs;

    [ShowInInspector, ReadOnly] private int totalObjsCount;
    [ShowInInspector, ReadOnly] private int activeObjsCount;
    [ShowInInspector, ReadOnly] private int inactiveObjsCount;

    [SerializeField] private bool ifCheckInactive;

    public int ActiveCount => objs.Count - inactiveObjs.Count;

    public bool InitDone { get; private set; } = false;

    private void Start()
    {
        Init(startCount);
    }

    private void Update()
    {
#if UNITY_EDITOR
        totalObjsCount = objs.Count;
        inactiveObjsCount = inactiveObjs.Count;
        activeObjsCount = totalObjsCount - inactiveObjsCount;
#endif
    }
    
    public void SetPrefab(GameObject prefab) => this.prefab = prefab;

    public void Init(int startCapacity = 100)
    {
        if (InitDone)
            return;
        
        objs = new List<GameObject>(startCapacity);
        inactiveObjs = new List<GameObject>(startCapacity);

        for (int i = 0; i < startCapacity; i++)
        {
#if UNITY_EDITOR
            GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(prefab, transform);
            go.name = go.name + " " + i;
#else
            GameObject go = Instantiate(prefab, transform);
            //TOOD: Try InstantiateAsync()
#endif
            go.SetActive(false);

            objs.Add(go);
            inactiveObjs.Add(go);
        }

        InitDone = true;
    }
    
    public GameObject Get(Vector2 position)
    {
        return Get(position, Quaternion.identity);
    }

    public GameObject Get(Vector2 position, Quaternion rotation)
    {
        GameObject go;

        int inactiveCount = inactiveObjs.Count;
        if (inactiveCount > 0)
        {
            go = inactiveObjs[inactiveCount - 1];
            inactiveObjs.RemoveAt(inactiveCount - 1);
#if UNITY_EDITOR
            if (go == null)
                Debug.LogError("GameObject in inactiveObjs[] in ObjectPool.cs is null/destoryed");
#endif
            go?.transform.SetPositionAndRotation(position, rotation);
        }
        else
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
                go = Instantiate(prefab, position, rotation, transform);
            else
                go = (GameObject)PrefabUtility.InstantiatePrefab(prefab, transform);
            go.transform.SetPositionAndRotation(position, rotation);
#else
            go = Instantiate(prefab, position, rotation, transform);
#endif
            objs.Add(go);
        }

        go.SetActive(true);

        return go;
    }

    public void Release(GameObject go)
    {
#if UNITY_EDITOR
        if (ifCheckInactive && inactiveObjs.Contains(go))
        {
            Debug.LogError("Releasing an object that's already inside the inactive list");
            // NOT returning to catch the bug in case I missed the error message
            //return;
        }
#endif

        go.SetActive(false);
        inactiveObjs.Add(go);
    }

    public void DestroyAll()
    {
        if (objs == null)
            return;

        foreach (var obj in objs)
        {
            GameObject go = obj.gameObject;

            DestroyImmediate(go);
        }

        objs.Clear();
        inactiveObjs.Clear();

        objs = null;
        inactiveObjs = null;

        InitDone = false;
    }
}