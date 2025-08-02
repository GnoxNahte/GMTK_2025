using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectibleUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image icon;

    [SerializeField] AnimationCurve animationSpeedCurve;
    [SerializeField] float maxDistance;
    [SerializeField] float maxTime;

    private int _collectedCount;
    
    private Camera _camera;
    private Canvas _canvas;
    
    // Singleton
    public static CollectibleUI Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            Debug.LogError("More than 1 CollectibleUI. Destroying this. Name: " + name);
            return;
        }
        
        _camera = Camera.main;
    }  
  
    private void OnDestroy()  
    {  
        if (Instance == this)
            Instance = null;
    }

    public IEnumerator AnimateToImage(Collectible collectible)
    {
        Vector3 startPos = collectible.transform.position;
        Vector3 targetPos = _camera.ScreenToWorldPoint(icon.transform.position);
        float animationTime = (collectible.transform.position - targetPos).magnitude / maxDistance * maxTime;
        float currTime = 0;
        while (currTime < animationTime)
        {
            currTime += Time.deltaTime;
            targetPos = _camera.ScreenToWorldPoint(icon.transform.position);
            collectible.transform.position = Vector3.Slerp(startPos, targetPos, currTime / animationTime);
            yield return null;
        }
        
        // TODO Play collected sfx
        _collectedCount++;
        text.text = _collectedCount.ToString();
    }
}
