using System;
using UnityEngine;

public abstract class EntityBase : MonoBehaviour
{
    protected bool _isPlayer;

    protected virtual void Awake()
    {
        _isPlayer = GetComponent<Player>() != null;
    }
}
