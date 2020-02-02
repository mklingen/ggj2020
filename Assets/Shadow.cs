using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour
{
    private float _initY = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        _initY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, _initY, transform.position.z);
    }
}
