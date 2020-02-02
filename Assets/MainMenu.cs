using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Cancel"))
        {
            ExitScene();
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("main");
    }

    public void ShowControls()
    {
        SceneManager.LoadScene("controls");
    }

    public void ShowCredits()
    {
        SceneManager.LoadScene("credits");
    }

    public void ExitScene()
    {
        Fence.Fences.Clear();
        Sheep.Sheeps.Clear();
        var scene = SceneManager.GetActiveScene();
       if (scene.name == "main" || scene.name == "controls" || scene.name == "credits" || scene.name == "win")
        {
            SceneManager.LoadScene("menus");
        }

        Application.Quit();
    }
}
