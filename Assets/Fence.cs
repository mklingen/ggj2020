using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fence : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Die()
    {
        Destroy(this.transform.root.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
