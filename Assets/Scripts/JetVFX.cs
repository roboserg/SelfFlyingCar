using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetVFX : MonoBehaviour
{
    public ParticleSystem[] enginePS;
    public ParticleSystem sparkle;
    public GameObject lightTrails;

    private Rigidbody _rb;
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }


    void Update()
    {
        //lightTrails.SetActive(_rb.velocity.magnitude > 5);
        //Debug.Log(_rb.velocity.magnitude);
    }

    private void OnCollisionEnter(Collision other)
    {
        sparkle.transform.position = other.contacts[0].point;
        sparkle.Play(true);
    }

    void OnCollisionStay(Collision collision)
    {
//        sparkle.transform.position = collision.contacts[0].point;
  //      sparkle.Play(true);
    }

    //Called when the ship stops colliding with something solid
    void OnCollisionExit(Collision collision)
    {
        sparkle.Stop(true);
    }
}
