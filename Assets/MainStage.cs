using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainStage : MonoBehaviour
{
    // Called when something leaves the main stage.
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Hello");
        bool isSheep = other.gameObject.layer == LayerMask.NameToLayer("Sheep");

        if (isSheep)
        {
            other.gameObject.GetComponentInChildren<Sheep>().Die();
        }
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
