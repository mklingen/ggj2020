using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainStage : MonoBehaviour
{
    // Called when something leaves the main stage.
    private void OnTriggerExit(Collider other)
    {
        bool isSheep = other.gameObject.layer == LayerMask.NameToLayer("Sheep");

        if (isSheep)
        {
            var sheep = other.gameObject.GetComponentInChildren<Sheep>();
            Sheep.Sheeps.Remove(sheep);
            sheep.Die();
            if (Sheep.Sheeps.Count == 0)
            {
                Win();       
            }
        }
    }

    public void Win()
    {
        Fence.Fences.Clear();
        Sheep.Sheeps.Clear();
        SceneManager.LoadScene("menus");
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
