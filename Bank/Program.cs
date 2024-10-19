using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

DateTime cedeBirth = new(year:2000,month:12,day:30);
DateTime accountStart = new(year:2024,month:1,day:1);

var peopleC = new Person() { FirstName = "Cedric", LastName = "Delval", BirthDate = cedeBirth};
var peopleB = new Person() { FirstName = "Bastien", LastName = "Delval", BirthDate = cedeBirth};  // method with setter
// Person PeopleB = new("Ced", "Delval", cede_birth); // method with constructor

var cAccount1 = new CurrentAccount() { Number = "1", Owner = peopleC, CreditLine = 5000};
var sAccount1 = new SavingAccount() { Number = "2", Owner = peopleC, DateLastWithdraw = accountStart};
var cAccount2 = new CurrentAccount() { Number = "3", Owner = peopleB, CreditLine = 5000};

cAccount1.Deposit(5000);
sAccount1.Deposit(23000);

var ifosupBank = new Bank() { Name = "ifosup"};

ifosupBank.AddAccount("0",cAccount1);
ifosupBank.AddAccount("1",sAccount1);

Console.WriteLine(ifosupBank.GetSumOfPersonBalances(peopleC));
Console.WriteLine(ifosupBank.GetRegisterOfPersonAccount(peopleC));

public class Person 
{
    public required string FirstName {get;set;}
    public required string LastName {get;set;}
    public required DateTime BirthDate {get;set;}

    public override string ToString()
    {
        return $"{FirstName} {LastName}";
    }

    // access type name { access get; access set;}

    // public Person(string firstName, string lastName, DateTime birthDate) // usual constructor
    // {
    //     this.FirstName = firstName;
    //     this.LastName = lastName;
    //     this.BirthDate = birthDate;
    // }
    // public Person() {} // strat for counter constructor with setter method
}

public class Account
{
    public required string Number {get;set;}
    public double Balance {get; private set;}
    public required Person Owner {get;set;}
    public virtual void Withdraw (double amount) 
    {
        this.Balance = Balance - amount;
    }
    public virtual void Deposit (double amount) 
    {
        this.Balance = Balance + amount;
    }
}

public class CurrentAccount : Account
{
    public double CreditLine {get;set;}
}

public class SavingAccount : Account
{
    public DateTime DateLastWithdraw {get;set;}
    public override void Withdraw (double amount) 
    {
        base.Withdraw(amount);
        this.DateLastWithdraw = DateTime.Today;
    }
}

public class Bank
{
    public Dictionary<string, Account> Accounts {get; private set;} = new Dictionary<string, Account>();
    public required string Name {get;set;}
    public void AddAccount(string number, Account account) 
    {
        Accounts.Add(number, account);
    }
    public void DeleteAccount(string number)
    {
        Accounts.Remove(number);
    }
    public double GetBalance(string number)
    {
        return Accounts[number].Balance;
    }
    public double GetSumOfPersonBalances(Person Owner)
    {
        double sum = 0;
        foreach (Account account in Accounts.Values) {
            if (account.Owner == Owner) 
            {
                sum = sum + account.Balance;
            }
        }
        return sum;
    }
    public string GetRegisterOfPersonAccount(Person Owner)
    {
        string view = "";
        foreach (Account account in Accounts.Values) {
            if (account.Owner == Owner) 
            {
                view = $"{view} \n {account.Owner} : {account.Number} => solde = {account.Balance}";
            }
        }
        return view;
    }
}