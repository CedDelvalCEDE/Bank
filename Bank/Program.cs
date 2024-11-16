using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

DateTime fedeBirth = new(year:2001,month:9,day:22);
DateTime modeBirth = new(year:1995,month:7,day:14);
DateTime accountStart = new(year:2024,month:11,day:16);

var peopleC = new Person("Frederic", "Delvaux", fedeBirth); // { FirstName = "Frederic", LastName = "Delvaux", BirthDate = fedeBirth};
var peopleB = new Person("Molie", "Delvaux", modeBirth); // { FirstName = "Molie", LastName = "Delvaux", BirthDate = modeBirth};  // method with setter

var cAccount1 = new CurrentAccount("1", peopleC, 2000); // { Number = "1", Owner = peopleC, CreditLine = 2000};
var sAccount1 = new SavingAccount("2", peopleC,  accountStart); // { Number = "2", Owner = peopleC, DateLastWithdraw = accountStart};
var cAccount2 = new CurrentAccount("3", peopleB, 2000); // { Number = "3", Owner = peopleB, CreditLine = 2000};

try 
{
    cAccount1.Deposit(5000);
    sAccount1.Deposit(23000);
    cAccount2.Deposit(5000);
}
catch (InsufficientBalanceException)
{
    Console.WriteLine("solde inférieur au retrait.");
}
catch (ArgumentOutOfRangeException)
{
    Console.WriteLine("Depot inférieur à zero.");
}


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
// try {instruction} catch(exceptionName ex) {treat the bug} finally {is execute in any case}

public class Person(string firstName, string lastName, DateTime birthDate) 
{
    public string FirstName {get; private set;} = firstName;
    public string LastName {get; private set;} = lastName;
    public DateTime BirthDate {get; private set;} = birthDate;

    public override string ToString()
    {
        return $"{FirstName} {LastName}";
    }

    // public Person() {} // default constructor with setter method

    // public Person(string firstName, string lastName, DateTime birthDate) // usual constructor
    // {
    //     this.FirstName = firstName;
    //     this.LastName = lastName;
    //     this.BirthDate = birthDate;
    // }
}

public interface IAccount
{
    public double Balance {get;}
    public void Deposit(double amount);
    public void Withdraw(double amount);
}

public interface IBankAccount : IAccount
{
    public void ApplyInterest();
    public string Number {get;}
    public Person Owner {get;}
}

public class InsufficientBalanceException : Exception
{

    public InsufficientBalanceException(string message): base(message)
    {
        Console.WriteLine(message);
    }
}

public delegate void NegativeBalanceDelegate(object sender, EventArgs e);

public abstract class Account(string number, Person owner) : IBankAccount
{
    public string Number {get; private set;} = number;
    public double Balance {get;set;} // TODO functionnal event with set reaction.
    // {
    //     get
    //     { 
    //         return Balance;
    //     }
    //     protected set
    //     {
    //         Balance = value;
    //         // if (value < 0)
    //         // {
    //         //     NegativeBalanceEventSend();
    //         // }
    //     }
    // } // = 0;
    public Person Owner {get; private set;} = owner;
    public event NegativeBalanceDelegate? NegativeBalanceEvent;

    protected virtual void NegativeBalanceEventSend() => NegativeBalanceEvent?.Invoke(this,EventArgs.Empty);

    // public Account(string number, Person owner) : this()
    // {
    //     this.Number = number;
    //     this.Owner = owner;
    // }

    public Account(string number, double balance, Person owner) : this(number, owner)
    {
        this.Balance = balance;
    }

    public virtual void Withdraw (double amount)
    {
        this.Balance = amount < this.Balance ? this.Balance - amount : throw new InsufficientBalanceException("The amount is superior to the balance.");
    }

    public virtual void Deposit (double amount)
    {
        this.Balance = amount >= 0 ? this.Balance + amount : throw new ArgumentOutOfRangeException();
    }

    protected abstract double CalculInterest();

    public virtual void ApplyInterest()  => Balance += CalculInterest();
}

public class CurrentAccount : Account
{
    public double CreditLine {get;set;}
    public const double interest_p = 0.03;
    public const double interest_n = 9.75;

    public CurrentAccount(string number, Person owner): base(number, owner) {}

    public CurrentAccount(string number, Person owner, double creditLine) : base(number, owner) 
    {
        this.CreditLine = creditLine >= 0 ? creditLine: throw new ArgumentOutOfRangeException();
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


    public SavingAccount(string number, Person owner) : base(number, owner) {}

    public SavingAccount(string number, Person owner, DateTime dateLastWithdraw) : base(number, owner) 
    {
        this.DateLastWithdraw = dateLastWithdraw;
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
        account.NegativeBalanceEvent += this.NegativeBalanceAction;
        Accounts.Add(number, account);
    }

    public void DeleteAccount(string number) => Accounts.Remove(number);

    public void NegativeBalanceAction(object sender, EventArgs e)
    {
        foreach (KeyValuePair<string, Account> account in Accounts) {
            if (account.Value == sender)
            {
                Console.WriteLine($"the number account {account.Key} is now in negative.");
            }
        }
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