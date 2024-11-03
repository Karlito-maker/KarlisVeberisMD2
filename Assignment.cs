using System;
using System.ComponentModel;

public class Assignment : INotifyPropertyChanged
{
    private DateTime _deadline;
    private Course _course;
    private string _description;

    public event PropertyChangedEventHandler PropertyChanged;

    public DateTime Deadline
    {
        get => _deadline;
        set
        {
            if (_deadline != value)
            {
                _deadline = value;
                OnPropertyChanged(nameof(Deadline));
            }
        }
    }

    public Course Course
    {
        get => _course;
        set
        {
            if (_course != value)
            {
                _course = value;
                OnPropertyChanged(nameof(Course));
            }
        }
    }

    public string Description
    {
        get => _description;
        set
        {
            if (_description != value)
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }
    }

    public Assignment(DateTime deadline, Course course, string description)
    {
        Deadline = deadline;
        Course = course;
        Description = description;
    }

    public override string ToString()
    {
        return $"Assignment: {Description}, Deadline: {Deadline}, Course: {Course.Name}";
    }

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
