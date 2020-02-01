using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fence : MonoBehaviour
{
    public static List<Fence> Fences = new List<Fence>();

    // Start is called before the first frame update
    void Start()
    {
        Fences.Add(this);    
    }

    public void Die()
    {
        this.transform.root.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
