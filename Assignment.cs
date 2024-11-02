public class Assignment
{
    // Uzdevuma termins
    public DateTime Deadline { get; set; }
    //Kurs, kura ir jaizpilda uzd
    public Course Course { get; set; }
    //uzdevuma apraksts
    public string Description { get; set; }
    //Konstruktors, kas uzstada ipasibas
    public Assignment(DateTime deadline, Course course, string description)
    {
        Deadline = deadline;
        Course = course;
        Description = description;
    }
    //Atgriez info ka tekstu
    public override string ToString()
    {
        return $"Assignment: {Description}, Deadline: {Deadline}, Course: {Course.Name}";
    }
}

