namespace ApprenticeManagement.Models;

/// <summary>Represents a company that employs apprentices.</summary>
public class Company
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name    { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Phone   { get; set; } = string.Empty;
    public string Email   { get; set; } = string.Empty;
}
