using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

DateTime cedeBirth = new(year:2001,month:9,day:22);
DateTime accountStart = new(year:2024,month:1,day:1);

var peopleC = new Person() { FirstName = "Frederic", LastName = "Delvaux", BirthDate = cedeBirth};
var peopleB = new Person() { FirstName = "Molie", LastName = "Delvaux", BirthDate = cedeBirth};  // method with setter
// Person PeopleB = new("Ced", "Delval", cede_birth); // method with constructor

var cAccount1 = new CurrentAccount() { Number = "1", Owner = peopleC, CreditLine = 5000};
var sAccount1 = new SavingAccount() { Number = "2", Owner = peopleC, DateLastWithdraw = accountStart};
var cAccount2 = new CurrentAccount() { Number = "3", Owner = peopleB, CreditLine = 5000};

cAccount1.Deposit(5000);
sAccount1.Deposit(23000);

var ifosupBank = new Bank() { Name = "ifosup"};

ifosupBank.AddAccount("0",cAccount1);
cAccount1.ApplyInterest();
ifosupBank.AddAccount("1",sAccount1);
sAccount1.ApplyInterest();

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

public abstract class Account
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

    protected abstract double CalculInterest();

    public virtual void ApplyInterest() 
    {
        this.Balance = Balance + CalculInterest();
    }
}

public class CurrentAccount : Account
{
    public double CreditLine {get;set;}
    public const double interest_p = 0.03;
    public const double interest_n = 9.75;
    protected override double CalculInterest() 
    {
        if (this.Balance >= 0) 
        {
            return this.Balance * interest_p;
        }
        else
        {
            return this.Balance * interest_n;
        }
    }
}

public class SavingAccount : Account
{
    public DateTime DateLastWithdraw {get;set;}
    public const double interest = 0.045;

    public override void Withdraw (double amount) 
    {
        base.Withdraw(amount);
        this.DateLastWithdraw = DateTime.Today;
    }

    protected override double CalculInterest() 
    {
        return this.Balance * interest;
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