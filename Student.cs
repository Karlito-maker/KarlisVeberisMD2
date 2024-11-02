public class Student : Person
{
    public int StudentIdNumber { get; set; }
    // Konstruktors, kas uzstada visas ipasibas
    public Student(string name, string surname, Gender gender, int studentIdNumber)
        : base(name, surname, gender)
    {
        StudentIdNumber = studentIdNumber;
    }
    // Konstruktors ar tikai studentu ID numuru un noklusetam vertibam
    public Student(int studentIdNumber) : base("", "", Gender.Man) // Tukšs konstruktors ar default vērtībām
    {
        StudentIdNumber = studentIdNumber;
    }
    // Atgriez studenta informaciju ka tekstu
    public override string ToString()
    {
        return base.ToString() + $", StudentIdNumber: {StudentIdNumber}";
    }
}

