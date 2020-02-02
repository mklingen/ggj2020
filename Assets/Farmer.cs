using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farmer : MonoBehaviour
{
    public LineRenderer Laser = null;
    private Fence _fenceToRepair = null;
    public float RepairRadius = 1.0f;
    private Rigidbody _body = null;
    private Vector3 _right = Vector3.back;
    // Start is called before the first frame update
    void Start()
    {
        _right = Camera.main.transform.right;
        _sprite = GetComponentsInChildren<SpriteRenderer>()[1];
        _body = GetComponentInChildren<Rigidbody>();
        _animator = GetComponentInChildren<Animator>();
    }

    void SearchFences()
    {
        Fence closestFence = null;
        float closestDist = float.MaxValue;
        foreach (var fence in Fence.Fences)
        {
            if (!fence.isActiveAndEnabled)
            {
                float dist = (fence.transform.position - transform.position).sqrMagnitude;
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestFence = fence;
                }
            }
        }
        _fenceToRepair = closestFence;
    }


    public float Kp = 0.1f;
    public float RepairSpeed = 1.5f;
    private float _repairCounter = 0.0f;
    private SpriteRenderer _sprite = null;
    private Animator _animator = null;

    void RepairFence()
    {
        if (_fenceToRepair != null)
        {
            Laser.enabled = true;
            Laser.SetPosition(0, transform.position + Vector3.up * 0.5f);
            Laser.SetPosition(1, _fenceToRepair.transform.position + Random.insideUnitSphere * 0.15f);
        }
        _repairCounter += Time.deltaTime;
        if (_repairCounter > RepairSpeed)
        {
            _fenceToRepair.gameObject.SetActive(true);
            GetComponent<AudioSource>().Play();
        }
    }

    void GoToFence()
    {
        var delta = (_fenceToRepair.transform.position - transform.position);
        if (delta.magnitude < RepairRadius)
        {
            RepairFence();
        }
        else
        {
            _body.AddForce(delta * Kp);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_fenceToRepair == null || _fenceToRepair.isActiveAndEnabled)
        {
            Laser.enabled = false;
            SearchFences();
            _repairCounter = 0.0f;
        }

        if (_fenceToRepair != null)
        {
            GoToFence();
        }

        float dotRight = Vector3.Dot(_body.velocity, _right);
        _sprite.flipX = dotRight > 0 ? true : false;

        _animator.speed = _body.velocity.magnitude / 0.5f;
        

    }
}
