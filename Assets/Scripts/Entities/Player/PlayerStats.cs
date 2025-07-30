using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Stats", menuName = "ScriptableObjects/Player Stats")]
public class PlayerStats : ScriptableObject
{
    [Header("Movement")]
    public float MaxSpeed;
    
    private void OnValidate()
    {
        // Limit variables
        float minValue = 0.01f;
        
        MaxSpeed = Mathf.Max(minValue, MaxSpeed);
        
        // Pre-calculate physics beforehand, slightly reduce runtime calculation
    }
}
