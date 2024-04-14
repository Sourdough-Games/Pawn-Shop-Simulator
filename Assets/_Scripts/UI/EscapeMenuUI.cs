using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;

[Serializable]
class LanguageSelectionData {
    public string languageCode;
    public string languageText;
}

public class EscapeMenuUI : Modal
{

    [SerializeField] TMP_Dropdown languageDropdown;

    [SerializeField] private List<LanguageSelectionData> languageOptions = new();

    public override void Draw() {
        
    }

    void Start() {
        foreach(LanguageSelectionData d in languageOptions) {
            languageDropdown.AddOptions(new List<string>() { d.languageText });
        }

        languageDropdown.onValueChanged.AddListener(OnLanguageDropdownValueChanged);
    }

    private void OnLanguageDropdownValueChanged(int chosen_id)
    {
        LanguageSelectionData d = languageOptions[chosen_id];
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale(d.languageCode);
        
        GetComponentsInChildren<LocalizeStringEvent>().ToList().ForEach(e => e.RefreshString());
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
            unit.CanSpawn = true;
            unit.CloseUnit();
        }

        Singleton<PlayerController>.Instance.ownedStorageUnits.Clear();
    }
}
