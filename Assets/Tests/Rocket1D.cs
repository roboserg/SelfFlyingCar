using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using MLAgents.Sensors;


public class Rocket1D : Agent
{
    Rigidbody _rb;
    private float[] _myVectorAction;
    
    void Awake () 
    {
        _rb = GetComponent<Rigidbody>();
        //Monitor.SetActive(true);
    }

    public override void Initialize()
    {
        OnEpisodeBegin();
    }
    
    public override void OnEpisodeBegin()
    {
        transform.position = new Vector3(0, 5, 0);
        transform.rotation = Quaternion.identity;
        _rb.velocity = Vector3.zero;
        //_rb.rotation = Quaternion.identity;
        _rb.angularVelocity = Vector3.zero;
        _myVectorAction = new[] {0f, 0f};
    }
    
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position.y);
        sensor.AddObservation(_rb.velocity.y);
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        _myVectorAction = vectorAction;

        if (transform.position.y >= 10 || Mathf.Abs(transform.position.z) >= 8 || Mathf.Abs(transform.position.x) >= 8)
        {
            AddReward(-1f);
            EndEpisode();
        }
        else
            AddReward(0.05f);
        
        
    }

    float _enginePower = 20;
    private void FixedUpdate()
    {
        var forceFront = transform.up * (_myVectorAction[0] * _enginePower);
        _rb.AddForce(forceFront);
    }

    private void OnCollisionEnter(Collision other)
    {
        AddReward(-1f);
        EndEpisode();
    }

    public override float[] Heuristic()
    {
        float[] actionsOut = new float[2];
        actionsOut[0] = Mathf.Abs(Input.GetAxis("RT"));
        actionsOut[1] = Mathf.Abs(Input.GetAxis("LT"));
        return actionsOut;
    }
}