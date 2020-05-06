﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SphereObject : MonoBehaviour
{
    public int distance = 30;
    public Vector2 mass = new Vector2(100,200);
    public Vector2 speed = new Vector2(50, 100);
    private Rigidbody _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.mass = Random.Range(mass.x, mass.y);
        
        Vector3 pos = GameObject.FindWithTag("Player").transform.position;
        InitPosition(pos);
        ShootTarget(pos);

        Destroy(gameObject, 4);
    }

    public void InitPosition(Vector3 pos)
    {
        transform.position = pos + Random.onUnitSphere * distance;
        if(transform.position.y < 1)
            transform.position = new Vector3(transform.position.x, 1.5f, transform.position.z);
    }

    public void ShootTarget(Vector3 targetPos)
    {
        Vector3 dir = targetPos - transform.position;
        _rb.velocity = dir.normalized * (Random.Range(speed.x, speed.y));
    }

    private void OnCollisionEnter(Collision other)
    {
        //_rb.useGravity = true;
        Destroy(gameObject, 2);
    }
}
