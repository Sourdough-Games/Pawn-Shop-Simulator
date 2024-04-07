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

    private void OnWalletAmountChanged(double new_amount)
    {
        moneyAmountText.text = $"${new_amount:C}";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
