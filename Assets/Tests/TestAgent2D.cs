using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using MLAgents.Sensors;


public class TestAgent2D : Agent
{
    Rigidbody _rb;
    private Transform _tf;
    float[] _myVectorAction;
    
    void Awake () 
    {
        _rb = GetComponent<Rigidbody>();
        _tf = transform;
    }

    public override void Initialize()
    {
        OnEpisodeBegin();
    }
    
    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(0, 4, 0);
        transform.localRotation = Quaternion.identity;
        _rb.velocity = Vector3.zero;
        //_rb.rotation = Quaternion.identity;
        _rb.angularVelocity = Vector3.zero;
        _myVectorAction = new[] {0f, 0f};
    }
    
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(_rb.velocity.y);
        sensor.AddObservation(_rb.velocity.z);
        sensor.AddObservation(_rb.angularVelocity.x);
        sensor.AddObservation(-Mathf.Atan2(transform.forward.y, transform.forward.z) * Mathf.Rad2Deg);
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        _myVectorAction[0] = Mathf.Clamp(vectorAction[0], 0, 1);
        _myVectorAction[1] = Mathf.Clamp(vectorAction[1], 0, 1);

        if (transform.localPosition.y >= 10 || Mathf.Abs(transform.localPosition.z) >= 8 || Mathf.Abs(transform.localPosition.x) >= 8)
        {
            AddReward(-1f);
            EndEpisode();
        }
        else
            AddReward(0.1f);
        
        AddReward(-_rb.velocity.magnitude/10);
        AddReward(-Mathf.Abs(_rb.angularVelocity.x/10));
    }

    public float _enginePower = 8;
    private void FixedUpdate()
    {
        var forceFront = transform.up * (_myVectorAction[0] * _enginePower);
        var posFront = 2 * transform.forward + transform.position;
        _rb.AddForceAtPosition(forceFront, posFront);
        Debug.DrawRay(posFront, -forceFront, Color.green);
        
        var forceBack = transform.up * (_myVectorAction[1] * _enginePower);
        var posBack = -2 * transform.forward + transform.position;
        _rb.AddForceAtPosition(forceBack, posBack);
        Debug.DrawRay(posBack, -forceBack, Color.green);
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