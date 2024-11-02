public class Course
{
    //Kursa nosaukums un  skolotajs kas maca to
    public string Name { get; set; }
    public Teacher Teacher { get; set; }
    //Konstruktors, kas uzstada ipasibas
    public Course(string name, Teacher teacher)
    {
        Name = name;
        Teacher = teacher;
    }
    // pārdefinē ToString(), lai atgrieztu formatētu tekstu
    public override string ToString()
    {
        return $"Course Name: {Name}, Teacher: {Teacher.FullName}";
    }
}


