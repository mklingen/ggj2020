﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheep : MonoBehaviour
{
    public float RandomMovementSpeed = 1.0f;
    public float RandomMovementChange = 1.0f;

    public void Die()
    {
        Destroy(this.transform.root.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
