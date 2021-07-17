using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public LevelLoader levelLoader;
    public void ExitButton(){
        Application.Quit();
        Debug.Log("Game closed");
    }

    public void StartGame(){

        levelLoader.LoadLevel(1);
        // SceneManager.LoadScene("DominicanoAR");
    }
}
