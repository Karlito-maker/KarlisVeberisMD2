using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System.ComponentModel;

namespace KarlisVeberisMD2
{
    public partial class MainPage : ContentPage, INotifyPropertyChanged
    {
        // Instance of DataManager for managing data operations
        private DataManager dm;
        private readonly string filePath = "C:\\Temp\\data.txt";

        // ObservableCollections for data binding
        public ObservableCollection<Teacher> Teachers { get; set; } = new ObservableCollection<Teacher>();
        public ObservableCollection<Student> Students { get; set; } = new ObservableCollection<Student>();
        public ObservableCollection<Course> Courses { get; set; } = new ObservableCollection<Course>();
        public ObservableCollection<Assignment> Assignments { get; set; } = new ObservableCollection<Assignment>();
        public ObservableCollection<Submission> Submissions { get; set; } = new ObservableCollection<Submission>();

        // Selected items for editing and deleting
        private Assignment? selectedAssignment;
        private Submission? selectedSubmission;

        // Implementing PropertyChanged event for INotifyPropertyChanged
        public new event PropertyChangedEventHandler? PropertyChanged;

        protected override void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Assignment? SelectedAssignment
        {
            get => selectedAssignment;
            set
            {
                selectedAssignment = value;
                OnPropertyChanged(nameof(SelectedAssignment));
                EditAssignmentButton.IsEnabled = selectedAssignment != null;
                DeleteAssignmentButton.IsEnabled = selectedAssignment != null;
            }
        }

        public Submission? SelectedSubmission
        {
            get => selectedSubmission;
            set
            {
                selectedSubmission = value;
                OnPropertyChanged(nameof(SelectedSubmission));
                EditSubmissionButton.IsEnabled = selectedSubmission != null;
                DeleteSubmissionButton.IsEnabled = selectedSubmission != null;
            }
        }

        public MainPage()
        {
            InitializeComponent();
            dm = new DataManager(filePath);
            BindingContext = this;
        }

        // Method to update ObservableCollections with data from DataManager
        private void UpdateCollections()
        {
            Teachers.Clear();
            Students.Clear();
            Courses.Clear();
            Assignments.Clear();
            Submissions.Clear();

            foreach (var person in dm.GetPersons())
            {
                if (person is Teacher teacher) Teachers.Add(teacher);
                else if (person is Student student) Students.Add(student);
            }

            foreach (var course in dm.GetCourses()) Courses.Add(course);
            foreach (var assignment in dm.GetAssignments()) Assignments.Add(assignment);
            foreach (var submission in dm.GetSubmissions()) Submissions.Add(submission);
        }

        // Create test data and update collections
        private void OnCreateTestDataClicked(object sender, EventArgs e)
        {
            try
            {
                dm.CreateTestData();
                UpdateCollections();
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", $"Error creating test data: {ex.Message}", "OK");
            }
        }

        // Load data from file and update collections
        private void OnLoadDataClicked(object sender, EventArgs e)
        {
            try
            {
                dm.Load(filePath);
                UpdateCollections();
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", $"Error loading data: {ex.Message}", "OK");
            }
        }

        // Save current data to file
        private void OnSaveDataClicked(object sender, EventArgs e)
        {
            try
            {
                dm.Save(filePath);
                DisplayAlert("Data Saved", "Data has been saved to the file.", "OK");
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", $"Error saving data: {ex.Message}", "OK");
            }
        }

        // Add a new student
        private async void OnAddStudentClicked(object sender, EventArgs e)
        {
            try
            {
                var name = await DisplayPromptAsync("Student's Name", "Enter first name:");
                var surname = await DisplayPromptAsync("Student's Surname", "Enter surname:");
                var genderStr = await DisplayActionSheet("Select Gender", "Cancel", null, "Man", "Woman");
                var studentIdStr = await DisplayPromptAsync("Student ID", "Enter ID number:");

                if (int.TryParse(studentIdStr, out int studentIdNumber))
                {
                    var gender = genderStr == "Man" ? Gender.Man : Gender.Woman;
                    var student = new Student(name ?? "", surname ?? "", gender, studentIdNumber);
                    dm.AddPerson(student);
                    Students.Add(student);
                }
                else
                {
                    await DisplayAlert("Invalid ID", "Please enter a valid student ID.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error adding student: {ex.Message}", "OK");
            }
        }

        // Add a new assignment
        private async void OnAddAssignmentClicked(object sender, EventArgs e)
        {
            try
            {
                if (Courses.Count > 0)
                {
                    var course = await SelectCourse();
                    if (course != null)
                    {
                        var description = await DisplayPromptAsync("Assignment Description", "Enter description:");
                        var deadlineStr = await DisplayPromptAsync("Assignment Deadline", "Enter deadline (yyyy-mm-dd):");
                        if (DateTime.TryParse(deadlineStr, out DateTime deadline))
                        {
                            var assignment = new Assignment(deadline, course, description ?? "Default Description");
                            dm.AddAssignment(assignment);
                            Assignments.Add(assignment);
                        }
                        else
                        {
                            await DisplayAlert("Invalid Date", "Please enter a valid date.", "OK");
                        }
                    }
                }
                else
                {
                    await DisplayAlert("No Courses", "Please add a course before adding an assignment.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error adding assignment: {ex.Message}", "OK");
            }
        }

        // Helper method to select a course
        private async Task<Course?> SelectCourse()
        {
            var courseNames = Courses.Select(c => c.Name).ToArray();
            var selectedName = await DisplayActionSheet("Select Course", "Cancel", null, courseNames);
            return selectedName == "Cancel" || string.IsNullOrEmpty(selectedName) ? null : Courses.FirstOrDefault(c => c.Name == selectedName);
        }

        // Edit an assignment
        private async void OnEditAssignmentClicked(object sender, EventArgs e)
        {
            if (SelectedAssignment != null)
            {
                var newDescription = await DisplayPromptAsync("Edit Assignment Description", "Enter new description:", initialValue: SelectedAssignment.Description);
                var newDeadlineStr = await DisplayPromptAsync("Edit Assignment Deadline", "Enter new deadline (yyyy-mm-dd):", initialValue: SelectedAssignment.Deadline.ToString("yyyy-MM-dd"));

                if (DateTime.TryParse(newDeadlineStr, out DateTime newDeadline) && !string.IsNullOrEmpty(newDescription))
                {
                    dm.UpdateAssignment(SelectedAssignment, newDeadline, newDescription);
                    UpdateCollections();
                }
                else
                {
                    await DisplayAlert("Invalid Data", "Please enter valid data for the assignment.", "OK");
                }
            }
        }

        // Delete an assignment
        private async void OnDeleteAssignmentClicked(object sender, EventArgs e)
        {
            if (SelectedAssignment != null)
            {
                bool confirm = await DisplayAlert("Confirm Delete", "Are you sure you want to delete this assignment?", "Yes", "No");
                if (confirm)
                {
                    dm.DeleteAssignment(SelectedAssignment);
                    Assignments.Remove(SelectedAssignment);
                    SelectedAssignment = null;
                }
            }
        }

        // Add a new submission
        private async void OnAddSubmissionClicked(object sender, EventArgs e)
        {
            try
            {
                if (Assignments.Count > 0 && Students.Count > 0)
                {
                    var assignment = await SelectAssignment();
                    var student = await SelectStudent();

                    if (assignment != null && student != null)
                    {
                        var submissionTimeStr = await DisplayPromptAsync("Submission Time", "Enter time (yyyy-mm-dd hh:mm):");
                        if (DateTime.TryParse(submissionTimeStr, out DateTime submissionTime))
                        {
                            var scoreStr = await DisplayPromptAsync("Score", "Enter score:");
                            if (int.TryParse(scoreStr, out int score))
                            {
                                var submission = new Submission(assignment, student, submissionTime, score);
                                dm.AddSubmission(submission);
                                Submissions.Add(submission);
                            }
                            else
                            {
                                await DisplayAlert("Invalid Score", "Please enter a valid score.", "OK");
                            }
                        }
                        else
                        {
                            await DisplayAlert("Invalid Date", "Please enter a valid date and time.", "OK");
                        }
                    }
                }
                else
                {
                    await DisplayAlert("Missing Data", "Please add assignments and students before adding a submission.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error adding submission: {ex.Message}", "OK");
            }
        }

        // Helper method to select an assignment
        private async Task<Assignment?> SelectAssignment()
        {
            var assignmentDescriptions = Assignments.Select(a => a.Description).ToArray();
            var selectedDescription = await DisplayActionSheet("Select Assignment", "Cancel", null, assignmentDescriptions);
            return selectedDescription == "Cancel" || string.IsNullOrEmpty(selectedDescription) ? null : Assignments.FirstOrDefault(a => a.Description == selectedDescription);
        }

        // Helper method to select a student
        private async Task<Student?> SelectStudent()
        {
            var studentNames = Students.Select(s => s.FullName).ToArray();
            var selectedName = await DisplayActionSheet("Select Student", "Cancel", null, studentNames);
            return selectedName == "Cancel" || string.IsNullOrEmpty(selectedName) ? null : Students.FirstOrDefault(s => s.FullName == selectedName);
        }

        // Edit a submission
        private async void OnEditSubmissionClicked(object sender, EventArgs e)
        {
            if (SelectedSubmission != null)
            {
                var newSubmissionTimeStr = await DisplayPromptAsync("Edit Submission Time", "Enter new time (yyyy-mm-dd hh:mm):", initialValue: SelectedSubmission.SubmissionTime.ToString("yyyy-MM-dd HH:mm"));
                var newScoreStr = await DisplayPromptAsync("Edit Score", "Enter new score:", initialValue: SelectedSubmission.Score.ToString());

                if (DateTime.TryParse(newSubmissionTimeStr, out DateTime newSubmissionTime) && int.TryParse(newScoreStr, out int newScore))
                {
                    dm.UpdateSubmission(SelectedSubmission, newSubmissionTime, newScore);
                    UpdateCollections();
                }
                else
                {
                    await DisplayAlert("Invalid Data", "Please enter valid data for the submission.", "OK");
                }
            }
        }

        // Delete a submission
        private async void OnDeleteSubmissionClicked(object sender, EventArgs e)
        {
            if (SelectedSubmission != null)
            {
                bool confirm = await DisplayAlert("Confirm Delete", "Are you sure you want to delete this submission?", "Yes", "No");
                if (confirm)
                {
                    dm.DeleteSubmission(SelectedSubmission);
                    Submissions.Remove(SelectedSubmission);
                    SelectedSubmission = null;
                }
            }
        }
    }
}
