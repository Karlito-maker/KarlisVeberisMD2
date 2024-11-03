// DataManager.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class DataManager : IDataManager
{
    // Datu kolekcijas
    private DataCollection data = new DataCollection();
    private readonly string _filePath;

    // Konstruktors, kas uzstāda faila ceļu
    public DataManager(string filePath)
    {
        _filePath = filePath;
    }

    // Metode, kas atgriež visu datu informāciju kā tekstu
    public string Print()
    {
        return string.Join(Environment.NewLine,
            data.Persons.Select(p => p.ToString()).Concat(
            data.Courses.Select(c => c.ToString())).Concat(
            data.Assignments.Select(a => a.ToString())).Concat(
            data.Submissions.Select(s => s.ToString())));
    }

    // Metode, kas saglabā datus norādītajā failā
    public void Save(string path)
    {
        using (StreamWriter writer = new StreamWriter(path))
        {
            // Saglabā Persons
            foreach (var person in data.Persons)
            {
                if (person is Teacher teacher)
                {
                    writer.WriteLine($"Teacher|{teacher.Name}|{teacher.Surname}|{teacher.Gender}|{teacher.ContractDate}");
                }
                else if (person is Student student)
                {
                    writer.WriteLine($"Student|{student.Name}|{student.Surname}|{student.Gender}|{student.StudentIdNumber}");
                }
            }

            // Saglabā Courses
            foreach (var course in data.Courses)
            {
                writer.WriteLine($"Course|{course.Name}|{course.Teacher.Name}|{course.Teacher.Surname}");
            }

            // Saglabā Assignments
            foreach (var assignment in data.Assignments)
            {
                writer.WriteLine($"Assignment|{assignment.Deadline}|{assignment.Course.Name}|{assignment.Description}");
            }

            // Saglabā Submissions
            foreach (var submission in data.Submissions)
            {
                writer.WriteLine($"Submission|{submission.Assignment.Description}|{submission.Student.Name}|{submission.Student.Surname}|{submission.SubmissionTime}|{submission.Score}");
            }
        }
    }

    // Metode, kas ielādē datus no norādītā faila
    public void Load(string path)
    {
        if (File.Exists(path))
        {
            data = new DataCollection(); // Atiestata datus

            var lines = File.ReadAllLines(path);

            // Dictionāri, lai atjaunotu atsauces
            Dictionary<string, Teacher> teachersDict = new Dictionary<string, Teacher>();
            Dictionary<string, Student> studentsDict = new Dictionary<string, Student>();
            Dictionary<string, Course> coursesDict = new Dictionary<string, Course>();
            Dictionary<string, Assignment> assignmentsDict = new Dictionary<string, Assignment>();

            foreach (var line in lines)
            {
                var tokens = line.Split('|');
                switch (tokens[0])
                {
                    case "Teacher":
                        {
                            var name = tokens[1];
                            var surname = tokens[2];
                            var gender = (Gender)Enum.Parse(typeof(Gender), tokens[3]);
                            var contractDate = DateTime.Parse(tokens[4]);

                            var teacher = new Teacher(name, surname, gender, contractDate);
                            data.Persons.Add(teacher);
                            teachersDict[$"{name}|{surname}"] = teacher;
                        }
                        break;
                    case "Student":
                        {
                            var name = tokens[1];
                            var surname = tokens[2];
                            var gender = (Gender)Enum.Parse(typeof(Gender), tokens[3]);
                            var studentIdNumber = int.Parse(tokens[4]);

                            var student = new Student(name, surname, gender, studentIdNumber);
                            data.Persons.Add(student);
                            studentsDict[$"{name}|{surname}"] = student;
                        }
                        break;
                    case "Course":
                        {
                            var courseName = tokens[1];
                            var teacherName = tokens[2];
                            var teacherSurname = tokens[3];
                            Teacher teacher = null;
                            if (teachersDict.TryGetValue($"{teacherName}|{teacherSurname}", out teacher))
                            {
                                var course = new Course(courseName, teacher);
                                data.Courses.Add(course);
                                coursesDict[courseName] = course;
                            }
                        }
                        break;
                    case "Assignment":
                        {
                            var deadline = DateTime.Parse(tokens[1]);
                            var courseName = tokens[2];
                            var description = tokens[3];
                            Course course = null;
                            if (coursesDict.TryGetValue(courseName, out course))
                            {
                                var assignment = new Assignment(deadline, course, description);
                                data.Assignments.Add(assignment);
                                assignmentsDict[description] = assignment;
                            }
                        }
                        break;
                    case "Submission":
                        {
                            var assignmentDescription = tokens[1];
                            var studentName = tokens[2];
                            var studentSurname = tokens[3];
                            var submissionTime = DateTime.Parse(tokens[4]);
                            var score = int.Parse(tokens[5]);
                            Assignment assignment = null;
                            Student student = null;
                            if (assignmentsDict.TryGetValue(assignmentDescription, out assignment) &&
                                studentsDict.TryGetValue($"{studentName}|{studentSurname}", out student))
                            {
                                var submission = new Submission(assignment, student, submissionTime, score);
                                data.Submissions.Add(submission);
                            }
                        }
                        break;
                }
            }
        }
    }

    // Metode, kas izveido testa datus
    public void CreateTestData()
    {
        var teacher = new Teacher("Gatis", "Murnieks", Gender.Man, DateTime.Now.AddYears(-5));
        var student = new Student("Liene", "Liepina", Gender.Woman, 12345);
        var course = new Course("Matemātika", teacher);
        var assignment = new Assignment(DateTime.Now.AddDays(7), course, "Mājasdarbs 1");

        data.Persons.Add(teacher);
        data.Persons.Add(student);
        data.Courses.Add(course);
        data.Assignments.Add(assignment);
    }

    // Metode, kas restartē datus
    public void Reset()
    {
        data = new DataCollection();
    }

    // Metodes, lai pievienotu un noņemtu datus
    public void AddPerson(Person person)
    {
        data.Persons.Add(person);
    }

    public void AddCourse(Course course)
    {
        data.Courses.Add(course);
    }

    public void AddAssignment(Assignment assignment)
    {
        data.Assignments.Add(assignment);
    }

    public void RemoveAssignment(Assignment assignment)
    {
        data.Assignments.Remove(assignment);
    }

    public void AddSubmission(Submission submission)
    {
        data.Submissions.Add(submission);
    }

    public void RemoveSubmission(Submission submission)
    {
        data.Submissions.Remove(submission);
    }

    // Metodes, lai iegūtu datus
    public List<Person> GetPersons()
    {
        return data.Persons;
    }

    public List<Course> GetCourses()
    {
        return data.Courses;
    }

    public List<Assignment> GetAssignments()
    {
        return data.Assignments;
    }

    public List<Submission> GetSubmissions()
    {
        return data.Submissions;
    }
}
