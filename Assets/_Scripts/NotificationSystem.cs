using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization.Components;

public class NotificationSystem : Singleton<NotificationSystem>
{
    [SerializeField] private GameObject SystemMainTransform;
    [SerializeField] private TextMeshProUGUI textObject;

    [SerializeField] private AudioSource soundEffect;

    [SerializeField] private LocalizeStringEvent localString;

    private Coroutine ActiveRoutine;

    private void Start() {
        SystemMainTransform.SetActive(false);
    }

    public void ShowMessage(string LocalizationKey, float seconds = 2f) {
        localString.StringReference.TableEntryReference = LocalizationKey;

        textObject.text = localString.StringReference.GetLocalizedString();

        StartNotification(seconds);
    }
 
    private void StartNotification(float seconds = 2f) {
        if(ActiveRoutine != null) {
            StopCoroutine(ActiveRoutine);
        }

        ActiveRoutine = StartCoroutine(ShowMessageRoutine(seconds));
    }

    public IEnumerator ShowMessageRoutine(float seconds) {
        SystemMainTransform.SetActive(true);
        soundEffect.Play();
        yield return new WaitForSeconds(seconds);
        SystemMainTransform.SetActive(false);
    }
}
