using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMoneyUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI moneyAmountText;

    PlayerMoneyManager moneyManager;

    void Start()
    {
        moneyManager = Singleton<PlayerMoneyManager>.Instance;

        moneyManager.WalletAmountChanged.AddListener(OnWalletAmountChanged);
    }

    private void OnWalletAmountChanged(float new_amount)
    {
        moneyAmountText.text = Helper.ConvertToDollarAmount((float)new_amount);
    }
}
