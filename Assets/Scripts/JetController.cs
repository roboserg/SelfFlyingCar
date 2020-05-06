using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class JetController : MonoBehaviour
{
    private Rigidbody _rb;
    private Transform _tf;
    public JetEngine EngineFL, EngineFR, EngineBR, EngineBL;
    private JetEngine[] _enginesArray;
    private JetAgent _agent;
    public int anglePitch;
    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _tf = GetComponent<Transform>();
        _agent = GetComponentInParent<JetAgent>();
        _enginesArray = GetComponentsInChildren<JetEngine>();
    }

    public float[] vectorAction = {0,0,0,0};
    public float[] vectorRotation = {0,0,0,0};
    public int[] engineAngles = new int[4];
    void FixedUpdate()
    {
        SetThrottle();
        ApplyRotation();

        for (int i = 0; i < _enginesArray.Length; i++)
        {
            engineAngles[i] = _enginesArray[i].angle;
        }
        
        anglePitch = Mathf.RoundToInt(Vector3.SignedAngle(transform.up, Vector3.up, transform.right));
    }

    void SetThrottle()
    {
        EngineFR.SetThrottle(vectorAction[0]);
        EngineFL.SetThrottle(vectorAction[1]);
        EngineBR.SetThrottle(vectorAction[2]);
        EngineBL.SetThrottle(vectorAction[3]);
    }
    private float _rotationSpeed = 5;
    void ApplyRotation()
    {
        //float input2 = Input.GetAxis("VerticalR");
        EngineFL.transform.Rotate(new Vector3(vectorRotation[0], 0, 0) * _rotationSpeed);
        EngineFR.transform.Rotate(new Vector3(vectorRotation[1], 0, 0) * _rotationSpeed);
        EngineBL.transform.Rotate(new Vector3(vectorRotation[2], 0, 0) * _rotationSpeed);
        EngineBR.transform.Rotate(new Vector3(vectorRotation[3], 0, 0) * _rotationSpeed);
    }
    
    public void Reset()
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        
        vectorAction = new float[] {0, 0, 0, 0};
        vectorRotation = new float[] {0, 0, 0, 0};
        ResetEngineRotation();
    }
    
    public void ResetEngineRotation()
    {
        EngineFL.ResetEngineRotation();
        EngineFR.ResetEngineRotation();
        EngineBL.ResetEngineRotation();
        EngineBR.ResetEngineRotation();
    }
}