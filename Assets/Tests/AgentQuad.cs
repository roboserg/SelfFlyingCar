using System;
using UnityEngine;
using MLAgents;
using MLAgents.Sensors;
using Random = UnityEngine.Random;


public class AgentQuad : Agent
{
    public Transform Target;
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

    void ResetTarget()
    {
        Vector3 posTarget = Random.onUnitSphere * 10;
        if (posTarget.y < 0)
            posTarget.y = -posTarget.y;
        if (Random.value < 0.5f)
        {
            if (Random.value < 0.25f)
                posTarget.y = 0;
            else
            {
                posTarget.y = 0.5f;
            }
        }

        Target.transform.localPosition = posTarget;
    }
    
    public override void OnEpisodeBegin()
    {
        Vector3 pos = Random.onUnitSphere * 15;
        if (pos.y < 0)
            pos.y = -pos.y;

        transform.localPosition = pos;
        transform.localRotation = Quaternion.identity;
        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _myVectorAction = new[] {0f, 0f, 0f, 0f};

        ResetTarget();
    }
    
    public override void CollectObservations(VectorSensor sensor)
    {
        //sensor.AddObservation(Target.position - transform.position);
        sensor.AddObservation(transform.position);
        sensor.AddObservation(transform.rotation);
        sensor.AddObservation(_rb.velocity);
        sensor.AddObservation(_rb.angularVelocity);
        
        /*
        float anglePitch = -Mathf.Atan2(transform.forward.y, transform.forward.z) * Mathf.Rad2Deg;
        float angleRoll = -Mathf.Atan2(transform.forward.y, transform.forward.x) * Mathf.Rad2Deg;
        float angleYaw = -Mathf.Atan2(transform.forward.z, transform.forward.x) * Mathf.Rad2Deg;
        
        Quaternion q = transform.rotation;
        float Pitch = Mathf.Rad2Deg * Mathf.Atan2(2 * q.x * q.w - 2 * q.y * q.z, 1 - 2 * q.x * q.x - 2 * q.z * q.z);
        float Yaw = Mathf.Rad2Deg * Mathf.Atan2(2 * q.y * q.w - 2 * q.x * q.z, 1 - 2 * q.y * q.y - 2 * q.z * q.z);
        float Roll = Mathf.Rad2Deg * Mathf.Asin(2 * q.x * q.y + 2 * q.z * q.w);
        
        Vector3 angle = new Vector3(anglePitch, angleRoll, angleYaw);
        Vector3 angle2 = new Vector3(Pitch, Roll, Yaw);
        */
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        _myVectorAction[0] = Mathf.Clamp(vectorAction[0], 0, 1);
        _myVectorAction[1] = Mathf.Clamp(vectorAction[1], 0, 1);
        _myVectorAction[2] = Mathf.Clamp(vectorAction[2], 0, 1);
        _myVectorAction[3] = Mathf.Clamp(vectorAction[3], 0, 1);

        if (transform.localPosition.y >= 30 || Mathf.Abs(transform.localPosition.z) >= 25 || Mathf.Abs(transform.localPosition.x) >= 25)
        {
            AddReward(-1f);
            EndEpisode();
        }

        AddReward(0.1f);
        //AddReward(-_rb.velocity.magnitude/10);
        //AddReward(-_rb.angularVelocity.magnitude/10);
        
        //AddReward(-0.001f);
        //var dirToTarget = Target.position - transform.position;
        //var movingTowardsDot = Vector3.Dot(_rb.velocity, dirToTarget.normalized);
        //AddReward(0.03f * movingTowardsDot);
        
        //Debug.Log(GetCumulativeReward());
        //float reward = 10 - Vector3.Distance(transform.position, Target.transform.position);

    }
    public float enginePower = 10;
    private void FixedUpdate()
    {
        var forceFront = transform.up * (_myVectorAction[0] * enginePower);
        var posFront = 1.5f * transform.forward + transform.position;
        _rb.AddForceAtPosition(forceFront, posFront);
        Debug.DrawRay(posFront, -forceFront, Color.green);
        
        var forceBack = transform.up * (_myVectorAction[1] * enginePower);
        var posBack = -1.5f * transform.forward + transform.position;
        _rb.AddForceAtPosition(forceBack, posBack);
        Debug.DrawRay(posBack, -forceBack, Color.green);
        
        var forceRight = transform.up * (_myVectorAction[2] * enginePower);
        var posRight = 1.5f * transform.right + transform.position;
        _rb.AddForceAtPosition(forceRight, posRight);
        Debug.DrawRay(posRight, -forceRight, Color.green);
        
        var forceLeft = transform.up * (_myVectorAction[3] * enginePower);
        var posLeft = -1.5f * transform.right + transform.position;
        _rb.AddForceAtPosition(forceLeft, posLeft);
        Debug.DrawRay(posLeft, -forceLeft, Color.green);
    }

/*
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Respawn"))
        {
            AddReward(1f);
            ResetTarget();
        }
    }
*/

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            AddReward(-1f);
            EndEpisode();
        }
        //AddReward(10-other.relativeVelocity.magnitude);
        //Debug.Log(GetCumulativeReward());
    }

    public override float[] Heuristic()
    {
        float[] actionsOut = new float[4];
        actionsOut[0] = Mathf.Abs(Input.GetAxis("RT"));
        actionsOut[1] = Mathf.Abs(Input.GetAxis("LT"));
        actionsOut[2] = Mathf.Abs(Input.GetAxis("Vertical"));
        actionsOut[3] = Mathf.Abs(Input.GetAxis("Horizontal"));
        return actionsOut;
    }
}