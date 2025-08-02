using System;
using UnityEngine;
using VInspector;

public class Chaser : MonoBehaviour
{
    [SerializeField] AnimationCurve speedCurve;

    // Just to show in inspector
    [ShowInInspector, ReadOnly] private float _speed;
    
    private void Update()
    {
        _speed = speedCurve.Evaluate(Time.timeSinceLevelLoad);
        transform.position += _speed * Vector3.right;
    }
}
