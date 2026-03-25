using ApprenticeManagement.Data;
using ApprenticeManagement.Models;

namespace ApprenticeManagement.Services;

/// <summary>
/// CRUD service for <see cref="Apprentice"/> entities, including their
/// work-journal entries and school-subject grades.
/// </summary>
public class ApprenticeService(DataStore store)
{
    private readonly DataStore _store = store;

    // ─── Apprentice CRUD ─────────────────────────────────────────────────────

    public Apprentice Create(
        string firstName,
        string lastName,
        DateTime dateOfBirth,
        DateTime startDate,
        Guid?    companyId          = null,
        Guid?    vocationalTrainerId = null)
    {
        var apprentice = new Apprentice
        {
            FirstName          = firstName.Trim(),
            LastName           = lastName.Trim(),
            DateOfBirth        = dateOfBirth,
            StartDate          = startDate,
            CompanyId          = companyId,
            VocationalTrainerId = vocationalTrainerId,
        };

        _store.Apprentices.Add(apprentice);
        _store.Save();
        return apprentice;
    }

    public IReadOnlyList<Apprentice> GetAll() => _store.Apprentices.AsReadOnly();

    public Apprentice? GetById(Guid id)
        => _store.Apprentices.FirstOrDefault(a => a.Id == id);

    /// <summary>Finds apprentices whose full name contains <paramref name="searchTerm"/> (case-insensitive).</summary>
    public IEnumerable<Apprentice> SearchByName(string searchTerm)
        => _store.Apprentices
                 .Where(a => a.FullName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));

    public bool Update(
        Guid     id,
        string   firstName,
        string   lastName,
        DateTime dateOfBirth,
        DateTime startDate,
        Guid?    companyId,
        Guid?    vocationalTrainerId)
    {
        var apprentice = GetById(id);
        if (apprentice is null) return false;

        apprentice.FirstName           = firstName.Trim();
        apprentice.LastName            = lastName.Trim();
        apprentice.DateOfBirth         = dateOfBirth;
        apprentice.StartDate           = startDate;
        apprentice.CompanyId           = companyId;
        apprentice.VocationalTrainerId = vocationalTrainerId;

        _store.Save();
        return true;
    }

    public bool Delete(Guid id)
    {
        var apprentice = GetById(id);
        if (apprentice is null) return false;

        _store.Apprentices.Remove(apprentice);
        _store.Save();
        return true;
    }

    // ─── Work Journal ─────────────────────────────────────────────────────────

    public WorkJournal? AddWorkJournal(Guid apprenticeId, DateTime date, string taskDescription, double hoursWorked)
    {
        var apprentice = GetById(apprenticeId);
        if (apprentice is null) return null;

        var entry = new WorkJournal
        {
            Date            = date,
            TaskDescription = taskDescription.Trim(),
            HoursWorked     = hoursWorked,
        };

        apprentice.AddWorkJournal(entry);
        _store.Save();
        return entry;
    }

    public bool UpdateWorkJournal(Guid apprenticeId, Guid journalId, DateTime date, string taskDescription, double hoursWorked)
    {
        var apprentice = GetById(apprenticeId);
        if (apprentice is null) return false;

        var entry = apprentice.WorkJournals.FirstOrDefault(j => j.Id == journalId);
        if (entry is null) return false;

        entry.Date            = date;
        entry.TaskDescription = taskDescription.Trim();
        entry.HoursWorked     = hoursWorked;

        _store.Save();
        return true;
    }

    public bool DeleteWorkJournal(Guid apprenticeId, Guid journalId)
    {
        var apprentice = GetById(apprenticeId);
        if (apprentice is null) return false;

        bool removed = apprentice.RemoveWorkJournal(journalId);
        if (removed) _store.Save();
        return removed;
    }

    // ─── School Subjects ──────────────────────────────────────────────────────

    public SchoolSubject? AddSchoolSubject(Guid apprenticeId, string subjectName)
    {
        var apprentice = GetById(apprenticeId);
        if (apprentice is null) return null;

        var subject = new SchoolSubject { SubjectName = subjectName.Trim() };
        apprentice.AddSchoolSubject(subject);
        _store.Save();
        return subject;
    }

    public bool UpdateSchoolSubject(Guid apprenticeId, Guid subjectId, string subjectName)
    {
        var apprentice = GetById(apprenticeId);
        if (apprentice is null) return false;

        var subject = apprentice.SchoolSubjects.FirstOrDefault(s => s.Id == subjectId);
        if (subject is null) return false;

        subject.SubjectName = subjectName.Trim();
        _store.Save();
        return true;
    }

    public bool DeleteSchoolSubject(Guid apprenticeId, Guid subjectId)
    {
        var apprentice = GetById(apprenticeId);
        if (apprentice is null) return false;

        bool removed = apprentice.RemoveSchoolSubject(subjectId);
        if (removed) _store.Save();
        return removed;
    }

    // ─── Grades ───────────────────────────────────────────────────────────────

    public Grade? AddGrade(Guid apprenticeId, Guid subjectId, double score, double weight, DateTime date, string description)
    {
        var apprentice = GetById(apprenticeId);
        if (apprentice is null) return null;

        var grade = new Grade
        {
            Score       = score,
            Weight      = weight,
            Date        = date,
            Description = description.Trim(),
        };

        bool added = apprentice.AddGrade(subjectId, grade);
        if (added) _store.Save();
        return added ? grade : null;
    }

    public bool UpdateGrade(Guid apprenticeId, Guid subjectId, Guid gradeId, double score, double weight, DateTime date, string description)
    {
        var apprentice = GetById(apprenticeId);
        if (apprentice is null) return false;

        var subject = apprentice.SchoolSubjects.FirstOrDefault(s => s.Id == subjectId);
        if (subject is null) return false;

        var grade = subject.Grades.FirstOrDefault(g => g.Id == gradeId);
        if (grade is null) return false;

        grade.Score       = score;
        grade.Weight      = weight;
        grade.Date        = date;
        grade.Description = description.Trim();

        _store.Save();
        return true;
    }

    public bool DeleteGrade(Guid apprenticeId, Guid subjectId, Guid gradeId)
    {
        var apprentice = GetById(apprenticeId);
        if (apprentice is null) return false;

        bool removed = apprentice.RemoveGrade(subjectId, gradeId);
        if (removed) _store.Save();
        return removed;
    }
}
