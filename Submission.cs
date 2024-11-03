using System;
using System.ComponentModel;

public class Submission : INotifyPropertyChanged
{
    private Assignment _assignment;
    private Student _student;
    private DateTime _submissionTime;
    private int _score;

    public event PropertyChangedEventHandler PropertyChanged;

    public Assignment Assignment
    {
        get => _assignment;
        set
        {
            if (_assignment != value)
            {
                _assignment = value;
                OnPropertyChanged(nameof(Assignment));
            }
        }
    }

    public Student Student
    {
        get => _student;
        set
        {
            if (_student != value)
            {
                _student = value;
                OnPropertyChanged(nameof(Student));
            }
        }
    }

    public DateTime SubmissionTime
    {
        get => _submissionTime;
        set
        {
            if (_submissionTime != value)
            {
                _submissionTime = value;
                OnPropertyChanged(nameof(SubmissionTime));
            }
        }
    }

    public int Score
    {
        get => _score;
        set
        {
            if (_score != value)
            {
                _score = value;
                OnPropertyChanged(nameof(Score));
            }
        }
    }

    public Submission(Assignment assignment, Student student, DateTime submissionTime, int score)
    {
        Assignment = assignment;
        Student = student;
        SubmissionTime = submissionTime;
        Score = score;
    }

    public override string ToString()
    {
        return $"Submission: {Assignment.Description}, Student: {Student.FullName}, Time: {SubmissionTime}, Score: {Score}";
    }

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
