using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera _camera;
    private Vector3 _relPos;
    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main;
        _relPos = transform.localPosition;
        GetComponentInChildren<SpriteRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        GetComponentInChildren<SpriteRenderer>().receiveShadows = true;
    }


    void Snap()
    {
        var proj = _camera.WorldToScreenPoint(transform.parent.position + _relPos);
        var intProj = new Vector3(Mathf.FloorToInt(proj.x), Mathf.FloorToInt(proj.y), proj.z);
        transform.position = _camera.ScreenToWorldPoint(intProj);
    }

    private void OnPreRender()
    {
        Snap();
    }

    // Update is called once per frame
    void Update()
    {
        Snap();
        transform.rotation = _camera.transform.rotation;
    }
}
