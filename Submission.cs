public class Submission
{
    public Assignment Assignment { get; set; }
    public Student Student { get; set; }
    public DateTime SubmissionTime { get; set; }
    public int Score { get; set; }
    // Konstruktors, kas uzstada ipasibas
    public Submission(Assignment assignment, Student student, DateTime submissionTime, int score)
    {
        Assignment = assignment;
        Student = student;
        SubmissionTime = submissionTime;
        Score = score;
    }
    //Atgriez teksta forma
    public override string ToString()
    {
        return $"Submission: {Assignment.Description}, Student: {Student.FullName}, Time: {SubmissionTime}, Score: {Score}";
    }
}
