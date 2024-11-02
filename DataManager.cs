using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class DataManager : IDataManager
{
    private DataCollection data = new DataCollection();


    // Atgriež visu datu tekstu sarakstā.

    public List<string> GetAllDataAsStrings()
    {
        var allData = new List<string>();

        // Pievieno personas
        allData.AddRange(data.Persons.Select(p => p.ToString()));

        // Pievieno kursus
        allData.AddRange(data.Courses.Select(c => c.ToString()));

        // Pievieno uzdevumus
        allData.AddRange(data.Assignments.Select(a => a.ToString()));

        // Pievieno iesniegumus
        allData.AddRange(data.Submissions.Select(s => s.ToString()));

        return allData;
    }


    // Pievieno personu datu kolekcijai.

    public void AddPerson(Person person)
    {
        data.Persons.Add(person);
    }


    // Pievieno kursu datu kolekcijai.

    public void AddCourse(Course course)
    {
        data.Courses.Add(course);
    }


    // Pievieno uzdevumu datu kolekcijai.

    public void AddAssignment(Assignment assignment)
    {
        data.Assignments.Add(assignment);
    }


    // Pievieno iesniegumu datu kolekcijai.

    public void AddSubmission(Submission submission)
    {
        data.Submissions.Add(submission);
    }


    // Dzēš, salīdzinot tekstu.

    public void DeleteItemByString(string itemString)
    {
        // Mēģina atrast un dzēst no katras kolekcijas

        var person = data.Persons.FirstOrDefault(p => p.ToString() == itemString);
        if (person != null)
        {
            data.Persons.Remove(person);
            return;
        }

        var course = data.Courses.FirstOrDefault(c => c.ToString() == itemString);
        if (course != null)
        {
            data.Courses.Remove(course);
            return;
        }

        var assignment = data.Assignments.FirstOrDefault(a => a.ToString() == itemString);
        if (assignment != null)
        {
            data.Assignments.Remove(assignment);
            return;
        }

        var submission = data.Submissions.FirstOrDefault(s => s.ToString() == itemString);
        if (submission != null)
        {
            data.Submissions.Remove(submission);
            return;
        }
    }


    // panem Print metodi no IDataManager interfeisa.

    public string Print()
    {
        return string.Join(Environment.NewLine, GetAllDataAsStrings());
    }


    // Saglabā datus failā JSON formātā.

    public void Save(string path)
    {
        //var jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
        //File.WriteAllText(path, jsonData);
    }


    // Ielādē datus no faila JSON formātā.

    public void Load(string path)
    {
        if (File.Exists(path))
        {
            //var jsonData = File.ReadAllText(path);
            //data = JsonConvert.DeserializeObject<DataCollection>(jsonData) ?? new DataCollection();
        }
    }


    // Izveido testa datus.

    public void CreateTestData()
    {
        // Izveido skolotāju
        var teacher = new Teacher("Gatis", "Mūrnieks", Gender.Man, DateTime.Now.AddYears(-5));
        AddPerson(teacher);

        // Izveido studentu
        var student = new Student("Liene", "Liepiņa", Gender.Woman, 12345);
        AddPerson(student);

        // Izveido kursu
        var course = new Course("Matemātika", teacher);
        AddCourse(course);

        // Izveido uzdevumu
        var assignment = new Assignment(DateTime.Now.AddDays(7), course, "Mājasdarbs 1");
        AddAssignment(assignment);

        // Izveido iesniegumu
        var submission = new Submission(assignment, student, DateTime.Now, 95);
        AddSubmission(submission);
    }


    // Atiestata datus.

    public void Reset()
    {
        data = new DataCollection();
    }

    // Papildu metodes, lai iegūtu sarakstus
    // Iegūst studentu sarakstu.

    public List<Student> GetStudents()
    {
        return data.Persons.OfType<Student>().ToList();
    }


    // Iegūst skolotāju sarakstu.

    public List<Teacher> GetTeachers()
    {
        return data.Persons.OfType<Teacher>().ToList();
    }


    // Iegūst kursu sarakstu.

    public List<Course> GetCourses()
    {
        return data.Courses;
    }


    // Iegūst uzdevumu sarakstu.

    public List<Assignment> GetAssignments()
    {
        return data.Assignments;
    }


    // Iegūst iesniegumu sarakstu.

    public List<Submission> GetSubmissions()
    {
        return data.Submissions;
    }
}

