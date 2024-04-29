using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public abstract partial class Modal : MonoBehaviour, IModal
{
    [Header("Base Modal")]
    [SerializeField] private Button CloseButton;

    public void Close()
    {
        Cursor.lockState = CursorLockMode.Locked;
        gameObject.SetActive(false);

        var c = Singleton<FirstPersonController>.Instance;

        c.Unfreeze();
        c.crosshair = true;

        Singleton<PlayerController>.Instance.openModal = null;
    }

    void Awake() {
        CloseButton.onClick.AddListener(() => {
            Close();
        });
    }

    void Update() {
        if(gameObject.activeSelf) {
            Draw();

            if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab)) {
                Close();
            }
        }
    }

    public abstract void Draw();

    public void Open()
    {
        Cursor.lockState = CursorLockMode.Confined;
        gameObject.SetActive(true);

        StartCoroutine(UnlockButtonsAfterTimeout(.1f));

        var c = Singleton<FirstPersonController>.Instance;

        c.Freeze();
        c.crosshair = false;

        Singleton<PlayerController>.Instance.openModal = this;
    }

    private IEnumerator UnlockButtonsAfterTimeout(float timeout) {
        foreach(Button btn in GetComponentsInChildren<Button>()) {
            btn.enabled = false;
        }
        yield return new WaitForSeconds(timeout);
        foreach (Button btn in GetComponentsInChildren<Button>(true))
        {
            btn.enabled = true;
        }
    }
}
