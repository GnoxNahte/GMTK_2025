using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    [SerializeField] private HealthUI healthUI;

    public void Init(Player player)
    {
        healthUI.Init(player);
    }
}
