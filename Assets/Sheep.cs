﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheep : MonoBehaviour
{
    public float RandomMovementSpeed = 1.0f;
    public float RandomMovementChange = 1.0f;

    public GameObject cloudTemplate;
    private Rigidbody _cloudBody = null;

    private float RandomMoveCounter = 0;
    private Vector3 _randomMotion = Vector3.zero;
    private Rigidbody _body = null;
    public void Die()
    {
        // create a new object where sheep used to be
        GameObject cloud = Instantiate(cloudTemplate);
        cloud.transform.SetPositionAndRotation(transform.position, transform.rotation);
        cloud.GetComponentInChildren<Rigidbody>().velocity = _body.velocity;

        Destroy(this.transform.root.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        _body = GetComponentInChildren<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (RandomMoveCounter < 0)
        {
            _randomMotion = Random.insideUnitSphere * RandomMovementSpeed;
            _randomMotion.y = 0;
            RandomMoveCounter = RandomMovementChange;
        }
        _body.AddForce(_randomMotion);
        RandomMoveCounter -= Time.deltaTime;
    }
}
