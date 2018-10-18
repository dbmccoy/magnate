using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Account {

    public Entity OwningEntity { get; set; }
    public Bank Bank;
    public float Rate;

    public float Balance;

    public Account(Bank bank, Entity owner, float rate)
    {
        Bank = bank;
        OwningEntity = owner;
        owner.Account = this;
        Rate = rate;
    }

    public void Credit(float amount)
    {
        if(OwningEntity != Bank)
            Bank.Deposits.Credit(amount);
        Balance += amount;
    }

    public float Debit(float amount)
    {
        if (OwningEntity != Bank)
            Bank.Deposits.Debit(amount);
        Balance -= amount;
        return amount;
    }

    public void Transfer(Account account, float amount)
    {
        account.Credit(Debit(amount));
    }

    public float ComputeInterest()
    {
        return Balance * Rate;
    }
}
