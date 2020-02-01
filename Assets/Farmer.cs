using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farmer : MonoBehaviour
{
    private Fence _fenceToRepair = null;
    public float RepairRadius = 1.0f;
    private Rigidbody _body = null;

    // Start is called before the first frame update
    void Start()
    {
        _body = GetComponentInChildren<Rigidbody>();
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

    void RepairFence()
    {
        _repairCounter += Time.deltaTime;
        if (_repairCounter > RepairSpeed)
        {
            _fenceToRepair.gameObject.SetActive(true);
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
            SearchFences();
            _repairCounter = 0.0f;
        }

        if (_fenceToRepair != null)
        {
            GoToFence();
        }
    }
}
