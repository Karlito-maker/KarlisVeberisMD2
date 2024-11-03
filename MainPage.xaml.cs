// MainPage.xaml.cs
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
        // DataManager instances datu pārvaldībai
        private DataManager dm;
        // Faila ceļš datu saglabāšanai un ielādei
        private string filePath = "C:\\Temp\\data.txt";

        // ObservableCollections, lai piesaistītu datus ListView
        public ObservableCollection<Teacher> Teachers { get; set; } = new ObservableCollection<Teacher>();
        public ObservableCollection<Student> Students { get; set; } = new ObservableCollection<Student>();
        public ObservableCollection<Course> Courses { get; set; } = new ObservableCollection<Course>();
        public ObservableCollection<Assignment> Assignments { get; set; } = new ObservableCollection<Assignment>();
        public ObservableCollection<Submission> Submissions { get; set; } = new ObservableCollection<Submission>();

        // Izvēlētie objekti labojumiem un dzēšanai
        private Assignment selectedAssignment;
        private Submission selectedSubmission;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Assignment SelectedAssignment
        {
            get => selectedAssignment;
            set
            {
                selectedAssignment = value;
                OnPropertyChanged(nameof(SelectedAssignment));
                // Iespējo vai atspējo pogas atkarībā no izvēles
                EditAssignmentButton.IsEnabled = selectedAssignment != null;
                DeleteAssignmentButton.IsEnabled = selectedAssignment != null;
            }
        }

        public Submission SelectedSubmission
        {
            get => selectedSubmission;
            set
            {
                selectedSubmission = value;
                OnPropertyChanged(nameof(SelectedSubmission));
                // Iespējo vai atspējo pogas atkarībā no izvēles
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

        // Notikums pogai "Izveidot testa datus"
        private void OnCreateTestDataClicked(object sender, EventArgs e)
        {
            try
            {
                dm.CreateTestData();
                UpdateCollections();
            }
            catch (Exception ex)
            {
                DisplayAlert("Kļūda", $"Kļūda izveidojot testa datus: {ex.Message}", "OK");
            }
        }

        // Notikums pogai "Ielādēt datus no faila"
        private void OnLoadDataClicked(object sender, EventArgs e)
        {
            try
            {
                dm.Load(filePath);
                UpdateCollections();
            }
            catch (Exception ex)
            {
                DisplayAlert("Kļūda", $"Kļūda ielādējot datus: {ex.Message}", "OK");
            }
        }

        // Notikums pogai "Saglabāt datus failā"
        private void OnSaveDataClicked(object sender, EventArgs e)
        {
            try
            {
                dm.Save(filePath);
                DisplayAlert("Dati saglabāti", "Dati ir saglabāti failā.", "OK");
            }
            catch (Exception ex)
            {
                DisplayAlert("Kļūda", $"Kļūda saglabājot datus: {ex.Message}", "OK");
            }
        }

        // Metode, lai atjauninātu kolekcijas no DataManager datiem
        private void UpdateCollections()
        {
            Teachers.Clear();
            Students.Clear();
            Courses.Clear();
            Assignments.Clear();
            Submissions.Clear();

            // Pievieno skolotājus un studentus attiecīgajās kolekcijās
            foreach (var person in dm.GetPersons())
            {
                if (person is Teacher teacher)
                {
                    Teachers.Add(teacher);
                }
                else if (person is Student student)
                {
                    Students.Add(student);
                }
            }

            // Pievieno kursus, uzdevumus un iesniegumus
            foreach (var course in dm.GetCourses())
            {
                Courses.Add(course);
            }

            foreach (var assignment in dm.GetAssignments())
            {
                Assignments.Add(assignment);
            }

            foreach (var submission in dm.GetSubmissions())
            {
                Submissions.Add(submission);
            }
        }

        // Notikums pogai "Pievienot studentu"
        private async void OnAddStudentClicked(object sender, EventArgs e)
        {
            try
            {
                // Ļauj lietotājam ievadīt studenta datus
                var name = await DisplayPromptAsync("Studenta vārds", "Ievadiet vārdu:");
                var surname = await DisplayPromptAsync("Studenta uzvārds", "Ievadiet uzvārdu:");
                var genderStr = await DisplayActionSheet("Izvēlieties dzimumu", "Atcelt", null, "Vīrietis", "Sieviete");
                var studentIdStr = await DisplayPromptAsync("Studenta ID numurs", "Ievadiet studenta ID numuru:");

                if (int.TryParse(studentIdStr, out int studentIdNumber))
                {
                    var gender = genderStr == "Vīrietis" ? Gender.Man : Gender.Woman;
                    var student = new Student(name, surname, gender, studentIdNumber);
                    dm.AddPerson(student);
                    Students.Add(student);
                }
                else
                {
                    await DisplayAlert("Nederīgs ID", "Lūdzu, ievadiet derīgu studenta ID numuru.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Kļūda", $"Kļūda pievienojot studentu: {ex.Message}", "OK");
            }
        }

        // Notikums pogai "Pievienot uzdevumu"
        private async void OnAddAssignmentClicked(object sender, EventArgs e)
        {
            try
            {
                // Pārbauda, vai ir pieejami kursi
                if (Courses.Count > 0)
                {
                    // Ļauj lietotājam izvēlēties kursu
                    var course = await SelectCourse();
                    if (course != null)
                    {
                        // Ļauj ievadīt uzdevuma detaļas
                        var description = await DisplayPromptAsync("Uzdevuma apraksts", "Ievadiet aprakstu:");
                        var deadlineStr = await DisplayPromptAsync("Uzdevuma termiņš", "Ievadiet termiņu (gggg-mm-dd):");
                        if (DateTime.TryParse(deadlineStr, out DateTime deadline))
                        {
                            var assignment = new Assignment(deadline, course, description);
                            dm.AddAssignment(assignment);
                            Assignments.Add(assignment);
                        }
                        else
                        {
                            await DisplayAlert("Nederīgs datums", "Lūdzu, ievadiet derīgu datumu.", "OK");
                        }
                    }
                }
                else
                {
                    await DisplayAlert("Nav kursu", "Lūdzu, pievienojiet kursu pirms uzdevuma pievienošanas.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Kļūda", $"Kļūda pievienojot uzdevumu: {ex.Message}", "OK");
            }
        }

        // Palīgmetode kursa izvēlei
        private async Task<Course> SelectCourse()
        {
            var courseNames = Courses.Select(c => c.Name).ToArray();
            var selectedName = await DisplayActionSheet("Izvēlieties kursu", "Atcelt", null, courseNames);
            if (selectedName == "Atcelt" || string.IsNullOrEmpty(selectedName))
                return null;

            return Courses.FirstOrDefault(c => c.Name == selectedName);
        }

        // Notikums pogai "Labot uzdevumu"
        private async void OnEditAssignmentClicked(object sender, EventArgs e)
        {
            if (SelectedAssignment != null)
            {
                try
                {
                    // Ļauj labot uzdevuma detaļas
                    var description = await DisplayPromptAsync("Labot aprakstu", "Ievadiet jaunu aprakstu:", initialValue: SelectedAssignment.Description);
                    var deadlineStr = await DisplayPromptAsync("Labot termiņu", "Ievadiet jaunu termiņu (gggg-mm-dd):", initialValue: SelectedAssignment.Deadline.ToString("yyyy-MM-dd"));
                    if (DateTime.TryParse(deadlineStr, out DateTime deadline))
                    {
                        SelectedAssignment.Description = description;
                        SelectedAssignment.Deadline = deadline;
                    }
                    else
                    {
                        await DisplayAlert("Nederīgs datums", "Lūdzu, ievadiet derīgu datumu.", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Kļūda", $"Kļūda labojot uzdevumu: {ex.Message}", "OK");
                }
            }
        }

        // Notikums pogai "Dzēst uzdevumu"
        private async void OnDeleteAssignmentClicked(object sender, EventArgs e)
        {
            if (SelectedAssignment != null)
            {
                try
                {
                    bool confirm = await DisplayAlert("Apstiprināt", "Vai tiešām vēlaties dzēst šo uzdevumu?", "Jā", "Nē");
                    if (confirm)
                    {
                        dm.RemoveAssignment(SelectedAssignment);
                        Assignments.Remove(SelectedAssignment);
                        SelectedAssignment = null;
                        // Atspējo pogas
                        EditAssignmentButton.IsEnabled = false;
                        DeleteAssignmentButton.IsEnabled = false;
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Kļūda", $"Kļūda dzēšot uzdevumu: {ex.Message}", "OK");
                }
            }
        }

        // Notikums pogai "Pievienot iesniegumu"
        private async void OnAddSubmissionClicked(object sender, EventArgs e)
        {
            try
            {
                // Pārbauda, vai ir uzdevumi un studenti
                if (Assignments.Count > 0 && Students.Count > 0)
                {
                    // Ļauj izvēlēties uzdevumu un studentu
                    var assignment = await SelectAssignment();
                    var student = await SelectStudent();

                    if (assignment != null && student != null)
                    {
                        var submissionTimeStr = await DisplayPromptAsync("Iesniegšanas laiks", "Ievadiet iesniegšanas laiku (gggg-mm-dd hh:mm):");
                        if (DateTime.TryParse(submissionTimeStr, out DateTime submissionTime))
                        {
                            var scoreStr = await DisplayPromptAsync("Punkti", "Ievadiet punktu skaitu:");
                            if (int.TryParse(scoreStr, out int score))
                            {
                                var submission = new Submission(assignment, student, submissionTime, score);
                                dm.AddSubmission(submission);
                                Submissions.Add(submission);
                            }
                            else
                            {
                                await DisplayAlert("Nederīgs punktu skaits", "Lūdzu, ievadiet derīgu punktu skaitu.", "OK");
                            }
                        }
                        else
                        {
                            await DisplayAlert("Nederīgs datums", "Lūdzu, ievadiet derīgu datumu un laiku.", "OK");
                        }
                    }
                }
                else
                {
                    await DisplayAlert("Trūkst datu", "Lūdzu, pievienojiet uzdevumus un studentus pirms iesnieguma pievienošanas.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Kļūda", $"Kļūda pievienojot iesniegumu: {ex.Message}", "OK");
            }
        }

        // Palīgmetode uzdevuma izvēlei
        private async Task<Assignment> SelectAssignment()
        {
            var assignmentDescriptions = Assignments.Select(a => a.Description).ToArray();
            var selectedDescription = await DisplayActionSheet("Izvēlieties uzdevumu", "Atcelt", null, assignmentDescriptions);
            if (selectedDescription == "Atcelt" || string.IsNullOrEmpty(selectedDescription))
                return null;

            return Assignments.FirstOrDefault(a => a.Description == selectedDescription);
        }

        // Palīgmetode studenta izvēlei
        private async Task<Student> SelectStudent()
        {
            var studentNames = Students.Select(s => s.FullName).ToArray();
            var selectedName = await DisplayActionSheet("Izvēlieties studentu", "Atcelt", null, studentNames);
            if (selectedName == "Atcelt" || string.IsNullOrEmpty(selectedName))
                return null;

            return Students.FirstOrDefault(s => s.FullName == selectedName);
        }

        // Notikums pogai "Labot iesniegumu"
        private async void OnEditSubmissionClicked(object sender, EventArgs e)
        {
            if (SelectedSubmission != null)
            {
                try
                {
                    // Ļauj labot iesnieguma detaļas
                    var submissionTimeStr = await DisplayPromptAsync("Labot iesniegšanas laiku", "Ievadiet jaunu iesniegšanas laiku (gggg-mm-dd hh:mm):", initialValue: SelectedSubmission.SubmissionTime.ToString("yyyy-MM-dd HH:mm"));
                    if (DateTime.TryParse(submissionTimeStr, out DateTime submissionTime))
                    {
                        var scoreStr = await DisplayPromptAsync("Labot punktus", "Ievadiet jaunu punktu skaitu:", initialValue: SelectedSubmission.Score.ToString());
                        if (int.TryParse(scoreStr, out int score))
                        {
                            SelectedSubmission.SubmissionTime = submissionTime;
                            SelectedSubmission.Score = score;
                        }
                        else
                        {
                            await DisplayAlert("Nederīgs punktu skaits", "Lūdzu, ievadiet derīgu punktu skaitu.", "OK");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Nederīgs datums", "Lūdzu, ievadiet derīgu datumu un laiku.", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Kļūda", $"Kļūda labojot iesniegumu: {ex.Message}", "OK");
                }
            }
        }

        // Notikums pogai "Dzēst iesniegumu"
        private async void OnDeleteSubmissionClicked(object sender, EventArgs e)
        {
            if (SelectedSubmission != null)
            {
                try
                {
                    bool confirm = await DisplayAlert("Apstiprināt", "Vai tiešām vēlaties dzēst šo iesniegumu?", "Jā", "Nē");
                    if (confirm)
                    {
                        dm.RemoveSubmission(SelectedSubmission);
                        Submissions.Remove(SelectedSubmission);
                        SelectedSubmission = null;
                        // Atspējo pogas
                        EditSubmissionButton.IsEnabled = false;
                        DeleteSubmissionButton.IsEnabled = false;
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Kļūda", $"Kļūda dzēšot iesniegumu: {ex.Message}", "OK");
                }
            }
        }
    }
}
