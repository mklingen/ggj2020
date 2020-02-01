using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farmer : MonoBehaviour
{
    private Camera _camera = null;
    private Vector3 _velocity = new Vector3(0, 0, 0);
    public float Friction = 0.9f;
    public float Speed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 fwd =  _camera.transform.forward;
        fwd.y = 0.0f;
        fwd.Normalize();
        Vector3 right =_camera.transform.right;
        right.y = 0.0f;
        right.Normalize();
        
        _velocity = (fwd * Input.GetAxis("Vertical1") + right * Input.GetAxis("Horizontal1")) * Speed;
        transform.position += _velocity * Time.deltaTime;
        _velocity *= Friction;
    }
}
