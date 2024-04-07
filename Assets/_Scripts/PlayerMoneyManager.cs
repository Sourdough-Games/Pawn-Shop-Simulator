using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMoneyManager : Singleton<PlayerMoneyManager>
{
    public UnityEvent<double> WalletAmountChanged;

    [SerializeField] private double walletAmount = 0.00;

    public double WalletAmount {
        get {
            return walletAmount;
        }
    }

    public bool TryTakeMoney(double amount) {
        if(walletAmount < amount) {
            return false;
        }

        TakeMoney(amount);
        return true;
    }

    public bool TryAddMoney(double amount) {
        AddMoney(amount);
        return true;
    }

    private void TakeMoney(double amount) {
        walletAmount -= amount;

        WalletAmountChanged?.Invoke(WalletAmount);
    }

    private void AddMoney(double amount) {
        walletAmount += amount;
        
        WalletAmountChanged?.Invoke(WalletAmount);
    }
}
