using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using MLAgents.Sensors;
using Random = UnityEngine.Random;

public class JetAgent : Agent
{
    public bool isQuadMode = false;
    public Transform target;
    [SerializeField] private float reward;
    
    Rigidbody _rb;
    JetController _controller;
    Vector3 _startingPosition;
    
    void Awake () 
    {
        _rb = GetComponent<Rigidbody>();
        _startingPosition = transform.position;
        _controller = GetComponent<JetController>();
        Cursor.visible = false;
    }
    
    public override void OnEpisodeBegin()
    {
        _controller.Reset();
        transform.localPosition = GetRandomPosition(90, 90, 40, 80);
        transform.position = _startingPosition;
        
        _isLanded = false;
        _isGotReward = false;
        _distScaled = 1;
        _relVelScaled = 1;
    }
    
    Vector3 GetRandomPosition(int minRadius, int maxRadius, int minHeight, int maxHeight)
    {
        var pos = Vector3.zero;
        Vector2 circle = Random.insideUnitCircle.normalized * Random.Range(minRadius, maxRadius);
        pos.x = circle.x;
        pos.y = Random.Range(minHeight, maxHeight);
        pos.z = circle.y;
        return pos;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(transform.rotation);
        sensor.AddObservation(_rb.velocity);
        sensor.AddObservation(_rb.angularVelocity);

        if(isQuadMode == false)
        {
            sensor.AddObservation(_controller.engineAngles[0]);
            sensor.AddObservation(_controller.engineAngles[1]);
            sensor.AddObservation(_controller.engineAngles[2]);
            sensor.AddObservation(_controller.engineAngles[3]);
        }
        
        //sensor.AddObservation(target.position - transform.position);
    }
    
    public override void OnActionReceived(float[] vectorAction)
    {
        _controller.vectorAction[0] = vectorAction[0];
        _controller.vectorAction[1] = vectorAction[1];
        _controller.vectorAction[2] = vectorAction[2];
        _controller.vectorAction[3] = vectorAction[3];

        if (isQuadMode == false)
        {
            _controller.vectorRotation[0] = vectorAction[4];
            _controller.vectorRotation[1] = vectorAction[5];
            _controller.vectorRotation[2] = vectorAction[6];
            _controller.vectorRotation[3] = vectorAction[7];
        }

        MyFixedUpdate();
        reward = GetCumulativeReward();
    }
    
    private void MyFixedUpdate()
    {
        //AddReward(0.001f);
        var facingForwardDot = Vector3.Dot(Vector3.up, transform.up);
        AddReward(facingForwardDot/1000);
        //AddReward(-0.00001f);
        
        if (_isLanded)
        {
            Debug.Log("--------------");
            Debug.Log("distScaled " + _distScaled);
            Debug.Log("relVelScaled: " + _relVelScaled);
            Debug.Log("--------------");
            //AddReward(1 - _distScaled);
            //AddReward(1 - _relVelScaled);
            EndEpisode();
        }
        
        if (Mathf.Abs(transform.position.x) >= 100 || transform.position.y >= 100 || Mathf.Abs(transform.position.z) >= 100)
        {
            AddReward(-1f);
            EndEpisode();
        }
    }
    
    private bool _isLanded = false;
    private bool _isGotReward = false;
    private float _distScaled, _relVelScaled;
    
    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("Sphere")) return;

        if (_isLanded) return;
        _isLanded = true;
        
        Debug.Log(StepCount +  " OnCollisionEnter");
            
        var vectorToTarget = target.transform.position - transform.position;
        vectorToTarget.y = 0;

        _distScaled = Mathf.Clamp(Mathf.Pow(vectorToTarget.magnitude / 100, 0.4f), 0, 1);
        //AddReward(1 - distScaled);

        _relVelScaled =  Mathf.Clamp(Mathf.Pow(other.relativeVelocity.magnitude / 50, 0.4f), 0, 1);
        //AddReward(1 - relVelScaled);
        
        var angularVelScaled =  Mathf.Clamp(Mathf.Pow(_rb.angularVelocity.magnitude / 30, 0.4f), 0, 1);
        //AddReward(1 - angularVelScaled);
        
        var facingForwardDot = Vector3.Dot(Vector3.up, transform.up);
        //AddReward(facingForwardDot);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // if(other.gameObject.CompareTag("Target"))
        // {
        //     AddReward(1f);
        //     ResetTarget();
        // }
    }

    void MyRewards(Collision other)
    {
        var vectorToTarget = target.transform.position - transform.position;
        vectorToTarget.y = 0;
        var distScaled = Mathf.Clamp(vectorToTarget.magnitude / 100, 0, 2);
        var distScaled2 = Mathf.Clamp(vectorToTarget.magnitude / 8, 0, 1);
        var relativeVelScaled = Mathf.Clamp(other.relativeVelocity.magnitude / 50, 0, 1);
        var relativeVelScaled2 = Mathf.Clamp(other.relativeVelocity.magnitude / 5, 0, 1);
        var facingForwardDot = Vector3.Dot(Vector3.up, transform.up);
        var angularVelScaled = Mathf.Clamp(_rb.angularVelocity.magnitude / 10, 0, 1);
        //var reward = 6 - distScaled - distScaled2*2 - relativeVelScaled - relativeVelScaled2 - angularVelScaled/2 + facingForwardDot;
    }
    
    public override float[] Heuristic()
    {
        float[] actionsOut = new float[8];
        actionsOut[0] = Mathf.Abs(Input.GetAxis("RT"));
        actionsOut[1] = Mathf.Abs(Input.GetAxis("RT"));
        actionsOut[2] = Mathf.Abs(Input.GetAxis("RT"));
        actionsOut[3] = Mathf.Abs(Input.GetAxis("RT"));

        if (isQuadMode == false)
        {
            actionsOut[4] = Input.GetAxis("Vertical");
            actionsOut[5] = Input.GetAxis("Vertical");
            actionsOut[6] = Input.GetAxis("Vertical");
            actionsOut[7] = Input.GetAxis("Vertical");
        }

        return actionsOut;
    }
    
    void ResetTarget()
    {
        Vector3 posTarget = Random.onUnitSphere * 60;
        if (posTarget.y < 0)
            posTarget.y = -posTarget.y;

        if (posTarget.y < 30)
            posTarget.y = 30;

        target.transform.localPosition = posTarget;
    }
    
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Backspace))
            EndEpisode();
        
        if(Input.GetKey(KeyCode.Escape))
            Application.Quit();
    }
}