using System;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private CinemachineCamera _camera;

    private void Awake() => _camera = GetComponentInChildren<CinemachineCamera>();

    public void Init(Player player) => _camera.Follow = player.transform;
}
