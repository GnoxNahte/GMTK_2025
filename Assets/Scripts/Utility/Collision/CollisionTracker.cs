using System.Collections.Generic;
using UnityEngine;

public abstract class CollisionTracker<T> : MonoBehaviour
{
    #region Public Variables
    [field: SerializeField] public List<T> CollidingObjs { get; private set; }
    public bool IsColliding => CollidingObjs.Count > 0;
    public Collider2D Collider { get; private set; }
    #endregion
    
    #region Unity Methods
    private void Awake()
    {
        CollidingObjs = new List<T>();
        Collider = GetComponent<Collider2D>();
    }
    #endregion
    
    #region Private Methods
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        // Ignore if it is a trigger
        if (collision.isTrigger)
            return;

        T obj = collision.GetComponent<T>();
        if (obj != null)
            CollidingObjs.Add(obj);
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        // Ignore if it is a trigger
        if (collision.isTrigger)
            return;

        T platform = collision.GetComponent<T>();
        if (platform != null)
            CollidingObjs.Remove(platform);
    }
    #endregion
}