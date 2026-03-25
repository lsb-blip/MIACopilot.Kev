namespace ApprenticeManagement.Models;

/// <summary>Represents a vocational trainer responsible for one or more apprentices.</summary>
public class VocationalTrainer
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string FirstName { get; set; } = string.Empty;
    public string LastName  { get; set; } = string.Empty;

    /// <summary>Full name helper.</summary>
    public string FullName => $"{FirstName} {LastName}";

    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
