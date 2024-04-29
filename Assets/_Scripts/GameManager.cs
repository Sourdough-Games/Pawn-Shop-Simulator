using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] public ConfigureDisplaySlotUI configureDisplaySlotUI;

    [SerializeField] private GenericModal welcomeModal;

    public void Start() {
        StartCoroutine(Helper.Wait(1f, () => {
            welcomeModal.Open();
        }));
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void LoadSceneByString(string sceneName) {

        if(Singleton<PlayerController>.Instance != null) {
            Destroy(Singleton<PlayerController>.Instance.gameObject);
        }
        if (Singleton<StorageUnitsManager>.Instance != null)
        {
            Destroy(Singleton<StorageUnitsManager>.Instance.gameObject);
        }
        
        SceneManager.LoadScene(sceneName);
    }
}
