using System;
using Unity.Mathematics;
using UnityEngine;

public class JetEngine : MonoBehaviour
{
    float _enginePower = 8;
    private Rigidbody _rb;
    private JetController _controller;
    public int angle;
    public bool FR, FL, BR, BL;

    private float _angleMinFront = 90, _angleMaxFront = -90, _angleMinBack = -180, _angleMaxBack = -90;
    private float _angleMin, _angleMax;
    private ParticleSystem[] _arrayPs;
    
    void Start()
    {
        _rb = GetComponentInParent<Rigidbody>();
        _controller = GetComponentInParent<JetController>();
        ResetEngineRotation();

        if (FR || FL)
        {
            _angleMax = _angleMaxFront;
            _angleMin = _angleMinFront;
        }
        else
        {
            _angleMax = _angleMaxBack;
            _angleMin = _angleMinBack;
        }
        
        _arrayPs = GetComponentsInChildren<ParticleSystem>();
    }

    public void ResetEngineRotation()
    {
        transform.localRotation = Quaternion.Euler(-180, 0,0);
    }

    private void FixedUpdate()
    {
        angle = Mathf.RoundToInt(Vector3.SignedAngle(-transform.up, _controller.transform.up, -transform.right));
        
        if(angle > 90)
            transform.localRotation = Quaternion.Euler(_angleMax, 0, 0);
        else if(angle < -90)
            transform.localRotation = Quaternion.Euler(_angleMin, 0, 0);
        if ((BR || BL) && angle < 0)
            transform.localRotation = Quaternion.Euler(_angleMin, 0, 0);
        
        ApplyThrottle();
    }

    private float _throttle = 0;
    public void SetThrottle(float throttle)
    {
        _throttle = Mathf.Clamp(throttle, 0, 1);
    }

    public void ApplyThrottle()
    {
        var force = transform.up * (_throttle * _enginePower);
        _rb.AddForceAtPosition(-force * 500, transform.position);
        
        if(BL || BR)
            Debug.DrawRay(transform.position, transform.up * (_throttle * 4), Color.green);
        else
            Debug.DrawRay(transform.position, transform.up * (_throttle * 4), Color.cyan);

        if (_arrayPs.Length == 0) return;
        foreach (ParticleSystem ps in _arrayPs)
        {
            var main = ps.main;
            main.startLifetime = _throttle / 3.5f;
        }
    }

    public void ApplyRotation(float value)
    {
        transform.Rotate(new Vector3(value, 0, 0));
    }

    void CheckAngles()
    {
        var dotFront = Vector3.Dot(_controller.transform.up, -transform.up);
        var crossFront = Vector3.Cross(_controller.transform.up, -transform.up);

        if (dotFront < 0 && crossFront.x <= 1 && crossFront.x > 0)
            transform.localRotation = Quaternion.Euler(_angleMin, 0, 0);

        if (dotFront < 0 && crossFront.x >= -1 && crossFront.x < 0)
            transform.localRotation = Quaternion.Euler(_angleMax, 0, 0);

        if (BR || BL)
        {
            if (dotFront < 1 && crossFront.x >= -1 && crossFront.x < 0)
                transform.localRotation = Quaternion.Euler(_angleMax, 0, 0);
        }
    }
}
