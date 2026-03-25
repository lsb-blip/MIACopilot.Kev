namespace ApprenticeManagement.Models;

/// <summary>Represents a school subject taken by an apprentice.</summary>
public class SchoolSubject
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string SubjectName { get; set; } = string.Empty;

    /// <summary>All grades received in this subject.</summary>
    public List<Grade> Grades { get; set; } = [];

    // ─── Computed helpers ────────────────────────────────────────────────────

    /// <summary>
    /// Returns the weighted average of all grades in this subject,
    /// or <c>null</c> when no grades exist.
    /// </summary>
    public double? GetWeightedAverage()
    {
        if (Grades.Count == 0) return null;

        double totalWeightedScore = Grades.Sum(g => g.Score * g.Weight);
        double totalWeight        = Grades.Sum(g => g.Weight);

        return totalWeight == 0 ? null : totalWeightedScore / totalWeight;
    }
}
