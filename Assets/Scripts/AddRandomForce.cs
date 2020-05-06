using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddRandomForce : MonoBehaviour
{
    public bool isAddRandomForce = false;
    public bool isThrowBall = false;
    public Transform sphere;   
    private Rigidbody _rb;
    
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }
    
    void FixedUpdate()
    {
        if (Time.time > _nextActionTime ) 
        {
            _nextActionTime += timeInterval;
            
            if (isAddRandomForce)
                _rb.AddForce(Random.onUnitSphere * Random.Range(minForce,maxForce), ForceMode.VelocityChange);
            
            if (isThrowBall)
            {
                Instantiate(sphere);
            }
        }
    }
    
    public float timeInterval = 5;
    public int minForce = 1;
    public int maxForce = 3;
    private float _nextActionTime = 2;
    private void AddForceRandom()
    {
        _rb.AddForce(Random.onUnitSphere * Random.Range(minForce,maxForce), ForceMode.VelocityChange);
    }
}
