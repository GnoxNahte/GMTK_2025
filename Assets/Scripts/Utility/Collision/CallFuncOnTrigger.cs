using UnityEngine;
using UnityEngine.Events;

public class CallFuncOnTrigger : MonoBehaviour
{
    [SerializeField] UnityEvent onEnter;
    
    private void OnTriggerEnter2D(Collider2D other) => onEnter?.Invoke();
}
