using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class DataManager : IDataManager
{
    private DataCollection data = new DataCollection();
    private readonly string _filePath;

    public DataManager(string filePath)
    {
        _filePath = filePath;
    }

    // Method to add a new assignment
    public void AddAssignment(Assignment assignment)
    {
        data.Assignments.Add(assignment);
    }

    // Method to add a new person (either a Teacher or Student)
    public void AddPerson(Person person)
    {
        data.Persons.Add(person);
    }

    // Method to add a new submission
    public void AddSubmission(Submission submission)
    {
        data.Submissions.Add(submission);
    }

    // Method to update an existing assignment
    public void UpdateAssignment(Assignment assignment, DateTime newDeadline, string newDescription)
    {
        assignment.Deadline = newDeadline;
        assignment.Description = newDescription;
    }

    // Method to update an existing submission
    public void UpdateSubmission(Submission submission, DateTime newSubmissionTime, int newScore)
    {
        submission.SubmissionTime = newSubmissionTime;
        submission.Score = newScore;
    }

    // Method to delete an assignment
    public void DeleteAssignment(Assignment assignment)
    {
        data.Assignments.Remove(assignment);
    }

    // Method to delete a submission
    public void DeleteSubmission(Submission submission)
    {
        data.Submissions.Remove(submission);
    }

    // Method to load data from a file
    public void Load(string path)
    {
        if (File.Exists(path))
        {
            data = new DataCollection(); // Reset data

            var lines = File.ReadAllLines(path);

            // Dictionaries to restore references
            Dictionary<string, Teacher> teachersDict = new Dictionary<string, Teacher>();
            Dictionary<string, Student> studentsDict = new Dictionary<string, Student>();
            Dictionary<string, Course> coursesDict = new Dictionary<string, Course>();
            Dictionary<string, Assignment> assignmentsDict = new Dictionary<string, Assignment>();

            foreach (var line in lines)
            {
                var tokens = line.Split('|');
                if (tokens.Length == 0)
                    continue;

                switch (tokens[0])
                {
                    case "Teacher":
                        if (tokens.Length >= 5)
                        {
                            try
                            {
                                var name = tokens[1] ?? "Unknown";
                                var surname = tokens[2] ?? "Unknown";
                                var gender = Enum.TryParse(tokens[3], out Gender parsedGender) ? parsedGender : Gender.Man;
                                var contractDate = DateTime.TryParse(tokens[4], out DateTime parsedDate) ? parsedDate : DateTime.Now;

                                var teacher = new Teacher(name, surname, gender, contractDate);
                                data.Persons.Add(teacher);
                                teachersDict[$"{name}|{surname}"] = teacher;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error reading teacher: {ex.Message}");
                            }
                        }
                        break;

                    case "Student":
                        if (tokens.Length >= 5)
                        {
                            try
                            {
                                var name = tokens[1] ?? "Unknown";
                                var surname = tokens[2] ?? "Unknown";
                                var gender = Enum.TryParse(tokens[3], out Gender parsedGender) ? parsedGender : Gender.Man;
                                var studentIdNumber = int.TryParse(tokens[4], out int id) ? id : 0;

                                var student = new Student(name, surname, gender, studentIdNumber);
                                data.Persons.Add(student);
                                studentsDict[$"{name}|{surname}"] = student;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error reading student: {ex.Message}");
                            }
                        }
                        break;

                    case "Course":
                        if (tokens.Length >= 4)
                        {
                            try
                            {
                                var courseName = tokens[1] ?? "Unknown";
                                var teacherName = tokens[2];
                                var teacherSurname = tokens[3];
                                if (teachersDict.TryGetValue($"{teacherName}|{teacherSurname}", out Teacher? teacher))
                                {
                                    var course = new Course(courseName, teacher);
                                    data.Courses.Add(course);
                                    coursesDict[courseName] = course;
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error reading course: {ex.Message}");
                            }
                        }
                        break;

                    case "Assignment":
                        if (tokens.Length >= 4)
                        {
                            try
                            {
                                var deadline = DateTime.TryParse(tokens[1], out DateTime parsedDate) ? parsedDate : DateTime.Now;
                                var courseName = tokens[2];
                                var description = tokens[3] ?? "No Description";

                                if (coursesDict.TryGetValue(courseName, out Course? course))
                                {
                                    var assignment = new Assignment(deadline, course, description);
                                    data.Assignments.Add(assignment);
                                    assignmentsDict[description] = assignment;
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error reading assignment: {ex.Message}");
                            }
                        }
                        break;

                    case "Submission":
                        if (tokens.Length >= 6)
                        {
                            try
                            {
                                var assignmentDescription = tokens[1];
                                var studentName = tokens[2];
                                var studentSurname = tokens[3];
                                var submissionTime = DateTime.TryParse(tokens[4], out DateTime parsedDate) ? parsedDate : DateTime.Now;
                                var score = int.TryParse(tokens[5], out int parsedScore) ? parsedScore : 0;

                                if (assignmentsDict.TryGetValue(assignmentDescription, out Assignment? assignment) &&
                                    studentsDict.TryGetValue($"{studentName}|{studentSurname}", out Student? student))
                                {
                                    var submission = new Submission(assignment, student, submissionTime, score);
                                    data.Submissions.Add(submission);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error reading submission: {ex.Message}");
                            }
                        }
                        break;
                }
            }
        }
        else
        {
            throw new FileNotFoundException($"File not found: {path}");
        }
    }

    // Method to save data to a file
    public void Save(string path)
    {
        using (StreamWriter writer = new StreamWriter(path))
        {
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

            foreach (var course in data.Courses)
            {
                writer.WriteLine($"Course|{course.Name}|{course.Teacher.Name}|{course.Teacher.Surname}");
            }

            foreach (var assignment in data.Assignments)
            {
                writer.WriteLine($"Assignment|{assignment.Deadline}|{assignment.Course.Name}|{assignment.Description}");
            }

            foreach (var submission in data.Submissions)
            {
                writer.WriteLine($"Submission|{submission.Assignment.Description}|{submission.Student.Name}|{submission.Student.Surname}|{submission.SubmissionTime}|{submission.Score}");
            }
        }
    }

    // Method to create test data
    public void CreateTestData()
    {
        var teacher1 = new Teacher("Gatis", "Murnieks", Gender.Man, DateTime.Now.AddYears(-5));
        var teacher2 = new Teacher("Anna", "Ozola", Gender.Woman, DateTime.Now.AddYears(-3));

        var student1 = new Student("Liene", "Liepina", Gender.Woman, 12345);
        var student2 = new Student("Jānis", "Bērziņš", Gender.Man, 67890);

        var course1 = new Course("Matemātika", teacher1);
        var course2 = new Course("Programmēšana", teacher2);

        var assignment1 = new Assignment(DateTime.Now.AddDays(7), course1, "Mājasdarbs 1");
        var assignment2 = new Assignment(DateTime.Now.AddDays(14), course2, "Projekts 1");

        var submission1 = new Submission(assignment1, student1, DateTime.Now, 90);
        var submission2 = new Submission(assignment2, student2, DateTime.Now, 85);

        data.Persons.Add(teacher1);
        data.Persons.Add(teacher2);
        data.Persons.Add(student1);
        data.Persons.Add(student2);
        data.Courses.Add(course1);
        data.Courses.Add(course2);
        data.Assignments.Add(assignment1);
        data.Assignments.Add(assignment2);
        data.Submissions.Add(submission1);
        data.Submissions.Add(submission2);
    }

    // Method to reset data
    public void Reset()
    {
        data = new DataCollection();
    }

    // Print method for IDataManager interface
    public string Print()
    {
        return string.Join(Environment.NewLine,
            data.Persons.Select(p => p.ToString()).Concat(
            data.Courses.Select(c => c.ToString())).Concat(
            data.Assignments.Select(a => a.ToString())).Concat(
            data.Submissions.Select(s => s.ToString())));
    }

    // Methods to get collections of data
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
