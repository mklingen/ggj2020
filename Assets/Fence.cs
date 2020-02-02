using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fence : MonoBehaviour
{
    public static List<Fence> Fences = new List<Fence>();
    public GameObject Explosion = null;

    // Start is called before the first frame update
    void Start()
    {
        Fences.Add(this);    
    }

    public void Die()
    {
        var exp = Instantiate(Explosion);
        exp.transform.SetPositionAndRotation(transform.position, transform.rotation);
        this.transform.root.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
