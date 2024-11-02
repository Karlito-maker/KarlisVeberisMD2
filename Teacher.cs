public class Teacher : Person
{
    public DateTime ContractDate { get; set; }
    // Konstruktors, kas uzstada ipasibas
    public Teacher(string name, string surname, Gender gender, DateTime contractDate)
        : base(name, surname, gender)
    {
        ContractDate = contractDate;
    }
    //Atgriez skolotaja info ka tekstu
    public override string ToString()
    {
        return base.ToString() + $", ContractDate: {ContractDate.ToShortDateString()}";
    }
}

