using System;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class PlatformBase : MonoBehaviour
{
    [Flags]
    public enum Type
    {
        None = 0,
        Everything = ~0,

        OneWay = 1 << 0,
        Ladder = 1 << 1,
        Moving = 1 << 2,
        Death  = 1 << 3,
    }

    [System.Serializable]
    public class ModifierPair
    {
        public Type Type;
        public PlatformModifierBase Modifier;
    }
    public Type PlatformType => platformType;

    [SerializeField] protected Type platformType;
    [SerializeField] protected ModifierPair[] modifierPairs;
    
    protected Dictionary<Type, PlatformModifierBase> modifiers;

    private void Awake()
    {
        modifiers = new Dictionary<Type, PlatformModifierBase>(modifierPairs.Length);
        foreach (var pair in modifierPairs)
            modifiers.Add(pair.Type, pair.Modifier);
    }

    public bool HasPlatformTypeFlag(Type type)
    {
        return platformType.HasFlag(type);
    }

    public PlatformModifierBase GetPlatformModifier(Type type)
    {
        return modifiers[type];
    }
    
    private void AddPlatformTypeFlag(Type type)
    {
        platformType |= type;
    }
    private void RemovePlatformTypeFlag(Type type)
    {
        platformType &= ~type;
    }

    protected virtual void OnValidate()
    {
        // Reset
        platformType = Type.None;
        modifiers = new SerializedDictionary<Type, PlatformModifierBase>();
        
        PlatformModifierBase[] modifiersArray = GetComponents<PlatformModifierBase>();
        
        foreach (PlatformModifierBase modifier in modifiersArray)
        {
            if (HasPlatformTypeFlag(modifier.Type))
            {
                Debug.LogError("Duplicate platform modifier");
                continue;
            }
            modifiers.Add(modifier.Type, modifier);
            AddPlatformTypeFlag(modifier.Type);
        }
    }
}