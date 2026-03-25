namespace ApprenticeManagement.Models;

/// <summary>Represents a single grade entry for a school subject.</summary>
public class Grade
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Numeric score (0–100).</summary>
    public double Score { get; set; }

    /// <summary>Weight/multiplier applied when computing the weighted average.</summary>
    public double Weight { get; set; } = 1.0;

    /// <summary>Date the grade was received.</summary>
    public DateTime Date { get; set; } = DateTime.Today;

    /// <summary>Optional description (e.g. "Mid-term exam").</summary>
    public string Description { get; set; } = string.Empty;
}
