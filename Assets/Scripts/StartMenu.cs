using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{

    public Button startButton;

    public GameObject startMenu;
    public GameObject optionsMenu;


    // Start is called before the first frame update
    void Start()
    {
        // Using Lambda callback
        //startButton.onClick.AddListener(() => SceneManager.LoadScene(1)); 
        //startButton.onClick.AddListener(LoadStartScene);

    }

    public void LoadStartScene()
    {
        SceneManager.LoadScene(1);
    }


    public void LoadStartPanel()
    {
        startMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }

    public void LoadOptionPanel()
    {
        startMenu.SetActive(false);
        optionsMenu.SetActive(true); 
    }

    public void Quit()
    {
        Application.Quit(); 
    }

}
