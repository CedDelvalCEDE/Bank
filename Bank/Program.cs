using System.Globalization;
using System.Text;

DateTime cede_birth = new(year:2000,month:12,day:30);


var peopleC = new Person() { FirstName = "Ced", LastName = "Delval", BirthDate = cede_birth}; // method with setter
// Person PeopleB = new("Ced", "Delval", cede_birth); // method with constructor

public class Person 
{
    public string FirstName {get;set;}
    public string LastName {get;set;}
    public DateTime BirthDate {get;set;}

    // access type name { access get; access set;}
    
    // public Person(string firstName, string lastName, DateTime birthDate) // usual constructor
    // {
    //     this.FirstName = firstName;
    //     this.LastName = lastName;
    //     this.BirthDate = birthDate;
    // }
    // public Person() {} // strat for counter constructor with setter method
}

public class CurrentAccount 
{
    public string Number {get;set;}
    public double Balance {get;}
    public double CreditLine {get;set;}
    public Person Owner {get;set;}

    public void Withdraw (double amount)
    {
        CreditLine = CreditLine - amount;
    }
    public void Deposit (double amount)
    {
        CreditLine = CreditLine + amount;
    }
}

/*
3. Créer une classe « Bank » pour gérer les comptes de la banque implémentant :
• Les propriétés
• Dictionary<string, Current> Accounts (lecture seule)
• string Name
• Les méthodes :
• void AddAccount(Current account)
• void DeleteAccount(string number)
*/