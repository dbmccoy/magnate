using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Bank : Entity, IOwnable {
    
    private Dictionary<Entity, Account> Accounts = new Dictionary<Entity, Account>();
    private Dictionary<Entity, Loan> Loans = new Dictionary<Entity, Loan>();

    public Account Deposits;

    public Bank(string name, Entity owner) : base(name) 
    {
        Name = name;
        OwningEntity = owner;
        OwningEntity.AcquireAsset(this);
        Deposits = new Account(this, this, 0f);
    }

    public Account NewAccount(Entity owner)
    {
        var account = new Account(this, owner, .01f);
        Accounts.Add(owner, account);

        return account;
    }

    public void IssueLoan(Loan loan)
    {
        Loans.Add(loan.Borrower, loan);
        Assets.Add(loan);

        loan.Borrower.Liabilities.Add(loan);
        loan.Borrower.Account.Credit(loan.Balance);
        Deposits.Balance -= loan.Balance;
    }
}


