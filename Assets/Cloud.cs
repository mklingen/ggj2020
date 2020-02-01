using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    public int axis = 0;
    public float cloudFreq = 0.1f;
    public float cloudAmp = 0.5f;
    private Vector3 _cloudStartPos = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        _cloudStartPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPosition = _cloudStartPos;
        
        if (axis == 0)
        {
            newPosition.z += cloudAmp * Mathf.Sin(cloudFreq * Time.time);
        } else
        {
            newPosition.x += cloudAmp * Mathf.Sin(cloudFreq * Time.time);
        }
        transform.position = newPosition;
    }
}
