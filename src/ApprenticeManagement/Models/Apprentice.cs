namespace ApprenticeManagement.Models;

/// <summary>Represents an apprentice enrolled in the system.</summary>
public class Apprentice
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string FirstName { get; set; } = string.Empty;
    public string LastName  { get; set; } = string.Empty;

    /// <summary>Full name helper.</summary>
    public string FullName => $"{FirstName} {LastName}";

    public DateTime DateOfBirth  { get; set; }
    public DateTime StartDate    { get; set; } = DateTime.Today;

    /// <summary>Foreign key to the apprentice's company.</summary>
    public Guid? CompanyId { get; set; }

    /// <summary>Foreign key to the apprentice's vocational trainer.</summary>
    public Guid? VocationalTrainerId { get; set; }

    public List<WorkJournal>   WorkJournals   { get; set; } = [];
    public List<SchoolSubject> SchoolSubjects { get; set; } = [];

    // ─── Work-journal helpers ────────────────────────────────────────────────

    /// <summary>Adds a new work-journal entry.</summary>
    public void AddWorkJournal(WorkJournal entry) => WorkJournals.Add(entry);

    /// <summary>Removes a work-journal entry by id.</summary>
    public bool RemoveWorkJournal(Guid id)
    {
        var entry = WorkJournals.FirstOrDefault(j => j.Id == id);
        if (entry is null) return false;
        WorkJournals.Remove(entry);
        return true;
    }

    /// <summary>Returns the total hours logged across all work-journal entries.</summary>
    public double GetTotalHoursWorked() => WorkJournals.Sum(j => j.HoursWorked);

    /// <summary>
    /// Returns work-journal entries whose date falls within [from, to] (inclusive).
    /// </summary>
    public IEnumerable<WorkJournal> GetJournalsByDateRange(DateTime from, DateTime to)
        => WorkJournals.Where(j => j.Date.Date >= from.Date && j.Date.Date <= to.Date)
                       .OrderBy(j => j.Date);

    // ─── School-subject / grade helpers ─────────────────────────────────────

    /// <summary>Adds a new school subject.</summary>
    public void AddSchoolSubject(SchoolSubject subject) => SchoolSubjects.Add(subject);

    /// <summary>Removes a school subject by id.</summary>
    public bool RemoveSchoolSubject(Guid id)
    {
        var subject = SchoolSubjects.FirstOrDefault(s => s.Id == id);
        if (subject is null) return false;
        SchoolSubjects.Remove(subject);
        return true;
    }

    /// <summary>Adds a grade to a specific school subject.</summary>
    public bool AddGrade(Guid subjectId, Grade grade)
    {
        var subject = SchoolSubjects.FirstOrDefault(s => s.Id == subjectId);
        if (subject is null) return false;
        subject.Grades.Add(grade);
        return true;
    }

    /// <summary>Removes a grade from a specific school subject.</summary>
    public bool RemoveGrade(Guid subjectId, Guid gradeId)
    {
        var subject = SchoolSubjects.FirstOrDefault(s => s.Id == subjectId);
        if (subject is null) return false;

        var grade = subject.Grades.FirstOrDefault(g => g.Id == gradeId);
        if (grade is null) return false;

        subject.Grades.Remove(grade);
        return true;
    }

    /// <summary>
    /// Calculates the overall GPA as the unweighted mean of each subject's
    /// weighted average.  Returns <c>null</c> when no graded subjects exist.
    /// </summary>
    public double? GetOverallGpa()
    {
        var subjectAverages = SchoolSubjects
            .Select(s => s.GetWeightedAverage())
            .Where(a => a.HasValue)
            .Select(a => a!.Value)
            .ToList();

        return subjectAverages.Count == 0 ? null : subjectAverages.Average();
    }
}
