using System;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private CinemachineCamera _camera;
    public GameObject bg;

    private void Awake()
    {
        _camera = GetComponentInChildren<CinemachineCamera>();
        bg?.SetActive(LevelManager.SelectedLevel == 0);
    }

    public void Init(Player player) => _camera.Follow = player.transform;
}
