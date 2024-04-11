using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeMenuUI : Modal
{
    public override void Draw() {
        
    }

    public void QuitToMenu() {
        Singleton<GameManager>.Instance.LoadSceneByString("MenuScreen");
    }

    public void Quit() {
        Singleton<GameManager>.Instance.QuitGame();
    }

    public void ResetStorageUnits() {
        foreach(StorageUnit unit in Singleton<StorageUnitsManager>.Instance.StorageUnits) {
            unit.IsOwned = false;
            unit.CloseUnit();
        }

        Singleton<PlayerController>.Instance.ownedStorageUnits.Clear();
    }
}
