using System;
using UnityEngine;

public class TriggerCollisionEvents : MonoBehaviour
{
    public Action<Collider2D> OnTriggerEnter;
    public Action<Collider2D> OnTriggerExit;

    private void OnTriggerEnter2D(Collider2D other) => OnTriggerEnter?.Invoke(other);
    private void OnTriggerExit2D(Collider2D other) => OnTriggerExit?.Invoke(other);

    private void OnEnable()
    {
        print("OnEnable");
    }
}
