namespace ApprenticeManagement.Models;

/// <summary>Represents a single entry in an apprentice's work journal.</summary>
public class WorkJournal
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime Date { get; set; } = DateTime.Today;

    public string TaskDescription { get; set; } = string.Empty;

    /// <summary>Hours worked during this journal entry (must be &gt; 0).</summary>
    public double HoursWorked { get; set; }
}
