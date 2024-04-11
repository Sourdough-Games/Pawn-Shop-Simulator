using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : Modal
{
    public override void Draw() {
        
    }

    public void Quit() {
        Application.Quit();
    }

    public void LoadGame()
    {
        SceneManager.LoadScene("MainScene");
    }
}
