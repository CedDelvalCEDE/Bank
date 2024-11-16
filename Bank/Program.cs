using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

DateTime fedeBirth = new(year:2001,month:9,day:22);
DateTime modeBirth = new(year:1995,month:7,day:14);
DateTime accountStart = new(year:2024,month:1,day:1);

var peopleC = new Person("Frederic", "Delvaux", fedeBirth); // { FirstName = "Frederic", LastName = "Delvaux", BirthDate = fedeBirth};
var peopleB = new Person("Molie", "Delvaux", modeBirth); // { FirstName = "Molie", LastName = "Delvaux", BirthDate = modeBirth};  // method with setter

var cAccount1 = new CurrentAccount("1", peopleC, 5000); // { Number = "1", Owner = peopleC, CreditLine = 5000};
var sAccount1 = new SavingAccount("2", 25000, peopleC,  accountStart); // { Number = "2", Owner = peopleC, DateLastWithdraw = accountStart};
var cAccount2 = new CurrentAccount("3", peopleB, 5000); // { Number = "3", Owner = peopleB, CreditLine = 5000};

cAccount1.Deposit(5000);
sAccount1.Deposit(23000);
cAccount2.Deposit(5000);

var ifosupBank = new Bank() { Name = "ifosup"};

ifosupBank.AddAccount("0",cAccount1);
cAccount1.ApplyInterest();
ifosupBank.AddAccount("1",sAccount1);
sAccount1.ApplyInterest();
ifosupBank.AddAccount("2",cAccount2);

Console.WriteLine(ifosupBank.GetSumOfPersonBalances(peopleC));
Console.WriteLine(ifosupBank.GetRegisterOfPersonAccount(peopleC));
Console.WriteLine(ifosupBank.GetRegisterOfPersonAccount(peopleB));

// access [abstrat, virtual, override] type name { access get; access set;}

public class Person 
{
    public string FirstName {get; private set;}
    public string LastName {get; private set;}
    public DateTime BirthDate {get; private set;}

    public override string ToString()
    {
        return $"{FirstName} {LastName}";
    }

    // public Person() {} // default constructor with setter method

    public Person(string firstName, string lastName, DateTime birthDate) // usual constructor
    {
        this.FirstName = firstName;
        this.LastName = lastName;
        this.BirthDate = birthDate;
    }
}

public interface IAccount
{
    // public double Balance;
    public void Deposit(double amount);
    public void Withdraw(double amount);
}

public interface IBankAccount : IAccount
{
    public void ApplyInterest();
    // public string Number;
    // public Person Owner;
}

public abstract class Account : IBankAccount
{
    public string Number {get; private set;}
    public double Balance {get; private set;}
    public Person Owner {get; private set;}

    public Account() {}

    public Account(string number, Person owner) : this()
    {
        this.Number = number;
        this.Owner = owner;
    }

    public Account(string number, double balance, Person owner) : this()
    {
        this.Number = number;
        this.Balance = balance;
        this.Owner = owner;
    }

    public virtual double TakeBalance() 
    {
        return this.Balance;
    }

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

    public CurrentAccount() {}

    public CurrentAccount(string number, Person owner, double creditLine) : base(number, owner) 
    {
        this.CreditLine = creditLine;
    }

    public CurrentAccount(string number, double balance, Person owner, double creditLine) : base(number, balance, owner) 
    {
        this.CreditLine = creditLine;
    }

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
    public DateTime DateLastWithdraw {get; private set;}
    public const double interest = 0.045;

    public SavingAccount() {}

    public SavingAccount(string number, Person owner) : base(number, owner) 
    {

    }

    public SavingAccount(string number, double balance, Person owner, DateTime dateLastWithdraw) : base(number, balance, owner) 
    {
        this.DateLastWithdraw = dateLastWithdraw;
    }

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