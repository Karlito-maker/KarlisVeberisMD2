using System;

public abstract class Person
{
    private string _name;
    private string _surname;
    // Publiska ipasiba Name ar validaciju, lai nepielautu tuksas vertibas
    public string Name
    {
        get => _name;
        set
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                _name = value;
            }
        }
    }
    // Tas pats kas ieprieks, tikai ar Surname
    public string Surname
    {
        get => _surname;
        set
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                _surname = value;
            }
        }
    }
    // lasa tikai FullName, kas apvieno Name un Surname
    public string FullName => $"{Name} {Surname}";

    public Gender Gender { get; set; }

    public Person(string name, string surname, Gender gender)
    {
        _name = name;
        _surname = surname;
        Gender = gender;
    }

    public override string ToString()
    {
        return $"Name: {Name}, Surname: {Surname}, FullName: {FullName}, Gender: {Gender}";
    }
}
// Parskatamais tips Gender ar vērtībām Man un Woman
public enum Gender
{
    Man,
    Woman
}

