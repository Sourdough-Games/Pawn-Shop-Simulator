using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMoneyManager : Singleton<PlayerMoneyManager>
{
    public UnityEvent<float> WalletAmountChanged;

    [SerializeField] private float walletAmount = 0.00f;

    public float WalletAmount {
        get {
            return walletAmount;
        }
    }

    public bool CanTakeMoney(float amount) {
        if (walletAmount < amount)
        {
            Debug.LogError($"{walletAmount} < {amount}");
            return false;
        }
        Debug.LogError($"{walletAmount} > {amount}");
        return true;
    }

    public bool TryTakeMoney(float amount) {
        if(!CanTakeMoney(amount)) {
            return false;
        }

        TakeMoney(amount);
        return true;
    }

    public bool TryAddMoney(float amount) {
        AddMoney(amount);
        return true;
    }

    public void TakeMoney(float amount) {
        walletAmount -= amount;

        WalletAmountChanged?.Invoke(WalletAmount);
    }

    public void AddMoney(float amount) {
        walletAmount += amount;
        
        WalletAmountChanged?.Invoke(WalletAmount);
    }
}
