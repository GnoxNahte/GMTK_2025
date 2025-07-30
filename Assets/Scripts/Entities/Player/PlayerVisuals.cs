using System;
using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    private SpriteRenderer _sr;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
    }
}
