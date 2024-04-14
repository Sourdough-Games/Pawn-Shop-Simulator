using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenuScene : MonoBehaviour
{
    [SerializeField] private StorageUnit unit;

    [SerializeField] private MainMenuUI menu;

    [SerializeField] private Animator guyAnimator;

    void Start()
    {
        StartCoroutine(HandleShowMenu());
    }

    public IEnumerator HandleShowMenu() {
        yield return new WaitForSeconds(1);

        //unit.CanSpawn = true;
        unit.OpenUnit();

        yield return new WaitForSeconds(.6f);

        guyAnimator.Play("Dance");

        while (!unit.IsOpen) {
            yield return new WaitForSeconds(.1f);
        }

        menu.gameObject.SetActive(true);
    }
}
