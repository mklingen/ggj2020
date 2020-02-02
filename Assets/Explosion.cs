using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private float _startTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        _startTime = Time.time;    
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - _startTime > 5.0f)
        {
            Destroy(this.transform.root.gameObject);
        }
    }
}
