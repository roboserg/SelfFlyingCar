using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MyCamera : MonoBehaviour
{
    private CinemachineVirtualCamera _cam;
    private CinemachineFramingTransposer _transposer;
    private CinemachineOrbitalTransposer _orbitalTransposer;
    private float _cameraDistance;
    
    void Start()
    {
        _cam = GetComponent<CinemachineVirtualCamera>();
        _transposer = _cam.GetCinemachineComponent<CinemachineFramingTransposer>();
        _cameraDistance = _transposer.m_CameraDistance;
    }

    public float sensitivity = 6;
    public int minZoom = 5;
    public int maxZoom = 50;
    void Update()
    {
        _cameraDistance -= Input.GetAxis("Mouse ScrollWheel") * sensitivity;
        _cameraDistance = Mathf.Clamp(_cameraDistance, minZoom, maxZoom);
        _transposer.m_CameraDistance = _cameraDistance;
    }
}
