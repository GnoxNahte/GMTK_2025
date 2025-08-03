using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    [SerializeField] private HealthUI healthUI;
    [SerializeField] private ChaserDistText chaserText;

    public void Init(Player player, Chaser chaser)
    {
        healthUI.Init(player);
        chaserText.Init(player, chaser);
    }
}
