using ApprenticeManagement.Models;
using ApprenticeManagement.Services;

namespace ApprenticeManagement.UI;

/// <summary>
/// Menu for managing <see cref="Apprentice"/> entities — including their
/// work journals and school subjects/grades.
/// </summary>
public class ApprenticeMenu(
    ApprenticeService      apprenticeService,
    CompanyService         companyService,
    VocationalTrainerService trainerService)
{
    private readonly ApprenticeService       _apprenticeService = apprenticeService;
    private readonly CompanyService          _companyService    = companyService;
    private readonly VocationalTrainerService _trainerService   = trainerService;

    public void Show()
    {
        while (true)
        {
            ConsoleHelper.PrintHeader("Apprentice Management");
            Console.WriteLine("  [1]  List all apprentices");
            Console.WriteLine("  [2]  Add apprentice");
            Console.WriteLine("  [3]  Edit apprentice");
            Console.WriteLine("  [4]  Delete apprentice");
            Console.WriteLine("  [5]  Search apprentice by name");
            Console.WriteLine("  [6]  Manage work journals");
            Console.WriteLine("  [7]  Manage school subjects & grades");
            Console.WriteLine("  [8]  View grade report (GPA)");
            Console.WriteLine("  [9]  View total hours worked");
            Console.WriteLine("  [0]  Back");
            ConsoleHelper.PrintSeparator();

            int choice = ConsoleHelper.ReadInt("Choice", 0, 9);

            switch (choice)
            {
                case 1: ListApprentices();            break;
                case 2: AddApprentice();              break;
                case 3: EditApprentice();             break;
                case 4: DeleteApprentice();           break;
                case 5: SearchApprentice();           break;
                case 6: ManageWorkJournals();         break;
                case 7: ManageSchoolSubjects();       break;
                case 8: ShowGradeReport();            break;
                case 9: ShowTotalHours();             break;
                case 0: return;
            }
        }
    }

    // ─── Apprentice CRUD ─────────────────────────────────────────────────────

    private void ListApprentices()
    {
        ConsoleHelper.PrintHeader("All Apprentices");
        var apprentices = _apprenticeService.GetAll();

        if (apprentices.Count == 0)
        {
            ConsoleHelper.PrintInfo("No apprentices found.");
            ConsoleHelper.Pause();
            return;
        }

        foreach (var a in apprentices)
            PrintApprenticeSummary(a);

        ConsoleHelper.Pause();
    }

    private void AddApprentice()
    {
        ConsoleHelper.PrintHeader("Add Apprentice");

        string   firstName   = ConsoleHelper.ReadString("First Name");
        string   lastName    = ConsoleHelper.ReadString("Last Name");
        DateTime dateOfBirth = ConsoleHelper.ReadDate("Date of Birth");
        DateTime startDate   = ConsoleHelper.ReadDate("Start Date");

        Guid? companyId = PickOptionalCompany();
        Guid? trainerId = PickOptionalTrainer();

        var a = _apprenticeService.Create(firstName, lastName, dateOfBirth, startDate, companyId, trainerId);
        ConsoleHelper.PrintSuccess($"Apprentice '{a.FullName}' created (ID: {a.Id}).");
        ConsoleHelper.Pause();
    }

    private void EditApprentice()
    {
        ConsoleHelper.PrintHeader("Edit Apprentice");
        var a = SelectApprentice();
        if (a is null) return;

        string   firstName   = ConsoleHelper.ReadStringOrDefault("First Name",  a.FirstName);
        string   lastName    = ConsoleHelper.ReadStringOrDefault("Last Name",   a.LastName);
        DateTime dateOfBirth = ConsoleHelper.ReadDateOrDefault("Date of Birth", a.DateOfBirth);
        DateTime startDate   = ConsoleHelper.ReadDateOrDefault("Start Date",    a.StartDate);

        Guid? companyId = PickOptionalCompany(a.CompanyId);
        Guid? trainerId = PickOptionalTrainer(a.VocationalTrainerId);

        _apprenticeService.Update(a.Id, firstName, lastName, dateOfBirth, startDate, companyId, trainerId);
        ConsoleHelper.PrintSuccess("Apprentice updated.");
        ConsoleHelper.Pause();
    }

    private void DeleteApprentice()
    {
        ConsoleHelper.PrintHeader("Delete Apprentice");
        var a = SelectApprentice();
        if (a is null) return;

        Console.Write($"  Are you sure you want to delete '{a.FullName}'? (y/N): ");
        string confirm = Console.ReadLine() ?? string.Empty;

        if (confirm.Equals("y", StringComparison.OrdinalIgnoreCase))
        {
            _apprenticeService.Delete(a.Id);
            ConsoleHelper.PrintSuccess("Apprentice deleted.");
        }
        else
        {
            ConsoleHelper.PrintInfo("Deletion cancelled.");
        }

        ConsoleHelper.Pause();
    }

    private void SearchApprentice()
    {
        ConsoleHelper.PrintHeader("Search Apprentice by Name");
        string term = ConsoleHelper.ReadString("Search term");

        var results = _apprenticeService.SearchByName(term).ToList();

        if (results.Count == 0)
        {
            ConsoleHelper.PrintInfo($"No apprentice found matching '{term}'.");
        }
        else
        {
            ConsoleHelper.PrintSuccess($"Found {results.Count} result(s):");
            foreach (var a in results)
                PrintApprenticeSummary(a);
        }

        ConsoleHelper.Pause();
    }

    // ─── Work Journals ────────────────────────────────────────────────────────

    private void ManageWorkJournals()
    {
        var a = SelectApprentice();
        if (a is null) return;

        while (true)
        {
            ConsoleHelper.PrintHeader($"Work Journals — {a.FullName}");
            Console.WriteLine("  [1] List all entries");
            Console.WriteLine("  [2] Filter by date range");
            Console.WriteLine("  [3] Add entry");
            Console.WriteLine("  [4] Edit entry");
            Console.WriteLine("  [5] Delete entry");
            Console.WriteLine("  [0] Back");
            ConsoleHelper.PrintSeparator();

            int choice = ConsoleHelper.ReadInt("Choice", 0, 5);

            switch (choice)
            {
                case 1: ListJournals(a);        break;
                case 2: FilterJournals(a);      break;
                case 3: AddJournal(a);          break;
                case 4: EditJournal(a);         break;
                case 5: DeleteJournal(a);       break;
                case 0: return;
            }
        }
    }

    private static void ListJournals(Apprentice a, IEnumerable<WorkJournal>? entries = null)
    {
        var list = (entries ?? a.WorkJournals).ToList();

        if (list.Count == 0)
        {
            ConsoleHelper.PrintInfo("No journal entries found.");
            return;
        }

        foreach (var j in list)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"  [{j.Date:yyyy-MM-dd}]  {j.TaskDescription}");
            Console.ResetColor();
            Console.WriteLine($"    Hours:  {j.HoursWorked:F1}");
            Console.WriteLine($"    ID:     {j.Id}");
            ConsoleHelper.PrintSeparator();
        }
    }

    private static void FilterJournals(Apprentice a)
    {
        ConsoleHelper.PrintHeader($"Filter Journals — {a.FullName}");

        DateTime from = ConsoleHelper.ReadDate("From date");
        DateTime to   = ConsoleHelper.ReadDate("To date");

        if (to < from)
        {
            ConsoleHelper.PrintError("'To' date must be on or after 'From' date.");
            ConsoleHelper.Pause();
            return;
        }

        var results = a.GetJournalsByDateRange(from, to).ToList();
        ConsoleHelper.PrintSuccess($"Entries between {from:yyyy-MM-dd} and {to:yyyy-MM-dd}:");
        ListJournals(a, results);

        if (results.Count == 0) ConsoleHelper.PrintInfo("No entries in that range.");

        ConsoleHelper.Pause();
    }

    private void AddJournal(Apprentice a)
    {
        ConsoleHelper.PrintHeader("Add Journal Entry");

        DateTime date        = ConsoleHelper.ReadDate("Date");
        string   description = ConsoleHelper.ReadString("Task Description");
        double   hours       = ConsoleHelper.ReadDouble("Hours Worked", min: 0.1, max: 24.0);

        _apprenticeService.AddWorkJournal(a.Id, date, description, hours);
        ConsoleHelper.PrintSuccess("Journal entry added.");
        ConsoleHelper.Pause();
    }

    private void EditJournal(Apprentice a)
    {
        ConsoleHelper.PrintHeader("Edit Journal Entry");

        if (a.WorkJournals.Count == 0)
        {
            ConsoleHelper.PrintInfo("No journal entries to edit.");
            ConsoleHelper.Pause();
            return;
        }

        var j = SelectJournal(a);
        if (j is null) return;

        DateTime date        = ConsoleHelper.ReadDateOrDefault("Date",             j.Date);
        string   description = ConsoleHelper.ReadStringOrDefault("Task Description", j.TaskDescription);
        double   hours       = ConsoleHelper.ReadDoubleOrDefault($"Hours Worked (0.1–24.0)", j.HoursWorked, 0.1, 24.0);

        _apprenticeService.UpdateWorkJournal(a.Id, j.Id, date, description, hours);
        ConsoleHelper.PrintSuccess("Journal entry updated.");
        ConsoleHelper.Pause();
    }

    private void DeleteJournal(Apprentice a)
    {
        ConsoleHelper.PrintHeader("Delete Journal Entry");

        if (a.WorkJournals.Count == 0)
        {
            ConsoleHelper.PrintInfo("No journal entries to delete.");
            ConsoleHelper.Pause();
            return;
        }

        var j = SelectJournal(a);
        if (j is null) return;

        Console.Write($"  Delete journal entry from {j.Date:yyyy-MM-dd}? (y/N): ");
        string confirm = Console.ReadLine() ?? string.Empty;

        if (confirm.Equals("y", StringComparison.OrdinalIgnoreCase))
        {
            _apprenticeService.DeleteWorkJournal(a.Id, j.Id);
            ConsoleHelper.PrintSuccess("Journal entry deleted.");
        }
        else
        {
            ConsoleHelper.PrintInfo("Deletion cancelled.");
        }

        ConsoleHelper.Pause();
    }

    private static WorkJournal? SelectJournal(Apprentice a)
    {
        var entries = a.WorkJournals.OrderBy(j => j.Date).ToList();
        if (entries.Count == 0) return null;

        var labels = entries.Select(j => $"{j.Date:yyyy-MM-dd}  –  {j.TaskDescription}  ({j.HoursWorked:F1}h)").ToList();
        int idx    = ConsoleHelper.ChooseFromList("Select journal entry", labels);
        return entries[idx - 1];
    }

    // ─── School Subjects & Grades ────────────────────────────────────────────

    private void ManageSchoolSubjects()
    {
        var a = SelectApprentice();
        if (a is null) return;

        while (true)
        {
            ConsoleHelper.PrintHeader($"School Subjects — {a.FullName}");
            Console.WriteLine("  [1] List subjects");
            Console.WriteLine("  [2] Add subject");
            Console.WriteLine("  [3] Edit subject name");
            Console.WriteLine("  [4] Delete subject");
            Console.WriteLine("  [5] Manage grades for a subject");
            Console.WriteLine("  [0] Back");
            ConsoleHelper.PrintSeparator();

            int choice = ConsoleHelper.ReadInt("Choice", 0, 5);

            switch (choice)
            {
                case 1: ListSubjects(a);          break;
                case 2: AddSubject(a);            break;
                case 3: EditSubject(a);           break;
                case 4: DeleteSubject(a);         break;
                case 5: ManageGrades(a);          break;
                case 0: return;
            }
        }
    }

    private static void ListSubjects(Apprentice a)
    {
        ConsoleHelper.PrintHeader($"Subjects — {a.FullName}");

        if (a.SchoolSubjects.Count == 0)
        {
            ConsoleHelper.PrintInfo("No subjects found.");
            ConsoleHelper.Pause();
            return;
        }

        foreach (var s in a.SchoolSubjects)
        {
            double? avg = s.GetWeightedAverage();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"  {s.SubjectName}");
            Console.ResetColor();
            Console.WriteLine($"    Grades: {s.Grades.Count}   Avg: {(avg.HasValue ? avg.Value.ToString("F2") : "N/A")}");
            Console.WriteLine($"    ID: {s.Id}");
            ConsoleHelper.PrintSeparator();
        }

        ConsoleHelper.Pause();
    }

    private void AddSubject(Apprentice a)
    {
        ConsoleHelper.PrintHeader("Add School Subject");
        string name = ConsoleHelper.ReadString("Subject Name");

        _apprenticeService.AddSchoolSubject(a.Id, name);
        ConsoleHelper.PrintSuccess($"Subject '{name}' added.");
        ConsoleHelper.Pause();
    }

    private void EditSubject(Apprentice a)
    {
        ConsoleHelper.PrintHeader("Edit Subject Name");
        var subject = SelectSubject(a);
        if (subject is null) return;

        string name = ConsoleHelper.ReadStringOrDefault("Subject Name", subject.SubjectName);
        _apprenticeService.UpdateSchoolSubject(a.Id, subject.Id, name);
        ConsoleHelper.PrintSuccess("Subject updated.");
        ConsoleHelper.Pause();
    }

    private void DeleteSubject(Apprentice a)
    {
        ConsoleHelper.PrintHeader("Delete Subject");
        var subject = SelectSubject(a);
        if (subject is null) return;

        Console.Write($"  Delete '{subject.SubjectName}' and all its grades? (y/N): ");
        string confirm = Console.ReadLine() ?? string.Empty;

        if (confirm.Equals("y", StringComparison.OrdinalIgnoreCase))
        {
            _apprenticeService.DeleteSchoolSubject(a.Id, subject.Id);
            ConsoleHelper.PrintSuccess("Subject deleted.");
        }
        else
        {
            ConsoleHelper.PrintInfo("Deletion cancelled.");
        }

        ConsoleHelper.Pause();
    }

    private void ManageGrades(Apprentice a)
    {
        var subject = SelectSubject(a);
        if (subject is null) return;

        while (true)
        {
            ConsoleHelper.PrintHeader($"Grades — {subject.SubjectName}");
            Console.WriteLine("  [1] List grades");
            Console.WriteLine("  [2] Add grade");
            Console.WriteLine("  [3] Edit grade");
            Console.WriteLine("  [4] Delete grade");
            Console.WriteLine("  [0] Back");
            ConsoleHelper.PrintSeparator();

            int choice = ConsoleHelper.ReadInt("Choice", 0, 4);

            switch (choice)
            {
                case 1: ListGrades(subject);           break;
                case 2: AddGrade(a, subject);          break;
                case 3: EditGrade(a, subject);         break;
                case 4: DeleteGrade(a, subject);       break;
                case 0: return;
            }
        }
    }

    private static void ListGrades(SchoolSubject subject)
    {
        ConsoleHelper.PrintHeader($"Grades — {subject.SubjectName}");

        if (subject.Grades.Count == 0)
        {
            ConsoleHelper.PrintInfo("No grades found.");
            ConsoleHelper.Pause();
            return;
        }

        foreach (var g in subject.Grades.OrderByDescending(g => g.Date))
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"  Score: {g.Score:F1}   Weight: {g.Weight:F2}   Date: {g.Date:yyyy-MM-dd}");
            Console.ResetColor();
            Console.WriteLine($"    Description: {(string.IsNullOrEmpty(g.Description) ? "—" : g.Description)}");
            Console.WriteLine($"    ID: {g.Id}");
            ConsoleHelper.PrintSeparator();
        }

        double? avg = subject.GetWeightedAverage();
        ConsoleHelper.PrintInfo($"Weighted average: {(avg.HasValue ? avg.Value.ToString("F2") : "N/A")}");
        ConsoleHelper.Pause();
    }

    private void AddGrade(Apprentice a, SchoolSubject subject)
    {
        ConsoleHelper.PrintHeader($"Add Grade — {subject.SubjectName}");

        double   score       = ConsoleHelper.ReadDouble("Score (0–100)", 0, 100);
        double   weight      = ConsoleHelper.ReadDouble("Weight/Multiplier (e.g. 1.0)", 0.01, 100);
        DateTime date        = ConsoleHelper.ReadDate("Date");
        string   description = ConsoleHelper.ReadString("Description (optional)", allowEmpty: true);

        _apprenticeService.AddGrade(a.Id, subject.Id, score, weight, date, description);
        ConsoleHelper.PrintSuccess("Grade added.");
        ConsoleHelper.Pause();
    }

    private void EditGrade(Apprentice a, SchoolSubject subject)
    {
        ConsoleHelper.PrintHeader($"Edit Grade — {subject.SubjectName}");

        if (subject.Grades.Count == 0)
        {
            ConsoleHelper.PrintInfo("No grades to edit.");
            ConsoleHelper.Pause();
            return;
        }

        var grade = SelectGrade(subject);
        if (grade is null) return;

        double   score       = ConsoleHelper.ReadDoubleOrDefault("Score (0–100)", grade.Score, 0, 100);
        double   weight      = ConsoleHelper.ReadDoubleOrDefault("Weight/Multiplier", grade.Weight, 0.01, 100);
        DateTime date        = ConsoleHelper.ReadDateOrDefault("Date", grade.Date);
        string   description = ConsoleHelper.ReadStringOrDefault("Description", grade.Description);

        _apprenticeService.UpdateGrade(a.Id, subject.Id, grade.Id, score, weight, date, description);
        ConsoleHelper.PrintSuccess("Grade updated.");
        ConsoleHelper.Pause();
    }

    private void DeleteGrade(Apprentice a, SchoolSubject subject)
    {
        ConsoleHelper.PrintHeader($"Delete Grade — {subject.SubjectName}");

        if (subject.Grades.Count == 0)
        {
            ConsoleHelper.PrintInfo("No grades to delete.");
            ConsoleHelper.Pause();
            return;
        }

        var grade = SelectGrade(subject);
        if (grade is null) return;

        Console.Write($"  Delete grade (Score: {grade.Score:F1})? (y/N): ");
        string confirm = Console.ReadLine() ?? string.Empty;

        if (confirm.Equals("y", StringComparison.OrdinalIgnoreCase))
        {
            _apprenticeService.DeleteGrade(a.Id, subject.Id, grade.Id);
            ConsoleHelper.PrintSuccess("Grade deleted.");
        }
        else
        {
            ConsoleHelper.PrintInfo("Deletion cancelled.");
        }

        ConsoleHelper.Pause();
    }

    // ─── Grade Report / GPA ──────────────────────────────────────────────────

    private void ShowGradeReport()
    {
        var a = SelectApprentice();
        if (a is null) return;

        ConsoleHelper.PrintHeader($"Grade Report — {a.FullName}");

        if (a.SchoolSubjects.Count == 0)
        {
            ConsoleHelper.PrintInfo("No school subjects registered.");
            ConsoleHelper.Pause();
            return;
        }

        foreach (var s in a.SchoolSubjects)
        {
            double? avg = s.GetWeightedAverage();
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"  {s.SubjectName,-35}");
            Console.ResetColor();
            Console.WriteLine($"  Avg: {(avg.HasValue ? avg.Value.ToString("F2") : "N/A"),6}  (Grades: {s.Grades.Count})");
        }

        ConsoleHelper.PrintSeparator();
        double? gpa = a.GetOverallGpa();

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"  Overall GPA: {(gpa.HasValue ? gpa.Value.ToString("F2") : "N/A")}");
        Console.ResetColor();

        ConsoleHelper.Pause();
    }

    // ─── Total Hours ─────────────────────────────────────────────────────────

    private void ShowTotalHours()
    {
        var a = SelectApprentice();
        if (a is null) return;

        ConsoleHelper.PrintHeader($"Total Hours Worked — {a.FullName}");
        double total = a.GetTotalHoursWorked();

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"  Total hours logged: {total:F1}h  ({a.WorkJournals.Count} journal entries)");
        Console.ResetColor();

        ConsoleHelper.Pause();
    }

    // ─── Helpers ─────────────────────────────────────────────────────────────

    private void PrintApprenticeSummary(Apprentice a)
    {
        string companyName  = a.CompanyId.HasValue
            ? (_companyService.GetById(a.CompanyId.Value)?.Name ?? "Unknown")
            : "—";
        string trainerName  = a.VocationalTrainerId.HasValue
            ? (_trainerService.GetById(a.VocationalTrainerId.Value)?.FullName ?? "Unknown")
            : "—";

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"  {a.FullName}");
        Console.ResetColor();
        Console.WriteLine($"    ID:       {a.Id}");
        Console.WriteLine($"    DOB:      {a.DateOfBirth:yyyy-MM-dd}");
        Console.WriteLine($"    Start:    {a.StartDate:yyyy-MM-dd}");
        Console.WriteLine($"    Company:  {companyName}");
        Console.WriteLine($"    Trainer:  {trainerName}");
        Console.WriteLine($"    Journals: {a.WorkJournals.Count}   Subjects: {a.SchoolSubjects.Count}");
        ConsoleHelper.PrintSeparator();
    }

    /// <summary>Lets the user pick an apprentice from a numbered list.</summary>
    public Apprentice? SelectApprentice()
    {
        var apprentices = _apprenticeService.GetAll();

        if (apprentices.Count == 0)
        {
            ConsoleHelper.PrintInfo("No apprentices available.");
            ConsoleHelper.Pause();
            return null;
        }

        var labels = apprentices.Select(a => a.FullName).ToList();
        int idx    = ConsoleHelper.ChooseFromList("Select apprentice", labels);
        return apprentices[idx - 1];
    }

    private static SchoolSubject? SelectSubject(Apprentice a)
    {
        if (a.SchoolSubjects.Count == 0)
        {
            ConsoleHelper.PrintInfo("No subjects available.");
            ConsoleHelper.Pause();
            return null;
        }

        var labels = a.SchoolSubjects.Select(s => s.SubjectName).ToList();
        int idx    = ConsoleHelper.ChooseFromList("Select subject", labels);
        return a.SchoolSubjects[idx - 1];
    }

    private static Grade? SelectGrade(SchoolSubject subject)
    {
        if (subject.Grades.Count == 0) return null;

        var grades = subject.Grades.OrderByDescending(g => g.Date).ToList();
        var labels = grades.Select(g =>
            $"{g.Date:yyyy-MM-dd}  Score: {g.Score:F1}  Weight: {g.Weight:F2}  {g.Description}").ToList();

        int idx = ConsoleHelper.ChooseFromList("Select grade", labels);
        return grades[idx - 1];
    }

    private Guid? PickOptionalCompany(Guid? current = null)
    {
        var companies = _companyService.GetAll();
        if (companies.Count == 0) return current;

        ConsoleHelper.PrintLine();
        ConsoleHelper.PrintInfo("Select a company (or 0 to leave unassigned):");

        for (int i = 0; i < companies.Count; i++)
        {
            string marker = companies[i].Id == current ? " ◄ current" : string.Empty;
            Console.WriteLine($"    [{i + 1}] {companies[i].Name}{marker}");
        }
        Console.WriteLine("    [0] None");

        int choice = ConsoleHelper.ReadInt("Choice", 0, companies.Count);
        return choice == 0 ? null : companies[choice - 1].Id;
    }

    private Guid? PickOptionalTrainer(Guid? current = null)
    {
        var trainers = _trainerService.GetAll();
        if (trainers.Count == 0) return current;

        ConsoleHelper.PrintLine();
        ConsoleHelper.PrintInfo("Select a vocational trainer (or 0 to leave unassigned):");

        for (int i = 0; i < trainers.Count; i++)
        {
            string marker = trainers[i].Id == current ? " ◄ current" : string.Empty;
            Console.WriteLine($"    [{i + 1}] {trainers[i].FullName}{marker}");
        }
        Console.WriteLine("    [0] None");

        int choice = ConsoleHelper.ReadInt("Choice", 0, trainers.Count);
        return choice == 0 ? null : trainers[choice - 1].Id;
    }
}
