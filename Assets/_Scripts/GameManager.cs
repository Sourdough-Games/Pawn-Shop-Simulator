using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public void QuitGame() {
        Application.Quit();
    }

    public void LoadSceneByString(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }
}
