using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenuScene : MonoBehaviour
{
    [SerializeField] private StorageUnit unit;

    [SerializeField] private MainMenuUI menu;

    void Start()
    {
        StartCoroutine(HandleShowMenu());
    }

    public IEnumerator HandleShowMenu() {
        yield return new WaitForSeconds(1);

        //unit.CanSpawn = true;
        unit.OpenUnit();

        while(!unit.IsOpen) {
            yield return new WaitForSeconds(.1f);
        }

        menu.gameObject.SetActive(true);
    }
}
