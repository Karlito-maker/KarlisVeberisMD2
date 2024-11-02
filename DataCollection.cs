using System.Collections.Generic;

// Klase, kas satur kolekcijas ar dazadiem datiem: Person, Course, Assignment, Submission
public class DataCollection
{
    public List<Person> Persons { get; set; } = new List<Person>();
    public List<Course> Courses { get; set; } = new List<Course>();
    public List<Assignment> Assignments { get; set; } = new List<Assignment>();
    public List<Submission> Submissions { get; set; } = new List<Submission>();
}
